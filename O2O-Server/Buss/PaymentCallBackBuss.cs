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
                string out_trade_no = resHandler.GetParameter("out_trade_no");
                string transaction_id = resHandler.GetParameter("transaction_id");

                Console.WriteLine();
                Console.WriteLine("return_code:" + return_code);
                Console.WriteLine("out_trade_no:" + out_trade_no);
                Console.WriteLine("openId:" + openid);
                Console.WriteLine("total_fee:" + total_fee);
                Console.WriteLine("time_end:" + time_end);
                Console.WriteLine("transaction_id:" + transaction_id);
                Console.WriteLine("------------------------------------------");

                resHandler.SetKey(tenPayV3Info.Key);
                //验证请求是否从微信发过来（安全）
                if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")
                {
                    /* 这里可以进行订单处理的逻辑 */
                    // transaction_id:微信支付单号
                    // out_trade_no:商城实际订单号
                    // openId:用户信息
                    // total_fee:实际支付价格

                }
                else
                {
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
