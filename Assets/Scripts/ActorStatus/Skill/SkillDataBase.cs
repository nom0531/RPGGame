using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�L���̎��
/// </summary>
public enum SkillType
{
    enAttack,       // �U��
    enBuff,         // �o�t
    enDeBuff,       // �f�o�t
    enHeal,         // ��
    enResurrection  // ����
}

/// <summary>
/// �X�L���̌��ʔ͈�
/// </summary>
public enum EffectRange
{
    enOne,      // �P��
    enAll,      // �S��
}

/// <summary>
/// �o�t�̎��
/// </summary>
public enum BuffType
{
    enATK,
    enDEF,
    enSPD,
    enNull,
}

/// <summary>
/// �������
/// </summary>
public enum NecessaryType
{
    enSP,       // SP�������
    enHP,       // HP�������
}

/// <summary>
/// �X�L���̍\����
/// </summary>
[System.Serializable]
public class SkillData
{
    [SerializeField, Header("�X�L�����")]
    public string SkillName;                                        // ���O
    public int ID;                                                  // ���g�̔ԍ�
    public Sprite SkillSprite;                                      // �摜
    public SkillType SkillType;                                     // �X�L���̎��
    public GameObject SkillEffect;                                  // �X�L���̃G�t�F�N�g
    public float EffectScale;                                       // �G�t�F�N�g�̃X�P�[��
    [SerializeField, Header("����")]
    public ElementType SkillElement;                                // �X�L���̑���
    public StateAbnormalData StateAbnormalData;                     // �ǉ�����
    [SerializeField, Header("�X�e�[�^�X")]
    public int POW;                                                 // �U���A�񕜂̊�b�l
    public int SkillNecessary;                                      // �K�vSP/HP��
    public NecessaryType Type;                                      // ����SP/HP
    [SerializeField, Header("�X�L���ڍ�")]
    public EffectRange EffectRange;                                 // �X�L���̌��ʔ͈�
    public TargetState TargetState;                                 // �X�L���̑Ώ�
    public BuffType BuffType;                                       // �o�t�̃^�C�v
    [SerializeField,Header("�K�v�����|�C���g")]
    public int EnhancementPoint;                                    // �K�v�����|�C���g
    [SerializeField, Header("�ڍאݒ�"), Multiline(2)]
    public string SkillDetail;                                      // �X�L���̏ڍאݒ�
}

[CreateAssetMenu(fileName = "SkillDataBase", menuName = "CreateSkillDataBace")]
public class SkillDataBase : ScriptableObject
{
    public List<SkillData> skillDataList = new List<SkillData>();
}
