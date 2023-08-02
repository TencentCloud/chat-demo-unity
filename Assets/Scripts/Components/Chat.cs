using System.Diagnostics;
using Com.Tencent.IM.Unity.UIKit;
using com.tencent.imsdk.unity;
using com.tencent.imsdk.unity.types;
using com.tencent.imsdk.unity.enums;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine.SceneManagement;

namespace Com.Tencent.Imsdk.Unity.UIKit
{
  public class Chat : MonoBehaviour
  {
    public GameObject overlayArea;
    public InputField messageInput;
    public Button sendMessage;
    public Button openSticker;
    public Button closeButton;
    public GameObject stickerItem;
    public GameObject menuItem;
    public Transform parent;
    public Transform stickerParent;
    public Transform stickerActionParent;
    private Message lastMsg;
    private string convID;
    public GameObject convName;
    // public GameObject recvMessageButton;
    // new scroll
    public ScrollRect scrollRect;
    public GameObject messageItem;
    public GameObject messageItemSelf;
    public GameObject stickerMessageItem;
    public GameObject stickerMessageItemSelf;
    public GameObject timeStampItem;
    public GameObject groupTipsItem;
    private void Start()
    {
      sendMessage.onClick.AddListener(SendTextMessage);
      openSticker.onClick.AddListener(OpenOverlay);
      closeButton.onClick.AddListener(CloseOverlay);
      Core.OnCurrentConvChanged += GenMsgItemForScroll;
      Core.OnChannelChanged += ChangeInputFieldWidth;
      Core.OnCurrentStickerIndexChanged += OnCurrentStickerIndexChanged;
      Core.OnMsgListChanged = OnMsgListChanged;
      Core.OnStickerPackageChanged += GenerateStickers;
      Core.OnStickerPackageChanged += GenerateStickerActions;
      GenerateStickers();
      GenerateStickerActions();
      scrollRect.onValueChanged.AddListener(OnScrollValueChange);
    }

    public void OnScrollValueChange(Vector2 position)
    {
      // At The Top
      if (position.y >= 1)
      {
        if (!string.IsNullOrEmpty(Core.currentConvID) && Core.convMessageMap.TryGetValue(Core.currentConvID, out MessageListObject msgListObj))
        {
          if (!msgListObj.finished && !msgListObj.loading)
          {
            print("get more list");
            msgListObj.index = 0.99f;
            // Get More Message
            Core.MsgGetMsgList(Core.currentConvID, Core.currentConvType, SetMsgList, lastMsg);
            print(msgListObj.list.Count);
          }
        }
        scrollRect.verticalNormalizedPosition = 1;

      }else{
        if (!string.IsNullOrEmpty(Core.currentConvID) && Core.convMessageMap.TryGetValue(Core.currentConvID, out MessageListObject msgListObj))
        {
          msgListObj.index = position.y;
        }
      }
    }

    private void SetMsgList(params string[] args)
    {
      if (Utils.IsCallbackLegit(args[0]))
      {
        var msgList = Utils.FromJson<List<Message>>(args[2]);
        Core.SetMessageList(Core.currentConvID, msgList, msgList.Count == 0);
      }
    }

    private void OnMsgListChanged(params string[] args)
    {
      var changedConvID = args[0];
      if (changedConvID == Core.currentConvID)
      {
        if (Core.convMessageMap.TryGetValue(Core.currentConvID, out MessageListObject msgListObj))
        {
          msgListObj.index = scrollRect.verticalNormalizedPosition;
        }
        // Render new messages
        // GenMsgItem();
        GenMsgItemForScroll();
      }
    }

    private void HandleMessageSent(params string[] args)
    {
      var msgList = new List<Message> { Utils.FromJson<Message>(args[2]) };

      Core.SetMessageList(Core.currentConvID, msgList);
      scrollRect.verticalNormalizedPosition = 0;
    }

    private void CloseOverlay()
    {
      if (overlayArea == null)
      {
        return;
      }
      overlayArea.SetActive(false);
      closeButton.gameObject.SetActive(false);
    }

