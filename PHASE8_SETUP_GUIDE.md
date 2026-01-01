# Phase 8 Unity操作指南 - 商城与支付系统

**适用对象**: 零基础开发者  
**预计时间**: 约60-90分钟  
**前置条件**: 已完成Phase 1-7的配置

---

## 📋 准备工作

### 检查清单
- [ ] Unity项目已打开
- [ ] Phase 1-7的Manager已创建（ResourceManager、GameTimerManager等）
- [ ] Console窗口已打开（Window → General → Console）
- [ ] Hierarchy和Inspector窗口可见

---

## 🔧 第一步：创建Manager物体（约10分钟）

### 1.1 创建商城管理器

1. **在Hierarchy中右键** → Create Empty
2. **重命名**为 `ShopManager`
3. **添加脚本**：
   - 点击Inspector底部的 `Add Component`
   - 搜索 `ShopManager`
   - 选择 `ShopManager` 脚本
4. **验证**：Inspector中应该显示Shop Manager (Script)组件

### 1.2 创建支付管理器

1. **在Hierarchy中右键** → Create Empty
2. **重命名**为 `PaymentManager`
3. **添加脚本**：
   - 点击Inspector底部的 `Add Component`
   - 搜索 `PaymentManager`
   - 选择 `PaymentManager` 脚本
4. **配置Inspector参数**：
   ```
   Is Test Mode: ✓（勾选）
   Test Mode Auto Success: ✓（勾选）
   Test Mode Delay: 1.5
   ```
5. **说明**：测试模式下无需真实支付，自动模拟成功

### 1.3 创建月卡管理器

1. **在Hierarchy中右键** → Create Empty
2. **重命名**为 `MonthlyCardManager`
3. **添加脚本**：
   - 点击Inspector底部的 `Add Component`
   - 搜索 `MonthlyCardManager`
   - 选择 `MonthlyCardManager` 脚本

### 1.4 验证Manager创建

**运行游戏**（点击Play按钮），查看Console：
```
=== 商城系统初始化 ===
商城初始化完成，共加载 14 件商品
  - 配置升级: 5 件
  - 外观装扮: 2 件
  - 虚拟币礼包: 4 件
  - 工具消耗品: 3 件

=== 支付系统初始化 ===
支付系统运行在测试模式
  - 自动成功: True
  - 模拟延迟: 1.5秒

=== 月卡系统初始化 ===
当前无激活月卡
```

如果看到这些信息，说明Manager创建成功！

---

## 🎨 第二步：创建测试UI（约40-50分钟）

### 2.1 创建Canvas（如果还没有）

1. **Hierarchy中右键** → UI → Canvas
2. **选中Canvas**，在Inspector中设置：
   ```
   Render Mode: Screen Space - Overlay
   Canvas Scaler:
     UI Scale Mode: Scale With Screen Size
     Reference Resolution: 1920 x 1080
   ```

### 2.2 创建主面板

1. **在Canvas下右键** → UI → Panel
2. **重命名**为 `Phase8TestPanel`
3. **设置Rect Transform**：
   - Anchor Presets: 拉伸（Stretch）
   - Left: 0, Right: 0, Top: 0, Bottom: 0
4. **设置Image组件**（可选）：
   - Color: 半透明灰色 (R:50, G:50, B:50, A:200)

### 2.3 创建状态显示区域

#### 2.3.1 创建状态Text

1. **在Phase8TestPanel下右键** → UI → Text - TextMeshPro
   - 如果提示导入TMP Essentials，点击 `Import TMP Essentials`
2. **重命名**为 `StatusText`
3. **设置Rect Transform**：
   - Anchor: 左上角
   - Pos X: 300, Pos Y: -100
   - Width: 500, Height: 800
4. **设置TextMeshProUGUI组件**：
   ```
   Font Size: 18
   Color: 白色
   Alignment: 左上对齐
   Overflow: Overflow（或使用Scroll View）
   ```

#### 2.3.2 创建日志Text

1. **在Phase8TestPanel下右键** → UI → Scroll View
2. **重命名Scroll View**为 `LogScrollView`
3. **设置Rect Transform**：
   - Anchor: 右上角到右下角
   - Pos X: -350, Pos Y: 0
   - Width: 600, Height: 全屏（Top: 50, Bottom: 50）
4. **找到Content下的Text**，重命名为 `LogText`
5. **设置LogText**：
   ```
   Font Size: 14
   Color: 浅绿色 (R:200, G:255, B:200)
   Alignment: 左上对齐
   ```
6. **设置Content的Content Size Fitter**：
   - Vertical Fit: Preferred Size

### 2.4 创建按钮（共20个）

