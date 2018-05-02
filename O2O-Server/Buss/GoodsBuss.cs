using Newtonsoft.Json;
using O2O_Server.Common;
using O2O_Server.Dao;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.WxOpen.Entities;
using Senparc.Weixin.WxOpen.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace O2O_Server.Buss
{
    public class GoodsBuss : IBuss
    {
        public ApiType GetApiType()
        {
            return ApiType.GoodsApi;
        }

        public object Do_GetGoods(object param)
        {
            GetGoodsParam ggp = JsonConvert.DeserializeObject<GetGoodsParam>(param.ToString());
            if(ggp == null)
            {
                throw new ApiException(CodeMessage.InvalidParam, "InvalidParam");
            }
            GoodsDao gd = new GoodsDao();
            DataTable dt =gd.getGoods(ggp.barcode);
            if (dt.Rows.Count>0)
            {
                Goods goods = new Goods();
                goods.id = dt.Rows[0]["id"].ToString();
                goods.slt = dt.Rows[0]["slt"].ToString();
                goods.goodsname = dt.Rows[0]["goodsname"].ToString();
                goods.price = dt.Rows[0]["price"].ToString();
                goods.stock = dt.Rows[0]["stock"].ToString();
                return goods;
            }
            else
            {
                throw new ApiException(CodeMessage.GoodsNotFound, "GoodsNotFound");
            }
        }
    }

    public class GetGoodsParam
    {
        public string barcode;
    }
    public class Goods
    {
        public string id;
        public string slt;
        public string goodsname;
        public string price;
        public string stock;
    }
}
