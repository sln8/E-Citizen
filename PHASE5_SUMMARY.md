# Phase 5 开发总结 - 生活系统

## 📋 概述

Phase 5 实现了《电子公民》游戏的**生活系统**，包括房产、汽车、宠物三大核心模块。玩家可以租赁/购买房产、购买汽车提升速度、养宠物获得持续心情加成。

---

## ✅ 已完成的功能

### 1. 房产数据类 (HousingData.cs)

**功能**：
- 支持两种房产类型：租赁和购买
- 存储房产的所有属性
- 提供心情值加成
- 支持家具槽位系统

**房产列表**：
| 名称 | 类型 | 费用 | 心情加成 | 家具槽位 | 解锁等级 |
|------|------|------|---------|---------|---------|
| 胶囊公寓 | 租赁 | 2币/5分钟 | 0 | 2个 | Lv.1 |
| 普通公寓 | 租赁 | 5币/5分钟 | +1/5分钟 | 5个 | Lv.1 |
| 独栋别墅 | 购买 | 50,000币 | +3/5分钟 | 15个 | Lv.10 |
| 数据豪宅 | 购买 | 500,000币+$9.99 | +10/5分钟 | 30个 | Lv.20 |

**关键方法**：
```csharp
// 创建租赁房产
HousingData.CreateRentalHousing(id, name, rent, moodBonus, interior, exterior, slots);

// 创建购买房产
HousingData.CreatePurchaseHousing(id, name, price, moodBonus, interior, exterior, slots);

// 检查是否能负担
housing.CanAfford(playerLevel, virtualCoin, hasRealMoney);

// 获取描述
string desc = housing.GetDescription();
```

---

### 2. 汽车数据类 (VehicleData.cs)

**功能**：
- 提供速度加成（缩短娱乐时间）
- 购买时获得一次性心情值加成
- 支持真实货币购买

**汽车列表**：
| 名称 | 价格 | 速度加成 | 心情加成 | 节省时间 | 解锁等级 |
|------|------|---------|---------|---------|---------|
| 数据滑板 | 500币 | 1.1x | +5 | 9.1% | Lv.1 |
| 光速跑车 | 10,000币 | 1.5x | +20 | 33.3% | Lv.5 |
| 量子飞行器 | 100,000币+$4.99 | 2.0x | +50 | 50% | Lv.15 |

**速度加成计算**：
```
实际时间 = 基础时间 / 速度加成
例如：10分钟的娱乐，使用光速跑车(1.5x) = 10 / 1.5 = 6.67分钟
```

**关键方法**：
```csharp
// 创建汽车
VehicleData.CreateVehicle(id, name, price, speed, mood, icon);

// 计算实际时间
float actualTime = vehicle.CalculateActualTime(baseTime);

// 获取时间节省百分比
float saving = vehicle.GetTimeSavingPercentage(); // 返回 0-100
```

---

### 3. 宠物数据类 (PetData.cs)

**功能**：
- 持续提供心情值加成（每5分钟）
- 支持Spine动画展示
- 预留饱食度和快乐度系统（未来扩展）

**宠物列表**：
| 名称 | 价格 | 心情加成 | 每小时加成 | 每天加成 | 解锁等级 |
|------|------|---------|-----------|---------|---------|
| 数据犬 | 1,000币 | +0.5/5分钟 | +6/小时 | +144/天 | Lv.1 |
| 赛博猫 | 1,500币 | +0.8/5分钟 | +9.6/小时 | +230/天 | Lv.1 |
| 像素龙 | 50,000币+$2.99 | +2.0/5分钟 | +24/小时 | +576/天 | Lv.10 |

**关键方法**：
```csharp
// 创建宠物
PetData.CreatePet(id, name, price, moodBonus, animation);

// 获取各种时间维度的加成
float hourly = pet.GetHourlyMoodBonus();
float daily = pet.GetDailyMoodBonus();

// 计算性价比
float value = pet.GetValueRatio(); // 每1000币带来的每小时心情加成
```

---

### 4. 生活系统管理器 (LifeSystemManager.cs)

**功能**：
- 管理玩家的房产、汽车、宠物
- 自动扣除租金（每5分钟）
- 自动应用心情加成（每5分钟）
- 与资源管理器无缝集成
- 完整的事件系统

**核心方法**：

