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
                //var appId = System.Environment.GetEnvironmentVariable("WxAppId", EnvironmentVariableTarget.User);
                var appId = System.Environment.GetEnvironmentVariable("WxAppId");
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
                //var appSecret = System.Environment.GetEnvironmentVariable("WxAppSecret", EnvironmentVariableTarget.User);
                var appSecret = System.Environment.GetEnvironmentVariable("WxAppSecret");
                return appSecret;
            }
        }


        /// <summary>
        /// 微信支付MchId
        /// </summary>
        public static string MCHID
        {
            get
            {
                //var mchId = System.Environment.GetEnvironmentVariable("WxMchId", EnvironmentVariableTarget.User);
                var mchId = System.Environment.GetEnvironmentVariable("WxMchId");
                return mchId;
            }
        }


        /// <summary>
        /// 微信支付Key
        /// </summary>
        public static string PaymentKey
        {
            get
            {
                //var paymentKey = System.Environment.GetEnvironmentVariable("WxPaymentKey", EnvironmentVariableTarget.User);
                var paymentKey = System.Environment.GetEnvironmentVariable("WxPaymentKey");
                return paymentKey;
            }
        }

        /// <summary>
        /// 微信支付回调地址
        /// </summary>
        public static string CallBackUrl
        {
            get
            {
                var callBackUrl = "https://wxapp.llwell.net/api/O2O/PaymentCallBack";
                return callBackUrl;
            }
        }
    }
}
