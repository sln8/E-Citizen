# Phase 6 Unity 操作指南

本指南将帮助你在Unity中配置和测试Phase 6的娱乐与病毒入侵系统。

**预计完成时间**：30-40分钟

---

## 📋 前置要求

在开始之前，请确保：
- ✅ 已完成Phase 1-5的配置
- ✅ 场景中有GameManager对象
- ✅ ResourceManager、GameTimerManager、LifeSystemManager已添加
- ✅ Unity编辑器已打开项目

---

## 第一步：添加Phase 6管理器（5分钟）

### 1.1 添加娱乐系统管理器

1. 在Hierarchy中找到**GameManager**对象
2. 在Inspector中点击**Add Component**
3. 输入**EntertainmentManager**，回车添加
4. 观察Console应该显示：
   ```
   [EntertainmentManager] 娱乐系统管理器已创建
   [EntertainmentManager] 娱乐系统初始化完成，共4个活动
   ```

### 1.2 添加安全卫士管理器

1. 在GameManager对象上继续**Add Component**
2. 输入**SecurityManager**，回车添加
3. 观察Console应该显示：
   ```
   [SecurityManager] 安全卫士管理器已创建
   [SecurityManager] 已订阅游戏周期事件
   [SecurityManager] 安全卫士系统初始化完成，共4个方案
   ```

### 1.3 添加病毒入侵管理器

1. 在GameManager对象上继续**Add Component**
2. 输入**VirusInvasionManager**，回车添加
3. 在Inspector中找到**VirusInvasionManager**组件
4. **重要**：勾选**Debug Mode**（将触发时间从30-60分钟缩短到30-60秒，方便测试）
5. 观察Console应该显示：
   ```
   [VirusInvasionManager] 病毒入侵管理器已创建
   [VirusInvasionManager] 病毒入侵系统初始化完成
   [VirusInvasionManager] 下次入侵时间: X.X秒后
   ```

### ✅ 验证

运行游戏，在Console中应该看到三个管理器的初始化日志，没有红色错误。

---

## 第二步：创建测试UI（20-25分钟）

### 2.1 创建Canvas和Panel

1. **创建Canvas**（如果还没有）：
   - Hierarchy右键 → UI → Canvas
   - Canvas会自动创建EventSystem

2. **创建主Panel**：
   - Canvas右键 → UI → Panel
   - 重命名为**Phase6TestPanel**
   - 调整大小：Width=800, Height=600

### 2.2 创建状态文本

1. **创建Text**：
   - Phase6TestPanel右键 → UI → Text - TextMeshPro
   - 如果提示导入TMP资源，点击**Import TMP Essentials**
   - 重命名为**StatusText**

2. **配置StatusText**：
   - 位置：Top-Left，Anchor
   - Width: 750, Height: 400
   - Font Size: 14
   - Alignment: Top-Left
   - Color: White
   - Overflow: Overflow（允许滚动）

### 2.3 创建按钮组

#### 娱乐系统按钮（4个）

创建第一个按钮：
1. Phase6TestPanel右键 → UI → Button - TextMeshPro
2. 重命名为**StartEnt1Btn**
3. Position: X=-200, Y=-150
4. Size: Width=150, Height=30
5. 展开按钮，选择**Text (TMP)**子对象
6. 修改文本为**"星际战争"**

复制创建其他3个按钮：
- **StartEnt2Btn** (X=-200, Y=-190)：文本"末日求生"
- **StartEnt3Btn** (X=-200, Y=-230)：文本"魔法学院"
- **CancelEntBtn** (X=-200, Y=-270)：文本"取消娱乐"

#### 安全卫士按钮（4个）

- **SubNoneBtn** (X=0, Y=-150)：文本"无安全卫士"
- **SubBasicBtn** (X=0, Y=-190)：文本"普通安全卫士"
- **SubAdvancedBtn** (X=0, Y=-230)：文本"高级安全卫士"
- **SubUltimateBtn** (X=0, Y=-270)：文本"神级安全卫士"

#### 病毒入侵按钮（3个）

- **TriggerVirusBtn** (X=200, Y=-150)：文本"触发入侵"
- **SimSuccessBtn** (X=200, Y=-190)：文本"模拟成功"
- **SimFailBtn** (X=200, Y=-230)：文本"模拟失败"

#### 测试按钮（3个）

- **AddCoinsBtn** (X=-200, Y=200)：文本"+10000币"
- **AddLevelBtn** (X=0, Y=200)：文本"+10级"
- **ShowStatsBtn** (X=200, Y=200)：文本"显示统计"

**提示**：使用Ctrl+D快速复制按钮，然后修改名称、位置和文本。

### 2.4 创建测试UI对象

1. **创建空物体**：
   - Hierarchy空白处右键 → Create Empty
   - 重命名为**Phase6TestUI**

2. **添加脚本**：
   - 在Inspector中点击**Add Component**
   - 输入**Phase6TestUI**，回车添加