#### 房产管理
```csharp
// 租赁房产
bool success = LifeSystemManager.Instance.RentHousing("rent_apartment");

// 购买房产
bool success = LifeSystemManager.Instance.BuyHousing("buy_house");

// 获取当前房产
HousingData current = LifeSystemManager.Instance.GetCurrentHousing();

// 获取所有可用房产
List<HousingData> all = LifeSystemManager.Instance.GetAvailableHousings();
```

#### 汽车管理
```csharp
// 购买汽车
bool success = LifeSystemManager.Instance.BuyVehicle("car_sport");

// 切换当前使用的汽车
bool success = LifeSystemManager.Instance.SetActiveVehicle("car_basic");

// 获取当前使用的汽车
VehicleData active = LifeSystemManager.Instance.GetActiveVehicle();

// 获取拥有的所有汽车
List<VehicleData> owned = LifeSystemManager.Instance.GetOwnedVehicles();
```

#### 宠物管理
```csharp
// 购买宠物
bool success = LifeSystemManager.Instance.BuyPet("pet_dog");

// 获取拥有的所有宠物
List<PetData> pets = LifeSystemManager.Instance.GetOwnedPets();
```

#### 辅助方法
```csharp
// 获取当前速度加成（用于娱乐系统）
float speedBonus = LifeSystemManager.Instance.GetCurrentSpeedBonus();

// 获取统计数据
var (totalRent, totalMood) = LifeSystemManager.Instance.GetStatistics();
```

**事件系统**：
```csharp
// 订阅事件
LifeSystemManager.Instance.OnHousingChanged += (housing) => { ... };
LifeSystemManager.Instance.OnVehicleBought += (vehicle) => { ... };
LifeSystemManager.Instance.OnVehicleChanged += (vehicle) => { ... };
LifeSystemManager.Instance.OnPetBought += (pet) => { ... };
LifeSystemManager.Instance.OnRentPaid += (amount) => { ... };
LifeSystemManager.Instance.OnMoodBonusApplied += (amount) => { ... };
```

**自动处理**：
- 每个游戏周期（5分钟）自动执行：
  1. 扣除租金（如果是租赁房产）
  2. 应用心情加成（房产+宠物）
  3. 触发相应事件

---

### 5. 生活系统测试UI (LifeSystemTestUI.cs)

**功能**：
- 完整的测试界面
- 13个操作按钮
- 实时状态显示
- 事件监听和响应

**按钮列表**：

**房产操作**（4个）：
- 租胶囊公寓
- 租普通公寓
- 买独栋别墅
- 买数据豪宅

**汽车操作**（3个）：
- 买数据滑板
- 买光速跑车
- 买量子飞行器

**宠物操作**（3个）：
- 买数据犬
- 买赛博猫
- 买像素龙

**通用操作**（3个）：
- 刷新显示
- 添加测试资金（+100,000币）
- 显示所有可用项

**状态显示**：
- 玩家信息（等级、虚拟币、心情值）
- 当前房产信息
- 拥有的汽车列表
- 拥有的宠物列表
- 统计数据（累计租金、累计心情）
- 每周期收支预览

---

## 📊 数据平衡设计

### 房产性价比分析

**租赁房产**（按1小时计算）：
| 房产 | 租金/小时 | 心情/小时 | 性价比 |
|------|----------|----------|--------|
| 胶囊公寓 | 24币 | 0 | 0 |
| 普通公寓 | 60币 | +12 | 0.2心情/币 |

**购买房产**（按100小时回本计算）：
| 房产 | 成本 | 心情/小时 | 100小时总心情 | 等效价值 |
|------|------|----------|--------------|---------|
| 独栋别墅 | 50,000币 | +36 | +3,600 | 13.9币/心情 |
| 数据豪宅 | 500,000币 | +120 | +12,000 | 41.7币/心情 |

**建议**：
- 前期使用普通公寓（低成本，有心情加成）
- 中期购买别墅（一次性投资，长期收益）
- 后期购买豪宅（顶级体验）

---

### 汽车投资回报

**速度收益**（节省娱乐时间）：
| 汽车 | 成本 | 速度 | 节省时间 | 心情加成 |
|------|------|------|---------|---------|
| 数据滑板 | 500币 | 1.1x | 9.1% | +5 |
| 光速跑车 | 10,000币 | 1.5x | 33.3% | +20 |
| 量子飞行器 | 100,000币 | 2.0x | 50% | +50 |

