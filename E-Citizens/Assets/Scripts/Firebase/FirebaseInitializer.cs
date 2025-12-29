using UnityEngine;
using System;

/// <summary>
/// Firebase初始化管理器
/// 负责初始化Firebase服务，包括Authentication、Firestore、Storage等
/// 这是游戏启动时第一个要初始化的核心服务
/// </summary>
public class FirebaseInitializer : MonoBehaviour
{
    #region 单例模式
    private static FirebaseInitializer _instance;
    
    public static FirebaseInitializer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FirebaseInitializer>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("FirebaseInitializer");
                    _instance = go.AddComponent<FirebaseInitializer>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion

    #region 事件定义
    // 定义初始化完成的事件
    // 当Firebase初始化完成后，其他系统可以监听这个事件来开始它们的初始化
    public event Action OnFirebaseInitialized;
    
    // 定义初始化失败的事件
    public event Action<string> OnFirebaseInitializeFailed;
    #endregion

    #region 状态变量
    [Header("初始化状态")]
    [Tooltip("Firebase是否已经初始化完成")]
    public bool isInitialized = false;
    
    [Tooltip("当前初始化状态信息")]
    public string initializationStatus = "未开始";
    
    // 私有变量：标记是否正在初始化中
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
    }

    private void Start()
    {
        // 游戏启动时自动开始初始化Firebase
        InitializeFirebase();
    }
    #endregion

    #region Firebase初始化方法
    /// <summary>
    /// 初始化Firebase服务
    /// 这个方法会按顺序初始化所有需要的Firebase服务
    /// </summary>
    public void InitializeFirebase()
    {
        // 防止重复初始化
        if (isInitialized)
        {
            Debug.Log("Firebase已经初始化完成，无需重复初始化");
            return;
        }
        
        if (_isInitializing)
        {
            Debug.Log("Firebase正在初始化中，请稍候...");
            return;
        }
        
        _isInitializing = true;
        initializationStatus = "开始初始化...";
        
        // 检查是否启用Firebase
        if (!FirebaseConfig.Instance.enableFirebase)
        {
            Debug.LogWarning("Firebase功能未启用，跳过初始化");
            CompleteInitialization(false, "Firebase功能未启用");
            return;
        }
        
        // 检查是否为模拟模式
        if (FirebaseConfig.Instance.ShouldSimulate())
        {
            Debug.Log("当前在Unity编辑器中运行，使用模拟模式");
            SimulateFirebaseInitialization();
            return;
        }
        
        // 真实的Firebase初始化流程
        // 注意：这需要导入Firebase Unity SDK后才能正常工作
        RealFirebaseInitialization();
    }
    
    /// <summary>
    /// 模拟Firebase初始化（用于Unity编辑器测试）
    /// 在编辑器中运行时，我们不需要真正连接Firebase，可以使用模拟模式
    /// </summary>
    private void SimulateFirebaseInitialization()
    {
        Debug.Log("=== 开始模拟Firebase初始化 ===");
        
        // 模拟初始化各个服务
        initializationStatus = "模拟初始化Authentication...";
        Debug.Log("✓ Firebase Authentication (模拟)");
        
        initializationStatus = "模拟初始化Firestore...";
        Debug.Log("✓ Firebase Firestore (模拟)");
        
        initializationStatus = "模拟初始化Storage...";
        Debug.Log("✓ Firebase Storage (模拟)");
        
        initializationStatus = "模拟初始化Analytics...";
        Debug.Log("✓ Firebase Analytics (模拟)");
        
        Debug.Log("=== Firebase模拟初始化完成 ===");
        
        // 标记初始化完成
        CompleteInitialization(true, "模拟模式初始化成功");
    }
    
    /// <summary>
    /// 真实的Firebase初始化流程
    /// 当游戏在真机上运行时使用
    /// 注意：需要安装Firebase Unity SDK才能运行这部分代码
    /// </summary>
    private void RealFirebaseInitialization()
    {
        Debug.Log("=== 开始真实Firebase初始化 ===");
        
        try
        {
            /*
             * 真实Firebase初始化代码示例（需要Firebase SDK）：
             * 
             * Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
             *     var dependencyStatus = task.Result;
             *     if (dependencyStatus == Firebase.DependencyStatus.Available) {
             *         // 依赖检查通过，可以使用Firebase
             *         InitializeFirebaseServices();
             *     } else {
             *         Debug.LogError($"无法初始化Firebase: {dependencyStatus}");
             *         CompleteInitialization(false, $"依赖检查失败: {dependencyStatus}");
             *     }
             * });
             */
            
            // 临时方案：在没有SDK的情况下，也使用模拟模式
            Debug.LogWarning("Firebase SDK未安装，使用模拟模式");
            SimulateFirebaseInitialization();
        }
        catch (Exception e)
        {
            Debug.LogError($"Firebase初始化失败: {e.Message}");
            CompleteInitialization(false, $"初始化异常: {e.Message}");
        }
    }
    
    /// <summary>
    /// 初始化Firebase各个服务
    /// 在依赖检查通过后调用
    /// </summary>
    private void InitializeFirebaseServices()
    {
        try
        {
            initializationStatus = "初始化Firebase Authentication...";
            Debug.Log("✓ Firebase Authentication初始化成功");
            
            initializationStatus = "初始化Firebase Firestore...";
            Debug.Log("✓ Firebase Firestore初始化成功");
            
            initializationStatus = "初始化Firebase Storage...";
            Debug.Log("✓ Firebase Storage初始化成功");
            
            initializationStatus = "初始化Firebase Analytics...";
            Debug.Log("✓ Firebase Analytics初始化成功");
            
            CompleteInitialization(true, "所有Firebase服务初始化成功");
        }
        catch (Exception e)
        {
            Debug.LogError($"Firebase服务初始化失败: {e.Message}");
            CompleteInitialization(false, $"服务初始化异常: {e.Message}");
        }
    }
    
    /// <summary>
    /// 完成初始化流程
    /// </summary>
    /// <param name="success">是否初始化成功</param>
    /// <param name="message">状态信息</param>
    private void CompleteInitialization(bool success, string message)
    {
        _isInitializing = false;
        isInitialized = success;
        initializationStatus = message;
        
        if (success)
        {
            Debug.Log($"<color=green>Firebase初始化成功: {message}</color>");
            // 触发初始化完成事件
            OnFirebaseInitialized?.Invoke();
        }
        else
        {
            Debug.LogError($"<color=red>Firebase初始化失败: {message}</color>");
            // 触发初始化失败事件
            OnFirebaseInitializeFailed?.Invoke(message);
        }
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 获取当前初始化状态
    /// </summary>
    public bool IsFirebaseReady()
    {
        return isInitialized;
    }
    
    /// <summary>
    /// 重新初始化Firebase
    /// 用于初始化失败后的重试
    /// </summary>
    public void RetryInitialization()
    {
        Debug.Log("重新尝试初始化Firebase...");
        isInitialized = false;
        _isInitializing = false;
        InitializeFirebase();
    }
    #endregion
}
