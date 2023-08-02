using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// UI Text 更新的信息
public class LanguageUIText : MonoBehaviour
{
    [SerializeField]
    internal LanguageTextName languageTextName;

    // Use this for initialization

    void Start()
    {
        // OnEnable 已经设置&#xff0c;可以注释掉
        SetLanguageTextName();

    }

    /// 脚本使能的使用调用
    /// UI 显示的时候 更新文本的对应 Language 信息
    private void OnEnable()
    {

        SetLanguageTextName();

    }

    /// 根据当前语言设置 Text 文本
    internal void SetLanguageTextName()
    {
        // 获取对应文本的语言对应信息名称
        string value = LanguageDataManager.Instance.GetLanguageText(languageTextName.ToString());

        // 更新语言信息
        if (string.IsNullOrEmpty(value) != true)
        {

            gameObject.GetComponent<Text>().text = value;

        }

    }
}