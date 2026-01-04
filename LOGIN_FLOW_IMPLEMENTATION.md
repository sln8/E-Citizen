# 登录流程实现文档

## 概述

本文档详细说明了电子公民（E-Citizen）游戏的完整登录和场景跳转流程实现。该流程支持：
1. 首次登录用户进入初始选择场景
2. 已完成初始选择的用户直接进入游戏场景
3. 用户属性的保存和加载

---

## 完整流程图

```
┌─────────────┐
│ LoginScene  │
│   (登录场景)  │
└──────┬──────┘
       │
       ↓
   用户登录
       │
       ↓
检查 hasCreatedCharacter
       │
   ┌───┴───┐
   │       │
  否      是
   │       │
   │       ↓
   │   ┌──────────────┐
   │   │  GameScene   │
   │   │  (游戏场景)    │
   │   │              │
   │   │ 加载用户属性   │
   │   └──────────────┘
   │
   ↓
┌──────────────┐
│ SelectScene  │
│ (初始选择场景) │
└──────┬───────┘
       │
       ↓
   选择身份类型
   (脑机连接者 / 纯虚拟人)
       │
       ↓
   保存选择结果
   设置 hasCreatedCharacter = true
       │
       ↓
┌──────────────┐
│  GameScene   │
│  (游戏场景)    │
│              │
│ 初始化资源配置 │
└──────────────┘
```

---

## 核心实现

### 1. LoginUIManager.cs 修改

**位置**: `Assets/Scripts/UI/LoginUIManager.cs`

**关键修改**: `OnLoginSuccess` 方法

```csharp
private void OnLoginSuccess(UserData userData)
{
    Debug.Log($"<color=green>登录成功！欢迎 {userData.username}</color>");
    
    ShowLoading(false);
    UpdateStatus($"登录成功！欢迎 {userData.username}");
    
    // 检查用户是否已完成初始选择
    if (!userData.hasCreatedCharacter)
    {
        // 首次登录，跳转到初始选择场景
        Debug.Log("检测到首次登录，跳转到初始选择场景");
        UnityEngine.SceneManagement.SceneManager.LoadScene("SelectScene");
    }
    else
    {
        // 已完成初始选择，直接跳转到游戏场景
        Debug.Log("欢迎回来！跳转到游戏场景");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
```

**功能说明**:
- 登录成功后检查 `userData.hasCreatedCharacter` 标志
- 首次用户（hasCreatedCharacter = false）→ SelectScene
- 返回用户（hasCreatedCharacter = true）→ GameScene

---

### 2. AuthenticationManager.cs 增强

**位置**: `Assets/Scripts/Authentication/AuthenticationManager.cs`

#### 2.1 新增方法：LoadUserGameData

```csharp
/// <summary>
/// 加载用户游戏数据
/// 从本地PlayerPrefs加载用户的游戏进度数据
/// </summary>
private void LoadUserGameData(UserData userData)
{
    // 尝试从PlayerPrefs加载用户数据
    string savedUserId = PlayerPrefs.GetString("SavedUserId", "");
    
    // 如果保存的用户ID与当前用户ID匹配，加载游戏数据
    if (savedUserId == userData.userId)
    {
        // 加载角色创建状态
        userData.hasCreatedCharacter = PlayerPrefs.GetInt("HasCreatedCharacter", 0) == 1;
        userData.identityType = PlayerPrefs.GetInt("IdentityType", 0);
        userData.level = PlayerPrefs.GetInt("Level", 1);
        userData.virtualCoin = PlayerPrefs.GetInt("VirtualCoin", 100);
        userData.moodValue = PlayerPrefs.GetInt("MoodValue", 10);
        
        Debug.Log($"成功加载用户数据: hasCreatedCharacter={userData.hasCreatedCharacter}, identityType={userData.identityType}");
    }
    else
    {
        // 新用户或不同的用户ID，使用默认值
        Debug.Log("未找到保存的用户数据，使用默认值");
        userData.hasCreatedCharacter = false;
        userData.identityType = 0;
    }
}
```

#### 2.2 修改模拟登录方法

所有模拟登录方法（SimulateGoogleLogin、SimulateFacebookLogin、SimulateAppleLogin、SimulateTestAccountLogin）都在创建用户数据后调用 `LoadUserGameData`：

