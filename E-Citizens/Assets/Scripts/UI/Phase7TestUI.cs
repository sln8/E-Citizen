using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// Phase 7 社交系统测试UI
/// 用于测试好友、邮件、礼物、排行榜和聊天功能
/// 
/// Unity操作步骤（详细的UI创建步骤请参考PHASE7_SETUP_GUIDE.md）：
/// 1. 在Canvas下创建空物体，命名为"Phase7TestPanel"
/// 2. 添加此脚本到该物体上
/// 3. 创建UI元素并连接引用（见Inspector）
/// 4. 运行游戏测试功能
/// 
/// 注意：这是测试UI，正式版本需要创建更精美的界面
/// </summary>
public class Phase7TestUI : MonoBehaviour
{
    #region UI元素引用
    
    [Header("主面板")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private TextMeshProUGUI detailText;
    
    [Header("好友系统按钮")]
    [SerializeField] private Button btnSendFriendRequest;
    [SerializeField] private Button btnShowFriendList;
    [SerializeField] private Button btnShowFriendRequests;
    [SerializeField] private Button btnAcceptFirstRequest;
    [SerializeField] private Button btnSendGift;
    
    [Header("邮件系统按钮")]
    [SerializeField] private Button btnShowInbox;
    [SerializeField] private Button btnClaimAllRewards;
    [SerializeField] private Button btnMarkAllRead;
    [SerializeField] private Button btnSendTestMail;
    
    [Header("排行榜按钮")]
    [SerializeField] private Button btnShowWealth;
    [SerializeField] private Button btnShowLevel;
    [SerializeField] private Button btnShowMood;
    [SerializeField] private Button btnUpdateRanking;
    
    [Header("聊天系统按钮")]
    [SerializeField] private Button btnShowConversations;
    [SerializeField] private Button btnSendMessage;
    [SerializeField] private Button btnShowUnread;
    
    [Header("综合功能按钮")]
    [SerializeField] private Button btnRefreshAll;
    [SerializeField] private Button btnShowSystemStatus;
    
    #endregion
    
    #region 私有变量
    
    /// <summary>
    /// 状态更新间隔
    /// </summary>
    private float statusUpdateInterval = 0.5f;
    
    /// <summary>
    /// 下次状态更新时间
    /// </summary>
    private float nextStatusUpdate;
    
    /// <summary>
    /// 当前选中的好友ID（用于测试）
    /// </summary>
    private string selectedFriendId = "";
    
    #endregion
    
    #region Unity生命周期
    
    void Start()
    {
        InitializeUI();
        SetupButtons();
        InitializeManagers();
        
        // 订阅所有事件
        SubscribeToEvents();
    }
    
    void Update()
    {
        // 定期更新状态显示
        if (Time.time >= nextStatusUpdate)
        {
            UpdateStatusDisplay();
            nextStatusUpdate = Time.time + statusUpdateInterval;
        }
    }
    
    void OnDestroy()
    {
        // 取消订阅事件
        UnsubscribeFromEvents();
    }
    
    #endregion
    
    #region 初始化
    
    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        if (statusText != null)
        {
            statusText.text = "Phase 7 社交系统测试\n准备就绪";
        }
        
        if (detailText != null)
        {
            detailText.text = "欢迎使用社交系统测试界面\n\n" +
                            "功能说明：\n" +
                            "- 好友系统：添加好友、查看列表、赠送礼物\n" +
                            "- 邮件系统：收发邮件、领取附件\n" +
                            "- 排行榜：查看各类排行榜\n" +
                            "- 聊天系统：私聊好友\n\n" +
                            "点击按钮开始测试";
        }
    }
    
