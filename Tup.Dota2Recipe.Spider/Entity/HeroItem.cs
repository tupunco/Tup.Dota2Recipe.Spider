using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tup.Dota2Recipe.Spider.Entity
{
    /// <summary>
    /// 英雄实体
    /// </summary>
    public class HeroSimpleItem
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        [JsonIgnore]
        public string key_name { get; set; }
        /// <summary>
        /// Uri索引名称
        /// </summary>
        [JsonIgnore]
        public string keyUri_name { get; set; }
        /// <summary>
        /// 属性[1:strength[力量]/2:agility[敏捷]/3:intelligence[智力]]
        /// </summary>
        public string hp { get; set; }
        /// <summary>
        /// 阵营[1:radiant[天辉]/2:dire[夜魇]]
        /// </summary>
        public string faction { get; set; }

        /// <summary>
        /// 英雄名称
        /// </summary>
        public string name { get; set; }
        public string name_l { get; set; }
        /// <summary>
        /// 攻击类型
        /// </summary>
        public string atk { get; set; }
        public string atk_l { get; set; }
        /// <summary>
        /// 角色定位
        /// </summary>
        public string[] roles { get; set; }
        public string[] roles_l { get; set; }

        public override string ToString()
        {
            return string.Format("[HeroSimpleItem name:{0},{1} atk:{2},{3}]", name, name_l, atk, atk_l);
        }
    }
    /// <summary>
    /// 英雄实体
    /// </summary>
    public class HeroItem : HeroSimpleItem
    {
        /// <summary>
        /// 描述信息
        /// </summary>
        public string bio { get; set; }
        public string bio_l { get; set; }

        /// <summary>
        /// 基本统计信息
        /// </summary>
        public string stats { get; set; }
        /// <summary>
        /// 基本统计信息
        /// </summary>
        public string[][] stats1 { get; set; }
        /// <summary>
        /// 详细统计信息
        /// </summary>
        public string detailstats { get; set; }
        /// <summary>
        /// 详细统计信息-1
        /// </summary>
        public string[][] detailstats1 { get; set; }
        /// <summary>
        /// 详细统计信息-2
        /// </summary>
        public string[][] detailstats2 { get; set; }
        /// <summary>
        /// 技能列表
        /// </summary>
        public AbilityItem[] abilities { get; set; }
        /// <summary>
        /// 英雄 推荐装备
        /// </summary>
        public IDictionary<string, string[]> itembuilds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[HeroItem {0} bio:{1}]", base.ToString(), bio_l);
        }
    }
}
