using UnityEngine;
using System;

/*
 * 娱乐活动数据类
 * Entertainment Activity Data Class
 * 
 * 功能说明：
 * 1. 定义娱乐活动的所有属性
 * 2. 提供时间计算方法（包含汽车速度加成）
 * 3. 管理心情值奖励
 * 4. 处理等级解锁和真实货币要求
 * 
 * 使用示例：
 * EntertainmentData activity = EntertainmentData.CreateSciFiWorld();
 * float actualTime = activity.CalculateActualDuration(1.5f); // 使用1.5倍速度加成
 * 
 * 作者：GitHub Copilot
 * 日期：2025-12-31
 */

/// <summary>
/// 娱乐活动数据类
/// 存储单个娱乐活动的所有信息
/// </summary>
[System.Serializable]
public class EntertainmentData
{
    // ==================== 基础信息 ====================
    
    /// <summary>娱乐活动唯一ID</summary>
    public string entertainmentId;
    
    /// <summary>娱乐活动名称</summary>
    public string entertainmentName;
    
    /// <summary>活动描述</summary>
    public string description;
    
    // ==================== 消耗与奖励 ====================
    
    /// <summary>参加活动的虚拟币成本</summary>
    public int cost;
    
    /// <summary>基础持续时间（秒）</summary>
    public float baseDuration;
    
    /// <summary>完成后的心情值奖励</summary>
    public int moodReward;
    
    // ==================== 解锁条件 ====================
    
    /// <summary>解锁所需等级</summary>
    public int unlockLevel;
    
    /// <summary>是否需要真实货币购买</summary>
    public bool requireRealMoney;
    
    /// <summary>需要的真实货币金额（美元）</summary>
    public float realMoneyPrice;
    
    // ==================== 视觉资源 ====================
    
    /// <summary>背景图片路径</summary>
    public string backgroundImage;
    
    /// <summary>图标路径</summary>
    public string iconImage;
    
    // ==================== 构造函数 ====================
    
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public EntertainmentData()
    {
        entertainmentId = "";
        entertainmentName = "";
        description = "";
        cost = 0;
        baseDuration = 600f; // 默认10分钟
        moodReward = 0;
        unlockLevel = 1;
        requireRealMoney = false;
        realMoneyPrice = 0f;
        backgroundImage = "";
        iconImage = "";
    }
    
    /// <summary>
    /// 完整参数构造函数
    /// </summary>
    public EntertainmentData(
        string id,
        string name,
        string desc,
        int cost,
        float duration,
        int mood,
        int level,
        string bgImage,
        string icon,
        bool needMoney = false,
        float price = 0f)
    {
        entertainmentId = id;
        entertainmentName = name;
        description = desc;
        this.cost = cost;
        baseDuration = duration;
        moodReward = mood;
        unlockLevel = level;
        backgroundImage = bgImage;
        iconImage = icon;
        requireRealMoney = needMoney;
        realMoneyPrice = price;
    }
    
    // ==================== 计算方法 ====================
    
    /// <summary>
    /// 计算实际持续时间（考虑汽车速度加成）
    /// </summary>
    /// <param name="speedBonus">汽车速度加成（1.0 = 无加成，1.5 = 1.5倍速度）</param>
    /// <returns>实际需要的时间（秒）</returns>
    public float CalculateActualDuration(float speedBonus)
    {
        // 防止除以0
        if (speedBonus <= 0f)
        {
            Debug.LogWarning($"[EntertainmentData] 无效的速度加成: {speedBonus}，使用默认值1.0");
            speedBonus = 1.0f;
        }
        
        // 实际时间 = 基础时间 / 速度加成
        float actualTime = baseDuration / speedBonus;
        
        return actualTime;
    }
    
    /// <summary>
    /// 计算节省的时间（秒）
    /// </summary>
    /// <param name="speedBonus">汽车速度加成</param>
    /// <returns>节省的时间（秒）</returns>
    public float CalculateTimeSaved(float speedBonus)
    {
        if (speedBonus <= 1.0f)
        {
            return 0f; // 无速度加成，不节省时间
        }
        
        float actualTime = CalculateActualDuration(speedBonus);
        return baseDuration - actualTime;
    }
    
    /// <summary>
    /// 格式化显示持续时间
    /// </summary>
    /// <param name="speedBonus">汽车速度加成</param>
    /// <returns>格式化的时间字符串（如"6分30秒"）</returns>
    public string GetFormattedDuration(float speedBonus)
    {
        float actualTime = CalculateActualDuration(speedBonus);
        int minutes = Mathf.FloorToInt(actualTime / 60f);
        int seconds = Mathf.FloorToInt(actualTime % 60f);
        
        if (minutes > 0)
        {
            return $"{minutes}分{seconds}秒";
        }
        else
        {
            return $"{seconds}秒";
        }
    }
    