    /// <summary>
    /// 设置按钮监听
    /// </summary>
    private void SetupButtons()
    {
        // 好友系统
        if (btnSendFriendRequest != null) btnSendFriendRequest.onClick.AddListener(OnSendFriendRequest);
        if (btnShowFriendList != null) btnShowFriendList.onClick.AddListener(OnShowFriendList);
        if (btnShowFriendRequests != null) btnShowFriendRequests.onClick.AddListener(OnShowFriendRequests);
        if (btnAcceptFirstRequest != null) btnAcceptFirstRequest.onClick.AddListener(OnAcceptFirstRequest);
        if (btnSendGift != null) btnSendGift.onClick.AddListener(OnSendGift);
        
        // 邮件系统
        if (btnShowInbox != null) btnShowInbox.onClick.AddListener(OnShowInbox);
        if (btnClaimAllRewards != null) btnClaimAllRewards.onClick.AddListener(OnClaimAllRewards);
        if (btnMarkAllRead != null) btnMarkAllRead.onClick.AddListener(OnMarkAllRead);
        if (btnSendTestMail != null) btnSendTestMail.onClick.AddListener(OnSendTestMail);
        
        // 排行榜
        if (btnShowWealth != null) btnShowWealth.onClick.AddListener(() => OnShowLeaderboard(LeaderboardType.Wealth));
        if (btnShowLevel != null) btnShowLevel.onClick.AddListener(() => OnShowLeaderboard(LeaderboardType.Level));
        if (btnShowMood != null) btnShowMood.onClick.AddListener(() => OnShowLeaderboard(LeaderboardType.Mood));
        if (btnUpdateRanking != null) btnUpdateRanking.onClick.AddListener(OnUpdateRanking);
        
        // 聊天系统
        if (btnShowConversations != null) btnShowConversations.onClick.AddListener(OnShowConversations);
        if (btnSendMessage != null) btnSendMessage.onClick.AddListener(OnSendMessage);
        if (btnShowUnread != null) btnShowUnread.onClick.AddListener(OnShowUnread);
        
        // 综合功能
        if (btnRefreshAll != null) btnRefreshAll.onClick.AddListener(OnRefreshAll);
        if (btnShowSystemStatus != null) btnShowSystemStatus.onClick.AddListener(OnShowSystemStatus);
    }
    
