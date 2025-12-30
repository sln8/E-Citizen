# Phase 3 UI界面实现指南

## 📋 概述

本指南将教你如何在Unity中创建Phase 3的三个主要UI界面：
1. **工作市场UI** - 浏览和承接工作
2. **技能商店UI** - 购买和下载技能
3. **算力分配UI** - 管理技能掌握度

所有步骤都针对零基础开发者，每一步都有详细说明。

---

## 🎯 前置准备

在开始之前，确保：
- ✅ Unity项目可以正常运行
- ✅ Phase 3核心脚本已添加到项目
- ✅ JobManager和SkillManager已添加到场景
- ✅ TextMeshPro已导入（如果未导入，Unity会提示）

---

## 一、工作市场UI (WorkMarketUI)

### 步骤1：创建主面板（5分钟）

1. 在Unity编辑器中，找到Hierarchy窗口
2. 找到Canvas对象，右键点击
3. 选择 `UI` -> `Panel`
4. 重命名为 `WorkMarketPanel`
5. 在Inspector中设置：
   - Pos X: `0`
   - Pos Y: `0`
   - Width: `1000`
   - Height: `700`
   - 选择Image组件，设置Color的Alpha为 `200`（半透明）

### 步骤2：创建标题栏（3分钟）

1. 右键点击 `WorkMarketPanel`
2. 选择 `UI` -> `Text - TextMeshPro`
3. 重命名为 `TitleText`
4. 在Inspector中设置：
   - Text: `工作市场`
   - Font Size: `36`
   - Alignment: 居中
   - Color: 白色
   - Pos Y: `300`
   - Width: `900`
   - Height: `60`

### 步骤3：创建工作列表ScrollView（10分钟）

1. 右键点击 `WorkMarketPanel`
2. 选择 `UI` -> `Scroll View`
3. 重命名为 `JobListScrollView`
4. 在Inspector中设置ScrollView：
   - Pos X: `-300`
   - Pos Y: `0`
   - Width: `380`
   - Height: `500`

5. 展开ScrollView，找到 `Viewport` -> `Content`
6. 选择Content，在Inspector中：
   - 添加 `Vertical Layout Group` 组件
   - 设置：
     * Spacing: `10`
     * Padding: Left=10, Right=10, Top=10, Bottom=10
     * Child Force Expand: Width勾选，Height不勾选
   - 添加 `Content Size Fitter` 组件
   - 设置：
     * Vertical Fit: `Preferred Size`

### 步骤4：创建工作项预制体（15分钟）

1. 右键点击Content
2. 选择 `UI` -> `Image`
3. 重命名为 `JobItem`
4. 设置：
   - Width: `360`
   - Height: `120`

5. 在JobItem下创建子对象：

   **A. 工作名称**：
   - 右键JobItem -> `UI` -> `Text - TextMeshPro`
   - 命名为 `JobName`
   - Text: `数据清洁工`
   - Font Size: `20`
   - Pos Y: `35`
   - Width: `300`, Height: `30`

   **B. 工作品级**：
   - 右键JobItem -> `UI` -> `Text - TextMeshPro`
   - 命名为 `JobTier`
   - Text: `[普通]`
   - Font Size: `16`
   - Pos Y: `5`
   - Width: `300`, Height: `25`

   **C. 薪资**：
   - 右键JobItem -> `UI` -> `Text - TextMeshPro`
   - 命名为 `Salary`
   - Text: `💰 15币/5分钟`
   - Font Size: `16`
   - Pos Y: `-20`
   - Width: `300`, Height: `25`

   **D. 等级要求**：
   - 右键JobItem -> `UI` -> `Text - TextMeshPro`
   - 命名为 `Level`
   - Text: `✓ Lv.1`
   - Font Size: `14`
   - Pos Y: `-45`
   - Width: `150`, Height: `20`

   **E. 查看按钮**：
   - 右键JobItem -> `UI` -> `Button - TextMeshPro`
   - 命名为 `ViewButton`
   - 设置按钮：
     * Pos X: `130`, Pos Y: `-45`
     * Width: `120`, Height: `30`
   - 修改按钮Text为：`查看详情`

6. 将JobItem拖拽到 `Assets/Prefabs/` 文件夹，创建预制体
7. 删除Hierarchy中的JobItem（保留预制体）

### 步骤5：创建工作详情面板（20分钟）

1. 右键点击 `WorkMarketPanel`
2. 选择 `UI` -> `Panel`
3. 命名为 `JobDetailPanel`
4. 设置：
   - Pos X: `250`
   - Pos Y: `0`
   - Width: `540`
   - Height: `600`

