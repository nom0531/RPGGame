using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawDamage : MonoBehaviour
{
    private string m_damageText = "";

    public string Damage
    {
        set => m_damageText = value;
    }

    /// <summary>
    /// ダメージを描画する
    /// </summary>
    public void Draw()
    {
        // テキストの設定
        var text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = m_damageText;
        // アニメーション
        var uiAnimation = GetComponent<UIAnimation>();
        uiAnimation.Animator = GetComponent<Animator>();
        uiAnimation.ButtonDown_Active();
    }

    /// <summary>
    /// 自身を削除する
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
