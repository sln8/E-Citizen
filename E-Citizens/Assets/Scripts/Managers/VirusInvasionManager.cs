using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * 病毒入侵管理器
 * Virus Invasion Manager
 * 
 * 功能说明：
 * 1. 管理病毒入侵的随机触发（30-60分钟间隔）
 * 2. 与安全卫士系统集成（防御检测）
 * 3. 管理病毒入侵游戏状态
 * 4. 计算失败惩罚（1%-5%虚拟币损失）
 * 5. 计算成功奖励（击杀数*5虚拟币+心情）
 * 6. 与GameTimerManager集成
 * 
 * 使用示例：
 * VirusInvasionManager.Instance.CheckForInvasion(); // 检查是否触发入侵
 * 
 * Unity操作步骤：
 * 1. 在Hierarchy中找到GameManager对象
 * 2. Add Component → VirusInvasionManager
 * 3. 运行游戏，系统会自动初始化
 * 4. 系统会在30-60分钟内随机触发病毒入侵
 * 
 * 注意：
 * - 实际的塔防小游戏由VirusGameController实现
 * - 本管理器只负责触发、奖惩和状态管理
 * 
 * 作者：GitHub Copilot
 * 日期：2025-12-31
 */

/// <summary>
/// 病毒入侵游戏状态
/// </summary>
public enum VirusGameState
{
    Idle,           // 空闲（等待下次入侵）
    Triggered,      // 已触发（等待游戏开始）
    Playing,        // 游戏进行中
    Completed       // 游戏完成（等待结算）
}

/// <summary>
/// 病毒入侵游戏结果
/// </summary>
public class VirusGameResult
{
    /// <summary>是否成功防御</summary>
    public bool success;
    
    /// <summary>击杀的病毒数量</summary>
    public int virusKilled;
    
    /// <summary>游戏内获得的金币</summary>
    public int coinsEarned;
    
    /// <summary>城墙剩余血量</summary>
    public int wallHealthRemaining;
}

/// <summary>
/// 病毒入侵管理器
/// 单例模式，全局只有一个实例
/// </summary>
public class VirusInvasionManager : MonoBehaviour
{
    // ==================== 单例模式 ====================
    
