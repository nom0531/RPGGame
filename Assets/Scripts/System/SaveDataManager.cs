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
    [SerializeField, Header("参照オブジェクト")]
    private SaveData GameSaveData;

    private const bool BOOL = true;
    private string m_filePath;  // 書き込み先のファイルパス

    public SaveData SaveData
    {
        get => GameSaveData;
    }

    private void Awake()
    {
        // 自身はシーンを跨いでも削除されないようにする
        DontDestroyOnLoad(gameObject);

        // セーブデータを読み込む
        m_filePath = $"{Application.persistentDataPath}/.savedata.json";
        var isLoad = Load();

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
        string json = JsonUtility.ToJson(GameSaveData);
        StreamWriter streamWriter = new StreamWriter(m_filePath);
        streamWriter.Write(json);
        streamWriter.Close();
    }

    /// <summary>
    /// 現在の状況をロードする
    /// </summary>
    /// <returns>成功したらtrue、失敗したらfalseを返す</returns>
    public bool Load()
    {
        if (File.Exists(m_filePath))
        {
            var streamReader = new StreamReader(m_filePath);
            string data = streamReader.ReadToEnd();
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
        GameSaveData.saveData.EnemyRegister = new bool[EnemyData.enemyDataList.Count];
        GameSaveData.saveData.ElementRegister = new Element[EnemyData.enemyDataList.Count];
        GameSaveData.saveData.Players = new Player[PlayerData.playerDataList.Count];
        GameSaveData.saveData.ClearStage = new bool[LevelData.levelDataList.Count];
        // 値を初期化
        GameSaveData.saveData.EnhancementPoint = 500;
        
        // エネミーと戦ったか、エネミーの属性を発見したか
        for (int i = 0;i< GameSaveData.saveData.EnemyRegister.Length; i++)
        {
            // 発見していない
            GameSaveData.saveData.EnemyRegister[i] = BOOL;
            GameSaveData.saveData.ElementRegister[i] = new Element { Elements = new bool[(int)ElementType.enNum] };

            for(int j = 0; j < (int)ElementType.enNum; j++)
            {
                GameSaveData.saveData.ElementRegister[i].Elements[j] = BOOL;
            }
        }

        // スキルの開放度
        GameSaveData.saveData.Players[0] = new Player { PlayerEnhancement = new bool[PlayerData.playerDataList[0].skillDataList.Count] };
        for(int i = 0;i< PlayerData.playerDataList[0].skillDataList.Count; i++)
        {
            GameSaveData.saveData.Players[0].PlayerEnhancement[i] = BOOL;
        }

        GameSaveData.saveData.Players[1] = new Player { PlayerEnhancement = new bool[PlayerData.playerDataList[1].skillDataList.Count] };
        for (int i = 0; i < PlayerData.playerDataList[1].skillDataList.Count; i++)
        {
            GameSaveData.saveData.Players[1].PlayerEnhancement[i] = BOOL;
        }
        
        GameSaveData.saveData.Players[2] = new Player { PlayerEnhancement = new bool[PlayerData.playerDataList[2].skillDataList.Count] };
        for (int i = 0; i < PlayerData.playerDataList[2].skillDataList.Count; i++)
        {
            GameSaveData.saveData.Players[2].PlayerEnhancement[i] = BOOL;
        }

        // ステージをクリアしているかどうか
        for (int i = 0;i < LevelData.levelDataList.Count; i++)
        {
            // クリアしていない
            GameSaveData.saveData.ClearStage[i] = BOOL;
        }

        Save();
    }
}
