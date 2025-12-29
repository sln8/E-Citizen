using System;

/// <summary>
/// 用户数据类
/// 存储用户的基本信息和游戏数据
/// </summary>
[Serializable]
public class UserData
{
    #region 基本信息
    /// <summary>
    /// 用户唯一ID
    /// Firebase会为每个用户生成唯一的ID
    /// </summary>
    public string userId;
    
    /// <summary>
    /// 用户名/昵称
    /// 在游戏中显示的名称
    /// </summary>
    public string username;
    
    /// <summary>
    /// 邮箱地址
    /// 用于某些登录方式
    /// </summary>
    public string email;
    
    /// <summary>
    /// 用户头像URL
    /// 从第三方登录获取或自定义上传
    /// </summary>
    public string avatarUrl;
    
    /// <summary>
    /// 登录提供商
    /// 记录用户是通过哪种方式登录的
    /// </summary>
    public LoginProvider loginProvider;
    
    /// <summary>
    /// 账号创建时间
    /// </summary>
    public string createdAt;
    
    /// <summary>
    /// 最后登录时间
    /// </summary>
    public string lastLoginAt;
    #endregion

    #region 游戏数据（后续扩展）
    /// <summary>
    /// 玩家等级
    /// </summary>
    public int level = 1;
    
    /// <summary>
    /// 虚拟币数量
    /// </summary>
    public int virtualCoin = 100;
    
    /// <summary>
    /// 心情值
    /// </summary>
    public int moodValue = 10;
    
    /// <summary>
    /// 身份类型
    /// 0 = 意识连接者, 1 = 完全虚拟人
    /// </summary>
    public int identityType = 0;
    
    /// <summary>
    /// 是否为VIP
    /// </summary>
    public bool isVIP = false;
    
    /// <summary>
    /// VIP到期时间
    /// </summary>
    public string vipExpireTime = "";
    #endregion

    #region 构造函数
    /// <summary>
    /// 默认构造函数
    /// 创建一个空的用户数据对象
    /// </summary>
    public UserData()
    {
        userId = "";
        username = "新玩家";
        email = "";
        avatarUrl = "";
        loginProvider = LoginProvider.None;
        createdAt = DateTime.Now.ToString();
        lastLoginAt = DateTime.Now.ToString();
    }
    
    /// <summary>
    /// 带参数的构造函数
    /// </summary>
    public UserData(string id, string name, string mail, LoginProvider provider)
    {
        userId = id;
        username = name;
        email = mail;
        loginProvider = provider;
        createdAt = DateTime.Now.ToString();
        lastLoginAt = DateTime.Now.ToString();
    }
    #endregion

    #region 辅助方法
    /// <summary>
    /// 更新最后登录时间
    /// </summary>
    public void UpdateLastLoginTime()
    {
        lastLoginAt = DateTime.Now.ToString();
    }
    
    /// <summary>
    /// 获取用户信息的字符串表示
    /// 用于调试和日志输出
    /// </summary>
    public override string ToString()
    {
        return $"UserData[ID:{userId}, Name:{username}, Provider:{loginProvider}, Level:{level}, Coin:{virtualCoin}]";
    }
    
    /// <summary>
    /// 检查VIP是否有效
    /// </summary>
    public bool IsVIPValid()
    {
        if (!isVIP) return false;
        
        if (string.IsNullOrEmpty(vipExpireTime)) return false;
        
        try
        {
            DateTime expireDate = DateTime.Parse(vipExpireTime);
            return DateTime.Now < expireDate;
        }
        catch
        {
            return false;
        }
    }
    #endregion
}

/// <summary>
/// 登录提供商枚举
/// 定义支持的登录方式
/// </summary>
public enum LoginProvider
{
    /// <summary>
    /// 未指定
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Google账号登录
    /// </summary>
    Google = 1,
    
    /// <summary>
    /// Facebook账号登录
    /// </summary>
    Facebook = 2,
    
    /// <summary>
    /// Apple账号登录（仅iOS）
    /// </summary>
    Apple = 3,
    
    /// <summary>
    /// 测试账号登录
    /// 用于开发和测试
    /// </summary>
    TestAccount = 4,
    
    /// <summary>
    /// 游客登录
    /// 无需注册，快速开始游戏
    /// </summary>
    Guest = 5
}
