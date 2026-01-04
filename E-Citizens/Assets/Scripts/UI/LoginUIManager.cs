using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 登录UI管理器
/// 负责管理登录界面的所有UI元素和交互
/// </summary>
public class LoginUIManager : MonoBehaviour
{
    #region UI元素引用
    [Header("登录按钮")]
    [Tooltip("Google登录按钮")]
    public Button googleLoginButton;
    
    [Tooltip("Facebook登录按钮")]
    public Button facebookLoginButton;
    
    [Tooltip("Apple登录按钮（仅iOS）")]
    public Button appleLoginButton;
    
    [Tooltip("测试账号登录按钮")]
    public Button testAccountLoginButton;
    
    [Tooltip("快速创建测试账号按钮")]
    public Button quickTestAccountButton;
    
    [Header("测试账号输入框")]
    [Tooltip("测试账号用户名输入框")]
    public TMP_InputField testUsernameInput;
    
    [Tooltip("测试账号密码输入框")]
    public TMP_InputField testPasswordInput;
    
    [Header("状态显示")]
    [Tooltip("状态文本显示")]
    public TMP_Text statusText;
    
    [Tooltip("加载动画对象")]
    public GameObject loadingPanel;
    
    [Header("测试模式提示")]
    [Tooltip("测试模式提示文本")]
    public TMP_Text testModeText;
    
    [Tooltip("测试模式面板")]
    public GameObject testModePanel;
    #endregion

    #region Unity生命周期方法
    private void Start()
    {
        Debug.Log("[LoginUI] Start() 被调用");
        
        // 初始化UI
        InitializeUI();
        
        // 注册按钮点击事件
        RegisterButtonEvents();
        
        // 注册认证管理器事件
        // 直接访问Instance以确保它被创建
        if (AuthenticationManager.Instance != null)
        {
            RegisterAuthenticationEvents();
            Debug.Log("[LoginUI] ✓ 已注册认证管理器事件");
        }
        else
        {
            Debug.LogError("[LoginUI] ✗ AuthenticationManager 初始化失败");
        }
        
        // 显示测试模式提示
        ShowTestModeWarning();
    }

    private void OnDestroy()
    {
        Debug.Log("[LoginUI] OnDestroy() 被调用");
        
        // 取消按钮事件
        UnregisterButtonEvents();
        
        // 取消认证管理器事件
        // 只有在实例存在时才取消订阅，避免在OnDestroy时创建新实例
        if (AuthenticationManager.HasInstance())
        {
            UnregisterAuthenticationEvents();
            Debug.Log("[LoginUI] ✓ 已取消注册认证管理器事件");
        }
    }
    #endregion

