using System;
using UnityEngine;

/// <summary>
/// 简历数据类 - 真实玩家在人才市场发布的简历
/// Resume Data Class - Resume published by real players in the talent market
/// 
/// 功能说明：
/// 1. 玩家可以发布简历，供其他玩家的公司招聘
/// 2. 简历包含玩家信息、提供的资源、期望薪资
/// 3. 简历状态管理（可用/已被雇佣/已撤回）
/// 4. 自动计算收入加成
/// 
/// 使用方法：
/// ProvidedResources resources = new ProvidedResources(2, 1, 100, 10);
/// ResumeData resume = new ResumeData("player_123", "虚拟人001", 15, resources, 50);
/// </summary>
[Serializable]
public class ResumeData
{
    #region 基本信息 / Basic Information
    
    /// <summary>
    /// 简历唯一ID
    /// Unique resume identifier
    /// </summary>
    public string resumeId;
    
    /// <summary>
    /// 玩家ID
    /// Player ID
    /// </summary>
    public string playerId;
    
    /// <summary>
    /// 玩家名称
    /// Player name
    /// </summary>
    public string playerName;
    
    /// <summary>
    /// 玩家等级
    /// Player level
    /// </summary>
    public int playerLevel;
    
    /// <summary>
    /// 发布时间
    /// Post time
    /// </summary>
    public DateTime postTime;
    
    /// <summary>
    /// 简历状态
    /// Resume status
    /// </summary>
    public ResumeStatus status;
    
    #endregion
    
    #region 资源和薪资 / Resources and Salary
    
    /// <summary>
    /// 提供的资源
    /// Offered resources
    /// </summary>
    public ProvidedResources offeredResources;
    
    /// <summary>
    /// 期望薪资（每5分钟）
    /// Expected salary (per 5 minutes)
    /// </summary>
    public float expectedSalary;
    
    /// <summary>
    /// 收入加成（自动计算）
    /// Income bonus (automatically calculated)
    /// </summary>
    public float incomeBonus;
    
    #endregion
    
    #region 雇佣信息 / Employment Information
    
    /// <summary>
    /// 雇佣此玩家的公司ID（如果已被雇佣）
    /// Company ID that hired this player (if hired)
    /// </summary>
    public string employedByCompanyId;
    
    /// <summary>
    /// 被雇佣时间
    /// Hired time
    /// </summary>
    public DateTime hiredTime;
    
    #endregion
    
    #region 构造函数 / Constructor
    
    /// <summary>
    /// 构造函数 - 创建新简历
    /// Constructor - Create a new resume
    /// </summary>
    /// <param name="pId">玩家ID</param>
    /// <param name="pName">玩家名称</param>
    /// <param name="pLevel">玩家等级</param>
    /// <param name="resources">提供的资源</param>
    /// <param name="salary">期望薪资</param>
    public ResumeData(string pId, string pName, int pLevel, ProvidedResources resources, float salary)
    {
        resumeId = Guid.NewGuid().ToString();
        playerId = pId;
        playerName = pName;
        playerLevel = pLevel;
        offeredResources = resources;
        expectedSalary = salary;
        postTime = DateTime.Now;
        status = ResumeStatus.Available;
        
        // 计算收入加成
        // Calculate income bonus
        incomeBonus = CalculateIncomeBonus();
    }
    
    #endregion
    
    #region 收入加成计算 / Income Bonus Calculation
    
    /// <summary>
    /// 计算收入加成
    /// Calculate income bonus
    /// 
    /// 公式（按照游戏设计文档）：
    /// 基础加成 = 1.2
    /// 等级加成 = 玩家等级 * 0.01
    /// 资源加成 = (提供的总资源价值 / 100) * 0.1
    /// 最终加成 = 1 + 基础加成 + 等级加成 + 资源加成
    /// </summary>
    private float CalculateIncomeBonus()
    {
        // 基础加成
        float baseBonus = 1.2f;
        
        // 等级加成
        float levelBonus = playerLevel * 0.01f;
        
        // 资源价值计算
        // 内存：10币/GB，CPU：20币/核，网速：0.1币/Mbps，算力：5币/点
        float resourceValue = 
            offeredResources.memory * 10f +
            offeredResources.cpu * 20f +
            offeredResources.bandwidth * 0.1f +
            offeredResources.computing * 5f;
        
        // 资源加成
        float resourceBonus = (resourceValue / 100f) * 0.1f;
        
        // 总加成
        float totalBonus = 1f + baseBonus + levelBonus + resourceBonus;
        
        return totalBonus;
    }
    
    /// <summary>
    /// 重新计算收入加成（当玩家等级或资源变化时调用）
    /// Recalculate income bonus (when player level or resources change)
    /// </summary>
    public void RecalculateIncomeBonus()
    {
        incomeBonus = CalculateIncomeBonus();
    }
    
    #endregion
    
    #region 简历状态管理 / Resume Status Management
    
    /// <summary>
    /// 标记为已雇佣
    /// Mark as hired
    /// </summary>
    /// <param name="companyId">雇佣的公司ID</param>
    public void MarkAsHired(string companyId)
    {
        if (status != ResumeStatus.Available)
        {
            Debug.LogWarning($"[ResumeData] 简历 {resumeId} 状态不是可用，无法雇佣");
            return;
        }
        
        status = ResumeStatus.Hired;
        employedByCompanyId = companyId;
        hiredTime = DateTime.Now;
        
        Debug.Log($"[ResumeData] 玩家 {playerName} 被公司 {companyId} 雇佣");
    }
    
