using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

/*
 * Phase 6 综合测试UI
 * Phase 6 Comprehensive Test UI
 * 
 * 功能说明：
 * 1. 娱乐系统测试（开始娱乐、查看进度）
 * 2. 安全卫士系统测试（订阅、取消、查看防御率）
 * 3. 病毒入侵系统测试（触发入侵、模拟结果）
 * 4. 统计数据显示
 * 5. 实时状态更新
 * 
 * Unity操作步骤：
 * 1. 创建Canvas（如果没有）：Hierarchy右键 → UI → Canvas
 * 2. 在Canvas下创建Panel：右键 → UI → Panel，命名为"Phase6TestPanel"
 * 3. 在Panel中创建以下UI元素：
 *    - TextMeshProUGUI：命名为"StatusText"（显示所有状态）
 *    - Button组（娱乐系统）：
 *      * "StartEnt1Btn" - 星际战争
 *      * "StartEnt2Btn" - 末日求生
 *      * "StartEnt3Btn" - 魔法学院
 *      * "CancelEntBtn" - 取消娱乐
 *    - Button组（安全卫士）：
 *      * "SubNoneBtn" - 取消订阅
 *      * "SubBasicBtn" - 订阅普通
 *      * "SubAdvancedBtn" - 订阅高级
 *      * "SubUltimateBtn" - 订阅神级
 *    - Button组（病毒入侵）：
 *      * "TriggerVirusBtn" - 触发入侵
 *      * "SimSuccessBtn" - 模拟成功
 *      * "SimFailBtn" - 模拟失败
 *    - Button组（测试）：
 *      * "AddCoinsBtn" - 添加10000币
 *      * "AddLevelBtn" - 提升10级
 *      * "ShowStatsBtn" - 显示统计
 * 4. 创建空物体"Phase6TestUI"，添加此脚本
 * 5. 在Inspector中连接所有UI引用
 * 
 * 作者：GitHub Copilot
 * 日期：2025-12-31
 */

/// <summary>
/// Phase 6 综合测试UI
/// 整合娱乐、安全卫士、病毒入侵三大系统的测试功能
/// </summary>
public class Phase6TestUI : MonoBehaviour
{
    [Header("UI引用")]
    [Tooltip("状态文本（显示所有信息）")]
    public TextMeshProUGUI statusText;
    
    [Header("娱乐系统按钮")]
    public Button startEnt1Btn;     // 星际战争
    public Button startEnt2Btn;     // 末日求生
    public Button startEnt3Btn;     // 魔法学院
    public Button cancelEntBtn;     // 取消娱乐
    
    [Header("安全卫士按钮")]
    public Button subNoneBtn;       // 取消订阅
    public Button subBasicBtn;      // 普通安全卫士
    public Button subAdvancedBtn;   // 高级安全卫士
    public Button subUltimateBtn;   // 神级安全卫士
    
    [Header("病毒入侵按钮")]
    public Button triggerVirusBtn;  // 触发病毒入侵
    public Button simSuccessBtn;    // 模拟成功
    public Button simFailBtn;       // 模拟失败
    
    [Header("测试按钮")]
    public Button addCoinsBtn;      // 添加虚拟币
    public Button addLevelBtn;      // 提升等级
    public Button showStatsBtn;     // 显示统计
    
    [Header("更新设置")]
    [Tooltip("UI更新间隔（秒）")]
    public float updateInterval = 0.5f;
    
    // 私有变量
    private float lastUpdateTime;
    
