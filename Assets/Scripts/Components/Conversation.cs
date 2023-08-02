using System.Reflection.Emit;
using Com.Tencent.IM.Unity.UIKit;
using com.tencent.imsdk.unity;
using com.tencent.imsdk.unity.types;
using com.tencent.imsdk.unity.enums;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace Com.Tencent.Imsdk.Unity.UIKit
{
  public class Conversation : MonoBehaviour
  {
    public GameObject conversationItem;
    public InputField searchItem;
    // public Button closeButton;
    private List<FriendProfile> friendProfiles;
    private convItem firstFriend;
    private string firstTeer;
    private Dictionary<string,convItem> convItems;
    void Start()
    {
      // 当前选择的会话变化
      Core.OnCurrentConvChanged += OnCurrentConvChanged;
      // 会话有变更
      Core.OnConvChanged += HandleGetConvList;
      Core.LoginIfNot(HandleGetConvList);
      searchItem.onValueChanged.AddListener(OnInputChanged);
      searchItem.onEndEdit.AddListener(OnInputEnd);
    }

    private void HandleCloseConversation()
    {
      Core.CloseChat();
    }

    private void OnInputChanged(string context){
      Console.WriteLine("on input changed"+context);
      sortSearchFriend(context);
    }

    private void OnInputEnd(string context){
      Console.WriteLine("on input end"+context);
      GenerateList(convItems);
      searchItem.text = "";
    }

    private void sortSearchFriend(string name){
      StringComparison comp = StringComparison.OrdinalIgnoreCase;
      if(convItems != null && convItems.Count > 0){
        Dictionary<string,convItem> items = new Dictionary<string,convItem>();
        foreach(var item in convItems){
          print(item.Value.name);
          if(item.Key.Contains(name,comp) || item.Value.name.Contains(name,comp)){
            items.Add(item.Key,item.Value);
          }
        }
        GenerateList(items);
      }
    }

    private void showFirstFriendMessage(params string[] args){
      if(Core.isFriendList){
        // Core.SetCurrentConv(firstFriend.Key, firstFriend.Value.name,firstFriend.kTIMConv_C2C,firstTeer);
      }
    }

    private void HandleGetConvList(params string[] args)
    {
      if (Utils.IsCallbackLegit(args[0]))
      {
        GetConversationList();
      }
    }
    private void GetConversationList()
    {
      TencentIMSDK.ConvGetConvList(Utils.HandleStringCallback(getConvList));
    }


    private void OnCurrentConvChanged(params string[] args)
    {
      if (string.IsNullOrEmpty(Core.currentConvID))
      {
        return;
      }
      // 更改选中会话样式
      var parent = GameObject.Find("ConversationList");
      if(parent!=null){
        foreach (Transform child in parent.transform)
        {
          if (child.GetComponentInChildren<Text>().name == Core.currentConvID)
          {
            child.GetComponent<Image>().color = new Color32(37, 73, 127,   200);
          }
          else
          {
            child.GetComponent<Image>().color = new Color32(33, 58, 90, 217);
          }
        }
      }
      
    }

    private void GenerateList(Dictionary<string,convItem> friendConv){
      var teer = new List<string>{"白银","黄金","铂金","钻石"};
      var parent = GameObject.Find("ConversationList");
        if (parent == null)
        {
          return;
        }

        foreach (Transform child in parent.transform)
        {
          GameObject.Destroy(child.gameObject);
        }
        bool isFirst = true;
        foreach (var friend in friendConv)
        {
          var obj = Instantiate(conversationItem, parent.transform);
          obj.SetActive(true);
          ConversationItem convItem = obj.GetComponentInChildren<ConversationItem>();
          string actualName = friend.Value.name;
          string shortenName = friend.Value.name;
          if(actualName.Length > 10){
            shortenName = actualName.Substring(0,7);
            shortenName += "..";
          }

          convItem.setName(shortenName,friend.Key);

          // 段位
          // string teerName = friend.Value.teer;
          // temp teer counting
          var teerName = teer[UnityEngine.Random.Range(0, 4)];
          print(teerName);
          convItem.setTeer(teerName);

          //faceurl
          GenFriendAvatar(friend.Value.avatarUrl,convItem.m_Avatar);  
          print(friend.Value.avatarUrl);
          // online
          List<string> userid = new List<string>();
          userid.Add(friend.Key);
          StartCoroutine(convItem.setOnline(userid));
          if (obj.GetComponentInChildren<Text>().name == Core.currentConvID)
          {
            obj.GetComponent<Image>().color = new Color32(37, 73, 127,   100);
          }
          else
          {
            obj.GetComponent<Image>().color = new Color32(33, 58, 90, 100);
          }
          obj.GetComponent<Button>().onClick.AddListener(() =>
          {
            Core.SetCurrentConv(friend.Key, friend.Value.name,TIMConvType.kTIMConv_C2C,teerName);
          });
          if(isFirst){
            isFirst = false;
            firstTeer = teerName;
            Core.firstFriend = new firstConv{convID = friend.Key,convName = friend.Value.name};
            Core.SetCurrentConv(friend.Key, friend.Value.name,TIMConvType.kTIMConv_C2C,teerName);
          }
        }
    }

    private void GenFriendAvatar(string avatarUrl,GameObject gameObject){
      if(avatarUrl != ""){
        if(avatarUrl.StartsWith("http"))
        {
          StartCoroutine(Utils.DownTexture(avatarUrl,gameObject));
        }
      }
    }

      // get convList
    private void getConvList(params string[] args){
      if(Utils.IsCallbackLegit(args[0]))
      {
        print("getConvListhere");
        convItems = new Dictionary<string,convItem>();
        var convList = Utils.FromJson<List<ConvInfo>>(args[2]);
        foreach(var convInfo in convList){
          print(convInfo.conv_show_name);
          if(convInfo.conv_type == TIMConvType.kTIMConv_C2C){
            convItem item = new convItem();
            item.name = convInfo.conv_show_name;
            convItems.Add(convInfo.conv_id,item);
            // print(convInfo.conv_id);
          }
        }
        TencentIMSDK.FriendshipGetFriendProfileList(Utils.HandleStringCallback(completeConvList));
      }
    }

    private void completeConvList(params string[] args){
      if (Utils.IsCallbackLegit(args[0]))
      {
        print("completeconvlist");
        var friendList = Utils.FromJson<List<FriendProfile>>(args[2]);
        foreach (var friend in friendList){
          print(friend.friend_profile_identifier);
          if(convItems.TryGetValue(friend.friend_profile_identifier,out convItem item)){
            item.avatarUrl = friend.friend_profile_user_profile.user_profile_face_url;
            // item.teer = friend.friend_profile_user_profile.user_profile_custom_string_array.Find(x => x.user_profile_custom_string_info_key == "teer").user_profile_custom_string_info_value;
          }else{
            
            string actualName = friend.friend_profile_remark != "" ? friend.friend_profile_remark : friend.friend_profile_user_profile.user_profile_nick_name;
            Console.WriteLine("addconvList" + actualName);
            if(actualName.Length == 0){
              actualName = friend.friend_profile_user_profile.user_profile_identifier;
            }
            convItems.Add(friend.friend_profile_user_profile.user_profile_identifier,new convItem{
              name = actualName,
              avatarUrl = friend.friend_profile_user_profile.user_profile_face_url,
              // teer = friend.friend_profile_user_profile.user_profile_custom_string_array.Find(x => x.user_profile_custom_string_info_key == "teer").user_profile_custom_string_info_value;
            });
          }
        }
        FriendShipGetProfileListParam json_get_user_profile_list_param = new FriendShipGetProfileListParam();
        json_get_user_profile_list_param.friendship_getprofilelist_param_identifier_array = new List<string>(convItems.Keys);
        TencentIMSDK.ProfileGetUserProfileList(json_get_user_profile_list_param,Utils.HandleStringCallback(addAvatar));
        
      }
    }

    private void addAvatar(params string[] args){
      if (Utils.IsCallbackLegit(args[0]))
      {
        var userList = Utils.FromJson<List<UserProfile>>(args[2]);
        foreach(var user in userList){
          if(convItems.TryGetValue(user.user_profile_identifier,out convItem item)){
            if(item.avatarUrl == ""){
              item.avatarUrl = user.user_profile_face_url;
            }
          }
        }
      }
      GenerateList(convItems);
    }
  }

  public class convItem {
    public string name = "";
    public string avatarUrl = "";
    string teer = "";
    // bool Online = false;
  }
}