using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 強化対象
/// </summary>
public enum EnhancementStatus
{
    enHP,
    enSP,
    enATK,
    enDEF,
    enSPD,
    enLUCK
}

/// <summary>
/// プレイヤーの強化の構造体
/// </summary>
[System.Serializable]
public class PlayerEnhancementData
{
    [SerializeField, Header("情報")]
    public string EnhancementName;                                  // 名前
    public Sprite EnhancementSprite;                                // 画像
    public EnhancementStatus EnhancementStatus;                     // 強化する対象
    public int AddValue;                                            // 強化値
    [SerializeField, Header("必要強化ポイント")]
    public int EnhancementPoint;                                    // 必要強化ポイント
}

[CreateAssetMenu(fileName = "PlayerEnhancementDataBase", menuName = "CreatePlayerEnhancementDataBase")]
public class PlayerEnhancementDataBase : ScriptableObject
{
    public List<PlayerEnhancementData> playerEnhancementDataList = new List<PlayerEnhancementData>();
}
