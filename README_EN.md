[简体中文](./README.md) ｜ [English](./README_EN.md)

# IM Unity UIKit & UIKit Demo
IM for Unity on iOS or Android.
This Chat Unity UIKit & UIKit Demo is a game scene UI component library based on Tencent Cloud IM Chat SDK. It currently includes Conversation and Chat components with sending and receiving text messages, sending and receiving emoji messages, Custom emoticons and other functions. Introducing this UIKit in your Unity project can help you quickly build your chat system.
For more information about Tencent Cloud Instant Messaging IM, please refer to [Tencent Cloud Chat](https://cloud.tencent.com/product/im)

![](https://qcloudimg.tencent-cloud.cn/raw/49726f02f6d943ff1d9f88c13fcf097c.png)

[IM Unity UIKit & UIKit Demo github](https://github.com/TencentCloud/chat-demo-unity)

## 环境要求
|平台 | 版本|
|----|----|
|Unity | 2019.4.15f1 and above|
|Android | Android Studio 3.5 and above, App requires Android 4.1 and above|
|iOS | Xcode 11.0 and above，Please ensure that your project has a valid developer signature certificate.|


## 前提条件
[Signed up](https://www.tencentcloud.com/document/product/378/17985?from=unity) for a Tencent Cloud account and completed [identity verification](https://www.tencentcloud.com/document/product/378/3629?from=unity).
1. Created a chat application as instructed in [Creating and Upgrading an Application](https://www.tencentcloud.com/document/product/1047/34577?from=unity) and recorded the SDKAppID.
>The same Tencent Cloud account can create up to 300 instant messaging IM applications. If there are already 300 applications, you can [deactivate and delete](https://www.tencentcloud.com/document/product/1047/34540?lang=en&pg=) the unused applications before creating new ones . **After the application is deleted, all data and services corresponding to the SDKAppID cannot be recovered, please operate with caution.*
![](https://main.qcloudimg.com/raw/15e61a874a0640d517eeb67e922a14bc.png)
1. Record the SDKAppID. You can view the status, business version, SDKAppID, label, creation time, and expiration time of the newly created application on the console overview page.
    ![](https://main.qcloudimg.com/raw/7954cc2882d050f68cd5d1df2ee776a6.png)
2. Click the created application, click **Auxiliary Tools**>**UserSig Generation & Verification** in the left navigation bar, create a UserID and its corresponding UserSig, copy the signature information, and use it for subsequent logins.
![](https://main.qcloudimg.com/raw/2286644d987d24caf565142ae30c4392.png)

## How to import UIKit into the project

#### import AssetPackage
1. Create/start an existing Unity project.
2. Add dependencies in the Packages/manifest.json file:
```json
   {
     "dependencies": {
       "com.tencent.imsdk.unity": "https://github.com/TencentCloud/chat-sdk-unity.git#unity"
     }
   }
```
1. Download the chat-uikit-unity.unitypackage under the [UIKit github](https://github.com/TencentCloud/chat-demo-unity) directory, and import the resource package.

#### Initialize and log in
There are two ways to initialize and log in to IM:

Outside the component: the entire application just needs to initialized and logged in once.
Inside the component(Recommanded): pass parameters into the component through configuration. UIKit has bound corresponding event callbacks for you, including events for receiving new messages and events for updating the session list.

##### Method 1: External between groups
Initialize IM in the Unity project you created, note that the IM application only needs to be initialized once. This step can be skipped if integrating in an existing IM project.
```csharp
public static void Init() {
        int sdkappid = 0;
        SdkConfig sdkConfig = new SdkConfig();

        sdkConfig.sdk_config_config_file_path = Application.persistentDataPath + "/TIM-Config";

        sdkConfig.sdk_config_log_file_path = Application.persistentDataPath + "/TIM-Log"; // Set local log address

        TIMResult res = TencentIMSDK.Init(long.Parse(sdkappid), sdkConfig);
}

public static void Login() {
  if (userid == "" || user_sig == "")
  {
      return;
  }
  TIMResult res = TencentIMSDK.Login(userid, user_sig, (int code, string desc, string json_param, string user_data)=>{
    // callback after login
  });
```

##### Method 2: inside the component
You can also pass SDKAppID, UserSig, and UserID into the component through configuration to initialize and log in IM. (same as demo)
```csharp
using com.tencent.imsdk.unity.uikit;

public static void Init() {
  Core.SetConfig(sdkappid, userId, sdkUserSig);
  Core.Init();
  Core.Login();
}
```

#### Use Conversation and Chat prefabs

You can put the following prefabs into your scene and modify the corresponding styles and layouts.
<p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/4aef3fda8f145f82041b46d419aa5d8e.png" width="80%">
    </p>


#### Structure

**Assets/Example**
This directory corresponds to the content displayed when the actual project is running, including two pages of Scenes, the corresponding codes are `Main.cs (login interface)` and `Chat.cs (chat interface)`.
- Chat contains single chat and group chat content, you can get the conversation (friends) list and send text and emoticon messages. The content in Chat is composed of components in `Prefabs`, you can modify the display content and style by modifying `Prefabs`.


**Assets/Prefabs**
The following components can be used in combination (refer to the Chat page of Scenes), or the components can be modified and used separately according to requirements.

- ChatPanel
     message history list
     - Message display area `ConvMessagePanel`
       - ConversationNamePanel `ConversationNamePanel`
       - Historical message display area `MessageContentPanel`
     - message input area `ActionPanel`
     - Emoticons area `OverlayPanel`
     - Close chat window button `CloseButton`
<p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/46ed11693a67410f367d40aefd60429f.png" width="60%">
    </p>


-ConversationPanel
   conversation list. Now it mainly displays the single-chat sessions of friends. The corresponding code is in `Script/Components/Concersation.cs`. Styles for each conversation are in `ConversationItem.prefabs`.
   - conversation list area `FriendPanel`
     - Search area `SearchPanel`
     - Conversation list `ConversationListPanel`
<p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/5f76276be14a45acd7aad426ff1cd7f6.png" width="60%">
    </p>

- ChannelPanel
     The channel list consists of 4 channel buttons, namely `World`, `Channel`, `Team`, `Friends`. The first three channels are group chat channels, and the friend channel is a single chat channel and will display a list of single chat sessions. Click events and styles for channel buttons are in `Script/Components/Chat.cs`.
-AvatarPanel
   The avatar style in a conversation (ConversationItem), a single chat record (messageItem, etc.). Contains avatars and segment avatars.
-ConversationItem
   The session style of the session list, including the avatar (AvatarPanel), session name and rank.
- MessageItem, MessageItemSelf
     Text message content. Separate text messages for others and text messages for yourself.
     - Avatar area `MessageSenderPanel`
     - Message area `MessageContentPanel`
       - Sender information area `SenderNamePanel`
         - sender name `MessageSender`
         - Sender segment Icon and name `Icon` and `Text`
       - message body `Panel`
-StickerMessageItem,StickerMessageSelf
     The content of the emoji message. The content is the same as MessageItem
-GroupTipItem
   Group reminder message content, for users to enter the group, withdraw from the group, admin messages, etc. Contains group name and message body.
-TimeStamp
     Time nodes in historical messages.
- StickerItem, MenuItem
     They are emoticons and emoticons in the shortcut menu respectively.


## ## How to start the demo project

### Initialize login
Pass the SDKAppID, UserSig, and UserID into the component through configuration to initialize and log in the IM.
**Note: the entire project only needs to be initialized once**
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

Open the Chat page directly after initial login.


### Channel
The demo is divided into four channels: `World`, `Channel`, `Team`, and `Friends`. Among them, the `Friends` channel displays a list of C2C sessions and added friends, click on a session to start chatting.
The other three channels are group conversations. If you need to send messages in this channel, you need to create a group first and add its ID to the project.

#### Create groups
**Added via RestAPI**
You can create a group through `create_group` in the background RestAPI. See [Link](https://www.tencentcloud.com/document/product/1047/34895) for details.
**add in console**
You can also create groups through the console. Go to your IM application in the console -> Group Management -> Add Group.

#### Add group to channel
Enter `Assets/Example/Scripts/Config/Config.cs`, fill in the group ID of the created group into `communityID (community)`, `channelID (channel)`, `groupID (team)`.
And call `joinGroup` after login to enter the corresponding group after login and send messages in the group.

### Send a message
If you have added a group to the channel, you can send group chat messages through the world, channel, and team channel.
You can also click on a single chat session in the friend channel to send a single chat message.


## Modify emoticons and Rank information

#### Rank
Now each user rank is randomly generated, if you need to use rank information, you can set it in the user's custom field.
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
And display the corresponding rank icon according to the rank name.
1. Load the icon or avatar frame corresponding to the rank into Resources. (If you use Url to get it, you can ignore this step)
2. Modify the display of the avatar frame and icon in the code. The parts that need to be modified are the conversation list and the message list
    1. Session list
       1. Supplement the acquired rank information in the session acquisition function `completeConvList`. The final friend session information displayed is in the `friendProfiles` list
       2. Modify the rendered icons and avatars in `GenerateList (conversation list rendering)` in `Conversation.cs`
    2. Message list
       1. Obtain the segment information in the message sender's information in `RenderMessageForScroll` of `Chat.cs` (if you need to modify other display content, you can also get it from here)
       2. Modify the displayed style and other details in `MsgItem.cs`

#### Emoticons
Emoticons are displayed in `OverlayPanel` in `Chat.cs` using `StickerPanel`. You can import your own emoticons to use. (You need to import your own emoticons in advance)
1. Import the emoticons used in the `Assets/Resources` folder
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/ea516e9b19793282a49c81570d17c559.png">
    </p>
2. Change the `Texture Type` of the image to `Sprite (2D and UI)`, and modify the `Pixels Per Unit` according to the size of the image
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/d5cad0548b08be9413a7e3a92ed0c956.png">
    </p>
3. Define the corresponding emoticon package data
   ```csharp
      // Generate a list of emoticons, StickerPackage is a set of emoticons
      List<StickerPackage> stickers = new List<StickerPackage> {
      new StickerPackage {
        name = "4350",
        baseUrl = "custom_sticker_resource/4350", 
        menuItem = new StickerItem {
          name = "menu@2x",
          index = 0,
        },
        stickerList = new List<StickerItem> {
          new StickerItem { // emoticon package data
          name = "menu@2x",
          index = 0 
        },
        }
      }
    };
   ```
4. Album emoticons for UIKit
   ```csharp
   using com.tencent.imsdk.unity.uikit;

      Core.SetStickerPackageList(Config.stickers);
   ```

## API 文档

[Tencent Cloud IM Chat SDK document](https://github.com/TencentCloud/chat-demo-unity)
[Tencent Cloud IM Chat SDK website](https://cloud.tencent.com/document/product/269/54111)
[Tencent Cloud IM Chat SDK Get Started](https://cloud.tencent.com/document/product/269/54106)

### SetConfig

Pass Config information before Init, including `sdkappid`, `userid` and `usersig`.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetConfig(sdkappid, userid, usersig);
```

### Init

Use the Init method provided by UIKit to initialize the SDK, and the `AddRecvNewMsgCallback` and `SetConvEventCallback` callbacks will be automatically bound.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.Init();
```

### SetStickerPackageList

Set sticker package list through `SetStickerPackageList`.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetStickerPackageList(Config.stickers);
```

### Login

Log in to the account through `Login`, and execute the bound callback function after the login is completed.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.Login((params string[] args) => {
      });
```

### SetMessageList

Add the message list of a session, merge it into the current session message dictionary after processing, and trigger the `OnMsgListChanged` event.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(currentConvID, newMsgList, isFinished);
```

### SetCurrentConv

Set the currently selected session and fire the `OnCurrentConvChanged` event.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(convID, convType);
```

### SetCurrentStickerIndex

Set the currently selected sticker group and trigger `OnCurrentStickerIndexChanged` event.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(stickerIndex);
```

### Logout

Log out and clear data.

```c#
   using com.tencent.imsdk.unity.uikit;

      Core.Logout((string[] parameters) => {
        // Logout callback
      });
```

## TencentIMSDK

[Unity TencentIMSDK](https://cloud.tencent.com/document/product/269/54106) Provides comprehensive instant communication capabilities based on the Unity platform. You can use `TencentIMSDK` to get other instant messaging related functions. For example, get user information through `TencentIMSDK`

```c#
using com.tencent.imsdk.unity;

    FriendShipGetProfileListParam param = new FriendShipGetProfileListParam
    {
      friendship_getprofilelist_param_identifier_array = new List<string>
      {
        "self_userid"
      }
    };

    TIMResult res = TencentIMSDK.ProfileGetUserProfileList(param, (int code, string desc, List<UserProfile> profile, string user_data)=>{
    });
```

## Communication and Feedback

If you have any questions during access and use, you can enter the Unity platform of [Tencent Cloud Instant Messaging IM ZhiLiao](https://zhiliao.qq.com/).