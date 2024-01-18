using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAbnormalCalculation : MonoBehaviour
{
   private enum StateNumber
    {
        enPoison = 7,
        enParalysis,
        enSilence,
        enConfusion,
    }

    [SerializeField, Header("�Q�ƃf�[�^")]
    private StateAbnormalDataBase StateAbnormalData;
    private BattleSystem m_battleSystem;

    private const int SPENT_PROBABILITY = 80;   // ��Ԉُ�ɂ�����m��
    private const int RECOVER_PROBABILITY = 60; // ��Ԉُ킩�畜������m��
    private const int ADDVALUE = 10;            // ���Z�l

    private int m_recoverProbability = 0;       // ��������m��
    private int m_count = 0;                    // ��Ԉُ킩�畜�����Ȃ�������

    private void Start()
    {
        m_battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
    }

    /// <summary>
    /// ��Ԉُ�ɂȂ邩�ǂ����̔���
    /// </summary>
    /// <returns>true�Ȃ炩�������Bfalse�Ȃ炩����Ȃ�����</returns>
    public bool SpentToStateAbnormal()
    {
        var rand = m_battleSystem.GetRandomValue(0, 100);
        // ���ȉ��Ȃ�true��Ԃ�
        if(rand > SPENT_PROBABILITY)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// ��Ԉُ킩�畜�����邩�ǂ����̔���
    /// </summary>
    /// <param name="actorAbnormalState">���݂̏�Ԉُ�</param>
    /// <returns>true�Ȃ畜���Bfalse�Ȃ畜�����Ȃ�</returns>
    public bool RecoverToAbnormal(ActorAbnormalState actorAbnormalState)
    {
        // ���^�[�����o�߂��Ă���Ȃ狭������
        if(StateAbnormalData.stateAbnormalList[(int)actorAbnormalState].EffectTime <= m_count)
        {
            return true;
        }
        var rand = m_battleSystem.GetRandomValue(0, 100);
        // ���ȉ��Ȃ�true��Ԃ�
        if (rand > m_recoverProbability)
        {
            m_recoverProbability = RECOVER_PROBABILITY;
            return true;
        }
        m_count++;
        m_recoverProbability += ADDVALUE;   // �m�����㏸������
        return false;
    }

    /// <summary>
    /// �ŏ�Ԃ̏���
    /// </summary>
    /// <param name="AttuckerHP">�U�����󂯂鑤��HP</param>
    /// <returns>�_���[�W�ʁB�����_�ȉ��͐؂�̂�</returns>
    public int Poison(int AttuckerHP)
    {
        var damage = AttuckerHP * (StateAbnormalData.stateAbnormalList[(int)StateNumber.enPoison].POW * 0.01f);
        return (int)damage;
    }

    /// <summary>
    /// ��჏�Ԃ̏���
    /// </summary>
    /// <returns>true�Ȃ�s���s�Bfalse�Ȃ�s���\</returns>
    public bool Paralysis()
    {
        var rand = m_battleSystem.GetRandomValue(0, 100);
        // ���ȉ��Ȃ�s���s��
        if (rand <= StateAbnormalData.stateAbnormalList[(int)StateNumber.enParalysis].POW)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// ������Ԃ̏���
    /// </summary>
    /// <returns>true�Ȃ�^�[�Q�b�g��ύX�Bfalse�Ȃ�ύX���Ȃ�</returns>
    public bool Confusion()
    {
        var rand = m_battleSystem.GetRandomValue(0, 100);
        // ���ȉ��Ȃ�^�[�Q�b�g��ύX
        if (rand <= StateAbnormalData.stateAbnormalList[(int)StateNumber.enConfusion].POW)
        {
            return true;
        }
        return false;
    }
}
