using UnityEngine;

/// <summary>
/// 初始选择功能测试脚本
/// 用于在没有UI的情况下测试初始选择逻辑
/// 
/// Unity使用说明：
/// 1. 在场景中创建空GameObject，命名为"InitialSelectionTest"
/// 2. 将此脚本挂载到该对象上
/// 3. 运行游戏，按键盘按键测试功能
/// </summary>
public class InitialSelectionTest : MonoBehaviour
{
    [Header("测试配置")]
    [Tooltip("是否启用测试模式")]
    public bool enableTestMode = true;
    
    [Tooltip("模拟的用户数据")]
    public UserData testUserData;
    
    private void Start()
    {
        if (!enableTestMode)
        {
            return;
        }
        
        Debug.Log("=== 初始选择功能测试模式启动 ===");
        Debug.Log("按键说明：");
        Debug.Log("1 - 测试新用户流程（脑机连接者）");
        Debug.Log("2 - 测试新用户流程（纯虚拟人）");
        Debug.Log("3 - 测试老用户流程");
        Debug.Log("4 - 重置hasCreatedCharacter为false");
        Debug.Log("5 - 显示当前资源配置");
        Debug.Log("6 - 显示当前用户数据");
    }
    
    private void Update()
    {
        if (!enableTestMode)
        {
            return;
        }
        
        // 测试1：新用户 - 脑机连接者
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            TestNewUserFlow(IdentityType.ConsciousnessLinker);
        }
        
