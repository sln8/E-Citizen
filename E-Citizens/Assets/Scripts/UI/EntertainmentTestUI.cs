using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

/*
 * 娱乐系统测试UI
 * Entertainment System Test UI
 * 
 * 功能说明：
 * 1. 显示所有娱乐活动列表
 * 2. 显示当前正在进行的娱乐活动
 * 3. 提供开始/取消娱乐的按钮
 * 4. 实时显示进度和剩余时间
 * 5. 显示统计数据
 * 
 * Unity操作步骤：
 * 1. 创建Canvas（如果没有）：Hierarchy右键 → UI → Canvas
 * 2. 在Canvas下创建Panel：右键 → UI → Panel，命名为"EntertainmentTestPanel"
 * 3. 在Panel中创建以下UI元素：
 *    - TextMeshProUGUI：命名为"StatusText"（显示状态信息）
 *    - Button：命名为"StartEntertainment1Btn"（星际战争）
 *    - Button：命名为"StartEntertainment2Btn"（末日求生）
 *    - Button：命名为"StartEntertainment3Btn"（魔法学院）
 *    - Button：命名为"StartEntertainment4Btn"（三体世界）
 *    - Button：命名为"CancelBtn"（取消娱乐）
 *    - Button：命名为"AddCoinsBtn"（测试：加虚拟币）
 *    - Button：命名为"ShowStatsBtn"（显示统计）
 * 4. 创建空物体"EntertainmentTestUI"，添加此脚本
 * 5. 在Inspector中连接所有UI引用
 * 
 * 作者：GitHub Copilot
 * 日期：2025-12-31
 */

/// <summary>
/// 娱乐系统测试UI
/// 用于测试娱乐系统的所有功能
/// </summary>
public class EntertainmentTestUI : MonoBehaviour
{
    [Header("UI引用")]
    [Tooltip("状态文本")]
    public TextMeshProUGUI statusText;
    
    [Tooltip("开始娱乐按钮")]
    public Button startEntertainment1Btn;
    public Button startEntertainment2Btn;
    public Button startEntertainment3Btn;
    public Button startEntertainment4Btn;
    
    [Tooltip("取消娱乐按钮")]
    public Button cancelBtn;
    
    [Tooltip("测试按钮")]
    public Button addCoinsBtn;
    public Button showStatsBtn;
    
    [Header("更新设置")]
    [Tooltip("UI更新间隔（秒）")]
    public float updateInterval = 0.5f;
    
    // 私有变量
    private float lastUpdateTime;
    
    void Start()
    {
        // 连接按钮事件
        if (startEntertainment1Btn != null)
            startEntertainment1Btn.onClick.AddListener(() => OnStartEntertainment("world_scifi"));
        
        if (startEntertainment2Btn != null)
            startEntertainment2Btn.onClick.AddListener(() => OnStartEntertainment("world_apocalypse"));
        
        if (startEntertainment3Btn != null)
            startEntertainment3Btn.onClick.AddListener(() => OnStartEntertainment("world_magic"));
        
        if (startEntertainment4Btn != null)
            startEntertainment4Btn.onClick.AddListener(() => OnStartEntertainment("world_novel"));
        
        if (cancelBtn != null)
            cancelBtn.onClick.AddListener(OnCancel);
        
        if (addCoinsBtn != null)
            addCoinsBtn.onClick.AddListener(OnAddCoins);
        
        if (showStatsBtn != null)
            showStatsBtn.onClick.AddListener(OnShowStats);
        
        // 订阅娱乐系统事件
        if (EntertainmentManager.Instance != null)
        {
            EntertainmentManager.Instance.OnEntertainmentStarted += OnEntertainmentStartedEvent;
            EntertainmentManager.Instance.OnEntertainmentCompleted += OnEntertainmentCompletedEvent;
            EntertainmentManager.Instance.OnEntertainmentCancelled += OnEntertainmentCancelledEvent;
        }
        
        // 初始更新
        lastUpdateTime = 0f;
        UpdateUI();
        
        Debug.Log("[EntertainmentTestUI] 娱乐系统测试UI已初始化");
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
        // 取消订阅事件
        if (EntertainmentManager.Instance != null)
        {
            EntertainmentManager.Instance.OnEntertainmentStarted -= OnEntertainmentStartedEvent;
            EntertainmentManager.Instance.OnEntertainmentCompleted -= OnEntertainmentCompletedEvent;
            EntertainmentManager.Instance.OnEntertainmentCancelled -= OnEntertainmentCancelledEvent;
        }
    }
    
