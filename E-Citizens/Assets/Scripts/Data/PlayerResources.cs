using System;
using UnityEngine;

/// <summary>
/// 玩家资源数据类
/// 存储玩家的所有核心资源信息
/// 包括：内存、CPU、网速、算力、存储、心情值等
/// </summary>
[Serializable]
public class PlayerResources
{
    #region 硬件资源
    /// <summary>
    /// 内存资源（Memory）
    /// 单位：GB
    /// 用途：影响可同时运行的程序数量
    /// </summary>
    [Header("内存资源（Memory）- 单位：GB")]
    public float memoryTotal = 16f;        // 总内存
    public float memoryUsed = 2f;          // 已使用内存
    
    /// <summary>
    /// CPU资源
    /// 单位：核心数
    /// 用途：影响处理速度和效率
    /// </summary>
    [Header("CPU资源 - 单位：核心数")]
    public float cpuTotal = 8f;            // 总CPU核心
    public float cpuUsed = 1f;             // 已使用CPU核心
    
    /// <summary>
    /// 网速资源（Bandwidth）
    /// 单位：Mbps
    /// 用途：影响数据传输和通信速度
    /// </summary>
    [Header("网速资源（Bandwidth）- 单位：Mbps")]
    public float bandwidthTotal = 1000f;   // 总网速
    public float bandwidthUsed = 50f;      // 已使用网速
    
    /// <summary>
    /// 算力资源（Computing Power）
    /// 单位：算力点数
    /// 用途：用于学习技能和提升掌握度
    /// </summary>
    [Header("算力资源（Computing）- 单位：点数")]
    public float computingTotal = 100f;    // 总算力
    public float computingUsed = 10f;      // 已使用算力
    
    /// <summary>
    /// 存储资源（Storage）
    /// 单位：GB
    /// 用途：存储数据，每5分钟自动产生数据
    /// </summary>
    [Header("存储资源（Storage）- 单位：GB")]
    public float storageTotal = 500f;      // 总存储空间
    public float storageUsed = 20f;        // 已使用存储空间
    #endregion
    
    #region 软属性
    /// <summary>
    /// 心情值（Mood Value）
    /// 无上下限，影响收入效率
    /// 每100点心情值 = ±1% 收入加成
    /// </summary>
    [Header("心情值 - 无上下限")]
    public int moodValue = 10;
    
    /// <summary>
    /// 玩家等级
    /// 等级越高，解锁更多功能和工作
    /// </summary>
    [Header("玩家等级")]
    public int level = 1;
    
    /// <summary>
    /// 虚拟币数量
    /// 游戏内主要货币
    /// </summary>
    [Header("虚拟币")]
    public int virtualCoin = 100;
    
    /// <summary>
    /// 数据产生速率
    /// 单位：GB/5分钟
    /// 根据身份类型和工作决定
    /// </summary>
    [Header("数据产生速率 - 单位：GB/5分钟")]
    public float dataGenerationRate = 0.5f;
    #endregion
    
    #region 计算属性（只读）
    /// <summary>
    /// 获取可用内存
    /// </summary>
    public float MemoryAvailable => Mathf.Max(0, memoryTotal - memoryUsed);
    
    /// <summary>
    /// 获取可用CPU
    /// </summary>
    public float CpuAvailable => Mathf.Max(0, cpuTotal - cpuUsed);
    
    /// <summary>
    /// 获取可用网速
    /// </summary>
    public float BandwidthAvailable => Mathf.Max(0, bandwidthTotal - bandwidthUsed);
    
    /// <summary>
    /// 获取可用算力
    /// </summary>
    public float ComputingAvailable => Mathf.Max(0, computingTotal - computingUsed);
    
    /// <summary>
    /// 获取可用存储空间
    /// </summary>
    public float StorageAvailable => Mathf.Max(0, storageTotal - storageUsed);
    
    /// <summary>
    /// 获取内存使用率（百分比）
    /// </summary>
    public float MemoryUsagePercent => memoryTotal > 0 ? (memoryUsed / memoryTotal) * 100f : 0f;
    
    /// <summary>
    /// 获取CPU使用率（百分比）
    /// </summary>
    public float CpuUsagePercent => cpuTotal > 0 ? (cpuUsed / cpuTotal) * 100f : 0f;
    
    /// <summary>
    /// 获取网速使用率（百分比）
    /// </summary>
    public float BandwidthUsagePercent => bandwidthTotal > 0 ? (bandwidthUsed / bandwidthTotal) * 100f : 0f;
    