```csharp
// 创建模拟用户数据
UserData simulatedUser = new UserData { ... };

// 加载用户的游戏数据
LoadUserGameData(simulatedUser);

CompleteLogin(true, "登录成功（模拟）", simulatedUser);
```

**功能说明**:
- 从 PlayerPrefs 加载用户的游戏进度
- 关键字段：hasCreatedCharacter、identityType、level 等
- 支持多用户（通过 userId 匹配）

---

### 3. InitialSelectionManager.cs 完善

**位置**: `Assets/Scripts/Managers/InitialSelectionManager.cs`

**关键修改**: `OnSelectionCompleted` 方法

```csharp
private void OnSelectionCompleted(IdentityType selectedIdentity)
{
    Debug.Log($"<color=green>玩家完成初始选择：{selectedIdentity}</color>");
    
    // 保存用户选择到用户数据
    if (AuthenticationManager.Instance != null && AuthenticationManager.Instance.currentUser != null)
    {
        UserData currentUser = AuthenticationManager.Instance.currentUser;
        currentUser.hasCreatedCharacter = true;
        currentUser.identityType = (int)selectedIdentity;
        
        // 保存到本地PlayerPrefs
        PlayerPrefs.SetInt("HasCreatedCharacter", 1);
        PlayerPrefs.SetInt("IdentityType", (int)selectedIdentity);
        PlayerPrefs.SetString("SavedUserId", currentUser.userId);
        PlayerPrefs.Save();
        
        Debug.Log("✓ 用户初始选择已保存");
    }
    
    // 初始化资源管理器的玩家身份
    if (ResourceManager.Instance != null)
    {
        ResourceManager.Instance.SetPlayerIdentity(selectedIdentity);
        Debug.Log($"✓ 资源管理器已设置玩家身份: {selectedIdentity}");
    }
    
    // 跳转到游戏场景
    Debug.Log("跳转到游戏场景...");
    UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
}
```

**功能说明**:
- 保存用户选择到 UserData 和 PlayerPrefs
- 设置 hasCreatedCharacter = true
- 初始化 ResourceManager 的玩家身份
- 自动跳转到 GameScene

---

### 4. GameManager.cs 增强

**位置**: `Assets/Scripts/Managers/GameManager.cs`

#### 4.1 新增场景加载事件监听

```csharp
private void Awake()
{
    // ... 单例模式检查 ...
    
    // 监听场景加载事件
    SceneManager.sceneLoaded += OnSceneLoaded;
}

private void OnDestroy()
{
    // 取消场景加载事件监听
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

/// <summary>
/// 场景加载完成回调
/// </summary>
private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    Debug.Log($"场景加载完成: {scene.name}");
    
    // 如果加载的是游戏场景，且用户已登录
    if (scene.name == "GameScene" && 
        AuthenticationManager.Instance != null && 
        AuthenticationManager.Instance.IsLoggedIn())
    {
        UserData currentUser = AuthenticationManager.Instance.GetCurrentUser();
        if (currentUser != null && currentUser.hasCreatedCharacter)
        {
            // 加载用户数据和属性
            Debug.Log("进入游戏场景，加载用户属性...");
            LoadUserData(currentUser);
        }
    }
}
```

#### 4.2 完善 LoadUserData 方法

```csharp
/// <summary>
/// 加载用户数据
/// </summary>
private void LoadUserData(UserData userData)
{
    Debug.Log($"加载用户数据: {userData.userId}");
    
    // 加载用户的身份类型和资源配置
    IdentityType userIdentity = (IdentityType)userData.identityType;
    Debug.Log($"用户身份类型: {userIdentity}");
    
    // 设置资源管理器的玩家身份
    if (ResourceManager.Instance != null)
    {
        ResourceManager.Instance.SetPlayerIdentity(userIdentity);
        Debug.Log("✓ 资源管理器已加载用户身份配置");
    }
    else
    {
        Debug.LogWarning("ResourceManager未找到，资源配置将在稍后加载");
    }
    
    // TODO: 从Firebase Firestore加载用户的完整游戏数据
    Debug.Log("用户数据加载完成");
}
```

**功能说明**:
- 监听场景加载事件
- GameScene 加载时自动加载用户属性
- 初始化 ResourceManager 的玩家身份配置

---

