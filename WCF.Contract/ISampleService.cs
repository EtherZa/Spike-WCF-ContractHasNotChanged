namespace WCF.Contract
{
    using System.ServiceModel;

    [ServiceContract(Namespace = "WCF.Contract")]
    public interface ISampleService
    {
        [OperationContract]
        void DoWork();

        [OperationContract(Name = "AddTwoNumbers")]
        int Add(int x, int y);

        [OperationContract]
        IComplexType GetComplexType();

        [OperationContract]
        void SetComplexType(IComplexType complexType);
    }
}