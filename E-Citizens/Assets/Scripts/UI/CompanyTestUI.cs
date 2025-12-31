using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

/// <summary>
/// 公司系统测试UI
/// Company System Test UI
/// 
/// 功能说明：
/// 1. 测试公司创建
/// 2. 测试AI员工招聘和培训
/// 3. 测试真实玩家简历发布和招聘
/// 4. 测试公司升级
/// 5. 实时显示公司状态
/// 
/// Unity操作步骤：
/// 1. 创建一个Canvas对象（如果还没有）
/// 2. 在Canvas下创建一个Panel，命名为CompanyTestPanel
/// 3. 添加本脚本到CompanyTestPanel
/// 4. 创建以下UI元素并连接引用：
///    - Text (TMP): statusText - 显示状态信息
///    - Button: createCompanyButton - 创建公司按钮
///    - Button: hireAIButton - 招聘AI员工按钮
///    - Button: postResumeButton - 发布简历按钮
///    - Button: viewResumesButton - 查看简历按钮
///    - Button: upgradeCompanyButton - 升级公司按钮
///    - Button: refreshButton - 刷新显示按钮
/// 5. 运行游戏测试
/// </summary>
public class CompanyTestUI : MonoBehaviour
{
    #region UI引用 / UI References
    
    [Header("显示组件")]
    [Tooltip("状态文本（显示公司信息）")]
    public TextMeshProUGUI statusText;
    
    [Header("按钮组件")]
    [Tooltip("创建公司按钮")]
    public Button createCompanyButton;
    
    [Tooltip("招聘AI员工按钮")]
    public Button hireAIButton;
    
    [Tooltip("培训员工按钮")]
    public Button trainEmployeeButton;
    
    [Tooltip("辞退员工按钮")]
    public Button dismissEmployeeButton;
    
    [Tooltip("发布简历按钮")]
    public Button postResumeButton;
    
    [Tooltip("查看简历按钮")]
    public Button viewResumesButton;
    
    [Tooltip("招聘玩家按钮")]
    public Button hirePlayerButton;
    
    [Tooltip("升级公司按钮")]
    public Button upgradeCompanyButton;
    
    [Tooltip("刷新显示按钮")]
    public Button refreshButton;
    
    [Tooltip("添加测试资金按钮")]
    public Button addMoneyButton;
    
    #endregion
    
    #region 私有变量 / Private Variables
    
    /// <summary>当前选中的公司</summary>
    private CompanyData currentCompany;
    
    /// <summary>测试用的公司ID列表</summary>
    private List<string> testCompanyIds = new List<string>();
    
    /// <summary>更新间隔</summary>
    private float updateInterval = 1f;
    
    /// <summary>上次更新时间</summary>
    private float lastUpdateTime;
    
    #endregion
    
    #region Unity生命周期 / Unity Lifecycle
    
    private void Start()
    {
        // 初始化按钮事件
        InitializeButtons();
        
        // 订阅事件
        SubscribeToEvents();
        
        // 初始更新显示
        UpdateDisplay();
        
        Debug.Log("[CompanyTestUI] 初始化完成");
    }
    
