using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�̃��[��
/// </summary>
public enum PlayerRoll
{ 
    enAttacker, // �A�^�b�J�[
    enBuffer,   // �o�b�t�@�[
    enHealer,   // �q�[���[
}

/// <summary>
/// �v���C���[�̍\����
/// </summary>
[System.Serializable]
public class PlayerData
{
    [SerializeField, Header("�v���C���[���")]
    public string PlayerName;                                       // ���O
    public Sprite PlayerSprite;                                     // �摜
    public PlayerRoll PlayerRoll;                                   // �v���C�����[��
    [SerializeField, Header("�����ϐ�")]
    public ElementResistance[] PlayerElement =
        new ElementResistance[(int)ElementType.enNum];              // �����ϐ�
    [SerializeField, Header("�X�e�[�^�X")]
    public int HP;                                                  // �̗�
    public int SP;                                                  // �X�y�V�����|�C���g
    public int ATK;                                                 // �U����
    public int DEF;                                                 // �h���
    public int SPD;                                                 // �f����
    public int LUCK;                                                // �^
    [SerializeField,Header("�X�L���ꗗ")]
    public List<SkillData> skillDataList = new List<SkillData>();
}

[CreateAssetMenu(fileName ="PlayerDataBase",menuName ="CreatePlayerDataBace")]
public class PlayerDataBase : ScriptableObject
{
    public List<PlayerData> playerDataList = new List<PlayerData>();
}