5. 在JobDetailPanel下创建：

   **A. 工作名称**：
   - Text - TextMeshPro，命名 `DetailJobName`
   - Text: `工作名称`
   - Font Size: `28`
   - Pos Y: `250`

   **B. 工作描述**：
   - Text - TextMeshPro，命名 `DetailJobDescription`
   - Text: `工作描述...`
   - Font Size: `16`
   - Pos Y: `180`
   - Width: `500`, Height: `80`
   - 启用Word Wrapping

   **C. 工作品级**：
   - Text - TextMeshPro，命名 `DetailJobTier`
   - Text: `品级：普通`
   - Pos Y: `120`

   **D. 基础薪资**：
   - Text - TextMeshPro，命名 `DetailBaseSalary`
   - Text: `基础薪资：15币/5分钟`
   - Pos Y: `90`

   **E. 所需技能**：
   - Text - TextMeshPro，命名 `DetailRequiredSkills`
   - Text: `所需技能：无`
   - Pos Y: `40`
   - Width: `500`, Height: `100`
   - Alignment: 左上对齐

   **F. 资源需求**：
   - Text - TextMeshPro，命名 `DetailResourceRequirement`
   - Text: `资源需求：...`
   - Pos Y: `-60`
   - Width: `500`, Height: `120`
   - Alignment: 左上对齐

   **G. 解锁等级**：
   - Text - TextMeshPro，命名 `DetailUnlockLevel`
   - Text: `解锁等级：Lv.1`
   - Pos Y: `-150`

   **H. 开始工作按钮**：
   - Button - TextMeshPro，命名 `StartJobButton`
   - Text: `开始工作`
   - Pos Y: `-220`
   - Width: `200`, Height: `50`

   **I. 关闭按钮**：
   - Button - TextMeshPro，命名 `CloseDetailButton`
   - Text: `关闭`
   - Pos Y: `-220`, Pos X: `150`
   - Width: `100`, Height: `50`

### 步骤6：创建控制按钮（5分钟）

1. 右键点击 `WorkMarketPanel`
2. 创建两个按钮：

   **刷新按钮**：
   - Button - TextMeshPro，命名 `RefreshButton`
   - Text: `刷新列表`
   - Pos X: `-400`, Pos Y: `-300`
   - Width: `150`, Height: `40`

   **关闭按钮**：
   - Button - TextMeshPro，命名 `CloseMarketButton`
   - Text: `关闭`
   - Pos X: `400`, Pos Y: `300`
   - Width: `80`, Height: `40`

### 步骤7：添加脚本并连接引用（10分钟）

1. 选中 `WorkMarketPanel`
2. 在Inspector点击 `Add Component`
3. 搜索 `WorkMarketUI`，添加脚本

4. 连接所有引用（拖拽对应对象到对应字段）：
   - Market Panel: WorkMarketPanel
   - Job List Content: JobListScrollView/Viewport/Content
   - Job Item Prefab: Assets/Prefabs/JobItem
   - Job Detail Panel: JobDetailPanel
   - Detail Job Name: DetailJobName
   - Detail Job Description: DetailJobDescription
   - Detail Job Tier: DetailJobTier
   - Detail Base Salary: DetailBaseSalary
   - Detail Required Skills: DetailRequiredSkills
   - Detail Resource Requirement: DetailResourceRequirement
   - Detail Unlock Level: DetailUnlockLevel
   - Start Job Button: StartJobButton
   - Close Detail Button: CloseDetailButton
   - Refresh Button: RefreshButton
   - Close Market Button: CloseMarketButton

5. 保存场景（Ctrl+S 或 Cmd+S）

---

## 二、技能商店UI (SkillShopUI)

### 步骤1：创建主面板（5分钟）

1. 右键Canvas，选择 `UI` -> `Panel`
2. 命名为 `SkillShopPanel`
3. 设置：
   - Pos X: `0`, Pos Y: `0`
   - Width: `1000`, Height: `700`
   - Image Alpha: `200`

### 步骤2：创建标题栏（3分钟）

1. 右键 `SkillShopPanel`
2. 创建Text - TextMeshPro，命名 `TitleText`
3. 设置：
   - Text: `技能商店`
   - Font Size: `36`
   - 居中对齐
   - Pos Y: `300`

### 步骤3：创建技能列表ScrollView（10分钟）

与工作市场类似：
1. 右键 `SkillShopPanel`
2. 创建Scroll View，命名 `SkillListScrollView`
3. 设置：
   - Pos X: `-300`
   - Pos Y: `0`
   - Width: `380`, Height: `500`
