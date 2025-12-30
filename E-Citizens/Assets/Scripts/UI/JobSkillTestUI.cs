using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 工作和技能系统测试UI
/// 用于测试Phase 3的核心功能
/// </summary>
public class JobSkillTestUI : MonoBehaviour
{
    [Header("UI元素引用")]
    [Tooltip("开始工作按钮")]
    public Button startJobButton;

    [Tooltip("辞职按钮")]
    public Button resignJobButton;

    [Tooltip("购买技能按钮")]
    public Button buySkillButton;

    [Tooltip("分配算力按钮")]
    public Button allocateComputingButton;

    [Tooltip("信息显示文本")]
    public TMP_Text jobInfoText;

    private string testJobId = "job_001";      // 数据清洁工
    private string testSkillId = "dataClean_lv1"; // 数据清理 Lv.1

    private void Start()
    {
        // 注册按钮点击事件
        if (startJobButton != null)
            startJobButton.onClick.AddListener(OnStartJobClicked);

        if (resignJobButton != null)
            resignJobButton.onClick.AddListener(OnResignJobClicked);

        if (buySkillButton != null)
            buySkillButton.onClick.AddListener(OnBuySkillClicked);

        if (allocateComputingButton != null)
            allocateComputingButton.onClick.AddListener(OnAllocateComputingClicked);

        // 注册事件监听
        RegisterEvents();

        // 初始更新显示
        UpdateJobInfo();
    }

    private void OnDestroy()
    {
        // 取消按钮事件
        if (startJobButton != null)
            startJobButton.onClick.RemoveListener(OnStartJobClicked);

        if (resignJobButton != null)
            resignJobButton.onClick.RemoveListener(OnResignJobClicked);

        if (buySkillButton != null)
            buySkillButton.onClick.RemoveListener(OnBuySkillClicked);

        if (allocateComputingButton != null)
            allocateComputingButton.onClick.RemoveListener(OnAllocateComputingClicked);

        // 取消事件监听
        UnregisterEvents();
    }

    /// <summary>
    /// 注册事件监听
    /// </summary>
    private void RegisterEvents()
    {
        if (JobManager.Instance != null)
        {
            JobManager.Instance.OnJobStarted += OnJobStarted;
            JobManager.Instance.OnJobResigned += OnJobResigned;
            JobManager.Instance.OnSalaryPaid += OnSalaryPaid;
        }

        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillPurchased += OnSkillPurchased;
            SkillManager.Instance.OnMasteryUpdated += OnMasteryUpdated;
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
            JobManager.Instance.OnJobResigned -= OnJobResigned;
            JobManager.Instance.OnSalaryPaid -= OnSalaryPaid;
        }

        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillPurchased -= OnSkillPurchased;
            SkillManager.Instance.OnMasteryUpdated -= OnMasteryUpdated;
        }
    }

    /// <summary>
    /// 开始工作按钮点击
    /// </summary>
    private void OnStartJobClicked()
    {
        Debug.Log("<color=cyan>点击开始工作按钮</color>");

        string errorMsg;
        bool success = JobManager.Instance.StartJob(testJobId, out errorMsg);

        if (success)
        {
            Debug.Log("<color=green>✓ 开始工作成功！</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>✗ 开始工作失败：{errorMsg}</color>");
        }

        UpdateJobInfo();
    }

    /// <summary>
    /// 辞职按钮点击
    /// </summary>
    private void OnResignJobClicked()
    {
        Debug.Log("<color=cyan>点击辞职按钮</color>");

        // 辞职第一个工作槽位
        string errorMsg;
        bool success = JobManager.Instance.ResignJob(0, out errorMsg);

        if (success)
        {
            Debug.Log("<color=green>✓ 辞职成功！</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>✗ 辞职失败：{errorMsg}</color>");
        }

        UpdateJobInfo();
    }

    /// <summary>
    /// 购买技能按钮点击
    /// </summary>
    private void OnBuySkillClicked()
    {
        Debug.Log("<color=cyan>点击购买技能按钮</color>");

        string errorMsg;
        bool success = SkillManager.Instance.PurchaseSkill(testSkillId, out errorMsg);

        if (success)
        {
            Debug.Log("<color=green>✓ 购买技能成功！开始下载...</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>✗ 购买技能失败：{errorMsg}</color>");
        }

        UpdateJobInfo();
    }

    /// <summary>
    /// 分配算力按钮点击
    /// </summary>
    private void OnAllocateComputingClicked()
    {
        Debug.Log("<color=cyan>点击分配算力按钮</color>");

        string errorMsg;
        bool success = SkillManager.Instance.AllocateComputing(testSkillId, 10f, out errorMsg);

        if (success)
        {
            Debug.Log("<color=green>✓ 分配算力成功！</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>✗ 分配算力失败：{errorMsg}</color>");
        }

        UpdateJobInfo();
    }

    /// <summary>
    /// 更新工作信息显示
    /// </summary>
    private void UpdateJobInfo()
    {
        if (jobInfoText == null) return;

        string info = "<b><size=20>工作和技能信息</size></b>\n\n";

        // 显示活跃工作
        List<PlayerJobInstance> activeJobs = JobManager.Instance.GetActiveJobs();
        info += $"<b>活跃工作数：</b>{activeJobs.Count}/{JobManager.Instance.unlockedJobSlots}\n";

        foreach (PlayerJobInstance job in activeJobs)
        {
            JobData jobData = JobManager.Instance.GetJobById(job.jobId);
            if (jobData != null)
            {
                info += $"  • {jobData.jobName} (槽位{job.slotId})\n";
                info += $"    已工作{job.completedCycles}周期，收入{job.totalEarned}币\n";
            }
        }

        info += "\n";

        // 显示已拥有技能
        List<PlayerSkillInstance> playerSkills = SkillManager.Instance.playerSkills;
        info += $"<b>已拥有技能数：</b>{playerSkills.Count}\n";

        foreach (PlayerSkillInstance skill in playerSkills)
        {
            SkillData skillData = SkillManager.Instance.GetSkillById(skill.skillId);
            if (skillData != null)
            {
                info += $"  • {skillData.skillName}\n";
                info += $"    掌握度：{skill.masteryPercent:F0}%\n";
                info += $"    算力：{skill.allocatedComputing:F0}\n";
            }
        }

        jobInfoText.text = info;
    }

    #region 事件回调
    private void OnJobStarted(int slotId, JobData job)
    {
        UpdateJobInfo();
    }

    private void OnJobResigned(int slotId)
    {
        UpdateJobInfo();
    }

    private void OnSalaryPaid(int slotId, int salary)
    {
        UpdateJobInfo();
    }

    private void OnSkillPurchased(string skillId)
    {
        UpdateJobInfo();
    }

    private void OnMasteryUpdated(string skillId, float mastery)
    {
        UpdateJobInfo();
    }
    #endregion
}
