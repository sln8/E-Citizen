using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * MonthlyCardData.cs - 月卡数据类
 * 
 * 功能说明：
 * 1. 定义月卡的数据结构
 * 2. 支持基础月卡和豪华月卡两种类型
 * 3. 管理月卡的每日奖励和额外福利
 * 4. 跟踪月卡状态（激活时间、到期时间、已领取天数）
 * 5. 提供月卡相关的计算和验证方法
 * 
 * Unity操作说明：
 * - 本脚本无需挂载到GameObject
 * - 通过MonthlyCardManager.Instance访问月卡数据
 * - 月卡购买后自动激活，每日登录领取奖励
 * 
 * 设计参考：
 * - 游戏设计.cs 第5.3节 月卡系统
 * - 基础月卡$4.99：每日200币，总计6000币，+1工作位
 * - 豪华月卡$9.99：每日500币，总计15000币，+2工作位，免费安全卫士
 */

namespace ECitizen.Data
{
    /// <summary>
    /// 月卡类型枚举
    /// </summary>
    public enum MonthlyCardType
    {
        None,               // 无月卡
        Basic,              // 基础月卡（$4.99）
        Premium             // 豪华月卡（$9.99）
    }

    /// <summary>
    /// 月卡数据类
    /// 存储月卡的所有信息和状态
    /// </summary>
    [Serializable]
    public class MonthlyCardData
    {
        // ==================== 基本信息 ====================
        
        /// <summary>月卡唯一ID</summary>
        public string cardId;
        
        /// <summary>月卡名称</summary>
        public string cardName;
        
        /// <summary>月卡类型</summary>
        public MonthlyCardType cardType;
        
        /// <summary>价格（美元）</summary>
        public float price;
        
        /// <summary>持续天数</summary>
        public int durationDays;
        
        /// <summary>月卡描述</summary>
        public string description;
        
        /// <summary>图标名称</summary>
        public string iconName;
        
        // ==================== 奖励配置 ====================
        
        /// <summary>每日虚拟币奖励</summary>
        public int dailyVirtualCoin;
        
        /// <summary>总虚拟币奖励（30天）</summary>
        public int totalVirtualCoin;
        
        /// <summary>额外工作位数量</summary>
        public int extraWorkSlots;
        
        /// <summary>每日数据清理工具数量</summary>
        public int dailyDataCleaners;
        
        /// <summary>是否包含免费基础安全卫士</summary>
        public bool includesBasicSecurity;
        
        /// <summary>是否包含专属皮肤</summary>
        public bool includesExclusiveSkin;
        
        /// <summary>专属皮肤ID（如果有）</summary>
        public string exclusiveSkinId;
        
        // ==================== 状态信息 ====================
        
        /// <summary>是否已激活</summary>
        public bool isActive;
        
        /// <summary>购买时间</summary>
        public DateTime purchaseTime;
        
        /// <summary>激活时间</summary>
        public DateTime activationTime;
        
        /// <summary>到期时间</summary>
        public DateTime expirationTime;
        
        /// <summary>已领取天数</summary>
        public int claimedDays;
        
        /// <summary>最后领取时间</summary>
        public DateTime lastClaimTime;
        