### 2.5 连接UI引用

选择**Phase6TestUI**对象，在Inspector中：

1. **Status Text**：
   - 点击右边的圆圈图标
   - 选择**StatusText**

2. **娱乐系统按钮**：
   - Start Ent1 Btn → StartEnt1Btn
   - Start Ent2 Btn → StartEnt2Btn
   - Start Ent3 Btn → StartEnt3Btn
   - Cancel Ent Btn → CancelEntBtn

3. **安全卫士按钮**：
   - Sub None Btn → SubNoneBtn
   - Sub Basic Btn → SubBasicBtn
   - Sub Advanced Btn → SubAdvancedBtn
   - Sub Ultimate Btn → SubUltimateBtn

4. **病毒入侵按钮**：
   - Trigger Virus Btn → TriggerVirusBtn
   - Sim Success Btn → SimSuccessBtn
   - Sim Fail Btn → SimFailBtn

5. **测试按钮**：
   - Add Coins Btn → AddCoinsBtn
   - Add Level Btn → AddLevelBtn
   - Show Stats Btn → ShowStatsBtn

6. **更新设置**：
   - Update Interval: 保持默认0.5秒

### ✅ 验证

所有引用字段都不应该显示**None (Game Object)**，应该都连接上了对应的UI元素。

---

## 第三步：测试功能（10-15分钟）

### 3.1 基础测试流程

1. **运行游戏**
2. **观察StatusText**应该显示：
   ```
   ========== Phase 6 系统测试 ==========
   
   【玩家信息】
   等级: Lv.1
   虚拟币: 100
   心情值: 10
   
   【娱乐系统】
   状态: 空闲
   总参加: 0次
   
   【安全卫士】
   当前方案: 无
   防御率: 0%
   费用: 0币/5分
   总支付: 0币
   拦截成功: 0次
   
   【病毒入侵】
   状态: 空闲
   下次入侵: XX分XX秒
   总入侵: 0次
   成功防御: 0次
   失败: 0次
   
   =====================================
   ```

3. **点击"+10000币"按钮**
   - 虚拟币应该变成10100
   - Console显示：`[Phase6TestUI] 添加10000虚拟币`

4. **点击"+10级"按钮**
   - 等级应该变成Lv.11
   - Console显示：`[Phase6TestUI] 提升10级，当前等级：11`

### 3.2 测试娱乐系统

1. **点击"星际战争"按钮**
   - Console显示：
     ```
     [EntertainmentManager] 开始娱乐活动: 星际战争，预计10.0分钟后完成
     [事件] 娱乐开始: 星际战争
     ```
   - StatusText显示：
     ```
     【娱乐系统】
     进行中: 星际战争
     进度: X%
     剩余: 9分XX秒
     ```

2. **等待完成**（或点击"取消娱乐"）
   - 完成后Console显示：
     ```
     [EntertainmentManager] 完成娱乐活动: 星际战争，获得30心情值
     [事件] 娱乐完成: 星际战争, +30心情
     ```
   - 心情值增加30

3. **测试汽车速度加成**：
   - 使用Phase5TestUI购买汽车（如果还没有）
   - 再次开始娱乐活动
   - 观察时间是否缩短（如光速跑车1.5x，10分钟变成6.7分钟）

### 3.3 测试安全卫士系统

1. **点击"普通安全卫士"按钮**
   - Console显示：
     ```
     [SecurityManager] 订阅方案: 普通安全卫士（防御率40%，费用5币/5分钟）
     [事件] 订阅安全卫士: 普通安全卫士, 5币/5分
     ```
   - StatusText显示当前方案为"普通"

2. **等待游戏周期**（5分钟或调试模式30秒）
   - Console显示：
     ```
     [SecurityManager] 支付安全卫士费用: 5币（普通安全卫士）
     ```
   - 虚拟币减少5

3. **切换方案**：
   - 点击"高级安全卫士"
   - 观察防御率变化

4. **取消订阅**：
   - 点击"无安全卫士"
   - 下个周期不再扣费

### 3.4 测试病毒入侵系统

#### 方式1：等待自然触发（调试模式30-60秒）

1. **等待触发**
   - StatusText显示倒计时
   - Console显示：
     ```
     [VirusInvasionManager] ⚠️ 病毒入侵警报！
     [事件] ⚠️ 病毒入侵警报！
     ```

2. **安全卫士拦截**（如果订阅了）
   - 有X%概率显示：
     ```
     [SecurityManager] 普通安全卫士成功拦截病毒！
     [事件] 普通安全卫士成功拦截病毒！
     ```
   - 或显示需要应对：
     ```
     [SecurityManager] 普通安全卫士未能拦截病毒，需要玩家应对
     [VirusInvasionManager] 病毒入侵游戏开始！
     ```

#### 方式2：强制触发（推荐测试）

1. **点击"触发入侵"按钮**
   - 立即触发病毒入侵

