[简体中文](./README_CN.md) ｜ [English](./README.md)

# IM(Chat) Unity UIKit & UIKit Demo
Chat for Unity on iOS or Android.
This Chat Unity UIKit & UIKit Demo is a game scene UI component Kit based on Tencent Cloud IM Chat SDK. It currently includes Conversation and Chat components with sending and receiving text messages,emoji messages, Custom emoticons and other functions. Adding this UIKit in your Unity project can help you quickly build your chat system.
For more information about Tencent Cloud Instant Messaging IM, please refer to [Tencent Cloud Chat](https://cloud.tencent.com/product/im)

![](https://qcloudimg.tencent-cloud.cn/raw/2864b976411750209121f92f0d2eb7dd.jpg)

[Chat Unity UIKit & UIKit Demo github](https://github.com/TencentCloud/chat-demo-unity)
[Chat Demo](https://www.tencentcloud.com/document/product/1047/34279)

## Environmental requirements
|Platform | version|
|----|----|
|Unity | 2019.4.15f1 and above|
|Android | Android Studio 3.5 and above, App requires Android 4.1 and above|
|iOS | Xcode 11.0 and above，Please ensure that your project has a valid developer signature certificate.|


## Perquisites
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

##### Method 1: Outside the component
Initialize IM in the Unity project you created. Note that the IM application only needs to be initialized once. This step can be skipped if integrating in an existing IM project.
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
}
```

##### Method 2: Inside the component
You can also pass SDKAppID, UserSig, and UserID into the component through configuration to initialize and log in. (same as demo)
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
- Chat contains C2C chat and group chat, you can get the conversation (friends) list and send text/emoticon messages. The content in Chat is composed of components in `Prefabs`, you can modify the display content and style by modifying `Prefabs`.


**Assets/Prefabs**
The following components can be used together (refer to the Chat page of Scenes), or the components can be modified and used separately according to requirements.

- ChatPanel
     message list.
     - Message display area `ConvMessagePanel`
       - Conversation name display area `ConversationNamePanel`
       - Historical message display area `MessageContentPanel`
     - message input area `ActionPanel`
     - Emoticons area `OverlayPanel`
     - Close chat window button `CloseButton`
<p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/46ed11693a67410f367d40aefd60429f.png" width="60%">
    </p>


- ConversationPanel
   conversation list. It mainly displays the C2C conversations of friends. The corresponding code is in `Script/Components/Concersation.cs`. Styles for each conversation are in `ConversationItem.prefabs`.
   - conversation list area `FriendPanel`
     - Search area `SearchPanel`
     - Conversation list `ConversationListPanel`
<p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/5f76276be14a45acd7aad426ff1cd7f6.png" width="60%">
    </p>

- ChannelPanel
     The channel list consists of 4 channel buttons, namely `World`, `Channel`, `Team`, `Friends`. The first three channels are group chat channels, and the friend channel is a C2C channel and will display a list of C2C chat conversations. Click events and styles for channel buttons are in `Script/Components/Chat.cs`.
- AvatarPanel
   The avatar style in a conversation (ConversationItem), a chat record (messageItem, etc.). Contains avatars and rank avatars.
- ConversationItem
   The conversation style of the conversation list, including the avatar (AvatarPanel), conversation name and rank.
- MessageItem, MessageItemSelf
     Text message content. Text message from others and from self is seperated.
     - Avatar area `MessageSenderPanel`
     - Message area `MessageContentPanel`
       - Sender information area `SenderNamePanel`
         - sender name `MessageSender`
         - Sender rank Icon and name `Icon` and `Text`
       - message body `Panel`
-StickerMessageItem,StickerMessageSelf
     The content of the emoji message. The content is the same as MessageItem
-GroupTipItem
   Group tips message content, for users to enter the group, withdraw from the group, admin messages, etc. Contains group name and message body.
-TimeStamp
     Time nodes in historical messages.
- StickerItem, MenuItem
     They are emoticons and emoticons in the shortcut menu respectively.


## How to start the demo project

### Initialize login
Pass the SDKAppID, UserSig, and UserID into the component through configuration to initialize and log in the IM.
**Note: the entire project only needs to be initialized once**
```csharp
using com.tencent.imsdk.unity.uikit;

public static void Init() {
  Core.SetConfig(sdkappid, userId, sdkUserSig);
  Core.Init();
  Core.Login();
  // you can pass function
  // Core.Login(HandleAfterLogin);
}
```

Open the Chat page directly after initial login.


### Channel
The demo is divided into four channels: `World`, `Channel`, `Team`, and `Friends`. Among them, the `Friends` channel displays a list of C2C conversations and added friends, click on a conversation to start chatting.
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
If you have added a group to the channel, you can send group chat messages through the World, Channel, and team channel.
You can also click on a c2c conversation in the friend channel to send a c2c chat message.


## Modify emoticons and Rank information

#### Rank
In this demo, each user's rank is randomly generated, if you need to use rank information, you can set it in the user's custom field.
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
    1. Conversation list
       1. Add the acquired rank information in the conversation acquisition function `completeConvList`. The final friend conversation information displayed is in the `friendProfiles` list
       2. Modify the rendered icons and avatars in `GenerateList (conversation list rendering)` in `Conversation.cs`
    2. Message list
       1. Obtain the rank information in the message sender's information in `RenderMessageForScroll` of `Chat.cs` (if you need to modify other display content, you can also get it from here)
       2. Modify the displayed style and other details in `MsgItem.cs`

#### Emojis
Emojis are displayed in `OverlayPanel` in `Chat.cs` using `StickerPanel`. You can import your own emoji to use. (You need to import your own emojis in advance)
1. Import the emojis used in the `Assets/Resources` folder
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/ea516e9b19793282a49c81570d17c559.png">
    </p>
2. Change the `Texture Type` of the image to `Sprite (2D and UI)`, and modify the `Pixels Per Unit` according to the size of the image
    <p align="center">
      <img src="https://qcloudimg.tencent-cloud.cn/raw/d5cad0548b08be9413a7e3a92ed0c956.png">
    </p>
3. Define the corresponding emoji package data
   ```csharp
      // Generate a list of emojis, StickerPackage is a set of emojis
      List<StickerPackage> stickers = new List<StickerPackage> {
      new StickerPackage {
        name = "4350",
        baseUrl = "custom_sticker_resource/4350", 
        menuItem = new StickerItem {
          name = "menu@2x",
          index = 0,
        },
        stickerList = new List<StickerItem> {
          new StickerItem { // emoji package data
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

#### Language Package
IM Unity UIKit Demo provides a language switching system based on the system language, and supports Simplified Chinese and English. You can add languages or modify the configuration inside according to your needs.

1. Language files
    Language data is placed in `Resources/LanguageTxt`. Now contains `Chinese.txt(Simplified Chinese)` and `English.txt(English)` for simplified Chinese and English. If you need other languages, you can add the corresponding txt file.
    The structure of the file is as follows:
    ```json
    //English.txt
    Key: Value

    //Chinese.txt
    Key: value
    ```
    Key should be consistent with the Key of other languages, and consistent with subsequent enum
    Value is the value of the language corresponding to Key
    Use a colon to separate Key and Value
2. Set language
    1. Set language and entry
       If you have added a language, add the corresponding language vocabulary txt file and add a new language in `Language` in `LanguageDataManager.cs`, and add the corresponding Key in `LanguageTextName`.
     2. Load the language files
         ```csharp
         private Dictionary<string,string> EnglishDictionary = new Dictionary<string,string>();
         LoadLanguageTxt(Language. English);
         ```
     3. Component settings (static modification)
       Add the `LanguageUIText(Script)` component to the text component that needs to be set, and select the Key of the word to be displayed. The displayed Key corresponds to the enum in LanguageTextName and the Key in the vocabulary file.
       ![](https://qcloudimg.tencent-cloud.cn/raw/04b53ea5e49b957ea9c5c7346bfb6807.png)
     4. Set language
       To set the language, call `SetCurrentLanguageValue` when the App starts. If you want to fix the language, you can directly assign a value to `currentLanguage` in `LanguageDataManager.cs` (it can be used as the default language). The Demo judges and assigns values according to the system language.
       If the components that need to be modified are not only static components, the simple method is to save the currently used language to config (saved to Core in Demo) and judge and display it in the code.

## API documents

[Tencent Cloud IM Chat SDK document](https://comm.qq.com/im/doc/unity/en/api/readme.html)
[Tencent Cloud IM Chat SDK website](https://cloud.tencent.com/document/product/269/54111)
[Tencent Cloud IM Chat SDK Get Started](https://cloud.tencent.com/document/product/269/54106)

### SetConfig

Pass Config information before Init, including `sdkappid`, `userid` and `usersig`.

```csharp
   using com.tencent.imsdk.unity.uikit;

      Core.SetConfig(sdkappid, userid, usersig);
```

### Init

Use the Init method provided by UIKit to initialize the SDK, and the `AddRecvNewMsgCallback` and `SetConvEventCallback` callbacks will be automatically bound.

```csharp
   using com.tencent.imsdk.unity.uikit;

      Core.Init();
```

### SetStickerPackageList

Set sticker package list through `SetStickerPackageList`.

```csharp
   using com.tencent.imsdk.unity.uikit;

      Core.SetStickerPackageList(Config.stickers);
```

### Login

Log in to the account through `Login`, and execute the bound callback function after the login is completed.

```csharp
   using com.tencent.imsdk.unity.uikit;

      Core.Login((params string[] args) => {
      });
```

### SetMessageList

Add the message list of a session, merge it into the current session message dictionary after processing, and trigger the `OnMsgListChanged` event.

```csharp
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(currentConvID, newMsgList, isFinished);
```

### SetCurrentConv

Set the currently selected session and fire the `OnCurrentConvChanged` event.

```csharp
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(convID, convType);
```

### SetCurrentStickerIndex

Set the currently selected sticker group and trigger `OnCurrentStickerIndexChanged` event.

```csharp
   using com.tencent.imsdk.unity.uikit;

      Core.SetMessageList(stickerIndex);
```

### Logout

Log out and clear data.

```csharp
   using com.tencent.imsdk.unity.uikit;

      Core.Logout((string[] parameters) => {
        // Logout callback
      });
```

## TencentChatSDK

[Unity TencentIMSDK](https://cloud.tencent.com/document/product/269/54106) Provides comprehensive instant communication capabilities based on the Unity platform. You can use `TencentChatSDK` to get other chatting related functions. For example, get user information through `TencentChatSDK`

```csharp
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