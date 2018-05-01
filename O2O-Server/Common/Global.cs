using O2O_Server.Buss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2O_Server.Common
{
    public class Global
    {
        /// <summary>
        /// 基础业务处理类对象
        /// </summary>
        public static BaseBuss BUSS = new BaseBuss();

        /// <summary>
        /// 小程序APPID
        /// </summary>
        public static string APPID
        {
            get
            {
                var appId = System.Environment.GetEnvironmentVariable("WxAppId", EnvironmentVariableTarget.User);
                return appId;
            }
        }

        /// <summary>
        /// 小程序APPSECRET
        /// </summary>
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
