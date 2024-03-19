using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawDamage : MonoBehaviour
{
    private int m_damage = 0;

    public int Damage
    {
        set => m_damage = value;
    }

    /// <summary>
    /// ダメージを描画する
    /// </summary>
    public void Draw()
    {
        GetComponent<TextMeshProUGUI>().text = m_damage.ToString();
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
