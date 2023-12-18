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
        int rand = m_battleSystem.GetRandomValue(0, 100);
        // ���ȉ��Ȃ�true��Ԃ�
        if(rand > SPENT_PROBABILITY)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// �ŏ�Ԃ̏���
    /// </summary>
    /// <param name="nowAbnormalState">���݂̏�Ԉُ�</param>
    /// <param name="AttuckerHP">�U�����󂯂鑤��HP</param>
    /// <returns>�_���[�W�ʁB�����_�ȉ��͐؂�̂�</returns>
    public int Poison(ActorAbnormalState nowAbnormalState, int AttuckerHP)
    {
        float damage = 0.0f;

        // �ŏ�ԂłȂ��Ȃ�_���[�W�͖���
        if (nowAbnormalState != ActorAbnormalState.enPoison)
        {
            return (int)damage;
        }

        damage = AttuckerHP * (StateAbnormalData.stateAbnormalList[(int)StateNumber.enPoison].POW * 0.01f);
        return (int)damage;
    }

    /// <summary>
    /// ��჏�Ԃ̏���
    /// <param name="nowAbnormalState">���݂̏�Ԉُ�</param>
    /// </summary>
    /// <returns>true�Ȃ�s���s�Bfalse�Ȃ�s���\</returns>
    public bool Paralysis(ActorAbnormalState nowAbnormalState)
    {
        // ��჏�ԂłȂ��Ȃ�s���\
        if(nowAbnormalState != ActorAbnormalState.enParalysis)
        {
            return false;
        }

        int rand = m_battleSystem.GetRandomValue(0, 100);
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
    /// <param name="nowAbnormalState">���݂̏�Ԉُ�</param>
    /// <returns>true�Ȃ�^�[�Q�b�g��ύX�Bfalse�Ȃ�ύX���Ȃ�</returns>
    public bool Confusion(ActorAbnormalState nowAbnormalState)
    {
        // ��჏�ԂłȂ��Ȃ�s���\
        if (nowAbnormalState != ActorAbnormalState.enConfusion)
        {
            return false;
        }

        int rand = m_battleSystem.GetRandomValue(0, 100);
        // ���ȉ��Ȃ�^�[�Q�b�g��ύX
        if (rand <= StateAbnormalData.stateAbnormalList[(int)StateNumber.enConfusion].POW)
        {
            return true;
        }
        return false;
    }
}
