using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 初始选择UI管理器
/// 玩家首次进入游戏时，选择角色的初始身份类型
/// 
/// Unity使用说明：
/// 1. 创建Canvas对象，命名为"InitialSelectionCanvas"
/// 2. 在Canvas下创建Panel，命名为"SelectionPanel"
/// 3. 添加标题文本（Title）、描述文本（Description）
/// 4. 创建两个选项按钮及其说明面板
/// 5. 将此脚本挂载到Canvas对象上
/// 6. 在Inspector中连接所有UI引用
/// </summary>
public class InitialSelectionUI : MonoBehaviour
{
    #region UI引用
    [Header("主面板")]
    [Tooltip("初始选择主面板")]
    public GameObject selectionPanel;
    
    [Header("标题和描述")]
    [Tooltip("标题文本")]
    public TextMeshProUGUI titleText;
    
    [Tooltip("描述文本")]
    public TextMeshProUGUI descriptionText;
    
    [Header("意识连接者选项")]
    [Tooltip("意识连接者按钮")]
    public Button consciousnessLinkerButton;
    
    [Tooltip("意识连接者名称文本")]
    public TextMeshProUGUI consciousnessLinkerNameText;
    
    [Tooltip("意识连接者描述文本")]
    public TextMeshProUGUI consciousnessLinkerDescText;
    
    [Tooltip("意识连接者详细信息面板")]
    public GameObject consciousnessLinkerInfoPanel;
    
    [Tooltip("意识连接者详细信息文本")]
    public TextMeshProUGUI consciousnessLinkerInfoText;
    
    [Header("完全虚拟人选项")]
    [Tooltip("完全虚拟人按钮")]
    public Button fullVirtualButton;
    
    [Tooltip("完全虚拟人名称文本")]
    public TextMeshProUGUI fullVirtualNameText;
    
    [Tooltip("完全虚拟人描述文本")]
    public TextMeshProUGUI fullVirtualDescText;
    
    [Tooltip("完全虚拟人详细信息面板")]
    public GameObject fullVirtualInfoPanel;
    
    [Tooltip("完全虚拟人详细信息文本")]
    public TextMeshProUGUI fullVirtualInfoText;
    
    [Header("确认按钮")]
    [Tooltip("确认选择按钮")]
    public Button confirmButton;
    
    [Tooltip("确认按钮文本")]
    public TextMeshProUGUI confirmButtonText;
    #endregion
    
    #region 状态变量
    private IdentityType selectedIdentity = IdentityType.ConsciousnessLinker;
    private bool hasSelected = false;
    #endregion
    
    #region 配置常量
    [Header("视觉配置")]
    [Tooltip("选中按钮的高亮颜色")]
    public Color highlightColor = new Color(0.3f, 0.8f, 1f, 1f);
    
    [Tooltip("未选中按钮的默认颜色")]
    public Color normalColor = Color.white;
    #endregion
    
    #region 事件定义
    /// <summary>
    /// 选择完成事件
    /// 参数：选择的身份类型
    /// </summary>
    public event Action<IdentityType> OnSelectionCompleted;
    #endregion
    
    #region Unity生命周期方法
    private void Start()
    {
        InitializeUI();
        RegisterButtonListeners();
    }
    
    private void OnDestroy()
    {
        UnregisterButtonListeners();
    }
    #endregion
    
    #region 初始化方法
    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        Debug.Log("=== 初始化初始选择UI ===");
        
        // 设置标题和描述
        if (titleText != null)
        {
            titleText.text = "欢迎来到电子公民";
        }
        
        if (descriptionText != null)
        {
            descriptionText.text = "请选择你的存在形式，这将决定你在虚拟世界中的起始配置";
        }
        
        // 设置意识连接者信息
        if (consciousnessLinkerNameText != null)
        {
            consciousnessLinkerNameText.text = "脑机连接者";
        }
        
        if (consciousnessLinkerDescText != null)
        {
            consciousnessLinkerDescText.text = "现实意识与虚拟世界的连接者";
        }
        
