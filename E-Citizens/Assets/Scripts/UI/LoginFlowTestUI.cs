using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 登录流程测试UI
/// 用于测试和验证登录流程是否正常工作
/// 
/// 使用说明：
/// 1. 在LoginScene中创建一个空GameObject，命名为"LoginFlowTest"
/// 2. 将此脚本挂载到该对象上
/// 3. 运行游戏后，按下相应的键盘按键进行测试
/// </summary>
public class LoginFlowTestUI : MonoBehaviour
{
    #region 测试配置
    [Header("测试配置")]
    [Tooltip("是否在启动时显示测试说明")]
    public bool showInstructionsOnStart = true;
    
    [Tooltip("测试账号前缀")]
    public string testAccountPrefix = "testuser";
    
    [Tooltip("测试密码 - 仅用于开发测试，不应在生产环境中使用")]
    [SerializeField]
    private string testPassword = "test123456";
    #endregion
    
    #region Unity生命周期
    private void Start()
    {
        if (showInstructionsOnStart)
        {
            ShowInstructions();
        }
    }
    
    private void Update()
    {
        // 测试1: 新用户登录（应该跳转到SelectScene）
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestNewUserLogin();
        }
        
        // 测试2: 模拟老用户登录（应该直接跳转到GameScene）
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestReturningUserLogin();
        }
        
        // 测试3: 清除所有PlayerPrefs数据
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ClearAllData();
        }
        
        // 测试4: 显示当前保存的用户数据
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ShowSavedUserData();
        }
        
        // 测试5: 显示帮助信息
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowInstructions();
        }
    }
    #endregion
    
    #region 测试方法
    /// <summary>
    /// 测试新用户登录
    /// </summary>
    private void TestNewUserLogin()
    {
        Debug.Log("\n<color=cyan>==========================================</color>");
        Debug.Log("<color=cyan>测试1: 新用户登录流程</color>");
        Debug.Log("<color=cyan>==========================================</color>");
        
        // 清除之前的数据，确保是新用户
        PlayerPrefs.DeleteKey("SavedUserId");
        PlayerPrefs.DeleteKey("HasCreatedCharacter");
        PlayerPrefs.DeleteKey("IdentityType");
        PlayerPrefs.Save();
        
        // 生成随机测试账号
        string randomId = Random.Range(1000, 9999).ToString();
        string username = testAccountPrefix + randomId;
        
        Debug.Log($"<color=yellow>步骤1: 清除旧数据 ✓</color>");
        Debug.Log($"<color=yellow>步骤2: 生成测试账号: {username}</color>");
        Debug.Log($"<color=yellow>步骤3: 调用登录方法</color>");
        Debug.Log($"<color=yellow>预期结果: 登录成功后跳转到 SelectScene</color>");
        Debug.Log("<color=cyan>------------------------------------------</color>\n");
        
        // 调用登录
        if (AuthenticationManager.Instance != null)
        {
            AuthenticationManager.Instance.SignInWithTestAccount(username, testPassword);
        }
        else
        {
            Debug.LogError("AuthenticationManager 未找到!");
        }
    }
    
    /// <summary>
    /// 测试老用户登录
    /// </summary>
    private void TestReturningUserLogin()
    {
        Debug.Log("\n<color=cyan>==========================================</color>");
        Debug.Log("<color=cyan>测试2: 老用户登录流程</color>");
        Debug.Log("<color=cyan>==========================================</color>");
        
        // 创建一个已完成角色创建的测试账号
        string username = testAccountPrefix + "999";
        string userId = "test_" + username;
        
        // 模拟已完成角色创建的用户数据
        PlayerPrefs.SetString("SavedUserId", userId);
        PlayerPrefs.SetInt("HasCreatedCharacter", 1);
        PlayerPrefs.SetInt("IdentityType", 0); // 脑机连接者
        PlayerPrefs.SetInt("Level", 5);
        PlayerPrefs.SetInt("VirtualCoin", 500);
        PlayerPrefs.Save();
        
        Debug.Log($"<color=yellow>步骤1: 创建老用户数据 ✓</color>");
        Debug.Log($"<color=yellow>  - userId: {userId}</color>");
        Debug.Log($"<color=yellow>  - hasCreatedCharacter: true</color>");
        Debug.Log($"<color=yellow>  - identityType: 0 (脑机连接者)</color>");
        Debug.Log($"<color=yellow>步骤2: 调用登录方法</color>");
        Debug.Log($"<color=yellow>预期结果: 登录成功后直接跳转到 GameScene</color>");
        Debug.Log("<color=cyan>------------------------------------------</color>\n");
        
        // 调用登录
        if (AuthenticationManager.Instance != null)
        {
            AuthenticationManager.Instance.SignInWithTestAccount(username, testPassword);
        }
        else
        {
            Debug.LogError("AuthenticationManager 未找到!");
        }
    }
    
    /// <summary>
    /// 清除所有数据
    /// </summary>
    private void ClearAllData()
    {
        Debug.Log("\n<color=red>==========================================</color>");
        Debug.Log("<color=red>测试3: 清除所有本地数据</color>");
        Debug.Log("<color=red>==========================================</color>");
        
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        Debug.Log("<color=yellow>✓ 已清除所有 PlayerPrefs 数据</color>");
        Debug.Log("<color=yellow>✓ 下次登录将被视为新用户</color>");
        Debug.Log("<color=red>------------------------------------------</color>\n");
    }
    
    /// <summary>
    /// 显示保存的用户数据
    /// </summary>
    private void ShowSavedUserData()
    {
        Debug.Log("\n<color=green>==========================================</color>");
        Debug.Log("<color=green>测试4: 显示当前保存的用户数据</color>");
        Debug.Log("<color=green>==========================================</color>");
        
        string savedUserId = PlayerPrefs.GetString("SavedUserId", "未设置");
        string savedUsername = PlayerPrefs.GetString("SavedUsername", "未设置");
        bool hasCreatedCharacter = PlayerPrefs.GetInt("HasCreatedCharacter", 0) == 1;
        int identityType = PlayerPrefs.GetInt("IdentityType", -1);
        int level = PlayerPrefs.GetInt("Level", -1);
        int virtualCoin = PlayerPrefs.GetInt("VirtualCoin", -1);
        
        Debug.Log($"<color=yellow>SavedUserId: {savedUserId}</color>");
        Debug.Log($"<color=yellow>SavedUsername: {savedUsername}</color>");
        Debug.Log($"<color=yellow>HasCreatedCharacter: {hasCreatedCharacter}</color>");
        Debug.Log($"<color=yellow>IdentityType: {identityType} ({GetIdentityTypeName(identityType)})</color>");
        Debug.Log($"<color=yellow>Level: {level}</color>");
        Debug.Log($"<color=yellow>VirtualCoin: {virtualCoin}</color>");
        Debug.Log("<color=green>------------------------------------------</color>\n");
    }
    
    /// <summary>
    /// 显示测试说明
    /// </summary>
    private void ShowInstructions()
    {
        Debug.Log("\n<color=magenta>==========================================</color>");
        Debug.Log("<color=magenta>登录流程测试 - 使用说明</color>");
        Debug.Log("<color=magenta>==========================================</color>");
        Debug.Log("<color=white>按键1: 测试新用户登录（应跳转到SelectScene）</color>");
        Debug.Log("<color=white>按键2: 测试老用户登录（应跳转到GameScene）</color>");
        Debug.Log("<color=white>按键3: 清除所有本地数据</color>");
        Debug.Log("<color=white>按键4: 显示当前保存的用户数据</color>");
        Debug.Log("<color=white>按键H: 显示此帮助信息</color>");
        Debug.Log("<color=magenta>==========================================</color>\n");
    }
    #endregion
    
    #region 辅助方法
    /// <summary>
    /// 获取身份类型名称
    /// </summary>
    private string GetIdentityTypeName(int identityType)
    {
        switch (identityType)
        {
            case 0:
                return "脑机连接者";
            case 1:
                return "纯虚拟人";
            case -1:
                return "未设置";
            default:
                return "未知";
        }
    }
    #endregion
}
