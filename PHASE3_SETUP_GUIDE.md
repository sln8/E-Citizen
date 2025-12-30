# Phase 3: 工作与技能系统 - Unity操作指南

## 📋 本阶段目标

完成游戏的工作与技能系统，包括：
- ✅ 工作数据结构和管理器（已完成）
- ✅ 技能数据结构和管理器（已完成）
- ✅ 薪资结算系统（已完成）
- ✅ 算力分配系统（已完成）
- 📝 工作市场UI界面（本指南将教你创建）
- 📝 技能商店UI界面（本指南将教你创建）
- 📝 算力分配UI（本指南将教你创建）

---

## 🎯 前置条件

在开始之前，请确保：
1. ✅ 已完成Phase 1（登录系统）
2. ✅ 已完成Phase 2（资源系统）
3. ✅ Unity项目可以正常运行
4. ✅ 已有ResourceManager和GameTimerManager

---

## 📁 新增文件说明

Phase 3新增了4个核心脚本，全部包含详细的中文注释：

### 1. JobData.cs
**位置**: `Assets/Scripts/Data/JobData.cs`

**功能**:
- 定义工作品级枚举（普通、精良、史诗、传说）
- 定义工作状态枚举
- 资源需求数据结构
- 完整的工作数据类（包含工作信息、技能要求、薪资等）
- 玩家工作实例类（存储玩家当前工作状态）

**关键类**:
```csharp
// 工作品级
public enum JobTier {
    Common,      // 普通 - 薪资 10-30/5分钟
    Rare,        // 精良 - 薪资 40-80/5分钟
    Epic,        // 史诗 - 薪资 100-200/5分钟
    Legendary    // 传说 - 薪资 300-500/5分钟
}

// 工作数据
public class JobData {
    public string jobId;              // 工作ID
    public string jobName;            // 工作名称
    public JobTier jobTier;           // 工作品级
    public string[] requiredSkillIds; // 所需技能
    public int baseSalary;            // 基础薪资
    // ... 更多字段
}
```

### 2. SkillData.cs
**位置**: `Assets/Scripts/Data/SkillData.cs`

**功能**:
- 定义技能品级枚举
- 定义技能状态枚举
- 完整的技能数据类（包含技能信息、价格、文件大小等）
- 玩家技能实例类（存储玩家技能状态和掌握度）
- 技能掌握度计算方法

**掌握度计算**:
```
初始掌握度：20%（刚购买时）
最低工作要求：20%
最高掌握度：200%

掌握度计算：
- 如果算力 ≤ 100%所需：20% + (算力/100%所需) × 80%
- 如果算力 > 100%所需：100% + (超出算力/额外所需) × 100%

影响：实际薪资 = 基础薪资 × (掌握度 / 100)
```

### 3. JobManager.cs
**位置**: `Assets/Scripts/Managers/JobManager.cs`

**功能**:
- 管理所有工作数据
- 开始工作/辞职功能
- 检查技能和资源要求
- 自动薪资结算（监听GameTimerManager的周期事件）
- 工作槽位管理
- 数据产生速率管理

**核心方法**:
```csharp
// 开始工作
JobManager.Instance.StartJob(jobId, out errorMessage);

// 辞职
JobManager.Instance.ResignJob(slotId, out errorMessage);

// 获取可用工作列表
List<JobData> jobs = JobManager.Instance.GetAvailableJobs();

// 检查是否有空闲槽位
bool hasSlot = JobManager.Instance.HasAvailableJobSlot();
```

### 4. SkillManager.cs
**位置**: `Assets/Scripts/Managers/SkillManager.cs`

**功能**:
- 管理所有技能数据
- 购买和下载技能
- 技能下载进度模拟
- 算力分配系统
- 技能掌握度自动计算

**核心方法**:
```csharp
// 购买技能
SkillManager.Instance.PurchaseSkill(skillId, out errorMessage);

// 分配算力
SkillManager.Instance.AllocateComputing(skillId, computing, out errorMessage);

// 获取可用技能列表
List<SkillData> skills = SkillManager.Instance.GetAvailableSkills();

// 检查是否拥有技能
bool hasSkill = SkillManager.Instance.HasSkill(skillId);
```