        // ==================== 构造函数 ====================

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MonthlyCardData()
        {
            durationDays = 30;
            claimedDays = 0;
            isActive = false;
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        public MonthlyCardData(MonthlyCardType type, DateTime purchaseTime)
        {
            this.cardType = type;
            this.purchaseTime = purchaseTime;
            this.activationTime = purchaseTime;
            this.expirationTime = purchaseTime.AddDays(30);
            this.isActive = true;
            this.claimedDays = 0;
            this.lastClaimTime = DateTime.MinValue;
            
            // 根据类型设置属性
            InitializeByType(type);
        }

        // ==================== 初始化方法 ====================

        /// <summary>
        /// 根据类型初始化月卡属性
        /// </summary>
        private void InitializeByType(MonthlyCardType type)
        {
            switch (type)
            {
                case MonthlyCardType.Basic:
                    cardId = "monthly_basic";
                    cardName = "基础月卡";
                    price = 4.99f;
                    durationDays = 30;
                    dailyVirtualCoin = 200;
                    totalVirtualCoin = 6000;
                    extraWorkSlots = 1;
                    dailyDataCleaners = 1;
                    includesBasicSecurity = false;
                    includesExclusiveSkin = false;
                    iconName = "icon_monthly_basic";
                    description = "每日领取200虚拟币，额外1个工作位，每日1个数据清理工具";
                    break;

                case MonthlyCardType.Premium:
                    cardId = "monthly_premium";
                    cardName = "豪华月卡";
                    price = 9.99f;
                    durationDays = 30;
                    dailyVirtualCoin = 500;
                    totalVirtualCoin = 15000;
                    extraWorkSlots = 2;
                    dailyDataCleaners = 3;
                    includesBasicSecurity = true;
                    includesExclusiveSkin = true;
                    exclusiveSkinId = "skin_monthly_premium";
                    iconName = "icon_monthly_premium";
                    description = "每日领取500虚拟币，额外2个工作位，每日3个数据清理工具，免费基础安全卫士，专属皮肤";
                    break;

                default:
                    cardId = "monthly_none";
                    cardName = "无月卡";
                    break;
            }
        }

        // ==================== 状态检查方法 ====================

        /// <summary>
        /// 检查月卡是否有效
        /// </summary>
        public bool IsValid()
        {
            if (!isActive) return false;
            return DateTime.Now < expirationTime;
        }

        /// <summary>
        /// 获取剩余天数
        /// </summary>
        public int GetRemainingDays()
        {
            if (!IsValid()) return 0;
            
            TimeSpan remaining = expirationTime - DateTime.Now;
            return Math.Max(0, (int)Math.Ceiling(remaining.TotalDays));
        }

        /// <summary>
        /// 检查今天是否已领取
        /// </summary>
        public bool HasClaimedToday()
        {
            if (lastClaimTime == DateTime.MinValue) return false;
            
            // 检查最后领取时间是否是今天
            DateTime today = DateTime.Now.Date;
            DateTime lastClaim = lastClaimTime.Date;
            return lastClaim == today;
        }

        /// <summary>
        /// 检查是否可以领取
        /// </summary>
        public bool CanClaim()
        {
            return IsValid() && !HasClaimedToday();
        }

        /// <summary>
        /// 获取距离到期的时间描述
        /// </summary>
        public string GetExpirationTimeText()
        {
            if (!IsValid()) return "已过期";
            
            int remainingDays = GetRemainingDays();
            if (remainingDays == 0)
            {
                TimeSpan remaining = expirationTime - DateTime.Now;
                return $"{remaining.Hours}小时后到期";
            }
            else
            {
                return $"{remainingDays}天后到期";
            }
        }

        /// <summary>
        /// 获取下次可领取时间描述
        /// </summary>
        public string GetNextClaimTimeText()
        {
            if (!IsValid()) return "月卡已过期";
            if (!HasClaimedToday()) return "现在可领取";
            
            // 计算明天0点的时间
            DateTime tomorrow = DateTime.Now.Date.AddDays(1);
            TimeSpan until = tomorrow - DateTime.Now;
            return $"{until.Hours}小时{until.Minutes}分钟后可领取";
        }

        // ==================== 领取奖励方法 ====================

        /// <summary>
        /// 领取每日奖励
        /// 返回是否领取成功
        /// </summary>
        public bool ClaimDailyReward()
        {
            if (!CanClaim())
            {
                Debug.LogWarning($"月卡 {cardName} 今天已经领取过或已过期");
                return false;
            }

            claimedDays++;
            lastClaimTime = DateTime.Now;
            
            Debug.Log($"成功领取月卡 {cardName} 每日奖励: {dailyVirtualCoin} 虚拟币");
            return true;
        }

        // ==================== 福利获取方法 ====================

        /// <summary>
        /// 获取所有福利描述
        /// </summary>
        public List<string> GetBenefitsList()
        {
            var benefits = new List<string>();
            
            benefits.Add($"每日领取 {dailyVirtualCoin} 虚拟币");
            benefits.Add($"30天总计 {totalVirtualCoin} 虚拟币");
            
            if (extraWorkSlots > 0)
            {
                benefits.Add($"额外 {extraWorkSlots} 个工作位");
            }
            
            if (dailyDataCleaners > 0)
            {
                benefits.Add($"每日 {dailyDataCleaners} 个数据清理工具");
            }
            
            if (includesBasicSecurity)
            {
                benefits.Add("免费基础安全卫士");
            }
            
            if (includesExclusiveSkin)
            {
                benefits.Add("专属皮肤");
            }
            
            return benefits;
        }

        /// <summary>
        /// 获取福利描述文本（换行分隔）
        /// </summary>
        public string GetBenefitsText()
        {
            return string.Join("\n", GetBenefitsList());
        }

        // ==================== 价值计算方法 ====================

        /// <summary>
        /// 计算性价比（每美元获得的虚拟币）
        /// </summary>
        public float CalculateValueRatio()
        {
            if (price <= 0) return 0f;
            return totalVirtualCoin / price;
        }

        /// <summary>
        /// 获取性价比描述
        /// </summary>
        public string GetValueRatioText()
        {
            float ratio = CalculateValueRatio();
            return $"每$1可获得 {ratio:F0} 虚拟币";
        }

        /// <summary>
        /// 计算相比直接购买虚拟币的优惠百分比
        /// 假设虚拟币中包（$4.99=6000币）为基准
        /// </summary>
        public float CalculateDiscountPercent()
        {
            // 基准：虚拟币中包 $4.99 = 6500虚拟币（含赠送）
            float baseRatio = 6500f / 4.99f; // 约1302币/美元
            float thisRatio = CalculateValueRatio();
            
            if (baseRatio <= 0) return 0f;
            return ((thisRatio - baseRatio) / baseRatio) * 100f;
        }

        // ==================== 显示辅助方法 ====================

        /// <summary>
        /// 获取月卡摘要信息（用于UI显示）
        /// </summary>
        public string GetSummaryText()
        {
            if (!IsValid())
            {
                return $"{cardName} - 未激活";
            }

            return $"{cardName}\n" +
                   $"剩余: {GetRemainingDays()} 天\n" +
                   $"已领取: {claimedDays}/{durationDays} 天\n" +
                   $"{GetNextClaimTimeText()}";
        }

        /// <summary>
        /// 获取详细信息（用于详情面板）
        /// </summary>
        public string GetDetailedInfo()
        {
            string info = $"=== {cardName} ===\n\n";
            info += $"价格: ${price:F2}\n";
            info += $"有效期: {durationDays} 天\n\n";
            info += "福利内容:\n";
            info += GetBenefitsText() + "\n\n";
            
            if (IsValid())
            {
                info += $"状态: 已激活\n";
                info += $"激活时间: {activationTime:yyyy-MM-dd HH:mm}\n";
                info += $"到期时间: {expirationTime:yyyy-MM-dd HH:mm}\n";
                info += $"剩余天数: {GetRemainingDays()} 天\n";
                info += $"已领取天数: {claimedDays}/{durationDays}\n";
                info += $"下次领取: {GetNextClaimTimeText()}\n";
            }
            else
            {
                info += "状态: 未激活\n";
            }
            
            info += $"\n性价比: {GetValueRatioText()}";
            
            return info;
        }

        // ==================== 静态工厂方法 ====================

        /// <summary>
        /// 创建基础月卡
        /// </summary>
        public static MonthlyCardData CreateBasicCard()
        {
            var card = new MonthlyCardData();
            card.InitializeByType(MonthlyCardType.Basic);
            return card;
        }

        /// <summary>
        /// 创建豪华月卡
        /// </summary>
        public static MonthlyCardData CreatePremiumCard()
        {
            var card = new MonthlyCardData();
            card.InitializeByType(MonthlyCardType.Premium);
            return card;
        }

        /// <summary>
        /// 获取所有月卡选项（用于商店展示）
        /// </summary>
        public static List<MonthlyCardData> GetAllCardOptions()
        {
            return new List<MonthlyCardData>
            {
                CreateBasicCard(),
                CreatePremiumCard()
            };
        }

        // ==================== 比较方法 ====================

        /// <summary>
        /// 比较两个月卡的价值
        /// 返回 1 表示当前月卡更优，-1 表示另一个更优，0 表示相同
        /// </summary>
        public int CompareValue(MonthlyCardData other)
        {
            if (other == null) return 1;
            
            float thisRatio = this.CalculateValueRatio();
            float otherRatio = other.CalculateValueRatio();
            
            if (thisRatio > otherRatio) return 1;
            if (thisRatio < otherRatio) return -1;
            return 0;
        }
    }

