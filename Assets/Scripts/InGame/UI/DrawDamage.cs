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
    /// �_���[�W��`�悷��
    /// </summary>
    public void Draw()
    {
        // �e�L�X�g�̐ݒ�
        var text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = m_damageText;
        // �A�j���[�V����
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
