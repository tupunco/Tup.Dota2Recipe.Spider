using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Tup.Dota2Recipe.Spider.Entity
{
    /// <summary>
    /// Dota2 客户端资源 物品 实体
    /// </summary>
    public class Dota2ResItemsItem
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        [JsonIgnore]
        public string key_name { get; set; }

        public string ID { get; set; }

        public string AbilityBehavior { get; set; }
        [JsonIgnore]
        public string[] AbilityBehavior2 { get; set; }

        public string AbilityCastRange { get; set; }
        public string AbilityCastPoint { get; set; }
        public string AbilityCooldown { get; set; }
        public string AbilityManaCost { get; set; }

        public string ItemCost { get; set; }
        [JsonIgnore]
        public int ItemCost2 { get; set; }

        public string ItemShopTags { get; set; }
        public string[] ItemShopTags2 { get; set; }

        public string ItemQuality { get; set; }

        public string ItemAliases { get; set; }
        public string[] ItemAliases2 { get; set; }

        public string SideShop { get; set; }

        public string ItemDeclarations { get; set; }
        [JsonIgnore]
        public string[] ItemDeclarations2 { get; set; }

        public string AbilityUnitTargetType { get; set; }
        public string AbilityUnitTargetTeam { get; set; }
        //public string Model { get; set; }
        //public string UIPickupSound { get; set; }
        //public string UIDropSound { get; set; }
        //public string WorldDropSound { get; set; }
        public string ItemShareability { get; set; }
        //public string Effect { get; set; }
        public string ItemSellable { get; set; }
        public string ItemStockMax { get; set; }
        public string ItemStockTime { get; set; }
        public string ItemSupport { get; set; }
        public string ItemKillable { get; set; }
        public string ItemContributesToNetWorthWhenDropped { get; set; }
        public string FightRecapLevel { get; set; }
        public string ItemPurchasable { get; set; }
        public string ItemStackable { get; set; }
        public string ItemPermanent { get; set; }
        public string ItemInitialCharges { get; set; }
        public string AbilitySharedCooldown { get; set; }
        public string ItemRequiresCharges { get; set; }
        public string ItemDisplayCharges { get; set; }
        public string ItemRecipe { get; set; }
        public string ItemResult { get; set; }
        public string AbilityUnitTargetFlags { get; set; }
        public string ItemStockInitial { get; set; }
        public string AbilityChannelTime { get; set; }
        public string SecretShop { get; set; }
        public string ItemDisassembleRule { get; set; }
        public string ItemAlertable { get; set; }
        public string MaxUpgradeLevel { get; set; }
        public string ItemBaseLevel { get; set; }
        public string UpgradesItems { get; set; }
        public string UpgradeRecipe { get; set; }
        public string ItemDroppable { get; set; }
        public string InvalidHeroes { get; set; }
        public string ItemHideCharges { get; set; }
        public string AbilityUnitDamageType { get; set; }
        public string AbilityDamage { get; set; }
        public string AbilityDuration { get; set; }
        public string AbilityModifierSupportValue { get; set; }
        public string ItemCastOnPickup { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public JObject[] AbilitySpecial { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string> AbilitySpecial2 { get; set; }

        /// <summary>
        /// 合成需要物品
        /// </summary>
        public string[] ItemRequirements { get; set; }
        /// <summary>
        /// 合成需要物品
        /// </summary>
        [JsonIgnore]
        public string[] ItemRequirements2 { get; set; }
    }

    /// <summary>
    /// Dota2 客户端资源 英雄 实体
    /// </summary>
    public class Dota2ResNpcHeroesItem
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        [JsonIgnore]
        public string key_name { get; set; }

        public string BaseClass { get; set; }
        //public string Model { get; set; }
        //public string SoundSet { get; set; }
        public string Enabled { get; set; }
        public string Level { get; set; }
        public string BotImplemented { get; set; }
        public string NewHero { get; set; }
        public string HeroPool1 { get; set; }
        public string HeroUnlockOrder { get; set; }
        public string CMEnabled { get; set; }
        public string CMTournamentIgnore { get; set; }
        public string ArmorPhysical { get; set; }
        public string MagicalResistance { get; set; }
        /// <summary>
        /// 攻击类型
        /// </summary>
        /// <remarks>
        /// DOTA_UNIT_CAP_RANGED_ATTACK 远程
        /// DOTA_UNIT_CAP_MELEE_ATTACK  近战
        /// </remarks>
        public string AttackCapabilities { get; set; }
        public string AttackDamageMin { get; set; }
        public string AttackDamageMax { get; set; }
        public string AttackDamageType { get; set; }
        public string AttackRate { get; set; }
        public string AttackAnimationPoint { get; set; }
        public string AttackAcquisitionRange { get; set; }
        public string AttackRange { get; set; }
        public string ProjectileModel { get; set; }
        public string ProjectileSpeed { get; set; }
        /// <summary>
        /// 主属性
        /// </summary>
        /// <remarks>
        /// DOTA_ATTRIBUTE_AGILITY
        /// DOTA_ATTRIBUTE_INTELLECT
        /// DOTA_ATTRIBUTE_STRENGTH
        /// </remarks>
        public string AttributePrimary { get; set; }
        public string AttributeBaseStrength { get; set; }
        public string AttributeStrengthGain { get; set; }
        public string AttributeBaseIntelligence { get; set; }
        public string AttributeIntelligenceGain { get; set; }
        public string AttributeBaseAgility { get; set; }
        public string AttributeAgilityGain { get; set; }
        public string BountyXP { get; set; }
        public string BountyGoldMin { get; set; }
        public string BountyGoldMax { get; set; }
        //public string BoundsHullName { get; set; }
        public string RingRadius { get; set; }
        public string MovementCapabilities { get; set; }
        public string MovementSpeed { get; set; }
        public string MovementTurnRate { get; set; }
        public string HasAggressiveStance { get; set; }
        public string StatusHealth { get; set; }
        public string StatusHealthRegen { get; set; }
        public string StatusMana { get; set; }
        public string StatusManaRegen { get; set; }
        public string TeamName { get; set; }
        public string CombatClassAttack { get; set; }
        public string CombatClassDefend { get; set; }
        public string UnitRelationshipClass { get; set; }
        public string VisionDaytimeRange { get; set; }
        public string VisionNighttimeRange { get; set; }
        public string HasInventory { get; set; }
        public string VoiceBackgroundSound { get; set; }
        public string HealthBarOffset { get; set; }
        //public string IdleExpression { get; set; }
        //public string IdleSoundLoop { get; set; }
        public string AbilityDraftDisabled { get; set; }
        public string ARDMDisabled { get; set; }
        public string HeroID { get; set; }

        public string Role { get; set; }
        public string[] Role2 { get; set; }

        public string Rolelevels { get; set; }
        /// <summary>
        /// 阵营
        /// </summary>
        /// <remarks>
        /// Bad
        /// Good
        /// </remarks>
        public string Team { get; set; }
        //public string Portrait { get; set; }
        //public string ModelScale { get; set; }
        //public string HeroGlowColor { get; set; }
        //public string PickSound { get; set; }
        //public string BanSound { get; set; }
        /// <summary>
        /// 英雄缩写
        /// </summary>
        public string NameAliases { get; set; }
        public string[] NameAliases2 { get; set; }

        public string url { get; set; }
        public string LastHitChallengeRival { get; set; }
        //public string HeroSelectSoundEffect { get; set; }
        //public string ParticleFile { get; set; }
        //public string GameSoundsFile { get; set; }
        //public string VoiceFile { get; set; }
        public string LoadoutScale { get; set; }
        public string NoCombine { get; set; }
        //public string Press { get; set; }
        //public string BotForceSelection { get; set; }
        public string ForceEnable { get; set; }

        public string new_player_enable { get; set; }

        public string AbilityLayout { get; set; }
        public string Ability1 { get; set; }
        public string Ability2 { get; set; }
        public string Ability3 { get; set; }
        public string Ability4 { get; set; }
        public string Ability5 { get; set; }
        public string Ability6 { get; set; }
        public string Ability7 { get; set; }
        public string Ability8 { get; set; }
        public string Ability9 { get; set; }
        public string Ability10 { get; set; }
        public string Ability11 { get; set; }
        public string Ability12 { get; set; }
        public string Ability13 { get; set; }
        public string Ability14 { get; set; }
        public string Ability15 { get; set; }
        public string Ability16 { get; set; }

        /// <summary>
        /// 技能列表
        /// </summary>
        [JsonIgnore]
        public string[] Abilitys2 { get; set; }
        //public Object HUD { get; set; }
        //public Object RenderablePortrait { get; set; }
        //public Array ItemSlots { get; set; }
        //public Object Bot { get; set; }
        //public Object AbilityPreview { get; set; }
        //public Object precache { get; set; }
        //public Object animation_transitions { get; set; }
    }

    /// <summary>
    /// Dota2 客户端资源 技能 实体
    /// </summary>
    public class Dota2ResNpcAbilitiesItem
    {
        /// <summary>
        /// 索引名称
        /// </summary>
        [JsonIgnore]
        public string key_name { get; set; }

        public string ID { get; set; }
        public string AbilityType { get; set; }

        public string AbilityBehavior { get; set; }
        [JsonIgnore]
        public string[] AbilityBehavior2 { get; set; }

        public string OnCastbar { get; set; }
        public string OnLearnbar { get; set; }
        public string FightRecapLevel { get; set; }
        public string AbilityCastRange { get; set; }
        public string AbilityCastRangeBuffer { get; set; }
        public string AbilityCastPoint { get; set; }
        public string AbilityChannelTime { get; set; }
        public string AbilityCooldown { get; set; }
        public string AbilityDuration { get; set; }
        public string AbilitySharedCooldown { get; set; }
        public string AbilityDamage { get; set; }
        public string AbilityManaCost { get; set; }
        public string AbilityModifierSupportValue { get; set; }
        public string AbilityModifierSupportBonus { get; set; }
        public string ItemCost { get; set; }
        public string ItemInitialCharges { get; set; }
        public string ItemCombinable { get; set; }
        public string ItemPermanent { get; set; }
        public string ItemStackable { get; set; }
        public string ItemRecipe { get; set; }
        public string ItemDroppable { get; set; }
        public string ItemPurchasable { get; set; }
        public string ItemSellable { get; set; }
        public string ItemRequiresCharges { get; set; }
        public string ItemKillable { get; set; }
        public string ItemDisassemblable { get; set; }
        public string ItemShareability { get; set; }
        public string ItemDeclaresPurchase { get; set; }
        public string AbilityUnitDamageType { get; set; }
        /// <summary>
        /// 是否无视魔法免疫
        /// </summary>
        /// <remarks>
        /// SPELL_IMMUNITY_ENEMIES_NO
        /// SPELL_IMMUNITY_ENEMIES_YES
        /// </remarks>
        public string SpellImmunityType { get; set; }
        public string AbilityUnitTargetTeam { get; set; }
        /// <summary>
        /// 技能目标类型
        /// </summary>
        /// <remarks>
        /// DOTA_UNIT_TARGET_HERO
        /// DOTA_UNIT_TARGET_BASIC
        /// </remarks>
        public string AbilityUnitTargetType { get; set; }
        [JsonIgnore]
        public string[] AbilityUnitTargetType2 { get; set; }
        /// <summary>
        /// 技能目标类型 标识
        /// </summary>
        /// <remarks>
        /// DOTA_UNIT_TARGET_FLAG_MAGIC_IMMUNE_ENEMIES
        /// DOTA_UNIT_TARGET_FLAG_NOT_ANCIENTS
        /// </remarks>
        public string AbilityUnitTargetFlags { get; set; }
        [JsonIgnore]
        public string[] AbilityUnitTargetFlags2 { get; set; }

        public string MaxLevel { get; set; }
        public string LevelsBetweenUpgrades { get; set; }
        public string RequiredLevel { get; set; }
        public string HotKeyOverride { get; set; }
        public string DisplayAdditionalHeroes { get; set; }
        public string AbilityTextureName { get; set; }
        public string AbilityUnitTargetFlag { get; set; }
        public string Modelscale { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public JObject[] AbilitySpecial { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string> AbilitySpecial2 { get; set; }
    }
}
