using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�|�[�������
/// </summary>
public enum LocationType
{
    enHell,         // ����
    enForest,       // �X
    enSea,          // �C
    enVolcano,      // �ΎR
    enAllLocation,  // �֌W�Ȃ�
}

/// <summary>
/// ���x���̍\����
/// </summary>
[System.Serializable]
public class LevelData
{
    [SerializeField, Header("���x�����")]
    public string LevelName;                                    // ���x���̖��O(�N�G�X�g��)
    public Texture LocationTexture;                             // ���̉摜
    public LocationType LocationType;                           // ���x���̊�
    [SerializeField, Header("�ڍאݒ�"), Multiline(3)]
    public string LevelDetail;                                  // ����
    [SerializeField, Header("�o���G�l�~�[�ꗗ")]
    public List<EnemyData> enemyDataList = new List<EnemyData>();   // �o������G�l�~�[�ꗗ
}

[CreateAssetMenu(fileName = "LevelDataBase", menuName = "CreateLevelDataBace")]
public class LevelDataBase : ScriptableObject
{
    public List<LevelData> levelDataList = new List<LevelData>();
}
