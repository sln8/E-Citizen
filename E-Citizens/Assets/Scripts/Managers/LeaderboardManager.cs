using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 排行榜管理器
/// 负责管理游戏的排行榜系统
/// 
/// 核心功能：
/// 1. 财富榜（虚拟币排名）
/// 2. 等级榜（玩家等级排名）
/// 3. 心情榜（心情值排名）
/// 4. 在线时长榜（累计在线时间排名）
/// 5. 排名奖励发放
/// 6. 排名变化追踪
/// 
/// Unity操作步骤：
/// 1. 在Hierarchy中创建空物体，命名为"LeaderboardManager"
/// 2. 添加此脚本到该物体上
/// 3. 脚本会自动初始化为单例
/// 4. 在游戏开始时调用Initialize()方法
/// 
/// 使用示例：
/// LeaderboardManager.Instance.UpdatePlayerRanking();
/// List<LeaderboardEntryData> topPlayers = LeaderboardManager.Instance.GetTopPlayers(LeaderboardType.Wealth, 10);
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    #region 单例模式
    
    private static LeaderboardManager instance;
    public static LeaderboardManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("LeaderboardManager");
                instance = go.AddComponent<LeaderboardManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    #endregion
    
    #region 数据存储
    
    /// <summary>
    /// 财富榜数据
    /// </summary>
    private List<LeaderboardEntryData> wealthLeaderboard = new List<LeaderboardEntryData>();
    
    /// <summary>
    /// 等级榜数据
    /// </summary>
    private List<LeaderboardEntryData> levelLeaderboard = new List<LeaderboardEntryData>();
    
    /// <summary>
    /// 心情榜数据
    /// </summary>
    private List<LeaderboardEntryData> moodLeaderboard = new List<LeaderboardEntryData>();
    
    /// <summary>
    /// 在线时长榜数据
    /// </summary>
    private List<LeaderboardEntryData> onlineTimeLeaderboard = new List<LeaderboardEntryData>();
    
    /// <summary>
    /// 排行榜配置
    /// </summary>
    private LeaderboardConfig[] leaderboardConfigs;
    
    /// <summary>
    /// 当前玩家的排名缓存
    /// </summary>
    private Dictionary<LeaderboardType, int> playerRankings = new Dictionary<LeaderboardType, int>();
    
    #endregion
    
    #region 配置
    
    /// <summary>
    /// 排行榜最大显示数量
    /// </summary>
    [SerializeField]
    private int maxLeaderboardSize = 100;
    
    /// <summary>
    /// 排行榜更新间隔（秒）
    /// </summary>
    [SerializeField]
    private float updateInterval = 300f; // 5分钟
    
    /// <summary>
    /// 下次更新时间
    /// </summary>
    private float nextUpdateTime;
    
    /// <summary>
    /// 是否启用调试模式
    /// </summary>
    [SerializeField]
    private bool debugMode = false;
    
    #endregion
    
    #region 统计数据
    
    /// <summary>
    /// 统计：排行榜更新次数
    /// </summary>
    public int TotalUpdates { get; private set; }
    
    /// <summary>
    /// 统计：发放的周奖励总数
    /// </summary>
    public int TotalWeeklyRewards { get; private set; }
    
    /// <summary>
    /// 统计：玩家最高排名
    /// </summary>
    public int BestRankEver { get; private set; } = 999999;
    
    #endregion
    
    #region 事件
    
    /// <summary>
    /// 事件：排行榜更新完成
    /// </summary>
    public event Action OnLeaderboardUpdated;
    
    /// <summary>
    /// 事件：玩家排名上升（排行榜类型，新排名，旧排名）
    /// </summary>
    public event Action<LeaderboardType, int, int> OnRankImproved;
    
    /// <summary>
    /// 事件：玩家进入前10（排行榜类型，新排名）
    /// </summary>
    public event Action<LeaderboardType, int> OnEnteredTop10;
    
    /// <summary>
    /// 事件：获得周奖励（虚拟币数量）
    /// </summary>
    public event Action<int> OnWeeklyRewardReceived;
    
    #endregion
    
    #region Unity生命周期
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Update()
    {
        // 定期更新排行榜
        if (Time.time >= nextUpdateTime)
        {
            UpdatePlayerRanking();
            nextUpdateTime = Time.time + updateInterval;
        }
    }
    
    #endregion
    
    #region 初始化
    
    /// <summary>
    /// 初始化排行榜系统
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[LeaderboardManager] 初始化排行榜系统");
        
        // 加载排行榜配置
        leaderboardConfigs = LeaderboardConfig.CreateDefaultConfigs();
        
        // 初始化排名字典
        playerRankings[LeaderboardType.Wealth] = 0;
        playerRankings[LeaderboardType.Level] = 0;
        playerRankings[LeaderboardType.Mood] = 0;
        playerRankings[LeaderboardType.OnlineTime] = 0;
        
        // 设置首次更新时间
        nextUpdateTime = Time.time + 10f; // 10秒后首次更新
        
        // 加载排行榜数据
        LoadData();
        
        Debug.Log($"[LeaderboardManager] 初始化完成");
    }
    
    #endregion
    
    #region 排行榜更新
    
    /// <summary>
    /// 更新玩家在排行榜中的排名
    /// </summary>
    public void UpdatePlayerRanking()
    {
        if (UserData.Instance == null || ResourceManager.Instance == null)
        {
            Debug.LogWarning("[LeaderboardManager] 用户数据或资源管理器未初始化");
            return;
        }
        
        // 创建或更新当前玩家的条目
        LeaderboardEntryData playerEntry = new LeaderboardEntryData(
            UserData.Instance.userId,
            UserData.Instance.playerName,
            UserData.Instance.level
        );
        
        playerEntry.totalWealth = ResourceManager.Instance.GetVirtualCoin();
        playerEntry.level = UserData.Instance.level;
        playerEntry.moodValue = ResourceManager.Instance.GetMoodValue();
        playerEntry.totalOnlineMinutes = UserData.Instance.totalOnlineTime; // 假设有这个字段
        playerEntry.identityType = UserData.Instance.identityType;
        playerEntry.isCurrentPlayer = true;
        
        // 更新各个排行榜
        UpdateLeaderboard(LeaderboardType.Wealth, playerEntry);
        UpdateLeaderboard(LeaderboardType.Level, playerEntry);
        UpdateLeaderboard(LeaderboardType.Mood, playerEntry);
        UpdateLeaderboard(LeaderboardType.OnlineTime, playerEntry);
        
        TotalUpdates++;
        
        // TODO: 同步到Firebase
        // FirebaseManager.UpdateLeaderboard(playerEntry);
        
        OnLeaderboardUpdated?.Invoke();
        
        if (debugMode)
        {
            Debug.Log($"[LeaderboardManager] 更新排名完成\n{GetPlayerRankingSummary()}");
        }
    }
    
    /// <summary>
    /// 更新指定类型的排行榜
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <param name="playerEntry">玩家条目</param>
    private void UpdateLeaderboard(LeaderboardType type, LeaderboardEntryData playerEntry)
    {
        List<LeaderboardEntryData> leaderboard = GetLeaderboardList(type);
        
        // 移除旧的玩家条目
        leaderboard.RemoveAll(e => e.userId == playerEntry.userId);
        
        // 添加新条目
        leaderboard.Add(playerEntry);
        
        // 排序
        SortLeaderboard(type, leaderboard);
        
        // 限制数量
        if (leaderboard.Count > maxLeaderboardSize)
        {
            leaderboard.RemoveRange(maxLeaderboardSize, leaderboard.Count - maxLeaderboardSize);
        }
        
        // 更新排名
        for (int i = 0; i < leaderboard.Count; i++)
        {
            int oldRank = leaderboard[i].currentRank;
            leaderboard[i].UpdateRank(i + 1);
            
            // 检查是否是当前玩家
            if (leaderboard[i].userId == UserData.Instance.userId)
            {
                int newRank = i + 1;
                int previousRank = playerRankings.ContainsKey(type) ? playerRankings[type] : 0;
                playerRankings[type] = newRank;
                
                // 检查排名提升
                if (previousRank > 0 && newRank < previousRank)
                {
                    OnRankImproved?.Invoke(type, newRank, previousRank);
                }
                
                // 检查是否进入前10
                if (previousRank > 10 && newRank <= 10)
                {
                    OnEnteredTop10?.Invoke(type, newRank);
                }
                
                // 更新最高排名
                if (newRank < BestRankEver)
                {
                    BestRankEver = newRank;
                }
            }
        }
    }
    
    /// <summary>
    /// 排序排行榜
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <param name="leaderboard">排行榜列表</param>
    private void SortLeaderboard(LeaderboardType type, List<LeaderboardEntryData> leaderboard)
    {
        switch (type)
        {
            case LeaderboardType.Wealth:
                leaderboard.Sort((a, b) => b.totalWealth.CompareTo(a.totalWealth));
                break;
                
            case LeaderboardType.Level:
                leaderboard.Sort((a, b) => b.level.CompareTo(a.level));
                break;
                
            case LeaderboardType.Mood:
                leaderboard.Sort((a, b) => b.moodValue.CompareTo(a.moodValue));
                break;
                
            case LeaderboardType.OnlineTime:
                leaderboard.Sort((a, b) => b.totalOnlineMinutes.CompareTo(a.totalOnlineMinutes));
                break;
        }
    }
    
    /// <summary>
    /// 获取指定类型的排行榜列表
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <returns>排行榜列表</returns>
    private List<LeaderboardEntryData> GetLeaderboardList(LeaderboardType type)
    {
        switch (type)
        {
            case LeaderboardType.Wealth:
                return wealthLeaderboard;
            case LeaderboardType.Level:
                return levelLeaderboard;
            case LeaderboardType.Mood:
                return moodLeaderboard;
            case LeaderboardType.OnlineTime:
                return onlineTimeLeaderboard;
            default:
                return wealthLeaderboard;
        }
    }
    
    #endregion
    
    #region 排行榜查询
    
    /// <summary>
    /// 获取前N名玩家
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <param name="count">数量</param>
    /// <returns>玩家列表</returns>
    public List<LeaderboardEntryData> GetTopPlayers(LeaderboardType type, int count)
    {
        List<LeaderboardEntryData> leaderboard = GetLeaderboardList(type);
        return leaderboard.Take(count).ToList();
    }
    
    /// <summary>
    /// 获取玩家排名
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <returns>排名（0表示不在榜）</returns>
    public int GetPlayerRank(LeaderboardType type)
    {
        if (playerRankings.ContainsKey(type))
        {
            return playerRankings[type];
        }
        return 0;
    }
    
    /// <summary>
    /// 获取玩家在排行榜中的条目
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <returns>条目数据，不在榜返回null</returns>
    public LeaderboardEntryData GetPlayerEntry(LeaderboardType type)
    {
        List<LeaderboardEntryData> leaderboard = GetLeaderboardList(type);
        return leaderboard.FirstOrDefault(e => e.userId == UserData.Instance.userId);
    }
    
    /// <summary>
    /// 获取排行榜配置
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <returns>配置数据</returns>
    public LeaderboardConfig GetConfig(LeaderboardType type)
    {
        return leaderboardConfigs.FirstOrDefault(c => c.leaderboardType == type);
    }
    
    /// <summary>
    /// 获取玩家周围的排名（显示前后几名）
    /// </summary>
    /// <param name="type">排行榜类型</param>
    /// <param name="range">前后范围</param>
    /// <returns>玩家列表</returns>
    public List<LeaderboardEntryData> GetPlayersAroundPlayer(LeaderboardType type, int range = 2)
    {
        List<LeaderboardEntryData> leaderboard = GetLeaderboardList(type);
        int playerIndex = leaderboard.FindIndex(e => e.userId == UserData.Instance.userId);
        
        if (playerIndex < 0)
        {
            return new List<LeaderboardEntryData>();
        }
        
        int startIndex = Mathf.Max(0, playerIndex - range);
        int endIndex = Mathf.Min(leaderboard.Count - 1, playerIndex + range);
        int count = endIndex - startIndex + 1;
        
        return leaderboard.GetRange(startIndex, count);
    }
    
    #endregion
    
    #region 奖励系统
    
    /// <summary>
    /// 发放周奖励（每周一调用）
    /// </summary>
    public void DistributeWeeklyRewards()
    {
        Debug.Log("[LeaderboardManager] 开始发放周奖励");
        
        // 财富榜奖励
        LeaderboardConfig wealthConfig = GetConfig(LeaderboardType.Wealth);
        if (wealthConfig.hasWeeklyReward)
        {
            DistributeRewardForLeaderboard(LeaderboardType.Wealth);
        }
        
        Debug.Log("[LeaderboardManager] 周奖励发放完成");
    }
    
    /// <summary>
    /// 为指定排行榜发放奖励
    /// </summary>
    /// <param name="type">排行榜类型</param>
    private void DistributeRewardForLeaderboard(LeaderboardType type)
    {
        int playerRank = GetPlayerRank(type);
        
        if (playerRank == 0 || playerRank > 10)
        {
            return; // 不在前10名，无奖励
        }
        
        // 根据排名计算奖励
        int rewardAmount = CalculateWeeklyReward(playerRank);
        
        if (rewardAmount > 0)
        {
            // 通过邮件发放奖励
            if (MailManager.Instance != null)
            {
                MailData mail = MailData.CreateSystemMail(
                    "排行榜周奖励",
                    $"恭喜你在{GetConfig(type).displayName}中排名第{playerRank}！\n" +
                    $"获得周奖励：{rewardAmount}虚拟币",
                    UserData.Instance.userId
                );
                mail.attachedVirtualCoin = rewardAmount;
                MailManager.Instance.ReceiveMail(mail);
            }
            else
            {
                // 直接发放
                ResourceManager.Instance.AddVirtualCoin(rewardAmount);
            }
            
            TotalWeeklyRewards++;
            OnWeeklyRewardReceived?.Invoke(rewardAmount);
            
            Debug.Log($"[LeaderboardManager] 发放周奖励：{rewardAmount}币（排名#{playerRank}）");
        }
    }
    
    /// <summary>
    /// 计算周奖励金额
    /// </summary>
    /// <param name="rank">排名</param>
    /// <returns>奖励金额</returns>
    private int CalculateWeeklyReward(int rank)
    {
        switch (rank)
        {
            case 1: return 1000;
            case 2: return 800;
            case 3: return 600;
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
                return 400;
            default:
                return 0;
        }
    }
    
    #endregion
    
    #region 数据持久化
    
    /// <summary>
    /// 保存数据
    /// </summary>
    public void SaveData()
    {
        // TODO: 实现保存到Firebase
        Debug.Log("[LeaderboardManager] 保存排行榜数据");
    }
    
    /// <summary>
    /// 加载数据
    /// </summary>
    public void LoadData()
    {
        // TODO: 实现从Firebase加载
        Debug.Log("[LeaderboardManager] 加载排行榜数据");
        
        // 调试模式：创建一些测试数据
        if (debugMode)
        {
            CreateTestLeaderboard();
        }
    }
    
    /// <summary>
    /// 创建测试排行榜（仅调试模式）
    /// </summary>
    private void CreateTestLeaderboard()
    {
        for (int i = 0; i < 20; i++)
        {
            LeaderboardEntryData entry = new LeaderboardEntryData(
                $"test_{i}",
                $"测试玩家{i + 1}",
                UnityEngine.Random.Range(1, 100)
            );
            
            entry.totalWealth = UnityEngine.Random.Range(1000, 100000);
            entry.level = UnityEngine.Random.Range(1, 100);
            entry.moodValue = UnityEngine.Random.Range(-100, 500);
            entry.totalOnlineMinutes = UnityEngine.Random.Range(60, 10000);
            
            wealthLeaderboard.Add(entry);
            levelLeaderboard.Add(entry);
            moodLeaderboard.Add(entry);
            onlineTimeLeaderboard.Add(entry);
        }
        
        // 排序
        SortLeaderboard(LeaderboardType.Wealth, wealthLeaderboard);
        SortLeaderboard(LeaderboardType.Level, levelLeaderboard);
        SortLeaderboard(LeaderboardType.Mood, moodLeaderboard);
        SortLeaderboard(LeaderboardType.OnlineTime, onlineTimeLeaderboard);
        
        Debug.Log("[LeaderboardManager] 创建了测试排行榜数据");
    }
    
    #endregion
    
    #region 调试功能
    
    /// <summary>
    /// 强制更新排行榜（仅调试）
    /// </summary>
    public void DebugForceUpdate()
    {
        if (!debugMode) return;
        UpdatePlayerRanking();
    }
    
    /// <summary>
    /// 模拟周奖励发放（仅调试）
    /// </summary>
    public void DebugDistributeWeeklyRewards()
    {
        if (!debugMode) return;
        DistributeWeeklyRewards();
    }
    
    /// <summary>
    /// 获取玩家排名摘要
    /// </summary>
    /// <returns>排名文本</returns>
    public string GetPlayerRankingSummary()
    {
        return $"玩家排名:\n" +
               $"- 财富榜: #{GetPlayerRank(LeaderboardType.Wealth)}\n" +
               $"- 等级榜: #{GetPlayerRank(LeaderboardType.Level)}\n" +
               $"- 心情榜: #{GetPlayerRank(LeaderboardType.Mood)}\n" +
               $"- 在线榜: #{GetPlayerRank(LeaderboardType.OnlineTime)}\n" +
               $"- 最高排名: #{BestRankEver}";
    }
    
    /// <summary>
    /// 获取系统状态摘要（调试用）
    /// </summary>
    /// <returns>状态文本</returns>
    public string GetStatusSummary()
    {
        return $"排行榜系统状态:\n" +
               $"- 财富榜人数: {wealthLeaderboard.Count}\n" +
               $"- 等级榜人数: {levelLeaderboard.Count}\n" +
               $"- 心情榜人数: {moodLeaderboard.Count}\n" +
               $"- 在线榜人数: {onlineTimeLeaderboard.Count}\n" +
               $"- 更新次数: {TotalUpdates}\n" +
               $"- 周奖励: {TotalWeeklyRewards}次\n" +
               $"{GetPlayerRankingSummary()}";
    }
    
    #endregion
}
