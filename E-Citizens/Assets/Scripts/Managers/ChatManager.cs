using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 聊天消息数据类
/// </summary>
[Serializable]
public class ChatMessageData
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public string messageId;
    
    /// <summary>
    /// 发送者用户ID
    /// </summary>
    public string senderUserId;
    
    /// <summary>
    /// 发送者名称
    /// </summary>
    public string senderName;
    
    /// <summary>
    /// 接收者用户ID
    /// </summary>
    public string receiverUserId;
    
    /// <summary>
    /// 消息内容
    /// </summary>
    public string content;
    
    /// <summary>
    /// 发送时间
    /// </summary>
    public DateTime sentTime;
    
    /// <summary>
    /// 是否已读
    /// </summary>
    public bool isRead;
    
    /// <summary>
    /// 是否是当前玩家发送的
    /// </summary>
    public bool isSentByPlayer;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public ChatMessageData()
    {
        messageId = Guid.NewGuid().ToString();
        senderUserId = "";
        senderName = "";
        receiverUserId = "";
        content = "";
        sentTime = DateTime.Now;
        isRead = false;
        isSentByPlayer = false;
    }
    
    /// <summary>
    /// 创建聊天消息
    /// </summary>
    /// <param name="senderId">发送者ID</param>
    /// <param name="senderName">发送者名称</param>
    /// <param name="receiverId">接收者ID</param>
    /// <param name="content">消息内容</param>
    public ChatMessageData(string senderId, string senderName, string receiverId, string content)
    {
        this.messageId = Guid.NewGuid().ToString();
        this.senderUserId = senderId;
        this.senderName = senderName;
        this.receiverUserId = receiverId;
        this.content = content;
        this.sentTime = DateTime.Now;
        this.isRead = false;
        this.isSentByPlayer = (senderId == AuthenticationManager.Instance.currentUser?.userId);
    }
    
    /// <summary>
    /// 获取时间显示文本
    /// </summary>
    /// <returns>时间文本</returns>
    public string GetTimeDisplay()
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
        else
        {
            return sentTime.ToString("MM-dd HH:mm");
        }
    }
}

/// <summary>
/// 聊天会话数据类
/// 表示与一个好友的聊天记录
/// </summary>
[Serializable]
public class ChatConversationData
{
    /// <summary>
    /// 好友用户ID
    /// </summary>
    public string friendUserId;
    
    /// <summary>
    /// 好友名称
    /// </summary>
    public string friendName;
    
    /// <summary>
    /// 消息列表
    /// </summary>
    public List<ChatMessageData> messages = new List<ChatMessageData>();
    
    /// <summary>
    /// 未读消息数量
    /// </summary>
    public int unreadCount;
    
    /// <summary>
    /// 最后一条消息时间
    /// </summary>
    public DateTime lastMessageTime;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public ChatConversationData(string friendId, string friendName)
    {
        this.friendUserId = friendId;
        this.friendName = friendName;
        this.messages = new List<ChatMessageData>();
        this.unreadCount = 0;
        this.lastMessageTime = DateTime.MinValue;
    }
    
    /// <summary>
    /// 添加消息
    /// </summary>
    /// <param name="message">消息数据</param>
    public void AddMessage(ChatMessageData message)
    {
        messages.Add(message);
        lastMessageTime = message.sentTime;
        
        // 如果是收到的消息，增加未读计数
        if (!message.isSentByPlayer && !message.isRead)
        {
            unreadCount++;
        }
    }
    
    /// <summary>
    /// 标记所有消息为已读
    /// </summary>
    public void MarkAllAsRead()
    {
        foreach (var message in messages.Where(m => !m.isSentByPlayer && !m.isRead))
        {
            message.isRead = true;
        }
        unreadCount = 0;
    }
    
