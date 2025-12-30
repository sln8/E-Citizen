using System;
using UnityEngine;

/// <summary>
/// 技能品级枚举
/// 定义技能的稀有度和价值等级
/// </summary>
public enum SkillTier
{
    Common,      // 普通 - 基础技能
    Rare,        // 精良 - 进阶技能
    Epic,        // 史诗 - 高级技能
    Legendary    // 传说 - 顶级技能
}

/// <summary>
/// 技能状态枚举
/// </summary>
public enum SkillStatus
{
    Locked,      // 未解锁
    Available,   // 可购买
    Downloading, // 下载中
    Installed    // 已安装
}

/// <summary>
/// 技能数据类
/// 存储单个技能的所有信息
/// 包括技能ID、名称、价格、文件大小、算力需求等
/// </summary>
[Serializable]
public class SkillData
{
    #region 基础信息
    /// <summary>
    /// 技能唯一ID
    /// 格式：dataClean_lv1, aiTraining_lv2 等
    /// </summary>
    [Header("基础信息")]
    [Tooltip("技能唯一标识符")]
    public string skillId;
    
    /// <summary>
    /// 技能名称
    /// 例如：数据清理 Lv.1、AI训练 Lv.2
    /// </summary>
    [Tooltip("技能名称")]
    public string skillName;
    
    /// <summary>
    /// 技能描述
    /// 介绍技能的功能和用途
    /// </summary>
    [Tooltip("技能描述")]
    [TextArea(2, 4)]
    public string skillDescription;
    
    /// <summary>
    /// 技能品级
    /// 决定技能的稀有度和价格范围
    /// </summary>
    [Tooltip("技能品级")]
    public SkillTier skillTier = SkillTier.Common;
    
    /// <summary>
    /// 技能等级
    /// 同一技能可以有多个等级（Lv.1, Lv.2, Lv.3 等）
    /// </summary>
    [Tooltip("技能等级")]
    public int skillLevel = 1;
    #endregion
    
    #region 购买信息
    /// <summary>
    /// 技能价格（虚拟币）
    /// 购买并下载技能所需的费用
    /// </summary>
    [Header("购买信息")]
    [Tooltip("购买价格（虚拟币）")]
    public int price = 50;
    
    /// <summary>
    /// 技能文件大小（GB）
    /// 决定下载所需的时间
    /// </summary>
    [Tooltip("技能文件大小（GB）")]
    public float fileSize = 1f;
    #endregion
    
    #region 掌握度配置
    /// <summary>
    /// 达到100%掌握度所需的最大算力
    /// 当分配的算力达到此值时，掌握度为100%
    /// </summary>
    [Header("掌握度配置")]
    [Tooltip("达到100%掌握度所需的算力")]
    public float maxComputingFor100Percent = 10f;
    
    /// <summary>
    /// 达到200%掌握度所需的最大算力
    /// 当分配的算力达到此值时，掌握度为200%（最高）
    /// </summary>
    [Tooltip("达到200%掌握度所需的算力")]
    public float maxComputingFor200Percent = 30f;
    #endregion
    
    #region 解锁条件
    /// <summary>
    /// 解锁等级
    /// 玩家达到此等级后才能购买这个技能
    /// </summary>
    [Header("解锁条件")]
    [Tooltip("解锁等级")]
    public int unlockLevel = 1;
    
    /// <summary>
    /// 前置技能ID
    /// 需要先学习前置技能才能解锁此技能
    /// 为空表示没有前置要求
    /// </summary>
    [Tooltip("前置技能ID（可选）")]
    public string prerequisiteSkillId = "";
    #endregion
    
    #region 运行时状态
    /// <summary>
    /// 技能当前状态
    /// 运行时动态更新，不序列化到数据库
    /// </summary>
    [NonSerialized]
    public SkillStatus status = SkillStatus.Locked;
    
    /// <summary>
    /// 下载进度（0-100）
    /// 仅在下载中状态有效
    /// </summary>
    [NonSerialized]
    public float downloadProgress = 0f;
    #endregion
    
    #region 方法
    /// <summary>
    /// 构造函数 - 创建默认技能数据
    /// </summary>
    public SkillData()
    {
        skillId = "skill_default";
        skillName = "未命名技能";
        skillDescription = "技能描述";
        skillTier = SkillTier.Common;
        skillLevel = 1;
        price = 50;
        fileSize = 1f;
        maxComputingFor100Percent = 10f;
        maxComputingFor200Percent = 30f;
        unlockLevel = 1;
        prerequisiteSkillId = "";
    }
    
    /// <summary>
    /// 获取技能品级的中文名称
    /// </summary>
    public string GetTierName()
    {
        switch (skillTier)
        {
            case SkillTier.Common: return "普通";
            case SkillTier.Rare: return "精良";
            case SkillTier.Epic: return "史诗";
            case SkillTier.Legendary: return "传说";
            default: return "未知";
        }
    }
    
    /// <summary>
    /// 获取技能品级的颜色
    /// 用于UI显示
    /// </summary>
    public Color GetTierColor()
    {
        switch (skillTier)
        {
            case SkillTier.Common: return Color.white;          // 普通 - 白色
            case SkillTier.Rare: return Color.blue;             // 精良 - 蓝色
            case SkillTier.Epic: return new Color(0.5f, 0f, 1f); // 史诗 - 紫色
            case SkillTier.Legendary: return Color.yellow;      // 传说 - 金色
            default: return Color.white;
        }
    }
    
