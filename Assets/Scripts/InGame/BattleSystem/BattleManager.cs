using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���̃X�e�[�^�X
/// </summary>
public enum GameState
{
    enPlay,         // �Q�[����
    enBattleWin,    // �o�g���I���B����
    enBattleLose,   // �o�g���I���B�s�k
}

/// <summary>
/// ���삵�Ă���L�����N�^�[
/// </summary>
public enum OperatingPlayer
{
    enAttacker, // �A�^�b�J�[
    enBuffer,   // �o�b�t�@�[
    enHealer,   // �q�[���[
    enNum
}

public class BattleManager : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private EnemyDataBase EnemyData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField]
    private LevelDataBase LevelData;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject PauseCanvas;
    [SerializeField, Tooltip("�v���C���[�̃A�C�R��")]
    private GameObject[] PlayerIcon;
    [SerializeField, Tooltip("�R�}���h�I�𒆂̃A�C�R��")]
    private GameObject CommandIcon;
    [SerializeField, Header("�o�g���f�[�^"), Tooltip("��������摜")]
    private GameObject Sprite;
    [SerializeField, Tooltip("�摜��ǉ�����I�u�W�F�N�g")]
    private GameObject Content;
    [SerializeField, Tooltip("�G�l�~�[�̃T�C�Y")]
    private float EnemySpriteSize = 450.0f;
    [SerializeField]
    private BattleButton[] BattleButton;

    private const int MAX_ENEMY_NUM = 4;                        // �o�g���ɏo������G�l�~�[�̍ő吔
    private const float ADD_SIZE = 1.6f;                        // �G�l�~�[�̉摜�̏�Z�T�C�Y

    private GameState m_gameState;                              // �Q�[���̃X�e�[�g
    private OperatingPlayer m_operatingPlayer;                  // ���삵�Ă���v���C���[
    private BattleSystem m_battleSystem;                        // �o�g���V�X�e��
    private PauseManager m_pauseManager;
    private TurnManager m_turnManager;                          // �^�[���Ǘ��V�X�e��
    private PlayerTurn m_playerTurn;                            // �v���C���[���̓���
    private EnemyTurn m_enemyTurn;                              // �G�l�~�[���̓���
    private StagingManager m_stagingManager;                    // ���o�p�V�X�e��
    private DrawStatusValue m_drawStatusValue;                  // �X�e�[�^�X��\������UI
    private List<PlayerMove> m_playerMoveList;                  // �v���C���[�̍s��
    private List<EnemyMove> m_enemyMoveList;                    // �G�l�~�[�̍s��
    private int m_enemySum = 0;                                 // �G�l�~�[�̑���
    private int m_enemyNumber = 0;                              // �G�l�~�[�̔ԍ�
    private bool m_isStagingStart = false;                      // ���o���J�n�������ǂ����Bfalse�Ȃ�J�n���Ă��Ȃ�

    public OperatingPlayer OperatingPlayer
    {
        get => m_operatingPlayer;
        set => m_operatingPlayer = value;
    }

    public GameState GameState
    {
        get => m_gameState;
        set => m_gameState = value;
    }

    public PlayerDataBase PlayerDataBase
    {
        get => PlayerData;
    }

    public EnemyDataBase EnemyDataBase
    {
        get => EnemyData;
    }

    public SkillDataBase SkillDataBase
    {
        get => SkillData;
    }

    public bool StagingStartFlag
    {
        get => m_isStagingStart;
        set => m_isStagingStart = value;
    }

    public int EnemySum
    {
        get => m_enemySum;
    }

    public List<PlayerMove> PlayerMoveList
    {
        get => m_playerMoveList;
    }

    public List<EnemyMove> EnemyMoveList
    {
        get => m_enemyMoveList;
    }

    private void Awake()
    {
        m_pauseManager = GetComponent<PauseManager>();
        m_battleSystem = GetComponent<BattleSystem>();
        m_drawStatusValue = GetComponent<DrawStatusValue>();
        m_stagingManager = GetComponent<StagingManager>();
        m_playerTurn = GetComponent<PlayerTurn>();
        m_enemyTurn = GetComponent<EnemyTurn>();
        m_turnManager = GetComponent<TurnManager>();
    }

    private void Start()
    {
        var SPD = int.MinValue;
        for (int i = 0; i < (int)OperatingPlayer.enNum; i++)
        {
            // SPD�̃p�����[�^����Ȃ�X�V����
            if (PlayerData.playerDataList[i].SPD >= SPD)
            {
                SPD = PlayerData.playerDataList[i].SPD;
                m_operatingPlayer = (OperatingPlayer)i;
            }
        }
        var playerMoveList = FindObjectsOfType<PlayerMove>();
        m_playerMoveList = new List<PlayerMove>(playerMoveList);
        m_playerMoveList.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));    // �ԍ����Ƀ\�[�g

        var levelNumber = GameManager.Instance.LevelNumber;

        // �G�l�~�[��p�ӂ���
        m_enemySum = m_battleSystem.GetRandomValue(1, MAX_ENEMY_NUM);
        // �G�l�~�[�̉摜��p�ӂ���
        for (int i = 0; i < m_enemySum; i++)
        {
            var sprite = Instantiate(Sprite);
            sprite.transform.SetParent(Content.transform);
            // �X�v���C�g��ݒ肷��
            var rand = m_battleSystem.GetRandomValue(0, LevelData.levelDataList[levelNumber].enemyDataList.Count, false);
            var number = LevelData.levelDataList[levelNumber].enemyDataList[rand].ID;
            sprite.GetComponent<SpriteRenderer>().sprite = EnemyData.enemyDataList[number].EnemySprite;
            // �G�l�~�[�Ɏ��g�̔ԍ���������
            var enemyMove = sprite.GetComponent<EnemyMove>();
            enemyMove.MyNumber = EnemyData.enemyDataList[number].ID;
            // �T�C�Y�A���W�𒲐�
            var width = EnemySpriteSize * ADD_SIZE;
            var height = EnemySpriteSize * ADD_SIZE;
            sprite.transform.localScale = new Vector3(width, height, 1.0f);
            sprite.transform.localPosition = Vector3.zero;
            sprite.transform.localRotation = Quaternion.identity;
            // �}�ӂɖ��o�^�Ȃ�
            if (enemyMove.GetTrueEnemyRegister(enemyMove.MyNumber) == false)
            {
                // �o�^����
                enemyMove.SetTrueEnemyRegister(enemyMove.MyNumber);
            }
        }
        var enemyMoveList = FindObjectsOfType<EnemyMove>();
        m_enemyMoveList = new List<EnemyMove>(enemyMoveList);

        // �ŏ��̑���L�����N�^�[�����肷��
        InitValue();
    }

    // Update is called once per frame
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_gameState = GameState.enBattleWin;
        }
