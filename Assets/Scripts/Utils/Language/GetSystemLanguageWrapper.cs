using UnityEngine;

/// 获得系统设置语言的方法
public class GetSystemLanguageWrapper 
{
    // Unity Android 上下文
    private static AndroidJavaObject _unityContext;

    public static AndroidJavaObject UnityContext {
        get {

            if (_unityContext == null)
            {
                AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _unityContext = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            }

            return _unityContext;
        }
    }

    /// 获取系统语言
    /// 这种方法目前不能区分中文简体繁体
    public static string GetSystemLanguage()
    {
        string systemLanguage;
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass localeClass = new AndroidJavaClass("java/util/Locale");
            AndroidJavaObject defaultLocale = localeClass.CallStatic<AndroidJavaObject>("getDefault");
            AndroidJavaObject usLocale = localeClass.GetStatic<AndroidJavaObject>("US");
            systemLanguage = defaultLocale.Call<string>("getDisplayLanguage", usLocale);
        }
        else
        {
            systemLanguage = Application.systemLanguage.ToString();
        }
        return systemLanguage;
    }
}