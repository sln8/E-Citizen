using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * ShopItemData.cs - 商城物品数据类
 * 
 * 功能说明：
 * 1. 定义商城中所有商品的数据结构
 * 2. 支持多种货币类型（虚拟币、真实货币）
 * 3. 支持多种商品类型（配置升级、外观装扮、虚拟币礼包、工具消耗品）
 * 4. 提供可重复购买支持
 * 5. 包含静态方法用于创建预定义商品
 * 
 * Unity操作说明：
 * - 本脚本无需挂载到GameObject
 * - 通过ShopManager.Instance访问商品数据
 * - 在商城UI中显示商品列表
 * 
 * 设计参考：
 * - 游戏设计.cs 第5章 商业化系统
 * - 支持虚拟币和真实货币两种支付方式
 */

namespace ECitizen.Data
{
    /// <summary>
    /// 商品类别枚举
    /// </summary>
    public enum ShopCategory
    {
        ConfigUpgrade,      // 配置升级（内存、CPU等）
        Appearance,         // 外观装扮（皮肤）
        VirtualCoin,        // 虚拟币礼包
        Tools               // 工具消耗品（数据清理等）
    }

    /// <summary>
    /// 货币类型枚举
    /// </summary>
    public enum CurrencyType
    {
        VirtualCoin,        // 虚拟币
        RealMoney           // 真实货币（USD）
    }

    /// <summary>
    /// 配置升级类型枚举
    /// </summary>
    public enum UpgradeType
    {
        Memory,             // 内存
        CPU,                // CPU核心
        Bandwidth,          // 网速
        Computing,          // 算力
        Storage             // 存储空间
    }

    /// <summary>
    /// 商城物品数据类
    /// 存储商城中单个商品的所有信息
    /// </summary>
    [Serializable]
    public class ShopItemData
    {
        // ==================== 基本信息 ====================
        
        /// <summary>商品唯一ID</summary>
        public string itemId;
        
        /// <summary>商品名称</summary>
        public string itemName;
        
        /// <summary>商品描述</summary>
        public string description;
        
        /// <summary>商品类别</summary>
        public ShopCategory category;
        
        /// <summary>货币类型</summary>
        public CurrencyType currencyType;
        
        /// <summary>价格（虚拟币或真实货币）</summary>
        public float price;
        
        /// <summary>是否可重复购买</summary>
        public bool isRepeatable;
        
        /// <summary>图标名称（用于加载Sprite）</summary>
        public string iconName;
        
        // ==================== 配置升级专用字段 ====================
        
        /// <summary>升级类型（仅配置升级类商品）</summary>
        public UpgradeType upgradeType;
        
        /// <summary>升级数值（如内存+1GB）</summary>
        public float upgradeAmount;
        
        // ==================== 虚拟币礼包专用字段 ====================
        
        /// <summary>虚拟币数量（仅虚拟币礼包）</summary>
        public int virtualCoinAmount;
        
        /// <summary>赠送虚拟币数量（额外赠送）</summary>
        public int bonusCoinAmount;
        
        // ==================== 外观装扮专用字段 ====================
        
        /// <summary>Spine资源路径（仅外观装扮）</summary>
        public string spineAssetPath;
        
        // ==================== 工具消耗品专用字段 ====================
        
        /// <summary>工具效果（如清理-10GB数据）</summary>
        public Dictionary<string, float> toolEffects;
        
        /// <summary>是否可堆叠</summary>
        public bool isStackable;
        
        /// <summary>最大堆叠数量</summary>
        public int maxStackSize;

        // ==================== 构造函数 ====================

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ShopItemData()
        {
            toolEffects = new Dictionary<string, float>();
            isStackable = true;
            maxStackSize = 99;
        }

        // ==================== 辅助方法 ====================

        /// <summary>
        /// 获取完整价格描述
        /// </summary>
        public string GetPriceText()
        {
            if (currencyType == CurrencyType.VirtualCoin)
            {
                return $"{price:N0} 虚拟币";
            }
            else
            {
                return $"${price:F2}";
            }
        }