    void Start()
    {
        // 连接娱乐系统按钮
        if (startEnt1Btn != null)
            startEnt1Btn.onClick.AddListener(() => StartEntertainment("world_scifi"));
        if (startEnt2Btn != null)
            startEnt2Btn.onClick.AddListener(() => StartEntertainment("world_apocalypse"));
        if (startEnt3Btn != null)
            startEnt3Btn.onClick.AddListener(() => StartEntertainment("world_magic"));
        if (cancelEntBtn != null)
            cancelEntBtn.onClick.AddListener(CancelEntertainment);
        
        // 连接安全卫士按钮
        if (subNoneBtn != null)
            subNoneBtn.onClick.AddListener(() => SubscribeSecurity(SecurityPlanType.None));
        if (subBasicBtn != null)
            subBasicBtn.onClick.AddListener(() => SubscribeSecurity(SecurityPlanType.Basic));
        if (subAdvancedBtn != null)
            subAdvancedBtn.onClick.AddListener(() => SubscribeSecurity(SecurityPlanType.Advanced));
        if (subUltimateBtn != null)
            subUltimateBtn.onClick.AddListener(() => SubscribeSecurity(SecurityPlanType.Ultimate));
        
        // 连接病毒入侵按钮
        if (triggerVirusBtn != null)
            triggerVirusBtn.onClick.AddListener(TriggerVirus);
        if (simSuccessBtn != null)
            simSuccessBtn.onClick.AddListener(() => SimulateGame(true));
        if (simFailBtn != null)
            simFailBtn.onClick.AddListener(() => SimulateGame(false));
        
        // 连接测试按钮
        if (addCoinsBtn != null)
            addCoinsBtn.onClick.AddListener(AddCoins);
        if (addLevelBtn != null)
            addLevelBtn.onClick.AddListener(AddLevel);
        if (showStatsBtn != null)
            showStatsBtn.onClick.AddListener(ShowStats);
        
        // 订阅所有事件
        SubscribeToEvents();
        
        // 初始更新
        lastUpdateTime = 0f;
        UpdateUI();
        
        Debug.Log("[Phase6TestUI] Phase 6 综合测试UI已初始化");
    }
    
    void Update()
    {
        // 定时更新UI
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateUI();
            lastUpdateTime = Time.time;
        }
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    // ==================== 事件订阅 ====================
    
    private void SubscribeToEvents()
    {
        // 娱乐系统事件
        if (EntertainmentManager.Instance != null)
        {
            EntertainmentManager.Instance.OnEntertainmentStarted += OnEntertainmentStarted;
            EntertainmentManager.Instance.OnEntertainmentCompleted += OnEntertainmentCompleted;
        }
        
        // 安全卫士事件
        if (SecurityManager.Instance != null)
        {
            SecurityManager.Instance.OnPlanSubscribed += OnSecuritySubscribed;
            SecurityManager.Instance.OnDefenseSuccess += OnDefenseSuccess;
        }
        
        // 病毒入侵事件
        if (VirusInvasionManager.Instance != null)
        {
            VirusInvasionManager.Instance.OnInvasionTriggered += OnVirusTriggered;
            VirusInvasionManager.Instance.OnSecurityBlocked += OnSecurityBlocked;
            VirusInvasionManager.Instance.OnGameSuccess += OnGameSuccess;
            VirusInvasionManager.Instance.OnGameFailed += OnGameFailed;
        }
    }
    
    private void UnsubscribeFromEvents()
    {
        if (EntertainmentManager.Instance != null)
        {
            EntertainmentManager.Instance.OnEntertainmentStarted -= OnEntertainmentStarted;
            EntertainmentManager.Instance.OnEntertainmentCompleted -= OnEntertainmentCompleted;
        }
        
        if (SecurityManager.Instance != null)
        {
            SecurityManager.Instance.OnPlanSubscribed -= OnSecuritySubscribed;
            SecurityManager.Instance.OnDefenseSuccess -= OnDefenseSuccess;
        }
        
        if (VirusInvasionManager.Instance != null)
        {
            VirusInvasionManager.Instance.OnInvasionTriggered -= OnVirusTriggered;
            VirusInvasionManager.Instance.OnSecurityBlocked -= OnSecurityBlocked;
            VirusInvasionManager.Instance.OnGameSuccess -= OnGameSuccess;
            VirusInvasionManager.Instance.OnGameFailed -= OnGameFailed;
        }
    }
    
    // ==================== UI更新 ====================
    
    private void UpdateUI()
    {
        if (statusText == null) return;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("========== Phase 6 系统测试 ==========\n");
        
        // 玩家信息
        DisplayPlayerInfo(sb);
        
        // 娱乐系统状态
        DisplayEntertainmentStatus(sb);
        
        // 安全卫士状态
        DisplaySecurityStatus(sb);
        
        // 病毒入侵状态
        DisplayVirusStatus(sb);
        
        sb.AppendLine("\n=====================================");
        
        statusText.text = sb.ToString();
    }
    
