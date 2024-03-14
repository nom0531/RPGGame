using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Element
{
    public bool[] Elements;                         // エネミーの属性を発見しているかどうか
}

[System.Serializable]
public class Skill
{
    public bool[] PlayerSkills;                     // スキルの個数
}

[System.Serializable]
public class Enhancement
{
    public bool[] PlayerEnhancements;               // スキルの個数
}

[System.Serializable]
public class PlayerStatus
{
    public int HP;
    public int SP;
    public int ATK;
    public int DEF;
    public int SPD;
    public int LUCK;
}

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct Data
    {
        public float BGMVolume;                     // BGMの音量
        public float SEVolume;                      // SEの音量
        public bool[] EnemyRegisters;               // エネミーを発見しているかどうか
        public Element[] ElementRegisters;          // 属性の発見状況
        public Skill[] SkillRegisters;              // スキルの開放状況
        public Enhancement[] EnhancementRegisters;  // 強化の開放状況
        public PlayerStatus[] PlayerList;           // プレイヤーのステータス
        public bool[] ClearStage;                   // ステージをクリアしているかどうか
        public int EnhancementPoint;                // 強化ポイント
    }

    // 各情報
    public Data saveData;
}