4. Content添加Vertical Layout Group和Content Size Fitter

### 步骤4：创建技能项预制体（15分钟）

1. 在Content下创建Image，命名 `SkillItem`
2. 设置：Width: `360`, Height: `100`

3. 创建子对象：
   - `SkillName` (Text - TextMeshPro): 技能名称
   - `SkillTier` (Text - TextMeshPro): 品级
   - `Price` (Text - TextMeshPro): 价格
   - `Status` (Text - TextMeshPro): 状态（已拥有/可购买/未解锁）
   - `ViewButton` (Button): 查看详情按钮

4. 创建预制体并保存到Assets/Prefabs/

### 步骤5：创建技能详情面板（25分钟）

1. 右键 `SkillShopPanel`
2. 创建Panel，命名 `SkillDetailPanel`
3. 设置：Pos X: `250`, Width: `540`, Height: `600`

4. 创建详情显示元素：
   - `DetailSkillName`: 技能名称
   - `DetailSkillDescription`: 技能描述
   - `DetailSkillTier`: 技能品级
   - `DetailPrice`: 价格
   - `DetailFileSize`: 文件大小
   - `DetailPrerequisite`: 前置技能
   - `DetailMasteryInfo`: 掌握度信息
   - `DetailUnlockLevel`: 解锁等级
   - `PurchaseButton`: 购买按钮
   - `CloseDetailButton`: 关闭按钮

### 步骤6：创建下载进度面板（10分钟）

1. 右键 `SkillShopPanel`
2. 创建Panel，命名 `DownloadPanel`
3. 设置：
   - Pos X: `0`, Pos Y: `-250`
   - Width: `400`, Height: `100`

4. 添加子对象：
   - `DownloadProgressText` (Text): 显示下载进度
   - `DownloadProgressBar` (Slider): 进度条
     * 在Slider下找到Fill Area/Fill，设置颜色为绿色

### 步骤7：添加脚本并连接引用（10分钟）

1. 选中 `SkillShopPanel`
2. 添加 `SkillShopUI` 组件
3. 连接所有引用（类似工作市场UI的步骤）
4. 保存场景

---

## 三、算力分配UI (ComputingAllocationUI)

### 步骤1：创建主面板（5分钟）

1. 右键Canvas，创建Panel
2. 命名 `ComputingAllocationPanel`
3. 设置：Width: `800`, Height: `600`

### 步骤2：创建算力信息显示（10分钟）

1. 在面板顶部创建三个Text显示：
   - `TotalComputingText`: 总算力
   - `AllocatedComputingText`: 已分配
   - `AvailableComputingText`: 可用
2. 布局：横向排列在顶部

### 步骤3：创建技能列表ScrollView（10分钟）

1. 创建ScrollView，命名 `SkillListScrollView`
2. 设置：Width: `760`, Height: `400`
3. Content添加Vertical Layout Group

### 步骤4：创建算力分配项预制体（20分钟）

1. 在Content下创建Image，命名 `ComputingItem`
2. 设置：Width: `740`, Height: `120`

3. 创建子对象：
   - `SkillName` (Text): 技能名称
   - `CurrentMastery` (Text): 当前掌握度
   - `Requirement` (Text): 算力需求信息
   - `ComputingSlider` (Slider): 算力分配滑动条
     * Min Value: `0`
     * Max Value: `200` (根据技能设置)
     * Whole Numbers: 不勾选
   - `AllocationValue` (Text): 分配的算力数值
   - `PreviewMastery` (Text): 预览掌握度

4. 创建预制体

### 步骤5：创建控制按钮（5分钟）

在面板底部创建：
- `ResetAllButton`: 重置所有算力
- `ApplyButton`: 应用更改
- `CloseButton`: 关闭面板

### 步骤6：添加脚本并连接引用（10分钟）

1. 选中 `ComputingAllocationPanel`
2. 添加 `ComputingAllocationUI` 组件
3. 连接所有引用
4. 保存场景

---

## 四、创建快捷入口（10分钟）

为了方便测试和使用，在主界面创建打开这些UI的按钮。

### 步骤1：创建UI入口面板

1. 在Canvas下创建Panel，命名 `UIEntrancePanel`
2. 设置：
   - 锚点：右上角
   - Pos X: `-100`, Pos Y: `-50`
   - Width: `180`, Height: `200`

### 步骤2：创建入口按钮

在UIEntrancePanel下创建三个按钮：
1. `OpenWorkMarketButton`
   - Text: `工作市场`
   - Pos Y: `60`

