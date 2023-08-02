using com.tencent.imsdk.unity;
using com.tencent.imsdk.unity.types;
using com.tencent.imsdk.unity.enums;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;
using Com.Tencent.IM.Unity.UIKit;
using Com.Tencent.IM.Unity.UIKit.Example;
namespace Com.Tencent.Imsdk.Unity.UIKit {
    public class MsgItem : MonoBehaviour
    {
        public Text m_CellText;
        public Image m_CellTextImage;
        public Image m_CellTextImageSelf;
        public Text m_CellSenderName;
        public Image m_CellImage;
        public Image m_CellAvatarImage;
        public Text m_CellAvatarName;
        public Image m_CellTeerImage;
        public Image m_CellTeerIcon;
        public Text m_CellTeerText;

        public void setData(string Text = null,string SenderName = null,string Url = null,string AvatarUrl = null,string Teer = null,bool isSelf = false){
            if (m_CellText != null)
            {
                m_CellText.text = Text;
                if(m_CellTextImage != null){
                    if(Text.Length > 10){
                        m_CellTextImage.sprite = (Sprite)Resources.Load("longMsgItem", typeof(Sprite));
                    }else if(Text.Length < 2){
                        m_CellTextImage.sprite = (Sprite)Resources.Load("shortMsgItem", typeof(Sprite));
                    }else {
                        m_CellTextImage.sprite = (Sprite)Resources.Load("msgItem", typeof(Sprite));
                    }
                }else if(m_CellTextImageSelf != null){
                    if(Text.Length > 10){
                        m_CellTextImageSelf.sprite = (Sprite)Resources.Load("longMsgItemSelf", typeof(Sprite));
                    }else if(Text.Length < 2){
                        m_CellTextImageSelf.sprite = (Sprite)Resources.Load("shortMsgItemSelf", typeof(Sprite));
                    }else {
                        m_CellTextImageSelf.sprite = (Sprite)Resources.Load("msgItemSelf", typeof(Sprite));
                    }
                }
                
            }
            if (m_CellSenderName != null)
            {
                if(SenderName == "admin"){
                m_CellSenderName.text = "系统";
                }else if(SenderName == "groupTips"){
                if(Core.currentConvID == Config.communityId){
                    m_CellSenderName.text = "世界";
                }else if(Core.currentConvID == Config.channelId){
                    m_CellSenderName.text = "频道";
                }else if(Core.currentConvID == Config.groupId){
                    m_CellSenderName.text = "组队";
                }else{
                    m_CellSenderName.text = "好友";
                }
                
                }else{
                    m_CellSenderName.text = SenderName;
                }
                
            }
            if (m_CellImage != null && !string.IsNullOrEmpty(Url))
            {
                if (Url.StartsWith("http"))
                {
                // Download Network Image
                StartCoroutine(Utils.DownTexture(Url, m_CellImage.gameObject));
                }
                else
                {
                // Load From Storage
                m_CellImage.sprite = (Sprite)Resources.Load(Url, typeof(Sprite));
                }
            }

            if (m_CellAvatarImage != null && !string.IsNullOrEmpty(AvatarUrl))
            {
                if(AvatarUrl == "" || AvatarUrl == null){
                    m_CellAvatarImage.sprite = (Sprite)Resources.Load("avatarring", typeof(Sprite));
                }
                else if (AvatarUrl.StartsWith("http"))
                {
                // Download Network Image
                StartCoroutine(Utils.DownTexture(AvatarUrl, m_CellAvatarImage.gameObject));
                }
                else
                {
                // Load From Storage
                m_CellAvatarImage.sprite = (Sprite)Resources.Load(AvatarUrl, typeof(Sprite));
                }
                m_CellAvatarImage.gameObject.SetActive(true);
            }
            else if (m_CellAvatarName != null)
            {
                string abbrSenderName = Utils.GenerateSenderName(SenderName);
                m_CellAvatarName.text = abbrSenderName;
                m_CellAvatarName.gameObject.SetActive(true);
            }
            if(m_CellTeerImage != null && m_CellTeerIcon != null){
                string iconName = Teer + "Icon";
                if(Teer == "王者" || Teer == "钻石"){
                    if(m_CellImage == null || string.IsNullOrEmpty(Url)){
                        m_CellTeerImage.rectTransform.sizeDelta = new Vector2(136.69f,106.42f);
                    }else if(m_CellImage != null){
                        m_CellTeerImage.rectTransform.offsetMin = new Vector2(-28.69499f,-12.795f);
                        m_CellTeerImage.rectTransform.offsetMax = new Vector2(29.01601f,12.795f);
                    }

                    
                }else{
                    if(!isSelf && (m_CellImage == null || string.IsNullOrEmpty(Url))){
                        m_CellTeerImage.rectTransform.sizeDelta = new Vector2(105.2318f,105.2318f);
                    }
                
                }
                m_CellTeerImage.sprite = (Sprite)Resources.Load(Teer, typeof(Sprite));
                m_CellTeerIcon.sprite = (Sprite)Resources.Load(iconName, typeof(Sprite));
            }
            if(m_CellTeerText != null){
                m_CellTeerText.text = Teer;
            }
        }
    }

    

    public class MsgData{
        public string Text = "";
        public string SenderName = "";
        public string Url = "";
        public string AvatarUrl = "";
        public string Teer = "";
    }
}