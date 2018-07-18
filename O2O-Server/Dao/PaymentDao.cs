﻿using Com.ACBC.Framework.Database;
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
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
        public DataTable getGoods(string barcode)
        {
            string sql = "select id,thumb,goodsname,price,stock from t_goods_list where barcode = '" + barcode+"'";
            return DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
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
                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string sql1 = "select wcode from t_goods_warehouse where barcode = '" + dt.Rows[0]["barcode"].ToString() + "' and suppliercode ='shingostory@163.com'  ";
                    DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];
                    string wcode = "";
                    if (dt1.Rows.Count>0)
                    {
                        wcode = dt1.Rows[0]["wcode"].ToString();
                    }
                    double total = Convert.ToDouble(dt.Rows[0]["price"]) * Convert.ToDouble(paymentParam.inputNum);
                    string insql = "insert into t_order_list(warehouseCode,customerCode,parentOrderId,merchantOrderId,tradeTime," +
                                                    "tradeAmount,goodsTotalAmount,consigneeName,consigneeMobile," +
                                                    "addrCountry,addrProvince,addrCity,addrDistrict," +
                                                    "addrDetail,zipCode,idType,idNumber," +
                                                    "idFountImgUrl,idBackImgUrl,status,purchaserId," +
                                                    "apitype,fqID,sendapi,orderType) " +
                            "values('" + wcode + "','" + openId + "','" + billid + "','" + billid + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
                            "" + total + "," + total + ",'" + paymentParam.inputName + "','" + paymentParam.inputPhone + "'," +
                            "'中国','" + addrSts[0] + "','" + addrSts[1] + "','" + addrSts[2] + "'," +
                            "'" + addrSts[3] + "','000000','1','" + paymentParam.inputIdCard + "'," +
                            "'','',0,'" + paymentParam.shop + "'," +
                            "'XXC','','XXC','"+ paymentParam.radio + "')";
                    if (DatabaseOperationWeb.ExecuteDML(insql))
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
            string sql = "select offer from t_goods_offer where barcode = '" + goodsDT.Rows[0]["barcode"].ToString() + "' and usercode ='O2O' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
            string sql1 = "select inprice from t_goods_warehouse where barcode = '" + goodsDT.Rows[0]["barcode"].ToString() + "' and suppliercode ='shingostory@163.com'  ";
            DataTable dt1 = DatabaseOperationWeb.ExecuteSelectDS(sql1, "t_goods_list").Tables[0];

            string insql = "insert into t_order_goods(merchantOrderId,barCode,slt,skuUnitPrice,quantity," +
                        " goodsName,sendType,skubillname,supplyPrice,purchasePrice) " +
                        " values('" + billid + "','" + goodsDT.Rows[0]["barcode"].ToString() + "','" + goodsDT.Rows[0]["slt"].ToString() + 
                        "'," + goodsDT.Rows[0]["price"].ToString() + "," + paymentParam.inputNum + "," +
                        " '" + goodsDT.Rows[0]["goodsname"].ToString() + "','XXC','" + goodsDT.Rows[0]["goodsname"].ToString() +
                        "',"+dt.Rows[0][0].ToString()+ "," + dt1.Rows[0][0].ToString() + ")";
            if (DatabaseOperationWeb.ExecuteDML(insql))
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
                string sql = "select price from t_goods_list where id =" + paymentParam.goodsId;
                DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
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
#if DEBUG
            return true;
#endif
            string sql = "select * from t_order_list where parentOrderId = '"+orderId+"'";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
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
            string upsql = "update t_order_list set payNo='"+payNo+ "',payType='微信支付',payTime=now(),status =1 where parentOrderId = '" + orderId + "' ";
            return DatabaseOperationWeb.ExecuteDML(upsql);
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
            DatabaseOperationWeb.ExecuteDML(insql);
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
            return DatabaseOperationWeb.ExecuteDML(upsql);
        }

        /// <summary>
        /// 获取prepayid
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="prePayId"></param>
        /// <returns></returns>
        public PaymentDataResults getPayData(string orderId)
        {
            string sql = "select s.shopName,g.goodsName,o.tradeTime,o.tradeAmount, o.prePayId, o.payNo,o.customerCode from t_order_list o,t_sys_shop s,t_order_goods g where o.merchantOrderId = g.merchantOrderId and o.purchaserId = s.shopCode and  parentOrderId = '" + orderId + "' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_order_list").Tables[0];
            PaymentDataResults p = new PaymentDataResults();
            p.shopName = dt.Rows[0]["shopName"].ToString();
            p.goodsName = dt.Rows[0]["goodsName"].ToString();
            p.tradeTime = dt.Rows[0]["tradeTime"].ToString();
            p.tradeAmount = dt.Rows[0]["tradeAmount"].ToString();
            p.prePayId = dt.Rows[0]["prePayId"].ToString();
            p.payNo = dt.Rows[0]["payNo"].ToString();
            p.customerCode = dt.Rows[0]["customerCode"].ToString();
            
            return p;
        }
    }
}