    // ==================== 验证方法 ====================
    
    /// <summary>
    /// 检查玩家是否满足解锁条件
    /// </summary>
    /// <param name="playerLevel">玩家当前等级</param>
    /// <param name="virtualCoin">玩家虚拟币</param>
    /// <param name="hasPurchased">是否已购买（针对真实货币内容）</param>
    /// <returns>是否可以参加</returns>
    public bool CanParticipate(int playerLevel, int virtualCoin, bool hasPurchased = false)
    {
        // 检查等级
        if (playerLevel < unlockLevel)
        {
            return false;
        }
        
        // 检查虚拟币
        if (virtualCoin < cost)
        {
            return false;
        }
        
        // 检查真实货币购买
        if (requireRealMoney && !hasPurchased)
        {
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 获取无法参加的原因
    /// </summary>
    public string GetLockReason(int playerLevel, int virtualCoin, bool hasPurchased = false)
    {
        if (playerLevel < unlockLevel)
        {
            return $"需要等级{unlockLevel}";
        }
        
        if (virtualCoin < cost)
        {
            return $"虚拟币不足（需要{cost}币）";
        }
        
        if (requireRealMoney && !hasPurchased)
        {
            return $"需要购买（${realMoneyPrice:F2}）";
        }
        
        return "已解锁";
    }
    
    // ==================== 性价比计算 ====================
    
    /// <summary>
    /// 计算性价比（心情值/虚拟币）
    /// </summary>
    public float GetMoodPerCoin()
    {
        if (cost <= 0)
        {
            return 0f;
        }
        
        return (float)moodReward / cost;
    }
    
    /// <summary>
    /// 计算时间效率（心情值/分钟）
    /// </summary>
    public float GetMoodPerMinute(float speedBonus = 1.0f)
    {
        float actualTime = CalculateActualDuration(speedBonus);
        float minutes = actualTime / 60f;
        
        if (minutes <= 0f)
        {
            return 0f;
        }
        
        return moodReward / minutes;
    }
    
    // ==================== 静态工厂方法 ====================
    
    /// <summary>
    /// 创建"星际战争"娱乐活动
    /// </summary>
    public static EntertainmentData CreateSciFiWorld()
    {
        return new EntertainmentData(
            id: "world_scifi",
            name: "星际战争",
            desc: "体验宇宙战争的刺激",
            cost: 50,
            duration: 600f, // 10分钟
            mood: 30,
            level: 1,
            bgImage: "scifi_world.png",
            icon: "scifi_icon.png"
        );
    }
    
    /// <summary>
    /// 创建"末日求生"娱乐活动
    /// </summary>
    public static EntertainmentData CreateApocalypseWorld()
    {
        return new EntertainmentData(
            id: "world_apocalypse",
            name: "末日求生",
            desc: "在废土上求生",
            cost: 80,
            duration: 600f,
            mood: 40,
            level: 5,
            bgImage: "apocalypse_world.png",
            icon: "apocalypse_icon.png"
        );
    }
    
    /// <summary>
    /// 创建"魔法学院"娱乐活动
    /// </summary>
    public static EntertainmentData CreateMagicWorld()
    {
        return new EntertainmentData(
            id: "world_magic",
            name: "魔法学院",
            desc: "学习魔法，探索奇幻世界",
            cost: 100,
            duration: 600f,
            mood: 50,
            level: 10,
            bgImage: "magic_world.png",
            icon: "magic_icon.png"
        );
    }
    
    /// <summary>
    /// 创建"《三体》世界"娱乐活动（需要真实货币）
    /// </summary>
    public static EntertainmentData CreateThreeBodyWorld()
    {
        return new EntertainmentData(
            id: "world_novel",
            name: "《三体》世界",
            desc: "进入刘慈欣的科幻世界",
            cost: 150,
            duration: 600f,
            mood: 60,
            level: 15,
            bgImage: "threebody_world.png",
            icon: "threebody_icon.png",
            needMoney: true,
            price: 1.99f
        );
    }
    
    // ==================== 调试信息 ====================
    
    /// <summary>
    /// 获取娱乐活动的详细信息（用于调试）
    /// </summary>
    public override string ToString()
    {
        return $"[娱乐活动] {entertainmentName}\n" +
               $"ID: {entertainmentId}\n" +
               $"描述: {description}\n" +
               $"费用: {cost}币\n" +
               $"基础时间: {baseDuration / 60f:F1}分钟\n" +
               $"心情奖励: +{moodReward}\n" +
               $"解锁等级: {unlockLevel}\n" +
               $"性价比: {GetMoodPerCoin():F2}心情/币\n" +
               $"时间效率: {GetMoodPerMinute():F2}心情/分钟\n" +
               $"真实货币: {(requireRealMoney ? $"${realMoneyPrice:F2}" : "否")}";
    }
}
