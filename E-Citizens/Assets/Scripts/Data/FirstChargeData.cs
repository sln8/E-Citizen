using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * FirstChargeData.cs - 首充数据类
 * 
 * 功能说明：
 * 1. 定义首充礼包的数据结构
 * 2. 管理首充状态（是否已购买）
 * 3. 配置超值奖励内容（虚拟币、道具、VIP体验卡等）
 * 4. 提供首充相关的验证和显示方法
 * 
 * Unity操作说明：
 * - 本脚本无需挂载到GameObject
 * - 通过ShopManager.Instance访问首充数据
 * - 首充礼包会在商城UI中高亮显示
 * 
 * 设计参考：
 * - 游戏设计.cs 第5.2节 首充系统
 * - 首充价格：$0.99
 * - 首充奖励：5000虚拟币 + 多种道具 + VIP体验卡
 * - 每个账号仅能购买一次
 */

namespace ECitizen.Data
{
    /// <summary>
    /// 首充奖励物品类型枚举
    /// </summary>
    public enum FirstChargeRewardType
    {
        VirtualCoin,        // 虚拟币
        Memory,             // 内存升级
        CPU,                // CPU升级
        Storage,            // 存储升级
        DataCleaner,        // 数据清理工具
        Skin,               // 外观皮肤
        VIPCard             // VIP体验卡
    }

    /// <summary>
    /// 首充奖励物品数据类
    /// </summary>
    [Serializable]
    public class FirstChargeReward
    {
        /// <summary>奖励类型</summary>
        public FirstChargeRewardType rewardType;
        
        /// <summary>物品ID</summary>
        public string itemId;
        
        /// <summary>物品名称</summary>
        public string itemName;
        
        /// <summary>数量</summary>
        public int amount;
        
        /// <summary>描述</summary>
        public string description;
        
        /// <summary>图标名称</summary>
        public string iconName;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FirstChargeReward(FirstChargeRewardType type, string id, string name, int amount, string desc, string icon)
        {
            this.rewardType = type;
            this.itemId = id;
            this.itemName = name;
            this.amount = amount;
            this.description = desc;
            this.iconName = icon;
        }

        /// <summary>
        /// 获取奖励显示文本
        /// </summary>
        public string GetDisplayText()
        {
            if (amount > 1)
            {
                return $"{itemName} x{amount}";
            }
            return itemName;
        }
    }

    /// <summary>
    /// 首充数据类
    /// 存储首充礼包的所有信息
    /// </summary>
    [Serializable]
    public class FirstChargeData
    {
        // ==================== 基本信息 ====================
        
        /// <summary>礼包ID</summary>
        public string offerId;
        
        /// <summary>礼包名称</summary>
        public string offerName;
        
        /// <summary>价格（美元）</summary>
        public float price;
        
        /// <summary>礼包描述</summary>
        public string description;
        
        /// <summary>图标名称</summary>
        public string iconName;
        
        /// <summary>横幅图标名称（用于商城首页展示）</summary>
        public string bannerIconName;
        
        // ==================== 奖励内容 ====================
        
        /// <summary>虚拟币奖励</summary>
        public int virtualCoinReward;
        
        /// <summary>所有奖励物品列表</summary>
        public List<FirstChargeReward> rewards;
        
        // ==================== 状态信息 ====================
        
        /// <summary>是否已购买</summary>
        public bool isPurchased;
        
        /// <summary>购买时间</summary>
        public DateTime purchaseTime;
        
        /// <summary>是否已领取奖励</summary>
        public bool isRewardClaimed;
        
        /// <summary>奖励领取时间</summary>
        public DateTime claimTime;
        
        // ==================== UI展示配置 ====================
        
        /// <summary>是否在商城首页高亮显示</summary>
        public bool isHighlighted;
        
        /// <summary>显示位置（商城横幅、弹窗等）</summary>
        public string displayPosition;
        
        /// <summary>标签文本（如"超值首充"）</summary>
        public string badgeText;

        // ==================== 构造函数 ====================

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FirstChargeData()
        {
            rewards = new List<FirstChargeReward>();
            isPurchased = false;
            isRewardClaimed = false;
            isHighlighted = true;
            displayPosition = "shop_banner";
            badgeText = "超值首充";
        }

        // ==================== 状态检查方法 ====================

        /// <summary>
        /// 检查是否可以购买
        /// </summary>
        public bool CanPurchase()
        {
            return !isPurchased;
        }

        /// <summary>
        /// 检查是否可以领取奖励
        /// </summary>
        public bool CanClaimReward()
        {
            return isPurchased && !isRewardClaimed;
        }

        /// <summary>
        /// 获取状态文本
        /// </summary>
        public string GetStatusText()
        {
            if (!isPurchased)
            {
                return "未购买";
            }
            else if (!isRewardClaimed)
            {
                return "待领取";
            }
            else
            {
                return "已完成";
            }
        }

