using Newtonsoft.Json;
using O2O_Server.Common;
using O2O_Server.Dao;
using Senparc.Weixin.WxOpen.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O2O_Server.Buss
{
    public class OrderBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.OrderApi;
        }

        public object Do_GetOrderList(object param)
        {
            OrderListParam orderListParam = JsonConvert.DeserializeObject<OrderListParam>(param.ToString());
            if (orderListParam == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }

             SessionBag sessionBag = SessionContainer.GetSession(orderListParam.token);
            var openId = sessionBag.OpenId;

            OrderDao orderDao = new OrderDao();
            OrderListResult orderListResult = orderDao.getOrderList(openId, orderListParam.shop);
            
            return orderListResult;
        }
    }

    public class OrderListParam
    {
        public string token;
        public string shop;
    }

    public class OrderListResult
    {
        public List<OrderListItem> orderList = new List<OrderListItem>();
    }

    public class OrderListItem
    {
        public string billId;//订单号
        public string imgUrl;//订单商品图片
        public string createTime;//订单创建时间
        public string product;//主要商品名
    }
}