        if (consciousnessLinkerInfoText != null)
        {
            consciousnessLinkerInfoText.text = 
                "<b>优势：</b>\n" +
                "• 较低的初始资源占用\n" +
                "• 每5分钟产生数据较少\n\n" +
                "<b>劣势：</b>\n" +
                "• 需要每5分钟支付连接费（5-10虚拟币）\n\n" +
                "<b>初始配置：</b>\n" +
                "内存：16GB（已用2GB）\n" +
                "CPU：8核（已用1核）\n" +
                "网速：1000Mbps（已用50Mbps）\n" +
                "算力：100（已用10）\n" +
                "存储：500GB（已用20GB）\n\n" +
                "每5分钟产生数据：0.5GB";
        }
        
        // 设置完全虚拟人信息
        if (fullVirtualNameText != null)
        {
            fullVirtualNameText.text = "纯虚拟人";
        }
        
        if (fullVirtualDescText != null)
        {
            fullVirtualDescText.text = "完全数字化的虚拟生命体";
        }
        
        if (fullVirtualInfoText != null)
        {
            fullVirtualInfoText.text = 
                "<b>优势：</b>\n" +
                "• 无需支付连接费\n" +
                "• 完全自由的虚拟生活\n\n" +
                "<b>劣势：</b>\n" +
                "• 较高的初始资源占用\n" +
                "• 每5分钟产生数据较多\n\n" +
                "<b>初始配置：</b>\n" +
                "内存：16GB（已用4GB）\n" +
                "CPU：8核（已用2核）\n" +
                "网速：1000Mbps（已用100Mbps）\n" +
                "算力：100（已用20）\n" +
                "存储：500GB（已用50GB）\n\n" +
                "每5分钟产生数据：1.2GB";
        }
        
        // 初始时隐藏详细信息面板
        if (consciousnessLinkerInfoPanel != null)
        {
            consciousnessLinkerInfoPanel.SetActive(false);
        }
        
        if (fullVirtualInfoPanel != null)
        {
            fullVirtualInfoPanel.SetActive(false);
        }
        
        // 设置确认按钮
        if (confirmButtonText != null)
        {
            confirmButtonText.text = "确认选择";
        }
        
        // 初始禁用确认按钮
        if (confirmButton != null)
        {
            confirmButton.interactable = false;
        }
        
