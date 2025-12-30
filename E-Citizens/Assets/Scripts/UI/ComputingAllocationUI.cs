using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 算力分配UI
/// 让玩家管理技能的算力分配，提升技能掌握度
/// 
/// Unity操作步骤（零基础）：
/// 1. 在Canvas下创建一个Panel，命名为"ComputingAllocationPanel"
/// 2. 在Panel下创建:
///    - 标题Text: "算力分配"
///    - 总算力显示Text
///    - ScrollView: 用于显示已拥有的技能列表
///    - 重置所有算力按钮
///    - 关闭按钮
/// 3. 创建技能算力项预制体（ComputingItemPrefab）包含:
///    - 技能名称Text
///    - 当前掌握度Text
///    - 算力滑动条Slider
///    - 分配算力数值Text
///    - 预览掌握度Text
/// 4. 将此脚本添加到ComputingAllocationPanel
/// 5. 连接所有UI引用
/// </summary>
public class ComputingAllocationUI : MonoBehaviour
{
    [Header("UI面板")]
    [Tooltip("算力分配面板根对象")]
    public GameObject allocationPanel;
    
    [Header("算力信息")]
    [Tooltip("总算力显示")]
    public TMP_Text totalComputingText;
    
    [Tooltip("已分配算力显示")]
    public TMP_Text allocatedComputingText;
    
    [Tooltip("可用算力显示")]
    public TMP_Text availableComputingText;
    
    [Header("技能列表")]
    [Tooltip("技能列表的ScrollView Content")]
    public Transform skillListContent;
    
    [Tooltip("技能算力项预制体")]
    public GameObject computingItemPrefab;
    
    [Header("按钮")]
    [Tooltip("重置所有算力按钮")]
    public Button resetAllButton;
    
    [Tooltip("应用更改按钮")]
    public Button applyButton;
    
    [Tooltip("关闭面板按钮")]
    public Button closeButton;
    
    // 所有技能算力项游戏对象（用于销毁）
    private List<GameObject> computingItems = new List<GameObject>();
    
    // 临时存储算力分配（用于预览）
    private Dictionary<string, float> tempAllocations = new Dictionary<string, float>();
    
    #region Unity生命周期
    
    private void Start()
    {
        // 注册按钮事件
        if (resetAllButton != null)
            resetAllButton.onClick.AddListener(ResetAllComput ing);
        
        if (applyButton != null)
            applyButton.onClick.AddListener(ApplyAllocations);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
        
        // 注册SkillManager事件
        RegisterEvents();
        
        // 初始时隐藏面板
        if (allocationPanel != null)
            allocationPanel.SetActive(false);
    }
    
    private void OnDestroy()
    {
        // 取消按钮事件
        if (resetAllButton != null)
            resetAllButton.onClick.RemoveListener(ResetAllComputing);
        
        if (applyButton != null)
            applyButton.onClick.RemoveListener(ApplyAllocations);
        
        if (closeButton != null)
            closeButton.onClick.RemoveListener(ClosePanel);
        
        // 取消SkillManager事件
        UnregisterEvents();
    }
    
    #endregion
    
    #region 事件管理
    
