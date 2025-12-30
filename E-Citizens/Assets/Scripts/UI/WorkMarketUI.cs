using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 工作市场UI
/// 显示所有可用工作，让玩家浏览和承接工作
/// 
/// Unity操作步骤（零基础）：
/// 1. 在Canvas下创建一个Panel，命名为"WorkMarketPanel"
/// 2. 在Panel下创建:
///    - 标题Text: "工作市场"
///    - ScrollView: 用于显示工作列表
///    - 关闭按钮
/// 3. 创建工作项预制体（WorkItemPrefab）包含:
///    - 工作名称Text
///    - 工作描述Text
///    - 薪资Text
///    - 品级图标Image
///    - "开始工作"按钮
/// 4. 将此脚本添加到WorkMarketPanel
/// 5. 连接所有UI引用
/// </summary>
public class WorkMarketUI : MonoBehaviour
{
    [Header("UI面板")]
    [Tooltip("工作市场面板根对象")]
    public GameObject marketPanel;
    
    [Header("工作列表")]
    [Tooltip("工作列表的ScrollView Content")]
    public Transform jobListContent;
    
    [Tooltip("工作项预制体")]
    public GameObject jobItemPrefab;
    
    [Header("工作详情面板")]
    [Tooltip("工作详情面板")]
    public GameObject jobDetailPanel;
    
    [Tooltip("详情：工作名称")]
    public TMP_Text detailJobName;
    
    [Tooltip("详情：工作描述")]
    public TMP_Text detailJobDescription;
    
    [Tooltip("详情：工作品级")]
    public TMP_Text detailJobTier;
    
    [Tooltip("详情：基础薪资")]
    public TMP_Text detailBaseSalary;
    
    [Tooltip("详情：所需技能")]
    public TMP_Text detailRequiredSkills;
    
    [Tooltip("详情：资源需求")]
    public TMP_Text detailResourceRequirement;
    
    [Tooltip("详情：解锁等级")]
    public TMP_Text detailUnlockLevel;
    
    [Tooltip("开始工作按钮")]
    public Button startJobButton;
    
    [Tooltip("关闭详情按钮")]
    public Button closeDetailButton;
    
    [Header("按钮")]
    [Tooltip("刷新列表按钮")]
    public Button refreshButton;
    
    [Tooltip("关闭市场面板按钮")]
    public Button closeMarketButton;
    
    // 当前选中的工作
    private JobData selectedJob = null;
    
    // 所有工作项游戏对象（用于销毁）
    private List<GameObject> jobItems = new List<GameObject>();
    
    #region Unity生命周期
    
    private void Start()
    {
        // 注册按钮事件
        if (refreshButton != null)
            refreshButton.onClick.AddListener(RefreshJobList);
        
        if (closeMarketButton != null)
            closeMarketButton.onClick.AddListener(CloseMarket);
        
        if (startJobButton != null)
            startJobButton.onClick.AddListener(OnStartJobClicked);
        
        if (closeDetailButton != null)
            closeDetailButton.onClick.AddListener(CloseJobDetail);
        
        // 注册JobManager事件
        RegisterEvents();
        
        // 初始时隐藏面板
        if (marketPanel != null)
            marketPanel.SetActive(false);
        
        if (jobDetailPanel != null)
            jobDetailPanel.SetActive(false);
    }
    
    private void OnDestroy()
    {
        // 取消按钮事件
        if (refreshButton != null)
            refreshButton.onClick.RemoveListener(RefreshJobList);
        
        if (closeMarketButton != null)
            closeMarketButton.onClick.RemoveListener(CloseMarket);
        
        if (startJobButton != null)
            startJobButton.onClick.RemoveListener(OnStartJobClicked);
        
        if (closeDetailButton != null)
            closeDetailButton.onClick.RemoveListener(CloseJobDetail);
        
        // 取消JobManager事件
        UnregisterEvents();
    }
    
    #endregion
    
    #region 事件管理
    
    /// <summary>
    /// 注册事件监听
    /// </summary>
    private void RegisterEvents()
    {
        if (JobManager.Instance != null)
        {
            JobManager.Instance.OnJobStarted += OnJobStarted;
            JobManager.Instance.OnJobListUpdated += OnJobListUpdated;
        }
    }
    
    /// <summary>
    /// 取消事件监听
    /// </summary>
    private void UnregisterEvents()
    {
        if (JobManager.Instance != null)
        {
            JobManager.Instance.OnJobStarted -= OnJobStarted;
            JobManager.Instance.OnJobListUpdated -= OnJobListUpdated;
        }
    }
    
    #endregion
    
    #region 公共方法
    
