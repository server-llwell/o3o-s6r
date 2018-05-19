using Com.ACBC.Framework.Database;
using O2O_Server.Buss;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
namespace O2O_Server.Dao
{
    public class UserDao
    {

        public UserDao()
        {
            if (DatabaseOperationWeb.TYPE == null)
            {
                DatabaseOperationWeb.TYPE = new DBManager();
            }
        }
        public string getShopName(string shop)
        {
            string sql = "select shopName from t_sys_shop  "
                       + " where shopCode = '" + shop + "' ";
            DataTable dt = DatabaseOperationWeb.ExecuteSelectDS(sql, "t_sys_shop").Tables[0];
            if (dt.Rows.Count>0)
            {
                return dt.Rows[0][0].ToString();
            }
            else
            {
                return null;
            }

        }
    }
}
