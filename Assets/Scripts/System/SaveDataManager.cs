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
    [SerializeField, Header("�Z�[�u�f�[�^")]
    private SaveData GameSaveData;

    private const bool BOOL = false;
    private const int DEFAULT_EP_POINT = 1500;

    private string m_filePath;  // �������ݐ�̃t�@�C���p�X

    public SaveData SaveData
    {
        get => GameSaveData;
    }

    private void Awake()
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
        var json = JsonUtility.ToJson(GameSaveData);
        var streamWriter = new StreamWriter(m_filePath);
        streamWriter.Write(json);
        streamWriter.Close();
    }

    /// <summary>
    /// ���݂̏󋵂����[�h����
    /// </summary>
    /// <returns>����������true�A���s������false��Ԃ�</returns>
    private bool Load()
    {
        if (File.Exists(m_filePath))
        {
            var streamReader = new StreamReader(m_filePath);
            var data = streamReader.ReadToEnd();
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
}