    /// <summary>
    /// 打开工作市场
    /// 显示面板并刷新工作列表
    /// </summary>
    public void OpenMarket()
    {
        if (marketPanel != null)
        {
            marketPanel.SetActive(true);
            RefreshJobList();
            Debug.Log("<color=cyan>打开工作市场</color>");
        }
    }
    
    /// <summary>
    /// 关闭工作市场
    /// </summary>
    public void CloseMarket()
    {
        if (marketPanel != null)
        {
            marketPanel.SetActive(false);
            CloseJobDetail();
            Debug.Log("<color=cyan>关闭工作市场</color>");
        }
    }
    
    /// <summary>
    /// 刷新工作列表
    /// 清除旧列表，从JobManager获取新列表并显示
    /// </summary>
    public void RefreshJobList()
    {
        // 清除旧的工作项
        ClearJobItems();
        
        // 获取可用工作列表
        List<JobData> availableJobs = JobManager.Instance.GetAvailableJobs();
        
        if (availableJobs == null || availableJobs.Count == 0)
        {
            Debug.LogWarning("没有可用的工作");
            return;
        }
        
        // 创建工作项
        foreach (JobData job in availableJobs)
        {
            CreateJobItem(job);
        }
        
        Debug.Log($"<color=green>刷新工作列表，共{availableJobs.Count}个工作</color>");
    }
    
    #endregion
    
    #region 私有方法
    
    /// <summary>
    /// 清除所有工作项
    /// </summary>
    private void ClearJobItems()
    {
        foreach (GameObject item in jobItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        jobItems.Clear();
    }
    
    /// <summary>
    /// 创建一个工作项
    /// </summary>
    private void CreateJobItem(JobData job)
    {
        if (jobItemPrefab == null || jobListContent == null)
        {
            Debug.LogError("工作项预制体或列表Content未设置！");
            return;
        }
        
        // 实例化工作项
        GameObject jobItem = Instantiate(jobItemPrefab, jobListContent);
        jobItems.Add(jobItem);
        
        // 查找并设置UI元素
        // 工作名称
        TMP_Text nameText = jobItem.transform.Find("JobName")?.GetComponent<TMP_Text>();
        if (nameText != null)
        {
            // 添加品级颜色
            Color tierColor = job.GetTierColor();
            nameText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(tierColor)}>{job.jobName}</color>";
        }
        
        // 工作品级
        TMP_Text tierText = jobItem.transform.Find("JobTier")?.GetComponent<TMP_Text>();
        if (tierText != null)
        {
            Color tierColor = job.GetTierColor();
            tierText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(tierColor)}>[{job.GetTierName()}]</color>";
        }
        
        // 薪资信息
        TMP_Text salaryText = jobItem.transform.Find("Salary")?.GetComponent<TMP_Text>();
        if (salaryText != null)
        {
            salaryText.text = $"💰 {job.baseSalary}币/5分钟";
        }
        
        // 解锁等级
        TMP_Text levelText = jobItem.transform.Find("Level")?.GetComponent<TMP_Text>();
        if (levelText != null)
        {
            int playerLevel = ResourceManager.Instance.GetLevel();
            if (playerLevel >= job.unlockLevel)
            {
                levelText.text = $"✓ Lv.{job.unlockLevel}";
                levelText.color = Color.green;
            }
            else
            {
                levelText.text = $"🔒 Lv.{job.unlockLevel}";
                levelText.color = Color.red;
            }
        }
        
