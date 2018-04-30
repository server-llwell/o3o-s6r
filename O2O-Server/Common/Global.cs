using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2O_Server.Common
{
    public class Global
    {
        public static string APPID
        {
            get
            {
                var appId = System.Environment.GetEnvironmentVariable("WxAppId", EnvironmentVariableTarget.User);
                return appId;
            }
        }

        public static string APPSECRET
        {
            get
            {
                var appSecret = System.Environment.GetEnvironmentVariable("WxAppSecret", EnvironmentVariableTarget.User);
                return appSecret;
            }
        }
    }
}
