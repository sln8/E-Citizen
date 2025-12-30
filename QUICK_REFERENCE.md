# 《电子公民》开发快速参考指南

## 🚀 常用操作速查表

### Unity基础操作

#### 运行游戏
```
点击Unity顶部的播放按钮 ▶️
或按快捷键：Ctrl+P (Windows) / Cmd+P (Mac)
```

#### 停止游戏
```
再次点击播放按钮
或按快捷键：Ctrl+P (Windows) / Cmd+P (Mac)
```

#### 保存场景
```
File -> Save Scene
或按快捷键：Ctrl+S (Windows) / Cmd+S (Mac)
```

#### 查看Console日志
```
Window -> General -> Console
或按快捷键：Ctrl+Shift+C
```

---

### 游戏测试流程

#### 测试完整游戏循环（5分钟流程）

1. **启动游戏**
   ```
   点击播放按钮
   查看Console确认初始化成功
   ```

2. **购买技能**
   ```
   点击"技能商店"按钮
   选择一个技能（例如：数据清理 Lv.1，50币）
   点击"购买"
   等待下载完成（约10-20秒，根据网速模拟）
   ```

3. **分配算力**
   ```
   点击"算力分配"按钮
   找到刚购买的技能
   拖动滑动条分配10点算力
   观察掌握度预览变为100%
   点击"应用更改"
   ```

4. **开始工作**
   ```
   点击"工作市场"按钮
   选择一个工作（例如：数据清洁工，15币/5分钟）
   点击"查看详情"
   检查技能要求（应该显示绿色✓）
   点击"开始工作"
   ```

5. **等待薪资结算**
   ```
   观察倒计时（如果开启Debug Mode，30秒）
   倒计时结束时，自动结算薪资
   查看Console日志确认薪资发放
   观察虚拟币增加
   ```

6. **辞职（可选）**
   ```
   可以继续工作，或点击"辞职"按钮
   资源会被释放
   工作槽位变为可用
   ```

---

### 调试技巧

#### 查看当前资源状态
在游戏运行时，按 `F1` 键显示调试信息面板（如果GameManager启用了showDebugUI）

#### 手动触发周期结算
在Phase 2的测试UI中，点击"立即触发结算"按钮

#### 增加虚拟币（测试用）
在ResourceManager中调用：
```csharp
ResourceManager.Instance.AddVirtualCoin(1000);
```

#### 解锁更多工作槽位
```csharp
JobManager.Instance.UnlockJobSlot();
```

---

### 常见问题快速解决

#### 问题：购买技能失败，提示"虚拟币不足"
**解决方案**：
```
方法1：先做简单工作赚钱（虚拟巡逻员，10币/5分钟）
方法2：在测试中增加虚拟币（开发模式）
方法3：调整技能价格（修改SkillManager.CreateSampleSkills）
```

#### 问题：开始工作失败，提示"缺少必需技能"
**解决方案**：
```
1. 先购买所需技能
2. 等待技能下载完成
3. 然后再开始工作

或选择不需要技能的工作：
- 虚拟巡逻员（无需技能）
```

#### 问题：开始工作失败，提示"资源不足"
**解决方案**：
```
1. 先辞职其他工作，释放资源
2. 或升级资源配置
3. 或选择资源需求更低的工作
```

#### 问题：开始工作失败，提示"没有可用的工作槽位"
**解决方案**：
```
1. 先辞职一个工作
2. 或升级等级解锁更多槽位（Lv.10/25/50）
3. 或购买VIP月卡（+1槽位）
```

#### 问题：UI按钮点击没反应
**解决方案**：
```
1. 检查EventSystem是否存在（应该自动创建）
2. 检查脚本是否正确添加到面板
3. 检查所有UI引用是否正确连接
4. 查看Console是否有错误信息
```

#### 问题：Text显示为方块
**解决方案**：
```
1. 选中Text组件
2. 在Font Asset中选择支持中文的字体
3. 如果没有，导入中文字体文件
```

---

### 数值参考

#### 初始配置（意识连接者）
```
虚拟币：100
内存：16GB（已用2GB，可用14GB）
CPU：8核（已用1核，可用7核）
网速：1000Mbps（已用50Mbps，可用950Mbps）
算力：100（已用10，可用90）
存储：500GB（已用20GB，可用480GB）
心情值：10
等级：1
```

#### 工作数据
```
数据清洁工：
  品级：普通
  薪资：15币/5分钟
  所需技能：数据清理 Lv.1
  资源：内存1GB, CPU0.5核, 网速50Mbps, 算力5

虚拟巡逻员：
  品级：普通
  薪资：10币/5分钟
  所需技能：无
  资源：内存0.5GB, CPU0.25核, 网速30Mbps, 算力3
```

#### 技能数据
```
数据清理 Lv.1：
  品级：普通
  价格：50币
  文件大小：1GB
  100%掌握度需要：10算力
  200%掌握度需要：30算力
```

#### 升级成本（参考）
```
内存 +1GB：100币
CPU +1核：200币
网速 +100Mbps：150币
算力 +10：180币
存储 +50GB：50币
```

