using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Element
{
    public bool[] Elements;                 // エネミーの属性を発見しているかどうか
}

[System.Serializable]
public class Player
{
    public bool[] PlayerEnhancement;        // スキルの個数
}

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct Data
    {
        public bool[] EnemyRegister;        // エネミーを発見しているかどうか
        public Element[] ElementRegister;   // 属性の発見状況
        public Player[] Players;            // スキルの開放状況
        public bool[] ClearStage;           // ステージをクリアしているかどうか
        public int EnhancementPoint;        // 強化ポイント
    }

    // 各情報
    public Data saveData;
}