    private void Update()
    {
        // 定期更新显示
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateDisplay();
            lastUpdateTime = Time.time;
        }
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件
        UnsubscribeFromEvents();
    }
    
    #endregion
    
    #region 初始化 / Initialization
    
    /// <summary>
    /// 初始化按钮事件
    /// Initialize button events
    /// </summary>
    private void InitializeButtons()
    {
        if (createCompanyButton != null)
            createCompanyButton.onClick.AddListener(OnCreateCompanyClick);
            
        if (hireAIButton != null)
            hireAIButton.onClick.AddListener(OnHireAIClick);
            
        if (trainEmployeeButton != null)
            trainEmployeeButton.onClick.AddListener(OnTrainEmployeeClick);
            
        if (dismissEmployeeButton != null)
            dismissEmployeeButton.onClick.AddListener(OnDismissEmployeeClick);
            
        if (postResumeButton != null)
            postResumeButton.onClick.AddListener(OnPostResumeClick);
            
        if (viewResumesButton != null)
            viewResumesButton.onClick.AddListener(OnViewResumesClick);
            
        if (hirePlayerButton != null)
            hirePlayerButton.onClick.AddListener(OnHirePlayerClick);
            
        if (upgradeCompanyButton != null)
            upgradeCompanyButton.onClick.AddListener(OnUpgradeCompanyClick);
            
        if (refreshButton != null)
            refreshButton.onClick.AddListener(OnRefreshClick);
            
        if (addMoneyButton != null)
            addMoneyButton.onClick.AddListener(OnAddMoneyClick);
    }
    
    /// <summary>
    /// 订阅管理器事件
    /// Subscribe to manager events
    /// </summary>
    private void SubscribeToEvents()
    {
        if (CompanyManager.Instance != null)
        {
            CompanyManager.Instance.OnCompanyCreated += OnCompanyCreated;
            CompanyManager.Instance.OnEmployeeHired += OnEmployeeHired;
            CompanyManager.Instance.OnCompanyUpgraded += OnCompanyUpgraded;
            CompanyManager.Instance.OnIncomeSettled += OnIncomeSettled;
        }
        
        if (TalentMarketManager.Instance != null)
        {
            TalentMarketManager.Instance.OnResumePosted += OnResumePosted;
            TalentMarketManager.Instance.OnResumeHired += OnResumeHired;
        }
    }
    
    /// <summary>
    /// 取消订阅管理器事件
    /// Unsubscribe from manager events
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (CompanyManager.Instance != null)
        {
            CompanyManager.Instance.OnCompanyCreated -= OnCompanyCreated;
            CompanyManager.Instance.OnEmployeeHired -= OnEmployeeHired;
            CompanyManager.Instance.OnCompanyUpgraded -= OnCompanyUpgraded;
            CompanyManager.Instance.OnIncomeSettled -= OnIncomeSettled;
        }
        
        if (TalentMarketManager.Instance != null)
        {
            TalentMarketManager.Instance.OnResumePosted -= OnResumePosted;
            TalentMarketManager.Instance.OnResumeHired -= OnResumeHired;
        }
    }
    
    #endregion
    
    #region 按钮事件 / Button Events
    
    /// <summary>
    /// 创建公司按钮点击
    /// Create company button clicked
    /// </summary>
    private void OnCreateCompanyClick()
    {
        if (CompanyManager.Instance == null)
        {
            Debug.LogError("[CompanyTestUI] CompanyManager未找到");
            return;
        }
        
        // 创建微型公司（需要1000币和等级5）
        string companyName = $"测试公司{testCompanyIds.Count + 1}";
        CompanyData company = CompanyManager.Instance.CreateCompany(companyName, CompanyTier.Small);
        
        if (company != null)
        {
            currentCompany = company;
            testCompanyIds.Add(company.companyId);
            Debug.Log($"[CompanyTestUI] 成功创建公司：{companyName}");
            UpdateDisplay();
        }
        else
        {
            Debug.LogWarning("[CompanyTestUI] 创建公司失败（检查等级和虚拟币）");
        }
    }
    
    /// <summary>
    /// 招聘AI员工按钮点击
    /// Hire AI employee button clicked
    /// </summary>
    private void OnHireAIClick()
    {
        if (currentCompany == null)
        {
            Debug.LogWarning("[CompanyTestUI] 请先创建公司");
            return;
        }
        
        if (CompanyManager.Instance == null)
        {
            Debug.LogError("[CompanyTestUI] CompanyManager未找到");
            return;
        }
        
        // 招聘普通AI员工（需要100币）
        bool success = CompanyManager.Instance.HireAIEmployee(currentCompany.companyId, EmployeeTier.Common);
        
        if (success)
        {
            Debug.Log("[CompanyTestUI] 成功招聘AI员工");
            UpdateDisplay();
        }
        else
        {
            Debug.LogWarning("[CompanyTestUI] 招聘失败（检查虚拟币和员工上限）");
        }
    }
    
    /// <summary>
    /// 培训员工按钮点击
    /// Train employee button clicked
    /// </summary>
    private void OnTrainEmployeeClick()
    {
        if (currentCompany == null || currentCompany.employees.Count == 0)
        {
            Debug.LogWarning("[CompanyTestUI] 公司中没有员工");
            return;
        }
        
        if (CompanyManager.Instance == null)
        {
            Debug.LogError("[CompanyTestUI] CompanyManager未找到");
            return;
        }
        
        // 培训第一个AI员工
        EmployeeData firstAI = currentCompany.employees.Find(e => e.type == EmployeeType.AI);
        if (firstAI != null)
        {
            bool success = CompanyManager.Instance.TrainEmployee(currentCompany.companyId, firstAI.employeeId);
            
            if (success)
            {
                Debug.Log($"[CompanyTestUI] 成功培训员工：{firstAI.employeeName} -> Lv.{firstAI.level}");
                UpdateDisplay();
            }
            else
            {
                Debug.LogWarning("[CompanyTestUI] 培训失败（检查虚拟币或员工等级）");
            }
        }
        else
        {
            Debug.LogWarning("[CompanyTestUI] 没有可培训的AI员工");
        }
    }
    
    /// <summary>
    /// 辞退员工按钮点击
    /// Dismiss employee button clicked
    /// </summary>
    private void OnDismissEmployeeClick()
    {
        if (currentCompany == null || currentCompany.employees.Count == 0)
        {
            Debug.LogWarning("[CompanyTestUI] 公司中没有员工");
            return;
        }
        
        if (CompanyManager.Instance == null)
        {
            Debug.LogError("[CompanyTestUI] CompanyManager未找到");
            return;
        }
        
        // 辞退第一个员工
        EmployeeData firstEmployee = currentCompany.employees[0];
        bool success = CompanyManager.Instance.DismissEmployee(currentCompany.companyId, firstEmployee.employeeId);
        
        if (success)
        {
            Debug.Log($"[CompanyTestUI] 成功辞退员工：{firstEmployee.employeeName}");
            UpdateDisplay();
        }
        else
        {
            Debug.LogWarning("[CompanyTestUI] 辞退失败（检查虚拟币以支付补偿）");
        }
    }
    
    /// <summary>
    /// 发布简历按钮点击
    /// Post resume button clicked
    /// </summary>
    private void OnPostResumeClick()
    {
        if (TalentMarketManager.Instance == null)
        {
            Debug.LogError("[CompanyTestUI] TalentMarketManager未找到");
            return;
        }
        
        // 创建测试资源
        ProvidedResources resources = new ProvidedResources(2, 1, 100, 10);
        float expectedSalary = 50f;
        
        bool success = TalentMarketManager.Instance.PostResume(resources, expectedSalary);
        
        if (success)
        {
            Debug.Log("[CompanyTestUI] 成功发布简历");
            UpdateDisplay();
        }
        else
        {
            Debug.LogWarning("[CompanyTestUI] 发布简历失败");
        }
    }
    
    /// <summary>
    /// 查看简历按钮点击
    /// View resumes button clicked
    /// </summary>
    private void OnViewResumesClick()
    {
        if (TalentMarketManager.Instance == null)
        {
            Debug.LogError("[CompanyTestUI] TalentMarketManager未找到");
            return;
        }
        
        List<ResumeData> resumes = TalentMarketManager.Instance.GetAvailableResumes();
        
        Debug.Log($"[CompanyTestUI] 可用简历数量：{resumes.Count}");
        
        foreach (ResumeData resume in resumes)
        {
            Debug.Log($"  - {resume.GetSummary()}");
        }
        
        UpdateDisplay();
    }
    
    /// <summary>
    /// 招聘玩家按钮点击
    /// Hire player button clicked
    /// </summary>
    private void OnHirePlayerClick()
    {
        if (currentCompany == null)
        {
            Debug.LogWarning("[CompanyTestUI] 请先创建公司");
            return;
        }
        
        if (TalentMarketManager.Instance == null || CompanyManager.Instance == null)
        {
            Debug.LogError("[CompanyTestUI] 管理器未找到");
            return;
        }
        
        // 获取第一个可用简历
        List<ResumeData> resumes = TalentMarketManager.Instance.GetAvailableResumes();
        
        if (resumes.Count == 0)
        {
            Debug.LogWarning("[CompanyTestUI] 没有可用简历");
            return;
        }
        
        ResumeData resume = resumes[0];
        bool success = CompanyManager.Instance.HirePlayerEmployee(currentCompany.companyId, resume);
        
        if (success)
        {
            Debug.Log($"[CompanyTestUI] 成功招聘玩家：{resume.playerName}");
            UpdateDisplay();
        }
        else
        {
            Debug.LogWarning("[CompanyTestUI] 招聘失败");
        }
    }
    
    /// <summary>
    /// 升级公司按钮点击
    /// Upgrade company button clicked
    /// </summary>
    private void OnUpgradeCompanyClick()
    {
        if (currentCompany == null)
        {
            Debug.LogWarning("[CompanyTestUI] 请先创建公司");
            return;
        }
        
        if (CompanyManager.Instance == null)
        {
            Debug.LogError("[CompanyTestUI] CompanyManager未找到");
            return;
        }
        
        bool success = CompanyManager.Instance.UpgradeCompany(currentCompany.companyId);
        
        if (success)
        {
            Debug.Log($"[CompanyTestUI] 成功升级公司到 Lv.{currentCompany.level}");
            UpdateDisplay();
        }
        else
        {
            Debug.LogWarning("[CompanyTestUI] 升级失败（检查收入和员工数要求）");
        }
    }
    
    /// <summary>
    /// 刷新显示按钮点击
    /// Refresh display button clicked
    /// </summary>
    private void OnRefreshClick()
    {
        UpdateDisplay();
    }
    
    /// <summary>
    /// 添加测试资金按钮点击
    /// Add test money button clicked
    /// </summary>
    private void OnAddMoneyClick()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddVirtualCoin(5000);
            Debug.Log("[CompanyTestUI] 添加5000虚拟币");
            UpdateDisplay();
        }
    }
    
    #endregion
    
    #region 事件处理 / Event Handlers
    
    private void OnCompanyCreated(CompanyData company)
    {
        Debug.Log($"[CompanyTestUI] 事件：公司创建 - {company.companyName}");
        UpdateDisplay();
    }
    
    private void OnEmployeeHired(string companyId, EmployeeData employee)
    {
        Debug.Log($"[CompanyTestUI] 事件：员工雇佣 - {employee.employeeName}");
        UpdateDisplay();
    }
    
    private void OnCompanyUpgraded(CompanyData company)
    {
        Debug.Log($"[CompanyTestUI] 事件：公司升级 - {company.companyName} Lv.{company.level}");
        UpdateDisplay();
    }
    
    private void OnIncomeSettled(string companyId, float netProfit)
    {
        Debug.Log($"[CompanyTestUI] 事件：收入结算 - 净利润：{netProfit:F1}币");
        UpdateDisplay();
    }
    
    private void OnResumePosted(ResumeData resume)
    {
        Debug.Log($"[CompanyTestUI] 事件：简历发布 - {resume.playerName}");
        UpdateDisplay();
    }
    
    private void OnResumeHired(ResumeData resume)
    {
        Debug.Log($"[CompanyTestUI] 事件：简历被雇佣 - {resume.playerName}");
        UpdateDisplay();
    }
    
    #endregion
    
    #region 显示更新 / Display Update
    
    /// <summary>
    /// 更新显示
    /// Update display
    /// </summary>
    private void UpdateDisplay()
    {
        if (statusText == null)
            return;
            
        StringBuilder sb = new StringBuilder();
        
        // 玩家信息
        sb.AppendLine("=== 玩家信息 ===");
        if (ResourceManager.Instance != null)
        {
            sb.AppendLine($"虚拟币：{ResourceManager.Instance.GetVirtualCoin():F0}");
            sb.AppendLine($"等级：{ResourceManager.Instance.GetPlayerLevel()}");
        }
        sb.AppendLine();
        
        // 公司信息
        sb.AppendLine("=== 公司信息 ===");
        if (currentCompany != null)
        {
            sb.AppendLine($"名称：{currentCompany.companyName}");
            sb.AppendLine($"品级：{currentCompany.GetTierName()}");
            sb.AppendLine($"等级：Lv.{currentCompany.level}");
            sb.AppendLine($"员工：{currentCompany.employees.Count}/{currentCompany.maxEmployees}");
            sb.AppendLine();
            
            sb.AppendLine("--- 财务信息 ---");
            sb.AppendLine($"基础收入：{currentCompany.baseIncome:F0}币/5分钟");
            sb.AppendLine($"总收入：{currentCompany.totalIncome:F0}币/5分钟");
            sb.AppendLine($"总支出：{currentCompany.totalExpenses:F0}币/5分钟");
            sb.AppendLine($"净利润：{currentCompany.netProfit:F0}币/5分钟");
            sb.AppendLine($"累计收入：{currentCompany.cumulativeIncome:F0}币");
            sb.AppendLine();
            
            sb.AppendLine("--- 员工列表 ---");
            foreach (EmployeeData emp in currentCompany.employees)
            {
                sb.AppendLine($"• {emp.employeeName} ({emp.GetTierName()})");
                sb.AppendLine($"  薪资：{emp.salary:F0}币 | 加成：+{(emp.incomeBonus - 1f) * 100:F1}%");
                if (emp.type == EmployeeType.AI)
                {
                    sb.AppendLine($"  等级：Lv.{emp.level}/{emp.maxLevel}");
                }
            }
            sb.AppendLine();
            
            sb.AppendLine("--- 升级要求 ---");
            sb.AppendLine($"收入要求：{currentCompany.totalIncome:F0}/{currentCompany.requiredIncomeForNextLevel:F0}");
            sb.AppendLine($"员工要求：{currentCompany.employees.Count}/{currentCompany.requiredEmployeesForNextLevel}");
            sb.AppendLine($"可升级：{(currentCompany.CanUpgrade() ? "是" : "否")}");
        }
        else
        {
            sb.AppendLine("（尚未创建公司）");
        }
        sb.AppendLine();
        
        // 人才市场信息
        sb.AppendLine("=== 人才市场 ===");
        if (TalentMarketManager.Instance != null)
        {
            List<ResumeData> resumes = TalentMarketManager.Instance.GetAvailableResumes();
            sb.AppendLine($"可用简历：{resumes.Count}");
            
            ResumeData myResume = TalentMarketManager.Instance.GetMyResume();
            if (myResume != null)
            {
                sb.AppendLine($"我的简历：{myResume.GetStatusName()}");
                if (myResume.status == ResumeStatus.Available)
                {
                    sb.AppendLine($"  薪资：{myResume.expectedSalary}币");
                    sb.AppendLine($"  加成：+{(myResume.incomeBonus - 1f) * 100:F1}%");
                }
            }
        }
        
        statusText.text = sb.ToString();
    }
    
    #endregion
}