**性价比**（心情加成/成本）：
- 数据滑板：0.01心情/币
- 光速跑车：0.002心情/币
- 量子飞行器：0.0005心情/币

**建议**：
- 数据滑板最高性价比，推荐优先购买
- 光速跑车平衡选择（速度提升显著）
- 量子飞行器土豪专属（炫耀用）

---

### 宠物长期收益

**每天收益**（按288个5分钟周期）：
| 宠物 | 成本 | 日心情 | 回本天数 | 年收益 |
|------|------|--------|---------|--------|
| 数据犬 | 1,000币 | +144 | 7天 | +52,560 |
| 赛博猫 | 1,500币 | +230 | 7天 | +84,120 |
| 像素龙 | 50,000币 | +576 | 87天 | +210,240 |

**性价比排名**：
1. 赛博猫（0.153心情/币/天）
2. 数据犬（0.144心情/币/天）
3. 像素龙（0.0115心情/币/天）

**建议**：
- 赛博猫性价比最高，推荐优先购买
- 可以多只宠物并存，心情加成叠加
- 像素龙适合高级玩家（收集）

---

## 💡 游戏机制说明

### 心情值系统

**心情值影响**：
```
收入加成 = 心情值 / 100 * 1%

示例：
心情值 +300 → +3% 收入加成
心情值 -200 → -2% 收入加成
```

**心情值来源**：
1. **房产**：每5分钟持续加成
2. **宠物**：每5分钟持续加成
3. **汽车**：购买时一次性加成
4. **娱乐活动**：完成时一次性加成（Phase 6实现）
5. **好友礼物**：收到时一次性加成（Phase 7实现）

**心情值消耗**：
1. **工作/管理公司**：每5分钟-2（Phase 3已实现）

---

### 租金系统

**自动扣除**：
- 每个游戏周期（5分钟）自动扣除
- 如果虚拟币不足，会在Console输出警告
- 未来可扩展：无法支付租金会被驱逐

**租金计算**：
```
每小时租金 = 租金/5分钟 × 12
每天租金 = 租金/5分钟 × 288
```

**示例**：
- 普通公寓：5币/5分钟 = 60币/小时 = 1,440币/天

---

### 速度加成系统

**用途**：
- 缩短娱乐活动时间（Phase 6会用到）
- 提升游戏体验效率

**计算公式**：
```
实际时间 = 基础时间 / 速度加成

示例：
基础娱乐时间：10分钟
使用光速跑车（1.5x）：10 / 1.5 = 6.67分钟
使用量子飞行器（2.0x）：10 / 2.0 = 5分钟
```

---

## 🎯 Unity操作指南

### 第一步：添加管理器

1. 打开Unity项目
2. 在Hierarchy中找到`GameManager`对象（如果没有，创建一个空GameObject）
3. 添加组件：
   - 点击"Add Component"
   - 搜索"LifeSystemManager"
   - 添加该脚本
4. 确保以下管理器也已添加：
   - `ResourceManager` ✓
   - `GameTimerManager` ✓
   - `JobManager` ✓
   - `SkillManager` ✓
   - `CompanyManager` ✓
   - `TalentMarketManager` ✓
5. 保存场景（Ctrl+S）

---

### 第二步：创建测试UI

#### 1. 创建Canvas（如果还没有）
```
Hierarchy右键 → UI → Canvas
Canvas命名为"UI Canvas"
设置Canvas Scaler：
- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1920 x 1080
```

#### 2. 创建测试面板
```
在Canvas下创建：
右键 → UI → Panel
命名为"LifeSystemTestPanel"
```

#### 3. 添加脚本
```
选中LifeSystemTestPanel
在Inspector中点击"Add Component"
搜索"LifeSystemTestUI"
添加该脚本
```

#### 4. 创建状态显示文本
```
在LifeSystemTestPanel下创建：
右键 → UI → Text - TextMeshPro
命名为"StatusText"
设置：
- Anchor: 左上角
- Position: (20, -20)
- Width: 600
- Height: 800
- Font Size: 16
- Alignment: 左上对齐
```

#### 5. 创建按钮（13个）

**布局**：
- 每行3个按钮
- 按钮大小：160x40
- 间距：10

**创建步骤**（重复13次）：
```
在LifeSystemTestPanel下创建：
右键 → UI → Button - TextMeshPro
命名和设置文本（见下表）
```

