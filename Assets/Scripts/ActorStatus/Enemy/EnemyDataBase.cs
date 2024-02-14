using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エネミーのサイズ
/// </summary>
public enum EnemySize
{
    enExtraSmal,
    enSmall,
    enMedium,
    enLarge
}

/// <summary>
/// エネミーの構造体
/// </summary>
[System.Serializable]
public class EnemyData
{
    [SerializeField,Header("エネミー情報")]
    public string EnemyName;                                        // 名前(8文字まで)
    public int ID;                                                  // 自身の番号
    public Sprite EnemySprite;                                      // 画像
    public EnemySize EnemySize;                                     // エネミーのサイズ
    public LocationType PopLocation;                                // スポーンする環境
    public LocationTime PopTime;                                    // スポーンする時間
    public int EnhancementPoint;                                    // ドロップする強化ポイント
    [SerializeField,Header("属性耐性")]
    public ElementResistance[] EnemyElement 
        = new ElementResistance[(int)ElementType.enNum];            // 属性耐性
    [SerializeField,Header("ステータス")]
    public int HP;                                                  // 体力
    public int ATK;                                                 // 攻撃力
    public int DEF;                                                 // 防御力
    public int SPD;                                                 // 素早さ
    public int LUCK;                                                // 運
    [SerializeField,Header("詳細設定"), Multiline(2)]
    public string EnemyDetail;
    [SerializeField, Header("スキル一覧")]
    public List<SkillData> skillDataList = new List<SkillData>();
    [SerializeField, Header("行動パターン")]
    public List<EnemyMoveData> enemyMoveList = new List<EnemyMoveData>();
}

// エネミー用のデータベース
[CreateAssetMenu(fileName ="EnemyDataBase",menuName="CreateEnemyDataBase")]
public class EnemyDataBase : ScriptableObject
{
    public List<EnemyData> enemyDataList = new List<EnemyData>();
}