    /// <summary>
    /// 标记为已撤回
    /// Mark as withdrawn
    /// </summary>
    public void MarkAsWithdrawn()
    {
        if (status == ResumeStatus.Hired)
        {
            Debug.LogWarning($"[ResumeData] 简历 {resumeId} 已被雇佣，无法撤回");
            return;
        }
        
        status = ResumeStatus.Withdrawn;
        
        Debug.Log($"[ResumeData] 玩家 {playerName} 撤回了简历");
    }
    
    /// <summary>
    /// 标记为可用（离职后重新发布）
    /// Mark as available (repost after leaving job)
    /// </summary>
    public void MarkAsAvailable()
    {
        status = ResumeStatus.Available;
        employedByCompanyId = null;
        postTime = DateTime.Now;
        
        Debug.Log($"[ResumeData] 玩家 {playerName} 重新发布了简历");
    }
    
    #endregion
    
    #region 验证方法 / Validation Methods
    
    /// <summary>
    /// 检查简历是否有效
    /// Check if resume is valid
    /// </summary>
    /// <returns>是否有效</returns>
    public bool IsValid()
    {
        // 检查基本信息
        if (string.IsNullOrEmpty(playerId) || string.IsNullOrEmpty(playerName))
        {
            return false;
        }
        
        // 检查等级
        if (playerLevel < 1)
        {
            return false;
        }
        
        // 检查薪资
        if (expectedSalary < 0)
        {
            return false;
        }
        
        // 检查资源（至少要提供一些资源）
        if (offeredResources.memory <= 0 && 
            offeredResources.cpu <= 0 && 
            offeredResources.bandwidth <= 0 && 
            offeredResources.computing <= 0)
        {
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 检查简历是否可雇佣
    /// Check if resume can be hired
    /// </summary>
    public bool CanBeHired()
    {
        return status == ResumeStatus.Available && IsValid();
    }
    
    #endregion
    
    #region 辅助方法 / Helper Methods
    
    /// <summary>
    /// 获取简历的总资源价值（用于排序和筛选）
    /// Get total resource value of the resume (for sorting and filtering)
    /// </summary>
    public float GetTotalResourceValue()
    {
        return offeredResources.memory * 10f +
               offeredResources.cpu * 20f +
               offeredResources.bandwidth * 0.1f +
               offeredResources.computing * 5f;
    }
    
    /// <summary>
    /// 获取性价比（收入加成 / 期望薪资）
    /// Get cost-effectiveness (income bonus / expected salary)
    /// </summary>
    public float GetCostEffectiveness()
    {
        if (expectedSalary <= 0)
            return 0f;
            
        return (incomeBonus - 1f) / expectedSalary;
    }
    
    /// <summary>
    /// 获取状态的中文名称
    /// Get status name in Chinese
    /// </summary>
    public string GetStatusName()
    {
        switch (status)
        {
            case ResumeStatus.Available:
                return "可用";
            case ResumeStatus.Hired:
                return "已被雇佣";
            case ResumeStatus.Withdrawn:
                return "已撤回";
            default:
                return "未知";
        }
    }
    
    /// <summary>
    /// 获取状态的颜色
    /// Get status color
    /// </summary>
    public Color GetStatusColor()
    {
        switch (status)
        {
            case ResumeStatus.Available:
                return Color.green;
            case ResumeStatus.Hired:
                return Color.gray;
            case ResumeStatus.Withdrawn:
                return Color.red;
            default:
                return Color.white;
        }
    }
    
    /// <summary>
    /// 获取简历摘要（用于列表显示）
    /// Get resume summary (for list display)
    /// </summary>
    public string GetSummary()
    {
        return $"{playerName} (Lv.{playerLevel}) | 薪资: {expectedSalary}币 | 加成: +{(incomeBonus - 1f) * 100:F1}%";
    }
    
    /// <summary>
    /// 获取详细描述
    /// Get detailed description
    /// </summary>
    public string GetDetailedDescription()
    {
        return $"玩家: {playerName}\n" +
               $"等级: {playerLevel}\n" +
               $"提供资源:\n" +
               $"  内存: {offeredResources.memory}GB\n" +
               $"  CPU: {offeredResources.cpu}核\n" +
               $"  网速: {offeredResources.bandwidth}Mbps\n" +
               $"  算力: {offeredResources.computing}\n" +
               $"期望薪资: {expectedSalary}币/5分钟\n" +
               $"收入加成: +{(incomeBonus - 1f) * 100:F1}%\n" +
               $"资源价值: {GetTotalResourceValue():F0}币\n" +
               $"性价比: {GetCostEffectiveness():F2}\n" +
               $"状态: {GetStatusName()}\n" +
               $"发布时间: {postTime:yyyy-MM-dd HH:mm}";
    }
    
    #endregion
}

/// <summary>
/// 简历状态枚举
/// Resume status enumeration
/// </summary>
[Serializable]
public enum ResumeStatus
{
    /// <summary>可用 - 等待被雇佣</summary>
    Available,
    
    /// <summary>已被雇佣</summary>
    Hired,
    
    /// <summary>已撤回</summary>
    Withdrawn
}
