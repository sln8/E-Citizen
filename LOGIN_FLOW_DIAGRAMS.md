# 登录流程可视化图表

## 整体流程图

```
┌─────────────────────────────────────────────────────────────────┐
│                         启动游戏                                  │
│                            ↓                                     │
│                      加载 LoginScene                              │
│                            ↓                                     │
│              ┌─────────────────────────┐                         │
│              │  LoginUIManager         │                         │
│              │  - Google 登录          │                         │
│              │  - Facebook 登录        │                         │
│              │  - Apple 登录           │                         │
│              │  - 测试账号登录          │                         │
│              └───────────┬─────────────┘                         │
│                          │                                       │
│                     用户点击登录                                   │
│                          ↓                                       │
│              ┌─────────────────────────┐                         │
│              │ AuthenticationManager   │                         │
│              │ - 验证用户身份           │                         │
│              │ - LoadUserGameData()    │                         │
│              │   (从 PlayerPrefs 加载)  │                         │
│              └───────────┬─────────────┘                         │
│                          │                                       │
│                     登录成功 ✓                                     │
│                          │                                       │
│              ┌───────────┴───────────┐                           │
│              │  OnLoginSuccess()     │                           │
│              │  检查 hasCreatedChar  │                           │
│              └───────────┬───────────┘                           │
│                          │                                       │
│          ┌───────────────┴───────────────┐                       │
│          │                               │                       │
│    hasCreatedCharacter                hasCreatedCharacter        │
│        = false                             = true                │
│          │                               │                       │
│          ↓                               ↓                       │
│  ┌───────────────┐              ┌───────────────┐               │
│  │ SelectScene   │              │  GameScene    │               │
│  └───────┬───────┘              └───────┬───────┘               │
│          │                              │                       │
│          │                              ↓                       │
│          │                     ┌────────────────┐               │
│          │                     │ OnSceneLoaded()│               │
│          │                     │ LoadUserData() │               │
│          │                     │ ↓              │               │
│          │                     │ ResourceManager│               │
│          │                     │ .SetIdentity() │               │
│          │                     └────────────────┘               │
│          ↓                              │                       │
│  ┌───────────────────┐                 ↓                       │
│  │ InitialSelectionUI│          ┌──────────────┐               │
│  │ - 脑机连接者      │          │  开始游戏    │               │
│  │ - 纯虚拟人        │          └──────────────┘               │
│  └────────┬──────────┘                                          │
│           │                                                     │
│      玩家选择身份                                                 │
│           ↓                                                     │
│  ┌────────────────────┐                                         │
│  │ OnSelectionComplete│                                         │
│  │ ↓                  │                                         │
│  │ 1. 保存到 UserData │                                         │
│  │ 2. 保存到PlayerPrefs│                                         │
│  │ 3. 设置 hasCreated=1│                                         │
│  │ 4. ResourceManager │                                         │
│  │    .SetIdentity()  │                                         │
│  └────────┬───────────┘                                         │
│           │                                                     │
│           ↓                                                     │
│  SceneManager.LoadScene("GameScene")                            │
│           │                                                     │
│           ↓                                                     │
│  ┌───────────────┐                                              │
│  │  GameScene    │                                              │
│  └───────┬───────┘                                              │
│          │                                                      │
│          ↓                                                      │
│  ┌────────────────┐                                             │
│  │ OnSceneLoaded()│                                             │
│  │ (自动触发)     │                                             │
│  └────────┬───────┘                                             │
│           │                                                     │
│           ↓                                                     │
│  ┌──────────────┐                                               │
│  │  开始游戏    │                                               │
│  └──────────────┘                                               │
└─────────────────────────────────────────────────────────────────┘
```

## 数据流图

