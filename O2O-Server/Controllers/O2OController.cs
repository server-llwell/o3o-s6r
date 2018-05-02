using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using O2O_Server.Buss;
using O2O_Server.Common;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.TenPayLibV3;

namespace O2O_Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class O2OController : Controller
    {
        /// <summary>
        /// 微信用户类API
        /// </summary>
        /// <param name="userApi"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Users([FromBody]UserApi userApi)
        {
            if (userApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(ApiType.UserApi,
                                                userApi.token,
                                                userApi.method, 
                                                userApi.param));
        }

        /// <summary>
        /// 商品信息类API
        /// </summary>
        /// <param name="goodsApi"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Goods([FromBody]GoodsApi goodsApi)
        {
            if (goodsApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(ApiType.GoodsApi,
                                                goodsApi.token,
                                                goodsApi.method,
                                                goodsApi.param));
        }

        /// <summary>
        /// 订单信息类API
        /// </summary>
        /// <param name="orderApi"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Order([FromBody]OrderApi orderApi)
        {
            if (orderApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(ApiType.OrderApi,
                                                orderApi.token,
                                                orderApi.method,
                                                orderApi.param));
        }

        /// <summary>
        /// 支付操作类API
        /// </summary>
        /// <param name="paymentApi"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Payment([FromBody]PaymentApi paymentApi)
        {
            if (paymentApi == null)
                return Json(new ResultsJson(new Message(CodeMessage.PostNull, "PostNull"), null));
            return Json(Global.BUSS.BussResults(ApiType.PaymentApi,
                                                paymentApi.token,
                                                paymentApi.method,
                                                paymentApi.param));
        }

        /// <summary>
        /// 支付操作类API回调地址
        /// </summary>
        /// <param name="paymentApi"></param>
        /// <returns></returns>
        [HttpPost]
        public XmlResult PaymentCallBack()
        {
            ResponseHandler resHandler = new ResponseHandler(HttpContext);

            PaymentCallBackBuss paymentCallBackBuss = new PaymentCallBackBuss();
            string result = paymentCallBackBuss.GetPaymentResult(resHandler);
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(result);
            return this.Xml(xdoc);

        }
    }
    public static class ControllerExtension
    {
        public static XmlResult Xml(this O2OController request, object obj) { return Xml(obj, null, null, XmlRequestBehavior.DenyGet); }
        public static XmlResult Xml(this O2OController request, object obj, XmlRequestBehavior behavior) { return Xml(obj, null, null, behavior); }
        public static XmlResult Xml(this O2OController request, object obj, Encoding contentEncoding, XmlRequestBehavior behavior) { return Xml(obj, null, contentEncoding, behavior); }
        public static XmlResult Xml(this O2OController request, object obj, string contentType, Encoding contentEncoding, XmlRequestBehavior behavior) { return Xml(obj, contentType, contentEncoding, behavior); }

        internal static XmlResult Xml(object data, string contentType, Encoding contentEncoding, XmlRequestBehavior behavior) { return new XmlResult() { ContentEncoding = contentEncoding, ContentType = contentType, Data = data, XmlRequestBehavior = behavior }; }
    }
}
