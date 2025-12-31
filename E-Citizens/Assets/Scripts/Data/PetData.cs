using System;
using UnityEngine;

/*
 * ========================================
 * 宠物数据类 (PetData.cs)
 * ========================================
 * 
 * 功能说明：
 * 存储宠物的所有信息，包括价格、持续心情加成等
 * 
 * 宠物类型：
 * 1. 数据犬 - 1000币，+0.5心情/5分钟
 * 2. 赛博猫 - 1500币，+0.8心情/5分钟
 * 3. 像素龙 - 50000币+$2.99，+2心情/5分钟
 * 
 * 持续加成说明：
 * 宠物会每个游戏周期（5分钟）持续提供心情值加成
 * 例如：数据犬每5分钟+0.5心情，每小时+6心情
 * 
 * 使用示例：
 * var dog = PetData.CreatePet("pet_dog", "数据犬", 1000, 0.5f, "data_dog.json");
 * var dragon = PetData.CreatePet("pet_dragon", "像素龙", 50000, 2.0f, "pixel_dragon.json", 2.99f);
 */

/// <summary>
/// 宠物品级枚举
/// </summary>
public enum PetTier
{
    Common,   // 普通（数据犬、赛博猫）
    Rare,     // 稀有
    Epic,     // 史诗
    Legendary // 传说（像素龙）
}

    [Serializable]
    public class PetData
    {
        // ===== 基本信息 =====
        [Header("基本信息")]
        [Tooltip("宠物唯一ID")]
        public string petId;

        [Tooltip("宠物名称")]
        public string petName;

        [Tooltip("宠物品级")]
        public PetTier tier;

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
        [Tooltip("心情值加成（每5分钟持续提供）")]
        public float moodBonusPer5Min;

        // ===== 视觉资源 =====
        [Header("视觉资源")]
        [Tooltip("Spine动画资源路径")]
        public string spineAnimation;

        [Tooltip("宠物图标路径")]
        public string iconSprite;

        // ===== 解锁条件 =====
        [Header("解锁条件")]
        [Tooltip("解锁所需等级")]
        public int requiredLevel;

        // ===== 宠物状态 =====
        [Header("宠物状态")]
        [Tooltip("宠物当前的饱食度（0-100，暂未使用）")]
        public float satiety;

        [Tooltip("宠物当前的快乐度（0-100，暂未使用）")]
        public float happiness;

        // ===== 构造函数 =====

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PetData()
        {
            petId = "";
            petName = "";
            tier = PetTier.Common;
            purchasePrice = 0;
            requireRealMoney = false;
            realMoneyPrice = 0;
            moodBonusPer5Min = 0;
            spineAnimation = "";
            iconSprite = "";
            requiredLevel = 1;
            satiety = 100;
            happiness = 100;
        }

        // ===== 静态工厂方法 =====

        /// <summary>
        /// 创建宠物数据
        /// </summary>
        /// <param name="id">宠物ID</param>
        /// <param name="name">宠物名称</param>
        /// <param name="price">购买价格（虚拟币）</param>
        /// <param name="moodBonus">心情值加成（每5分钟）</param>
        /// <param name="animation">Spine动画路径</param>
        /// <param name="tier">宠物品级</param>
        /// <param name="realMoney">真实货币价格（0表示不需要）</param>
        /// <param name="level">解锁等级</param>
        /// <returns>宠物数据对象</returns>
        public static PetData CreatePet(
            string id,
            string name,
            float price,
            float moodBonus,
            string animation,
            PetTier tier = PetTier.Common,
            float realMoney = 0,
            int level = 1)
        {
            var pet = new PetData
            {
                petId = id,
                petName = name,
                tier = tier,
                purchasePrice = price,
                moodBonusPer5Min = moodBonus,
                spineAnimation = animation,
                iconSprite = animation.Replace(".json", ".png"), // 默认使用相同名称的图标
                requireRealMoney = realMoney > 0,
                realMoneyPrice = realMoney,
                requiredLevel = level,
                satiety = 100,
                happiness = 100
            };
            return pet;
        }

        // ===== 辅助方法 =====

        /// <summary>
        /// 获取宠物的每小时心情加成
        /// </summary>
        /// <returns>每小时心情加成</returns>
        public float GetHourlyMoodBonus()
        {
            return moodBonusPer5Min * 12; // 12个5分钟周期 = 1小时
        }

        /// <summary>
        /// 获取宠物的每天心情加成
        /// </summary>
        /// <returns>每天心情加成</returns>
        public float GetDailyMoodBonus()
        {
            return moodBonusPer5Min * 288; // 288个5分钟周期 = 24小时
        }

        /// <summary>
        /// 检查玩家是否能购买此宠物
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
        /// 获取宠物品级的中文名称
        /// </summary>
        /// <returns>品级名称</returns>
        public string GetTierName()
        {
            switch (tier)
            {
                case PetTier.Common:
                    return "普通";
                case PetTier.Rare:
                    return "稀有";
                case PetTier.Epic:
                    return "史诗";
                case PetTier.Legendary:
                    return "传说";
                default:
                    return "未知";
            }
        }

        /// <summary>
        /// 获取宠物的详细描述
        /// </summary>
        /// <returns>详细描述字符串</returns>
        public string GetDescription()
        {
            string desc = $"【{petName}】\n";
            desc += $"品级: {GetTierName()}\n";
            desc += $"价格: {purchasePrice:F0}虚拟币\n";

            if (requireRealMoney)
            {
                desc += $"      + ${realMoneyPrice:F2} 真实货币\n";
            }

            desc += $"心情加成: +{moodBonusPer5Min:F1}/5分钟\n";
            desc += $"          (+{GetHourlyMoodBonus():F1}/小时)\n";
            desc += $"          (+{GetDailyMoodBonus():F1}/天)\n";
            desc += $"解锁等级: Lv.{requiredLevel}";

            return desc;
        }

        /// <summary>
        /// 计算宠物的性价比（每1000虚拟币带来的每小时心情加成）
        /// </summary>
        /// <returns>性价比值</returns>
        public float GetValueRatio()
        {
            if (purchasePrice <= 0)
            {
                return 0;
            }
            return (GetHourlyMoodBonus() / purchasePrice) * 1000;
        }

        /// <summary>
        /// 更新宠物状态（暂未实现，预留接口）
        /// </summary>
        /// <param name="deltaTime">时间增量（秒）</param>
        public void UpdateState(float deltaTime)
        {
            // TODO: 未来可以实现宠物饱食度和快乐度的衰减
            // satiety -= deltaTime * 0.01f;
            // happiness -= deltaTime * 0.01f;
            // satiety = Mathf.Max(0, satiety);
            // happiness = Mathf.Max(0, happiness);
        }

        /// <summary>
        /// 喂养宠物（暂未实现，预留接口）
        /// </summary>
        /// <param name="amount">喂养量</param>
        public void Feed(float amount)
        {
            // TODO: 未来可以实现喂养功能
            // satiety = Mathf.Min(100, satiety + amount);
        }

        /// <summary>
        /// 与宠物互动（暂未实现，预留接口）
        /// </summary>
        /// <param name="amount">互动量</param>
        public void Interact(float amount)
        {
            // TODO: 未来可以实现互动功能
            // happiness = Mathf.Min(100, happiness + amount);
        }

        /// <summary>
        /// 克隆宠物数据
        /// </summary>
        /// <returns>新的宠物数据副本</returns>
        public PetData Clone()
        {
            return new PetData
            {
                petId = this.petId,
                petName = this.petName,
                tier = this.tier,
                purchasePrice = this.purchasePrice,
                requireRealMoney = this.requireRealMoney,
                realMoneyPrice = this.realMoneyPrice,
                moodBonusPer5Min = this.moodBonusPer5Min,
                spineAnimation = this.spineAnimation,
                iconSprite = this.iconSprite,
                requiredLevel = this.requiredLevel,
                satiety = this.satiety,
                happiness = this.happiness
            };
        }

        /// <summary>
        /// 比较两个宠物的性价比
        /// </summary>
        /// <param name="other">另一个宠物</param>
        /// <returns>1表示更高，-1表示更低，0表示相同</returns>
        public int CompareValue(PetData other)
        {
            if (other == null)
            {
                return 1;
            }

            float thisValue = GetValueRatio();
            float otherValue = other.GetValueRatio();

            if (thisValue > otherValue)
            {
                return 1;
            }
            else if (thisValue < otherValue)
            {
                return -1;
            }
            return 0;
        }
    }