---

## 🛠️ Unity操作步骤（零基础）

### 第一步：检查新脚本（2分钟）

1. 打开Unity Hub，启动你的项目 `E-Citizens`
2. 等待Unity编辑器加载完成
3. 在底部的 **Project** 窗口中：
   - 展开 `Assets/Scripts/Data/`
   - 你应该看到两个新文件：
     * `JobData.cs` ✓
     * `SkillData.cs` ✓
   - 展开 `Assets/Scripts/Managers/`
   - 你应该看到两个新文件：
     * `JobManager.cs` ✓
     * `SkillManager.cs` ✓

4. 双击任何一个新脚本，在代码编辑器中查看
5. 注意每个文件都有详细的中文注释，仔细阅读理解

### 第二步：添加管理器到场景（5分钟）

#### 2.1 打开测试场景
1. 在Project窗口，展开 `Assets/Scenes/`
2. 双击 `GameTestScene`（如果没有，使用你在Phase 2创建的测试场景）

#### 2.2 添加新管理器
1. 在 **Hierarchy** 窗口中，选中 `GameManagers` 对象
2. 在 **Inspector** 窗口中，点击 `Add Component`
3. 输入 `JobManager`，按回车添加
4. 再次点击 `Add Component`
5. 输入 `SkillManager`，按回车添加

现在你的 `GameManagers` 对象应该有以下组件：
```
GameManagers
├── GameManager
├── FirebaseConfig
├── FirebaseInitializer
├── AuthenticationManager
├── ResourceManager
├── GameTimerManager
├── JobManager      ← 新增
└── SkillManager    ← 新增
```

### 第三步：创建简单的测试UI（15分钟）

我们先创建一个简单的按钮UI来测试工作和技能系统。

#### 3.1 创建工作测试面板
1. 在 **Hierarchy** 窗口，右键点击 `Canvas`
2. 选择 `UI` -> `Panel`
3. 重命名为 `JobTestPanel`
4. 在Inspector中设置位置：
   - 点击 Anchor Presets，选择右侧中央
   - Pos X: `-210`
   - Pos Y: `0`
   - Width: `400`
   - Height: `600`

#### 3.2 添加工作测试按钮
1. 右键点击 `JobTestPanel`
2. 选择 `UI` -> `Button - TextMeshPro`
3. 重命名为 `StartJobButton`
4. 展开按钮，选择其子对象 `Text (TMP)`
5. 将文本改为：`开始工作：数据清洁工`
6. 调整按钮位置：
   - Width: `350`
   - Height: `50`
   - Pos Y: `250`

#### 3.3 添加更多测试按钮
重复上述步骤，创建以下按钮：
- `ResignJobButton` - 文本：`辞职`，Pos Y: `190`
- `BuySkillButton` - 文本：`购买技能：数据清理 Lv.1`，Pos Y: `100`
- `AllocateComputingButton` - 文本：`分配算力：10点`，Pos Y: `40`

#### 3.4 添加信息显示文本
1. 右键点击 `JobTestPanel`
2. 选择 `UI` -> `Text - TextMeshPro`
3. 重命名为 `JobInfoText`
4. 在Inspector中设置：
   - Text: `工作信息：无`
   - Font Size: `16`
   - Alignment: 左上对齐
   - Color: 白色
5. 调整位置：
   - Width: `350`
   - Height: `300`
   - Pos Y: `-100`

### 第四步：创建测试脚本（20分钟）

#### 4.1 创建脚本文件
1. 在Project窗口，展开 `Assets/Scripts/UI/`
2. 右键点击空白处，选择 `Create` -> `C# Script`
3. 命名为 `JobSkillTestUI`
4. 双击打开脚本

#### 4.2 复制以下代码

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 工作和技能系统测试UI
/// 用于测试Phase 3的核心功能
/// </summary>
public class JobSkillTestUI : MonoBehaviour
{
    [Header("UI元素引用")]
    [Tooltip("开始工作按钮")]
    public Button startJobButton;
    
