# 初始选择功能实现指南

## 功能说明

根据游戏设计文档，本次实现了完整的初始选择功能，让玩家在首次进入游戏时选择自己的身份类型（脑机连接者或纯虚拟人），不同的选择会有不同的初始属性配置。

---

## 游戏流程

### 完整流程图

```
登录成功
    ↓
检查 hasCreatedCharacter
    ↓
┌───────────────┐
│ 是否为新用户？  │
└───────┬───────┘
        │
    ┌───┴───┐
    │       │
   否      是
    │       │
    │   显示初始选择界面
    │       │
    │   ┌─────────────┐
    │   │ 选择身份类型  │
    │   └──────┬──────┘
    │          │
    │   ┌──────┴──────┐
    │   │             │
    │ 脑机连接者    纯虚拟人
    │   │             │
    │   └──────┬──────┘
    │          │
    │   初始化角色属性
    │          │
    │   设置 hasCreatedCharacter = true
    │          │
    └──────┬───┘
           │
       进入游戏场景
```

### 详细说明

1. **新用户流程**（hasCreatedCharacter = false）：
   - 登录成功后进入初始选择界面
   - 显示两个选项的详细信息
   - 玩家选择身份类型
   - 根据选择初始化资源配置
   - 标记 hasCreatedCharacter = true
   - 进入正式游戏场景

2. **老用户流程**（hasCreatedCharacter = true）：
   - 登录成功后直接进入游戏场景
   - 不再显示初始选择界面
   - 使用已保存的资源配置

---

## 新增文件列表

### 核心脚本（2个）

1. **InitialSelectionUI.cs** (约350行)
   - 位置：`Assets/Scripts/UI/InitialSelectionUI.cs`
   - 功能：初始选择界面UI管理
   - 包含：按钮处理、信息展示、选择确认

2. **InitialSelectionManager.cs** (约140行)
   - 位置：`Assets/Scripts/Managers/InitialSelectionManager.cs`
   - 功能：初始选择流程管理
   - 包含：显示/隐藏界面、事件协调

### 修改的文件（1个）

3. **GameManager.cs**
   - 修改：`IsNewUser()` 方法，使用 `hasCreatedCharacter` 字段
   - 新增：`CompleteCharacterCreation()` 方法
   - 新增：`SaveUserDataToFirebase()` 方法
   - 修改：`EnterCharacterCreation()` 方法，调用 `InitialSelectionManager`

---

## Unity操作步骤

### 步骤1：在GameScene中创建初始选择UI

#### 1.1 创建主Canvas（如果不存在）
1. 在Hierarchy中右键 → UI → Canvas
2. 命名为 "InitialSelectionCanvas"
3. 设置Canvas属性：
   - Render Mode: Screen Space - Overlay
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080

#### 1.2 创建主面板
1. 在InitialSelectionCanvas下右键 → UI → Panel
2. 命名为 "SelectionPanel"
3. 设置颜色为半透明黑色（例如：RGBA 0, 0, 0, 200）

#### 1.3 创建标题和描述
1. 在SelectionPanel下创建：
   - TextMeshPro - Text: 命名为 "TitleText"
     - 文本：欢迎来到电子公民
     - 字号：48
     - 对齐：居中
     - 位置：顶部中央
   
   - TextMeshPro - Text: 命名为 "DescriptionText"
     - 文本：请选择你的存在形式，这将决定你在虚拟世界中的起始配置
     - 字号：24
     - 对齐：居中
     - 位置：标题下方

#### 1.4 创建脑机连接者选项
1. 在SelectionPanel下创建空GameObject，命名为 "ConsciousnessLinkerOption"
2. 在其下创建：
   - Button: 命名为 "ConsciousnessLinkerButton"
     - 大小：400x100
     - 位置：屏幕左侧中间
   
   - 在Button下创建TextMeshPro，命名为 "NameText"
     - 文本：脑机连接者
     - 字号：32
   
   - 在ConsciousnessLinkerOption下创建TextMeshPro，命名为 "DescText"
     - 文本：现实意识与虚拟世界的连接者
     - 字号：18
     - 位置：Button下方
   
   - 在ConsciousnessLinkerOption下创建Panel，命名为 "InfoPanel"
     - 大小：350x500
     - 位置：Button下方
     - 初始状态：禁用（Inactive）
   
   - 在InfoPanel下创建TextMeshPro，命名为 "InfoText"
     - 启用富文本
     - 字号：16
     - 对齐：左对齐

#### 1.5 创建纯虚拟人选项
1. 复制整个ConsciousnessLinkerOption
2. 重命名为 "FullVirtualOption"
3. 修改名称文本为 "纯虚拟人"
4. 修改描述文本为 "完全数字化的虚拟生命体"
5. 位置：屏幕右侧中间

#### 1.6 创建确认按钮
1. 在SelectionPanel下创建Button，命名为 "ConfirmButton"
2. 大小：300x80
3. 位置：底部中央
4. 在Button下创建TextMeshPro，命名为 "ConfirmButtonText"
   - 文本：确认选择
   - 字号：32

