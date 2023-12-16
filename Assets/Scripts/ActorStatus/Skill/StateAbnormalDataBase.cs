using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状態異常のリスト
/// </summary>
[System.Serializable]
public class StateAbnormalData
{
    public string StateName;                        // 名前
    public Sprite StateImage;                       // 表示する画像
    public int StateNumber;                         // 識別番号
    public int POW;                                 // ダメージ量・効果発生の割合
    public int EffectTime;                          // ターン数
    public ActorAbnormalState ActorAbnormalState;   // 状態異常の種類
}

[CreateAssetMenu(fileName = "StateAbnormalDataBase", menuName = "CreateStateAbnormalDataBase")]
public class StateAbnormalDataBase : ScriptableObject
{
    public List<StateAbnormalData> stateAbnormalList = new List<StateAbnormalData>();
}