    /// <summary>
    /// 获取算力使用率（百分比）
    /// </summary>
    public float ComputingUsagePercent => computingTotal > 0 ? (computingUsed / computingTotal) * 100f : 0f;
    
    /// <summary>
    /// 获取存储使用率（百分比）
    /// </summary>
    public float StorageUsagePercent => storageTotal > 0 ? (storageUsed / storageTotal) * 100f : 0f;
    
    /// <summary>
    /// 获取空闲资源的平均百分比
    /// 用于计算效率加成
    /// </summary>
    public float AverageIdlePercent
    {
        get
        {
            float memoryIdle = 100f - MemoryUsagePercent;
            float cpuIdle = 100f - CpuUsagePercent;
            float bandwidthIdle = 100f - BandwidthUsagePercent;
            float computingIdle = 100f - ComputingUsagePercent;
            
            return (memoryIdle + cpuIdle + bandwidthIdle + computingIdle) / 4f;
        }
    }
    #endregion
    
    #region 构造函数
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public PlayerResources()
    {
        // 使用默认值（已在字段声明中设置）
    }
    
    /// <summary>
    /// 根据身份类型创建资源配置
    /// </summary>
    /// <param name="identityType">身份类型：0=意识连接者，1=完全虚拟人</param>
    public PlayerResources(int identityType)
    {
        if (identityType == 0)
        {
            // 意识连接者（Consciousness Linker）配置
            memoryTotal = 16f;
            memoryUsed = 2f;
            cpuTotal = 8f;
            cpuUsed = 1f;
            bandwidthTotal = 1000f;
            bandwidthUsed = 50f;
            computingTotal = 100f;
            computingUsed = 10f;
            storageTotal = 500f;
            storageUsed = 20f;
            dataGenerationRate = 0.5f;
        }
        else if (identityType == 1)
        {
            // 完全虚拟人（Full Virtual）配置
            memoryTotal = 16f;
            memoryUsed = 4f;
            cpuTotal = 8f;
            cpuUsed = 2f;
            bandwidthTotal = 1000f;
            bandwidthUsed = 100f;
            computingTotal = 100f;
            computingUsed = 20f;
            storageTotal = 500f;
            storageUsed = 50f;
            dataGenerationRate = 1.2f;
        }
        
        moodValue = 10;
        level = 1;
        virtualCoin = 100;
    }
    #endregion
    
