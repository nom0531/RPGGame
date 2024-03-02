using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField, Header("セーブデータ用設定値"),Tooltip("プレイヤーデータ")]
    private PlayerDataBase PlayerData;
    [SerializeField, Tooltip("エネミーデータ")]
    private EnemyDataBase EnemyData;
    [SerializeField, Tooltip("レベルデータ")]
    private LevelDataBase LevelData;
    [SerializeField, Header("セーブデータ")]
    private SaveData GameSaveData;

    private const bool BOOL = false;
    private const int DEFAULT_EP_POINT = 1500;

    private string m_filePath;  // 書き込み先のファイルパス

    public SaveData SaveData
    {
        get => GameSaveData;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // セーブデータを読み込む
        m_filePath = $"{Application.persistentDataPath}/.savedata.json";
        var isLoad = Load();
        // セーブデータがないなら
        if (isLoad == false)
        {
            InitData();
        }
    }

    /// <summary>
    /// 現在の状況をセーブする
    /// </summary>
    public void Save()
    {
        var json = JsonUtility.ToJson(GameSaveData);
        var streamWriter = new StreamWriter(m_filePath);
        streamWriter.Write(json);
        streamWriter.Close();
    }

    /// <summary>
    /// 現在の状況をロードする
    /// </summary>
    /// <returns>成功したらtrue、失敗したらfalseを返す</returns>
    private bool Load()
    {
        if (File.Exists(m_filePath))
        {
            var streamReader = new StreamReader(m_filePath);
            var data = streamReader.ReadToEnd();
            streamReader.Close();
            GameSaveData = JsonUtility.FromJson<SaveData>(data);
            // ロードが出来たのでtrueを返す
            return true;
        }
        // セーブデータが見つからなかったのでfalseを返す
        return false;
    }

    /// <summary>
    /// データを初期化する処理
    /// </summary>
    private void InitData()
    {
        // データを用意する
        GameSaveData.saveData.EnemyRegisters = new bool[EnemyData.enemyDataList.Count];                 // エネミーの発見度
        GameSaveData.saveData.ElementRegisters = new Element[EnemyData.enemyDataList.Count];            // 属性耐性の発見度
        GameSaveData.saveData.SkillRegisters = new Skill[PlayerData.playerDataList.Count];              // スキルの開放度
        GameSaveData.saveData.EnhancementRegisters = new Enhancement[PlayerData.playerDataList.Count];  // 強化の開放度
        GameSaveData.saveData.PlayerList = new PlayerStatus[PlayerData.playerDataList.Count];           // プレイヤーのステータス
        GameSaveData.saveData.ClearStage = new bool[LevelData.levelDataList.Count];                     // ステージのクリア数
        // 値を初期化
        GameSaveData.saveData.EnhancementPoint = DEFAULT_EP_POINT;                                      // 所持している強化ポイント
        
        // エネミーと戦ったか、エネミーの属性を発見したか
        for (int enemyNumber = 0; enemyNumber < GameSaveData.saveData.EnemyRegisters.Length; enemyNumber++)
        {
            // 発見していない
            GameSaveData.saveData.EnemyRegisters[enemyNumber] = BOOL;
            GameSaveData.saveData.ElementRegisters[enemyNumber] = new Element { Elements = new bool[(int)ElementType.enNum] };

            for(int elementNumber = 0; elementNumber < (int)ElementType.enNum; elementNumber++)
            {
                GameSaveData.saveData.ElementRegisters[enemyNumber].Elements[elementNumber] = BOOL;
            }
        }

        // プレイヤー
        for (int playerNumber = 0; playerNumber < PlayerData.playerDataList.Count; playerNumber++)
        {
            // スキルの開放度
            GameSaveData.saveData.SkillRegisters[playerNumber] =
                new Skill { PlayerSkills = new bool[PlayerData.playerDataList[playerNumber].skillDataList.Count] };
            for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[playerNumber].skillDataList.Count; skillNumber++)
            {
                GameSaveData.saveData.SkillRegisters[playerNumber].PlayerSkills[skillNumber] = BOOL;
            }
            // 強化の開放度
            GameSaveData.saveData.EnhancementRegisters[playerNumber] =
                new Enhancement { PlayerEnhancements = new bool[PlayerData.playerDataList[playerNumber].enhancementDataList.Count] };
            for (int enhancementNumber = 0; enhancementNumber < PlayerData.playerDataList[playerNumber].enhancementDataList.Count; enhancementNumber++)
            {
                GameSaveData.saveData.EnhancementRegisters[playerNumber].PlayerEnhancements[enhancementNumber] = BOOL;
            }
            // ステータスを記録
            GameSaveData.saveData.PlayerList[playerNumber] =
                new PlayerStatus
                {
                    HP = PlayerData.playerDataList[playerNumber].HP,
                    SP = PlayerData.playerDataList[playerNumber].SP,
                    ATK = PlayerData.playerDataList[playerNumber].ATK,
                    DEF = PlayerData.playerDataList[playerNumber].DEF,
                    SPD = PlayerData.playerDataList[playerNumber].SPD,
                    LUCK = PlayerData.playerDataList[playerNumber].LUCK
                };
        }

        // ステージをクリアしているかどうか
        for (int stageNumber = 0; stageNumber < LevelData.levelDataList.Count; stageNumber++)
        {
            // クリアしていない
            GameSaveData.saveData.ClearStage[stageNumber] = BOOL;
        }

        Save();
    }
}
