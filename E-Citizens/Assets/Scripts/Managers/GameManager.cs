using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏主管理器
/// 负责游戏的整体流程控制和各个系统的协调
/// 这是游戏的入口点，负责初始化所有核心系统
/// </summary>
public class GameManager : MonoBehaviour
{
    #region 单例模式
    private static GameManager _instance;
    
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion

    #region 游戏状态
    /// <summary>
    /// 游戏状态枚举
    /// 定义游戏的各个状态
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// 初始化中
        /// 正在加载和初始化各个系统
        /// </summary>
        Initializing,
        
        /// <summary>
        /// 登录界面
        /// 等待用户登录
        /// </summary>
        Login,
        
        /// <summary>
        /// 角色创建
        /// 新用户创建角色
        /// </summary>
        CharacterCreation,
        
        /// <summary>
        /// 主游戏
        /// 正常游戏进行中
        /// </summary>
        MainGame,
        
        /// <summary>
        /// 暂停
        /// 游戏暂停状态
        /// </summary>
        Paused,
        
        /// <summary>
        /// 加载中
        /// 场景切换或资源加载中
        /// </summary>
        Loading
    }
    
    [Header("游戏状态")]
    [Tooltip("当前游戏状态")]
    public GameState currentState = GameState.Initializing;
    
    [Tooltip("上一个游戏状态")]
    public GameState previousState = GameState.Initializing;
    #endregion

    #region 配置参数
    [Header("场景配置")]
    [Tooltip("登录场景名称")]
    public string loginSceneName = "LoginScene";
    
    [Tooltip("角色创建场景名称")]
    public string characterCreationSceneName = "SelectScene";
    
    [Tooltip("主游戏场景名称")]
    public string mainGameSceneName = "GameScene";
    
    [Header("调试配置")]
    [Tooltip("是否跳过登录（仅开发调试使用）")]
    public bool skipLogin = false;
    
    [Tooltip("是否显示调试UI")]
    public bool showDebugUI = true;
    #endregion

    #region 初始化标记
    private bool _isInitialized = false;
    private bool _isInitializing = false;
    #endregion

    #region Unity生命周期方法
    private void Awake()
    {
        // 单例模式检查
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("=== 游戏管理器已创建 ===");
        
        // 监听场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // 开始初始化游戏
        InitializeGame();
    }

    private void Update()
    {
        // 调试快捷键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapePressed();
        }
        
        // 显示调试信息
        if (showDebugUI && Input.GetKeyDown(KeyCode.F1))
        {
            ToggleDebugInfo();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("=== 游戏退出 ===");
        
        // 保存游戏数据
        SaveGameData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("游戏进入后台");
            // 自动保存数据
            SaveGameData();
        }
        else
        {
            Debug.Log("游戏返回前台");
        }
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
        if (scene.name == "GameScene" && AuthenticationManager.Instance != null && AuthenticationManager.Instance.IsLoggedIn())
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
    #endregion

    #region 游戏初始化
    /// <summary>
    /// 初始化游戏
    /// 按顺序初始化所有核心系统
    /// </summary>
    private void InitializeGame()
    {
        if (_isInitialized || _isInitializing)
        {
            Debug.LogWarning("游戏已经初始化或正在初始化中");
            return;
        }
        
        _isInitializing = true;
        currentState = GameState.Initializing;
        
        Debug.Log("=== 开始游戏初始化 ===");
        
        // 步骤1: 初始化Firebase配置
        InitializeFirebaseConfig();
        
        // 步骤2: 初始化Firebase服务
        InitializeFirebaseServices();
        
        // 步骤3: 初始化认证系统
        InitializeAuthenticationSystem();
        
        // 步骤4: 注册事件监听
        RegisterEventListeners();
        
        // 步骤5: 完成初始化
        CompleteInitialization();
    }
    
    /// <summary>
    /// 初始化Firebase配置
    /// </summary>
    private void InitializeFirebaseConfig()
    {
        Debug.Log("→ 初始化Firebase配置...");
        
        // 确保FirebaseConfig存在
        if (FirebaseConfig.Instance == null)
        {
            Debug.LogError("FirebaseConfig未找到！");
            return;
        }
        
        Debug.Log("✓ Firebase配置初始化完成");
    }
    
    /// <summary>
    /// 初始化Firebase服务
    /// </summary>
    private void InitializeFirebaseServices()
    {
        Debug.Log("→ 初始化Firebase服务...");
        
        // 确保FirebaseInitializer存在
        if (FirebaseInitializer.Instance == null)
        {
            Debug.LogError("FirebaseInitializer未找到！");
            return;
        }
        
        // Firebase会在Start方法中自动初始化
        Debug.Log("✓ Firebase服务正在初始化中...");
    }
    
    /// <summary>
    /// 初始化认证系统
    /// </summary>
    private void InitializeAuthenticationSystem()
    {
        Debug.Log("→ 初始化认证系统...");
        
        // 确保AuthenticationManager存在
        if (AuthenticationManager.Instance == null)
        {
            Debug.LogError("AuthenticationManager未找到！");
            return;
        }
        
        Debug.Log("✓ 认证系统初始化完成");
    }
    
    /// <summary>
    /// 注册事件监听
    /// </summary>
    private void RegisterEventListeners()
    {
        Debug.Log("→ 注册事件监听...");
        
        // 监听Firebase初始化完成事件
        if (FirebaseInitializer.Instance != null)
        {
            FirebaseInitializer.Instance.OnFirebaseInitialized += OnFirebaseInitialized;
            FirebaseInitializer.Instance.OnFirebaseInitializeFailed += OnFirebaseInitializeFailed;
        }
        
        // 监听认证事件
        if (AuthenticationManager.Instance != null)
        {
            AuthenticationManager.Instance.OnLoginSuccess += OnLoginSuccess;
            AuthenticationManager.Instance.OnLoginFailed += OnLoginFailed;
            AuthenticationManager.Instance.OnLogout += OnLogout;
        }
        
        Debug.Log("✓ 事件监听注册完成");
    }
    
    /// <summary>
    /// 完成初始化
    /// </summary>
    private void CompleteInitialization()
    {
        _isInitializing = false;
        _isInitialized = true;
        
        Debug.Log("=== 游戏初始化完成 ===");
        
        // 根据配置决定下一步
        if (skipLogin)
        {
            Debug.LogWarning("<color=yellow>跳过登录模式已启用（仅用于开发调试）</color>");
            // 直接进入主游戏（使用测试数据）
            EnterMainGame();
        }
        else
        {
            // 进入登录状态
            ChangeState(GameState.Login);
        }
    }
    #endregion

    #region 事件回调
    /// <summary>
    /// Firebase初始化完成回调
    /// </summary>
    private void OnFirebaseInitialized()
    {
        Debug.Log("<color=green>[GameManager] Firebase初始化成功</color>");
    }
    
    /// <summary>
    /// Firebase初始化失败回调
    /// </summary>
    private void OnFirebaseInitializeFailed(string errorMessage)
    {
        Debug.LogError($"<color=red>[GameManager] Firebase初始化失败: {errorMessage}</color>");
        
        // 可以显示错误UI，让用户重试
        // 或者继续使用离线模式
    }
    
    /// <summary>
    /// 登录成功回调
    /// </summary>
    private void OnLoginSuccess(UserData userData)
    {
        Debug.Log($"<color=green>[GameManager] 用户登录成功: {userData.username}</color>");
        
        // 检查是否是新用户
        if (IsNewUser(userData))
        {
            Debug.Log("检测到新用户，进入角色创建流程");
            EnterCharacterCreation();
        }
        else
        {
            Debug.Log("欢迎回来！加载用户数据...");
            LoadUserData(userData);
            EnterMainGame();
        }
    }
    
    /// <summary>
    /// 登录失败回调
    /// </summary>
    private void OnLoginFailed(string errorMessage)
    {
        Debug.LogError($"<color=red>[GameManager] 登录失败: {errorMessage}</color>");
        
        // 保持在登录界面，显示错误信息
        ChangeState(GameState.Login);
    }
    
    /// <summary>
    /// 登出回调
    /// </summary>
    private void OnLogout()
    {
        Debug.Log("[GameManager] 用户已登出");
        
        // 清除当前游戏数据
        ClearGameData();
        
        // 返回登录界面
        LoadLoginScene();
    }
    #endregion

    #region 状态管理
    /// <summary>
    /// 改变游戏状态
    /// </summary>
    public void ChangeState(GameState newState)
    {
        if (currentState == newState)
        {
            return;
        }
        
        Debug.Log($"[状态切换] {currentState} -> {newState}");
        
        previousState = currentState;
        currentState = newState;
        
        // 根据新状态执行相应操作
        OnStateChanged(newState);
    }
    
    /// <summary>
    /// 状态改变时的处理
    /// </summary>
    private void OnStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Login:
                // 已经在登录场景，不需要额外操作
                break;
                
            case GameState.CharacterCreation:
                // 加载角色创建场景
                LoadCharacterCreationScene();
                break;
                
            case GameState.MainGame:
                // 加载主游戏场景
                LoadMainGameScene();
                break;
                
            case GameState.Paused:
                Time.timeScale = 0f; // 暂停游戏时间
                break;
                
            case GameState.Loading:
                // 显示加载界面
                break;
        }
        
        // 如果从暂停状态恢复，重置时间流速
        if (previousState == GameState.Paused && newState != GameState.Paused)
        {
            Time.timeScale = 1f;
        }
    }
    #endregion

    #region 场景管理
    /// <summary>
    /// 加载登录场景
    /// </summary>
    private void LoadLoginScene()
    {
        ChangeState(GameState.Loading);
        Debug.Log($"加载登录场景: {loginSceneName}");
        
        // 这里应该使用实际的场景加载逻辑
        // SceneManager.LoadScene(loginSceneName);
        
        ChangeState(GameState.Login);
    }
    
    /// <summary>
    /// 加载角色创建场景
    /// </summary>
    private void LoadCharacterCreationScene()
    {
        ChangeState(GameState.Loading);
        Debug.Log($"加载角色创建场景: {characterCreationSceneName}");
        
        // 这里应该使用实际的场景加载逻辑
        //SceneManager.LoadScene(characterCreationSceneName);
        
        ChangeState(GameState.CharacterCreation);
    }
    
    /// <summary>
    /// 加载主游戏场景
    /// </summary>
    private void LoadMainGameScene()
    {
        ChangeState(GameState.Loading);
        Debug.Log($"加载主游戏场景: {mainGameSceneName}");
        
        // 这里应该使用实际的场景加载逻辑
        // SceneManager.LoadScene(mainGameSceneName);
        
        ChangeState(GameState.MainGame);
    }
    
    /// <summary>
    /// 进入角色创建流程
    /// </summary>
    private void EnterCharacterCreation()
    {
        ChangeState(GameState.CharacterCreation);
        
        // 显示初始选择界面
        if (InitialSelectionManager.Instance != null)
        {
            InitialSelectionManager.Instance.ShowInitialSelection();
        }
        else
        {
            Debug.LogError("InitialSelectionManager未找到！");
        }
    }
    
    /// <summary>
    /// 进入主游戏
    /// </summary>
    private void EnterMainGame()
    {
        ChangeState(GameState.MainGame);
    }
    #endregion

    #region 用户数据管理
    /// <summary>
    /// 检查是否为新用户
    /// </summary>
    private bool IsNewUser(UserData userData)
    {
        // 使用hasCreatedCharacter字段判断是否为新用户
        // 如果为false，说明用户还未完成初始选择流程
        return !userData.hasCreatedCharacter;
    }
    
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
        // 包括资源、工作、公司、房产等信息
        Debug.Log("用户数据加载完成");
    }
    
    /// <summary>
    /// 完成角色创建
    /// 在玩家完成初始选择后调用
    /// </summary>
    /// <param name="selectedIdentity">玩家选择的身份类型</param>
    public void CompleteCharacterCreation(IdentityType selectedIdentity)
    {
        Debug.Log($"<color=green>完成角色创建，身份类型：{selectedIdentity}</color>");
        
        // 1. 设置资源管理器的玩家身份
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.SetPlayerIdentity(selectedIdentity);
        }
        else
        {
            Debug.LogError("ResourceManager未找到！");
        }
        
        // 2. 更新用户数据
        if (AuthenticationManager.Instance != null && AuthenticationManager.Instance.currentUser != null)
        {
            UserData currentUser = AuthenticationManager.Instance.currentUser;
            currentUser.hasCreatedCharacter = true;
            currentUser.identityType = (int)selectedIdentity;
            
            // 保存到Firebase（实际项目中需要实现Firebase保存逻辑）
            SaveUserDataToFirebase(currentUser);
        }
        else
        {
            Debug.LogError("无法获取当前用户信息！");
        }
        
        // 3. 进入主游戏场景
        EnterMainGame();
    }
    
    /// <summary>
    /// 保存用户数据到Firebase
    /// </summary>
    private void SaveUserDataToFirebase(UserData userData)
    {
        Debug.Log("保存用户数据到Firebase...");
        
        // TODO: 实际项目中需要实现Firebase Firestore保存逻辑
        // 当前版本使用本地PlayerPrefs作为临时存储
        
        // 临时方案：保存到本地PlayerPrefs
        PlayerPrefs.SetInt("HasCreatedCharacter", userData.hasCreatedCharacter ? 1 : 0);
        PlayerPrefs.SetInt("IdentityType", userData.identityType);
        PlayerPrefs.SetInt("Level", userData.level);
        PlayerPrefs.SetInt("VirtualCoin", userData.virtualCoin);
        PlayerPrefs.SetInt("MoodValue", userData.moodValue);
        PlayerPrefs.SetString("Username", userData.username);
        PlayerPrefs.SetString("UserId", userData.userId);
        PlayerPrefs.Save();
        
        Debug.Log("✓ 用户数据已保存到本地PlayerPrefs（临时方案）");
        Debug.LogWarning("注意：生产环境需要实现Firebase Firestore保存逻辑");
    }
    
    /// <summary>
    /// 保存游戏数据
    /// </summary>
    private void SaveGameData()
    {
        if (!AuthenticationManager.Instance.IsLoggedIn())
        {
            return;
        }
        
        Debug.Log("保存游戏数据...");
        
        // 这里应该将当前游戏状态保存到Firebase Firestore
        // 包括资源、工作进度、心情值等
    }
    
    /// <summary>
    /// 清除游戏数据
    /// </summary>
    private void ClearGameData()
    {
        Debug.Log("清除本地游戏数据");
        
        // 清除内存中的游戏数据
        // 不删除Firebase上的数据
    }
    #endregion

    #region 输入处理
    /// <summary>
    /// ESC键按下处理
    /// </summary>
    private void OnEscapePressed()
    {
        switch (currentState)
        {
            case GameState.MainGame:
                // 暂停游戏
                ChangeState(GameState.Paused);
                Debug.Log("游戏已暂停");
                break;
                
            case GameState.Paused:
                // 恢复游戏
                ChangeState(GameState.MainGame);
                Debug.Log("游戏已恢复");
                break;
        }
    }
    
    /// <summary>
    /// 切换调试信息显示
    /// </summary>
    private void ToggleDebugInfo()
    {
        showDebugUI = !showDebugUI;
        Debug.Log($"调试UI显示: {showDebugUI}");
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 获取当前游戏状态
    /// </summary>
    public GameState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// 检查游戏是否已初始化
    /// </summary>
    public bool IsGameInitialized()
    {
        return _isInitialized;
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("退出游戏");
        SaveGameData();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    #endregion

    #region 调试GUI
    private void OnGUI()
    {
        if (!showDebugUI) return;
        
        // 显示调试信息
        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.Box("=== 调试信息 ===");
        GUILayout.Label($"游戏状态: {currentState}");
        GUILayout.Label($"Firebase状态: {(FirebaseInitializer.Instance.isInitialized ? "已初始化" : "未初始化")}");
        GUILayout.Label($"登录状态: {(AuthenticationManager.Instance.isLoggedIn ? "已登录" : "未登录")}");
        
        if (AuthenticationManager.Instance.isLoggedIn && AuthenticationManager.Instance.currentUser != null)
        {
            GUILayout.Label($"用户名: {AuthenticationManager.Instance.currentUser.username}");
            GUILayout.Label($"用户ID: {AuthenticationManager.Instance.currentUser.userId}");
        }
        
        GUILayout.Label($"平台: {FirebaseConfig.Instance.GetPlatformName()}");
        GUILayout.Label($"测试模式: {FirebaseConfig.Instance.isTestMode}");
        
        GUILayout.EndArea();
    }
    #endregion
}