    /// <summary>
    /// 获取最后一条消息预览
    /// </summary>
    /// <returns>消息预览文本</returns>
    public string GetLastMessagePreview()
    {
        if (messages.Count == 0)
        {
            return "暂无消息";
        }
        
        ChatMessageData lastMessage = messages[messages.Count - 1];
        string prefix = lastMessage.isSentByPlayer ? "我: " : "";
        string content = lastMessage.content;
        
        // 限制长度
        if (content.Length > 20)
        {
            content = content.Substring(0, 20) + "...";
        }
        
        return prefix + content;
    }
}

/// <summary>
/// 聊天管理器
/// 负责管理好友之间的私聊功能
/// 
/// 核心功能：
/// 1. 发送消息给好友
/// 2. 接收好友消息
/// 3. 聊天记录管理
/// 4. 未读消息追踪
/// 5. 实时消息通知
/// 
/// Unity操作步骤：
/// 1. 在Hierarchy中创建空物体，命名为"ChatManager"
/// 2. 添加此脚本到该物体上
/// 3. 脚本会自动初始化为单例
/// 4. 在游戏开始时调用Initialize()方法
/// 
/// 使用示例：
/// ChatManager.Instance.SendMessage("friend_id", "你好！");
/// List<ChatMessageData> messages = ChatManager.Instance.GetConversation("friend_id");
/// </summary>
public class ChatManager : MonoBehaviour
{
    #region 单例模式
    
    private static ChatManager instance;
    public static ChatManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ChatManager");
                instance = go.AddComponent<ChatManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    #endregion
    
    #region 数据存储
    
    /// <summary>
    /// 所有聊天会话
    /// Key: 好友用户ID, Value: 会话数据
    /// </summary>
    private Dictionary<string, ChatConversationData> conversations = new Dictionary<string, ChatConversationData>();
    
    /// <summary>
    /// 最大保存消息数量（每个会话）
    /// </summary>
    [SerializeField]
    private int maxMessagesPerConversation = 500;
    
    /// <summary>
    /// 是否启用调试模式
    /// </summary>
    [SerializeField]
    private bool debugMode = false;
    
    #endregion
    
    #region 统计数据
    
    /// <summary>
    /// 统计：发送的消息总数
    /// </summary>
    public int TotalMessagesSent { get; private set; }
    
    /// <summary>
    /// 统计：接收的消息总数
    /// </summary>
    public int TotalMessagesReceived { get; private set; }
    
    /// <summary>
    /// 统计：活跃会话数
    /// </summary>
    public int ActiveConversations => conversations.Count;
    
    #endregion
    
    #region 事件
    
    /// <summary>
    /// 事件：收到新消息（发送者名称，消息内容）
    /// </summary>
    public event Action<string, string> OnMessageReceived;
    
    /// <summary>
    /// 事件：消息发送成功（接收者名称）
    /// </summary>
    public event Action<string> OnMessageSent;
    
    /// <summary>
    /// 事件：会话更新（好友用户ID）
    /// </summary>
    public event Action<string> OnConversationUpdated;
    
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
    
    #endregion
    
    #region 初始化
    
    /// <summary>
    /// 初始化聊天系统
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[ChatManager] 初始化聊天系统");
        
        // 加载聊天记录
        LoadData();
        
