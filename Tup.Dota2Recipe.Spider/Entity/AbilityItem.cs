
namespace Tup.Dota2Recipe.Spider.Entity
{
    /// <summary>
    /// 技能实体
    /// </summary>
    public class AbilityItem
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        //[JsonIgnore]
        public string key_name { get; set; }

        public string dname { get; set; }
        public string affects { get; set; }
        public string desc { get; set; }
        public string dmg { get; set; }
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
        /*
dname: "电子涡流",
affects: "技能： <span class="attribVal">单位目标</span><br />影响： <span class="attribVal">敌方单位</span><br />",
desc: "将一个敌人拖到风暴之灵所在位置的涡流，同时也会降低风暴之灵自己50%的移动速度3秒。",
dmg: "",
attrib: "持续时间： <span class="attribVal">1 / 1.5 / 2 / 2.5</span>",
cmb: "<div class="cooldownMana"><div class="mana"><img alt="魔法消耗" title="魔法消耗" class="manaImg" src="http://media.steampowered.com/apps/dota2/images/tooltips/mana.png" width="16" height="16" border="0" /> 100/110/120/130</div><div class="cooldown"><img alt="冷却时间" title="冷却时间" class="cooldownImg" src="http://media.steampowered.com/apps/dota2/images/tooltips/cooldown.png" width="16" height="16" border="0" /> 20</div><br clear="left" /></div>",
lore: "雷神的狂暴的雷霆力量总是让其他人被电到。",
hurl: "Storm_Spirit"
         */
    }
}