    private void ChangeStickerPackage(int index)
    {
      if (index == Core.currentStickerIndex)
      {
        return;
      }
      Core.SetCurrentStickerIndex(index);
      GenerateStickers();
    }

    private void OnCurrentStickerIndexChanged(params string[] args)
    {
      if (stickerActionParent == null)
      {
        return;
      }

      // 更改选中会话样式
      foreach (Transform child in stickerActionParent.transform)
      {
        if (child.GetSiblingIndex() == Core.currentStickerIndex)
        {
          child.GetComponent<Image>().color = new Color32(22, 74, 165, 100);
        }
        else
        {
          child.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }
      }
    }

    private void GenerateStickerActions(params string[] args)
    {
      // var stickerParent = GameObject.Find("StickerList");
      if (stickerActionParent == null)
      {
        return;
      }

      foreach (Transform child in stickerActionParent.transform)
      {
        GameObject.Destroy(child.gameObject);
      }
      for (int i = 0; i < Core.stickers.Count; i++)
      {
        var stickerPackage = Core.stickers[i];

        var menuItemPath = !string.IsNullOrEmpty(stickerPackage.menuItem.url) ? stickerPackage.menuItem.url : (stickerPackage.baseUrl + "/" + stickerPackage.menuItem.name);

        var sprite = Resources.Load(menuItemPath, typeof(Sprite));

        var obj = Instantiate(menuItem, stickerActionParent.transform);

        obj.SetActive(true);
        obj.transform.Find("ButtonSticker").GetComponent<Image>().sprite = (Sprite)sprite;

        if (i == Core.currentStickerIndex)
        {
          obj.GetComponent<Image>().color = new Color32(26, 46, 73, 95);
        }
        else
        {
          obj.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        }

        var index = i;
        obj.GetComponent<Button>().onClick.AddListener(() =>
      {
        ChangeStickerPackage(index);
      });
      }
    }

    private void GenerateStickers(params string[] args)
    {
      // var stickerParent = GameObject.Find("StickerList");
      if (stickerParent == null)
      {
        return;
      }

      foreach (Transform child in stickerParent.transform)
      {
        GameObject.Destroy(child.gameObject);
      }

      if (Core.stickers.Count < 1)
      {
        return;
      }

      for (int i = 0; i < Core.stickers[Core.currentStickerIndex].stickerList.Count; i++)
      {
        var sticker = Core.stickers[Core.currentStickerIndex].stickerList[i];

        var path = !string.IsNullOrEmpty(sticker.url) ? sticker.url : (Core.stickers[Core.currentStickerIndex].baseUrl + "/" + sticker.name);

        var sprite = Resources.Load(path, typeof(Sprite));

        var obj = Instantiate(stickerItem, stickerParent.transform);
        obj.SetActive(true);
        obj.GetComponent<Image>().sprite = (Sprite)sprite;
        obj.GetComponent<Button>().onClick.AddListener(() =>
      {
        SendFaceMessage(path, sticker.index);
        CloseOverlay();
      });
      }
    }

    private void OpenOverlay()
    {
      overlayArea.SetActive(true);
      closeButton.gameObject.SetActive(true);
    }

    private void SendFaceMessage(string path, int index)
    {
      if (string.IsNullOrEmpty(Core.currentConvID))
      {
        return;
      }
      var message = new Message
      {
        message_conv_type = Core.currentConvType,
        message_cloud_custom_str = "unity uikit",
        message_elem_array = new List<Elem>{new Elem
      {
        elem_type = TIMElemType.kTIMElem_Face,
        face_elem_index = index,
        face_elem_buf = path
      }},
      };
      StringBuilder messageId = new StringBuilder(128);
      TencentIMSDK.MsgSendMessage(Core.currentConvID, Core.currentConvType, message, messageId, Utils.HandleStringCallback(HandleMessageSent));
    }

