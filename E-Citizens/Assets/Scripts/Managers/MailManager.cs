using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 邮箱系统管理器
/// 负责管理游戏内邮件功能
/// 
/// 核心功能：
/// 1. 接收系统邮件
/// 2. 接收好友邮件
/// 3. 处理工资、奖励等附件
/// 4. 邮件已读/未读管理
/// 5. 邮件批量操作
/// 
/// Unity操作步骤：
/// 1. 在Hierarchy中创建空物体，命名为"MailManager"
/// 2. 添加此脚本到该物体上
/// 3. 脚本会自动初始化为单例
/// 4. 在游戏开始时调用Initialize()方法
/// 
/// 使用示例：
/// MailManager.Instance.SendSystemMail("欢迎", "欢迎来到电子公民！", player_id);
/// MailManager.Instance.ClaimMailReward(mail_id);
/// </summary>
public class MailManager : MonoBehaviour
{
    #region 单例模式
    
    private static MailManager instance;
    public static MailManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("MailManager");
                instance = go.AddComponent<MailManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    #endregion
    
    #region 数据存储
    
    /// <summary>
    /// 收件箱（所有邮件）
    /// </summary>
    private List<MailData> inbox = new List<MailData>();
    
    /// <summary>
    /// 最大邮件保存数量
    /// </summary>
    [SerializeField]
    private int maxMailCount = 200;
    
    /// <summary>
    /// 邮件自动删除天数（超过此天数的已读邮件自动删除）
    /// </summary>
    [SerializeField]
    private int autoDeleteDays = 30;
    
    /// <summary>
    /// 是否启用调试模式
    /// </summary>
    [SerializeField]
    private bool debugMode = false;
    
    #endregion
    
    #region 统计数据
    
    /// <summary>
    /// 统计：收到的邮件总数
    /// </summary>
    public int TotalMailsReceived { get; private set; }
    
    /// <summary>
    /// 统计：已读邮件数
    /// </summary>
    public int TotalMailsRead { get; private set; }
    
    /// <summary>
    /// 统计：领取的虚拟币总数（来自邮件）
    /// </summary>
    public int TotalCoinsFromMail { get; private set; }
    
    /// <summary>
    /// 统计：删除的邮件数
    /// </summary>
    public int TotalMailsDeleted { get; private set; }
    
    #endregion
    
    #region 事件
    
    /// <summary>
    /// 事件：收到新邮件（邮件类型，标题）
    /// </summary>
    public event Action<string, string> OnNewMailReceived;
    
    /// <summary>
    /// 事件：邮件列表更新
    /// </summary>
    public event Action OnMailListUpdated;
    
    /// <summary>
    /// 事件：邮件已读
    /// </summary>
    public event Action<string> OnMailRead;
    
    /// <summary>
    /// 事件：附件已领取（虚拟币数量，心情值）
    /// </summary>
    public event Action<int, int> OnAttachmentClaimed;
    
    #endregion
    
    #region Unity生命周期
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Update()
    {
        // 定期检查邮件自动删除（每小时检查一次）
        if (Time.frameCount % (60 * 60 * 60) == 0) // 约每小时
        {
            AutoDeleteOldMails();
        }
    }
    
    #endregion
    
    #region 初始化
    
    /// <summary>
    /// 初始化邮箱系统
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[MailManager] 初始化邮箱系统");
        
        // 加载数据（从Firebase或本地）
        LoadData();
        
        // 发送欢迎邮件（如果是新玩家）
        if (inbox.Count == 0)
        {
            SendWelcomeMail();
        }
        