2. **点击"模拟成功"或"模拟失败"**
   - 成功：
     ```
     [事件] 防御成功！击杀10，+50币，+10心情
     ```
     虚拟币增加50，心情值增加10
   
   - 失败：
     ```
     [事件] 防御失败！损失XXX币
     ```
     虚拟币减少（总虚拟币的1%-5%）

3. **查看统计**：
   - 点击"显示统计"按钮
   - Console显示详细统计数据

### 3.5 综合测试流程

**完整的游戏循环测试**：

1. 添加虚拟币（+10000币）
2. 订阅高级安全卫士（70%防御率）
3. 开始娱乐活动（星际战争）
4. 等待娱乐完成（+30心情）
5. 等待或触发病毒入侵
6. 观察安全卫士是否拦截成功
7. 如果未拦截，模拟游戏结果
8. 查看虚拟币和心情值变化
9. 重复2-8步测试不同场景

### ✅ 验证清单

- [ ] 娱乐系统可以正常开始和完成
- [ ] 汽车速度加成生效（如果有汽车）
- [ ] 心情值正确发放
- [ ] 安全卫士可以订阅和取消
- [ ] 每5分钟自动扣除安全卫士费用
- [ ] 病毒入侵可以触发
- [ ] 安全卫士防御判定正常工作
- [ ] 成功/失败奖惩计算正确
- [ ] 统计数据正确累计

---

## 第四步：高级测试（可选，5分钟）

### 4.1 测试边界情况

1. **虚拟币不足时参加娱乐**
   - 清空虚拟币
   - 尝试开始娱乐
   - 应该失败并显示提示

2. **虚拟币不足时安全卫士自动取消**
   - 订阅安全卫士
   - 清空虚拟币
   - 等待下个周期
   - 安全卫士应该自动取消

3. **等级不足时无法订阅高级方案**
   - 降低等级到1
   - 尝试订阅高级/神级安全卫士
   - 应该失败

4. **娱乐进行中无法开始新娱乐**
   - 开始一个娱乐
   - 尝试开始另一个
   - 应该失败

### 4.2 测试事件系统

在Console观察以下事件是否正确触发：

```
[事件] 娱乐开始: XXX
[事件] 娱乐完成: XXX, +XX心情
[事件] 订阅安全卫士: XXX, XX币/5分
[事件] XXX成功拦截病毒！
[事件] ⚠️ 病毒入侵警报！
[事件] 防御成功！击杀XX，+XX币，+XX心情
[事件] 防御失败！损失XX币
```

所有事件应该在合适的时机触发，并且颜色编码正确（绿色=成功，红色=警告，黄色=状态变化）。

---

## 🐛 常见问题排查

### 问题1：管理器初始化失败

**症状**：Console显示"XXX未初始化"错误

**解决方案**：
1. 检查GameManager对象是否存在
2. 检查是否添加了所有Phase 6管理器
3. 确认脚本编译无错误

### 问题2：UI按钮点击无响应

**症状**：点击按钮没有任何反应

**解决方案**：
1. 检查Phase6TestUI脚本是否添加
2. 检查所有UI引用是否正确连接
3. 检查Canvas是否有EventSystem
4. 在按钮OnClick事件中添加断点调试

### 问题3：娱乐活动不会自动完成

**症状**：进度条到100%但不结算

**解决方案**：
1. 检查EntertainmentManager是否在Update中检查完成
2. 确认Time.time正常工作
3. 查看Console是否有错误日志

### 问题4：安全卫士不扣费

**症状**：订阅后不扣虚拟币

**解决方案**：
1. 检查SecurityManager是否订阅了OnGameTick事件
2. 检查GameTimerManager是否正常运行
3. 等待一个完整的游戏周期（5分钟或调试模式30秒）

### 问题5：病毒入侵不触发

**症状**：等待很久也不触发

**解决方案**：
1. 确认VirusInvasionManager的Debug Mode已勾选
2. 使用"触发入侵"按钮强制触发
3. 检查nextInvasionTime是否正确设置

---

## 📝 保存场景

测试完成后，记得保存场景：
1. 菜单：File → Save Scene
2. 或按 Ctrl+S (Mac: Cmd+S)

---

## 🎉 完成！

恭喜你完成Phase 6的Unity配置！

你已经成功配置了：
- ✅ 娱乐系统（4种活动）
- ✅ 安全卫士系统（4种方案）
- ✅ 病毒入侵系统（随机触发）
- ✅ 综合测试UI（18个按钮）

**下一步**：
- 阅读PHASE6_SUMMARY.md了解系统设计
- 实现VirusGameController（塔防小游戏）
- 准备进入Phase 7（社交系统）

**如有问题**，请查看：
- Console中的详细日志
- PHASE6_SUMMARY.md的常见问题部分
- 在代码中添加Debug.Log调试

---

**祝游戏开发顺利！** 🚀