        /// <summary>
        /// 获取商品详细描述
        /// </summary>
        public string GetDetailedDescription()
        {
            string detail = description;

            // 配置升级商品
            if (category == ShopCategory.ConfigUpgrade)
            {
                string upgradeText = upgradeType switch
                {
                    UpgradeType.Memory => $"+{upgradeAmount}GB 内存",
                    UpgradeType.CPU => $"+{upgradeAmount} CPU核心",
                    UpgradeType.Bandwidth => $"+{upgradeAmount}Mbps 网速",
                    UpgradeType.Computing => $"+{upgradeAmount} 算力",
                    UpgradeType.Storage => $"+{upgradeAmount}GB 存储",
                    _ => ""
                };
                detail += $"\n效果: {upgradeText}";
            }

            // 虚拟币礼包
            if (category == ShopCategory.VirtualCoin)
            {
                detail += $"\n包含: {virtualCoinAmount:N0} 虚拟币";
                if (bonusCoinAmount > 0)
                {
                    detail += $"\n额外赠送: {bonusCoinAmount:N0} 虚拟币";
                }
            }

            // 工具消耗品
            if (category == ShopCategory.Tools && toolEffects.Count > 0)
            {
                detail += "\n效果:";
                foreach (var effect in toolEffects)
                {
                    detail += $"\n  {effect.Key}: {effect.Value}";
                }
            }

            // 可重复购买提示
            if (isRepeatable)
            {
                detail += "\n\n可重复购买";
            }

            return detail;
        }

        /// <summary>
        /// 获取总虚拟币数量（含赠送）
        /// </summary>
        public int GetTotalVirtualCoin()
        {
            return virtualCoinAmount + bonusCoinAmount;
        }

        /// <summary>
        /// 获取性价比（虚拟币礼包专用）
        /// 返回每1美元能获得多少虚拟币
        /// </summary>
        public float GetValueRatio()
        {
            if (category == ShopCategory.VirtualCoin && currencyType == CurrencyType.RealMoney && price > 0)
            {
                return GetTotalVirtualCoin() / price;
            }
            return 0f;
        }

        // ==================== 静态工厂方法 - 配置升级 ====================

        /// <summary>
        /// 创建内存升级商品
        /// </summary>
        public static ShopItemData CreateMemoryUpgrade()
        {
            return new ShopItemData
            {
                itemId = "memory_1gb",
                itemName = "内存 +1GB",
                description = "增加1GB内存容量，提升可同时运行的程序数量",
                category = ShopCategory.ConfigUpgrade,
                currencyType = CurrencyType.VirtualCoin,
                price = 100f,
                isRepeatable = true,
                iconName = "icon_memory",
                upgradeType = UpgradeType.Memory,
                upgradeAmount = 1f
            };
        }

        /// <summary>
        /// 创建CPU升级商品
        /// </summary>
        public static ShopItemData CreateCPUUpgrade()
        {
            return new ShopItemData
            {
                itemId = "cpu_1core",
                itemName = "CPU +1核",
                description = "增加1个CPU核心，提升处理速度和效率",
                category = ShopCategory.ConfigUpgrade,
                currencyType = CurrencyType.VirtualCoin,
                price = 200f,
                isRepeatable = true,
                iconName = "icon_cpu",
                upgradeType = UpgradeType.CPU,
                upgradeAmount = 1f
            };
        }

        /// <summary>
        /// 创建网速升级商品
        /// </summary>
        public static ShopItemData CreateBandwidthUpgrade()
        {
            return new ShopItemData
            {
                itemId = "bandwidth_100mbps",
                itemName = "网速 +100Mbps",
                description = "增加100Mbps网速，加快数据传输和技能下载",
                category = ShopCategory.ConfigUpgrade,
                currencyType = CurrencyType.VirtualCoin,
                price = 150f,
                isRepeatable = true,
                iconName = "icon_bandwidth",
                upgradeType = UpgradeType.Bandwidth,
                upgradeAmount = 100f
            };
        }

        /// <summary>
        /// 创建算力升级商品
        /// </summary>
        public static ShopItemData CreateComputingUpgrade()
        {
            return new ShopItemData
            {
                itemId = "computing_10",
                itemName = "算力 +10",
                description = "增加10点算力，用于学习技能和提升掌握度",
                category = ShopCategory.ConfigUpgrade,
                currencyType = CurrencyType.VirtualCoin,
                price = 180f,
                isRepeatable = true,
                iconName = "icon_computing",
                upgradeType = UpgradeType.Computing,
                upgradeAmount = 10f
            };
        }

