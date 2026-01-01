using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * PaymentManager.cs - 支付管理器
 * 
 * 功能说明：
 * 1. 处理真实货币支付（Google Play、Apple IAP）
 * 2. 支持测试模式（开发阶段绕过真实支付）
 * 3. 管理支付流程和回调
 * 4. 记录交易历史
 * 5. 防作弊验证
 * 6. 提供完整的事件系统
 * 
 * Unity操作说明：
 * 1. 在Hierarchy中创建空物体"PaymentManager"
 * 2. 添加此脚本组件
 * 3. 在Inspector中配置测试模式开关
 * 4. 正式发布前需要集成真实的IAP SDK
 * 
 * 使用示例：
 * // 测试模式购买
 * PaymentManager.Instance.ProcessPayment("coin_pack_2", 4.99f, (success, txId) =>
 * {
 *     if (success)
 *     {
 *         Debug.Log($"购买成功: {txId}");
 *     }
 * });
 * 
 * // 监听支付事件
 * PaymentManager.Instance.OnPaymentSuccess += OnPaymentCompleted;
 * 
 * 重要提示：
 * - 当前版本为测试模式实现
 * - 正式版需要集成Unity IAP或原生SDK
 * - 测试模式不涉及真实金钱交易
 */

namespace ECitizen.Managers
{
    /// <summary>
    /// 支付平台枚举
    /// </summary>
    public enum PaymentPlatform
    {
        GooglePlay,         // Google Play
        AppleAppStore,      // Apple App Store
        TestMode            // 测试模式（绕过真实支付）
    }

    /// <summary>
    /// 支付状态枚举
    /// </summary>
    public enum PaymentStatus
    {
        Pending,            // 待处理
        Processing,         // 处理中
        Success,            // 成功
        Failed,             // 失败
        Cancelled,          // 用户取消
        Refunded            // 已退款
    }

    /// <summary>
    /// 交易记录数据类
    /// </summary>
    [Serializable]
    public class TransactionRecord
    {
        public string transactionId;
        public string itemId;
        public float amount;
        public string currency;
        public PaymentPlatform platform;
        public PaymentStatus status;
        public DateTime transactionTime;
        public bool isVerified;
        public string receipt;

        public TransactionRecord(string itemId, float amount, PaymentPlatform platform)
        {
            this.transactionId = Guid.NewGuid().ToString();
            this.itemId = itemId;
            this.amount = amount;
            this.currency = "USD";
            this.platform = platform;
            this.status = PaymentStatus.Pending;
            this.transactionTime = DateTime.Now;
            this.isVerified = false;
        }

        public string GetRecordText()
        {
            return $"{transactionTime:yyyy-MM-dd HH:mm} - {itemId} - ${amount:F2} - {status}";
        }
    }

    /// <summary>
    /// 支付管理器
    /// 单例模式，管理所有支付相关功能
    /// </summary>
    public class PaymentManager : MonoBehaviour
    {
        // ==================== 单例模式 ====================
        