        // ==================== 购买和领取方法 ====================

        /// <summary>
        /// 标记为已购买
        /// </summary>
        public void MarkAsPurchased()
        {
            isPurchased = true;
            purchaseTime = DateTime.Now;
            Debug.Log($"首充礼包已购买: {offerName}");
        }

        /// <summary>
        /// 领取奖励
        /// </summary>
        public bool ClaimRewards()
        {
            if (!CanClaimReward())
            {
                Debug.LogWarning("无法领取首充奖励：未购买或已领取");
                return false;
            }

            isRewardClaimed = true;
            claimTime = DateTime.Now;
            Debug.Log($"首充奖励已领取: {offerName}");
            return true;
        }

        // ==================== 奖励管理方法 ====================

        /// <summary>
        /// 添加奖励
        /// </summary>
        public void AddReward(FirstChargeReward reward)
        {
            if (reward != null)
            {
                rewards.Add(reward);
            }
        }

        /// <summary>
        /// 获取所有奖励的显示列表
        /// </summary>
        public List<string> GetRewardDisplayList()
        {
            var displayList = new List<string>();
            
            // 添加虚拟币奖励
            if (virtualCoinReward > 0)
            {
                displayList.Add($"{virtualCoinReward:N0} 虚拟币");
            }
            
            // 添加其他奖励
            foreach (var reward in rewards)
            {
                displayList.Add(reward.GetDisplayText());
            }
            
            return displayList;
        }

        /// <summary>
        /// 获取奖励显示文本（换行分隔）
        /// </summary>
        public string GetRewardsText()
        {
            return string.Join("\n", GetRewardDisplayList());
        }

        // ==================== 价值计算方法 ====================

        /// <summary>
        /// 计算总价值（以虚拟币计算）
        /// </summary>
        public int CalculateTotalValue()
        {
            int totalValue = virtualCoinReward;
            
            foreach (var reward in rewards)
            {
                // 根据物品类型估算虚拟币价值
                int itemValue = reward.rewardType switch
                {
                    FirstChargeRewardType.Memory => 100 * reward.amount,  // 内存 100币/GB
                    FirstChargeRewardType.CPU => 200 * reward.amount,     // CPU 200币/核
                    FirstChargeRewardType.Storage => 50 * reward.amount,  // 存储 50币/50GB
                    FirstChargeRewardType.DataCleaner => 400 * reward.amount, // 数据清理工具 400币/个
                    FirstChargeRewardType.Skin => 500 * reward.amount,    // 皮肤 500币/个
                    FirstChargeRewardType.VIPCard => 1500 * reward.amount, // VIP卡 1500币/张
                    _ => 0
                };
                totalValue += itemValue;
            }
            
            return totalValue;
        }

        /// <summary>
        /// 计算性价比（总价值 / 实际价格）
        /// </summary>
        public float CalculateValueRatio()
        {
            if (price <= 0) return 0f;
            return CalculateTotalValue() / price;
        }

        /// <summary>
        /// 获取性价比描述
        /// </summary>
        public string GetValueRatioText()
        {
            float ratio = CalculateValueRatio();
            return $"价值 {CalculateTotalValue():N0} 虚拟币（相当于每$1获得 {ratio:F0} 币）";
        }

        /// <summary>
        /// 计算相比普通虚拟币包的优惠百分比
        /// </summary>
        public float CalculateDiscountPercent()
        {
            // 虚拟币小包：$0.99 = 1000币（基准）
            float baseRatio = 1000f / 0.99f; // 约1010币/美元
            float thisRatio = CalculateValueRatio();
            
            if (baseRatio <= 0) return 0f;
            return ((thisRatio - baseRatio) / baseRatio) * 100f;
        }

        // ==================== 显示辅助方法 ====================

        /// <summary>
        /// 获取详细信息（用于详情面板）
        /// </summary>
        public string GetDetailedInfo()
        {
            string info = $"=== {offerName} ===\n\n";
            info += $"{description}\n\n";
            info += $"价格: ${price:F2}\n";
            info += $"限制: 每账号仅可购买一次\n\n";
            info += "奖励内容:\n";
            info += GetRewardsText() + "\n\n";
            info += $"总价值: {CalculateTotalValue():N0} 虚拟币\n";
            info += $"相比普通礼包优惠: {CalculateDiscountPercent():F0}%\n\n";
            info += $"状态: {GetStatusText()}";
            
            if (isPurchased)
            {
                info += $"\n购买时间: {purchaseTime:yyyy-MM-dd HH:mm}";
            }
            
            if (isRewardClaimed)
            {
                info += $"\n领取时间: {claimTime:yyyy-MM-dd HH:mm}";
            }
            
            return info;
        }