        Debug.Log($"[MailManager] 初始化完成，当前邮件数：{inbox.Count}，未读：{GetUnreadCount()}");
    }
    
    /// <summary>
    /// 发送欢迎邮件
    /// </summary>
    private void SendWelcomeMail()
    {
        MailData welcomeMail = MailData.CreateSystemMail(
            "欢迎来到电子公民世界！",
            "亲爱的数字生命体：\n\n" +
            "欢迎来到3026年的虚拟世界！在这里，你将体验全新的数字生活。\n\n" +
            "工作、创业、社交，一切皆有可能。记得管理好你的资源，保持良好的心情值！\n\n" +
            "祝你在虚拟世界中找到属于自己的意义。\n\n" +
            "——系统管理员 AI-9527",
            AuthenticationManager.Instance.currentUser.userId
        );
        
        welcomeMail.attachedVirtualCoin = 100; // 新手奖励
        
        ReceiveMail(welcomeMail);
    }
    
    #endregion
    
    #region 发送邮件
    
    /// <summary>
    /// 发送系统通知邮件
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="content">内容</param>
    /// <param name="receiverId">接收者ID</param>
    /// <returns>邮件ID</returns>
    public string SendSystemMail(string title, string content, string receiverId)
    {
        MailData mail = MailData.CreateSystemMail(title, content, receiverId);
        
        // 如果是发给自己，直接加入收件箱
        if (receiverId == AuthenticationManager.Instance.currentUser.userId)
        {
            ReceiveMail(mail);
        }
        else
        {
            // TODO: 通过Firebase发送给其他玩家
            // FirebaseManager.SendMail(mail);
        }
        
        return mail.mailId;
    }
    
    /// <summary>
    /// 发送工资邮件
    /// </summary>
    /// <param name="companyName">公司名称</param>
    /// <param name="amount">工资金额</param>
    /// <param name="receiverId">接收者ID</param>
    public void SendSalaryMail(string companyName, int amount, string receiverId)
    {
        MailData mail = MailData.CreateSalaryMail(companyName, amount, receiverId);
        
        if (receiverId == AuthenticationManager.Instance.currentUser.userId)
        {
            ReceiveMail(mail);
        }
        else
        {
            // TODO: 通过Firebase发送
        }
    }
    
    /// <summary>
    /// 发送任务奖励邮件
    /// </summary>
    /// <param name="questName">任务名称</param>
    /// <param name="coinReward">虚拟币奖励</param>
    /// <param name="items">物品奖励JSON</param>
    /// <param name="receiverId">接收者ID</param>
    public void SendRewardMail(string questName, int coinReward, string items, string receiverId)
    {
        MailData mail = MailData.CreateRewardMail(questName, coinReward, items, receiverId);
        
        if (receiverId == AuthenticationManager.Instance.currentUser.userId)
        {
            ReceiveMail(mail);
        }
        else
        {
            // TODO: 通过Firebase发送
        }
    }
    
    #endregion
    
    #region 接收邮件
    
    /// <summary>
    /// 接收邮件（添加到收件箱）
    /// </summary>
    /// <param name="mail">邮件数据</param>
    public void ReceiveMail(MailData mail)
    {
        // 检查邮件数量限制
        if (inbox.Count >= maxMailCount)
        {
            // 删除最旧的已读邮件
            MailData oldestRead = inbox
                .Where(m => m.isRead && m.isClaimed)
                .OrderBy(m => m.sentTime)
                .FirstOrDefault();
            
            if (oldestRead != null)
            {
                inbox.Remove(oldestRead);
                TotalMailsDeleted++;
            }
        }
        
        inbox.Add(mail);
        TotalMailsReceived++;
        
        // 触发事件
        OnNewMailReceived?.Invoke(mail.mailType, mail.title);
        OnMailListUpdated?.Invoke();
        
        Debug.Log($"[MailManager] 收到新邮件：{mail.GetIcon()} {mail.title}");
    }
    
    #endregion
    
    #region 邮件操作
    
    /// <summary>
    /// 标记邮件为已读
    /// </summary>
    /// <param name="mailId">邮件ID</param>
    /// <returns>是否成功</returns>
    public bool MarkAsRead(string mailId)
    {
        MailData mail = inbox.FirstOrDefault(m => m.mailId == mailId);
        
        if (mail == null)
        {
            Debug.LogWarning($"[MailManager] 未找到邮件 {mailId}");
            return false;
        }
        
        if (mail.isRead)
        {
            return true; // 已经是已读状态
        }
        
        mail.MarkAsRead();
        TotalMailsRead++;
        
        OnMailRead?.Invoke(mailId);
        OnMailListUpdated?.Invoke();
        
        return true;
    }
    
    /// <summary>
    /// 领取邮件附件
    /// </summary>
    /// <param name="mailId">邮件ID</param>
    /// <returns>是否成功领取</returns>
    public bool ClaimMailReward(string mailId)
    {
        MailData mail = inbox.FirstOrDefault(m => m.mailId == mailId);
        
        if (mail == null)
        {
            Debug.LogWarning($"[MailManager] 未找到邮件 {mailId}");
            return false;
        }
        
        if (!mail.ClaimAttachments())
        {
            return false;
        }
        
        // 发放奖励
        if (mail.attachedVirtualCoin > 0)
        {
            ResourceManager.Instance.AddVirtualCoin(mail.attachedVirtualCoin);
            TotalCoinsFromMail += mail.attachedVirtualCoin;
        }
        
        if (mail.attachedMoodValue > 0)
        {
            ResourceManager.Instance.AddMoodValue(mail.attachedMoodValue);
        }
        
        // TODO: 处理物品附件
        if (!string.IsNullOrEmpty(mail.attachedItems))
        {
            // 解析JSON并添加物品到背包
        }
        
        // 触发事件
        OnAttachmentClaimed?.Invoke(mail.attachedVirtualCoin, mail.attachedMoodValue);
        OnMailListUpdated?.Invoke();
        
        Debug.Log($"[MailManager] 领取邮件附件：{mail.attachedVirtualCoin}币，{mail.attachedMoodValue}心情");
        return true;
    }
    
    /// <summary>
    /// 删除邮件
    /// </summary>
    /// <param name="mailId">邮件ID</param>
    /// <returns>是否成功删除</returns>
    public bool DeleteMail(string mailId)
    {
        MailData mail = inbox.FirstOrDefault(m => m.mailId == mailId);
        
        if (mail == null)
        {
            Debug.LogWarning($"[MailManager] 未找到邮件 {mailId}");
            return false;
        }
        
        // 如果有未领取的附件，提示警告
        if (mail.HasAttachments() && !mail.isClaimed)
        {
            Debug.LogWarning($"[MailManager] 邮件 {mailId} 有未领取的附件");
            return false;
        }
        
        inbox.Remove(mail);
        TotalMailsDeleted++;
        
        OnMailListUpdated?.Invoke();
        
        Debug.Log($"[MailManager] 删除邮件：{mail.title}");
        return true;
    }
    
    /// <summary>
    /// 批量领取所有附件
    /// </summary>
    /// <returns>领取的邮件数量</returns>
    public int ClaimAllRewards()
    {
        int claimedCount = 0;
        
        List<MailData> mailsWithRewards = inbox
            .Where(m => m.HasAttachments() && !m.isClaimed)
            .ToList();
        
        foreach (MailData mail in mailsWithRewards)
        {
            if (ClaimMailReward(mail.mailId))
            {
                claimedCount++;
            }
        }
        
        Debug.Log($"[MailManager] 批量领取了 {claimedCount} 封邮件的附件");
        return claimedCount;
    }
    
    /// <summary>
    /// 标记所有邮件为已读
    /// </summary>
    /// <returns>标记的邮件数量</returns>
    public int MarkAllAsRead()
    {
        int markedCount = 0;
        
        List<MailData> unreadMails = inbox
            .Where(m => !m.isRead)
            .ToList();
        
        foreach (MailData mail in unreadMails)
        {
            if (MarkAsRead(mail.mailId))
            {
                markedCount++;
            }
        }
        
        Debug.Log($"[MailManager] 标记了 {markedCount} 封邮件为已读");
        return markedCount;
    }
    
    /// <summary>
    /// 删除所有已读且已领取的邮件
    /// </summary>
    /// <returns>删除的邮件数量</returns>
    public int DeleteAllReadAndClaimed()
    {
        int deletedCount = 0;
        
        List<MailData> mailsToDelete = inbox
            .Where(m => m.isRead && (!m.HasAttachments() || m.isClaimed))
            .ToList();
        
        foreach (MailData mail in mailsToDelete)
        {
            if (DeleteMail(mail.mailId))
            {
                deletedCount++;
            }
        }
        
        Debug.Log($"[MailManager] 删除了 {deletedCount} 封已读邮件");
        return deletedCount;
    }
    
    #endregion
    
    #region 邮件查询
    
    /// <summary>
    /// 获取收件箱（所有邮件）
    /// </summary>
    /// <param name="sortByTime">是否按时间排序（新的在前）</param>
    /// <returns>邮件列表</returns>
    public List<MailData> GetInbox(bool sortByTime = true)
    {
        if (sortByTime)
        {
            return inbox.OrderByDescending(m => m.sentTime).ToList();
        }
        return new List<MailData>(inbox);
    }
    
    /// <summary>
    /// 获取未读邮件列表
    /// </summary>
    /// <returns>未读邮件列表</returns>
    public List<MailData> GetUnreadMails()
    {
        return inbox.Where(m => !m.isRead).OrderByDescending(m => m.sentTime).ToList();
    }
    
    /// <summary>
    /// 获取有附件未领取的邮件列表
    /// </summary>
    /// <returns>邮件列表</returns>
    public List<MailData> GetMailsWithUnclaimedRewards()
    {
        return inbox.Where(m => m.HasAttachments() && !m.isClaimed).ToList();
    }
    
    /// <summary>
    /// 按类型获取邮件
    /// </summary>
    /// <param name="mailType">邮件类型</param>
    /// <returns>邮件列表</returns>
    public List<MailData> GetMailsByType(string mailType)
    {
        return inbox.Where(m => m.mailType == mailType).OrderByDescending(m => m.sentTime).ToList();
    }
    
    /// <summary>
    /// 获取邮件
    /// </summary>
    /// <param name="mailId">邮件ID</param>
    /// <returns>邮件数据，不存在返回null</returns>
    public MailData GetMail(string mailId)
    {
        return inbox.FirstOrDefault(m => m.mailId == mailId);
    }
    
    /// <summary>
    /// 获取未读邮件数量
    /// </summary>
    /// <returns>数量</returns>
    public int GetUnreadCount()
    {
        return inbox.Count(m => !m.isRead);
    }
    
    /// <summary>
    /// 获取有未领取附件的邮件数量
    /// </summary>
    /// <returns>数量</returns>
    public int GetUnclaimedRewardCount()
    {
        return inbox.Count(m => m.HasAttachments() && !m.isClaimed);
    }
    
    #endregion
    
    #region 自动处理
    
    /// <summary>
    /// 自动删除过期邮件
    /// </summary>
    private void AutoDeleteOldMails()
    {
        DateTime cutoffDate = DateTime.Now.AddDays(-autoDeleteDays);
        
        List<MailData> expiredMails = inbox
            .Where(m => m.isRead && 
                       (!m.HasAttachments() || m.isClaimed) && 
                       m.sentTime < cutoffDate)
            .ToList();
        
        int deletedCount = 0;
        foreach (MailData mail in expiredMails)
        {
            inbox.Remove(mail);
            deletedCount++;
            TotalMailsDeleted++;
        }
        
        if (deletedCount > 0)
        {
            Debug.Log($"[MailManager] 自动删除了 {deletedCount} 封过期邮件");
            OnMailListUpdated?.Invoke();
        }
    }
    
    #endregion
    
    #region 数据持久化
    
    /// <summary>
    /// 保存数据
    /// </summary>
    public void SaveData()
    {
        // TODO: 实现保存到Firebase
        Debug.Log("[MailManager] 保存邮箱数据");
    }
    
    /// <summary>
    /// 加载数据
    /// </summary>
    public void LoadData()
    {
        // TODO: 实现从Firebase加载
        Debug.Log("[MailManager] 加载邮箱数据");
        
        // 调试模式：创建一些测试邮件
        if (debugMode && inbox.Count == 0)
        {
            CreateTestMails();
        }
    }
    
    /// <summary>
    /// 创建测试邮件（仅调试模式）
    /// </summary>
    private void CreateTestMails()
    {
        // 系统邮件
        ReceiveMail(MailData.CreateSystemMail(
            "测试系统通知",
            "这是一条测试系统通知",
            AuthenticationManager.Instance.currentUser.userId
        ));
        
        // 工资邮件
        ReceiveMail(MailData.CreateSalaryMail(
            "测试公司",
            500,
            AuthenticationManager.Instance.currentUser.userId
        ));
        
        Debug.Log("[MailManager] 创建了测试邮件");
    }
    
    #endregion
    
    #region 调试功能
    
    /// <summary>
    /// 发送测试邮件（仅调试）
    /// </summary>
    /// <param name="mailType">邮件类型</param>
    public void DebugSendTestMail(string mailType)
    {
        if (!debugMode) return;
        
        switch (mailType)
        {
            case "system":
                SendSystemMail("测试通知", "这是一条测试通知", AuthenticationManager.Instance.currentUser.userId);
                break;
                
            case "salary":
                SendSalaryMail("测试公司", UnityEngine.Random.Range(100, 1000), AuthenticationManager.Instance.currentUser.userId);
                break;
                
            case "reward":
                SendRewardMail("测试任务", UnityEngine.Random.Range(100, 500), "", AuthenticationManager.Instance.currentUser.userId);
                break;
                
            default:
                Debug.LogWarning($"[MailManager] 未知的邮件类型: {mailType}");
                break;
        }
    }
    
    /// <summary>
    /// 获取系统状态摘要（调试用）
    /// </summary>
    /// <returns>状态文本</returns>
    public string GetStatusSummary()
    {
        return $"邮箱系统状态:\n" +
               $"- 邮件总数: {inbox.Count}/{maxMailCount}\n" +
               $"- 未读邮件: {GetUnreadCount()}\n" +
               $"- 未领取附件: {GetUnclaimedRewardCount()}\n" +
               $"- 收到邮件: {TotalMailsReceived}\n" +
               $"- 已读邮件: {TotalMailsRead}\n" +
               $"- 从邮件获得: {TotalCoinsFromMail}币";
    }
    
    #endregion
}
