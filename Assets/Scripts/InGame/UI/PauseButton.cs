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

    private BattleManager m_battleManager;

    private void Start()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void ButtonDown()
    {
        m_battleManager.PauseFlag = true;
        PauseButtonObjct.GetComponent<Image>().sprite = PauseAfterImage;
    }

    /// <summary>
    /// �Q�[���ɖ߂�{�^���������ꂽ���̏���
    /// </summary>
    public void ReturnGameButtonDown()
    {
        m_battleManager.PauseFlag = false;
    }
}