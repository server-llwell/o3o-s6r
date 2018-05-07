using Com.Portsoft.Framework.Database;
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
            if (DatabaseOperation.TYPE == null)
            {
                DatabaseOperation.TYPE = new DBManager();
            }
        }
        public OrderListResult getOrderList(string openid,string shop)
        {
            OrderListResult orderListResult = new OrderListResult();
            string sql = "select l.merchantOrderId,l.tradeTime from t_order_list l "
                       + " where l.sendapi = 'XXC' and l.customerCode = '" + openid + "' and l.`status` != '未支付' and l.purchaserId = '" + shop + "' ";
            DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count>0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string slt = "";
                    string pro = "";
                    string sql1 = "select g.barCode,max(g.skuBillName) goodsname,max(l.slt) slt " +
                        "from t_order_goods g,t_goods_list l " +
                        "where l.barcode = g.barCode and g.merchantOrderId = '" + dt.Rows[i]["merchantOrderId"].ToString() + "' " +
                        "group by g.barCode";
                    DataTable dt1 = DatabaseOperation.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
                    if (dt1.Rows.Count > 0)
                    {
                        slt = dt1.Rows[0]["slt"].ToString();
                        for (int j = 0; j < dt1.Rows.Count; j++)
                        {
                            pro += dt1.Rows[0]["goodsname"].ToString()+",";
                        }
                    }
                    OrderListItem orderListItem = new OrderListItem();
                    orderListItem.billId = dt.Rows[i]["merchantOrderId"].ToString();
                    orderListItem.imgUrl = slt;
                    orderListItem.createTime = dt.Rows[i]["tradeTime"].ToString();
                    orderListItem.product = pro.Substring(0,pro.Length-1);
                    orderListResult.orderList.Add(orderListItem);
                }
                return orderListResult;
            }
            else
            {
                return null;
            }

        }
    }
}
