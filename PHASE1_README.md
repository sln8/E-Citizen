# Phase 1: Unity项目搭建 + Firebase集成 + 登录系统

## 📋 概述

本阶段完成了《电子公民》游戏的基础架构搭建，包括：
- ✅ Unity项目结构搭建
- ✅ Firebase集成框架
- ✅ 多平台登录系统（Google、Facebook、Apple、测试账号）
- ✅ 详细的中文注释
- ✅ 完整的配置指南

## 📁 项目结构

```
E-Citizens/
├── Assets/
│   ├── Scenes/              # 场景文件夹
│   ├── Scripts/             # 脚本文件夹
│   │   ├── Core/           # 核心系统脚本
│   │   ├── Firebase/       # Firebase相关脚本
│   │   │   ├── FirebaseConfig.cs          # Firebase配置管理
│   │   │   └── FirebaseInitializer.cs     # Firebase初始化
│   │   ├── Authentication/ # 认证系统脚本
│   │   │   └── AuthenticationManager.cs   # 认证管理器
│   │   ├── UI/            # UI相关脚本
│   │   │   └── LoginUIManager.cs         # 登录UI管理
│   │   ├── Managers/      # 游戏管理器
│   │   │   └── GameManager.cs            # 主游戏管理器
│   │   ├── Data/          # 数据类
│   │   │   └── UserData.cs               # 用户数据类
│   │   └── Utils/         # 工具类（待扩展）
│   ├── Prefabs/           # 预制件文件夹
│   ├── Resources/         # 资源文件夹
│   │   ├── UI/           # UI资源
│   │   ├── Audio/        # 音频资源
│   │   └── Data/         # 数据资源
│   └── Plugins/          # 第三方插件
└── SETUP_GUIDE.md        # 详细配置指南
```

## 🔧 核心脚本说明

### 1. FirebaseConfig.cs
**功能**: Firebase配置管理
- 管理所有Firebase相关的配置参数
- 支持编辑器模拟模式和真机模式
- 测试模式开关控制
- 详细的中文注释说明每个配置项

**关键配置**:
```csharp
public bool enableFirebase = true;           // 是否启用Firebase
public bool simulateInEditor = true;         // 编辑器模拟模式
public bool isTestMode = true;               // 测试模式
public bool enableGoogleSignIn = true;       // Google登录
public bool enableFacebookSignIn = true;     // Facebook登录
public bool enableAppleSignIn = true;        // Apple登录
public bool enableTestAccounts = true;       // 测试账号
```

### 2. FirebaseInitializer.cs
**功能**: Firebase初始化管理
- 负责初始化Firebase所有服务
- 支持模拟模式和真实模式
- 提供初始化成功/失败事件
- 自动处理依赖检查

**使用方式**:
```csharp
// 监听初始化完成事件
FirebaseInitializer.Instance.OnFirebaseInitialized += () => {
    Debug.Log("Firebase已就绪");
};

// 检查是否初始化完成
if (FirebaseInitializer.Instance.IsFirebaseReady()) {
    // 可以使用Firebase功能
}
```

### 3. AuthenticationManager.cs
**功能**: 认证系统管理
- 支持4种登录方式：Google、Facebook、Apple、测试账号
- 自动处理登录流程
- 登录状态管理
- 用户数据管理

**登录方法**:
```csharp
// Google登录
AuthenticationManager.Instance.SignInWithGoogle();

// Facebook登录
AuthenticationManager.Instance.SignInWithFacebook();

// Apple登录（仅iOS）
AuthenticationManager.Instance.SignInWithApple();

// 测试账号登录
AuthenticationManager.Instance.SignInWithTestAccount("username", "password");

// 快速创建测试账号
AuthenticationManager.Instance.CreateQuickTestAccount();
```

**事件监听**:
```csharp
// 监听登录成功
AuthenticationManager.Instance.OnLoginSuccess += (userData) => {
    Debug.Log($"登录成功: {userData.username}");
};

// 监听登录失败
AuthenticationManager.Instance.OnLoginFailed += (errorMessage) => {
    Debug.LogError($"登录失败: {errorMessage}");
};
```

### 4. UserData.cs
**功能**: 用户数据模型
- 存储用户基本信息
- 存储游戏数据
- 支持序列化

**数据结构**:
```csharp
public class UserData {
    public string userId;           // 用户ID
    public string username;         // 用户名
    public string email;           // 邮箱
    public LoginProvider loginProvider; // 登录方式
    public int level;              // 等级
    public int virtualCoin;        // 虚拟币
    public int moodValue;          // 心情值
    // ... 更多游戏数据
}
```