    /// <summary>
    /// 月卡领取记录数据类
    /// 用于记录每次领取的详细信息
    /// </summary>
    [Serializable]
    public class MonthlyCardClaimRecord
    {
        /// <summary>领取时间</summary>
        public DateTime claimTime;
        
        /// <summary>月卡类型</summary>
        public MonthlyCardType cardType;
        
        /// <summary>领取的虚拟币数量</summary>
        public int virtualCoinAmount;
        
        /// <summary>领取的数据清理工具数量</summary>
        public int dataCleanerAmount;
        
        /// <summary>是否是首次领取</summary>
        public bool isFirstClaim;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MonthlyCardClaimRecord(MonthlyCardType cardType, int coinAmount, int cleanerAmount, bool isFirst = false)
        {
            this.claimTime = DateTime.Now;
            this.cardType = cardType;
            this.virtualCoinAmount = coinAmount;
            this.dataCleanerAmount = cleanerAmount;
            this.isFirstClaim = isFirst;
        }

        /// <summary>
        /// 获取记录描述
        /// </summary>
        public string GetRecordText()
        {
            string typeText = cardType == MonthlyCardType.Basic ? "基础月卡" : "豪华月卡";
            string firstText = isFirstClaim ? "[首次]" : "";
            return $"{claimTime:yyyy-MM-dd HH:mm} {firstText}{typeText} - {virtualCoinAmount}币 + {dataCleanerAmount}工具";
        }
    }
}
