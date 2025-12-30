# Phase 2: 核心资源系统 - Unity操作指南

## 📋 本阶段目标

完成游戏的核心资源系统，包括：
- ✅ 资源管理（内存、CPU、网速、算力、存储）
- ✅ 效率计算系统
- ✅ 5分钟定时器系统
- ✅ 数据产生机制
- 📝 资源显示UI（本指南将教你创建）

---

## 🎯 前置条件

在开始之前，请确保：
1. ✅ 已完成Phase 1（登录系统）
2. ✅ Unity项目可以正常运行
3. ✅ 了解基本的Unity操作

---

## 📁 新增文件说明

Phase 2新增了3个核心脚本，都包含详细的中文注释：

### 1. PlayerResources.cs
**位置**: `Assets/Scripts/Data/PlayerResources.cs`

**功能**:
- 存储玩家的所有资源数据
- 定义资源类型和身份类型枚举
- 提供资源计算属性（如可用资源、使用率等）
- 资源操作方法（分配、释放、升级等）

**关键特性**:
```csharp
// 硬件资源
public float memoryTotal = 16f;        // 总内存
public float memoryUsed = 2f;          // 已使用内存

// 计算属性（自动计算）
public float MemoryAvailable => memoryTotal - memoryUsed;  // 可用内存
public float MemoryUsagePercent => (memoryUsed / memoryTotal) * 100f;  // 使用率
```

### 2. ResourceManager.cs
**位置**: `Assets/Scripts/Managers/ResourceManager.cs`

**功能**:
- 管理玩家的所有资源
- 处理资源分配和释放
- 计算收入效率（根据游戏设计文档的公式）
- 虚拟币和心情值管理
- 数据产生和清理

**核心方法**:
```csharp
// 分配资源（例如：开始工作）
bool success = ResourceManager.Instance.TryAllocateResources(
    memory: 1f, cpu: 0.5f, bandwidth: 50f, computing: 5f
);

// 计算收入效率
float efficiency = ResourceManager.Instance.CalculateIncomeEfficiency();

// 添加虚拟币
ResourceManager.Instance.AddVirtualCoin(100, "工作薪资");
```

### 3. GameTimerManager.cs
**位置**: `Assets/Scripts/Managers/GameTimerManager.cs`

**功能**:
- 管理游戏的5分钟周期系统
- 每个周期自动执行结算操作
- 支持调试模式（缩短周期便于测试）
- 时间缩放功能（加速测试）

**核心功能**:
- 每5分钟自动触发一次结算
- 结算内容：薪资发放、费用支付、数据产生、心情值变化等
- 提供事件系统，其他系统可以监听周期事件

---

## 🛠️ Unity操作步骤（零基础）

### 第一步：打开Unity项目（1分钟）

1. 打开Unity Hub
2. 点击你的项目 `E-Citizens`
3. 等待Unity编辑器加载完成

### 第二步：检查新脚本（2分钟）

1. 在Unity编辑器底部的 **Project** 窗口中：
   - 展开 `Assets` 文件夹
   - 展开 `Scripts` 文件夹
   - 你应该看到以下新文件：
     ```
     Scripts/
     ├── Data/
     │   └── PlayerResources.cs     (新增)
     └── Managers/
         ├── ResourceManager.cs     (新增)
         └── GameTimerManager.cs    (新增)
     ```

2. 双击任何一个新脚本，应该会在Visual Studio或VS Code中打开
3. 查看代码中的详细中文注释，了解每个部分的功能

### 第三步：创建测试场景（5分钟）

#### 3.1 创建新场景
1. 在Unity菜单栏，点击 `File` -> `New Scene`
2. 选择 `Basic (Built-in)` 模板
3. 点击 `Create`

#### 3.2 保存场景
1. 按 `Ctrl+S` (Mac: `Cmd+S`)
2. 在弹出的对话框中：
   - 确保位置在 `Assets/Scenes/` 文件夹
   - 文件名输入：`GameTestScene`
   - 点击 `Save`

#### 3.3 添加管理器对象
1. 在 **Hierarchy** 窗口（场景层次结构）中：
   - 右键点击空白处
   - 选择 `Create Empty`
   - 在Inspector中将其命名为 `GameManagers`

2. 选中 `GameManagers` 对象，在 **Inspector** 窗口中：
   - 点击底部的 `Add Component` 按钮
   - 搜索并添加以下组件（依次添加）：
     1. 输入 `GameManager`，按回车
     2. 输入 `FirebaseConfig`，按回车
     3. 输入 `FirebaseInitializer`，按回车
     4. 输入 `AuthenticationManager`，按回车
     5. 输入 `ResourceManager`，按回车（新增）
     6. 输入 `GameTimerManager`，按回车（新增）