### 5. LoginUIManager.cs
**功能**: 登录界面管理
- 管理所有登录按钮和输入框
- 处理用户交互
- 显示登录状态和加载动画
- 测试模式提示

**需要的UI元素**:
- Google登录按钮
- Facebook登录按钮
- Apple登录按钮（iOS）
- 测试账号登录按钮
- 快速创建测试账号按钮
- 用户名输入框
- 密码输入框
- 状态文本
- 加载面板

### 6. GameManager.cs
**功能**: 游戏主管理器
- 游戏整体流程控制
- 状态管理（初始化、登录、主游戏等）
- 场景切换
- 游戏数据保存/加载
- 调试信息显示

**游戏状态**:
```csharp
public enum GameState {
    Initializing,        // 初始化中
    Login,              // 登录界面
    CharacterCreation,  // 角色创建
    MainGame,           // 主游戏
    Paused,             // 暂停
    Loading             // 加载中
}
```

## 🎮 使用步骤（零基础开发者）

### 第一步：理解项目结构
1. 打开Unity Hub
2. 打开 `E-Citizens` 项目
3. 在Project窗口中浏览 `Assets/Scripts/` 文件夹
4. 每个脚本文件都有详细的中文注释，建议先阅读理解

### 第二步：配置Firebase
**重要**: 必须先完成Firebase配置，否则登录功能无法正常工作

1. 阅读根目录的 `SETUP_GUIDE.md` 文件
2. 按照指南创建Firebase项目
3. 下载配置文件（`google-services.json` 和 `GoogleService-Info.plist`）
4. 将配置文件放到 `Assets/` 文件夹
5. 安装Firebase Unity SDK

**详细步骤**: 请参考 `SETUP_GUIDE.md` 中的"Firebase项目创建"和"Firebase SDK安装"章节

### 第三步：创建登录场景
1. 在Unity中，右键 `Assets/Scenes/` 文件夹
2. 选择 `Create` -> `Scene`
3. 命名为 `LoginScene`
4. 双击打开场景

### 第四步：添加管理器对象
1. 在Hierarchy窗口右键，选择 `Create Empty`
2. 重命名为 `GameManager`
3. 在Inspector窗口，点击 `Add Component`
4. 搜索并添加以下组件：
   - `GameManager`
   - `FirebaseConfig`
   - `FirebaseInitializer`
   - `AuthenticationManager`

### 第五步：创建登录UI
1. 在Hierarchy窗口右键，选择 `UI` -> `Canvas`
2. 创建以下UI元素（都在Canvas下）：

**按钮**:
- 右键Canvas -> `UI` -> `Button - TextMeshPro`
- 创建4个按钮，分别命名为：
  - `GoogleLoginButton`
  - `FacebookLoginButton`
  - `AppleLoginButton`
  - `TestAccountLoginButton`
  - `QuickTestAccountButton`

**输入框**:
- 右键Canvas -> `UI` -> `InputField - TextMeshPro`
- 创建2个输入框，分别命名为：
  - `UsernameInput`
  - `PasswordInput`

**文本显示**:
- 右键Canvas -> `UI` -> `Text - TextMeshPro`
- 创建1个文本，命名为 `StatusText`

**加载面板**:
- 右键Canvas -> `UI` -> `Panel`
- 命名为 `LoadingPanel`
- 添加一个旋转的图片或文本显示"加载中..."

### 第六步：连接UI和脚本
1. 创建一个空对象，命名为 `LoginUIManager`
2. 添加 `LoginUIManager` 组件
3. 在Inspector中，将UI元素拖拽到对应的字段：
   - `Google Login Button` -> 拖拽 `GoogleLoginButton`
   - `Facebook Login Button` -> 拖拽 `FacebookLoginButton`
   - 依此类推...

### 第七步：测试
1. 点击Unity顶部的播放按钮 ▶️
2. 在Console窗口观察日志输出
3. 应该看到：
   - "=== Firebase配置信息 ==="
   - "Firebase已就绪，认证系统可以使用"
4. 点击各个登录按钮测试
5. 在编辑器中会使用模拟模式，应该能看到"登录成功（模拟）"

## 🧪 测试模式说明

### 编辑器模拟模式
当在Unity编辑器中运行时：
- `FirebaseConfig.simulateInEditor = true`
- 不需要真实的Firebase连接
- 所有登录都会返回模拟的成功结果
- 方便开发和调试

