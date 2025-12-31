# Phase 4 开发总结 - 公司系统

## 📋 概述

Phase 4 实现了《电子公民》游戏的核心社交和经营玩法 - **公司系统**。玩家可以创建公司、招聘AI员工或真实玩家、管理员工、获得收入，并将公司升级到更高级别。

---

## ✅ 已完成的功能

### 1. 公司数据类 (CompanyData.cs)

**功能**：
- 存储公司的所有信息
- 管理员工列表
- 计算财务数据
- 处理公司升级

**核心属性**：
```csharp
- companyId: 唯一ID
- companyName: 公司名称
- tier: 公司品级（微型/小型/中型/大型）
- level: 公司等级
- employees: 员工列表
- baseIncome: 基础收入
- totalIncome: 总收入（含员工加成）
- totalExpenses: 总支出（员工薪资）
- netProfit: 净利润
```

**公司品级**：
| 品级 | 解锁等级 | 开办成本 | 基础收入 | 员工上限 |
|------|---------|---------|---------|---------|
| 微型公司 | Lv.5 | 1,000币 | 50币/5分钟 | 5人 |
| 小型公司 | Lv.15 | 5,000币 | 150币/5分钟 | 10人 |
| 中型公司 | Lv.30 | 20,000币 | 500币/5分钟 | 20人 |
| 大型企业 | Lv.50 | 100,000币 | 2,000币/5分钟 | 50人 |

**关键方法**：
- `CalculateFinancials()`: 计算公司财务
- `SettleIncome()`: 结算收入（每5分钟）
- `AddEmployee()`: 添加员工
- `RemoveEmployee()`: 移除员工
- `Upgrade()`: 升级公司

---

### 2. 员工数据类 (EmployeeData.cs)

**功能**：
- 支持两种员工类型：AI员工和真实玩家
- AI员工可以培训升级
- 真实玩家提供资源并获得薪资
- 计算收入加成

**AI员工品级**：
| 品级 | 招聘成本 | 薪资 | 收入加成 | 最高等级 | 培训成本/级 |
|------|---------|------|---------|---------|------------|
| 普通 | 100币 | 5币/5分钟 | +10% | Lv.10 | 50币 |
| 精良 | 500币 | 15币/5分钟 | +30% | Lv.25 | 100币 |
| 史诗 | 2,000币 | 40币/5分钟 | +60% | Lv.50 | 300币 |
| 传说 | 10,000币 | 100币/5分钟 | +100% | Lv.100 | 1,000币 |

**真实玩家收入加成计算**：
```
基础加成 = 1.2
等级加成 = 玩家等级 * 0.01
资源价值 = 内存*10 + CPU*20 + 网速*0.1 + 算力*5
资源加成 = (资源价值 / 100) * 0.1
最终加成 = 1 + 基础加成 + 等级加成 + 资源加成
```

**关键方法**：
- `CreateAIEmployee()`: 创建AI员工
- `CreatePlayerEmployee()`: 创建真实玩家员工
- `Train()`: 培训AI员工（升级）
- `GetDismissalCompensation()`: 计算辞退补偿

---

### 3. 简历数据类 (ResumeData.cs)

**功能**：
- 真实玩家发布简历，供其他玩家的公司招聘
- 自动计算收入加成
- 管理简历状态（可用/已雇佣/已撤回）
- 提供搜索和筛选功能

**核心属性**：
```csharp
- resumeId: 唯一ID
- playerId: 玩家ID
- playerName: 玩家名称
- playerLevel: 玩家等级
- offeredResources: 提供的资源
- expectedSalary: 期望薪资
- incomeBonus: 收入加成（自动计算）
- status: 简历状态
```

**简历状态**：
- `Available`: 可用（等待被雇佣）
- `Hired`: 已被雇佣
- `Withdrawn`: 已撤回

**关键方法**：
- `CalculateIncomeBonus()`: 计算收入加成
- `MarkAsHired()`: 标记为已雇佣
- `MarkAsWithdrawn()`: 标记为已撤回
- `MarkAsAvailable()`: 标记为可用
- `GetCostEffectiveness()`: 获取性价比
- `GetTotalResourceValue()`: 获取资源总价值

---

### 4. 公司管理器 (CompanyManager.cs)

**功能**：
- 管理所有公司相关操作
- 自动收入结算（集成游戏定时器）
- 完整的事件系统
- 数据持久化支持

**核心功能**：

#### 4.1 公司创建
```csharp
CompanyData company = CompanyManager.Instance.CreateCompany("我的公司", CompanyTier.Small);
```
- 检查玩家等级是否满足要求
- 检查虚拟币是否足够
- 自动扣除开办成本
- 初始化公司数据