    /// <summary>
    /// 初始化管理器
    /// </summary>
    private void InitializeManagers()
    {
        // 确保所有管理器已初始化
        if (FriendManager.Instance != null)
        {
            FriendManager.Instance.Initialize();
        }
        
        if (MailManager.Instance != null)
        {
            MailManager.Instance.Initialize();
        }
        
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.Initialize();
        }
        
        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.Initialize();
        }
        
        Debug.Log("[Phase7TestUI] 所有管理器初始化完成");
    }
    
    #endregion
    
    #region 事件订阅
    
    /// <summary>
    /// 订阅所有事件
    /// </summary>
    private void SubscribeToEvents()
    {
        // 好友系统事件
        if (FriendManager.Instance != null)
        {
            FriendManager.Instance.OnFriendListUpdated += OnFriendListUpdated;
            FriendManager.Instance.OnFriendRequestReceived += OnFriendRequestReceived;
            FriendManager.Instance.OnGiftReceived += OnGiftReceived;
        }
        
        // 邮件系统事件
        if (MailManager.Instance != null)
        {
            MailManager.Instance.OnNewMailReceived += OnNewMailReceived;
            MailManager.Instance.OnAttachmentClaimed += OnAttachmentClaimed;
        }
        
        // 排行榜事件
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.OnRankImproved += OnRankImproved;
            LeaderboardManager.Instance.OnEnteredTop10 += OnEnteredTop10;
        }
        
        // 聊天系统事件
        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.OnMessageReceived += OnChatMessageReceived;
        }
    }
    
    /// <summary>
    /// 取消订阅事件
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (FriendManager.Instance != null)
        {
            FriendManager.Instance.OnFriendListUpdated -= OnFriendListUpdated;
            FriendManager.Instance.OnFriendRequestReceived -= OnFriendRequestReceived;
            FriendManager.Instance.OnGiftReceived -= OnGiftReceived;
        }
        
        if (MailManager.Instance != null)
        {
            MailManager.Instance.OnNewMailReceived -= OnNewMailReceived;
            MailManager.Instance.OnAttachmentClaimed -= OnAttachmentClaimed;
        }
        
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.OnRankImproved -= OnRankImproved;
            LeaderboardManager.Instance.OnEnteredTop10 -= OnEnteredTop10;
        }
        
        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.OnMessageReceived -= OnChatMessageReceived;
        }
    }
    
    #endregion
    
    #region 好友系统按钮处理
    
    private void OnSendFriendRequest()
    {
        // 模拟收到好友请求（调试功能）
        if (FriendManager.Instance != null)
        {
            FriendManager.Instance.DebugReceiveFriendRequest();
            ShowMessage("模拟收到好友请求");
        }
    }
    
    private void OnShowFriendList()
    {
        if (FriendManager.Instance == null) return;
        
        List<FriendData> friends = FriendManager.Instance.GetFriendList();
        
        string info = $"=== 好友列表 ({friends.Count}) ===\n\n";
        
        foreach (var friend in friends)
        {
            info += $"{friend.GetDisplaySummary()}\n";
            info += $"  好友关系: {friend.GetFriendshipDuration()}\n\n";
        }
        
        if (friends.Count == 0)
        {
            info += "暂无好友\n";
        }
        
        ShowDetail(info);
    }
    
    private void OnShowFriendRequests()
    {
        if (FriendManager.Instance == null) return;
        
        List<FriendRequestData> requests = FriendManager.Instance.GetReceivedRequests(true);
        
        string info = $"=== 好友请求 ({requests.Count}) ===\n\n";
        
        foreach (var request in requests)
        {
            info += $"来自: {request.senderName} Lv.{request.senderLevel}\n";
            info += $"消息: {request.message}\n";
            info += $"时间: {request.requestTime:MM-dd HH:mm}\n\n";
        }
        
        if (requests.Count == 0)
        {
            info += "暂无待处理请求\n";
        }
        
        ShowDetail(info);
    }
    
    private void OnAcceptFirstRequest()
    {
        if (FriendManager.Instance == null) return;
        
        List<FriendRequestData> requests = FriendManager.Instance.GetReceivedRequests(true);
        
        if (requests.Count > 0)
        {
            bool success = FriendManager.Instance.AcceptFriendRequest(requests[0].requestId);
            ShowMessage(success ? $"已添加好友：{requests[0].senderName}" : "添加好友失败");
        }
        else
        {
            ShowMessage("没有待处理的好友请求");
        }
    }
    
    private void OnSendGift()
    {
        if (FriendManager.Instance == null) return;
        
        List<FriendData> friends = FriendManager.Instance.GetFriendList();
        
        if (friends.Count == 0)
        {
            ShowMessage("没有好友可以赠送礼物");
            return;
        }
        
        // 获取第一个好友
        FriendData friend = friends[0];
        selectedFriendId = friend.friendUserId;
        
        // 获取第一个礼物
        GiftData[] gifts = FriendManager.Instance.GetAvailableGifts();
        if (gifts.Length > 0)
        {
            bool success = FriendManager.Instance.SendGift(selectedFriendId, gifts[0], "送你个礼物！");
            ShowMessage(success ? $"已赠送【{gifts[0].giftName}】给 {friend.friendName}" : "赠送礼物失败");
        }
    }
    
    #endregion
    
    #region 邮件系统按钮处理
    
    private void OnShowInbox()
    {
        if (MailManager.Instance == null) return;
        
        List<MailData> mails = MailManager.Instance.GetInbox(true);
        
        string info = $"=== 收件箱 ({mails.Count}) ===\n";
        info += $"未读: {MailManager.Instance.GetUnreadCount()}\n";
        info += $"未领取: {MailManager.Instance.GetUnclaimedRewardCount()}\n\n";
        
        foreach (var mail in mails.Take(10)) // 只显示前10封
        {
            info += $"{mail.GetSummary()}\n";
            if (mail.HasAttachments())
            {
                info += $"  {mail.GetAttachmentSummary()}\n";
            }
            info += "\n";
        }
        
        if (mails.Count == 0)
        {
            info += "收件箱为空\n";
        }
        
        ShowDetail(info);
    }
    
    private void OnClaimAllRewards()
    {
        if (MailManager.Instance == null) return;
        
        int count = MailManager.Instance.ClaimAllRewards();
        ShowMessage($"领取了 {count} 封邮件的附件");
    }
    
    private void OnMarkAllRead()
    {
        if (MailManager.Instance == null) return;
        
        int count = MailManager.Instance.MarkAllAsRead();
        ShowMessage($"标记了 {count} 封邮件为已读");
    }
    
    private void OnSendTestMail()
    {
        if (MailManager.Instance == null) return;
        
        string[] types = new string[] { "system", "salary", "reward" };
        string type = types[UnityEngine.Random.Range(0, types.Length)];
        
        MailManager.Instance.DebugSendTestMail(type);
        ShowMessage($"发送了测试邮件：{type}");
    }
    
    #endregion
    
    #region 排行榜按钮处理
    
    private void OnShowLeaderboard(LeaderboardType type)
    {
        if (LeaderboardManager.Instance == null) return;
        
        List<LeaderboardEntryData> topPlayers = LeaderboardManager.Instance.GetTopPlayers(type, 10);
        int playerRank = LeaderboardManager.Instance.GetPlayerRank(type);
        
        string info = $"=== {GetLeaderboardName(type)} ===\n";
        info += $"你的排名: #{playerRank}\n\n";
        
        foreach (var entry in topPlayers)
        {
            info += $"{entry.GetRankDisplay()}. {entry.playerName} Lv.{entry.playerLevel}\n";
            
            switch (type)
            {
                case LeaderboardType.Wealth:
                    info += $"   财富: {entry.totalWealth}币\n";
                    break;
                case LeaderboardType.Level:
                    info += $"   等级: {entry.level}\n";
                    break;
                case LeaderboardType.Mood:
                    info += $"   心情: {entry.moodValue}\n";
                    break;
                case LeaderboardType.OnlineTime:
                    info += $"   在线: {entry.GetOnlineTimeDisplay()}\n";
                    break;
            }
            
            info += "\n";
        }
        
        ShowDetail(info);
    }
    
    private void OnUpdateRanking()
    {
        if (LeaderboardManager.Instance == null) return;
        
        LeaderboardManager.Instance.DebugForceUpdate();
        ShowMessage("已更新排行榜排名");
    }
    
    private string GetLeaderboardName(LeaderboardType type)
    {
        switch (type)
        {
            case LeaderboardType.Wealth: return "财富榜";
            case LeaderboardType.Level: return "等级榜";
            case LeaderboardType.Mood: return "心情榜";
            case LeaderboardType.OnlineTime: return "在线时长榜";
            default: return "排行榜";
        }
    }
    
    #endregion
    
    #region 聊天系统按钮处理
    
    private void OnShowConversations()
    {
        if (ChatManager.Instance == null) return;
        
        List<ChatConversationData> conversations = ChatManager.Instance.GetAllConversations();
        
        string info = $"=== 聊天会话 ({conversations.Count}) ===\n";
        info += $"总未读: {ChatManager.Instance.GetTotalUnreadCount()}\n\n";
        
        foreach (var conv in conversations)
        {
            info += $"{conv.friendName}\n";
            info += $"  {conv.GetLastMessagePreview()}\n";
            if (conv.unreadCount > 0)
            {
                info += $"  [未读: {conv.unreadCount}]\n";
            }
            info += "\n";
        }
        
        if (conversations.Count == 0)
        {
            info += "暂无聊天记录\n";
        }
        
        ShowDetail(info);
    }
    
    private void OnSendMessage()
    {
        if (ChatManager.Instance == null || FriendManager.Instance == null) return;
        
        List<FriendData> friends = FriendManager.Instance.GetFriendList();
        
        if (friends.Count == 0)
        {
            ShowMessage("没有好友可以发送消息");
            return;
        }
        
        // 发送给第一个好友
        FriendData friend = friends[0];
        bool success = ChatManager.Instance.SendMessage(friend.friendUserId, "你好，测试消息！");
        ShowMessage(success ? $"已发送消息给 {friend.friendName}" : "发送消息失败");
    }
    
    private void OnShowUnread()
    {
        if (ChatManager.Instance == null) return;
        
        List<ChatConversationData> unread = ChatManager.Instance.GetConversationsWithUnread();
        
        string info = $"=== 未读消息 ({ChatManager.Instance.GetTotalUnreadCount()}) ===\n\n";
        
        foreach (var conv in unread)
        {
            info += $"{conv.friendName} [未读: {conv.unreadCount}]\n";
            info += $"  {conv.GetLastMessagePreview()}\n\n";
        }
        
        if (unread.Count == 0)
        {
            info += "没有未读消息\n";
        }
        
        ShowDetail(info);
    }
    
    #endregion
    
    #region 综合功能按钮处理
    
    private void OnRefreshAll()
    {
        // 刷新所有数据
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.UpdatePlayerRanking();
        }
        
        ShowMessage("已刷新所有数据");
    }
    
    private void OnShowSystemStatus()
    {
        string info = "=== 系统状态 ===\n\n";
        
        if (FriendManager.Instance != null)
        {
            info += FriendManager.Instance.GetStatusSummary() + "\n\n";
        }
        
        if (MailManager.Instance != null)
        {
            info += MailManager.Instance.GetStatusSummary() + "\n\n";
        }
        
        if (LeaderboardManager.Instance != null)
        {
            info += LeaderboardManager.Instance.GetStatusSummary() + "\n\n";
        }
        
        if (ChatManager.Instance != null)
        {
            info += ChatManager.Instance.GetStatusSummary() + "\n\n";
        }
        
        ShowDetail(info);
    }
    
    #endregion
    
    #region 事件处理器
    
    private void OnFriendListUpdated()
    {
        ShowMessage("好友列表已更新");
    }
    
    private void OnFriendRequestReceived(string senderName)
    {
        ShowMessage($"收到来自 {senderName} 的好友请求");
    }
    
    private void OnGiftReceived(string senderName, string giftName)
    {
        ShowMessage($"收到来自 {senderName} 的礼物：{giftName}");
    }
    
    private void OnNewMailReceived(string type, string title)
    {
        ShowMessage($"收到新邮件：{title}");
    }
    
    private void OnAttachmentClaimed(int coins, int mood)
    {
        ShowMessage($"领取附件：{coins}币，心情+{mood}");
    }
    
    private void OnRankImproved(LeaderboardType type, int newRank, int oldRank)
    {
        ShowMessage($"{GetLeaderboardName(type)}排名提升：{oldRank} → {newRank}");
    }
    
    private void OnEnteredTop10(LeaderboardType type, int rank)
    {
        ShowMessage($"恭喜进入{GetLeaderboardName(type)}前10名！当前排名：{rank}");
    }
    
    private void OnChatMessageReceived(string senderName, string content)
    {
        ShowMessage($"{senderName}: {content}");
    }
    
    #endregion
    
    #region 状态更新
    
    /// <summary>
    /// 更新状态显示
    /// </summary>
    private void UpdateStatusDisplay()
    {
        if (statusText == null) return;
        
        string status = "Phase 7 社交系统测试\n\n";
        
        // 好友系统状态
        if (FriendManager.Instance != null)
        {
            List<FriendData> friends = FriendManager.Instance.GetFriendList();
            int pendingRequests = FriendManager.Instance.GetPendingRequestCount();
            status += $"好友: {friends.Count} | 请求: {pendingRequests}\n";
        }
        
        // 邮件系统状态
        if (MailManager.Instance != null)
        {
            int unread = MailManager.Instance.GetUnreadCount();
            int unclaimed = MailManager.Instance.GetUnclaimedRewardCount();
            status += $"邮件: 未读{unread} | 未领{unclaimed}\n";
        }
        
        // 排行榜状态
        if (LeaderboardManager.Instance != null)
        {
            int wealthRank = LeaderboardManager.Instance.GetPlayerRank(LeaderboardType.Wealth);
            status += $"财富榜排名: #{wealthRank}\n";
        }
        
        // 聊天系统状态
        if (ChatManager.Instance != null)
        {
            int totalUnread = ChatManager.Instance.GetTotalUnreadCount();
            status += $"未读消息: {totalUnread}\n";
        }
        
        statusText.text = status;
    }
    
    #endregion
    
    #region 辅助方法
    
    /// <summary>
    /// 显示消息（状态栏）
    /// </summary>
    private void ShowMessage(string message)
    {
        Debug.Log($"[Phase7TestUI] {message}");
        
        if (statusText != null)
        {
            // 在状态文本末尾添加消息
            string currentText = statusText.text;
            statusText.text = currentText + $"\n[{DateTime.Now:HH:mm:ss}] {message}";
        }
    }
    
    /// <summary>
    /// 显示详细信息（详情面板）
    /// </summary>
    private void ShowDetail(string detail)
    {
        if (detailText != null)
        {
            detailText.text = detail;
        }
        
        // 滚动到顶部
        if (scrollView != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollView.verticalNormalizedPosition = 1f;
        }
    }
    
    #endregion
}
