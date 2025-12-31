# Phase 5 Unity操作指南 - 生活系统

## 📘 概述

本指南将帮助你在Unity中完成Phase 5生活系统的配置和测试。整个过程预计需要**30-45分钟**。

---

## 🎯 目标

完成以下任务：
1. ✅ 在Unity中添加生活系统管理器
2. ✅ 创建完整的测试UI界面
3. ✅ 测试房产、汽车、宠物功能
4. ✅ 验证自动扣租金和心情加成

---

## 📋 前提条件

在开始之前，请确保：
- ✅ Unity项目已打开
- ✅ 已完成Phase 1-4的配置
- ✅ 以下管理器已添加到场景：
  - ResourceManager ✓
  - GameTimerManager ✓
  - JobManager ✓
  - SkillManager ✓
  - CompanyManager ✓
  - TalentMarketManager ✓

---

## 第一部分：添加生活系统管理器（5分钟）

### Step 1: 找到GameManager对象

1. 打开Unity项目
2. 在**Hierarchy**窗口中查找`GameManager`对象
3. 如果没有找到，创建一个：
   ```
   Hierarchy右键 → Create Empty
   命名为"GameManager"
   ```

### Step 2: 添加LifeSystemManager组件

1. 选中`GameManager`对象
2. 在**Inspector**窗口中点击**Add Component**
3. 搜索`LifeSystemManager`
4. 点击添加

### Step 3: 验证管理器

在Inspector中确认`LifeSystemManager`脚本已成功添加，应该看到：
- Available Housings (空列表)
- Available Vehicles (空列表)
- Available Pets (空列表)
- Current Housing (None)
- Owned Vehicles (空列表)
- Owned Pets (空列表)

### Step 4: 保存场景

按`Ctrl+S`保存场景

---

## 第二部分：创建测试UI（20-25分钟）

### Step 1: 创建Canvas（如果还没有）

1. 在**Hierarchy**窗口右键
2. 选择`UI → Canvas`
3. Canvas自动命名为"Canvas"或"UI Canvas"

#### 配置Canvas Scaler

1. 选中Canvas
2. 在Inspector中找到`Canvas Scaler`组件
3. 设置以下参数：
   ```
   UI Scale Mode: Scale With Screen Size
   Reference Resolution: 
     X: 1920
     Y: 1080
   Match: 0.5
   ```

### Step 2: 创建测试面板

1. 在Canvas下右键
2. 选择`UI → Panel`
3. 命名为`LifeSystemTestPanel`

#### 配置面板

1. 选中LifeSystemTestPanel
2. 在Inspector中设置：
   ```
   Rect Transform:
     Anchor: 拉到左上角
     Position X: 320
     Position Y: -400
     Width: 640
     Height: 800
   
   Image (背景色):
     Color: 半透明灰色 (R:0, G:0, B:0, A:200)
   ```

### Step 3: 添加测试UI脚本

1. 选中`LifeSystemTestPanel`
2. 在Inspector中点击**Add Component**
3. 搜索`LifeSystemTestUI`
4. 点击添加

### Step 4: 创建状态显示文本

1. 在LifeSystemTestPanel下右键
2. 选择`UI → Text - TextMeshPro`
3. 命名为`StatusText`

> **注意**：如果是第一次使用TextMeshPro，Unity会提示导入TMP Essentials，点击**Import**

#### 配置StatusText

```
Rect Transform:
  Anchor: 左上角
  Position X: 10
  Position Y: -10
  Width: 620
  Height: 550

TextMeshPro - Text:
  Text: (留空，将通过脚本更新)
  Font Size: 14
  Alignment: 左上对齐
  Color: 白色
  Overflow: Overflow (允许超出)
  Wrapping: 启用
```

### Step 5: 创建按钮（13个）

我们将创建13个按钮，分为4组。

#### 按钮通用设置

每个按钮的基础设置：
```
Size: 160 x 40
Text Font Size: 14
```

#### 创建步骤（重复13次）

1. 在LifeSystemTestPanel下右键
2. 选择`UI → Button - TextMeshPro`
3. 按照下表命名和定位

