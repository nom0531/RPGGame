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
    private bool m_isOnemore = false;   // �ēx�s���ł��邩�ǂ���

    public bool OneMore
    {
        get => m_isOnemore;
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
        int rand = GetRandomValue(0, 9);    // �␳�l

        float damage =
            (attackATK * 0.5f) - (attackedDEF * 0.25f) + rand;

        damage = Mathf.Max(0.0f, damage);
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
        int rand = GetRandomValue(0, 5);    // �␳�l

        float damage =
            (attackATK + skillPOW * 0.01f) - attackedDEF + rand;

        damage = Mathf.Max(0.0f, damage);
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

        recoveryQuantity = Mathf.Max(0.0f, recoveryQuantity);
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

        statusFloatingValue = Mathf.Max(0.0f, statusFloatingValue);
        return (int)statusFloatingValue;
    }

    /// <summary>
    /// �X�L���U�����̑����ϐ����l�������_���[�W���v�Z����
    /// </summary>
    /// <param name="playerData">�v���C���[�f�[�^</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="skillElement">�X�L���̑���</param>
    /// <param name="damage">�������l�����Ȃ��_���[�W</param>
    /// <returns>�_���[�W�ʁB�����_�ȉ��͐؂�̂�</returns>
    public int PlayerElementResistance(PlayerData playerData, int skillNumber, int skillElement, int damage)
    {
        float finalDamage = damage;
        switch(playerData.PlayerElement[skillElement])
        {
            case global::ElementResistance.enNormal:
                break;
            case global::ElementResistance.enWeak:
                Debug.Log("ONE MORE!");
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

        finalDamage = Mathf.Max(0.0f, finalDamage);
        return (int)finalDamage;
    }

    /// <summary>
    /// �X�L���U�����̑����ϐ����l�������_���[�W���v�Z����
    /// </summary>
    /// <param name="enemyData">�G�l�~�[�f�[�^</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="skillElement">�X�L���̑���</param>
    /// <param name="damage">�������l�����Ȃ��_���[�W</param>
    /// <returns>�_���[�W�ʁB�����_�ȉ��͐؂�̂�</returns>
    public int EnemyElementResistance(EnemyData enemyData, int skillNumber, int skillElement, int damage)
    {
        float finalDamage = damage;
        switch (enemyData.EnemyElement[skillElement])
        {
            case global::ElementResistance.enNormal:
                break;
            case global::ElementResistance.enWeak:
                Debug.Log("ONE MORE!");
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
        int rand = GetRandomValue(0, 100);
        bool flag = false;

        // ������LUCK�̃p�����[�^�ȉ��Ȃ�
        if (rand <= LUCK)
        {
            flag = true;
        }

        return flag;
    }
}
