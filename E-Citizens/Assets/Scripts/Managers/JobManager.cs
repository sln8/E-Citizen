using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 工作管理器
/// 负责管理游戏中的所有工作相关功能
/// 包括：工作列表、承接工作、辞职、薪资结算等
/// </summary>
public class JobManager : MonoBehaviour
{
    #region 单例模式
    private static JobManager _instance;
    
    public static JobManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<JobManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("JobManager");
                    _instance = go.AddComponent<JobManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion
    
    #region 事件定义
    /// <summary>
    /// 工作列表更新事件
    /// 当可用工作列表发生变化时触发
    /// </summary>
    public event Action<List<JobData>> OnJobListUpdated;
    
    /// <summary>
    /// 开始工作事件
    /// 参数：工作槽位ID, 工作数据
    /// </summary>
    public event Action<int, JobData> OnJobStarted;
    
    /// <summary>
    /// 辞职事件
    /// 参数：工作槽位ID
    /// </summary>
    public event Action<int> OnJobResigned;
    
    /// <summary>
    /// 薪资发放事件
    /// 参数：工作槽位ID, 薪资金额
    /// </summary>
    public event Action<int, int> OnSalaryPaid;
    
    /// <summary>
    /// 工作槽位解锁事件
    /// 参数：解锁的槽位数量
    /// </summary>
    public event Action<int> OnJobSlotUnlocked;
    #endregion
    
    #region 配置数据
    [Header("工作槽位配置")]
    [Tooltip("最大工作槽位数量（基础4个 + VIP1个）")]
    public int maxJobSlots = 5;
    
    [Tooltip("当前解锁的工作槽位数量")]
    public int unlockedJobSlots = 1;  // 初始只有1个
    
    [Header("工作数据")]
    [Tooltip("所有可用的工作列表")]
    public List<JobData> allJobs = new List<JobData>();
    
    [Tooltip("玩家当前正在进行的工作")]
    public List<PlayerJobInstance> activeJobs = new List<PlayerJobInstance>();
    #endregion
    
