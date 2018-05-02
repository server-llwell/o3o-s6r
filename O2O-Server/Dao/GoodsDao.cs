using Com.Portsoft.Framework.Database;
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
            if (DatabaseOperation.TYPE == null)
            {
                DatabaseOperation.TYPE = new DBManager();
            }
        }
        public DataTable getGoods(string barcode)
        {
            string sql = "select id,slt,goodsname,price,stock from t_goods_list where barcode = '" + barcode+"'";
            return DatabaseOperation.ExecuteSelectDS(sql, "t_goods_list").Tables[0];
        }
    }
}