        // 测试2：新用户 - 纯虚拟人
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            TestNewUserFlow(IdentityType.FullVirtual);
        }
        
        // 测试3：老用户流程
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            TestExistingUserFlow();
        }
        
        // 测试4：重置
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            ResetUserData();
        }
        
        // 测试5：显示资源配置
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            ShowResourceConfiguration();
        }
        
        // 测试6：显示用户数据
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            ShowUserData();
        }
    }
    
    /// <summary>
    /// 测试新用户流程
    /// </summary>
    private void TestNewUserFlow(IdentityType selectedIdentity)
    {
        Debug.Log($"\n<color=cyan>=== 测试新用户流程：{selectedIdentity} ===</color>");
        
        // 1. 创建新用户数据
        UserData newUser = new UserData
        {
            userId = "test_user_" + Random.Range(10000000, 99999999).ToString(),
            username = "测试用户",
            hasCreatedCharacter = false
        };
        
        Debug.Log($"1. 创建新用户: {newUser.userId}");
        Debug.Log($"   hasCreatedCharacter: {newUser.hasCreatedCharacter}");
        
        // 2. 检查是否为新用户
        bool isNew = !newUser.hasCreatedCharacter;
        Debug.Log($"2. 是否为新用户: {isNew}");
        
        // 3. 模拟选择身份类型
        Debug.Log($"3. 玩家选择身份类型: {selectedIdentity}");
        
        // 4. 初始化资源
        if (ResourceManager.Instance != null)
        {
            Debug.Log("4. 初始化资源管理器...");
            ResourceManager.Instance.SetPlayerIdentity(selectedIdentity);
            
            // 显示资源配置
            PlayerResources resources = ResourceManager.Instance.playerResources;
            Debug.Log($"   内存：{resources.memoryTotal}GB（已用{resources.memoryUsed}GB）");
            Debug.Log($"   CPU：{resources.cpuTotal}核（已用{resources.cpuUsed}核）");
            Debug.Log($"   网速：{resources.bandwidthTotal}Mbps（已用{resources.bandwidthUsed}Mbps）");
            Debug.Log($"   算力：{resources.computingTotal}（已用{resources.computingUsed}）");
            Debug.Log($"   存储：{resources.storageTotal}GB（已用{resources.storageUsed}GB）");
            Debug.Log($"   数据生成速率：{resources.dataGenerationRate}GB/5分钟");
        }
        else
        {
            Debug.LogError("ResourceManager未找到！");
        }
        
        // 5. 标记角色已创建
        newUser.hasCreatedCharacter = true;
        newUser.identityType = (int)selectedIdentity;
        Debug.Log($"5. 标记角色已创建: hasCreatedCharacter = {newUser.hasCreatedCharacter}");
        
        // 6. 保存用户数据（模拟）
        testUserData = newUser;
        Debug.Log("6. 用户数据已保存（测试模式）");
        
        Debug.Log($"<color=green>✓ 新用户流程测试完成！</color>\n");
    }
    
    /// <summary>
    /// 测试老用户流程
    /// </summary>
    private void TestExistingUserFlow()
    {
        Debug.Log($"\n<color=cyan>=== 测试老用户流程 ===</color>");
        
        // 检查是否有测试用户数据
        if (testUserData == null || !testUserData.hasCreatedCharacter)
        {
            Debug.LogWarning("没有找到已创建角色的用户数据！");
            Debug.LogWarning("请先按1或2创建新用户。");
            return;
        }
        
        Debug.Log($"1. 加载用户数据: {testUserData.userId}");
        Debug.Log($"   hasCreatedCharacter: {testUserData.hasCreatedCharacter}");
        Debug.Log($"   identityType: {(IdentityType)testUserData.identityType}");
        
        // 检查是否为新用户
        bool isNew = !testUserData.hasCreatedCharacter;
        Debug.Log($"2. 是否为新用户: {isNew}");
        
        if (isNew)
        {
            Debug.LogError("错误：用户应该已经创建角色！");
        }
        else
        {
            Debug.Log("3. 跳过初始选择，直接进入游戏");
            
            // 加载已保存的资源配置
            if (ResourceManager.Instance != null)
            {
                Debug.Log("4. 加载已保存的资源配置...");
                ShowResourceConfiguration();
            }
            
            Debug.Log($"<color=green>✓ 老用户流程测试完成！</color>\n");
        }
    }
    
    /// <summary>
    /// 重置用户数据
    /// </summary>
    private void ResetUserData()
    {
        Debug.Log($"\n<color=yellow>=== 重置用户数据 ===</color>");
        
        if (testUserData != null)
        {
            testUserData.hasCreatedCharacter = false;
            testUserData.identityType = 0;
            Debug.Log("用户数据已重置");
            Debug.Log($"hasCreatedCharacter: {testUserData.hasCreatedCharacter}");
        }
        else
        {
            Debug.LogWarning("没有用户数据可重置");
        }
        
        Debug.Log("");
    }
    
    /// <summary>
    /// 显示资源配置
    /// </summary>
    private void ShowResourceConfiguration()
    {
        Debug.Log($"\n<color=cyan>=== 当前资源配置 ===</color>");
        
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("ResourceManager未找到！");
            return;
        }
        
        PlayerResources resources = ResourceManager.Instance.playerResources;
        IdentityType identity = ResourceManager.Instance.playerIdentity;
        
        Debug.Log($"身份类型: {identity}");
        Debug.Log($"内存: {resources.memoryTotal}GB（已用{resources.memoryUsed}GB，可用{resources.MemoryAvailable}GB）");
        Debug.Log($"CPU: {resources.cpuTotal}核（已用{resources.cpuUsed}核，可用{resources.CpuAvailable}核）");
        Debug.Log($"网速: {resources.bandwidthTotal}Mbps（已用{resources.bandwidthUsed}Mbps，可用{resources.BandwidthAvailable}Mbps）");
        Debug.Log($"算力: {resources.computingTotal}（已用{resources.computingUsed}，可用{resources.ComputingAvailable}）");
        Debug.Log($"存储: {resources.storageTotal}GB（已用{resources.storageUsed}GB，可用{resources.StorageAvailable}GB）");
        Debug.Log($"虚拟币: {resources.virtualCoin}");
        Debug.Log($"心情值: {resources.moodValue}");
        Debug.Log($"等级: {resources.level}");
        Debug.Log($"数据生成速率: {resources.dataGenerationRate}GB/5分钟");
        Debug.Log($"空闲资源平均: {resources.AverageIdlePercent:F1}%");
        
        Debug.Log("");
    }
    
    /// <summary>
    /// 显示用户数据
    /// </summary>
    private void ShowUserData()
    {
        Debug.Log($"\n<color=cyan>=== 当前用户数据 ===</color>");
        
        if (testUserData == null)
        {
            Debug.LogWarning("没有用户数据");
            return;
        }
        
        Debug.Log($"用户ID: {testUserData.userId}");
        Debug.Log($"用户名: {testUserData.username}");
        Debug.Log($"已创建角色: {testUserData.hasCreatedCharacter}");
        Debug.Log($"身份类型: {(IdentityType)testUserData.identityType}");
        Debug.Log($"等级: {testUserData.level}");
        Debug.Log($"虚拟币: {testUserData.virtualCoin}");
        Debug.Log($"心情值: {testUserData.moodValue}");
        
        Debug.Log("");
    }
    
    private void OnGUI()
    {
        if (!enableTestMode)
        {
            return;
        }
        
        // 显示测试说明
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 16;
        style.normal.textColor = Color.white;
        
        string info = "初始选择功能测试\n\n" +
                     "按键说明：\n" +
                     "1 - 新用户（脑机连接者）\n" +
                     "2 - 新用户（纯虚拟人）\n" +
                     "3 - 老用户流程\n" +
                     "4 - 重置数据\n" +
                     "5 - 显示资源\n" +
                     "6 - 显示用户";
        
        GUI.Box(new Rect(10, 10, 250, 180), info, style);
    }
}
