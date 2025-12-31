using System;
using UnityEngine;

/// <summary>
/// 邮件数据类
/// 用于游戏内邮箱系统
/// 
/// 功能：
/// - 系统通知邮件
/// - 工资发放通知
/// - 好友礼物通知
/// - 任务奖励邮件
/// 
/// Unity操作步骤：
/// 1. 此文件会自动被Unity识别
/// 2. 无需手动操作，管理器会使用此数据类
/// </summary>
[Serializable]
public class MailData
{
    #region 基本属性
    
    /// <summary>
    /// 邮件ID
    /// </summary>
    public string mailId;
    
    /// <summary>
    /// 邮件类型（system、salary、gift、reward、friend）
    /// </summary>
    public string mailType;
    
    /// <summary>
    /// 邮件标题
    /// </summary>
    public string title;
    
    /// <summary>
    /// 邮件内容
    /// </summary>
    public string content;
    
    /// <summary>
    /// 发送者ID（系统邮件为空）
    /// </summary>
    public string senderId;
    
    /// <summary>
    /// 发送者名称（系统邮件为"系统"）
    /// </summary>
    public string senderName;
    
    /// <summary>
    /// 接收者ID
    /// </summary>
    public string receiverId;
    
    /// <summary>
    /// 发送时间
    /// </summary>
    public DateTime sentTime;
    
    /// <summary>
    /// 是否已读
    /// </summary>
    public bool isRead;
    
    /// <summary>
    /// 是否已领取附件
    /// </summary>
    public bool isClaimed;
    
    #endregion
    
    #region 附件内容
    
    /// <summary>
    /// 附件：虚拟币
    /// </summary>
    public int attachedVirtualCoin;
    
    /// <summary>
    /// 附件：心情值
    /// </summary>
    public int attachedMoodValue;
    
    /// <summary>
    /// 附件：物品ID列表（JSON格式）
    /// </summary>
    public string attachedItems;
    
    #endregion
    
    #region 构造函数
    
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public MailData()
    {
        mailId = Guid.NewGuid().ToString();
        mailType = "system";
        title = "";
        content = "";
        senderId = "";
        senderName = "系统";
        receiverId = "";
        sentTime = DateTime.Now;
        isRead = false;
        isClaimed = false;
        attachedVirtualCoin = 0;
        attachedMoodValue = 0;
        attachedItems = "";
    }
    
    /// <summary>
    /// 创建邮件
    /// </summary>
    /// <param name="type">邮件类型</param>
    /// <param name="title">标题</param>
    /// <param name="content">内容</param>
    /// <param name="receiverId">接收者ID</param>
    public MailData(string type, string title, string content, string receiverId)
    {
        this.mailId = Guid.NewGuid().ToString();
        this.mailType = type;
        this.title = title;
        this.content = content;
        this.senderId = "";
        this.senderName = "系统";
        this.receiverId = receiverId;
        this.sentTime = DateTime.Now;
        this.isRead = false;
        this.isClaimed = false;
        this.attachedVirtualCoin = 0;
        this.attachedMoodValue = 0;
        this.attachedItems = "";
    }
    
    #endregion
    
    #region 公共方法
    
    /// <summary>
    /// 标记为已读
    /// </summary>
    public void MarkAsRead()
    {
        isRead = true;
    }
    
    /// <summary>
    /// 领取附件
    /// </summary>
    /// <returns>是否成功领取</returns>
    public bool ClaimAttachments()
    {
        if (isClaimed)
        {
            Debug.LogWarning($"邮件 {mailId} 的附件已经领取过了");
            return false;
        }
        
        if (!HasAttachments())
        {
            Debug.LogWarning($"邮件 {mailId} 没有附件可领取");
            return false;
        }
        
        isRead = true;
        isClaimed = true;
        return true;
    }
    
    /// <summary>
    /// 检查是否有附件
    /// </summary>
    /// <returns>是否有附件</returns>
    public bool HasAttachments()
    {
        return attachedVirtualCoin > 0 || 
               attachedMoodValue > 0 || 
               !string.IsNullOrEmpty(attachedItems);
    }
    
    /// <summary>
    /// 获取邮件类型的中文名称
    /// </summary>
    /// <returns>类型名称</returns>
    public string GetTypeName()
    {
        switch (mailType)
        {
            case "system":
                return "系统通知";
            case "salary":
                return "工资发放";
            case "gift":
                return "好友礼物";
            case "reward":
                return "任务奖励";
            case "friend":
                return "好友消息";
            default:
                return "未知";
        }
    }
    
    /// <summary>
    /// 获取邮件图标（用于UI显示）
    /// </summary>
    /// <returns>图标Unicode字符</returns>
    public string GetIcon()
    {
        switch (mailType)
        {
            case "system":
                return "📢";
            case "salary":
                return "💰";
            case "gift":
                return "🎁";
            case "reward":
                return "🏆";
            case "friend":
                return "👤";
            default:
                return "✉";
        }
    }
    
