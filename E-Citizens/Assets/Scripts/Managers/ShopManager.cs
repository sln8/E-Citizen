using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ECitizen.Data;

/*
 * ShopManager.cs - 商城管理器
 * 
 * 功能说明：
 * 1. 管理所有商城商品（配置升级、外观装扮、虚拟币礼包、工具消耗品）
 * 2. 处理商品购买逻辑（虚拟币支付和真实货币支付）
 * 3. 管理购买历史记录
 * 4. 与ResourceManager、PaymentManager集成
 * 5. 提供完整的事件系统
 * 6. 支持测试模式
 * 
 * Unity操作说明：
 * 1. 在Hierarchy中创建空物体"ShopManager"
 * 2. 添加此脚本组件
 * 3. 确保ResourceManager、PaymentManager已经初始化
 * 4. 通过ShopManager.Instance访问商城功能
 * 
 * 使用示例：
 * // 获取所有商品
 * List<ShopItemData> allItems = ShopManager.Instance.GetAllItems();
 * 
 * // 购买商品（虚拟币）
 * bool success = ShopManager.Instance.PurchaseItem("memory_1gb");
 * 
 * // 购买商品（真实货币）
 * ShopManager.Instance.PurchaseItemWithRealMoney("coin_pack_2", OnPurchaseComplete);
 * 
 * // 监听购买事件
 * ShopManager.Instance.OnItemPurchased += OnItemBought;
 */

namespace ECitizen.Managers
{
    /// <summary>
    /// 购买结果枚举
    /// </summary>
    public enum PurchaseResult
    {
        Success,                // 成功
        InsufficientFunds,      // 虚拟币不足
        ItemNotFound,           // 商品不存在
        AlreadyOwned,           // 已拥有（不可重复购买的商品）
        PaymentFailed,          // 支付失败
        SystemError             // 系统错误
    }

    /// <summary>
    /// 购买记录数据类
    /// </summary>
    [Serializable]
    public class PurchaseRecord
    {
        public string itemId;
        public string itemName;
        public ShopCategory category;
        public CurrencyType currencyType;
        public float price;
        public DateTime purchaseTime;
        public string transactionId;

        public PurchaseRecord(ShopItemData item, string txId = "")
        {
            this.itemId = item.itemId;
            this.itemName = item.itemName;
            this.category = item.category;
            this.currencyType = item.currencyType;
            this.price = item.price;
            this.purchaseTime = DateTime.Now;
            this.transactionId = string.IsNullOrEmpty(txId) ? Guid.NewGuid().ToString() : txId;
        }

        public string GetRecordText()
        {
            string priceText = currencyType == CurrencyType.VirtualCoin ? 
                $"{price:N0}币" : $"${price:F2}";
            return $"{purchaseTime:yyyy-MM-dd HH:mm} - {itemName} - {priceText}";
        }
    }

    /// <summary>
    /// 商城管理器
    /// 单例模式，管理所有商城相关功能
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        // ==================== 单例模式 ====================
        
