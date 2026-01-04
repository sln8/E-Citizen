# 登录流程快速参考

## 流程概览

```
LoginScene → 登录 → 检查 hasCreatedCharacter
                              ↓
                    ┌─────────┴─────────┐
                    │                   │
               false (首次)         true (返回)
                    │                   │
                    ↓                   ↓
              SelectScene          GameScene
               (初始选择)           (加载属性)
                    │
                    ↓
               选择身份
                    │
                    ↓
              GameScene
              (初始化资源)
```

## 关键文件修改

### 1. LoginUIManager.cs
```csharp
private void OnLoginSuccess(UserData userData)
{
    if (!userData.hasCreatedCharacter)
        SceneManager.LoadScene("SelectScene");  // 首次登录
    else
        SceneManager.LoadScene("GameScene");    // 返回用户
}
```

### 2. AuthenticationManager.cs
```csharp
// 新增方法：加载用户游戏数据
private void LoadUserGameData(UserData userData)
{
    string savedUserId = PlayerPrefs.GetString("SavedUserId", "");
    if (savedUserId == userData.userId)
    {
        userData.hasCreatedCharacter = PlayerPrefs.GetInt("HasCreatedCharacter", 0) == 1;
        userData.identityType = PlayerPrefs.GetInt("IdentityType", 0);
        // ...
    }
}
```

### 3. InitialSelectionManager.cs
```csharp
private void OnSelectionCompleted(IdentityType selectedIdentity)
{
    // 保存选择
    PlayerPrefs.SetInt("HasCreatedCharacter", 1);
    PlayerPrefs.SetInt("IdentityType", (int)selectedIdentity);
    PlayerPrefs.Save();
    
    // 初始化资源管理器
    ResourceManager.Instance.SetPlayerIdentity(selectedIdentity);
    
    // 跳转到游戏场景
    SceneManager.LoadScene("GameScene");
}
```

### 4. GameManager.cs
```csharp
// 新增：监听场景加载
private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    if (scene.name == "GameScene" && 用户已登录 && hasCreatedCharacter)
    {
        LoadUserData(currentUser);  // 加载用户属性
    }
}

// 完善：加载用户数据
private void LoadUserData(UserData userData)
{
    IdentityType userIdentity = (IdentityType)userData.identityType;
    ResourceManager.Instance.SetPlayerIdentity(userIdentity);
}
```

## 数据存储

### PlayerPrefs 键值对
```
SavedUserId          → 用户ID
HasCreatedCharacter  → 0/1 (是否完成初始选择)
IdentityType         → 0/1 (0=脑机连接者, 1=纯虚拟人)
Level                → 等级
VirtualCoin          → 虚拟币
MoodValue            → 心情值
```

## 测试步骤

### 测试首次登录
```csharp
// 1. 清除数据
PlayerPrefs.DeleteAll();

// 2. 运行游戏，登录
// 3. 验证：自动跳转到 SelectScene
// 4. 选择身份，确认
// 5. 验证：自动跳转到 GameScene
```

### 测试返回用户
```csharp
// 1. 完成首次登录流程
// 2. 重启游戏，再次登录
// 3. 验证：直接跳转到 GameScene（跳过 SelectScene）
```

## 调试命令

```csharp
// 检查用户数据
Debug.Log($"UserId: {PlayerPrefs.GetString("SavedUserId")}");
Debug.Log($"HasCreated: {PlayerPrefs.GetInt("HasCreatedCharacter")}");
Debug.Log($"Identity: {PlayerPrefs.GetInt("IdentityType")}");

// 重置用户选择
PlayerPrefs.SetInt("HasCreatedCharacter", 0);
PlayerPrefs.Save();
```

## 注意事项

1. ✅ 确保三个场景名称正确：LoginScene、SelectScene、GameScene
2. ✅ 确保所有场景都添加到 Build Settings
3. ✅ 管理器都使用 DontDestroyOnLoad，在场景间保持存在
4. ✅ 每次修改后检查控制台日志，确保流程正确

## 常见问题

**Q: 登录后没有跳转？**
- 检查场景名称拼写
- 查看控制台错误信息

**Q: 返回用户仍进入 SelectScene？**
- 检查 PlayerPrefs HasCreatedCharacter 值
- 验证 userId 是否匹配

**Q: ResourceManager 未初始化？**
- 确保 ResourceManager 在场景中存在
- 检查 SetPlayerIdentity 是否被调用

---

详细文档请参考：[LOGIN_FLOW_IMPLEMENTATION.md](./LOGIN_FLOW_IMPLEMENTATION.md)
