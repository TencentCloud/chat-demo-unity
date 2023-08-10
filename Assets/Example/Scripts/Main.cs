using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Jing.ULiteWebView;
using UnityEngine.SceneManagement;
using com.tencent.imsdk.unity;
// using UnityEditor.Compilation;

namespace Com.Tencent.IM.Unity.UIKit.Example
{
  public class Main : MonoBehaviour
  {
    public InputField phoneNumber;
    public InputField captcha;
    public Button getCaptchaButton;
    public Button loginButton;
    // public GameObject headerImg;
    private int totalTime = 60;
    private string sessionId;
    private IEnumerator countDownCoroutine;

    private void Start()
    {
      // StartCoroutine(DownTexture(Config.headerImgUrl, headerImg));
      // 设置表情包信息
      Core.SetStickerPackageList(Config.stickers);
      ChangeSystemLanguage();
      string userId = PlayerPrefs.GetString("userId");
      string sdkUserSig = PlayerPrefs.GetString("sdkUserSig");

      if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(sdkUserSig))
      {
        Core.SetConfig(Config.sdkappid, userId, sdkUserSig);
        Core.Init();
        Core.Login(HandleAfterLogin);
      }

      getCaptchaButton.onClick.AddListener(GetCaptcha);
      loginButton.onClick.AddListener(Login);


      countDownCoroutine = CountDown();
      string storedPhone = PlayerPrefs.GetString("phone");

      if (!string.IsNullOrEmpty(storedPhone))
      {
        phoneNumber.text = storedPhone;
      }
    }

    private void ChangeSystemLanguage(){
      string language = GetSystemLanguageWrapper.GetSystemLanguage();
      Debug.Log("system language: "+language);
      switch (language){
        case "Chinese":
          LanguageDataManager.SetCurrentLanguageValue(Language.Chinese);
          break;
        default:
          LanguageDataManager.SetCurrentLanguageValue(Language.English);
          break;
      }
    }

    private IEnumerator DownTexture(string url, GameObject gameObject)
    {
      UnityWebRequest WebRequest = new UnityWebRequest(url);
      DownloadHandlerTexture Download = new DownloadHandlerTexture(true);
      WebRequest.downloadHandler = Download;
      yield return WebRequest.SendWebRequest();
      //等待资源下载完成
      while (!WebRequest.isDone)
      {
        yield return null;
      }
      if (string.IsNullOrEmpty(WebRequest.error))
      {
        //文件下载成功
        //读取的图片
        Texture2D rexture = Download.texture;
        gameObject.SetActive(true);
        gameObject.GetComponent<Image>().sprite = GetSpriteByTexture(rexture);
        // Debug.Log("图片下载成功");
      }
      else
      {
        //文件下载失败
        // Debug.Log("图片下载失败");
      }
    }
    //将texture转成image的Sprite
    private Sprite GetSpriteByTexture(Texture2D tex)
    {
      Sprite _sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
      return _sprite;
    }

    private void Reset()
    {
      getCaptchaButton.interactable = true;
      totalTime = 60;
      getCaptchaButton.GetComponentInChildren<Text>().text = "获取验证码";
    }

    private IEnumerator CountDown()
    {
      Debug.Log("incountdown");
      Text buttonText = getCaptchaButton.GetComponentInChildren<Text>();
      getCaptchaButton.interactable = false;
      while (totalTime >= 0)
      {
        buttonText.text = totalTime.ToString();
        yield return new WaitForSeconds(1);
        totalTime--;
      }
      Reset();
    }

    private void GetCaptcha()
    {
      if (string.IsNullOrEmpty(phoneNumber.text))
      {
        return;
      }

      ULiteWebView.Ins.RegistJsInterfaceAction("messageHandler", GetCaptchaRes);
      ULiteWebView.Ins.RegistJsInterfaceAction("capClose", HandleCapClose);
      ULiteWebView.Ins.RegistJsInterfaceAction("getsize", getCaptchasize);
      var marginW = (UnityEngine.Screen.width - 768) / 2;
      var marginH = (UnityEngine.Screen.height - 768) / 2;
      ULiteWebView.Ins.Show(marginH, marginH, marginW, marginW);
      // ULiteWebView.Ins.Show();
      ULiteWebView.Ins.LoadLocal(Config.captchaUrl);
      // 倒计时
      Debug.LogError("marginW"+marginW);
      Debug.LogError("marginH" + marginH);
      IEnumerator countdowntry = CountDown();
      StartCoroutine(countdowntry);
    }
    private void getCaptchasize(string res){
      Debug.LogError("capchasize"+res);
    }
    private IEnumerator SMSRequest(string path, string dataStr, Action<string> cb)
    {
      byte[] postData = System.Text.Encoding.UTF8.GetBytes(dataStr);
      var url = Config.smsLoginHttpBase + path;

      var www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
      www.chunkedTransfer = false;
      www.uploadHandler = new UploadHandlerRaw(postData);
      www.downloadHandler = new DownloadHandlerBuffer();
      www.SetRequestHeader("Content-Type", "application/json");
      www.SetRequestHeader("Accept", "application/json");

      yield return www.SendWebRequest();

      if (www.isNetworkError || www.isHttpError)
      {
        Debug.Log(www.error);
      }
      else
      {
        // data.sessionId
        // Debug.Log("Form upload complete! " + path + www.downloadHandler.text);
        cb(www.downloadHandler.text);
      }
    }