    #region 初始化方法
    /// <summary>
    /// 初始化UI元素
    /// </summary>
    private void InitializeUI()
    {
        // 隐藏加载面板
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        
        // 设置初始状态文本
        if (statusText != null)
        {
            statusText.text = "请选择登录方式";
        }
        
        // 根据平台显示/隐藏Apple登录按钮
        #if !UNITY_IOS
        if (appleLoginButton != null)
        {
            appleLoginButton.gameObject.SetActive(false);
        }
        #endif
        
        // 根据配置显示/隐藏测试账号功能
        if (!FirebaseConfig.Instance.enableTestAccounts)
        {
            if (testAccountLoginButton != null)
            {
                testAccountLoginButton.gameObject.SetActive(false);
            }
            if (quickTestAccountButton != null)
            {
                quickTestAccountButton.gameObject.SetActive(false);
            }
            if (testUsernameInput != null)
            {
                testUsernameInput.gameObject.SetActive(false);
            }
            if (testPasswordInput != null)
            {
                testPasswordInput.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 显示测试模式警告
    /// </summary>
    private void ShowTestModeWarning()
    {
        if (FirebaseConfig.Instance.isTestMode)
        {
            if (testModePanel != null)
            {
                testModePanel.SetActive(true);
            }
            
            if (testModeText != null)
            {
                testModeText.text = FirebaseConfig.Instance.testModeMessage;
            }
            
            Debug.LogWarning($"<color=yellow>{FirebaseConfig.Instance.testModeMessage}</color>");
        }
        else
        {
            if (testModePanel != null)
            {
                testModePanel.SetActive(false);
            }
        }
    }
    #endregion

    #region 事件注册
    /// <summary>
    /// 注册按钮点击事件
    /// </summary>
    private void RegisterButtonEvents()
    {
        // Google登录按钮
        if (googleLoginButton != null)
        {
            googleLoginButton.onClick.AddListener(OnGoogleLoginClicked);
        }
        
        // Facebook登录按钮
        if (facebookLoginButton != null)
        {
            facebookLoginButton.onClick.AddListener(OnFacebookLoginClicked);
        }
        
        // Apple登录按钮
        if (appleLoginButton != null)
        {
            appleLoginButton.onClick.AddListener(OnAppleLoginClicked);
        }
        
        // 测试账号登录按钮
        if (testAccountLoginButton != null)
        {
            testAccountLoginButton.onClick.AddListener(OnTestAccountLoginClicked);
        }
        
        // 快速创建测试账号按钮
        if (quickTestAccountButton != null)
        {
            quickTestAccountButton.onClick.AddListener(OnQuickTestAccountClicked);
        }
    }
    
    /// <summary>
    /// 取消注册按钮点击事件
    /// </summary>
    private void UnregisterButtonEvents()
    {
        if (googleLoginButton != null)
        {
            googleLoginButton.onClick.RemoveListener(OnGoogleLoginClicked);
        }
        
        if (facebookLoginButton != null)
        {
            facebookLoginButton.onClick.RemoveListener(OnFacebookLoginClicked);
        }
        
        if (appleLoginButton != null)
        {
            appleLoginButton.onClick.RemoveListener(OnAppleLoginClicked);
        }
        
        if (testAccountLoginButton != null)
        {
            testAccountLoginButton.onClick.RemoveListener(OnTestAccountLoginClicked);
        }
        
        if (quickTestAccountButton != null)
        {
            quickTestAccountButton.onClick.RemoveListener(OnQuickTestAccountClicked);
        }
    }
    
    /// <summary>
    /// 注册认证管理器事件
    /// </summary>
    private void RegisterAuthenticationEvents()
    {
        Debug.Log("[LoginUI] RegisterAuthenticationEvents() 被调用");
        
        // 确保AuthenticationManager.Instance不为null
        if (AuthenticationManager.Instance == null)
        {
            Debug.LogError("[LoginUI] AuthenticationManager.Instance 为 null，无法注册事件");
            return;
        }
        
        // 先清除可能存在的旧订阅，避免重复订阅
        AuthenticationManager.Instance.OnLoginSuccess -= OnLoginSuccess;
        AuthenticationManager.Instance.OnLoginFailed -= OnLoginFailed;
        
        // 重新订阅
        AuthenticationManager.Instance.OnLoginSuccess += OnLoginSuccess;
        AuthenticationManager.Instance.OnLoginFailed += OnLoginFailed;
        
        Debug.Log("[LoginUI] ✓ 事件订阅完成");
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        // 仅在编辑器或开发构建中显示订阅者数量，避免生产环境性能影响
        Debug.Log($"[LoginUI] OnLoginSuccess 订阅者数量: {AuthenticationManager.Instance.OnLoginSuccess?.GetInvocationList()?.Length ?? 0}");
        #endif
    }
    
    /// <summary>
    /// 取消注册认证管理器事件
    /// </summary>
    private void UnregisterAuthenticationEvents()
    {
        Debug.Log("[LoginUI] UnregisterAuthenticationEvents() 被调用");
        
        // 使用HasInstance检查后安全访问Instance
        if (AuthenticationManager.Instance != null)
        {
            AuthenticationManager.Instance.OnLoginSuccess -= OnLoginSuccess;
            AuthenticationManager.Instance.OnLoginFailed -= OnLoginFailed;
            Debug.Log("[LoginUI] ✓ 事件取消订阅完成");
        }
    }
    #endregion

    #region 按钮点击回调
    /// <summary>
    /// Google登录按钮点击回调
    /// </summary>
    private void OnGoogleLoginClicked()
    {
        Debug.Log("用户点击了Google登录按钮");
        UpdateStatus("正在启动Google登录...");
        ShowLoading(true);
        
        // 调用认证管理器的Google登录方法
        AuthenticationManager.Instance.SignInWithGoogle();
    }
    
    /// <summary>
    /// Facebook登录按钮点击回调
    /// </summary>
    private void OnFacebookLoginClicked()
    {
        Debug.Log("用户点击了Facebook登录按钮");
        UpdateStatus("正在启动Facebook登录...");
        ShowLoading(true);
        
        // 调用认证管理器的Facebook登录方法
        AuthenticationManager.Instance.SignInWithFacebook();
    }
    
    /// <summary>
    /// Apple登录按钮点击回调
    /// </summary>
    private void OnAppleLoginClicked()
    {
        Debug.Log("用户点击了Apple登录按钮");
        UpdateStatus("正在启动Apple登录...");
        ShowLoading(true);
        
        // 调用认证管理器的Apple登录方法
        AuthenticationManager.Instance.SignInWithApple();
    }
    
    /// <summary>
    /// 测试账号登录按钮点击回调
    /// </summary>
    private void OnTestAccountLoginClicked()
    {
        Debug.Log("[LoginUI] ========== 测试账号登录按钮被点击 ==========");
        
        // 获取输入的账号和密码
        string username = testUsernameInput != null ? testUsernameInput.text : "";
        string password = testPasswordInput != null ? testPasswordInput.text : "";
        
        Debug.Log($"[LoginUI] 输入的用户名: '{username}', 密码长度: {password.Length}");
        
        // 验证输入
        if (string.IsNullOrEmpty(username))
        {
            UpdateStatus("请输入测试账号");
            Debug.LogWarning("[LoginUI] 验证失败: 用户名为空");
            return;
        }
        
        if (string.IsNullOrEmpty(password))
        {
            UpdateStatus("请输入密码");
            Debug.LogWarning("[LoginUI] 验证失败: 密码为空");
            return;
        }
        
        UpdateStatus("正在使用测试账号登录...");
        ShowLoading(true);
        
        Debug.Log($"[LoginUI] ✓ 输入验证通过，调用 AuthenticationManager.SignInWithTestAccount()");
        
        // 调用认证管理器的测试账号登录方法
        AuthenticationManager.Instance.SignInWithTestAccount(username, password);
    }
    
    /// <summary>
    /// 快速创建测试账号按钮点击回调
    /// </summary>
    private void OnQuickTestAccountClicked()
    {
        Debug.Log("用户点击了快速创建测试账号按钮");
        UpdateStatus("正在创建测试账号...");
        ShowLoading(true);
        
        // 调用认证管理器的快速创建测试账号方法
        AuthenticationManager.Instance.CreateQuickTestAccount();
    }
    #endregion

    #region 认证事件回调
    /// <summary>
    /// 登录成功回调
    /// </summary>
    private void OnLoginSuccess(UserData userData)
    {
        Debug.Log($"<color=green>[LoginUI] 登录成功回调被触发！欢迎 {userData.username}</color>");
        Debug.Log($"[LoginUI] 用户数据检查:");
        Debug.Log($"[LoginUI] - userId: {userData.userId}");
        Debug.Log($"[LoginUI] - username: {userData.username}");
        Debug.Log($"[LoginUI] - hasCreatedCharacter: {userData.hasCreatedCharacter}");
        Debug.Log($"[LoginUI] - identityType: {userData.identityType}");
        
        ShowLoading(false);
        UpdateStatus($"登录成功！欢迎 {userData.username}");
        
        // 检查用户是否已完成初始选择
        if (!userData.hasCreatedCharacter)
        {
            // 首次登录，跳转到初始选择场景
            Debug.Log("<color=cyan>[LoginUI] ➤➤➤ 检测到首次登录，准备跳转到 SelectScene</color>");
            try
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("SelectScene");
                Debug.Log("<color=cyan>[LoginUI] ✓ LoadScene('SelectScene') 调用成功</color>");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"<color=red>[LoginUI] ✗ 加载SelectScene失败: {e.Message}</color>");
            }
        }
        else
        {
            // 已完成初始选择，直接跳转到游戏场景
            Debug.Log("<color=yellow>[LoginUI] ➤➤➤ 欢迎回来！准备跳转到 GameScene</color>");
            try
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
                Debug.Log("<color=yellow>[LoginUI] ✓ LoadScene('GameScene') 调用成功</color>");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"<color=red>[LoginUI] ✗ 加载GameScene失败: {e.Message}</color>");
            }
        }
    }
    
    /// <summary>
    /// 登录失败回调
    /// </summary>
    private void OnLoginFailed(string errorMessage)
    {
        Debug.LogError($"<color=red>登录失败: {errorMessage}</color>");
        
        ShowLoading(false);
        UpdateStatus($"登录失败: {errorMessage}");
    }
    #endregion

    #region UI辅助方法
    /// <summary>
    /// 更新状态文本
    /// </summary>
    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        
        Debug.Log($"[UI状态] {message}");
    }
    
    /// <summary>
    /// 显示/隐藏加载动画
    /// </summary>
    private void ShowLoading(bool show)
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(show);
        }
        
        // 禁用/启用所有按钮
        SetButtonsInteractable(!show);
    }
    
    /// <summary>
    /// 设置所有按钮的可交互状态
    /// </summary>
    private void SetButtonsInteractable(bool interactable)
    {
        if (googleLoginButton != null)
        {
            googleLoginButton.interactable = interactable;
        }
        
        if (facebookLoginButton != null)
        {
            facebookLoginButton.interactable = interactable;
        }
        
        if (appleLoginButton != null)
        {
            appleLoginButton.interactable = interactable;
        }
        
        if (testAccountLoginButton != null)
        {
            testAccountLoginButton.interactable = interactable;
        }
        
        if (quickTestAccountButton != null)
        {
            quickTestAccountButton.interactable = interactable;
        }
    }
    #endregion
}
