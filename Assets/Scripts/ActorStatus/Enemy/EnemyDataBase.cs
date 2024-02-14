using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�l�~�[�̃T�C�Y
/// </summary>
public enum EnemySize
{
    enExtraSmal,
    enSmall,
    enMedium,
    enLarge
}

/// <summary>
/// �G�l�~�[�̍\����
/// </summary>
[System.Serializable]
public class EnemyData
{
    [SerializeField,Header("�G�l�~�[���")]
    public string EnemyName;                                        // ���O(8�����܂�)
    public int ID;                                                  // ���g�̔ԍ�
    public Sprite EnemySprite;                                      // �摜
    public EnemySize EnemySize;                                     // �G�l�~�[�̃T�C�Y
    public LocationType PopLocation;                                // �X�|�[�������
    public LocationTime PopTime;                                    // �X�|�[�����鎞��
    public int EnhancementPoint;                                    // �h���b�v���鋭���|�C���g
    [SerializeField,Header("�����ϐ�")]
    public ElementResistance[] EnemyElement 
        = new ElementResistance[(int)ElementType.enNum];            // �����ϐ�
    [SerializeField,Header("�X�e�[�^�X")]
    public int HP;                                                  // �̗�
    public int ATK;                                                 // �U����
    public int DEF;                                                 // �h���
    public int SPD;                                                 // �f����
    public int LUCK;                                                // �^
    [SerializeField,Header("�ڍאݒ�"), Multiline(2)]
    public string EnemyDetail;
    [SerializeField, Header("�X�L���ꗗ")]
    public List<SkillData> skillDataList = new List<SkillData>();
    [SerializeField, Header("�s���p�^�[��")]
    public List<EnemyMoveData> enemyMoveList = new List<EnemyMoveData>();
}

// �G�l�~�[�p�̃f�[�^�x�[�X
[CreateAssetMenu(fileName ="EnemyDataBase",menuName="CreateEnemyDataBase")]
public class EnemyDataBase : ScriptableObject
{
    public List<EnemyData> enemyDataList = new List<EnemyData>();
}