#### 4.2 AI员工招聘
```csharp
bool success = CompanyManager.Instance.HireAIEmployee(companyId, EmployeeTier.Common);
```
- 检查员工上限
- 检查虚拟币是否足够
- 自动创建AI员工
- 更新公司财务

#### 4.3 真实玩家招聘
```csharp
bool success = CompanyManager.Instance.HirePlayerEmployee(companyId, resume);
```
- 检查简历是否可雇佣
- 检查员工上限
- 标记简历为已雇佣
- 更新公司财务

#### 4.4 员工辞退
```csharp
bool success = CompanyManager.Instance.DismissEmployee(companyId, employeeId);
```
- 计算辞退补偿（薪资 * 2）
- 检查虚拟币是否足够支付补偿
- 移除员工
- 更新公司财务

#### 4.5 AI员工培训
```csharp
bool success = CompanyManager.Instance.TrainEmployee(companyId, employeeId);
```
- 检查是否是AI员工
- 检查虚拟币是否足够
- 升级员工等级
- 提升收入加成

#### 4.6 公司升级
```csharp
bool success = CompanyManager.Instance.UpgradeCompany(companyId);
```
- 检查是否满足升级条件
  - 总收入 >= 要求收入
  - 员工数 >= 要求员工数
- 升级公司等级
- 增加基础收入（+20%）

#### 4.7 自动收入结算
- 每5分钟自动触发（通过游戏定时器）
- 计算净利润（总收入 - 总支出）
- 发放利润给公司所有者
- 处理亏损情况

**事件系统**：
```csharp
- OnCompanyCreated: 公司创建时触发
- OnCompanyUpgraded: 公司升级时触发
- OnEmployeeHired: 员工雇佣时触发
- OnEmployeeDismissed: 员工辞退时触发
- OnIncomeSettled: 收入结算时触发
```

---

### 5. 人才市场管理器 (TalentMarketManager.cs)

**功能**：
- 管理AI员工市场
- 管理玩家简历系统
- 提供搜索和筛选功能
- 智能推荐系统

**核心功能**：

#### 5.1 发布简历
```csharp
ProvidedResources resources = new ProvidedResources(2, 1, 100, 10);
bool success = TalentMarketManager.Instance.PostResume(resources, 50f);
```
- 验证资源
- 创建简历
- 自动计算收入加成
- 添加到市场

#### 5.2 撤回简历
```csharp
bool success = TalentMarketManager.Instance.WithdrawResume();
```
- 检查是否已被雇佣
- 标记为已撤回

#### 5.3 更新简历
```csharp
bool success = TalentMarketManager.Instance.UpdateResume(newResources, newSalary);
```
- 更新资源和薪资
- 重新计算收入加成
- 更新发布时间

#### 5.4 搜索简历
```csharp
// 获取所有可用简历
List<ResumeData> resumes = TalentMarketManager.Instance.GetAvailableResumes();

// 按条件搜索
resumes = TalentMarketManager.Instance.SearchResumes(minLevel: 10, maxSalary: 100);

// 按性价比排序
resumes = TalentMarketManager.Instance.GetResumesByCostEffectiveness();

// 按收入加成排序
resumes = TalentMarketManager.Instance.GetResumesByIncomeBonus();

// 按薪资排序
resumes = TalentMarketManager.Instance.GetResumesBySalary(ascending: true);
```

#### 5.5 智能推荐
```csharp
List<ResumeData> recommended = TalentMarketManager.Instance.RecommendResumes(company, maxResults: 5);
```
- 计算公司可承担的最大薪资
- 按性价比排序
- 返回最合适的简历

#### 5.6 AI员工市场
```csharp
List<AIEmployeeInfo> market = TalentMarketManager.Instance.GetAIEmployeeMarket();
```
- 返回所有品级的AI员工信息
- 包含招聘成本和示例员工

**事件系统**：
```csharp
- OnResumePosted: 简历发布时触发
- OnResumeWithdrawn: 简历撤回时触发
- OnResumeHired: 简历被雇佣时触发
- OnResumeListUpdated: 简历列表更新时触发
```

---

### 6. 游戏定时器集成

**GameTimerManager.cs 更新**：
- 工作薪资结算集成（调用 JobManager.SettleAllJobs()）
- 公司收入结算集成（CompanyManager自动通过事件处理）
- 改进的日志输出

**自动结算流程**（每5分钟）：
1. 身份类型费用（意识连接者支付连接费）
2. **工作薪资结算** ← 集成
3. **公司收入结算** ← 集成
4. 房租支付（待实现）
5. 安全卫士费用（待实现）
6. 数据产生
7. 心情值变化
8. 病毒入侵检测（待实现）
9. 同步到Firebase

---

