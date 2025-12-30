using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 资源显示UI
/// 负责在屏幕上显示玩家的资源信息
/// </summary>
public class ResourceDisplayUI : MonoBehaviour
{
    [Header("UI元素引用")]
    [Tooltip("资源信息文本")]
    public TMP_Text resourceText;

    [Tooltip("定时器文本")]
    public TMP_Text timerText;

    [Tooltip("测试按钮")]
    public Button testTickButton;

    private void Start()
    {
        // 注册事件监听
        RegisterEvents();

        // 注册按钮点击事件
        if (testTickButton != null)
        {
            testTickButton.onClick.AddListener(OnTestTickButtonClicked);
        }

        // 初始显示
        UpdateResourceDisplay();
    }

    private void Update()
    {
        // 每帧更新定时器显示
        UpdateTimerDisplay();
    }

    private void OnDestroy()
    {
        // 取消事件监听
        UnregisterEvents();

        if (testTickButton != null)
        {
            testTickButton.onClick.RemoveListener(OnTestTickButtonClicked);
        }
    }

    /// <summary>
    /// 注册事件监听
    /// </summary>
    private void RegisterEvents()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourcesChanged += OnResourcesChanged;
        }
    }

    /// <summary>
    /// 取消事件监听
    /// </summary>
    private void UnregisterEvents()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourcesChanged -= OnResourcesChanged;
        }
    }

    /// <summary>
    /// 资源变化回调
    /// </summary>
    private void OnResourcesChanged(PlayerResources resources)
    {
        UpdateResourceDisplay();
    }

    /// <summary>
    /// 更新资源显示
    /// </summary>
    private void UpdateResourceDisplay()
    {
        if (resourceText == null || ResourceManager.Instance == null)
        {
            return;
        }

        PlayerResources res = ResourceManager.Instance.GetResourcesCopy();

        if (res == null)
        {
            resourceText.text = "资源数据加载中...";
            return;
        }

        // 格式化显示资源信息
        string displayText = $"<b><size=24>玩家资源</size></b>\n\n";
        displayText += $"<b>虚拟币:</b> <color=yellow>{res.virtualCoin}</color>\n";
        displayText += $"<b>等级:</b> Lv.{res.level}\n";
        displayText += $"<b>心情值:</b> <color={(res.moodValue >= 0 ? "green" : "red")}>{res.moodValue}</color>\n\n";

        displayText += $"<b>内存:</b> {res.memoryUsed:F1}/{res.memoryTotal:F1} GB ({res.MemoryUsagePercent:F0}%)\n";
        displayText += $"<b>CPU:</b> {res.cpuUsed:F1}/{res.cpuTotal:F1} 核 ({res.CpuUsagePercent:F0}%)\n";
        displayText += $"<b>网速:</b> {res.bandwidthUsed:F0}/{res.bandwidthTotal:F0} Mbps ({res.BandwidthUsagePercent:F0}%)\n";
        displayText += $"<b>算力:</b> {res.computingUsed:F1}/{res.computingTotal:F1} ({res.ComputingUsagePercent:F0}%)\n";
        displayText += $"<b>存储:</b> {res.storageUsed:F1}/{res.storageTotal:F1} GB ({res.StorageUsagePercent:F0}%)\n\n";

        // 显示效率信息
        float efficiency = ResourceManager.Instance.CalculateIncomeEfficiency();
        displayText += $"<b>当前效率:</b> <color=cyan>{efficiency:F1}%</color>\n";

        resourceText.text = displayText;
    }

    /// <summary>
    /// 更新定时器显示
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText == null || GameTimerManager.Instance == null)
        {
            return;
        }

        string timeStr = GameTimerManager.Instance.GetRemainingTimeFormatted();
        int totalTicks = GameTimerManager.Instance.GetTotalTicks();

        timerText.text = $"下次结算: <color=yellow>{timeStr}</color> | 周期: {totalTicks}";
    }

    /// <summary>
    /// 测试按钮点击回调
    /// </summary>
    private void OnTestTickButtonClicked()
    {
        Debug.Log("<color=cyan>点击测试按钮，立即触发一次结算</color>");

        if (GameTimerManager.Instance != null)
        {
            GameTimerManager.Instance.TriggerGameTickNow();
        }
    }
}