    private static VirusInvasionManager instance;
    public static VirusInvasionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<VirusInvasionManager>();
                if (instance == null)
                {
                    Debug.LogError("[VirusInvasionManager] 场景中没有找到VirusInvasionManager！");
                }
            }
            return instance;
        }
    }
    
    // ==================== 配置参数 ====================
    
    [Header("触发设置")]
    [Tooltip("最小触发间隔（秒），默认1800秒=30分钟")]
    public float minInterval = 1800f;
    
    [Tooltip("最大触发间隔（秒），默认3600秒=60分钟")]
    public float maxInterval = 3600f;
    
    [Tooltip("调试模式（缩短间隔到30-60秒）")]
    public bool debugMode = false;
    
    [Header("惩罚与奖励")]
    [Tooltip("失败时最小损失比例（0.01 = 1%）")]
    public float minLossPercentage = 0.01f;
    
    [Tooltip("失败时最大损失比例（0.05 = 5%）")]
    public float maxLossPercentage = 0.05f;
    
    [Tooltip("成功时每击杀一个病毒的虚拟币奖励")]
    public int rewardPerKill = 5;
    
    [Tooltip("成功时的心情值奖励")]
    public int successMoodReward = 10;
    
    // ==================== 运行时状态 ====================
    
    /// <summary>当前游戏状态</summary>
    private VirusGameState currentState;
    
    /// <summary>下次触发的时间</summary>
    private float nextInvasionTime;
    
    /// <summary>当前游戏结果（游戏进行中或完成时）</summary>
    private VirusGameResult currentGameResult;
    
    // ==================== 统计数据 ====================
    
    /// <summary>总入侵次数</summary>
    public int totalInvasions { get; private set; }
    
    /// <summary>成功防御次数</summary>
    public int successfulDefenses { get; private set; }
    
    /// <summary>失败次数</summary>
    public int failedDefenses { get; private set; }
    
    /// <summary>安全卫士直接拦截次数</summary>
    public int securityBlocked { get; private set; }
    
    /// <summary>总损失虚拟币</summary>
    public int totalCoinsLost { get; private set; }
    
    /// <summary>总获得虚拟币</summary>
    public int totalCoinsEarned { get; private set; }
    
    // ==================== 事件系统 ====================
    
    /// <summary>病毒入侵触发事件</summary>
    public event Action OnInvasionTriggered;
    
    /// <summary>安全卫士成功拦截事件</summary>
    public event Action OnSecurityBlocked;
    
    /// <summary>游戏开始事件</summary>
    public event Action OnGameStarted;
    
    /// <summary>游戏成功事件（击杀数，虚拟币奖励，心情奖励）</summary>
    public event Action<int, int, int> OnGameSuccess;
    
    /// <summary>游戏失败事件（损失虚拟币）</summary>
    public event Action<int> OnGameFailed;
    
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
        currentState = VirusGameState.Idle;
        currentGameResult = null;
        totalInvasions = 0;
        successfulDefenses = 0;
        failedDefenses = 0;
        securityBlocked = 0;
        totalCoinsLost = 0;
        totalCoinsEarned = 0;
        
        Debug.Log("[VirusInvasionManager] 病毒入侵管理器已创建");
    }
    
    void Start()
    {
        // 设置第一次入侵时间
        ScheduleNextInvasion();
        
        Debug.Log($"[VirusInvasionManager] 病毒入侵系统初始化完成");
        Debug.Log($"[VirusInvasionManager] 下次入侵时间: {GetTimeUntilNextInvasion() / 60f:F1}分钟后");
    }
    
    void Update()
    {
        // 检查是否到达入侵时间
        if (currentState == VirusGameState.Idle && Time.time >= nextInvasionTime)
        {
            TriggerInvasion();
        }
    }
    
    // ==================== 触发机制 ====================
    
    /// <summary>
    /// 安排下次入侵时间
    /// </summary>
    private void ScheduleNextInvasion()
    {
        float minTime = debugMode ? 30f : minInterval;
        float maxTime = debugMode ? 60f : maxInterval;
        
        float randomInterval = UnityEngine.Random.Range(minTime, maxTime);
        nextInvasionTime = Time.time + randomInterval;
        
        string timeStr = debugMode ? $"{randomInterval:F0}秒" : $"{randomInterval / 60f:F1}分钟";
        Debug.Log($"[VirusInvasionManager] 下次入侵安排在{timeStr}后");
    }
    
    /// <summary>
    /// 获取距离下次入侵的时间（秒）
    /// </summary>
    public float GetTimeUntilNextInvasion()
    {
        if (currentState != VirusGameState.Idle)
        {
            return 0f;
        }
        
        return Mathf.Max(0f, nextInvasionTime - Time.time);
    }
    
    /// <summary>
    /// 触发病毒入侵
    /// </summary>
    private void TriggerInvasion()
    {
        Debug.Log("<color=red>[VirusInvasionManager] ⚠️ 病毒入侵警报！</color>");
        
        totalInvasions++;
        currentState = VirusGameState.Triggered;
        
        // 触发事件
        OnInvasionTriggered?.Invoke();
        
        // 检查安全卫士防御
        if (SecurityManager.Instance != null)
        {
            bool defended = SecurityManager.Instance.TryDefendAgainstVirus();
            
            if (defended)
            {
                // 安全卫士成功拦截
                HandleSecurityBlock();
                return;
            }
        }
        
        // 需要玩家应对，启动游戏
        StartVirusGame();
    }
    
    /// <summary>
    /// 安全卫士成功拦截病毒
    /// </summary>
    private void HandleSecurityBlock()
    {
        securityBlocked++;
        OnSecurityBlocked?.Invoke();
        
        Debug.Log("<color=green>[VirusInvasionManager] ✓ 安全卫士成功拦截病毒！</color>");
        
        // 重新安排下次入侵
        currentState = VirusGameState.Idle;
        ScheduleNextInvasion();
    }
    
    // ==================== 游戏管理 ====================
    
    /// <summary>
    /// 开始病毒入侵游戏
    /// </summary>
    public void StartVirusGame()
    {
        if (currentState != VirusGameState.Triggered)
        {
            Debug.LogWarning($"[VirusInvasionManager] 当前状态不允许开始游戏: {currentState}");
            return;
        }
        
        currentState = VirusGameState.Playing;
        currentGameResult = new VirusGameResult
        {
            success = false,
            virusKilled = 0,
            coinsEarned = 0,
            wallHealthRemaining = 0
        };
        
        OnGameStarted?.Invoke();
        
        Debug.Log("[VirusInvasionManager] 病毒入侵游戏开始！");
        
        // 注意：实际的游戏逻辑由VirusGameController实现
        // VirusGameController会调用CompleteGame()来通知结果
    }
    
    /// <summary>
    /// 完成游戏（由VirusGameController调用）
    /// </summary>
    /// <param name="result">游戏结果</param>
    public void CompleteGame(VirusGameResult result)
    {
        if (currentState != VirusGameState.Playing)
        {
            Debug.LogWarning($"[VirusInvasionManager] 当前状态不允许完成游戏: {currentState}");
            return;
        }
        
        currentGameResult = result;
        currentState = VirusGameState.Completed;
        
        Debug.Log($"[VirusInvasionManager] 游戏完成 - 成功:{result.success}, 击杀:{result.virusKilled}");
        
        // 结算奖惩
        SettleGameResult();
    }
    
    /// <summary>
    /// 结算游戏结果
    /// </summary>
    private void SettleGameResult()
    {
        if (currentGameResult == null)
        {
            Debug.LogError("[VirusInvasionManager] 游戏结果为空！");
            return;
        }
        
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[VirusInvasionManager] ResourceManager未初始化！");
            return;
        }
        
        if (currentGameResult.success)
        {
            // 成功防御
            HandleSuccess();
        }
        else
        {
            // 防御失败
            HandleFailure();
        }
        
        // 重置状态，安排下次入侵
        currentState = VirusGameState.Idle;
        currentGameResult = null;
        ScheduleNextInvasion();
    }
    
    /// <summary>
    /// 处理成功防御
    /// </summary>
    private void HandleSuccess()
    {
        successfulDefenses++;
        
        // 计算虚拟币奖励
        int coinReward = currentGameResult.virusKilled * rewardPerKill;
        totalCoinsEarned += coinReward;
        
        // 发放虚拟币
        ResourceManager.Instance.EarnVirtualCoin(coinReward, "成功防御病毒入侵");
        
        // 发放心情值
        ResourceManager.Instance.ChangeMoodValue(successMoodReward, "成功防御病毒入侵");
        
        // 触发事件
        OnGameSuccess?.Invoke(currentGameResult.virusKilled, coinReward, successMoodReward);
        
        Debug.Log($"<color=green>[VirusInvasionManager] ✓ 防御成功！</color>");
        Debug.Log($"<color=green>击杀病毒: {currentGameResult.virusKilled}个</color>");
        Debug.Log($"<color=green>获得虚拟币: {coinReward}</color>");
        Debug.Log($"<color=green>获得心情值: +{successMoodReward}</color>");
    }
    
    /// <summary>
    /// 处理防御失败
    /// </summary>
    private void HandleFailure()
    {
        failedDefenses++;
        
        // 计算损失（总虚拟币的1%-5%）
        PlayerResources resources = ResourceManager.Instance.GetPlayerResources();
        float lossPercentage = UnityEngine.Random.Range(minLossPercentage, maxLossPercentage);
        int coinsLost = Mathf.RoundToInt(resources.virtualCoin * lossPercentage);
        
        // 至少损失1币（如果有币），但不超过当前虚拟币
        if (resources.virtualCoin > 0)
        {
            coinsLost = Mathf.Clamp(coinsLost, 1, resources.virtualCoin);
        }
        else
        {
            coinsLost = 0; // 没有虚拟币，不扣费
        }
        
        totalCoinsLost += coinsLost;
        
        // 扣除虚拟币
        ResourceManager.Instance.SpendVirtualCoin(coinsLost, "病毒入侵失败惩罚");
        
        // 触发事件
        OnGameFailed?.Invoke(coinsLost);
        
        Debug.Log($"<color=red>[VirusInvasionManager] ✗ 防御失败！</color>");
        Debug.Log($"<color=red>被盗虚拟币: {coinsLost} ({lossPercentage * 100f:F1}%)</color>");
    }
    
    // ==================== 状态查询 ====================
    
    /// <summary>
    /// 获取当前游戏状态
    /// </summary>
    public VirusGameState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// 检查是否正在进行游戏
    /// </summary>
    public bool IsGameActive()
    {
        return currentState == VirusGameState.Playing;
    }
    
    /// <summary>
    /// 获取当前游戏结果（仅在游戏进行中或完成时有效）
    /// </summary>
    public VirusGameResult GetCurrentGameResult()
    {
        return currentGameResult;
    }
    
    // ==================== 统计查询 ====================
    
    /// <summary>
    /// 获取统计摘要
    /// </summary>
    public string GetStatsSummary()
    {
        int totalDefended = successfulDefenses + securityBlocked;
        float successRate = totalInvasions > 0 ? (float)totalDefended / totalInvasions * 100f : 0f;
        
        return $"总入侵: {totalInvasions}次\n" +
               $"成功防御: {successfulDefenses}次\n" +
               $"失败防御: {failedDefenses}次\n" +
               $"安全卫士拦截: {securityBlocked}次\n" +
               $"成功率: {successRate:F1}%\n" +
               $"总损失: {totalCoinsLost}币\n" +
               $"总获得: {totalCoinsEarned}币\n" +
               $"净收益: {totalCoinsEarned - totalCoinsLost}币";
    }
    
    // ==================== 调试功能 ====================
    
    /// <summary>
    /// 强制触发病毒入侵（用于测试）
    /// </summary>
    public void DebugTriggerInvasion()
    {
        if (currentState != VirusGameState.Idle)
        {
            Debug.LogWarning($"[VirusInvasionManager] 当前状态不允许触发入侵: {currentState}");
            return;
        }
        
        Debug.Log("[VirusInvasionManager] [DEBUG] 强制触发病毒入侵");
        TriggerInvasion();
    }
    
    /// <summary>
    /// 模拟游戏完成（用于测试）
    /// </summary>
    public void DebugSimulateGameResult(bool success, int virusKilled = 10)
    {
        if (currentState != VirusGameState.Playing)
        {
            Debug.LogWarning($"[VirusInvasionManager] 当前状态不允许完成游戏: {currentState}");
            return;
        }
        
        VirusGameResult result = new VirusGameResult
        {
            success = success,
            virusKilled = virusKilled,
            coinsEarned = virusKilled * 5,
            wallHealthRemaining = success ? 500 : 0
        };
        
        Debug.Log($"[VirusInvasionManager] [DEBUG] 模拟游戏结果: {(success ? "成功" : "失败")}");
        CompleteGame(result);
    }
}
