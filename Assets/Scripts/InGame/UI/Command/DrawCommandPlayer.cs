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
    private float AddRotation = 0.5f;
    [SerializeField, Tooltip("�ړ����x")]
    private float MoveSpeed = 10.0f;

    private BattleManager m_battleManager;
    private TurnManager m_turnManager;
    private PauseManager m_pauseManager;
    private float m_timer = 0.0f;          // �^�C�}�[

    // Start is called before the first frame update
    private void Start()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_pauseManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<PauseManager>();
        m_turnManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<TurnManager>();
        CommandUI.transform.position = PlayerIcon[(int)m_battleManager.OperatingPlayer].transform.position;
    }

    private void FixedUpdate()
    {
        SetUIRotation();
        SetUIPosition();

        // �v���C���łȂ��Ȃ璆�f
        if (m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }
        // �|�[�Y���Ȃ璆�f
        if (m_pauseManager.PauseFlag == true)
        {
            return;
        }
        // �G�l�~�[�̃^�[���͏��������s���Ȃ�
        if (m_turnManager.TurnStatus == TurnStatus.enEnemy)
        {
            return;
        }
        m_timer = 0.0f;
    }

    /// <summary>
    /// UI����]�����鏈��
    /// </summary>
    private void SetUIRotation()
    {
        CommandUI.transform.Rotate(0, 0, -AddRotation);
    }

    /// <summary>
    /// UI�̍��W�����肷��
    /// </summary>
    private void SetUIPosition()
    {
        if(m_timer >= 1.0f)
        {
            return;
        }

        // �n�_�ƏI�_���擾
        var start = CommandUI.transform.position;
        var end = PlayerIcon[(int)m_battleManager.OperatingPlayer].transform.position;
        // ��Ԉʒu���v�Z
        m_timer =+ Time.deltaTime * MoveSpeed;
        // -t^2 + 2t
        float rate = ((Mathf.Pow(m_timer, 2.0f) * -1.0f) + (2.0f * m_timer));
        rate = Mathf.Min(rate, 1.0f);
        // ���W�𔽉f
        CommandUI.transform.position = Vector3.Lerp(start, end, rate);
    }
}
