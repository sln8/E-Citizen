using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ECitizen.Data;
using ECitizen.Managers;

/*
 * Phase8TestUI.cs - Phase 8 综合测试UI
 * 
 * 功能说明：
 * 本UI用于测试Phase 8的所有功能：
 * 1. 商城系统测试
 * 2. 月卡系统测试
 * 3. 首充系统测试
 * 4. 支付系统测试
 * 5. 实时状态显示
 * 6. 事件监听和响应
 * 
 * Unity操作步骤：
 * 1. 创建Canvas（如果还没有）
 * 2. 在Canvas下创建Panel作为主面板
 * 3. 添加此脚本到主面板
 * 4. 创建以下UI元素并连接引用：
 *    - statusText: 显示系统状态的Text组件
 *    - logText: 显示操作日志的Text组件
 *    - 20个测试按钮（见下方按钮说明）
 * 5. 运行游戏，点击按钮测试各功能
 * 
 * 按钮列表（按功能分组）：
 * 
 * 【商城系统测试】
 * 1. btnShowAllItems - 显示所有商品
 * 2. btnShowConfigItems - 显示配置升级商品
 * 3. btnShowCoinPacks - 显示虚拟币礼包
 * 4. btnBuyMemory - 购买内存升级
 * 5. btnBuyCPU - 购买CPU升级
 * 6. btnBuySmallPack - 购买虚拟币小包
 * 7. btnShowPurchaseHistory - 显示购买历史
 * 
 * 【月卡系统测试】
 * 8. btnBuyBasicCard - 购买基础月卡
 * 9. btnBuyPremiumCard - 购买豪华月卡
 * 10. btnClaimDaily - 领取每日奖励
 * 11. btnCheckCardStatus - 检查月卡状态
 * 12. btnShowClaimHistory - 显示领取历史
 * 
 * 【首充系统测试】
 * 13. btnShowFirstCharge - 显示首充信息
 * 14. btnBuyFirstCharge - 购买首充礼包
 * 15. btnClaimFirstCharge - 领取首充奖励
 * 
 * 【支付系统测试】
 * 16. btnShowPaymentStats - 显示支付统计
 * 17. btnToggleTestMode - 切换测试模式
 * 18. btnSimulateFailure - 模拟支付失败
 * 
 * 【综合测试】
 * 19. btnRefreshStatus - 刷新状态显示
 * 20. btnClearLog - 清空日志
 */

namespace ECitizen.UI
{
    public class Phase8TestUI : MonoBehaviour
    {
        // ==================== UI组件引用 ====================
        
        [Header("状态显示")]
        public TextMeshProUGUI statusText;
        public TextMeshProUGUI logText;
        public ScrollRect logScrollRect;
        
        [Header("商城测试按钮")]
        public Button btnShowAllItems;
        public Button btnShowConfigItems;
        public Button btnShowCoinPacks;
        public Button btnBuyMemory;
        public Button btnBuyCPU;
        public Button btnBuySmallPack;
        public Button btnShowPurchaseHistory;
        
        [Header("月卡测试按钮")]
        public Button btnBuyBasicCard;
        public Button btnBuyPremiumCard;
        public Button btnClaimDaily;
        public Button btnCheckCardStatus;
        public Button btnShowClaimHistory;
        
        [Header("首充测试按钮")]
        public Button btnShowFirstCharge;
        public Button btnBuyFirstCharge;
        public Button btnClaimFirstCharge;
        
        [Header("支付测试按钮")]
        public Button btnShowPaymentStats;
        public Button btnToggleTestMode;
        public Button btnSimulateFailure;
        
        [Header("通用按钮")]
        public Button btnRefreshStatus;
        public Button btnClearLog;

        // ==================== 私有变量 ====================
        
        private FirstChargeData firstCharge;
        private List<string> logMessages = new List<string>();
        private const int MAX_LOG_MESSAGES = 100;
        
        private float updateInterval = 0.5f;
        private float nextUpdateTime = 0f;

        // ==================== Unity生命周期 ====================

        private void Start()
        {
            InitializeUI();
            SubscribeToEvents();
            RefreshStatus();
            
            // 初始化首充数据
            firstCharge = FirstChargeData.CreateDefaultFirstCharge();
            
            AddLog("=== Phase 8 测试UI已启动 ===");
            AddLog("提示：点击按钮测试各项功能");
        }

