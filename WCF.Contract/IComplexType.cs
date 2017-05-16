namespace WCF.Contract
{
    using System.Collections.Generic;

    public interface IComplexType
    {
        int Int { get; }

        string String { get; }

        IDictionary<string, int> Dictionary { get; }
    }
}