    // ==================== UI更新 ====================
    
    /// <summary>
    /// 更新UI显示
    /// </summary>
    private void UpdateUI()
    {
        if (statusText == null) return;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("========== 娱乐系统测试 ==========\n");
        
        // 1. 显示玩家资源
        if (ResourceManager.Instance != null)
        {
            PlayerResources resources = ResourceManager.Instance.GetPlayerResources();
            sb.AppendLine($"【玩家信息】");
            sb.AppendLine($"等级: Lv.{resources.level}");
            sb.AppendLine($"虚拟币: {resources.virtualCoin}");
            sb.AppendLine($"心情值: {resources.moodValue}");
            sb.AppendLine();
        }
        
        // 2. 显示汽车加成
        if (LifeSystemManager.Instance != null)
        {
            VehicleData vehicle = LifeSystemManager.Instance.GetCurrentVehicle();
            if (vehicle != null)
            {
                sb.AppendLine($"【当前汽车】");
                sb.AppendLine($"{vehicle.vehicleName}");
                sb.AppendLine($"速度加成: {vehicle.speedBonus}x");
                sb.AppendLine();
            }
        }
        
        // 3. 显示正在进行的娱乐
        if (EntertainmentManager.Instance != null)
        {
            OngoingEntertainment current = EntertainmentManager.Instance.GetCurrentEntertainment();
            if (current != null)
            {
                sb.AppendLine($"【正在进行】");
                sb.AppendLine($"{current.entertainmentName}");
                sb.AppendLine($"进度: {current.GetProgress() * 100f:F1}%");
                
                float remaining = current.GetRemainingTime();
                int minutes = Mathf.FloorToInt(remaining / 60f);
                int seconds = Mathf.FloorToInt(remaining % 60f);
                sb.AppendLine($"剩余: {minutes}分{seconds}秒");
                sb.AppendLine($"完成奖励: +{current.moodReward}心情");
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine("【当前状态】");
                sb.AppendLine("无正在进行的娱乐活动");
                sb.AppendLine();
            }
        }
        
        // 4. 显示可用娱乐活动
        if (EntertainmentManager.Instance != null && ResourceManager.Instance != null)
        {
            PlayerResources resources = ResourceManager.Instance.GetPlayerResources();
            var allActivities = EntertainmentManager.Instance.GetAllEntertainment();
            
            sb.AppendLine($"【娱乐活动列表】");
            foreach (var activity in allActivities)
            {
                bool hasPurchased = EntertainmentManager.Instance.HasPurchased(activity.entertainmentId);
                bool canParticipate = activity.CanParticipate(resources.level, resources.virtualCoin, hasPurchased);
                
                string status = canParticipate ? "✓" : "✗";
                sb.Append($"{status} {activity.entertainmentName}");
                
                if (!canParticipate)
                {
                    string reason = activity.GetLockReason(resources.level, resources.virtualCoin, hasPurchased);
                    sb.Append($" ({reason})");
                }
                else
                {
                    // 获取汽车速度加成
                    float speedBonus = 1.0f;
                    if (LifeSystemManager.Instance != null)
                    {
                        VehicleData vehicle = LifeSystemManager.Instance.GetCurrentVehicle();
                        if (vehicle != null)
                        {
                            speedBonus = vehicle.speedBonus;
                        }
                    }
                    
                    float actualTime = activity.CalculateActualDuration(speedBonus);
                    sb.Append($" - {activity.cost}币 / {actualTime / 60f:F1}分 / +{activity.moodReward}心情");
                }
                
                sb.AppendLine();
            }
        }
        
        sb.AppendLine("\n==============================");
        
        statusText.text = sb.ToString();
        
        // 更新按钮状态
        UpdateButtonStates();
    }
    
