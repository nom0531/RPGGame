using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エネミーの行動パターンリスト
/// </summary>
[System.Serializable]
public class EnemyMoveData
{
    public string MoveName;                         // 行動の名前
    public int ID;                                  // 識別番号
    public ActorHPState ActorHPState;               // HPの状態
    public ActorAbnormalState ActorAbnormalState;   // 状態異常
    public ActionType ActionType;                   // 行動パターン
}

[CreateAssetMenu(fileName = "EnemyDataMoveBase", menuName = "CreateEnemyMoveDataBase")]
public class EnemyMoveDataBase : ScriptableObject
{
    public List<EnemyMoveData> enemyMoveDataList = new List<EnemyMoveData>();
}
