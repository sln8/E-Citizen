using System;
using UnityEngine;

/*
 * ========================================
 * 汽车数据类 (VehicleData.cs)
 * ========================================
 * 
 * 功能说明：
 * 存储汽车的所有信息，包括价格、属性加成等
 * 
 * 汽车类型：
 * 1. 数据滑板 - 500币，1.1倍速度，+5心情
 * 2. 光速跑车 - 10000币，1.5倍速度，+20心情
 * 3. 量子飞行器 - 100000币+$4.99，2.0倍速度，+50心情
 * 
 * 速度加成说明：
 * 速度加成会缩短娱乐活动的时间
 * 例如：基础娱乐时间10分钟，1.5倍速度 = 10/1.5 = 6.67分钟
 * 
 * 使用示例：
 * var skateboard = VehicleData.CreateVehicle("car_basic", "数据滑板", 500, 1.1f, 5, "skateboard.png");
 * var quantum = VehicleData.CreateVehicle("car_luxury", "量子飞行器", 100000, 2.0f, 50, "quantum.png", 4.99f);
 */

/// <summary>
/// 汽车品级枚举
/// </summary>
public enum VehicleTier
{
    Basic,    // 基础（数据滑板）
    Sport,    // 运动（光速跑车）
    Luxury    // 豪华（量子飞行器）
}

    [Serializable]
    public class VehicleData
    {
        // ===== 基本信息 =====
        [Header("基本信息")]
        [Tooltip("汽车唯一ID")]
        public string vehicleId;

        [Tooltip("汽车名称")]
        public string vehicleName;

        [Tooltip("汽车品级")]
        public VehicleTier tier;

        // ===== 购买信息 =====
        [Header("购买信息")]
        [Tooltip("购买价格（虚拟币）")]
        public float purchasePrice;

        [Tooltip("是否需要真实货币")]
        public bool requireRealMoney;

        [Tooltip("真实货币价格（美元）")]
        public float realMoneyPrice;

        // ===== 属性加成 =====
        [Header("属性加成")]
        [Tooltip("速度加成倍数（用于缩短娱乐时间）")]
        public float speedBonus;

        [Tooltip("购买时的一次性心情值加成")]
        public float moodBonus;

        // ===== 视觉资源 =====
        [Header("视觉资源")]
        [Tooltip("汽车图标资源路径")]
        public string iconSprite;

        // ===== 解锁条件 =====
        [Header("解锁条件")]
        [Tooltip("解锁所需等级")]
        public int requiredLevel;

        // ===== 构造函数 =====

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public VehicleData()
        {
            vehicleId = "";
            vehicleName = "";
            tier = VehicleTier.Basic;
            purchasePrice = 0;
            requireRealMoney = false;
            realMoneyPrice = 0;
            speedBonus = 1.0f;
            moodBonus = 0;
            iconSprite = "";
            requiredLevel = 1;
        }

        // ===== 静态工厂方法 =====

        /// <summary>
        /// 创建汽车数据
        /// </summary>
        /// <param name="id">汽车ID</param>
        /// <param name="name">汽车名称</param>
        /// <param name="price">购买价格（虚拟币）</param>
        /// <param name="speed">速度加成倍数</param>
        /// <param name="mood">心情值加成</param>
        /// <param name="icon">图标路径</param>
        /// <param name="tier">汽车品级</param>
        /// <param name="realMoney">真实货币价格（0表示不需要）</param>
        /// <param name="level">解锁等级</param>
        /// <returns>汽车数据对象</returns>
        public static VehicleData CreateVehicle(
            string id,
            string name,
            float price,
            float speed,
            float mood,
            string icon,
            VehicleTier tier = VehicleTier.Basic,
            float realMoney = 0,
            int level = 1)
        {
            var vehicle = new VehicleData
            {
                vehicleId = id,
                vehicleName = name,
                tier = tier,
                purchasePrice = price,
                speedBonus = speed,
                moodBonus = mood,
                iconSprite = icon,
                requireRealMoney = realMoney > 0,
                realMoneyPrice = realMoney,
                requiredLevel = level
            };
            return vehicle;
        }

        // ===== 辅助方法 =====

        /// <summary>
        /// 计算使用此汽车后的实际活动时间
        /// </summary>
        /// <param name="baseTime">基础活动时间（秒）</param>
        /// <returns>实际活动时间（秒）</returns>
        public float CalculateActualTime(float baseTime)
        {
            if (speedBonus <= 0)
            {
                Debug.LogWarning($"[VehicleData] 速度加成无效: {speedBonus}，使用基础时间");
                return baseTime;
            }
            return baseTime / speedBonus;
        }

        /// <summary>
        /// 获取时间节省百分比
        /// </summary>
        /// <returns>节省的时间百分比（0-100）</returns>
        public float GetTimeSavingPercentage()
        {
            if (speedBonus <= 1.0f)
            {
                return 0;
            }
            return (1 - 1.0f / speedBonus) * 100;
        }

        /// <summary>
        /// 检查玩家是否能购买此汽车
        /// </summary>
        /// <param name="playerLevel">玩家等级</param>
        /// <param name="virtualCoin">玩家虚拟币</param>
        /// <param name="hasRealMoney">是否有真实货币支付能力</param>
        /// <returns>是否可以购买</returns>
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

            // 检查虚拟币
            return virtualCoin >= purchasePrice;
        }

        /// <summary>
        /// 获取汽车品级的中文名称
        /// </summary>
        /// <returns>品级名称</returns>
        public string GetTierName()
        {
            switch (tier)
            {
                case VehicleTier.Basic:
                    return "基础";
                case VehicleTier.Sport:
                    return "运动";
                case VehicleTier.Luxury:
                    return "豪华";
                default:
                    return "未知";
            }
        }

        /// <summary>
        /// 获取汽车的详细描述
        /// </summary>
        /// <returns>详细描述字符串</returns>
        public string GetDescription()
        {
            string desc = $"【{vehicleName}】\n";
            desc += $"品级: {GetTierName()}\n";
            desc += $"价格: {purchasePrice:F0}虚拟币\n";

            if (requireRealMoney)
            {
                desc += $"      + ${realMoneyPrice:F2} 真实货币\n";
            }

            desc += $"速度加成: {speedBonus:F1}x (节省{GetTimeSavingPercentage():F1}%时间)\n";
            desc += $"心情加成: +{moodBonus:F0} (购买时)\n";
            desc += $"解锁等级: Lv.{requiredLevel}";

            return desc;
        }

        /// <summary>
        /// 克隆汽车数据
        /// </summary>
        /// <returns>新的汽车数据副本</returns>
        public VehicleData Clone()
        {
            return new VehicleData
            {
                vehicleId = this.vehicleId,
                vehicleName = this.vehicleName,
                tier = this.tier,
                purchasePrice = this.purchasePrice,
                requireRealMoney = this.requireRealMoney,
                realMoneyPrice = this.realMoneyPrice,
                speedBonus = this.speedBonus,
                moodBonus = this.moodBonus,
                iconSprite = this.iconSprite,
                requiredLevel = this.requiredLevel
            };
        }

        /// <summary>
        /// 比较两个汽车的速度
        /// </summary>
        /// <param name="other">另一个汽车</param>
        /// <returns>1表示更快，-1表示更慢，0表示相同</returns>
        public int CompareSpeed(VehicleData other)
        {
            if (other == null)
            {
                return 1;
            }

            if (speedBonus > other.speedBonus)
            {
                return 1;
            }
            else if (speedBonus < other.speedBonus)
            {
                return -1;
            }
            return 0;
        }
    }