    [Tooltip("辞职按钮")]
    public Button resignJobButton;
    
    [Tooltip("购买技能按钮")]
    public Button buySkillButton;
    
    [Tooltip("分配算力按钮")]
    public Button allocateComputingButton;
    
    [Tooltip("信息显示文本")]
    public TMP_Text jobInfoText;
    
    private string testJobId = "job_001";      // 数据清洁工
    private string testSkillId = "dataClean_lv1"; // 数据清理 Lv.1
    
    private void Start()
    {
        // 注册按钮点击事件
        if (startJobButton != null)
            startJobButton.onClick.AddListener(OnStartJobClicked);
        
        if (resignJobButton != null)
            resignJobButton.onClick.AddListener(OnResignJobClicked);
        
        if (buySkillButton != null)
            buySkillButton.onClick.AddListener(OnBuySkillClicked);
        
        if (allocateComputingButton != null)
            allocateComputingButton.onClick.AddListener(OnAllocateComputingClicked);
        
        // 注册事件监听
        RegisterEvents();
        
        // 初始更新显示
        UpdateJobInfo();
    }
    
    private void OnDestroy()
    {
        // 取消按钮事件
        if (startJobButton != null)
            startJobButton.onClick.RemoveListener(OnStartJobClicked);
        
        if (resignJobButton != null)
            resignJobButton.onClick.RemoveListener(OnResignJobClicked);
        
        if (buySkillButton != null)
            buySkillButton.onClick.RemoveListener(OnBuySkillClicked);
        
        if (allocateComputingButton != null)
            allocateComputingButton.onClick.RemoveListener(OnAllocateComputingClicked);
        
        // 取消事件监听
        UnregisterEvents();
    }
    
