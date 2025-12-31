using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 好友系统管理器
/// 负责管理游戏中的好友功能
/// 
/// 核心功能：
/// 1. 添加/删除好友
/// 2. 好友请求处理
/// 3. 好友列表管理
/// 4. 在线状态追踪
/// 5. 赠送礼物
/// 
/// Unity操作步骤：
/// 1. 在Hierarchy中创建空物体，命名为"FriendManager"
/// 2. 添加此脚本到该物体上
/// 3. 脚本会自动初始化为单例
/// 4. 在游戏开始时调用Initialize()方法
/// 
/// 使用示例：
/// FriendManager.Instance.SendFriendRequest("player_123", "你好，交个朋友吧！");
/// FriendManager.Instance.SendGift("friend_id", giftData, "送你个礼物");
/// </summary>
public class FriendManager : MonoBehaviour
{
    #region 单例模式
    
    private static FriendManager instance;
    public static FriendManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("FriendManager");
                instance = go.AddComponent<FriendManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    #endregion
    
    #region 数据存储
    
    /// <summary>
    /// 好友列表
    /// </summary>
    private List<FriendData> friendsList = new List<FriendData>();
    
    /// <summary>
    /// 收到的好友请求列表
    /// </summary>
    private List<FriendRequestData> receivedRequests = new List<FriendRequestData>();
    
    /// <summary>
    /// 发送的好友请求列表
    /// </summary>
    private List<FriendRequestData> sentRequests = new List<FriendRequestData>();
    
    /// <summary>
    /// 可用的礼物列表
    /// </summary>
    private GiftData[] availableGifts;
    
    /// <summary>
    /// 礼物交易记录
    /// </summary>
    private List<GiftTransactionData> giftTransactions = new List<GiftTransactionData>();
    
    #endregion
    
    #region 配置
    
    /// <summary>
    /// 最大好友数量
    /// </summary>
    [SerializeField]
    private int maxFriends = 100;
    
    /// <summary>
    /// 最大待处理请求数量
    /// </summary>
    [SerializeField]
    private int maxPendingRequests = 50;
    
    /// <summary>
    /// 是否启用调试模式
    /// </summary>
    [SerializeField]
    private bool debugMode = false;
    
    #endregion
    
    #region 统计数据
    
    /// <summary>
    /// 统计：发送的好友请求总数
    /// </summary>
    public int TotalFriendRequestsSent { get; private set; }
    
    /// <summary>
    /// 统计：接受的好友请求总数
    /// </summary>
    public int TotalFriendRequestsAccepted { get; private set; }
    
    /// <summary>
    /// 统计：发送的礼物总数
    /// </summary>
    public int TotalGiftsSent { get; private set; }
    
    /// <summary>
    /// 统计：接收的礼物总数
    /// </summary>
    public int TotalGiftsReceived { get; private set; }
    
    #endregion
    
    #region 事件
    
    /// <summary>
    /// 事件：好友列表更新
    /// </summary>
    public event Action OnFriendListUpdated;
    
    /// <summary>
    /// 事件：收到新的好友请求（请求者名称）
    /// </summary>
    public event Action<string> OnFriendRequestReceived;
    
    /// <summary>
    /// 事件：好友请求被接受（好友名称）
    /// </summary>
    public event Action<string> OnFriendRequestAccepted;
    
    /// <summary>
    /// 事件：收到礼物（发送者名称，礼物名称）
    /// </summary>
    public event Action<string, string> OnGiftReceived;
    
    /// <summary>
    /// 事件：好友上线（好友名称）
    /// </summary>
    public event Action<string> OnFriendCameOnline;
    
    /// <summary>
    /// 事件：好友下线（好友名称）
    /// </summary>
    public event Action<string> OnFriendWentOffline;
    
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
    /// 初始化好友系统
    /// 在游戏开始时调用
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[FriendManager] 初始化好友系统");
        
        // 初始化礼物列表
        availableGifts = GiftData.CreateDefaultGifts();
        
        // 加载数据（从Firebase或本地）
        LoadData();
        
