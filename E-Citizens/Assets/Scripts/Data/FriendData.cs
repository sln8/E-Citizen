using System;
using UnityEngine;

/// <summary>
/// 好友数据类
/// 用于存储好友信息和在线状态
/// 
/// 功能：
/// - 存储好友基本信息（ID、名字、等级等）
/// - 追踪在线状态
/// - 记录好友关系时间
/// 
/// Unity操作步骤：
/// 1. 此文件会自动被Unity识别
/// 2. 无需手动操作，管理器会使用此数据类
/// </summary>
[Serializable]
public class FriendData
{
    #region 基本信息
    
    /// <summary>
    /// 好友的用户ID
    /// </summary>
    public string friendUserId;
    
    /// <summary>
    /// 好友的显示名称
    /// </summary>
    public string friendName;
    
    /// <summary>
    /// 好友的等级
    /// </summary>
    public int friendLevel;
    
    /// <summary>
    /// 好友的身份类型（意识连接者/完全虚拟人）
    /// </summary>
    public IdentityType identityType;
    
    /// <summary>
    /// 好友的头像URL（可选）
    /// </summary>
    public string avatarUrl;
    
    #endregion
    
    #region 状态信息
    
    /// <summary>
    /// 好友是否在线
    /// </summary>
    public bool isOnline;
    
    /// <summary>
    /// 好友最后在线时间
    /// </summary>
    public DateTime lastOnlineTime;
    
    /// <summary>
    /// 成为好友的时间
    /// </summary>
    public DateTime friendsSinceTime;
    
    #endregion
    
    #region 游戏数据（可选，用于显示）
    
    /// <summary>
    /// 好友的虚拟币数量（仅显示用）
    /// </summary>
    public int virtualCoin;
    
    /// <summary>
    /// 好友的心情值（仅显示用）
    /// </summary>
    public int moodValue;
    
    /// <summary>
    /// 好友当前房产类型（仅显示用）
    /// </summary>
    public string currentHousing;
    
    #endregion
    
    #region 构造函数
    
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public FriendData()
    {
        friendUserId = "";
        friendName = "未知玩家";
        friendLevel = 1;
        identityType = IdentityType.ConsciousnessLinker;
        avatarUrl = "";
        isOnline = false;
        lastOnlineTime = DateTime.Now;
        friendsSinceTime = DateTime.Now;
        virtualCoin = 0;
        moodValue = 0;
        currentHousing = "胶囊公寓";
    }
    
    /// <summary>
    /// 从用户ID创建好友数据
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="name">用户名</param>
    /// <param name="level">等级</param>
    public FriendData(string userId, string name, int level)
    {
        this.friendUserId = userId;
        this.friendName = name;
        this.friendLevel = level;
        this.identityType = IdentityType.ConsciousnessLinker;
        this.avatarUrl = "";
        this.isOnline = true;
        this.lastOnlineTime = DateTime.Now;
        this.friendsSinceTime = DateTime.Now;
        this.virtualCoin = 0;
        this.moodValue = 0;
        this.currentHousing = "胶囊公寓";
    }
    
    #endregion
    
    #region 公共方法
    
    /// <summary>
    /// 更新在线状态
    /// </summary>
    /// <param name="online">是否在线</param>
    public void UpdateOnlineStatus(bool online)
    {
        isOnline = online;
        if (online)
        {
            lastOnlineTime = DateTime.Now;
        }
    }
    
    /// <summary>
    /// 获取最后在线时间的描述文本
    /// </summary>
    /// <returns>时间描述（如：5分钟前、1小时前）</returns>
    public string GetLastOnlineDescription()
    {
        if (isOnline)
        {
            return "在线";
        }
        
        TimeSpan timeSince = DateTime.Now - lastOnlineTime;
        
        if (timeSince.TotalMinutes < 1)
        {
            return "刚刚";
        }
        else if (timeSince.TotalMinutes < 60)
        {
            return $"{(int)timeSince.TotalMinutes}分钟前";
        }
        else if (timeSince.TotalHours < 24)
        {
            return $"{(int)timeSince.TotalHours}小时前";
        }
        else if (timeSince.TotalDays < 7)
        {
            return $"{(int)timeSince.TotalDays}天前";
        }
        else
        {
            return lastOnlineTime.ToString("yyyy-MM-dd");
        }
    }
    
