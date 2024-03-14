using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Element
{
    public bool[] Elements;                         // �G�l�~�[�̑����𔭌����Ă��邩�ǂ���
}

[System.Serializable]
public class Skill
{
    public bool[] PlayerSkills;                     // �X�L���̌�
}

[System.Serializable]
public class Enhancement
{
    public bool[] PlayerEnhancements;               // �X�L���̌�
}

[System.Serializable]
public class PlayerStatus
{
    public int HP;
    public int SP;
    public int ATK;
    public int DEF;
    public int SPD;
    public int LUCK;
}

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct Data
    {
        public float BGMVolume;                     // BGM�̉���
        public float SEVolume;                      // SE�̉���
        public bool[] EnemyRegisters;               // �G�l�~�[�𔭌����Ă��邩�ǂ���
        public Element[] ElementRegisters;          // �����̔�����
        public Skill[] SkillRegisters;              // �X�L���̊J����
        public Enhancement[] EnhancementRegisters;  // �����̊J����
        public PlayerStatus[] PlayerList;           // �v���C���[�̃X�e�[�^�X
        public bool[] ClearStage;                   // �X�e�[�W���N���A���Ă��邩�ǂ���
        public int EnhancementPoint;                // �����|�C���g
    }

    // �e���
    public Data saveData;
}