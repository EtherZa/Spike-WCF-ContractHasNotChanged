namespace WCF.Contract.Tests
{
    using System;
    using System.Xml;

    using NUnit.Framework;

    using WCF.Contract.Tests.Helper;
    using WCF.Contract.Tests.Properties;

    // ReSharper disable once InconsistentNaming
    public class ISampleServiceTests
    {
        [Test]
        [Category("WCF Service Contract")]
        public void ContractHasNotChanged()
        {
            var expectedXml = new XmlDocument();
            expectedXml.LoadXml(Wsdl.ISampleService);

            string[] errors;
            var actualXml = WsdlHelper.GetSingleWsdl<ISampleService>(out errors);
            if (errors.Length > 0)
            {
                Console.WriteLine(@"Warning(s) were generated while creating the WSDL.");
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
            }

            XmlDocument diffGramXml;
            Assert.True(WsdlHelper.CompareXml(actualXml, expectedXml, out diffGramXml), "WSDL has changed - contract broken");
        }
    }
}