    private void DisplayPlayerInfo(StringBuilder sb)
    {
        if (ResourceManager.Instance != null)
        {
            PlayerResources res = ResourceManager.Instance.GetPlayerResources();
            sb.AppendLine($"【玩家信息】");
            sb.AppendLine($"等级: Lv.{res.level}");
            sb.AppendLine($"虚拟币: {res.virtualCoin}");
            sb.AppendLine($"心情值: {res.moodValue}");
            sb.AppendLine();
        }
    }
    
    private void DisplayEntertainmentStatus(StringBuilder sb)
    {
        sb.AppendLine($"【娱乐系统】");
        
        if (EntertainmentManager.Instance != null)
        {
            OngoingEntertainment current = EntertainmentManager.Instance.GetCurrentEntertainment();
            if (current != null)
            {
                sb.AppendLine($"进行中: {current.entertainmentName}");
                sb.AppendLine($"进度: {current.GetProgress() * 100f:F0}%");
                float remaining = current.GetRemainingTime();
                sb.AppendLine($"剩余: {Mathf.FloorToInt(remaining / 60f)}分{Mathf.FloorToInt(remaining % 60f)}秒");
            }
            else
            {
                sb.AppendLine("状态: 空闲");
            }
            
            sb.AppendLine($"总参加: {EntertainmentManager.Instance.totalEntertainmentCount}次");
        }
        sb.AppendLine();
    }
    
    private void DisplaySecurityStatus(StringBuilder sb)
    {
        sb.AppendLine($"【安全卫士】");
        
        if (SecurityManager.Instance != null)
        {
            SecurityPlanData plan = SecurityManager.Instance.GetCurrentPlan();
            if (plan != null)
            {
                sb.AppendLine($"当前方案: {plan.planName}");
                sb.AppendLine($"防御率: {plan.GetDefenseRateString()}");
                sb.AppendLine($"费用: {plan.costPer5Min}币/5分");
            }
            
            sb.AppendLine($"总支付: {SecurityManager.Instance.totalCostPaid}币");
            sb.AppendLine($"拦截成功: {SecurityManager.Instance.totalDefenseSuccess}次");
        }
        sb.AppendLine();
    }
    
    private void DisplayVirusStatus(StringBuilder sb)
    {
        sb.AppendLine($"【病毒入侵】");
        
        if (VirusInvasionManager.Instance != null)
        {
            VirusGameState state = VirusInvasionManager.Instance.GetCurrentState();
            sb.AppendLine($"状态: {GetStateText(state)}");
            
            if (state == VirusGameState.Idle)
            {
                float timeLeft = VirusInvasionManager.Instance.GetTimeUntilNextInvasion();
                sb.AppendLine($"下次入侵: {Mathf.FloorToInt(timeLeft / 60f)}分{Mathf.FloorToInt(timeLeft % 60f)}秒");
            }
            
            sb.AppendLine($"总入侵: {VirusInvasionManager.Instance.totalInvasions}次");
            sb.AppendLine($"成功防御: {VirusInvasionManager.Instance.successfulDefenses}次");
            sb.AppendLine($"失败: {VirusInvasionManager.Instance.failedDefenses}次");
        }
        sb.AppendLine();
    }
    
    private string GetStateText(VirusGameState state)
    {
        switch (state)
        {
            case VirusGameState.Idle: return "空闲";
            case VirusGameState.Triggered: return "已触发";
            case VirusGameState.Playing: return "游戏中";
            case VirusGameState.Completed: return "已完成";
            default: return "未知";
        }
    }
    
    // ==================== 娱乐系统操作 ====================
    
    private void StartEntertainment(string entertainmentId)
    {
        if (EntertainmentManager.Instance == null) return;
        
        bool success = EntertainmentManager.Instance.StartEntertainment(entertainmentId);
        if (!success)
        {
            Debug.LogWarning("[Phase6TestUI] 无法开始娱乐活动");
        }
        UpdateUI();
    }
    