        Debug.Log($"[ChatManager] 初始化完成，活跃会话：{ActiveConversations}");
    }
    
    #endregion
    
    #region 发送消息
    
    /// <summary>
    /// 发送消息给好友
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    /// <param name="content">消息内容</param>
    /// <returns>是否成功发送</returns>
    public bool SendMessage(string friendUserId, string content)
    {
        // 验证：消息内容不能为空
        if (string.IsNullOrWhiteSpace(content))
        {
            Debug.LogWarning("[ChatManager] 消息内容不能为空");
            return false;
        }
        
        // 验证：是否是好友
        if (FriendManager.Instance != null && !FriendManager.Instance.IsFriend(friendUserId))
        {
            Debug.LogWarning("[ChatManager] 只能给好友发送消息");
            return false;
        }
        
        // 获取或创建会话
        ChatConversationData conversation = GetOrCreateConversation(friendUserId);
        
        // 创建消息
        ChatMessageData message = new ChatMessageData(
            AuthenticationManager.Instance.currentUser.userId,
            AuthenticationManager.Instance.currentUser.username,
            friendUserId,
            content
        );
        
        // 添加到会话
        conversation.AddMessage(message);
        
        // 限制消息数量
        if (conversation.messages.Count > maxMessagesPerConversation)
        {
            int removeCount = conversation.messages.Count - maxMessagesPerConversation;
            conversation.messages.RemoveRange(0, removeCount);
        }
        
        TotalMessagesSent++;
        
        // TODO: 通过Firebase发送消息
        // FirebaseManager.SendChatMessage(message);
        
        OnMessageSent?.Invoke(conversation.friendName);
        OnConversationUpdated?.Invoke(friendUserId);
        
        Debug.Log($"[ChatManager] 发送消息给 {conversation.friendName}: {content}");
        return true;
    }
    
    #endregion
    
    #region 接收消息
    
    /// <summary>
    /// 接收好友消息（由Firebase监听触发）
    /// </summary>
    /// <param name="message">消息数据</param>
    public void ReceiveMessage(ChatMessageData message)
    {
        if (message.receiverUserId != AuthenticationManager.Instance.currentUser.userId)
        {
            Debug.LogWarning("[ChatManager] 消息接收者不是当前玩家");
            return;
        }
        
        // 获取或创建会话
        ChatConversationData conversation = GetOrCreateConversation(message.senderUserId);
        
        // 添加消息
        conversation.AddMessage(message);
        
        // 限制消息数量
        if (conversation.messages.Count > maxMessagesPerConversation)
        {
            int removeCount = conversation.messages.Count - maxMessagesPerConversation;
            conversation.messages.RemoveRange(0, removeCount);
        }
        
        TotalMessagesReceived++;
        
        // 触发事件
        OnMessageReceived?.Invoke(message.senderName, message.content);
        OnConversationUpdated?.Invoke(message.senderUserId);
        
        Debug.Log($"[ChatManager] 收到来自 {message.senderName} 的消息: {message.content}");
    }
    
    #endregion
    
    #region 会话管理
    
    /// <summary>
    /// 获取或创建会话
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    /// <returns>会话数据</returns>
    private ChatConversationData GetOrCreateConversation(string friendUserId)
    {
        if (!conversations.ContainsKey(friendUserId))
        {
            // 获取好友名称
            string friendName = "未知好友";
            if (FriendManager.Instance != null)
            {
                FriendData friend = FriendManager.Instance.GetFriend(friendUserId);
                if (friend != null)
                {
                    friendName = friend.friendName;
                }
            }
            
            conversations[friendUserId] = new ChatConversationData(friendUserId, friendName);
        }
        
        return conversations[friendUserId];
    }
    
    /// <summary>
    /// 获取会话
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    /// <returns>会话数据，不存在返回null</returns>
    public ChatConversationData GetConversation(string friendUserId)
    {
        if (conversations.ContainsKey(friendUserId))
        {
            return conversations[friendUserId];
        }
        return null;
    }
    
    /// <summary>
    /// 获取所有会话列表（按最后消息时间排序）
    /// </summary>
    /// <returns>会话列表</returns>
    public List<ChatConversationData> GetAllConversations()
    {
        return conversations.Values
            .OrderByDescending(c => c.lastMessageTime)
            .ToList();
    }
    
    /// <summary>
    /// 获取有未读消息的会话列表
    /// </summary>
    /// <returns>会话列表</returns>
    public List<ChatConversationData> GetConversationsWithUnread()
    {
        return conversations.Values
            .Where(c => c.unreadCount > 0)
            .OrderByDescending(c => c.lastMessageTime)
            .ToList();
    }
    
    /// <summary>
    /// 标记会话为已读
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    /// <returns>是否成功</returns>
    public bool MarkConversationAsRead(string friendUserId)
    {
        ChatConversationData conversation = GetConversation(friendUserId);
        
        if (conversation == null)
        {
            Debug.LogWarning($"[ChatManager] 未找到与 {friendUserId} 的会话");
            return false;
        }
        
        conversation.MarkAllAsRead();
        
        // TODO: 同步到Firebase
        // FirebaseManager.MarkMessagesAsRead(friendUserId);
        
        OnConversationUpdated?.Invoke(friendUserId);
        
        return true;
    }
    
    /// <summary>
    /// 删除会话
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    /// <returns>是否成功删除</returns>
    public bool DeleteConversation(string friendUserId)
    {
        if (!conversations.ContainsKey(friendUserId))
        {
            Debug.LogWarning($"[ChatManager] 未找到与 {friendUserId} 的会话");
            return false;
        }
        
        conversations.Remove(friendUserId);
        
        // TODO: 同步到Firebase
        // FirebaseManager.DeleteConversation(friendUserId);
        
        Debug.Log($"[ChatManager] 删除了与 {friendUserId} 的会话");
        return true;
    }
    
    #endregion
    
    #region 查询
    
    /// <summary>
    /// 获取总未读消息数
    /// </summary>
    /// <returns>未读数量</returns>
    public int GetTotalUnreadCount()
    {
        return conversations.Values.Sum(c => c.unreadCount);
    }
    
    /// <summary>
    /// 检查是否有未读消息
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    /// <returns>是否有未读消息</returns>
    public bool HasUnreadMessages(string friendUserId)
    {
        ChatConversationData conversation = GetConversation(friendUserId);
        return conversation != null && conversation.unreadCount > 0;
    }
    
    #endregion
    
    #region 数据持久化
    
    /// <summary>
    /// 保存数据
    /// </summary>
    public void SaveData()
    {
        // TODO: 实现保存到Firebase
        Debug.Log("[ChatManager] 保存聊天数据");
    }
    
    /// <summary>
    /// 加载数据
    /// </summary>
    public void LoadData()
    {
        // TODO: 实现从Firebase加载
        Debug.Log("[ChatManager] 加载聊天数据");
        
        // 调试模式：创建一些测试会话
        if (debugMode && conversations.Count == 0)
        {
            CreateTestConversations();
        }
    }
    
    /// <summary>
    /// 创建测试会话（仅调试模式）
    /// </summary>
    private void CreateTestConversations()
    {
        // 创建测试会话
        ChatConversationData conv1 = new ChatConversationData("test_001", "虚拟人小王");
        conv1.AddMessage(new ChatMessageData("test_001", "虚拟人小王", AuthenticationManager.Instance.currentUser.userId, "你好啊！"));
        conversations["test_001"] = conv1;
        
        ChatConversationData conv2 = new ChatConversationData("test_002", "数据体小李");
        conv2.AddMessage(new ChatMessageData(AuthenticationManager.Instance.currentUser.userId, AuthenticationManager.Instance.currentUser.username, "test_002", "最近怎么样？"));
        conversations["test_002"] = conv2;
        
        Debug.Log("[ChatManager] 创建了测试会话");
    }
    
    #endregion
    
    #region 调试功能
    
    /// <summary>
    /// 模拟收到消息（仅调试）
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    public void DebugReceiveMessage(string friendUserId)
    {
        if (!debugMode) return;
        
        string[] testMessages = new string[]
        {
            "你好！",
            "最近怎么样？",
            "今天工作顺利吗？",
            "有空一起玩吗？",
            "谢谢你的礼物！"
        };
        
        ChatMessageData message = new ChatMessageData(
            friendUserId,
            "测试好友",
            AuthenticationManager.Instance.currentUser.userId,
            testMessages[UnityEngine.Random.Range(0, testMessages.Length)]
        );
        
        ReceiveMessage(message);
    }
    
    /// <summary>
    /// 获取系统状态摘要（调试用）
    /// </summary>
    /// <returns>状态文本</returns>
    public string GetStatusSummary()
    {
        return $"聊天系统状态:\n" +
               $"- 活跃会话: {ActiveConversations}\n" +
               $"- 总未读数: {GetTotalUnreadCount()}\n" +
               $"- 发送消息: {TotalMessagesSent}\n" +
               $"- 接收消息: {TotalMessagesReceived}";
    }
    
    #endregion
}
