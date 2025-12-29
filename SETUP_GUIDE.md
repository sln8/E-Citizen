# 《电子公民》游戏开发 - Unity与Firebase配置指南

## 目录
1. [Unity项目配置](#unity项目配置)
2. [Firebase项目创建](#firebase项目创建)
3. [Firebase SDK安装](#firebase-sdk安装)
4. [Google登录配置](#google登录配置)
5. [Facebook登录配置](#facebook登录配置)
6. [Apple登录配置](#apple登录配置)
7. [测试账号系统配置](#测试账号系统配置)
8. [测试和验证](#测试和验证)

---

## Unity项目配置

### 步骤1: 打开Unity项目
1. 打开Unity Hub
2. 点击"打开"按钮
3. 选择项目文件夹：`E-Citizens`
4. 等待Unity加载项目（首次打开可能需要几分钟）

### 步骤2: 检查项目设置
1. 在Unity中，点击菜单 `Edit` -> `Project Settings`
2. 在左侧选择 `Player`
3. 检查以下设置：
   - **Company Name**: 输入你的公司名称（例如：`YourCompany`）
   - **Product Name**: `E-Citizen` 或 `电子公民`
   - **Package Name**: 
     - Android: `com.yourcompany.ecitizen`
     - iOS: `com.yourcompany.ecitizen`

### 步骤3: 配置Android平台
1. 在 `Project Settings` -> `Player` 中
2. 点击Android标签（小机器人图标）
3. 展开 `Other Settings`
4. 设置以下内容：
   - **Minimum API Level**: 至少选择 `API Level 21 (Android 5.0)`
   - **Target API Level**: 选择最新版本
   - **Scripting Backend**: 选择 `IL2CPP`
   - **Target Architectures**: 勾选 `ARM64`

### 步骤4: 配置iOS平台（如果需要）
1. 在 `Project Settings` -> `Player` 中
2. 点击iOS标签（苹果图标）
3. 展开 `Other Settings`
4. 设置以下内容：
   - **Target minimum iOS Version**: 至少 `11.0`
   - **Camera Usage Description**: `需要相机权限用于拍照上传头像`
   - **Microphone Usage Description**: `需要麦克风权限用于语音功能`

---

## Firebase项目创建

### 步骤1: 创建Firebase项目
1. 打开浏览器，访问 [Firebase Console](https://console.firebase.google.com/)
2. 点击 "添加项目" 或 "Add project"
3. 输入项目名称：`ECitizen` 或 `电子公民`
4. 点击"继续"
5. 选择是否启用Google Analytics（建议启用）
6. 如果启用了Analytics，选择或创建一个Google Analytics账号
7. 点击"创建项目"，等待项目创建完成（约1-2分钟）

### 步骤2: 添加Android应用
1. 在Firebase项目概览页面，点击Android图标
2. 填写以下信息：
   - **Android包名称**: 输入之前在Unity中设置的包名，例如 `com.yourcompany.ecitizen`
   - **应用昵称（可选）**: `E-Citizen Android`
   - **调试签名证书SHA-1（可选）**: 暂时可以跳过，后面配置Google登录时需要
3. 点击"注册应用"
4. 下载 `google-services.json` 文件
5. **重要**: 将 `google-services.json` 文件放到Unity项目的 `Assets/` 文件夹中

### 步骤3: 添加iOS应用（如果需要）
1. 在Firebase项目概览页面，点击iOS图标
2. 填写以下信息：
   - **iOS软件包ID**: 输入之前在Unity中设置的包名，例如 `com.yourcompany.ecitizen`
   - **应用昵称（可选）**: `E-Citizen iOS`
   - **App Store ID（可选）**: 可以暂时跳过
3. 点击"注册应用"
4. 下载 `GoogleService-Info.plist` 文件
5. **重要**: 将 `GoogleService-Info.plist` 文件放到Unity项目的 `Assets/` 文件夹中

### 步骤4: 启用Firebase Authentication
1. 在Firebase Console左侧菜单，点击 `Authentication`
2. 点击 "开始使用" 或 "Get started"
3. 在 "Sign-in method" 标签页中，启用以下登录方式：
   - **电子邮件/密码**: 点击启用（用于测试账号）
   - **Google**: 点击启用，填写项目支持电子邮件
   - **Facebook**: 暂时不启用，后面单独配置
   - **Apple**: 暂时不启用，后面单独配置

### 步骤5: 启用Firebase Firestore
1. 在Firebase Console左侧菜单，点击 `Firestore Database`
2. 点击 "创建数据库" 或 "Create database"
3. 选择模式：
   - **测试模式**: 适合开发阶段（30天后自动关闭公开访问）
   - **生产模式**: 需要配置安全规则
   - 建议选择"测试模式"
4. 选择数据库位置（建议选择离你最近的地区，例如 `asia-east1 (台湾)`）
5. 点击"启用"

### 步骤6: 启用Firebase Storage
1. 在Firebase Console左侧菜单，点击 `Storage`
2. 点击 "开始使用" 或 "Get started"
3. 在安全规则对话框中，选择"测试模式"
4. 选择存储位置（与Firestore相同的地区）
5. 点击"完成"

---

## Firebase SDK安装

### 步骤1: 下载Firebase Unity SDK
1. 访问 [Firebase Unity SDK下载页面](https://firebase.google.com/download/unity)
2. 点击下载最新版本的SDK（建议下载 `firebase_unity_sdk_11.x.x.zip`）
3. 解压下载的zip文件到一个临时文件夹

### 步骤2: 导入Firebase SDK到Unity
1. 回到Unity编辑器
2. 点击菜单 `Assets` -> `Import Package` -> `Custom Package...`
3. 浏览到解压的Firebase SDK文件夹
4. 按顺序导入以下包（每次导入一个）：
   - `FirebaseAuth.unitypackage` （认证功能，必需）
   - `FirebaseFirestore.unitypackage` （数据库功能，必需）
   - `FirebaseStorage.unitypackage` （存储功能，必需）
   - `FirebaseAnalytics.unitypackage` （分析功能，可选）
5. 每次导入时，在弹出的对话框中点击"Import"按钮
6. 等待Unity完成导入（可能需要几分钟）

### 步骤3: 解决依赖问题
1. 导入完成后，Unity可能会提示安装Android Resolver
2. 如果弹出对话框，点击"Install"或"Resolve"
3. 等待依赖解析完成
4. 如果没有自动弹出，可以手动解析：
   - 点击菜单 `Assets` -> `External Dependency Manager` -> `Android Resolver` -> `Resolve`

### 步骤4: 验证安装
1. 在Unity的Project窗口中，检查是否有以下文件夹：
   - `Assets/Firebase/`
   - `Assets/Plugins/`
2. 在Console窗口中，不应该有红色的错误信息
3. 如果有错误，记录错误信息，可能需要重新导入或更新SDK

---

## Google登录配置

### 步骤1: 获取SHA-1指纹
1. 打开命令行工具（Windows: CMD，Mac: Terminal）
2. 导航到你的项目文件夹
3. 运行以下命令获取调试证书的SHA-1：

**Windows:**
```bash
"%JAVA_HOME%\bin\keytool" -list -v -keystore "%USERPROFILE%\.android\debug.keystore" -alias androiddebugkey -storepass android -keypass android
```

**Mac/Linux:**
```bash
keytool -list -v -keystore ~/.android/debug.keystore -alias androiddebugkey -storepass android -keypass android
```

4. 在输出中找到 `SHA1:` 开头的行，复制这个SHA-1指纹

### 步骤2: 在Firebase中添加SHA-1
1. 回到Firebase Console
2. 点击项目设置（齿轮图标）-> `项目设置`
3. 找到你的Android应用
4. 在"SHA证书指纹"部分，点击"添加指纹"
5. 粘贴你复制的SHA-1指纹
6. 点击"保存"

### 步骤3: 重新下载配置文件
1. 在Firebase Console的项目设置页面
2. 找到Android应用部分
3. 点击"下载google-services.json"
4. 用新下载的文件替换Unity项目 `Assets/` 文件夹中的旧文件

### 步骤4: 配置Google Sign-In
1. 在Firebase Console，进入 `Authentication` -> `Sign-in method`
2. 找到Google登录方式，点击编辑
3. 确认已启用
4. 填写"项目的公开名称"（例如：`电子公民`）
5. 填写"项目支持电子邮件"（你的邮箱）
6. 点击"保存"

---

## Facebook登录配置

### 步骤1: 创建Facebook应用
1. 访问 [Facebook开发者平台](https://developers.facebook.com/)
2. 登录你的Facebook账号
3. 点击"我的应用"
4. 点击"创建应用"
5. 选择应用类型：选择"游戏"或"消费者"
6. 填写应用信息：
   - **应用名称**: `电子公民` 或 `E-Citizen`
   - **应用联系邮箱**: 你的邮箱
7. 点击"创建应用"

### 步骤2: 获取Facebook App ID和App Secret
1. 在Facebook应用面板，点击左侧的"设置" -> "基本"
2. 找到以下信息：
   - **应用编号（App ID）**: 复制这个编号
   - **应用密钥（App Secret）**: 点击"显示"按钮，然后复制

### 步骤3: 配置Android平台
1. 在Facebook应用面板，点击左侧的"设置" -> "基本"
2. 滚动到底部，点击"+ 添加平台"
3. 选择"Android"
4. 填写以下信息：
   - **Google Play软件包名称**: `com.yourcompany.ecitizen`（与Unity中的包名一致）
   - **类名**: `com.unity3d.player.UnityPlayerActivity`
   - **密钥散列**: 需要生成，参考下一步
5. 点击"保存更改"

### 步骤4: 生成Facebook密钥散列
1. 打开命令行工具
2. 运行以下命令：

**Windows:**
```bash
"%JAVA_HOME%\bin\keytool" -exportcert -alias androiddebugkey -keystore "%USERPROFILE%\.android\debug.keystore" | openssl sha1 -binary | openssl base64
```

**Mac/Linux:**
```bash
keytool -exportcert -alias androiddebugkey -keystore ~/.android/debug.keystore | openssl sha1 -binary | openssl base64
```

3. 输入密码：`android`
4. 复制输出的密钥散列
5. 在Facebook应用设置中，粘贴到"密钥散列"字段

### 步骤5: 在Firebase中启用Facebook登录
1. 回到Firebase Console
2. 进入 `Authentication` -> `Sign-in method`
3. 找到Facebook，点击它
4. 启用Facebook登录
5. 填写从Facebook获取的：
   - **应用编号**: 粘贴Facebook App ID
   - **应用密钥**: 粘贴Facebook App Secret
6. 复制Firebase提供的"OAuth重定向URI"
7. 点击"保存"

### 步骤6: 在Facebook中添加OAuth重定向URI
1. 回到Facebook应用面板
2. 点击左侧的"Facebook登录" -> "设置"
3. 在"有效OAuth跳转URI"中，粘贴从Firebase复制的URI
4. 点击"保存更改"

---

## Apple登录配置

**注意**: Apple登录仅适用于iOS平台，且需要Apple开发者账号（$99/年）

### 步骤1: 配置Apple开发者账号
1. 登录 [Apple Developer](https://developer.apple.com/)
2. 进入"Certificates, Identifiers & Profiles"
3. 点击左侧的"Identifiers"
4. 找到你的应用Bundle ID（例如：`com.yourcompany.ecitizen`）
5. 如果还没有，点击"+"创建新的App ID
6. 确保勾选了"Sign In with Apple"功能

### 步骤2: 创建Service ID
1. 在Apple Developer页面，点击"Identifiers"
2. 点击右上角的"+"按钮
3. 选择"Services IDs"
4. 填写信息：
   - **Description**: `E-Citizen Sign In`
   - **Identifier**: `com.yourcompany.ecitizen.signin`
5. 勾选"Sign In with Apple"
6. 点击"Configure"
7. 添加域名和返回URL（从Firebase获取）
8. 点击"Continue"和"Register"

### 步骤3: 在Firebase中启用Apple登录
1. 回到Firebase Console
2. 进入 `Authentication` -> `Sign-in method`
3. 找到Apple，点击它
4. 启用Apple登录
5. 如果需要，配置OAuth代码流程
6. 点击"保存"

### 步骤4: 在Unity中配置
1. 打开Unity项目
2. 进入 `Project Settings` -> `Player` -> iOS标签
3. 在"Signing"部分，输入你的Team ID
4. 确保Bundle Identifier与Apple Developer中配置的一致

---

## 测试账号系统配置

### 步骤1: 在Firebase中创建测试用户
1. 打开Firebase Console
2. 进入 `Authentication` -> `Users`
3. 点击"添加用户"按钮
4. 填写测试账号信息：
   - **电子邮件**: `test@ecitizen.com`
   - **密码**: `test123456`（至少6位）
5. 点击"添加用户"
6. 重复此步骤，创建多个测试账号（可选）

### 步骤2: 在Unity中配置测试模式
1. 打开Unity编辑器
2. 在Hierarchy窗口中，找到或创建包含 `FirebaseConfig` 组件的GameObject
3. 在Inspector窗口中，检查以下设置：
   - **Enable Firebase**: 勾选
   - **Simulate In Editor**: 勾选（在编辑器中测试时使用模拟模式）
   - **Enable Test Accounts**: 勾选
   - **Is Test Mode**: 勾选（测试模式下会绕过真实支付）

### 步骤3: 测试账号功能说明
在游戏的登录界面，测试账号系统提供以下功能：
1. **手动输入测试账号**: 
   - 输入测试账号（例如：`test_user_001`）
   - 输入密码（至少6位）
   - 点击"测试账号登录"按钮

2. **快速创建测试账号**:
   - 点击"创建测试账号"按钮
   - 系统会自动生成一个随机测试账号并登录
   - 账号格式：`test_user_XXXXX`（XXXXX是随机数字）

---

## 测试和验证

### 步骤1: 在Unity编辑器中测试
1. 打开Unity编辑器
2. 在Project窗口中，打开 `Assets/Scenes/` 文件夹
3. 双击打开主场景（如果还没有场景，需要先创建登录场景）
4. 点击Unity顶部的播放按钮（▶️）
5. 在Game窗口中测试登录功能：
   - 尝试点击"Google登录"（应该显示模拟登录成功）
   - 尝试点击"Facebook登录"（应该显示模拟登录成功）
   - 尝试点击"Apple登录"（应该显示模拟登录成功）
   - 尝试输入测试账号登录

### 步骤2: 查看Console日志
1. 在Unity中，打开Console窗口（菜单 `Window` -> `General` -> `Console`）
2. 观察日志输出：
   - 应该看到 "Firebase初始化成功" 的绿色日志
   - 登录时应该看到详细的登录流程日志
   - 不应该有红色的错误信息

### 步骤3: 在Android设备上测试
1. 连接Android设备到电脑
2. 在Unity中，点击菜单 `File` -> `Build Settings`
3. 选择Android平台
4. 点击"Switch Platform"（如果需要）
5. 点击"Build And Run"
6. 选择保存位置，Unity会自动构建并安装到设备
7. 在设备上测试所有登录方式

### 步骤4: 在Firebase Console中验证
1. 打开Firebase Console
2. 进入 `Authentication` -> `Users`
3. 测试登录后，应该能看到新用户出现在列表中
4. 进入 `Firestore Database`
5. 检查是否创建了用户数据文档

---

## 常见问题和解决方案

### 问题1: Firebase初始化失败
**症状**: Console显示"Firebase初始化失败"或相关错误

**解决方案**:
1. 确认 `google-services.json`（Android）或 `GoogleService-Info.plist`（iOS）文件在 `Assets/` 文件夹中
2. 检查包名是否与Firebase控制台中配置的一致
3. 重新导入Firebase SDK
4. 重启Unity编辑器

### 问题2: Google登录失败
**症状**: 点击Google登录后没有反应或显示错误

**解决方案**:
1. 确认已在Firebase控制台启用Google登录
2. 检查SHA-1指纹是否正确配置
3. 重新下载 `google-services.json` 文件
4. 在真机上测试（编辑器中使用模拟模式）

### 问题3: Facebook登录失败
**症状**: Facebook登录弹出窗口后失败

**解决方案**:
1. 确认Facebook App ID和App Secret正确填写到Firebase
2. 检查OAuth重定向URI是否正确配置
3. 确认Facebook应用状态为"开发"或"上线"
4. 检查Facebook密钥散列是否正确

### 问题4: 编译错误
**症状**: Unity无法编译或显示大量错误

**解决方案**:
1. 确保导入了所有必需的Firebase包
2. 运行Android Resolver: `Assets` -> `External Dependency Manager` -> `Android Resolver` -> `Resolve`
3. 清除编译缓存: `Assets` -> `External Dependency Manager` -> `Android Resolver` -> `Delete Resolved Libraries`
4. 重启Unity编辑器

---

## 下一步

完成以上所有配置后，你已经成功搭建了游戏的基础架构！

接下来的开发阶段：
1. **Phase 2**: 核心资源系统（内存、CPU、网速等）
2. **Phase 3**: 工作系统和技能系统
3. **Phase 4**: 公司系统和人才市场
4. **Phase 5**: 生活系统（房产、汽车、宠物）
5. **Phase 6**: 娱乐系统和病毒入侵玩法

每个阶段都会提供详细的代码和配置指南！

---

## 技术支持

如果遇到问题：
1. 查看Console窗口的详细错误信息
2. 参考Firebase官方文档: https://firebase.google.com/docs/unity/setup
3. 记录错误信息和复现步骤，方便排查问题

祝开发顺利！🎮
