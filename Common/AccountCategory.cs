using System;
using System.Collections.Generic;

namespace Common
{
    public class AccountCategory
    {
        public const string ACCOUNT_RECEIVABLE = "AR";
        public const string ACCOUNT_PAYABLE = "AP";

        public static List<string> ProcessFlowCategories = new List<string>
        {
            ACCOUNT_RECEIVABLE,
            ACCOUNT_PAYABLE,
        };
    }
}
