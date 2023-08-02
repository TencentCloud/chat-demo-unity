using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using com.tencent.imsdk.unity.types;
using com.tencent.imsdk.unity.enums;
using com.tencent.imsdk.unity.callback;

namespace Com.Tencent.IM.Unity.UIKit
{
  public class Utils
  {

    // 下载图像缓存
    private static Dictionary<string, Sprite> downloadSpriteStore = new Dictionary<string, Sprite>();

    public static ValueCallback<string> HandleStringCallback(Callback callback = null)
    {
      return (int code, string desc, string callbackData, string user_data) =>
      {
        if (callback != null)
        {
          callback(code.ToString(), desc, callbackData, user_data);
        }
      };
    }

    public delegate void Callback(params string[] parameters);
    public static Callback CbMethod;

    public static bool IsCallbackLegit(string code)
    {
      return code == "0";
    }

    public static T FromJson<T>(string pJson)
    {
      if (typeof(T) == typeof(string))
      {
        return (T)(object)pJson;
      }

      if (string.IsNullOrEmpty(pJson))
      {
        return default(T);
      }

      try
      {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.MissingMemberHandling = MissingMemberHandling.Ignore;
        T ret = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(pJson, settings);

        return ret;
      }
      catch (System.Exception error)
      {
        Debug.LogError(error);
      }

      return default(T);
    }

    public static string ToJson(object pData)
    {
      try
      {
        var setting = new JsonSerializerSettings();

        setting.NullValueHandling = NullValueHandling.Ignore;
        return Newtonsoft.Json.JsonConvert.SerializeObject(pData, setting);
      }
      catch (System.Exception error)
      {
        Debug.LogError(error);
      }

      return null;
    }

    public static List<Message> FilterMsgList(List<Message> msgTotal, List<Message> msgIn)
    {
      var msgList = new List<Message>();
      foreach (var msg in msgIn)
      {
        if (!msgTotal.Contains(msg))
        {
          msgList.Add(msg);
        }
      }

      return msgList;
    }

    public static bool IsMsgListBefore(List<Message> msgListA, List<Message> msgListB)
    {
      if (msgListA.Count == 0 || msgListB.Count == 0)
      {
        return true;
      }

      var timeA = GetMessageTime(msgListA[0]);
      var timeB = GetMessageTime(msgListB[0]);

      return timeA > timeB;
    }

    public static ulong GetMessageTime(Message msg)
    {
      if (msg.message_server_time.HasValue && msg.message_server_time != 0)
      {
        return msg.message_server_time.Value;
      }

      if (msg.message_client_time.HasValue)
      {
        return msg.message_client_time.Value;
      }

      return 0;
    }

    public static void HandleNewMsgReceived(List<Message> msgList, string userData)
    {
      Dictionary<string, List<Message>> tempDict = new Dictionary<string, List<Message>>();

      foreach (Message msg in msgList)
      {
        // sort with conv_id
        if (tempDict.TryGetValue(msg.message_conv_id, out List<Message> list))
        {
          list.Add(msg);
        }
        else
        {
          tempDict.Add(msg.message_conv_id, new List<Message> { msg });
        }
      }

      foreach (var item in tempDict)
      {
        /// TODO Otherwise the editor is crashed
        // Core.SetMessageList(item.Key, FromJson<List<Message>>(ToJson(item.Value)));
        Core.SetMessageList(item.Key, item.Value,false,true);
      }
    }

    public static bool ShouldGenerateTimeStampItem(Message msgBefore, Message msgAfter)
    {
      var timeStampBefore = GetMessageTime(msgBefore);
      var timeStampAfter = GetMessageTime(msgAfter);
      return timeStampAfter - timeStampBefore > 360;
    }

