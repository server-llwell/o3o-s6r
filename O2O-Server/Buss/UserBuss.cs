using Newtonsoft.Json;
using O2O_Server.Common;
using O2O_Server.Dao;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.WxOpen.Entities;
using Senparc.Weixin.WxOpen.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2O_Server.Buss
{
    public class UserBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.UserApi;
        }

        public object Do_Login(object param)
        {
            LoginParam loginParam = JsonConvert.DeserializeObject<LoginParam>(param.ToString());
            if(loginParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var jsonResult = SnsApi.JsCode2Json(Global.APPID, Global.APPSECRET, loginParam.code);
            if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                var manager = Senparc.Weixin.Cache.Redis.RedisManager.Manager;
                
                AccessTokenContainer.Register(Global.APPID, Global.APPSECRET);
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);
                return new { sessionId = sessionBag.Key };
            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, jsonResult.errmsg);
            }
        }

        public object Do_CheckSignature(object param)
        {
            CheckSignatureParam checkSignatureParam = JsonConvert.DeserializeObject<CheckSignatureParam>(param.ToString());
            if (checkSignatureParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            var checkSuccess = EncryptHelper.CheckSignature(checkSignatureParam.token, checkSignatureParam.rawData, checkSignatureParam.signature);
            if (checkSuccess)
            {
                return new { check = checkSuccess };
            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, "校验失败");
            }
        }

        public object Do_DecodeEncryptedData(object param)
        {
            DecodeEncryptedDataParam decodeEncryptedDataParam = JsonConvert.DeserializeObject<DecodeEncryptedDataParam>(param.ToString());
            if (decodeEncryptedDataParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

            DecodeEntityBase decodedEntity = null;
            switch (decodeEncryptedDataParam.type.ToUpper())
            {
                case "USERINFO"://wx.getUserInfo()
                    decodedEntity = EncryptHelper.DecodeUserInfoBySessionId(
                        decodeEncryptedDataParam.token,
                        decodeEncryptedDataParam.encryptedData, decodeEncryptedDataParam.iv);
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

            if (checkWartmark)
            {
                return new { check = checkWartmark };
            }
            else
            {
                throw new ApiException(CodeMessage.SenparcCode, "校验失败");
            }
        }
        public object Do_GetShopName(object param)
        {
            ShopParam shopParam = JsonConvert.DeserializeObject<ShopParam>(param.ToString());
            if (shopParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            UserDao userDao = new UserDao();
            return userDao.getShopName(shopParam.shop);
        }
    }

    public class LoginParam
    {
        public string code;
    }

    public class CheckSignatureParam
    {
        public string token;
        public string rawData;
        public string signature;
    }

    public class DecodeEncryptedDataParam
    {
        public string token;
        public string type;
        public string encryptedData;
        public string iv;
    }

    public class ShopParam
    {
        public string shop;
    }
}
