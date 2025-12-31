using System;
using UnityEngine;

/*
 * ========================================
 * 房产数据类 (HousingData.cs)
 * ========================================
 * 
 * 功能说明：
 * 存储房产的所有信息，包括租赁和购买类型的房产
 * 
 * 房产类型：
 * 1. 胶囊公寓 - 租赁，2币/5分钟，无心情加成
 * 2. 普通公寓 - 租赁，5币/5分钟，+1心情/5分钟
 * 3. 独栋别墅 - 购买，50000币，+3心情/5分钟
 * 4. 数据豪宅 - 购买，500000币+$9.99，+10心情/5分钟
 * 
 * 使用示例：
 * var housing = HousingData.CreateRentalHousing("rent_capsule", "胶囊公寓", 2, 0, "capsule.png", 2);
 * var villa = HousingData.CreatePurchaseHousing("buy_villa", "独栋别墅", 50000, 3, "villa.png", 15);
 */

namespace ECitizen.Data
{
    /// <summary>
    /// 房产类型枚举
    /// </summary>
    public enum HousingType
    {
        Rental,    // 租赁
        Purchase   // 购买
    }

    /// <summary>
    /// 房产品级枚举
    /// </summary>
    public enum HousingTier
    {
        Basic,     // 基础（胶囊公寓）
        Standard,  // 标准（普通公寓）
        Luxury,    // 豪华（独栋别墅）
        Premium    // 顶级（数据豪宅）
    }

    [Serializable]
    public class HousingData
    {
        // ===== 基本信息 =====
        [Header("基本信息")]
        [Tooltip("房产唯一ID")]
        public string housingId;

        [Tooltip("房产名称")]
        public string housingName;

        [Tooltip("房产类型（租赁/购买）")]
        public HousingType housingType;

        [Tooltip("房产品级")]
        public HousingTier tier;

        // ===== 费用信息 =====
        [Header("费用信息")]
        [Tooltip("租金（每5分钟，仅租赁类型有效）")]
        public float rentPer5Min;

        [Tooltip("购买价格（虚拟币，仅购买类型有效）")]
        public float purchasePrice;

        [Tooltip("是否需要真实货币（美元）")]
        public bool requireRealMoney;

        [Tooltip("真实货币价格（美元）")]
        public float realMoneyPrice;

        // ===== 属性加成 =====
        [Header("属性加成")]
        [Tooltip("心情值加成（每5分钟）")]
        public float moodBonusPer5Min;

        // ===== 视觉资源 =====
        [Header("视觉资源")]
        [Tooltip("室内背景图资源路径")]
        public string interiorSprite;

        [Tooltip("室外背景图资源路径")]
        public string exteriorSprite;

        [Tooltip("家具槽位数量")]
        public int furnitureSlots;

        // ===== 解锁条件 =====
        [Header("解锁条件")]
        [Tooltip("解锁所需等级")]
        public int requiredLevel;

        // ===== 构造函数 =====

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public HousingData()
        {
            housingId = "";
            housingName = "";
            housingType = HousingType.Rental;
            tier = HousingTier.Basic;
            rentPer5Min = 0;
            purchasePrice = 0;
            requireRealMoney = false;
            realMoneyPrice = 0;
            moodBonusPer5Min = 0;
            interiorSprite = "";
            exteriorSprite = "";
            furnitureSlots = 0;
            requiredLevel = 1;
        }

        // ===== 静态工厂方法 =====

        /// <summary>
        /// 创建租赁类型房产
        /// </summary>
        /// <param name="id">房产ID</param>
        /// <param name="name">房产名称</param>
        /// <param name="rent">租金（每5分钟）</param>
        /// <param name="moodBonus">心情值加成（每5分钟）</param>
        /// <param name="interior">室内图</param>
        /// <param name="exterior">室外图</param>
        /// <param name="slots">家具槽位</param>
        /// <param name="tier">房产品级</param>
        /// <param name="level">解锁等级</param>
        /// <returns>房产数据对象</returns>
        public static HousingData CreateRentalHousing(
            string id,
            string name,
            float rent,
            float moodBonus,
            string interior,
            string exterior,
            int slots,
            HousingTier tier = HousingTier.Basic,
            int level = 1)
        {
            var housing = new HousingData
            {
                housingId = id,
                housingName = name,
                housingType = HousingType.Rental,
                tier = tier,
                rentPer5Min = rent,
                moodBonusPer5Min = moodBonus,
                interiorSprite = interior,
                exteriorSprite = exterior,
                furnitureSlots = slots,
                requiredLevel = level
            };
            return housing;
        }

