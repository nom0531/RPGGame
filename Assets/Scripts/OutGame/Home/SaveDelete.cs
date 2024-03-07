using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveDelete : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject GameObject;

    private const float TIMER = 3.0f;   // �{�^����������悤�ɂȂ�܂ł̑ҋ@����

    private Button m_button;
    private SetTimer m_setTimer;

    /// <summary>
    /// �Z�[�u�f�[�^���폜����
    /// </summary>
    public void Delete()
    {
        GameManager.Instance.SaveDataManager.Delete();
    }

    private void Start()
    {
        m_setTimer = GameObject.GetComponent<SetTimer>();
        m_button = GetComponent<Button>();
    }

    private void FixedUpdate()
    {
        WaitTime();
    }

    /// <summary>
    /// �{�^����������悤�ɂȂ�܂őҋ@����
    /// </summary>
    private void WaitTime()
    {
        // ���Ƀ{�^����������Ȃ���s���Ȃ�
        if(m_setTimer.InteractableFlag == true)
        {
            return;
        }
        m_button.interactable = false;
        m_setTimer.Timer += Time.deltaTime;

        // �l�����ȉ��Ȃ���s���Ȃ�
        if(m_setTimer.Timer <= TIMER)
        {
            return;
        }
        m_button.interactable = true;
        m_setTimer.InteractableFlag = true;
    }
}