        private void Update()
        {
            // 定时刷新状态显示
            if (Time.time >= nextUpdateTime)
            {
                RefreshStatus();
                nextUpdateTime = Time.time + updateInterval;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        // ==================== 初始化方法 ====================

        private void InitializeUI()
        {
            // 商城测试按钮
            if (btnShowAllItems) btnShowAllItems.onClick.AddListener(OnShowAllItems);
            if (btnShowConfigItems) btnShowConfigItems.onClick.AddListener(OnShowConfigItems);
            if (btnShowCoinPacks) btnShowCoinPacks.onClick.AddListener(OnShowCoinPacks);
            if (btnBuyMemory) btnBuyMemory.onClick.AddListener(OnBuyMemory);
            if (btnBuyCPU) btnBuyCPU.onClick.AddListener(OnBuyCPU);
            if (btnBuySmallPack) btnBuySmallPack.onClick.AddListener(OnBuySmallPack);
            if (btnShowPurchaseHistory) btnShowPurchaseHistory.onClick.AddListener(OnShowPurchaseHistory);
            
            // 月卡测试按钮
            if (btnBuyBasicCard) btnBuyBasicCard.onClick.AddListener(OnBuyBasicCard);
            if (btnBuyPremiumCard) btnBuyPremiumCard.onClick.AddListener(OnBuyPremiumCard);
            if (btnClaimDaily) btnClaimDaily.onClick.AddListener(OnClaimDaily);
            if (btnCheckCardStatus) btnCheckCardStatus.onClick.AddListener(OnCheckCardStatus);
            if (btnShowClaimHistory) btnShowClaimHistory.onClick.AddListener(OnShowClaimHistory);
            
            // 首充测试按钮
            if (btnShowFirstCharge) btnShowFirstCharge.onClick.AddListener(OnShowFirstCharge);
            if (btnBuyFirstCharge) btnBuyFirstCharge.onClick.AddListener(OnBuyFirstCharge);
            if (btnClaimFirstCharge) btnClaimFirstCharge.onClick.AddListener(OnClaimFirstCharge);
            
            // 支付测试按钮
            if (btnShowPaymentStats) btnShowPaymentStats.onClick.AddListener(OnShowPaymentStats);
            if (btnToggleTestMode) btnToggleTestMode.onClick.AddListener(OnToggleTestMode);
            if (btnSimulateFailure) btnSimulateFailure.onClick.AddListener(OnSimulateFailure);
            
            // 通用按钮
            if (btnRefreshStatus) btnRefreshStatus.onClick.AddListener(() => RefreshStatus());
            if (btnClearLog) btnClearLog.onClick.AddListener(OnClearLog);
        }

        // ==================== 事件订阅 ====================

        private void SubscribeToEvents()
        {
            // 商城事件
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OnItemPurchased += OnItemPurchased;
                ShopManager.Instance.OnPurchaseFailed += OnPurchaseFailed;
                ShopManager.Instance.OnItemApplied += OnItemApplied;
            }
            
            // 月卡事件
            if (MonthlyCardManager.Instance != null)
            {
                MonthlyCardManager.Instance.OnCardPurchased += OnCardPurchased;
                MonthlyCardManager.Instance.OnCardActivated += OnCardActivated;
                MonthlyCardManager.Instance.OnDailyRewardClaimed += OnDailyRewardClaimed;
                MonthlyCardManager.Instance.OnCardExpired += OnCardExpired;
            }
            
            // 支付事件
            if (PaymentManager.Instance != null)
            {
                PaymentManager.Instance.OnPaymentSuccess += OnPaymentSuccess;
                PaymentManager.Instance.OnPaymentFailed += OnPaymentFailed;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OnItemPurchased -= OnItemPurchased;
                ShopManager.Instance.OnPurchaseFailed -= OnPurchaseFailed;
                ShopManager.Instance.OnItemApplied -= OnItemApplied;
            }
            
            if (MonthlyCardManager.Instance != null)
            {
                MonthlyCardManager.Instance.OnCardPurchased -= OnCardPurchased;
                MonthlyCardManager.Instance.OnCardActivated -= OnCardActivated;
                MonthlyCardManager.Instance.OnDailyRewardClaimed -= OnDailyRewardClaimed;
                MonthlyCardManager.Instance.OnCardExpired -= OnCardExpired;
            }
            
            if (PaymentManager.Instance != null)
            {
                PaymentManager.Instance.OnPaymentSuccess -= OnPaymentSuccess;
                PaymentManager.Instance.OnPaymentFailed -= OnPaymentFailed;
            }
        }

        // ==================== 状态刷新 ====================

        private void RefreshStatus()
        {
            if (statusText == null) return;

            string status = "=== Phase 8 系统状态 ===\n\n";
            
            // 玩家资源状态
            if (ResourceManager.Instance != null)
            {
                status += "[玩家资源]\n";
                status += $"虚拟币: {ResourceManager.Instance.GetVirtualCoin():N0}\n";
                status += $"等级: {ResourceManager.Instance.GetLevel()}\n\n";
            }
            
            // 商城状态
            if (ShopManager.Instance != null)
            {
                status += "[商城状态]\n";
                status += $"总购买次数: {ShopManager.Instance.TotalPurchaseCount}\n";
                status += $"虚拟币消费: {ShopManager.Instance.TotalVirtualCoinSpent:N0}\n";
                status += $"真实货币消费: ${ShopManager.Instance.TotalRealMoneySpent:F2}\n\n";
            }
            
            // 月卡状态
            if (MonthlyCardManager.Instance != null)
            {
                status += "[月卡状态]\n";
                var card = MonthlyCardManager.Instance.GetCurrentCard();
                if (card != null && card.IsValid())
                {
                    status += $"当前月卡: {card.cardName}\n";
                    status += $"剩余天数: {card.GetRemainingDays()}\n";
                    status += $"已领取: {card.claimedDays}/{card.durationDays} 天\n";
                    status += $"下次领取: {card.GetNextClaimTimeText()}\n";
                }
                else
                {
                    status += "无激活月卡\n";
                }
                status += $"总购买: {MonthlyCardManager.Instance.TotalCardsPurchased} 次\n";
                status += $"总领取虚拟币: {MonthlyCardManager.Instance.TotalCoinsFromCards:N0}\n\n";
            }
            
            // 首充状态
            status += "[首充状态]\n";
            status += $"状态: {firstCharge.GetStatusText()}\n";
            if (firstCharge.isPurchased)
            {
                status += $"购买时间: {firstCharge.purchaseTime:yyyy-MM-dd HH:mm}\n";
            }
            status += "\n";
            
            // 支付状态
            if (PaymentManager.Instance != null)
            {
                status += "[支付状态]\n";
                status += $"测试模式: {(PaymentManager.Instance.isTestMode ? "开启" : "关闭")}\n";
                status += $"总交易: {PaymentManager.Instance.TotalTransactionCount}\n";
                status += $"成功: {PaymentManager.Instance.SuccessfulTransactionCount}\n";
                status += $"失败: {PaymentManager.Instance.FailedTransactionCount}\n";
                status += $"成功率: {PaymentManager.Instance.GetSuccessRate():F1}%\n";
                status += $"总收入: ${PaymentManager.Instance.TotalRevenue:F2}\n";
            }
            
            statusText.text = status;
        }

        // ==================== 商城测试方法 ====================

        private void OnShowAllItems()
        {
            var items = ShopManager.Instance.GetAllItems();
            AddLog($"=== 所有商品 ({items.Count}件) ===");
            foreach (var item in items)
            {
                AddLog($"{item.itemName} - {item.GetPriceText()}");
            }
        }

        private void OnShowConfigItems()
        {
            var items = ShopManager.Instance.GetItemsByCategory(ShopCategory.ConfigUpgrade);
            AddLog($"=== 配置升级商品 ({items.Count}件) ===");
            foreach (var item in items)
            {
                AddLog($"{item.itemName} - {item.GetPriceText()}");
            }
        }

        private void OnShowCoinPacks()
        {
            var items = ShopManager.Instance.GetItemsByCategory(ShopCategory.VirtualCoin);
            AddLog($"=== 虚拟币礼包 ({items.Count}件) ===");
            foreach (var item in items)
            {
                float ratio = item.GetValueRatio();
                AddLog($"{item.itemName} - {item.GetPriceText()} (性价比: {ratio:F0}币/美元)");
            }
        }

        private void OnBuyMemory()
        {
            AddLog("购买内存升级...");
            ShopManager.Instance.PurchaseItem("memory_1gb", (success, result) =>
            {
                if (success)
                {
                    AddLog("✓ 内存升级购买成功！");
                }
                else
                {
                    AddLog($"✗ 购买失败: {result}");
                }
            });
        }

        private void OnBuyCPU()
        {
            AddLog("购买CPU升级...");
            ShopManager.Instance.PurchaseItem("cpu_1core", (success, result) =>
            {
                if (success)
                {
                    AddLog("✓ CPU升级购买成功！");
                }
                else
                {
                    AddLog($"✗ 购买失败: {result}");
                }
            });
        }

        private void OnBuySmallPack()
        {
            AddLog("购买虚拟币小包...");
            ShopManager.Instance.PurchaseItem("coin_pack_1", (success, result) =>
            {
                if (success)
                {
                    AddLog("✓ 虚拟币小包购买成功！");
                }
                else
                {
                    AddLog($"✗ 购买失败: {result}");
                }
            });
        }

        private void OnShowPurchaseHistory()
        {
            var history = ShopManager.Instance.GetRecentPurchases(10);
            AddLog($"=== 最近购买记录 ({history.Count}条) ===");
            foreach (var record in history)
            {
                AddLog(record.GetRecordText());
            }
        }

        // ==================== 月卡测试方法 ====================

        private void OnBuyBasicCard()
        {
            AddLog("购买基础月卡...");
            MonthlyCardManager.Instance.PurchaseCard(MonthlyCardType.Basic, (success, msg) =>
            {
                if (success)
                {
                    AddLog($"✓ 基础月卡购买成功！");
                }
                else
                {
                    AddLog($"✗ 购买失败: {msg}");
                }
            });
        }

        private void OnBuyPremiumCard()
        {
            AddLog("购买豪华月卡...");
            MonthlyCardManager.Instance.PurchaseCard(MonthlyCardType.Premium, (success, msg) =>
            {
                if (success)
                {
                    AddLog($"✓ 豪华月卡购买成功！");
                }
                else
                {
                    AddLog($"✗ 购买失败: {msg}");
                }
            });
        }

        private void OnClaimDaily()
        {
            AddLog("领取每日奖励...");
            bool success = MonthlyCardManager.Instance.ClaimDailyReward();
            if (success)
            {
                AddLog("✓ 每日奖励领取成功！");
            }
            else
            {
                AddLog("✗ 领取失败（今天已领取或无月卡）");
            }
        }

        private void OnCheckCardStatus()
        {
            var card = MonthlyCardManager.Instance.GetCurrentCard();
            if (card != null && card.IsValid())
            {
                AddLog("=== 月卡状态 ===");
                AddLog(card.GetSummaryText());
            }
            else
            {
                AddLog("当前无激活月卡");
            }
        }

        private void OnShowClaimHistory()
        {
            var history = MonthlyCardManager.Instance.GetClaimHistory();
            AddLog($"=== 领取历史 ({history.Count}条) ===");
            int count = 0;
            for (int i = history.Count - 1; i >= 0 && count < 10; i--, count++)
            {
                AddLog(history[i].GetRecordText());
            }
        }

        // ==================== 首充测试方法 ====================

        private void OnShowFirstCharge()
        {
            AddLog("=== 首充礼包信息 ===");
            AddLog(firstCharge.GetDetailedInfo());
        }

        private void OnBuyFirstCharge()
        {
            if (!firstCharge.CanPurchase())
            {
                AddLog("✗ 首充已购买，无法重复购买");
                return;
            }
            
            AddLog("购买首充礼包...");
            PaymentManager.Instance.ProcessPayment(firstCharge.offerId, firstCharge.price, (success, txId) =>
            {
                if (success)
                {
                    firstCharge.MarkAsPurchased();
                    AddLog($"✓ 首充礼包购买成功！交易ID: {txId}");
                    AddLog("请点击"领取首充奖励"按钮领取");
                }
                else
                {
                    AddLog("✗ 首充购买失败");
                }
            });
        }

        private void OnClaimFirstCharge()
        {
            if (!firstCharge.CanClaimReward())
            {
                AddLog("✗ 无法领取（未购买或已领取）");
                return;
            }
            
            AddLog("领取首充奖励...");
            bool success = firstCharge.ClaimRewards();
            if (success)
            {
                // 发放虚拟币
                if (ResourceManager.Instance != null)
                {
                    ResourceManager.Instance.AddVirtualCoin(firstCharge.virtualCoinReward, "首充奖励");
                }
                
                // 发放其他奖励
                foreach (var reward in firstCharge.rewards)
                {
                    AddLog($"  获得: {reward.GetDisplayText()}");
                }
                
                AddLog($"✓ 首充奖励领取成功！获得 {firstCharge.virtualCoinReward} 虚拟币");
            }
            else
            {
                AddLog("✗ 领取失败");
            }
        }

        // ==================== 支付测试方法 ====================

        private void OnShowPaymentStats()
        {
            if (PaymentManager.Instance != null)
            {
                PaymentManager.Instance.PrintPaymentStatistics();
                AddLog("支付统计信息已输出到Console");
            }
        }

        private void OnToggleTestMode()
        {
            if (PaymentManager.Instance != null)
            {
                PaymentManager.Instance.isTestMode = !PaymentManager.Instance.isTestMode;
                AddLog($"测试模式已{(PaymentManager.Instance.isTestMode ? "开启" : "关闭")}");
            }
        }

        private void OnSimulateFailure()
        {
            if (PaymentManager.Instance != null && PaymentManager.Instance.isTestMode)
            {
                bool currentSetting = PaymentManager.Instance.testModeAutoSuccess;
                PaymentManager.Instance.SimulatePaymentFailure(!currentSetting);
                AddLog($"支付失败模拟已{(currentSetting ? "开启" : "关闭")}");
            }
            else
            {
                AddLog("仅测试模式下可用");
            }
        }

        // ==================== 通用方法 ====================

        private void OnClearLog()
        {
            logMessages.Clear();
            if (logText != null)
            {
                logText.text = "";
            }
            AddLog("日志已清空");
        }

        // ==================== 事件处理方法 ====================

        private void OnItemPurchased(ShopItemData item, PurchaseRecord record)
        {
            AddLog($"[事件] 商品购买成功: {item.itemName}");
        }

        private void OnPurchaseFailed(ShopItemData item, PurchaseResult result)
        {
            AddLog($"[事件] 商品购买失败: {item?.itemName ?? "未知"} - {result}");
        }

        private void OnItemApplied(ShopItemData item)
        {
            AddLog($"[事件] 商品效果已应用: {item.itemName}");
        }

        private void OnCardPurchased(MonthlyCardData card)
        {
            AddLog($"[事件] 月卡购买成功: {card.cardName}");
        }

        private void OnCardActivated(MonthlyCardData card)
        {
            AddLog($"[事件] 月卡已激活: {card.cardName}，有效期 {card.durationDays} 天");
        }

        private void OnDailyRewardClaimed(MonthlyCardData card, int coins, int cleaners)
        {
            AddLog($"[事件] 每日奖励已领取: {coins}币 + {cleaners}清理工具");
        }

        private void OnCardExpired(MonthlyCardData card)
        {
            AddLog($"[事件] 月卡已到期: {card.cardName}");
        }

        private void OnPaymentSuccess(TransactionRecord transaction)
        {
            AddLog($"[事件] 支付成功: {transaction.itemId} - ${transaction.amount:F2}");
        }

        private void OnPaymentFailed(string itemId, string reason)
        {
            AddLog($"[事件] 支付失败: {itemId} - {reason}");
        }

        // ==================== 日志方法 ====================

        private void AddLog(string message)
        {
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
            string logMessage = $"[{timestamp}] {message}";
            
            logMessages.Add(logMessage);
            
            // 限制日志数量
            if (logMessages.Count > MAX_LOG_MESSAGES)
            {
                logMessages.RemoveAt(0);
            }
            
            // 更新UI
            if (logText != null)
            {
                logText.text = string.Join("\n", logMessages);
                
                // 滚动到底部
                if (logScrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    logScrollRect.verticalNormalizedPosition = 0f;
                }
            }
        }
    }
}
