using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Com.Tencent.IM.Unity.UIKit;
public enum Language{
    Chinese,
    English,
}

public enum LanguageTextName{
    Phone,
    Captcha,
    Login,
    Send,
    GetCaptcha,
    OpenChat,
    World,
    Channel,
    Group,
    Friend,
    EnterText,
    SearchFriend,
    Logout,
    Calloutchat
}

public class LanguageDataManager : Singleton<LanguageDataManager>
{
    internal static Language currentLanguage = Language.English;

    private Dictionary<string,string> ChineseDictionary = new Dictionary<string,string>();
    private Dictionary<string,string> EnglishDictionary = new Dictionary<string,string>();

    private static string LANGUAGE_TXT_PATH = "LanguageTxt/";
    public LanguageDataManager() {

        // 加载语言资源
        LoadLanguageTxt(Language.Chinese);
        LoadLanguageTxt(Language.English);
    }

    public void Init() { }

    /// 设置语言
    public static void SetCurrentLanguageValue(Language language)
    {

        currentLanguage = language;
        Core.setCurrentLanguage(language);
        //获取场景中所有LanguageUIText类型脚本;用于更新对应语言UI信息
        LanguageUIText[] languageUITexts = Resources.FindObjectsOfTypeAll<LanguageUIText>();

        for (int i = 0; i < languageUITexts.Length; i++)
        {            

            languageUITexts[i].SetLanguageTextName();
            // Debug.Log("languageUITexts:"+ languageUITexts[i].languageTextName);
        }        

    }

    /// 加载对应的语言资源txt&#xff0c;保存到字典中
    internal void LoadLanguageTxt(Language language)
    {

        TextAsset ta = Resources.Load<TextAsset>(LANGUAGE_TXT_PATH + language.ToString());
        // TextAsset ta = Resources.Load<TextAsset>("LanguageTxt/English");
        if (ta == null)
        {
            Debug.Log("没有该语言的文本文件");

            return;

        }

        // 解析文本信息
        string[] lines = ta.text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {

            if (string.IsNullOrEmpty(lines[i]))
            {

                continue;

            }
            string[] kv = lines[i].Split(":");
            if (language == Language.Chinese)
            {

                ChineseDictionary.Add(kv[0], kv[1]);

            }
            else if (language == Language.English)
            {

                EnglishDictionary.Add(kv[0], kv[1]);

            }


        }

    }

    /// 获得对应语言字典中的对应的 key 值
    internal string GetLanguageText(string key)
    {

        if (currentLanguage == Language.Chinese)
        {

            if (ChineseDictionary.ContainsKey(key))
            {

                return ChineseDictionary[key];

            }

            else
            {

                Debug.Log("!ChineseDictionary.ContainsKey(key)");

            }

        }
        else if (currentLanguage == Language.English)
        {

            if (EnglishDictionary.ContainsKey(key))
            {

                return EnglishDictionary[key];

            }
            else
            {
                Debug.Log("!EnglishDictionary.ContainsKey(key)");

            }

        }
        return string.Empty;

    }
}

public abstract class Singleton<T> where T : class, new()
{
    private static T instance = null;

    // 多线程安全机制
    private static readonly object locker = new object();

    public static T Instance
    {
        get
        {
            lock (locker)
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }
    }

}