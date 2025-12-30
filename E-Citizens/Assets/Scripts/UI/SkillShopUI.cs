using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 技能商店UI
/// 显示所有可用技能，让玩家浏览和购买技能
/// 
/// Unity操作步骤（零基础）：
/// 1. 在Canvas下创建一个Panel，命名为"SkillShopPanel"
/// 2. 在Panel下创建:
///    - 标题Text: "技能商店"
///    - ScrollView: 用于显示技能列表
///    - 关闭按钮
/// 3. 创建技能项预制体（SkillItemPrefab）包含:
///    - 技能名称Text
///    - 技能描述Text
///    - 价格Text
///    - 品级图标Image
///    - 状态Text（未解锁/可购买/下载中/已安装）
///    - "购买"或"查看"按钮
/// 4. 将此脚本添加到SkillShopPanel
/// 5. 连接所有UI引用
/// </summary>
public class SkillShopUI : MonoBehaviour
{
    [Header("UI面板")]
    [Tooltip("技能商店面板根对象")]
    public GameObject shopPanel;
    
    [Header("技能列表")]
    [Tooltip("技能列表的ScrollView Content")]
    public Transform skillListContent;
    
    [Tooltip("技能项预制体")]
    public GameObject skillItemPrefab;
    
    [Header("技能详情面板")]
    [Tooltip("技能详情面板")]
    public GameObject skillDetailPanel;
    
    [Tooltip("详情：技能名称")]
    public TMP_Text detailSkillName;
    
    [Tooltip("详情：技能描述")]
    public TMP_Text detailSkillDescription;
    
    [Tooltip("详情：技能品级")]
    public TMP_Text detailSkillTier;
    
    [Tooltip("详情：价格")]
    public TMP_Text detailPrice;
    
    [Tooltip("详情：文件大小")]
    public TMP_Text detailFileSize;
    
    [Tooltip("详情：前置技能")]
    public TMP_Text detailPrerequisite;
    
    [Tooltip("详情：掌握度信息")]
    public TMP_Text detailMasteryInfo;
    
    [Tooltip("详情：解锁等级")]
    public TMP_Text detailUnlockLevel;
    
    [Tooltip("购买按钮")]
    public Button purchaseButton;
    
    [Tooltip("关闭详情按钮")]
    public Button closeDetailButton;
    
    [Header("下载进度")]
    [Tooltip("下载进度条")]
    public Slider downloadProgressBar;
    
    [Tooltip("下载进度文本")]
    public TMP_Text downloadProgressText;
    
    [Tooltip("下载面板")]
    public GameObject downloadPanel;
    
    [Header("按钮")]
    [Tooltip("刷新列表按钮")]
    public Button refreshButton;
    
    [Tooltip("关闭商店面板按钮")]
    public Button closeShopButton;
    
    // 当前选中的技能
    private SkillData selectedSkill = null;
    
    // 所有技能项游戏对象（用于销毁）
    private List<GameObject> skillItems = new List<GameObject>();
    
    // 当前正在下载的技能
    private string downloadingSkillId = "";
    
    #region Unity生命周期
    
    private void Start()
    {
        // 注册按钮事件
        if (refreshButton != null)
            refreshButton.onClick.AddListener(RefreshSkillList);
        
        if (closeShopButton != null)
            closeShopButton.onClick.AddListener(CloseShop);
        
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        
        if (closeDetailButton != null)
            closeDetailButton.onClick.AddListener(CloseSkillDetail);
        
        // 注册SkillManager事件
        RegisterEvents();
        
        // 初始时隐藏面板
        if (shopPanel != null)
            shopPanel.SetActive(false);
        
        if (skillDetailPanel != null)
            skillDetailPanel.SetActive(false);
        
        if (downloadPanel != null)
            downloadPanel.SetActive(false);
    }
    
