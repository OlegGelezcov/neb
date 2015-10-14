using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{

    public static class ACTION_RESULT
    {
        public const string RESULT = "result";
        public const string SUCCESS = "success";
        public const string FAIL = "fail";
        public const string MESSAGE = "message";
        public const string RETURN = "return";

        public static string Status(LogicErrorCode errorCode) {
            if (errorCode == LogicErrorCode.OK) {
                return SUCCESS;
            } else {
                return FAIL;
            }
        }
    }
}