        // 查看详情按钮
        Button viewButton = jobItem.transform.Find("ViewButton")?.GetComponent<Button>();
        if (viewButton != null)
        {
            // 捕获job变量，避免闭包问题
            JobData capturedJob = job;
            viewButton.onClick.AddListener(() => ShowJobDetail(capturedJob));
        }
    }
    
    /// <summary>
    /// 显示工作详情
    /// </summary>
    private void ShowJobDetail(JobData job)
    {
        if (jobDetailPanel == null)
        {
            Debug.LogWarning("工作详情面板未设置！");
            return;
        }
        
        selectedJob = job;
        
        // 显示详情面板
        jobDetailPanel.SetActive(true);
        
        // 设置工作名称（带颜色）
        if (detailJobName != null)
        {
            Color tierColor = job.GetTierColor();
            detailJobName.text = $"<color=#{ColorUtility.ToHtmlStringRGB(tierColor)}>{job.jobName}</color>";
        }
        
        // 设置工作描述
        if (detailJobDescription != null)
        {
            detailJobDescription.text = job.jobDescription;
        }
        
        // 设置工作品级
        if (detailJobTier != null)
        {
            Color tierColor = job.GetTierColor();
            detailJobTier.text = $"品级：<color=#{ColorUtility.ToHtmlStringRGB(tierColor)}>{job.GetTierName()}</color>";
        }
        
        // 设置基础薪资
        if (detailBaseSalary != null)
        {
            detailBaseSalary.text = $"基础薪资：<color=yellow>{job.baseSalary}币/5分钟</color>";
        }
        
        // 设置所需技能
        if (detailRequiredSkills != null)
        {
            if (job.requiredSkillIds != null && job.requiredSkillIds.Length > 0)
            {
                string skillsText = "所需技能：\n";
                foreach (string skillId in job.requiredSkillIds)
                {
                    SkillData skill = SkillManager.Instance.GetSkillById(skillId);
                    bool hasSkill = SkillManager.Instance.HasSkill(skillId);
                    string skillName = skill != null ? skill.skillName : skillId;
                    
                    if (hasSkill)
                    {
                        skillsText += $"  <color=green>✓ {skillName}</color>\n";
                    }
                    else
                    {
                        skillsText += $"  <color=red>✗ {skillName}</color>\n";
                    }
                }
                detailRequiredSkills.text = skillsText;
            }
            else
            {
                detailRequiredSkills.text = "所需技能：<color=green>无</color>";
            }
        }
        
        // 设置资源需求
        if (detailResourceRequirement != null)
        {
            string resText = "资源需求：\n";
            resText += $"  内存：{job.resourceRequirement.memory}GB\n";
            resText += $"  CPU：{job.resourceRequirement.cpu}核\n";
            resText += $"  网速：{job.resourceRequirement.bandwidth}Mbps\n";
            resText += $"  算力：{job.resourceRequirement.computing}\n";
            detailResourceRequirement.text = resText;
        }
        
        // 设置解锁等级
        if (detailUnlockLevel != null)
        {
            int playerLevel = ResourceManager.Instance.GetLevel();
            if (playerLevel >= job.unlockLevel)
            {
                detailUnlockLevel.text = $"解锁等级：<color=green>Lv.{job.unlockLevel} ✓</color>";
            }
            else
            {
                detailUnlockLevel.text = $"解锁等级：<color=red>Lv.{job.unlockLevel} (当前Lv.{playerLevel})</color>";
            }
        }
        
        // 设置开始工作按钮状态
        if (startJobButton != null)
        {
            // 检查是否可以开始工作
            int playerLevel = ResourceManager.Instance.GetLevel();
            bool canStart = playerLevel >= job.unlockLevel 
                && JobManager.Instance.HasAvailableJobSlot();
            
            startJobButton.interactable = canStart;
            
            TMP_Text buttonText = startJobButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                if (!canStart)
                {
                    if (playerLevel < job.unlockLevel)
                    {
                        buttonText.text = "等级不足";
                    }
                    else if (!JobManager.Instance.HasAvailableJobSlot())
                    {
                        buttonText.text = "没有空闲槽位";
                    }
                }
                else
                {
                    buttonText.text = "开始工作";
                }
            }
        }
        
        Debug.Log($"<color=cyan>查看工作详情：{job.jobName}</color>");
    }
    
    /// <summary>
    /// 关闭工作详情
    /// </summary>
    private void CloseJobDetail()
    {
        if (jobDetailPanel != null)
        {
            jobDetailPanel.SetActive(false);
        }
        selectedJob = null;
    }
    
    /// <summary>
    /// 开始工作按钮点击
    /// </summary>
    private void OnStartJobClicked()
    {
        if (selectedJob == null)
        {
            Debug.LogWarning("没有选中的工作！");
            return;
        }
        
        // 尝试开始工作
        string errorMsg;
        bool success = JobManager.Instance.StartJob(selectedJob.jobId, out errorMsg);
        
        if (success)
        {
            Debug.Log($"<color=green>✓ 成功开始工作：{selectedJob.jobName}</color>");
            // 关闭详情面板
            CloseJobDetail();
            // 可以选择关闭市场面板或刷新列表
            // CloseMarket();
        }
        else
        {
            Debug.LogWarning($"<color=red>✗ 开始工作失败：{errorMsg}</color>");
            // 可以显示错误提示UI
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
    /// 工作开始事件回调
    /// </summary>
    private void OnJobStarted(int slotId, JobData job)
    {
        // 刷新列表（可选）
        // RefreshJobList();
    }
    
    /// <summary>
    /// 工作列表更新事件回调
    /// </summary>
    private void OnJobListUpdated(List<JobData> jobs)
    {
        // 刷新显示
        RefreshJobList();
    }
    
    #endregion
}