2. `OpenSkillShopButton`
   - Text: `技能商店`
   - Pos Y: `0`

3. `OpenComputingButton`
   - Text: `算力分配`
   - Pos Y: `-60`

### 步骤3：连接按钮事件

1. 选中 `OpenWorkMarketButton`
2. 在Inspector中找到Button组件的OnClick事件
3. 点击 `+` 添加事件
4. 将 `WorkMarketPanel` 拖拽到对象框
5. 在下拉菜单选择 `WorkMarketUI` -> `OpenMarket()`

6. 对另外两个按钮重复类似操作：
   - `OpenSkillShopButton` -> `SkillShopUI.OpenShop()`
   - `OpenComputingButton` -> `ComputingAllocationUI.OpenPanel()`

---

## 五、测试UI（10分钟）

### 测试清单

1. **测试工作市场UI**：
   - [ ] 点击"工作市场"按钮，面板正常打开
   - [ ] 工作列表正常显示
   - [ ] 点击工作项，详情面板正常显示
   - [ ] 开始工作按钮功能正常
   - [ ] 关闭按钮正常工作

2. **测试技能商店UI**：
   - [ ] 点击"技能商店"按钮，面板正常打开
   - [ ] 技能列表正常显示
   - [ ] 点击技能项，详情面板正常显示
   - [ ] 购买按钮功能正常
   - [ ] 下载进度显示正常
   - [ ] 关闭按钮正常工作

3. **测试算力分配UI**：
   - [ ] 点击"算力分配"按钮，面板正常打开
   - [ ] 技能列表正常显示（只显示已拥有的技能）
   - [ ] 滑动条可以调整算力分配
   - [ ] 掌握度预览正常更新
   - [ ] 应用更改功能正常
   - [ ] 重置功能正常
   - [ ] 关闭按钮正常工作

---

## 🐛 常见问题和解决方案

### 问题1：Text显示为方块
**原因**：TextMeshPro未正确设置字体

**解决方法**：
1. 选中Text组件
2. 在Inspector中找到Font Asset
3. 点击选择按钮，选择支持中文的字体
4. 如果没有中文字体，导入一个或使用系统字体

### 问题2：预制体无法创建
**原因**：Assets/Prefabs文件夹不存在

**解决方法**：
1. 在Project窗口，右键Assets
2. 选择 `Create` -> `Folder`
3. 命名为 `Prefabs`

### 问题3：按钮点击没有反应
**原因**：未连接脚本或EventSystem缺失

**解决方法**：
1. 检查EventSystem是否存在（Canvas创建时自动生成）
2. 检查脚本是否正确添加到面板
3. 检查所有引用是否正确连接

### 问题4：UI显示不正确
**原因**：Canvas Scaler设置不当

**解决方法**：
1. 选中Canvas
2. 在Canvas Scaler组件中
3. 设置UI Scale Mode为 `Scale With Screen Size`
4. Reference Resolution: `1920 x 1080`

---

## 💡 优化建议

### 1. 美化UI
- 添加背景图片
- 使用更好的按钮样式
- 添加图标和装饰元素
- 使用渐变和阴影效果

### 2. 添加动画
- 面板打开/关闭动画
- 按钮点击效果
- 列表项滑入动画
- 进度条动画

### 3. 添加音效
- 按钮点击音效
- 面板打开/关闭音效
- 成功/失败提示音效
- 背景音乐

### 4. 添加提示信息
- Tooltip提示
- 帮助按钮
- 新手引导

---

## 🎯 完成检查清单

完成以下所有项目，表示Phase 3 UI已成功创建：

- [ ] 工作市场UI主面板创建完成
- [ ] 工作项预制体创建完成
- [ ] 工作详情面板创建完成
- [ ] WorkMarketUI脚本连接完成
- [ ] 技能商店UI主面板创建完成
- [ ] 技能项预制体创建完成
- [ ] 技能详情面板创建完成
- [ ] 下载进度面板创建完成
- [ ] SkillShopUI脚本连接完成
- [ ] 算力分配UI主面板创建完成
- [ ] 算力分配项预制体创建完成
- [ ] ComputingAllocationUI脚本连接完成
- [ ] UI入口按钮创建完成
- [ ] 所有UI测试通过
- [ ] Console无错误信息

---

## 🚀 下一步

完成Phase 3 UI后，你可以：
1. 美化UI界面，添加更多视觉效果
2. 实现数据持久化（保存到Firebase）
3. 开始Phase 4公司系统开发
4. 添加更多工作和技能数据

---

**祝开发顺利！如有问题，请查看Console日志或重新检查每个步骤。** 🎉
