using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Element
{
    public bool[] Elements;                 // �G�l�~�[�̑����𔭌����Ă��邩�ǂ���
}

[System.Serializable]
public class Player
{
    public bool[] PlayerEnhancement;        // �X�L���̌�
}

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct Data
    {
        public bool[] EnemyRegister;        // �G�l�~�[�𔭌����Ă��邩�ǂ���
        public Element[] ElementRegister;   // �����̔�����
        public Player[] Players;            // �X�L���̊J����
        public bool[] ClearStage;           // �X�e�[�W���N���A���Ă��邩�ǂ���
        public int EnhancementPoint;        // �����|�C���g
    }

    // �e���
    public Data saveData;
}