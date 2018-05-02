using Newtonsoft.Json;
using O2O_Server.Common;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.TenPayLibV3;
using Senparc.Weixin.WxOpen.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2O_Server.Buss
{
    public class PaymentBuss : IBuss
    {
        private TenPayV3Info tenPayV3Info;

        public PaymentBuss()
        {
            tenPayV3Info = new TenPayV3Info(
                Global.APPID, 
                Global.APPSECRET, 
                Global.MCHID, 
                Global.PaymentKey,
                Global.CallBackUrl);
        }

        public ApiType GetApiType()
        {
            return ApiType.PaymentApi;
        }

        public object Do_Payment(object param)
        {
            PaymentParam paymentParam = JsonConvert.DeserializeObject<PaymentParam>(param.ToString());
            if (paymentParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            try
            {
                SessionBag sessionBag =SessionContainer.GetSession(paymentParam.token);
                var openId = sessionBag.OpenId;

                var billId = string.Format("{0}{1}{2}", tenPayV3Info.MchId/*10位*/, DateTime.Now.ToString("yyyyMMddHHmmss"),
                            TenPayV3Util.BuildRandomStr(6));

                var timeStamp = TenPayV3Util.GetTimestamp();
                var nonceStr = TenPayV3Util.GetNoncestr();
                var product = "流连优选-测试商品";
                var xmlDataInfo = 
                    new TenPayV3UnifiedorderRequestData(
                        tenPayV3Info.AppId, 
                        tenPayV3Info.MchId,
                        product,
                        billId, 
                        1, 
                        "127.0.0.1", 
                        tenPayV3Info.TenPayV3Notify, 
                        TenPayV3Type.JSAPI, 
                        openId, 
                        tenPayV3Info.Key, 
                        nonceStr);

                var result = TenPayV3.Html5Order(xmlDataInfo);
                var package = string.Format("prepay_id={0}", result.prepay_id);
                var paySign = TenPayV3.GetJsPaySign(tenPayV3Info.AppId, timeStamp, nonceStr, package, tenPayV3Info.Key);
                
                PaymentResults paymentResults = new PaymentResults();
                paymentResults.appId = tenPayV3Info.AppId;
                paymentResults.nonceStr = nonceStr;
                paymentResults.package = package;
                paymentResults.paySign = paySign;
                paymentResults.timeStamp = timeStamp;
                paymentResults.product = product;

                return paymentResults;
            }
            catch
            {
                throw new ApiException(CodeMessage.PaymentError, "PaymentError");
            }
        }

        
    }

    public class PaymentParam
    {
        public string token;
    }

    public class PaymentResults
    {
        public string product;
        public string appId;
        public string timeStamp;
        public string nonceStr;
        public string package;
        public string paySign;

    }
}
