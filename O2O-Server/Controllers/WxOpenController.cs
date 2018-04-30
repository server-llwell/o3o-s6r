using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using O2O_Server.Common;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.WxOpen.Entities;
using Senparc.Weixin.WxOpen.Helpers;
using Newtonsoft.Json;

namespace O2O_Server.Controllers
{
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    public class WxOpenController : Controller
    {
        [HttpPost]
        public ActionResult Test([FromBody]object obj)
        {
            var headers = Request.Headers;
            string token = "";
            if (headers.Keys.Contains("token"))
            {
                token = headers["token"].First();
            }
            WxLogin s = JsonConvert.DeserializeObject<WxLogin>(obj.ToString());
            return Json(new { success = true, msg = token });
        }

        [HttpPost]
        public ActionResult OnLogin([FromBody]WxLogin wxLogin)
        {
            var jsonResult = SnsApi.JsCode2Json(Global.APPID, Global.APPSECRET, wxLogin.code);
            if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);

                //注意：生产环境下SessionKey属于敏感信息，不能进行传输！
                return Json(new { success = true, msg = "OK", sessionId = sessionBag.Key });
            }
            else
            {
                return Json(new { success = false, msg = jsonResult.errmsg });
            }
        }

        [HttpPost]
        public ActionResult CheckWxOpenSignature([FromBody]CheckSignature checkSignature)
        {
            try
            {
                var checkSuccess = EncryptHelper.CheckSignature(checkSignature.sessionId, checkSignature.rawData, checkSignature.signature);
                return Json(new { success = checkSuccess, msg = checkSuccess ? "校验成功":"校验失败" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DecodeEncryptedData([FromBody]DecodeEncryptedData decodeEncryptedData)
        {
            DecodeEntityBase decodedEntity = null;
            switch (decodeEncryptedData.type.ToUpper())
            {
                case "USERINFO"://wx.getUserInfo()
                    decodedEntity = EncryptHelper.DecodeUserInfoBySessionId(
                        decodeEncryptedData.sessionId,
                        decodeEncryptedData.encryptedData, decodeEncryptedData.iv);
                    break;
                default:
                    break;
            }
            //检验水印
            var checkWartmark = false;
            if (decodedEntity != null)
            {
                checkWartmark = decodedEntity.CheckWatermark(Global.APPID);
            }
            //注意：此处仅为演示，敏感信息请勿传递到客户端！
            return Json(new
            {
                success = checkWartmark,
                msg = string.Format("水印验证：{0}",
                checkWartmark ? "通过" : "不通过")
            });
        }
    }
}