    private void SendTextMessage()
    {
      if (string.IsNullOrEmpty(Core.currentConvID) || string.IsNullOrEmpty(messageInput.text))
      {
        return;
      }
      var message = new Message
      {
        message_conv_type = Core.currentConvType,
        message_cloud_custom_str = "unity uikit",
        message_elem_array = new List<Elem>{new Elem
      {
        elem_type = TIMElemType.kTIMElem_Text,
        text_elem_content = messageInput.text
      }},
      };
      StringBuilder messageId = new StringBuilder(128);
      TencentIMSDK.MsgSendMessage(Core.currentConvID, Core.currentConvType, message, messageId, Utils.HandleStringCallback(HandleMessageSent));
      messageInput.text = "";
    }


    private void ChangeInputFieldWidth(params string[] args){
      print("changeInputWidth");
      RectTransform transform  = messageInput.transform.GetComponent<RectTransform>();
      print(Core.isFriendList);
      if(!Core.isFriendList){
        transform.sizeDelta = new Vector2(902.26f,64.5f);
      }else{
        transform.sizeDelta = new Vector2(544.89f,64.5f);
        print(Core.firstFriend.convName);
        Core.SetCurrentConv(Core.firstFriend.convID,Core.firstFriend.convName,TIMConvType.kTIMConv_C2C,"铂金");
        GenMsgItemForScroll();
      }
    }

    private void RenderMessageForScroll(Message msg){
      string senderName = "";
      string avatarUrl = "";
      string senderTeer = "黄金";
      string userid = "";
      if (msg.message_sender_group_member_info != null)
      {
        senderName = Utils.GetUserNameFromGroupMemberInfo(msg.message_sender_group_member_info);
        avatarUrl = msg.message_sender_group_member_info.group_member_info_face_url;
      }
      if (msg.message_sender_profile != null)
      {
        if (senderName == "")
        {
          senderName = !string.IsNullOrEmpty(msg.message_sender_profile.user_profile_nick_name) ? msg.message_sender_profile.user_profile_nick_name : msg.message_sender_profile.user_profile_identifier;
        }

        if (avatarUrl == "")
        {
          avatarUrl = msg.message_sender_profile.user_profile_face_url;
        }
      }
      if (string.IsNullOrEmpty(senderName))
      {
        senderName = msg.message_sender;
      }

      bool isSelf = msg.message_is_from_self.GetValueOrDefault();
      GameObject obj = null;
      // Render Face Message
      if (msg.message_elem_array[0].elem_type == TIMElemType.kTIMElem_Face)
      {
        if (isSelf)
        {
          obj = Instantiate(stickerMessageItemSelf, parent.transform);
          MsgItem item = obj.GetComponentInChildren<MsgItem>();
          item.setData(null,senderName,msg.message_elem_array[0].face_elem_buf,avatarUrl,senderTeer,true);
        }
        else
        {
          obj = Instantiate(stickerMessageItem, parent.transform);
          MsgItem item = obj.GetComponentInChildren<MsgItem>();
          item.setData(null,senderName,msg.message_elem_array[0].face_elem_buf,avatarUrl,Core.currentTeer);
        }

      }
      // Render Text Message
      if (msg.message_elem_array[0].elem_type == TIMElemType.kTIMElem_Text && senderName != "admin")
      {
        if (isSelf)
        {
          obj = Instantiate(messageItemSelf, parent.transform);
          MsgItem item = obj.GetComponentInChildren<MsgItem>();
          item.setData(msg.message_elem_array[0].text_elem_content,senderName,null,avatarUrl,senderTeer,true);
        }
        else
        {
          obj = Instantiate(messageItem, parent.transform);
          MsgItem item = obj.GetComponentInChildren<MsgItem>();
          item.setData(msg.message_elem_array[0].text_elem_content,senderName,null,avatarUrl,Core.currentTeer);
        }

      }
      if(msg.message_elem_array[0].elem_type == TIMElemType.kTIMElem_GroupTips){
        print(msg.message_elem_array[0].group_tips_elem_tip_type);
        string message = "";
        switch(msg.message_elem_array[0].group_tips_elem_tip_type){
          case TIMGroupTipType.kTIMGroupTip_Invite:
            message = string.Join(" ",(object[])msg.message_elem_array[0].group_tips_elem_user_array.ToArray());
            message += " 加入群聊";
            
          break;
          case TIMGroupTipType.kTIMGroupTip_Kick:
            message = string.Join(" ",(object[])msg.message_elem_array[0].group_tips_elem_user_array.ToArray());
            message += " 被踢出群聊";
          break;
          case TIMGroupTipType.kTIMGroupTip_Quit:
            message = string.Join(" ",(object[])msg.message_elem_array[0].group_tips_elem_user_array.ToArray());
            message += " 退出群聊";
          break;
          case TIMGroupTipType.kTIMGroupTip_GroupInfoChange:
          break;
          case TIMGroupTipType.kTIMGroupTip_MemberInfoChange:
          break;
        }
        obj = Instantiate(groupTipsItem, parent.transform);
        MsgItem item = obj.GetComponentInChildren<MsgItem>();
        item.setData(message,"groupTips",null,null,null);
      }
      if (obj == null)
      {
        return;
      }
    }