    /// <summary>
    /// 获取好友关系持续时间描述
    /// </summary>
    /// <returns>时间描述（如：好友1天、好友3个月）</returns>
    public string GetFriendshipDuration()
    {
        TimeSpan duration = DateTime.Now - friendsSinceTime;
        
        if (duration.TotalDays < 1)
        {
            return "新好友";
        }
        else if (duration.TotalDays < 30)
        {
            return $"好友{(int)duration.TotalDays}天";
        }
        else if (duration.TotalDays < 365)
        {
            return $"好友{(int)(duration.TotalDays / 30)}个月";
        }
        else
        {
            return $"好友{(int)(duration.TotalDays / 365)}年";
        }
    }
    
    /// <summary>
    /// 更新好友的游戏数据（从Firestore同步）
    /// </summary>
    /// <param name="level">等级</param>
    /// <param name="coin">虚拟币</param>
    /// <param name="mood">心情值</param>
    /// <param name="housing">房产</param>
    public void UpdateGameData(int level, int coin, int mood, string housing)
    {
        this.friendLevel = level;
        this.virtualCoin = coin;
        this.moodValue = mood;
        this.currentHousing = housing;
    }
    
    /// <summary>
    /// 创建用于显示的简短描述
    /// </summary>
    /// <returns>好友信息摘要</returns>
    public string GetDisplaySummary()
    {
        return $"{friendName} Lv.{friendLevel} | {(isOnline ? "在线" : GetLastOnlineDescription())}";
    }
    
    #endregion
}

/// <summary>
/// 好友请求数据类
/// 用于处理添加好友的请求
/// </summary>
[Serializable]
public class FriendRequestData
{
    /// <summary>
    /// 请求ID
    /// </summary>
    public string requestId;
    
    /// <summary>
    /// 发送者用户ID
    /// </summary>
    public string senderUserId;
    
    /// <summary>
    /// 发送者名称
    /// </summary>
    public string senderName;
    
    /// <summary>
    /// 发送者等级
    /// </summary>
    public int senderLevel;
    
    /// <summary>
    /// 接收者用户ID
    /// </summary>
    public string receiverUserId;
    
    /// <summary>
    /// 请求状态（pending、accepted、rejected）
    /// </summary>
    public string status;
    
    /// <summary>
    /// 请求发送时间
    /// </summary>
    public DateTime requestTime;
    
    /// <summary>
    /// 附加消息（可选）
    /// </summary>
    public string message;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public FriendRequestData()
    {
        requestId = Guid.NewGuid().ToString();
        senderUserId = "";
        senderName = "";
        senderLevel = 1;
        receiverUserId = "";
        status = "pending";
        requestTime = DateTime.Now;
        message = "";
    }
    
    /// <summary>
    /// 创建好友请求
    /// </summary>
    /// <param name="senderId">发送者ID</param>
    /// <param name="senderName">发送者名称</param>
    /// <param name="senderLevel">发送者等级</param>
    /// <param name="receiverId">接收者ID</param>
    /// <param name="msg">附加消息</param>
    public FriendRequestData(string senderId, string senderName, int senderLevel, string receiverId, string msg = "")
    {
        this.requestId = Guid.NewGuid().ToString();
        this.senderUserId = senderId;
        this.senderName = senderName;
        this.senderLevel = senderLevel;
        this.receiverUserId = receiverId;
        this.status = "pending";
        this.requestTime = DateTime.Now;
        this.message = msg;
    }
    
    /// <summary>
    /// 接受好友请求
    /// </summary>
    public void Accept()
    {
        status = "accepted";
    }
    
    /// <summary>
    /// 拒绝好友请求
    /// </summary>
    public void Reject()
    {
        status = "rejected";
    }
}
