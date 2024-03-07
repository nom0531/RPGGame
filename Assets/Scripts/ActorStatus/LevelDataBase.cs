using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Փx
/// </summary>
public enum LevelState
{
    enOne,
    enTwo,
    enThree,
    enFour,
    enFive,
}

/// <summary>
/// ���x���̍\����
/// </summary>
[System.Serializable]
public class LevelData
{
    [SerializeField, Header("���x�����")]
    public string LevelName;                                    // ���x���̖��O(�N�G�X�g��)
    public LevelState LevelState;                               // ��Փx
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