**按钮列表和命名**：
| 序号 | 命名 | 按钮文本 | 位置Y |
|------|------|---------|-------|
| 1 | RentCapsuleButton | 租胶囊公寓 | -50 |
| 2 | RentApartmentButton | 租普通公寓 | -50 |
| 3 | BuyVillaButton | 买独栋别墅 | -50 |
| 4 | BuyMansionButton | 买数据豪宅 | -100 |
| 5 | BuySkateboardButton | 买数据滑板 | -100 |
| 6 | BuySportsCarButton | 买光速跑车 | -100 |
| 7 | BuyQuantumButton | 买量子飞行器 | -150 |
| 8 | BuyDogButton | 买数据犬 | -150 |
| 9 | BuyCatButton | 买赛博猫 | -150 |
| 10 | BuyDragonButton | 买像素龙 | -200 |
| 11 | RefreshButton | 刷新显示 | -200 |
| 12 | AddMoneyButton | 添加测试资金 | -200 |
| 13 | ShowAllButton | 显示所有可用项 | -250 |

#### 6. 连接UI引用

1. 选中`LifeSystemTestPanel`
2. 在Inspector中找到`LifeSystemTestUI`脚本
3. 将UI元素拖到对应字段：
   - statusText → StatusText
   - rentCapsuleButton → RentCapsuleButton
   - rentApartmentButton → RentApartmentButton
   - ... (其他按钮同理)
4. 保存场景

---

### 第三步：测试功能

#### 基础测试流程（10分钟）

1. **运行游戏**（点击Play按钮）

2. **添加测试资金**
   ```
   点击"添加测试资金"按钮
   获得100,000虚拟币
   观察StatusText更新
   ```

3. **租赁房产**
   ```
   点击"租普通公寓"按钮
   检查Console日志：
   - [LifeSystemManager] 成功租赁: 普通公寓...
   观察StatusText显示当前房产信息
   ```

4. **购买汽车**
   ```
   点击"买数据滑板"按钮
   检查：
   - 虚拟币减少500
   - 心情值增加5
   - 汽车列表显示"数据滑板 [使用中]"
   ```

5. **购买宠物**
   ```
   点击"买数据犬"按钮
   检查：
   - 虚拟币减少1000
   - 宠物列表显示"数据犬 (+0.5/5分钟)"
   ```

6. **等待游戏周期**
   ```
   等待30秒（调试模式）或5分钟（正常模式）
   观察Console日志：
   - [LifeSystemManager] 支付租金: 5币
   - [LifeSystemManager] 应用心情加成: +1.5
   点击"刷新显示"查看最新状态
   ```

7. **查看统计数据**
   ```
   在StatusText底部查看：
   - 累计租金：XX币
   - 累计心情：+XX
   - 净支出：XX币/5分钟
   ```

#### 进阶测试（20分钟）

8. **测试所有房产**
   ```
   依次测试：
   - 租胶囊公寓（最便宜）
   - 租普通公寓（有心情加成）
   - 买独栋别墅（需要50,000币）
   - 买数据豪宅（需要500,000币）
   ```

9. **测试所有汽车**
   ```
   依次购买：
   - 数据滑板
   - 光速跑车
   - 量子飞行器
   观察拥有的汽车列表
   ```

10. **测试所有宠物**
    ```
    依次购买：
    - 数据犬
    - 赛博猫
    - 像素龙
    观察宠物心情加成叠加
    ```

11. **压力测试**
    ```
    - 添加多次测试资金
    - 购买所有物品
    - 等待多个周期
    - 观察长期收支变化
    ```

---

## 🐛 常见问题

### Q1: 为什么点击按钮没反应？
**A**: 检查以下几点：
1. LifeSystemManager是否添加到场景中
2. UI按钮是否正确连接到脚本
3. Console是否有错误信息
4. ResourceManager是否正常工作

### Q2: 为什么扣除租金后虚拟币变成负数？
**A**: 当前版本允许负数，未来可以添加破产机制。建议在测试前添加足够资金。

### Q3: 如何加快测试速度？
**A**: 在GameTimerManager中：
- 设置`debugMode = true`（周期变为30秒）
- 设置`timeScale = 5`（时间流速5倍）
- 最快6秒触发一次周期

### Q4: 为什么购买豪宅失败？
**A**: 豪宅需要真实货币，当前版本暂未实现支付接口。将在Phase 8实现。