```
┌─────────────────────────────────────────────────────────────┐
│                         数据流动                             │
└─────────────────────────────────────────────────────────────┘

登录阶段：
┌─────────────┐
│ 用户输入    │
│ (账号/密码) │
└──────┬──────┘
       │
       ↓
┌─────────────────────┐
│ AuthenticationManager│
│ - 创建 UserData     │
│ - LoadUserGameData()│
└──────┬──────────────┘
       │
       ↓ (读取)
┌─────────────────┐
│  PlayerPrefs    │
│ SavedUserId     │
│ HasCreatedChar  │
│ IdentityType    │
│ Level           │
│ VirtualCoin     │
│ MoodValue       │
└──────┬──────────┘
       │
       ↓ (填充)
┌─────────────────┐
│   UserData      │
│ userId          │
│ username        │
│ hasCreatedChar ←─── 关键字段！
│ identityType    │
│ level           │
└──────┬──────────┘
       │
       ↓
┌─────────────────┐
│ LoginUIManager  │
│ OnLoginSuccess()│
└──────┬──────────┘
       │
       ↓ (判断)
  hasCreatedChar?
       │
   ┌───┴───┐
   NO     YES
   │       │
   ↓       ↓
Select  GameScene
Scene


选择阶段（仅首次）：
┌─────────────────┐
│ InitialSelection│
│ Manager         │
│ - 用户选择身份  │
└──────┬──────────┘
       │
       ↓ (保存)
┌─────────────────┐     ┌─────────────────┐
│  PlayerPrefs    │ ←── │   UserData      │
│ HasCreatedChar=1│     │ hasCreatedChar=1│
│ IdentityType=X  │     │ identityType=X  │
└─────────────────┘     └─────────────────┘
       │
       ↓
┌─────────────────┐
│ ResourceManager │
│ SetIdentity(X)  │
└─────────────────┘


进入游戏阶段：
┌─────────────────┐
│  GameScene      │
│  加载完成       │
└──────┬──────────┘
       │
       ↓
┌─────────────────┐
│ GameManager     │
│ OnSceneLoaded() │
└──────┬──────────┘
       │
       ↓ (检查)
   用户已登录？
   hasCreatedChar?
       │
       ↓ YES
┌─────────────────┐
│ LoadUserData()  │
└──────┬──────────┘
       │
       ↓ (读取)
┌─────────────────┐
│   UserData      │
│ identityType    │
└──────┬──────────┘
       │
       ↓ (设置)
┌─────────────────┐
│ ResourceManager │
│ SetIdentity()   │
│ ↓               │
│ PlayerResources │
│ - memory        │
│ - cpu           │
│ - bandwidth     │
│ - computing     │
│ - storage       │
└─────────────────┘
```

## 关键决策点

```
┌────────────────────────────────────────────────────┐
│              关键决策点分析                         │
└────────────────────────────────────────────────────┘

决策点 1: 登录后场景跳转
┌──────────────────────────┐
│ hasCreatedCharacter?     │
└────────┬─────────────────┘
         │
    ┌────┴────┐
    │         │
   false     true
    │         │
    │         └──→ 直接进入 GameScene
    │             - 用户熟悉游戏
    │             - 已有资源配置
    │             - 无需重新选择
    │
    └──→ 进入 SelectScene
        - 首次体验
        - 需要选择身份
        - 初始化资源配置


决策点 2: 数据加载来源
┌──────────────────────────┐
│ SavedUserId 匹配?        │
└────────┬─────────────────┘
         │
    ┌────┴────┐
    │         │
   匹配      不匹配
    │         │
    │         └──→ 使用默认值
    │             - 新用户
    │             - hasCreatedChar = false
    │             - identityType = 0
    │
    └──→ 加载保存的数据
        - 返回用户
        - 从 PlayerPrefs 恢复
        - 保持游戏进度


决策点 3: ResourceManager 初始化时机
┌──────────────────────────────┐
│ 何时初始化 ResourceManager?  │
└────────┬─────────────────────┘
         │
    ┌────┴────┐
    │         │
首次用户    返回用户
    │         │
    │         └──→ GameScene.OnSceneLoaded()
    │             - 场景加载时自动触发
    │             - 从 UserData 读取身份
    │             - 恢复资源配置
    │
    └──→ SelectScene 选择完成后
        - InitialSelectionManager
        - OnSelectionCompleted()
        - 设置新的资源配置
```

## 状态转换图

```
┌────────────────────────────────────────────────────┐
│              游戏状态转换                           │
└────────────────────────────────────────────────────┘

[未登录] ──登录成功──→ [已登录-待判断]
                           │
              ┌────────────┴────────────┐
              │                         │
        首次用户                    返回用户
              │                         │
              ↓                         ↓
    [在 SelectScene]              [在 GameScene]
    [等待选择]                    [游戏进行中]
              │                         │
         选择完成                        │
              │                         │
              ↓                         │
    [跳转到 GameScene] ────────────────→┘
              │
              ↓
        [游戏进行中]


用户数据状态：
[新建用户]
  hasCreatedCharacter = false
  identityType = 0
         │
         ↓ (完成选择)
[完成初始化]
  hasCreatedCharacter = true
  identityType = 0 或 1
         │
         ↓ (保存)
[数据持久化]
  PlayerPrefs 已保存
         │
         ↓ (下次登录)
[数据恢复]
  从 PlayerPrefs 加载
```

## 时序图

