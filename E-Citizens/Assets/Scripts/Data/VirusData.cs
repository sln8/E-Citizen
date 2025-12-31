using UnityEngine;

/*
 * 病毒数据类
 * Virus Data Class
 * 
 * 功能说明：
 * 1. 定义病毒的所有属性（类型、血量、速度等）
 * 2. 提供病毒生成的工厂方法
 * 3. 支持三种病毒类型：普通、精英、BOSS
 * 
 * 使用示例：
 * VirusData virus = VirusData.CreateNormalVirus();
 * virus.TakeDamage(5); // 造成5点伤害
 * 
 * 作者：GitHub Copilot
 * 日期：2025-12-31
 */

/// <summary>
/// 病毒类型枚举
/// </summary>
public enum VirusType
{
    Normal,     // 普通病毒
    Elite,      // 精英病毒
    Boss        // BOSS病毒
}

/// <summary>
/// 病毒数据类
/// 存储单个病毒的所有信息和状态
/// </summary>
[System.Serializable]
public class VirusData
{
    // ==================== 基础信息 ====================
    
    /// <summary>病毒唯一ID（运行时生成）</summary>
    public string virusId;
    
    /// <summary>病毒类型</summary>
    public VirusType virusType;
    
    /// <summary>病毒显示的数字</summary>
    public int displayNumber;
    
    // ==================== 战斗属性 ====================
    
    /// <summary>最大血量</summary>
    public int maxHealth;
    
    /// <summary>当前血量</summary>
    public int currentHealth;
    
    /// <summary>对城墙造成的伤害</summary>
    public int damage;
    
    /// <summary>移动速度（单位/秒）</summary>
    public float moveSpeed;
    
    // ==================== 奖励 ====================
    
    /// <summary>击杀奖励（游戏内金币，非虚拟币）</summary>
    public int killReward;
    
    /// <summary>完成后的虚拟币奖励倍数</summary>
    public float coinMultiplier;
    
    // ==================== 运行时状态 ====================
    
    /// <summary>当前位置（Y坐标）</summary>
    public float currentPosition;
    
    /// <summary>是否已死亡</summary>
    public bool isDead;
    
    // ==================== 构造函数 ====================
    
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public VirusData()
    {
        virusId = System.Guid.NewGuid().ToString();
        virusType = VirusType.Normal;
        displayNumber = 1;
        maxHealth = 1;
        currentHealth = 1;
        damage = 1;
        moveSpeed = 1f;
        killReward = 5;
        coinMultiplier = 1f;
        currentPosition = 0f;
        isDead = false;
    }
    
    /// <summary>
    /// 完整参数构造函数
    /// </summary>
    public VirusData(
        VirusType type,
        int number,
        int health,
        int damage,
        float speed,
        int reward,
        float multiplier = 1f)
    {
        virusId = System.Guid.NewGuid().ToString();
        virusType = type;
        displayNumber = number;
        maxHealth = health;
        currentHealth = health;
        this.damage = damage;
        moveSpeed = speed;
        killReward = reward;
        coinMultiplier = multiplier;
        currentPosition = 0f;
        isDead = false;
    }
    