    private void CancelEntertainment()
    {
        if (EntertainmentManager.Instance == null) return;
        
        EntertainmentManager.Instance.CancelCurrentEntertainment();
        UpdateUI();
    }
    
    // ==================== 安全卫士操作 ====================
    
    private void SubscribeSecurity(SecurityPlanType planType)
    {
        if (SecurityManager.Instance == null) return;
        
        SecurityManager.Instance.Subscribe(planType);
        UpdateUI();
    }
    
    // ==================== 病毒入侵操作 ====================
    
    private void TriggerVirus()
    {
        if (VirusInvasionManager.Instance == null) return;
        
        VirusInvasionManager.Instance.DebugTriggerInvasion();
        UpdateUI();
    }
    
    private void SimulateGame(bool success)
    {
        if (VirusInvasionManager.Instance == null) return;
        
        // 如果当前不在游戏中，先触发入侵
        if (VirusInvasionManager.Instance.GetCurrentState() != VirusGameState.Playing)
        {
            Debug.LogWarning("[Phase6TestUI] 当前没有进行中的游戏，请先触发病毒入侵");
            return;
        }
        
        VirusInvasionManager.Instance.DebugSimulateGameResult(success, 10);
        UpdateUI();
    }
    
    // ==================== 测试操作 ====================
    
    private void AddCoins()
    {
        if (ResourceManager.Instance == null) return;
        
        ResourceManager.Instance.EarnVirtualCoin(10000, "测试添加");
        Debug.Log("[Phase6TestUI] 添加10000虚拟币");
        UpdateUI();
    }
    
    private void AddLevel()
    {
        if (ResourceManager.Instance == null) return;
        
        PlayerResources res = ResourceManager.Instance.GetPlayerResources();
        res.level += 10;
        Debug.Log($"[Phase6TestUI] 提升10级，当前等级：{res.level}");
        UpdateUI();
    }
    
    private void ShowStats()
    {
        Debug.Log("========== Phase 6 系统统计 ==========");
        
        if (EntertainmentManager.Instance != null)
        {
            Debug.Log("\n【娱乐系统】");
            Debug.Log(EntertainmentManager.Instance.GetStatsSummary());
        }
        
        if (SecurityManager.Instance != null)
        {
            Debug.Log("\n【安全卫士】");
            Debug.Log(SecurityManager.Instance.GetStatsSummary());
        }
        
        if (VirusInvasionManager.Instance != null)
        {
            Debug.Log("\n【病毒入侵】");
            Debug.Log(VirusInvasionManager.Instance.GetStatsSummary());
        }
        
        Debug.Log("\n====================================");
    }
    
    // ==================== 事件回调 ====================
    
    private void OnEntertainmentStarted(string name, float duration)
    {
        Debug.Log($"<color=cyan>[事件] 娱乐开始: {name}</color>");
    }
    
    private void OnEntertainmentCompleted(string name, int mood)
    {
        Debug.Log($"<color=cyan>[事件] 娱乐完成: {name}, +{mood}心情</color>");
    }
    
    private void OnSecuritySubscribed(string name, int cost)
    {
        Debug.Log($"<color=yellow>[事件] 订阅安全卫士: {name}, {cost}币/5分</color>");
    }
    
    private void OnDefenseSuccess(string name)
    {
        Debug.Log($"<color=green>[事件] {name}成功拦截病毒！</color>");
    }
    
    private void OnVirusTriggered()
    {
        Debug.Log($"<color=red>[事件] ⚠️ 病毒入侵警报！</color>");
    }
    
    private void OnSecurityBlocked()
    {
        Debug.Log($"<color=green>[事件] 安全卫士拦截病毒成功！</color>");
    }
    
    private void OnGameSuccess(int kills, int coins, int mood)
    {
        Debug.Log($"<color=green>[事件] 防御成功！击杀{kills}，+{coins}币，+{mood}心情</color>");
    }
    
    private void OnGameFailed(int loss)
    {
        Debug.Log($"<color=red>[事件] 防御失败！损失{loss}币</color>");
    }
}
