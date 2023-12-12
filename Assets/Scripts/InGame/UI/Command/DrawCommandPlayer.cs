using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCommandPlayer : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"), Tooltip("�v���C���[�A�C�R��")]
    private GameObject[] PlayerIcon;
    [SerializeField, Tooltip("COMMAND��UI")]
    private GameObject CommandUI;
    [SerializeField, Header("�\���f�[�^"), Tooltip("��]��")]
    private float m_addRotation = -0.5f;

    private BattleManager m_battleManager;
    private int m_operatingPlayer = 0;     // ���ݑ��삵�Ă���v���C���[

    // Start is called before the first frame update
    private void Start()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        SetUIPosition();
    }

    private void FixedUpdate()
    {
        SetUIRotation();

        // �v���C���łȂ��Ȃ璆�f
        if (m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }
        // �|�[�Y���Ȃ璆�f
        if (m_battleManager.PauseFlag == true)
        {
            return;
        }

        // �G�l�~�[�̃^�[���@�܂��́@�ԍ����������͏��������s���Ȃ�
        if (m_battleManager.TurnStatus == TurnStatus.enEnemy
            || m_operatingPlayer == m_battleManager.OperatingPlayerNumber)
        {
            return;
        }

        SetUIPosition();
    }

    /// <summary>
    /// UI����]�����鏈��
    /// </summary>
    private void SetUIRotation()
    {
        CommandUI.transform.Rotate(0, 0, m_addRotation);
    }

    /// <summary>
    /// UI�̍��W�����肷��
    /// </summary>
    private void SetUIPosition()
    {
        m_operatingPlayer = m_battleManager.OperatingPlayerNumber;
        CommandUI.transform.position = PlayerIcon[m_operatingPlayer].transform.position;
    }
}
