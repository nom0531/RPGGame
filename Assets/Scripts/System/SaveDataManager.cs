using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    [SerializeField, Header("セーブデータ用設定値"),Tooltip("プレイヤーデータ")]
    private PlayerDataBase PlayerData;
    [SerializeField, Tooltip("エネミーデータ")]
    private EnemyDataBase EnemyData;
    [SerializeField, Tooltip("レベルデータ")]
    private LevelDataBase LevelData;
    [SerializeField, Header("セーブデータ")]
    private SaveData GameSaveData;

    private const bool BOOL = true;
    private const int DEFAULT_EP_POINT = 1500;

    private string m_filePath;  // 書き込み先のファイルパス
    private static readonly string EncryptKey = "c6eahbq9sjuawhvdr9kvhpsm5qv393ga";
    private static readonly int EncryptPasswordCount = 16;
    private static readonly string PasswordChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly int PasswordCharsLength = PasswordChars.Length;

    public SaveData SaveData
    {
        get => GameSaveData;
    }

    private void Start()
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
        // 暗号化
        var json = JsonUtility.ToJson(GameSaveData);
        var iv = "";
        var base64 = "";
        EncryptAesBase64(json, out iv, out base64);
        // 保存
        byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
        byte[] base64Bytes = Encoding.UTF8.GetBytes(base64);
        using (FileStream fs = new FileStream(m_filePath, FileMode.Create, FileAccess.Write))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            bw.Write(ivBytes.Length);
            bw.Write(ivBytes);
            bw.Write(base64Bytes.Length);
            bw.Write(base64Bytes);
            bw.Close();
        }
    }

    /// <summary>
    /// 現在の状況をロードする
    /// </summary>
    /// <returns>成功したらtrue、失敗したらfalseを返す</returns>
    private bool Load()
    {
        if (File.Exists(m_filePath) == false)
        {
            // セーブデータが見つからなかったのでfalseを返す
            return false;
        }
        // 読み込み
        byte[] ivBytes = null;
        byte[] base64Bytes = null;
        using (FileStream fs = new FileStream(m_filePath, FileMode.Open, FileAccess.Read))
        using (BinaryReader br = new BinaryReader(fs))
        {
            int length = br.ReadInt32();
            ivBytes = br.ReadBytes(length);

            length = br.ReadInt32();
            base64Bytes = br.ReadBytes(length);
        }
        // 複合化
        string json;
        string iv = Encoding.UTF8.GetString(ivBytes);
        string base64 = Encoding.UTF8.GetString(base64Bytes);
        DecryptAesBase64(base64, iv, out json);

        // セーブデータ復元
        GameSaveData = JsonUtility.FromJson<SaveData>(json);
        return true;
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

    /// <summary>
    /// AES暗号化(Base64形式)
    /// </summary>
    public static void EncryptAesBase64(string json, out string iv, out string base64)
    {
        byte[] src = Encoding.UTF8.GetBytes(json);
        byte[] dst;
        EncryptAes(src, out iv, out dst);
        base64 = Convert.ToBase64String(dst);
    }

    /// <summary>
    /// AES複合化(Base64形式)
    /// </summary>
    public static void DecryptAesBase64(string base64, string iv, out string json)
    {
        byte[] src = Convert.FromBase64String(base64);
        byte[] dst;
        DecryptAes(src, iv, out dst);
        json = Encoding.UTF8.GetString(dst).Trim('\0');
    }

    /// <summary>
    /// AES暗号化
    /// </summary>
    public static void EncryptAes(byte[] src, out string iv, out byte[] dst)
    {
        iv = CreatePassword(EncryptPasswordCount);
        dst = null;
        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.Mode = CipherMode.CBC;
            rijndael.KeySize = 256;
            rijndael.BlockSize = 128;

            byte[] key = Encoding.UTF8.GetBytes(EncryptKey);
            byte[] vec = Encoding.UTF8.GetBytes(iv);

            using (ICryptoTransform encryptor = rijndael.CreateEncryptor(key, vec))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(src, 0, src.Length);
                cs.FlushFinalBlock();
                dst = ms.ToArray();
            }
        }
    }

    /// <summary>
    /// AES複合化
    /// </summary>
    public static void DecryptAes(byte[] src, string iv, out byte[] dst)
    {
        dst = new byte[src.Length];
        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.Mode = CipherMode.CBC;
            rijndael.KeySize = 256;
            rijndael.BlockSize = 128;

            byte[] key = Encoding.UTF8.GetBytes(EncryptKey);
            byte[] vec = Encoding.UTF8.GetBytes(iv);

            using (ICryptoTransform decryptor = rijndael.CreateDecryptor(key, vec))
            using (MemoryStream ms = new MemoryStream(src))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                cs.Read(dst, 0, dst.Length);
            }
        }
    }

    /// <summary>
    /// パスワード生成
    /// </summary>
    /// <param name="count">文字列数</param>
    /// <returns>パスワード</returns>
    public static string CreatePassword(int count)
    {
        StringBuilder sb = new StringBuilder(count);
        for (int i = count - 1; i >= 0; i--)
        {
            char c = PasswordChars[UnityEngine.Random.Range(0, PasswordCharsLength)];
            sb.Append(c);
        }
        return sb.ToString();
    }
}
