# 《电子公民》快速开始指南

## 🚀 5分钟快速体验

如果你想立即在Unity编辑器中测试登录系统，按照以下步骤操作：

### 步骤1: 打开项目（1分钟）
1. 打开Unity Hub
2. 点击"打开"，选择 `E-Citizens` 文件夹
3. 等待Unity加载完成（可能需要1-2分钟）

### 步骤2: 创建测试场景（2分钟）
1. 在Unity中，点击菜单 `File` -> `New Scene`
2. 保存场景：`File` -> `Save As` -> 命名为 `TestLoginScene`
3. 保存位置：`Assets/Scenes/TestLoginScene.unity`

### 步骤3: 添加核心管理器（1分钟）
1. 在Hierarchy窗口（场景层次结构）中，右键点击空白处
2. 选择 `Create Empty` 创建一个空对象
3. 在Inspector窗口（检查器）中，将其命名为 `Managers`
4. 点击Inspector底部的 `Add Component` 按钮
5. 依次添加以下组件：
   - 输入 `GameManager`，回车添加
   - 输入 `FirebaseConfig`，回车添加
   - 输入 `FirebaseInitializer`，回车添加
   - 输入 `AuthenticationManager`，回车添加

### 步骤4: 运行测试（1分钟）
1. 点击Unity顶部中间的播放按钮 ▶️
2. 打开Console窗口：`Window` -> `General` -> `Console`
3. 你应该看到以下日志输出：

```
=== Firebase配置信息 ===
Firebase启用状态: True
编辑器模拟模式: True
测试模式: True
Google登录: True
Facebook登录: True
Apple登录: True
测试账号登录: True
====================
=== 开始游戏初始化 ===
→ 初始化Firebase配置...
✓ Firebase配置初始化完成
→ 初始化Firebase服务...
=== 开始模拟Firebase初始化 ===
✓ Firebase Authentication (模拟)
✓ Firebase Firestore (模拟)
✓ Firebase Storage (模拟)
✓ Firebase Analytics (模拟)
=== Firebase模拟初始化完成 ===
Firebase初始化成功: 模拟模式初始化成功
Firebase已就绪，认证系统可以使用
=== 游戏初始化完成 ===
```

4. 按 `F1` 键，你会看到调试信息面板显示：
```
=== 调试信息 ===
游戏状态: Login
Firebase状态: 已初始化
登录状态: 未登录
平台: Editor
测试模式: True
```

### 🎉 恭喜！
基础系统已经正常运行！现在你可以：
1. 查看各个脚本的详细注释，理解代码逻辑
2. 按照 `PHASE1_README.md` 创建完整的登录UI
3. 按照 `SETUP_GUIDE.md` 配置真实的Firebase功能

---

## 🧪 立即测试登录功能

如果你想测试登录功能（无需创建UI）：

### 方法1: 使用代码直接测试
1. 在Unity编辑器中，点击菜单 `Window` -> `General` -> `Console`
2. 在Console窗口底部，有一个输入框
3. 运行游戏后，在Console中输入以下命令进行测试：

```csharp
// 测试Google登录
AuthenticationManager.Instance.SignInWithGoogle();

// 测试Facebook登录
AuthenticationManager.Instance.SignInWithFacebook();

// 测试快速创建测试账号
AuthenticationManager.Instance.CreateQuickTestAccount();
```

### 方法2: 创建简单的测试脚本
1. 在 `Assets/Scripts/` 创建新脚本 `TestLogin.cs`：

```csharp
using UnityEngine;

public class TestLogin : MonoBehaviour
{
    void Start()
    {
        // 等待Firebase初始化
        FirebaseInitializer.Instance.OnFirebaseInitialized += OnFirebaseReady;
    }

    void OnFirebaseReady()
    {
        Debug.Log("准备测试登录功能...");
    }

    void Update()
    {
        // 按1键测试Google登录
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("测试Google登录");
            AuthenticationManager.Instance.SignInWithGoogle();
        }

        // 按2键测试Facebook登录
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("测试Facebook登录");
            AuthenticationManager.Instance.SignInWithFacebook();
        }

        // 按3键测试Apple登录
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("测试Apple登录");
            AuthenticationManager.Instance.SignInWithApple();
        }

        // 按4键快速创建测试账号
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("快速创建测试账号");
            AuthenticationManager.Instance.CreateQuickTestAccount();
        }
    }
}
```

