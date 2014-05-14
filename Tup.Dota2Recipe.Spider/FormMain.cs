using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;

using Tup.Dota2Recipe.Spider.Common;
using Tup.Dota2Recipe.Spider.Entity;

namespace Tup.Dota2Recipe.Spider
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private static readonly Random s_SysRandom = new Random();
        private static readonly string s_LSChinese = "&l=schinese";
        private static readonly string s_Dota2HostUri = "http://www.dota2.com";
        //private static readonly string s_Dota2SteamMedia = "http://media.steampowered.com/apps/dota2/images";
        private static readonly string s_Dota2SteamMedia = "http://cdn.dota2.com/apps/dota2/images";

        /// <summary>
        /// GB2312 Encoding
        /// </summary>
        private static readonly Encoding s_GB2312Encoding = Encoding.GetEncoding("GB2312");
        /// <summary>
        /// 技能图片
        /// </summary>
        private static readonly string s_GetImageAbilitiesUri = s_Dota2SteamMedia + "/abilities/{0}_hp1.png";
        /// <summary>
        /// 英雄头像小图片
        /// </summary>
        private static readonly string s_GetImageHeroesSBUri = s_Dota2SteamMedia + "/heroes/{0}_sb.png";
        /// <summary>
        /// 英雄头像大图片
        /// </summary>
        private static readonly string s_GetImageHeroesHPHoverUri = s_Dota2SteamMedia + "/heroes/{0}_hphover.png";
        /// <summary>
        /// 英雄完整(full)图片
        /// </summary>
        private static readonly string s_GetImageHeroesFullUri = s_Dota2SteamMedia + "/heroes/{0}_full.png";
        /// <summary>
        /// 英雄肖像(Vert)图片
        /// </summary>
        private static readonly string s_GetImageHeroesVertUri = s_Dota2SteamMedia + "/heroes/{0}_vert.jpg";
        /// <summary>
        /// 英雄肖像(Icon/标识)图片
        /// </summary>
        private static readonly string s_GetImageHeroesIconUri = "http://www.dota2.com.cn/images/heroes/{0}_icon.png";
        /// <summary>
        /// 物品图片
        /// </summary>
        private static readonly string s_GetImageItemsLGUri = s_Dota2SteamMedia + "/items/{0}_lg.png";

        /// <summary>
        /// 英雄列表
        /// </summary>
        private static readonly string s_GetHeroDataJsonUri = s_Dota2HostUri + "/jsfeed/heropickerdata?v={0}";
        /// <summary>
        /// 英雄列表HTML页面
        /// </summary>
        private static readonly string s_GetHeroDataHtmlUri = s_Dota2HostUri + "/heroes/?v={0}";
        /// <summary>
        /// 英雄详细HTML页面
        /// </summary>
        private static readonly string s_GetHeroItemDataHtmlUri = s_Dota2HostUri + "/hero/{0}/?v={1}";
        /// <summary>
        /// 英雄技能JSON页面
        /// </summary>
        private static readonly string s_GetAbilityDataJsonUri = s_Dota2HostUri + "/jsfeed/heropediadata?feeds=abilitydata&v={0}";
        /// <summary>
        /// 物品JSON页面
        /// </summary>
        private static readonly string s_GetItemsDataJsonUri = s_Dota2HostUri + "/jsfeed/heropediadata?feeds=itemdata&v={0}";
        /// <summary>
        /// 物品HTML页面
        /// </summary>
        private static readonly string s_GetItemsDataHtmlUri = s_Dota2HostUri + "/items/?v={0}";

        /// <summary>
        /// replays.net GetHeroData_List
        /// </summary>
        private static readonly string s_Replays_GetHeroData_List_Uri = @"http://dota2.replays.net/hero/";
        /// <summary>
        /// replays.net GetHeroData_Detail AJAX
        /// </summary>
        private static readonly string s_Replays_GetHeroData_Detail_Uri = s_Replays_GetHeroData_List_Uri + @"services/heroData.ashx?heroID={0}";
        /// <summary>
        /// replays.net GetHeroData_Detail HTML 英雄详细页面
        /// </summary>
        private static readonly string s_Replays_GetHeroData_Html_Detail_Uri = s_Replays_GetHeroData_List_Uri + @"html/{0}.shtml";
        /// <summary>
        /// replays.net GetHeroData_List HTML 正则
        /// </summary>
        private static readonly string s_Replays_GetHeroData_List_Reg = @"<li\s+data-heroid=""(?<id>\d+)""\s+data-type=""[^""]+"">\s*<a\s+href=""[^""]+""\s+target=""_blank"">\s*<img\s+src=""http://rnimg\.cn/dota2/images/heros/(?<keyname>[^""]+)_sb\.(?:png|jpg)""\s+alt=""(?<name1>[^""]+)"">\s*</a>\s*<em>\s*<a\s+href=""/hero/html/(?<pagename>.+)\.shtml""\s+target=""_blank"">[^<]+</a>\s*</em>\s*</li>";
        /// <summary>
        /// replays.net GetHeroData_Detail HTML Skill 正则 英雄详细页面-技能加点
        /// </summary>
        /// <remarks>
        /// groupName/skillId/desc
        /// </remarks>
        private static readonly string s_Replays_GetHeroData_Detail_Skill_Reg = @"<div\s+class='Hero-tops-box'>\s*<h3>\s*?(?<groupName>[^<]+)\s*?</h3>\s*<div\s+class='Hero-img-list'>\s*?(?<skillGroup><img\s+id='only-\d+'\s+data-skill='(?<skillId>\d+)'\s+src=""http://dota2.replays.net/hero/images/[^""]+""[^>]+/>)+\s*?</div>\s*<div\s+class='Hero-text-box'>\s*?(?<desc>[\s\S]*?)\s*?</div>\s*</div>";
        /// <summary>
        /// replays.net 英雄技能名称到 Dota2 官方数据名称转换 映射字典
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, string>> s_Replays_HeroDataAbilityName_Convert_Map
            = new Dictionary<string, Dictionary<string, string>>() 
            { 
                #region Replays_AbilityName_Convert_Map
                {"bane",
                    new Dictionary<string, string>(){
                        {"恶魔之握", "魔爪"}
                    }
                },
                {"crystal_maiden",
                    new Dictionary<string, string>(){
                        {"秘法光环", "奥术光环"}
                    }
                },
                {"earthshaker",
                    new Dictionary<string, string>(){
                        {"加强图腾", "强化图腾"}
                    }
                },
                {"pudge",
                    new Dictionary<string, string>(){
                        {"堆积腐肉", "腐肉堆积"}
                    }
                },
                {"tiny",
                    new Dictionary<string, string>(){
                        {"长大！", "长大"}
                    }
                },
                {"windrunner",
                    new Dictionary<string, string>(){
                        {"火力聚焦", "集中火力"}
                    }
                },
                {"kunkka",
                    new Dictionary<string, string>(){
                        {"地图地标", "X标记"}
                    }
                },
                {"lich",
                    new Dictionary<string, string>(){
                        {"邪恶祭祀", "牺牲"}
                    }
                },
                {"lion",
                    new Dictionary<string, string>(){
                        {"法力汲取", "法力吸取"}
                    }
                },
                {"skeleton_king",
                    new Dictionary<string, string>(){
                        {"冥火暴击", "冥火爆击"},
                        {"致命一击", "殊死一搏"}
                    }
                },
                {"batrider",
                    new Dictionary<string, string>(){
                        {"黏性燃油", "粘性燃油"},
                        {"烈焰冲击波", "烈焰破击"}
                    }
                },
                {"ancient_apparition",
                    new Dictionary<string, string>(){
                        {"寒冰之触", "极寒之触"}
                    }
                },
                {"alchemist",
                    new Dictionary<string, string>(){
                        {"地精的贪婪", "贪魔的贪婪"}
                    }
                },
                {"lycan",
                    new Dictionary<string, string>(){
                        {"变狼", "变形"}
                    }
                },
                {"chaos_knight",
                    new Dictionary<string, string>(){
                        {"致命一击", "混沌一击"}
                    }
                },
                {"visage",
                    new Dictionary<string, string>(){
                        {"守墓人的斗篷", "陵卫斗篷"}
                    }
                },
                {"slark",
                    new Dictionary<string, string>(){
                        {"能量转换", "能量转移"}
                    }
                },
                {"magnataur",
                    new Dictionary<string, string>(){
                        {"獠牙冲刺", "巨角冲撞"}
                    }
                },
                {"shredder",
                    new Dictionary<string, string>(){
                        {"伐木链锯", "伐木锯链"}
                    }
                },
                {"tusk",
                    new Dictionary<string, string>(){
                        {"海象挥击", "海象神拳！"}
                    }
                },
                {"skywrath_mage",
                    new Dictionary<string, string>(){
                        {"秘法箭", "秘法鹰隼"}
                    }
                },
                {"abaddon",
                    new Dictionary<string, string>(){
                        {"霜寒之剑", "魔霭诅咒"}
                    }
                },
                {"elder_titan",
                    new Dictionary<string, string>(){
                        {"先祖之魂", "星体游魂"},
                        {"先祖之魂回归", "星体游魂回归"}
                    }
                },
                {"legion_commander",
                    new Dictionary<string, string>(){
                        {"压制", "强攻"}
                    }
                }
                #endregion
            };

        /// <summary>
        /// 英雄列表筛选 正则
        /// </summary>
        private static readonly string s_RegGetHeroHtml = @"<div\s+class=""heroIcons"">\s*(<a\s+id=""link_(?<key>[\w\-_]+)""[^>]*?href=""http://www.dota2.com/hero/(?<name>[\w\-_]+)/""[^>]*?>\s*<img[^>]*?/>\s*<img[^>]*?/>\s*</a>\s*)+?\s*</div>";
        /// <summary>
        /// 英雄详细-统计 正则
        /// </summary>
        private static readonly string s_RegGetHeroItemDataStatsHtml = @"<div\s+id=""overviewPrimaryStats"">(?<stats>[\s\S]*?)</div>\s*</div>\s*<div\s+id=""overviewHeroAbilities"">";
        /// <summary>
        /// 英雄详细-详细统计 正则
        /// </summary>
        private static readonly string s_RegGetHeroItemDataDetailStatsHtml = @"<h3>属性</h3>\s*<div\s+class=""redboxOuter"">(?<detailstats>[\s\S]*?)</div>\s*<h3>技能</h3>";
        /// <summary>
        /// 物品列表筛选 正则
        /// </summary>
        private static readonly string s_RegGetItemsHtml = @"<div\s+class=""shopColumn"">\s*<img\s+class=""shopColumnHeaderImg""\s+src=""[:/\.\w]+/apps/dota2/images/heropedia/itemcat_(?<key>[\w\-_]+)\.png""[\s\S]*?alt=""(?<name>[\w\-_]+)""\s+title=""[\w\-_]+""\s*/>\s*(<div[\s\S]*?itemname=""(?<itemkey>[\w\-_]+)""[^>]*?>\s*<img[^>]*?/>\s*</div>\s*)+?</div>";

        /// <summary>
        /// itemslist.json
        /// </summary>
        private static readonly string s_ToJsonFile_ItemsList = "itemslist.json";
        /// <summary>
        /// herolist.json
        /// </summary>
        private static readonly string s_ToJsonFile_HeroList = "herolist.json";
        /// <summary>
        /// {0}\hero-{1}.json
        /// </summary>
        private static readonly string s_ToJsonFile_HeroDetial = @"{0}\hero-{1}.json";

        /// <summary>
        /// Dota2 客户端资源中物品 keyname 转换到网站对应项 映射字典
        /// </summary>
        private static readonly Dictionary<string, string> s_Dota2Itembuilds_KeyName_Convert_Map = new Dictionary<string, string>() {
            {"gauntlet", "gauntlets"}, //力量手套
            {"assault_cuirass", "assault"}, //强袭胸甲
            {"shivas", "shivas_guard"}, //希瓦的守护
            {"circlet_of_nobility", "circlet"}, //圆环
            {"branch", "branches"}, //铁树枝干
            {"desolater", "desolator"}, //黯灭
            {"veil_of_discard", "veil_of_discord"}, //纷争面纱
        };

        #region GetHerosData Button
        /// <summary>
        /// Dota2 Itembuilds path browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBrowerDota2Itembuilds_Click(object sender, EventArgs e)
        {
            var browser = new FolderBrowserDialog();
            if (browser.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                this.TextBoxDota2Itembuilds.Text = browser.SelectedPath;
        }
        /// <summary>
        /// 英雄数据获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGetHeroData_Click(object sender, EventArgs e)
        {
            this.ClearMsg();

            if (this.CheckBoxHeroDetail.Checked
                && (string.IsNullOrEmpty(this.TextBoxDota2Itembuilds.Text)
                    || !Directory.Exists(this.TextBoxDota2Itembuilds.Text)))
            {
                Msg("----------dota2 itembuilds null.");
                return;
            }

            var heroDic = new Dictionary<string, HeroItem>();
            var http = new HttpClient();

            #region 英雄基础数据
            Msg("hero-get-base");
            var lurl = string.Format(s_GetHeroDataJsonUri, GetRandom()) + s_LSChinese;
            var eurl = string.Format(s_GetHeroDataJsonUri, GetRandom());
            //中文英雄数据
            var lHeroData = JObject.Parse(await http.GetStringAsync(lurl));
            Msg("hero-get-base-URL:{0}", lurl);
            //英文英雄数据
            var eHeroData = JObject.Parse(await http.GetStringAsync(eurl));
            Msg("hero-get-base-URL:{0}", eurl);

            if (lHeroData.Count > 0 && eHeroData.Count > 0
                && eHeroData.Count == lHeroData.Count)
            {
                foreach (var cLHeroItem in lHeroData)
                {
                    var cVLHeroItem = cLHeroItem.Value;
                    var cVEHeroItem = eHeroData[cLHeroItem.Key];

                    var tVHeroItem = new HeroItem()
                    {
                        key_name = cLHeroItem.Key,

                        name = cVEHeroItem["name"].Value<string>(),
                        //name = cVLHeroItem["name"].Value<string>(),
                        name_l = cVLHeroItem["name"].Value<string>(),

                        atk = cVLHeroItem["atk"].Value<string>(),
                        atk_l = cVLHeroItem["atk_l"].Value<string>(),

                        bio = cVEHeroItem["bio"].Value<string>(),
                        //bio = cVLHeroItem["bio"].Value<string>(),
                        bio_l = cVLHeroItem["bio"].Value<string>(),

                        roles = (from p in (JArray)cVLHeroItem["roles"] select p.Value<string>()).ToArray(),
                        roles_l = (from p in (JArray)cVLHeroItem["roles_l"] select p.Value<string>()).ToArray(),
                    };

                    heroDic[cLHeroItem.Key] = tVHeroItem;

                    Msg("hero-get-base-GET:{0}", tVHeroItem.key_name);
                }
            }
            Msg("hero-get-base-end");
            #endregion

            #region 英雄属性/阵营信息
            Msg("hero-get-HTML");
            var htmlUri = string.Format(s_GetHeroDataHtmlUri, GetRandom());
            var htmlReg = new Regex(s_RegGetHeroHtml, RegexOptions.IgnoreCase);
            var htmlContent = await http.GetStringAsync(htmlUri);

            var match = htmlReg.Match(htmlContent);
            int cMatchIndex = 0, tMatchIndex = 0;
            var cHP = "strength";//英雄属性:[1:strength[力量]/2:agility[敏捷]/3:intelligence[智力]]
            var cFaction = "radiant";//英雄阵营:[1:radiant[天辉]/2:dire[夜魇]]
            HeroItem cHeroItem = null;
            while (match.Success)
            {
                tMatchIndex = cMatchIndex;
                if (tMatchIndex > 2)
                    tMatchIndex = tMatchIndex - 3;

                if (tMatchIndex == 0)
                    cHP = "strength";
                else if (tMatchIndex == 1)
                    cHP = "agility";
                else if (tMatchIndex == 2)
                    cHP = "intelligence";
                else
                    throw new NotSupportedException();

                if (cMatchIndex > 2)
                    cFaction = "dire";

                var matchKey = match.Groups["key"].Captures;
                var matchName = match.Groups["name"].Captures;
                int cIndex = 0;
                foreach (Capture cCaptures in matchKey)
                {
                    cHeroItem = heroDic[cCaptures.Value];
                    cHeroItem.hp = cHP;
                    cHeroItem.faction = cFaction;
                    cHeroItem.keyUri_name = matchName[cIndex].Value;

                    LogHelper.LogDebug("Hero-cName-Key:{0}-Name:{1}", cCaptures, cHeroItem.keyUri_name);
                    cIndex++;
                }

                match = match.NextMatch();
                cMatchIndex++;
            }
            Msg("hero-get-HTML-end");
            #endregion

            #region 从 replays.net 获取英雄数据信息(别名/技能加点)
            await GetHeroDataFromReplays(heroDic);
            #endregion

            #region HeroDetail/Ability
            if (this.CheckBoxHeroDetail.Checked)
                await GetHeroDetailAndAbility(heroDic);
            #endregion

            #region ToJsonFile_HeroList
            var resDic = heroDic
                .ToDictionary(x => x.Key, x => new HeroSimpleItem()
                {
                    name = x.Value.name,
                    name_l = x.Value.name_l,
                    faction = x.Value.faction,
                    atk = x.Value.atk,
                    atk_l = x.Value.atk_l,
                    hp = x.Value.hp,
                    roles = x.Value.roles,
                    roles_l = x.Value.roles_l,
                    nickname_l = x.Value.nickname_l,
                    statsall = x.Value.statsall,
                });
            JsonUtil.SerializeToFile(resDic, s_ToJsonFile_HeroList);
            Msg("hero-get-save---------------------Count:{0}-", heroDic.Count);
            #endregion
            //LogHelper.LogDebug("ButtonGetHeroData_Click-{0}-\r\n{1}", heroDic, JsonUtil.Serialize(heroDic));
        }
        /// <summary>
        /// 从 replays.net 获取英雄数据信息(别名/技能加点)
        /// </summary>
        /// <param name="heroDic"></param>
        /// <returns></returns>
        private async Task GetHeroDataFromReplays(Dictionary<string, HeroItem> heroDic)
        {
            ThrowHelper.ThrowIfNull(heroDic, "heroDic");

            var http = new HttpClient();

            #region replays.net-HeroList
            Msg("hero-get-Replays_GetHeroData_List-begion");
            var lReplaysHeroDataHtmlBytes = await http.GetByteArrayAsync(s_Replays_GetHeroData_List_Uri);
            if (lReplaysHeroDataHtmlBytes != null && lReplaysHeroDataHtmlBytes.Length > 0)
            {
                var lReplaysHeroDataHtml_Reg = new Regex(s_Replays_GetHeroData_List_Reg, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var m = lReplaysHeroDataHtml_Reg.Match(s_GB2312Encoding.GetString(lReplaysHeroDataHtmlBytes));
                var tmKeyname = string.Empty;
                while (m.Success)
                {
                    //earth_spirit   earth
                    //ember_spirit	ember
                    HeroItem tHero = null;
                    tmKeyname = m.Groups["keyname"].Value;
                    if (tmKeyname == "earth") //大地之灵
                        tmKeyname = "earth_spirit";
                    else if (tmKeyname == "ember") //灰烬之灵
                        tmKeyname = "ember_spirit";
                    else if (tmKeyname == "legion") //军团指挥官
                        tmKeyname = "legion_commander";
                    else if (tmKeyname == "tb") //恐怖利刃
                        tmKeyname = "terrorblade";

                    if (!heroDic.TryGetValue(tmKeyname, out tHero))
                        Msg("********NULL:hero-get-Replays_GetHeroData_List-HTML-2-{0}", tmKeyname);
                    else
                    {
                        tHero.replays_id = m.Groups["id"].Value;
                        tHero.replays_pageName = m.Groups["pagename"].Value;
                    }

                    m = m.NextMatch();
                }
            }
            else
                Msg("********NULL:hero-get-Replays_GetHeroData_List-HTML-1");

            Msg("hero-get-Replays_GetHeroData_List-end");
            #endregion

            #region replays.net-HeroDetail JSON
            Msg("hero-get-Replays_GetHeroData_Detail-begion");
            foreach (var heroItem in heroDic)
            {
                var cVHeroItem = heroItem.Value;
                var replaysId = cVHeroItem.replays_id;
                if (!string.IsNullOrEmpty(replaysId))
                {
                    try
                    {
                        if (replaysId == "6") //魅惑魔女 适配(replays.net AJAX 加载异常)
                        {
                            #region 魅惑魔女 适配
                            cVHeroItem.nickname_l = new string[] { "AS", "Enchantress", "Aiushtha", "小鹿" };
                            //cVHeroItem.replays_id = "6";
                            //cVHeroItem.replays_pageName = "AS";
                            cVHeroItem.replays_skill = new Dictionary<int, ReplaysHeroSkillItem>()
                            {
                                {11, 
                                    new ReplaysHeroSkillItem()
                                    {
                                        Name = "不可侵犯",
                                        SkillID = 11,
                                        SouXie = "U"
                                    }
                                },
                                {12, 
                                    new ReplaysHeroSkillItem()
                                    {
                                        Name = "魅惑",
                                        SkillID = 12,
                                        SouXie = "C"
                                    }
                                },
                                {13, 
                                    new ReplaysHeroSkillItem()
                                    {
                                        Name = "自然之助",
                                        SkillID = 13,
                                        SouXie = "R"
                                    }
                                },
                                {14, 
                                    new ReplaysHeroSkillItem()
                                    {
                                        Name = "推进",
                                        SkillID = 14,
                                        SouXie = "T"
                                    }
                                }
                            };
                            #endregion
                        }
                        else
                        {
                            var tGetHeroData_Detail_Ajax_Uri = string.Format(s_Replays_GetHeroData_Detail_Uri, replaysId);
                            Msg("hero-get-Replays_GetHeroData_AJAX_Detail-{0}", tGetHeroData_Detail_Ajax_Uri);
                            var replaysHeroData_Detail_Ajax = JObject.Parse(await http.GetStringAsync(tGetHeroData_Detail_Ajax_Uri));

                            #region 昵称,别名
                            var tHeroSuoXie = replaysHeroData_Detail_Ajax["heroData"]["HeroSuoXie"].Value<string>();
                            if (replaysId == "66")
                                tHeroSuoXie += ",骷髅王";
                            cVHeroItem.nickname_l = tHeroSuoXie.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            #endregion

                            #region 技能列表
                            var tHeroSkillObj = replaysHeroData_Detail_Ajax["heroSkill"];
                            var tHeroSkillList = new Dictionary<int, ReplaysHeroSkillItem>();
                            var skillID = 0;
                            foreach (var skillItem in tHeroSkillObj)
                            {
                                skillID = skillItem.Value<int>("SkillID");
                                if (skillID <= 0)
                                {
                                    Msg("********NULL:hero-get-Replays_GetHeroData_Detail-ID-2-{0}-skillID==0", heroItem.Key);
                                    continue;
                                }

                                tHeroSkillList[skillID] = new ReplaysHeroSkillItem()
                                {
                                    SkillID = skillID,
                                    Name = skillItem.Value<string>("Name"),
                                    SouXie = skillItem.Value<string>("SouXie"),
                                };
                            }
                            cVHeroItem.replays_skill = tHeroSkillList;
                            #endregion
                        }

                        #region 技能加点
                        FixReplaysHeroDataSkillName(cVHeroItem);//适配 replays.net 技能名称

                        var tGetHeroData_Html_Detail_Uri = string.Format(s_Replays_GetHeroData_Html_Detail_Uri, cVHeroItem.replays_pageName);

                        Msg("hero-get-Replays_GetHeroData_HTML_Detail-{0}", tGetHeroData_Html_Detail_Uri);

                        var replaysHeroDataDetailHtmlBytes = await http.GetByteArrayAsync(tGetHeroData_Html_Detail_Uri);
                        if (replaysHeroDataDetailHtmlBytes != null && replaysHeroDataDetailHtmlBytes.Length > 0)
                        {
                            var replaysHeroDataDetailHtml_Reg = new Regex(s_Replays_GetHeroData_Detail_Skill_Reg, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            var m = replaysHeroDataDetailHtml_Reg.Match(s_GB2312Encoding.GetString(replaysHeroDataDetailHtmlBytes));
                            var skillup = new List<ReplaysAbilitySkillItem>();

                            while (m.Success)
                            {
                                var item = new ReplaysAbilitySkillItem()
                                {
                                    groupName = m.Groups["groupName"].Value,
                                    desc = ItemUtils.Common_FixBrHtml(m.Groups["desc"].Value),
                                    skillIDs = m.Groups["skillId"]
                                                    .Captures
                                                    .Cast<Capture>()
                                                    .Select(xx => Convert.ToInt32(xx.Value))
                                                    .ToList(),
                                };

                                if (item.skillIDs != null && item.skillIDs.Count > 0)
                                    skillup.Add(item);

                                m = m.NextMatch();
                            }
                            cVHeroItem.skillup = skillup.ToArray();
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Msg("********NULL:hero-get-Replays_GetHeroData_Detail-ID-1-{0}-EX:{1}", heroItem.Key, ex.Message);
                        ex = null;
                    }
                }
                else
                    Msg("********NULL:hero-get-Replays_GetHeroData_Detail-ID-2-{0}", heroItem.Key);
            }
            Msg("hero-get-Replays_GetHeroData_Detail-end");
            #endregion
        }
        /// <summary>
        /// 英雄详细/技能/英雄头像/技能图片 获取
        /// </summary>
        /// <param name="heroDic"></param>
        private async Task GetHeroDetailAndAbility(Dictionary<string, HeroItem> heroDic)
        {
            if (heroDic == null || heroDic.Count <= 0)
                return;

            var dota2ItembuildsPath = this.TextBoxDota2Itembuilds.Text;
            ThrowHelper.ThrowIfFalse(Directory.Exists(dota2ItembuildsPath), "dota2ItembuildsPath");

            var heroDetailDir = "hero_detail"; //英雄详细 JSON 保存路径
            var abilitiesFilesDir = "abilities_images"; //英雄技能 保存路径
            var heroesFilesDir = "heroes_images"; //英雄头像 保存路径
            var heroesIconFilesDir = "heroes_icons"; //英雄头像 ICON 标识 保存路径
            if (!Directory.Exists(heroDetailDir))
                Directory.CreateDirectory(heroDetailDir);
            if (!Directory.Exists(abilitiesFilesDir))
                Directory.CreateDirectory(abilitiesFilesDir);
            if (!Directory.Exists(heroesFilesDir))
                Directory.CreateDirectory(heroesFilesDir);
            if (!Directory.Exists(heroesIconFilesDir))
                Directory.CreateDirectory(heroesIconFilesDir);

            var http = new HttpClient();
            var abilityDic = new Dictionary<string, AbilityItem>();

            #region get-DetailAndAbility JSON
            Msg("hero-get-DetailAndAbility");
            var lurl = string.Format(s_GetAbilityDataJsonUri, GetRandom()) + s_LSChinese;
            var lAbilityData = JObject.Parse(await http.GetStringAsync(lurl));

            Msg("hero-get-DetailAndAbility-Ability-URL:{0}", lurl);
            //技能列表
            lAbilityData = (JObject)lAbilityData["abilitydata"];
            foreach (var cAbilityItem in lAbilityData)
            {
                var cVAbilityItem = cAbilityItem.Value;

                var tVAbilityItem = new AbilityItem()
                {
                    key_name = cAbilityItem.Key,

                    dname = cVAbilityItem["dname"].Value<string>(),
                    affects = ItemUtils.Common_FixBrHtml(cVAbilityItem["affects"].Value<string>()),
                    desc = ItemUtils.AbilityItem_FixDescField(cVAbilityItem["desc"].Value<string>()),
                    dmg = ItemUtils.Common_FixBrHtml(cVAbilityItem["dmg"].Value<string>()),
                    attrib = cVAbilityItem["attrib"].Value<string>(),
                    cmb = ItemUtils.AbilityItem_FixCmbField(cVAbilityItem["cmb"].Value<string>()),
                    lore = ItemUtils.Common_FixBrHtml(cVAbilityItem["lore"].Value<string>()),
                    hurl = cVAbilityItem["hurl"].Value<string>(),
                };

                abilityDic[cAbilityItem.Key] = tVAbilityItem;
                Msg("hero-get-DetailAndAbility-Ability-GET:{0}", tVAbilityItem.key_name);

                //下载技能图片
                if (this.CheckBoxHeroAbilityImage.Checked)
                {
                    var imgUri = string.Format(s_GetImageAbilitiesUri, cAbilityItem.Key);
                    lock (s_DownloadImageQueue)
                    {
                        s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUri, string.Format(@"{0}\{1}_hp1.png", abilitiesFilesDir, cAbilityItem.Key)));
                    }
                }
            }
            Msg("hero-get-DetailAndAbility-Ability-END");
            #endregion

            #region get-HeroItemDataDetail
            Dictionary<string, ItemsItem> itemsList = null;
            if (File.Exists(s_ToJsonFile_ItemsList))
                itemsList = JsonUtil.Deserialize<Dictionary<string, ItemsItem>>(File.ReadAllText(s_ToJsonFile_ItemsList));

            //s_RegGetHeroItemDataStatsHtml
            //s_RegGetHeroItemDataStatsHtml_Clear
            //s_RegGetHeroItemDataDetailStatsHtml
            var regStatsHtml = new Regex(s_RegGetHeroItemDataStatsHtml, RegexOptions.IgnoreCase);
            var regDetailStatsHtml = new Regex(s_RegGetHeroItemDataDetailStatsHtml, RegexOptions.IgnoreCase);
            foreach (var heroItem in heroDic)
            {
                Msg("hero-get-DetailAndAbility-Hero:{0}", heroItem.Key);

                var cVHeroItem = heroItem.Value;

                #region 英雄技能
                cVHeroItem.abilities = abilityDic
                                            .Where(x => x.Value.hurl == cVHeroItem.keyUri_name)
                                            .Select(x => x.Value)
                                            .ToArray();

                if (cVHeroItem.abilities != null && cVHeroItem.abilities.Length > 0
                    && cVHeroItem.replays_skill != null && cVHeroItem.replays_skill.Count > 0)
                {
                    //关联 replays 技能与 Dota2 官方技能
                    cVHeroItem.replays_skill.Values
                        .ToList().ForEach(x =>
                        {
                            var cSkill = cVHeroItem.abilities.FirstOrDefault(xx => xx.dname == x.Name);
                            if (cSkill != null)
                                x.key_name = cSkill.key_name;
                            else
                                Msg("********NULL:hero-get-DetailAndAbility-Hero-replays_skill/abilities:{0}-{1}", heroItem.Key, x);
                        });

                    //填充技能加点 技能 key_name
                    if (cVHeroItem.skillup != null && cVHeroItem.skillup.Length > 0)
                    {
                        Array.ForEach(cVHeroItem.skillup, suItem =>
                        {
                            suItem.abilityKeys = suItem.skillIDs.Select(siItem =>
                            {
                                if (siItem <= 0)
                                    return "attribute_bonus"; //黄点
                                else
                                    return cVHeroItem.replays_skill[siItem].key_name;
                            }).ToArray();
                        });
                    }
                }
                else
                {
                    Msg("********NULL:hero-get-DetailAndAbility-Hero-replays_skill/abilities:{0}", heroItem.Key);
                }
                #endregion

                #region 英雄详细信息
                try
                {
                    //英雄详细信息
                    var lHeroUrl = string.Format(s_GetHeroItemDataHtmlUri, cVHeroItem.keyUri_name, GetRandom()) + s_LSChinese;

                    Msg("hero-get-DetailAndAbility-Hero-URL:{0}", lHeroUrl);
                    var lHeroHtml = await http.GetStringAsync(lHeroUrl);
                    if (!string.IsNullOrEmpty(lHeroHtml))
                    {
                        //File.WriteAllText(string.Format(@"{0}\{1}.html", heroDetailDir, cVHeroItem.keyUri_name), lHeroHtml);

                        //--------统计信息
                        var match = regStatsHtml.Match(lHeroHtml);
                        if (match.Success)
                        {
                            string[][] stats1 = null;
                            cVHeroItem.stats = ItemUtils.HeroItem_FixStatsField(match.Groups["stats"].Value, out stats1);
                            cVHeroItem.stats1 = stats1;
                        }
                        else
                            Msg("********NULL:hero-get-DetailAndAbility-Hero-HTML-stats-1:{0}", heroItem.Key);

                        //--------详细统计信息
                        match = regDetailStatsHtml.Match(lHeroHtml);
                        if (match.Success)
                        {
                            cVHeroItem.detailstats = ItemUtils.Common_FixBrHtml(match.Groups["detailstats"].Value);
                            cVHeroItem.detailstats1 = ItemUtils.HeroItem_FixDetailstatsField(cVHeroItem.detailstats, 1);
                            cVHeroItem.detailstats2 = ItemUtils.HeroItem_FixDetailstatsField(cVHeroItem.detailstats, 2);
                        }
                        else
                            Msg("********NULL:hero-get-DetailAndAbility-Hero-HTML-detailstats-1:{0}", heroItem.Key);

                        //hp: "strength",
                        cVHeroItem.statsall = ItemUtils.HeroItem_FixStatsAllField(cVHeroItem.hp, cVHeroItem.stats1, cVHeroItem.detailstats1);
                    }
                    else
                    {
                        Msg("********NULL:hero-get-DetailAndAbility-Hero-HTML:{0}", heroItem.Key);
                    }

                    cVHeroItem.itembuilds = GetHeroDetailItembuilds(cVHeroItem.key_name, dota2ItembuildsPath);
                    //INFO: check itembuilds keyname
                    CheckHeroDetailItembuilds(itemsList, cVHeroItem.key_name, cVHeroItem.itembuilds);
                }
                catch (Exception ex)
                {
                    Msg("********EX:hero-get-DetailAndAbility-Hero:{0},EX:{1}", heroItem.Key, ex);
                }
                #endregion

                Msg("hero-get-DetailAndAbility-Hero-GET:{0}-abilities:{1}-----", cVHeroItem.key_name, cVHeroItem.abilities.Length);
                JsonUtil.SerializeToFile(cVHeroItem, string.Format(s_ToJsonFile_HeroDetial, heroDetailDir, heroItem.Key));
                Msg("hero-get-DetailAndAbility-Hero-GET:{0}-save-------", cVHeroItem.key_name);

                #region 下载英雄图片
                if (this.CheckBoxHeroImage.Checked)
                {
                    lock (s_DownloadImageQueue)
                    {
                        var imgUrl = string.Format(s_GetImageHeroesFullUri, heroItem.Key);
                        s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUrl, string.Format(@"{0}\{1}_full.png", heroesFilesDir, heroItem.Key)));

                        //INFO:完美官方 ICON 获取 keyname 差异适配
                        //噬魂鬼 life_stealer->life_lifestealer
                        //末日使者 doom_bringer->doom
                        //风行者 windrunner->windranger
                        var cnIconKeyName = heroItem.Key;
                        if (cnIconKeyName == "life_stealer")
                            cnIconKeyName = "life_lifestealer";
                        else if (cnIconKeyName == "doom_bringer")
                            cnIconKeyName = "doom";
                        else if (cnIconKeyName == "windrunner")
                            cnIconKeyName = "windranger";
                        imgUrl = string.Format(s_GetImageHeroesIconUri, cnIconKeyName);
                        s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUrl, string.Format(@"{0}\{1}_icon.png", heroesIconFilesDir, heroItem.Key)));

                        //imgUrl = string.Format(s_GetImageHeroesVertUri, heroItem.Key);
                        //s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUrl, string.Format(@"{0}\{1}_vert.jpg", heroesFilesDir, heroItem.Key)));

                        //imgUrl = string.Format(s_GetImageHeroesSBUri, heroItem.Key);
                        //s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUrl, string.Format(@"{0}\{1}_sb.png", heroesFilesDir, heroItem.Key)));

                        //imgUrl = string.Format(s_GetImageHeroesHPHoverUri, heroItem.Key);
                        //s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUrl, string.Format(@"{0}\{1}_hphover.png", heroesFilesDir, heroItem.Key)));
                    }
                }
                #endregion

                await Task.Delay(s_SysRandom.Next(50, 200));
            }
            #endregion

            #region 保存物品配置
            if (itemsList != null)
            {
                foreach (var itemsItem in itemsList.Values)
                {
                    if (itemsItem.toheroSets == null)
                        continue;
                    itemsItem.toheros = itemsItem.toheroSets.ToArray();
                }
                JsonUtil.SerializeToFile(itemsList, s_ToJsonFile_ItemsList);
            }
            #endregion

            Msg("hero-get-DetailAndAbility--save---------------------END-");
        }
        /// <summary>
        /// 获取英雄推荐装备
        /// </summary>
        /// <param name="heroKeyname"></param>
        /// <remarks>
        /// 从 Dota2 客户端资源加载
        /// </remarks>
        private IDictionary<string, string[]> GetHeroDetailItembuilds(string heroKeyname, string dota2ItembuildsPath)
        {
            ThrowHelper.ThrowIfNull(heroKeyname, "heroKeyname");
            ThrowHelper.ThrowIfNull(dota2ItembuildsPath, "dota2ItembuildsPath");

            var path = Path.Combine(dota2ItembuildsPath, string.Format("default_{0}.txt", heroKeyname));
            if (!File.Exists(path))
            {
                Msg("******---GetHeroDetailItembuilds-{0}-!Exists", path);
                return null;
            }

            try
            {
                var qmapScript = File.ReadAllText(path);
                if (qmapScript == null)
                    return null;

                var qmap = SimpleQMapParser.Parse(qmapScript);
                QMapObjectValue itemsElm = null;
                if (qmap == null
                    || (itemsElm = qmap.Value.ToObjectValue().Find("Items").ToObjectValue()) == null
                        || itemsElm.Value == null)
                {
                    Msg("******---GetHeroDetailItembuilds-{0}-qmap-NULL", path);
                    return null;
                }

                var result = new Dictionary<string, string[]>(4);
                foreach (var item in itemsElm.Value)
                {
                    var key = string.Empty;
                    if (item.Key == "#DOTA_Item_Build_Starting_Items")
                        key = "Starting";
                    else if (item.Key == "#DOTA_Item_Build_Early_Game")
                        key = "Early";
                    else if (item.Key == "#DOTA_Item_Build_Core_Items")
                        key = "Core";
                    else if (item.Key == "#DOTA_Item_Build_Luxury")
                        key = "Luxury";

                    var items = item.Value.ToObjectValue().Value
                                            .Select(x =>
                                            {
                                                var t = x.Value.Value;
                                                if (t.StartsWith("item_"))
                                                    t = t.Substring(5);
                                                if (t == "sheep") //INFO:亚巴顿数据错误
                                                    t = "sheepstick";
                                                return t;
                                            })
                                            .ToArray();

                    result[key] = items;
                }
                return result;
            }
            catch (Exception ex)
            {
                Msg("***********-GetHeroDetailItembuilds-EX:-{0}--{1}", path, ex);
            }
            return null;
        }
        /// <summary>
        /// 检查英雄推荐道具Key名称
        /// </summary>
        /// <param name="sysItemsList"></param>
        /// <param name="itembuilds"></param>
        /// <remarks>
        /// dota2 客户端资源内的 keyname, 有稍许不同
        /// </remarks>
        private void CheckHeroDetailItembuilds(Dictionary<string, ItemsItem> sysItemsList, string heroKeyName, IDictionary<string, string[]> itembuilds)
        {
            ThrowHelper.ThrowIfNull(heroKeyName, "heroKeyName");

            if (sysItemsList == null || itembuilds == null || sysItemsList.Count <= 0 || itembuilds.Count <= 0)
                return;

            Msg("---------CheckHeroDetailItembuilds-----------Hero:{0}---", heroKeyName);

            ItemsItem tItemsItem = null;
            foreach (var item in itembuilds)
            {
                for (int i = 0; i < item.Value.Length; i++)
                {
                    var cItems = item.Value[i];

                    cItems = GetFixedHeroDetailItembuildsName(cItems);

                    if (cItems != item.Value[i])
                        item.Value[i] = cItems;

                    if (!sysItemsList.ContainsKey(cItems))
                        Msg("****-Itembuilds-NULL-Hero:{0},Items:{1}", heroKeyName, cItems);
                    else
                    {
                        tItemsItem = null;
                        if (sysItemsList.TryGetValue(cItems, out tItemsItem) && tItemsItem != null)
                        {
                            if (tItemsItem.toheroSets == null)
                                tItemsItem.toheroSets = new HashSet<string>();

                            tItemsItem.toheroSets.Add(heroKeyName);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 适配 replays.net 技能名称
        /// </summary>
        /// <param name="cVHeroItem"></param>
        private void FixReplaysHeroDataSkillName(HeroItem cVHeroItem)
        {
            ThrowHelper.ThrowIfNull(cVHeroItem, "cVHeroItem");

            if (cVHeroItem.replays_skill == null)
                return;

            Dictionary<string, string> mapDic = null;
            if (!s_Replays_HeroDataAbilityName_Convert_Map.TryGetValue(cVHeroItem.key_name, out mapDic)
                || mapDic == null || mapDic.Count <= 0)
            {
                return;
            }

            string tName = null;
            foreach (var skillItem in cVHeroItem.replays_skill.Values)
            {
                if (mapDic.TryGetValue(skillItem.Name, out tName))
                    skillItem.Name = tName;
            }
        }
        /// <summary>
        /// 获取适配过的 Dota2 客户端资源内的 keyname
        /// </summary>
        /// <param name="itemName"></param>
        private string GetFixedHeroDetailItembuildsName(string itemName)
        {
            ThrowHelper.ThrowIfNull(itemName, "itemName");

            string tItemName = null;
            if (s_Dota2Itembuilds_KeyName_Convert_Map.TryGetValue(itemName, out tItemName))
                return tItemName;
            else
                return itemName;
        }
        #endregion

        #region GetItemsData Button
        /// <summary>
        /// 物品数据获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGetItemsData_Click(object sender, EventArgs e)
        {
            this.ClearMsg();

            var itemsDic = new Dictionary<string, ItemsItem>();
            var http = new HttpClient();
            var itemsFilesDir = "items_images";
            if (!Directory.Exists(itemsFilesDir))
                Directory.CreateDirectory(itemsFilesDir);

            #region json基础数据
            Msg("items-get-base");
            //json 数据
            var jsonUrl = string.Format(s_GetItemsDataJsonUri, GetRandom()) + s_LSChinese;
            var ejsonUrl = string.Format(s_GetItemsDataJsonUri, GetRandom());
            var jsonData = JObject.Parse(await http.GetStringAsync(jsonUrl));
            Msg("items-get-base-URL:{0}", jsonUrl);
            var ejsonData = JObject.Parse(await http.GetStringAsync(ejsonUrl));
            Msg("items-get-base-URL:{0}", ejsonUrl);

            if (jsonData.Count <= 0 || ejsonData.Count <= 0
                || jsonData.Count != ejsonData.Count)
                return;

            jsonData = (JObject)jsonData["itemdata"];
            ejsonData = (JObject)ejsonData["itemdata"];
            if (jsonData.Count <= 0 || ejsonData.Count <= 0
                || jsonData.Count != ejsonData.Count)
                return;

            JToken componentsJToken = null;
            foreach (var cItem in jsonData)
            {
                var cVItem = cItem.Value;
                var cVEItem = ejsonData[cItem.Key];


                var tVItem = new ItemsItem()
                {
                    key_name = cItem.Key,
                    attrib = cVItem["attrib"].Value<string>(),
                    img = cVItem["img"].Value<string>(),

                    dname = cVEItem["dname"].Value<string>(),
                    dname_l = cVItem["dname"].Value<string>(),

                    qual = cVItem["qual"].Value<string>(),

                    cost = cVItem["cost"].Value<int>(),
                    desc = ItemUtils.Common_FixBrHtml(cVItem["desc"].Value<string>()),
                    mc = cVItem["mc"].Value<string>(),
                    cd = cVItem["cd"].Value<int>(),
                    lore = cVItem["lore"].Value<string>(),

                    created = cVItem["created"].Value<bool>(),
                };

                var cKeyName = tVItem.key_name;
                var cCost = tVItem.cost;
                //INFO:官方'紫怨'价格错误
                if (cKeyName == "orchid" && cCost == 5025)
                    tVItem.cost = 4125;
                //INFO:官方二级'净魂之刃'价格错误
                if (cKeyName == "diffusal_blade_2" && cCost == 3300)
                    tVItem.cost = 4150;

                //INFO:官方'达贡之神力'价格错误(2720/3970/5220/6470/7720)
                if (cKeyName == "dagon" && cCost == 2800)
                    tVItem.cost = 2720;
                if (cKeyName.StartsWith("dagon_") && cCost == 2850)
                {
                    if (cKeyName == "dagon_2")
                        tVItem.cost = 3970;
                    else if (cKeyName == "dagon_3")
                        tVItem.cost = 5220;
                    else if (cKeyName == "dagon_4")
                        tVItem.cost = 6470;
                    else if (cKeyName == "dagon_5")
                        tVItem.cost = 7720;
                }
                //INFO:官方二级/三级'死灵书'价格错误(2700/3950/5200)
                if (cKeyName.StartsWith("necronomicon_") && cCost == 2700)
                {
                    if (cKeyName == "necronomicon_2")
                        tVItem.cost = 3950;
                    else if (cKeyName == "necronomicon_3")
                        tVItem.cost = 5200;
                }

                //填充合成配方
                componentsJToken = cVItem["components"];
                if (componentsJToken != null && componentsJToken is JArray)
                    tVItem.components = (from p in (JArray)componentsJToken select p.Value<string>()).ToArray();

                if (tVItem.mc == "False")
                    tVItem.mc = "";
                tVItem.qual_l = GetItemsQualL(tVItem.qual);

                itemsDic[cItem.Key] = tVItem;
                Msg("items-get-base-GET:{0}", tVItem.key_name);
            }
            Msg("items-get-base-end");
            //填充进阶合成配方/下载图片
            foreach (var item in itemsDic)
            {
                var cItemKey = item.Key;
                item.Value.tocomponents = (from p in itemsDic
                                           where p.Value.components != null && p.Value.components.Contains(cItemKey)
                                           select p.Key).ToArray();
                if (item.Value.tocomponents.Length <= 0)
                    item.Value.tocomponents = null;

                if (CheckBoxItemsImage.Checked)
                {
                    var imgUrl = string.Format(s_GetImageItemsLGUri, item.Key);
                    lock (s_DownloadImageQueue)
                    {
                        s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUrl, string.Format(@"{0}\{1}_lg.png", itemsFilesDir, item.Key)));
                    }
                }
            }
            Msg("items-get-base-tocomponents-end");
            #endregion

            #region HTML数据,填充分类等信息
            Msg("items-get-HTML");
            var htmlUrl = string.Format(s_GetItemsDataHtmlUri, GetRandom()) + s_LSChinese;
            var htmlContent = await http.GetStringAsync(htmlUrl);

            var htmlReg = new Regex(s_RegGetItemsHtml, RegexOptions.IgnoreCase);
            var match = htmlReg.Match(htmlContent);
            int cMatchIndex = 0;
            string itembasecat = "basics";
            ItemsItem cItemsItem = null;
            while (match.Success)
            {
                if (cMatchIndex <= 3)
                    itembasecat = "basics";
                else if (cMatchIndex > 3 && cMatchIndex <= 9)
                    itembasecat = "upgrades";
                else
                    itembasecat = "secrets";

                var matchKey = match.Groups["key"].Value;
                var matchName = match.Groups["name"].Value;
                //if(cMatchIndex)
                var matchItemKeys = match.Groups["itemkey"].Captures;
                foreach (Capture cCaptures in matchItemKeys)
                {
                    cItemsItem = itemsDic[cCaptures.Value];
                    cItemsItem.ispublic = true; //是否物品列表显示
                    cItemsItem.itembasecat = itembasecat;
                    cItemsItem.itemcat = matchKey;
                    cItemsItem.itemcat_l = matchName;

                    LogHelper.LogDebug("Items-cName-Key:{0}-Name:{1}", cCaptures, cItemsItem);
                }
                match = match.NextMatch();
                cMatchIndex++;
            }
            Msg("items-get-HTML-end");
            #endregion

            //var resDic = itemsDic.Where(x => !string.IsNullOrEmpty(x.Value.itemcat))
            //                     .ToDictionary(x => x.Key, x => x.Value);
            JsonUtil.SerializeToFile(itemsDic, s_ToJsonFile_ItemsList);

            Msg("items-get-save---------------------Count:{0}-", itemsDic.Count);
        }
        /// <summary>
        /// 道具品质
        /// </summary>
        /// <param name="qual"></param>
        /// <returns></returns>
        private string GetItemsQualL(string qual)
        {
            if (qual == "component")
                return "组件";
            else if (qual == "common")
                return "普通";
            else if (qual == "consumable")
                return "消耗品";
            else if (qual == "secret_shop")
                return "神秘商店";
            else if (qual == "rare")
                return "稀有";
            else if (qual == "epic")
                return "史诗";
            else if (qual == "artifact")
                return "圣物";
            else
                return null;
        }
        #endregion

        #region GetHeroRnData Button
        /// <summary>
        /// 英雄昵称数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGetHeroNnData_Click(object sender, EventArgs e)
        {
            //Dictionary<string, HeroSimpleItem> heroList = null;
            //if (File.Exists(s_ToJsonFile_HeroList))
            //    heroList = JsonUtil.Deserialize<Dictionary<string, HeroSimpleItem>>(File.ReadAllText(s_ToJsonFile_HeroList));
            //else
            //{
            //    Msg("********EX:hero-get-NnData-HeroList-FileNotFound-File:{0}", s_ToJsonFile_HeroList);
            //    return;
            //}
            ////1.replays_英雄列表 http://dota2.replays.net/hero/
            ////2.replays_英雄详细数据 http://dota2.replays.net/hero/services/heroData.ashx?heroID=11

            //JsonUtil.SerializeToFile(heroList, s_ToJsonFile_HeroList);
        }
        #endregion

        #region Utility
        /// <summary>
        /// 随即发生器
        /// </summary>
        /// <returns></returns>
        private static int GetRandom()
        {
            return s_SysRandom.Next(99999, 999999999);
        }

        #region SaveJpg
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pngBuffer"></param>
        /// <param name="?"></param>
        private static void ResaveJpgFile(byte[] imgBuffer, string savePath)
        {
            if (imgBuffer == null || imgBuffer.Length <= 0)
                return;

            ThrowHelper.ThrowIfNull(savePath, "savePath");

            var ext = Path.GetExtension(savePath).ToLower();
            if (ext == ".jpg" || ext == ".jpeg")
                File.WriteAllBytes(savePath, imgBuffer);
            else
            {
                savePath = savePath.Substring(0, savePath.Length - ext.Length) + ".jpg";
                using (var bmp = Bitmap.FromStream(new MemoryStream(imgBuffer)))
                {
                    SaveJpeg(bmp, savePath, 86L);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetImageEncoder(ImageFormat format)
        {
            var guid = format.Guid;
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(x => x.FormatID == guid);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img"></param>
        /// <param name="filePath"></param>
        /// <param name="quality"></param>
        private static void SaveJpeg(Image img, string filePath, long quality)
        {
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            img.Save(filePath, GetImageEncoder(ImageFormat.Jpeg), encoderParameters);
        }
        #endregion

        #region Msg
        /// <summary>
        /// 
        /// </summary>
        private void ClearMsg()
        {
            this.ListBoxMsg.BeginInvoke(new Action(() =>
            {
                this.ListBoxMsg.Items.Clear();
            }));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        private void Msg(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return;

            this.ListBoxMsg.BeginInvoke(new Action(() =>
            {
                this.ListBoxMsg.Items.Add(msg ?? string.Empty);
            }));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        private void Msg(string format, params object[] args)
        {
            Msg(string.Format(format, args));
        }
        #endregion
        #endregion

        #region Download Image
        /// <summary>
        /// 下载图片并保存到指定路径
        /// </summary>
        /// <param name="http"></param>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        /// <remarks>
        /// 如果图片已经存在不下载
        /// </remarks>
        private static async Task<bool> DownloadImage(HttpClient http, string url, string savePath)
        {
            ThrowHelper.ThrowIfNull(http, "http");
            ThrowHelper.ThrowIfNull(url, "url");
            ThrowHelper.ThrowIfNull(savePath, "savePath");

            var ext = Path.GetExtension(savePath).ToLower();
            var tsavePath = savePath.Substring(0, savePath.Length - ext.Length) + ".jpg";
            if (File.Exists(tsavePath))
                return true;

            var imgBytes = await http.GetByteArrayAsync(url);
            if (imgBytes != null && imgBytes.Length > 0)
            {
                //File.WriteAllBytes(savePath, imgBytes);
                ResaveJpgFile(imgBytes, savePath);
                await Task.Delay(s_SysRandom.Next(10, 100));

                return true;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="http"></param>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        private static bool DownloadImageSync(HttpClient http, string url, string savePath)
        {
            return Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("DownloadImageSync-{0}-{1}", url, savePath);

                    return await DownloadImage(http, url, savePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("***--DownloadImageSync-{0}-{1}\r\n{2}", url, savePath, ex);
                    return false;
                }
            }).Result;
        }

        private static DownloadImageThreadQueue s_DownloadImageQueue = new DownloadImageThreadQueue(int.MaxValue / 10);
        /// <summary>
        /// 统计提交队列
        /// </summary>
        private class DownloadImageThreadQueue : ConcurrentConsumerQueue<Tuple<HttpClient, string, string>>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="maxQueueLength"></param>
            /// <param name="threadSleepMilliseconds"></param>
            public DownloadImageThreadQueue(int maxQueueLength)
            {
            }
            /// <summary>
            /// 处理
            /// </summary>
            /// <param name="item">[http, url, savePath]</param>
            protected override void Process(Tuple<HttpClient, string, string> item)
            {
                DownloadImageSync(item.Item1, item.Item2, item.Item3);
            }
        }
        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    static class ItemUtils
    {
        private static Regex s_RegAbilityItemCmb = new Regex(@"<div\s+class=""(?<type>mana|cooldown)"">\s*<img\s+alt=""(?<alt>.*?)""[^>]+?/>\s*(?<value>[\s\S]+?)\s*</div>", RegexOptions.IgnoreCase);
        private static Regex s_RegAbilityItemDesc = new Regex(@"color='(\#[\w]{6})'", RegexOptions.IgnoreCase);
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
        /// 
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
        /// 
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
