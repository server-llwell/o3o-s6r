using O2O_Server.Common;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2O_Server.Buss
{
    public class PaymentCallBackBuss
    {
        private TenPayV3Info tenPayV3Info;

        public PaymentCallBackBuss()
        {
            tenPayV3Info = new TenPayV3Info(
                Global.APPID,
                Global.APPSECRET,
                Global.MCHID,
                Global.PaymentKey,
                Global.CallBackUrl);
        }

        public string GetPaymentResult(ResponseHandler resHandler)
        {
            try
            {
                string return_code = resHandler.GetParameter("return_code");
                string return_msg = resHandler.GetParameter("return_msg");
                string openid = resHandler.GetParameter("openid");
                string total_fee = resHandler.GetParameter("total_fee");
                string time_end = resHandler.GetParameter("time_end");

                Console.WriteLine(return_code);
                Console.WriteLine(openid);
                Console.WriteLine(total_fee);
                Console.WriteLine(time_end);
                Console.WriteLine("------------------------------------------");
                string res = null;

                resHandler.SetKey(tenPayV3Info.Key);
                //验证请求是否从微信发过来（安全）
                if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")
                {
                    res = "success";//正确的订单处理
                                    //直到这里，才能认为交易真正成功了，可以进行数据库操作，但是别忘了返回规定格式的消息！

                    /* 这里可以进行订单处理的逻辑 */

                }
                else
                {
                    res = "wrong";//错误的订单处理
                    return_code = "FAIL";
                    return_msg = "签名失败";
                }
                return string.Format(@"<xml><return_code><![CDATA[{0}]]></return_code><return_msg><![CDATA[{1}]]></return_msg></xml>", return_code, return_msg);

            }
            catch
            {
                return "";
            }
        }
    }

    
}