    /// <summary>
    /// 获取发送时间的描述文本
    /// </summary>
    /// <returns>时间描述</returns>
    public string GetSentTimeDescription()
    {
        TimeSpan timeSince = DateTime.Now - sentTime;
        
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
            return sentTime.ToString("yyyy-MM-dd");
        }
    }
    
    /// <summary>
    /// 获取附件摘要文本
    /// </summary>
    /// <returns>附件描述</returns>
    public string GetAttachmentSummary()
    {
        if (!HasAttachments())
        {
            return "无附件";
        }
        
        string summary = "附件: ";
        
        if (attachedVirtualCoin > 0)
        {
            summary += $"{attachedVirtualCoin}币 ";
        }
        
        if (attachedMoodValue > 0)
        {
            summary += $"心情+{attachedMoodValue} ";
        }
        
        if (!string.IsNullOrEmpty(attachedItems))
        {
            summary += "道具 ";
        }
        
        return summary.Trim();
    }
    
    /// <summary>
    /// 获取邮件简短摘要（用于列表显示）
    /// </summary>
    /// <returns>邮件摘要</returns>
    public string GetSummary()
    {
        string readStatus = isRead ? "" : "[新] ";
        return $"{GetIcon()} {readStatus}{title}\n{GetSentTimeDescription()}";
    }
    
    #endregion
    
    #region 静态工厂方法
    
    /// <summary>
    /// 创建系统通知邮件
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="content">内容</param>
    /// <param name="receiverId">接收者ID</param>
    /// <returns>邮件数据</returns>
    public static MailData CreateSystemMail(string title, string content, string receiverId)
    {
        return new MailData("system", title, content, receiverId);
    }
    
    /// <summary>
    /// 创建工资发放邮件
    /// </summary>
    /// <param name="companyName">公司名称</param>
    /// <param name="amount">工资金额</param>
    /// <param name="receiverId">接收者ID</param>
    /// <returns>邮件数据</returns>
    public static MailData CreateSalaryMail(string companyName, int amount, string receiverId)
    {
        MailData mail = new MailData(
            "salary",
            "工资发放",
            $"您在【{companyName}】工作获得工资: {amount}币",
            receiverId
        );
        mail.attachedVirtualCoin = amount;
        return mail;
    }
    
    /// <summary>
    /// 创建好友礼物邮件
    /// </summary>
    /// <param name="senderName">发送者名称</param>
    /// <param name="giftName">礼物名称</param>
    /// <param name="moodBonus">心情值</param>
    /// <param name="message">附加消息</param>
    /// <param name="senderId">发送者ID</param>
    /// <param name="receiverId">接收者ID</param>
    /// <returns>邮件数据</returns>
    public static MailData CreateGiftMail(string senderName, string giftName, int moodBonus, 
                                          string message, string senderId, string receiverId)
    {
        string content = $"{senderName} 赠送了【{giftName}】给你！\n" +
                        (string.IsNullOrEmpty(message) ? "" : $"\n附言: {message}");
        
        MailData mail = new MailData(
            "gift",
            "好友礼物",
            content,
            receiverId
        );
        mail.senderId = senderId;
        mail.senderName = senderName;
        mail.attachedMoodValue = moodBonus;
        return mail;
    }
    
    /// <summary>
    /// 创建任务奖励邮件
    /// </summary>
    /// <param name="questName">任务名称</param>
    /// <param name="coinReward">虚拟币奖励</param>
    /// <param name="items">物品奖励（JSON）</param>
    /// <param name="receiverId">接收者ID</param>
    /// <returns>邮件数据</returns>
    public static MailData CreateRewardMail(string questName, int coinReward, string items, string receiverId)
    {
        MailData mail = new MailData(
            "reward",
            "任务奖励",
            $"完成任务: 【{questName}】\n恭喜你获得丰厚奖励！",
            receiverId
        );
        mail.attachedVirtualCoin = coinReward;
        mail.attachedItems = items;
        return mail;
    }
    
    /// <summary>
    /// 创建好友消息邮件
    /// </summary>
    /// <param name="senderName">发送者名称</param>
    /// <param name="message">消息内容</param>
    /// <param name="senderId">发送者ID</param>
    /// <param name="receiverId">接收者ID</param>
    /// <returns>邮件数据</returns>
    public static MailData CreateFriendMessageMail(string senderName, string message, 
                                                    string senderId, string receiverId)
    {
        MailData mail = new MailData(
            "friend",
            $"来自 {senderName} 的消息",
            message,
            receiverId
        );
        mail.senderId = senderId;
        mail.senderName = senderName;
        return mail;
    }
    
    #endregion
}