由于按钮较多，我们按功能分组创建。

#### 创建按钮的通用步骤：
1. **在Phase8TestPanel下右键** → UI → Button - TextMeshPro
2. **重命名**按钮
3. **调整位置和大小**
4. **修改按钮文本**（在Button下的Text (TMP)子对象中）

#### 按钮布局建议：

```
【左侧区域 - 商城测试】（X: 50, Y从-50开始，每个按钮间隔70）
┌────────────────────┐
│ 显示所有商品        │ ← btnShowAllItems (Y: -50)
│ 显示配置商品        │ ← btnShowConfigItems (Y: -120)
│ 显示虚拟币礼包      │ ← btnShowCoinPacks (Y: -190)
│ 购买内存升级        │ ← btnBuyMemory (Y: -260)
│ 购买CPU升级         │ ← btnBuyCPU (Y: -330)
│ 购买虚拟币小包      │ ← btnBuySmallPack (Y: -400)
│ 显示购买历史        │ ← btnShowPurchaseHistory (Y: -470)
└────────────────────┘

【中间区域 - 月卡测试】（X: 50, Y从-550开始）
┌────────────────────┐
│ 购买基础月卡        │ ← btnBuyBasicCard (Y: -550)
│ 购买豪华月卡        │ ← btnBuyPremiumCard (Y: -620)
│ 领取每日奖励        │ ← btnClaimDaily (Y: -690)
│ 检查月卡状态        │ ← btnCheckCardStatus (Y: -760)
│ 显示领取历史        │ ← btnShowClaimHistory (Y: -830)
└────────────────────┘

【左下区域 - 首充测试】（X: 50, Y从-910开始）
┌────────────────────┐
│ 显示首充信息        │ ← btnShowFirstCharge (Y: -910)
│ 购买首充礼包        │ ← btnBuyFirstCharge (Y: -980)
│ 领取首充奖励        │ ← btnClaimFirstCharge (Y: -1050)
└────────────────────┘

【底部区域 - 支付和通用】（横向排列，Y: -1130）
┌──────┬──────┬──────┬──────┬──────┐
│支付统计│测试模式│模拟失败│刷新状态│清空日志│
└──────┴──────┴──────┴──────┴──────┘
  X:50   X:170   X:290   X:410   X:530
```

#### 快速创建方法：

1. **创建第一个按钮**：
   - 在Phase8TestPanel下右键 → UI → Button - TextMeshPro
   - 重命名为 `btnShowAllItems`
   - 设置位置：Anchor左上角，Pos X: 150, Pos Y: -50
   - 设置大小：Width: 200, Height: 50
   - 修改文本为 "显示所有商品"

2. **复制按钮**：
   - 选中btnShowAllItems
   - Ctrl+D（复制）
   - 重命名为下一个按钮名
   - 调整Y坐标（-70）
   - 修改按钮文本

3. **重复步骤2**，创建其余19个按钮

#### 按钮列表（用于对照）：

**商城测试（7个）**：
1. btnShowAllItems - "显示所有商品"
2. btnShowConfigItems - "显示配置商品"
3. btnShowCoinPacks - "显示虚拟币礼包"
4. btnBuyMemory - "购买内存升级"
5. btnBuyCPU - "购买CPU升级"
6. btnBuySmallPack - "购买虚拟币小包"
7. btnShowPurchaseHistory - "显示购买历史"

**月卡测试（5个）**：
8. btnBuyBasicCard - "购买基础月卡"
9. btnBuyPremiumCard - "购买豪华月卡"
10. btnClaimDaily - "领取每日奖励"
11. btnCheckCardStatus - "检查月卡状态"
12. btnShowClaimHistory - "显示领取历史"

**首充测试（3个）**：
13. btnShowFirstCharge - "显示首充信息"
14. btnBuyFirstCharge - "购买首充礼包"
15. btnClaimFirstCharge - "领取首充奖励"

**支付测试（3个）**：
16. btnShowPaymentStats - "支付统计"
17. btnToggleTestMode - "切换测试模式"
18. btnSimulateFailure - "模拟支付失败"

**通用按钮（2个）**：
19. btnRefreshStatus - "刷新状态"
20. btnClearLog - "清空日志"

### 2.5 添加测试UI脚本

1. **选中Phase8TestPanel**
2. **添加脚本**：
   - Inspector底部 → Add Component
   - 搜索 `Phase8TestUI`
   - 选择脚本

