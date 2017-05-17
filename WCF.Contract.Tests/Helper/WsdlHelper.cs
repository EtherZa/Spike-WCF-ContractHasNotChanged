namespace WCF.Contract.Tests.Helper
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel.Description;
    using System.Xml;
    using System.Xml.Linq;

    using Microsoft.XmlDiffPatch;

    using ServiceDescription = System.Web.Services.Description.ServiceDescription;

    public static class WsdlHelper
    {
        public static bool CompareXml(XmlDocument actualXml, XmlDocument expectedXml, out XmlDocument diffGramXml)
        {
            Func<XmlDocument, string> prettyPrint = (xml) =>
                {
                    using (var nodeReader = new XmlNodeReader(xml))
                    {
                        nodeReader.MoveToContent();
                        return XDocument.Load(nodeReader).ToString();
                    }
                };

            using (XmlReader actualReader = new XmlNodeReader(actualXml))
            {
                using (XmlReader expectedReader = new XmlNodeReader(expectedXml))
                {
                    var xmlDiff = new XmlDiff(XmlDiffOptions.IgnoreNamespaces | XmlDiffOptions.IgnoreXmlDecl | XmlDiffOptions.IgnoreChildOrder);

                    diffGramXml = new XmlDocument();
                    using (var diffGramWriter = diffGramXml.CreateNavigator().AppendChild())
                    {
                        var identical = xmlDiff.Compare(actualReader, expectedReader, diffGramWriter);
                        diffGramWriter.Close();

                        if (!identical)
                        {
                            Console.WriteLine("Diff gram:");
                            Console.WriteLine(prettyPrint(diffGramXml));
                            Console.WriteLine();

                            Console.WriteLine("Expected:");
                            Console.WriteLine(prettyPrint(expectedXml));
                            Console.WriteLine();

                            Console.WriteLine("Actual:");
                            Console.WriteLine(prettyPrint(actualXml));
                            Console.WriteLine();
                        }

                        return identical;
                    }
                }
            }
        }

        public static XmlDocument GetSingleWsdl<TContract>(out string[] errors)
        {
            var description = ContractDescription.GetContract(typeof(TContract));
            var wsdlExporter = new WsdlExporter();

            wsdlExporter.ExportContract(description);
            errors = wsdlExporter.Errors.Select(e => e.Message).ToArray();

            var wsdlMetadata = wsdlExporter.GetGeneratedMetadata();

            var asy = Assembly.GetAssembly(typeof(WsdlExporter));
            var wsdlHelperType = asy.GetType("System.ServiceModel.Description.WsdlHelper", true);
            var getSingleWsdl = wsdlHelperType.GetMethod("GetSingleWsdl", BindingFlags.Public | BindingFlags.Static);

            var parameters = new object[]
                                 {
                                     wsdlMetadata
                                 };
            var serviceDescription = (ServiceDescription)getSingleWsdl.Invoke(null, parameters);

            var doc = new XmlDocument();
            var writer = doc.CreateNavigator().AppendChild();
            try
            {
                serviceDescription.Write(writer);

                return doc;
            }
            finally
            {
                writer.Close();
            }
        }
    }
}