#### 3.4 配置定时器（用于测试）
1. 在 **Hierarchy** 中选中 `GameManagers`
2. 在 **Inspector** 中找到 `Game Timer Manager` 组件
3. 勾选 `Debug Mode`（这样周期会从5分钟变成30秒，便于测试）
4. 设置 `Time Scale` 为 `2`（2倍速，进一步加速测试）

**重要说明**：
- `Debug Mode` 开启后，每30秒就会触发一次结算（而不是5分钟）
- `Time Scale` 设为2倍速，实际上15秒就会触发一次
- 这样可以快速看到效果，正式版本时要关闭Debug Mode

### 第四步：创建简单的测试UI（10分钟）

#### 4.1 创建Canvas
1. 在 **Hierarchy** 窗口右键，选择 `UI` -> `Canvas`
2. Unity会自动创建：
   - Canvas（画布）
   - EventSystem（事件系统，处理点击等）

#### 4.2 创建资源显示面板
1. 右键点击 `Canvas`
2. 选择 `UI` -> `Panel`
3. 重命名为 `ResourcePanel`

#### 4.3 创建资源文本显示
1. 右键点击 `ResourcePanel`
2. 选择 `UI` -> `Text - TextMeshPro`
3. 如果弹出导入TMP资源的对话框，点击 `Import TMP Essentials`
4. 重命名文本对象为 `ResourceText`
5. 在Inspector中设置：
   - **Text**: 留空（将由脚本设置）
   - **Font Size**: `18`
   - **Alignment**: 左上对齐
   - **Color**: 白色

#### 4.4 调整面板位置和大小
1. 选中 `ResourcePanel`
2. 在Inspector的 `Rect Transform` 部分：
   - 点击左上角的小方框（Anchor Presets）
   - 按住 `Alt+Shift`，点击左上角的选项（这样可以同时设置锚点和位置）
3. 设置大小：
   - **Width**: `400`
   - **Height**: `500`
   - **Pos X**: `210` （向右偏移一点）
   - **Pos Y**: `-260` （向下偏移一点）

#### 4.5 创建定时器显示
1. 右键点击 `Canvas`
2. 选择 `UI` -> `Text - TextMeshPro`
3. 重命名为 `TimerText`
4. 在Inspector中设置：
   - **Text**: `下次结算: 00:00`
   - **Font Size**: `24`
   - **Alignment**: 居中
   - **Color**: 黄色
5. 在 `Rect Transform` 部分：
   - 点击Anchor Presets，选择顶部中央
   - **Pos Y**: `-30`

#### 4.6 创建测试按钮
1. 右键点击 `Canvas`
2. 选择 `UI` -> `Button - TextMeshPro`
3. 重命名为 `TestTickButton`
4. 展开按钮，选择其子对象 `Text (TMP)`
5. 将文本改为：`立即触发结算（测试）`
6. 调整按钮位置：
   - 在 `Rect Transform` 部分，选择底部中央锚点
   - **Width**: `300`
   - **Height**: `60`
   - **Pos Y**: `100`

### 第五步：创建UI控制脚本（15分钟）

#### 5.1 创建脚本文件
1. 在Project窗口，展开 `Assets/Scripts/UI/`
2. 右键点击空白处，选择 `Create` -> `C# Script`
3. 命名为 `ResourceDisplayUI`
4. 双击打开脚本

#### 5.2 复制以下代码

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 资源显示UI
/// 负责在屏幕上显示玩家的资源信息
/// </summary>
public class ResourceDisplayUI : MonoBehaviour
{
    [Header("UI元素引用")]
    [Tooltip("资源信息文本")]
    public TMP_Text resourceText;
    
    [Tooltip("定时器文本")]
    public TMP_Text timerText;
    
    [Tooltip("测试按钮")]
    public Button testTickButton;
    
    private void Start()
    {
        // 注册事件监听
        RegisterEvents();
        
        // 注册按钮点击事件
        if (testTickButton != null)
        {
            testTickButton.onClick.AddListener(OnTestTickButtonClicked);
        }
        
        // 初始显示
        UpdateResourceDisplay();
    }
    
    private void Update()
    {
        // 每帧更新定时器显示
        UpdateTimerDisplay();
    }
    
    private void OnDestroy()
    {
        // 取消事件监听
        UnregisterEvents();
        
        if (testTickButton != null)
        {
            testTickButton.onClick.RemoveListener(OnTestTickButtonClicked);
        }
    }
    
