using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �o�t�E�f�o�t
/// </summary>
public enum BuffStatus
{
    enBuff_ATK,
    enBuff_DEF,
    enBuff_SPD,
    enDeBuff_ATK,
    enDeBuff_DEF,
    enDeBuff_SPD,
    enNum
}

public class BuffCalculation : MonoBehaviour
{
    private BattleManager m_battleManager;
    private bool[] m_buffFlag =
        new bool[(int)BuffStatus.enNum];    // �o�t���������Ă��邩�ǂ���
    private bool[] m_effectEndFlag =
        new bool[(int)BuffStatus.enNum];    // ���ʎ��Ԃ��I�����Ă��邩�ǂ���
    private int[] m_buffTargetParam =
       new int[(int)BuffStatus.enNum];      // �p�����[�^�̏㏸�E���~�l
    private int[] m_buffEffectTime =
        new int[(int)BuffStatus.enNum];     // �o�t�̌��ʎ���
    private int m_turnSum = 0;              // �^�[���̑���

    /// <summary>
    /// �㏸�l�E���~�l���擾����
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <returns>�ϓ���̒l</returns>
    public int GetBuffParam(BuffStatus buffStatus)
    {
        int param = m_buffTargetParam[(int)buffStatus];
        return param;
    }

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

    private void Awake()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
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
        if(m_battleManager.PauseFlag == true)
        {
            return;
        }

        // �ێ����Ă���o�߃^�[���������݂̌o�߃^�[�����Ɠ����Ȃ璆�f
        if(m_turnSum == m_battleManager.TurnSum)
        {
            return;
        }
        // �l���X�V����
        m_turnSum = m_battleManager.TurnSum;

        for (int i = 0; i< (int)BuffStatus.enNum; i++)
        {
            if(m_buffFlag[i] == false)
            {
                // �o�t���������Ă��Ȃ��Ȃ玟
                continue;
            }

            Decrement((BuffStatus)i);
        }
    }

    /// <summary>
    /// ���ʎ��Ԃ����炷
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    private void Decrement(BuffStatus buffStatus)
    {
        m_buffEffectTime[(int)buffStatus]--;

        // �������ʎ��Ԃ�0�ȉ��Ȃ�
        if (m_buffEffectTime[(int)buffStatus] <= 0)
        {
            SetEffectEndFlag(buffStatus, true);
            gameObject.GetComponent<DrawCommandText>().ReSetStatusText();
        }
    }

    /// <summary>
    /// �o�t�����������Ƃ��̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <param name="statusFloatingValue">�����l</param>
    /// <param name="originalValue">���̒l</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <returns>�ϓ���̒l</returns>
    public int CalcBuff(BuffStatus buffStatus, int statusFloatingValue, int originalValue, int effectTime)
    {
        // ���ɂ������Ă���Ȃ�
        if (m_buffFlag[(int)buffStatus] == true)
        {
            SetEffectTime(buffStatus, effectTime);  // ���ʎ��Ԃ�ǉ�

            Debug.Log("���ʎ��Ԃ�" + effectTime + "�L�т�");

            return originalValue;
        }

        SetBuffParam(buffStatus, statusFloatingValue);
        SetBuffFlag(buffStatus, true);
        SetEffectTime(buffStatus, effectTime);

        Debug.Log(buffStatus + "���㏸");

        return originalValue + statusFloatingValue;
    }

    /// <summary>
    /// �f�o�t�����������Ƃ��̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    /// <param name="statusFloatingValue">�����l</param>
    /// <param name="originalValue">���̒l</param>
    ///     /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <returns>�ϓ���̒l</returns>
    public int CalcDebuff(BuffStatus buffStatus, int statusFloatingValue, int originalValue, int effectTime)
    {
        // ���ɂ������Ă���Ȃ�
        if (m_buffFlag[(int)buffStatus] == true)
        {
            SetEffectTime(buffStatus, effectTime);   // ���ʎ��Ԃ�ǉ�
            Debug.Log("���ʎ��Ԃ�" + effectTime + "�L�т�");

            return originalValue;
        }

        SetBuffParam(buffStatus, statusFloatingValue);
        SetBuffFlag(buffStatus, true);
        SetEffectTime(buffStatus, effectTime);

        Debug.Log(buffStatus + "������");

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