    private void GenMsgItemForScroll(params string[] args){
      if (parent == null)
      {
        return;
      }

      CloseOverlay();
      bool isConvChanged = false;

      // Set previous message list position
      if (!string.IsNullOrEmpty(convID) && convID != Core.currentConvID && Core.convMessageMap.TryGetValue(convID, out MessageListObject preMsgListObj))
      {
        preMsgListObj.index = scrollRect.verticalNormalizedPosition;
        isConvChanged = true;
      }
      convID = Core.currentConvID;
      print("currentconvID");
      print(convID);
      // Destroy the old list
      foreach (Transform child in parent.transform)
      {
        GameObject.Destroy(child.gameObject);
      }
      convName.GetComponentInChildren<Text>().text = Core.currentConvName;
      // Generate the new list
      if (Core.convMessageMap.TryGetValue(Core.currentConvID, out MessageListObject msgListObj))
      {
        // Reversed message list. Old on the top, now at the bottom.
        for (int i = msgListObj.list.Count - 1; i >= 0; i--)
        {
          var msg = msgListObj.list[i];

          if (i == msgListObj.list.Count - 1)
          {
            lastMsg = msg;
          }
          // Generate Time Stamp
          if ((Utils.IsFaceOrTextMsg(msg) && i == msgListObj.list.Count - 1) || (i < msgListObj.list.Count - 1 && Utils.ShouldGenerateTimeStampItem(msgListObj.list[i + 1], msg)))
          {
            var timeStr = Utils.GenerateTimeStampItem(msgListObj.list[i]);
            var timeStampObj = Instantiate(timeStampItem, parent.transform);
            timeStampObj.SetActive(true);
            timeStampObj.GetComponentInChildren<Text>().text = timeStr;
          }

          // Render Message
          RenderMessageForScroll(msg);
        }

        if (isConvChanged)
        {
          // Set current message list position
          Canvas.ForceUpdateCanvases();
          // Debug.Log("hello " + msgListObj.index);
          scrollRect.verticalNormalizedPosition = msgListObj.index;
        }
        else if (!msgListObj.isPrevMessage)
        {
          if(msgListObj.index != 0 && msgListObj.isRecvNewMsg){
            // recvMessageButton.SetActive(true);
            msgListObj.isRecvNewMsg = false;
          }
          // scrollRect.verticalNormalizedPosition = 0;
          // 有几条新的消息
        }
      }
      else
      {
        // No message list, go get it
        Core.MsgGetMsgList(Core.currentConvID, Core.currentConvType, SetMsgList);
        // 预设消息
        if(Core.currentConvID != "" && Core.currentConvType != TIMConvType.kTIMConv_C2C){
          GameObject obj = Instantiate(groupTipsItem, parent.transform);
          MsgItem item = obj.GetComponentInChildren<MsgItem>();

          item.setData("你可以在这里开始聊天","groupTips",null,null,null);
        }
      }
    }

    private UnityEngine.EventSystems.PointerEventData data;
  }
}