        /// <summary>
        /// 创建存储升级商品
        /// </summary>
        public static ShopItemData CreateStorageUpgrade()
        {
            return new ShopItemData
            {
                itemId = "storage_50gb",
                itemName = "存储 +50GB",
                description = "增加50GB存储空间，延缓数据清理压力",
                category = ShopCategory.ConfigUpgrade,
                currencyType = CurrencyType.VirtualCoin,
                price = 50f,
                isRepeatable = true,
                iconName = "icon_storage",
                upgradeType = UpgradeType.Storage,
                upgradeAmount = 50f
            };
        }

        // ==================== 静态工厂方法 - 虚拟币礼包 ====================

        /// <summary>
        /// 创建虚拟币小包
        /// </summary>
        public static ShopItemData CreateCoinPackSmall()
        {
            return new ShopItemData
            {
                itemId = "coin_pack_1",
                itemName = "虚拟币小包",
                description = "快速获得1000虚拟币，适合小额购买",
                category = ShopCategory.VirtualCoin,
                currencyType = CurrencyType.RealMoney,
                price = 0.99f,
                isRepeatable = true,
                iconName = "icon_coin_small",
                virtualCoinAmount = 1000,
                bonusCoinAmount = 0
            };
        }

        /// <summary>
        /// 创建虚拟币中包
        /// </summary>
        public static ShopItemData CreateCoinPackMedium()
        {
            return new ShopItemData
            {
                itemId = "coin_pack_2",
                itemName = "虚拟币中包",
                description = "获得6000虚拟币+500赠送币，性价比高",
                category = ShopCategory.VirtualCoin,
                currencyType = CurrencyType.RealMoney,
                price = 4.99f,
                isRepeatable = true,
                iconName = "icon_coin_medium",
                virtualCoinAmount = 6000,
                bonusCoinAmount = 500
            };
        }

        /// <summary>
        /// 创建虚拟币大包
        /// </summary>
        public static ShopItemData CreateCoinPackLarge()
        {
            return new ShopItemData
            {
                itemId = "coin_pack_3",
                itemName = "虚拟币大包",
                description = "获得15000虚拟币+2000赠送币，超值优惠",
                category = ShopCategory.VirtualCoin,
                currencyType = CurrencyType.RealMoney,
                price = 9.99f,
                isRepeatable = true,
                iconName = "icon_coin_large",
                virtualCoinAmount = 15000,
                bonusCoinAmount = 2000
            };
        }

        /// <summary>
        /// 创建虚拟币超级包
        /// </summary>
        public static ShopItemData CreateCoinPackUltra()
        {
            return new ShopItemData
            {
                itemId = "coin_pack_4",
                itemName = "虚拟币超级包",
                description = "获得50000虚拟币+10000赠送币，最划算的选择",
                category = ShopCategory.VirtualCoin,
                currencyType = CurrencyType.RealMoney,
                price = 29.99f,
                isRepeatable = true,
                iconName = "icon_coin_ultra",
                virtualCoinAmount = 50000,
                bonusCoinAmount = 10000
            };
        }

        // ==================== 静态工厂方法 - 工具消耗品 ====================

        /// <summary>
        /// 创建数据清理工具（小）
        /// </summary>
        public static ShopItemData CreateDataCleanerSmall()
        {
            var item = new ShopItemData
            {
                itemId = "data_cleaner_small",
                itemName = "数据清理工具(小)",
                description = "清除10GB已用存储空间，缓解存储压力",
                category = ShopCategory.Tools,
                currencyType = CurrencyType.VirtualCoin,
                price = 50f,
                isRepeatable = true,
                iconName = "icon_cleaner_small",
                isStackable = true,
                maxStackSize = 99
            };
            item.toolEffects["storage"] = -10f; // 负数表示减少已用存储
            return item;
        }