#### 按钮列表和布局

**第1行 - 房产按钮**（Y = -570）
| 序号 | 对象名 | 按钮文本 | Position X |
|------|--------|---------|-----------|
| 1 | RentCapsuleButton | 租胶囊公寓 | 90 |
| 2 | RentApartmentButton | 租普通公寓 | 260 |
| 3 | BuyVillaButton | 买独栋别墅 | 430 |

**第2行 - 房产+汽车**（Y = -620）
| 序号 | 对象名 | 按钮文本 | Position X |
|------|--------|---------|-----------|
| 4 | BuyMansionButton | 买数据豪宅 | 90 |
| 5 | BuySkateboardButton | 买数据滑板 | 260 |
| 6 | BuySportsCarButton | 买光速跑车 | 430 |

**第3行 - 汽车+宠物**（Y = -670）
| 序号 | 对象名 | 按钮文本 | Position X |
|------|--------|---------|-----------|
| 7 | BuyQuantumButton | 买量子飞行器 | 90 |
| 8 | BuyDogButton | 买数据犬 | 260 |
| 9 | BuyCatButton | 买赛博猫 | 430 |

**第4行 - 宠物+工具**（Y = -720）
| 序号 | 对象名 | 按钮文本 | Position X |
|------|--------|---------|-----------|
| 10 | BuyDragonButton | 买像素龙 | 90 |
| 11 | RefreshButton | 刷新显示 | 260 |
| 12 | AddMoneyButton | 添加测试资金 | 430 |

**第5行 - 额外工具**（Y = -770）
| 序号 | 对象名 | 按钮文本 | Position X |
|------|--------|---------|-----------|
| 13 | ShowAllButton | 显示所有可用项 | 90 |

#### 快速创建技巧

为了加快速度，你可以：
1. 创建第一个按钮并完全配置好
2. 复制（Ctrl+D）12次
3. 逐个修改名称、文本和位置

### Step 6: 连接UI引用

1. 选中`LifeSystemTestPanel`
2. 在Inspector中找到`LifeSystemTestUI`脚本
3. 将UI元素从Hierarchy拖到对应字段：

```
Status Text → StatusText

房产操作按钮：
- Rent Capsule Button → RentCapsuleButton
- Rent Apartment Button → RentApartmentButton
- Buy Villa Button → BuyVillaButton
- Buy Mansion Button → BuyMansionButton

汽车操作按钮：
- Buy Skateboard Button → BuySkateboardButton
- Buy Sports Car Button → BuySportsCarButton
- Buy Quantum Button → BuyQuantumButton

宠物操作按钮：
- Buy Dog Button → BuyDogButton
- Buy Cat Button → BuyCatButton
- Buy Dragon Button → BuyDragonButton

通用操作按钮：
- Refresh Button → RefreshButton
- Add Money Button → AddMoneyButton
- Show All Button → ShowAllButton
```

### Step 7: 保存

1. 保存场景（Ctrl+S）
2. 保存项目（Ctrl+Shift+S）

---

## 第三部分：测试功能（10-15分钟）

### 基础测试流程

#### Test 1: 启动游戏

1. 点击Unity编辑器顶部的**Play按钮**▶
2. 观察Console窗口的日志：
   ```
   [LifeSystemManager] 初始化生活系统...
   [LifeSystemManager] 数据库初始化完成: 4个房产, 3个汽车, 3个宠物
   [LifeSystemManager] 自动分配初始房产: 胶囊公寓
   [LifeSystemManager] 已订阅游戏周期事件
   [LifeSystemManager] 生活系统初始化完成
   ```

#### Test 2: 添加测试资金

1. 点击**"添加测试资金"**按钮
2. 观察StatusText更新：虚拟币增加100,000
3. 检查Console日志：
   ```
   [ResourceManager] 添加虚拟币: 100000 (来源: 测试)
   ```

#### Test 3: 租赁房产

1. 点击**"租普通公寓"**按钮
2. 观察StatusText的"当前房产"部分更新
3. 检查Console日志：
   ```
   [LifeSystemManager] 成功租赁: 普通公寓, 租金5币/5分钟
   ```
