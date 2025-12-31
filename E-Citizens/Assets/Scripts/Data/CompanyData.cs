using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 公司数据类 - 存储公司的所有信息
/// Company Data Class - Stores all company information
/// 
/// 功能说明：
/// 1. 公司基本信息（名称、等级、品级）
/// 2. 员工管理（AI员工和真实玩家）
/// 3. 财务管理（收入、支出、净利润）
/// 4. 升级系统（等级要求）
/// 
/// 使用方法：
/// CompanyData company = new CompanyData("我的公司", CompanyTier.Small, "player_123");
/// </summary>
[Serializable]
public class CompanyData
{
    #region 公司基本信息 / Basic Information
    
    /// <summary>
    /// 公司唯一ID
    /// Unique company identifier
    /// </summary>
    public string companyId;
    
    /// <summary>
    /// 公司名称
    /// Company name
    /// </summary>
    public string companyName;
    
    /// <summary>
    /// 公司品级（微型/小型/中型/大型）
    /// Company tier (Small/Medium/Large/Corporation)
    /// </summary>
    public CompanyTier tier;
    
    /// <summary>
    /// 公司等级（当前等级）
    /// Company level
    /// </summary>
    public int level;
    
    /// <summary>
    /// 公司所有者玩家ID
    /// Owner player ID
    /// </summary>
    public string ownerPlayerId;
    
    /// <summary>
    /// 公司创建时间
    /// Company creation time
    /// </summary>
    public DateTime createdAt;
    
    #endregion
    
    #region 员工管理 / Employee Management
    
    /// <summary>
    /// 员工列表（包括AI员工和真实玩家）
    /// Employee list (including AI employees and real players)
    /// </summary>
    public List<EmployeeData> employees;
    
    /// <summary>
    /// 最大员工数量（根据公司品级决定）
    /// Maximum number of employees (determined by company tier)
    /// </summary>
    public int maxEmployees;
    
    #endregion
    
    #region 财务数据 / Financial Data
    
    /// <summary>
    /// 基础收入（每5分钟，根据公司品级）
    /// Base income (per 5 minutes, based on company tier)
    /// </summary>
    public float baseIncome;
    
    /// <summary>
    /// 总收入（基础收入 + 员工加成）
    /// Total income (base income + employee bonuses)
    /// </summary>
    public float totalIncome;
    
    /// <summary>
    /// 总支出（员工薪资总和）
    /// Total expenses (sum of employee salaries)
    /// </summary>
    public float totalExpenses;
    
    /// <summary>
    /// 净利润（总收入 - 总支出）
    /// Net profit (total income - total expenses)
    /// </summary>
    public float netProfit;
    
    /// <summary>
    /// 数据产生率（每5分钟产生的数据量，GB）
    /// Data generation rate (GB per 5 minutes)
    /// </summary>
    public float dataGeneration;
    
    /// <summary>
    /// 累计收入（用于统计）
    /// Cumulative income (for statistics)
    /// </summary>
    public float cumulativeIncome;
    
    #endregion
    
    #region 升级系统 / Upgrade System
    
    /// <summary>
    /// 下一级所需收入（每5分钟）
    /// Required income per 5 minutes for next level
    /// </summary>
    public float requiredIncomeForNextLevel;
    
    /// <summary>
    /// 下一级所需员工数
    /// Required number of employees for next level
    /// </summary>
    public int requiredEmployeesForNextLevel;
    
    #endregion
    
    #region 构造函数 / Constructor
    
    /// <summary>
    /// 构造函数 - 创建新公司
    /// Constructor - Create a new company
    /// </summary>
    /// <param name="name">公司名称</param>
    /// <param name="companyTier">公司品级</param>
    /// <param name="ownerId">所有者ID</param>
    public CompanyData(string name, CompanyTier companyTier, string ownerId)
    {
        companyId = Guid.NewGuid().ToString();
        companyName = name;
        tier = companyTier;
        level = 1;
        ownerPlayerId = ownerId;
        createdAt = DateTime.Now;
        
        employees = new List<EmployeeData>();
        
        // 根据公司品级初始化数据
        // Initialize data based on company tier
        InitializeByTier(companyTier);
        
        // 计算财务数据
        // Calculate financial data
        CalculateFinancials();
        
        // 设置升级要求
        // Set upgrade requirements
        SetUpgradeRequirements();
    }
    
