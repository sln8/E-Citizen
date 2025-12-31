using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 公司管理器 - 管理所有公司相关功能
/// Company Manager - Manages all company-related functions
/// 
/// 功能说明：
/// 1. 公司创建和管理
/// 2. 员工招聘和辞退
/// 3. 公司收入结算（每5分钟）
/// 4. 公司升级系统
/// 5. 数据持久化
/// 
/// 使用方法：
/// CompanyManager.Instance.CreateCompany("我的公司", CompanyTier.Small);
/// CompanyManager.Instance.HireAIEmployee(companyId, EmployeeTier.Common);
/// 
/// Unity操作步骤：
/// 1. 在Hierarchy中找到GameManager对象
/// 2. 添加CompanyManager脚本组件
/// 3. 确保ResourceManager和GameManager已添加
/// 4. 运行游戏测试功能
/// </summary>
public class CompanyManager : MonoBehaviour
{
    #region 单例模式 / Singleton Pattern
    
    private static CompanyManager instance;
    
    /// <summary>
    /// 单例实例
    /// Singleton instance
    /// </summary>
    public static CompanyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CompanyManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("CompanyManager");
                    instance = go.AddComponent<CompanyManager>();
                }
            }
            return instance;
        }
    }
    
    #endregion
    
    #region 数据存储 / Data Storage
    
    /// <summary>
    /// 玩家拥有的公司列表（玩家ID -> 公司列表）
    /// Player-owned companies (Player ID -> Company List)
    /// </summary>
    private Dictionary<string, List<CompanyData>> playerCompanies = new Dictionary<string, List<CompanyData>>();
    
    /// <summary>
    /// 所有公司字典（公司ID -> 公司数据）
    /// All companies dictionary (Company ID -> Company Data)
    /// </summary>
    private Dictionary<string, CompanyData> allCompanies = new Dictionary<string, CompanyData>();
    
    #endregion
    
    #region 事件系统 / Event System
    
    /// <summary>
    /// 公司创建事件
    /// Company created event
    /// </summary>
    public event Action<CompanyData> OnCompanyCreated;
    
    /// <summary>
    /// 公司升级事件
    /// Company upgraded event
    /// </summary>
    public event Action<CompanyData> OnCompanyUpgraded;
    
    /// <summary>
    /// 员工雇佣事件
    /// Employee hired event
    /// </summary>
    public event Action<string, EmployeeData> OnEmployeeHired; // companyId, employee
    
    /// <summary>
    /// 员工辞退事件
    /// Employee dismissed event
    /// </summary>
    public event Action<string, EmployeeData> OnEmployeeDismissed; // companyId, employee
    
    /// <summary>
    /// 公司收入结算事件
    /// Company income settled event
    /// </summary>
    public event Action<string, float> OnIncomeSettled; // companyId, netProfit
    
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
        
        Debug.Log("[CompanyManager] 初始化完成");
    }
    
    private void Start()
    {
        // 订阅游戏定时器的周期事件
        if (GameTimerManager.Instance != null)
        {
            GameTimerManager.Instance.OnGameTick += HandleGameTick;
            Debug.Log("[CompanyManager] 已订阅游戏周期事件");
        }
        else
        {
            Debug.LogWarning("[CompanyManager] 未找到GameTimerManager，公司收入将不会自动结算");
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件
        if (GameTimerManager.Instance != null)
        {
            GameTimerManager.Instance.OnGameTick -= HandleGameTick;
        }
    }
    
    #endregion
    
    #region 公司创建 / Company Creation
    
    /// <summary>
    /// 创建公司
    /// Create a company
    /// </summary>
    /// <param name="companyName">公司名称</param>
    /// <param name="tier">公司品级</param>
    /// <param name="ownerId">所有者ID（默认使用当前玩家）</param>
    /// <returns>创建的公司数据，如果失败返回null</returns>
    public CompanyData CreateCompany(string companyName, CompanyTier tier, string ownerId = null)
    {
        // 使用当前玩家ID（如果未指定）
        if (string.IsNullOrEmpty(ownerId))
        {
            ownerId = "current_player"; // TODO: 从UserData获取实际玩家ID
        }
        
        // 检查玩家等级是否满足要求
        int requiredLevel = CompanyData.GetUnlockLevel(tier);
        int playerLevel = ResourceManager.Instance.GetPlayerLevel();
        if (playerLevel < requiredLevel)
        {
            Debug.LogWarning($"[CompanyManager] 玩家等级不足：需要 Lv.{requiredLevel}，当前 Lv.{playerLevel}");
            return null;
        }
        
        // 检查虚拟币是否足够
        float creationCost = CompanyData.GetCreationCost(tier);
        if (!ResourceManager.Instance.CanAfford(creationCost))
        {
            Debug.LogWarning($"[CompanyManager] 虚拟币不足：需要 {creationCost}币");
            return null;
        }
        
        // 扣除虚拟币
        if (!ResourceManager.Instance.SpendVirtualCoin(creationCost))
        {
            Debug.LogWarning($"[CompanyManager] 扣除虚拟币失败");
            return null;
        }
        
        // 创建公司
        CompanyData company = new CompanyData(companyName, tier, ownerId);
        
        // 添加到字典
        allCompanies[company.companyId] = company;
        
        // 添加到玩家公司列表
        if (!playerCompanies.ContainsKey(ownerId))
        {
            playerCompanies[ownerId] = new List<CompanyData>();
        }
        playerCompanies[ownerId].Add(company);
        
        // 触发事件
        OnCompanyCreated?.Invoke(company);
        
        Debug.Log($"[CompanyManager] 成功创建公司：{companyName} ({company.GetTierName()})，花费 {creationCost}币");
        return company;
    }
    
    /// <summary>
    /// 获取玩家的公司列表
    /// Get player's company list
    /// </summary>
    public List<CompanyData> GetPlayerCompanies(string playerId = null)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            playerId = "current_player"; // TODO: 从UserData获取实际玩家ID
        }
        
        if (playerCompanies.ContainsKey(playerId))
        {
            return playerCompanies[playerId];
        }
        
        return new List<CompanyData>();
    }
    
    /// <summary>
    /// 通过ID获取公司
    /// Get company by ID
    /// </summary>
    public CompanyData GetCompany(string companyId)
    {
        if (allCompanies.ContainsKey(companyId))
        {
            return allCompanies[companyId];
        }
        
        Debug.LogWarning($"[CompanyManager] 未找到公司：{companyId}");
        return null;
    }
    
    #endregion
    
    #region 员工招聘 / Employee Recruitment
    
    /// <summary>
    /// 招聘AI员工
    /// Hire AI employee
    /// </summary>
    /// <param name="companyId">公司ID</param>
    /// <param name="tier">员工品级</param>
    /// <returns>是否招聘成功</returns>
    public bool HireAIEmployee(string companyId, EmployeeTier tier)
    {
        // 获取公司
        CompanyData company = GetCompany(companyId);
        if (company == null)
        {
            return false;
        }
        
        // 检查是否已达到员工上限
        if (company.employees.Count >= company.maxEmployees)
        {
            Debug.LogWarning($"[CompanyManager] 公司 {company.companyName} 已达到员工上限");
            return false;
        }
        
        // 检查招聘成本
        float recruitmentCost = EmployeeData.GetRecruitmentCost(tier);
        if (!ResourceManager.Instance.CanAfford(recruitmentCost))
        {
            Debug.LogWarning($"[CompanyManager] 虚拟币不足：需要 {recruitmentCost}币");
            return false;
        }
        
        // 扣除虚拟币
        if (!ResourceManager.Instance.SpendVirtualCoin(recruitmentCost))
        {
            Debug.LogWarning($"[CompanyManager] 扣除虚拟币失败");
            return false;
        }
        
        // 创建AI员工
        EmployeeData employee = EmployeeData.CreateAIEmployee(tier);
        
        // 添加到公司
        if (!company.AddEmployee(employee))
        {
            // 如果添加失败，退还虚拟币
            ResourceManager.Instance.AddVirtualCoin(recruitmentCost);
            return false;
        }
        
        // 触发事件
        OnEmployeeHired?.Invoke(companyId, employee);
        
        Debug.Log($"[CompanyManager] 成功招聘AI员工：{employee.employeeName}，花费 {recruitmentCost}币");
        return true;
    }
    
    /// <summary>
    /// 招聘真实玩家员工（通过简历）
    /// Hire real player employee (through resume)
    /// </summary>
    /// <param name="companyId">公司ID</param>
    /// <param name="resume">简历数据</param>
    /// <returns>是否招聘成功</returns>
    public bool HirePlayerEmployee(string companyId, ResumeData resume)
    {
        // 获取公司
        CompanyData company = GetCompany(companyId);
        if (company == null)
        {
            return false;
        }
        
        // 检查简历是否可雇佣
        if (!resume.CanBeHired())
        {
            Debug.LogWarning($"[CompanyManager] 简历不可雇佣：{resume.playerName}");
            return false;
        }
        
        // 检查是否已达到员工上限
        if (company.employees.Count >= company.maxEmployees)
        {
            Debug.LogWarning($"[CompanyManager] 公司 {company.companyName} 已达到员工上限");
            return false;
        }
        
        // 创建员工数据
        EmployeeData employee = EmployeeData.CreatePlayerEmployee(
            resume.playerId,
            resume.playerName,
            resume.playerLevel,
            resume.offeredResources,
            resume.expectedSalary
        );
        
        // 添加到公司
        if (!company.AddEmployee(employee))
        {
            return false;
        }
        
        // 标记简历为已雇佣
        resume.MarkAsHired(companyId);
        
        // 触发事件
        OnEmployeeHired?.Invoke(companyId, employee);
        
        Debug.Log($"[CompanyManager] 成功招聘真实玩家：{employee.employeeName}，薪资 {employee.salary}币/5分钟");
        return true;
    }
    
    #endregion
    
    #region 员工辞退 / Employee Dismissal
    
    /// <summary>
    /// 辞退员工
    /// Dismiss employee
    /// </summary>
    /// <param name="companyId">公司ID</param>
    /// <param name="employeeId">员工ID</param>
    /// <returns>是否辞退成功</returns>
    public bool DismissEmployee(string companyId, string employeeId)
    {
        // 获取公司
        CompanyData company = GetCompany(companyId);
        if (company == null)
        {
            return false;
        }
        
        // 获取员工
        EmployeeData employee = company.GetEmployee(employeeId);
        if (employee == null)
        {
            Debug.LogWarning($"[CompanyManager] 未找到员工：{employeeId}");
            return false;
        }
        
        // 计算辞退补偿
        float compensation = employee.GetDismissalCompensation();
        
        // 检查虚拟币是否足够支付补偿
        if (!ResourceManager.Instance.CanAfford(compensation))
        {
            Debug.LogWarning($"[CompanyManager] 虚拟币不足以支付辞退补偿：需要 {compensation}币");
            return false;
        }
        
        // 扣除虚拟币
        if (!ResourceManager.Instance.SpendVirtualCoin(compensation))
        {
            Debug.LogWarning($"[CompanyManager] 扣除虚拟币失败");
            return false;
        }
        
        // 从公司移除员工
        if (!company.RemoveEmployee(employeeId))
        {
            // 如果移除失败，退还虚拟币
            ResourceManager.Instance.AddVirtualCoin(compensation);
            return false;
        }
        
        // 如果是真实玩家，需要更新简历状态（通过TalentMarketManager）
        // TODO: 集成TalentMarketManager后实现
        
        // 触发事件
        OnEmployeeDismissed?.Invoke(companyId, employee);
        
        Debug.Log($"[CompanyManager] 成功辞退员工：{employee.employeeName}，支付补偿 {compensation}币");
        return true;
    }
    
    #endregion
    
    #region AI员工培训 / AI Employee Training
    
    /// <summary>
    /// 培训AI员工
    /// Train AI employee
    /// </summary>
    /// <param name="companyId">公司ID</param>
    /// <param name="employeeId">员工ID</param>
    /// <returns>是否培训成功</returns>
    public bool TrainEmployee(string companyId, string employeeId)
    {
        // 获取公司
        CompanyData company = GetCompany(companyId);
        if (company == null)
        {
            return false;
        }
        
        // 获取员工
        EmployeeData employee = company.GetEmployee(employeeId);
        if (employee == null)
        {
            Debug.LogWarning($"[CompanyManager] 未找到员工：{employeeId}");
            return false;
        }
        
        // 检查是否是AI员工
        if (employee.type != EmployeeType.AI)
        {
            Debug.LogWarning($"[CompanyManager] 只能培训AI员工");
            return false;
        }
        
        // 获取培训成本
        float trainingCost = employee.GetTrainingCost();
        
        // 检查虚拟币
        if (!ResourceManager.Instance.CanAfford(trainingCost))
        {
            Debug.LogWarning($"[CompanyManager] 虚拟币不足：需要 {trainingCost}币");
            return false;
        }
        
        // 扣除虚拟币
        if (!ResourceManager.Instance.SpendVirtualCoin(trainingCost))
        {
            Debug.LogWarning($"[CompanyManager] 扣除虚拟币失败");
            return false;
        }
        
        // 培训员工
        if (!employee.Train())
        {
            // 如果培训失败，退还虚拟币
            ResourceManager.Instance.AddVirtualCoin(trainingCost);
            return false;
        }
        
        // 重新计算公司财务
        company.CalculateFinancials();
        
        Debug.Log($"[CompanyManager] 成功培训员工：{employee.employeeName} -> Lv.{employee.level}，花费 {trainingCost}币");
        return true;
    }
    
    #endregion
    
    #region 公司升级 / Company Upgrade
    
    /// <summary>
    /// 升级公司
    /// Upgrade company
    /// </summary>
    /// <param name="companyId">公司ID</param>
    /// <returns>是否升级成功</returns>
    public bool UpgradeCompany(string companyId)
    {
        // 获取公司
        CompanyData company = GetCompany(companyId);
        if (company == null)
        {
            return false;
        }
        
        // 尝试升级
        if (company.Upgrade())
        {
            // 触发事件
            OnCompanyUpgraded?.Invoke(company);
            
            return true;
        }
        
        return false;
    }
    
    #endregion
    
    #region 收入结算 / Income Settlement
    
    /// <summary>
    /// 处理游戏周期（每5分钟）
    /// Handle game tick (every 5 minutes)
    /// </summary>
    private void HandleGameTick()
    {
        Debug.Log("[CompanyManager] 开始结算所有公司收入");
        
        // 结算所有公司的收入
        foreach (var company in allCompanies.Values)
        {
            SettleCompanyIncome(company);
        }
    }
    
    /// <summary>
    /// 结算单个公司的收入
    /// Settle single company income
    /// </summary>
    private void SettleCompanyIncome(CompanyData company)
    {
        // 结算收入
        float netProfit = company.SettleIncome();
        
        // 如果有利润，发给公司所有者
        if (netProfit > 0)
        {
            ResourceManager.Instance.AddVirtualCoin(netProfit);
            Debug.Log($"[CompanyManager] 公司 {company.companyName} 结算收入：{netProfit:F1}币");
        }
        else if (netProfit < 0)
        {
            // 如果亏损，从所有者账户扣除
            if (ResourceManager.Instance.CanAfford(-netProfit))
            {
                ResourceManager.Instance.SpendVirtualCoin(-netProfit);
                Debug.Log($"[CompanyManager] 公司 {company.companyName} 亏损：{-netProfit:F1}币");
            }
            else
            {
                Debug.LogWarning($"[CompanyManager] 公司 {company.companyName} 亏损，但所有者账户余额不足");
                // TODO: 处理破产情况
            }
        }
        
        // 产生数据
        ResourceManager.Instance.GenerateData(company.dataGeneration);
        
        // 触发事件
        OnIncomeSettled?.Invoke(company.companyId, netProfit);
    }
    
    #endregion
    
    #region 数据持久化 / Data Persistence
    
    /// <summary>
    /// 保存所有公司数据
    /// Save all company data
    /// </summary>
    public void SaveData()
    {
        // TODO: 实现Firebase同步
        Debug.Log($"[CompanyManager] 保存数据：{allCompanies.Count}个公司");
    }
    
    /// <summary>
    /// 加载所有公司数据
    /// Load all company data
    /// </summary>
    public void LoadData()
    {
        // TODO: 从Firebase加载
        Debug.Log("[CompanyManager] 加载数据");
    }
    
    #endregion
    
    #region 调试方法 / Debug Methods
    
    /// <summary>
    /// 获取所有公司统计信息
    /// Get all companies statistics
    /// </summary>
    public string GetStatistics()
    {
        return $"公司总数：{allCompanies.Count}\n" +
               $"玩家拥有的公司：{GetPlayerCompanies().Count}\n" +
               $"总员工数：{allCompanies.Values.Sum(c => c.employees.Count)}";
    }
    
    #endregion
}
