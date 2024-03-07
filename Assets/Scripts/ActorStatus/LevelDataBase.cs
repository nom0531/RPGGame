using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 難易度
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
/// レベルの構造体
/// </summary>
[System.Serializable]
public class LevelData
{
    [SerializeField, Header("レベル情報")]
    public string LevelName;                                    // レベルの名前(クエスト名)
    public LevelState LevelState;                               // 難易度
    [SerializeField, Header("詳細設定"), Multiline(3)]
    public string LevelDetail;                                  // 説明
    [SerializeField, Header("出現エネミー一覧")]
    public List<EnemyData> enemyDataList = new List<EnemyData>();   // 出現するエネミー一覧
}

[CreateAssetMenu(fileName = "LevelDataBase", menuName = "CreateLevelDataBace")]
public class LevelDataBase : ScriptableObject
{
    public List<LevelData> levelDataList = new List<LevelData>();
}