        // 确保面板可见
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);
        }
        
        Debug.Log("✓ 初始选择UI初始化完成");
    }
    
    /// <summary>
    /// 注册按钮监听器
    /// </summary>
    private void RegisterButtonListeners()
    {
        if (consciousnessLinkerButton != null)
        {
            consciousnessLinkerButton.onClick.AddListener(OnConsciousnessLinkerSelected);
        }
        
        if (fullVirtualButton != null)
        {
            fullVirtualButton.onClick.AddListener(OnFullVirtualSelected);
        }
        
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
    }
    
    /// <summary>
    /// 取消注册按钮监听器
    /// </summary>
    private void UnregisterButtonListeners()
    {
        if (consciousnessLinkerButton != null)
        {
            consciousnessLinkerButton.onClick.RemoveListener(OnConsciousnessLinkerSelected);
        }
        
        if (fullVirtualButton != null)
        {
            fullVirtualButton.onClick.RemoveListener(OnFullVirtualSelected);
        }
        
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
        }
    }
    #endregion
    
    #region 按钮回调方法
    /// <summary>
    /// 选择意识连接者
    /// </summary>
    private void OnConsciousnessLinkerSelected()
    {
        Debug.Log("<color=cyan>选择：脑机连接者</color>");
        
        selectedIdentity = IdentityType.ConsciousnessLinker;
        hasSelected = true;
        
        // 显示意识连接者详细信息
        if (consciousnessLinkerInfoPanel != null)
        {
            consciousnessLinkerInfoPanel.SetActive(true);
        }
        
        // 隐藏完全虚拟人详细信息
        if (fullVirtualInfoPanel != null)
        {
            fullVirtualInfoPanel.SetActive(false);
        }
        
        // 启用确认按钮
        if (confirmButton != null)
        {
            confirmButton.interactable = true;
        }
        
        // 高亮选中的按钮
        UpdateButtonVisuals();
    }
    
    /// <summary>
    /// 选择完全虚拟人
    /// </summary>
    private void OnFullVirtualSelected()
    {
        Debug.Log("<color=cyan>选择：纯虚拟人</color>");
        
        selectedIdentity = IdentityType.FullVirtual;
        hasSelected = true;
        
        // 显示完全虚拟人详细信息
        if (fullVirtualInfoPanel != null)
        {
            fullVirtualInfoPanel.SetActive(true);
        }
        
        // 隐藏意识连接者详细信息
        if (consciousnessLinkerInfoPanel != null)
        {
            consciousnessLinkerInfoPanel.SetActive(false);
        }
        
        // 启用确认按钮
        if (confirmButton != null)
        {
            confirmButton.interactable = true;
        }
        
        // 高亮选中的按钮
        UpdateButtonVisuals();
    }
    
    /// <summary>
    /// 确认按钮点击
    /// </summary>
    private void OnConfirmButtonClicked()
    {
        if (!hasSelected)
        {
            Debug.LogWarning("请先选择一个身份类型！");
            return;
        }
        
        Debug.Log($"<color=green>确认选择：{selectedIdentity}</color>");
        
        // 触发选择完成事件
        OnSelectionCompleted?.Invoke(selectedIdentity);
        
        // 隐藏面板
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 更新按钮视觉效果
    /// </summary>
    private void UpdateButtonVisuals()
    {
        // 可以在这里添加选中状态的视觉反馈
        // 例如：改变按钮颜色、添加边框等
        
        if (consciousnessLinkerButton != null && fullVirtualButton != null)
        {
            ColorBlock consciousnessColors = consciousnessLinkerButton.colors;
            ColorBlock fullVirtualColors = fullVirtualButton.colors;
            
            if (selectedIdentity == IdentityType.ConsciousnessLinker)
            {
                // 高亮意识连接者按钮
                consciousnessColors.normalColor = highlightColor;
                fullVirtualColors.normalColor = normalColor;
            }
            else
            {
                // 高亮完全虚拟人按钮
                fullVirtualColors.normalColor = highlightColor;
                consciousnessColors.normalColor = normalColor;
            }
            
            consciousnessLinkerButton.colors = consciousnessColors;
            fullVirtualButton.colors = fullVirtualColors;
        }
    }
    
    /// <summary>
    /// 重置按钮颜色
    /// </summary>
    private void ResetButtonColors()
    {
        if (consciousnessLinkerButton != null)
        {
            ColorBlock colors = consciousnessLinkerButton.colors;
            colors.normalColor = normalColor;
            consciousnessLinkerButton.colors = colors;
        }
        
        if (fullVirtualButton != null)
        {
            ColorBlock colors = fullVirtualButton.colors;
            colors.normalColor = normalColor;
            fullVirtualButton.colors = colors;
        }
    }
    #endregion
    
    #region 公共方法
    /// <summary>
    /// 显示初始选择界面
    /// </summary>
    public void Show()
    {
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);
            hasSelected = false;
            selectedIdentity = IdentityType.ConsciousnessLinker; // 重置为默认值
            
            if (confirmButton != null)
            {
                confirmButton.interactable = false;
            }
            
            // 隐藏详细信息面板
            if (consciousnessLinkerInfoPanel != null)
            {
                consciousnessLinkerInfoPanel.SetActive(false);
            }
            
            if (fullVirtualInfoPanel != null)
            {
                fullVirtualInfoPanel.SetActive(false);
            }
            
            // 重置按钮颜色
            ResetButtonColors();
        }
    }
    
    /// <summary>
    /// 隐藏初始选择界面
    /// </summary>
    public void Hide()
    {
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 获取当前选择的身份类型
    /// </summary>
    public IdentityType GetSelectedIdentity()
    {
        return selectedIdentity;
    }
    
    /// <summary>
    /// 检查是否已选择
    /// </summary>
    public bool HasMadeSelection()
    {
        return hasSelected;
    }
    #endregion
}