    #region Unity生命周期
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        InitializeJobSystem();
    }
    #endregion
    
    #region 初始化
    /// <summary>
    /// 初始化工作系统
    /// 加载工作数据、检查工作槽位解锁等
    /// </summary>
    private void InitializeJobSystem()
    {
        Debug.Log("=== 初始化工作系统 ===");
        
        // 初始化工作槽位数量
        unlockedJobSlots = 1; // 初始1个工作位
        
        // 加载工作数据
        LoadJobData();
        
        // 监听游戏周期事件（用于薪资结算）
        if (GameTimerManager.Instance != null)
        {
            GameTimerManager.Instance.OnGameTickEnd += OnGameTick;
        }
        
        Debug.Log($"工作系统初始化完成。解锁工作位：{unlockedJobSlots}/{maxJobSlots}");
    }
    
    /// <summary>
    /// 加载工作数据
    /// 这里先创建一些示例工作，后续可以从配置文件或数据库加载
    /// </summary>
    private void LoadJobData()
    {
        // 清空现有数据
        allJobs.Clear();
        
        // 创建示例工作数据
        CreateSampleJobs();
        
        Debug.Log($"加载了 {allJobs.Count} 个工作");
        
        // 触发工作列表更新事件
        OnJobListUpdated?.Invoke(allJobs);
    }
    
    /// <summary>
    /// 创建示例工作数据
    /// 用于测试和演示
    /// </summary>
    private void CreateSampleJobs()
    {
        // 普通工作1：数据清洁工
        JobData job1 = new JobData
        {
            jobId = "job_001",
            jobName = "数据清洁工",
            jobDescription = "清理和整理虚拟空间中的冗余数据，保持数据库整洁。",
            jobTier = JobTier.Common,
            requiredSkillIds = new string[] { "dataClean_lv1" },
            resourceRequirement = new ResourceRequirement(1f, 0.5f, 50f, 5f),
            baseSalary = 15,
            payInterval = 300,
            dataGeneration = 0.2f,
            unlockLevel = 1
        };
        allJobs.Add(job1);
        
        // 普通工作2：虚拟巡逻员
        JobData job2 = new JobData
        {
            jobId = "job_002",
            jobName = "虚拟巡逻员",
            jobDescription = "在虚拟世界中巡逻，确保数据流正常运行。",
            jobTier = JobTier.Common,
            requiredSkillIds = new string[] { },  // 不需要技能
            resourceRequirement = new ResourceRequirement(0.5f, 0.5f, 30f, 3f),
            baseSalary = 10,
            payInterval = 300,
            dataGeneration = 0.1f,
            unlockLevel = 1
        };
        allJobs.Add(job2);
        
        // 精良工作：AI训练师
        JobData job3 = new JobData
        {
            jobId = "job_003",
            jobName = "AI训练师",
            jobDescription = "训练和优化人工智能模型，提升AI的性能。",
            jobTier = JobTier.Rare,
            requiredSkillIds = new string[] { "aiTraining_lv1", "dataAnalysis_lv1" },
            resourceRequirement = new ResourceRequirement(2f, 1f, 100f, 10f),
            baseSalary = 50,
            payInterval = 300,
            dataGeneration = 0.4f,
            unlockLevel = 5
        };
        allJobs.Add(job3);
        
        // 史诗工作：虚拟建筑师
        JobData job4 = new JobData
        {
            jobId = "job_004",
            jobName = "虚拟建筑师",
            jobDescription = "设计和构建虚拟世界的建筑结构，创造独特的数字空间。",
            jobTier = JobTier.Epic,
            requiredSkillIds = new string[] { "virtualDesign_lv2", "3dModeling_lv2", "programming_lv1" },
            resourceRequirement = new ResourceRequirement(4f, 2f, 200f, 20f),
            baseSalary = 120,
            payInterval = 300,
            dataGeneration = 0.8f,
            unlockLevel = 10
        };
        allJobs.Add(job4);
        
        // 传说工作：量子程序员
        JobData job5 = new JobData
        {
            jobId = "job_005",
            jobName = "量子程序员",
            jobDescription = "开发量子计算算法，解决超级复杂的计算问题。",
            jobTier = JobTier.Legendary,
            requiredSkillIds = new string[] { "quantumComputing_lv3", "advancedMath_lv3", "algorithm_lv3", "systemDesign_lv2" },
            resourceRequirement = new ResourceRequirement(8f, 4f, 500f, 50f),
            baseSalary = 350,
            payInterval = 300,
            dataGeneration = 1.5f,
            unlockLevel = 25
        };
        allJobs.Add(job5);
    }
    #endregion
    
    #region 工作查询
    /// <summary>
    /// 获取所有可用的工作列表
    /// 根据玩家等级和技能过滤
    /// </summary>
    /// <returns>玩家可以看到的工作列表</returns>
    public List<JobData> GetAvailableJobs()
    {
        if (ResourceManager.Instance == null)
        {
            return new List<JobData>();
        }
        
        int playerLevel = ResourceManager.Instance.GetLevel();
        List<JobData> availableJobs = new List<JobData>();
        
        foreach (JobData job in allJobs)
        {
            // 检查等级要求
            if (playerLevel >= job.unlockLevel)
            {
                availableJobs.Add(job);
            }
        }
        
        return availableJobs;
    }
    
    /// <summary>
    /// 根据ID获取工作数据
    /// </summary>
    public JobData GetJobById(string jobId)
    {
        foreach (JobData job in allJobs)
        {
            if (job.jobId == jobId)
            {
                return job;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 获取玩家当前正在进行的工作
    /// </summary>
    public List<PlayerJobInstance> GetActiveJobs()
    {
        return new List<PlayerJobInstance>(activeJobs);
    }
    
    /// <summary>
    /// 检查玩家是否有空闲的工作槽位
    /// </summary>
    public bool HasAvailableJobSlot()
    {
        return activeJobs.Count < unlockedJobSlots;
    }
    
    /// <summary>
    /// 获取下一个可用的工作槽位ID
    /// </summary>
    public int GetNextAvailableSlotId()
    {
        for (int i = 0; i < unlockedJobSlots; i++)
        {
            bool slotUsed = false;
            foreach (PlayerJobInstance job in activeJobs)
            {
                if (job.slotId == i)
                {
                    slotUsed = true;
                    break;
                }
            }
            
            if (!slotUsed)
            {
                return i;
            }
        }
        return -1; // 没有可用槽位
    }
    #endregion
    
    #region 工作操作
    /// <summary>
    /// 开始一份工作
    /// 检查所有条件（技能、资源、槽位等），如果满足则开始工作
    /// </summary>
    /// <param name="jobId">要开始的工作ID</param>
    /// <param name="errorMessage">如果失败，返回错误信息</param>
    /// <returns>成功返回true，失败返回false</returns>
    public bool StartJob(string jobId, out string errorMessage)
    {
        errorMessage = "";
        
        // 1. 检查是否有可用槽位
        if (!HasAvailableJobSlot())
        {
            errorMessage = $"没有可用的工作槽位！当前解锁：{unlockedJobSlots}个";
            return false;
        }
        
        // 2. 获取工作数据
        JobData job = GetJobById(jobId);
        if (job == null)
        {
            errorMessage = "工作不存在！";
            return false;
        }
        
        // 3. 检查等级要求
        int playerLevel = ResourceManager.Instance.GetLevel();
        if (playerLevel < job.unlockLevel)
        {
            errorMessage = $"等级不足！需要等级{job.unlockLevel}";
            return false;
        }
        
        // 4. 检查技能要求（暂时跳过，等SkillManager实现后再检查）
        // TODO: 检查玩家是否拥有所需技能
        
        // 5. 检查资源是否足够
        bool resourceAvailable = ResourceManager.Instance.TryAllocateResources(
            job.resourceRequirement.memory,
            job.resourceRequirement.cpu,
            job.resourceRequirement.bandwidth,
            job.resourceRequirement.computing
        );
        
        if (!resourceAvailable)
        {
            errorMessage = "资源不足！请释放其他工作或升级配置。";
            return false;
        }
        
        // 6. 创建工作实例
        int slotId = GetNextAvailableSlotId();
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        PlayerJobInstance jobInstance = new PlayerJobInstance(slotId, jobId, timestamp);
        
        // 7. 添加到活跃工作列表
        activeJobs.Add(jobInstance);
        
        // 8. 增加数据产生速率
        ResourceManager.Instance.AddDataGenerationRate(job.dataGeneration);
        
        // 9. 触发事件
        OnJobStarted?.Invoke(slotId, job);
        
        Debug.Log($"<color=green>✓ 开始工作：{job.jobName}（槽位{slotId}）</color>");
        Debug.Log($"  占用资源 - 内存:{job.resourceRequirement.memory}GB, CPU:{job.resourceRequirement.cpu}核");
        
        return true;
    }
    
    /// <summary>
    /// 辞职（停止工作）
    /// 释放占用的资源
    /// </summary>
    /// <param name="slotId">工作槽位ID</param>
    /// <param name="errorMessage">如果失败，返回错误信息</param>
    /// <returns>成功返回true，失败返回false</returns>
    public bool ResignJob(int slotId, out string errorMessage)
    {
        errorMessage = "";
        
        // 1. 查找工作实例
        PlayerJobInstance jobInstance = null;
        foreach (PlayerJobInstance job in activeJobs)
        {
            if (job.slotId == slotId)
            {
                jobInstance = job;
                break;
            }
        }
        
        if (jobInstance == null)
        {
            errorMessage = "该槽位没有正在进行的工作！";
            return false;
        }
        
        // 2. 获取工作数据
        JobData job = GetJobById(jobInstance.jobId);
        if (job == null)
        {
            errorMessage = "工作数据不存在！";
            return false;
        }
        
        // 3. 释放占用的资源
        ResourceManager.Instance.ReleaseResources(
            job.resourceRequirement.memory,
            job.resourceRequirement.cpu,
            job.resourceRequirement.bandwidth,
            job.resourceRequirement.computing
        );
        
        // 4. 减少数据产生速率
        ResourceManager.Instance.AddDataGenerationRate(-job.dataGeneration);
        
        // 5. 从活跃工作列表中移除
        activeJobs.Remove(jobInstance);
        
        // 6. 触发事件
        OnJobResigned?.Invoke(slotId);
        
        Debug.Log($"<color=yellow>辞职：{job.jobName}（槽位{slotId}）</color>");
        Debug.Log($"  已工作{jobInstance.completedCycles}个周期，累计收入：{jobInstance.totalEarned}币");
        
        return true;
    }
    #endregion
    
    #region 薪资结算
    /// <summary>
    /// 游戏周期回调
    /// 在每个游戏周期结束时调用，用于结算所有工作的薪资
    /// </summary>
    private void OnGameTick()
    {
        PayAllSalaries();
    }
    
    /// <summary>
    /// 结算所有工作的薪资
    /// 在每个游戏周期（5分钟）调用一次
    /// </summary>
    private void PayAllSalaries()
    {
        if (activeJobs.Count == 0)
        {
            return; // 没有工作，无需结算
        }
        
        Debug.Log($"<color=cyan>=== 结算工作薪资（{activeJobs.Count}个工作）===</color>");
        
        int totalSalary = 0;
        
        foreach (PlayerJobInstance jobInstance in activeJobs)
        {
            // 获取工作数据
            JobData job = GetJobById(jobInstance.jobId);
            if (job == null)
            {
                Debug.LogWarning($"工作数据不存在：{jobInstance.jobId}");
                continue;
            }
            
            // 计算实际薪资
            int salary = CalculateSalary(job, jobInstance);
            
            // 发放薪资
            ResourceManager.Instance.AddVirtualCoin(salary, $"工作薪资-{job.jobName}");
            
            // 更新工作实例数据
            jobInstance.completedCycles++;
            jobInstance.totalEarned += salary;
            
            // 触发事件
            OnSalaryPaid?.Invoke(jobInstance.slotId, salary);
            
            Debug.Log($"  {job.jobName}（槽位{jobInstance.slotId}）: +{salary}币");
            
            totalSalary += salary;
        }
        
        Debug.Log($"<color=cyan>总薪资：+{totalSalary}币</color>");
    }
    
    /// <summary>
    /// 计算实际薪资
    /// 公式：实际薪资 = 基础薪资 × 技能掌握度 × 效率加成
    /// </summary>
    /// <param name="job">工作数据</param>
    /// <param name="jobInstance">工作实例</param>
    /// <returns>实际薪资</returns>
    private int CalculateSalary(JobData job, PlayerJobInstance jobInstance)
    {
        // 基础薪资
        float baseSalary = job.baseSalary;
        
        // 技能掌握度加成（暂时使用jobInstance中的值，后续从SkillManager获取）
        float masteryBonus = jobInstance.skillMastery / 100f;
        
        // 效率加成（从ResourceManager获取）
        float efficiencyBonus = ResourceManager.Instance.CalculateIncomeEfficiency() / 100f;
        
        // 计算最终薪资
        float finalSalary = baseSalary * masteryBonus * efficiencyBonus;
        
        return Mathf.RoundToInt(finalSalary);
    }
    #endregion
    
    #region 工作槽位解锁
    /// <summary>
    /// 解锁工作槽位
    /// 根据玩家等级或VIP状态解锁
    /// </summary>
    /// <param name="level">玩家当前等级</param>
    /// <param name="hasVip">是否有VIP</param>
    public void CheckAndUnlockJobSlots(int level, bool hasVip)
    {
        int newSlotCount = 1; // 基础1个槽位
        
        // 根据等级解锁
        if (level >= 10) newSlotCount = 2;
        if (level >= 25) newSlotCount = 3;
        if (level >= 50) newSlotCount = 4;
        
        // VIP额外1个槽位
        if (hasVip) newSlotCount++;
        
        // 限制最大值
        newSlotCount = Mathf.Min(newSlotCount, maxJobSlots);
        
        // 如果有变化，更新并触发事件
        if (newSlotCount > unlockedJobSlots)
        {
            int oldCount = unlockedJobSlots;
            unlockedJobSlots = newSlotCount;
            
            Debug.Log($"<color=green>✓ 工作槽位已解锁：{oldCount} -> {newSlotCount}</color>");
            
            OnJobSlotUnlocked?.Invoke(unlockedJobSlots);
        }
    }
    #endregion
    
    #region 数据持久化
    /// <summary>
    /// 保存工作数据
    /// 保存玩家当前的工作状态
    /// </summary>
    public void SaveJobData()
    {
        // TODO: 保存到Firebase或本地
        // 暂时使用PlayerPrefs作为临时方案
        
        Debug.Log("保存工作数据...");
    }
    
    /// <summary>
    /// 加载工作数据
    /// 从存储中恢复玩家的工作状态
    /// </summary>
    public void LoadJobData()
    {
        // TODO: 从Firebase或本地加载
        
        Debug.Log("加载工作数据...");
    }
    #endregion
    
    #region 清理
    private void OnDestroy()
    {
        // 取消事件监听
        if (GameTimerManager.Instance != null)
        {
            GameTimerManager.Instance.OnGameTickEnd -= OnGameTick;
        }
    }
    #endregion
}