        /// <summary>
        /// 获取简短描述（用于UI卡片）
        /// </summary>
        public string GetShortDescription()
        {
            return $"仅需${price:F2}\n" +
                   $"获得价值{CalculateTotalValue():N0}虚拟币的超值奖励！\n" +
                   $"限购一次，错过不再有！";
        }

        // ==================== 静态工厂方法 ====================

        /// <summary>
        /// 创建默认首充礼包
        /// 根据游戏设计.cs 第5.2节配置
        /// </summary>
        public static FirstChargeData CreateDefaultFirstCharge()
        {
            var firstCharge = new FirstChargeData
            {
                offerId = "first_charge",
                offerName = "超值首充礼包",
                price = 0.99f,
                description = "首次充值专享超值礼包！价值超过8000虚拟币的豪华奖励，仅此一次机会！",
                iconName = "icon_first_charge",
                bannerIconName = "banner_first_charge",
                virtualCoinReward = 5000,
                isHighlighted = true,
                displayPosition = "shop_banner",
                badgeText = "超值首充"
            };

            // 添加各类奖励物品
            firstCharge.AddReward(new FirstChargeReward(
                FirstChargeRewardType.Memory,
                "memory_10gb",
                "内存升级",
                10,
                "增加10GB内存",
                "icon_memory"
            ));

            firstCharge.AddReward(new FirstChargeReward(
                FirstChargeRewardType.CPU,
                "cpu_2core",
                "CPU升级",
                2,
                "增加2个CPU核心",
                "icon_cpu"
            ));

            firstCharge.AddReward(new FirstChargeReward(
                FirstChargeRewardType.Storage,
                "storage_200gb",
                "存储升级",
                4,
                "增加200GB存储空间",
                "icon_storage"
            ));

            firstCharge.AddReward(new FirstChargeReward(
                FirstChargeRewardType.DataCleaner,
                "data_cleaner_large",
                "数据清理工具(大)",
                5,
                "清除100GB数据 x5",
                "icon_cleaner_large"
            ));

            firstCharge.AddReward(new FirstChargeReward(
                FirstChargeRewardType.Skin,
                "exclusive_skin_newcomer",
                "专属新人皮肤",
                1,
                "独特的新人专属外观",
                "icon_skin_newcomer"
            ));

            firstCharge.AddReward(new FirstChargeReward(
                FirstChargeRewardType.VIPCard,
                "vip_trial_7days",
                "VIP体验卡",
                1,
                "7天VIP特权体验",
                "icon_vip_trial"
            ));

            return firstCharge;
        }

        /// <summary>
        /// 创建测试用首充礼包（价值较低，便于测试）
        /// </summary>
        public static FirstChargeData CreateTestFirstCharge()
        {
            var firstCharge = new FirstChargeData
            {
                offerId = "first_charge_test",
                offerName = "测试首充礼包",
                price = 0.01f, // 测试用低价
                description = "测试用首充礼包",
                iconName = "icon_first_charge",
                bannerIconName = "banner_first_charge",
                virtualCoinReward = 500,
                isHighlighted = true,
                displayPosition = "shop_banner",
                badgeText = "测试首充"
            };

            firstCharge.AddReward(new FirstChargeReward(
                FirstChargeRewardType.Memory,
                "memory_1gb",
                "内存升级",
                1,
                "增加1GB内存",
                "icon_memory"
            ));

            return firstCharge;
        }
    }

    /// <summary>
    /// 首充购买记录数据类
    /// </summary>
    [Serializable]
    public class FirstChargePurchaseRecord
    {
        /// <summary>玩家ID</summary>
        public string playerId;
        
        /// <summary>购买时间</summary>
        public DateTime purchaseTime;
        
        /// <summary>支付金额</summary>
        public float paidAmount;
        
        /// <summary>支付方式</summary>
        public string paymentMethod;
        
        /// <summary>交易ID</summary>
        public string transactionId;
        
        /// <summary>是否已发放奖励</summary>
        public bool rewardsGranted;
        
        /// <summary>奖励发放时间</summary>
        public DateTime rewardGrantTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FirstChargePurchaseRecord(string playerId, float amount, string method, string txId)
        {
            this.playerId = playerId;
            this.purchaseTime = DateTime.Now;
            this.paidAmount = amount;
            this.paymentMethod = method;
            this.transactionId = txId;
            this.rewardsGranted = false;
        }

        /// <summary>
        /// 标记奖励已发放
        /// </summary>
        public void MarkRewardsGranted()
        {
            rewardsGranted = true;
            rewardGrantTime = DateTime.Now;
        }

        /// <summary>
        /// 获取记录描述
        /// </summary>
        public string GetRecordText()
        {
            string status = rewardsGranted ? "[已发放]" : "[待发放]";
            return $"{status} {purchaseTime:yyyy-MM-dd HH:mm} - ${paidAmount:F2} ({paymentMethod}) - {transactionId}";
        }
    }
}
