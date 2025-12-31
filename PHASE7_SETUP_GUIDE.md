# Phase 7 Unity操作指南 - 社交系统

**预计完成时间：40-50分钟**

---

## 📋 目录

1. [准备工作](#准备工作)
2. [步骤1：初始化管理器](#步骤1初始化管理器-5分钟)
3. [步骤2：创建测试UI](#步骤2创建测试ui-30分钟)
4. [步骤3：测试功能](#步骤3测试功能-10分钟)
5. [常见问题](#常见问题)
6. [验证清单](#验证清单)

---

## 准备工作

### 确认已完成的内容

确保以下文件已存在于项目中：

```
E-Citizens/Assets/Scripts/
├── Data/
│   ├── FriendData.cs ✅
│   ├── GiftData.cs ✅
│   ├── MailData.cs ✅
│   └── LeaderboardData.cs ✅
├── Managers/
│   ├── FriendManager.cs ✅
│   ├── MailManager.cs ✅
│   ├── LeaderboardManager.cs ✅
│   └── ChatManager.cs ✅
└── UI/
    └── Phase7TestUI.cs ✅
```

### 需要的Unity版本
- Unity 2021.3 或更高版本
- TextMeshPro已导入

---

## 步骤1：初始化管理器（5分钟）

### 1.1 创建管理器物体

1. **打开Unity项目**
2. **在Hierarchy中创建空物体**：
   - 右键 → Create Empty
   - 命名为 `SocialSystemManagers`
   - Position设置为 (0, 0, 0)

3. **添加管理器组件**：
   - 选中 `SocialSystemManagers`
   - 点击 Inspector 中的 **Add Component**
   - 搜索并添加以下组件（按顺序）：
     * `FriendManager`
     * `MailManager`
     * `LeaderboardManager`
     * `ChatManager`

4. **配置调试模式**（可选）：
   - 在每个Manager组件中
   - 勾选 `Debug Mode` 复选框
   - 这会创建测试数据方便测试

### 1.2 确认GameManager集成

确保GameManager会初始化社交系统：

```csharp
// 在GameManager.cs的Start()或Initialize()方法中添加：
FriendManager.Instance.Initialize();
MailManager.Instance.Initialize();
LeaderboardManager.Instance.Initialize();
ChatManager.Instance.Initialize();
```

---

## 步骤2：创建测试UI（30分钟）

### 2.1 创建Canvas和主面板

1. **创建Canvas**（如果还没有）：
   - 右键 Hierarchy → UI → Canvas
   - 命名为 `Phase7Canvas`
   - Canvas Scaler设置：
     * UI Scale Mode: Scale With Screen Size
     * Reference Resolution: 1920x1080

2. **创建主测试面板**：
   - 在Canvas下右键 → Create Empty
   - 命名为 `Phase7TestPanel`
   - RectTransform设置：
     * Anchor: Stretch (按Alt+Shift点击右下角预设)
     * Left/Right/Top/Bottom: 50, 50, 50, 50

3. **添加背景**：
   - 在Phase7TestPanel下添加 UI → Image
   - 命名为 `Background`
   - Color: 半透明黑色 (R:0, G:0, B:0, A:200)

### 2.2 创建状态显示区域

1. **创建状态文本**：
   - 在Phase7TestPanel下添加 UI → Text - TextMeshPro
   - 命名为 `StatusText`
   - RectTransform：
     * Anchor Preset: Top Stretch
     * Height: 150
     * Left/Right/Top: 10, 10, 10
   - TextMeshPro设置：
     * Font Size: 18
     * Color: 白色
     * Alignment: Left, Top
     * Overflow: Overflow

### 2.3 创建滚动详情区域

1. **创建ScrollView**：
   - 在Phase7TestPanel下添加 UI → Scroll View
   - 命名为 `DetailScrollView`
   - RectTransform：
     * Anchor: Stretch
     * Left/Right/Top/Bottom: 10, 10, 170, 300
   
2. **配置ScrollView**：
   - Scroll Rect组件：
     * Horizontal: 关闭
     * Vertical: 开启
     * Movement Type: Elastic

3. **修改Content下的详情文本**：
   - 选中 ScrollView → Viewport → Content
   - Content RectTransform：
     * Anchor: Top Stretch
     * Pivot: X=0.5, Y=1
     * Left/Right/Top: 5, 5, 0
   - 删除Content下的Text，添加新的Text - TextMeshPro：
     * 命名为 `DetailText`
     * RectTransform：
       - Anchor: Top Stretch
       - Left/Right/Top: 0, 0, 0
       - Height: 2000
     * TextMeshPro设置：
       - Font Size: 16
       - Color: 白色
       - Alignment: Left, Top

### 2.4 创建按钮区域

创建一个按钮容器：

1. **创建按钮区域**：
   - 在Phase7TestPanel下创建Empty
   - 命名为 `ButtonArea`
   - RectTransform：
     * Anchor: Bottom Stretch
     * Height: 280
     * Left/Right/Bottom: 10, 10, 10

2. **创建按钮网格**（使用Grid Layout Group）：
   - 选中ButtonArea
   - Add Component → Layout → Grid Layout Group
   - Grid Layout Group设置：
     * Cell Size: X=220, Y=40
     * Spacing: X=10, Y=10
     * Start Axis: Horizontal
     * Child Alignment: Upper Left

### 2.5 创建所有按钮

在ButtonArea下创建以下按钮（共19个）：

**好友系统按钮（5个）：**
1. `btnSendFriendRequest` - "模拟收到请求"
2. `btnShowFriendList` - "查看好友列表"
3. `btnShowFriendRequests` - "查看好友请求"
4. `btnAcceptFirstRequest` - "接受第一个请求"
5. `btnSendGift` - "赠送礼物"

**邮件系统按钮（4个）：**
6. `btnShowInbox` - "查看收件箱"
7. `btnClaimAllRewards` - "领取所有附件"
8. `btnMarkAllRead` - "全部标记已读"
9. `btnSendTestMail` - "发送测试邮件"

**排行榜按钮（4个）：**
10. `btnShowWealth` - "财富榜"
11. `btnShowLevel` - "等级榜"
12. `btnShowMood` - "心情榜"
13. `btnUpdateRanking` - "更新排名"

**聊天系统按钮（3个）：**
14. `btnShowConversations` - "查看会话"
15. `btnSendMessage` - "发送消息"
16. `btnShowUnread` - "未读消息"

**综合功能按钮（2个）：**
17. `btnRefreshAll` - "刷新全部"
18. `btnShowSystemStatus` - "系统状态"

**创建按钮的快速方法：**

1. 在ButtonArea下添加 UI → Button - TextMeshPro
2. 命名为对应的按钮名（如：btnSendFriendRequest）
3. 选中Button的子对象Text (TMP)，修改文本为对应的显示名称
4. TextMeshPro设置：
   - Font Size: 14
   - Color: 黑色
   - Alignment: Center, Middle
5. Button的Image组件：
   - Color设置为淡蓝色 (R:200, G:220, B:255, A:255)

**复制创建技巧：**
- 创建第一个按钮后
- Ctrl+D 复制
- 修改名称和文本即可

### 2.6 连接脚本引用

1. **添加Phase7TestUI脚本**：
   - 选中 `Phase7TestPanel`
   - Add Component → Phase7TestUI

2. **连接所有引用**：
   
   **主面板：**
   - Status Text → 拖入StatusText
   - Scroll View → 拖入DetailScrollView
   - Detail Text → 拖入DetailScrollView/Viewport/Content/DetailText

   **好友系统按钮：**
   - Btn Send Friend Request → 拖入btnSendFriendRequest
   - Btn Show Friend List → 拖入btnShowFriendList
   - Btn Show Friend Requests → 拖入btnShowFriendRequests
   - Btn Accept First Request → 拖入btnAcceptFirstRequest
   - Btn Send Gift → 拖入btnSendGift

   **邮件系统按钮：**
   - Btn Show Inbox → 拖入btnShowInbox
   - Btn Claim All Rewards → 拖入btnClaimAllRewards
   - Btn Mark All Read → 拖入btnMarkAllRead
   - Btn Send Test Mail → 拖入btnSendTestMail

   **排行榜按钮：**
   - Btn Show Wealth → 拖入btnShowWealth
   - Btn Show Level → 拖入btnShowLevel
   - Btn Show Mood → 拖入btnShowMood
   - Btn Update Ranking → 拖入btnUpdateRanking

   **聊天系统按钮：**
   - Btn Show Conversations → 拖入btnShowConversations
   - Btn Send Message → 拖入btnSendMessage
   - Btn Show Unread → 拖入btnShowUnread

   **综合功能按钮：**
   - Btn Refresh All → 拖入btnRefreshAll
   - Btn Show System Status → 拖入btnShowSystemStatus

---

## 步骤3：测试功能（10分钟）

### 3.1 运行游戏

1. **保存场景**：Ctrl+S
2. **运行游戏**：点击Play按钮

### 3.2 测试好友系统

1. 点击 **"模拟收到请求"**
   - 应该看到提示：模拟收到好友请求
   - 状态栏显示 `请求: 1`

2. 点击 **"查看好友请求"**
   - 详情区域显示待处理的请求信息

3. 点击 **"接受第一个请求"**
   - 提示：已添加好友
   - 状态栏显示 `好友: 1`

4. 点击 **"查看好友列表"**
   - 详情区域显示好友列表

5. 点击 **"赠送礼物"**
   - 提示：已赠送礼物
   - 虚拟币减少

### 3.3 测试邮件系统

1. 点击 **"发送测试邮件"**
   - 提示：发送了测试邮件
   - 状态栏显示 `未读1`

2. 点击 **"查看收件箱"**
   - 详情区域显示邮件列表

3. 点击 **"领取所有附件"**
   - 提示：领取了X封邮件的附件
   - 虚拟币增加

4. 点击 **"全部标记已读"**
   - 状态栏显示 `未读0`

### 3.4 测试排行榜系统

1. 点击 **"更新排名"**
   - 提示：已更新排行榜排名

2. 点击 **"财富榜"**
   - 详情区域显示财富排行榜
   - 显示当前玩家排名

3. 点击 **"等级榜"、"心情榜"**
   - 查看不同排行榜

### 3.5 测试聊天系统

1. 点击 **"发送消息"**
   - 提示：已发送消息给XXX

2. 点击 **"查看会话"**
   - 详情区域显示聊天会话列表

3. 点击 **"未读消息"**
   - 显示有未读消息的会话

### 3.6 测试综合功能

1. 点击 **"系统状态"**
   - 详情区域显示所有系统的统计数据

2. 点击 **"刷新全部"**
   - 所有数据刷新

---

## 常见问题

### Q1: 按钮点击没有反应
**A**: 检查以下几点：
1. Phase7TestUI脚本是否正确添加到Phase7TestPanel
2. 所有按钮是否正确连接到脚本引用
3. Console是否有错误信息
4. EventSystem是否存在（UI → Event System）

### Q2: 显示文本乱码或不显示
**A**: 
1. 确认使用的是TextMeshPro组件
2. 检查TMP字体是否支持中文
3. 如果字体不支持，导入中文字体：
   - Window → TextMeshPro → Font Asset Creator
   - 选择支持中文的字体
   - 生成Font Asset

### Q3: 管理器报NullReferenceException
**A**:
1. 确保所有Manager都已正确初始化
2. 检查GameManager是否调用了Initialize()
3. 确认UserData.Instance和ResourceManager.Instance存在

### Q4: 测试数据不显示
**A**:
1. 在Manager组件中勾选 `Debug Mode`
2. 重新运行游戏
3. 检查LoadData()方法是否正确执行

### Q5: 好友/礼物/邮件功能不工作
**A**:
1. 确认FriendManager、MailManager已正确初始化
2. 检查Console是否有警告信息
3. 确认调试模式已开启

---

## 验证清单

测试完成后，确认以下功能正常工作：

### ✅ 好友系统
- [ ] 可以模拟收到好友请求
- [ ] 可以查看好友请求列表
- [ ] 可以接受好友请求
- [ ] 可以查看好友列表
- [ ] 可以赠送礼物给好友

### ✅ 邮件系统
- [ ] 可以发送测试邮件
- [ ] 可以查看收件箱
- [ ] 可以领取邮件附件
- [ ] 可以标记邮件为已读
- [ ] 邮件数量正确显示

### ✅ 排行榜系统
- [ ] 可以查看财富榜
- [ ] 可以查看等级榜
- [ ] 可以查看心情榜
- [ ] 可以更新排名
- [ ] 玩家排名正确显示

### ✅ 聊天系统
- [ ] 可以发送消息给好友
- [ ] 可以查看聊天会话
- [ ] 可以查看未读消息
- [ ] 消息数量正确显示

### ✅ 综合功能
- [ ] 状态栏实时更新
- [ ] 系统状态显示正确
- [ ] 所有事件触发正常
- [ ] Console无错误信息

---

## 下一步

完成测试后，你可以：

1. **查看PHASE7_SUMMARY.md** - 了解系统详细设计
2. **开始Phase 8** - 商业化系统（商城、支付）
3. **优化UI** - 创建更精美的社交界面
4. **集成Firebase** - 实现真实的多玩家互动

---

## 技术支持

如遇到问题：
1. 检查Console的详细错误信息
2. 查看PHASE7_SUMMARY.md的常见问题章节
3. 确认所有前置Phase（1-6）都已正确完成

**恭喜你完成Phase 7社交系统的Unity操作！** 🎉

社交系统为游戏增加了丰富的互动性，让玩家可以：
- 结交好友并互相赠送礼物
- 通过邮件接收系统通知和奖励
- 在排行榜上竞争并获得奖励
- 与好友私聊交流

继续加油！距离完成项目只剩2个阶段了！💪
