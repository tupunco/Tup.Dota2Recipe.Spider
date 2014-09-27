using System.Collections.Generic;

using Newtonsoft.Json;

namespace Tup.Dota2Recipe.Spider.Entity
{
    /// <summary>
    /// Replays 推荐技能加点 实体
    /// </summary>
    public class ReplaysAbilitySkillItem
    {
        /// <summary>
        /// 分组名
        /// </summary>
        public string groupName { get; set; }
        /// <summary>
        /// 加点描述
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 推荐技能加点 索引数组
        /// </summary>
        public string[] abilityKeys { get; set; }
        /// <summary>
        /// replays 推荐技能加点 skillID 列表
        /// </summary>
        [JsonIgnore]
        public List<int> skillIDs { get; set; }
    }
    /// <summary>
    /// 技能实体
    /// </summary>
    public class AbilityItem
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        public string key_name { get; set; }

        public string dname { get; set; }
        public string affects { get; set; }
        public string desc { get; set; }
        public string dmg { get; set; }
        /// <summary>
        /// notes
        /// </summary>
        public string notes { get; set; }
        public string attrib { get; set; }
        /// <summary>
        /// 冷却时间/魔法消耗
        /// </summary>
        public string cmb { get; set; }
        public string lore { get; set; }
        /// <summary>
        /// 关联英雄名称
        /// </summary>
        public string hurl { get; set; }

        public override string ToString()
        {
            return string.Format("[AbilityItem name:{0},{1} hurl:{2}]", key_name, dname, hurl);
        }
    }
}