        /// <summary>
        /// 创建数据清理工具（大）
        /// </summary>
        public static ShopItemData CreateDataCleanerLarge()
        {
            var item = new ShopItemData
            {
                itemId = "data_cleaner_large",
                itemName = "数据清理工具(大)",
                description = "清除100GB已用存储空间，大幅缓解存储压力",
                category = ShopCategory.Tools,
                currencyType = CurrencyType.VirtualCoin,
                price = 400f,
                isRepeatable = true,
                iconName = "icon_cleaner_large",
                isStackable = true,
                maxStackSize = 99
            };
            item.toolEffects["storage"] = -100f;
            return item;
        }

        /// <summary>
        /// 创建数据清理工具（终极）
        /// </summary>
        public static ShopItemData CreateDataCleanerUltimate()
        {
            var item = new ShopItemData
            {
                itemId = "data_cleaner_ultimate",
                itemName = "终极数据清理工具",
                description = "清除1000GB已用存储空间，彻底解决存储问题",
                category = ShopCategory.Tools,
                currencyType = CurrencyType.RealMoney,
                price = 1.99f,
                isRepeatable = true,
                iconName = "icon_cleaner_ultimate",
                isStackable = true,
                maxStackSize = 99
            };
            item.toolEffects["storage"] = -1000f;
            return item;
        }

        // ==================== 静态工厂方法 - 外观装扮 ====================

        /// <summary>
        /// 创建基础外观-科技风
        /// </summary>
        public static ShopItemData CreateSkinBasicTech()
        {
            return new ShopItemData
            {
                itemId = "skin_basic_1",
                itemName = "基础外观-科技风",
                description = "简约的科技风格外观，展现数字世界的理性美",
                category = ShopCategory.Appearance,
                currencyType = CurrencyType.VirtualCoin,
                price = 500f,
                isRepeatable = false,
                iconName = "icon_skin_tech",
                spineAssetPath = "Skins/tech_skin"
            };
        }

        /// <summary>
        /// 创建高级外观-赛博朋克
        /// </summary>
        public static ShopItemData CreateSkinPremiumCyberpunk()
        {
            return new ShopItemData
            {
                itemId = "skin_premium_1",
                itemName = "高级外观-赛博朋克",
                description = "炫酷的赛博朋克风格外观，独特的霓虹特效",
                category = ShopCategory.Appearance,
                currencyType = CurrencyType.RealMoney,
                price = 4.99f,
                isRepeatable = false,
                iconName = "icon_skin_cyberpunk",
                spineAssetPath = "Skins/cyberpunk_skin"
            };
        }

        // ==================== 静态方法 - 获取所有预定义商品 ====================

        /// <summary>
        /// 获取所有配置升级商品
        /// </summary>
        public static List<ShopItemData> GetAllConfigUpgrades()
        {
            return new List<ShopItemData>
            {
                CreateMemoryUpgrade(),
                CreateCPUUpgrade(),
                CreateBandwidthUpgrade(),
                CreateComputingUpgrade(),
                CreateStorageUpgrade()
            };
        }

        /// <summary>
        /// 获取所有虚拟币礼包
        /// </summary>
        public static List<ShopItemData> GetAllCoinPacks()
        {
            return new List<ShopItemData>
            {
                CreateCoinPackSmall(),
                CreateCoinPackMedium(),
                CreateCoinPackLarge(),
                CreateCoinPackUltra()
            };
        }

        /// <summary>
        /// 获取所有工具消耗品
        /// </summary>
        public static List<ShopItemData> GetAllTools()
        {
            return new List<ShopItemData>
            {
                CreateDataCleanerSmall(),
                CreateDataCleanerLarge(),
                CreateDataCleanerUltimate()
            };
        }

        /// <summary>
        /// 获取所有外观装扮
        /// </summary>
        public static List<ShopItemData> GetAllAppearances()
        {
            return new List<ShopItemData>
            {
                CreateSkinBasicTech(),
                CreateSkinPremiumCyberpunk()
            };
        }

        /// <summary>
        /// 获取所有商品
        /// </summary>
        public static List<ShopItemData> GetAllItems()
        {
            var allItems = new List<ShopItemData>();
            allItems.AddRange(GetAllConfigUpgrades());
            allItems.AddRange(GetAllCoinPacks());
            allItems.AddRange(GetAllTools());
            allItems.AddRange(GetAllAppearances());
            return allItems;
        }
    }
}
