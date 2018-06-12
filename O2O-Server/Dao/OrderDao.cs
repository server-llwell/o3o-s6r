using Com.ACBC.Framework.Database;
using O2O_Server.Buss;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
namespace O2O_Server.Dao
{
    public class OrderDao
    {

        public OrderDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
        public OrderListResult getOrderList(string openid, string shop)
        {
            OrderListResult orderListResult = new OrderListResult();
            string sql = "select l.merchantOrderId,l.tradeTime,l.status,l.tradeAmount from t_order_list l "
                       + " where l.sendapi = 'XXC' and l.customerCode = '" + openid + "' and l.`status` != '未支付' and l.purchaserId = '" + shop + "' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string slt = "";
                    string pro = "";
                    string sql1 = "select barCode,max(skuBillName) goodsname,max(slt) slt " +
                                 "from t_order_goods " +
                                 "where merchantOrderId = '" + dt.Rows[i]["merchantOrderId"].ToString() + "' " +
                                 "group by barCode";
                    DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
                    if (dt1.Rows.Count > 0)
                    {
                        slt = dt1.Rows[0]["slt"].ToString();
                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {
                            pro += dt1.Rows[j]["goodsname"].ToString() + ",";
                        }
                    }
                    OrderListItem orderListItem = new OrderListItem();
                    orderListItem.billId = dt.Rows[i]["merchantOrderId"].ToString();
                    orderListItem.imgUrl = slt;
                    orderListItem.createTime = dt.Rows[i]["tradeTime"].ToString();
                    orderListItem.product = pro.Substring(0, pro.Length - 1);
                    orderListItem.total = dt.Rows[i]["tradeAmount"].ToString();
                    string status = dt.Rows[i]["status"].ToString();
                    if (status == "新订单" || status == "已导出")
                    {
                        status = "等待发货";
                    }
                    else if (status == "已回传")
                    {
                        status = "已完成";
                    }
                    else if (status == "已录运单号")
                    {
                        status = "已发货";
                    }
                    orderListItem.status = status;
                    orderListResult.orderList.Add(orderListItem);
                }
                return orderListResult;
            }
            else
            {
                return null;
            }
        }


        public OrderListItem getOrder(string openid, string orderId)
        {
            OrderListItem orderListItem = new OrderListItem();
            string sql = "select l.merchantOrderId,l.tradeTime,l.status,l.tradeAmount,l.waybillno from t_order_list l "
                       + " where l.merchantOrderId = '" + orderId + "' and l.customerCode = '" + openid + "' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string slt = "";
                string pro = "";
                string sql1 = "select barCode,skuBillName,slt,quantity,skuUnitPrice " +
                             "from t_order_goods " +
                             "where merchantOrderId = '" + orderId + "' " +
                             "group by barCode";
                DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    List<OrderGoodsListItem> orderGoodsList = new List<OrderGoodsListItem>();
                    for (int j = 0; j < dt1.Rows.Count; j++)
                    {
                        OrderGoodsListItem orderGoodsListItem = new OrderGoodsListItem();
                        orderGoodsListItem.barCode = dt1.Rows[j]["barCode"].ToString();
                        orderGoodsListItem.slt = dt1.Rows[j]["slt"].ToString();
                        orderGoodsListItem.skuUnitPrice = dt1.Rows[j]["skuUnitPrice"].ToString();
                        orderGoodsListItem.quantity = dt1.Rows[j]["quantity"].ToString();
                        orderGoodsListItem.skuBillName = dt1.Rows[j]["skuBillName"].ToString();
                        orderGoodsList.Add(orderGoodsListItem);
                    }
                    orderListItem.orderGoodsList = orderGoodsList;
                }
                orderListItem.billId = dt.Rows[0]["merchantOrderId"].ToString();
                orderListItem.imgUrl = slt;
                orderListItem.createTime = dt.Rows[0]["tradeTime"].ToString();
                orderListItem.waybillno = dt.Rows[0]["waybillno"].ToString();
                //orderListItem.product = pro.Substring(0, pro.Length - 1);
                orderListItem.total = dt.Rows[0]["tradeAmount"].ToString();
                string status = dt.Rows[0]["status"].ToString();
                if (status == "新订单" || status == "已导出")
                {
                    status = "等待发货";
                }
                else if (status == "已回传")
                {
                    status = "已完成";
                }
                else if (status == "已录运单号")
                {
                    status = "已发货";
                }
                orderListItem.status = status;
                return orderListItem;
            }
            else
            {
                return null;
            }
        }
    }
}
