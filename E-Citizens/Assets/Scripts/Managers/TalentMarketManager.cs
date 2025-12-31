using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 人才市场管理器 - 管理AI员工市场和玩家简历系统
/// Talent Market Manager - Manages AI employee market and player resume system
/// 
/// 功能说明：
/// 1. AI员工市场（提供各品级AI员工）
/// 2. 玩家简历发布和管理
/// 3. 简历搜索和筛选
/// 4. 简历匹配推荐
/// 
/// 使用方法：
/// // 发布简历
/// TalentMarketManager.Instance.PostResume(resources, salary);
/// 
/// // 搜索简历
/// List<ResumeData> resumes = TalentMarketManager.Instance.SearchResumes(minLevel, maxSalary);
/// 
/// Unity操作步骤：
/// 1. 在Hierarchy中找到GameManager对象
/// 2. 添加TalentMarketManager脚本组件
/// 3. 确保CompanyManager和ResourceManager已添加
/// 4. 运行游戏测试功能
/// </summary>
public class TalentMarketManager : MonoBehaviour
{
    #region 单例模式 / Singleton Pattern
    
    private static TalentMarketManager instance;
    
    /// <summary>
    /// 单例实例
    /// Singleton instance
    /// </summary>
    public static TalentMarketManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TalentMarketManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("TalentMarketManager");
                    instance = go.AddComponent<TalentMarketManager>();
                }
            }
            return instance;
        }
    }
    
    #endregion
    
    #region 数据存储 / Data Storage
    
    /// <summary>
    /// 所有玩家简历列表
    /// All player resumes list
    /// </summary>
    private List<ResumeData> allResumes = new List<ResumeData>();
    
    /// <summary>
    /// 玩家简历字典（玩家ID -> 简历数据）
    /// Player resume dictionary (Player ID -> Resume Data)
    /// </summary>
    private Dictionary<string, ResumeData> playerResumes = new Dictionary<string, ResumeData>();
    
    /// <summary>
    /// 当前玩家的简历
    /// Current player's resume
    /// </summary>
    private ResumeData currentPlayerResume;
    
    #endregion
    
    #region 配置参数 / Configuration Parameters
    
    /// <summary>
    /// 简历刷新间隔（秒）
    /// Resume refresh interval (seconds)
    /// </summary>
    [SerializeField]
    private float resumeRefreshInterval = 300f; // 5分钟
    
    /// <summary>
    /// 上次刷新时间
    /// Last refresh time
    /// </summary>
    private float lastRefreshTime;
    
    #endregion
    
    #region 事件系统 / Event System
    
    /// <summary>
    /// 简历发布事件
    /// Resume posted event
    /// </summary>
    public event Action<ResumeData> OnResumePosted;
    
    /// <summary>
    /// 简历撤回事件
    /// Resume withdrawn event
    /// </summary>
    public event Action<ResumeData> OnResumeWithdrawn;
    
    /// <summary>
    /// 简历被雇佣事件
    /// Resume hired event
    /// </summary>
    public event Action<ResumeData> OnResumeHired;
    
    /// <summary>
    /// 简历列表更新事件
    /// Resume list updated event
    /// </summary>
    public event Action OnResumeListUpdated;
    
    #endregion
    
    #region Unity生命周期 / Unity Lifecycle
    
    private void Awake()
    {
        // 单例模式设置
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Debug.Log("[TalentMarketManager] 初始化完成");
    }
    
    private void Start()
    {
        // 初始化人才市场
        InitializeMarket();
        
        lastRefreshTime = Time.time;
    }
    
    private void Update()
    {
        // 定期刷新简历列表（移除过期的简历等）
        if (Time.time - lastRefreshTime >= resumeRefreshInterval)
        {
            RefreshResumeList();
            lastRefreshTime = Time.time;
        }
    }
    
    #endregion
    
    #region 市场初始化 / Market Initialization
    
    /// <summary>
    /// 初始化人才市场
    /// Initialize talent market
    /// </summary>
    private void InitializeMarket()
    {
        Debug.Log("[TalentMarketManager] 初始化人才市场");
        
        // 生成一些示例简历（用于测试）
        GenerateSampleResumes(5);
    }
    
    /// <summary>
    /// 生成示例简历（用于测试）
    /// Generate sample resumes (for testing)
    /// </summary>
    private void GenerateSampleResumes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 随机生成玩家数据
            string playerId = $"sample_player_{i + 1}";
            string playerName = $"虚拟人{UnityEngine.Random.Range(100, 999)}";
            int playerLevel = UnityEngine.Random.Range(5, 30);
            
            // 随机生成资源
            ProvidedResources resources = new ProvidedResources(
                UnityEngine.Random.Range(1f, 5f),      // memory
                UnityEngine.Random.Range(0.5f, 2f),    // cpu
                UnityEngine.Random.Range(50f, 200f),   // bandwidth
                UnityEngine.Random.Range(5f, 20f)      // computing
            );
            
            // 根据资源和等级计算期望薪资
            float expectedSalary = UnityEngine.Random.Range(30f, 100f);
            
            // 创建简历
            ResumeData resume = new ResumeData(playerId, playerName, playerLevel, resources, expectedSalary);
            
            // 添加到列表
            allResumes.Add(resume);
            playerResumes[playerId] = resume;
        }
        
        Debug.Log($"[TalentMarketManager] 生成了 {count} 个示例简历");
    }
    
    #endregion
    
    #region 玩家简历管理 / Player Resume Management
    
    /// <summary>
    /// 发布简历（当前玩家）
    /// Post resume (current player)
    /// </summary>
    /// <param name="offeredResources">提供的资源</param>
    /// <param name="expectedSalary">期望薪资</param>
    /// <returns>是否发布成功</returns>
    public bool PostResume(ProvidedResources offeredResources, float expectedSalary)
    {
        // 检查是否已有简历
        if (currentPlayerResume != null && currentPlayerResume.status == ResumeStatus.Available)
        {
            Debug.LogWarning("[TalentMarketManager] 已有可用简历，请先撤回");
            return false;
        }
        
        // 获取当前玩家信息
        string playerId = "current_player"; // TODO: 从UserData获取实际玩家ID
        string playerName = "我的虚拟人"; // TODO: 从UserData获取实际玩家名称
        int playerLevel = ResourceManager.Instance.GetPlayerLevel();
        
        // 验证资源
        if (!ValidateOfferedResources(offeredResources))
        {
            Debug.LogWarning("[TalentMarketManager] 提供的资源无效或不足");
            return false;
        }
        
        // 创建简历
        ResumeData resume = new ResumeData(playerId, playerName, playerLevel, offeredResources, expectedSalary);
        
        // 验证简历
        if (!resume.IsValid())
        {
            Debug.LogWarning("[TalentMarketManager] 简历验证失败");
            return false;
        }
        
        // 添加到市场
        allResumes.Add(resume);
        playerResumes[playerId] = resume;
        currentPlayerResume = resume;
        
        // 触发事件
        OnResumePosted?.Invoke(resume);
        OnResumeListUpdated?.Invoke();
        
        Debug.Log($"[TalentMarketManager] 成功发布简历：薪资 {expectedSalary}币/5分钟，加成 +{(resume.incomeBonus - 1f) * 100:F1}%");
        return true;
    }
    
    /// <summary>
    /// 撤回简历
    /// Withdraw resume
    /// </summary>
    /// <returns>是否撤回成功</returns>
    public bool WithdrawResume()
    {
        if (currentPlayerResume == null)
        {
            Debug.LogWarning("[TalentMarketManager] 没有可撤回的简历");
            return false;
        }
        
        if (currentPlayerResume.status == ResumeStatus.Hired)
        {
            Debug.LogWarning("[TalentMarketManager] 已被雇佣，无法撤回");
            return false;
        }
        
        // 撤回简历
        currentPlayerResume.MarkAsWithdrawn();
        
        // 触发事件
        OnResumeWithdrawn?.Invoke(currentPlayerResume);
        OnResumeListUpdated?.Invoke();
        
        Debug.Log("[TalentMarketManager] 成功撤回简历");
        return true;
    }
    
    /// <summary>
    /// 更新简历（修改资源或薪资）
    /// Update resume (modify resources or salary)
    /// </summary>
    public bool UpdateResume(ProvidedResources newResources, float newSalary)
    {
        if (currentPlayerResume == null)
        {
            Debug.LogWarning("[TalentMarketManager] 没有可更新的简历");
            return false;
        }
        
        if (currentPlayerResume.status != ResumeStatus.Available)
        {
            Debug.LogWarning("[TalentMarketManager] 只能更新可用状态的简历");
            return false;
        }
        
        // 验证资源
        if (!ValidateOfferedResources(newResources))
        {
            Debug.LogWarning("[TalentMarketManager] 提供的资源无效或不足");
            return false;
        }
        
        // 更新简历
        currentPlayerResume.offeredResources = newResources;
        currentPlayerResume.expectedSalary = newSalary;
        currentPlayerResume.RecalculateIncomeBonus();
        currentPlayerResume.postTime = DateTime.Now;
        
        // 触发事件
        OnResumeListUpdated?.Invoke();
        
        Debug.Log("[TalentMarketManager] 成功更新简历");
        return true;
    }
    
    /// <summary>
    /// 验证提供的资源是否有效
    /// Validate offered resources
    /// </summary>
    private bool ValidateOfferedResources(ProvidedResources resources)
    {
        // 检查玩家是否有足够的可用资源
        // TODO: 需要从ResourceManager获取玩家的可用资源
        
        // 暂时允许任何非负值
        return resources.memory >= 0 && 
               resources.cpu >= 0 && 
               resources.bandwidth >= 0 && 
               resources.computing >= 0;
    }
    
    /// <summary>
    /// 获取当前玩家的简历
    /// Get current player's resume
    /// </summary>
    public ResumeData GetMyResume()
    {
        return currentPlayerResume;
    }
    
    #endregion
    
    #region 简历搜索 / Resume Search
    
    /// <summary>
    /// 获取所有可用简历
    /// Get all available resumes
    /// </summary>
    public List<ResumeData> GetAvailableResumes()
    {
        return allResumes.Where(r => r.status == ResumeStatus.Available).ToList();
    }
    
    /// <summary>
    /// 搜索简历
    /// Search resumes
    /// </summary>
    /// <param name="minLevel">最低等级（0表示不限）</param>
    /// <param name="maxLevel">最高等级（0表示不限）</param>
    /// <param name="maxSalary">最高薪资（0表示不限）</param>
    /// <returns>符合条件的简历列表</returns>
    public List<ResumeData> SearchResumes(int minLevel = 0, int maxLevel = 0, float maxSalary = 0)
    {
        var query = allResumes.Where(r => r.status == ResumeStatus.Available);
        
        // 等级筛选
        if (minLevel > 0)
        {
            query = query.Where(r => r.playerLevel >= minLevel);
        }
        if (maxLevel > 0)
        {
            query = query.Where(r => r.playerLevel <= maxLevel);
        }
        
        // 薪资筛选
        if (maxSalary > 0)
        {
            query = query.Where(r => r.expectedSalary <= maxSalary);
        }
        
        return query.ToList();
    }
    
    /// <summary>
    /// 按性价比排序简历
    /// Sort resumes by cost-effectiveness
    /// </summary>
    public List<ResumeData> GetResumesByCostEffectiveness()
    {
        return GetAvailableResumes()
            .OrderByDescending(r => r.GetCostEffectiveness())
            .ToList();
    }
    
    /// <summary>
    /// 按收入加成排序简历
    /// Sort resumes by income bonus
    /// </summary>
    public List<ResumeData> GetResumesByIncomeBonus()
    {
        return GetAvailableResumes()
            .OrderByDescending(r => r.incomeBonus)
            .ToList();
    }
    
    /// <summary>
    /// 按薪资排序简历
    /// Sort resumes by salary
    /// </summary>
    public List<ResumeData> GetResumesBySalary(bool ascending = true)
    {
        var resumes = GetAvailableResumes();
        
        if (ascending)
        {
            return resumes.OrderBy(r => r.expectedSalary).ToList();
        }
        else
        {
            return resumes.OrderByDescending(r => r.expectedSalary).ToList();
        }
    }
    
    #endregion
    
    #region AI员工市场 / AI Employee Market
    
    /// <summary>
    /// 获取所有AI员工品级的信息
    /// Get information for all AI employee tiers
    /// </summary>
    public List<AIEmployeeInfo> GetAIEmployeeMarket()
    {
        List<AIEmployeeInfo> market = new List<AIEmployeeInfo>();
        
        // 添加所有品级的AI员工
        foreach (EmployeeTier tier in Enum.GetValues(typeof(EmployeeTier)))
        {
            AIEmployeeInfo info = new AIEmployeeInfo
            {
                tier = tier,
                recruitmentCost = EmployeeData.GetRecruitmentCost(tier),
                sampleEmployee = EmployeeData.CreateAIEmployee(tier)
            };
            
            market.Add(info);
        }
        
        return market;
    }
    
    #endregion
    
    #region 简历管理 / Resume Management
    
    /// <summary>
    /// 刷新简历列表（清理过期或无效的简历）
    /// Refresh resume list (clean up expired or invalid resumes)
    /// </summary>
    private void RefreshResumeList()
    {
        // 移除超过30天的已撤回简历
        DateTime cutoffTime = DateTime.Now.AddDays(-30);
        int removedCount = allResumes.RemoveAll(r => 
            r.status == ResumeStatus.Withdrawn && r.postTime < cutoffTime);
        
        if (removedCount > 0)
        {
            Debug.Log($"[TalentMarketManager] 清理了 {removedCount} 个过期简历");
            OnResumeListUpdated?.Invoke();
        }
    }
    
    /// <summary>
    /// 通过ID获取简历
    /// Get resume by ID
    /// </summary>
    public ResumeData GetResume(string resumeId)
    {
        return allResumes.Find(r => r.resumeId == resumeId);
    }
    
    /// <summary>
    /// 标记简历为已雇佣（由CompanyManager调用）
    /// Mark resume as hired (called by CompanyManager)
    /// </summary>
    public void MarkResumeAsHired(string resumeId, string companyId)
    {
        ResumeData resume = GetResume(resumeId);
        if (resume != null)
        {
            resume.MarkAsHired(companyId);
            
            // 触发事件
            OnResumeHired?.Invoke(resume);
            OnResumeListUpdated?.Invoke();
        }
    }
    
    /// <summary>
    /// 标记简历为可用（玩家离职后）
    /// Mark resume as available (after player leaves job)
    /// </summary>
    public void MarkResumeAsAvailable(string playerId)
    {
        if (playerResumes.ContainsKey(playerId))
        {
            ResumeData resume = playerResumes[playerId];
            resume.MarkAsAvailable();
            
            OnResumeListUpdated?.Invoke();
        }
    }
    
    #endregion
    
    #region 推荐系统 / Recommendation System
    
    /// <summary>
    /// 为公司推荐合适的简历
    /// Recommend suitable resumes for a company
    /// </summary>
    /// <param name="company">公司数据</param>
    /// <param name="maxResults">最多返回的结果数</param>
    /// <returns>推荐的简历列表</returns>
    public List<ResumeData> RecommendResumes(CompanyData company, int maxResults = 5)
    {
        // 计算公司可以承担的最大薪资
        float maxAffordableSalary = company.netProfit * 0.5f; // 公司利润的50%
        
        // 获取可用简历
        var resumes = GetAvailableResumes();
        
        // 按性价比和薪资筛选
        var recommended = resumes
            .Where(r => r.expectedSalary <= maxAffordableSalary)
            .OrderByDescending(r => r.GetCostEffectiveness())
            .Take(maxResults)
            .ToList();
        
        Debug.Log($"[TalentMarketManager] 为公司 {company.companyName} 推荐了 {recommended.Count} 个简历");
        return recommended;
    }
    
    #endregion
    
    #region 数据持久化 / Data Persistence
    
    /// <summary>
    /// 保存数据
    /// Save data
    /// </summary>
    public void SaveData()
    {
        // TODO: 实现Firebase同步
        Debug.Log($"[TalentMarketManager] 保存数据：{allResumes.Count}个简历");
    }
    
    /// <summary>
    /// 加载数据
    /// Load data
    /// </summary>
    public void LoadData()
    {
        // TODO: 从Firebase加载
        Debug.Log("[TalentMarketManager] 加载数据");
    }
    
    #endregion
    
    #region 调试方法 / Debug Methods
    
    /// <summary>
    /// 获取统计信息
    /// Get statistics
    /// </summary>
    public string GetStatistics()
    {
        int availableCount = allResumes.Count(r => r.status == ResumeStatus.Available);
        int hiredCount = allResumes.Count(r => r.status == ResumeStatus.Hired);
        int withdrawnCount = allResumes.Count(r => r.status == ResumeStatus.Withdrawn);
        
        return $"简历总数：{allResumes.Count}\n" +
               $"可用：{availableCount}\n" +
               $"已雇佣：{hiredCount}\n" +
               $"已撤回：{withdrawnCount}";
    }
    
    #endregion
}

/// <summary>
/// AI员工信息（用于市场展示）
/// AI employee information (for market display)
/// </summary>
[Serializable]
public class AIEmployeeInfo
{
    /// <summary>员工品级</summary>
    public EmployeeTier tier;
    
    /// <summary>招聘成本</summary>
    public float recruitmentCost;
    
    /// <summary>示例员工（用于展示属性）</summary>
    public EmployeeData sampleEmployee;
}