    /// <summary>
    /// 计算下载所需的时间（秒）
    /// 公式：下载时间 = 文件大小 / (网速 / 1000)
    /// </summary>
    /// <param name="bandwidth">玩家的可用网速（Mbps）</param>
    /// <returns>下载所需的秒数</returns>
    public float CalculateDownloadTime(float bandwidth)
    {
        if (bandwidth <= 0)
        {
            return float.MaxValue; // 网速为0，无法下载
        }
        
        // 文件大小（GB）转换为 Mb：1GB = 1024MB = 1024*8 Mb
        float fileSizeInMb = fileSize * 1024f * 8f;
        
        // 下载时间（秒）= 文件大小（Mb）/ 网速（Mbps）
        float downloadTime = fileSizeInMb / bandwidth;
        
        return downloadTime;
    }
    
    /// <summary>
    /// 计算基于算力分配的掌握度
    /// 根据分配的算力计算技能掌握度（20% - 200%）
    /// </summary>
    /// <param name="allocatedComputing">分配的算力</param>
    /// <returns>计算出的掌握度百分比</returns>
    public float CalculateMastery(float allocatedComputing)
    {
        return CalculateMasteryFromComputing(allocatedComputing, maxComputingFor100Percent, maxComputingFor200Percent);
    }
    
    /// <summary>
    /// 计算掌握度的通用方法（静态）
    /// 根据分配的算力和技能配置计算掌握度
    /// </summary>
    /// <param name="allocatedComputing">分配的算力</param>
    /// <param name="maxFor100">达到100%掌握度所需的算力</param>
    /// <param name="maxFor200">达到200%掌握度所需的算力</param>
    /// <returns>计算出的掌握度百分比（20%-200%）</returns>
    public static float CalculateMasteryFromComputing(float allocatedComputing, float maxFor100, float maxFor200)
    {
        // 基础掌握度20%
        float baseMastery = 20f;
        
        // 根据分配的算力计算额外掌握度
        float additionalMastery = 0f;
        
        if (allocatedComputing > 0)
        {
            if (allocatedComputing <= maxFor100)
            {
                // 未达到100%：20% + (算力 / 100%所需) × 80%
                additionalMastery = (allocatedComputing / maxFor100) * 80f;
            }
            else
            {
                // 超过100%：100% + (超出算力 / 额外所需) × 100%
                float excessComputing = allocatedComputing - maxFor100;
                float maxExcessComputing = maxFor200 - maxFor100;
                
                additionalMastery = 80f; // 先加上80%达到100%
                additionalMastery += Mathf.Min(100f, (excessComputing / maxExcessComputing) * 100f);
            }
        }
        
        // 最终掌握度 = 基础 + 额外，限制在20%-200%之间
        return Mathf.Clamp(baseMastery + additionalMastery, 20f, 200f);
    }
    
    /// <summary>
    /// 获取技能的详细信息字符串
    /// 用于调试和显示
    /// </summary>
    public override string ToString()
    {
        return $"[{GetTierName()}] {skillName} - 价格:{price}币 | 大小:{fileSize}GB";
    }
    #endregion
}

/// <summary>
/// 玩家技能实例类
/// 存储玩家已拥有技能的运行时信息
/// </summary>
[Serializable]
public class PlayerSkillInstance
{
    /// <summary>
    /// 技能ID
    /// </summary>
    public string skillId;
    
    /// <summary>
    /// 获得技能的时间戳
    /// </summary>
    public long acquiredTimestamp;
    
    /// <summary>
    /// 当前分配的算力
    /// 决定技能的掌握度
    /// </summary>
    public float allocatedComputing = 0f;
    
    /// <summary>
    /// 当前掌握度（20% - 200%）
    /// 计算公式：
    /// - 最低掌握度：20%（刚购买时）
    /// - 掌握度 = 20% + (当前算力 / 达到100%所需算力) × 80%
    /// - 最高掌握度：200%
    /// </summary>
    public float masteryPercent = 20f;
    
    /// <summary>
    /// 是否正在使用（被某个工作使用中）
    /// </summary>
    public bool isInUse = false;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public PlayerSkillInstance(string skillId, long acquiredTimestamp)
    {
        this.skillId = skillId;
        this.acquiredTimestamp = acquiredTimestamp;
        this.allocatedComputing = 0f;
        this.masteryPercent = 20f; // 初始掌握度为20%
        this.isInUse = false;
    }
    
    /// <summary>
    /// 计算掌握度
    /// 根据分配的算力和技能配置计算当前掌握度
    /// </summary>
    /// <param name="skillData">技能配置数据</param>
    public void CalculateMastery(SkillData skillData)
    {
        if (skillData == null)
        {
            masteryPercent = 20f;
            return;
        }
        
        // 使用SkillData的静态方法计算掌握度
        masteryPercent = SkillData.CalculateMasteryFromComputing(
            allocatedComputing, 
            skillData.maxComputingFor100Percent, 
            skillData.maxComputingFor200Percent
        );
    }
}
