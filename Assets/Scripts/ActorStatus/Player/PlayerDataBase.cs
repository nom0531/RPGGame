using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのロール
/// </summary>
public enum PlayerRoll
{ 
    enAttacker, // アタッカー
    enBuffer,   // バッファー
    enHealer,   // ヒーラー
}

/// <summary>
/// プレイヤーの構造体
/// </summary>
[System.Serializable]
public class PlayerData
{
    [SerializeField, Header("プレイヤー情報")]
    public string PlayerName;                                       // 名前
    public Sprite PlayerSprite;                                     // 画像
    public PlayerRoll PlayerRoll;                                   // プレイヤロール
    [SerializeField, Header("属性耐性")]
    public ElementResistance[] PlayerElement =
        new ElementResistance[(int)ElementType.enNum];              // 属性耐性
    [SerializeField, Header("ステータス")]
    public int HP;                                                  // 体力
    public int SP;                                                  // スペシャルポイント
    public int ATK;                                                 // 攻撃力
    public int DEF;                                                 // 防御力
    public int SPD;                                                 // 素早さ
    public int LUCK;                                                // 運
    [SerializeField,Header("スキル一覧")]
    public List<SkillData> skillDataList = new List<SkillData>();
}

[CreateAssetMenu(fileName ="PlayerDataBase",menuName ="CreatePlayerDataBace")]
public class PlayerDataBase : ScriptableObject
{
    public List<PlayerData> playerDataList = new List<PlayerData>();
}