    private void OnDestroy()
    {
        // 取消按钮事件
        if (refreshButton != null)
            refreshButton.onClick.RemoveListener(RefreshSkillList);
        
        if (closeShopButton != null)
            closeShopButton.onClick.RemoveListener(CloseShop);
        
        if (purchaseButton != null)
            purchaseButton.onClick.RemoveListener(OnPurchaseClicked);
        
        if (closeDetailButton != null)
            closeDetailButton.onClick.RemoveListener(CloseSkillDetail);
        
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
            SkillManager.Instance.OnSkillPurchased += OnSkillPurchased;
            SkillManager.Instance.OnSkillDownloadStarted += OnSkillDownloadStarted;
            SkillManager.Instance.OnSkillDownloadProgress += OnSkillDownloadProgress;
            SkillManager.Instance.OnSkillDownloadCompleted += OnSkillDownloadCompleted;
            SkillManager.Instance.OnSkillListUpdated += OnSkillListUpdated;
        }
    }
    
    /// <summary>
    /// 取消事件监听
    /// </summary>
    private void UnregisterEvents()
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillPurchased -= OnSkillPurchased;
            SkillManager.Instance.OnSkillDownloadStarted -= OnSkillDownloadStarted;
            SkillManager.Instance.OnSkillDownloadProgress -= OnSkillDownloadProgress;
            SkillManager.Instance.OnSkillDownloadCompleted -= OnSkillDownloadCompleted;
            SkillManager.Instance.OnSkillListUpdated -= OnSkillListUpdated;
        }
    }
    
    #endregion
    
    #region 公共方法
    
    /// <summary>
    /// 打开技能商店
    /// 显示面板并刷新技能列表
    /// </summary>
    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            RefreshSkillList();
            Debug.Log("<color=cyan>打开技能商店</color>");
        }
    }
    
    /// <summary>
    /// 关闭技能商店
    /// </summary>
    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            CloseSkillDetail();
            Debug.Log("<color=cyan>关闭技能商店</color>");
        }
    }
    
    /// <summary>
    /// 刷新技能列表
    /// 清除旧列表，从SkillManager获取新列表并显示
    /// </summary>
    public void RefreshSkillList()
    {
        // 清除旧的技能项
        ClearSkillItems();
        
        // 获取可用技能列表
        List<SkillData> availableSkills = SkillManager.Instance.GetAvailableSkills();
        
        if (availableSkills == null || availableSkills.Count == 0)
        {
            Debug.LogWarning("没有可用的技能");
            return;
        }
        
        // 创建技能项
        foreach (SkillData skill in availableSkills)
        {
            CreateSkillItem(skill);
        }
        
        Debug.Log($"<color=green>刷新技能列表，共{availableSkills.Count}个技能</color>");
    }
    
    #endregion
    
    #region 私有方法
    
    /// <summary>
    /// 清除所有技能项
    /// </summary>
    private void ClearSkillItems()
    {
        foreach (GameObject item in skillItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        skillItems.Clear();
    }
    
    /// <summary>
    /// 创建一个技能项
    /// </summary>
    private void CreateSkillItem(SkillData skill)
    {
        if (skillItemPrefab == null || skillListContent == null)
        {
            Debug.LogError("技能项预制体或列表Content未设置！");
            return;
        }
        
        // 实例化技能项
        GameObject skillItem = Instantiate(skillItemPrefab, skillListContent);
        skillItems.Add(skillItem);
        
        // 查找并设置UI元素
        // 技能名称
        TMP_Text nameText = skillItem.transform.Find("SkillName")?.GetComponent<TMP_Text>();
        if (nameText != null)
        {
            // 添加品级颜色
            Color tierColor = skill.GetTierColor();
            nameText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(tierColor)}>{skill.skillName}</color>";
        }
        
        // 技能品级
        TMP_Text tierText = skillItem.transform.Find("SkillTier")?.GetComponent<TMP_Text>();
        if (tierText != null)
        {
            Color tierColor = skill.GetTierColor();
            tierText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(tierColor)}>[{skill.GetTierName()}]</color>";
        }
        
        // 价格信息
        TMP_Text priceText = skillItem.transform.Find("Price")?.GetComponent<TMP_Text>();
        if (priceText != null)
        {
            priceText.text = $"💰 {skill.price}币";
        }
        
        // 状态信息
        TMP_Text statusText = skillItem.transform.Find("Status")?.GetComponent<TMP_Text>();
        if (statusText != null)
        {
            bool hasSkill = SkillManager.Instance.HasSkill(skill.skillId);
            if (hasSkill)
            {
                statusText.text = "<color=green>✓ 已拥有</color>";
            }
            else
            {
                int playerLevel = ResourceManager.Instance.GetLevel();
                if (playerLevel >= skill.unlockLevel)
                {
                    // 检查前置技能
                    if (!string.IsNullOrEmpty(skill.prerequisiteSkillId))
                    {
                        bool hasPrerequisite = SkillManager.Instance.HasSkill(skill.prerequisiteSkillId);
                        if (hasPrerequisite)
                        {
                            statusText.text = "<color=yellow>可购买</color>";
                        }
                        else
                        {
                            statusText.text = "<color=red>缺少前置技能</color>";
                        }
                    }
                    else
                    {
                        statusText.text = "<color=yellow>可购买</color>";
                    }
                }
                else
                {
                    statusText.text = $"<color=gray>🔒 Lv.{skill.unlockLevel}</color>";
                }
            }
        }
        
        // 查看详情按钮
        Button viewButton = skillItem.transform.Find("ViewButton")?.GetComponent<Button>();
        if (viewButton != null)
        {
            // 捕获skill变量，避免闭包问题
            SkillData capturedSkill = skill;
            viewButton.onClick.AddListener(() => ShowSkillDetail(capturedSkill));
        }
    }
    
    /// <summary>
    /// 显示技能详情
    /// </summary>
    private void ShowSkillDetail(SkillData skill)
    {
        if (skillDetailPanel == null)
        {
            Debug.LogWarning("技能详情面板未设置！");
            return;
        }
        
        selectedSkill = skill;
        
        // 显示详情面板
        skillDetailPanel.SetActive(true);
        
        // 设置技能名称（带颜色）
        if (detailSkillName != null)
        {
            Color tierColor = skill.GetTierColor();
            detailSkillName.text = $"<color=#{ColorUtility.ToHtmlStringRGB(tierColor)}>{skill.skillName}</color>";
        }
        
        // 设置技能描述
        if (detailSkillDescription != null)
        {
            detailSkillDescription.text = skill.skillDescription;
        }
        
        // 设置技能品级
        if (detailSkillTier != null)
        {
            Color tierColor = skill.GetTierColor();
            detailSkillTier.text = $"品级：<color=#{ColorUtility.ToHtmlStringRGB(tierColor)}>{skill.GetTierName()}</color>";
        }
        
        // 设置价格
        if (detailPrice != null)
        {
            int currentCoins = ResourceManager.Instance.GetVirtualCoin();
            if (currentCoins >= skill.price)
            {
                detailPrice.text = $"价格：<color=yellow>{skill.price}币</color> (余额:{currentCoins}币)";
            }
            else
            {
                detailPrice.text = $"价格：<color=red>{skill.price}币</color> (余额:{currentCoins}币 <color=red>不足!</color>)";
            }
        }
        
        // 设置文件大小
        if (detailFileSize != null)
        {
            float availableStorage = ResourceManager.Instance.GetStorageAvailable();
            if (availableStorage >= skill.fileSize)
            {
                detailFileSize.text = $"文件大小：<color=cyan>{skill.fileSize}GB</color> (可用:{availableStorage:F1}GB)";
            }
            else
            {
                detailFileSize.text = $"文件大小：<color=red>{skill.fileSize}GB</color> (可用:{availableStorage:F1}GB <color=red>不足!</color>)";
            }
        }
        
        // 设置前置技能
        if (detailPrerequisite != null)
        {
            if (!string.IsNullOrEmpty(skill.prerequisiteSkillId))
            {
                SkillData prereqSkill = SkillManager.Instance.GetSkillById(skill.prerequisiteSkillId);
                bool hasPrereq = SkillManager.Instance.HasSkill(skill.prerequisiteSkillId);
                string prereqName = prereqSkill != null ? prereqSkill.skillName : skill.prerequisiteSkillId;
                
                if (hasPrereq)
                {
                    detailPrerequisite.text = $"前置技能：<color=green>✓ {prereqName}</color>";
                }
                else
                {
                    detailPrerequisite.text = $"前置技能：<color=red>✗ {prereqName}</color>";
                }
            }
            else
            {
                detailPrerequisite.text = "前置技能：<color=green>无</color>";
            }
        }
        
        // 设置掌握度信息
        if (detailMasteryInfo != null)
        {
            string masteryText = "掌握度机制：\n";
            masteryText += $"  初始掌握度：20%\n";
            masteryText += $"  达到100%需要：{skill.maxComputingFor100Percent}算力\n";
            masteryText += $"  达到200%需要：{skill.maxComputingFor200Percent}算力\n";
            masteryText += "  掌握度影响工作薪资";
            detailMasteryInfo.text = masteryText;
        }
        
        // 设置解锁等级
        if (detailUnlockLevel != null)
        {
            int playerLevel = ResourceManager.Instance.GetLevel();
            if (playerLevel >= skill.unlockLevel)
            {
                detailUnlockLevel.text = $"解锁等级：<color=green>Lv.{skill.unlockLevel} ✓</color>";
            }
            else
            {
                detailUnlockLevel.text = $"解锁等级：<color=red>Lv.{skill.unlockLevel} (当前Lv.{playerLevel})</color>";
            }
        }
        
        // 设置购买按钮状态
        if (purchaseButton != null)
        {
            bool hasSkill = SkillManager.Instance.HasSkill(skill.skillId);
            
            if (hasSkill)
            {
                // 已拥有，不能再购买
                purchaseButton.interactable = false;
                TMP_Text buttonText = purchaseButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = "已拥有";
                }
            }
            else
            {
                // 检查是否可以购买
                int playerLevel = ResourceManager.Instance.GetLevel();
                int currentCoins = ResourceManager.Instance.GetVirtualCoin();
                float availableStorage = ResourceManager.Instance.GetStorageAvailable();
                bool hasPrereq = string.IsNullOrEmpty(skill.prerequisiteSkillId) || 
                    SkillManager.Instance.HasSkill(skill.prerequisiteSkillId);
                
                bool canPurchase = playerLevel >= skill.unlockLevel 
                    && currentCoins >= skill.price 
                    && availableStorage >= skill.fileSize
                    && hasPrereq;
                
                purchaseButton.interactable = canPurchase;
                
                TMP_Text buttonText = purchaseButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    if (!canPurchase)
                    {
                        if (playerLevel < skill.unlockLevel)
                        {
                            buttonText.text = "等级不足";
                        }
                        else if (currentCoins < skill.price)
                        {
                            buttonText.text = "虚拟币不足";
                        }
                        else if (availableStorage < skill.fileSize)
                        {
                            buttonText.text = "存储空间不足";
                        }
                        else if (!hasPrereq)
                        {
                            buttonText.text = "缺少前置技能";
                        }
                    }
                    else
                    {
                        buttonText.text = "购买";
                    }
                }
            }
        }
        
        Debug.Log($"<color=cyan>查看技能详情：{skill.skillName}</color>");
    }
    
    /// <summary>
    /// 关闭技能详情
    /// </summary>
    private void CloseSkillDetail()
    {
        if (skillDetailPanel != null)
        {
            skillDetailPanel.SetActive(false);
        }
        selectedSkill = null;
    }
    
    /// <summary>
    /// 购买按钮点击
    /// </summary>
    private void OnPurchaseClicked()
    {
        if (selectedSkill == null)
        {
            Debug.LogWarning("没有选中的技能！");
            return;
        }
        
        // 尝试购买技能
        string errorMsg;
        bool success = SkillManager.Instance.PurchaseSkill(selectedSkill.skillId, out errorMsg);
        
        if (success)
        {
            Debug.Log($"<color=green>✓ 成功购买技能：{selectedSkill.skillName}，开始下载...</color>");
            // 关闭详情面板
            CloseSkillDetail();
        }
        else
        {
            Debug.LogWarning($"<color=red>✗ 购买技能失败：{errorMsg}</color>");
            // 显示错误提示
            ShowErrorMessage(errorMsg);
        }
    }
    
    /// <summary>
    /// 显示错误信息（可以用弹窗替代）
    /// </summary>
    private void ShowErrorMessage(string message)
    {
        // TODO: 实现更友好的错误提示UI，比如弹窗
        Debug.LogWarning($"错误：{message}");
    }
    
    #endregion
    
    #region 事件回调
    
    /// <summary>
    /// 技能购买成功事件回调
    /// </summary>
    private void OnSkillPurchased(string skillId)
    {
        // 刷新列表
        RefreshSkillList();
    }
    
    /// <summary>
    /// 技能下载开始事件回调
    /// </summary>
    private void OnSkillDownloadStarted(string skillId)
    {
        downloadingSkillId = skillId;
        
        // 显示下载面板
        if (downloadPanel != null)
        {
            downloadPanel.SetActive(true);
        }
        
        // 重置进度条
        if (downloadProgressBar != null)
        {
            downloadProgressBar.value = 0f;
        }
        
        if (downloadProgressText != null)
        {
            SkillData skill = SkillManager.Instance.GetSkillById(skillId);
            string skillName = skill != null ? skill.skillName : skillId;
            downloadProgressText.text = $"正在下载：{skillName}\n0%";
        }
    }
    
    /// <summary>
    /// 技能下载进度更新事件回调
    /// </summary>
    private void OnSkillDownloadProgress(string skillId, float progress)
    {
        if (skillId != downloadingSkillId)
            return;
        
        // 更新进度条
        if (downloadProgressBar != null)
        {
            downloadProgressBar.value = progress / 100f;
        }
        
        if (downloadProgressText != null)
        {
            SkillData skill = SkillManager.Instance.GetSkillById(skillId);
            string skillName = skill != null ? skill.skillName : skillId;
            downloadProgressText.text = $"正在下载：{skillName}\n{progress:F0}%";
        }
    }
    
    /// <summary>
    /// 技能下载完成事件回调
    /// </summary>
    private void OnSkillDownloadCompleted(string skillId)
    {
        if (skillId != downloadingSkillId)
            return;
        
        downloadingSkillId = "";
        
        // 隐藏下载面板
        if (downloadPanel != null)
        {
            downloadPanel.SetActive(false);
        }
        
        // 刷新列表
        RefreshSkillList();
        
        SkillData skill = SkillManager.Instance.GetSkillById(skillId);
        string skillName = skill != null ? skill.skillName : skillId;
        Debug.Log($"<color=green>✓ 技能下载完成：{skillName}</color>");
    }
    
    /// <summary>
    /// 技能列表更新事件回调
    /// </summary>
    private void OnSkillListUpdated(List<SkillData> skills)
    {
        // 刷新显示
        RefreshSkillList();
    }
    
    #endregion
}
