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
        public object Do_GetOrder(object param)
        {
            OrderListParam orderListParam = JsonConvert.DeserializeObject<OrderListParam>(param.ToString());
            if (orderListParam == null||orderListParam.orderId==""|| orderListParam.orderId ==null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            var openId = "oXgq05HXGBIABxsW8i-toDUsikrc";
            //SessionBag sessionBag = SessionContainer.GetSession(orderListParam.token);
            //var openId = sessionBag.OpenId;

            OrderDao orderDao = new OrderDao();
            OrderListItem orderListItem = orderDao.getOrder(openId, orderListParam.orderId);
            return orderListItem;
        }
    }

    public class OrderListParam
    {
        public string token;
        public string shop;
        public string orderId;
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
        public string payTime;//支付时间
        public string product;//主要商品名
        public string status;//状态
        public string total;//总价
        public string waybillno;//运单号
        public List<OrderGoodsListItem> orderGoodsList = new List<OrderGoodsListItem>();
    }
    public class OrderGoodsListItem
    {
        public string barCode;//条码
        public string slt;//商品图片
        public string skuUnitPrice;//单价
        public string quantity;//数量
        public string skuBillName;//商品名
    }
}