## 数据流

### PlayerPrefs 存储结构

| 键名 | 类型 | 说明 |
|-----|------|------|
| SavedUserId | String | 用户唯一ID |
| HasCreatedCharacter | Int (0/1) | 是否完成初始选择 |
| IdentityType | Int (0/1) | 身份类型（0=脑机连接者，1=纯虚拟人） |
| Level | Int | 玩家等级 |
| VirtualCoin | Int | 虚拟币数量 |
| MoodValue | Int | 心情值 |

### UserData 关键字段

```csharp
public class UserData
{
    public string userId;                    // 用户ID
    public string username;                  // 用户名
    public bool hasCreatedCharacter = false; // 是否完成初始选择
    public int identityType = 0;             // 身份类型
    public int level = 1;                    // 等级
    public int virtualCoin = 100;            // 虚拟币
    public int moodValue = 10;               // 心情值
}
```

---

## 使用场景

### 场景 1：首次登录用户

1. 用户在 LoginScene 输入账号密码登录
2. AuthenticationManager 创建新用户，hasCreatedCharacter = false
3. LoginUIManager 检测到 hasCreatedCharacter = false
4. 跳转到 SelectScene
5. 用户选择身份类型（脑机连接者 / 纯虚拟人）
6. InitialSelectionManager 保存选择，设置 hasCreatedCharacter = true
7. 自动跳转到 GameScene
8. GameManager 加载用户属性，初始化 ResourceManager

### 场景 2：返回用户

1. 用户在 LoginScene 输入账号密码登录
2. AuthenticationManager 从 PlayerPrefs 加载用户数据
3. hasCreatedCharacter = true
4. LoginUIManager 检测到用户已完成初始选择
5. 直接跳转到 GameScene
6. GameManager 加载用户属性，恢复游戏状态

### 场景 3：切换账号

1. 用户 A 登录，完成初始选择
2. 用户 A 登出
3. 用户 B 登录（首次）
4. 因为 userId 不同，LoadUserGameData 返回默认值
5. hasCreatedCharacter = false
6. 跳转到 SelectScene，重新进行初始选择

---

## 测试步骤

### 测试 1：首次登录流程

1. 清除 PlayerPrefs（开发环境）：
   ```csharp
   PlayerPrefs.DeleteAll();
   PlayerPrefs.Save();
   ```

2. 运行游戏，进入 LoginScene

3. 点击任意登录方式（建议使用"快速创建测试账号"）

4. 验证：
   - ✓ 自动跳转到 SelectScene
   - ✓ 显示两个身份选项的详细信息

5. 选择一个身份类型，点击"确认选择"

6. 验证：
   - ✓ 自动跳转到 GameScene
   - ✓ 控制台显示"用户初始选择已保存"
   - ✓ 控制台显示"资源管理器已设置玩家身份"

### 测试 2：返回用户流程

1. 完成测试 1 后，退出游戏

2. 重新运行游戏，进入 LoginScene

3. 使用相同的账号登录

4. 验证：
   - ✓ 直接跳转到 GameScene（跳过 SelectScene）
   - ✓ 控制台显示"成功加载用户数据: hasCreatedCharacter=True"
   - ✓ 控制台显示"进入游戏场景，加载用户属性..."
   - ✓ 资源管理器正确加载之前选择的身份类型

### 测试 3：多用户切换

1. 使用账号 A 登录，完成初始选择

2. 登出（如果有登出功能）

3. 使用账号 B 登录（不同的测试账号）

4. 验证：
   - ✓ 账号 B 视为新用户
   - ✓ 跳转到 SelectScene 进行初始选择
   - ✓ 账号 A 的数据不影响账号 B

---

## 调试建议

### 启用详细日志

所有关键步骤都有详细的 Debug.Log 输出：

```
登录成功！欢迎 xxx
检测到首次登录，跳转到初始选择场景
场景加载完成: SelectScene
玩家完成初始选择：ConsciousnessLinker
✓ 用户初始选择已保存
✓ 资源管理器已设置玩家身份: ConsciousnessLinker
跳转到游戏场景...
场景加载完成: GameScene
进入游戏场景，加载用户属性...
用户身份类型: ConsciousnessLinker
✓ 资源管理器已加载用户身份配置
用户数据加载完成
```

### 检查 PlayerPrefs