    // 验证验证码后台下发短信
    private void VervifyPicture(MessageObj messageObj)
    {
      var data = new VerifyPictureReq
      {
        phone = "+86" + phoneNumber.text,
        ticket = messageObj.ticket,
        randstr = messageObj.randstr,
        appId = Config.sdkappid,
        apaasAppId = ""
      };

      StartCoroutine(SMSRequest("/base/v1/auth_users/user_verify_by_picture", Utils.ToJson(data), (string res) => HandleVerifyByPicture(res)));
    }

    private void HandleVerifyByPicture(string res)
    {
      var resObj = Utils.FromJson<SMSRes<VerifyResData>>(res);

      if (resObj.errorCode != 0)
      {
        Debug.LogError("Res Error: " + resObj.errorMessage);
        return;
      }

      sessionId = resObj.data.sessionId;
    }

    private void HandleLogin(string res)
    {
      var resObj = Utils.FromJson<SMSRes<LoginResData>>(res);

      if (resObj.errorCode != 0)
      {
        Debug.LogError("Res Error: " + resObj.errorMessage);
        return;
      }

      // Debug.LogError("11111111userID: " + resObj.data.userId);
      // Debug.LogError("11111111sdkUserSig: " + resObj.data.sdkUserSig);

      PlayerPrefs.SetString("userId", resObj.data.userId);
      PlayerPrefs.SetString("sdkUserSig", resObj.data.sdkUserSig);
      
      Core.SetConfig(Config.sdkappid, resObj.data.userId, resObj.data.sdkUserSig);
      Core.Init();
      Core.Login(HandleAfterLogin);
    }

    private void HandleAfterLogin(params string[] args)
    {
      if (!Utils.IsCallbackLegit(args[0]))
      {
        PlayerPrefs.DeleteKey("userId");
        PlayerPrefs.DeleteKey("sdkUserSig");
        return;
      }
      StopCoroutine(countDownCoroutine);
      if (!string.IsNullOrEmpty(phoneNumber.text))
      {
        PlayerPrefs.SetString("phone", phoneNumber.text);
      }
      
      // 不用重复进group
      TencentIMSDK.GroupJoin(Config.communityId, "Hello", (int code, string desc, string user_data) =>
      {
        Debug.Log("Joined " + Config.communityId);
      });
      TencentIMSDK.GroupJoin(Config.channelId, "Hello", (int code, string desc, string user_data) =>
      {
        Debug.Log("Joined " + Config.channelId);
      });
      TencentIMSDK.GroupJoin(Config.groupId, "Hello", (int code, string desc, string user_data) =>
      {
        Debug.Log("Joined " + Config.groupId);
      });
      
      Reset();
      SceneManager.LoadScene("Chat");
    }

    private void HandleCapClose(string res)
    {
      ULiteWebView.Ins.Close();
    }

    private void GetCaptchaRes(string res)
    {
      var msgObj = Utils.FromJson<MessageObj>(res);
      VervifyPicture(msgObj);
      ULiteWebView.Ins.Close();
    }

    private void Login()
    {
      if (string.IsNullOrEmpty(phoneNumber.text) || string.IsNullOrEmpty(captcha.text) || string.IsNullOrEmpty(sessionId))
      {
        return;
      }

      loginButton.interactable = false;

      var data = new SMSLoginReq
      {
        sessionId = sessionId,
        phone = "+86" + phoneNumber.text,
        code = captcha.text,
        apaasAppId = "",
        tag = "im"
      };

      StartCoroutine(SMSRequest("/base/v1/auth_users/user_login_code", Utils.ToJson(data), (string res) => HandleLogin(res)));
      loginButton.interactable = true;
    }
    void OnApplicationQuit()
    {
      Core.Uninit();
    }
  }

  public class MessageObj
  {
    public string randstr;
    public string ticket;
  }

  public class VerifyPictureReq
  {
    public string phone;
    public string appId;
    public string ticket;
    public string randstr;
    public string apaasAppId;
  }

  public class SMSLoginReq
  {
    public string sessionId;
    public string phone;
    public string code;
    public string apaasAppId;
    public string tag;
  }

  public class SMSRes<T>
  {
    public int errorCode;
    public string codeStr;
    public string errorMessage;
    public T data;
  }

  public class LoginResData
  {
    public string userId;
    public int sdkAppId;
    public string sdkUserSig;
    public string token;
    public string expire;
    public string phone;
    public string email;
    public string name;
    public string avatar;
    public string apaasAppId;
    public string apaasUserId;

  }

  public class VerifyResData
  {
    public string sessionId;
  }
}