    /// <summary>
    /// 更新按钮的可用状态
    /// </summary>
    private void UpdateButtonStates()
    {
        bool hasOngoing = EntertainmentManager.Instance != null && 
                         EntertainmentManager.Instance.HasOngoingEntertainment();
        
        // 开始按钮：有正在进行的娱乐时禁用
        if (startEntertainment1Btn != null) startEntertainment1Btn.interactable = !hasOngoing;
        if (startEntertainment2Btn != null) startEntertainment2Btn.interactable = !hasOngoing;
        if (startEntertainment3Btn != null) startEntertainment3Btn.interactable = !hasOngoing;
        if (startEntertainment4Btn != null) startEntertainment4Btn.interactable = !hasOngoing;
        
        // 取消按钮：有正在进行的娱乐时启用
        if (cancelBtn != null) cancelBtn.interactable = hasOngoing;
    }
    
    // ==================== 按钮事件 ====================
    
    /// <summary>
    /// 开始娱乐活动
    /// </summary>
    private void OnStartEntertainment(string entertainmentId)
    {
        if (EntertainmentManager.Instance == null)
        {
            Debug.LogError("[EntertainmentTestUI] EntertainmentManager未初始化！");
            return;
        }
        
        EntertainmentData data = EntertainmentManager.Instance.GetEntertainment(entertainmentId);
        if (data == null)
        {
            Debug.LogError($"[EntertainmentTestUI] 娱乐活动不存在: {entertainmentId}");
            return;
        }
        
        Debug.Log($"[EntertainmentTestUI] 尝试开始娱乐: {data.entertainmentName}");
        bool success = EntertainmentManager.Instance.StartEntertainment(entertainmentId);
        
        if (!success)
        {
            Debug.LogWarning($"[EntertainmentTestUI] 无法开始娱乐活动");
        }
        
        UpdateUI();
    }
    
    /// <summary>
    /// 取消当前娱乐活动
    /// </summary>
    private void OnCancel()
    {
        if (EntertainmentManager.Instance == null)
        {
            Debug.LogError("[EntertainmentTestUI] EntertainmentManager未初始化！");
            return;
        }
        
        Debug.Log("[EntertainmentTestUI] 取消当前娱乐活动");
        EntertainmentManager.Instance.CancelCurrentEntertainment();
        UpdateUI();
    }
    
    /// <summary>
    /// 测试：添加虚拟币
    /// </summary>
    private void OnAddCoins()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[EntertainmentTestUI] ResourceManager未初始化！");
            return;
        }
        
        int amount = 10000;
        ResourceManager.Instance.EarnVirtualCoin(amount, "测试添加");
        Debug.Log($"[EntertainmentTestUI] 添加{amount}虚拟币");
        UpdateUI();
    }
    
    /// <summary>
    /// 显示统计信息
    /// </summary>
    private void OnShowStats()
    {
        if (EntertainmentManager.Instance == null)
        {
            Debug.LogError("[EntertainmentTestUI] EntertainmentManager未初始化！");
            return;
        }
        
        string stats = EntertainmentManager.Instance.GetStatsSummary();
        Debug.Log($"========== 娱乐系统统计 ==========\n{stats}\n================================");
    }
    
    // ==================== 事件监听 ====================
    
    /// <summary>
    /// 娱乐开始事件
    /// </summary>
    private void OnEntertainmentStartedEvent(string name, float duration)
    {
        Debug.Log($"<color=green>[事件] 娱乐开始: {name}，预计{duration / 60f:F1}分钟</color>");
        UpdateUI();
    }
    
    /// <summary>
    /// 娱乐完成事件
    /// </summary>
    private void OnEntertainmentCompletedEvent(string name, int moodReward)
    {
        Debug.Log($"<color=green>[事件] 娱乐完成: {name}，获得{moodReward}心情值</color>");
        UpdateUI();
    }
    
    /// <summary>
    /// 娱乐取消事件
    /// </summary>
    private void OnEntertainmentCancelledEvent(string name)
    {
        Debug.Log($"<color=yellow>[事件] 娱乐取消: {name}</color>");
        UpdateUI();
    }
}