4. 预期结果：
   - 名称：普通公寓
   - 类型：租赁
   - 租金：5币/5分钟
   - 心情加成：+1/5分钟
   - 家具槽位：5个

#### Test 4: 购买汽车

1. 点击**"买数据滑板"**按钮
2. 观察变化：
   - 虚拟币减少500
   - 心情值增加5
   - 拥有的汽车列表显示"数据滑板 [使用中]"
3. 检查Console日志：
   ```
   [LifeSystemManager] 成功购买: 数据滑板, 花费500币, 获得5心情值
   ```

#### Test 5: 购买宠物

1. 点击**"买数据犬"**按钮
2. 观察变化：
   - 虚拟币减少1000
   - 拥有的宠物列表显示"数据犬 (+0.5/5分钟)"
3. 检查Console日志：
   ```
   [LifeSystemManager] 成功购买: 数据犬, 花费1000币, 每5分钟+0.5心情值
   ```

#### Test 6: 等待自动周期

这是最重要的测试！

**如果GameTimerManager处于调试模式（周期30秒）**：
1. 等待30秒
2. 观察Console日志：
   ```
   [GameTimerManager] ===== 游戏周期 #X =====
   [LifeSystemManager] 处理生活系统周期结算...
   [LifeSystemManager] 支付租金: 5币 (累计5币)
   [LifeSystemManager] 应用心情加成: +1.5 (房产+1, 宠物+0.5)
   [LifeSystemManager] 生活系统周期结算完成
   ```

**如果GameTimerManager处于正常模式（周期5分钟）**：
1. 等待5分钟（可以做其他事情）
2. 观察相同的日志

3. 点击**"刷新显示"**按钮
4. 验证：
   - 虚拟币减少了5（租金）
   - 心情值增加了1.5（房产1 + 宠物0.5，四舍五入为2）
   - 统计数据显示：
     - 累计租金：5币
     - 累计心情：+2（四舍五入）

#### Test 7: 多周期测试

1. 等待2-3个游戏周期
2. 每次周期后点击**"刷新显示"**
3. 验证：
   - 租金持续扣除（5币/周期）
   - 心情值持续增加（+2/周期）
   - 统计数据正确累计

### 进阶测试

#### Test 8: 测试所有房产

依次尝试：
1. 租胶囊公寓（最便宜，无心情加成）
2. 租普通公寓（有心情加成）
3. 买独栋别墅（需要50,000币）
4. ~~买数据豪宅~~（需要真实货币，暂未实现）

#### Test 9: 测试所有汽车

1. 买数据滑板（500币）
2. 买光速跑车（10,000币）
3. ~~买量子飞行器~~（需要真实货币，暂未实现）

观察拥有的汽车列表

#### Test 10: 测试所有宠物

1. 买数据犬（1,000币）
2. 买赛博猫（1,500币）
3. ~~买像素龙~~（需要真实货币，暂未实现）

观察心情加成叠加：
- 1只宠物：+0.5/周期
- 2只宠物：+1.3/周期（0.5 + 0.8）

#### Test 11: 购买别墅

1. 确保有至少50,000虚拟币
2. 点击**"买独栋别墅"**
3. 观察：
   - 虚拟币减少50,000
   - 当前房产变为"独栋别墅"
   - 类型变为"已购买"（不再有租金）
   - 心情加成变为+3/5分钟
   - 家具槽位增加到15个

4. 等待下一个周期
5. 验证：**不再扣租金**

#### Test 12: 显示所有可用项

1. 点击**"显示所有可用项"**按钮
2. 打开**Console**窗口
3. 查看详细的房产、汽车、宠物列表
4. 这包括每个项目的完整描述

---

## 第四部分：故障排除

### 问题1: 点击按钮没反应

**症状**：点击任何按钮都没有效果

**解决方案**：
1. 检查Console是否有错误信息
2. 确认LifeSystemManager已添加到场景
3. 确认按钮的OnClick事件是否已连接
4. 尝试重启Unity编辑器

### 问题2: 无法找到LifeSystemManager

**症状**：添加组件时找不到LifeSystemManager

