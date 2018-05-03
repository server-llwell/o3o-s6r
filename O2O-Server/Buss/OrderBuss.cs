using Newtonsoft.Json;
using O2O_Server.Common;
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

            OrderListItem orderListItem = new OrderListItem();
            orderListItem.billId = "327710274213934286";
            orderListItem.imgUrl = "http://ecc-product.oss-cn-beijing.aliyuncs.com/xcx/首页-扫码下单色块@3x.png";
            orderListItem.createTime = "2018.4.23 23:05";
            orderListItem.product = "资生堂男士滋润乳100ml 补水保湿";
            OrderListResult orderListResult = new OrderListResult();
            orderListResult.orderList.Add(orderListItem);
            orderListResult.orderList.Add(orderListItem);
            orderListResult.orderList.Add(orderListItem);
            orderListResult.orderList.Add(orderListItem);
            orderListResult.orderList.Add(orderListItem);
            orderListResult.orderList.Add(orderListItem);
            orderListResult.orderList.Add(orderListItem);
            orderListResult.orderList.Add(orderListItem);
            orderListResult.orderList.Add(orderListItem);
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
        public string billId;
        public string imgUrl;
        public string createTime;
        public string product;
    }
}