    /// <summary>
    /// 注册事件监听
    /// </summary>
    private void RegisterEvents()
    {
        if (JobManager.Instance != null)
        {
            JobManager.Instance.OnJobStarted += OnJobStarted;
            JobManager.Instance.OnJobResigned += OnJobResigned;
            JobManager.Instance.OnSalaryPaid += OnSalaryPaid;
        }
        
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillPurchased += OnSkillPurchased;
            SkillManager.Instance.OnMasteryUpdated += OnMasteryUpdated;
        }
    }
    
    /// <summary>
    /// 取消事件监听
    /// </summary>
    private void UnregisterEvents()
    {
        if (JobManager.Instance != null)
        {
            JobManager.Instance.OnJobStarted -= OnJobStarted;
            JobManager.Instance.OnJobResigned -= OnJobResigned;
            JobManager.Instance.OnSalaryPaid -= OnSalaryPaid;
        }
        
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.OnSkillPurchased -= OnSkillPurchased;
            SkillManager.Instance.OnMasteryUpdated -= OnMasteryUpdated;
        }
    }
    
    /// <summary>
    /// 开始工作按钮点击
    /// </summary>
    private void OnStartJobClicked()
    {
        Debug.Log("<color=cyan>点击开始工作按钮</color>");
        
        string errorMsg;
        bool success = JobManager.Instance.StartJob(testJobId, out errorMsg);
        
        if (success)
        {
            Debug.Log("<color=green>✓ 开始工作成功！</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>✗ 开始工作失败：{errorMsg}</color>");
        }
        
        UpdateJobInfo();
    }
    
    /// <summary>
    /// 辞职按钮点击
    /// </summary>
    private void OnResignJobClicked()
    {
        Debug.Log("<color=cyan>点击辞职按钮</color>");
        
        // 辞职第一个工作槽位
        string errorMsg;
        bool success = JobManager.Instance.ResignJob(0, out errorMsg);
        
        if (success)
        {
            Debug.Log("<color=green>✓ 辞职成功！</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>✗ 辞职失败：{errorMsg}</color>");
        }
        
        UpdateJobInfo();
    }
    
    /// <summary>
    /// 购买技能按钮点击
    /// </summary>
    private void OnBuySkillClicked()
    {
        Debug.Log("<color=cyan>点击购买技能按钮</color>");
        
        string errorMsg;
        bool success = SkillManager.Instance.PurchaseSkill(testSkillId, out errorMsg);
        
        if (success)
        {
            Debug.Log("<color=green>✓ 购买技能成功！开始下载...</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>✗ 购买技能失败：{errorMsg}</color>");
        }
        
        UpdateJobInfo();
    }
    
    /// <summary>
    /// 分配算力按钮点击
    /// </summary>
    private void OnAllocateComputingClicked()
    {
        Debug.Log("<color=cyan>点击分配算力按钮</color>");
        
        string errorMsg;
        bool success = SkillManager.Instance.AllocateComputing(testSkillId, 10f, out errorMsg);
        
        if (success)
        {
            Debug.Log("<color=green>✓ 分配算力成功！</color>");
        }
        else
        {
            Debug.LogWarning($"<color=red>✗ 分配算力失败：{errorMsg}</color>");
        }
        
        UpdateJobInfo();
    }
    
    /// <summary>
    /// 更新工作信息显示
    /// </summary>
    private void UpdateJobInfo()
    {
        if (jobInfoText == null) return;
        
        string info = "<b><size=20>工作和技能信息</size></b>\n\n";
        
        // 显示活跃工作
        List<PlayerJobInstance> activeJobs = JobManager.Instance.GetActiveJobs();
        info += $"<b>活跃工作数：</b>{activeJobs.Count}/{JobManager.Instance.unlockedJobSlots}\n";
        
        foreach (PlayerJobInstance job in activeJobs)
        {
            JobData jobData = JobManager.Instance.GetJobById(job.jobId);
            if (jobData != null)
            {
                info += $"  • {jobData.jobName} (槽位{job.slotId})\n";
                info += $"    已工作{job.completedCycles}周期，收入{job.totalEarned}币\n";
            }
        }
        
        info += "\n";
        
        // 显示已拥有技能
        List<PlayerSkillInstance> playerSkills = SkillManager.Instance.playerSkills;
        info += $"<b>已拥有技能数：</b>{playerSkills.Count}\n";
        
        foreach (PlayerSkillInstance skill in playerSkills)
        {
            SkillData skillData = SkillManager.Instance.GetSkillById(skill.skillId);
            if (skillData != null)
            {
                info += $"  • {skillData.skillName}\n";
                info += $"    掌握度：{skill.masteryPercent:F0}%\n";
                info += $"    算力：{skill.allocatedComputing:F0}\n";
            }
        }
        
        jobInfoText.text = info;
    }
    
    #region 事件回调
    private void OnJobStarted(int slotId, JobData job)
    {
        UpdateJobInfo();
    }
    
    private void OnJobResigned(int slotId)
    {
        UpdateJobInfo();
    }
    
    private void OnSalaryPaid(int slotId, int salary)
    {
        UpdateJobInfo();
    }
    
    private void OnSkillPurchased(string skillId)
    {
        UpdateJobInfo();
    }
    
    private void OnMasteryUpdated(string skillId, float mastery)
    {
        UpdateJobInfo();
    }
    #endregion
}
```

5. 保存文件（Ctrl+S 或 Cmd+S）

### 第五步：连接UI和脚本（5分钟）

1. 回到Unity编辑器，等待脚本编译完成
2. 在Hierarchy中，右键点击 `Canvas`，选择 `Create Empty`
3. 重命名为 `JobSkillTestManager`
4. 选中它，在Inspector中点击 `Add Component`
5. 搜索 `JobSkillTestUI`，按回车添加

6. 现在连接UI元素：
   - 将 `StartJobButton` 拖拽到 `Start Job Button` 字段
   - 将 `ResignJobButton` 拖拽到 `Resign Job Button` 字段
   - 将 `BuySkillButton` 拖拽到 `Buy Skill Button` 字段
   - 将 `AllocateComputingButton` 拖拽到 `Allocate Computing Button` 字段
   - 将 `JobInfoText` 拖拽到 `Job Info Text` 字段

### 第六步：测试运行（10分钟）

#### 6.1 开始测试
1. 点击Unity顶部的播放按钮 ▶️
2. 你应该看到：
   - 左侧：Phase 2的资源显示面板
   - 右侧：Phase 3的工作测试面板

#### 6.2 测试购买技能
1. 点击 `购买技能：数据清理 Lv.1` 按钮
2. 查看Console窗口，应该看到：
   ```
   ✓ 购买技能成功：数据清理 Lv.1，花费50币
   开始下载技能：数据清理 Lv.1，预计X秒
   ```
3. 等待几秒后，会看到：
   ```
   ✓ 技能下载完成：数据清理 Lv.1
   ```
4. 右侧面板会显示你拥有的技能

#### 6.3 测试分配算力
1. 技能下载完成后，点击 `分配算力：10点` 按钮
2. 查看Console：
   ```
   算力分配：数据清理 Lv.1 - 10.0 -> 掌握度100.0%
   ```
3. 右侧面板会显示技能掌握度变化

#### 6.4 测试开始工作
1. 点击 `开始工作：数据清洁工` 按钮
2. 查看Console：
   ```
   ✓ 开始工作：数据清洁工（槽位0）
   占用资源 - 内存:1.0GB, CPU:0.5核
   ```
3. 观察左侧资源面板，资源使用会增加
4. 右侧面板会显示活跃工作信息

#### 6.5 测试薪资结算
1. 等待一个游戏周期（如果开启Debug Mode，30秒或更短）
2. 定时器到0时，会自动结算薪资
3. 查看Console：
   ```
   === 结算工作薪资（1个工作）===
   数据清洁工（槽位0）: +15币
   总薪资：+15币
   ```
4. 观察虚拟币增加

#### 6.6 测试辞职
1. 点击 `辞职` 按钮
2. 查看Console：
   ```
   辞职：数据清洁工（槽位0）
   已工作1个周期，累计收入：15币
   ```
3. 观察资源被释放

---

## 🎮 完整测试流程

按照以下顺序进行完整测试：

### 测试1：购买技能流程
1. 启动游戏
2. 查看初始虚拟币（100币）
3. 点击"购买技能"按钮
4. 等待下载完成
5. 验证：虚拟币减少50，存储占用增加1GB

### 测试2：算力分配流程
1. 技能下载完成后
2. 点击"分配算力"按钮
3. 验证：算力使用增加10，掌握度从20%提升到100%

### 测试3：开始工作流程
1. 拥有技能后
2. 点击"开始工作"按钮
3. 验证：
   - 资源被占用（内存1GB, CPU0.5核等）
   - 工作信息显示在面板上
   - 没有空闲槽位了（0/1）

### 测试4：薪资结算流程
1. 工作开始后
2. 等待一个游戏周期
3. 验证：
   - 自动结算薪资
   - 虚拟币增加
   - 已工作周期数增加
   - Console显示详细结算日志

### 测试5：辞职流程
1. 工作进行中
2. 点击"辞职"按钮
3. 验证：
   - 资源被释放
   - 工作从面板消失
   - 有空闲槽位了（0/1 -> 0/1）

---

## 🐛 常见问题和解决方案

### 问题1：点击按钮没有反应
**现象**：点击按钮，Console没有任何输出

**解决方法**：
1. 检查EventSystem是否存在（在Canvas创建时应该自动创建）
2. 检查按钮是否正确连接到脚本
3. 检查JobManager和SkillManager是否已添加到场景

### 问题2：购买技能失败
**现象**：显示"虚拟币不足"或"存储空间不足"

**解决方法**：
1. 检查当前虚拟币数量（初始100币，技能50币）
2. 如果不够，在ResourceDisplayUI中添加测试按钮增加虚拟币
3. 或者修改技能价格（在SkillManager的CreateSampleSkills方法中）

### 问题3：开始工作失败
**现象**：显示"技能不足"或"资源不足"

**解决方法**：
1. 确保已经购买并下载完成所需技能
2. 检查资源是否被其他工作占用
3. 如果资源不足，可以：
   - 先辞职其他工作
   - 或升级资源配置

### 问题4：薪资没有结算
**现象**：等了很久也没有收到薪资

**解决方法**：
1. 检查GameTimerManager是否启用
2. 检查是否开启了Debug Mode（缩短周期）
3. 在Console查看是否有周期结算日志
4. 手动触发：点击Phase 2的"立即触发结算"按钮

---

## 💡 进阶功能和优化

### 1. 创建更多工作和技能

打开 `JobManager.cs` 和 `SkillManager.cs`，在 `CreateSampleJobs()` 和 `CreateSampleSkills()` 方法中添加更多数据：

```csharp
// 在JobManager.cs的CreateSampleJobs方法中添加
JobData newJob = new JobData
{
    jobId = "job_custom",
    jobName = "我的自定义工作",
    jobDescription = "这是一个自定义的工作",
    jobTier = JobTier.Common,
    requiredSkillIds = new string[] { },
    resourceRequirement = new ResourceRequirement(1f, 0.5f, 50f, 5f),
    baseSalary = 20,
    payInterval = 300,
    dataGeneration = 0.2f,
    unlockLevel = 1
};
allJobs.Add(newJob);
```

### 2. 调整薪资和价格

修改各个数值来平衡游戏：
- 在JobData中修改 `baseSalary`
- 在SkillData中修改 `price`
- 在ResourceManager中修改初始虚拟币

### 3. 添加调试按钮

在测试UI中添加更多按钮：
- 增加虚拟币按钮
- 升级资源按钮
- 解锁更多工作槽位按钮
- 重置算力分配按钮

---

## 📝 Phase 3 完成检查清单

完成以下所有项目，表示Phase 3成功完成：

- [ ] 所有新脚本都已添加到项目中
- [ ] 脚本没有编译错误
- [ ] JobManager和SkillManager已添加到场景
- [ ] 创建了测试UI界面
- [ ] UI正确连接到脚本
- [ ] 游戏可以正常运行
- [ ] 可以成功购买技能
- [ ] 技能下载进度正常
- [ ] 可以分配算力
- [ ] 技能掌握度正确计算
- [ ] 可以开始工作
- [ ] 资源被正确占用
- [ ] 薪资自动结算
- [ ] 可以辞职
- [ ] 资源被正确释放
- [ ] Console显示详细日志
- [ ] 理解了每个系统的功能

全部完成后，恭喜你！你已经完成了Phase 3的开发！🎉

---

## 🚀 下一步：Phase 4

完成Phase 3后，我们将开发公司系统：
- 创建和管理公司
- 招聘AI员工
- 真实玩家简历系统
- 人才市场
- 公司收入分成

敬请期待！💪

---

## 📚 学习建议

### 对于零基础开发者

1. **理解数据结构**
   - 仔细阅读JobData.cs和SkillData.cs
   - 理解每个字段的作用
   - 了解枚举类型的用法

2. **理解管理器模式**
   - JobManager和SkillManager都使用单例模式
   - 理解为什么需要管理器
   - 学习如何通过 `.Instance` 访问

3. **理解事件系统**
   - 了解事件的发布和订阅
   - 理解UI如何监听管理器事件
   - 学习解耦的好处

4. **实践和修改**
   - 尝试修改薪资数值
   - 创建自己的工作和技能
   - 添加新的功能

### 对于有经验的开发者

1. **优化数据存储**
   - 实现Firebase数据持久化
   - 添加数据缓存机制
   - 优化查询性能

2. **扩展功能**
   - 实现工作搜索和过滤
   - 添加技能升级系统
   - 实现技能树

3. **改进UI**
   - 创建更美观的工作市场界面
   - 添加技能商店列表
   - 实现算力分配滑动条

---

**记住**：
- 所有代码都有详细的中文注释，不懂的地方多看注释
- 多测试、多观察，理解系统如何工作
- 遇到问题先查看Console的日志输出
- 不要害怕修改代码，实践是最好的老师

祝开发顺利！🚀
