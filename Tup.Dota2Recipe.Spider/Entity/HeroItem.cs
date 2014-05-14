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

        /// <summary>
        /// replays 站 HeroID
        /// </summary>
        [JsonIgnore]
        public string replays_id { get; set; }
        /// <summary>
        /// replays 站 pageName keyName
        /// </summary>
        [JsonIgnore]
        public string replays_pageName { get; set; }
        /// <summary>
        /// replays 站 技能列表
        /// </summary>
        [JsonIgnore]
        public Dictionary<int, ReplaysHeroSkillItem> replays_skill { get; set; }

        /// <summary>
        /// 昵称/别称
        /// </summary>
        /// <remarks>
        /// FROM:http://dota2.replays.net/
        /// </remarks>
        public string[] nickname_l { get; set; }
        /// <summary>
        /// 本英雄统计参数信息
        /// </summary>
        public HeroStatsItem statsall { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[HeroSimpleItem name:{0},{1} atk:{2},{3}]", name, name_l, atk, atk_l);
        }
    }
    /// <summary>
    /// replays 站 英雄技能实体
    /// </summary>
    public class ReplaysHeroSkillItem
    {
        /// <summary>
        /// 技能 ID
        /// </summary>
        public int SkillID { get; set; }
        /// <summary>
        /// 缩写, 快捷键
        /// </summary>
        public string SouXie { get; set; }
        /// <summary>
        /// 中文技能名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Dota2 官方技能 索引名称
        /// </summary>
        public string key_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[ReplaysHeroSkillItem Name:{0} SkillID:{1} SouXie:{2}]", Name, SkillID, SouXie);
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
        /// Replays 推荐技能加点
        /// </summary>
        public ReplaysAbilitySkillItem[] skillup { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[HeroItem {0} bio:{1}]", base.ToString(), bio_l);
        }
    }
    /// <summary>
    /// 英雄统计参数实体
    /// </summary>
    public class HeroStatsItem
    {
        /// <summary>
        /// 初始力量
        /// </summary>
        public double init_str { get; set; }
        /// <summary>
        /// 初始敏捷
        /// </summary>
        public double init_agi { get; set; }
        /// <summary>
        /// 初始智力
        /// </summary>
        public double init_int { get; set; }

        /// <summary>
        /// 力量成长
        /// </summary>
        public double lv_str { get; set; }
        /// <summary>
        /// 敏捷成长
        /// </summary>
        public double lv_agi { get; set; }
        /// <summary>
        /// 智力成长
        /// </summary>
        public double lv_int { get; set; }

        /// <summary>
        /// 初始血量
        /// </summary>
        public double init_hp { get; set; }
        /// <summary>
        /// 初始魔法
        /// </summary>
        public double init_mp { get; set; }
        /// <summary>
        /// 初始护甲
        /// </summary>
        public double init_armor { get; set; }

        /// <summary>
        /// 血量成长
        /// </summary>
        public double lv_hp { get; set; }
        /// <summary>
        /// 魔法成长
        /// </summary>
        public double lv_mp { get; set; }
        /// <summary>
        /// 护甲成长
        /// </summary>
        public double lv_armor { get; set; }

        /// <summary>
        /// 初始最小攻击力
        /// </summary>
        public double init_min_dmg { get; set; }
        /// <summary>
        /// 初始最大攻击力
        /// </summary>
        public double init_max_dmg { get; set; }
        /// <summary>
        /// 攻击成长
        /// </summary>
        public double lv_dmg { get; set; }
    }
}
