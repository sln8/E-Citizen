using UnityEngine;
using System;

/// <summary>
/// 认证管理器
/// 负责处理所有登录相关的功能，包括：
/// - Google登录
/// - Facebook登录
/// - Apple登录
/// - 测试账号登录
/// </summary>
public class AuthenticationManager : MonoBehaviour
{
    #region 单例模式
    private static AuthenticationManager _instance;
    
    public static AuthenticationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AuthenticationManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("AuthenticationManager");
                    _instance = go.AddComponent<AuthenticationManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// 检查实例是否存在，不会创建新实例
    /// </summary>
    public static bool HasInstance()
    {
        return _instance != null;
    }
    #endregion

    #region 事件定义
    // 登录成功事件
    public event Action<UserData> OnLoginSuccess;
    
    // 登录失败事件
    public event Action<string> OnLoginFailed;
    
    // 登出事件
    public event Action OnLogout;
    #endregion

    #region 状态变量
    [Header("登录状态")]
    [Tooltip("当前是否已登录")]
    public bool isLoggedIn = false;
    
    [Tooltip("当前登录的用户信息")]
    public UserData currentUser = null;
    
    [Tooltip("当前登录状态信息")]
    public string loginStatus = "未登录";
    
    // 私有变量
    private bool _isLoggingIn = false;
    #endregion

