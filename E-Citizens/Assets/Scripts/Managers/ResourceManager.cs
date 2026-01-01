using UnityEngine;
using System;

/// <summary>
/// 资源管理器
/// 负责管理玩家的所有资源
/// 包括资源分配、效率计算、资源升级等功能
/// 这是游戏的核心系统之一
/// </summary>
public class ResourceManager : MonoBehaviour
{
    #region 单例模式
    private static ResourceManager _instance;
    
    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ResourceManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ResourceManager");
                    _instance = go.AddComponent<ResourceManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion
    
    #region 事件定义
    /// <summary>
    /// 资源变化事件
    /// 当资源发生变化时触发，UI可以监听此事件更新显示
    /// </summary>
    public event Action<PlayerResources> OnResourcesChanged;
    
    /// <summary>
    /// 虚拟币变化事件
    /// </summary>
    public event Action<int> OnVirtualCoinChanged;
    
    /// <summary>
    /// 心情值变化事件
    /// </summary>
    public event Action<int> OnMoodValueChanged;
    
    /// <summary>
    /// 等级提升事件
    /// </summary>
    public event Action<int> OnLevelUp;
    
    /// <summary>
    /// 存储空间警告事件
    /// 当存储空间不足时触发
    /// </summary>
    public event Action<float> OnStorageWarning;
    #endregion
    
    #region 资源数据
    [Header("玩家资源")]
    [Tooltip("当前玩家的资源数据")]
    public PlayerResources playerResources;
    
    [Header("身份配置")]
    [Tooltip("玩家选择的身份类型")]
    public IdentityType playerIdentity = IdentityType.ConsciousnessLinker;
    #endregion
    
    #region 效率计算配置
    [Header("效率计算配置")]
    [Tooltip("基础效率（默认100%）")]
    public float baseEfficiency = 100f;
    
    [Tooltip("心情值加成系数（每100心情值提供的加成）")]
    public float moodEfficiencyRate = 1f;  // 每100心情值 = 1%加成
    
    [Tooltip("等级加成系数（每级提供的加成）")]
    public float levelEfficiencyRate = 0.5f;  // 每级 = 0.5%加成
    #endregion
    
