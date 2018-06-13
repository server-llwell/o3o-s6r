using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
namespace O2O_Server.Dao
{
    public class GoodsDao
    {

        public GoodsDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
        public DataTable getGoods(string barcode)
        {
            string sql = "select g.id,g.thumb,g.goodsname,g.price,g.stock from t_goods_list g,t_goods_offer o " +
                "where g.barcode = o.barcode and o.usercode='O2O' and o.flag='1' and g.barcode = '" + barcode+"'";
            return DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
        }
    }
}