#endif
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        Debug.Log($"���݂̃^�[�����F{m_turnManager.TurnSum}�^�[����");
#endif
        // �Q�[�����I�����Ă���Ȃ�A������ȉ��̏����͎��s����Ȃ�
        if (GameState != GameState.enPlay)
        {
            return;
        }
        // �|�[�Y���Ă���Ȃ���s���Ȃ�
        if (m_pauseManager.PauseFlag == true)
        {
            return;
        }
        // ���o���J�n���ꂽ�Ȃ���s���Ȃ�
        if(m_stagingManager.StangingState == StagingState.enStangingStart)
        {
            return;
        }
        switch (m_turnManager.TurnStatus)
        {
            // �v���C���[�̃^�[��
            case TurnStatus.enPlayer:
                switch (m_operatingPlayer)
                {
                    case OperatingPlayer.enAttacker:
                    case OperatingPlayer.enBuffer:
                    case OperatingPlayer.enHealer:
                        m_playerTurn.PlayerAction((int)m_operatingPlayer);
                        break;
                }
                break;
            // �G�l�~�[�̃^�[��
            case TurnStatus.enEnemy:
                for (int enemyNumber = m_enemyNumber; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                {
                    // ���S���Ă���ۂ͎��s���Ȃ�
                    if (m_enemyMoveList[enemyNumber].ActorHPState == ActorHPState.enDie)
                    {
                        continue;
                    }
                    m_enemyTurn.EnemyAction(enemyNumber);
                }
                m_enemyNumber++;
                break;
        }
        UpdateUIStatus();
        m_turnManager.IsTurnEnd();
    }

    /// <summary>
    /// �l������������
    /// </summary>
    public void InitValue()
    {
        m_operatingPlayer = NextOperatingPlayer();
        m_enemyNumber = 0;
    }

    /// <summary>
    /// ���̑���L�����N�^�[�����肷��
    /// </summary>
    public OperatingPlayer NextOperatingPlayer()
    {
        var operatingPlayer = OperatingPlayer;
        var SPD = int.MinValue;

        for (int i = 0; i < (int)OperatingPlayer.enNum; i++)
        {
            // ���ɍs�����Ă���Ȃ珈�������s���Ȃ�
            if (m_playerMoveList[i].ActionEndFlag == true)
            {
                continue;
            }
            // �Ђ񎀂Ȃ珈�������s���Ȃ�
            if (m_playerMoveList[i].ActorHPState == ActorHPState.enDie)
            {
                continue;
            }
            // SPD�̃p�����[�^����Ȃ�X�V
            if (PlayerData.playerDataList[i].SPD > SPD)
            {
                SPD = PlayerData.playerDataList[i].SPD;
                operatingPlayer = (OperatingPlayer)i;
            }
        }
        // �s�����I�����Ă���Ȃ�^�[����n��
        if (m_playerMoveList[0].ActionEndFlag == true
        && m_playerMoveList[1].ActionEndFlag == true
        && m_playerMoveList[2].ActionEndFlag == true)
        {
            m_turnManager.TurnStatus = TurnStatus.enEnemy;
        }
        return operatingPlayer;
    }

    /// <summary>
    /// UI���X�V����
    /// </summary>
    private void UpdateUIStatus()
    {
        m_drawStatusValue.SetStatus();
        m_drawStatusValue.SetStatusText();
    }

    /// <summary>
    /// �X�e�[�^�X������������
    /// </summary>
    public void ResetGameStatus()
    {
        // �X�e�[�^�X��������
        ResetPlayerAction();
        ResetEnemyAction();
        ResetBattleButton();
    }

    /// <summary>
    /// �v���C���[�̃X�e�[�^�X�����Z�b�g����
    /// </summary>
    private void ResetPlayerAction()
    {
        for (int i = 0; i < m_playerMoveList.Count; i++)
        {
            m_playerMoveList[i].ResetStatus();
        }
    }

    /// <summary>
    /// �R�}���h�̏�Ԃ����Z�b�g����
    /// </summary>
    public void ResetBattleButton()
    {
        for (int i = 0; i < BattleButton.Length; i++)
        {
            BattleButton[i].ResetStatus();
        }
    }

    /// <summary>
    /// �����ϐ��̓o�^����
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    public void PlayerAction_Register(int myNumber, int skillNumber, int targetNumber)
    {
        // ���ɑ����ϐ��𔭌����Ă���Ȃ���s���Ȃ�
        if (m_enemyMoveList[targetNumber].GetTrueElementRegister
            ((int)PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement) != true)
        {
            return;
        }
        // �t���O��ture�ɂ���
        m_enemyMoveList[targetNumber].SetTrueElementRegister
            ((int)PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement);
    }

    /// <summary>
    /// �G�l�~�[�Ƀ_���[�W��^����
    /// </summary>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="damage">�_���[�W��</param>
    public void DamageEnemy(int targetNumber, int damage)
    {
        EnemyMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// �G�l�~�[�̏�Ԃ�ύX����
    /// </summary>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="actorAbnormalState">�ύX��̏��</param>
    public void PlayerAction_ChangeStateEnemy(int targetNumber, ActorAbnormalState actorAbnormalState)
    {
        EnemyMoveList[targetNumber].ActorAbnormalState = actorAbnormalState;
    }

    /// <summary>
    /// �v���C���[�Ƀ_���[�W��^����
    /// </summary>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="damage">�_���[�W��</param>
    public void DamagePlayer(int targetNumber, int damage)
    {
        m_playerMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// �G�l�~�[�̏�Ԃ�ύX����
    /// </summary>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="actorAbnormalState">�ύX��̏��</param>
    public void EnemyAction_ChangeStatePlayer(int targetNumber, ActorAbnormalState actorAbnormalState)
    {
        m_enemyMoveList[targetNumber].ActorAbnormalState = actorAbnormalState;
    }

    /// <summary>
    /// �G�l�~�[�̃X�e�[�^�X�����Z�b�g����
    /// </summary>
    private void ResetEnemyAction()
    {
        for (int i = 0; i < m_enemyMoveList.Count; i++)
        {
            m_enemyMoveList[i].ResetStatus();
        }
    }

    /// <summary>
    /// ���X�g�ɒǉ����A�ԍ����Đݒ肷��
    /// </summary>
    /// <returns>�^�[�Q�b�g�̔ԍ�</returns>
    public int EnemyListAdd(EnemyMove enemyMove)
    {
        m_enemyMoveList.Add(enemyMove);
        return (int)m_enemyMoveList.Count - 1;
    }

    /// <summary>
    /// ���X�g���玩�g���폜����
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    public void EnemyListRemove(int myNumber)
    {
        for(int i= 0; i < m_enemyMoveList.Count; i++)
        {
            if(m_enemyMoveList[i].MyNumber != myNumber)
            {
                continue;
            }
            Destroy(m_enemyMoveList[i].gameObject);
            m_enemyMoveList.Remove(m_enemyMoveList[i]);
            break;
        }
    }
}
