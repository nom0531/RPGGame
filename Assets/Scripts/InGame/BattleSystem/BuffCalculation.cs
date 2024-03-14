using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffCalculation : MonoBehaviour
{
    private BattleManager m_battleManager;
    private TurnManager m_turnManager;
    private PauseManager m_pauseManager;
    private bool[] m_buffFlag =
        new bool[(int)BuffStatus.enNum];     // �o�t���������Ă��邩�ǂ���
    private bool[] m_effectEndFlag =
        new bool[(int)BuffStatus.enNum];    // ���ʎ��Ԃ��I�����Ă��邩�ǂ���
    private int[] m_buffTargetParam =
       new int[(int)BuffStatus.enNum];      // �p�����[�^�̏㏸�E���~�l
    private int[] m_buffEffectTime =
        new int[(int)BuffStatus.enNum];     // �o�t�̌��ʎ���
    private int m_turnSum = 0;              // �^�[���̑���

    /// <summary>
    /// �㏸�l�E���~�l��ݒ肷��
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <param name="value">�ϓ���̒l</param>
    public void SetBuffParam(BuffStatus buffStatus, int value)
    {
        m_buffTargetParam[(int)buffStatus] = value;
    }

    /// <summary>
    /// �o�t���|�����Ă��邩�ǂ����̃t���O��ݒ肷��
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <param name="flag">true�Ȃ炩�����Ă���Bfalse�Ȃ炩�����Ă��Ȃ�</param>
    private void SetBuffFlag(BuffStatus buffStatus, bool flag)
    {
        m_buffFlag[(int)buffStatus] = flag;
    }

    /// <summary>
    /// �o�t���������Ă��邩�ǂ����̃t���O���擾����
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <returns>ture�Ȃ炩�����Ă���Bfalse�Ȃ炩�����Ă��Ȃ�</returns>
    public bool GetBuffFlag(BuffStatus buffStatus)
    {
        return m_buffFlag[(int)buffStatus];
    }

    /// <summary>
    /// ���ʎ��Ԃ��I�����Ă��邩�ǂ����̃t���O��ݒ肷��
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <param name="flag">ture�Ȃ�I�����Ă���Bfalse�Ȃ�I�����Ă��Ȃ�</param>
    public void SetEffectEndFlag(BuffStatus buffStatus, bool flag)
    {
        m_effectEndFlag[(int)buffStatus] = flag;
    }

    /// <summary>
    /// ���ʎ��Ԃ��I�����Ă��邩�ǂ����̃t���O���擾����
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <returns>ture�Ȃ�I�����Ă���Bfalse�Ȃ�I�����Ă��Ȃ�</returns>
    public bool GetEffectEndFlag(BuffStatus buffStatus)
    {
        return m_effectEndFlag[(int)buffStatus];
    }

    /// <summary>
    /// ���ʎ��Ԃ�ݒ肷��
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <param name="effectTime">���ʎ���</param>
    private void SetEffectTime(BuffStatus buffStatus, int effectTime)
    {
        m_buffEffectTime[(int)buffStatus] += effectTime;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_turnManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<TurnManager>();
        m_pauseManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<PauseManager>();
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        for (int i = 0; i < (int)BuffStatus.enNum; i++)
        {
            // �l������������
            m_buffFlag[i] = false;
            m_buffTargetParam[i] = 0;
            m_buffEffectTime[i] = 0;
        }
    }

    private void FixedUpdate()
    {
        // �v���C���łȂ��Ȃ璆�f
        if(m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }
        // �|�[�Y���Ȃ璆�f
        if(m_pauseManager.PauseFlag == true)
        {
            return;
        }
        // �ێ����Ă���o�߃^�[���������݂̌o�߃^�[�����Ɠ����Ȃ璆�f
        if(m_turnSum == m_turnManager.TurnSum)
        {
            return;
        }
        // �l���X�V����
        m_turnSum = m_turnManager.TurnSum;

        for (int i = 0; i< (int)BuffStatus.enNum; i++)
        {
            if(m_buffFlag[i] == false)
            {
                // �o�t���������Ă��Ȃ��Ȃ玟
                continue;
            }

            Decrement((BuffStatus)i);
#if UNITY_EDITOR
            Debug.Log("�c����ʎ���" + m_buffEffectTime[i]);
#endif
        }
    }

    /// <summary>
    /// ���ʎ��Ԃ����炷
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    private void Decrement(BuffStatus buffStatus)
    {
        // ���ɏI�����Ă���Ȃ���s���Ȃ�
        if(m_buffEffectTime[(int)buffStatus] < 0)
        {
            return;
        }

        m_buffEffectTime[(int)buffStatus]--;

        // �������ʎ��Ԃ�0�ȉ��Ȃ�
        if (m_buffEffectTime[(int)buffStatus] <= 0)
        {
            SetEffectEndFlag(buffStatus, true);
            GetComponent<DrawCommandText>().ReSetStatusText();
        }
    }

    /// <summary>
    /// �o�t�����������Ƃ��̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <param name="statusFloatingValue">�����l</param>
    /// <param name="originalValue">���̒l</param>
    /// <returns>�ϓ���̒l</returns>
    public int CalcBuff(BuffStatus buffStatus, int statusFloatingValue, int originalValue, int effectTime)
    {
        // ���ɂ������Ă���Ȃ�
        if (m_buffFlag[(int)buffStatus] == true)
        {
            SetEffectTime(buffStatus, effectTime);  // ���ʎ��Ԃ�ǉ�

#if UNITY_EDITOR
            Debug.Log("���ʎ��Ԃ�" + effectTime + "�L�т�");
#endif
            return originalValue;
        }
        // �l��ݒ肷��
        SetBuffParam(buffStatus, statusFloatingValue);
        SetBuffFlag(buffStatus, true);
        SetEffectTime(buffStatus, effectTime);

#if UNITY_EDITOR
        Debug.Log(buffStatus + "���㏸");
#endif
        return originalValue + statusFloatingValue;
    }

    /// <summary>
    /// �f�o�t�����������Ƃ��̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <param name="statusFloatingValue">�����l</param>
    /// <param name="originalValue">���̒l</param>
    /// <returns>�ϓ���̒l</returns>
    public int CalcDebuff(BuffStatus buffStatus, int statusFloatingValue, int originalValue, int effectTime)
    {
        // ���ɂ������Ă���Ȃ�
        if (m_buffFlag[(int)buffStatus] == true)
        {
            SetEffectTime(buffStatus, effectTime);   // ���ʎ��Ԃ�ǉ�
#if UNITY_EDITOR
            Debug.Log("���ʎ��Ԃ�" + effectTime + "�L�т�");
#endif

            return originalValue;
        }
        // �l��ݒ肷��
        SetBuffParam(buffStatus, statusFloatingValue);
        SetBuffFlag(buffStatus, true);
        SetEffectTime(buffStatus, effectTime);

#if UNITY_EDITOR
        Debug.Log(buffStatus + "������");
#endif
        return originalValue - statusFloatingValue;
    }

    /// <summary>
    /// �X�e�[�^�X��߂�
    /// </summary>
    public int ResetStatus(BuffStatus buffStatus, int originalValue, bool isBuff)
    {
        if(isBuff == true)
        {
            return originalValue - m_buffTargetParam[(int)buffStatus];
        }
        return originalValue + m_buffTargetParam[(int)buffStatus];
    }
}