    /// <summary>
    /// 注册事件监听
    /// </summary>
    private void RegisterEvents()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnComputingAllocationUpdated += OnComputingAllocationUpdated;
            SkillManager.Instance.OnMasteryUpdated += OnMasteryUpdated;
        }
    }
    
    /// <summary>
    /// 取消事件监听
    /// </summary>
    private void UnregisterEvents()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnComputingAllocationUpdated -= OnComputingAllocationUpdated;
            SkillManager.Instance.OnMasteryUpdated -= OnMasteryUpdated;
        }
    }
    
    #endregion
    
    #region 公共方法
    
    /// <summary>
    /// 打开算力分配面板
    /// 显示面板并刷新技能列表
    /// </summary>
    public void OpenPanel()
    {
        if (allocationPanel != null)
        {
            allocationPanel.SetActive(true);
            InitializeTempAllocations();
            RefreshSkillList();
            UpdateComputingInfo();
            Debug.Log("<color=cyan>打开算力分配面板</color>");
        }
    }
    
    /// <summary>
    /// 关闭算力分配面板
    /// </summary>
    public void ClosePanel()
    {
        if (allocationPanel != null)
        {
            allocationPanel.SetActive(false);
            Debug.Log("<color=cyan>关闭算力分配面板</color>");
        }
    }
    
    #endregion
    
    #region 私有方法
    
    /// <summary>
    /// 初始化临时分配数据
    /// 从当前SkillManager获取现有分配
    /// </summary>
    private void InitializeTempAllocations()
    {
        tempAllocations.Clear();
        
        List<PlayerSkillInstance> playerSkills = SkillManager.Instance.playerSkills;
        foreach (PlayerSkillInstance skill in playerSkills)
        {
            tempAllocations[skill.skillId] = skill.allocatedComputing;
        }
    }
    
    /// <summary>
    /// 更新算力信息显示
    /// </summary>
    private void UpdateComputingInfo()
    {
        // 获取总算力
        float totalComputing = ResourceManager.Instance.GetComputingTotal();
        
        // 计算已分配算力（使用临时分配）
        float allocated = 0f;
        foreach (float allocation in tempAllocations.Values)
        {
            allocated += allocation;
        }
        
        // 计算可用算力
        float available = totalComputing - allocated;
        
        // 更新显示
        if (totalComputingText != null)
        {
            totalComputingText.text = $"总算力：<color=cyan>{totalComputing:F0}</color>";
        }
        
        if (allocatedComputingText != null)
        {
            allocatedComputingText.text = $"已分配：<color=yellow>{allocated:F0}</color>";
        }
        
        if (availableComputingText != null)
        {
            if (available >= 0)
            {
                availableComputingText.text = $"可用：<color=green>{available:F0}</color>";
            }
            else
            {
                availableComputingText.text = $"可用：<color=red>{available:F0} (超额!)</color>";
            }
        }
    }
    
    /// <summary>
    /// 清除所有技能项
    /// </summary>
    private void ClearSkillItems()
    {
        foreach (GameObject item in computingItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        computingItems.Clear();
    }
    
    /// <summary>
    /// 刷新技能列表
    /// 显示所有已拥有的技能
    /// </summary>
    private void RefreshSkillList()
    {
        // 清除旧的技能项
        ClearSkillItems();
        
        // 获取玩家已拥有的技能
        List<PlayerSkillInstance> playerSkills = SkillManager.Instance.playerSkills;
        
        if (playerSkills == null || playerSkills.Count == 0)
        {
            Debug.LogWarning("没有已拥有的技能");
            return;
        }
        
        // 创建技能算力项
        foreach (PlayerSkillInstance skillInstance in playerSkills)
        {
            SkillData skillData = SkillManager.Instance.GetSkillById(skillInstance.skillId);
            if (skillData != null)
            {
                CreateComputingItem(skillData, skillInstance);
            }
        }
        
        Debug.Log($"<color=green>刷新算力分配列表，共{playerSkills.Count}个技能</color>");
    }
    
    /// <summary>
    /// 创建一个技能算力项
    /// </summary>
    private void CreateComputingItem(SkillData skillData, PlayerSkillInstance skillInstance)
    {
        if (computingItemPrefab == null || skillListContent == null)
        {
            Debug.LogError("技能算力项预制体或列表Content未设置！");
            return;
        }
        
        // 实例化技能算力项
        GameObject item = Instantiate(computingItemPrefab, skillListContent);
        computingItems.Add(item);
        
        // 技能名称
        TMP_Text nameText = item.transform.Find("SkillName")?.GetComponent<TMP_Text>();
        if (nameText != null)
        {
            Color tierColor = skillData.GetTierColor();
            nameText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(tierColor)}>{skillData.skillName}</color>";
        }
        
        // 当前掌握度
        TMP_Text currentMasteryText = item.transform.Find("CurrentMastery")?.GetComponent<TMP_Text>();
        if (currentMasteryText != null)
        {
            float mastery = skillInstance.masteryPercent;
            Color masteryColor = GetMasteryColor(mastery);
            currentMasteryText.text = $"当前掌握度：<color=#{ColorUtility.ToHtmlStringRGB(masteryColor)}>{mastery:F0}%</color>";
        }
        
        // 算力需求信息
        TMP_Text requirementText = item.transform.Find("Requirement")?.GetComponent<TMP_Text>();
        if (requirementText != null)
        {
            requirementText.text = $"100%:{skillData.maxComputingFor100Percent} | 200%:{skillData.maxComputingFor200Percent}";
        }
        
        // 算力滑动条
        Slider computingSlider = item.transform.Find("ComputingSlider")?.GetComponent<Slider>();
        if (computingSlider != null)
        {
            // 设置滑动条范围
            computingSlider.minValue = 0f;
            computingSlider.maxValue = skillData.maxComputingFor200Percent;
            
            // 设置当前值（从临时分配获取）
            float currentAllocation = tempAllocations.ContainsKey(skillData.skillId) ? 
                tempAllocations[skillData.skillId] : skillInstance.allocatedComputing;
            computingSlider.value = currentAllocation;
            
            // 监听滑动条变化
            string capturedSkillId = skillData.skillId;
            computingSlider.onValueChanged.AddListener((value) => OnComputingSliderChanged(capturedSkillId, value));
        }
        
        // 分配算力数值显示
        TMP_Text allocationText = item.transform.Find("AllocationValue")?.GetComponent<TMP_Text>();
        if (allocationText != null)
        {
            float currentAllocation = tempAllocations.ContainsKey(skillData.skillId) ? 
                tempAllocations[skillData.skillId] : skillInstance.allocatedComputing;
            allocationText.text = $"{currentAllocation:F0}";
        }
        
        // 预览掌握度
        TMP_Text previewText = item.transform.Find("PreviewMastery")?.GetComponent<TMP_Text>();
        if (previewText != null)
        {
            float currentAllocation = tempAllocations.ContainsKey(skillData.skillId) ? 
                tempAllocations[skillData.skillId] : skillInstance.allocatedComputing;
            float previewMastery = skillData.CalculateMastery(currentAllocation);
            Color masteryColor = GetMasteryColor(previewMastery);
            previewText.text = $"→ <color=#{ColorUtility.ToHtmlStringRGB(masteryColor)}>{previewMastery:F0}%</color>";
        }
    }
    
    /// <summary>
    /// 滑动条值变化回调
    /// </summary>
    private void OnComputingSliderChanged(string skillId, float value)
    {
        // 更新临时分配
        tempAllocations[skillId] = value;
        
        // 更新算力信息显示
        UpdateComputingInfo();
        
        // 更新对应技能项的显示
        UpdateSkillItemDisplay(skillId, value);
    }
    
    /// <summary>
    /// 更新技能项的显示
    /// </summary>
    private void UpdateSkillItemDisplay(string skillId, float allocation)
    {
        // 找到对应的技能项
        foreach (GameObject item in computingItems)
        {
            // 通过查找组件来判断是否是目标技能项
            // 这里简化处理，实际项目中可以存储skillId到GameObject的某个组件中
            TMP_Text allocationText = item.transform.Find("AllocationValue")?.GetComponent<TMP_Text>();
            if (allocationText != null)
            {
                allocationText.text = $"{allocation:F0}";
            }
            
            TMP_Text previewText = item.transform.Find("PreviewMastery")?.GetComponent<TMP_Text>();
            if (previewText != null)
            {
                SkillData skillData = SkillManager.Instance.GetSkillById(skillId);
                if (skillData != null)
                {
                    float previewMastery = skillData.CalculateMastery(allocation);
                    Color masteryColor = GetMasteryColor(previewMastery);
                    previewText.text = $"→ <color=#{ColorUtility.ToHtmlStringRGB(masteryColor)}>{previewMastery:F0}%</color>";
                }
            }
        }
    }
    
    /// <summary>
    /// 重置所有算力分配
    /// </summary>
    private void ResetAllComputing()
    {
        // 清空所有临时分配
        List<string> skillIds = new List<string>(tempAllocations.Keys);
        foreach (string skillId in skillIds)
        {
            tempAllocations[skillId] = 0f;
        }
        
        // 刷新显示
        RefreshSkillList();
        UpdateComputingInfo();
        
        Debug.Log("<color=yellow>重置所有算力分配</color>");
    }
    
    /// <summary>
    /// 应用算力分配更改
    /// 将临时分配应用到SkillManager
    /// </summary>
    private void ApplyAllocations()
    {
        // 检查总算力是否超额
        float totalComputing = ResourceManager.Instance.GetComputingTotal();
        float totalAllocated = 0f;
        foreach (float allocation in tempAllocations.Values)
        {
            totalAllocated += allocation;
        }
        
        if (totalAllocated > totalComputing)
        {
            Debug.LogWarning("<color=red>算力分配超额！请调整分配</color>");
            ShowErrorMessage($"算力分配超额！已分配{totalAllocated:F0}，总算力{totalComputing:F0}");
            return;
        }
        
        // 应用所有分配
        bool allSuccess = true;
        foreach (var kvp in tempAllocations)
        {
            string skillId = kvp.Key;
            float allocation = kvp.Value;
            
            string errorMsg;
            bool success = SkillManager.Instance.AllocateComputing(skillId, allocation, out errorMsg);
            
            if (!success)
            {
                Debug.LogWarning($"<color=red>分配算力失败：{errorMsg}</color>");
                allSuccess = false;
            }
        }
        
        if (allSuccess)
        {
            Debug.Log("<color=green>✓ 算力分配更新成功！</color>");
            // 可以选择关闭面板
            // ClosePanel();
        }
        else
        {
            Debug.LogWarning("<color=yellow>部分算力分配失败，请检查</color>");
        }
    }
    
    /// <summary>
    /// 根据掌握度获取颜色
    /// </summary>
    private Color GetMasteryColor(float mastery)
    {
        if (mastery < 50f)
            return Color.red;
        else if (mastery < 100f)
            return Color.yellow;
        else if (mastery < 150f)
            return Color.cyan;
        else
            return Color.green;
    }
    
    /// <summary>
    /// 显示错误信息
    /// </summary>
    private void ShowErrorMessage(string message)
    {
        // TODO: 实现更友好的错误提示UI，比如弹窗
        Debug.LogWarning($"错误：{message}");
    }
    
    #endregion
    
    #region 事件回调
    
    /// <summary>
    /// 算力分配更新事件回调
    /// </summary>
    private void OnComputingAllocationUpdated()
    {
        // 刷新显示
        InitializeTempAllocations();
        RefreshSkillList();
        UpdateComputingInfo();
    }
    
    /// <summary>
    /// 技能掌握度更新事件回调
    /// </summary>
    private void OnMasteryUpdated(string skillId, float mastery)
    {
        // 刷新显示
        RefreshSkillList();
    }
    
    #endregion
}