    /// <summary>
    /// 注册事件监听
    /// </summary>
    private void RegisterEvents()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourcesChanged += OnResourcesChanged;
        }
    }
    
    /// <summary>
    /// 取消事件监听
    /// </summary>
    private void UnregisterEvents()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourcesChanged -= OnResourcesChanged;
        }
    }
    
    /// <summary>
    /// 资源变化回调
    /// </summary>
    private void OnResourcesChanged(PlayerResources resources)
    {
        UpdateResourceDisplay();
    }
    
    /// <summary>
    /// 更新资源显示
    /// </summary>
    private void UpdateResourceDisplay()
    {
        if (resourceText == null || ResourceManager.Instance == null)
        {
            return;
        }
        
        PlayerResources res = ResourceManager.Instance.GetResourcesCopy();
        
        if (res == null)
        {
            resourceText.text = "资源数据加载中...";
            return;
        }
        
        // 格式化显示资源信息
        string displayText = $"<b><size=24>玩家资源</size></b>\n\n";
        displayText += $"<b>虚拟币:</b> <color=yellow>{res.virtualCoin}</color>\n";
        displayText += $"<b>等级:</b> Lv.{res.level}\n";
        displayText += $"<b>心情值:</b> <color={(res.moodValue >= 0 ? "green" : "red")}>{res.moodValue}</color>\n\n";
        
        displayText += $"<b>内存:</b> {res.memoryUsed:F1}/{res.memoryTotal:F1} GB ({res.MemoryUsagePercent:F0}%)\n";
        displayText += $"<b>CPU:</b> {res.cpuUsed:F1}/{res.cpuTotal:F1} 核 ({res.CpuUsagePercent:F0}%)\n";
        displayText += $"<b>网速:</b> {res.bandwidthUsed:F0}/{res.bandwidthTotal:F0} Mbps ({res.BandwidthUsagePercent:F0}%)\n";
        displayText += $"<b>算力:</b> {res.computingUsed:F1}/{res.computingTotal:F1} ({res.ComputingUsagePercent:F0}%)\n";
        displayText += $"<b>存储:</b> {res.storageUsed:F1}/{res.storageTotal:F1} GB ({res.StorageUsagePercent:F0}%)\n\n";
        
        // 显示效率信息
        float efficiency = ResourceManager.Instance.CalculateIncomeEfficiency();
        displayText += $"<b>当前效率:</b> <color=cyan>{efficiency:F1}%</color>\n";
        
        resourceText.text = displayText;
    }
    
    /// <summary>
    /// 更新定时器显示
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText == null || GameTimerManager.Instance == null)
        {
            return;
        }
        
        string timeStr = GameTimerManager.Instance.GetRemainingTimeFormatted();
        int totalTicks = GameTimerManager.Instance.GetTotalTicks();
        
        timerText.text = $"下次结算: <color=yellow>{timeStr}</color> | 周期: {totalTicks}";
    }
    
    /// <summary>
    /// 测试按钮点击回调
    /// </summary>
    private void OnTestTickButtonClicked()
    {
        Debug.Log("<color=cyan>点击测试按钮，立即触发一次结算</color>");
        
        if (GameTimerManager.Instance != null)
        {
            GameTimerManager.Instance.TriggerGameTickNow();
        }
    }
}
```

5. 保存文件（`Ctrl+S` 或 `Cmd+S`）

#### 5.3 连接UI和脚本
1. 回到Unity编辑器
2. 等待脚本编译完成（底部进度条）
3. 在Hierarchy中，右键点击 `Canvas`，选择 `Create Empty`
4. 重命名为 `ResourceDisplayManager`
5. 选中它，在Inspector中点击 `Add Component`
6. 搜索 `ResourceDisplayUI`，按回车添加

7. 现在需要连接UI元素到脚本：
   - 在Hierarchy中，将 `ResourceText` **拖拽** 到Inspector中 `Resource Display UI` 组件的 `Resource Text` 字段
   - 将 `TimerText` **拖拽** 到 `Timer Text` 字段
   - 将 `TestTickButton` **拖拽** 到 `Test Tick Button` 字段

### 第六步：测试运行（5分钟）

#### 6.1 开始测试
1. 点击Unity顶部的播放按钮 ▶️
2. 查看Game窗口，你应该看到：
   - 左上角显示资源面板，包含所有资源信息
   - 顶部显示定时器倒计时
   - 底部有一个"立即触发结算"按钮

#### 6.2 观察定时器
1. 定时器会从30秒（或15秒，如果Time Scale设为2）开始倒计时
2. 倒计时到0时，会自动触发一次结算
3. 在Console窗口中，你会看到详细的日志输出：
   ```
   === 第 1 个游戏周期开始 ===
   → 开始执行周期操作...
   → [1/9] 检查身份类型费用...
   支付 7 虚拟币，剩余余额：93
   → [2/9] 结算工作薪资...
   ...
   ```

#### 6.3 测试按钮功能
1. 点击底部的"立即触发结算"按钮
2. 应该会立即触发一次结算，不需要等待倒计时
3. 观察资源面板的变化：
   - 虚拟币会减少（支付连接费）
   - 心情值会降低（工作导致）
   - 存储使用会增加（数据产生）

#### 6.4 观察资源变化
- 每次结算后，查看资源面板的数值变化
- 存储空间会逐渐增加（每次增加0.5GB或1.2GB，取决于身份类型）
- 虚拟币会减少（支付各种费用）
- 心情值会降低（工作导致）

### 第七步：测试不同身份类型（可选，5分钟）

#### 7.1 修改身份类型
1. 停止游戏运行（再次点击播放按钮）
2. 在Hierarchy中选中 `GameManagers`
3. 在Inspector中找到 `Resource Manager` 组件
4. 找到 `Player Identity` 字段
5. 从下拉菜单中选择：
   - `Consciousness Linker`（意识连接者）- 默认
   - `Full Virtual`（完全虚拟人）

#### 7.2 比较差异
- **意识连接者**：
  - 初始资源占用较低
  - 每次结算需要支付5-10虚拟币的连接费
  - 每次产生0.5GB数据

- **完全虚拟人**：
  - 初始资源占用较高
  - 不需要支付连接费
  - 每次产生1.2GB数据

#### 7.3 重新运行测试
1. 点击播放按钮
2. 观察不同身份类型下的资源变化

---

## 🐛 常见问题和解决方案

### 问题1：脚本编译错误
**现象**：Console显示红色错误信息

**解决方法**：
1. 仔细阅读错误信息
2. 检查是否所有文件都在正确的位置
3. 确保脚本文件名与类名一致
4. 尝试重启Unity编辑器

### 问题2：UI不显示
**现象**：运行游戏时看不到UI

**解决方法**：
1. 检查Canvas是否存在
2. 检查Canvas的 `Render Mode` 是否为 `Screen Space - Overlay`
3. 检查UI元素是否在Canvas之下
4. 在Scene视图中查看UI元素的位置

### 问题3：定时器不工作
**现象**：定时器不倒计时或不触发结算

**解决方法**：
1. 检查 `GameTimerManager` 组件的 `Timer Enabled` 是否勾选
2. 检查游戏是否暂停（`Is Paused` 字段）
3. 查看Console是否有错误信息

### 问题4：资源数据不显示
**现象**：资源面板显示"资源数据加载中..."

**解决方法**：
1. 确保 `ResourceManager` 组件已添加
2. 检查Console是否有初始化错误
3. 确认 `ResourceDisplayUI` 的引用都已正确连接

---

## 💡 进阶功能

### 调试技巧

#### 1. 使用Debug Mode加速测试
- 在 `GameTimerManager` 中勾选 `Debug Mode`
- 周期从5分钟变为30秒
- 配合 `Time Scale` 可以进一步加速

#### 2. 手动触发结算
```csharp
// 在Console中输入或通过按钮调用
GameTimerManager.Instance.TriggerGameTickNow();
```

#### 3. 修改资源
```csharp
// 添加虚拟币
ResourceManager.Instance.AddVirtualCoin(1000, "测试");

