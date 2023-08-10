using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Collections;
using System.Collections.Generic;
using com.tencent.imsdk.unity;
using com.tencent.imsdk.unity.enums;
using com.tencent.imsdk.unity.types;
using Com.Tencent.IM.Unity.UIKit;
using UnityEngine;
using UnityEngine.UI;
namespace Com.Tencent.Imsdk.Unity.UIKit {
    public class ConversationItem : MonoBehaviour
{
    public Text m_ConvName;
    public GameObject m_Avatar;
    public GameObject m_Online;
    public Image m_TeerAvatar;
    public Image m_TeerImage;
    public Text m_TeerText;

    public void hello(){
        print("hello");
    }
    public void setName(string text,string name){
        m_ConvName.text = text;
        m_ConvName.name = name;
    }
    public IEnumerator setOnline(List<string> userid){
      print("setonline");
       yield return TencentIMSDK.GetUserStatus(userid,Utils.HandleStringCallback((string[] args) => {
          print(Utils.IsCallbackLegit(args[0]));
          if (Utils.IsCallbackLegit(args[0])){
            // if (string.IsNullOrEmpty(Core.currentConvID)){
              if(m_Online != null){
                var userStatus = Utils.FromJson<List<UserStatus>>(args[2]);
                print(userStatus[0]);
                if(userStatus[0].user_status_status_type == TIMUserStatusType.kTIMUserStatusType_Online){
                  m_Online.SetActive(true);
                }else{
                  m_Online.SetActive(false);
                }
              }
              
            // }
          }
      }));
    }
    public void setTeer(string teer){
      if(Core.currentLanguage == Language.Chinese){
        m_TeerText.text = teer;
      } else {
        if(teer == "钻石"){
          m_TeerText.text = "Diamond";
        }else if(teer == "白银"){
          m_TeerText.text = "Silver";
        }else if(teer == "黄金"){
          m_TeerText.text = "Gold";
        }else if(teer == "铂金"){
          m_TeerText.text = "Platinum";
        }
      }
        
        Sprite sprite = (Sprite)Resources.Load(teer,typeof(Sprite));
        if(teer == "王者" || teer == "钻石"){
          m_TeerAvatar.rectTransform.sizeDelta = new Vector2(394.93f,95.93f);
        }else{
          m_TeerAvatar.rectTransform.sizeDelta = new Vector2(283.896f,87.91f);
        }
        
        m_TeerAvatar.sprite = (Sprite)Resources.Load(teer,typeof(Sprite));
        m_TeerImage.sprite = (Sprite)Resources.Load(teer+"Icon",typeof(Sprite));
    }
}
}

