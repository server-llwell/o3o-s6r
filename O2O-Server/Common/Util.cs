using Com.ACBC.Framework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace O2O_Server.Common
{
    public class Util
    {
        public string[] getAddr(string addrDetail)
        {
            if (Global.provincesDT==null)
            {
                string psql = "select * from t_base_provinces";
                Global.provincesDT = DatabaseOperationWeb.ExecuteSelectDS(psql, "provinces").Tables["provinces"];
                string csql = "select * from t_base_cities";
                Global.cityDT = DatabaseOperationWeb.ExecuteSelectDS(csql, "cities").Tables["cities"];
                string asql = "select * from t_base_areas";
                Global.areasDT = DatabaseOperationWeb.ExecuteSelectDS(asql, "areas").Tables["areas"];
            }

            string[] sts = new string[4];
            for (int i = 0; i < Global.provincesDT.Rows.Count; i++)
            {
                string pro = Global.provincesDT.Rows[i]["province"].ToString().Replace("省", "").Replace("回族自治区", "").Replace("壮族自治区", "").Replace("维吾尔自治区", "").Replace("自治区", "");
                if (addrDetail.IndexOf(pro) > -1 && addrDetail.IndexOf(pro) < 2)
                {
                    sts[0] = pro;
                    string proId = Global.provincesDT.Rows[i]["provinceid"].ToString(), cityId = "";
                    if (proId == "110" || proId == "120" || proId == "140" || proId == "330")
                    {
                        sts[1] = pro + "市";
                        cityId = proId + "01";
                    }
                    else
                    {
                        DataRow[] citydrs = Global.cityDT.Select("provinceid='" + proId + "'");
                        for (int j = 0; j < citydrs.Length; j++)
                        {
                            if (addrDetail.IndexOf(citydrs[j]["city"].ToString()) > 1)
                            {
                                sts[1] = citydrs[j]["city"].ToString();
                                cityId = citydrs[j]["cityid"].ToString();
                            }
                        }
                    }
                    if (cityId != "")
                    {
                        DataRow[] areasdrs = Global.areasDT.Select("cityid='" + cityId + "'");
                        for (int j = 0; j < areasdrs.Length; j++)
                        {
                            if (addrDetail.IndexOf(areasdrs[j]["area"].ToString()) > 1)
                            {
                                sts[2] = areasdrs[j]["area"].ToString();
                                sts[3] = addrDetail.Substring(addrDetail.IndexOf(sts[2])+ sts[2].Length);
                            }
                        }
                    }
                }
            }
            return sts;
        }
    }
}
