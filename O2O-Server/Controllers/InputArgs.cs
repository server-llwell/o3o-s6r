using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2O_Server.Controllers
{
    /// <summary>
    /// 微信小程序授权登录
    /// </summary>
    public class WxLogin
    {
        public string code;
    }

    public class CheckSignature
    {
        public string sessionId;
        public string rawData;
        public string signature;
    }

    public class DecodeEncryptedData
    {
        public string type;
        public string sessionId;
        public string encryptedData;
        public string iv;
    }
}
