using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * 娱乐系统管理器
 * Entertainment System Manager
 * 
 * 功能说明：
 * 1. 管理所有娱乐活动数据
 * 2. 处理玩家参加娱乐活动的流程
 * 3. 追踪正在进行的娱乐活动
 * 4. 计算和发放完成奖励
 * 5. 与生活系统集成（汽车速度加成）
 * 6. 提供事件系统供UI订阅
 * 
 * 使用示例：
 * EntertainmentManager.Instance.StartEntertainment("world_scifi");
 * 
 * Unity操作步骤：
 * 1. 在Hierarchy中找到GameManager对象
 * 2. Add Component → EntertainmentManager
 * 3. 运行游戏，系统会自动初始化
 * 
 * 作者：GitHub Copilot
 * 日期：2025-12-31
 */

/// <summary>
/// 正在进行的娱乐活动信息
/// </summary>
[System.Serializable]
public class OngoingEntertainment
{
    /// <summary>娱乐活动ID</summary>
    public string entertainmentId;
    
    /// <summary>娱乐活动名称</summary>
    public string entertainmentName;
    
    /// <summary>开始时间</summary>
    public float startTime;
    
    /// <summary>实际持续时间（秒，已考虑速度加成）</summary>
    public float actualDuration;
    
    /// <summary>完成后的心情奖励</summary>
    public int moodReward;
    
    /// <summary>
    /// 计算完成进度（0-1）
    /// </summary>
    public float GetProgress()
    {
        float elapsed = Time.time - startTime;
        return Mathf.Clamp01(elapsed / actualDuration);
    }
    
    /// <summary>
    /// 计算剩余时间（秒）
    /// </summary>
    public float GetRemainingTime()
    {
        float elapsed = Time.time - startTime;
        return Mathf.Max(0f, actualDuration - elapsed);
    }
    
    /// <summary>
    /// 是否已完成
    /// </summary>
    public bool IsCompleted()
    {
        return GetRemainingTime() <= 0f;
    }
}

/// <summary>
/// 娱乐系统管理器
/// 单例模式，全局只有一个实例
/// </summary>
public class EntertainmentManager : MonoBehaviour
{
    // ==================== 单例模式 ====================
    
