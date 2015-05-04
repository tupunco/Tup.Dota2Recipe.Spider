using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Tup.Dota2Recipe.Spider.Common;
using Tup.Dota2Recipe.Spider.Entity;

namespace Tup.Dota2Recipe.Spider
{
    /// <summary>
    /// 
    /// </summary>
    static class ItemUtils
    {
        private static Regex s_RegAbilityItemCmb = new Regex(@"<div\s+class=""(?<type>mana|cooldown)"">\s*<img\s+alt=""(?<alt>.*?)""[^>]+?/>\s*(?<value>[\s\S]+?)\s*</div>", RegexOptions.IgnoreCase);
        private static Regex s_RegAbilityItemDesc = new Regex(@"color='(\#[\w]{6})'", RegexOptions.IgnoreCase);
        private static Regex s_RegAbilityItemAttrib = new Regex(@"<span\s+class=""scepterVal"">([^>]*?)</span>", RegexOptions.IgnoreCase);
        private static Regex s_RegCommonBr2 = new Regex(@"(?:<br[^>]*?>\s*){2,}", RegexOptions.IgnoreCase);
        private static Regex s_RegCommonBrEnd = new Regex(@"<br[^>]*?>\s*$", RegexOptions.IgnoreCase);
        private static Regex s_RegCommonRN = new Regex(@"[\s]+(?=[<])|(?<=[>])[\s]+|[\r\n]+", RegexOptions.IgnoreCase);
        private static Regex s_RegHeroItemStats = new Regex(@"<img\s+title=""(?<alt>[\w]+)""[^>]*/>\s*<div\s+class=""overview_StatVal""\s+id=""overview_(?<type>[\w]+?)Val"">\s*(?<value>[\s\S]+?)\s*</div>", RegexOptions.IgnoreCase);
        private static Regex s_RegHeroItemDetailstats1 = new Regex(@"<div\s+class=""statRowB?"">\s*(?:<div\s+class=""statRowCol2?W"">(?<value>[^<]+?)</div>\s*){3}\s*(?<type>[^<]+?)\s*</div>", RegexOptions.IgnoreCase);
        private static Regex s_RegHeroItemDetailstats2 = new Regex(@"<div\s+class=""statRowB?"">\s*(?:<div\s+class=""statRowCol2?W"">(?<value>[^<]+?)</div>\s*){1}\s*(?<type>[^<]+?)\s*</div>", RegexOptions.IgnoreCase);
        /// <summary>
        /// Fix 英雄详细统计信息
        /// </summary>
        /// <param name="html"></param>
        /// <param name="type">1,2</param>
        /// <returns></returns>
        public static string[][] HeroItem_FixDetailstatsField(string html, int type)
        {
            ThrowHelper.ThrowIfOutOfRange(type, new int[] { 1, 2 }, "type");
            if (string.IsNullOrEmpty(html))
                return null;

            var sb = new StringBuilder();
            var cReg = type == 1 ? s_RegHeroItemDetailstats1 : s_RegHeroItemDetailstats2;
            var res = new List<string[]>();
            var match = cReg.Match(html);
            while (match.Success)
            {
                //value/type
                var typev = match.Groups["type"].Value;
                var values = match.Groups["value"];
                if (values.Captures.Count <= 1)
                    res.Add(new string[] { typev, values.Value });
                else
                {
                    var elm = new List<string>();
                    elm.Add(typev);
                    elm.AddRange(values.Captures.Cast<Capture>().Select(x => x.Value));
                    res.Add(elm.ToArray());
                }
                match = match.NextMatch();
            }
            return res.ToArray();
        }
        /// <summary>
        /// Fix 英雄基本统计信息
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HeroItem_FixStatsField(string html, out string[][] stats)
        {
            stats = null;
            if (string.IsNullOrEmpty(html))
                return html;

            var statsList = new List<string[]>();
            var sb = new StringBuilder();
            var match = s_RegHeroItemStats.Match(html);
            while (match.Success)
            {
                //type/alt/value
                if (sb.Length > 0)
                    sb.Append("<br />");
                sb.AppendFormat(@"<img src=""{0}"" alt=""{1}"" />{1}:{2}", match.Groups["type"].Value, match.Groups["alt"].Value, match.Groups["value"].Value);
                statsList.Add(new string[] { match.Groups["type"].Value, match.Groups["alt"].Value, match.Groups["value"].Value });

                match = match.NextMatch();
            }
            stats = statsList.ToArray();
            return sb.ToString();
        }
        /// <summary>
        /// Fix 英雄统计参数信息
        /// </summary>
        /// <param name="hp">英雄属性[1:strength[力量]/2:agility[敏捷]/3:intelligence[智力]]</param>
        /// <param name="stats"></param>
        /// <param name="detailstats"></param>
        /// <returns></returns>
        public static HeroStatsItem HeroItem_FixStatsAllField(string hp, string[][] stats, string[][] detailstats)
        {
            if (string.IsNullOrEmpty(hp) || stats == null || detailstats == null || stats.Length < 6 || detailstats.Length < 4)
                return null;

            //stats1: [["Int","Intelligence","21 + 2.00"],
            //           ["Agi","Agility","17 + 1.50"],
            //           ["Str","Strength","23 + 2.70"],
            //           ["Attack","Damage","32 - 42"],
            //           ["Speed","Movespeed","310"],
            //           ["Defense","Armor","1.38"]],

            var s = new HeroStatsItem();
            //智力
            var intsp = HeroItem_FixStatsAllField_Split(stats[0][2], '+');
            s.init_int = intsp.Item1;
            s.lv_int = intsp.Item2;
            //敏捷
            var agisp = HeroItem_FixStatsAllField_Split(stats[1][2], '+');
            s.init_agi = agisp.Item1;
            s.lv_agi = agisp.Item2;
            //力量
            var strsp = HeroItem_FixStatsAllField_Split(stats[2][2], '+');
            s.init_str = strsp.Item1;
            s.lv_str = strsp.Item2;

            //攻击力
            var dmgsp = HeroItem_FixStatsAllField_Split(stats[3][2], '-');
            s.init_min_dmg = dmgsp.Item1;
            s.init_max_dmg = dmgsp.Item2;
            if (hp == "strength")
                s.lv_dmg = s.lv_str;
            else if (hp == "agility")
                s.lv_dmg = s.lv_agi;
            else if (hp == "intelligence")
                s.lv_dmg = s.lv_int;
            else
                throw new NotSupportedException();

            //初始移动速度
            s.init_ms = double.Parse(stats[4][2]);

            //"detailstats1":[["生命值","2,198","1,305","587"],
            //               ["魔法值","1,157","637","273"],
            //               ["攻击力","140-150","93-103","55-65"],
            //               ["护甲","9","4","1"]]

            //护甲
            var armorsp = HeroItem_FixStatsAllField_Split(stats[5][2], 'x');
            s.init_armor = armorsp.Item1;
            s.lv_armor = Math.Round(1D / 7D, 2);

            s.lv_hp = 19D;
            s.lv_mp = 13D;
            s.init_hp = HeroItem_FixStatsAllField_Split(detailstats[0][3], 'x').Item1;
            s.init_mp = HeroItem_FixStatsAllField_Split(detailstats[1][3], 'x').Item1;

            return s;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        private static Tuple<double, double> HeroItem_FixStatsAllField_Split(string value, char splitChar)
        {
            ThrowHelper.ThrowIfNull(value, "value");

            if (splitChar == 'x')
                return Tuple.Create(double.Parse(value.Replace(",", "")), 0D);
            else
            {
                var t = value.Split(splitChar);
                if (t.Length > 1)
                    return Tuple.Create(double.Parse(t[0].Replace(",", "").Trim()), double.Parse(t[1].Replace(",", "").Trim()));
            }
            throw new NotSupportedException();
        }
        /// <summary>
        /// fix AbilityItem Cmb Field
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string AbilityItem_FixCmbField(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            var sb = new StringBuilder();
            var m = s_RegAbilityItemCmb.Match(html);
            var f = @"<img src=""{0}"" alt=""{1}"" />{1}:{2}";
            while (m.Success)
            {
                //type/alt/value
                if (sb.Length > 0)
                    sb.Append("<br />");
                sb.AppendFormat(f, m.Groups["type"].Value, m.Groups["alt"].Value, m.Groups["value"].Value);

                m = m.NextMatch();
            }
            return sb.ToString();
        }
        /// <summary>
        /// fix AbilityItem Desc Field
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string AbilityItem_FixDescField(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            return s_RegAbilityItemDesc.Replace(html, @"color=""$1""");
        }
        /// <summary>
        /// fix AbilityItem Attrib Field
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string AbilityItem_FixAttribField(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            return s_RegAbilityItemAttrib.Replace(html, @"<span class=""scepterVal""><font color=""#6fe771"">$1</font></span>");
        }
        /// <summary>
        /// 归置 HTML 内 Br, 末尾/中间多个/
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string Common_FixBrHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            html = s_RegCommonBr2.Replace(html, "<br />");
            html = s_RegCommonBrEnd.Replace(html, "");
            html = s_RegCommonRN.Replace(html, "");
            html = html.Replace("&nbsp;", "");

            return html;
        }
    }
}
