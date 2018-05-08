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
        /// <summary>
        /// 保存订单
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="billid"></param>
        /// <param name="paymentParam"></param>
        /// <returns></returns>
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
                            "'','','未付款','" + paymentParam.shop + "'," +
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
        /// <summary>
        /// 保存订单商品
        /// </summary>
        /// <param name="billid"></param>
        /// <param name="goodsDT"></param>
        /// <param name="paymentParam"></param>
        /// <returns></returns>
        private bool saveOrderGoods(string billid, DataTable goodsDT, PaymentParam paymentParam)
        {
            string insql = "insert into t_order_goods(merchantOrderId,barCode,skuUnitPrice,quantity," +
                        " goodsName,sendType,skubillname,supplyPrice,purchasePrice) " +
                        " values('" + billid + "','" + goodsDT.Rows[0]["barcode"].ToString() + "'," + goodsDT.Rows[0]["price"].ToString() + "," + paymentParam.inputNum + "," +
                        " '" + goodsDT.Rows[0]["goodsname"].ToString() + "','XXC','" + goodsDT.Rows[0]["goodsname"].ToString() + "',0,0)";
            if (DatabaseOperation.ExecuteDML(insql))
            {
                //setGoodsNum(billid, goodsDT.Rows[0]["barcode"].ToString(),Convert.ToInt32( paymentParam.inputNum));
                return true;
            }
            else
            {
                return false;
            }

        }
        //private void setGoodsNum(string orderid, string tm, int num)
        //{
        //    string sql = "update t_ck_goods_warehouse set goodsnum=goodsnum-" + num + " where barcode = '" + tm;//+ "' and wcode= '" + wcode + "' ";
        //    if (DatabaseOperation.ExecuteDML(sql))
        //    {
        //        setGoodsNumLog(orderid, tm, num, "扣除库存");
        //    }
        //}

        //private void setGoodsNumLog(string orderid, string tm, int num, string state)
        //{
        //    string sql = "insert into t_log_goodsnum (createtime,wcode,orderid,tm,goodsnum,state) values(now(),'" + "" + "','" + orderid + "','" + tm + "'," + num + ",'" + state + "')";
        //    DatabaseOperation.ExecuteDML(sql);
        //}

        /// <summary>
        /// 获取订单总金额
        /// </summary>
        /// <param name="paymentParam"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 核对订单总金额和支付金额
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="totalPrice"></param>
        /// <returns></returns>
        public bool checkOrderTotalPrice(string orderId,double totalPrice)
        {
            string sql = "select * from t_order_list where parentOrderId = '"+orderId+"'";
            DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            if (dt.Rows.Count > 0)
            {
                double total = Convert.ToDouble(dt.Rows[0]["tradeAmount"]);
                if (total*100 == totalPrice)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 修改支付状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payNo"></param>
        public bool updateOrderForPay(string orderId,string payNo)
        {
            string upsql = "update t_order_list set payNo='"+payNo+ "',payType='微信支付',status ='新订单' where parentOrderId = '" + orderId + "' ";
            return DatabaseOperation.ExecuteDML(upsql);
        }

        /// <summary>
        /// 保存支付日志
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payNo"></param>
        /// <param name="totalPrice"></param>
        /// <param name="openid"></param>
        /// <param name="status"></param>
        public void insertPayLog(string orderId, string payNo, string totalPrice,string openid,string status)
        {
            string insql = "insert into t_log_pay(orderId,payType,payNo,totalPrice,openid,createtime,status) " +
                "values('"+orderId+ "','微信支付','" + payNo + "'," + totalPrice + ",'" + openid + "',now(),'" + status + "')";
            DatabaseOperation.ExecuteDML(insql);
        }

        /// <summary>
        /// 写入prepayid
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="prePayId"></param>
        /// <returns></returns>
        public bool writePrePayId(string orderId,string prePayId)
        {
            string upsql = "update t_order_list set prePayId= '"+prePayId+ "' where parentOrderId = '" + orderId + "' ";
            return DatabaseOperation.ExecuteDML(upsql);
        }

        /// <summary>
        /// 获取prepayid
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="prePayId"></param>
        /// <returns></returns>
        public PaymentDataResults getPayData(string orderId)
        {
            string sql = "select s.shopName,g.goodsName,o.tradeTime,o.tradeAmount, o.prePayId from t_order_list o,t_sys_shop s,t_order_goods g where o.merchantOrderId = g.merchantOrderId and o.purchaserId = s.shopCode and  parentOrderId = '" + orderId + "' ";
            DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            PaymentDataResults p = new PaymentDataResults();
            p.shopName = dt.Rows[0]["shopName"].ToString();
            p.goodsName = dt.Rows[0]["goodsName"].ToString();
            p.tradeTime = dt.Rows[0]["tradeTime"].ToString();
            p.tradeAmount = dt.Rows[0]["tradeAmount"].ToString();
            p.prePayId = dt.Rows[0]["prePayId"].ToString();
            return p;
        }
    }
}
