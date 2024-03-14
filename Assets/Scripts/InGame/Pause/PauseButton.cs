using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField, Header("�|�[�Y�{�^��")]
    private GameObject PauseButtonObjct;
    [SerializeField, Tooltip("�ύX��̉摜")]
    private Sprite PauseAfterImage;

    private PauseManager m_pauseManager;

    private void Start()
    {
        m_pauseManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<PauseManager>();
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void ButtonDown()
    {
        m_pauseManager.PauseFlag = true;
        PauseButtonObjct.GetComponent<Image>().sprite = PauseAfterImage;
    }
}