### 步骤2：添加InitialSelectionUI脚本

1. 选中InitialSelectionCanvas对象
2. 在Inspector中点击 "Add Component"
3. 搜索并添加 "InitialSelectionUI" 脚本
4. 连接所有UI引用：
   - Selection Panel: 拖入SelectionPanel
   - Title Text: 拖入TitleText
   - Description Text: 拖入DescriptionText
   - Consciousness Linker Button: 拖入脑机连接者按钮
   - Consciousness Linker Name Text: 拖入名称文本
   - Consciousness Linker Desc Text: 拖入描述文本
   - Consciousness Linker Info Panel: 拖入信息面板
   - Consciousness Linker Info Text: 拖入详细信息文本
   - Full Virtual Button: 拖入纯虚拟人按钮
   - Full Virtual Name Text: 拖入名称文本
   - Full Virtual Desc Text: 拖入描述文本
   - Full Virtual Info Panel: 拖入信息面板
   - Full Virtual Info Text: 拖入详细信息文本
   - Confirm Button: 拖入确认按钮
   - Confirm Button Text: 拖入确认按钮文本

### 步骤3：创建InitialSelectionManager

1. 在Hierarchy中创建空GameObject，命名为 "InitialSelectionManager"
2. 在Inspector中点击 "Add Component"
3. 搜索并添加 "InitialSelectionManager" 脚本
4. 在Inspector中：
   - Selection UI: 拖入刚才创建的InitialSelectionCanvas对象

### 步骤4：初始状态设置

1. 选中SelectionPanel，在Inspector中禁用（取消勾选）
   - 这样游戏开始时不会显示选择界面
   - 只有新用户登录时才会自动显示

---

## 测试流程

### 测试1：新用户流程

1. **准备工作**：
   - 删除PlayerPrefs（如果有本地保存）
   - 或使用新的测试账号登录

2. **执行测试**：
   ```
   1. 启动游戏
   2. 登录（使用新账号或清空数据后的账号）
   3. 验证：显示初始选择界面
   4. 点击"脑机连接者"按钮
   5. 验证：显示脑机连接者详细信息
   6. 点击"纯虚拟人"按钮
   7. 验证：显示纯虚拟人详细信息
   8. 选择一个身份类型
   9. 点击"确认选择"按钮
   10. 验证：
       - Console输出选择的身份类型
       - ResourceManager初始化对应的资源配置
       - hasCreatedCharacter设置为true
       - 进入游戏场景
   ```

3. **验证资源配置**：
   - 在Hierarchy中找到ResourceManager
   - 在Inspector中查看playerResources的值
   - 脑机连接者应该是：内存2GB已用、CPU 1核已用
   - 纯虚拟人应该是：内存4GB已用、CPU 2核已用

### 测试2：老用户流程

1. **准备工作**：
   - 使用测试1中完成初始选择的账号

2. **执行测试**：
   ```
   1. 重启游戏
   2. 使用同一账号登录
   3. 验证：直接进入游戏场景，不显示初始选择界面
   4. 验证：资源配置保持不变
   ```

### 测试3：切换选择测试

1. **执行测试**：
   ```
   1. 显示初始选择界面
   2. 点击"脑机连接者"
   3. 验证：显示脑机连接者信息，确认按钮启用
   4. 点击"纯虚拟人"
   5. 验证：显示纯虚拟人信息，脑机连接者信息隐藏
   6. 选择并确认
   ```

---

## 数据配置说明

### 脑机连接者（ConsciousnessLinker）

```csharp
优势：
  - 较低的初始资源占用
  - 每5分钟产生数据较少

劣势：
  - 需要每5分钟支付连接费（5-10虚拟币）

初始配置：
  内存：16GB（已用2GB）
  CPU：8核（已用1核）
  网速：1000Mbps（已用50Mbps）
  算力：100（已用10）
  存储：500GB（已用20GB）
  虚拟币：100
  心情值：10

每5分钟产生数据：0.5GB
```

### 纯虚拟人（FullVirtual）

```csharp
优势：
  - 无需支付连接费
  - 完全自由的虚拟生活

劣势：
  - 较高的初始资源占用
  - 每5分钟产生数据较多

初始配置：
  内存：16GB（已用4GB）
  CPU：8核（已用2核）
  网速：1000Mbps（已用100Mbps）
  算力：100（已用20）
  存储：500GB（已用50GB）
  虚拟币：100
  心情值：10

每5分钟产生数据：1.2GB
```

---

## 事件系统

### InitialSelectionUI事件

```csharp
// 选择完成事件
public event Action<IdentityType> OnSelectionCompleted;

// 触发时机：玩家点击确认按钮后
// 参数：选择的身份类型（ConsciousnessLinker 或 FullVirtual）
```

### 调用流程

