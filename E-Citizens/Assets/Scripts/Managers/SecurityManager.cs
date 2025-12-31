using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * 安全卫士管理器
 * Security Manager
 * 
 * 功能说明：
 * 1. 管理所有安全卫士方案
 * 2. 处理订阅/取消订阅
 * 3. 每5分钟自动扣除安全卫士费用
 * 4. 提供防御率查询
 * 5. 与GameTimerManager集成
 * 6. 提供事件系统供UI订阅
 * 
 * 使用示例：
 * SecurityManager.Instance.Subscribe(SecurityPlanType.Basic);
 * bool defended = SecurityManager.Instance.TryDefendAgainstVirus();
 * 
 * Unity操作步骤：
 * 1. 在Hierarchy中找到GameManager对象
 * 2. Add Component → SecurityManager
 * 3. 运行游戏，系统会自动初始化
 * 4. 系统会自动订阅GameTimerManager的周期事件
 * 
 * 作者：GitHub Copilot
 * 日期：2025-12-31
 */

/// <summary>
/// 安全卫士管理器
/// 单例模式，全局只有一个实例
/// </summary>
public class SecurityManager : MonoBehaviour
{
    // ==================== 单例模式 ====================
    
    private static SecurityManager instance;
    public static SecurityManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SecurityManager>();
                if (instance == null)
                {
                    Debug.LogError("[SecurityManager] 场景中没有找到SecurityManager！");
                }
            }
            return instance;
        }
    }
    
    // ==================== 数据存储 ====================
    
    /// <summary>所有可用的安全卫士方案</summary>
    private Dictionary<SecurityPlanType, SecurityPlanData> planDatabase;
    
    /// <summary>当前订阅的方案类型</summary>
    private SecurityPlanType currentPlanType;
    
    /// <summary>当前订阅的方案数据</summary>
    private SecurityPlanData currentPlan;
    
    // ==================== 统计数据 ====================
    
    /// <summary>总支付费用</summary>
    public int totalCostPaid { get; private set; }
    
    /// <summary>总防御成功次数</summary>
    public int totalDefenseSuccess { get; private set; }
    
    /// <summary>总防御失败次数</summary>
    public int totalDefenseFail { get; private set; }
    
    // ==================== 事件系统 ====================
    
    /// <summary>订阅方案事件（方案名称，费用）</summary>
    public event Action<string, int> OnPlanSubscribed;
    
    /// <summary>取消订阅事件</summary>
    public event Action OnPlanCancelled;
    
    /// <summary>支付费用事件（费用）</summary>
    public event Action<int> OnFeePaid;
    
    /// <summary>防御成功事件（方案名称）</summary>
    public event Action<string> OnDefenseSuccess;
    
    /// <summary>防御失败事件</summary>
    public event Action OnDefenseFail;
    
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
        planDatabase = new Dictionary<SecurityPlanType, SecurityPlanData>();
        currentPlanType = SecurityPlanType.None;
        currentPlan = null;
        totalCostPaid = 0;
        totalDefenseSuccess = 0;
        totalDefenseFail = 0;
        
        Debug.Log("[SecurityManager] 安全卫士管理器已创建");
    }
    
    void Start()
    {
        InitializeDatabase();
        
        // 订阅游戏周期事件
        if (GameTimerManager.Instance != null)
        {
            GameTimerManager.Instance.OnGameTickEnd += OnGameTick;
            Debug.Log("[SecurityManager] 已订阅游戏周期事件");
        }
        else
        {
            Debug.LogWarning("[SecurityManager] GameTimerManager未找到，无法订阅周期事件");
        }
        
        Debug.Log($"[SecurityManager] 安全卫士系统初始化完成，共{planDatabase.Count}个方案");
    }
    
    void OnDestroy()
    {
        // 取消订阅游戏周期事件
        if (GameTimerManager.Instance != null)
        {
            GameTimerManager.Instance.OnGameTickEnd -= OnGameTick;
        }
    }
    
    // ==================== 数据库初始化 ====================
    
    /// <summary>
    /// 初始化安全卫士方案数据库
    /// </summary>
    private void InitializeDatabase()
    {
        Debug.Log("[SecurityManager] 初始化安全卫士方案数据库...");
        
        // 添加4个方案
        AddPlan(SecurityPlanData.CreateNonePlan());
        AddPlan(SecurityPlanData.CreateBasicPlan());
        AddPlan(SecurityPlanData.CreateAdvancedPlan());
        AddPlan(SecurityPlanData.CreateUltimatePlan());
        
        // 默认为无安全卫士
        currentPlan = planDatabase[SecurityPlanType.None];
        
        Debug.Log($"[SecurityManager] 数据库初始化完成，共{planDatabase.Count}个方案");
    }
    
    /// <summary>
    /// 添加方案到数据库
    /// </summary>
    private void AddPlan(SecurityPlanData data)
    {
        if (planDatabase.ContainsKey(data.planType))
        {
            Debug.LogWarning($"[SecurityManager] 方案类型重复: {data.planType}");
            return;
        }
        
        planDatabase.Add(data.planType, data);
        Debug.Log($"[SecurityManager] 添加方案: {data.planName} (防御率{data.GetDefenseRateString()})");
    }
    
    // ==================== 方案查询 ====================
    
    /// <summary>
    /// 获取所有方案
    /// </summary>
    public List<SecurityPlanData> GetAllPlans()
    {
        return new List<SecurityPlanData>(planDatabase.Values);
    }
    
    /// <summary>
    /// 根据类型获取方案
    /// </summary>
    public SecurityPlanData GetPlan(SecurityPlanType planType)
    {
        if (planDatabase.TryGetValue(planType, out SecurityPlanData data))
        {
            return data;
        }
        
        Debug.LogWarning($"[SecurityManager] 找不到方案: {planType}");
        return null;
    }
    
    /// <summary>
    /// 获取当前订阅的方案
    /// </summary>
    public SecurityPlanData GetCurrentPlan()
    {
        return currentPlan;
    }
    
    /// <summary>
    /// 获取当前订阅的方案类型
    /// </summary>
    public SecurityPlanType GetCurrentPlanType()
    {
        return currentPlanType;
    }
    
    /// <summary>
    /// 获取当前防御率
    /// </summary>
    public float GetCurrentDefenseRate()
    {
        return currentPlan != null ? currentPlan.defenseRate : 0f;
    }
    
    /// <summary>
    /// 获取当前每周期费用
    /// </summary>
    public int GetCurrentCost()
    {
        return currentPlan != null ? currentPlan.costPer5Min : 0;
    }
    
    // ==================== 订阅操作 ====================
    
    /// <summary>
    /// 订阅安全卫士方案
    /// </summary>
    /// <param name="planType">方案类型</param>
    /// <returns>是否成功订阅</returns>
    public bool Subscribe(SecurityPlanType planType)
    {
        // 1. 检查是否已经是该方案
        if (currentPlanType == planType)
        {
            Debug.LogWarning($"[SecurityManager] 已经订阅了该方案: {planType}");
            return false;
        }
        
        // 2. 获取方案数据
        SecurityPlanData plan = GetPlan(planType);
        if (plan == null)
        {
            Debug.LogError($"[SecurityManager] 方案不存在: {planType}");
            return false;
        }
        
        // 3. 检查ResourceManager
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[SecurityManager] ResourceManager未初始化！");
            return false;
        }
        
        // 4. 检查等级
        PlayerResources resources = ResourceManager.Instance.GetPlayerResources();
        if (!plan.IsUnlocked(resources.level))
        {
            Debug.LogWarning($"[SecurityManager] 等级不足，需要等级{plan.unlockLevel}");
            return false;
        }
        
        // 5. 更新当前方案
        currentPlanType = planType;
        currentPlan = plan;
        
        // 6. 触发事件
        OnPlanSubscribed?.Invoke(plan.planName, plan.costPer5Min);
        
        Debug.Log($"[SecurityManager] 订阅方案: {plan.planName}（防御率{plan.GetDefenseRateString()}，费用{plan.costPer5Min}币/5分钟）");
        
        return true;
    }
    
    /// <summary>
    /// 取消当前订阅（切换回"无"方案）
    /// </summary>
    public void Unsubscribe()
    {
        if (currentPlanType == SecurityPlanType.None)
        {
            Debug.LogWarning("[SecurityManager] 当前没有订阅任何方案");
            return;
        }
        
        currentPlanType = SecurityPlanType.None;
        currentPlan = planDatabase[SecurityPlanType.None];
        
        OnPlanCancelled?.Invoke();
        
        Debug.Log("[SecurityManager] 已取消安全卫士订阅");
    }
    
    // ==================== 游戏周期处理 ====================
    
    /// <summary>
    /// 游戏周期回调（每5分钟调用一次）
    /// </summary>
    private void OnGameTick()
    {
        // 扣除安全卫士费用
        PaySecurityFee();
    }
    
    /// <summary>
    /// 支付安全卫士费用
    /// </summary>
    private void PaySecurityFee()
    {
        if (currentPlan == null || currentPlan.IsFree())
        {
            return; // 无方案或免费方案，无需支付
        }
        
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[SecurityManager] ResourceManager未初始化！");
            return;
        }
        
        int cost = currentPlan.costPer5Min;
        
        // 尝试扣费
        if (ResourceManager.Instance.SpendVirtualCoin(cost, $"安全卫士费用：{currentPlan.planName}"))
        {
            totalCostPaid += cost;
            OnFeePaid?.Invoke(cost);
            Debug.Log($"[SecurityManager] 支付安全卫士费用: {cost}币（{currentPlan.planName}）");
        }
        else
        {
            // 虚拟币不足，自动取消订阅
            Debug.LogWarning($"[SecurityManager] 虚拟币不足，自动取消安全卫士订阅");
            Unsubscribe();
        }
    }
    
    // ==================== 防御机制 ====================
    
    /// <summary>
    /// 尝试防御病毒入侵
    /// </summary>
    /// <returns>true表示防御成功，false表示需要玩家应对</returns>
    public bool TryDefendAgainstVirus()
    {
        if (currentPlan == null || !currentPlan.HasDefense())
        {
            totalDefenseFail++;
            OnDefenseFail?.Invoke();
            Debug.Log("[SecurityManager] 无安全卫士防护，病毒入侵！");
            return false;
        }
        
        // 尝试防御
        bool defended = currentPlan.TryDefend();
        
        if (defended)
        {
            totalDefenseSuccess++;
            OnDefenseSuccess?.Invoke(currentPlan.planName);
            Debug.Log($"<color=green>[SecurityManager] {currentPlan.planName}成功拦截病毒！</color>");
        }
        else
        {
            totalDefenseFail++;
            OnDefenseFail?.Invoke();
            Debug.Log($"[SecurityManager] {currentPlan.planName}未能拦截病毒，需要玩家应对");
        }
        
        return defended;
    }
    
    // ==================== 统计查询 ====================
    
    /// <summary>
    /// 获取统计摘要
    /// </summary>
    public string GetStatsSummary()
    {
        int totalAttempts = totalDefenseSuccess + totalDefenseFail;
        float successRate = totalAttempts > 0 ? (float)totalDefenseSuccess / totalAttempts * 100f : 0f;
        
        return $"当前方案: {(currentPlan != null ? currentPlan.planName : "无")}\n" +
               $"总支付: {totalCostPaid}币\n" +
               $"防御成功: {totalDefenseSuccess}次\n" +
               $"防御失败: {totalDefenseFail}次\n" +
               $"成功率: {successRate:F1}%";
    }
    
    // ==================== 数据持久化 ====================
    
    /// <summary>
    /// 保存数据到Firebase（预留接口，Phase 8实现）
    /// </summary>
    public void SaveData()
    {
        // TODO: 保存到Firebase
        // - currentPlanType
        // - 统计数据
        Debug.Log("[SecurityManager] 数据保存功能将在Phase 8实现");
    }
    
    /// <summary>
    /// 从Firebase加载数据（预留接口，Phase 8实现）
    /// </summary>
    public void LoadData()
    {
        // TODO: 从Firebase加载
        Debug.Log("[SecurityManager] 数据加载功能将在Phase 8实现");
    }
    
    // ==================== 调试功能 ====================
    
    /// <summary>
    /// 打印所有方案信息（用于调试）
    /// </summary>
    public void DebugPrintAllPlans()
    {
        Debug.Log("========== 安全卫士方案列表 ==========");
        foreach (var plan in planDatabase.Values)
        {
            Debug.Log(plan.ToString());
            Debug.Log("---");
        }
        Debug.Log("================================");
    }
}
