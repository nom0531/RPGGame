using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// �^�[�����񂷑�
/// </summary>
public enum TurnStatus
{
    enPlayer,   // �v���C���[
    enEnemy,    // �G�l�~�[
}

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
    private LevelDataBase LevelData;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject PauseCanvas;
    [SerializeField]
    private BattleButton[] BattleButton;
    [SerializeField, Tooltip("�v���C���[�̃A�C�R��")]
    private GameObject[] PlayerIcon;
    [SerializeField, Tooltip("�R�}���h�I�𒆂̃A�C�R��")]
    private GameObject CommandIcon;
    [SerializeField, Header("�o�g���p�f�[�^"), Tooltip("��������摜")]
    private GameObject Sprite;
    [SerializeField, Tooltip("�摜��ǉ�����I�u�W�F�N�g")]
    private GameObject Content;
    [SerializeField, Header("�o�g���f�[�^"), Tooltip("�^�[���J�n���̐�s��")]
    private TurnStatus m_turnStatus = TurnStatus.enPlayer;
    [SerializeField, Tooltip("�G�l�~�[�̃T�C�Y")]
    private float EnemySpriteSize = 450.0f;

    private const int MAX_ENEMY_NUM = 4;                        // �o�g���ɏo������G�l�~�[�̍ő吔
    private const float ADD_SIZE = 1.0f;                        // �G�l�~�[�̉摜�̏�Z�T�C�Y

    private GameState m_gameState = GameState.enPlay;           // �Q�[���̏��
    private OperatingPlayer m_operatingPlayer;                  // ���삵�Ă���v���C���[
    private BattleSystem m_battleSystem;                        // �o�g���V�X�e��
    private StagingManager m_stagingManager;                    // ���o�p�V�X�e��
    private LockOnSystem m_lockOnSystem;                        // ���b�N�I���V�X�e��
    private DrawStatusValue m_drawStatusValue;                  // �X�e�[�^�X��\������UI
    private DrawBattleResult m_drawBattleResult;                // ���o
    private List<PlayerMove> m_playerMoveList;                  // �v���C���[�̍s��
    private List<EnemyMove> m_enemyMoveList;                    // �G�l�~�[�̍s��
    private int m_turnSum = 1;                                  // �����^�[����
    private int m_selectQuestNumber = 0;                        // �I�������N�G�X�g�̔ԍ�
    private int m_enemySum = 0;                                 // �G�l�~�[�̑���
    private int m_enemyNumber = 0;                              // �G�l�~�[�̔ԍ�
    private bool m_isPause = false;                             // �|�[�Y��ʂ��ǂ���
    private bool m_isPushDown = false;                          // �{�^���������ꂽ���ǂ���
    private bool m_isTurnEnd = false;                           // �^�[�����I������

    public bool PauseFlag
    {
        get => m_isPause;
        set => m_isPushDown = value;
    }

    public int OperatingPlayerNumber
    {
        get => (int)m_operatingPlayer;
    }

    public List<PlayerMove> PlayerMoveList
    {
        get => m_playerMoveList;
    }

    public List<EnemyMove> EnemyMoveList
    {
        get => m_enemyMoveList;
    }

    public int TurnSum
    {
        get => m_turnSum;
    }

    public TurnStatus TurnStatus
    {
        get => m_turnStatus;
    }

    public GameState GameState
    {
        get => m_gameState;
    }

    private void Awake()
    {
        m_battleSystem = gameObject.GetComponent<BattleSystem>();
        m_lockOnSystem = gameObject.GetComponent<LockOnSystem>();
        m_drawStatusValue = gameObject.GetComponent<DrawStatusValue>();
        m_drawBattleResult = gameObject.GetComponent<DrawBattleResult>();
        m_stagingManager = gameObject.GetComponent<StagingManager>();

        var SPD = int.MinValue;
        for(int i = 0; i < (int)OperatingPlayer.enNum; i++)
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
    }

    private void Start()
    {
        DrawStatus();

        // �G�l�~�[��p�ӂ���
        m_enemySum = m_battleSystem.GetRandomValue(1, MAX_ENEMY_NUM);
        // �G�l�~�[�̉摜��p�ӂ���
        for (int i = 0; i < m_enemySum; i++)
        {
            var sprite = Instantiate(Sprite);
            sprite.transform.SetParent(Content.transform);
            // �X�v���C�g��ݒ肷��
            int rand = m_battleSystem.GetRandomValue(0, 1, false);
            sprite.GetComponent<SpriteRenderer>().sprite = EnemyData.enemyDataList[rand].EnemySprite;
            // �G�l�~�[�Ɏ��g�̔ԍ���������
            sprite.GetComponent<EnemyMove>().MyNumber = EnemyData.enemyDataList[rand].EnemyNumber;
            // �T�C�Y���擾����
            float width = EnemySpriteSize * ADD_SIZE;
            float height = EnemySpriteSize * ADD_SIZE;
            // �T�C�Y�A���W�𒲐�
            sprite.transform.localScale = new Vector3(width, height, 1.0f);
            sprite.transform.localPosition = Vector3.zero;
            sprite.transform.localRotation = Quaternion.identity;
            // �}�ӂɖ��o�^�Ȃ�
            if (sprite.GetComponent<EnemyMove>().GetTrueEnemyRegister(
                EnemyData.enemyDataList[rand].EnemyNumber) == false)
            {
                // �o�^����
                sprite.GetComponent<EnemyMove>().SetTrueEnemyRegister(
                    EnemyData.enemyDataList[rand].EnemyNumber);
            }
        }
        var enemyMoveList = FindObjectsOfType<EnemyMove>();
        m_enemyMoveList = new List<EnemyMove>(enemyMoveList);
        // �ŏ��̑���L�����N�^�[�����肷��
        m_operatingPlayer = NextOperatingPlayer();
        m_enemyNumber = 0;
        // �^�X�N��ݒ肷��
        GameClearTask().Forget();
        GameOverTask().Forget();
    }

    // Update is called once per frame
    private void Update()
    {
        // �|�[�Y����
        if (m_isPushDown == true)
        {
            if (m_isPause)
            {
                PauseCanvas.SetActive(false);
            }
            else
            {
                PauseCanvas.SetActive(true);
            }

            m_isPause = !m_isPause;     // �t���O�𔽓]������
            m_isPushDown = false;       // �t���O��߂�
        }
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    m_playerMoveList[1].DecrementSP(50);
        //}
    }

    private void FixedUpdate()
    {
        Debug.Log($"���݂̃^�[�����F{m_turnSum}�^�[����");
        IsGameClear();
        IsGameOver();

        // �Q�[�����I�����Ă���Ȃ�A������ȉ��̏����͎��s����Ȃ�
        if (m_gameState != GameState.enPlay)
        {
            return;
        }
        // �|�[�Y���Ă���Ȃ���s���Ȃ�
        if (PauseFlag == true)
        {
            return;
        }
        // ���o���J�n���ꂽ�Ȃ���s���Ȃ�
        if(m_stagingManager.StangingState == StangingState.enStangingStart)
        {
            return;
        }

        DrawStatus();

        switch (m_turnStatus)
        {
            // �v���C���[�̃^�[��
            case TurnStatus.enPlayer:
                switch (m_operatingPlayer)
                {
                    case OperatingPlayer.enAttacker:
                    case OperatingPlayer.enBuffer:
                    case OperatingPlayer.enHealer:
                        PlayerAction((int)m_operatingPlayer);

                        // �ēx�s���\�Ȃ�
                        if (m_battleSystem.OneMore == true)
                        {
                            // �^�[����n��
                            m_playerMoveList[(int)m_operatingPlayer].ResetStatus();
                            BattleButton[(int)m_operatingPlayer].ResetStatus();
                            break;
                        }
                        break;
                }
                break;
            // �G�l�~�[�̃^�[��
            case TurnStatus.enEnemy:
                for(int enemyNumber = m_enemyNumber; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                {
                    // ���S���Ă���ۂ͎��s���Ȃ�
                    if (m_enemyMoveList[enemyNumber].ActorHPState == ActorHPState.enDie)
                    {
                        continue;
                    }
                    EnemyAction(enemyNumber);
                    // �ēx�s���\�Ȃ�
                    if (m_battleSystem.OneMore == true)
                    {
                        // �^�[����n��
                        m_enemyMoveList[enemyNumber].ResetStatus();
                        enemyNumber--;
                        continue;
                    }
                }
                m_enemyNumber++;
                break;
        }
        IsTurnEnd();
    }

    /// <summary>
    /// �^�[�����I�����Ă��邩���肷��
    /// </summary>
    private void IsTurnEnd()
    {
        // �S���̍s�����I�����Ă��Ȃ��Ȃ���s���Ȃ�
        for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
        {
            if (m_playerMoveList[playerNumber].ActionEndFlag == false)
            {
                return;
            }
        }
        for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
        {
            if (m_enemyMoveList[enemyNumber].ActionEndFlag == false)
            {
                return;
            }
        }
        TurnEnd();
    }

    /// <summary>
    /// ��̃X�e�[�^�X�����Z�b�g���āA���̃^�[���Ɉڍs����
    /// </summary>
    private void TurnEnd()
    {
        m_isTurnEnd = true;
        // ���̑���L�����N�^�[������A�J�������Đݒ肷��
        m_operatingPlayer = NextOperatingPlayer();
        m_lockOnSystem.ResetCinemachine();
        m_enemyNumber = 0;
        // �^�[����n��
        m_turnStatus = TurnStatus.enPlayer;
        m_turnSum++;
        // �X�e�[�^�X��������
        ResetPlayerAction();
        ResetEnemyAction();
        ResetBattleButton();
        m_isTurnEnd = false;
    }

    /// <summary>
    /// �Q�[���N���A���擾����
    /// </summary>
    private void IsGameClear()
    {
        for(int i = 0; i < m_enemyMoveList.Count; i++)
        {
            // ���肪1�̂ł��������Ă���Ȃ�Q�[���N���A�ł͂Ȃ�
            if(m_enemyMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                return;
            }
        }
        m_gameState = GameState.enBattleWin;
    }

    /// <summary>
    /// �Q�[���I�[�o�[���擾����
    /// </summary>
    private void IsGameOver()
    {
        for(int i = 0; i < m_playerMoveList.Count; i++)
        {
            // 1�̂ł��������Ă���Ȃ�Q�[���I�[�o�[�ł͂Ȃ�
            if (m_playerMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                return;
            }
        }
        m_gameState = GameState.enBattleLose;
    }

    /// <summary>
    /// �Q�[���N���A���o�̃^�X�N
    /// </summary>
    async UniTask GameClearTask()
    {
        // ���o���I�������Ȃ�ȉ��̏��������s����
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StangingState.enStangingEnd);
        await UniTask.WaitUntil(() => m_gameState == GameState.enBattleWin);
        m_drawBattleResult.GameClearStaging();
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o�̃^�X�N
    /// </summary>
    async UniTask GameOverTask()
    {
        // ���o���I�������Ȃ�ȉ��̏��������s����
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StangingState.enStangingEnd);
        await UniTask.WaitUntil(() => m_gameState == GameState.enBattleLose);
        m_drawBattleResult.GameOverStaging();
    }

    /// <summary>
    /// �v���C���[�̍s��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    async private void PlayerAction(int myNumber)
    {
        Debug.Log(PlayerData.playerDataList[(int)m_operatingPlayer].PlayerName + "�̃^�[��");
        // ���o���J�n���ꂽ�Ȃ���s���Ȃ�
        if (m_stagingManager.StangingState == StangingState.enStangingStart)
        {
            return;
        }
        // ���ɍs�����Ă���Ȃ�s���͂��Ȃ�
        if (m_playerMoveList[(int)m_operatingPlayer].ActionEndFlag == true)
        {
            m_operatingPlayer = NextOperatingPlayer();
            return;
        }

        for (int i = 0; i < BattleButton.Length; i++)
        {
            // ���������ꂩ�̃{�^���������ꂽ��ȉ��̏��������s����
            if (BattleButton[i].ButtonDown == true)
            {
                var skillNumber = m_playerMoveList[myNumber].SelectSkillNumber;
                var targetNumber = 0;

                // �K�[�h�ȊO�̃R�}���h  ���@�P�̍U���Ȃ�
                if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange != EffectRange.enAll
                    && m_playerMoveList[myNumber].NextActionType != ActionType.enGuard)
                {
                    m_lockOnSystem.SetTargetState(PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillNumber,
                        m_playerMoveList[myNumber].NextActionType);
                    // �U���Ώۂ��I�����ꂽ��ȉ��̏��������s����
                    await UniTask.WaitUntil(() => m_lockOnSystem.ButtonDown == true);
                    // �Ώۂ��Đݒ肷��
                    skillNumber = m_playerMoveList[myNumber].SelectSkillNumber;
                    targetNumber = m_lockOnSystem.TargetNumber;
                }
                PlayerAction_Move(myNumber, skillNumber, targetNumber);
            }
        }
    }

    /// <summary>
    /// �v���C���[�̍s��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    async private void PlayerAction_Move(int myNumber, int skillNumber, int targetNumber)
    {
        m_playerMoveList[myNumber].CalculationAbnormalState();
        PlayerAction_Command(myNumber, targetNumber, m_playerMoveList[myNumber].NextActionType, skillNumber);
        // ���o���J�n����
        m_stagingManager.RegistrationTargets(m_playerMoveList[myNumber].NextActionType, m_turnStatus, targetNumber, myNumber,
            PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillNumber, PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange);
        // �s�����I������
        m_playerMoveList[myNumber].ActionEnd(m_playerMoveList[myNumber].NextActionType, skillNumber);
        // ���o���I�������Ȃ�ȉ��̏��������s����
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StangingState.enStangingEnd);
        m_playerMoveList[myNumber].DecrementHP(m_playerMoveList[myNumber].PoisonDamage);
        // ���̃v���C���[��ݒ肷��
        m_operatingPlayer = NextOperatingPlayer();
        // ���b�N�I���̐ݒ���������E�Đݒ肷��
        m_lockOnSystem.ButtonDown = false;
        m_lockOnSystem.ResetCinemachine();
    }

    /// <summary>
    /// �s������
    /// </summary>
    private void PlayerAction_Command(int myNumber, int targetNumber, ActionType actionType, int skillNumber)
    {
        // ���ɍs�����Ă���Ȃ���s���Ȃ�
        if (m_playerMoveList[myNumber].ActionEndFlag == true)
        {
            return;
        }
        // �s��
        switch (actionType)
        {
            case ActionType.enAttack:
                var DEF = m_enemyMoveList[targetNumber].EnemyStatus.DEF;
                // ������ԂȂ�^�[�Q�b�g���Đݒ肷��
                if (m_playerMoveList[myNumber].ConfusionFlag == true)
                {
                    targetNumber = m_battleSystem.GetRandomValue(0, m_playerMoveList.Count);
                    DEF = m_playerMoveList[targetNumber].PlayerStatus.DEF;
                    Debug.Log($"{PlayerData.playerDataList[targetNumber].PlayerName}�ɍU��");
                }
                m_playerMoveList[myNumber].PlayerAction_Attack(targetNumber, DEF);
                break;
            case ActionType.enSkillAttack:
                switch (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillType)
                {
                    case SkillType.enAttack:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_enemySum; enemyNumber++)
                            {
                                m_playerMoveList[myNumber].PlayerAction_SkillAttack(
                                    skillNumber,                                        // �X�L���̔ԍ�
                                    enemyNumber,                                        // �^�[�Q�b�g�̔ԍ�
                                    m_enemyMoveList[enemyNumber].EnemyStatus.DEF,       // �h���
                                    m_enemyMoveList[enemyNumber].MyNumber               // �G�l�~�[�̔ԍ�
                                    );
                            }
                            break;
                        }
                        m_playerMoveList[myNumber].PlayerAction_SkillAttack(
                            skillNumber,                                                // �X�L���̔ԍ�
                            targetNumber,                                               // �^�[�Q�b�g�̔ԍ�
                            m_enemyMoveList[targetNumber].EnemyStatus.DEF,              // �h���
                            m_enemyMoveList[targetNumber].MyNumber                      // �G�l�~�[�̔ԍ�
                            );
                        break;
                    case SkillType.enBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
                            {
                                m_playerMoveList[myNumber].PlayerAction_Buff(
                                    skillNumber,                                        // �X�L���̔ԍ�
                                    m_enemyMoveList[playerNumber].EnemyStatus.ATK,      // �U����
                                    m_enemyMoveList[playerNumber].EnemyStatus.DEF,      // �h���
                                    m_enemyMoveList[playerNumber].EnemyStatus.SPD,      // �f����
                                    true);
                            }
                            break;
                        }
                        m_playerMoveList[myNumber].PlayerAction_Buff(
                            skillNumber,                                                // �X�L���̔ԍ�
                            m_enemyMoveList[targetNumber].EnemyStatus.ATK,              // �U����
                            m_enemyMoveList[targetNumber].EnemyStatus.DEF,              // �h���
                            m_enemyMoveList[targetNumber].EnemyStatus.SPD,              // �f����
                            true);
                        break;
                    case SkillType.enDeBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                            {
                                m_playerMoveList[myNumber].PlayerAction_Buff(
                                    skillNumber,                                        // �X�L���̔ԍ�
                                    m_enemyMoveList[enemyNumber].EnemyStatus.ATK,       // �U����
                                    m_enemyMoveList[enemyNumber].EnemyStatus.DEF,       // �h���
                                    m_enemyMoveList[enemyNumber].EnemyStatus.SPD,       // �f����
                                    false);
                            }
                            break;
                        }
                        m_playerMoveList[myNumber].PlayerAction_Buff(
                            skillNumber,                                                // �X�L���̔ԍ�
                            m_enemyMoveList[targetNumber].EnemyStatus.ATK,              // �U����
                            m_enemyMoveList[targetNumber].EnemyStatus.DEF,              // �h���
                            m_enemyMoveList[targetNumber].EnemyStatus.SPD,              // �f����
                            false);
                        break;
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
                            {
                                m_playerMoveList[myNumber].PlayerAction_HPRecover(playerNumber, skillNumber);
                            }
                            break;
                        }
                        m_playerMoveList[myNumber].PlayerAction_HPRecover(targetNumber, skillNumber);
                        break;
                }
                break;
            case ActionType.enGuard:
                m_playerMoveList[myNumber].PlayerAction_Guard();
                break;
            case ActionType.enNull:
                break;
        }
    }

    /// <summary>
    /// �G�l�~�[�Ƀ_���[�W��^����
    /// </summary>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="damage">�_���[�W��</param>
    public void PlayerAction_DamageEnemy(int targetNumber, int damage)
    {
        m_enemyMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// �G�l�~�[�̏�Ԃ�ύX����
    /// </summary>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="actorAbnormalState">�ύX��̏��</param>
    public void PlayerAction_ChangeStateEnemy(int targetNumber, ActorAbnormalState actorAbnormalState)
    {
        m_enemyMoveList[targetNumber].ActorAbnormalState = actorAbnormalState;
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
    /// ���̑���L�����N�^�[�����肷��
    /// </summary>
    private OperatingPlayer NextOperatingPlayer()
    {
        var operatingPlayer = OperatingPlayer.enAttacker;
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
            if (PlayerData.playerDataList[i].SPD >= SPD)
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
            m_turnStatus = TurnStatus.enEnemy;
        }

        return operatingPlayer;
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
    /// �X�e�[�^�X��\������
    /// </summary>
    private void DrawStatus()
    {
        m_drawStatusValue.SetStatus();
        m_drawStatusValue.SetStatusText();
    }

    /// <summary>
    /// �G�l�~�[�̍s��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    private void EnemyAction(int myNumber)
    {
        Debug.Log(EnemyData.enemyDataList[m_enemyMoveList[myNumber].MyNumber].EnemyName + "�̃^�[��");

        // ���o���J�n���ꂽ�Ȃ���s���Ȃ�
        if (m_stagingManager.StangingState == StangingState.enStangingStart)
        {
            return;
        }
        // �Ђ񎀂Ȃ���s���Ȃ�
        if (m_enemyMoveList[myNumber].ActorHPState == ActorHPState.enDie)
        {
            return;
        }
        var actionType = m_enemyMoveList[myNumber].SelectAttackType();
        var skillNumber = m_enemyMoveList[myNumber].SelectSkill();

        m_lockOnSystem.SetTargetState(0, actionType);
        EnemyAction_Move(myNumber, actionType, skillNumber);
    }

    /// <summary>
    /// �G�l�~�[�̍s��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="actionType">�s���p�^�[��</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    async private void EnemyAction_Move(int myNumber, ActionType actionType, int skillNumber)
    {
        m_enemyMoveList[myNumber].CalculationAbnormalState();
        // �^�[�Q�b�g�̔ԍ����擾����
        var targetNumber = m_enemyMoveList[myNumber].SelectTargetPlayer();
        EnemyAction_Command(myNumber, actionType, skillNumber, targetNumber);
        // ���o���J�n����
        m_stagingManager.RegistrationTargets(actionType, m_turnStatus, targetNumber, myNumber);
        m_enemyMoveList[myNumber].ActionEnd(actionType, skillNumber);
        // ���o���I�������Ȃ�ȉ��̏��������s����
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StangingState.enStangingEnd);
        // �ŏ�Ԏ��̃_���[�W��^����
        m_enemyMoveList[myNumber].DecrementHP(m_enemyMoveList[myNumber].PoisonDamage);
    }

    /// <summary>
    /// �s������
    /// </summary>
    private void EnemyAction_Command(int myNumber, ActionType actionType, int skillNumber, int targetNumber)
    {
        // ���ɍs�����Ă���Ȃ���s���Ȃ�
        if(m_enemyMoveList[myNumber].ActionEndFlag == true)
        {
            return;
        }

        switch (actionType)
        {
            case ActionType.enAttack:
                m_enemyMoveList[myNumber].EnemyAction_Attack(targetNumber,m_playerMoveList[targetNumber].PlayerStatus.DEF);
                break;
            case ActionType.enSkillAttack:
                switch (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].SkillType)
                {
                    // �^�C�v�F�U��
                    case SkillType.enAttack:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
                            {
                                m_enemyMoveList[myNumber].EnemyAction_SkillAttack(
                                    skillNumber,                                                            // �X�L���̔ԍ�
                                    playerNumber,                                                           // �^�[�Q�b�g�̔ԍ�
                                    m_playerMoveList[playerNumber].PlayerStatus.DEF,                        // �h���
                                    m_playerMoveList[playerNumber].MyNumber                                 // �v���C���[�̔ԍ�
                                    );
                            }
                            break;
                        }
                        m_enemyMoveList[myNumber].EnemyAction_SkillAttack(
                            skillNumber,                                                                    // �X�L���̔ԍ�
                            targetNumber,                                                           // �^�[�Q�b�g�̔ԍ�
                            m_playerMoveList[targetNumber].PlayerStatus.DEF,                                // �h���
                            m_playerMoveList[targetNumber].MyNumber                                         // �v���C���[�̔ԍ�
                            );
                        break;
                    // �^�C�v�F�o�t
                    case SkillType.enBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                            {
                                m_enemyMoveList[myNumber].EnemyAction_Buff(
                                    skillNumber,                                                            // �X�L���̔ԍ�
                                    m_enemyMoveList[enemyNumber].EnemyStatus.ATK,                           // �U����
                                    m_enemyMoveList[enemyNumber].EnemyStatus.DEF,                           // �h���
                                    m_enemyMoveList[enemyNumber].EnemyStatus.SPD,                           // �f����
                                    true);
                            }
                            break;
                        }
                        // �^�[�Q�b�g���đI��
                        targetNumber = m_enemyMoveList[myNumber].SelectTargetEnemy(m_enemyMoveList.Count);
                        m_enemyMoveList[myNumber].EnemyAction_Buff(
                            skillNumber,                                                                    // �X�L���̔ԍ�
                            m_enemyMoveList[targetNumber].EnemyStatus.ATK,                                  // �U����
                            m_enemyMoveList[targetNumber].EnemyStatus.DEF,                                  // �h���
                            m_enemyMoveList[targetNumber].EnemyStatus.SPD,                                  // �f����
                            true);
                        break;
                    // �^�C�v�F�f�o�t
                    case SkillType.enDeBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_enemyMoveList.Count; playerNumber++)
                            {
                                m_enemyMoveList[myNumber].EnemyAction_Buff(
                                    skillNumber,                                                            // �X�L���̔ԍ�
                                    m_playerMoveList[playerNumber].PlayerStatus.ATK,                        // �U����
                                    m_playerMoveList[playerNumber].PlayerStatus.DEF,                        // �h���
                                    m_playerMoveList[playerNumber].PlayerStatus.SPD,                        // �f����
                                    false);
                            }
                            break;
                        }
                        m_enemyMoveList[myNumber].EnemyAction_Buff(
                            skillNumber,                                                                    // �X�L���̔ԍ�
                            m_playerMoveList[targetNumber].PlayerStatus.ATK,                                // �U����
                            m_playerMoveList[targetNumber].PlayerStatus.DEF,                                // �h���
                            m_playerMoveList[targetNumber].PlayerStatus.SPD,                                // �f����
                            false);
                        break;
                    // �^�C�v�F��
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                        // �^�[�Q�b�g��I��
                        targetNumber = m_enemyMoveList[myNumber].SelectTargetEnemy(m_enemyMoveList.Count);
                        m_enemyMoveList[myNumber].EnemyAction_HPResurrection(skillNumber, targetNumber);
                        m_enemyMoveList[myNumber].EnemyAction_HPRecover(m_enemyMoveList[targetNumber].EnemyStatus.HP, skillNumber);
                        break;
                }
                break;
            case ActionType.enGuard:
                m_enemyMoveList[myNumber].EnemyAction_Guard();
                break;
            case ActionType.enEscape:
                m_enemyMoveList[myNumber].EnemyAction_Escape();
                break;
            case ActionType.enNull:
                break;
        }
    }

    /// <summary>
    /// �v���C���[�Ƀ_���[�W��^����
    /// </summary>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="damage">�_���[�W��</param>
    public void EnemyAction_DamagePlayer(int targetNumber, int damage)
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
        Destroy(m_enemyMoveList[myNumber].gameObject);
        m_enemyMoveList.Remove(m_enemyMoveList[myNumber]);
    }

    /// <summary>
    /// �S�̂��񕜂�����
    /// </summary>
    /// <param name="recoverValue">�񕜗�</param>
    public void EnemyAction_AllRecover(int recoverValue)
    {
        for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
        {
            m_enemyMoveList[enemyNumber].RecoverHP(recoverValue);
        }
    }
}
