using System;
using UnityEngine;

/// <summary>
/// 礼物数据类
/// 用于好友之间赠送虚拟道具
/// 
/// 功能：
/// - 定义可赠送的礼物类型
/// - 存储礼物属性（成本、效果）
/// - 礼物赠送记录
/// 
/// Unity操作步骤：
/// 1. 此文件会自动被Unity识别
/// 2. 无需手动操作，管理器会使用此数据类
/// </summary>
[Serializable]
public class GiftData
{
    #region 基本属性
    
    /// <summary>
    /// 礼物ID
    /// </summary>
    public string giftId;
    
    /// <summary>
    /// 礼物名称
    /// </summary>
    public string giftName;
    
    /// <summary>
    /// 礼物描述
    /// </summary>
    public string description;
    
    /// <summary>
    /// 礼物成本（虚拟币）
    /// </summary>
    public int cost;
    
    /// <summary>
    /// 赠送给好友的心情值加成
    /// </summary>
    public int moodBonus;
    
    /// <summary>
    /// 礼物图标资源路径
    /// </summary>
    public string iconPath;
    
    /// <summary>
    /// 礼物品级（common、rare、epic）
    /// </summary>
    public string tier;
    
    #endregion
    
    #region 构造函数
    
    /// <summary>
    /// 默认构造函数
    /// </summary>
    public GiftData()
    {
        giftId = "";
        giftName = "未知礼物";
        description = "";
        cost = 0;
        moodBonus = 0;
        iconPath = "";
        tier = "common";
    }
    
    /// <summary>
    /// 创建礼物数据
    /// </summary>
    /// <param name="id">礼物ID</param>
    /// <param name="name">礼物名称</param>
    /// <param name="desc">描述</param>
    /// <param name="cost">成本</param>
    /// <param name="mood">心情加成</param>
    /// <param name="tier">品级</param>
    public GiftData(string id, string name, string desc, int cost, int mood, string tier)
    {
        this.giftId = id;
        this.giftName = name;
        this.description = desc;
        this.cost = cost;
        this.moodBonus = mood;
        this.iconPath = $"UI/Gifts/{id}";
        this.tier = tier;
    }
    
    #endregion
    
    #region 公共方法
    
    /// <summary>
    /// 计算性价比（心情值/虚拟币）
    /// </summary>
    /// <returns>性价比值</returns>
    public float GetCostEfficiency()
    {
        if (cost <= 0) return 0f;
        return (float)moodBonus / cost;
    }
    
    /// <summary>
    /// 获取品级颜色
    /// </summary>
    /// <returns>Unity Color对象</returns>
    public Color GetTierColor()
    {
        switch (tier)
        {
            case "common":
                return Color.white;
            case "rare":
                return Color.blue;
            case "epic":
                return new Color(0.5f, 0f, 0.5f); // 紫色
            case "legendary":
                return new Color(1f, 0.5f, 0f); // 橙色
            default:
                return Color.gray;
        }
    }
    
    /// <summary>
    /// 获取品级中文名
    /// </summary>
    /// <returns>品级名称</returns>
    public string GetTierName()
    {
        switch (tier)
        {
            case "common":
                return "普通";
            case "rare":
                return "精良";
            case "epic":
                return "史诗";
            case "legendary":
                return "传说";
            default:
                return "未知";
        }
    }
    
    /// <summary>
    /// 获取显示信息
    /// </summary>
    /// <returns>礼物完整信息</returns>
    public string GetDisplayInfo()
    {
        return $"{giftName} [{GetTierName()}]\n" +
               $"{description}\n" +
               $"成本: {cost}币 | 心情+{moodBonus}";
    }
    
    #endregion
    
    #region 静态数据 - 预定义礼物
    
