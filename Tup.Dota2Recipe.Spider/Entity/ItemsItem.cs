using Newtonsoft.Json;

namespace Tup.Dota2Recipe.Spider.Entity
{
    /// <summary>
    /// 物品实体
    /// </summary>
    public class ItemsItem
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        [JsonIgnore]
        public string key_name { get; set; }
        [JsonIgnore]
        public string img { get; set; }
        
        public string dname { get; set; }
        public string dname_l { get; set; }
        /// <summary>
        /// 资质属性
        /// </summary>
        public string qual { get; set; }
        public string qual_l { get; set; }
        /// <summary>
        /// 物品基础分类
        /// </summary>
        public string itembasecat { get; set; }
        /// <summary>
        /// 物品分类
        /// </summary>
        public string itemcat { get; set; }
        public string itemcat_l { get; set; }
        /// <summary>
        /// 价值/价格
        /// </summary>
        public int cost { get; set; }
        /// <summary>
        /// 描述说明
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 知识提示
        /// </summary>
        public string lore { get; set; } 
        /// <summary>
        /// 属性加成
        /// </summary>
        public string attrib { get; set; }
        /// <summary>
        /// 魔法消耗
        /// </summary>
        public string mc { get; set; }
        /// <summary>
        /// cd[false=0]
        /// </summary>
        public int cd { get; set; }

        /// <summary>
        /// 合成所需物品
        /// </summary>
        public string[] components { get; set; }
        /// <summary>
        /// 进阶合成物品
        /// </summary>
        public string[] tocomponents { get; set; }

        public bool created { get; set; }
        /// <summary>
        /// 是否物品列表显示
        /// </summary>
        public bool ispublic { get; set; }

        public override string ToString()
        {
            return string.Format("[ItemsItem name:{0},{1}]", key_name, dname);
        }
    }
}
