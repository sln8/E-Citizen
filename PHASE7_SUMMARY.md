# Phase 7 开发总结 - 社交系统

本文档详细说明Phase 7社交系统的功能、使用方法和数据平衡。

---

## 📋 目录

1. [系统概览](#系统概览)
2. [好友系统](#好友系统)
3. [邮件系统](#邮件系统)
4. [排行榜系统](#排行榜系统)
5. [聊天系统](#聊天系统)
6. [集成指南](#集成指南)
7. [数据平衡](#数据平衡)
8. [常见问题](#常见问题)

---

## 系统概览

Phase 7实现了完整的社交系统，包含4个核心子系统：

| 系统 | 主要功能 | 管理器 | 数据类 |
|------|---------|--------|--------|
| 好友 | 添加好友、赠送礼物 | FriendManager | FriendData, GiftData |
| 邮件 | 收发邮件、领取附件 | MailManager | MailData |
| 排行榜 | 排名竞技、周奖励 | LeaderboardManager | LeaderboardData |
| 聊天 | 私聊消息、会话管理 | ChatManager | ChatMessageData |

**系统关系图**：
```
好友系统 ←→ 礼物系统 → 邮件系统
    ↓           ↓           ↓
聊天系统 ← 好友列表 → 排行榜
```

---

## 好友系统

### 功能说明

#### 1. 添加好友
```csharp
// 发送好友请求
bool success = FriendManager.Instance.SendFriendRequest(
    targetUserId: "player_123",
    message: "你好，交个朋友吧！"
);
```

#### 2. 处理请求
```csharp
// 接受请求
FriendManager.Instance.AcceptFriendRequest(requestId);

// 拒绝请求
FriendManager.Instance.RejectFriendRequest(requestId);
```

#### 3. 好友管理
```csharp
// 获取好友列表
List<FriendData> friends = FriendManager.Instance.GetFriendList();

// 获取在线好友
List<FriendData> onlineFriends = FriendManager.Instance.GetFriendList(onlineOnly: true);

// 删除好友
FriendManager.Instance.RemoveFriend(friendUserId);
```

### 礼物系统

#### 预定义礼物列表

| ID | 名称 | 品级 | 成本 | 心情加成 | 性价比 |
|----|------|------|------|----------|--------|
| gift_mood_small | 数据花束 | 普通 | 50币 | +10 | 0.20 |
| gift_mood_medium | 虚拟巧克力 | 普通 | 100币 | +20 | 0.20 |
| gift_memory_stone | 记忆水晶 | 精良 | 300币 | +60 | 0.20 |
| gift_mood_large | 心情大礼包 | 精良 | 500币 | +100 | 0.20 |
| gift_vacation_ticket | 虚拟假期券 | 史诗 | 800币 | +150 | 0.19 |
| gift_digital_pet | 迷你数据宠物 | 史诗 | 1000币 | +200 | 0.20 |
| gift_lucky_star | 幸运之星 | 传说 | 2000币 | +500 | 0.25 |

#### 赠送礼物
```csharp
// 获取可用礼物
GiftData[] gifts = FriendManager.Instance.GetAvailableGifts();

// 赠送礼物
bool success = FriendManager.Instance.SendGift(
    friendUserId: "friend_001",
    gift: gifts[0],
    message: "送你个礼物！"
);
```

### 事件监听

```csharp
void Start()
{
    // 好友列表更新
    FriendManager.Instance.OnFriendListUpdated += () => {
        Debug.Log("好友列表已更新");
        RefreshFriendUI();
    };
    
    // 收到好友请求
    FriendManager.Instance.OnFriendRequestReceived += (senderName) => {
        ShowNotification($"收到来自 {senderName} 的好友请求");
    };
    
    // 收到礼物
    FriendManager.Instance.OnGiftReceived += (senderName, giftName) => {
        ShowNotification($"{senderName} 赠送了 {giftName}");
    };
}
```

---

## 邮件系统

### 邮件类型

#### 1. 系统邮件
```csharp
MailManager.Instance.SendSystemMail(
    title: "维护公告",
    content: "游戏将于今晚维护...",
    receiverId: userId
);
```

#### 2. 工资邮件
```csharp
MailManager.Instance.SendSalaryMail(
    companyName: "数据公司",
    amount: 500,
    receiverId: userId
);
```

#### 3. 礼物邮件（自动发送）
```csharp
// 赠送礼物时自动创建邮件
FriendManager.Instance.SendGift(...);
// → 自动发送礼物邮件到好友邮箱
```

#### 4. 奖励邮件
```csharp
MailManager.Instance.SendRewardMail(
    questName: "新手任务",
    coinReward: 1000,
    items: "{\"memory_1gb\":1}",
    receiverId: userId
);
```

### 邮件操作

#### 查看邮件
```csharp
// 获取收件箱
List<MailData> inbox = MailManager.Instance.GetInbox();

// 获取未读邮件
List<MailData> unread = MailManager.Instance.GetUnreadMails();

// 获取有附件的邮件
List<MailData> withRewards = MailManager.Instance.GetMailsWithUnclaimedRewards();
```

#### 领取附件
```csharp
// 领取单个邮件
MailManager.Instance.ClaimMailReward(mailId);

// 一键领取所有
int count = MailManager.Instance.ClaimAllRewards();
```

#### 批量操作
```csharp
// 全部标记已读
MailManager.Instance.MarkAllAsRead();

// 删除已读邮件
MailManager.Instance.DeleteAllReadAndClaimed();
```

---

## 排行榜系统

### 排行榜类型

```csharp
public enum LeaderboardType
{
    Wealth,      // 财富榜（虚拟币）
    Level,       // 等级榜
    Mood,        // 心情榜
    OnlineTime   // 在线时长榜
}
```

### 查询排名

```csharp
// 获取前10名
List<LeaderboardEntryData> topPlayers = 
    LeaderboardManager.Instance.GetTopPlayers(LeaderboardType.Wealth, 10);

// 获取玩家排名
int rank = LeaderboardManager.Instance.GetPlayerRank(LeaderboardType.Wealth);

// 获取玩家周围的排名
List<LeaderboardEntryData> nearby = 
    LeaderboardManager.Instance.GetPlayersAroundPlayer(LeaderboardType.Wealth, range: 2);
```

### 更新排名

```csharp
// 手动更新
LeaderboardManager.Instance.UpdatePlayerRanking();

// 自动更新（每5分钟）
// 在Manager的Update()中自动执行
```

### 周奖励

```csharp
// 发放周奖励（每周一调用）
LeaderboardManager.Instance.DistributeWeeklyRewards();
```

**奖励规则**：
- 第1名：1000币
- 第2名：800币
- 第3名：600币
- 4-10名：400币

---

## 聊天系统

### 发送消息

```csharp
// 发送消息给好友
bool success = ChatManager.Instance.SendMessage(
    friendUserId: "friend_001",
    content: "你好，最近怎么样？"
);
```

### 查看会话

```csharp
// 获取所有会话
List<ChatConversationData> conversations = 
    ChatManager.Instance.GetAllConversations();

// 获取有未读消息的会话
List<ChatConversationData> unread = 
    ChatManager.Instance.GetConversationsWithUnread();

// 获取与特定好友的会话
ChatConversationData conv = ChatManager.Instance.GetConversation("friend_001");
```

### 未读管理

```csharp
// 获取总未读数
int totalUnread = ChatManager.Instance.GetTotalUnreadCount();

// 标记会话为已读
ChatManager.Instance.MarkConversationAsRead("friend_001");
```

---

## 集成指南

### 初始化顺序

```csharp
void Start()
{
    // 1. 初始化基础系统
    UserData.Instance.Initialize();
    ResourceManager.Instance.Initialize();
    
    // 2. 初始化社交系统
    FriendManager.Instance.Initialize();
    MailManager.Instance.Initialize();
    LeaderboardManager.Instance.Initialize();
    ChatManager.Instance.Initialize();
}
```

### 与其他系统集成

#### 工作系统 → 邮件系统
```csharp
// 工作结算时发送邮件
void OnJobCompleted(string companyName, int salary)
{
    MailManager.Instance.SendSalaryMail(companyName, salary, UserData.Instance.userId);
}
```

#### 任务系统 → 邮件系统
```csharp
// 任务完成时发送奖励邮件
void OnQuestCompleted(string questName, int reward)
{
    MailManager.Instance.SendRewardMail(questName, reward, "", UserData.Instance.userId);
}
```

#### 资源系统 → 排行榜系统
```csharp
// 虚拟币变化时更新排行榜
ResourceManager.Instance.OnVirtualCoinChanged += (newAmount) => {
    LeaderboardManager.Instance.UpdatePlayerRanking();
};
```

---

## 数据平衡

### 好友系统平衡

| 项目 | 数值 | 说明 |
|------|------|------|
| 最大好友数 | 100 | 可扩展 |
| 最大待处理请求 | 50 | 防止刷屏 |
| 礼物成本范围 | 50-2000币 | 适合不同阶段 |
| 心情加成范围 | 10-500点 | 性价比0.19-0.25 |

### 邮件系统平衡

| 项目 | 数值 | 说明 |
|------|------|------|
| 最大邮件数 | 200封 | 自动清理 |
| 自动删除 | 30天 | 已读且已领取 |
| 欢迎奖励 | 100币 | 新手福利 |

### 排行榜平衡

| 项目 | 数值 | 说明 |
|------|------|------|
| 更新间隔 | 5分钟 | 实时性与性能平衡 |
| 显示数量 | 100名 | 足够覆盖 |
| 周奖励总额 | 6600币 | 前10名总和 |

### 聊天系统平衡

| 项目 | 数值 | 说明 |
|------|------|------|
| 消息保存 | 500条/会话 | 防止过多占用 |
| 无限制数量 | - | 会话数量不限 |

---

## 常见问题

### Q1: 如何添加新的礼物？

**A**: 编辑 `GiftData.cs`，在 `CreateDefaultGifts()` 方法中添加：

```csharp
new GiftData(
    "gift_new",        // ID
    "新礼物",          // 名称
    "描述",            // 描述
    300,               // 成本
    50,                // 心情加成
    "rare"             // 品级
)
```

### Q2: 如何创建新的邮件类型？

**A**: 在 `MailData.cs` 中添加静态工厂方法：

```csharp
public static MailData CreateMyCustomMail(...)
{
    MailData mail = new MailData("custom", title, content, receiverId);
    // 设置附件等
    return mail;
}
```

### Q3: 排行榜何时更新？

**A**: 有两种更新方式：
1. 自动更新：每5分钟
2. 手动更新：调用 `UpdatePlayerRanking()`

### Q4: 如何监听社交事件？

**A**: 订阅管理器的事件：

```csharp
FriendManager.Instance.OnFriendListUpdated += HandleFriendListUpdate;
MailManager.Instance.OnNewMailReceived += HandleNewMail;
// 记得在OnDestroy中取消订阅
```

### Q5: 好友和聊天数据存储在哪？

**A**: 当前存储在内存中，集成Firebase后将同步到云端：
- 好友列表 → Firestore: `users/{userId}/friends`
- 聊天记录 → Firestore: `chats/{conversationId}`

---

## 下一步

完成Phase 7后，你可以：

1. **Phase 8 - 商业化系统**
   - 实现商城和支付
   - 首充和月卡系统
   
2. **Phase 9 - 打磨与测试**
   - UI优化
   - 性能优化
   - 上线准备

3. **Firebase集成**
   - 实现真实的多玩家互动
   - 云端数据同步

---

**恭喜完成Phase 7社交系统开发！** 🎉

这个系统为游戏增加了丰富的社交互动，提升了玩家粘性和留存率。继续加油！💪
