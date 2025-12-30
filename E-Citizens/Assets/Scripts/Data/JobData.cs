using System;
using UnityEngine;

/// <summary>
/// 工作品级枚举
/// 定义工作的稀有度和价值等级
/// </summary>
public enum JobTier
{
    Common,      // 普通 - 薪资 10-30/5分钟，需要1个技能
    Rare,        // 精良 - 薪资 40-80/5分钟，需要2个技能
    Epic,        // 史诗 - 薪资 100-200/5分钟，需要3个技能
    Legendary    // 传说 - 薪资 300-500/5分钟，需要4个技能
}

/// <summary>
/// 工作状态枚举
/// 定义工作的当前状态
/// </summary>
public enum JobStatus
{
    Available,   // 可承接
    Working,     // 工作中
    Completed,   // 已完成
    Resigned     // 已辞职
}

/// <summary>
/// 资源需求数据结构
/// 定义工作所需的各项硬件资源
/// </summary>
[Serializable]
public struct ResourceRequirement
{
    [Tooltip("所需内存（GB）")]
    public float memory;
    
    [Tooltip("所需CPU核心数")]
    public float cpu;
    
    [Tooltip("所需网速（Mbps）")]
    public float bandwidth;
    
    [Tooltip("所需算力点数")]
    public float computing;
    
    /// <summary>
    /// 构造函数 - 创建资源需求
    /// </summary>
    public ResourceRequirement(float memory, float cpu, float bandwidth, float computing)
    {
        this.memory = memory;
        this.cpu = cpu;
        this.bandwidth = bandwidth;
        this.computing = computing;
    }
    
    /// <summary>
    /// 转换为字符串显示
    /// </summary>
    public override string ToString()
    {
        return $"内存:{memory}GB, CPU:{cpu}核, 网速:{bandwidth}Mbps, 算力:{computing}";
    }
}

/// <summary>
/// 工作数据类
/// 存储单个工作的所有信息
/// 包括工作ID、名称、薪资、技能要求、资源需求等
/// </summary>
[Serializable]
public class JobData
{
    #region 基础信息
    /// <summary>
    /// 工作唯一ID
    /// 格式：job_001, job_002 等
    /// </summary>
    [Header("基础信息")]
    [Tooltip("工作唯一标识符")]
    public string jobId;
    
    /// <summary>
    /// 工作名称
    /// 例如：数据清洁工、AI训练师、虚拟建筑师等
    /// </summary>
    [Tooltip("工作名称")]
    public string jobName;
    
    /// <summary>
    /// 工作描述
    /// 介绍这份工作的内容和特点
    /// </summary>
    [Tooltip("工作描述")]
    [TextArea(2, 4)]
    public string jobDescription;
    
    /// <summary>
    /// 工作品级
    /// 决定工作的稀有度和薪资范围
    /// </summary>
    [Tooltip("工作品级")]
    public JobTier jobTier = JobTier.Common;
    #endregion
    
    #region 技能要求
    /// <summary>
    /// 所需技能ID列表
    /// 例如：["dataClean_lv1", "aiTraining_lv2"]
    /// 玩家必须拥有所有列出的技能才能承接这份工作
    /// </summary>
    [Header("技能要求")]
    [Tooltip("所需的技能ID列表")]
    public string[] requiredSkillIds;
    #endregion
    
    #region 资源需求
    /// <summary>
    /// 工作所需的硬件资源
    /// 开始工作时会占用这些资源，辞职后释放
    /// </summary>
    [Header("资源需求")]
    [Tooltip("工作所需的资源")]
    public ResourceRequirement resourceRequirement;
    #endregion
    
    #region 薪资信息
    /// <summary>
    /// 基础薪资（每5分钟）
    /// 实际薪资 = 基础薪资 × 技能掌握度 × 效率加成
    /// </summary>
    [Header("薪资信息")]
    [Tooltip("基础薪资（虚拟币/5分钟）")]
    public int baseSalary = 10;
    
    /// <summary>
    /// 结算间隔（秒）
    /// 默认300秒（5分钟）
    /// </summary>
    [Tooltip("薪资结算间隔（秒）")]
    public int payInterval = 300;
    #endregion
    
    #region 数据产生
    /// <summary>
    /// 每个周期产生的数据量（GB）
    /// 工作会产生额外的数据占用存储空间
    /// </summary>
    [Header("数据产生")]
    [Tooltip("每周期产生的数据量（GB）")]
    public float dataGeneration = 0.2f;
    #endregion
    
