using System;
using System.Collections.Generic;
using UnityEngine;
using ECitizen.Data;

/*
 * MonthlyCardManager.cs - 月卡管理器
 * 
 * 功能说明：
 * 1. 管理月卡购买、激活和续费
 * 2. 处理每日奖励领取
 * 3. 检查月卡到期和自动续期提醒
 * 4. 应用月卡额外福利（工作位、安全卫士等）
 * 5. 与GameTimerManager集成，实现每日奖励自动发放
 * 6. 提供完整的事件系统
 * 
 * Unity操作说明：
 * 1. 在Hierarchy中创建空物体"MonthlyCardManager"
 * 2. 添加此脚本组件
 * 3. 确保PaymentManager、ResourceManager、GameTimerManager已初始化
 * 4. 通过MonthlyCardManager.Instance访问月卡功能
 * 
 * 使用示例：
 * // 购买基础月卡
 * MonthlyCardManager.Instance.PurchaseCard(MonthlyCardType.Basic, OnPurchaseComplete);
 * 
 * // 领取每日奖励
 * bool success = MonthlyCardManager.Instance.ClaimDailyReward();
 * 
 * // 检查月卡状态
 * MonthlyCardData currentCard = MonthlyCardManager.Instance.GetCurrentCard();
 * 
 * // 监听月卡事件
 * MonthlyCardManager.Instance.OnCardActivated += OnCardActivated;
 * MonthlyCardManager.Instance.OnDailyRewardClaimed += OnRewardClaimed;
 */

namespace ECitizen.Managers
{
    /// <summary>
    /// 月卡管理器
    /// 单例模式，管理月卡相关功能
    /// </summary>
    public class MonthlyCardManager : MonoBehaviour
    {
        // ==================== 单例模式 ====================
        
