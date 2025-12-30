using UnityEngine;
using System;

/// <summary>
/// 游戏定时器管理器
/// 负责管理游戏的时间系统，核心是5分钟一个周期
/// 在每个周期结算工资、支付费用、产生数据等
/// </summary>
public class GameTimerManager : MonoBehaviour
{
    #region 单例模式
    private static GameTimerManager _instance;
    
    public static GameTimerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameTimerManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameTimerManager");
                    _instance = go.AddComponent<GameTimerManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion
    
    #region 事件定义
    /// <summary>
    /// 游戏周期开始事件
    /// 每5分钟触发一次
    /// </summary>
    public event Action OnGameTickStart;
    
    /// <summary>
    /// 游戏周期结束事件
    /// 在所有结算完成后触发
    /// </summary>
    public event Action OnGameTickEnd;
    
    /// <summary>
    /// 定时器更新事件
    /// 每秒触发一次，用于更新UI显示
    /// </summary>
    public event Action<int> OnTimerUpdate;  // 参数：剩余秒数
    #endregion
    
    #region 配置参数
    [Header("定时器配置")]
    [Tooltip("游戏周期时长（秒）")]
    public float gameTickInterval = 300f;  // 5分钟 = 300秒
    
    [Tooltip("是否启用定时器（用于测试）")]
    public bool timerEnabled = true;
    
    [Tooltip("是否加速时间（用于测试，1倍=正常速度，2倍=2倍速）")]
    [Range(1f, 10f)]
    public float timeScale = 1f;
    
    [Header("调试模式")]
    [Tooltip("是否启用调试模式（使用更短的周期便于测试）")]
    public bool debugMode = false;
    
    [Tooltip("调试模式下的周期时长（秒）")]
    public float debugTickInterval = 30f;  // 30秒
    #endregion
    
    #region 状态变量
    [Header("定时器状态")]
    [Tooltip("当前周期的已用时间")]
    public float currentTick = 0f;
    
    [Tooltip("当前周期数（从游戏开始计算）")]
    public int totalTicks = 0;
    
    [Tooltip("定时器是否暂停")]
    public bool isPaused = false;
    
