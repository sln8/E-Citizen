using System;
using UnityEngine;

/// <summary>
/// 排行榜条目数据类
/// 用于存储排行榜中的玩家信息
/// 
/// 功能：
/// - 存储玩家排名数据
/// - 支持多种排行榜类型（财富、等级、心情、在线时长）
/// - 排名变化追踪
/// 
/// Unity操作步骤：
/// 1. 此文件会自动被Unity识别
/// 2. 无需手动操作，管理器会使用此数据类
/// </summary>
[Serializable]
public class LeaderboardEntryData
{
    #region 基本信息
    
    /// <summary>
    /// 玩家用户ID
    /// </summary>
    public string userId;
    
    /// <summary>
    /// 玩家名称
    /// </summary>
    public string playerName;
    
    /// <summary>
    /// 玩家等级
    /// </summary>
    public int playerLevel;
    
    /// <summary>
    /// 玩家头像URL
    /// </summary>
    public string avatarUrl;
    
    /// <summary>
    /// 当前排名
    /// </summary>
    public int currentRank;
    
    /// <summary>
    /// 上周排名（用于显示变化）
    /// </summary>
    public int lastWeekRank;
    
    #endregion
    
    #region 排行榜数值
    
    /// <summary>
    /// 财富榜：虚拟币总资产
    /// </summary>
    public int totalWealth;
    
    /// <summary>
    /// 等级榜：玩家等级
    /// </summary>
    public int level;
    
    /// <summary>
    /// 心情榜：当前心情值
    /// </summary>
    public int moodValue;
    
    /// <summary>
    /// 在线时长榜：累计在线分钟数
    /// </summary>
    public int totalOnlineMinutes;
    
    #endregion
    
    #region 额外信息
    
    /// <summary>
    /// 身份类型
    /// </summary>
    public IdentityType identityType;
    
    /// <summary>
    /// 是否是当前玩家
    /// </summary>
    public bool isCurrentPlayer;
    
    /// <summary>
    /// 数据更新时间
    /// </summary>
    public DateTime lastUpdateTime;
    
    #endregion
    
    #region 构造函数
    
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public LeaderboardEntryData()
    {
        userId = "";
        playerName = "未知玩家";
        playerLevel = 1;
        avatarUrl = "";
        currentRank = 0;
        lastWeekRank = 0;
        totalWealth = 0;
        level = 1;
        moodValue = 0;
        totalOnlineMinutes = 0;
        identityType = IdentityType.ConsciousnessLinker;
        isCurrentPlayer = false;
        lastUpdateTime = DateTime.Now;
    }
    
    /// <summary>
    /// 创建排行榜条目
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="name">玩家名称</param>
    /// <param name="level">等级</param>
    public LeaderboardEntryData(string userId, string name, int level)
    {
        this.userId = userId;
        this.playerName = name;
        this.playerLevel = level;
        this.avatarUrl = "";
        this.currentRank = 0;
        this.lastWeekRank = 0;
        this.totalWealth = 0;
        this.level = level;
        this.moodValue = 0;
        this.totalOnlineMinutes = 0;
        this.identityType = IdentityType.ConsciousnessLinker;
        this.isCurrentPlayer = false;
        this.lastUpdateTime = DateTime.Now;
    }
    
    #endregion
    
    #region 公共方法
    
    /// <summary>
    /// 获取排名变化
    /// </summary>
    /// <returns>排名变化值（正数表示上升，负数表示下降）</returns>
    public int GetRankChange()
    {
        if (lastWeekRank == 0) return 0;
        return lastWeekRank - currentRank;
    }
    
    /// <summary>
    /// 获取排名变化图标
    /// </summary>
    /// <returns>图标字符串（⬆、⬇、-）</returns>
    public string GetRankChangeIcon()
    {
        int change = GetRankChange();
        if (change > 0) return "⬆";
        if (change < 0) return "⬇";
        return "-";
    }
    
    /// <summary>
    /// 获取排名变化颜色
    /// </summary>
    /// <returns>Unity Color对象</returns>
    public Color GetRankChangeColor()
    {
        int change = GetRankChange();
        if (change > 0) return Color.green;
        if (change < 0) return Color.red;
        return Color.gray;
    }
    
    /// <summary>
    /// 获取排名显示文本（前3名显示奖牌）
    /// </summary>
    /// <returns>排名文本</returns>
    public string GetRankDisplay()
    {
        switch (currentRank)
        {
            case 1:
                return "🥇";
            case 2:
                return "🥈";
            case 3:
                return "🥉";
            default:
                return currentRank.ToString();
        }
    }
    
