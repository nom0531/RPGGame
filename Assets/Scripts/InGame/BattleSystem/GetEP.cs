using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetEP : MonoBehaviour
{
    [SerializeField, Header("�\������e�L�X�g")]
    private GameObject Text;

    private int m_EP = 0;

    /// <summary>
    /// �h���b�v����EP���L�^����
    /// </summary>
    /// <param name="EP">EnhancementPoint</param>
    public void SaveDropEP(int EP)
    {
        m_EP += EP;
        GameManager.Instance.SaveData.SaveData.saveData.EnhancementPoint += m_EP;
        GameManager.Instance.SaveData.Save();
    }

    /// <summary>
    /// �l������EP�̗ʂ�`�悷��
    /// </summary>
    public void DrawText()
    {
        Text.GetComponent<TextMeshProUGUI>().text = $"�l��EP�F{m_EP}";
    }
}
