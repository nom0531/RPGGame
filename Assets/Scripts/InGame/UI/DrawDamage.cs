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
    /// �_���[�W��`�悷��
    /// </summary>
    public void Draw()
    {
        GetComponent<TextMeshProUGUI>().text = m_damage.ToString();
        var uiAnimation = GetComponent<UIAnimation>();
        uiAnimation.Animator = GetComponent<Animator>();
        uiAnimation.ButtonDown_Active();
    }

    /// <summary>
    /// ���g���폜����
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