        /// <summary>
        /// 创建购买类型房产
        /// </summary>
        /// <param name="id">房产ID</param>
        /// <param name="name">房产名称</param>
        /// <param name="price">购买价格（虚拟币）</param>
        /// <param name="moodBonus">心情值加成（每5分钟）</param>
        /// <param name="interior">室内图</param>
        /// <param name="exterior">室外图</param>
        /// <param name="slots">家具槽位</param>
        /// <param name="tier">房产品级</param>
        /// <param name="realMoney">真实货币价格（0表示不需要）</param>
        /// <param name="level">解锁等级</param>
        /// <returns>房产数据对象</returns>
        public static HousingData CreatePurchaseHousing(
            string id,
            string name,
            float price,
            float moodBonus,
            string interior,
            string exterior,
            int slots,
            HousingTier tier = HousingTier.Luxury,
            float realMoney = 0,
            int level = 1)
        {
            var housing = new HousingData
            {
                housingId = id,
                housingName = name,
                housingType = HousingType.Purchase,
                tier = tier,
                purchasePrice = price,
                moodBonusPer5Min = moodBonus,
                interiorSprite = interior,
                exteriorSprite = exterior,
                furnitureSlots = slots,
                requireRealMoney = realMoney > 0,
                realMoneyPrice = realMoney,
                requiredLevel = level
            };
            return housing;
        }

        // ===== 辅助方法 =====

        /// <summary>
        /// 获取房产的每小时成本
        /// </summary>
        /// <returns>每小时成本（虚拟币）</returns>
        public float GetHourlyCost()
        {
            if (housingType == HousingType.Rental)
            {
                return rentPer5Min * 12; // 12个5分钟周期 = 1小时
            }
            return 0; // 购买类型没有持续成本
        }

        /// <summary>
        /// 获取房产的每小时心情加成
        /// </summary>
        /// <returns>每小时心情加成</returns>
        public float GetHourlyMoodBonus()
        {
            return moodBonusPer5Min * 12; // 12个5分钟周期 = 1小时
        }

        /// <summary>
        /// 检查玩家是否满足解锁条件
        /// </summary>
        /// <param name="playerLevel">玩家等级</param>
        /// <param name="virtualCoin">玩家虚拟币</param>
        /// <param name="hasRealMoney">是否有真实货币支付能力</param>
        /// <returns>是否可以获取此房产</returns>
        public bool CanAfford(int playerLevel, float virtualCoin, bool hasRealMoney = true)
        {
            // 检查等级
            if (playerLevel < requiredLevel)
            {
                return false;
            }

            // 检查真实货币要求
            if (requireRealMoney && !hasRealMoney)
            {
                return false;
            }

            // 检查虚拟币（租赁检查至少一个周期，购买检查全价）
            if (housingType == HousingType.Rental)
            {
                return virtualCoin >= rentPer5Min;
            }
            else
            {
                return virtualCoin >= purchasePrice;
            }
        }

        /// <summary>
        /// 获取房产品级的中文名称
        /// </summary>
        /// <returns>品级名称</returns>
        public string GetTierName()
        {
            switch (tier)
            {
                case HousingTier.Basic:
                    return "基础";
                case HousingTier.Standard:
                    return "标准";
                case HousingTier.Luxury:
                    return "豪华";
                case HousingTier.Premium:
                    return "顶级";
                default:
                    return "未知";
            }
        }

        /// <summary>
        /// 获取房产的详细描述
        /// </summary>
        /// <returns>详细描述字符串</returns>
        public string GetDescription()
        {
            string desc = $"【{housingName}】\n";
            desc += $"品级: {GetTierName()}\n";
            desc += $"类型: {(housingType == HousingType.Rental ? "租赁" : "购买")}\n";

            if (housingType == HousingType.Rental)
            {
                desc += $"租金: {rentPer5Min}币/5分钟 ({GetHourlyCost()}币/小时)\n";
            }
            else
            {
                desc += $"价格: {purchasePrice:F0}虚拟币\n";
                if (requireRealMoney)
                {
                    desc += $"      + ${realMoneyPrice:F2} 真实货币\n";
                }
            }

            desc += $"心情加成: +{moodBonusPer5Min}/5分钟 (+{GetHourlyMoodBonus()}/小时)\n";
            desc += $"家具槽位: {furnitureSlots}个\n";
            desc += $"解锁等级: Lv.{requiredLevel}";

            return desc;
        }

        /// <summary>
        /// 克隆房产数据
        /// </summary>
        /// <returns>新的房产数据副本</returns>
        public HousingData Clone()
        {
            return new HousingData
            {
                housingId = this.housingId,
                housingName = this.housingName,
                housingType = this.housingType,
                tier = this.tier,
                rentPer5Min = this.rentPer5Min,
                purchasePrice = this.purchasePrice,
                requireRealMoney = this.requireRealMoney,
                realMoneyPrice = this.realMoneyPrice,
                moodBonusPer5Min = this.moodBonusPer5Min,
                interiorSprite = this.interiorSprite,
                exteriorSprite = this.exteriorSprite,
                furnitureSlots = this.furnitureSlots,
                requiredLevel = this.requiredLevel
            };
        }
    }
}
