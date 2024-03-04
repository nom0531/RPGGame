using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{
    [SerializeField, Header("�Z�[�u�f�[�^�p�ݒ�l"),Tooltip("�v���C���[�f�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField, Tooltip("�G�l�~�[�f�[�^")]
    private EnemyDataBase EnemyData;
    [SerializeField, Tooltip("���x���f�[�^")]
    private LevelDataBase LevelData;
    [SerializeField, Header("�Z�[�u�f�[�^")]
    private SaveData GameSaveData;

    private const bool BOOL = true;
    private const int DEFAULT_EP_POINT = 1500;

    private string m_filePath;  // �������ݐ�̃t�@�C���p�X
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
        // �Z�[�u�f�[�^��ǂݍ���
        m_filePath = $"{Application.persistentDataPath}/.savedata.json";
        var isLoad = Load();
        // �Z�[�u�f�[�^���Ȃ��Ȃ�
        if (isLoad == false)
        {
            InitData();
        }
    }

    /// <summary>
    /// ���݂̏󋵂��Z�[�u����
    /// </summary>
    public void Save()
    {
        // �Í���
        var json = JsonUtility.ToJson(GameSaveData);
        var iv = "";
        var base64 = "";
        EncryptAesBase64(json, out iv, out base64);
        // �ۑ�
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
    /// ���݂̏󋵂����[�h����
    /// </summary>
    /// <returns>����������true�A���s������false��Ԃ�</returns>
    private bool Load()
    {
        if (File.Exists(m_filePath) == false)
        {
            // �Z�[�u�f�[�^��������Ȃ������̂�false��Ԃ�
            return false;
        }
        // �ǂݍ���
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
        // ������
        string json;
        string iv = Encoding.UTF8.GetString(ivBytes);
        string base64 = Encoding.UTF8.GetString(base64Bytes);
        DecryptAesBase64(base64, iv, out json);

        // �Z�[�u�f�[�^����
        GameSaveData = JsonUtility.FromJson<SaveData>(json);
        return true;
    }

    /// <summary>
    /// �f�[�^�����������鏈��
    /// </summary>
    private void InitData()
    {
        // �f�[�^��p�ӂ���
        GameSaveData.saveData.EnemyRegisters = new bool[EnemyData.enemyDataList.Count];                 // �G�l�~�[�̔����x
        GameSaveData.saveData.ElementRegisters = new Element[EnemyData.enemyDataList.Count];            // �����ϐ��̔����x
        GameSaveData.saveData.SkillRegisters = new Skill[PlayerData.playerDataList.Count];              // �X�L���̊J���x
        GameSaveData.saveData.EnhancementRegisters = new Enhancement[PlayerData.playerDataList.Count];  // �����̊J���x
        GameSaveData.saveData.PlayerList = new PlayerStatus[PlayerData.playerDataList.Count];           // �v���C���[�̃X�e�[�^�X
        GameSaveData.saveData.ClearStage = new bool[LevelData.levelDataList.Count];                     // �X�e�[�W�̃N���A��
        // �l��������
        GameSaveData.saveData.EnhancementPoint = DEFAULT_EP_POINT;                                      // �������Ă��鋭���|�C���g
        
        // �G�l�~�[�Ɛ�������A�G�l�~�[�̑����𔭌�������
        for (int enemyNumber = 0; enemyNumber < GameSaveData.saveData.EnemyRegisters.Length; enemyNumber++)
        {
            // �������Ă��Ȃ�
            GameSaveData.saveData.EnemyRegisters[enemyNumber] = BOOL;
            GameSaveData.saveData.ElementRegisters[enemyNumber] = new Element { Elements = new bool[(int)ElementType.enNum] };

            for(int elementNumber = 0; elementNumber < (int)ElementType.enNum; elementNumber++)
            {
                GameSaveData.saveData.ElementRegisters[enemyNumber].Elements[elementNumber] = BOOL;
            }
        }

        // �v���C���[
        for (int playerNumber = 0; playerNumber < PlayerData.playerDataList.Count; playerNumber++)
        {
            // �X�L���̊J���x
            GameSaveData.saveData.SkillRegisters[playerNumber] =
                new Skill { PlayerSkills = new bool[PlayerData.playerDataList[playerNumber].skillDataList.Count] };
            for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[playerNumber].skillDataList.Count; skillNumber++)
            {
                GameSaveData.saveData.SkillRegisters[playerNumber].PlayerSkills[skillNumber] = BOOL;
            }
            // �����̊J���x
            GameSaveData.saveData.EnhancementRegisters[playerNumber] =
                new Enhancement { PlayerEnhancements = new bool[PlayerData.playerDataList[playerNumber].enhancementDataList.Count] };
            for (int enhancementNumber = 0; enhancementNumber < PlayerData.playerDataList[playerNumber].enhancementDataList.Count; enhancementNumber++)
            {
                GameSaveData.saveData.EnhancementRegisters[playerNumber].PlayerEnhancements[enhancementNumber] = BOOL;
            }
            // �X�e�[�^�X���L�^
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

        // �X�e�[�W���N���A���Ă��邩�ǂ���
        for (int stageNumber = 0; stageNumber < LevelData.levelDataList.Count; stageNumber++)
        {
            // �N���A���Ă��Ȃ�
            GameSaveData.saveData.ClearStage[stageNumber] = BOOL;
        }

        Save();
    }

    /// <summary>
    /// AES�Í���(Base64�`��)
    /// </summary>
    public static void EncryptAesBase64(string json, out string iv, out string base64)
    {
        byte[] src = Encoding.UTF8.GetBytes(json);
        byte[] dst;
        EncryptAes(src, out iv, out dst);
        base64 = Convert.ToBase64String(dst);
    }

    /// <summary>
    /// AES������(Base64�`��)
    /// </summary>
    public static void DecryptAesBase64(string base64, string iv, out string json)
    {
        byte[] src = Convert.FromBase64String(base64);
        byte[] dst;
        DecryptAes(src, iv, out dst);
        json = Encoding.UTF8.GetString(dst).Trim('\0');
    }

    /// <summary>
    /// AES�Í���
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
    /// AES������
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
    /// �p�X���[�h����
    /// </summary>
    /// <param name="count">������</param>
    /// <returns>�p�X���[�h</returns>
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