2. 将这个脚本添加到场景中的 `Managers` 对象上
3. 运行游戏，按1、2、3、4键测试不同的登录方式
4. 在Console查看结果

### 测试结果示例
按下键盘1后，你会看到：

```
测试Google登录
=== 开始Google登录流程 ===
模拟Google登录成功
<color=green>登录成功: Google登录成功（模拟）</color>
用户ID: google_test_xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
用户名: Google测试用户
登录方式: Google
登录信息已保存到本地
<color=green>[GameManager] 用户登录成功: Google测试用户</color>
```

---

## 📱 下一步：创建完整的登录UI

完成快速测试后，你可以：

### 选项1: 简单UI（推荐初学者）
1. 创建一个Canvas
2. 添加几个Button
3. 将按钮的OnClick事件连接到登录方法
4. 参考 `PHASE1_README.md` 的第五步

### 选项2: 完整UI（推荐完整体验）
1. 按照 `PHASE1_README.md` 的完整步骤
2. 创建所有UI元素
3. 使用 `LoginUIManager` 管理UI
4. 体验完整的登录流程

### 选项3: 直接配置Firebase（推荐真机测试）
1. 按照 `SETUP_GUIDE.md` 配置Firebase
2. 下载并导入Firebase SDK
3. 在真机上测试真实登录功能

---

## 🐛 遇到问题？

### 问题1: Console显示很多红色错误
**可能原因**: TextMeshPro未安装

**解决方法**:
1. 点击菜单 `Window` -> `TextMeshPro` -> `Import TMP Essential Resources`
2. 点击 `Import` 按钮
3. 等待导入完成
4. 重新运行游戏

### 问题2: 找不到某些组件
**可能原因**: 脚本编译错误

**解决方法**:
1. 打开Console窗口
2. 查看红色错误信息
3. 双击错误可以跳转到问题代码
4. 检查代码是否完整复制

### 问题3: 游戏运行但没有日志输出
**可能原因**: Console日志被过滤

**解决方法**:
1. 在Console窗口右上角
2. 确保勾选了 `Collapse`、`Clear on Play`、`Error Pause`
3. 点击日志类型按钮（Info、Warning、Error）确保都显示

### 问题4: 按F1没有显示调试信息
**可能原因**: GameManager的showDebugUI未启用

**解决方法**:
1. 在Hierarchy中选择 `Managers` 对象
2. 在Inspector中找到 `GameManager` 组件
3. 勾选 `Show Debug UI`

---

## 💡 使用技巧

### 技巧1: 快速重新加载场景
运行游戏时按 `Ctrl+R`（Mac: `Cmd+R`）可以快速重新加载当前场景

### 技巧2: 暂停和单步调试
运行游戏时按 `Ctrl+Shift+P`（Mac: `Cmd+Shift+P`）可以暂停游戏，然后可以单步调试

### 技巧3: 查看GameObject属性
运行游戏时，在Hierarchy中选择对象，可以实时查看和修改其属性值

### 技巧4: 保存Console日志
在Console窗口右上角点击三个点的菜单，选择 `Export`可以导出日志到文件

---

## 📚 继续学习

完成快速开始后，建议按以下顺序深入学习：

1. **阅读代码注释**（30分钟）
   - 打开 `FirebaseConfig.cs` 阅读注释
   - 理解单例模式的实现
   - 学习如何使用事件系统

2. **创建登录UI**（1小时）
   - 按照 `PHASE1_README.md` 创建UI
   - 理解UI和脚本的连接方式
   - 测试完整的登录流程

3. **配置Firebase**（1-2小时）
   - 按照 `SETUP_GUIDE.md` 配置
   - 理解Firebase的工作原理
   - 在真机上测试

4. **准备Phase 2**（规划）
   - 阅读游戏设计文档
   - 了解资源系统的设计
   - 思考如何实现

---

## 🎯 目标检查

完成快速开始后，你应该能够：
- ✅ 在Unity编辑器中运行项目
- ✅ 看到Firebase初始化成功的日志
- ✅ 理解项目的基本结构
- ✅ 测试登录功能（模拟模式）
- ✅ 查看调试信息
- ✅ 知道如何查找和解决问题

---

**祝你开发顺利！如有问题，请查看详细文档或留言咨询。** 🚀