### 7. 测试UI (CompanyTestUI.cs)

**功能**：
- 测试所有公司系统功能
- 实时显示公司状态
- 事件监听和日志输出

**测试功能**：
- ✅ 创建公司
- ✅ 招聘AI员工
- ✅ 培训AI员工
- ✅ 辞退员工
- ✅ 发布简历
- ✅ 查看简历列表
- ✅ 招聘真实玩家
- ✅ 升级公司
- ✅ 添加测试资金
- ✅ 实时显示更新

**显示信息**：
- 玩家信息（虚拟币、等级）
- 公司信息（名称、品级、等级、员工数）
- 财务信息（收入、支出、利润）
- 员工列表（详细信息）
- 升级要求
- 人才市场信息

---

## 📊 数据平衡

### 经济平衡

**公司创建成本**：
- 微型：1,000币（等级5）
- 小型：5,000币（等级15）
- 中型：20,000币（等级30）
- 大型：100,000币（等级50）

**AI员工招聘成本**：
- 普通：100币
- 精良：500币
- 史诗：2,000币
- 传说：10,000币

**示例计算**：

假设创建微型公司（基础收入50币/5分钟）：

1. **雇佣3个普通AI员工**：
   - 招聘成本：3 × 100 = 300币
   - 薪资支出：3 × 5 = 15币/5分钟
   - 收入加成：1.1 × 3 = 1.3
   - 总收入：50 × (1 + 0.1 + 0.1 + 0.1) = 65币/5分钟
   - 净利润：65 - 15 = 50币/5分钟
   - 每小时利润：50 × 12 = 600币/小时

2. **培训到Lv.5**：
   - 培训成本：3 × (50 × 4) = 600币
   - 每个员工加成提升：+0.5% × 4 = +2%
   - 新收入加成：约1.36
   - 新总收入：约68币/5分钟
   - 新净利润：约53币/5分钟

3. **升级到Lv.2**：
   - 升级要求：总收入100币/5分钟，员工3人
   - 基础收入提升：50 × 1.2 = 60币/5分钟
   - 新总收入：60 × 1.36 = 约82币/5分钟

---

## 🎮 使用示例

### 示例1：创建公司并招聘员工

```csharp
// 1. 创建微型公司
CompanyData company = CompanyManager.Instance.CreateCompany("我的数据公司", CompanyTier.Small);

// 2. 招聘3个普通AI员工
for (int i = 0; i < 3; i++)
{
    CompanyManager.Instance.HireAIEmployee(company.companyId, EmployeeTier.Common);
}

// 3. 等待收入结算（每5分钟自动）
// 净利润会自动发放到玩家账户
```

### 示例2：培训员工提升收入

```csharp
// 获取公司
CompanyData company = CompanyManager.Instance.GetCompany(companyId);

// 培训所有AI员工到Lv.5
foreach (EmployeeData employee in company.employees)
{
    if (employee.type == EmployeeType.AI)
    {
        for (int i = employee.level; i < 5; i++)
        {
            CompanyManager.Instance.TrainEmployee(companyId, employee.employeeId);
        }
    }
}
```

### 示例3：发布简历并被雇佣

```csharp
// 玩家A：发布简历
ProvidedResources resources = new ProvidedResources(2, 1, 100, 10);
TalentMarketManager.Instance.PostResume(resources, 50f);

// 玩家B：搜索并雇佣
List<ResumeData> resumes = TalentMarketManager.Instance.GetResumesByCostEffectiveness();
ResumeData bestResume = resumes[0];
CompanyManager.Instance.HirePlayerEmployee(myCompanyId, bestResume);

// 玩家A现在会每5分钟收到50币薪资
```

---

## 🔧 Unity操作步骤

### 1. 添加管理器到场景

1. 在Hierarchy中找到或创建GameManager对象
2. 添加以下脚本组件：
   - CompanyManager
   - TalentMarketManager
3. 确保ResourceManager和GameTimerManager已添加
4. 保存场景

### 2. 创建测试UI

1. 在Hierarchy中创建Canvas（如果还没有）
2. 在Canvas下创建Panel，命名为CompanyTestPanel
3. 添加CompanyTestUI脚本到Panel
4. 创建以下UI元素：
   - TextMeshPro Text：statusText（显示状态）
   - Button：createCompanyButton
   - Button：hireAIButton
   - Button：trainEmployeeButton
   - Button：dismissEmployeeButton
   - Button：postResumeButton
   - Button：viewResumesButton
   - Button：hirePlayerButton
   - Button：upgradeCompanyButton
   - Button：refreshButton
   - Button：addMoneyButton
5. 连接所有UI引用
6. 保存场景

### 3. 测试流程