    #region 资源操作方法
    /// <summary>
    /// 尝试分配资源
    /// 检查是否有足够的资源可以分配
    /// </summary>
    /// <returns>如果资源充足返回true，否则返回false</returns>
    public bool TryAllocateResources(float memory, float cpu, float bandwidth, float computing)
    {
        if (MemoryAvailable >= memory && 
            CpuAvailable >= cpu && 
            BandwidthAvailable >= bandwidth && 
            ComputingAvailable >= computing)
        {
            memoryUsed += memory;
            cpuUsed += cpu;
            bandwidthUsed += bandwidth;
            computingUsed += computing;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 释放资源
    /// 将已使用的资源归还到可用资源池
    /// </summary>
    public void ReleaseResources(float memory, float cpu, float bandwidth, float computing)
    {
        memoryUsed = Mathf.Max(0, memoryUsed - memory);
        cpuUsed = Mathf.Max(0, cpuUsed - cpu);
        bandwidthUsed = Mathf.Max(0, bandwidthUsed - bandwidth);
        computingUsed = Mathf.Max(0, computingUsed - computing);
    }
    
    /// <summary>
    /// 升级资源上限
    /// 购买更多的资源容量
    /// </summary>
    public void UpgradeResource(ResourceType type, float amount)
    {
        switch (type)
        {
            case ResourceType.Memory:
                memoryTotal += amount;
                break;
            case ResourceType.CPU:
                cpuTotal += amount;
                break;
            case ResourceType.Bandwidth:
                bandwidthTotal += amount;
                break;
            case ResourceType.Computing:
                computingTotal += amount;
                break;
            case ResourceType.Storage:
                storageTotal += amount;
                break;
        }
    }
    
    /// <summary>
    /// 添加虚拟币
    /// </summary>
    public void AddVirtualCoin(int amount)
    {
        virtualCoin += amount;
        Debug.Log($"<color=green>获得 {amount} 虚拟币，当前余额：{virtualCoin}</color>");
    }
    
    /// <summary>
    /// 扣除虚拟币
    /// </summary>
    /// <returns>如果余额充足返回true，否则返回false</returns>
    public bool TrySpendVirtualCoin(int amount)
    {
        if (virtualCoin >= amount)
        {
            virtualCoin -= amount;
            Debug.Log($"<color=yellow>支付 {amount} 虚拟币，剩余余额：{virtualCoin}</color>");
            return true;
        }
        Debug.LogWarning($"<color=red>虚拟币不足！需要 {amount}，当前只有 {virtualCoin}</color>");
        return false;
    }
    
    /// <summary>
    /// 修改心情值
    /// </summary>
    public void ChangeMoodValue(int amount)
    {
        moodValue += amount;
        Debug.Log($"心情值变化：{(amount > 0 ? "+" : "")}{amount}，当前心情值：{moodValue}");
    }
    
    /// <summary>
    /// 产生数据（每5分钟调用一次）
    /// 根据数据产生速率增加已使用存储空间
    /// </summary>
    /// <returns>如果存储空间不足返回false</returns>
    public bool GenerateData()
    {
        float newDataSize = dataGenerationRate;
        
        if (StorageAvailable >= newDataSize)
        {
            storageUsed += newDataSize;
            Debug.Log($"产生数据 {newDataSize:F2} GB，当前存储使用：{storageUsed:F2}/{storageTotal:F2} GB");
            return true;
        }
        else
        {
            Debug.LogWarning($"<color=red>存储空间不足！需要 {newDataSize:F2} GB，只剩 {StorageAvailable:F2} GB</color>");
            return false;
        }
    }
    
    /// <summary>
    /// 清理数据（使用清理工具）
    /// 减少已使用的存储空间
    /// </summary>
    public void CleanData(float amount)
    {
        storageUsed = Mathf.Max(0, storageUsed - amount);
        Debug.Log($"<color=green>清理数据 {amount:F2} GB，当前存储使用：{storageUsed:F2}/{storageTotal:F2} GB</color>");
    }
    #endregion
    
    #region 辅助方法
    /// <summary>
    /// 获取资源信息的字符串表示
    /// 用于调试和日志输出
    /// </summary>
    public override string ToString()
    {
        return $"PlayerResources[\n" +
               $"  内存: {memoryUsed:F1}/{memoryTotal:F1} GB ({MemoryUsagePercent:F1}%)\n" +
               $"  CPU: {cpuUsed:F1}/{cpuTotal:F1} 核 ({CpuUsagePercent:F1}%)\n" +
               $"  网速: {bandwidthUsed:F0}/{bandwidthTotal:F0} Mbps ({BandwidthUsagePercent:F1}%)\n" +
               $"  算力: {computingUsed:F1}/{computingTotal:F1} ({ComputingUsagePercent:F1}%)\n" +
               $"  存储: {storageUsed:F1}/{storageTotal:F1} GB ({StorageUsagePercent:F1}%)\n" +
               $"  心情值: {moodValue}\n" +
               $"  等级: {level}\n" +
               $"  虚拟币: {virtualCoin}\n" +
               $"]";
    }
    
    /// <summary>
    /// 检查存储空间是否即将满（超过80%）
    /// </summary>
    public bool IsStorageNearlyFull()
    {
        return StorageUsagePercent >= 80f;
    }
    
    /// <summary>
    /// 检查存储空间是否已满（超过95%）
    /// </summary>
    public bool IsStorageFull()
    {
        return StorageUsagePercent >= 95f;
    }
    #endregion
}

/// <summary>
/// 资源类型枚举
/// 定义游戏中的各种资源类型
/// </summary>
public enum ResourceType
{
    Memory,      // 内存
    CPU,         // CPU
    Bandwidth,   // 网速
    Computing,   // 算力
    Storage      // 存储
}

/// <summary>
/// 身份类型枚举
/// 定义玩家可选择的身份类型
/// </summary>
public enum IdentityType
{
    /// <summary>
    /// 意识连接者
    /// 优势：较低的初始资源占用，每5分钟产生数据较少
    /// 劣势：需要每5分钟支付连接费（5-10虚拟币）
    /// </summary>
    ConsciousnessLinker = 0,
    
    /// <summary>
    /// 完全虚拟人
    /// 优势：无需支付连接费，完全自由的虚拟生活
    /// 劣势：较高的初始资源占用，每5分钟产生数据较多
    /// </summary>
    FullVirtual = 1
}
