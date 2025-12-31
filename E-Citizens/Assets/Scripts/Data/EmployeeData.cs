using System;
using UnityEngine;

/// <summary>
/// 员工数据类 - 存储员工信息（包括AI员工和真实玩家）
/// Employee Data Class - Stores employee information (AI and real players)
/// 
/// 功能说明：
/// 1. 支持两种员工类型：AI员工和真实玩家
/// 2. AI员工：可以培训升级，有品级系统
/// 3. 真实玩家：提供资源，收入加成基于等级和资源
/// 4. 薪资和加成计算
/// 
/// 使用方法：
/// // AI员工
/// EmployeeData aiEmployee = EmployeeData.CreateAIEmployee(EmployeeTier.Common);
/// 
/// // 真实玩家员工
/// EmployeeData playerEmployee = EmployeeData.CreatePlayerEmployee("player_123", resources, 50);
/// </summary>
[Serializable]
public class EmployeeData
{
    #region 基本信息 / Basic Information
    
    /// <summary>
    /// 员工唯一ID
    /// Unique employee identifier
    /// </summary>
    public string employeeId;
    
    /// <summary>
    /// 员工名称
    /// Employee name
    /// </summary>
    public string employeeName;
    
    /// <summary>
    /// 员工类型（AI或真实玩家）
    /// Employee type (AI or real player)
    /// </summary>
    public EmployeeType type;
    
    /// <summary>
    /// 雇佣时间
    /// Hire time
    /// </summary>
    public DateTime hiredAt;
    
    #endregion
    
    #region AI员工属性 / AI Employee Properties
    
    /// <summary>
    /// AI员工品级（普通/精良/史诗/传说）
    /// AI employee tier
    /// </summary>
    public EmployeeTier tier;
    
    /// <summary>
    /// AI员工等级（可以培训升级）
    /// AI employee level (can be trained)
    /// </summary>
    public int level;
    
    /// <summary>
    /// 最大等级（根据品级决定）
    /// Maximum level (determined by tier)
    /// </summary>
    public int maxLevel;
    
    /// <summary>
    /// 培训费用（每级）
    /// Training cost per level
    /// </summary>
    public float trainingCostPerLevel;
    
    #endregion
    
    #region 真实玩家属性 / Real Player Properties
    
    /// <summary>
    /// 真实玩家ID
    /// Real player ID
    /// </summary>
    public string realPlayerId;
    
    /// <summary>
    /// 真实玩家等级
    /// Real player level
    /// </summary>
    public int playerLevel;
    
    /// <summary>
    /// 提供的资源（真实玩家提供）
    /// Provided resources (by real player)
    /// </summary>
    public ProvidedResources providedResources;
    
    #endregion
    
    #region 薪资和加成 / Salary and Bonus
    
    /// <summary>
    /// 基础薪资（每5分钟）
    /// Base salary (per 5 minutes)
    /// </summary>
    public float baseSalary;
    
    /// <summary>
    /// 实际薪资（可能包含等级加成）
    /// Actual salary (may include level bonuses)
    /// </summary>
    public float salary;
    
    /// <summary>
    /// 收入加成（公司收入乘以此数值）
    /// Income bonus (company income multiplied by this)
    /// </summary>
    public float incomeBonus;
    
    #endregion
    
    #region 构造函数 / Constructors
    
    /// <summary>
    /// 私有构造函数 - 使用静态工厂方法创建
    /// Private constructor - use static factory methods
    /// </summary>
    private EmployeeData()
    {
        employeeId = Guid.NewGuid().ToString();
        hiredAt = DateTime.Now;
        level = 1;
    }
    
    #endregion
    
    #region 静态工厂方法 / Static Factory Methods
    
