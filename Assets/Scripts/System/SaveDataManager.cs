using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField, Header("�Z�[�u�f�[�^�p�ݒ�l"),Tooltip("�v���C���[�f�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField, Tooltip("�G�l�~�[�f�[�^")]
    private EnemyDataBase EnemyData;
    [SerializeField, Tooltip("���x���f�[�^")]
    private LevelDataBase LevelData;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private SaveData GameSaveData;

    private const bool BOOL = true;
    private string m_filePath;  // �������ݐ�̃t�@�C���p�X

    public SaveData SaveData
    {
        get => GameSaveData;
    }

    private void Awake()
    {
        // ���g�̓V�[�����ׂ��ł��폜����Ȃ��悤�ɂ���
        DontDestroyOnLoad(gameObject);

        // �Z�[�u�f�[�^��ǂݍ���
        m_filePath = $"{Application.persistentDataPath}/.savedata.json";
        var isLoad = Load();

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
        string json = JsonUtility.ToJson(GameSaveData);
        StreamWriter streamWriter = new StreamWriter(m_filePath);
        streamWriter.Write(json);
        streamWriter.Close();
    }

    /// <summary>
    /// ���݂̏󋵂����[�h����
    /// </summary>
    /// <returns>����������true�A���s������false��Ԃ�</returns>
    public bool Load()
    {
        if (File.Exists(m_filePath))
        {
            var streamReader = new StreamReader(m_filePath);
            string data = streamReader.ReadToEnd();
            streamReader.Close();
            GameSaveData = JsonUtility.FromJson<SaveData>(data);
            // ���[�h���o�����̂�true��Ԃ�
            return true;
        }
        // �Z�[�u�f�[�^��������Ȃ������̂�false��Ԃ�
        return false;
    }

    /// <summary>
    /// �f�[�^�����������鏈��
    /// </summary>
    private void InitData()
    {
        // �f�[�^��p�ӂ���
        GameSaveData.saveData.EnemyRegister = new bool[EnemyData.enemyDataList.Count];
        GameSaveData.saveData.ElementRegister = new Element[EnemyData.enemyDataList.Count];
        GameSaveData.saveData.Players = new Player[PlayerData.playerDataList.Count];
        GameSaveData.saveData.ClearStage = new bool[LevelData.levelDataList.Count];
        // �l��������
        GameSaveData.saveData.EnhancementPoint = 500;
        
        // �G�l�~�[�Ɛ�������A�G�l�~�[�̑����𔭌�������
        for (int i = 0;i< GameSaveData.saveData.EnemyRegister.Length; i++)
        {
            // �������Ă��Ȃ�
            GameSaveData.saveData.EnemyRegister[i] = BOOL;
            GameSaveData.saveData.ElementRegister[i] = new Element { Elements = new bool[(int)ElementType.enNum] };

            for(int j = 0; j < (int)ElementType.enNum; j++)
            {
                GameSaveData.saveData.ElementRegister[i].Elements[j] = BOOL;
            }
        }

        // �X�L���̊J���x
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

        // �X�e�[�W���N���A���Ă��邩�ǂ���
        for (int i = 0;i < LevelData.levelDataList.Count; i++)
        {
            // �N���A���Ă��Ȃ�
            GameSaveData.saveData.ClearStage[i] = BOOL;
        }

        Save();
    }
}