    private static EntertainmentManager instance;
    public static EntertainmentManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EntertainmentManager>();
                if (instance == null)
                {
                    Debug.LogError("[EntertainmentManager] 场景中没有找到EntertainmentManager！");
                }
            }
            return instance;
        }
    }
    
    // ==================== 数据存储 ====================
    
    /// <summary>所有可用的娱乐活动</summary>
    private Dictionary<string, EntertainmentData> entertainmentDatabase;
    
    /// <summary>正在进行的娱乐活动</summary>
    private OngoingEntertainment currentEntertainment;
    
    /// <summary>玩家已购买的真实货币内容</summary>
    private HashSet<string> purchasedContent;
    
    // ==================== 统计数据 ====================
    
    /// <summary>总参加娱乐次数</summary>
    public int totalEntertainmentCount { get; private set; }
    
    /// <summary>总消耗虚拟币</summary>
    public int totalCoinsSpent { get; private set; }
    
    /// <summary>总获得心情值</summary>
    public int totalMoodGained { get; private set; }
    
    // ==================== 事件系统 ====================
    
    /// <summary>开始娱乐活动事件</summary>
    public event Action<string, float> OnEntertainmentStarted;
    
    /// <summary>娱乐活动完成事件</summary>
    public event Action<string, int> OnEntertainmentCompleted;
    
    /// <summary>娱乐活动取消事件</summary>
    public event Action<string> OnEntertainmentCancelled;
    
    // ==================== Unity生命周期 ====================
    
    void Awake()
    {
        // 单例模式检查
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        // 初始化
        entertainmentDatabase = new Dictionary<string, EntertainmentData>();
        purchasedContent = new HashSet<string>();
        currentEntertainment = null;
        totalEntertainmentCount = 0;
        totalCoinsSpent = 0;
        totalMoodGained = 0;
        
        Debug.Log("[EntertainmentManager] 娱乐系统管理器已创建");
    }
    
    void Start()
    {
        InitializeDatabase();
        Debug.Log($"[EntertainmentManager] 娱乐系统初始化完成，共{entertainmentDatabase.Count}个活动");
    }
    
    void Update()
    {
        // 检查当前娱乐活动是否完成
        if (currentEntertainment != null && currentEntertainment.IsCompleted())
        {
            CompleteCurrentEntertainment();
        }
    }
    
    // ==================== 数据库初始化 ====================
    
    /// <summary>
    /// 初始化娱乐活动数据库
    /// 添加所有可用的娱乐活动
    /// </summary>
    private void InitializeDatabase()
    {
        Debug.Log("[EntertainmentManager] 初始化娱乐活动数据库...");
        
        // 添加4个娱乐活动
        AddEntertainment(EntertainmentData.CreateSciFiWorld());
        AddEntertainment(EntertainmentData.CreateApocalypseWorld());
        AddEntertainment(EntertainmentData.CreateMagicWorld());
        AddEntertainment(EntertainmentData.CreateThreeBodyWorld());
        
        Debug.Log($"[EntertainmentManager] 数据库初始化完成，共{entertainmentDatabase.Count}个娱乐活动");
    }
    
    /// <summary>
    /// 添加娱乐活动到数据库
    /// </summary>
    private void AddEntertainment(EntertainmentData data)
    {
        if (entertainmentDatabase.ContainsKey(data.entertainmentId))
        {
            Debug.LogWarning($"[EntertainmentManager] 娱乐活动ID重复: {data.entertainmentId}");
            return;
        }
        
        entertainmentDatabase.Add(data.entertainmentId, data);
        Debug.Log($"[EntertainmentManager] 添加娱乐活动: {data.entertainmentName} (ID: {data.entertainmentId})");
    }
    
    // ==================== 娱乐活动查询 ====================
    
    /// <summary>
    /// 获取所有娱乐活动
    /// </summary>
    public List<EntertainmentData> GetAllEntertainment()
    {
        return new List<EntertainmentData>(entertainmentDatabase.Values);
    }
    
    /// <summary>
    /// 根据ID获取娱乐活动
    /// </summary>
    public EntertainmentData GetEntertainment(string entertainmentId)
    {
        if (entertainmentDatabase.TryGetValue(entertainmentId, out EntertainmentData data))
        {
            return data;
        }
        
        Debug.LogWarning($"[EntertainmentManager] 找不到娱乐活动: {entertainmentId}");
        return null;
    }
    
    /// <summary>
    /// 获取玩家可以参加的娱乐活动
    /// </summary>
    public List<EntertainmentData> GetAvailableEntertainment(int playerLevel, int virtualCoin)
    {
        List<EntertainmentData> available = new List<EntertainmentData>();
        
        foreach (var data in entertainmentDatabase.Values)
        {
            bool hasPurchased = purchasedContent.Contains(data.entertainmentId);
            if (data.CanParticipate(playerLevel, virtualCoin, hasPurchased))
            {
                available.Add(data);
            }
        }
        
        return available;
    }
    
    /// <summary>
    /// 检查是否有正在进行的娱乐活动
    /// </summary>
    public bool HasOngoingEntertainment()
    {
        return currentEntertainment != null;
    }
    
    /// <summary>
    /// 获取当前正在进行的娱乐活动
    /// </summary>
    public OngoingEntertainment GetCurrentEntertainment()
    {
        return currentEntertainment;
    }
    
    // ==================== 娱乐活动操作 ====================
    
    /// <summary>
    /// 开始娱乐活动
    /// </summary>
    /// <param name="entertainmentId">娱乐活动ID</param>
    /// <returns>是否成功开始</returns>
    public bool StartEntertainment(string entertainmentId)
    {
        // 1. 检查是否已有正在进行的娱乐
        if (HasOngoingEntertainment())
        {
            Debug.LogWarning($"[EntertainmentManager] 已有正在进行的娱乐活动: {currentEntertainment.entertainmentName}");
            return false;
        }
        
        // 2. 获取娱乐活动数据
        EntertainmentData data = GetEntertainment(entertainmentId);
        if (data == null)
        {
            Debug.LogError($"[EntertainmentManager] 娱乐活动不存在: {entertainmentId}");
            return false;
        }
        
        // 3. 检查ResourceManager是否存在
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[EntertainmentManager] ResourceManager未初始化！");
            return false;
        }
        
        // 4. 获取玩家资源
        PlayerResources resources = ResourceManager.Instance.GetPlayerResources();
        
        // 5. 检查是否满足参加条件
        bool hasPurchased = purchasedContent.Contains(entertainmentId);
        if (!data.CanParticipate(resources.level, resources.virtualCoin, hasPurchased))
        {
            string reason = data.GetLockReason(resources.level, resources.virtualCoin, hasPurchased);
            Debug.LogWarning($"[EntertainmentManager] 无法参加娱乐活动: {reason}");
            return false;
        }
        
        // 6. 扣除虚拟币
        if (!ResourceManager.Instance.SpendVirtualCoin(data.cost, $"参加娱乐活动：{data.entertainmentName}"))
        {
            Debug.LogError($"[EntertainmentManager] 虚拟币不足！需要{data.cost}币");
            return false;
        }
        
        // 7. 获取汽车速度加成
        float speedBonus = 1.0f;
        if (LifeSystemManager.Instance != null)
        {
            VehicleData vehicle = LifeSystemManager.Instance.GetCurrentVehicle();
            if (vehicle != null)
            {
                speedBonus = vehicle.speedBonus;
            }
        }
        
        // 8. 计算实际持续时间
        float actualDuration = data.CalculateActualDuration(speedBonus);
        
        // 验证持续时间
        if (actualDuration <= 0f)
        {
            Debug.LogError($"[EntertainmentManager] 无效的娱乐持续时间: {actualDuration}秒");
            return false;
        }
        
        // 9. 创建正在进行的娱乐活动
        currentEntertainment = new OngoingEntertainment
        {
            entertainmentId = data.entertainmentId,
            entertainmentName = data.entertainmentName,
            startTime = Time.time,
            actualDuration = actualDuration,
            moodReward = data.moodReward
        };
        
        // 10. 更新统计
        totalEntertainmentCount++;
        totalCoinsSpent += data.cost;
        
        // 11. 触发事件
        OnEntertainmentStarted?.Invoke(data.entertainmentName, actualDuration);
        
        Debug.Log($"[EntertainmentManager] 开始娱乐活动: {data.entertainmentName}，预计{actualDuration / 60f:F1}分钟后完成（速度加成{speedBonus}x）");
        
        return true;
    }
    
    /// <summary>
    /// 取消当前娱乐活动（不退款，不获得奖励）
    /// </summary>
    public bool CancelCurrentEntertainment()
    {
        if (currentEntertainment == null)
        {
            Debug.LogWarning("[EntertainmentManager] 没有正在进行的娱乐活动");
            return false;
        }
        
        string name = currentEntertainment.entertainmentName;
        
        // 触发事件
        OnEntertainmentCancelled?.Invoke(name);
        
        // 清除当前娱乐
        currentEntertainment = null;
        
        Debug.Log($"[EntertainmentManager] 取消娱乐活动: {name}");
        
        return true;
    }
    
    /// <summary>
    /// 完成当前娱乐活动
    /// </summary>
    private void CompleteCurrentEntertainment()
    {
        if (currentEntertainment == null)
        {
            return;
        }
        
        string name = currentEntertainment.entertainmentName;
        int moodReward = currentEntertainment.moodReward;
        
        // 发放心情值奖励
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.ChangeMoodValue(moodReward, $"完成娱乐活动：{name}");
        }
        
        // 更新统计
        totalMoodGained += moodReward;
        
        // 触发事件
        OnEntertainmentCompleted?.Invoke(name, moodReward);
        
        Debug.Log($"[EntertainmentManager] 完成娱乐活动: {name}，获得{moodReward}心情值");
        
        // 清除当前娱乐
        currentEntertainment = null;
    }
    
    // ==================== 真实货币购买 ====================
    
    /// <summary>
    /// 记录玩家购买了某个真实货币内容
    /// （实际支付在Phase 8实现）
    /// </summary>
    public void MarkContentAsPurchased(string entertainmentId)
    {
        if (!entertainmentDatabase.ContainsKey(entertainmentId))
        {
            Debug.LogWarning($"[EntertainmentManager] 娱乐活动不存在: {entertainmentId}");
            return;
        }
        
        if (purchasedContent.Contains(entertainmentId))
        {
            Debug.LogWarning($"[EntertainmentManager] 已购买过该内容: {entertainmentId}");
            return;
        }
        
        purchasedContent.Add(entertainmentId);
        Debug.Log($"[EntertainmentManager] 记录购买内容: {entertainmentId}");
    }
    
    /// <summary>
    /// 检查是否已购买某个内容
    /// </summary>
    public bool HasPurchased(string entertainmentId)
    {
        return purchasedContent.Contains(entertainmentId);
    }
    
    // ==================== 统计查询 ====================
    
    /// <summary>
    /// 获取统计摘要
    /// </summary>
    public string GetStatsSummary()
    {
        return $"总参加次数: {totalEntertainmentCount}\n" +
               $"总消耗: {totalCoinsSpent}币\n" +
               $"总心情: +{totalMoodGained}\n" +
               $"平均效率: {(totalEntertainmentCount > 0 ? (float)totalMoodGained / totalCoinsSpent : 0):F2}心情/币";
    }
    
    // ==================== 数据持久化 ====================
    
    /// <summary>
    /// 保存数据到Firebase（预留接口，Phase 8实现）
    /// </summary>
    public void SaveData()
    {
        // TODO: 保存到Firebase
        // - currentEntertainment（如果有）
        // - purchasedContent
        // - 统计数据
        Debug.Log("[EntertainmentManager] 数据保存功能将在Phase 8实现");
    }
    
    /// <summary>
    /// 从Firebase加载数据（预留接口，Phase 8实现）
    /// </summary>
    public void LoadData()
    {
        // TODO: 从Firebase加载
        Debug.Log("[EntertainmentManager] 数据加载功能将在Phase 8实现");
    }
    
    // ==================== 调试功能 ====================
    
    /// <summary>
    /// 打印所有娱乐活动信息（用于调试）
    /// </summary>
    public void DebugPrintAllEntertainment()
    {
        Debug.Log("========== 娱乐活动列表 ==========");
        foreach (var data in entertainmentDatabase.Values)
        {
            Debug.Log(data.ToString());
            Debug.Log("---");
        }
        Debug.Log("================================");
    }
}
