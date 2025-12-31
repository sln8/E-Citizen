using UnityEngine;

/*
 * 安全卫士方案数据类
 * Security Plan Data Class
 * 
 * 功能说明：
 * 1. 定义安全卫士方案的所有属性
 * 2. 提供防御率和费用查询
 * 3. 处理等级解锁要求
 * 
 * 使用示例：
 * SecurityPlanData plan = SecurityPlanData.CreateBasicPlan();
 * bool canBlock = Random.value < plan.defenseRate; // 判断是否防御成功
 * 
 * 作者：GitHub Copilot
 * 日期：2025-12-31
 */

/// <summary>
/// 安全卫士方案类型枚举
/// </summary>
public enum SecurityPlanType
{
    None,       // 无安全卫士
    Basic,      // 普通安全卫士
    Advanced,   // 高级安全卫士
    Ultimate    // 神级安全卫士
}

/// <summary>
/// 安全卫士方案数据类
/// 存储单个安全卫士方案的所有信息
/// </summary>
[System.Serializable]
public class SecurityPlanData
{
    // ==================== 基础信息 ====================
    
    /// <summary>方案ID</summary>
    public string planId;
    
    /// <summary>方案名称</summary>
    public string planName;
    
    /// <summary>方案类型</summary>
    public SecurityPlanType planType;
    
    // ==================== 费用与防御 ====================
    
    /// <summary>每5分钟的费用（虚拟币）</summary>
    public int costPer5Min;
    
    /// <summary>防御率（0-1，如0.4表示40%概率直接防御成功）</summary>
    public float defenseRate;
    
    // ==================== 解锁条件 ====================
    
    /// <summary>解锁所需等级</summary>
    public int unlockLevel;
    
    // ==================== 视觉资源 ====================
    
    /// <summary>图标路径</summary>
    public string iconPath;
    
    // ==================== 构造函数 ====================
    
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public SecurityPlanData()
    {
        planId = "";
        planName = "";
        planType = SecurityPlanType.None;
        costPer5Min = 0;
        defenseRate = 0f;
        unlockLevel = 1;
        iconPath = "";
    }
    
    /// <summary>
    /// 完整参数构造函数
    /// </summary>
    public SecurityPlanData(
        string id,
        string name,
        SecurityPlanType type,
        int cost,
        float defense,
        int level,
        string icon)
    {
        planId = id;
        planName = name;
        planType = type;
        costPer5Min = cost;
        defenseRate = Mathf.Clamp01(defense); // 确保在0-1之间
        unlockLevel = level;
        iconPath = icon;
    }
    
    // ==================== 查询方法 ====================
    
    /// <summary>
    /// 检查是否满足解锁条件
    /// </summary>
    /// <param name="playerLevel">玩家当前等级</param>
    /// <returns>是否已解锁</returns>
    public bool IsUnlocked(int playerLevel)
    {
        return playerLevel >= unlockLevel;
    }
    
    /// <summary>
    /// 获取解锁提示
    /// </summary>
    public string GetUnlockHint(int playerLevel)
    {
        if (IsUnlocked(playerLevel))
        {
            return "已解锁";
        }
        else
        {
            return $"需要等级{unlockLevel}";
        }
    }
    
    /// <summary>
    /// 判断是否免费方案
    /// </summary>
    public bool IsFree()
    {
        return costPer5Min == 0;
    }
    
    /// <summary>
    /// 判断是否有防御能力
    /// </summary>
    public bool HasDefense()
    {
        return defenseRate > 0f;
    }
    
    // ==================== 防御计算 ====================
    
    /// <summary>
    /// 计算是否成功防御病毒入侵
    /// </summary>
    /// <returns>true表示防御成功，false表示需要玩家应对</returns>
    public bool TryDefend()
    {
        if (defenseRate <= 0f)
        {
            return false; // 无防御能力
        }
        
        // 随机判断是否防御成功
        float roll = Random.value; // 0-1之间的随机数
        return roll < defenseRate;
    }
    
    /// <summary>
    /// 获取防御率百分比字符串
    /// </summary>
    public string GetDefenseRateString()
    {
        return $"{defenseRate * 100f:F0}%";
    }
    
    // ==================== 性价比计算 ====================
    
    /// <summary>
    /// 计算每小时费用
    /// </summary>
    public int GetCostPerHour()
    {
        return costPer5Min * 12; // 1小时 = 12个5分钟周期
    }
    
    /// <summary>
    /// 计算每天费用
    /// </summary>
    public int GetCostPerDay()
    {
        return costPer5Min * 288; // 1天 = 288个5分钟周期
    }
    
    /// <summary>
    /// 计算防御性价比（防御率/每小时费用）
    /// 值越大越划算
    /// </summary>
    public float GetDefenseValueRatio()
    {
        if (costPer5Min <= 0)
        {
            return defenseRate > 0 ? float.MaxValue : 0f; // 免费且有防御 = 无限划算
        }
        
        int costPerHour = GetCostPerHour();
        return defenseRate / costPerHour * 1000f; // 乘以1000便于显示
    }
    
    // ==================== 静态工厂方法 ====================
    
    /// <summary>
    /// 创建"无安全卫士"方案
    /// </summary>
    public static SecurityPlanData CreateNonePlan()
    {
        return new SecurityPlanData(
            id: "security_none",
            name: "无",
            type: SecurityPlanType.None,
            cost: 0,
            defense: 0f,
            level: 1,
            icon: ""
        );
    }
    
    /// <summary>
    /// 创建"普通安全卫士"方案
    /// </summary>
    public static SecurityPlanData CreateBasicPlan()
    {
        return new SecurityPlanData(
            id: "security_basic",
            name: "普通安全卫士",
            type: SecurityPlanType.Basic,
            cost: 5,
            defense: 0.4f, // 40%防御率
            level: 1,
            icon: "security_basic.png"
        );
    }
    
    /// <summary>
    /// 创建"高级安全卫士"方案
    /// </summary>
    public static SecurityPlanData CreateAdvancedPlan()
    {
        return new SecurityPlanData(
            id: "security_advanced",
            name: "高级安全卫士",
            type: SecurityPlanType.Advanced,
            cost: 15,
            defense: 0.7f, // 70%防御率
            level: 15,
            icon: "security_advanced.png"
        );
    }
    
    /// <summary>
    /// 创建"神级安全卫士"方案
    /// </summary>
    public static SecurityPlanData CreateUltimatePlan()
    {
        return new SecurityPlanData(
            id: "security_ultimate",
            name: "神级安全卫士",
            type: SecurityPlanType.Ultimate,
            cost: 50,
            defense: 0.99f, // 99%防御率
            level: 40,
            icon: "security_ultimate.png"
        );
    }
    
    // ==================== 调试信息 ====================
    
    /// <summary>
    /// 获取安全卫士方案的详细信息（用于调试）
    /// </summary>
    public override string ToString()
    {
        return $"[安全卫士] {planName}\n" +
               $"ID: {planId}\n" +
               $"类型: {planType}\n" +
               $"费用: {costPer5Min}币/5分钟 ({GetCostPerHour()}币/小时)\n" +
               $"防御率: {GetDefenseRateString()}\n" +
               $"解锁等级: {unlockLevel}\n" +
               $"性价比: {GetDefenseValueRatio():F2}";
    }
}