    #region Unity生命周期方法
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 监听Firebase初始化完成事件
        if (FirebaseInitializer.HasInstance())
        {
            FirebaseInitializer.Instance.OnFirebaseInitialized += OnFirebaseReady;
        }
    }

    private void OnDestroy()
    {
        // 取消事件监听 - 使用HasInstance避免创建新实例
        if (FirebaseInitializer.HasInstance())
        {
            FirebaseInitializer.Instance.OnFirebaseInitialized -= OnFirebaseReady;
        }
        
        // 清理单例引用
        if (_instance == this)
        {
            _instance = null;
        }
    }
    #endregion

    #region Firebase就绪回调
    /// <summary>
    /// Firebase初始化完成后的回调
    /// </summary>
    private void OnFirebaseReady()
    {
        Debug.Log("Firebase已就绪，认证系统可以使用");
        loginStatus = "认证系统已就绪";
        
        // 检查是否有保存的登录信息（自动登录）
        CheckSavedLogin();
    }
    #endregion

    #region 登录方法
    /// <summary>
    /// Google登录
    /// 使用Google账号进行登录
    /// </summary>
    public void SignInWithGoogle()
    {
        if (!CanStartLogin("Google")) return;
        
        Debug.Log("=== 开始Google登录流程 ===");
        _isLoggingIn = true;
        loginStatus = "正在通过Google登录...";
        
        // 检查是否启用Google登录
        if (!FirebaseConfig.Instance.enableGoogleSignIn)
        {
            CompleteLogin(false, "Google登录未启用", null);
            return;
        }
        
        // 检查是否为模拟模式
        if (FirebaseConfig.Instance.ShouldSimulate())
        {
            SimulateGoogleLogin();
        }
        else
        {
            RealGoogleLogin();
        }
    }
    
    /// <summary>
    /// Facebook登录
    /// 使用Facebook账号进行登录
    /// </summary>
    public void SignInWithFacebook()
    {
        if (!CanStartLogin("Facebook")) return;
        
        Debug.Log("=== 开始Facebook登录流程 ===");
        _isLoggingIn = true;
        loginStatus = "正在通过Facebook登录...";
        
        if (!FirebaseConfig.Instance.enableFacebookSignIn)
        {
            CompleteLogin(false, "Facebook登录未启用", null);
            return;
        }
        
        if (FirebaseConfig.Instance.ShouldSimulate())
        {
            SimulateFacebookLogin();
        }
        else
        {
            RealFacebookLogin();
        }
    }
    
    /// <summary>
    /// Apple登录
    /// 使用Apple账号进行登录（仅iOS平台）
    /// </summary>
    public void SignInWithApple()
    {
        if (!CanStartLogin("Apple")) return;
        
        Debug.Log("=== 开始Apple登录流程 ===");
        _isLoggingIn = true;
        loginStatus = "正在通过Apple登录...";
        
        if (!FirebaseConfig.Instance.enableAppleSignIn)
        {
            CompleteLogin(false, "Apple登录未启用", null);
            return;
        }
        
        // 检查平台
        #if !UNITY_IOS
        Debug.LogWarning("Apple登录仅支持iOS平台");
        CompleteLogin(false, "Apple登录仅支持iOS平台", null);
        return;
        #else
        
        if (FirebaseConfig.Instance.ShouldSimulate())
        {
            SimulateAppleLogin();
        }
        else
        {
            RealAppleLogin();
        }
        #endif
    }
    
    /// <summary>
    /// 测试账号登录
    /// 使用测试账号和密码进行登录
    /// </summary>
    /// <param name="username">测试账号</param>
    /// <param name="password">测试密码</param>
    public void SignInWithTestAccount(string username, string password)
    {
        if (!CanStartLogin("测试账号")) return;
        
        Debug.Log($"[AuthManager] ========== 开始测试账号登录流程 ========== 账号: {username}");
        _isLoggingIn = true;
        loginStatus = "正在使用测试账号登录...";
        
        if (!FirebaseConfig.Instance.enableTestAccounts)
        {
            Debug.LogWarning("[AuthManager] 测试账号功能未启用");
            CompleteLogin(false, "测试账号登录未启用", null);
            return;
        }
        
        // 验证输入
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("[AuthManager] 账号或密码为空");
            CompleteLogin(false, "账号或密码不能为空", null);
            return;
        }
        
        Debug.Log($"[AuthManager] 配置检查: isTestMode={FirebaseConfig.Instance.isTestMode}, ShouldSimulate={FirebaseConfig.Instance.ShouldSimulate()}");
        
        if (FirebaseConfig.Instance.ShouldSimulate() || FirebaseConfig.Instance.isTestMode)
        {
            Debug.Log("[AuthManager] 使用模拟登录模式");
            SimulateTestAccountLogin(username, password);
        }
        else
        {
            Debug.Log("[AuthManager] 使用真实登录模式");
            RealTestAccountLogin(username, password);
        }
    }
    
    /// <summary>
    /// 快速创建测试账号
    /// 自动生成一个测试账号并登录
    /// </summary>
    public void CreateQuickTestAccount()
    {
        Debug.Log("=== 创建快速测试账号 ===");
        
        // 生成随机测试账号
        string randomId = UnityEngine.Random.Range(10000, 99999).ToString();
        string testUsername = $"test_user_{randomId}";
        string testPassword = "test123456";
        
        Debug.Log($"创建测试账号: {testUsername} / {testPassword}");
        
        // 使用生成的账号登录
        SignInWithTestAccount(testUsername, testPassword);
    }
    #endregion

    #region 模拟登录方法（用于测试）
    /// <summary>
    /// 模拟Google登录
    /// 在编辑器中运行时使用
    /// </summary>
    private void SimulateGoogleLogin()
    {
        Debug.Log("模拟Google登录成功");
        
        // 创建模拟用户数据
        UserData simulatedUser = new UserData
        {
            userId = "google_test_" + System.Guid.NewGuid().ToString(),
            username = "Google测试用户",
            email = "test@google.com",
            loginProvider = LoginProvider.Google,
            createdAt = DateTime.Now.ToString(),
            lastLoginAt = DateTime.Now.ToString()
        };
        
        // 加载用户的游戏数据
        LoadUserGameData(simulatedUser);
        
        CompleteLogin(true, "Google登录成功（模拟）", simulatedUser);
    }
    
    /// <summary>
    /// 模拟Facebook登录
    /// </summary>
    private void SimulateFacebookLogin()
    {
        Debug.Log("模拟Facebook登录成功");
        
        UserData simulatedUser = new UserData
        {
            userId = "facebook_test_" + System.Guid.NewGuid().ToString(),
            username = "Facebook测试用户",
            email = "test@facebook.com",
            loginProvider = LoginProvider.Facebook,
            createdAt = DateTime.Now.ToString(),
            lastLoginAt = DateTime.Now.ToString()
        };
        
        // 加载用户的游戏数据
        LoadUserGameData(simulatedUser);
        
        CompleteLogin(true, "Facebook登录成功（模拟）", simulatedUser);
    }
    
    /// <summary>
    /// 模拟Apple登录
    /// </summary>
    private void SimulateAppleLogin()
    {
        Debug.Log("模拟Apple登录成功");
        
        UserData simulatedUser = new UserData
        {
            userId = "apple_test_" + System.Guid.NewGuid().ToString(),
            username = "Apple测试用户",
            email = "test@apple.com",
            loginProvider = LoginProvider.Apple,
            createdAt = DateTime.Now.ToString(),
            lastLoginAt = DateTime.Now.ToString()
        };
        
        // 加载用户的游戏数据
        LoadUserGameData(simulatedUser);
        
        CompleteLogin(true, "Apple登录成功（模拟）", simulatedUser);
    }
    
    /// <summary>
    /// 模拟测试账号登录
    /// </summary>
    private void SimulateTestAccountLogin(string username, string password)
    {
        Debug.Log($"[AuthManager] 模拟测试账号登录: {username}");
        
        // 简单的密码验证（实际项目中应该更安全）
        if (password.Length < 6)
        {
            Debug.LogWarning($"[AuthManager] 密码验证失败，长度: {password.Length}");
            CompleteLogin(false, "密码至少需要6个字符", null);
            return;
        }
        
        UserData simulatedUser = new UserData
        {
            userId = "test_" + username,
            username = username,
            email = $"{username}@test.com",
            loginProvider = LoginProvider.TestAccount,
            createdAt = DateTime.Now.ToString(),
            lastLoginAt = DateTime.Now.ToString()
        };
        
        Debug.Log($"[AuthManager] 创建用户数据 - userId: {simulatedUser.userId}");
        Debug.Log($"[AuthManager] hasCreatedCharacter (初始): {simulatedUser.hasCreatedCharacter}");
        
        // 加载用户的游戏数据
        LoadUserGameData(simulatedUser);
        
        Debug.Log($"[AuthManager] hasCreatedCharacter (加载后): {simulatedUser.hasCreatedCharacter}");
        
        CompleteLogin(true, "测试账号登录成功（模拟）", simulatedUser);
    }
    #endregion

    #region 真实登录方法（需要Firebase SDK）
    /// <summary>
    /// 真实的Google登录实现
    /// 需要Google Play Services和Firebase Authentication SDK
    /// </summary>
    private void RealGoogleLogin()
    {
        /*
         * 真实Google登录代码示例：
         * 
         * GoogleSignIn.Configuration = new GoogleSignInConfiguration
         * {
         *     RequestIdToken = true,
         *     WebClientId = "your-web-client-id"
         * };
         * 
         * GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task => {
         *     if (task.IsCanceled) {
         *         CompleteLogin(false, "用户取消了Google登录", null);
         *     } else if (task.IsFaulted) {
         *         CompleteLogin(false, "Google登录失败", null);
         *     } else {
         *         var googleUser = task.Result;
         *         // 使用Google凭证登录Firebase
         *         SignInWithGoogleOnFirebase(googleUser.IdToken);
         *     }
         * });
         */
        
        Debug.LogWarning("真实Google登录需要Firebase SDK，当前使用模拟模式");
        SimulateGoogleLogin();
    }
    
    /// <summary>
    /// 真实的Facebook登录实现
    /// 需要Facebook SDK和Firebase Authentication SDK
    /// </summary>
    private void RealFacebookLogin()
    {
        /*
         * 真实Facebook登录代码示例：
         * 
         * FB.LogInWithReadPermissions(new List<string> { "public_profile", "email" }, result => {
         *     if (FB.IsLoggedIn) {
         *         var accessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
         *         // 使用Facebook凭证登录Firebase
         *         SignInWithFacebookOnFirebase(accessToken.TokenString);
         *     } else {
         *         CompleteLogin(false, "Facebook登录失败", null);
         *     }
         * });
         */
        
        Debug.LogWarning("真实Facebook登录需要Firebase SDK，当前使用模拟模式");
        SimulateFacebookLogin();
    }
    
    /// <summary>
    /// 真实的Apple登录实现
    /// 需要Apple Sign In SDK和Firebase Authentication SDK
    /// </summary>
    private void RealAppleLogin()
    {
        /*
         * 真实Apple登录代码示例：
         * 
         * var appleAuthManager = new AppleAuthManager(new PayloadDeserializer());
         * var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
         * 
         * appleAuthManager.LoginWithAppleId(loginArgs, credential => {
         *     var appleIdCredential = credential as IAppleIDCredential;
         *     if (appleIdCredential != null) {
         *         // 使用Apple凭证登录Firebase
         *         SignInWithAppleOnFirebase(appleIdCredential.IdentityToken);
         *     }
         * }, error => {
         *     CompleteLogin(false, "Apple登录失败", null);
         * });
         */
        
        Debug.LogWarning("真实Apple登录需要Firebase SDK，当前使用模拟模式");
        SimulateAppleLogin();
    }
    
    /// <summary>
    /// 真实的测试账号登录实现
    /// 使用Firebase Email/Password认证
    /// </summary>
    private void RealTestAccountLogin(string username, string password)
    {
        /*
         * 真实测试账号登录代码示例：
         * 
         * string email = $"{username}@test.ecitizen.com";
         * 
         * FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password)
         *     .ContinueWith(task => {
         *         if (task.IsCanceled || task.IsFaulted) {
         *             CompleteLogin(false, "登录失败：" + task.Exception?.Message, null);
         *         } else {
         *             var firebaseUser = task.Result;
         *             UserData userData = CreateUserDataFromFirebase(firebaseUser);
         *             CompleteLogin(true, "登录成功", userData);
         *         }
         *     });
         */
        
        Debug.LogWarning("真实测试账号登录需要Firebase SDK，当前使用模拟模式");
        SimulateTestAccountLogin(username, password);
    }
    #endregion

    #region 辅助方法
    /// <summary>
    /// 检查是否可以开始登录
    /// </summary>
    private bool CanStartLogin(string loginMethod)
    {
        // 检查Firebase是否就绪 - 使用HasInstance避免创建新实例
        if (!FirebaseInitializer.HasInstance())
        {
            Debug.LogWarning("Firebase尚未初始化完成");
            CompleteLogin(false, "系统初始化中，请稍候", null);
            return false;
        }
        
        // HasInstance返回true，所以Instance必定不为null
        if (!FirebaseInitializer.Instance.IsFirebaseReady())
        {
            Debug.LogWarning("Firebase尚未初始化完成");
            CompleteLogin(false, "系统初始化中，请稍候", null);
            return false;
        }
        
        // 检查是否已经登录
        if (isLoggedIn)
        {
            Debug.LogWarning("已经登录，无需重复登录");
            return false;
        }
        
        // 检查是否正在登录中
        if (_isLoggingIn)
        {
            Debug.LogWarning("正在登录中，请稍候");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 完成登录流程
    /// </summary>
    private void CompleteLogin(bool success, string message, UserData userData)
    {
        _isLoggingIn = false;
        loginStatus = message;
        
        Debug.Log($"[AuthManager] CompleteLogin 被调用 - success: {success}, message: {message}");
        
        if (success && userData != null)
        {
            // 登录成功
            isLoggedIn = true;
            currentUser = userData;
            
            Debug.Log($"<color=green>[AuthManager] ✓ 登录成功: {message}</color>");
            Debug.Log($"[AuthManager] 用户ID: {userData.userId}");
            Debug.Log($"[AuthManager] 用户名: {userData.username}");
            Debug.Log($"[AuthManager] 登录方式: {userData.loginProvider}");
            Debug.Log($"[AuthManager] hasCreatedCharacter: {userData.hasCreatedCharacter}");
            
            // 保存登录信息到本地（用于自动登录）
            SaveLoginInfo(userData);
            
            // 触发登录成功事件
            Debug.Log($"[AuthManager] 准备触发 OnLoginSuccess 事件，订阅者数量: {OnLoginSuccess?.GetInvocationList()?.Length ?? 0}");
            OnLoginSuccess?.Invoke(userData);
            Debug.Log($"[AuthManager] OnLoginSuccess 事件已触发");
        }
        else
        {
            // 登录失败
            isLoggedIn = false;
            currentUser = null;
            
            Debug.LogError($"<color=red>[AuthManager] ✗ 登录失败: {message}</color>");
            
            // 触发登录失败事件
            OnLoginFailed?.Invoke(message);
        }
    }
    
    /// <summary>
    /// 保存登录信息到本地
    /// 用于下次启动时自动登录
    /// </summary>
    private void SaveLoginInfo(UserData userData)
    {
        // 使用PlayerPrefs保存登录信息
        PlayerPrefs.SetString("SavedUserId", userData.userId);
        PlayerPrefs.SetString("SavedUsername", userData.username);
        PlayerPrefs.SetString("SavedLoginProvider", userData.loginProvider.ToString());
        PlayerPrefs.SetString("LastLoginTime", DateTime.Now.ToString());
        PlayerPrefs.Save();
        
        Debug.Log("登录信息已保存到本地");
    }
    
    /// <summary>
    /// 检查保存的登录信息
    /// 尝试自动登录
    /// </summary>
    private void CheckSavedLogin()
    {
        if (!PlayerPrefs.HasKey("SavedUserId"))
        {
            Debug.Log("没有保存的登录信息");
            return;
        }
        
        string savedUserId = PlayerPrefs.GetString("SavedUserId");
        string savedUsername = PlayerPrefs.GetString("SavedUsername");
        string savedProvider = PlayerPrefs.GetString("SavedLoginProvider");
        
        Debug.Log($"发现保存的登录信息: {savedUsername} ({savedProvider})");
        
        // 这里可以实现自动登录逻辑
        // 实际项目中需要验证token是否过期等
    }
    
    /// <summary>
    /// 登出
    /// </summary>
    public void SignOut()
    {
        if (!isLoggedIn)
        {
            Debug.Log("当前未登录");
            return;
        }
        
        Debug.Log($"用户 {currentUser.username} 登出");
        
        // 清除登录状态
        isLoggedIn = false;
        currentUser = null;
        loginStatus = "已登出";
        
        // 清除保存的登录信息
        PlayerPrefs.DeleteKey("SavedUserId");
        PlayerPrefs.DeleteKey("SavedUsername");
        PlayerPrefs.DeleteKey("SavedLoginProvider");
        PlayerPrefs.Save();
        
        // 触发登出事件
        OnLogout?.Invoke();
    }
    
    /// <summary>
    /// 获取当前登录状态
    /// </summary>
    public bool IsLoggedIn()
    {
        return isLoggedIn && currentUser != null;
    }
    
    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    public UserData GetCurrentUser()
    {
        return currentUser;
    }
    
    /// <summary>
    /// 加载用户游戏数据
    /// 从本地PlayerPrefs加载用户的游戏进度数据
    /// </summary>
    private void LoadUserGameData(UserData userData)
    {
        // 尝试从PlayerPrefs加载用户数据
        string savedUserId = PlayerPrefs.GetString("SavedUserId", "");
        
        Debug.Log($"[AuthManager] LoadUserGameData - 当前userId: {userData.userId}, 保存的userId: {savedUserId}");
        
        // 如果保存的用户ID与当前用户ID匹配，加载游戏数据
        if (savedUserId == userData.userId)
        {
            // 加载角色创建状态
            userData.hasCreatedCharacter = PlayerPrefs.GetInt("HasCreatedCharacter", 0) == 1;
            userData.identityType = PlayerPrefs.GetInt("IdentityType", 0);
            userData.level = PlayerPrefs.GetInt("Level", 1);
            userData.virtualCoin = PlayerPrefs.GetInt("VirtualCoin", 100);
            userData.moodValue = PlayerPrefs.GetInt("MoodValue", 10);
            
            Debug.Log($"[AuthManager] ✓ 成功加载用户数据: hasCreatedCharacter={userData.hasCreatedCharacter}, identityType={userData.identityType}");
        }
        else
        {
            // 新用户或不同的用户ID，使用默认值
            Debug.Log($"[AuthManager] ⚠ 未找到保存的用户数据，使用默认值");
            userData.hasCreatedCharacter = false;
            userData.identityType = 0;
            Debug.Log($"[AuthManager] 设置 hasCreatedCharacter = {userData.hasCreatedCharacter}");
        }
    }
    #endregion
}