    #endregion
    
    #region 初始化方法 / Initialization Methods
    
    /// <summary>
    /// 根据公司品级初始化基础数据
    /// Initialize basic data based on company tier
    /// </summary>
    private void InitializeByTier(CompanyTier companyTier)
    {
        switch (companyTier)
        {
            case CompanyTier.Small:
                // 微型公司 (Small Company)
                baseIncome = 50f;
                maxEmployees = 5;
                dataGeneration = 0.5f;
                break;
                
            case CompanyTier.Medium:
                // 小型公司 (Medium Company)
                baseIncome = 150f;
                maxEmployees = 10;
                dataGeneration = 1.5f;
                break;
                
            case CompanyTier.Large:
                // 中型公司 (Large Company)
                baseIncome = 500f;
                maxEmployees = 20;
                dataGeneration = 5f;
                break;
                
            case CompanyTier.Corporation:
                // 大型企业 (Corporation)
                baseIncome = 2000f;
                maxEmployees = 50;
                dataGeneration = 20f;
                break;
        }
    }
    
    #endregion
    
    #region 财务计算 / Financial Calculations
    
    /// <summary>
    /// 计算公司财务数据
    /// Calculate company financial data
    /// </summary>
    public void CalculateFinancials()
    {
        // 计算总支出（所有员工薪资之和）
        // Calculate total expenses (sum of all employee salaries)
        totalExpenses = 0f;
        foreach (EmployeeData employee in employees)
        {
            totalExpenses += employee.salary;
        }
        
        // 计算总收入（基础收入 * 员工加成）
        // Calculate total income (base income * employee bonuses)
        float totalBonus = 1f;
        foreach (EmployeeData employee in employees)
        {
            totalBonus += (employee.incomeBonus - 1f); // 累加加成百分比
        }
        totalIncome = baseIncome * totalBonus;
        
        // 计算净利润
        // Calculate net profit
        netProfit = totalIncome - totalExpenses;
    }
    
    /// <summary>
    /// 结算收入（每5分钟调用）
    /// Settle income (called every 5 minutes)
    /// </summary>
    /// <returns>本次结算的净利润</returns>
    public float SettleIncome()
    {
        // 重新计算财务数据
        CalculateFinancials();
        
        // 累计收入
        cumulativeIncome += totalIncome;
        
        // 返回净利润（这个金额会发给公司所有者）
        return netProfit;
    }
    
    #endregion
    
    #region 员工管理 / Employee Management
    
    /// <summary>
    /// 添加员工
    /// Add employee
    /// </summary>
    /// <param name="employee">员工数据</param>
    /// <returns>是否添加成功</returns>
    public bool AddEmployee(EmployeeData employee)
    {
        // 检查是否达到上限
        if (employees.Count >= maxEmployees)
        {
            Debug.LogWarning($"[CompanyData] 无法添加员工：已达到上限 {maxEmployees}");
            return false;
        }
        
        // 添加员工
        employees.Add(employee);
        
        // 重新计算财务
        CalculateFinancials();
        
        Debug.Log($"[CompanyData] 成功添加员工：{employee.employeeName}");
        return true;
    }
    
    /// <summary>
    /// 移除员工
    /// Remove employee
    /// </summary>
    /// <param name="employeeId">员工ID</param>
    /// <returns>是否移除成功</returns>
    public bool RemoveEmployee(string employeeId)
    {
        // 查找员工
        EmployeeData employee = employees.Find(e => e.employeeId == employeeId);
        if (employee == null)
        {
            Debug.LogWarning($"[CompanyData] 无法找到员工：{employeeId}");
            return false;
        }
        
        // 移除员工
        employees.Remove(employee);
        
        // 重新计算财务
        CalculateFinancials();
        
        Debug.Log($"[CompanyData] 成功移除员工：{employee.employeeName}");
        return true;
    }
    
