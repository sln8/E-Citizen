using UnityEngine;

/// <summary>
/// Firebase配置类
/// 用于管理Firebase相关的配置信息
/// 这个类包含了Firebase初始化所需的所有配置参数
/// </summary>
public class FirebaseConfig : MonoBehaviour
{
    #region 单例模式 (Singleton Pattern)
    // 单例实例，确保整个游戏中只有一个FirebaseConfig对象
    private static FirebaseConfig _instance;
    
    /// <summary>
    /// 获取FirebaseConfig的单例实例
    /// 单例模式：确保全局只有一个配置对象，方便在任何地方访问
    /// </summary>
    public static FirebaseConfig Instance
    {
        get
        {
            // 如果实例不存在，尝试在场景中查找
            if (_instance == null)
            {
                _instance = FindObjectOfType<FirebaseConfig>();
                
                // 如果场景中也没有，创建一个新的GameObject并添加此组件
                if (_instance == null)
                {
                    GameObject go = new GameObject("FirebaseConfig");
                    _instance = go.AddComponent<FirebaseConfig>();
                    DontDestroyOnLoad(go); // 确保切换场景时不被销毁
                }
            }
            return _instance;
        }
    }
    #endregion

    #region Firebase配置参数
    [Header("Firebase基础配置")]
    [Tooltip("是否启用Firebase功能")]
    public bool enableFirebase = true;
    
    [Tooltip("是否在编辑器中模拟Firebase功能（用于测试）")]
    public bool simulateInEditor = true;
    
    [Header("Firebase Authentication配置")]
    [Tooltip("是否启用Google登录")]
    public bool enableGoogleSignIn = true;
    
    [Tooltip("是否启用Facebook登录")]
    public bool enableFacebookSignIn = true;
    
    [Tooltip("是否启用Apple登录（仅iOS）")]
    public bool enableAppleSignIn = true;
    
    [Tooltip("是否启用测试账号登录")]
    public bool enableTestAccounts = true;
    
    [Header("测试模式配置")]
    [Tooltip("是否为测试模式（测试模式下会绕过真实支付）")]
    public bool isTestMode = true;
    
    [Tooltip("测试模式提示信息")]
    public string testModeMessage = "当前为测试模式，所有支付功能将被模拟";
    
    [Header("Firebase数据库配置")]
    [Tooltip("Firestore数据库URL")]
    public string firestoreDatabaseUrl = "";
    
    [Tooltip("Firebase Storage URL")]
    public string storageUrl = "";
    
    [Header("调试配置")]
    [Tooltip("是否显示详细日志")]
    public bool enableDebugLogs = true;
    
    [Tooltip("是否在UI上显示错误信息（方便调试）")]
    public bool showErrorUI = true;
    #endregion

    #region Unity生命周期方法
    /// <summary>
    /// Unity的Awake方法，在对象创建时自动调用
    /// 用于初始化单例模式
    /// </summary>
    private void Awake()
    {
        // 如果已经存在实例且不是当前对象，销毁当前对象
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // 设置当前对象为单例实例
        _instance = this;
        
        // 确保切换场景时此对象不被销毁
        DontDestroyOnLoad(gameObject);
        
        // 输出配置信息到控制台
        LogConfig();
    }
    #endregion

    #region 辅助方法
    /// <summary>
    /// 在控制台输出当前的配置信息
    /// 方便开发者查看和调试
    /// </summary>
    private void LogConfig()
    {
        if (!enableDebugLogs) return;
        
        Debug.Log("=== Firebase配置信息 ===");
        Debug.Log($"Firebase启用状态: {enableFirebase}");
        Debug.Log($"编辑器模拟模式: {simulateInEditor}");
        Debug.Log($"测试模式: {isTestMode}");
        Debug.Log($"Google登录: {enableGoogleSignIn}");
        Debug.Log($"Facebook登录: {enableFacebookSignIn}");
        Debug.Log($"Apple登录: {enableAppleSignIn}");
        Debug.Log($"测试账号登录: {enableTestAccounts}");
        Debug.Log("====================");
    }
    
    /// <summary>
    /// 检查当前是否应该使用模拟模式
    /// 在Unity编辑器中且开启了模拟模式时返回true
    /// </summary>
    public bool ShouldSimulate()
    {
        #if UNITY_EDITOR
        return simulateInEditor;
        #else
        return false;
        #endif
    }
    
    /// <summary>
    /// 获取当前平台名称
    /// 用于区分iOS、Android等不同平台
    /// </summary>
    public string GetPlatformName()
    {
        #if UNITY_IOS
        return "iOS";
        #elif UNITY_ANDROID
        return "Android";
        #elif UNITY_EDITOR
        return "Editor";
        #else
        return "Unknown";
        #endif
    }
    #endregion
}