3. **连接UI引用**（非常重要！）：
   - 在Inspector中找到Phase8TestUI脚本组件
   - 依次将UI元素拖拽到对应的引用槽：

   ```
   Status Text: 拖入StatusText
   Log Text: 拖入LogText
   Log Scroll Rect: 拖入LogScrollView
   
   【商城测试按钮】
   Btn Show All Items: 拖入btnShowAllItems
   Btn Show Config Items: 拖入btnShowConfigItems
   Btn Show Coin Packs: 拖入btnShowCoinPacks
   Btn Buy Memory: 拖入btnBuyMemory
   Btn Buy CPU: 拖入btnBuyCPU
   Btn Buy Small Pack: 拖入btnBuySmallPack
   Btn Show Purchase History: 拖入btnShowPurchaseHistory
   
   【月卡测试按钮】
   Btn Buy Basic Card: 拖入btnBuyBasicCard
   Btn Buy Premium Card: 拖入btnBuyPremiumCard
   Btn Claim Daily: 拖入btnClaimDaily
   Btn Check Card Status: 拖入btnCheckCardStatus
   Btn Show Claim History: 拖入btnShowClaimHistory
   
   【首充测试按钮】
   Btn Show First Charge: 拖入btnShowFirstCharge
   Btn Buy First Charge: 拖入btnBuyFirstCharge
   Btn Claim First Charge: 拖入btnClaimFirstCharge
   
   【支付测试按钮】
   Btn Show Payment Stats: 拖入btnShowPaymentStats
   Btn Toggle Test Mode: 拖入btnToggleTestMode
   Btn Simulate Failure: 拖入btnSimulateFailure
   
   【通用按钮】
   Btn Refresh Status: 拖入btnRefreshStatus
   Btn Clear Log: 拖入btnClearLog
   ```

4. **验证连接**：
   - 所有引用槽都应显示对应的UI元素
   - 如果有 `None (Game Object)` 或 `None (Text Mesh Pro UGUI)`，说明未正确连接

---

## ✅ 第三步：测试功能（约10-15分钟）

### 3.1 运行游戏

1. **点击Play按钮**启动游戏
2. **观察Console**，应该看到：
   ```
   === Phase 8 测试UI已启动 ===
   提示：点击按钮测试各项功能
   ```
3. **观察StatusText**，应该显示系统状态

### 3.2 测试商城功能

#### 测试1：查看商品
1. **点击 "显示所有商品"** 按钮
2. **在LogText中查看**，应该显示14件商品
3. **点击 "显示虚拟币礼包"**，查看4种礼包

#### 测试2：购买配置升级
1. **确认当前虚拟币**（StatusText中显示）
2. **点击 "购买内存升级"**
3. **观察Log**，应该显示：
   ```
   [时间] 购买内存升级...
   [时间] [事件] 商品购买成功: 内存 +1GB
   [时间] [事件] 商品效果已应用: 内存 +1GB
   [时间] ✓ 内存升级购买成功！
   ```
4. **查看StatusText**，虚拟币应减少100

#### 测试3：购买虚拟币礼包
1. **点击 "购买虚拟币小包"**
2. **等待1.5秒**（模拟支付延迟）
3. **观察Log**，应该显示：
   ```
   [时间] 购买虚拟币小包...
   [时间] [事件] 支付成功: coin_pack_1 - $0.99
   [时间] [事件] 商品购买成功: 虚拟币小包
   [时间] [事件] 商品效果已应用: 虚拟币小包
   [时间] ✓ 虚拟币小包购买成功！
   ```
4. **查看StatusText**，虚拟币应增加1000

### 3.3 测试月卡功能

#### 测试1：购买月卡
1. **点击 "购买基础月卡"**
2. **等待支付完成**
3. **观察Log**，应该显示：
   ```
   [时间] 购买基础月卡...
   [时间] [事件] 支付成功: monthly_basic - $4.99
   [时间] [事件] 月卡购买成功: 基础月卡
   [时间] [事件] 月卡已激活: 基础月卡，有效期 30 天
   [时间] ✓ 基础月卡购买成功！
   ```
4. **查看StatusText**，月卡状态应更新

#### 测试2：领取每日奖励
1. **点击 "领取每日奖励"**
2. **观察Log**，应该显示：
   ```
   [时间] 领取每日奖励...
   [时间] [事件] 每日奖励已领取: 200币 + 1清理工具
   [时间] ✓ 每日奖励领取成功！
   ```
3. **查看StatusText**，虚拟币应增加200

#### 测试3：再次领取（应失败）
1. **再次点击 "领取每日奖励"**
2. **应该显示**：
   ```
   [时间] 领取每日奖励...
   [时间] ✗ 领取失败（今天已领取或无月卡）
   ```

### 3.4 测试首充功能

#### 测试1：查看首充信息
1. **点击 "显示首充信息"**
2. **在Log中查看**，应该显示详细的首充礼包信息