    /// <summary>
    /// 创建所有可用礼物列表
    /// </summary>
    /// <returns>礼物数据数组</returns>
    public static GiftData[] CreateDefaultGifts()
    {
        return new GiftData[]
        {
            // 普通品级礼物
            new GiftData(
                "gift_mood_small",
                "数据花束",
                "一束精心编织的数据之花，让人心情愉悦",
                50,
                10,
                "common"
            ),
            
            new GiftData(
                "gift_mood_medium",
                "虚拟巧克力",
                "甜蜜的数字味道，回忆起真实世界的美好",
                100,
                20,
                "common"
            ),
            
            // 精良品级礼物
            new GiftData(
                "gift_mood_large",
                "心情大礼包",
                "包含多种心情提升道具的豪华礼包",
                500,
                100,
                "rare"
            ),
            
            new GiftData(
                "gift_memory_stone",
                "记忆水晶",
                "承载美好记忆的水晶，让好友回想起快乐时光",
                300,
                60,
                "rare"
            ),
            
            // 史诗品级礼物
            new GiftData(
                "gift_digital_pet",
                "迷你数据宠物",
                "一只可爱的迷你宠物，提供短期陪伴",
                1000,
                200,
                "epic"
            ),
            
            new GiftData(
                "gift_vacation_ticket",
                "虚拟假期券",
                "一张通往美好虚拟世界的门票",
                800,
                150,
                "epic"
            ),
            
            // 传说品级礼物
            new GiftData(
                "gift_lucky_star",
                "幸运之星",
                "传说中能带来好运的星星，大幅提升心情",
                2000,
                500,
                "legendary"
            )
        };
    }
    
    #endregion
}

/// <summary>
/// 礼物发送记录数据类
/// 记录好友之间的礼物赠送历史
/// </summary>
[Serializable]
public class GiftTransactionData
{
    /// <summary>
    /// 交易ID
    /// </summary>
    public string transactionId;
    
    /// <summary>
    /// 发送者用户ID
    /// </summary>
    public string senderUserId;
    
    /// <summary>
    /// 发送者名称
    /// </summary>
    public string senderName;
    
    /// <summary>
    /// 接收者用户ID
    /// </summary>
    public string receiverUserId;
    
    /// <summary>
    /// 接收者名称
    /// </summary>
    public string receiverName;
    
    /// <summary>
    /// 礼物ID
    /// </summary>
    public string giftId;
    
    /// <summary>
    /// 礼物名称
    /// </summary>
    public string giftName;
    
    /// <summary>
    /// 赠送时间
    /// </summary>
    public DateTime sentTime;
    
    /// <summary>
    /// 附加消息
    /// </summary>
    public string message;
    
    /// <summary>
    /// 是否已读
    /// </summary>
    public bool isRead;
    
    /// <summary>
    /// 是否已领取
    /// </summary>
    public bool isClaimed;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public GiftTransactionData()
    {
        transactionId = Guid.NewGuid().ToString();
        senderUserId = "";
        senderName = "";
        receiverUserId = "";
        receiverName = "";
        giftId = "";
        giftName = "";
        sentTime = DateTime.Now;
        message = "";
        isRead = false;
        isClaimed = false;
    }
    
    /// <summary>
    /// 创建礼物交易记录
    /// </summary>
    /// <param name="senderId">发送者ID</param>
    /// <param name="senderName">发送者名称</param>
    /// <param name="receiverId">接收者ID</param>
    /// <param name="receiverName">接收者名称</param>
    /// <param name="gift">礼物数据</param>
    /// <param name="msg">附加消息</param>
    public GiftTransactionData(string senderId, string senderName, string receiverId, 
                               string receiverName, GiftData gift, string msg = "")
    {
        this.transactionId = Guid.NewGuid().ToString();
        this.senderUserId = senderId;
        this.senderName = senderName;
        this.receiverUserId = receiverId;
        this.receiverName = receiverName;
        this.giftId = gift.giftId;
        this.giftName = gift.giftName;
        this.sentTime = DateTime.Now;
        this.message = msg;
        this.isRead = false;
        this.isClaimed = false;
    }
    
    /// <summary>
    /// 标记为已读
    /// </summary>
    public void MarkAsRead()
    {
        isRead = true;
    }
    
    /// <summary>
    /// 领取礼物
    /// </summary>
    public void ClaimGift()
    {
        isRead = true;
        isClaimed = true;
    }
}