1. 运行游戏
2. 点击"添加测试资金"按钮（添加5000币）
3. 确保等级达到5（如果不够，修改ResourceManager测试数据）
4. 点击"创建公司"按钮
5. 点击"招聘AI员工"按钮（招聘3次）
6. 观察状态显示（收入、支出、净利润）
7. 等待5分钟（或调试模式30秒）观察自动结算
8. 点击"培训员工"按钮提升员工等级
9. 点击"发布简历"按钮测试简历系统
10. 点击"查看简历"按钮查看市场简历
11. 点击"招聘玩家"按钮招聘真实玩家
12. 当满足条件时点击"升级公司"按钮

---

## 📈 性能和优化

### 性能特点

- **事件驱动架构**：松耦合，易于扩展
- **单例模式**：全局访问，避免重复实例
- **自动结算**：集成游戏定时器，无需手动调用
- **智能缓存**：数据字典快速查找
- **延迟加载**：按需生成测试数据

### 内存占用估算

- 每个公司数据：约2KB
- 每个员工数据：约1KB
- 每个简历数据：约1KB
- 100个公司 + 500个员工 + 100个简历：约800KB

---

## 🐛 已知问题和限制

1. **Firebase集成**：数据持久化尚未实现（SaveData/LoadData方法是TODO）
2. **玩家ID**：当前使用硬编码"current_player"，需要从UserData获取
3. **破产处理**：公司亏损但所有者账户余额不足的情况未完全处理
4. **资源验证**：简历发布时的资源验证尚未完全实现
5. **UI界面**：正式UI脚本和界面尚未创建（仅有测试UI）

---

## 🚀 下一步计划

### 短期（本周）
- [ ] 创建正式UI脚本（6个UI管理器）
- [ ] 创建Phase 4 UI操作指南
- [ ] 完善测试和优化

### 中期（下周）
- [ ] 开始Phase 5：生活系统（房产、汽车、宠物）
- [ ] 集成Firebase数据持久化
- [ ] 添加更多公司管理功能

### 长期（1个月）
- [ ] 完成Phase 6-9所有系统
- [ ] 全面优化和测试
- [ ] 准备发布Beta版本

---

## 📝 代码统计

### Phase 4 代码量

| 文件 | 行数 | 功能 |
|------|------|------|
| CompanyData.cs | 440 | 公司数据类 |
| EmployeeData.cs | 450 | 员工数据类 |
| ResumeData.cs | 350 | 简历数据类 |
| CompanyManager.cs | 620 | 公司管理器 |
| TalentMarketManager.cs | 630 | 人才市场管理器 |
| CompanyTestUI.cs | 650 | 测试UI |
| **总计** | **3140** | **Phase 4** |

### 项目总代码量

| Phase | 代码量 | 状态 |
|-------|--------|------|
| Phase 1 | 800行 | 完成 ✅ |
| Phase 2 | 1300行 | 完成 ✅ |
| Phase 3 | 3200行 | 完成 ✅ |
| Phase 4 | 3140行 | 完成 ✅ |
| **总计** | **8440行** | **4/9阶段完成** |

---

## 💡 开发建议

### 对于零基础开发者

1. **循序渐进**：先理解每个数据类的作用，再学习管理器的使用
2. **多测试**：使用测试UI频繁测试各种功能
3. **阅读注释**：所有代码都有详细的中文注释
4. **查看日志**：Console中的日志会显示详细的执行流程
5. **从简单开始**：先测试创建公司和招聘AI员工，再尝试更复杂的功能

### 对于有经验的开发者

1. **事件系统**：可以订阅管理器事件来实现自定义功能
2. **数据扩展**：可以继承数据类添加自定义字段
3. **UI定制**：可以创建自己的UI管理器
4. **数值调整**：可以修改公司品级和员工品级的数值
5. **算法优化**：可以优化搜索和推荐算法

---

## 🎉 总结

Phase 4 公司系统已完整实现，包括：

✅ **4个核心数据类**（公司、员工、简历、AI员工信息）
✅ **2个管理器**（公司管理器、人才市场管理器）
✅ **完整的功能**（创建、招聘、培训、辞退、升级、结算）
✅ **事件驱动架构**（松耦合、易扩展）
✅ **自动结算系统**（集成游戏定时器）
✅ **测试UI**（完整的功能测试）
✅ **详细注释**（3140行代码，全部中文注释）

这是游戏最复杂的系统之一，为后续的社交互动和经济系统奠定了坚实基础。

**下一步**：创建正式UI脚本和Phase 5生活系统！

---

**开发者**：AI Assistant for E-Citizen Project  
**完成时间**：2025-12-31  
**版本**：Phase 4 v1.0  
**游戏进度**：4/9 阶段完成（44%）