```
InitialSelectionUI.OnConfirmButtonClicked()
    ↓
触发 OnSelectionCompleted 事件
    ↓
InitialSelectionManager.OnSelectionCompleted()
    ↓
GameManager.CompleteCharacterCreation()
    ↓
ResourceManager.SetPlayerIdentity()
    ↓
触发 ResourceManager.OnResourcesChanged 事件
```

---

## 常见问题解答

### Q1: 初始选择界面不显示怎么办？

**检查清单**：
1. SelectionPanel初始是否设置为Inactive？（应该是）
2. GameManager是否正确识别为新用户？检查Console日志
3. InitialSelectionManager是否正确挂载在场景中？
4. InitialSelectionUI的所有引用是否正确连接？

### Q2: 选择后资源配置不正确？

**排查步骤**：
1. 在Console中查看"选择身份类型：XXX"的日志
2. 检查ResourceManager中的playerIdentity字段
3. 检查PlayerResources构造函数是否正确执行
4. 在ResourceManager的Inspector中查看实际的资源值

### Q3: 老用户仍然显示初始选择界面？

**可能原因**：
1. UserData.hasCreatedCharacter字段没有正确保存
2. 使用了新的账号或清空了数据
3. Firebase数据同步失败

**解决方案**：
1. 检查SaveUserDataToFirebase方法是否正确调用
2. 实现Firebase Firestore的保存和加载逻辑
3. 添加本地PlayerPrefs作为备份存储

### Q4: 如何修改初始资源配置？

**修改位置**：
- 打开：`Assets/Scripts/Data/PlayerResources.cs`
- 找到：`PlayerResources(int identityType)` 构造函数
- 修改：对应身份类型的初始值

**修改示例**：
```csharp
if (identityType == 0)
{
    // 意识连接者配置
    memoryUsed = 3f;  // 修改已用内存
    cpuUsed = 1.5f;   // 修改已用CPU
    // ... 其他配置
}
```

### Q5: 如何添加第三种身份类型？

**步骤**：
1. 在 `PlayerResources.cs` 的 `IdentityType` 枚举中添加新类型
2. 在 `PlayerResources` 构造函数中添加新类型的配置
3. 在 `InitialSelectionUI.cs` 中添加新的UI选项
4. 在UI中创建新的选项按钮和信息面板

---

## 后续优化建议

### 1. Firebase数据持久化

当前实现中，`SaveUserDataToFirebase()` 方法只是占位符。建议：

```csharp
private async void SaveUserDataToFirebase(UserData userData)
{
    try
    {
        var firestore = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
        var docRef = firestore.Collection("users").Document(userData.userId);
        
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "hasCreatedCharacter", userData.hasCreatedCharacter },
            { "identityType", userData.identityType },
            { "level", userData.level },
            { "virtualCoin", userData.virtualCoin },
            // ... 其他字段
        };
        
        await docRef.SetAsync(data, SetOptions.MergeAll);
        Debug.Log("✓ 用户数据已保存到Firebase");
    }
    catch (Exception e)
    {
        Debug.LogError($"保存用户数据失败: {e.Message}");
    }
}
```

### 2. 本地缓存备份

建议使用PlayerPrefs作为本地备份：

```csharp
// 保存
PlayerPrefs.SetInt("HasCreatedCharacter", userData.hasCreatedCharacter ? 1 : 0);
PlayerPrefs.SetInt("IdentityType", userData.identityType);
PlayerPrefs.Save();

// 加载
userData.hasCreatedCharacter = PlayerPrefs.GetInt("HasCreatedCharacter", 0) == 1;
userData.identityType = PlayerPrefs.GetInt("IdentityType", 0);
```

### 3. UI动画效果

建议添加：
- 面板淡入淡出动画
- 按钮悬停效果
- 选择时的高亮动画
- 确认时的过渡效果

### 4. 音效反馈

建议添加：
- 按钮点击音效
- 选择切换音效
- 确认选择音效

### 5. 多语言支持

当前文本是硬编码的中文，建议：
- 使用Unity Localization Package
- 将所有文本提取到本地化表
- 支持中英文切换

---

## 代码统计

- **新增脚本**：2个
- **修改脚本**：1个
- **总代码行数**：约500行（含详细注释）
- **注释率**：约40%
- **开发时间**：约2-3小时

---

## 集成验证清单

在完成Unity操作后，请验证以下内容：

- [ ] InitialSelectionUI脚本正确挂载到Canvas上
- [ ] 所有UI引用都已正确连接（无null引用）
- [ ] InitialSelectionManager脚本正确挂载到场景中
- [ ] SelectionPanel初始状态为Inactive
- [ ] 两个InfoPanel初始状态为Inactive
- [ ] Console没有脚本编译错误
- [ ] 新用户登录能正确显示初始选择界面
- [ ] 点击按钮能正确显示/隐藏信息面板
- [ ] 确认按钮初始禁用，选择后启用
- [ ] 选择完成后能正确初始化资源
- [ ] hasCreatedCharacter正确设置为true
- [ ] 老用户登录不显示初始选择界面

---

**《电子公民》开发团队**  
初始选择功能实现完成！🎉