#### 测试2：购买首充
1. **点击 "购买首充礼包"**
2. **等待支付完成**
3. **应该显示购买成功**

#### 测试3：领取首充奖励
1. **点击 "领取首充奖励"**
2. **观察虚拟币增加5000**
3. **查看Log**，显示所有奖励物品

#### 测试4：再次购买（应失败）
1. **再次点击 "购买首充礼包"**
2. **应该显示**：
   ```
   ✗ 首充已购买，无法重复购买
   ```

### 3.5 测试支付功能

#### 测试1：查看支付统计
1. **点击 "支付统计"**
2. **在Console查看**，显示详细统计信息

#### 测试2：模拟支付失败
1. **点击 "模拟支付失败"**（开启失败模拟）
2. **尝试购买商品**
3. **应该失败**
4. **再次点击 "模拟支付失败"**（关闭失败模拟）
5. **购买应该成功**

---

## 🐛 常见问题排查

### 问题1：Manager初始化失败

**症状**：Console显示 "XXXManager未初始化"

**解决方案**：
1. 确认Manager物体已创建
2. 确认脚本已正确添加
3. 尝试重启Unity

### 问题2：按钮点击无反应

**症状**：点击按钮没有Log输出

**解决方案**：
1. 检查按钮引用是否正确连接到Phase8TestUI
2. 确认Phase8TestPanel上有Phase8TestUI脚本
3. 检查EventSystem是否存在（Canvas创建时自动添加）

### 问题3：虚拟币不足

**症状**：购买商品时提示虚拟币不足

**解决方案**：
1. 先购买虚拟币礼包
2. 或在ResourceManager中手动添加虚拟币：
   ```csharp
   ResourceManager.Instance.AddVirtualCoin(10000, "测试");
   ```

### 问题4：StatusText不更新

**症状**：状态显示不刷新

**解决方案**：
1. 点击 "刷新状态" 按钮
2. 检查StatusText引用是否正确
3. 确认游戏正在运行（Play模式）

### 问题5：Log显示不完整

**症状**：Log文本被截断

**解决方案**：
1. 调整LogScrollView的高度
2. 使用ScrollView的滚动条查看更多内容
3. 点击 "清空日志" 清理旧Log

---

## 📊 验证清单

完成所有操作后，请对照以下清单验证：

### Manager创建
- [ ] ShopManager物体已创建并添加脚本
- [ ] PaymentManager物体已创建并添加脚本（测试模式已开启）
- [ ] MonthlyCardManager物体已创建并添加脚本
- [ ] 运行游戏时Console显示3个Manager的初始化信息

### UI创建
- [ ] Canvas已创建
- [ ] Phase8TestPanel已创建
- [ ] StatusText已创建并正确放置
- [ ] LogScrollView和LogText已创建
- [ ] 20个测试按钮已创建并正确命名
- [ ] Phase8TestUI脚本已添加到主面板
- [ ] 所有UI引用已正确连接（无None引用）

### 功能测试
- [ ] 可以查看所有商品
- [ ] 可以购买配置升级（虚拟币扣除）
- [ ] 可以购买虚拟币礼包（真实货币模拟支付）
- [ ] 可以购买月卡并激活
- [ ] 可以领取每日奖励（今天只能领取一次）
- [ ] 可以购买首充礼包
- [ ] 可以领取首充奖励（只能领取一次）
- [ ] 可以查看购买历史和统计信息
- [ ] StatusText实时显示系统状态
- [ ] LogText记录所有操作日志

---

## 🎓 学习资源

### Unity基础
- Unity官方教程：https://learn.unity.com/
- Unity中文文档：https://docs.unity.cn/

### C#脚本
- C#入门教程：https://www.runoob.com/csharp/
- Unity脚本API：https://docs.unity3d.com/ScriptReference/

### 问题求助
- Unity官方论坛：https://forum.unity.com/
- Unity中文社区：https://connect.unity.com/

---

## 📝 下一步

完成Phase 8后，你可以：

1. **测试完整游戏流程**：
   - 从登录开始
   - 购买配置升级
   - 购买月卡领取奖励
   - 体验完整商业化循环

2. **准备Phase 9**：
   - UI/UX优化
   - 性能优化
   - 真实IAP集成
   - 最终测试和发布

3. **查看其他文档**：
   - PHASE8_SUMMARY.md - 详细功能说明
   - PHASE8_COMPLETION_REPORT.md - 完成报告

---

**祝你开发顺利！** 🚀

如有问题，请查看Console错误信息或参考PHASE8_SUMMARY.md中的API使用示例。

**《电子公民》开发团队**