    /// <summary>
    /// 创建AI员工
    /// Create AI employee
    /// </summary>
    /// <param name="employeeTier">员工品级</param>
    /// <returns>AI员工数据</returns>
    public static EmployeeData CreateAIEmployee(EmployeeTier employeeTier)
    {
        EmployeeData employee = new EmployeeData
        {
            type = EmployeeType.AI,
            tier = employeeTier,
            employeeName = GenerateAIName(employeeTier)
        };
        
        // 根据品级设置属性
        // Set properties based on tier
        switch (employeeTier)
        {
            case EmployeeTier.Common:
                // 普通AI员工
                employee.baseSalary = 5f;
                employee.incomeBonus = 1.1f;
                employee.maxLevel = 10;
                employee.trainingCostPerLevel = 50f;
                break;
                
            case EmployeeTier.Rare:
                // 精良AI员工
                employee.baseSalary = 15f;
                employee.incomeBonus = 1.3f;
                employee.maxLevel = 25;
                employee.trainingCostPerLevel = 100f;
                break;
                
            case EmployeeTier.Epic:
                // 史诗AI员工
                employee.baseSalary = 40f;
                employee.incomeBonus = 1.6f;
                employee.maxLevel = 50;
                employee.trainingCostPerLevel = 300f;
                break;
                
            case EmployeeTier.Legendary:
                // 传说AI员工
                employee.baseSalary = 100f;
                employee.incomeBonus = 2.0f;
                employee.maxLevel = 100;
                employee.trainingCostPerLevel = 1000f;
                break;
        }
        
        // 初始薪资等于基础薪资
        employee.salary = employee.baseSalary;
        
        return employee;
    }
    
    /// <summary>
    /// 创建真实玩家员工
    /// Create real player employee
    /// </summary>
    /// <param name="playerId">玩家ID</param>
    /// <param name="playerName">玩家名称</param>
    /// <param name="level">玩家等级</param>
    /// <param name="resources">提供的资源</param>
    /// <param name="expectedSalary">期望薪资</param>
    /// <returns>真实玩家员工数据</returns>
    public static EmployeeData CreatePlayerEmployee(
        string playerId, 
        string playerName,
        int level, 
        ProvidedResources resources, 
        float expectedSalary)
    {
        EmployeeData employee = new EmployeeData
        {
            type = EmployeeType.RealPlayer,
            realPlayerId = playerId,
            employeeName = playerName,
            playerLevel = level,
            providedResources = resources,
            salary = expectedSalary
        };
        
        // 计算收入加成
        // Calculate income bonus
        employee.incomeBonus = CalculatePlayerIncomeBonus(level, resources);
        
        return employee;
    }
    
    #endregion
    
    #region AI员工培训 / AI Employee Training
    
    /// <summary>
    /// 培训AI员工（升级）
    /// Train AI employee (level up)
    /// </summary>
    /// <returns>培训是否成功</returns>
    public bool Train()
    {
        if (type != EmployeeType.AI)
        {
            Debug.LogWarning("[EmployeeData] 只能培训AI员工");
            return false;
        }
        
        if (level >= maxLevel)
        {
            Debug.LogWarning($"[EmployeeData] AI员工 {employeeName} 已达到最大等级 {maxLevel}");
            return false;
        }
        
        // 升级
        level++;
        
        // 提升收入加成（每级+0.5%）
        incomeBonus += 0.005f;
        
        Debug.Log($"[EmployeeData] AI员工 {employeeName} 升级到 Lv.{level}，收入加成：{incomeBonus:P1}");
        return true;
    }
    
    /// <summary>
    /// 获取培训成本
    /// Get training cost
    /// </summary>
    public float GetTrainingCost()
    {
        if (type != EmployeeType.AI)
            return 0f;
            
        return trainingCostPerLevel;
    }
    
    #endregion
    
    #region 辞退补偿 / Dismissal Compensation
    
    /// <summary>
    /// 计算辞退补偿金
    /// Calculate dismissal compensation
    /// </summary>
    /// <returns>补偿金额</returns>
    public float GetDismissalCompensation()
    {
        if (type == EmployeeType.AI)
        {
            // AI员工：薪资 * 2
            return salary * 2f;
        }
        else
        {
            // 真实玩家：薪资 * 2
            return salary * 2f;
        }
    }
    
    #endregion
    
    #region 辅助方法 / Helper Methods
    
    /// <summary>
    /// 生成AI员工名称
    /// Generate AI employee name
    /// </summary>
    private static string GenerateAIName(EmployeeTier tier)
    {
        string[] prefixes = { "数据", "赛博", "虚拟", "智能", "量子" };
        string[] suffixes = { "助手", "工程师", "专家", "大师", "精英" };
        
        string prefix = prefixes[UnityEngine.Random.Range(0, prefixes.Length)];
        string suffix = suffixes[UnityEngine.Random.Range(0, suffixes.Length)];
        int number = UnityEngine.Random.Range(1000, 9999);
        
        return $"{prefix}{suffix}-{number}";
    }
    
