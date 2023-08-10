using System.Net.Mime;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using com.tencent.imsdk.unity.types;

namespace Com.Tencent.IM.Unity.UIKit.Example
{
  public class Chat : MonoBehaviour
  {
    public GameObject logoutButton;
    public GameObject openChatObject;
    public GameObject infomation;
    // 新消息提醒text
    public GameObject buttonText;
    public GameObject channelText;
    public GameObject chat;
    public Button closeChat;
    public GameObject channelPanel;
    public GameObject conversationPanel;
    public GameObject conversationNamePanel;
    private void Awake()
    {
      logoutButton.GetComponent<Button>().onClick.AddListener(Logout);
      openChatObject.GetComponent<Button>().onClick.AddListener(OpenChat);
      Core.OnCloseConversation = CloseChat;
      Core.OnRecvNewMessage = HandleUnreadMessage;
      closeChat.onClick.AddListener(HandleCloseConversation);
      channelText.SetActive(false);
      HandleChannel();
    }

    private void OpenChat()
    {
      chat.SetActive(true);
      openChatObject.SetActive(false);
      infomation.SetActive(false);
      channelChanged(true,"FriendButton");
      if(Core.currentLanguage == Language.Chinese){
        buttonText.GetComponent<Text>().text = "唤出聊天";
      }else{
        buttonText.GetComponent<Text>().text = "Open Chats";
      }
      
    }
    private void CloseChat()
    {
      chat.SetActive(false);
      infomation.SetActive(true);
      openChatObject.SetActive(true);
      channelText.SetActive(false);
    }
     private void HandleCloseConversation()
    {
       if(Core.currentLanguage == Language.Chinese){
        buttonText.GetComponent<Text>().text = "唤出聊天";
      }else{
        buttonText.GetComponent<Text>().text = "Open Chats";
      }
      Core.CloseChat();
    }

    private void HandleChannel()
    {
      Button[] channelButtons = channelPanel.GetComponentsInChildren<Button>();
      foreach(Button a in channelButtons){
        switch(a.gameObject.name){
          case "WorldButton":
            a.onClick.AddListener(worldChannel);
            break;
          case "ChannelButton":
            a.onClick.AddListener(channelChannel);
            break;
          case "GroupButton":
            a.onClick.AddListener(groupChannel);
            break;
          case "FriendButton":
            a.onClick.AddListener(friendChannel);
            break;
        }
      }
    }
    private void channelChanged(bool active,string buttonName = "",string convId="",string convName=""){
      conversationPanel.SetActive(active);
      conversationNamePanel.SetActive(active);
      Core.ChannelChanged(active);
      if(convId != ""){
        Core.SetCurrentConv(convId,convName,com.tencent.imsdk.unity.enums.TIMConvType.kTIMConv_Group,"铂金");
      }
      
      Button[] channelButtons = channelPanel.GetComponentsInChildren<Button>();
      foreach(Button a in channelButtons){
        if(a.gameObject.name == buttonName){
          a.gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("选中", typeof(Sprite));
          a.gameObject.GetComponent<Image>().color =  new Color32(255, 255, 255,   217);

        }else if(a.gameObject.name != "ChannelPanel"){
          // a.gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("未选中", typeof(Sprite));
          a.gameObject.GetComponent<Image>().sprite = null;
          a.gameObject.GetComponent<Image>().color =  new Color32(29, 53, 84,   217);
        }
      }
    }

    private void worldChannel(){
      channelChanged(false,"WorldButton",Config.communityId,"世界");
    }
    private void channelChannel(){
      channelChanged(false,"ChannelButton",Config.channelId,"频道");
    }
    private void groupChannel(){
      channelChanged(false,"GroupButton",Config.groupId,"组队");
    }
    private void friendChannel(){
      channelChanged(true,"FriendButton");
    }
    private void Logout()
    {
      print("logout");
      Core.Logout(HandleAfterLogout);
    }

    private void HandleAfterLogout(params string[] args)
    {
      if (Utils.IsCallbackLegit(args[0]))
      {
        PlayerPrefs.DeleteKey("userId");
        PlayerPrefs.DeleteKey("sdkUserSig");
        SceneManager.LoadScene("Main");
      }
    }

    private void HandleUnreadMessage(params string[] args){
      string text = (string)args[0].ToString();
      string actualText = text;
      if(text.Length > 10){
        actualText = text.Substring(0,15);
        actualText += "...";
      }
      buttonText.GetComponent<Text>().text = actualText;
      // channel according to convID
      channelText.SetActive(true);
      if(Core.currentLanguage == Language.Chinese){
        channelText.GetComponentInChildren<Text>().text = "好友";
      }else if(Core.currentLanguage == Language.English){
        channelText.GetComponentInChildren<Text>().text = "Friend";
      }
    }
  }
}