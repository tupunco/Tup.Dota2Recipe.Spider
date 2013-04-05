﻿using System;
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
        private static readonly string s_Dota2SteamMedia = "http://media.steampowered.com/apps/dota2/images";

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
        private static readonly string s_RegGetHeroItemDataDetailStatsHtml = @"<h3>Stats</h3>\s*<div\s+class=""redboxOuter"">(?<detailstats>[\s\S]*?)</div>\s*<h3>Abilities</h3>";
        /// <summary>
        /// 物品列表筛选 正则
        /// </summary>
        private static readonly string s_RegGetItemsHtml = @"<div\s+class=""shopColumn"">\s*<img\s+class=""shopColumnHeaderImg""\s+src=""http://media.steampowered.com/apps/dota2/images/heropedia/itemcat_(?<key>[\w\-_]+)\.png""[\s\S]*?alt=""(?<name>[\w\-_]+)""\s+title=""[\w\-_]+""\s*/>\s*(<div[\s\S]*?itemname=""(?<itemkey>[\w\-_]+)""[^>]*?>\s*<img[^>]*?/>\s*</div>\s*)+?</div>";

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
                    roles_l = x.Value.roles_l
                });
            JsonUtil.SerializeToFile(resDic, "herolist.json");
            Msg("hero-get-save---------------------Count:{0}-", heroDic.Count);

            if (this.CheckBoxHeroDetail.Checked)
                GetHeroDetailAndAbility(heroDic);
            //LogHelper.LogDebug("ButtonGetHeroData_Click-{0}-\r\n{1}", heroDic, JsonUtil.Serialize(heroDic));
        }
        /// <summary>
        /// 英雄详细/技能/英雄头像/技能图片 获取
        /// </summary>
        /// <param name="heroDic"></param>
        private async void GetHeroDetailAndAbility(Dictionary<string, HeroItem> heroDic)
        {
            if (heroDic == null || heroDic.Count <= 0)
                return;

            var dota2ItembuildsPath = this.TextBoxDota2Itembuilds.Text;
            ThrowHelper.ThrowIfFalse(Directory.Exists(dota2ItembuildsPath), "dota2ItembuildsPath");
            Dictionary<string, ItemsItem> itemsList = null;
            if (File.Exists("itemslist.json"))
                itemsList = JsonUtil.Deserialize<Dictionary<string, ItemsItem>>(File.ReadAllText("itemslist.json"));

            var heroDetailDir = "hero_detail";
            var abilitiesFilesDir = "abilities_images";
            var heroesFilesDir = "heroes_images";
            if (!Directory.Exists(heroDetailDir))
                Directory.CreateDirectory(heroDetailDir);
            if (!Directory.Exists(abilitiesFilesDir))
                Directory.CreateDirectory(abilitiesFilesDir);
            if (!Directory.Exists(heroesFilesDir))
                Directory.CreateDirectory(heroesFilesDir);

            var http = new HttpClient();
            var abilityDic = new Dictionary<string, AbilityItem>();

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

            //s_RegGetHeroItemDataStatsHtml
            //s_RegGetHeroItemDataStatsHtml_Clear
            //s_RegGetHeroItemDataDetailStatsHtml
            var regStatsHtml = new Regex(s_RegGetHeroItemDataStatsHtml, RegexOptions.IgnoreCase);
            var regDetailStatsHtml = new Regex(s_RegGetHeroItemDataDetailStatsHtml, RegexOptions.IgnoreCase);
            //英雄技能列表
            foreach (var heroItem in heroDic)
            {
                Msg("hero-get-DetailAndAbility-Hero:{0}", heroItem.Key);

                var cVHeroItem = heroItem.Value;
                cVHeroItem.abilities = abilityDic
                                            .Where(x => x.Value.hurl == cVHeroItem.keyUri_name)
                                            .Select(x => x.Value)
                                            .ToArray();

                #region 英雄详细信息
                try
                {
                    //英雄详细信息
                    var lHeroUrl = string.Format(s_GetHeroItemDataHtmlUri, cVHeroItem.keyUri_name, GetRandom()) + s_LSChinese;

                    Msg("hero-get-DetailAndAbility-Hero-URL:{0}", lHeroUrl);
                    var lHeroHtml = await http.GetStringAsync(lHeroUrl);
                    if (!string.IsNullOrEmpty(lHeroHtml))
                    {
                        File.WriteAllText(string.Format(@"{0}\{1}.html", heroDetailDir, cVHeroItem.keyUri_name), lHeroHtml);

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
                            cVHeroItem.detailstats = match.Groups["detailstats"].Value;
                            cVHeroItem.detailstats1 = ItemUtils.HeroItem_FixDetailstatsField(cVHeroItem.detailstats, 1);
                            cVHeroItem.detailstats2 = ItemUtils.HeroItem_FixDetailstatsField(cVHeroItem.detailstats, 2);
                        }
                        else
                            Msg("********NULL:hero-get-DetailAndAbility-Hero-HTML-detailstats-1:{0}", heroItem.Key);
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
                JsonUtil.SerializeToFile(cVHeroItem, string.Format(@"{0}\hero-{1}.json", heroDetailDir, heroItem.Key));
                Msg("hero-get-DetailAndAbility-Hero-GET:{0}-save-------", cVHeroItem.key_name);

                #region 下载英雄图片
                if (this.CheckBoxHeroImage.Checked)
                {
                    lock (s_DownloadImageQueue)
                    {
                        var imgUrl = string.Format(s_GetImageHeroesFullUri, heroItem.Key);
                        s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUrl, string.Format(@"{0}\{1}_full.png", heroesFilesDir, heroItem.Key)));

                        imgUrl = string.Format(s_GetImageHeroesVertUri, heroItem.Key);
                        s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUrl, string.Format(@"{0}\{1}_vert.jpg", heroesFilesDir, heroItem.Key)));

                        imgUrl = string.Format(s_GetImageHeroesSBUri, heroItem.Key);
                        s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUrl, string.Format(@"{0}\{1}_sb.png", heroesFilesDir, heroItem.Key)));

                        imgUrl = string.Format(s_GetImageHeroesHPHoverUri, heroItem.Key);
                        s_DownloadImageQueue.Enqueue(Tuple.Create(http, imgUrl, string.Format(@"{0}\{1}_hphover.png", heroesFilesDir, heroItem.Key)));
                    }
                }
                #endregion

                await Task.Delay(s_SysRandom.Next(50, 200));
            }

            Msg("hero-get-DetailAndAbility--save---------------------END-");
        }
        /// <summary>
        /// 获取英雄推荐装备
        /// </summary>
        /// <param name="heroKeyname"></param>
        /// <remarks>
        /// 从dota2客户端资源加载
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
        /// dot2 客户端资源内的keyname, 有稍许不同
        /// </remarks>
        private void CheckHeroDetailItembuilds(Dictionary<string, ItemsItem> sysItemsList, string heroKeyName, IDictionary<string, string[]> itembuilds)
        {
            ThrowHelper.ThrowIfNull(heroKeyName, "heroKeyName");

            if (sysItemsList == null || itembuilds == null || sysItemsList.Count <= 0 || itembuilds.Count <= 0)
                return;

            Msg("---------CheckHeroDetailItembuilds-----------Hero:{0}---", heroKeyName);

            //--------------------------
            //gauntlet:	gauntlets
            //assault_cuirass:	assault
            //shivas:		shivas_guard
            foreach (var item in itembuilds)
            {
                for (int i = 0; i < item.Value.Length; i++)
                {
                    var cItems = item.Value[i];
                    if (cItems == "gauntlet")
                        cItems = "gauntlets";
                    else if (cItems == "assault_cuirass")
                        cItems = "assault";
                    else if (cItems == "shivas")
                        cItems = "shivas_guard";

                    if (cItems != item.Value[i])
                        item.Value[i] = cItems;

                    if (!sysItemsList.ContainsKey(cItems))
                        Msg("****-Itembuilds-NULL-Hero:{0},Items:{1}", heroKeyName, cItems);
                }
            }
        }
        /// <summary>
        /// 物品数据获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGetItemsData_Click(object sender, EventArgs e)
        {
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
            JsonUtil.SerializeToFile(itemsDic, "itemslist.json");

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

        /// <summary>
        /// 随即发生器
        /// </summary>
        /// <returns></returns>
        private static int GetRandom()
        {
            return s_SysRandom.Next(99999, 999999999);
        }

        private static DownloadImageThreadQueue s_DownloadImageQueue = new DownloadImageThreadQueue(int.MaxValue / 10);
        /// <summary>
        /// 统计提交队列
        /// </summary>
        private class DownloadImageThreadQueue : SingleThreadQueue<Tuple<HttpClient, string, string>>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="maxQueueLength"></param>
            /// <param name="threadSleepMilliseconds"></param>
            public DownloadImageThreadQueue(int maxQueueLength)
                : base(maxQueueLength)
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
        private static Regex s_RegCommonRN = new Regex(@"[\r\n]+", RegexOptions.IgnoreCase);
        private static Regex s_RegHeroItemStats = new Regex(@"<img\s+title=""(?<alt>[\w]+)""[^>]*/>\s*<div\s+class=""overview_StatVal""\s+id=""overview_(?<type>[\w]+?)Val"">\s*(?<value>[\s\S]+?)\s*</div>", RegexOptions.IgnoreCase);
        private static Regex s_RegHeroItemDetailstats1 = new Regex(@"<div\s+class=""statRowB?"">\s*(?:<div\s+class=""statRowCol2?W"">(?<value>[^<]+?)</div>\s*){3}\s*(?<type>[^<]+?)\s*</div>", RegexOptions.IgnoreCase);
        private static Regex s_RegHeroItemDetailstats2 = new Regex(@"<div\s+class=""statRowB?"">\s*(?:<div\s+class=""statRowCol2?W"">(?<value>[^<]+?)</div>\s*){1}\s*(?<type>[^<]+?)\s*</div>", RegexOptions.IgnoreCase);
        /// <summary>
        /// 
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
        /// 
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