### Q5: 宠物的Spine动画在哪里？
**A**: 当前版本暂未包含Spine动画资源，仅保存了动画路径。需要在Phase 9添加美术资源。

---

## 📝 代码统计

### 文件列表
```
Phase 5 代码量：
├── HousingData.cs: 310行
├── VehicleData.cs: 275行
├── PetData.cs: 325行
├── LifeSystemManager.cs: 650行
└── LifeSystemTestUI.cs: 425行
总计：1,985行

累计项目代码量：
├── Phase 1: 800行
├── Phase 2: 1,300行
├── Phase 3: 3,200行
├── Phase 4: 3,140行
└── Phase 5: 1,985行
总计：10,425行（全部含详细中文注释）

完成进度：5/9 阶段 (56%)
```

---

## 🎓 设计亮点

### 1. 数据驱动设计
- 所有配置通过数据类管理
- 易于添加新的房产/汽车/宠物
- 支持游戏设计师调整数值

### 2. 事件驱动架构
- 管理器之间松耦合
- UI通过事件响应变化
- 易于扩展新功能

### 3. 自动化系统
- 自动扣租金
- 自动加心情
- 与游戏定时器无缝集成

### 4. 完整的验证
- 购买前检查等级、虚拟币
- 防止重复购买
- 友好的错误提示

### 5. 灵活的扩展性
- 预留家具系统接口
- 预留宠物饱食度系统
- 预留真实货币支付接口

---

## 🚀 下一步计划

### Phase 6: 娱乐与战斗系统（预计2-3周）

**娱乐系统**：
- [ ] 娱乐活动数据类（4种虚拟世界）
- [ ] 娱乐管理器
- [ ] 时间计算（使用汽车速度加成）
- [ ] 心情值奖励

**病毒入侵系统**：
- [ ] 病毒数据类
- [ ] 病毒入侵触发机制
- [ ] 塔防小游戏
- [ ] 安全卫士订阅系统
- [ ] 失败惩罚和成功奖励

---

## 📖 学习建议

### 对于初学者

**第一天**：理解数据结构
1. 阅读`HousingData.cs`，理解房产属性
2. 阅读`VehicleData.cs`，理解速度加成
3. 阅读`PetData.cs`，理解持续加成

**第二天**：理解管理器
1. 阅读`LifeSystemManager.cs`的初始化部分
2. 理解购买流程（验证 → 扣费 → 添加）
3. 理解自动周期处理

**第三天**：Unity实践
1. 在Unity中添加管理器
2. 创建测试UI
3. 运行并测试基础功能

**第四天**：深入理解
1. 理解事件系统的使用
2. 尝试修改数值平衡
3. 添加自己的房产/汽车/宠物

### 对于有经验的开发者

**重点关注**：
1. **工厂模式**：`CreateRentalHousing()`、`CreatePurchaseHousing()`等静态方法
2. **事件驱动**：6个事件的触发和订阅
3. **自动集成**：订阅`GameTimerManager.OnGameTickStart`实现自动处理
4. **数据计算**：各种辅助方法（性价比、回本时间等）
5. **扩展性**：预留的TODO接口

---

## 🎉 总结

Phase 5 生活系统已完成！这是游戏最有趣的系统之一，包含：

✅ **3个核心数据类**（房产、汽车、宠物）  
✅ **1个管理器**（生活系统管理器）  
✅ **1个测试UI**（完整的功能测试）  
✅ **1份文档**（本开发总结）  
✅ **1,985行代码**（全部含详细中文注释）  
✅ **0个安全问题**（通过CodeQL检查）

这个系统实现了：
- 房产租赁和购买
- 汽车购买和速度加成
- 宠物购买和持续心情加成
- 自动扣租金和心情加成
- 完整的事件系统
- 数据统计功能

**你现在可以**：
1. 在Unity中测试所有功能
2. 理解生活系统的设计思路
3. 学习数据驱动和事件驱动架构
4. 准备开始下一阶段开发

**继续加油！** 🚀

下一个阶段是Phase 6娱乐与战斗系统（虚拟世界体验、病毒入侵塔防），让游戏更加有趣！

---

**开发者**：GitHub Copilot  
**完成时间**：2025-12-31  
**版本**：Phase 5 v1.0  
**项目进度**：56% (5/9)  
**代码质量**：✅ 通过代码审查和安全检查