    // ==================== 战斗方法 ====================
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damageAmount">伤害值</param>
    /// <returns>是否被击杀</returns>
    public bool TakeDamage(int damageAmount)
    {
        if (isDead)
        {
            return true;
        }
        
        currentHealth -= damageAmount;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 移动病毒
    /// </summary>
    /// <param name="deltaTime">时间增量</param>
    public void Move(float deltaTime)
    {
        if (isDead)
        {
            return;
        }
        
        currentPosition += moveSpeed * deltaTime;
    }
    
    /// <summary>
    /// 检查是否到达城墙（position >= 目标位置）
    /// </summary>
    public bool HasReachedWall(float wallPosition)
    {
        return currentPosition >= wallPosition;
    }
    
    /// <summary>
    /// 获取血量百分比（0-1）
    /// </summary>
    public float GetHealthPercentage()
    {
        if (maxHealth <= 0)
        {
            return 0f;
        }
        
        return (float)currentHealth / maxHealth;
    }
    
    // ==================== 静态工厂方法 ====================
    
    /// <summary>
    /// 创建普通病毒
    /// 数字1-5，移动速度慢
    /// </summary>
    public static VirusData CreateNormalVirus()
    {
        int number = Random.Range(1, 6); // 1-5
        return new VirusData(
            type: VirusType.Normal,
            number: number,
            health: number,
            damage: number,
            speed: 50f, // 慢速
            reward: number * 5,
            multiplier: 1f
        );
    }
    
    /// <summary>
    /// 创建精英病毒
    /// 数字6-15，移动速度中
    /// </summary>
    public static VirusData CreateEliteVirus()
    {
        int number = Random.Range(6, 16); // 6-15
        return new VirusData(
            type: VirusType.Elite,
            number: number,
            health: number,
            damage: number,
            speed: 80f, // 中速
            reward: number * 5,
            multiplier: 1.5f
        );
    }
    
    /// <summary>
    /// 创建BOSS病毒
    /// 数字20-50，移动速度慢但血量高
    /// </summary>
    public static VirusData CreateBossVirus()
    {
        int number = Random.Range(20, 51); // 20-50
        return new VirusData(
            type: VirusType.Boss,
            number: number,
            health: number,
            damage: number,
            speed: 40f, // 慢速但血厚
            reward: number * 10, // BOSS奖励更高
            multiplier: 2f
        );
    }
    
    /// <summary>
    /// 根据游戏时间创建随机病毒
    /// </summary>
    /// <param name="gameTime">游戏进行时间（秒）</param>
    public static VirusData CreateRandomVirus(float gameTime)
    {
        // 根据时间调整病毒类型概率
        float roll = Random.value;
        
        // 前30秒：只有普通病毒
        if (gameTime < 30f)
        {
            return CreateNormalVirus();
        }
        // 30-60秒：普通60%，精英40%
        else if (gameTime < 60f)
        {
            return roll < 0.6f ? CreateNormalVirus() : CreateEliteVirus();
        }
        // 60-90秒：普通40%，精英50%，BOSS10%
        else if (gameTime < 90f)
        {
            if (roll < 0.4f)
                return CreateNormalVirus();
            else if (roll < 0.9f)
                return CreateEliteVirus();
            else
                return CreateBossVirus();
        }
        // 90秒后：普通30%，精英50%，BOSS20%
        else
        {
            if (roll < 0.3f)
                return CreateNormalVirus();
            else if (roll < 0.8f)
                return CreateEliteVirus();
            else
                return CreateBossVirus();
        }
    }
    
    // ==================== 视觉辅助 ====================
    
    /// <summary>
    /// 获取病毒颜色（用于UI显示）
    /// </summary>
    public Color GetVirusColor()
    {
        switch (virusType)
        {
            case VirusType.Normal:
                return Color.white;
            case VirusType.Elite:
                return new Color(1f, 0.5f, 0f); // 橙色
            case VirusType.Boss:
                return Color.red;
            default:
                return Color.white;
        }
    }
    
    /// <summary>
    /// 获取病毒类型名称
    /// </summary>
    public string GetTypeName()
    {
        switch (virusType)
        {
            case VirusType.Normal:
                return "普通";
            case VirusType.Elite:
                return "精英";
            case VirusType.Boss:
                return "BOSS";
            default:
                return "未知";
        }
    }
    
    // ==================== 调试信息 ====================
    
    /// <summary>
    /// 获取病毒的详细信息（用于调试）
    /// </summary>
    public override string ToString()
    {
        return $"[病毒] {GetTypeName()} - 数字{displayNumber}\n" +
               $"ID: {virusId}\n" +
               $"血量: {currentHealth}/{maxHealth}\n" +
               $"伤害: {damage}\n" +
               $"速度: {moveSpeed}\n" +
               $"奖励: {killReward}金币\n" +
               $"状态: {(isDead ? "已死亡" : "存活")}";
    }
}
