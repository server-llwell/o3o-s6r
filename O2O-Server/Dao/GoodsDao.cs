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
            string sql = "select id,thumb,goodsname,price,stock from t_goods_list where barcode = '" + barcode+"'";
            return DatabaseOperationWeb.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
        }
    }
}
