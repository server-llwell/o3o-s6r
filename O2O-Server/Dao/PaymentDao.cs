using Com.Portsoft.Framework.Database;
using O2O_Server.Buss;
using O2O_Server.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
namespace O2O_Server.Dao
{
    public class PaymentDao
    {

        public PaymentDao()
        {
            if (DatabaseOperation.TYPE == null)
            {
                DatabaseOperation.TYPE = new DBManager();
            }
        }
        public DataTable getGoods(string barcode)
        {
            string sql = "select id,thumb,goodsname,price,stock from t_goods_list where barcode = '" + barcode+"'";
            return DatabaseOperation.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
        }
        public bool saveOrder(string openId, string billid, PaymentParam paymentParam)
        {
            try
            {
                Util util = new Util();
                string[] addrSts = util.getAddr(paymentParam.inputAddress);//获取地址编码
                string sql = "select * from t_goods_list where id =" + paymentParam.goodsId;
                DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    double total = Convert.ToDouble(dt.Rows[0]["price"]) * Convert.ToDouble(paymentParam.inputNum);
                    string insql = "insert into t_order_list(customerCode,parentOrderId,merchantOrderId,tradeTime," +
                                                    "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
                                                    "addrCountry,addrProvince,addrCity,addrDistrict," +
                                                    "addrDetail,zipCode,idType,idNumber," +
                                                    "idFountImgUrl,idBackImgUrl,status,purchaserId," +
                                                    "apitype,fqID,sendapi,orderType) " +
                            "values('" + openId + "','" + billid + "','" + billid + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                            "" + total + "," + total + ",'" + paymentParam.inputName + "','" + paymentParam.inputPhone + "'," +
                            "'中国','" + addrSts[0] + "','" + addrSts[1] + "','" + addrSts[2] + "'," +
                            "'" + addrSts[3] + "','000000','1','" + paymentParam.inputIdCard + "'," +
                            "'','','新订单','" + paymentParam.shop + "'," +
                            "'XXC','','XXC','"+ paymentParam.radio + "')";
                    if (DatabaseOperation.ExecuteDML(insql))
                    {
                        if (saveOrderGoods(billid,dt,paymentParam))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool saveOrderGoods(string billid, DataTable goodsDT, PaymentParam paymentParam)
        {
            string insql = "insert into t_order_goods(merchantOrderId,barCode,skuUnitPrice,quantity," +
                        " goodsName,sendType,skubillname,supplyPrice,purchasePrice) " +
                        " values('" + billid + "','" + goodsDT.Rows[0]["barcode"].ToString() + "'," + goodsDT.Rows[0]["price"].ToString() + "," + paymentParam.inputNum + "," +
                        " '" + goodsDT.Rows[0]["goodsname"].ToString() + "','XXC','" + goodsDT.Rows[0]["goodsname"].ToString() + "',0,0)";
            if (DatabaseOperation.ExecuteDML(insql))
            {
                setGoodsNum(billid, goodsDT.Rows[0]["barcode"].ToString(),Convert.ToInt32( paymentParam.inputNum));
                return true;
            }
            else
            {
                return false;
            }

        }
        private void setGoodsNum(string orderid, string tm, int num)
        {
            string sql = "update t_ck_goods_warehouse set goodsnum=goodsnum-" + num + " where barcode = '" + tm + "' and wcode= '" + wcode + "' ";
            if (DatabaseOperation.ExecuteDML(sql))
            {
                setGoodsNumLog(orderid, tm, num, "扣除库存");
            }
        }

        private void setGoodsNumLog(string orderid, string tm, int num, string state)
        {
            string sql = "insert into t_log_goodsnum (createtime,wcode,orderid,tm,goodsnum,state) values(now(),'" + wcode + "','" + orderid + "','" + tm + "'," + num + ",'" + state + "')";
            DatabaseOperation.ExecuteDML(sql);
        }

        public int getOrderTotalPrice(PaymentParam paymentParam)
        {
            try
            {
                string sql = "select * from t_goods_list where id =" + paymentParam.goodsId;
                DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    double total = Convert.ToDouble(dt.Rows[0]["price"]) * Convert.ToDouble(paymentParam.inputNum);
                    return Convert.ToInt32( total * 100);
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
