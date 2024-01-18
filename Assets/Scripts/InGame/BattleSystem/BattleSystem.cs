using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �s���p�^�[��
/// </summary>
public enum ActionType
{ 
    enAttack,               // �ʏ�U��
    enSkillAttack,          // �X�L���U��
    enGuard,                // �h��
    enEscape,               // ����
    enNull,                 // �������Ȃ�
}

/// <summary>
/// HP�̏��
/// </summary>
public enum ActorHPState
{
    enMaxHP,                // HP���ő�
    enFewHP,                // HP�����Ȃ�
    enDie,                  // �Ђ�
    enNull,                 // �w��Ȃ�
}

/// <summary>
/// ��Ԉُ�
/// </summary>
public enum ActorAbnormalState
{
    enNormal,               // �ʏ���
    enPoison,               // ��
    enParalysis,            // ���
    enSilence,              // ����
    enConfusion,            // ����
    enNull,                 // �w��Ȃ�
}

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

public class BattleSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private EnemyDataBase EnemyData;

    private bool m_isOnemore = false;   // �ēx�s���ł��邩�ǂ���
    private bool m_isHit = false;       // �U���������邩�ǂ���

    private const int NORMAL_ATTACK_PROBABILITY = 95;
    private const int SKILL_ATTACK_PROBABILITY = 95;
    private const int SKILL_HEAL_PROBABILITY = 100;
    private const int SKILL_BUFF_PROBABILITY = 95;

    public bool OneMore
    {
        get => m_isOnemore;
    }

    public bool Hit
    {
        get => m_isHit;
    }

    /// <summary>
    /// �����𐶐�����֐�
    /// </summary>
    /// <param name="min">�ŏ��l</param>
    /// <param name="max">�ő�l</param>
    /// <param name="isShouldAdd">int�̎d�l���l�������v�Z���s�����ǂ���</param>
    /// <returns>�ŏ��l����ő�l�̊ԂŒl��Ԃ�</returns>
    public int GetRandomValue(int min, int max, bool isShouldAdd=true)
    {
        if (isShouldAdd)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }

        return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// �ʏ�U���̏���
    /// </summary>
    /// <param name="attackATK">�U�����̍U����</param>
    /// <param name="attackedDEF">�h�䑤�̖h���</param>
    /// <returns>�_���[�W�ʁB�����_�ȉ��͐؂�̂�</returns>
    public int NormalAttack(int attackATK, int attackedDEF)
    {
        var rand = GetRandomValue(0, 9);    // �␳�l

        float damage =
            (attackATK * 0.5f) - (attackedDEF * 0.25f) + rand;
        // �␳
        damage = Mathf.Max(0.0f, damage);
        damage = AttackHit(damage, NORMAL_ATTACK_PROBABILITY);
        return (int)damage;
    }

    /// <summary>
    /// �X�L���ł̍U���̏���
    /// </summary>
    /// <param name="attackATK">�U�����̍U����</param>
    /// <param name="skillPOW">�X�L���̊�{�l</param>
    /// <param name="attackedDEF">�h�䑤�̖h���</param>
    /// <returns>�_���[�W�ʁB�����_�ȉ��͐؂�̂�</returns>
    public int SkillAttack(int attackATK, int skillPOW, int attackedDEF)
    {
        var rand = GetRandomValue(0, 5);    // �␳�l

        float damage =
            (attackATK + skillPOW * 0.01f) - attackedDEF + rand;
        // �␳
        damage = Mathf.Max(0.0f, damage);
        damage = AttackHit(damage, SKILL_ATTACK_PROBABILITY);
        return (int)damage;
    }

    /// <summary>
    /// �X�L���ł̉񕜂̏���
    /// </summary>
    /// <param name="attackedMaxHP">�g�p����鑤�̍ő�HP</param>
    /// <param name="skillPOW">�X�L���̊�{�l</param>
    /// <returns>�񕜗ʁB�����_�ȉ��͐؂�̂�</returns>
    public int SkillHeal(int attackedMaxHP, int skillPOW)
    {
        float recoveryQuantity = attackedMaxHP * (skillPOW * 0.01f);
        // �␳
        recoveryQuantity = Mathf.Max(0.0f, recoveryQuantity);
        recoveryQuantity = AttackHit(recoveryQuantity, SKILL_HEAL_PROBABILITY);
        return (int)recoveryQuantity;
    }

    /// <summary>
    /// �X�L���ł̃o�t�E�f�o�t�̏���
    /// </summary>
    /// <param name="attackedParam">�g�p����鑤�̃p�����[�^</param>
    /// <param name="skillPOW">�X�L���̊�{�l</param>
    /// <returns>�ϓ���̒l�B�����_�ȉ��͐؂�̂�</returns>
    public int SkillBuff(int attackedParam,int skillPOW)
    {
        float statusFloatingValue = attackedParam * (skillPOW * 0.01f);
        // �␳
        statusFloatingValue = Mathf.Max(0.0f, statusFloatingValue);
        statusFloatingValue = AttackHit(statusFloatingValue, SKILL_BUFF_PROBABILITY);
        return (int)statusFloatingValue;
    }

    /// <summary>
    /// �X�L���U�����̑����ϐ����l�������_���[�W���v�Z����
    /// </summary>
    /// <param name="playerNumber">�v���C���[�̔ԍ�</param>
    /// <param name="skillElement">�X�L���̑���</param>
    /// <param name="damage">�������l�����Ȃ��_���[�W</param>
    /// <returns>�_���[�W�ʁB�����_�ȉ��͐؂�̂�</returns>
    public int PlayerElementResistance(int playerNumber, int skillElement, int damage)
    {
        float finalDamage = damage;
        switch(EnemyData.enemyDataList[playerNumber].EnemyElement[skillElement])
        {
            case global::ElementResistance.enNormal:
                break;
            case global::ElementResistance.enWeak:
                finalDamage *= 2.0f;

                // �ēx�s���ł��邩�ǂ����̃t���O
                if (m_isOnemore == true)
                {
                    // ����true�ɂȂ��Ă���Ȃ�t���O��߂�
                    m_isOnemore = false;
                }
                else
                {
                    // �����łȂ��Ȃ�ēx�s���ł���悤�ɂ���
                    m_isOnemore = true;
                }
                break;
            case global::ElementResistance.enResist:
                finalDamage *= 0.5f;
                break;
        }
        // �␳
        finalDamage = Mathf.Max(0.0f, finalDamage);
        return (int)finalDamage;
    }

    /// <summary>
    /// �X�L���U�����̑����ϐ����l�������_���[�W���v�Z����
    /// </summary>
    /// <param name="enemyNumber">�G�l�~�[�̔ԍ�</param>
    /// <param name="skillElement">�X�L���̑���</param>
    /// <param name="damage">�������l�����Ȃ��_���[�W</param>
    /// <returns>�_���[�W�ʁB�����_�ȉ��͐؂�̂�</returns>
    public int EnemyElementResistance(int enemyNumber, int skillElement, int damage)
    {
        float finalDamage = damage;
        switch (EnemyData.enemyDataList[enemyNumber].EnemyElement[skillElement])
        {
            case global::ElementResistance.enNormal:
                break;
            case global::ElementResistance.enWeak:
                finalDamage *= 2.0f;

                // �ēx�s���ł��邩�ǂ����̃t���O
                if (m_isOnemore == true)
                {
                    // ����true�ɂȂ��Ă���Ȃ�t���O��߂�
                    m_isOnemore = false;
                }
                else
                {
                    // �����łȂ��Ȃ�ēx�s���ł���悤�ɂ���
                    m_isOnemore = true;
                }
                break;
            case global::ElementResistance.enResist:
                finalDamage *= 0.5f;
                break;
        }
        // �␳
        finalDamage = Mathf.Max(0.0f, finalDamage);
        return (int)finalDamage;
    }

    /// <summary>
    /// �h�䏈��
    /// </summary>
    /// <param name="attackDEF">�g�p���鑤��DEF</param>
    /// <returns>�h��́B�����_�ȉ��͐؂�̂�</returns>
    public int Guard(int attackDEF)
    {
        float defensePower = attackDEF + attackDEF * 0.01f;
        return (int)defensePower;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="LUCK">���g�̉^</param>
    /// <returns>true�Ȃ琬���Bfalse�Ȃ玸�s</returns>
    public bool Escape(int LUCK)
    {
        var rand = GetRandomValue(0, 100);
        var flag = false;

        // ������LUCK�̃p�����[�^�ȉ��Ȃ�
        if (rand <= LUCK)
        {
            flag = true;
        }

        return flag;
    }

    /// <summary>
    /// �U���������邩�ǂ����̔���
    /// </summary>
    /// <param name="value">�_���[�W</param>
    /// <param name="probability">�U����������m��</param>
    /// <returns>�ŏI�_���[�W</returns>
    private float AttackHit(float value, int probability)
    {
        var rand = GetRandomValue(0, 100);
        // �������m���ȉ��Ȃ�
        if (rand <= probability)
        {
            m_isHit = true;
            return value;
        }
        m_isHit = false;
        return 0;
    }
}
