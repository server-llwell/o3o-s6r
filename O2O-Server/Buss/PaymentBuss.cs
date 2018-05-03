using Newtonsoft.Json;
using O2O_Server.Common;
using O2O_Server.Dao;
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
                var billId = this.createBill(openId,paymentParam);
                var totalPrice = this.getBillPrice(paymentParam);
                if (totalPrice==0)
                {
                    throw new ApiException(CodeMessage.PaymentTotalPriceZero, "PaymentTotalPriceZero");
                }
                var timeStamp = TenPayV3Util.GetTimestamp();
                var nonceStr = TenPayV3Util.GetNoncestr();
                var product = paymentParam.product;
                var xmlDataInfo = 
                    new TenPayV3UnifiedorderRequestData(
                        tenPayV3Info.AppId, 
                        tenPayV3Info.MchId,
                        product,
                        billId,
                        totalPrice, 
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
            catch(Exception ex)
            {
                throw new ApiException(CodeMessage.PaymentError, "PaymentError");
            }
        }

        private string createBill(string openId,PaymentParam paymentParam)
        {
            PaymentDao pDao = new PaymentDao();

            string pre = DateTime.Now.ToString("yyyyMMddHHmm");
            string billId = pre + "XC" + TenPayV3Util.BuildRandomStr(4);
            if (pDao.saveOrder(openId,billId, paymentParam))
            {
                return billId;
            }
            else
            {
                throw new ApiException(CodeMessage.InitOrderError, "InitOrderError");
            }
        }

        private int getBillPrice(PaymentParam paymentParam)
        {
            int totalPrice = 1;
        #if !DEBUG
            //实际计算具体价格
            PaymentDao pDao = new PaymentDao();
            totalPrice = pDao.getOrderTotalPrice(paymentParam);

        #endif
            return totalPrice;
        }
       
    }

    public class PaymentParam
    {
        public string token; //
        public string goodsId;//商品id
        public string inputAddress;//地址
        public string inputIdCard;//身份证
        public string inputName;//姓名对应身份证
        public string inputNum;//数量
        public string inputPerson;//姓名
        public string inputPhone;//电话
        public string radio;//1是取货2是邮寄
        public string shop;//店铺id
        public string product;//商品名称
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