    public static string GenerateTimeStampItem(Message msg)
    {
      var timeStamp = GetMessageTime(msg);

      if (timeStamp == 0)
      {
        return null;
      }

      var nowTime = DateTime.Now;
      nowTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day);
      var ftime = DateTimeOffset.FromUnixTimeSeconds((long)timeStamp);
      var offset = TimeZoneInfo.Local.BaseUtcOffset;
      ftime = ftime.Add(offset);
      var preFix = ftime.Hour >= 12 ? "下午" : "上午";
      var timeStr = ftime.ToString("hh:mm");
      // 一年外 年月日 + 上/下午 + 时间 (12小时制)
      if (nowTime.Year != ftime.Year)
      {
        return string.Format("{0:yyyy-MM-dd} {1} {2}", ftime, preFix, timeStr);
      }
      // 一年内一周外 月日 + 上/下午 + 时间 (12小时制）
      if (ftime < nowTime.Subtract(new TimeSpan(6, 0, 0, 0)))
      {
        return string.Format("{0:MM-dd} {1} {2}", ftime, preFix, timeStr);
      }
      // 一周内一天外 星期 + 上/下午 + 时间 (12小时制）
      if (ftime < nowTime.Subtract(new TimeSpan(1, 0, 0, 0)))
      {
        return string.Format("{0} {1} {2}", GetWeekday(ftime.DayOfWeek), preFix, timeStr);
      }
      // 昨日 昨天 + 上/下午 + 时间 (12小时制)
      if (nowTime.Day != ftime.Day)
      {
        string option2 = string.Format("{0} {1}", preFix, timeStr);
        return string.Format("昨天 {0}", option2);
      }
      // 同年月日 上/下午 + 时间 (12小时制)
      return string.Format("{0} {1}", preFix, timeStr);
    }

    public static string GetWeekday(DayOfWeek dayOfWeek)
    {
      switch (dayOfWeek)
      {
        case DayOfWeek.Monday:
          return "星期一";
        case DayOfWeek.Tuesday:
          return "星期二";
        case DayOfWeek.Wednesday:
          return "星期三";
        case DayOfWeek.Thursday:
          return "星期四";
        case DayOfWeek.Friday:
          return "星期五";
        case DayOfWeek.Saturday:
          return "星期六";
        default:
          return "星期天";
      }
    }

    public static string GetUserNameFromGroupMemberInfo(GroupMemberInfo groupMemberInfo)
    {
      // 成员备注
      if (!string.IsNullOrEmpty(groupMemberInfo.group_member_info_remark))
      {
        return groupMemberInfo.group_member_info_remark;
      }

      // 成员群名片
      if (!string.IsNullOrEmpty(groupMemberInfo.group_member_info_name_card))
      {
        return groupMemberInfo.group_member_info_name_card;
      }

      // 成员昵称
      if (!string.IsNullOrEmpty(groupMemberInfo.group_member_info_nick_name))
      {
        return groupMemberInfo.group_member_info_nick_name;
      }

      // 成员ID
      return groupMemberInfo.group_member_info_identifier;
    }

    public static string GenerateSenderName(string originalSenderName)
    {
      if (originalSenderName.Length <= 2)
      {
        return originalSenderName;
      }
      return originalSenderName.Substring(0, 2);
    }

    public static IEnumerator DownTexture(string url, GameObject gameObject)
    {
      if (downloadSpriteStore.TryGetValue(url, out Sprite sprite))
      {
        gameObject.GetComponent<Image>().sprite = sprite;
      }
      else
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
          Sprite sprex = GetSpriteByTexture(rexture);
          try
          {
            gameObject.SetActive(true);
            gameObject.GetComponent<Image>().sprite = sprex;
          }
          catch (System.Exception e)
          {

          }
          Debug.Log("图片下载成功");

          if (!downloadSpriteStore.ContainsKey(url))
          {
            downloadSpriteStore.Add(url, sprex);
          }
        }
        else
        {
          //文件下载失败
          Debug.Log("图片下载失败");
        }
      }
    }
    //将texture转成image的Sprite
    public static Sprite GetSpriteByTexture(Texture2D tex)
    {
      Sprite _sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
      return _sprite;
    }

    public static bool IsFaceOrTextMsg(Message msg)
    {
      if (msg == null || msg.message_elem_array.Count == 0)
      {
        return false;
      }
      return msg.message_elem_array[0].elem_type == TIMElemType.kTIMElem_Face || msg.message_elem_array[0].elem_type == TIMElemType.kTIMElem_Text;
    }
  }
}