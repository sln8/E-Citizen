using UnityEngine;

/// <summary>
/// 初始选择管理器
/// 负责管理玩家的初始身份选择流程
/// 在玩家首次登录时显示选择界面
/// 
/// Unity使用说明：
/// 1. 在GameScene中创建空GameObject，命名为"InitialSelectionManager"
/// 2. 将此脚本挂载到该对象上
/// 3. 在场景中创建InitialSelectionUI（参考InitialSelectionUI.cs的说明）
/// 4. 在Inspector中将InitialSelectionUI的Canvas拖入selectionUI字段
/// </summary>
public class InitialSelectionManager : MonoBehaviour
{
    #region 单例模式
    private static InitialSelectionManager _instance;
    
    public static InitialSelectionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InitialSelectionManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("InitialSelectionManager");
                    _instance = go.AddComponent<InitialSelectionManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion
    
    #region UI引用
    [Header("UI引用")]
    [Tooltip("初始选择UI组件")]
    public InitialSelectionUI selectionUI;
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
        // 查找InitialSelectionUI组件
        if (selectionUI == null)
        {
            selectionUI = FindObjectOfType<InitialSelectionUI>(true); // 包括非激活的对象
        }
        
        // 注册事件监听
        if (selectionUI != null)
        {
            selectionUI.OnSelectionCompleted += OnSelectionCompleted;
        }
        else
        {
            Debug.LogWarning("未找到InitialSelectionUI组件！请在场景中创建。");
        }
    }
    
    private void OnDestroy()
    {
        // 取消事件监听
        if (selectionUI != null)
        {
            selectionUI.OnSelectionCompleted -= OnSelectionCompleted;
        }
    }
    #endregion
    
    #region 公共方法
    /// <summary>
    /// 显示初始选择界面
    /// 在玩家首次登录时调用
    /// </summary>
    public void ShowInitialSelection()
    {
        Debug.Log("=== 显示初始选择界面 ===");
        
        if (selectionUI != null)
        {
            selectionUI.Show();
        }
        else
        {
            Debug.LogError("无法显示初始选择界面：selectionUI为null");
            
            // 如果找不到UI，尝试重新查找
            selectionUI = FindObjectOfType<InitialSelectionUI>(true);
            if (selectionUI != null)
            {
                selectionUI.OnSelectionCompleted += OnSelectionCompleted;
                selectionUI.Show();
            }
            else
            {
                Debug.LogError("场景中不存在InitialSelectionUI组件！");
            }
        }
    }
    
    /// <summary>
    /// 隐藏初始选择界面
    /// </summary>
    public void HideInitialSelection()
    {
        if (selectionUI != null)
        {
            selectionUI.Hide();
        }
    }
    #endregion
    
    #region 事件回调
    /// <summary>
    /// 选择完成回调
    /// </summary>
    private void OnSelectionCompleted(IdentityType selectedIdentity)
    {
        Debug.Log($"<color=green>玩家完成初始选择：{selectedIdentity}</color>");
        
        // 通知GameManager完成角色创建
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteCharacterCreation(selectedIdentity);
        }
        else
        {
            Debug.LogError("GameManager未找到！无法完成角色创建流程。");
        }
    }
    #endregion
}