// 改变心情值
ResourceManager.Instance.ChangeMoodValue(50, "测试");

// 清理存储数据
ResourceManager.Instance.CleanData(100f);
```

#### 4. 查看效率详情
```csharp
// 在Console中查看效率计算详情
Debug.Log(ResourceManager.Instance.GetEfficiencyBreakdown());
```

### 自定义配置

#### 修改初始资源
1. 打开 `PlayerResources.cs`
2. 在构造函数中修改初始值
3. 或者在Unity Inspector中直接修改

#### 修改周期时长
1. 选中 `GameManagers` 对象
2. 在 `GameTimerManager` 组件中：
   - `Game Tick Interval`：正常周期时长（秒）
   - `Debug Tick Interval`：调试周期时长（秒）

---

## 📝 Phase 2 完成检查清单

完成以下所有项目，表示Phase 2成功完成：

- [ ] 所有新脚本都已添加到项目中
- [ ] 脚本没有编译错误
- [ ] 创建了测试场景 `GameTestScene`
- [ ] 添加了所有管理器组件
- [ ] 创建了资源显示UI
- [ ] UI正确连接到脚本
- [ ] 游戏可以正常运行
- [ ] 定时器正常倒计时
- [ ] 点击测试按钮可以立即触发结算
- [ ] 资源数据正确显示
- [ ] 每次结算后资源会发生变化
- [ ] Console显示详细的结算日志
- [ ] 理解了每个脚本的功能
- [ ] 可以修改配置参数并看到效果

全部完成后，恭喜你！你已经完成了Phase 2的开发！🎉

---

## 🚀 下一步：Phase 3

完成Phase 2后，我们将开发：
- 工作系统（承接工作、薪资结算）
- 技能系统（学习技能、算力分配）
- 工作市场UI

敬请期待！💪
