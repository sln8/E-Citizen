using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能管理器
/// 负责管理游戏中的所有技能相关功能
/// 包括：技能商店、购买技能、下载技能、算力分配、掌握度计算等
/// </summary>
public class SkillManager : MonoBehaviour
{
    #region 单例模式
    private static SkillManager _instance;
    
    public static SkillManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SkillManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SkillManager");
                    _instance = go.AddComponent<SkillManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion
    
    #region 事件定义
    /// <summary>
    /// 技能列表更新事件
    /// 当可用技能列表发生变化时触发
    /// </summary>
    public event Action<List<SkillData>> OnSkillListUpdated;
    
    /// <summary>
    /// 技能购买成功事件
    /// 参数：技能ID
    /// </summary>
    public event Action<string> OnSkillPurchased;
    
    /// <summary>
    /// 技能下载开始事件
    /// 参数：技能ID
    /// </summary>
    public event Action<string> OnSkillDownloadStarted;
    
    /// <summary>
    /// 技能下载进度更新事件
    /// 参数：技能ID, 进度(0-100)
    /// </summary>
    public event Action<string, float> OnSkillDownloadProgress;
    
    /// <summary>
    /// 技能下载完成事件
    /// 参数：技能ID
    /// </summary>
    public event Action<string> OnSkillDownloadCompleted;
    
    /// <summary>
    /// 算力分配更新事件
    /// 当玩家调整算力分配时触发
    /// </summary>
    public event Action OnComputingAllocationUpdated;
    
    /// <summary>
    /// 技能掌握度更新事件
    /// 参数：技能ID, 新的掌握度
    /// </summary>
    public event Action<string, float> OnMasteryUpdated;
    #endregion
    
    #region 配置数据
    [Header("技能数据")]
    [Tooltip("所有可用的技能列表")]
    public List<SkillData> allSkills = new List<SkillData>();
    
    [Tooltip("玩家已拥有的技能")]
    public List<PlayerSkillInstance> playerSkills = new List<PlayerSkillInstance>();
    
    [Header("下载管理")]
    [Tooltip("当前正在下载的技能")]
    private Dictionary<string, Coroutine> downloadingSkills = new Dictionary<string, Coroutine>();
    #endregion
    