    /// <summary>
    /// 获取在线时长显示文本
    /// </summary>
    /// <returns>格式化的时长文本</returns>
    public string GetOnlineTimeDisplay()
    {
        if (totalOnlineMinutes < 60)
        {
            return $"{totalOnlineMinutes}分钟";
        }
        else if (totalOnlineMinutes < 1440) // 24小时
        {
            int hours = totalOnlineMinutes / 60;
            int minutes = totalOnlineMinutes % 60;
            return $"{hours}小时{minutes}分钟";
        }
        else
        {
            int days = totalOnlineMinutes / 1440;
            int hours = (totalOnlineMinutes % 1440) / 60;
            return $"{days}天{hours}小时";
        }
    }
    
    /// <summary>
    /// 更新排名数据
    /// </summary>
    /// <param name="newRank">新排名</param>
    public void UpdateRank(int newRank)
    {
        lastWeekRank = currentRank;
        currentRank = newRank;
        lastUpdateTime = DateTime.Now;
    }
    
    #endregion
}

/// <summary>
/// 排行榜类型枚举
/// </summary>
public enum LeaderboardType
{
    /// <summary>
    /// 财富榜（虚拟币总资产）
    /// </summary>
    Wealth,
    
    /// <summary>
    /// 等级榜（玩家等级）
    /// </summary>
    Level,
    
    /// <summary>
    /// 心情榜（当前心情值）
    /// </summary>
    Mood,
    
    /// <summary>
    /// 在线时长榜（累计在线时间）
    /// </summary>
    OnlineTime
}

/// <summary>
/// 排行榜配置数据类
/// 定义排行榜的规则和奖励
/// </summary>
[Serializable]
public class LeaderboardConfig
{
    /// <summary>
    /// 排行榜类型
    /// </summary>
    public LeaderboardType leaderboardType;
    
    /// <summary>
    /// 排行榜名称
    /// </summary>
    public string displayName;
    
    /// <summary>
    /// 排行榜描述
    /// </summary>
    public string description;
    
    /// <summary>
    /// 排行榜图标
    /// </summary>
    public string iconPath;
    
    /// <summary>
    /// 是否启用周奖励
    /// </summary>
    public bool hasWeeklyReward;
    
    /// <summary>
    /// 周奖励配置（JSON格式）
    /// </summary>
    public string weeklyRewardConfig;
    
    /// <summary>
    /// 是否启用月奖励
    /// </summary>
    public bool hasMonthlyReward;
    
    /// <summary>
    /// 月奖励配置（JSON格式）
    /// </summary>
    public string monthlyRewardConfig;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <param name="name">显示名称</param>
    /// <param name="desc">描述</param>
    public LeaderboardConfig(LeaderboardType type, string name, string desc)
    {
        this.leaderboardType = type;
        this.displayName = name;
        this.description = desc;
        this.iconPath = $"UI/Leaderboard/{type}";
        this.hasWeeklyReward = false;
        this.weeklyRewardConfig = "";
        this.hasMonthlyReward = false;
        this.monthlyRewardConfig = "";
    }
    
    /// <summary>
    /// 获取排行榜图标Unicode
    /// </summary>
    /// <returns>图标字符串</returns>
    public string GetIcon()
    {
        switch (leaderboardType)
        {
            case LeaderboardType.Wealth:
                return "💰";
            case LeaderboardType.Level:
                return "⭐";
            case LeaderboardType.Mood:
                return "❤️";
            case LeaderboardType.OnlineTime:
                return "⏰";
            default:
                return "📊";
        }
    }
    
    /// <summary>
    /// 创建默认排行榜配置
    /// </summary>
    /// <returns>配置数组</returns>
    public static LeaderboardConfig[] CreateDefaultConfigs()
    {
        return new LeaderboardConfig[]
        {
            new LeaderboardConfig(
                LeaderboardType.Wealth,
                "财富榜",
                "比拼虚拟币总资产，每周前10名获得额外奖励"
            )
            {
                hasWeeklyReward = true,
                weeklyRewardConfig = "{\"rank1\":1000,\"rank2\":800,\"rank3\":600,\"rank4-10\":400}"
            },
            
            new LeaderboardConfig(
                LeaderboardType.Level,
                "等级榜",
                "展示玩家等级排名，追求更高的成长"
            ),
            
            new LeaderboardConfig(
                LeaderboardType.Mood,
                "心情榜",
                "展示当前心情值最高的玩家"
            ),
            
            new LeaderboardConfig(
                LeaderboardType.OnlineTime,
                "在线时长榜",
                "累计在线时长排名，每月前10名获得专属称号"
            )
            {
                hasMonthlyReward = true,
                monthlyRewardConfig = "{\"rank1\":\"终极玩家\",\"rank2\":\"骨灰级玩家\",\"rank3\":\"核心玩家\",\"rank4-10\":\"活跃玩家\"}"
            }
        };
    }
}
