using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�l�~�[�̍s���p�^�[�����X�g
/// </summary>
[System.Serializable]
public class EnemyMoveData
{
    public string MoveName;                         // �s���̖��O
    public int MoveNumber;                          // ���ʔԍ�
    public ActorHPState ActorHPState;               // HP�̏��
    public ActorAbnormalState ActorAbnormalState;   // ��Ԉُ�
    public ActionType ActionType;                   // �s���p�^�[��
    public bool IsMove;                             // �s�����s�����ǂ���
}

[CreateAssetMenu(fileName = "EnemyDataMoveBase", menuName = "CreateEnemyMoveDataBase")]
public class EnemyMoveDataBase : ScriptableObject
{
    public List<EnemyMoveData> enemyMoveDataList = new List<EnemyMoveData>();
}