        private static ShopManager _instance;
        public static ShopManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ShopManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("ShopManager");
                        _instance = go.AddComponent<ShopManager>();
                    }
                }
                return _instance;
            }
        }

        // ==================== 事件定义 ====================
        
        /// <summary>商品购买成功事件</summary>
        public event Action<ShopItemData, PurchaseRecord> OnItemPurchased;
        
        /// <summary>商品购买失败事件</summary>
        public event Action<ShopItemData, PurchaseResult> OnPurchaseFailed;
        
        /// <summary>商品应用成功事件（如升级已应用）</summary>
        public event Action<ShopItemData> OnItemApplied;
        
        /// <summary>虚拟币不足事件</summary>
        public event Action<float> OnInsufficientFunds;

        // ==================== 商品数据 ====================
        
        /// <summary>所有商品列表</summary>
        private List<ShopItemData> allItems = new List<ShopItemData>();
        
        /// <summary>已拥有的外观皮肤ID列表</summary>
        private HashSet<string> ownedSkins = new HashSet<string>();
        
        /// <summary>购买历史记录</summary>
        private List<PurchaseRecord> purchaseHistory = new List<PurchaseRecord>();

        // ==================== 统计数据 ====================
        
        /// <summary>总购买次数</summary>
        public int TotalPurchaseCount { get; private set; }
        
        /// <summary>总虚拟币消费</summary>
        public float TotalVirtualCoinSpent { get; private set; }
        
        /// <summary>总真实货币消费</summary>
        public float TotalRealMoneySpent { get; private set; }

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
            
            InitializeShop();
        }

        // ==================== 初始化方法 ====================

        /// <summary>
        /// 初始化商城
        /// 加载所有商品数据
        /// </summary>
        private void InitializeShop()
        {
            Debug.Log("=== 商城系统初始化 ===");
            
            // 加载所有预定义商品
            allItems.Clear();
            allItems.AddRange(ShopItemData.GetAllItems());
            
            Debug.Log($"商城初始化完成，共加载 {allItems.Count} 件商品");
            Debug.Log($"  - 配置升级: {allItems.Count(i => i.category == ShopCategory.ConfigUpgrade)} 件");
            Debug.Log($"  - 外观装扮: {allItems.Count(i => i.category == ShopCategory.Appearance)} 件");
            Debug.Log($"  - 虚拟币礼包: {allItems.Count(i => i.category == ShopCategory.VirtualCoin)} 件");
            Debug.Log($"  - 工具消耗品: {allItems.Count(i => i.category == ShopCategory.Tools)} 件");
        }

        // ==================== 商品查询方法 ====================

        /// <summary>
        /// 获取所有商品
        /// </summary>
        public List<ShopItemData> GetAllItems()
        {
            return new List<ShopItemData>(allItems);
        }

        /// <summary>
        /// 根据类别获取商品
        /// </summary>
        public List<ShopItemData> GetItemsByCategory(ShopCategory category)
        {
            return allItems.Where(item => item.category == category).ToList();
        }

        /// <summary>
        /// 根据货币类型获取商品
        /// </summary>
        public List<ShopItemData> GetItemsByCurrency(CurrencyType currencyType)
        {
            return allItems.Where(item => item.currencyType == currencyType).ToList();
        }

        /// <summary>
        /// 根据ID获取商品
        /// </summary>
        public ShopItemData GetItemById(string itemId)
        {
            return allItems.FirstOrDefault(item => item.itemId == itemId);
        }

        /// <summary>
        /// 检查玩家是否已拥有某个皮肤
        /// </summary>
        public bool HasSkin(string skinId)
        {
            return ownedSkins.Contains(skinId);
        }

        // ==================== 购买方法 ====================

        /// <summary>
        /// 购买商品（通用方法）
        /// 自动判断使用虚拟币还是真实货币
        /// </summary>
        public void PurchaseItem(string itemId, Action<bool, PurchaseResult> callback = null)
        {
            ShopItemData item = GetItemById(itemId);
            if (item == null)
            {
                Debug.LogError($"商品不存在: {itemId}");
                callback?.Invoke(false, PurchaseResult.ItemNotFound);
                OnPurchaseFailed?.Invoke(null, PurchaseResult.ItemNotFound);
                return;
            }

            if (item.currencyType == CurrencyType.VirtualCoin)
            {
                bool success = PurchaseWithVirtualCoin(item);
                PurchaseResult result = success ? PurchaseResult.Success : PurchaseResult.InsufficientFunds;
                callback?.Invoke(success, result);
            }
            else
            {
                PurchaseWithRealMoney(item, (success, result) => callback?.Invoke(success, result));
            }
        }

        /// <summary>
        /// 使用虚拟币购买商品
        /// </summary>
        private bool PurchaseWithVirtualCoin(ShopItemData item)
        {
            // 检查商品是否存在
            if (item == null)
            {
                Debug.LogError("商品不存在");
                OnPurchaseFailed?.Invoke(item, PurchaseResult.ItemNotFound);
                return false;
            }

            // 检查是否已拥有（仅限不可重复购买的商品）
            if (!item.isRepeatable && item.category == ShopCategory.Appearance && HasSkin(item.itemId))
            {
                Debug.LogWarning($"已拥有该皮肤: {item.itemName}");
                OnPurchaseFailed?.Invoke(item, PurchaseResult.AlreadyOwned);
                return false;
            }

            // 检查虚拟币是否足够
            if (ResourceManager.Instance == null)
            {
                Debug.LogError("ResourceManager未初始化");
                OnPurchaseFailed?.Invoke(item, PurchaseResult.SystemError);
                return false;
            }

            float currentCoin = ResourceManager.Instance.GetVirtualCoin();
            if (currentCoin < item.price)
            {
                Debug.LogWarning($"虚拟币不足: 需要 {item.price}，当前 {currentCoin}");
                OnInsufficientFunds?.Invoke(item.price - currentCoin);
                OnPurchaseFailed?.Invoke(item, PurchaseResult.InsufficientFunds);
                return false;
            }

            // 扣除虚拟币
            bool deductSuccess = ResourceManager.Instance.SpendVirtualCoin(item.price, $"购买{item.itemName}");
            if (!deductSuccess)
            {
                Debug.LogError("扣除虚拟币失败");
                OnPurchaseFailed?.Invoke(item, PurchaseResult.SystemError);
                return false;
            }

            // 应用商品效果
            ApplyItemEffect(item);

            // 记录购买
            RecordPurchase(item);
            TotalPurchaseCount++;
            TotalVirtualCoinSpent += item.price;

            Debug.Log($"成功购买: {item.itemName}，花费 {item.price} 虚拟币");
            return true;
        }

        /// <summary>
        /// 使用真实货币购买商品
        /// </summary>
        private void PurchaseWithRealMoney(ShopItemData item, Action<bool, PurchaseResult> callback)
        {
            // 检查商品是否存在
            if (item == null)
            {
                Debug.LogError("商品不存在");
                callback?.Invoke(false, PurchaseResult.ItemNotFound);
                OnPurchaseFailed?.Invoke(item, PurchaseResult.ItemNotFound);
                return;
            }

            // 检查PaymentManager是否存在
            if (PaymentManager.Instance == null)
            {
                Debug.LogError("PaymentManager未初始化");
                callback?.Invoke(false, PurchaseResult.SystemError);
                OnPurchaseFailed?.Invoke(item, PurchaseResult.SystemError);
                return;
            }

            // 调用PaymentManager处理真实货币支付
            PaymentManager.Instance.ProcessPayment(item.itemId, item.price, (success, transactionId) =>
            {
                if (success)
                {
                    // 应用商品效果
                    ApplyItemEffect(item);
                    
                    // 记录购买
                    RecordPurchase(item, transactionId);
                    TotalPurchaseCount++;
                    TotalRealMoneySpent += item.price;
                    
                    Debug.Log($"成功购买: {item.itemName}，花费 ${item.price:F2}");
                    callback?.Invoke(true, PurchaseResult.Success);
                }
                else
                {
                    Debug.LogWarning($"支付失败: {item.itemName}");
                    callback?.Invoke(false, PurchaseResult.PaymentFailed);
                    OnPurchaseFailed?.Invoke(item, PurchaseResult.PaymentFailed);
                }
            });
        }

        // ==================== 商品效果应用 ====================

        /// <summary>
        /// 应用商品效果
        /// 根据商品类型执行不同的操作
        /// </summary>
        private void ApplyItemEffect(ShopItemData item)
        {
            switch (item.category)
            {
                case ShopCategory.ConfigUpgrade:
                    ApplyConfigUpgrade(item);
                    break;

                case ShopCategory.VirtualCoin:
                    ApplyVirtualCoinPack(item);
                    break;

                case ShopCategory.Tools:
                    ApplyToolEffect(item);
                    break;

                case ShopCategory.Appearance:
                    ApplySkin(item);
                    break;
            }

            OnItemApplied?.Invoke(item);
        }

        /// <summary>
        /// 应用配置升级
        /// </summary>
        private void ApplyConfigUpgrade(ShopItemData item)
        {
            if (ResourceManager.Instance == null)
            {
                Debug.LogError("ResourceManager未初始化，无法应用升级");
                return;
            }

            switch (item.upgradeType)
            {
                case UpgradeType.Memory:
                    ResourceManager.Instance.UpgradeMemory(item.upgradeAmount);
                    Debug.Log($"内存升级: +{item.upgradeAmount}GB");
                    break;

                case UpgradeType.CPU:
                    ResourceManager.Instance.UpgradeCPU((int)item.upgradeAmount);
                    Debug.Log($"CPU升级: +{item.upgradeAmount}核");
                    break;

                case UpgradeType.Bandwidth:
                    ResourceManager.Instance.UpgradeBandwidth(item.upgradeAmount);
                    Debug.Log($"网速升级: +{item.upgradeAmount}Mbps");
                    break;

                case UpgradeType.Computing:
                    ResourceManager.Instance.UpgradeComputing(item.upgradeAmount);
                    Debug.Log($"算力升级: +{item.upgradeAmount}");
                    break;

                case UpgradeType.Storage:
                    ResourceManager.Instance.UpgradeStorage(item.upgradeAmount);
                    Debug.Log($"存储升级: +{item.upgradeAmount}GB");
                    break;
            }
        }

        /// <summary>
        /// 应用虚拟币礼包
        /// </summary>
        private void ApplyVirtualCoinPack(ShopItemData item)
        {
            if (ResourceManager.Instance == null)
            {
                Debug.LogError("ResourceManager未初始化");
                return;
            }

            int totalCoins = item.GetTotalVirtualCoin();
            ResourceManager.Instance.AddVirtualCoin(totalCoins, $"购买{item.itemName}");
            Debug.Log($"获得虚拟币: {totalCoins}（基础{item.virtualCoinAmount} + 赠送{item.bonusCoinAmount}）");
        }

        /// <summary>
        /// 应用工具效果
        /// </summary>
        private void ApplyToolEffect(ShopItemData item)
        {
            if (ResourceManager.Instance == null)
            {
                Debug.LogError("ResourceManager未初始化");
                return;
            }

            foreach (var effect in item.toolEffects)
            {
                if (effect.Key == "storage")
                {
                    // 负值表示减少已用存储
                    ResourceManager.Instance.CleanData(-effect.Value);
                    Debug.Log($"清理存储空间: {-effect.Value}GB");
                }
            }
        }

        /// <summary>
        /// 应用外观皮肤
        /// </summary>
        private void ApplySkin(ShopItemData item)
        {
            if (!ownedSkins.Contains(item.itemId))
            {
                ownedSkins.Add(item.itemId);
                Debug.Log($"获得新皮肤: {item.itemName}");
                
                // TODO: 通知UI系统更新皮肤显示
                // TODO: 保存到玩家数据
            }
        }

        // ==================== 购买记录方法 ====================

        /// <summary>
        /// 记录购买
        /// </summary>
        private void RecordPurchase(ShopItemData item, string transactionId = "")
        {
            var record = new PurchaseRecord(item, transactionId);
            purchaseHistory.Add(record);
            
            // 触发购买成功事件
            OnItemPurchased?.Invoke(item, record);
            
            // TODO: 保存购买记录到Firebase
        }

        /// <summary>
        /// 获取购买历史
        /// </summary>
        public List<PurchaseRecord> GetPurchaseHistory()
        {
            return new List<PurchaseRecord>(purchaseHistory);
        }

        /// <summary>
        /// 获取最近N条购买记录
        /// </summary>
        public List<PurchaseRecord> GetRecentPurchases(int count = 10)
        {
            return purchaseHistory
                .OrderByDescending(r => r.purchaseTime)
                .Take(count)
                .ToList();
        }

        // ==================== 统计方法 ====================

        /// <summary>
        /// 获取按类别统计的购买次数
        /// </summary>
        public Dictionary<ShopCategory, int> GetPurchaseCountByCategory()
        {
            return purchaseHistory
                .GroupBy(r => r.category)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        /// <summary>
        /// 获取最常购买的商品
        /// </summary>
        public List<(string itemId, string itemName, int count)> GetMostPurchasedItems(int topCount = 5)
        {
            return purchaseHistory
                .GroupBy(r => new { r.itemId, r.itemName })
                .Select(g => (g.Key.itemId, g.Key.itemName, g.Count()))
                .OrderByDescending(item => item.Item3)
                .Take(topCount)
                .ToList();
        }

        // ==================== 调试方法 ====================

        /// <summary>
        /// 打印商城统计信息
        /// </summary>
        public void PrintShopStatistics()
        {
            Debug.Log("=== 商城统计信息 ===");
            Debug.Log($"总购买次数: {TotalPurchaseCount}");
            Debug.Log($"总虚拟币消费: {TotalVirtualCoinSpent:N0}");
            Debug.Log($"总真实货币消费: ${TotalRealMoneySpent:F2}");
            Debug.Log($"已拥有皮肤数: {ownedSkins.Count}");
            
            var categoryStats = GetPurchaseCountByCategory();
            Debug.Log("按类别购买统计:");
            foreach (var stat in categoryStats)
            {
                Debug.Log($"  {stat.Key}: {stat.Value} 次");
            }
            
            var topItems = GetMostPurchasedItems(3);
            Debug.Log("最常购买商品:");
            for (int i = 0; i < topItems.Count; i++)
            {
                Debug.Log($"  {i + 1}. {topItems[i].itemName} - {topItems[i].count} 次");
            }
        }

        // ==================== 数据持久化接口 ====================

        /// <summary>
        /// 保存商城数据（供Firebase或本地存储使用）
        /// </summary>
        public void SaveShopData()
        {
            // TODO: 实现保存到Firebase或本地存储
            Debug.Log("保存商城数据...");
        }

        /// <summary>
        /// 加载商城数据（供Firebase或本地存储使用）
        /// </summary>
        public void LoadShopData()
        {
            // TODO: 实现从Firebase或本地存储加载
            Debug.Log("加载商城数据...");
        }
    }
}
