简体中文 ｜ [English](./README_EN.md)

# IM Unity UIKit & UIKit Demo
IM for Unity on iOS or Android.
此 IM(Instant Messaging) Unity UIKit & UIKit Demo 是基于Tencent Cloud IM Chat SDK实现的游戏场景业务 UI 组件库，目前包含了会话 (Conversation)和聊天 (Chat)组件，收发文字消息、收发表情包消息、自定义表情包等功能。在您的 Unity 项目下引用此 UIKit 可助您快速搭建您的聊天系统。
有关腾讯云即时通信 IM 的更多内容请参考 [即时通信(IM)](https://cloud.tencent.com/product/im)

![](https://qcloudimg.tencent-cloud.cn/raw/49726f02f6d943ff1d9f88c13fcf097c.png)

[IM Unity UIKit & UIKit Demo 链接](https://github.com/TencentCloud/chat-demo-unity)
[体验 Demo](https://cloud.tencent.com/document/product/269/36852)

## 环境要求
|平台 | 版本|
|----|----|
|Unity | 2019.4.15f1 及以上版本。|
|Android | Android Studio 3.5及以上版本，App 要求 Android 4.1及以上版本设备。|
|iOS | Xcode 11.0及以上版本，请确保您的项目已设置有效的开发者签名。|


## 前提条件
您已 [注册腾讯云](https://cloud.tencent.com/document/product/378/17985) 账号，并完成 [实名认证](https://cloud.tencent.com/document/product/378/3629)。
1. 登录 [即时通信 IM 控制台](https://console.cloud.tencent.com/im)。
>?如果您已有应用，请记录其 SDKAppID 并 [获取密钥信息](#step2)。
>同一个腾讯云账号，最多可创建300个即时通信 IM 应用。若已有300个应用，您可以先 [停用并删除](https://intl.cloud.tencent.com/document/product/1047/34540) 无需使用的应用后再创建新的应用。**应用删除后，该 SDKAppID 对应的所有数据和服务不可恢复，请谨慎操作。**
>
2. 单击**创建新应用**，在**创建应用**对话框中输入您的应用名称，单击**确定**。
![](https://qcloudimg.tencent-cloud.cn/raw/575af1542a58aeb75eb560f38d12fbd1.png)
3. 请保存 SDKAppID 信息。可在控制台总览页查看新建应用的状态、业务版本、SDKAppID、标签、创建时间以及到期时间。
    ![](https://qcloudimg.tencent-cloud.cn/raw/9dacc5ab4915ae45e4b3f29b77fadf8b.png)
4. 单击创建后的应用，左侧导航栏单击**辅助工具**>**UserSig 生成&校验**，创建一个 UserID 及其对应的 UserSig，复制签名信息，后续登录使用。
![](https://qcloudimg.tencent-cloud.cn/raw/488cfa074b5bca64217b98e35ccdc271.png)

## 如何将UIKit导入到项目中

#### 导入AssetPackage
1. 创建/启动已存在的 Unity 项目。
2. 在 Packages/manifest.json 文件中的 dependencies 下添加：
```json
  {
    "dependencies":{
      "com.tencent.imsdk.unity":"https://github.com/TencentCloud/chat-sdk-unity.git#unity"
    }
  }
```
3. 下载 [UIKit github](https://github.com/TencentCloud/chat-demo-unity)目录下的 chat-uikit-unity.unitypackage，并导入资源包。

#### 初始化并登录
初始化并登录 IM 有两种方式:

组件外部: 整个应用初始化并登录一次即可。
组件内部: 通过配置的方式将参数传入组件内部。建议您使用内部登录，UIKit 已帮您绑定了相应的事件回调，包括接收新消息的事件以及会话列表更新的事件。

##### 方法1:组间外部
在您创建的 Unity 项目中初始化 IM, 注意 IM 应用只需初始化一次即可。如若在现有 IM 项目中集成可跳过该步骤。
```csharp
public static void Init() {
        int sdkappid = 0; // 从即时通信 IM 控制台获取应用 SDKAppID。
        SdkConfig sdkConfig = new SdkConfig();

        sdkConfig.sdk_config_config_file_path = Application.persistentDataPath + "/TIM-Config";

        sdkConfig.sdk_config_log_file_path = Application.persistentDataPath + "/TIM-Log"; // 设置本地日志地址

        TIMResult res = TencentIMSDK.Init(long.Parse(sdkappid), sdkConfig);
}

public static void Login() {
  if (userid == "" || user_sig == "")
  {
      return;
  }
  TIMResult res = TencentIMSDK.Login(userid, user_sig, (int code, string desc, string json_param, string user_data)=>{
    // 处理登录回调逻辑
  });
```

##### 方法2:组件内部
您也可将SDKAppID、UserSig、UserID通过配置的方式传入组件内部进行 IM 的初始化和登录。（与demo运行方式相同）
```csharp
using com.tencent.imsdk.unity.uikit;

public static void Init() {
  Core.SetConfig(sdkappid, userId, sdkUserSig);
  Core.Init();
  Core.Login();
}
```

#### 使用 Conversation和Chat预制件

您可将下列预制件放入您的场景中，修改相应样式和layout。

![](https://qcloudimg.tencent-cloud.cn/raw/4aef3fda8f145f82041b46d419aa5d8e.png)

#### 项目结构

**Assets/Example**
该目录对应实际项目运行时显示的内容，包含Scenes的两个页面，分别对应的代码为`Main.cs（登录界面）` 和 `Chat.cs（聊天界面）`。
- Chat 里包含单聊、群聊的内容，可以获取到会话（好友）列表并发送文字、表情包消息。Chat里的内容由 `Prefabs`里的组件构成，可以通过修改 `Prefabs`修改显示内容和样式。


**Assets/Prefabs**
下列组件可以联合使用（参考Scenes的Chat页面），也可根据需求将组件单独修改并使用。

- ChatPanel
    消息历史列表
    - 消息展示区 `ConvMessagePanel`
      - 会话名展示区 `ConversationNamePanel`
      - 历史消息展示区 `MessageContentPanel`
    - 消息输入区 `ActionPanel`
    - 表情包区 `OverlayPanel`
    - 关闭聊天窗口按钮 `CloseButton`
<p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/46ed11693a67410f367d40aefd60429f.png" width="60%">
    </p>


- ConversationPanel
  会话列表。现主要显示好友的单聊会话。相应代码在 `Script/Components/Concersation.cs`里。每个会话的样式在 `ConversationItem.prefabs`里。
  - 会话列表区 `FriendPanel`
    - 搜索区 `SearchPanel`
    - 会话列表 `ConversationListPanel`
<p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/5f76276be14a45acd7aad426ff1cd7f6.png" width="60%">
    </p>

- ChannelPanel
    频道列表，由4个频道按钮组成，分别为`世界`,`频道`,`组队`,`好友`。其中前三个频道为群聊频道，好友频道为单聊频道并会显示单聊会话列表。频道按钮的点击事件和样式在 `Script/Components/Chat.cs`里。
- AvatarPanel
  会话（ConversationItem）、单条聊天记录（messageItem等）里的头像样式。包含头像和段位头像。
- ConversationItem
  会话列表的会话样式，包含头像（AvatarPanel），会话名称以及段位。
- MessageItem、MessageItemSelf
    文字消息内容。分别为他人发送文字消息和自己发送文字消息。
    - 头像区 `MessageSenderPanel`
    - 消息区 `MessageContentPanel`
      - 发送者信息区 `SenderNamePanel`
        - 发送者名字 `MessageSender`
        - 发送者段位Icon和名称 `Icon`和`Text`
      - 消息体 `Panel`
- StickerMessageItem,StickerMessageSelf
    表情包消息内容。内容与MessageItem相同
- GroupTipItem
  群提醒消息内容，为用户进群、退群、admin消息等。包含群名和消息体。
- TimeStamp
    历史消息中的时间节点。
- StickerItem,MenuItem
    分别为表情包和快捷menu里的表情包。


## 如何启动demo项目

### 初始化登录
将SDKAppID、UserSig、UserID通过配置的方式传入组件内部进行 IM 的初始化和登录。
**注意：整个项目只需要初始化一次**
```csharp
using com.tencent.imsdk.unity.uikit;

public static void Init() {
  Core.SetConfig(sdkappid, userId, sdkUserSig);
  Core.Init();
  Core.Login();
  // 可传递函数
  // Core.Login(HandleAfterLogin);
}
```

初始化登录后直接打开Chat页面即可。


### 频道
demo中分 `世界`、`频道`、`组队`、`好友` 四个频道。其中 `好友` 频道显示C2C会话和已填加的好友的列表，点击某个会话可开始聊天。
其他三个频道为群组会话，若需要在该频道发消息则需要先创建群组并将其ID添加到项目中。

#### 创建群组
**通过RestAPI添加**
您可以通过后台 RestAPI中`create_group`创建群组。具体可见 [链接](https://cloud.tencent.com/document/product/269/1615)。
**在控制台添加**
您也可以通过控制台创建群组。进入控制台中您的IM应用 -> 群组管理 -> 添加群组。

#### 将群组添加到频道
进入`Assets/Example/Scripts/Config/Config.cs`, 将创建的群组的群组ID填入`communityID(社群)`,`channelID(频道)`,`groupID(组队)`。
并在登录之后调用`joinGroup`即可实现登录后进入相应群组，并可以在群内发送消息。

### 发送消息
若您有添加群组到频道中，您可以通过世界、频道、组队频道发送群聊消息。
您也可以在好友频道点击某个单聊会话发送单聊消息。


## 修改表情包和段位信息

#### 段位
现各个用户段位为随机生成，若您需要使用段位信息，您可以在用户的自定义字段设置。
```csharp
UserProfileCustemStringInfo teer = new UserProfileCustemStringInfo{
    user_profile_custom_string_info_key:"段位",
    user_profile_custom_string_info_value:"teer"
}
List<UserProfileCustemStringInfo> customArray = new List<UserProfileCustemStringInfo>();
customArray.Add(teer);
TencentIMSDK.ProfileModifySelfUserProfile(new UserProfileItem{
    user_profile_item_custom_string_array:customArray;
});
```
并按照段位的名称显示相应的段位图标。
1. 将段位对应的图标或者头像框加载到Resources里。（若使用Url获取时可忽略这一步）
2. 修改代码中头像框和图标的显示。需要修改的部分为会话列表和消息列表
   1. 会话列表
      1. 在获取会话的函数`completeConvList`中补充获取到的段位信息。最终显示的好友会话信息在 `friendProfiles` 列表中
      2. 在 `Conversation.cs`中的`GenerateList(会话列表渲染)`中修改渲染的图标和头像
   2. 消息列表
      1. 在 `Chat.cs`的 `RenderMessageForScroll`中获取消息发送者的信息中的段位信息（若需要修改其他显示内容，也可以从这里获取）
      2. 在 `MsgItem.cs`中修改显示的样式等细节内容

#### 表情包
表情包使用 `StickerPanel` 显示在 `Chat.cs` 里的 `OverlayPanel` 中。您可以导入自己的表情包使用。（需要您提前导入自己的表情包）
1. 在 `Assets/Resources` 文件夹内导入所用的表情包图片
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/ea516e9b19793282a49c81570d17c559.png">
    </p>
    
2. 更改图片的 `Texture Type` 为 `Sprite (2D and UI)`，并根据图片尺寸修改 `Pixels Per Unit`
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/d5cad0548b08be9413a7e3a92ed0c956.png">
    </p>

3. 定义相应的表情包数据
   ```csharp
      // 生成表情包列表，StickerPackage 为一组表情包
      List<StickerPackage> stickers = new List<StickerPackage> {
      new StickerPackage {
        name = "4350",
        baseUrl = "custom_sticker_resource/4350", //Resource 文件夹内相对路径
        menuItem = new StickerItem { // 表情栏表情项目
          name = "menu@2x",
          index = 0,
        },
        stickerList = new List<StickerItem> { // 表情包项目组
          new StickerItem { // 具体表情包数据
          name = "menu@2x",
          index = 0 // 表情包顺序
        },
        }
      }
    };
   ```
4. 注册表情包给 UIKit
   ```csharp
   using com.tencent.imsdk.unity.uikit;

      Core.SetStickerPackageList(Config.stickers);
   ```
#### 语言包
IM Unity UIKit Demo提供根据系统语言切换语言系统，现支持简体中文和英语。您可以按照需求增加语言或者修改里面的配置。

1. 语言资料
   语言资料放在 `Resources/LanguageTxt`里。现在包含`Chinese.txt(简体中文)`和`English.txt(英文)`。若需要其他语言，可以添加对应的txt文件。
   该文件的结构如下：
   ```json
   //English.txt
   Key:Value

   //Chinese.txt
   Key:值
   ```
   Key应与其他语言的Key一致，并与后续的enum一致
   Value为Key对应的该语言的值
   Key 和 Value之间使用冒号分隔开
2. 设置语言
   1. 设置语言和词条
      若你添加了语言，添加相应的语言词汇txt文件后在 `LanguageDataManager.cs` 中的`Language`中添加新的语言，并在`LanguageTextName`中增加对应的Key。
    2. 加载语言词条文件
        ```csharp
        private Dictionary<string,string> EnglishDictionary = new Dictionary<string,string>();
        LoadLanguageTxt(Language.English);
        ```
    3. 组件设置（静态修改）
      在需要设置的text组件中添加 `LanguageUIText(Script)` component，将需要显示的词的Key选中。改显示的Key对应LanguageTextName中的enum和词汇文件里的Key。
      ![](https://qcloudimg.tencent-cloud.cn/raw/04b53ea5e49b957ea9c5c7346bfb6807.png)
    4. 设置语言
      若要设置语言，在软件开启时调用 `SetCurrentLanguageValue`。若要固定语言，可直接在 `LanguageDataManager.cs`对`currentLanguage`赋值（可当成默认语言）。该Demo根据系统语言判断并赋值。
      若需要修改的组件不仅为静态组件，则简单的方法为将现在使用的语言保存到config中（在Demo中保存到了Core）在代码中判断显示。
## API 文档

[Tencent Cloud IM Chat SDK 文档链接](https://comm.qq.com/im/doc/unity/zh/api/readme.html)
[Tencent Cloud IM Chat SDK 官网链接](https://cloud.tencent.com/document/product/269/54111)
[Tencent Cloud IM Chat SDK 快速入门](https://cloud.tencent.com/document/product/269/54106)

### SetConfig

在 Init 前传入 Config 信息，包括 `sdkappid`, `userid` 以及 `usersig`。

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetConfig(sdkappid, userid, usersig);
```

### Init

采用 UIKit 提供的 Init 方法来初始化 SDK，会自动绑定 `AddRecvNewMsgCallback` 和 `SetConvEventCallback` 回调。

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.Init();
```

### SetStickerPackageList

通过 `SetStickerPackageList` 设定表情包列表。

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetStickerPackageList(Config.stickers);
```

### Login

通过 `Login` 登录账号，登录完成后执行绑定的回调函数。

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.Login((params string[] args) => {
        // 处理Login回调
      });
```

### SetMessageList

添加某个会话的消息列表，处理后合并到当前会话消息字典里，并触发 `OnMsgListChanged` 事件。

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(currentConvID, newMsgList, isFinished);
```

### SetCurrentConv

设置当前选中的会话，并触发 `OnCurrentConvChanged` 事件。

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(convID, convType);
```

### SetCurrentStickerIndex

设置当前选中的表情包组，并触发 `OnCurrentStickerIndexChanged` 事件。

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(stickerIndex);
```

### Logout

登出，并清空数据。

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.Logout((string[] parameters) => {
        // 处理Logout回调
      });
```

## TencentIMSDK

[Unity TencentIMSDK](https://cloud.tencent.com/document/product/269/54106) 提供了基于 Unity 平台的全面的即时通信能力。您可以使用 `TencentIMSDK` 来获取其他即时通信的相关功能。例如通过 `TencentIMSDK` 来获取用户资料

```c#
using com.tencent.imsdk.unity;

    // 获取个人资料
    FriendShipGetProfileListParam param = new FriendShipGetProfileListParam
    {
      friendship_getprofilelist_param_identifier_array = new List<string>
      {
        "self_userid"
      }
    };

    TIMResult res = TencentIMSDK.ProfileGetUserProfileList(param, (int code, string desc, List<UserProfile> profile, string user_data)=>{
      // 处理异步逻辑
    });
```

## 交流与反馈
[点此进入IM社群]((https://zhiliao.qq.com/))，享有专业工程师的支持，解决您的难题