    // 私有变量
    private float _actualInterval;  // 实际使用的周期时长
    private float _lastSecondUpdate = 0f;  // 上次秒级更新的时间
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
        InitializeTimer();
    }
    
    private void Update()
    {
        if (!timerEnabled || isPaused)
        {
            return;
        }
        
        // 更新计时器
        UpdateTimer();
    }
    #endregion
    
    #region 初始化方法
    /// <summary>
    /// 初始化定时器
    /// </summary>
    private void InitializeTimer()
    {
        Debug.Log("=== 初始化游戏定时器 ===");
        
        // 根据是否调试模式选择周期时长
        _actualInterval = debugMode ? debugTickInterval : gameTickInterval;
        
        Debug.Log($"定时器周期：{_actualInterval} 秒 ({_actualInterval / 60f:F1} 分钟)");
        Debug.Log($"时间缩放：{timeScale}x");
        Debug.Log($"调试模式：{(debugMode ? "开启" : "关闭")}");
        
        // 重置计时器
        currentTick = 0f;
        totalTicks = 0;
        
        Debug.Log("✓ 游戏定时器初始化完成");
    }
    #endregion
    
    #region 定时器更新
    /// <summary>
    /// 更新定时器
    /// 每帧调用
    /// </summary>
    private void UpdateTimer()
    {
        // 应用时间缩放
        float deltaTime = Time.deltaTime * timeScale;
        currentTick += deltaTime;
        
        // 每秒触发一次更新事件（用于UI）
        if (Time.time - _lastSecondUpdate >= 1f)
        {
            _lastSecondUpdate = Time.time;
            int remainingSeconds = GetRemainingSeconds();
            OnTimerUpdate?.Invoke(remainingSeconds);
        }
        
        // 检查是否到达一个周期
        if (currentTick >= _actualInterval)
        {
            ExecuteGameTick();
            currentTick = 0f;  // 重置计时器
        }
    }
    
    /// <summary>
    /// 执行游戏周期结算
    /// 每5分钟（或调试模式下更短）执行一次
    /// </summary>
    private void ExecuteGameTick()
    {
        totalTicks++;
        
        Debug.Log($"=== 第 {totalTicks} 个游戏周期开始 ===");
        
        // 触发周期开始事件
        OnGameTickStart?.Invoke();
        
        // 按顺序执行各项结算
        ExecuteTickOperations();
        
        Debug.Log($"=== 第 {totalTicks} 个游戏周期结束 ===");
        
        // 触发周期结束事件
        OnGameTickEnd?.Invoke();
    }
    
    /// <summary>
    /// 执行周期内的各项操作
    /// 按照游戏设计文档的顺序执行
    /// </summary>
    private void ExecuteTickOperations()
    {
        Debug.Log("→ 开始执行周期操作...");
        
        // 1. 身份类型检查和费用支付
        HandleIdentityFees();
        
        // 2. 工作结算（薪资发放）
        HandleJobSalary();
        
        // 3. 公司结算
        HandleCompanyIncome();
        
        // 4. 房租支付
        HandleRentPayment();
        
        // 5. 安全卫士费用
        HandleSecurityFee();
        
        // 6. 数据产生
        HandleDataGeneration();
        
        // 7. 心情值变化
        HandleMoodChange();
        
        // 8. 病毒入侵检测
        HandleVirusCheck();
        
        // 9. 同步到Firebase（保存数据）
        SyncToFirebase();
        
        Debug.Log("✓ 周期操作执行完成");
    }
    #endregion
    
    #region 周期操作方法
    /// <summary>
    /// 处理身份类型相关的费用
    /// 意识连接者需要支付连接费
    /// </summary>
    private void HandleIdentityFees()
    {
        Debug.Log("→ [1/9] 检查身份类型费用...");
        
        if (ResourceManager.Instance == null)
        {
            return;
        }
        
        // 如果是意识连接者，支付连接费
        bool success = ResourceManager.Instance.PayConnectionFee();
        
        if (!success)
        {
            Debug.LogError("<color=red>连接费支付失败！</color>");
            // TODO: 触发连接中断事件
        }
    }
    
    /// <summary>
    /// 处理工作薪资结算
    /// </summary>
    private void HandleJobSalary()
    {
        Debug.Log("→ [2/9] 结算工作薪资...");
        
        // TODO: 在实现工作系统后，这里会调用JobSystem.Instance.PaySalary()
        // 示例代码：
        // if (JobSystem.Instance != null)
        // {
        //     JobSystem.Instance.PaySalary();
        // }
        
        Debug.Log("  （工作系统尚未实现）");
    }
    
    /// <summary>
    /// 处理公司收入结算
    /// </summary>
    private void HandleCompanyIncome()
    {
        Debug.Log("→ [3/9] 结算公司收入...");
        
        // TODO: 在实现公司系统后，这里会调用CompanySystem
        // 示例代码：
        // if (CompanySystem.Instance != null)
        // {
        //     CompanySystem.Instance.SettleCompanyIncome();
        //     CompanySystem.Instance.PayEmployeeSalaries();
        // }
        
        Debug.Log("  （公司系统尚未实现）");
    }
    
    /// <summary>
    /// 处理房租支付
    /// </summary>
    private void HandleRentPayment()
    {
        Debug.Log("→ [4/9] 支付房租...");
        
        // TODO: 在实现房产系统后，这里会调用HousingSystem
        // 示例代码：
        // if (HousingSystem.Instance != null)
        // {
        //     HousingSystem.Instance.PayRent();
        // }
        
        Debug.Log("  （房产系统尚未实现）");
    }
    
    /// <summary>
    /// 处理安全卫士费用
    /// </summary>
    private void HandleSecurityFee()
    {
        Debug.Log("→ [5/9] 支付安全卫士费用...");
        
        // TODO: 在实现安全系统后，这里会调用SecuritySystem
        // 示例代码：
        // if (SecuritySystem.Instance != null)
        // {
        //     SecuritySystem.Instance.PaySecurityFee();
        // }
        
        Debug.Log("  （安全系统尚未实现）");
    }
    
    /// <summary>
    /// 处理数据产生
    /// 每个周期根据数据产生速率增加存储占用
    /// </summary>
    private void HandleDataGeneration()
    {
        Debug.Log("→ [6/9] 产生数据...");
        
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.GenerateData();
        }
    }
    
    /// <summary>
    /// 处理心情值变化
    /// 工作会降低心情值，房产和宠物会提升心情值
    /// </summary>
    private void HandleMoodChange()
    {
        Debug.Log("→ [7/9] 更新心情值...");
        
        if (ResourceManager.Instance == null)
        {
            return;
        }
        
        // 工作/管理公司会降低心情值
        // TODO: 根据玩家是否在工作决定扣除量
        // 目前暂时扣除2点（假设玩家在工作）
        ResourceManager.Instance.ChangeMoodValue(-2, "工作导致心情下降");
        
        // TODO: 房产和宠物的心情加成会在对应系统实现后添加
    }
    
    /// <summary>
    /// 检查病毒入侵
    /// 随机触发病毒入侵事件
    /// </summary>
    private void HandleVirusCheck()
    {
        Debug.Log("→ [8/9] 检查病毒入侵...");
        
        // TODO: 在实现病毒系统后，这里会调用VirusSystem
        // 示例代码：
        // if (VirusSystem.Instance != null)
        // {
        //     VirusSystem.Instance.CheckVirusInvasion();
        // }
        
        Debug.Log("  （病毒系统尚未实现）");
    }
    
    /// <summary>
    /// 同步数据到Firebase
    /// 保存当前游戏状态
    /// </summary>
    private void SyncToFirebase()
    {
        Debug.Log("→ [9/9] 同步数据到Firebase...");
        
        // 保存资源数据
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.SaveResources();
        }
        
        // TODO: 保存其他系统的数据
        // 示例代码：
        // if (JobSystem.Instance != null) JobSystem.Instance.SaveData();
        // if (CompanySystem.Instance != null) CompanySystem.Instance.SaveData();
        
        Debug.Log("✓ 数据同步完成");
    }
    #endregion
    
    #region 定时器控制方法
    /// <summary>
    /// 暂停定时器
    /// </summary>
    public void PauseTimer()
    {
        isPaused = true;
        Debug.Log("<color=yellow>游戏定时器已暂停</color>");
    }
    
    /// <summary>
    /// 恢复定时器
    /// </summary>
    public void ResumeTimer()
    {
        isPaused = false;
        Debug.Log("<color=green>游戏定时器已恢复</color>");
    }
    
    /// <summary>
    /// 启用/禁用定时器
    /// </summary>
    public void SetTimerEnabled(bool enabled)
    {
        timerEnabled = enabled;
        Debug.Log($"游戏定时器{(enabled ? "启用" : "禁用")}");
    }
    
    /// <summary>
    /// 设置时间缩放
    /// </summary>
    public void SetTimeScale(float scale)
    {
        timeScale = Mathf.Clamp(scale, 1f, 10f);
        Debug.Log($"时间缩放设置为 {timeScale}x");
    }
    
    /// <summary>
    /// 立即触发一次游戏周期（用于测试）
    /// </summary>
    public void TriggerGameTickNow()
    {
        Debug.Log("<color=cyan>手动触发游戏周期</color>");
        ExecuteGameTick();
    }
    
    /// <summary>
    /// 重置定时器
    /// </summary>
    public void ResetTimer()
    {
        currentTick = 0f;
        totalTicks = 0;
        Debug.Log("定时器已重置");
    }
    #endregion
    
    #region 查询方法
    /// <summary>
    /// 获取剩余秒数
    /// </summary>
    public int GetRemainingSeconds()
    {
        return Mathf.CeilToInt(_actualInterval - currentTick);
    }
    
    /// <summary>
    /// 获取剩余时间（格式化字符串）
    /// 例如：03:25
    /// </summary>
    public string GetRemainingTimeFormatted()
    {
        int remainingSeconds = GetRemainingSeconds();
        int minutes = remainingSeconds / 60;
        int seconds = remainingSeconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }
    
    /// <summary>
    /// 获取当前进度（0-1）
    /// </summary>
    public float GetProgress()
    {
        return currentTick / _actualInterval;
    }
    
    /// <summary>
    /// 获取当前进度（百分比）
    /// </summary>
    public float GetProgressPercent()
    {
        return GetProgress() * 100f;
    }
    
    /// <summary>
    /// 获取总周期数
    /// </summary>
    public int GetTotalTicks()
    {
        return totalTicks;
    }
    
    /// <summary>
    /// 获取定时器状态信息
    /// </summary>
    public string GetTimerInfo()
    {
        return $"定时器状态：\n" +
               $"  周期：{_actualInterval}秒 ({_actualInterval / 60f:F1}分钟)\n" +
               $"  总周期数：{totalTicks}\n" +
               $"  当前进度：{GetProgressPercent():F1}%\n" +
               $"  剩余时间：{GetRemainingTimeFormatted()}\n" +
               $"  状态：{(isPaused ? "暂停" : "运行中")}\n" +
               $"  时间缩放：{timeScale}x";
    }
    #endregion
}