    #region 解锁条件
    /// <summary>
    /// 解锁等级
    /// 玩家达到此等级后才能看到并承接这份工作
    /// </summary>
    [Header("解锁条件")]
    [Tooltip("解锁等级")]
    public int unlockLevel = 1;
    #endregion
    
    #region 工作状态（运行时）
    /// <summary>
    /// 当前工作状态
    /// 运行时动态更新，不序列化到数据库
    /// </summary>
    [NonSerialized]
    public JobStatus status = JobStatus.Available;
    
    /// <summary>
    /// 工作开始时间
    /// 用于计算工作时长
    /// </summary>
    [NonSerialized]
    public DateTime startTime;
    
    /// <summary>
    /// 已工作时长（秒）
    /// 用于显示和结算
    /// </summary>
    [NonSerialized]
    public float workedDuration = 0f;
    #endregion
    
    #region 方法
    /// <summary>
    /// 构造函数 - 创建默认工作数据
    /// </summary>
    public JobData()
    {
        jobId = "job_default";
        jobName = "未命名工作";
        jobDescription = "工作描述";
        jobTier = JobTier.Common;
        requiredSkillIds = new string[0];
        resourceRequirement = new ResourceRequirement(1f, 0.5f, 50f, 5f);
        baseSalary = 10;
        payInterval = 300;
        dataGeneration = 0.2f;
        unlockLevel = 1;
    }
    
    /// <summary>
    /// 获取工作品级的中文名称
    /// </summary>
    public string GetTierName()
    {
        switch (jobTier)
        {
            case JobTier.Common: return "普通";
            case JobTier.Rare: return "精良";
            case JobTier.Epic: return "史诗";
            case JobTier.Legendary: return "传说";
            default: return "未知";
        }
    }
    
    /// <summary>
    /// 获取工作品级的颜色
    /// 用于UI显示
    /// </summary>
    public Color GetTierColor()
    {
        switch (jobTier)
        {
            case JobTier.Common: return Color.white;          // 普通 - 白色
            case JobTier.Rare: return Color.blue;             // 精良 - 蓝色
            case JobTier.Epic: return new Color(0.5f, 0f, 1f); // 史诗 - 紫色
            case JobTier.Legendary: return Color.yellow;      // 传说 - 金色
            default: return Color.white;
        }
    }
    
    /// <summary>
    /// 检查玩家是否满足技能要求
    /// </summary>
    /// <param name="playerSkillIds">玩家拥有的技能ID列表</param>
    /// <returns>如果满足所有技能要求返回true，否则返回false</returns>
    public bool CheckSkillRequirement(string[] playerSkillIds)
    {
        if (requiredSkillIds == null || requiredSkillIds.Length == 0)
        {
            return true; // 不需要技能
        }
        
        // 检查玩家是否拥有所有必需的技能
        foreach (string requiredSkill in requiredSkillIds)
        {
            bool hasSkill = false;
            foreach (string playerSkill in playerSkillIds)
            {
                if (playerSkill == requiredSkill)
                {
                    hasSkill = true;
                    break;
                }
            }
            
            if (!hasSkill)
            {
                return false; // 缺少必需技能
            }
        }
        
        return true; // 满足所有技能要求
    }
    
    /// <summary>
    /// 获取工作的详细信息字符串
    /// 用于调试和显示
    /// </summary>
    public override string ToString()
    {
        return $"[{GetTierName()}] {jobName} - 薪资:{baseSalary}/5分钟 | 需求: {resourceRequirement}";
    }
    #endregion
}

/// <summary>
/// 玩家工作实例类
/// 存储玩家当前正在进行的工作的运行时信息
/// </summary>
[Serializable]
public class PlayerJobInstance
{
    /// <summary>
    /// 工作槽位ID（0-4，对应5个工作位）
    /// </summary>
    public int slotId;
    
    /// <summary>
    /// 关联的工作数据ID
    /// </summary>
    public string jobId;
    
    /// <summary>
    /// 工作开始时间戳
    /// </summary>
    public long startTimestamp;
    
    /// <summary>
    /// 已完成的周期数
    /// </summary>
    public int completedCycles = 0;
    
    /// <summary>
    /// 累计获得的薪资
    /// </summary>
    public int totalEarned = 0;
    
    /// <summary>
    /// 技能掌握度（百分比）
    /// 影响实际薪资：实际薪资 = 基础薪资 × (掌握度 / 100)
    /// </summary>
    public float skillMastery = 100f;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public PlayerJobInstance(int slotId, string jobId, long startTimestamp)
    {
        this.slotId = slotId;
        this.jobId = jobId;
        this.startTimestamp = startTimestamp;
        this.completedCycles = 0;
        this.totalEarned = 0;
        this.skillMastery = 100f;
    }
}