    /// <summary>
    /// 计算真实玩家的收入加成
    /// Calculate real player income bonus
    /// 
    /// 公式：基础加成(1.2) + 等级加成(等级*0.01) + 资源加成(资源价值/100*0.1)
    /// </summary>
    private static float CalculatePlayerIncomeBonus(int level, ProvidedResources resources)
    {
        // 基础加成
        float baseBonus = 1.2f;
        
        // 等级加成
        float levelBonus = level * 0.01f;
        
        // 资源价值计算
        float resourceValue = 
            resources.memory * 10f +      // 内存：10币/GB
            resources.cpu * 20f +          // CPU：20币/核
            resources.bandwidth * 0.1f +   // 网速：0.1币/Mbps
            resources.computing * 5f;      // 算力：5币/点
        
        // 资源加成
        float resourceBonus = (resourceValue / 100f) * 0.1f;
        
        // 总加成
        float totalBonus = 1f + baseBonus + levelBonus + resourceBonus;
        
        return totalBonus;
    }
    
    /// <summary>
    /// 获取员工品级的中文名称
    /// Get employee tier name in Chinese
    /// </summary>
    public string GetTierName()
    {
        if (type == EmployeeType.RealPlayer)
        {
            // 真实玩家没有品级概念
            return "真实玩家";
        }
        
        switch (tier)
        {
            case EmployeeTier.Common:
                return "普通";
            case EmployeeTier.Rare:
                return "精良";
            case EmployeeTier.Epic:
                return "史诗";
            case EmployeeTier.Legendary:
                return "传说";
            default:
                return "未知";
        }
    }
    
    /// <summary>
    /// 获取员工品级的颜色
    /// Get employee tier color
    /// </summary>
    public Color GetTierColor()
    {
        if (type == EmployeeType.RealPlayer)
        {
            return new Color(0.2f, 0.8f, 1f); // 天蓝色
        }
        
        switch (tier)
        {
            case EmployeeTier.Common:
                return Color.white;
            case EmployeeTier.Rare:
                return new Color(0.2f, 0.6f, 1f); // 蓝色
            case EmployeeTier.Epic:
                return new Color(0.7f, 0.2f, 1f); // 紫色
            case EmployeeTier.Legendary:
                return new Color(1f, 0.6f, 0f);   // 橙色
            default:
                return Color.gray;
        }
    }
    
    /// <summary>
    /// 获取招聘成本
    /// Get recruitment cost
    /// </summary>
    public static float GetRecruitmentCost(EmployeeTier tier)
    {
        switch (tier)
        {
            case EmployeeTier.Common:
                return 100f;
            case EmployeeTier.Rare:
                return 500f;
            case EmployeeTier.Epic:
                return 2000f;
            case EmployeeTier.Legendary:
                return 10000f;
            default:
                return 0f;
        }
    }
    
    #endregion
}

/// <summary>
/// 员工类型枚举
/// Employee type enumeration
/// </summary>
[Serializable]
public enum EmployeeType
{
    /// <summary>AI员工</summary>
    AI,
    
    /// <summary>真实玩家</summary>
    RealPlayer
}

/// <summary>
/// 员工品级枚举（AI员工）
/// Employee tier enumeration (AI employees)
/// </summary>
[Serializable]
public enum EmployeeTier
{
    /// <summary>普通（招聘100币，薪资5，加成1.1，最高Lv.10）</summary>
    Common,
    
    /// <summary>精良（招聘500币，薪资15，加成1.3，最高Lv.25）</summary>
    Rare,
    
    /// <summary>史诗（招聘2000币，薪资40，加成1.6，最高Lv.50）</summary>
    Epic,
    
    /// <summary>传说（招聘10000币，薪资100，加成2.0，最高Lv.100）</summary>
    Legendary
}

/// <summary>
/// 真实玩家提供的资源
/// Resources provided by real player
/// </summary>
[Serializable]
public struct ProvidedResources
{
    /// <summary>提供的内存（GB）</summary>
    public float memory;
    
    /// <summary>提供的CPU（核）</summary>
    public float cpu;
    
    /// <summary>提供的网速（Mbps）</summary>
    public float bandwidth;
    
    /// <summary>提供的算力</summary>
    public float computing;
    
    public ProvidedResources(float mem, float cpuCores, float bw, float comp)
    {
        memory = mem;
        cpu = cpuCores;
        bandwidth = bw;
        computing = comp;
    }
}
