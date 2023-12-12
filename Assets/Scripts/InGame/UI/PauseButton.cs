using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject PauseCanvas;

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
        PauseCanvas.SetActive(true);
        m_battleManager.PauseFlag = true;
    }

    /// <summary>
    /// �Q�[���ɖ߂�{�^���������ꂽ���̏���
    /// </summary>
    public void ReturnGameButtonDown()
    {
        PauseCanvas.SetActive(false);
        m_battleManager.PauseFlag = false;
    }
}