---

### 代码修改指南

#### 添加新工作
在 `JobManager.cs` 的 `CreateSampleJobs()` 方法中添加：
```csharp
JobData newJob = new JobData
{
    jobId = "job_custom",
    jobName = "我的自定义工作",
    jobDescription = "这是一个自定义的工作",
    jobTier = JobTier.Common,  // 普通/精良/史诗/传说
    requiredSkillIds = new string[] { },
    resourceRequirement = new ResourceRequirement(1f, 0.5f, 50f, 5f),
    baseSalary = 20,
    payInterval = 300,
    dataGeneration = 0.2f,
    unlockLevel = 1
};
allJobs.Add(newJob);
```

#### 添加新技能
在 `SkillManager.cs` 的 `CreateSampleSkills()` 方法中添加：
```csharp
SkillData newSkill = new SkillData
{
    skillId = "mySkill_lv1",
    skillName = "我的技能 Lv.1",
    skillDescription = "这是一个自定义技能",
    skillTier = SkillTier.Common,
    skillLevel = 1,
    price = 100,
    fileSize = 2f,
    maxComputingFor100Percent = 15f,
    maxComputingFor200Percent = 45f,
    unlockLevel = 1,
    prerequisiteSkillId = ""
};
allSkills.Add(newSkill);
```

#### 调整游戏速度（调试用）
在 `GameTimerManager.cs` 中：
```csharp
[Header("调试设置")]
public bool debugMode = true;
public float debugCycleTime = 30f;  // 改为30秒一个周期
public float timeScale = 1f;        // 改为2f可以2倍速
```

---

### 键盘快捷键

#### Unity编辑器
```
F1: 显示调试信息（如果启用）
F2: 重命名选中对象
F: 聚焦到选中对象
Ctrl+D: 复制选中对象
Delete: 删除选中对象
Ctrl+Z: 撤销
Ctrl+Y: 重做
Ctrl+P: 运行/停止游戏
Ctrl+S: 保存场景
```

#### 游戏中测试快捷键（需要TestLogin.cs脚本）
```
1: 测试Google登录
2: 测试Facebook登录
3: 测试Apple登录
4: 快速创建测试账号
```

---

### 项目文件结构

```
E-Citizen/
├── E-Citizens/                    # Unity项目文件夹
│   ├── Assets/
│   │   ├── Scenes/               # 场景文件
│   │   ├── Scripts/              # 所有脚本
│   │   │   ├── Firebase/         # Firebase相关
│   │   │   ├── Authentication/   # 登录系统
│   │   │   ├── Data/             # 数据类
│   │   │   ├── Managers/         # 管理器
│   │   │   └── UI/               # UI脚本
│   │   ├── Prefabs/              # 预制体
│   │   ├── Resources/            # 资源文件
│   │   └── Plugins/              # 插件
│   └── ProjectSettings/          # Unity项目设置
│
├── README.md                      # 项目总览
├── 游戏设计.cs                    # 游戏设计文档
├── QUICKSTART.md                  # 快速开始
├── SETUP_GUIDE.md                 # Firebase配置
├── PHASE1_README.md               # Phase 1指南
├── PHASE2_SUMMARY.md              # Phase 2总结
├── PHASE2_SETUP_GUIDE.md          # Phase 2指南
├── PHASE3_SUMMARY.md              # Phase 3总结
├── PHASE3_SETUP_GUIDE.md          # Phase 3指南
├── PHASE3_UI_GUIDE.md             # Phase 3 UI指南
├── PHASE3_LATEST_UPDATE.md        # Phase 3最新更新
└── CODE_REVIEW_PHASE3.md          # Phase 3代码审查
```

---

### 学习资源

#### Unity官方文档
- Unity基础教程：https://learn.unity.com/
- Unity Scripting API：https://docs.unity3d.com/ScriptReference/
- Unity中文文档：https://docs.unity.cn/

#### C#学习
- Microsoft C#文档：https://docs.microsoft.com/zh-cn/dotnet/csharp/
- C#基础教程：https://www.runoob.com/csharp/csharp-tutorial.html

#### Firebase
- Firebase Unity SDK：https://firebase.google.com/docs/unity/setup
- Firebase Firestore：https://firebase.google.com/docs/firestore

---

### 获取帮助

#### 查看日志
游戏运行时，所有重要信息都会输出到Console：
```
Window -> General -> Console
```

#### 保存日志
Console右上角点击菜单 -> Export，可以导出日志到文件

#### 调试代码
1. 在代码中添加断点（在行号左侧点击）
2. 点击 Attach to Unity 开始调试
3. 运行游戏，程序会在断点处暂停

---

## 🎯 记住这些要点

1. **遇到问题先看Console** - 大部分错误都有详细提示
2. **所有代码都有中文注释** - 不懂就看注释
3. **一步一步来** - 不要急于求成
4. **多测试** - 每次改动后都测试一下
5. **保存进度** - 经常保存场景和脚本

---

**这个参考指南是你的好伙伴，遇到问题随时查看！** 📚✨

**开发愉快！** 🚀