        private static MonthlyCardManager _instance;
        public static MonthlyCardManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MonthlyCardManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("MonthlyCardManager");
                        _instance = go.AddComponent<MonthlyCardManager>();
                    }
                }
                return _instance;
            }
        }

        // ==================== 事件定义 ====================
        
        /// <summary>月卡购买成功事件</summary>
        public event Action<MonthlyCardData> OnCardPurchased;
        
        /// <summary>月卡激活事件</summary>
        public event Action<MonthlyCardData> OnCardActivated;
        
        /// <summary>每日奖励领取成功事件</summary>
        public event Action<MonthlyCardData, int, int> OnDailyRewardClaimed; // (月卡, 虚拟币, 清理工具)
        
        /// <summary>月卡到期事件</summary>
        public event Action<MonthlyCardData> OnCardExpired;
        
        /// <summary>月卡即将到期提醒事件（剩余3天）</summary>
        public event Action<MonthlyCardData, int> OnCardExpiringSoon; // (月卡, 剩余天数)

        // ==================== 月卡数据 ====================
        
        /// <summary>当前激活的月卡</summary>
        private MonthlyCardData currentCard;
        
        /// <summary>月卡领取记录</summary>
        private List<MonthlyCardClaimRecord> claimHistory = new List<MonthlyCardClaimRecord>();
        
        /// <summary>月卡购买历史</summary>
        private List<MonthlyCardData> purchaseHistory = new List<MonthlyCardData>();

        // ==================== 统计数据 ====================
        
        /// <summary>总购买月卡次数</summary>
        public int TotalCardsPurchased { get; private set; }
        
        /// <summary>总领取虚拟币数</summary>
        public int TotalCoinsFromCards { get; private set; }
        
        /// <summary>总领取数据清理工具数</summary>
        public int TotalCleanersFromCards { get; private set; }
        
        /// <summary>当前连续领取天数</summary>
        public int CurrentStreak { get; private set; }

        // ==================== Unity生命周期 ====================

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeCardSystem();
        }

        private void Start()
        {
            // 订阅游戏周期事件，用于检查到期
            if (GameTimerManager.Instance != null)
            {
                GameTimerManager.Instance.OnCycleCompleted += CheckCardExpiration;
            }
        }

        private void OnDestroy()
        {
            // 取消订阅
            if (GameTimerManager.Instance != null)
            {
                GameTimerManager.Instance.OnCycleCompleted -= CheckCardExpiration;
            }
        }

        // ==================== 初始化方法 ====================

        /// <summary>
        /// 初始化月卡系统
        /// </summary>
        private void InitializeCardSystem()
        {
            Debug.Log("=== 月卡系统初始化 ===");
            
            // TODO: 从Firebase或本地存储加载月卡数据
            LoadCardData();
            
            // 检查当前月卡状态
            if (currentCard != null)
            {
                if (currentCard.IsValid())
                {
                    Debug.Log($"当前月卡: {currentCard.cardName}");
                    Debug.Log($"  - 剩余天数: {currentCard.GetRemainingDays()}");
                    Debug.Log($"  - 已领取: {currentCard.claimedDays}/{currentCard.durationDays} 天");
                }
                else
                {
                    Debug.Log("当前月卡已过期");
                    HandleCardExpiration();
                }
            }
            else
            {
                Debug.Log("当前无激活月卡");
            }
        }

        // ==================== 月卡购买方法 ====================

        /// <summary>
        /// 购买月卡
        /// </summary>
        /// <param name="cardType">月卡类型</param>
        /// <param name="callback">完成回调</param>
        public void PurchaseCard(MonthlyCardType cardType, Action<bool, string> callback = null)
        {
            // 检查是否已有激活的月卡
            if (HasActiveCard())
            {
                Debug.LogWarning("已有激活的月卡，请等待到期后再购买");
                callback?.Invoke(false, "已有激活的月卡");
                return;
            }

            // 创建月卡数据
            var newCard = cardType == MonthlyCardType.Basic ? 
                MonthlyCardData.CreateBasicCard() : 
                MonthlyCardData.CreatePremiumCard();

            // 通过PaymentManager处理支付
            if (PaymentManager.Instance == null)
            {
                Debug.LogError("PaymentManager未初始化");
                callback?.Invoke(false, "支付系统未就绪");
                return;
            }

            Debug.Log($"开始购买月卡: {newCard.cardName}");

            PaymentManager.Instance.ProcessPayment(newCard.cardId, newCard.price, (success, transactionId) =>
            {
                if (success)
                {
                    // 激活月卡
                    ActivateCard(newCard);
                    
                    // 记录购买
                    purchaseHistory.Add(newCard);
                    TotalCardsPurchased++;
                    
                    Debug.Log($"月卡购买成功: {newCard.cardName}");
                    
                    // 触发事件
                    OnCardPurchased?.Invoke(newCard);
                    
                    // 发送邮件通知（可选）
                    SendCardPurchaseNotification(newCard);
                    
                    callback?.Invoke(true, "购买成功");
                }
                else
                {
                    Debug.LogWarning($"月卡购买失败: {newCard.cardName}");
                    callback?.Invoke(false, "支付失败");
                }
            });
        }

        /// <summary>
        /// 激活月卡
        /// </summary>
        private void ActivateCard(MonthlyCardData card)
        {
            // 设置激活时间
            card.activationTime = DateTime.Now;
            card.expirationTime = DateTime.Now.AddDays(card.durationDays);
            card.isActive = true;
            
            // 设置为当前月卡
            currentCard = card;
            
            Debug.Log($"月卡已激活: {card.cardName}");
            Debug.Log($"  - 激活时间: {card.activationTime:yyyy-MM-dd HH:mm}");
            Debug.Log($"  - 到期时间: {card.expirationTime:yyyy-MM-dd HH:mm}");
            
            // 应用月卡福利
            ApplyCardBenefits(card);
            
            // 触发激活事件
            OnCardActivated?.Invoke(card);
            
            // 保存数据
            SaveCardData();
        }

        // ==================== 每日奖励方法 ====================

        /// <summary>
        /// 领取每日奖励
        /// </summary>
        public bool ClaimDailyReward()
        {
            if (currentCard == null || !currentCard.IsValid())
            {
                Debug.LogWarning("没有激活的月卡");
                return false;
            }

            if (!currentCard.CanClaim())
            {
                Debug.LogWarning("今天已经领取过或月卡已过期");
                return false;
            }

            // 执行领取
            bool success = currentCard.ClaimDailyReward();
            if (!success)
            {
                return false;
            }

            // 发放虚拟币
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.AddVirtualCoin(
                    currentCard.dailyVirtualCoin, 
                    $"{currentCard.cardName}每日奖励"
                );
            }

            // 发放数据清理工具（通过邮件）
            SendDataCleanerReward(currentCard.dailyDataCleaners);

            // 更新统计
            TotalCoinsFromCards += currentCard.dailyVirtualCoin;
            TotalCleanersFromCards += currentCard.dailyDataCleaners;
            UpdateStreak();

            // 记录领取
            var record = new MonthlyCardClaimRecord(
                currentCard.cardType,
                currentCard.dailyVirtualCoin,
                currentCard.dailyDataCleaners,
                currentCard.claimedDays == 1 // 是否是首次领取
            );
            claimHistory.Add(record);

            Debug.Log($"成功领取月卡每日奖励:");
            Debug.Log($"  - 虚拟币: {currentCard.dailyVirtualCoin}");
            Debug.Log($"  - 数据清理工具: {currentCard.dailyDataCleaners}");
            Debug.Log($"  - 已领取天数: {currentCard.claimedDays}/{currentCard.durationDays}");

            // 触发事件
            OnDailyRewardClaimed?.Invoke(currentCard, currentCard.dailyVirtualCoin, currentCard.dailyDataCleaners);

            // 保存数据
            SaveCardData();

            return true;
        }

        /// <summary>
        /// 尝试自动领取每日奖励（登录时调用）
        /// </summary>
        public void TryAutoClaimDailyReward()
        {
            if (HasActiveCard() && currentCard.CanClaim())
            {
                Debug.Log("检测到可领取的月卡奖励，尝试自动领取...");
                ClaimDailyReward();
            }
        }

        // ==================== 月卡福利应用 ====================

        /// <summary>
        /// 应用月卡福利
        /// </summary>
        private void ApplyCardBenefits(MonthlyCardData card)
        {
            Debug.Log($"应用月卡福利: {card.cardName}");

            // 1. 额外工作位（与JobManager集成）
            if (card.extraWorkSlots > 0)
            {
                if (JobManager.Instance != null)
                {
                    // TODO: JobManager需要添加月卡工作位支持
                    Debug.Log($"  - 额外工作位: +{card.extraWorkSlots}");
                }
            }

            // 2. 免费基础安全卫士（与SecurityManager集成）
            if (card.includesBasicSecurity)
            {
                if (SecurityManager.Instance != null)
                {
                    // TODO: SecurityManager需要添加月卡免费支持
                    Debug.Log("  - 免费基础安全卫士");
                }
            }

            // 3. 专属皮肤（与ShopManager集成）
            if (card.includesExclusiveSkin && !string.IsNullOrEmpty(card.exclusiveSkinId))
            {
                // TODO: 通过ShopManager或玩家数据系统解锁皮肤
                Debug.Log($"  - 专属皮肤: {card.exclusiveSkinId}");
            }
        }

        /// <summary>
        /// 移除月卡福利（到期时调用）
        /// </summary>
        private void RemoveCardBenefits(MonthlyCardData card)
        {
            Debug.Log($"移除月卡福利: {card.cardName}");

            // 1. 移除额外工作位
            if (card.extraWorkSlots > 0)
            {
                // TODO: 通知JobManager移除额外工作位
                Debug.Log($"  - 移除额外工作位: {card.extraWorkSlots}");
            }

            // 2. 取消免费安全卫士
            if (card.includesBasicSecurity)
            {
                // TODO: 通知SecurityManager取消免费订阅
                Debug.Log("  - 取消免费基础安全卫士");
            }

            // 注意：专属皮肤购买后永久保留，不会移除
        }

        // ==================== 到期检查方法 ====================

        /// <summary>
        /// 检查月卡是否到期
        /// 由GameTimerManager每周期调用
        /// </summary>
        private void CheckCardExpiration(int cycleCount)
        {
            if (currentCard == null || !currentCard.isActive)
            {
                return;
            }

            // 检查是否到期
            if (!currentCard.IsValid())
            {
                HandleCardExpiration();
                return;
            }

            // 检查即将到期（剩余3天）
            int remainingDays = currentCard.GetRemainingDays();
            if (remainingDays <= 3 && remainingDays > 0)
            {
                Debug.Log($"月卡即将到期: {currentCard.cardName}，剩余 {remainingDays} 天");
                OnCardExpiringSoon?.Invoke(currentCard, remainingDays);
                
                // 发送提醒通知
                SendExpirationReminder(remainingDays);
            }
        }

        /// <summary>
        /// 处理月卡到期
        /// </summary>
        private void HandleCardExpiration()
        {
            if (currentCard == null) return;

            Debug.Log($"月卡已到期: {currentCard.cardName}");
            
            // 移除福利
            RemoveCardBenefits(currentCard);
            
            // 触发到期事件
            OnCardExpired?.Invoke(currentCard);
            
            // 发送到期通知
            SendExpirationNotification();
            
            // 重置当前月卡
            currentCard.isActive = false;
            currentCard = null;
            CurrentStreak = 0;
            
            // 保存数据
            SaveCardData();
        }

        // ==================== 状态查询方法 ====================

        /// <summary>
        /// 检查是否有激活的月卡
        /// </summary>
        public bool HasActiveCard()
        {
            return currentCard != null && currentCard.IsValid();
        }

        /// <summary>
        /// 获取当前月卡
        /// </summary>
        public MonthlyCardData GetCurrentCard()
        {
            return currentCard;
        }

        /// <summary>
        /// 获取月卡领取历史
        /// </summary>
        public List<MonthlyCardClaimRecord> GetClaimHistory()
        {
            return new List<MonthlyCardClaimRecord>(claimHistory);
        }

        /// <summary>
        /// 获取月卡购买历史
        /// </summary>
        public List<MonthlyCardData> GetPurchaseHistory()
        {
            return new List<MonthlyCardData>(purchaseHistory);
        }

        // ==================== 辅助方法 ====================

        /// <summary>
        /// 更新连续领取天数
        /// </summary>
        private void UpdateStreak()
        {
            if (currentCard != null && currentCard.claimedDays > 0)
            {
                // 检查是否连续领取
                if (currentCard.lastClaimTime.Date == DateTime.Now.Date.AddDays(-1) ||
                    currentCard.lastClaimTime.Date == DateTime.Now.Date)
                {
                    CurrentStreak++;
                }
                else
                {
                    CurrentStreak = 1;
                }
            }
        }

        /// <summary>
        /// 发送数据清理工具奖励（通过邮件）
        /// </summary>
        private void SendDataCleanerReward(int amount)
        {
            if (MailManager.Instance != null && amount > 0)
            {
                // TODO: 创建包含数据清理工具的邮件
                Debug.Log($"通过邮件发送 {amount} 个数据清理工具");
            }
        }

        /// <summary>
        /// 发送月卡购买通知
        /// </summary>
        private void SendCardPurchaseNotification(MonthlyCardData card)
        {
            if (MailManager.Instance != null)
            {
                // TODO: 发送购买成功邮件
                Debug.Log($"发送月卡购买通知: {card.cardName}");
            }
        }

        /// <summary>
        /// 发送到期提醒通知
        /// </summary>
        private void SendExpirationReminder(int daysLeft)
        {
            if (MailManager.Instance != null)
            {
                // TODO: 发送即将到期提醒邮件
                Debug.Log($"发送到期提醒: 剩余 {daysLeft} 天");
            }
        }

        /// <summary>
        /// 发送到期通知
        /// </summary>
        private void SendExpirationNotification()
        {
            if (MailManager.Instance != null)
            {
                // TODO: 发送月卡到期通知邮件
                Debug.Log("发送月卡到期通知");
            }
        }

        // ==================== 调试方法 ====================

        /// <summary>
        /// 打印月卡统计信息
        /// </summary>
        public void PrintCardStatistics()
        {
            Debug.Log("=== 月卡统计信息 ===");
            Debug.Log($"总购买次数: {TotalCardsPurchased}");
            Debug.Log($"总领取虚拟币: {TotalCoinsFromCards:N0}");
            Debug.Log($"总领取清理工具: {TotalCleanersFromCards}");
            Debug.Log($"当前连续领取: {CurrentStreak} 天");
            
            if (currentCard != null && currentCard.IsValid())
            {
                Debug.Log($"\n当前月卡:");
                Debug.Log($"  类型: {currentCard.cardName}");
                Debug.Log($"  剩余天数: {currentCard.GetRemainingDays()}");
                Debug.Log($"  已领取: {currentCard.claimedDays}/{currentCard.durationDays}");
                Debug.Log($"  下次领取: {currentCard.GetNextClaimTimeText()}");
            }
            else
            {
                Debug.Log("\n当前无激活月卡");
            }
        }

        // ==================== 数据持久化接口 ====================

        /// <summary>
        /// 保存月卡数据
        /// </summary>
        private void SaveCardData()
        {
            // TODO: 实现保存到Firebase或本地存储
            Debug.Log("保存月卡数据...");
        }

        /// <summary>
        /// 加载月卡数据
        /// </summary>
        private void LoadCardData()
        {
            // TODO: 实现从Firebase或本地存储加载
            Debug.Log("加载月卡数据...");
        }
    }
}