```
┌────────┐ ┌──────────┐ ┌──────────┐ ┌────────┐ ┌──────────┐
│ 用户   │ │LoginUI   │ │AuthMgr   │ │UserData│ │PlayerPrefs│
└───┬────┘ └────┬─────┘ └────┬─────┘ └───┬────┘ └────┬─────┘
    │           │            │           │           │
    │ 点击登录   │            │           │           │
    ├──────────→│            │           │           │
    │           │ SignIn()   │           │           │
    │           ├───────────→│           │           │
    │           │            │ LoadUserGameData()    │
    │           │            ├──────────────────────→│
    │           │            │           │ 读取数据  │
    │           │            │←──────────────────────┤
    │           │            │ 创建/更新  │           │
    │           │            ├──────────→│           │
    │           │ OnLoginSuccess(userData)│           │
    │           │←───────────┤           │           │
    │           │            │           │           │
    │           │ 检查 hasCreatedChar     │           │
    │           ├────────────────────────→│           │
    │           │            │           │           │
    │           │ 决定跳转场景│           │           │
    │           │            │           │           │
    │  跳转     │            │           │           │
    │←──────────┤            │           │           │
    │           │            │           │           │

首次用户在 SelectScene：
┌────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│ 用户   │ │Selection │ │UserData  │ │PlayerPrefs│
└───┬────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘
    │           │            │           │
    │ 选择身份  │            │           │
    ├──────────→│            │           │
    │           │ 确认       │           │
    │           │            │           │
    │           │ 更新UserData│          │
    │           ├───────────→│           │
    │           │            │           │
    │           │ 保存到PlayerPrefs      │
    │           ├───────────────────────→│
    │           │            │           │
    │           │ SetPlayerIdentity()    │
    │           │(ResourceManager)       │
    │           │            │           │
    │           │ LoadScene("GameScene") │
    │           │            │           │
    │  跳转     │            │           │
    │←──────────┤            │           │
```

## 类关系图

```
┌─────────────────────────────────────────────────────┐
│                   核心类关系                         │
└─────────────────────────────────────────────────────┘

                    ┌──────────────┐
                    │ GameManager  │
                    │ (DontDestroy)│
                    └───────┬──────┘
                            │
                            │ 监听场景加载
                            │
        ┌───────────────────┼───────────────────┐
        │                   │                   │
        ↓                   ↓                   ↓
┌───────────────┐   ┌───────────────┐   ┌──────────────┐
│Authentication │   │Initial        │   │Resource      │
│Manager        │   │Selection      │   │Manager       │
│(DontDestroy)  │   │Manager        │   │(DontDestroy) │
└───────┬───────┘   │(DontDestroy)  │   └──────┬───────┘
        │           └───────┬───────┘          │
        │                   │                  │
        │                   │                  │
        ↓                   ↓                  ↓
┌───────────────┐   ┌───────────────┐   ┌──────────────┐
│   UserData    │   │Initial        │   │Player        │
│               │   │SelectionUI    │   │Resources     │
│- userId       │   │               │   │              │
│- username     │   │- 选择界面    │   │- memory      │
│- hasCreated ←─┼───┼→ 监听选择   │   │- cpu         │
│- identityType←┼───┼──────────────┼──→│- bandwidth   │
└───────┬───────┘   └───────────────┘   └──────────────┘
        │
        │ 持久化
        ↓
┌───────────────┐
│ PlayerPrefs   │
│               │
│- SavedUserId  │
│- HasCreated   │
│- IdentityType │
└───────────────┘

UI层：
┌───────────────┐   ┌───────────────┐
│ LoginUI       │   │ GameScene UI  │
│ Manager       │   │               │
│               │   │               │
│- 监听登录     │   │- 显示资源    │
│- 调用AuthMgr  │   │- 游戏界面    │
└───────────────┘   └───────────────┘
```

## 文件依赖关系

```
LoginUIManager.cs
    │
    ├─→ AuthenticationManager.cs
    │       │
    │       ├─→ UserData.cs
    │       ├─→ PlayerPrefs (Unity API)
    │       └─→ FirebaseConfig.cs
    │
    └─→ SceneManager (Unity API)


InitialSelectionManager.cs
    │
    ├─→ InitialSelectionUI.cs
    ├─→ AuthenticationManager.cs
    │       └─→ UserData.cs
    ├─→ ResourceManager.cs
    │       └─→ PlayerResources.cs
    └─→ SceneManager (Unity API)


GameManager.cs
    │
    ├─→ AuthenticationManager.cs
    ├─→ ResourceManager.cs
    ├─→ InitialSelectionManager.cs
    ├─→ SceneManager (Unity API)
    └─→ PlayerPrefs (Unity API)
```

---

## 注意事项

1. **DontDestroyOnLoad**: 所有核心管理器都使用此设置，确保在场景切换时不被销毁
2. **单例模式**: 所有管理器都是单例，通过 Instance 属性访问
3. **事件驱动**: 使用 C# 事件（event）进行组件间通信
4. **场景名称**: 必须与代码中的字符串完全匹配（区分大小写）
5. **数据持久化**: 当前使用 PlayerPrefs，生产环境应使用 Firebase Firestore

---

**文档版本**: v1.0
**创建日期**: 2026-01-04