        private static PaymentManager _instance;
        public static PaymentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PaymentManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("PaymentManager");
                        _instance = go.AddComponent<PaymentManager>();
                    }
                }
                return _instance;
            }
        }

        // ==================== Inspector配置 ====================
        
        [Header("支付配置")]
        [Tooltip("是否启用测试模式（绕过真实支付）")]
        public bool isTestMode = true;
        
        [Tooltip("测试模式下是否自动成功")]
        public bool testModeAutoSuccess = true;
        
        [Tooltip("测试模式支付延迟（秒）")]
        public float testModeDelay = 1.5f;

        // ==================== 事件定义 ====================
        
        /// <summary>支付开始事件</summary>
        public event Action<string, float> OnPaymentStarted;
        
        /// <summary>支付成功事件</summary>
        public event Action<TransactionRecord> OnPaymentSuccess;
        
        /// <summary>支付失败事件</summary>
        public event Action<string, string> OnPaymentFailed;
        
        /// <summary>支付取消事件</summary>
        public event Action<string> OnPaymentCancelled;

        // ==================== 支付数据 ====================
        
        /// <summary>当前支付平台</summary>
        private PaymentPlatform currentPlatform;
        
        /// <summary>交易历史记录</summary>
        private List<TransactionRecord> transactionHistory = new List<TransactionRecord>();
        
        /// <summary>待处理的支付请求</summary>
        private Dictionary<string, TransactionRecord> pendingTransactions = new Dictionary<string, TransactionRecord>();

        // ==================== 统计数据 ====================
        
        /// <summary>总交易次数</summary>
        public int TotalTransactionCount { get; private set; }
        
        /// <summary>成功交易次数</summary>
        public int SuccessfulTransactionCount { get; private set; }
        
        /// <summary>失败交易次数</summary>
        public int FailedTransactionCount { get; private set; }
        
        /// <summary>总收入（美元）</summary>
        public float TotalRevenue { get; private set; }

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
            
            InitializePaymentSystem();
        }

        // ==================== 初始化方法 ====================

        /// <summary>
        /// 初始化支付系统
        /// </summary>
        private void InitializePaymentSystem()
        {
            Debug.Log("=== 支付系统初始化 ===");
            
            // 检测当前平台
            DetectPlatform();
            
            if (isTestMode)
            {
                Debug.Log("支付系统运行在测试模式");
                Debug.Log($"  - 自动成功: {testModeAutoSuccess}");
                Debug.Log($"  - 模拟延迟: {testModeDelay}秒");
            }
            else
            {
                Debug.Log($"支付系统运行在真实模式 - 平台: {currentPlatform}");
                // TODO: 初始化真实的IAP SDK
                InitializeRealPaymentSDK();
            }
        }

        /// <summary>
        /// 检测当前平台
        /// </summary>
        private void DetectPlatform()
        {
            if (isTestMode)
            {
                currentPlatform = PaymentPlatform.TestMode;
            }
            else
            {
                #if UNITY_ANDROID
                currentPlatform = PaymentPlatform.GooglePlay;
                #elif UNITY_IOS
                currentPlatform = PaymentPlatform.AppleAppStore;
                #else
                currentPlatform = PaymentPlatform.TestMode;
                Debug.LogWarning("当前平台不支持真实支付，切换到测试模式");
                isTestMode = true;
                #endif
            }
        }

        /// <summary>
        /// 初始化真实支付SDK
        /// </summary>
        private void InitializeRealPaymentSDK()
        {
            // TODO: 集成Unity IAP或原生SDK
            Debug.Log("初始化真实支付SDK...");
            
            // 示例代码（需要Unity IAP包）:
            /*
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct("coin_pack_1", ProductType.Consumable);
            builder.AddProduct("coin_pack_2", ProductType.Consumable);
            // ...添加更多商品
            UnityPurchasing.Initialize(this, builder);
            */
        }

        // ==================== 支付处理方法 ====================

        /// <summary>
        /// 处理支付请求
        /// </summary>
        /// <param name="itemId">商品ID</param>
        /// <param name="amount">金额（美元）</param>
        /// <param name="callback">完成回调(success, transactionId)</param>
        public void ProcessPayment(string itemId, float amount, Action<bool, string> callback)
        {
            Debug.Log($"开始处理支付: {itemId}, ${amount:F2}");
            
            // 触发支付开始事件
            OnPaymentStarted?.Invoke(itemId, amount);
            
            // 创建交易记录
            var transaction = new TransactionRecord(itemId, amount, currentPlatform);
            transactionHistory.Add(transaction);
            pendingTransactions[transaction.transactionId] = transaction;
            TotalTransactionCount++;

            if (isTestMode)
            {
                // 测试模式支付
                ProcessTestPayment(transaction, callback);
            }
            else
            {
                // 真实支付
                ProcessRealPayment(transaction, callback);
            }
        }

        /// <summary>
        /// 处理测试模式支付
        /// </summary>
        private void ProcessTestPayment(TransactionRecord transaction, Action<bool, string> callback)
        {
            // 模拟支付延迟
            StartCoroutine(SimulatePaymentDelay(transaction, callback));
        }

        /// <summary>
        /// 模拟支付延迟协程
        /// </summary>
        private System.Collections.IEnumerator SimulatePaymentDelay(TransactionRecord transaction, Action<bool, string> callback)
        {
            transaction.status = PaymentStatus.Processing;
            Debug.Log($"[测试模式] 正在处理支付: {transaction.transactionId}");
            
            yield return new WaitForSeconds(testModeDelay);
            
            bool success = testModeAutoSuccess;
            
            if (success)
            {
                CompletePayment(transaction, callback);
            }
            else
            {
                FailPayment(transaction, "测试模式模拟失败", callback);
            }
        }

        /// <summary>
        /// 处理真实支付
        /// </summary>
        private void ProcessRealPayment(TransactionRecord transaction, Action<bool, string> callback)
        {
            // TODO: 实现真实的IAP支付流程
            Debug.Log($"[真实支付] 启动支付流程: {transaction.itemId}");
            
            transaction.status = PaymentStatus.Processing;
            
            // 示例代码（需要Unity IAP）:
            /*
            var productId = transaction.itemId;
            if (IsInitialized())
            {
                var product = storeController.products.WithID(productId);
                if (product != null && product.availableToPurchase)
                {
                    storeController.InitiatePurchase(product);
                    // 支付结果会通过IAP回调处理
                }
                else
                {
                    FailPayment(transaction, "商品不可购买", callback);
                }
            }
            else
            {
                FailPayment(transaction, "支付系统未初始化", callback);
            }
            */
            
            // 暂时在真实模式下也使用测试流程
            Debug.LogWarning("真实支付SDK未集成，使用测试流程");
            ProcessTestPayment(transaction, callback);
        }

        // ==================== 支付完成处理 ====================

        /// <summary>
        /// 完成支付
        /// </summary>
        private void CompletePayment(TransactionRecord transaction, Action<bool, string> callback)
        {
            transaction.status = PaymentStatus.Success;
            transaction.isVerified = true;
            
            // 从待处理列表移除
            if (pendingTransactions.ContainsKey(transaction.transactionId))
            {
                pendingTransactions.Remove(transaction.transactionId);
            }
            
            // 更新统计
            SuccessfulTransactionCount++;
            TotalRevenue += transaction.amount;
            
            Debug.Log($"支付成功: {transaction.itemId}, 交易ID: {transaction.transactionId}");
            
            // 触发事件
            OnPaymentSuccess?.Invoke(transaction);
            
            // 执行回调
            callback?.Invoke(true, transaction.transactionId);
            
            // TODO: 保存交易记录到服务器
            SaveTransactionToServer(transaction);
        }

        /// <summary>
        /// 支付失败
        /// </summary>
        private void FailPayment(TransactionRecord transaction, string reason, Action<bool, string> callback)
        {
            transaction.status = PaymentStatus.Failed;
            
            // 从待处理列表移除
            if (pendingTransactions.ContainsKey(transaction.transactionId))
            {
                pendingTransactions.Remove(transaction.transactionId);
            }
            
            // 更新统计
            FailedTransactionCount++;
            
            Debug.LogWarning($"支付失败: {transaction.itemId}, 原因: {reason}");
            
            // 触发事件
            OnPaymentFailed?.Invoke(transaction.itemId, reason);
            
            // 执行回调
            callback?.Invoke(false, null);
        }

        /// <summary>
        /// 取消支付
        /// </summary>
        public void CancelPayment(string transactionId)
        {
            if (pendingTransactions.TryGetValue(transactionId, out var transaction))
            {
                transaction.status = PaymentStatus.Cancelled;
                pendingTransactions.Remove(transactionId);
                
                Debug.Log($"支付已取消: {transaction.itemId}");
                
                // 触发事件
                OnPaymentCancelled?.Invoke(transaction.itemId);
            }
        }

        // ==================== 验证方法 ====================

        /// <summary>
        /// 验证收据（防作弊）
        /// </summary>
        /// <param name="receipt">支付收据</param>
        /// <param name="callback">验证结果回调</param>
        public void VerifyReceipt(string receipt, Action<bool> callback)
        {
            if (isTestMode)
            {
                // 测试模式直接通过
                Debug.Log("[测试模式] 收据验证通过");
                callback?.Invoke(true);
                return;
            }
            
            // TODO: 实现服务器端收据验证
            Debug.Log("验证收据...");
            
            // 真实验证流程：
            // 1. 将收据发送到自己的服务器
            // 2. 服务器调用Google/Apple的验证API
            // 3. 返回验证结果
            
            callback?.Invoke(true);
        }

        // ==================== 交易查询方法 ====================

        /// <summary>
        /// 获取交易历史
        /// </summary>
        public List<TransactionRecord> GetTransactionHistory()
        {
            return new List<TransactionRecord>(transactionHistory);
        }

        /// <summary>
        /// 获取成功的交易
        /// </summary>
        public List<TransactionRecord> GetSuccessfulTransactions()
        {
            return transactionHistory.FindAll(t => t.status == PaymentStatus.Success);
        }

        /// <summary>
        /// 获取失败的交易
        /// </summary>
        public List<TransactionRecord> GetFailedTransactions()
        {
            return transactionHistory.FindAll(t => t.status == PaymentStatus.Failed);
        }

        /// <summary>
        /// 根据交易ID查询交易
        /// </summary>
        public TransactionRecord GetTransactionById(string transactionId)
        {
            return transactionHistory.Find(t => t.transactionId == transactionId);
        }

        // ==================== 统计方法 ====================

        /// <summary>
        /// 获取支付成功率
        /// </summary>
        public float GetSuccessRate()
        {
            if (TotalTransactionCount == 0) return 0f;
            return (float)SuccessfulTransactionCount / TotalTransactionCount * 100f;
        }

        /// <summary>
        /// 获取平均交易金额
        /// </summary>
        public float GetAverageTransactionAmount()
        {
            if (SuccessfulTransactionCount == 0) return 0f;
            return TotalRevenue / SuccessfulTransactionCount;
        }

        // ==================== 调试方法 ====================

        /// <summary>
        /// 打印支付统计信息
        /// </summary>
        public void PrintPaymentStatistics()
        {
            Debug.Log("=== 支付统计信息 ===");
            Debug.Log($"当前模式: {(isTestMode ? "测试模式" : "真实支付")}");
            Debug.Log($"支付平台: {currentPlatform}");
            Debug.Log($"总交易次数: {TotalTransactionCount}");
            Debug.Log($"成功次数: {SuccessfulTransactionCount}");
            Debug.Log($"失败次数: {FailedTransactionCount}");
            Debug.Log($"成功率: {GetSuccessRate():F1}%");
            Debug.Log($"总收入: ${TotalRevenue:F2}");
            Debug.Log($"平均交易额: ${GetAverageTransactionAmount():F2}");
            Debug.Log($"待处理支付: {pendingTransactions.Count}");
        }

        /// <summary>
        /// 模拟支付失败（调试用）
        /// </summary>
        public void SimulatePaymentFailure(bool enable)
        {
            if (isTestMode)
            {
                testModeAutoSuccess = !enable;
                Debug.Log($"测试模式支付失败模拟: {(enable ? "开启" : "关闭")}");
            }
        }

        // ==================== 数据持久化 ====================

        /// <summary>
        /// 保存交易记录到服务器
        /// </summary>
        private void SaveTransactionToServer(TransactionRecord transaction)
        {
            // TODO: 实现保存到Firebase或自己的服务器
            Debug.Log($"保存交易记录: {transaction.transactionId}");
        }

        /// <summary>
        /// 加载交易历史
        /// </summary>
        public void LoadTransactionHistory()
        {
            // TODO: 从服务器加载交易历史
            Debug.Log("加载交易历史...");
        }

        // ==================== 退款处理 ====================

        /// <summary>
        /// 处理退款
        /// </summary>
        public void ProcessRefund(string transactionId)
        {
            var transaction = GetTransactionById(transactionId);
            if (transaction != null && transaction.status == PaymentStatus.Success)
            {
                transaction.status = PaymentStatus.Refunded;
                TotalRevenue -= transaction.amount;
                SuccessfulTransactionCount--;
                
                Debug.Log($"退款处理: {transactionId}");
                
                // TODO: 撤销已发放的商品
                // TODO: 通知服务器
            }
        }
    }
}