**解决方案**：
1. 确认脚本文件存在：`Assets/Scripts/Managers/LifeSystemManager.cs`
2. 检查脚本是否有编译错误
3. 点击Unity菜单：`Assets → Reimport All`

### 问题3: 自动周期不触发

**症状**：等待很久也不扣租金和加心情

**解决方案**：
1. 检查GameTimerManager是否在运行
2. 查看Console是否有游戏周期日志
3. 确认GameTimerManager.debugMode设置：
   - true = 30秒周期
   - false = 5分钟周期

### 问题4: UI显示不正确

**症状**：StatusText显示乱码或重叠

**解决方案**：
1. 调整StatusText的Width和Height
2. 启用TextMeshPro的Overflow选项
3. 减小Font Size（建议12-14）

### 问题5: 虚拟币变成负数

**症状**：租金扣除后虚拟币为负

**解决方案**：
- 这是预期行为（当前版本允许负数）
- 使用"添加测试资金"按钮补充虚拟币
- 未来版本会添加破产机制

---

## 第五部分：验证清单

完成以下清单以确保一切正常：

### 管理器验证
- [ ] LifeSystemManager已添加到GameManager对象
- [ ] 运行游戏时看到初始化日志
- [ ] 自动分配了胶囊公寓作为初始房产

### UI验证
- [ ] 所有13个按钮都已创建
- [ ] StatusText显示正常
- [ ] 所有UI引用已正确连接

### 功能验证
- [ ] 可以添加测试资金
- [ ] 可以租赁房产
- [ ] 可以购买汽车
- [ ] 可以购买宠物
- [ ] 自动扣租金正常
- [ ] 自动加心情正常
- [ ] 统计数据正确

### 数据验证
- [ ] 房产信息显示正确
- [ ] 汽车列表显示正确
- [ ] 宠物列表显示正确
- [ ] 每周期收支计算正确

---

## 🎓 学习建议

### 对于零基础开发者

**完成后，你应该理解**：
1. Unity UI系统的基本使用
2. 脚本如何与UI交互
3. 事件系统的工作原理
4. 游戏周期和自动化系统

**进一步学习**：
1. 阅读`LifeSystemManager.cs`的中文注释
2. 尝试修改房产/汽车/宠物的数值
3. 尝试添加新的房产类型
4. 学习如何创建更复杂的UI

### 对于有经验的开发者

**关注点**：
1. 事件驱动架构的实现
2. 数据驱动设计的优势
3. 自动化系统的集成方式
4. Unity UI的性能优化

---

## 📊 预期结果

完成本指南后，你应该：

### 能够做到
✅ 在Unity中测试完整的生活系统  
✅ 购买和管理房产、汽车、宠物  
✅ 观察自动扣租金和心情加成  
✅ 理解游戏的经济循环  

### 实际体验
- **租赁系统**：每5分钟自动扣费，类似订阅服务
- **购买系统**：一次性投资，长期收益
- **心情系统**：持续加成，影响收入
- **统计系统**：追踪累计支出和收益

---

## 🚀 下一步

完成Phase 5后，你可以：

1. **继续Phase 6**：娱乐与战斗系统
   - 虚拟世界体验
   - 病毒入侵塔防
   - 汽车速度加成将在这里生效

2. **创建正式UI**：
   - 替换测试UI为美观的正式UI
   - 添加过渡动画
   - 使用图标和美术资源

3. **数值调整**：
   - 根据测试结果调整价格
   - 平衡心情加成
   - 优化游戏经济

---

## 📞 获取帮助

如果遇到问题：
1. 查看Console的详细错误信息
2. 参考PHASE5_SUMMARY.md的"常见问题"章节
3. 检查所有引用是否正确连接
4. 尝试重启Unity编辑器

---

**祝测试顺利！** 🎮

如果一切正常，你会看到一个功能完整的生活系统，玩家可以租房、买车、养宠物，享受数字化生活！

---

**文档版本**：Phase 5 v1.0  
**更新时间**：2025-12-31  
**预计完成时间**：30-45分钟  
**难度等级**：⭐⭐⭐ (中等)