    /// <summary>
    /// 获取员工
    /// Get employee
    /// </summary>
    /// <param name="employeeId">员工ID</param>
    /// <returns>员工数据，如果不存在返回null</returns>
    public EmployeeData GetEmployee(string employeeId)
    {
        return employees.Find(e => e.employeeId == employeeId);
    }
    
    #endregion
    
    #region 升级系统 / Upgrade System
    
    /// <summary>
    /// 设置升级要求
    /// Set upgrade requirements
    /// </summary>
    private void SetUpgradeRequirements()
    {
        // 根据当前等级计算下一级要求
        // Calculate next level requirements based on current level
        requiredIncomeForNextLevel = baseIncome * (2f + level * 0.5f);
        requiredEmployeesForNextLevel = Mathf.Min(3 + level, maxEmployees);
    }
    
    /// <summary>
    /// 检查是否可以升级
    /// Check if company can be upgraded
    /// </summary>
    /// <returns>是否满足升级条件</returns>
    public bool CanUpgrade()
    {
        return totalIncome >= requiredIncomeForNextLevel && 
               employees.Count >= requiredEmployeesForNextLevel;
    }
    
    /// <summary>
    /// 升级公司
    /// Upgrade company
    /// </summary>
    /// <returns>是否升级成功</returns>
    public bool Upgrade()
    {
        if (!CanUpgrade())
        {
            Debug.LogWarning($"[CompanyData] 不满足升级条件");
            return false;
        }
        
        // 升级
        level++;
        
        // 增加基础收入
        baseIncome *= 1.2f;
        
        // 更新升级要求
        SetUpgradeRequirements();
        
        // 重新计算财务
        CalculateFinancials();
        
        Debug.Log($"[CompanyData] 公司升级到 Lv.{level}");
        return true;
    }
    
    #endregion
    
    #region 辅助方法 / Helper Methods
    
    /// <summary>
    /// 获取公司品级的中文名称
    /// Get company tier name in Chinese
    /// </summary>
    public string GetTierName()
    {
        switch (tier)
        {
            case CompanyTier.Small:
                return "微型公司";
            case CompanyTier.Medium:
                return "小型公司";
            case CompanyTier.Large:
                return "中型公司";
            case CompanyTier.Corporation:
                return "大型企业";
            default:
                return "未知";
        }
    }
    
    /// <summary>
    /// 获取开办成本
    /// Get company creation cost
    /// </summary>
    public static float GetCreationCost(CompanyTier tier)
    {
        switch (tier)
        {
            case CompanyTier.Small:
                return 1000f;
            case CompanyTier.Medium:
                return 5000f;
            case CompanyTier.Large:
                return 20000f;
            case CompanyTier.Corporation:
                return 100000f;
            default:
                return 0f;
        }
    }
    
    /// <summary>
    /// 获取解锁等级要求
    /// Get unlock level requirement
    /// </summary>
    public static int GetUnlockLevel(CompanyTier tier)
    {
        switch (tier)
        {
            case CompanyTier.Small:
                return 5;
            case CompanyTier.Medium:
                return 15;
            case CompanyTier.Large:
                return 30;
            case CompanyTier.Corporation:
                return 50;
            default:
                return 0;
        }
    }
    
    #endregion
}

/// <summary>
/// 公司品级枚举
/// Company tier enumeration
/// </summary>
[Serializable]
public enum CompanyTier
{
    /// <summary>微型公司（等级5解锁，1000币）</summary>
    Small,
    
    /// <summary>小型公司（等级15解锁，5000币）</summary>
    Medium,
    
    /// <summary>中型公司（等级30解锁，20000币）</summary>
    Large,
    
    /// <summary>大型企业（等级50解锁，100000币）</summary>
    Corporation
}