### 测试账号系统
- 开启 `FirebaseConfig.enableTestAccounts = true`
- 可以使用测试账号快速登录
- 测试账号格式：`test_user_XXXXX`
- 密码至少6位字符

### 生产模式
发布正式版本时：
- 设置 `FirebaseConfig.isTestMode = false`
- 关闭 `FirebaseConfig.simulateInEditor`
- 确保所有Firebase服务正确配置

## 📝 代码注释说明

所有代码都包含详细的中文注释，分为以下几类：

### 1. 类和方法注释
```csharp
/// <summary>
/// 类或方法的功能说明
/// </summary>
```

### 2. 参数注释
```csharp
[Tooltip("这个参数的作用")]
public bool enableFeature;
```

### 3. 代码块注释
```csharp
// 步骤1: 初始化配置
// 这里解释这段代码在做什么
```

### 4. 重要提示
```csharp
// 注意：这里需要特别注意的内容
// 实际项目中应该如何处理
```

## 🔍 调试技巧

### 查看日志
1. 打开Console窗口：`Window` -> `General` -> `Console`
2. 查看不同颜色的日志：
   - 白色：普通信息
   - 黄色：警告信息
   - 红色：错误信息

### 使用调试UI
1. 运行游戏后，按 `F1` 键显示/隐藏调试信息
2. 调试信息显示：
   - 当前游戏状态
   - Firebase状态
   - 登录状态
   - 用户信息

### 断点调试
1. 在代码行号左侧点击，设置断点（红点）
2. 在Unity中启用调试模式
3. 使用Visual Studio或VS Code附加到Unity进程
4. 逐步执行代码，查看变量值

## 🚀 下一步开发

完成Phase 1后，可以继续开发：

### Phase 2: 核心资源系统
- 创建资源管理器（内存、CPU、网速、算力、存储）
- 实现资源分配和计算系统
- 5分钟定时器系统

### Phase 3: 工作系统
- 工作数据结构
- 工作市场
- 薪资结算
- 多工作位管理

### Phase 4: 技能系统
- 技能下载和安装
- 算力分配
- 掌握度计算

## 📚 学习资源

### Unity官方文档
- [Unity中文文档](https://docs.unity.cn/)
- [Unity脚本API](https://docs.unity3d.com/ScriptReference/)

### Firebase文档
- [Firebase Unity SDK](https://firebase.google.com/docs/unity/setup)
- [Firebase Authentication](https://firebase.google.com/docs/auth/unity/start)

### C#学习
- [C#基础教程](https://docs.microsoft.com/zh-cn/dotnet/csharp/)
- [Unity C#脚本教程](https://learn.unity.com/tutorial/scripts-as-behaviour-components)

## 💡 常见问题

### Q: 为什么编辑器中点击登录没有反应？
A: 检查以下内容：
1. Console是否有错误信息
2. UI按钮是否正确连接到LoginUIManager
3. GameManager是否已添加到场景中

### Q: 如何切换模拟模式和真实模式？
A: 在Hierarchy中选择FirebaseConfig对象，在Inspector中：
- 勾选 `Simulate In Editor` 使用模拟模式
- 取消勾选使用真实Firebase

### Q: 测试账号是否会保存到Firebase？
A: 
- 模拟模式：不会保存
- 真实模式：会保存到Firebase Authentication

### Q: 如何查看Firebase中的用户数据？
A: 
1. 打开Firebase Console
2. 进入 Authentication -> Users
3. 可以看到所有注册用户

## 📞 技术支持

如果遇到问题：
1. 查看Console的详细错误信息
2. 参考 `SETUP_GUIDE.md` 中的"常见问题和解决方案"
3. 检查代码中的注释说明
4. 记录错误信息和操作步骤

---

## ✅ Phase 1 完成检查清单

- [ ] Unity项目可以正常打开
- [ ] 所有脚本没有编译错误
- [ ] Firebase配置文件已放置在正确位置
- [ ] 登录场景已创建
- [ ] UI元素已正确连接
- [ ] 在编辑器中可以测试登录（模拟模式）
- [ ] Console显示"Firebase初始化成功"
- [ ] Console显示"认证系统已就绪"
- [ ] 点击登录按钮有日志输出
- [ ] 理解了所有核心脚本的功能

全部完成后，恭喜你！你已经完成了《电子公民》游戏的Phase 1开发！🎉
