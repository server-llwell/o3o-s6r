using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2O_Server.Common
{
    /// <summary>
    /// API类型分组
    /// </summary>
    public enum ApiType
    {
        UserApi,
        GoodsApi,
        OrderApi,
        PaymentApi,
    }

    /// <summary>
    /// 微信用户类API
    /// </summary>
    public class UserApi
    {
        public string method;
        public string token;
        public object param;
    }

    /// <summary>
    /// 商品信息类API
    /// </summary>
    public class GoodsApi
    {
        public string method;
        public string token;
        public object param;
    }

    /// <summary>
    /// 订单信息类API
    /// </summary>
    public class OrderApi
    {
        public string method;
        public string token;
        public object param;
    }

    /// <summary>
    /// 支付操作类API
    /// </summary>
    public class PaymentApi
    {
        public string method;
        public string token;
        public object param;
    }
}