在编辑器中查看 PlayerPrefs：
- Windows: `HKEY_CURRENT_USER\Software\Unity\UnityEditor\[CompanyName]\[ProductName]`
- Mac: `~/Library/Preferences/com.[CompanyName].[ProductName].plist`

或使用代码：

```csharp
Debug.Log($"SavedUserId: {PlayerPrefs.GetString("SavedUserId")}");
Debug.Log($"HasCreatedCharacter: {PlayerPrefs.GetInt("HasCreatedCharacter")}");
Debug.Log($"IdentityType: {PlayerPrefs.GetInt("IdentityType")}");
```

---

## 注意事项

### 1. 场景名称必须正确

确保以下场景名称与代码中的字符串完全一致：
- LoginScene
- SelectScene
- GameScene

### 2. Build Settings 配置

确保所有场景都添加到 Build Settings 中：
1. File → Build Settings
2. 点击 "Add Open Scenes" 或拖拽场景文件
3. 确保场景顺序正确（通常 LoginScene 应该是 Scene 0）

### 3. DontDestroyOnLoad 管理器

以下管理器使用 DontDestroyOnLoad，在场景切换时保持存在：
- GameManager
- AuthenticationManager
- ResourceManager
- InitialSelectionManager

这些管理器会自动在场景间传递用户数据。

### 4. Firebase 集成

当前实现使用 PlayerPrefs 作为临时存储方案。生产环境应该：
- 实现 Firebase Firestore 数据保存
- 实现 Firebase Authentication 真实登录
- 添加数据同步和冲突解决机制

---

## 扩展功能建议

### 1. 添加加载进度条

在场景切换时显示加载进度：

```csharp
IEnumerator LoadSceneAsync(string sceneName)
{
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
    
    while (!asyncLoad.isDone)
    {
        float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
        // 更新进度条 UI
        yield return null;
    }
}
```

### 2. 添加场景过渡动画

使用淡入淡出效果：

```csharp
public void FadeToScene(string sceneName)
{
    StartCoroutine(FadeAndLoadScene(sceneName));
}

IEnumerator FadeAndLoadScene(string sceneName)
{
    // 淡出
    fadeImage.DOFade(1, 0.5f);
    yield return new WaitForSeconds(0.5f);
    
    // 加载场景
    SceneManager.LoadScene(sceneName);
    
    // 淡入
    fadeImage.DOFade(0, 0.5f);
}
```

### 3. 添加重新选择身份功能

允许用户在设置中重新选择身份类型：

```csharp
public void ResetCharacterSelection()
{
    if (AuthenticationManager.Instance.currentUser != null)
    {
        PlayerPrefs.SetInt("HasCreatedCharacter", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SelectScene");
    }
}
```

---

## 常见问题

### Q1: 登录后一直停留在 LoginScene

**可能原因**:
- LoginUIManager 的 OnLoginSuccess 方法没有被调用
- 场景名称拼写错误

**解决方案**:
- 检查 Debug.Log 输出
- 验证场景名称与代码中的字符串一致

### Q2: 返回用户仍然进入 SelectScene

**可能原因**:
- hasCreatedCharacter 没有正确保存
- LoadUserGameData 没有正确加载数据
- userId 不匹配

**解决方案**:
- 检查 PlayerPrefs 中的 HasCreatedCharacter 值
- 验证 SavedUserId 与当前 userId 是否匹配

### Q3: ResourceManager 未正确初始化

**可能原因**:
- ResourceManager 在场景中不存在
- SetPlayerIdentity 方法调用失败

**解决方案**:
- 确保 ResourceManager 预制体在场景中
- 检查 ResourceManager 是否使用 DontDestroyOnLoad

---

## 版本历史

### v1.0 (当前版本)
- ✅ 实现基本的登录流程
- ✅ 支持首次用户和返回用户的不同流程
- ✅ 实现用户数据的保存和加载
- ✅ 集成 ResourceManager 身份配置
- ✅ 添加详细的日志输出

### 未来计划
- 🔲 集成 Firebase Firestore 数据持久化
- 🔲 添加场景过渡动画
- 🔲 实现数据云同步
- 🔲 添加账号迁移功能

---

## 联系与支持

如有问题或建议，请联系开发团队或在项目 Issues 中提出。