    #region Unity生命周期
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
        InitializeSkillSystem();
    }
    #endregion
    
    #region 初始化
    /// <summary>
    /// 初始化技能系统
    /// 加载技能数据
    /// </summary>
    private void InitializeSkillSystem()
    {
        Debug.Log("=== 初始化技能系统 ===");
        
        // 加载技能数据
        LoadSkillData();
        
        Debug.Log($"技能系统初始化完成。共有 {allSkills.Count} 个技能");
    }
    
    /// <summary>
    /// 加载技能数据
    /// 这里先创建一些示例技能，后续可以从配置文件或数据库加载
    /// </summary>
    private void LoadSkillData()
    {
        // 清空现有数据
        allSkills.Clear();
        
        // 创建示例技能数据
        CreateSampleSkills();
        
        Debug.Log($"加载了 {allSkills.Count} 个技能");
        
        // 触发技能列表更新事件
        OnSkillListUpdated?.Invoke(allSkills);
    }
    
    /// <summary>
    /// 创建示例技能数据
    /// 用于测试和演示
    /// </summary>
    private void CreateSampleSkills()
    {
        // 普通技能1：数据清理 Lv.1
        SkillData skill1 = new SkillData
        {
            skillId = "dataClean_lv1",
            skillName = "数据清理 Lv.1",
            skillDescription = "基础的数据清理技能，可以清理简单的冗余数据。",
            skillTier = SkillTier.Common,
            skillLevel = 1,
            price = 50,
            fileSize = 1f,
            maxComputingFor100Percent = 10f,
            maxComputingFor200Percent = 30f,
            unlockLevel = 1,
            prerequisiteSkillId = ""
        };
        allSkills.Add(skill1);
        
        // 普通技能2：数据分析 Lv.1
        SkillData skill2 = new SkillData
        {
            skillId = "dataAnalysis_lv1",
            skillName = "数据分析 Lv.1",
            skillDescription = "分析数据模式，提取有用信息。",
            skillTier = SkillTier.Common,
            skillLevel = 1,
            price = 80,
            fileSize = 1.5f,
            maxComputingFor100Percent = 12f,
            maxComputingFor200Percent = 35f,
            unlockLevel = 3,
            prerequisiteSkillId = ""
        };
        allSkills.Add(skill2);
        
        // 精良技能：AI训练 Lv.1
        SkillData skill3 = new SkillData
        {
            skillId = "aiTraining_lv1",
            skillName = "AI训练 Lv.1",
            skillDescription = "训练基础的人工智能模型。",
            skillTier = SkillTier.Rare,
            skillLevel = 1,
            price = 200,
            fileSize = 3f,
            maxComputingFor100Percent = 20f,
            maxComputingFor200Percent = 60f,
            unlockLevel = 5,
            prerequisiteSkillId = "dataAnalysis_lv1"
        };
        allSkills.Add(skill3);
        
        // 精良技能：虚拟设计 Lv.2
        SkillData skill4 = new SkillData
        {
            skillId = "virtualDesign_lv2",
            skillName = "虚拟设计 Lv.2",
            skillDescription = "设计复杂的虚拟空间和建筑。",
            skillTier = SkillTier.Rare,
            skillLevel = 2,
            price = 300,
            fileSize = 4f,
            maxComputingFor100Percent = 25f,
            maxComputingFor200Percent = 70f,
            unlockLevel = 8,
            prerequisiteSkillId = ""
        };
        allSkills.Add(skill4);
        
        // 史诗技能：3D建模 Lv.2
        SkillData skill5 = new SkillData
        {
            skillId = "3dModeling_lv2",
            skillName = "3D建模 Lv.2",
            skillDescription = "创建高质量的三维模型。",
            skillTier = SkillTier.Epic,
            skillLevel = 2,
            price = 600,
            fileSize = 6f,
            maxComputingFor100Percent = 35f,
            maxComputingFor200Percent = 100f,
            unlockLevel = 10,
            prerequisiteSkillId = "virtualDesign_lv2"
        };
        allSkills.Add(skill5);
        
        // 史诗技能：编程 Lv.1
        SkillData skill6 = new SkillData
        {
            skillId = "programming_lv1",
            skillName = "编程 Lv.1",
            skillDescription = "基础的编程能力，可以编写简单的程序。",
            skillTier = SkillTier.Epic,
            skillLevel = 1,
            price = 500,
            fileSize = 5f,
            maxComputingFor100Percent = 30f,
            maxComputingFor200Percent = 90f,
            unlockLevel = 10,
            prerequisiteSkillId = ""
        };
        allSkills.Add(skill6);
        
        // 传说技能：量子计算 Lv.3
        SkillData skill7 = new SkillData
        {
            skillId = "quantumComputing_lv3",
            skillName = "量子计算 Lv.3",
            skillDescription = "掌握量子计算的高级技术。",
            skillTier = SkillTier.Legendary,
            skillLevel = 3,
            price = 2000,
            fileSize = 15f,
            maxComputingFor100Percent = 60f,
            maxComputingFor200Percent = 180f,
            unlockLevel = 25,
            prerequisiteSkillId = "programming_lv1"
        };
        allSkills.Add(skill7);
    }
    #endregion
    
    #region 技能查询
    /// <summary>
    /// 获取所有可用的技能列表
    /// 根据玩家等级和前置技能过滤
    /// </summary>
    public List<SkillData> GetAvailableSkills()
    {
        if (ResourceManager.Instance == null)
        {
            return new List<SkillData>();
        }
        
        int playerLevel = ResourceManager.Instance.GetLevel();
        List<SkillData> availableSkills = new List<SkillData>();
        
        foreach (SkillData skill in allSkills)
        {
            // 检查等级要求
            if (playerLevel >= skill.unlockLevel)
            {
                // 检查前置技能
                if (string.IsNullOrEmpty(skill.prerequisiteSkillId) || 
                    HasSkill(skill.prerequisiteSkillId))
                {
                    availableSkills.Add(skill);
                }
            }
        }
        
        return availableSkills;
    }
    
    /// <summary>
    /// 根据ID获取技能数据
    /// </summary>
    public SkillData GetSkillById(string skillId)
    {
        foreach (SkillData skill in allSkills)
        {
            if (skill.skillId == skillId)
            {
                return skill;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 检查玩家是否拥有某个技能
    /// </summary>
    public bool HasSkill(string skillId)
    {
        foreach (PlayerSkillInstance skill in playerSkills)
        {
            if (skill.skillId == skillId)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 获取玩家的技能实例
    /// </summary>
    public PlayerSkillInstance GetPlayerSkill(string skillId)
    {
        foreach (PlayerSkillInstance skill in playerSkills)
        {
            if (skill.skillId == skillId)
            {
                return skill;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 获取玩家拥有的所有技能ID
    /// </summary>
    public string[] GetPlayerSkillIds()
    {
        string[] skillIds = new string[playerSkills.Count];
        for (int i = 0; i < playerSkills.Count; i++)
        {
            skillIds[i] = playerSkills[i].skillId;
        }
        return skillIds;
    }
    #endregion
    
    #region 购买和下载技能
    /// <summary>
    /// 购买技能
    /// 检查条件并扣除虚拟币，然后开始下载
    /// </summary>
    public bool PurchaseSkill(string skillId, out string errorMessage)
    {
        errorMessage = "";
        
        // 1. 检查是否已经拥有
        if (HasSkill(skillId))
        {
            errorMessage = "你已经拥有这个技能了！";
            return false;
        }
        
        // 2. 获取技能数据
        SkillData skill = GetSkillById(skillId);
        if (skill == null)
        {
            errorMessage = "技能不存在！";
            return false;
        }
        
        // 3. 检查虚拟币是否足够
        int coins = ResourceManager.Instance.GetVirtualCoin();
        if (coins < skill.price)
        {
            errorMessage = $"虚拟币不足！需要{skill.price}币，当前只有{coins}币";
            return false;
        }
        
        // 4. 检查存储空间是否足够
        float availableStorage = ResourceManager.Instance.GetStorageAvailable();
        if (availableStorage < skill.fileSize)
        {
            errorMessage = $"存储空间不足！需要{skill.fileSize}GB，可用{availableStorage:F1}GB";
            return false;
        }
        
        // 5. 扣除虚拟币
        ResourceManager.Instance.AddVirtualCoin(-skill.price, $"购买技能-{skill.skillName}");
        
        // 6. 占用存储空间
        ResourceManager.Instance.AddStorageUsed(skill.fileSize);
        
        // 7. 创建技能实例（初始状态为下载中）
        long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        PlayerSkillInstance skillInstance = new PlayerSkillInstance(skillId, timestamp);
        playerSkills.Add(skillInstance);
        
        // 8. 开始下载
        StartSkillDownload(skillId);
        
        // 9. 触发事件
        OnSkillPurchased?.Invoke(skillId);
        
        Debug.Log($"<color=green>✓ 购买技能成功：{skill.skillName}，花费{skill.price}币</color>");
        
        return true;
    }
    
    /// <summary>
    /// 开始下载技能
    /// 根据网速计算下载时间，模拟下载过程
    /// </summary>
    private void StartSkillDownload(string skillId)
    {
        SkillData skill = GetSkillById(skillId);
        if (skill == null) return;
        
        // 计算下载时间
        float bandwidth = ResourceManager.Instance.GetBandwidthAvailable();
        float downloadTime = skill.CalculateDownloadTime(bandwidth);
        
        Debug.Log($"开始下载技能：{skill.skillName}，预计{downloadTime:F0}秒");
        
        // 开始下载协程
        Coroutine downloadCoroutine = StartCoroutine(DownloadSkillCoroutine(skillId, downloadTime));
        downloadingSkills[skillId] = downloadCoroutine;
        
        // 触发下载开始事件
        OnSkillDownloadStarted?.Invoke(skillId);
    }
    
    /// <summary>
    /// 下载技能协程
    /// 模拟下载过程，更新进度
    /// </summary>
    private IEnumerator DownloadSkillCoroutine(string skillId, float downloadTime)
    {
        SkillData skill = GetSkillById(skillId);
        if (skill == null) yield break;
        
        float elapsed = 0f;
        
        while (elapsed < downloadTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / downloadTime) * 100f;
            
            // 更新技能状态
            skill.status = SkillStatus.Downloading;
            skill.downloadProgress = progress;
            
            // 触发进度更新事件
            OnSkillDownloadProgress?.Invoke(skillId, progress);
            
            yield return null;
        }
        
        // 下载完成
        skill.status = SkillStatus.Installed;
        skill.downloadProgress = 100f;
        
        // 从下载列表中移除
        downloadingSkills.Remove(skillId);
        
        // 触发下载完成事件
        OnSkillDownloadCompleted?.Invoke(skillId);
        
        Debug.Log($"<color=green>✓ 技能下载完成：{skill.skillName}</color>");
    }
    #endregion
    
    #region 算力分配
    /// <summary>
    /// 为技能分配算力
    /// 算力影响技能的掌握度
    /// </summary>
    public bool AllocateComputing(string skillId, float computing, out string errorMessage)
    {
        errorMessage = "";
        
        // 1. 检查是否拥有该技能
        PlayerSkillInstance skillInstance = GetPlayerSkill(skillId);
        if (skillInstance == null)
        {
            errorMessage = "你还没有这个技能！";
            return false;
        }
        
        // 2. 获取技能数据
        SkillData skillData = GetSkillById(skillId);
        if (skillData == null)
        {
            errorMessage = "技能数据不存在！";
            return false;
        }
        
        // 3. 检查是否有足够的可用算力
        float oldComputing = skillInstance.allocatedComputing;
        float computingChange = computing - oldComputing;
        
        if (computingChange > 0)
        {
            float availableComputing = ResourceManager.Instance.GetComputingAvailable();
            if (availableComputing < computingChange)
            {
                errorMessage = $"算力不足！需要额外{computingChange:F1}，可用{availableComputing:F1}";
                return false;
            }
        }
        
        // 4. 更新算力分配
        // 先释放旧的算力
        if (oldComputing > 0)
        {
            ResourceManager.Instance.ReleaseResources(0, 0, 0, oldComputing);
        }
        
        // 再分配新的算力
        if (computing > 0)
        {
            bool success = ResourceManager.Instance.TryAllocateResources(0, 0, 0, computing);
            if (!success)
            {
                errorMessage = "分配算力失败！";
                // 恢复旧的分配
                if (oldComputing > 0)
                {
                    ResourceManager.Instance.TryAllocateResources(0, 0, 0, oldComputing);
                }
                return false;
            }
        }
        
        // 5. 更新技能实例
        skillInstance.allocatedComputing = computing;
        
        // 6. 重新计算掌握度
        skillInstance.CalculateMastery(skillData);
        
        // 7. 触发事件
        OnComputingAllocationUpdated?.Invoke();
        OnMasteryUpdated?.Invoke(skillId, skillInstance.masteryPercent);
        
        Debug.Log($"算力分配：{skillData.skillName} - {computing:F1} -> 掌握度{skillInstance.masteryPercent:F1}%");
        
        return true;
    }
    
    /// <summary>
    /// 重置所有技能的算力分配
    /// </summary>
    public void ResetAllComputingAllocation()
    {
        foreach (PlayerSkillInstance skill in playerSkills)
        {
            if (skill.allocatedComputing > 0)
            {
                // 释放算力
                ResourceManager.Instance.ReleaseResources(0, 0, 0, skill.allocatedComputing);
                
                // 重置分配
                skill.allocatedComputing = 0f;
                
                // 重新计算掌握度
                SkillData skillData = GetSkillById(skill.skillId);
                if (skillData != null)
                {
                    skill.CalculateMastery(skillData);
                }
            }
        }
        
        OnComputingAllocationUpdated?.Invoke();
        
        Debug.Log("已重置所有技能的算力分配");
    }
    #endregion
    
    #region 数据持久化
    /// <summary>
    /// 保存技能数据
    /// </summary>
    public void SaveSkillData()
    {
        // TODO: 保存到Firebase或本地
        Debug.Log("保存技能数据...");
    }
    
    /// <summary>
    /// 加载技能数据
    /// </summary>
    public void LoadSkillData()
    {
        // TODO: 从Firebase或本地加载
        Debug.Log("加载技能数据...");
    }
    #endregion
}