        Debug.Log($"[FriendManager] 初始化完成，当前好友数：{friendsList.Count}");
    }
    
    #endregion
    
    #region 好友请求
    
    /// <summary>
    /// 发送好友请求
    /// </summary>
    /// <param name="targetUserId">目标用户ID</param>
    /// <param name="message">附加消息</param>
    /// <returns>是否成功发送</returns>
    public bool SendFriendRequest(string targetUserId, string message = "")
    {
        // 验证：不能向自己发送请求
        if (targetUserId == UserData.Instance.userId)
        {
            Debug.LogWarning("[FriendManager] 不能向自己发送好友请求");
            return false;
        }
        
        // 验证：是否已经是好友
        if (IsFriend(targetUserId))
        {
            Debug.LogWarning("[FriendManager] 该用户已经是好友");
            return false;
        }
        
        // 验证：是否已经发送过请求
        if (sentRequests.Any(r => r.receiverUserId == targetUserId && r.status == "pending"))
        {
            Debug.LogWarning("[FriendManager] 已经向该用户发送过请求");
            return false;
        }
        
        // 创建好友请求
        FriendRequestData request = new FriendRequestData(
            UserData.Instance.userId,
            UserData.Instance.playerName,
            UserData.Instance.level,
            targetUserId,
            message
        );
        
        sentRequests.Add(request);
        TotalFriendRequestsSent++;
        
        // TODO: 同步到Firebase
        // FirebaseManager.SendFriendRequest(request);
        
        Debug.Log($"[FriendManager] 成功发送好友请求到用户 {targetUserId}");
        return true;
    }
    
    /// <summary>
    /// 接受好友请求
    /// </summary>
    /// <param name="requestId">请求ID</param>
    /// <returns>是否成功接受</returns>
    public bool AcceptFriendRequest(string requestId)
    {
        FriendRequestData request = receivedRequests.FirstOrDefault(r => r.requestId == requestId);
        
        if (request == null)
        {
            Debug.LogWarning($"[FriendManager] 未找到请求 {requestId}");
            return false;
        }
        
        if (request.status != "pending")
        {
            Debug.LogWarning($"[FriendManager] 请求 {requestId} 已处理");
            return false;
        }
        
        // 验证：好友数量限制
        if (friendsList.Count >= maxFriends)
        {
            Debug.LogWarning("[FriendManager] 好友数量已达上限");
            return false;
        }
        
        // 接受请求
        request.Accept();
        
        // 添加好友
        FriendData newFriend = new FriendData(
            request.senderUserId,
            request.senderName,
            request.senderLevel
        );
        friendsList.Add(newFriend);
        
        TotalFriendRequestsAccepted++;
        
        // TODO: 同步到Firebase
        // FirebaseManager.AcceptFriendRequest(requestId);
        
        // 触发事件
        OnFriendListUpdated?.Invoke();
        OnFriendRequestAccepted?.Invoke(request.senderName);
        
        Debug.Log($"[FriendManager] 成功添加好友：{request.senderName}");
        return true;
    }
    
    /// <summary>
    /// 拒绝好友请求
    /// </summary>
    /// <param name="requestId">请求ID</param>
    /// <returns>是否成功拒绝</returns>
    public bool RejectFriendRequest(string requestId)
    {
        FriendRequestData request = receivedRequests.FirstOrDefault(r => r.requestId == requestId);
        
        if (request == null)
        {
            Debug.LogWarning($"[FriendManager] 未找到请求 {requestId}");
            return false;
        }
        
        request.Reject();
        
        // TODO: 同步到Firebase
        // FirebaseManager.RejectFriendRequest(requestId);
        
        Debug.Log($"[FriendManager] 已拒绝来自 {request.senderName} 的好友请求");
        return true;
    }
    
    #endregion
    
    #region 好友管理
    
    /// <summary>
    /// 删除好友
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    /// <returns>是否成功删除</returns>
    public bool RemoveFriend(string friendUserId)
    {
        FriendData friend = friendsList.FirstOrDefault(f => f.friendUserId == friendUserId);
        
        if (friend == null)
        {
            Debug.LogWarning($"[FriendManager] 未找到好友 {friendUserId}");
            return false;
        }
        
        friendsList.Remove(friend);
        
        // TODO: 同步到Firebase
        // FirebaseManager.RemoveFriend(friendUserId);
        
        OnFriendListUpdated?.Invoke();
        
        Debug.Log($"[FriendManager] 已删除好友：{friend.friendName}");
        return true;
    }
    
    /// <summary>
    /// 检查是否是好友
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>是否是好友</returns>
    public bool IsFriend(string userId)
    {
        return friendsList.Any(f => f.friendUserId == userId);
    }
    
    /// <summary>
    /// 获取好友列表
    /// </summary>
    /// <param name="onlineOnly">是否只获取在线好友</param>
    /// <returns>好友列表</returns>
    public List<FriendData> GetFriendList(bool onlineOnly = false)
    {
        if (onlineOnly)
        {
            return friendsList.Where(f => f.isOnline).ToList();
        }
        return new List<FriendData>(friendsList);
    }
    
    /// <summary>
    /// 获取好友数据
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    /// <returns>好友数据，不存在返回null</returns>
    public FriendData GetFriend(string friendUserId)
    {
        return friendsList.FirstOrDefault(f => f.friendUserId == friendUserId);
    }
    
    /// <summary>
    /// 更新好友在线状态
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    /// <param name="isOnline">是否在线</param>
    public void UpdateFriendOnlineStatus(string friendUserId, bool isOnline)
    {
        FriendData friend = GetFriend(friendUserId);
        if (friend == null) return;
        
        bool wasOnline = friend.isOnline;
        friend.UpdateOnlineStatus(isOnline);
        
        // 触发上线/下线事件
        if (!wasOnline && isOnline)
        {
            OnFriendCameOnline?.Invoke(friend.friendName);
        }
        else if (wasOnline && !isOnline)
        {
            OnFriendWentOffline?.Invoke(friend.friendName);
        }
    }
    
    #endregion
    
    #region 礼物系统
    
    /// <summary>
    /// 获取可用礼物列表
    /// </summary>
    /// <returns>礼物数组</returns>
    public GiftData[] GetAvailableGifts()
    {
        return availableGifts;
    }
    
    /// <summary>
    /// 赠送礼物给好友
    /// </summary>
    /// <param name="friendUserId">好友用户ID</param>
    /// <param name="gift">礼物数据</param>
    /// <param name="message">附加消息</param>
    /// <returns>是否成功赠送</returns>
    public bool SendGift(string friendUserId, GiftData gift, string message = "")
    {
        // 验证：是否是好友
        if (!IsFriend(friendUserId))
        {
            Debug.LogWarning("[FriendManager] 只能给好友赠送礼物");
            return false;
        }
        
        // 验证：虚拟币是否足够
        if (ResourceManager.Instance.GetVirtualCoin() < gift.cost)
        {
            Debug.LogWarning($"[FriendManager] 虚拟币不足，需要{gift.cost}币");
            return false;
        }
        
        FriendData friend = GetFriend(friendUserId);
        
        // 扣除虚拟币
        ResourceManager.Instance.AddVirtualCoin(-gift.cost);
        
        // 创建礼物交易记录
        GiftTransactionData transaction = new GiftTransactionData(
            UserData.Instance.userId,
            UserData.Instance.playerName,
            friendUserId,
            friend.friendName,
            gift,
            message
        );
        
        giftTransactions.Add(transaction);
        TotalGiftsSent++;
        
        // TODO: 同步到Firebase并通知好友
        // FirebaseManager.SendGift(transaction);
        // 这会触发好友端的OnGiftReceived事件
        
        Debug.Log($"[FriendManager] 成功赠送【{gift.giftName}】给 {friend.friendName}");
        return true;
    }
    
    /// <summary>
    /// 接收礼物（由Firebase监听触发）
    /// </summary>
    /// <param name="transaction">礼物交易数据</param>
    public void ReceiveGift(GiftTransactionData transaction)
    {
        if (transaction.receiverUserId != UserData.Instance.userId)
        {
            Debug.LogWarning("[FriendManager] 礼物接收者不是当前玩家");
            return;
        }
        
        giftTransactions.Add(transaction);
        TotalGiftsReceived++;
        
        // 触发事件
        OnGiftReceived?.Invoke(transaction.senderName, transaction.giftName);
        
        // 发送到邮箱系统
        if (MailManager.Instance != null)
        {
            GiftData gift = availableGifts.FirstOrDefault(g => g.giftId == transaction.giftId);
            if (gift != null)
            {
                MailData mail = MailData.CreateGiftMail(
                    transaction.senderName,
                    transaction.giftName,
                    gift.moodBonus,
                    transaction.message,
                    transaction.senderUserId,
                    transaction.receiverUserId
                );
                MailManager.Instance.ReceiveMail(mail);
            }
        }
        
        Debug.Log($"[FriendManager] 收到来自 {transaction.senderName} 的礼物：{transaction.giftName}");
    }
    
    /// <summary>
    /// 获取礼物交易历史
    /// </summary>
    /// <param name="sent">true=发送的礼物，false=接收的礼物</param>
    /// <returns>交易记录列表</returns>
    public List<GiftTransactionData> GetGiftHistory(bool sent)
    {
        if (sent)
        {
            return giftTransactions
                .Where(t => t.senderUserId == UserData.Instance.userId)
                .ToList();
        }
        else
        {
            return giftTransactions
                .Where(t => t.receiverUserId == UserData.Instance.userId)
                .ToList();
        }
    }
    
    #endregion
    
    #region 好友请求查询
    
    /// <summary>
    /// 获取收到的好友请求列表
    /// </summary>
    /// <param name="pendingOnly">是否只获取待处理的请求</param>
    /// <returns>请求列表</returns>
    public List<FriendRequestData> GetReceivedRequests(bool pendingOnly = true)
    {
        if (pendingOnly)
        {
            return receivedRequests.Where(r => r.status == "pending").ToList();
        }
        return new List<FriendRequestData>(receivedRequests);
    }
    
    /// <summary>
    /// 获取发送的好友请求列表
    /// </summary>
    /// <param name="pendingOnly">是否只获取待处理的请求</param>
    /// <returns>请求列表</returns>
    public List<FriendRequestData> GetSentRequests(bool pendingOnly = true)
    {
        if (pendingOnly)
        {
            return sentRequests.Where(r => r.status == "pending").ToList();
        }
        return new List<FriendRequestData>(sentRequests);
    }
    
    /// <summary>
    /// 获取待处理请求数量
    /// </summary>
    /// <returns>数量</returns>
    public int GetPendingRequestCount()
    {
        return receivedRequests.Count(r => r.status == "pending");
    }
    
    #endregion
    
    #region 数据持久化
    
    /// <summary>
    /// 保存数据
    /// </summary>
    public void SaveData()
    {
        // TODO: 实现保存到Firebase
        Debug.Log("[FriendManager] 保存好友数据");
    }
    
    /// <summary>
    /// 加载数据
    /// </summary>
    public void LoadData()
    {
        // TODO: 实现从Firebase加载
        Debug.Log("[FriendManager] 加载好友数据");
        
        // 调试模式：创建一些测试好友
        if (debugMode)
        {
            CreateTestFriends();
        }
    }
    
    /// <summary>
    /// 创建测试好友（仅调试模式）
    /// </summary>
    private void CreateTestFriends()
    {
        friendsList.Add(new FriendData("test_001", "虚拟人小王", 15));
        friendsList.Add(new FriendData("test_002", "数据体小李", 20));
        friendsList.Add(new FriendData("test_003", "意识体小张", 10));
        
        Debug.Log("[FriendManager] 创建了3个测试好友");
    }
    
    #endregion
    
    #region 调试功能
    
    /// <summary>
    /// 模拟收到好友请求（仅调试）
    /// </summary>
    public void DebugReceiveFriendRequest()
    {
        if (!debugMode) return;
        
        FriendRequestData request = new FriendRequestData(
            $"test_{UnityEngine.Random.Range(100, 999)}",
            $"测试玩家{UnityEngine.Random.Range(1, 100)}",
            UnityEngine.Random.Range(1, 50),
            UserData.Instance.userId,
            "你好，交个朋友吧！"
        );
        
        receivedRequests.Add(request);
        OnFriendRequestReceived?.Invoke(request.senderName);
        
        Debug.Log($"[FriendManager] [调试] 模拟收到好友请求：{request.senderName}");
    }
    
    /// <summary>
    /// 模拟收到礼物（仅调试）
    /// </summary>
    public void DebugReceiveGift()
    {
        if (!debugMode || friendsList.Count == 0) return;
        
        FriendData randomFriend = friendsList[UnityEngine.Random.Range(0, friendsList.Count)];
        GiftData randomGift = availableGifts[UnityEngine.Random.Range(0, availableGifts.Length)];
        
        GiftTransactionData transaction = new GiftTransactionData(
            randomFriend.friendUserId,
            randomFriend.friendName,
            UserData.Instance.userId,
            UserData.Instance.playerName,
            randomGift,
            "送你个礼物！"
        );
        
        ReceiveGift(transaction);
    }
    
    /// <summary>
    /// 获取系统状态摘要（调试用）
    /// </summary>
    /// <returns>状态文本</returns>
    public string GetStatusSummary()
    {
        return $"好友系统状态:\n" +
               $"- 好友数量: {friendsList.Count}/{maxFriends}\n" +
               $"- 在线好友: {friendsList.Count(f => f.isOnline)}\n" +
               $"- 待处理请求: {GetPendingRequestCount()}\n" +
               $"- 发送礼物: {TotalGiftsSent}\n" +
               $"- 接收礼物: {TotalGiftsReceived}";
    }
    
    #endregion
}