    #region Unity生命周期方法
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        // 初始化资源系统
        InitializeResources();
    }
    #endregion
    
    #region 初始化方法
    /// <summary>
    /// 初始化资源系统
    /// 根据玩家身份类型初始化资源
    /// </summary>
    private void InitializeResources()
    {
        Debug.Log("=== 初始化资源系统 ===");
        
        // 检查是否已经有资源数据（从保存的游戏加载）
        if (playerResources == null)
        {
            Debug.Log($"创建新的资源配置，身份类型：{playerIdentity}");
            playerResources = new PlayerResources((int)playerIdentity);
        }
        
        // 输出资源信息
        LogResourceInfo();
        
        // 触发资源变化事件
        OnResourcesChanged?.Invoke(playerResources);
        
        Debug.Log("✓ 资源系统初始化完成");
    }
    
    /// <summary>
    /// 设置玩家身份类型
    /// 通常在角色创建时调用
    /// </summary>
    public void SetPlayerIdentity(IdentityType identity)
    {
        playerIdentity = identity;
        Debug.Log($"<color=cyan>选择身份类型：{identity}</color>");
        
        // 根据身份类型初始化资源
        playerResources = new PlayerResources((int)identity);
        
        // 触发事件
        OnResourcesChanged?.Invoke(playerResources);
    }
    #endregion
    
    #region 资源操作方法
    /// <summary>
    /// 尝试分配资源
    /// 例如：开始一份工作需要占用一定的资源
    /// </summary>
    /// <returns>如果资源充足返回true，否则返回false</returns>
    public bool TryAllocateResources(float memory, float cpu, float bandwidth, float computing)
    {
        bool success = playerResources.TryAllocateResources(memory, cpu, bandwidth, computing);
        
        if (success)
        {
            Debug.Log($"<color=green>资源分配成功：</color>" +
                     $"内存 {memory:F1}GB, CPU {cpu:F1}核, " +
                     $"网速 {bandwidth:F0}Mbps, 算力 {computing:F1}");
            
            // 触发资源变化事件
            OnResourcesChanged?.Invoke(playerResources);
        }
        else
        {
            Debug.LogWarning($"<color=red>资源不足！</color>" +
                           $"需要：内存 {memory:F1}GB, CPU {cpu:F1}核, " +
                           $"网速 {bandwidth:F0}Mbps, 算力 {computing:F1}");
        }
        
        return success;
    }
    
    /// <summary>
    /// 释放资源
    /// 例如：辞职后释放工作占用的资源
    /// </summary>
    public void ReleaseResources(float memory, float cpu, float bandwidth, float computing)
    {
        playerResources.ReleaseResources(memory, cpu, bandwidth, computing);
        
        Debug.Log($"<color=green>资源释放：</color>" +
                 $"内存 {memory:F1}GB, CPU {cpu:F1}核, " +
                 $"网速 {bandwidth:F0}Mbps, 算力 {computing:F1}");
        
        // 触发资源变化事件
        OnResourcesChanged?.Invoke(playerResources);
    }
    
    /// <summary>
    /// 升级资源
    /// 在商城购买资源升级
    /// </summary>
    /// <param name="cost">升级所需虚拟币</param>
    public bool TryUpgradeResource(ResourceType type, float amount, int cost)
    {
        // 检查虚拟币是否充足
        if (!playerResources.TrySpendVirtualCoin(cost))
        {
            Debug.LogWarning($"<color=red>虚拟币不足，无法升级{type}</color>");
            return false;
        }
        
        // 升级资源
        playerResources.UpgradeResource(type, amount);
        
        Debug.Log($"<color=green>升级成功！{type} +{amount}</color>");
        
        // 触发事件
        OnResourcesChanged?.Invoke(playerResources);
        OnVirtualCoinChanged?.Invoke(playerResources.virtualCoin);
        
        return true;
    }
    
    /// <summary>
    /// 升级内存
    /// </summary>
    /// <param name="amount">增加的内存量（GB）</param>
    public void UpgradeMemory(float amount)
    {
        playerResources.memoryTotal += amount;
        Debug.Log($"<color=green>内存升级: +{amount}GB，总内存: {playerResources.memoryTotal}GB</color>");
        OnResourcesChanged?.Invoke(playerResources);
    }
    
    /// <summary>
    /// 升级CPU
    /// </summary>
    /// <param name="cores">增加的核心数</param>
    public void UpgradeCPU(int cores)
    {
        playerResources.cpuTotal += cores;
        Debug.Log($"<color=green>CPU升级: +{cores}核，总CPU: {playerResources.cpuTotal}核</color>");
        OnResourcesChanged?.Invoke(playerResources);
    }
    
    /// <summary>
    /// 升级网速
    /// </summary>
    /// <param name="amount">增加的网速（Mbps）</param>
    public void UpgradeBandwidth(float amount)
    {
        playerResources.bandwidthTotal += amount;
        Debug.Log($"<color=green>网速升级: +{amount}Mbps，总网速: {playerResources.bandwidthTotal}Mbps</color>");
        OnResourcesChanged?.Invoke(playerResources);
    }
    
    /// <summary>
    /// 升级算力
    /// </summary>
    /// <param name="amount">增加的算力</param>
    public void UpgradeComputing(float amount)
    {
        playerResources.computingTotal += amount;
        Debug.Log($"<color=green>算力升级: +{amount}，总算力: {playerResources.computingTotal}</color>");
        OnResourcesChanged?.Invoke(playerResources);
    }
    
    /// <summary>
    /// 升级存储空间
    /// </summary>
    /// <param name="amount">增加的存储空间（GB）</param>
    public void UpgradeStorage(float amount)
    {
        playerResources.storageTotal += amount;
        Debug.Log($"<color=green>存储升级: +{amount}GB，总存储: {playerResources.storageTotal}GB</color>");
        OnResourcesChanged?.Invoke(playerResources);
    }
    #endregion
    
    #region 虚拟币操作
    /// <summary>
    /// 添加虚拟币
    /// 通常是工作薪资、任务奖励等
    /// </summary>
    public void AddVirtualCoin(int amount, string source = "")
    {
        playerResources.AddVirtualCoin(amount);
        
        if (!string.IsNullOrEmpty(source))
        {
            Debug.Log($"<color=green>来源：{source}</color>");
        }
        
        // 触发事件
        OnVirtualCoinChanged?.Invoke(playerResources.virtualCoin);
    }
    
    /// <summary>
    /// 添加虚拟币（浮点数版本）
    /// </summary>
    public void AddVirtualCoin(float amount, string source = "")
    {
        AddVirtualCoin(Mathf.RoundToInt(amount), source);
    }
    
    /// <summary>
    /// 尝试扣除虚拟币
    /// 购买物品、支付费用等
    /// </summary>
    public bool TrySpendVirtualCoin(int amount, string purpose = "")
    {
        bool success = playerResources.TrySpendVirtualCoin(amount);
        
        if (success && !string.IsNullOrEmpty(purpose))
        {
            Debug.Log($"<color=yellow>用途：{purpose}</color>");
        }
        
        if (success)
        {
            // 触发事件
            OnVirtualCoinChanged?.Invoke(playerResources.virtualCoin);
        }
        
        return success;
    }
    
    /// <summary>
    /// 获取当前虚拟币数量
    /// </summary>
    public int GetVirtualCoin()
    {
        return playerResources.virtualCoin;
    }
    
    /// <summary>
    /// 检查是否有足够的虚拟币
    /// </summary>
    /// <param name="amount">需要的虚拟币数量</param>
    /// <returns>如果足够返回true，否则返回false</returns>
    public bool CanAfford(float amount)
    {
        return playerResources.virtualCoin >= amount;
    }
    
    /// <summary>
    /// 扣除虚拟币（不返回结果的版本）
    /// </summary>
    /// <param name="amount">要扣除的虚拟币数量</param>
    /// <returns>如果成功扣除返回true，否则返回false</returns>
    public bool SpendVirtualCoin(float amount)
    {
        return TrySpendVirtualCoin(Mathf.RoundToInt(amount));
    }
    
    /// <summary>
    /// 扣除虚拟币（带用途说明的版本）
    /// </summary>
    /// <param name="amount">要扣除的虚拟币数量</param>
    /// <param name="purpose">用途说明</param>
    /// <returns>如果成功扣除返回true，否则返回false</returns>
    public bool SpendVirtualCoin(float amount, string purpose)
    {
        return TrySpendVirtualCoin(Mathf.RoundToInt(amount), purpose);
    }
    #endregion
    
    #region 心情值操作
    /// <summary>
    /// 改变心情值
    /// </summary>
    public void ChangeMoodValue(int amount, string reason = "")
    {
        playerResources.ChangeMoodValue(amount);
        
        if (!string.IsNullOrEmpty(reason))
        {
            Debug.Log($"原因：{reason}");
        }
        
        // 触发事件
        OnMoodValueChanged?.Invoke(playerResources.moodValue);
    }
    
    /// <summary>
    /// 获取当前心情值
    /// </summary>
    public int GetMoodValue()
    {
        return playerResources.moodValue;
    }
    #endregion
    
    #region 效率计算
    /// <summary>
    /// 计算当前的收入效率
    /// 根据游戏设计文档的公式：
    /// 基础效率 = 100%
    /// 空闲资源加成 = (可用内存% + 可用CPU% + 可用网速% + 可用算力%) / 4
    /// 心情值加成 = 心情值 / 100 * 1%
    /// 等级加成 = 等级 * 0.5%
    /// 最终效率 = 基础效率 * (1 + 空闲资源加成 + 心情值加成 + 等级加成)
    /// </summary>
    /// <returns>最终效率（百分比）</returns>
    public float CalculateIncomeEfficiency()
    {
        // 基础效率
        float efficiency = baseEfficiency;
        
        // 空闲资源加成（百分比）
        float idleResourceBonus = playerResources.AverageIdlePercent;
        
        // 心情值加成（百分比）
        float moodBonus = (playerResources.moodValue / 100f) * moodEfficiencyRate;
        
        // 等级加成（百分比）
        float levelBonus = playerResources.level * levelEfficiencyRate;
        
        // 总加成率（转换为小数）
        float totalBonusRate = (idleResourceBonus + moodBonus + levelBonus) / 100f;
        
        // 最终效率
        float finalEfficiency = efficiency * (1f + totalBonusRate);
        
        return finalEfficiency;
    }
    
    /// <summary>
    /// 计算实际收入
    /// 实际收入 = 基础收入 * 效率加成
    /// </summary>
    public int CalculateActualIncome(int baseIncome)
    {
        float efficiency = CalculateIncomeEfficiency();
        int actualIncome = Mathf.RoundToInt(baseIncome * (efficiency / 100f));
        
        Debug.Log($"收入计算：基础 {baseIncome} * 效率 {efficiency:F2}% = 实际 {actualIncome}");
        
        return actualIncome;
    }
    
    /// <summary>
    /// 获取效率加成的详细信息
    /// 用于UI显示
    /// </summary>
    public string GetEfficiencyBreakdown()
    {
        float idleResourceBonus = playerResources.AverageIdlePercent;
        float moodBonus = (playerResources.moodValue / 100f) * moodEfficiencyRate;
        float levelBonus = playerResources.level * levelEfficiencyRate;
        float totalEfficiency = CalculateIncomeEfficiency();
        
        return $"效率详情：\n" +
               $"  基础效率：{baseEfficiency:F1}%\n" +
               $"  空闲资源加成：+{idleResourceBonus:F1}%\n" +
               $"  心情值加成：+{moodBonus:F1}%\n" +
               $"  等级加成：+{levelBonus:F1}%\n" +
               $"  最终效率：{totalEfficiency:F1}%";
    }
    #endregion
    
    #region 数据产生和存储
    /// <summary>
    /// 产生数据（每5分钟调用一次）
    /// </summary>
    public void GenerateData()
    {
        bool success = playerResources.GenerateData();
        
        if (!success)
        {
            // 存储空间不足，触发警告
            OnStorageWarning?.Invoke(playerResources.StorageUsagePercent);
        }
        
        // 检查存储空间是否即将满
        if (playerResources.IsStorageNearlyFull())
        {
            Debug.LogWarning($"<color=yellow>警告：存储空间即将满（{playerResources.StorageUsagePercent:F1}%）</color>");
        }
        
        // 触发资源变化事件
        OnResourcesChanged?.Invoke(playerResources);
    }
    
    /// <summary>
    /// 清理数据
    /// 使用数据清理工具
    /// </summary>
    public void CleanData(float amount)
    {
        playerResources.CleanData(amount);
        
        // 触发资源变化事件
        OnResourcesChanged?.Invoke(playerResources);
    }
    #endregion
    
    #region 等级系统
    /// <summary>
    /// 尝试升级
    /// 检查是否满足升级条件
    /// </summary>
    public bool TryLevelUp()
    {
        // TODO: 根据游戏设计文档实现升级条件检查
        // 例如：总内存、总CPU、总网速、总算力、心情值等要求
        
        // 临时简化版：只检查虚拟币
        int requiredCoin = playerResources.level * 1000;
        
        if (playerResources.virtualCoin >= requiredCoin)
        {
            playerResources.level++;
            Debug.Log($"<color=green>恭喜！升级到 Lv.{playerResources.level}</color>");
            
            // 触发等级提升事件
            OnLevelUp?.Invoke(playerResources.level);
            
            return true;
        }
        
        Debug.LogWarning($"<color=yellow>升级条件未满足</color>");
        return false;
    }
    
    /// <summary>
    /// 获取当前等级
    /// </summary>
    public int GetLevel()
    {
        return playerResources.level;
    }
    
    /// <summary>
    /// 获取玩家等级（别名方法）
    /// </summary>
    public int GetPlayerLevel()
    {
        return GetLevel();
    }
    #endregion
    
    #region 身份特殊逻辑
    /// <summary>
    /// 支付连接费（仅意识连接者）
    /// 每5分钟调用一次
    /// </summary>
    public bool PayConnectionFee()
    {
        // 只有意识连接者需要支付连接费
        if (playerIdentity != IdentityType.ConsciousnessLinker)
        {
            return true;
        }
        
        // 连接费：5-10虚拟币（随机）
        int fee = UnityEngine.Random.Range(5, 11);
        
        bool success = TrySpendVirtualCoin(fee, "意识连接费");
        
        if (!success)
        {
            Debug.LogError($"<color=red>无法支付连接费！账户余额不足</color>");
            // TODO: 触发连接中断事件，可能导致游戏进度暂停
        }
        
        return success;
    }
    #endregion
    
    #region 数据持久化
    /// <summary>
    /// 保存资源数据到PlayerPrefs
    /// 实际项目中应该保存到Firebase Firestore
    /// </summary>
    public void SaveResources()
    {
        if (playerResources == null)
        {
            Debug.LogWarning("没有资源数据可以保存");
            return;
        }
        
        // 这里使用PlayerPrefs作为临时方案
        // 实际项目中应该保存到Firebase
        PlayerPrefs.SetFloat("MemoryTotal", playerResources.memoryTotal);
        PlayerPrefs.SetFloat("MemoryUsed", playerResources.memoryUsed);
        PlayerPrefs.SetFloat("CpuTotal", playerResources.cpuTotal);
        PlayerPrefs.SetFloat("CpuUsed", playerResources.cpuUsed);
        PlayerPrefs.SetFloat("BandwidthTotal", playerResources.bandwidthTotal);
        PlayerPrefs.SetFloat("BandwidthUsed", playerResources.bandwidthUsed);
        PlayerPrefs.SetFloat("ComputingTotal", playerResources.computingTotal);
        PlayerPrefs.SetFloat("ComputingUsed", playerResources.computingUsed);
        PlayerPrefs.SetFloat("StorageTotal", playerResources.storageTotal);
        PlayerPrefs.SetFloat("StorageUsed", playerResources.storageUsed);
        PlayerPrefs.SetInt("MoodValue", playerResources.moodValue);
        PlayerPrefs.SetInt("Level", playerResources.level);
        PlayerPrefs.SetInt("VirtualCoin", playerResources.virtualCoin);
        PlayerPrefs.SetFloat("DataGenerationRate", playerResources.dataGenerationRate);
        PlayerPrefs.SetInt("PlayerIdentity", (int)playerIdentity);
        
        PlayerPrefs.Save();
        
        Debug.Log("资源数据已保存");
    }
    
    /// <summary>
    /// 从PlayerPrefs加载资源数据
    /// </summary>
    public void LoadResources()
    {
        if (!PlayerPrefs.HasKey("Level"))
        {
            Debug.Log("没有找到保存的资源数据");
            return;
        }
        
        playerResources = new PlayerResources();
        
        playerResources.memoryTotal = PlayerPrefs.GetFloat("MemoryTotal", 16f);
        playerResources.memoryUsed = PlayerPrefs.GetFloat("MemoryUsed", 2f);
        playerResources.cpuTotal = PlayerPrefs.GetFloat("CpuTotal", 8f);
        playerResources.cpuUsed = PlayerPrefs.GetFloat("CpuUsed", 1f);
        playerResources.bandwidthTotal = PlayerPrefs.GetFloat("BandwidthTotal", 1000f);
        playerResources.bandwidthUsed = PlayerPrefs.GetFloat("BandwidthUsed", 50f);
        playerResources.computingTotal = PlayerPrefs.GetFloat("ComputingTotal", 100f);
        playerResources.computingUsed = PlayerPrefs.GetFloat("ComputingUsed", 10f);
        playerResources.storageTotal = PlayerPrefs.GetFloat("StorageTotal", 500f);
        playerResources.storageUsed = PlayerPrefs.GetFloat("StorageUsed", 20f);
        playerResources.moodValue = PlayerPrefs.GetInt("MoodValue", 10);
        playerResources.level = PlayerPrefs.GetInt("Level", 1);
        playerResources.virtualCoin = PlayerPrefs.GetInt("VirtualCoin", 100);
        playerResources.dataGenerationRate = PlayerPrefs.GetFloat("DataGenerationRate", 0.5f);
        playerIdentity = (IdentityType)PlayerPrefs.GetInt("PlayerIdentity", 0);
        
        Debug.Log("资源数据已加载");
        
        // 触发资源变化事件
        OnResourcesChanged?.Invoke(playerResources);
    }
    #endregion
    
    #region 辅助方法
    /// <summary>
    /// 输出当前资源信息到控制台
    /// </summary>
    private void LogResourceInfo()
    {
        if (playerResources == null)
        {
            Debug.LogWarning("资源数据为空");
            return;
        }
        
        Debug.Log("=== 当前资源状态 ===");
        Debug.Log(playerResources.ToString());
        Debug.Log($"当前效率：{CalculateIncomeEfficiency():F2}%");
        Debug.Log("==================");
    }
    
    /// <summary>
    /// 获取资源数据的副本
    /// 用于UI显示，避免直接修改原始数据
    /// </summary>
    public PlayerResources GetResourcesCopy()
    {
        // 返回一个浅拷贝
        return playerResources;
    }
    
    /// <summary>
    /// 获取玩家资源对象
    /// </summary>
    public PlayerResources GetPlayerResources()
    {
        return playerResources;
    }
    
    /// <summary>
    /// 赚取虚拟币（别名方法）
    /// </summary>
    public void EarnVirtualCoin(int amount, string source = "")
    {
        AddVirtualCoin(amount, source);
    }
    
    /// <summary>
    /// 获取玩家身份类型
    /// </summary>
    public IdentityType GetPlayerIdentity()
    {
        return playerIdentity;
    }
    
    /// <summary>
    /// 获取可用存储空间
    /// </summary>
    public float GetStorageAvailable()
    {
        return playerResources.StorageAvailable;
    }
    
    /// <summary>
    /// 获取可用网速
    /// </summary>
    public float GetBandwidthAvailable()
    {
        return playerResources.BandwidthAvailable;
    }
    
    /// <summary>
    /// 获取可用算力
    /// </summary>
    public float GetComputingAvailable()
    {
        return playerResources.ComputingAvailable;
    }
    
    /// <summary>
    /// 获取总算力
    /// </summary>
    public float GetComputingTotal()
    {
        return playerResources.computingTotal;
    }
    
    /// <summary>
    /// 添加存储使用量
    /// 例如：购买技能占用存储空间
    /// </summary>
    /// <param name="amount">增加的存储使用量（GB）</param>
    public void AddStorageUsed(float amount)
    {
        playerResources.storageUsed += amount;
        
        // 检查是否超过限制
        if (playerResources.storageUsed >= playerResources.storageTotal * 0.9f)
        {
            Debug.LogWarning($"<color=yellow>⚠ 存储空间不足！已使用{playerResources.StorageUsagePercent:F0}%</color>");
            OnStorageWarning?.Invoke(playerResources.StorageUsagePercent);
        }
        
        // 触发事件
        OnResourcesChanged?.Invoke(playerResources);
    }
    
    /// <summary>
    /// 添加或减少数据产生速率
    /// 例如：开始工作会增加，辞职会减少
    /// </summary>
    /// <param name="amount">速率变化量（可以是负数）</param>
    public void AddDataGenerationRate(float amount)
    {
        playerResources.dataGenerationRate += amount;
        
        // 确保不为负数
        if (playerResources.dataGenerationRate < 0)
        {
            playerResources.dataGenerationRate = 0;
        }
        
        Debug.Log($"数据产生速率更新：{playerResources.dataGenerationRate:F2} GB/5分钟");
        
        // 触发事件
        OnResourcesChanged?.Invoke(playerResources);
    }
    #endregion
}
