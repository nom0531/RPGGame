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
/// ���삵�Ă���L�����N�^�[
/// </summary>
public enum OperatingPlayer
{
    enAttacker, // �A�^�b�J�[
    enBuffer,   // �o�b�t�@�[
    enHealer,   // �q�[���[
    enNum
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

    private const int MAX_ENEMY_NUM = 4;                        // �o�g���ɏo������G�l�~�[�̍ő吔
    private const float ADD_SIZE = 1.4f;                        // �G�l�~�[�̉摜�̏�Z�T�C�Y

    private BattleSystem m_battleSystem;                        // �o�g���V�X�e��
    private GameState m_gameState = GameState.enPlay;           // �Q�[���̏��
    private List<PlayerMove> m_playerMoveList;                  // �v���C���[�̍s��
    private List<EnemyMove> m_enemyMoveList;                    // �G�l�~�[�̍s��
    private OperatingPlayer m_operatingPlayer;                  // ���삵�Ă���v���C���[
    private LockOnSystem m_lockOnSystem;                        // ���b�N�I���V�X�e��
    private StateAbnormalCalculation m_abnormalCalculation;     // ��Ԉُ�̌v�Z
    private DrawStatusValue m_drawStatusValue;                  // �X�e�[�^�X��\������UI
    private DrawBattleResult m_drawBattleResult;                // ���o
    private int m_turnSum = 0;                                  // �����^�[����
    private int m_selectQuestNumber = 0;                        // �I�������N�G�X�g�̔ԍ�
    private int m_enemySum = 0;                                 // �G�l�~�[�̑���
    private bool m_isPause = false;                             // �|�[�Y��ʂ��ǂ���
    private bool m_isPushDown = false;                          // �{�^���������ꂽ���ǂ���

    public bool PauseFlag
    {
        get => m_isPause;
        set => m_isPushDown = value;
    }

    public int OperatingPlayerNumber
    {
        get => (int)m_operatingPlayer;
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
        m_abnormalCalculation = GetComponent<StateAbnormalCalculation>();
        m_drawStatusValue = gameObject.GetComponent<DrawStatusValue>();
        m_drawBattleResult = gameObject.GetComponent<DrawBattleResult>();

        int SPD = int.MinValue;

        for(int i = 0; i < (int)OperatingPlayer.enNum; i++)
        {
            // SPD�̃p�����[�^����Ȃ�X�V����
            if (PlayerData.playerDataList[i].SPD >= SPD)
            {
                SPD = PlayerData.playerDataList[i].SPD;
                m_operatingPlayer = (OperatingPlayer)i;
            }
        }
    }

    private void Start()
    {
        PlayerAction_DrawStatus();

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
            // �T�C�Y���擾����
            var spriteRenderer = sprite.GetComponent<SpriteRenderer>();
            float width = spriteRenderer.bounds.size.x * ADD_SIZE;
            float height = spriteRenderer.bounds.size.y * ADD_SIZE;
            // �T�C�Y�A���W�𒲐�
            sprite.transform.localScale = new Vector3(width, height, 1.0f);
            sprite.transform.localPosition = Vector3.zero;
            sprite.transform.localRotation = Quaternion.identity;
            // �f�[�^�����o�����߂ɔԍ����擾���Ă���
            // �G�l�~�[�Ɏ��g�̔ԍ���������
            sprite.GetComponent<EnemyMove>().MyNumber = EnemyData.enemyDataList[rand].EnemyNumber;

            // �}�ӂɖ��o�^�Ȃ�
            if (sprite.GetComponent<EnemyMove>().GetTrueEnemyRegister(
                EnemyData.enemyDataList[rand].EnemyNumber) == false)
            {
                // �o�^����
                sprite.GetComponent<EnemyMove>().SetTrueEnemyRegister(
                    EnemyData.enemyDataList[rand].EnemyNumber);
            }
        }

        // �z���p��
        // playerMove��l�����p��
        var playerMoveList = FindObjectsOfType<PlayerMove>();
        m_playerMoveList = new List<PlayerMove>(playerMoveList);
        m_playerMoveList.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));    // �ԍ����Ƀ\�[�g

        // enemyMove��l�����p��
        var enemyMoveList = FindObjectsOfType<EnemyMove>();
        m_enemyMoveList = new List<EnemyMove>(enemyMoveList);

        // �ŏ��̑���L�����N�^�[�����肷��
        m_operatingPlayer = NextOperatingPlayer();

        // �^�X�N��ݒ肷��
        GameClearTask().Forget();
        GameOverTask().Forget();
    }

    // Update is called once per frame
    void Update()
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

        PlayerAction_DrawStatus();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_playerMoveList[0].ActionEndFlag = true;
            m_playerMoveList[1].ActionEndFlag = true;
            m_playerMoveList[2].ActionEndFlag = true;
        }
    }

    private void FixedUpdate()
    {
        IsGameClear();
        IsGameOver();

        // �Q�[�����I�����Ă���Ȃ�A������ȉ��̏����͎��s����Ȃ�
        if (m_gameState != GameState.enPlay)
        {
            return;
        }

        if (PauseFlag == true)
        {
            return;
        }

        // �s������
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
                            m_playerMoveList[(int)m_operatingPlayer].ResetPlayerStatus();
                            BattleButton[(int)m_operatingPlayer].ResetStatus();
                            break;
                        }
                        break;
                }
                break;
            // �G�l�~�[�̃^�[��
            case TurnStatus.enEnemy:
                for(int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                {
                    // ���S���Ă���ۂ͎��s���Ȃ�
                    if (m_enemyMoveList[enemyNumber].ActorHPState == ActorHPState.enDie)
                    {
                        m_enemySum--;
                        m_enemyMoveList.Remove(m_enemyMoveList[enemyNumber]);
                        continue;
                    }

                    EnemyAction(enemyNumber);

                    // �ēx�s���\�Ȃ�
                    if (m_battleSystem.OneMore == true)
                    {
                        enemyNumber--;
                        continue;
                    }
                }
                // �^�[����n��
                m_turnStatus = TurnStatus.enPlayer;
                m_turnSum++;    // �^�[���������Z
                // �X�e�[�^�X��������
                PlayerAction_ResetPlayerAction();
                PlayerAction_ResetBattleButton();
                // ���̑���L�����N�^�[������A�J�������Đݒ肷��
                m_operatingPlayer = NextOperatingPlayer();
                m_lockOnSystem.ResetCinemachine();
                break;
        }
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
        // ���肪�S�ł���
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
        // �S�ł���
        m_gameState = GameState.enBattleLose;
    }

    /// <summary>
    /// �Q�[���N���A���o�̃^�X�N
    /// </summary>
    async UniTask GameClearTask()
    {
        await UniTask.WaitUntil(() => m_gameState == GameState.enBattleWin);
        m_drawBattleResult.GameClearStaging();
    }

    /// <summary>
    /// �Q�[���I�[�o�[���o�̃^�X�N
    /// </summary>
    async UniTask GameOverTask()
    {
        await UniTask.WaitUntil(() => m_gameState == GameState.enBattleLose);
        m_drawBattleResult.GameOverStaging();
    }

    /// <summary>
    /// �v���C���[�̍s��
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    async private void PlayerAction(int number)
    {
        Debug.Log(PlayerData.playerDataList[(int)m_operatingPlayer].PlayerName + "�̃^�[��");

        // ���ɍs�����Ă���Ȃ�s���͂��Ȃ�
        if(m_playerMoveList[(int)m_operatingPlayer].ActionEndFlag == true)
        {
            m_operatingPlayer = NextOperatingPlayer();
            return;
        }

        for (int i = 0; i < BattleButton.Length; i++)
        {
            // ���������ꂩ�������ꂽ��
            if (BattleButton[i].ButtonDown == true)
            {
                ActionType actionType = m_playerMoveList[number].NextActionType;
                int skillNumber = m_playerMoveList[number].SelectSkillNumber;
                int targetNumber = 0;

                // �K�[�h�ȊO�̃R�}���h  ���@�P�̍U���Ȃ�
                if (actionType != ActionType.enGuard
                    && PlayerData.playerDataList[number].skillDataList[skillNumber].EffectRange != EffectRange.enAll)
                {
                    // ���b�N�I���̑Ώۂ����肷��
                    m_lockOnSystem.SetTargetState(PlayerData.playerDataList[number].skillDataList[skillNumber].SkillNumber, actionType);
                    m_lockOnSystem.LockOn = true;
                    // �U���Ώۂ��I�����ꂽ��ȉ��̏��������s����
                    await UniTask.WaitUntil(() => m_lockOnSystem.ButtonDown == true);

                    targetNumber = m_lockOnSystem.TargetNumber;
                    PlayerAction_Move(number, targetNumber, actionType, skillNumber);
                    Debug.Log("������HP : " + m_playerMoveList[number].PlayerStatus.HP);
                    Debug.Log("�����HP : " + m_enemyMoveList[targetNumber].EnemyStatus.HP);
                    break;
                }
                // ���b�N�I�������Ƀ_���[�W�v�Z���s��
                m_lockOnSystem.LockOn = false;

                PlayerAction_Move(number, targetNumber, actionType, skillNumber);
                Debug.Log("������HP : " + m_playerMoveList[number].PlayerStatus.HP);
                Debug.Log("�����HP : " + m_enemyMoveList[targetNumber].EnemyStatus.HP);
                break;
            }
        }
    }

    /// <summary>
    /// �v���C���[�̍s��
    /// </summary>
    private void PlayerAction_Move(int myNumber, int targetNumber, ActionType actionType, int skillNumber)
    {
        // ��჏�ԂȂ�
        if (m_abnormalCalculation.Paralysis(m_playerMoveList[myNumber].ActorAbnormalState) == true)
        {
            Debug.Log($"{PlayerData.playerDataList[myNumber].PlayerName}�͖�Ⴢ��Ă���");
            actionType = ActionType.enNull;
        }

        // ������ԂȂ�
        if (m_abnormalCalculation.Confusion(m_playerMoveList[myNumber].ActorAbnormalState) == true)
        {
            Debug.Log($"{PlayerData.playerDataList[myNumber].PlayerName}�͍������Ă���");
            actionType = ActionType.enAttack;
            m_playerMoveList[myNumber].Confusion = true;
        }

            switch (actionType)
        {
            case ActionType.enAttack:
                int DEF = m_enemyMoveList[targetNumber].EnemyStatus.DEF;
                // ������ԂȂ�^�[�Q�b�g���Đݒ肷��
                if (m_playerMoveList[myNumber].Confusion == true)
                {
                    targetNumber = m_battleSystem.GetRandomValue(0, m_playerMoveList.Count);
                    DEF = m_playerMoveList[targetNumber].PlayerStatus.DEF;
                    Debug.Log($"{PlayerData.playerDataList[targetNumber].PlayerName}�ɍU��");
                }
                PlayerAction_Attack(myNumber, targetNumber, DEF);
                break;
            case ActionType.enSkillAttack:
                switch (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillType)
                {
                    case SkillType.enAttack:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_playerMoveList.Count; enemyNumber++)
                            {
                                PlayerAction_SkillAttack(myNumber, enemyNumber, skillNumber);
                            }
                            break;
                        }
                        PlayerAction_SkillAttack(myNumber, targetNumber, skillNumber);
                        break;
                    case SkillType.enBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
                            {
                                PlayerAction_Buff(myNumber, playerNumber, skillNumber, true);
                            }
                            break;
                        }
                        PlayerAction_Buff(myNumber, targetNumber, skillNumber, true);
                        break;
                    case SkillType.enDeBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                            {
                                PlayerAction_Buff(myNumber, enemyNumber, skillNumber, false);
                            }
                            break;
                        }
                        PlayerAction_Buff(myNumber, targetNumber, skillNumber, false);
                        break;
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
                            {
                                PlayerAction_HPRecover(myNumber, playerNumber, skillNumber);
                            }
                            break;
                        }
                        PlayerAction_HPRecover(myNumber, targetNumber, skillNumber);
                        break;
                }
                break;
            case ActionType.enGuard:
                PlayerAction_Guard(myNumber);
                break;
            case ActionType.enNull:
                break;
        }

        // �ŏ�ԂȂ�HP�����炷
        int damage = m_abnormalCalculation.Poison(
            m_playerMoveList[(int)m_operatingPlayer].ActorAbnormalState, m_playerMoveList[myNumber].PlayerStatus.HP);
        m_playerMoveList[myNumber].DecrementHP(damage);

        m_playerMoveList[myNumber].gameObject.GetComponent<DrawCommandText>().SetCommandText(
            actionType, PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillNumber);
        // �s�����I�����A���̃v���C���[��I������
        m_playerMoveList[(int)m_operatingPlayer].ActionEndFlag = true;
        m_operatingPlayer = NextOperatingPlayer();
        // ���b�N�I���̐ݒ���������E�Đݒ肷��
        m_lockOnSystem.ButtonDown = false;
        m_lockOnSystem.ResetCinemachine();
    }

    /// <summary>
    /// �ʏ�U���̏���
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    private void PlayerAction_Attack(int myNumber, int targetNumber, int DEF)
    {
        // �_���[�W�ʂ��v�Z
        int damage = m_battleSystem.NormalAttack(
            m_playerMoveList[myNumber].PlayerStatus.ATK,// �U����
            DEF                                         // �h���
            );
        // ������ԂȂ�
        if (m_playerMoveList[myNumber].Confusion)
        {
            m_playerMoveList[targetNumber].DecrementHP(damage);
            return;
        }
        // �_���[�W��ݒ肷��
        m_enemyMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// �U���^�C�v�̃X�L������
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    private void PlayerAction_SkillAttack(int myNumber,int targetNumber,int skillNumber)
    {
        // �_���[�W�ʂ��v�Z
        int damage = m_battleSystem.SkillAttack(
            m_playerMoveList[myNumber].PlayerStatus.ATK,             // �U����
            PlayerData.playerDataList[myNumber].skillDataList[skillNumber].POW,     // �X�L���̊�b�l
            m_enemyMoveList[targetNumber].EnemyStatus.DEF            // �h���
            );

        // �������łȂ��Ȃ瑮�����l�������v�Z���s��
        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNone
            && PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNum)
        {
            damage = m_battleSystem.EnemyElementResistance(
                EnemyData.enemyDataList[targetNumber],                                          // �G�l�~�[�f�[�^
                skillNumber,                                                                // �X�L���̔ԍ�
                (int)PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement,   // �X�L���̑���
                damage                                                                      // �_���[�W
                );

            // ���ɑ����ϐ��𔭌����Ă��Ȃ��Ȃ�
            if (m_enemyMoveList[targetNumber].GetTrueElementRegister
                ((int)PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement) == false)
            {
                // ���������t���O��ture�ɂ���
                m_enemyMoveList[targetNumber].SetTrueElementRegister
                    ((int)PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement);
            }
        }

        // �_���[�W��ݒ肷��
        m_enemyMoveList[targetNumber].DecrementHP(damage);
        PlayerAction_Decrement(myNumber, skillNumber);
    }

    /// <summary>
    /// �o�t�E�f�o�t�̏���
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="isBuff">true�Ȃ�o�t�Bfalse�Ȃ�f�o�t</param>
    private void PlayerAction_Buff(int myNumber, int targetNumber, int skillNumber, bool isBuff)
    {
        // �p�����[�^���Q��
        int param = 0;
        switch (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].BuffType)
        {
            case BuffType.enATK:
                param = m_playerMoveList[targetNumber].PlayerStatus.ATK;
                break;
            case BuffType.enDEF:
                param = m_playerMoveList[targetNumber].PlayerStatus.DEF;
                break;
            case BuffType.enSPD:
                param = m_playerMoveList[targetNumber].PlayerStatus.SPD;
                break;
        }
        // �l�̌v�Z
        int value = m_battleSystem.SkillBuff(
            param,
            PlayerData.playerDataList[myNumber].skillDataList[skillNumber].POW
            );

        // �l��ݒ肷��
        m_playerMoveList[targetNumber].SetPlayerBuffStatus(
            PlayerData.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
            value,
            skillNumber,
            isBuff
            );

        PlayerAction_Decrement(myNumber, skillNumber);
    }

    /// <summary>
    /// HP���񕜂��鏈��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    private void PlayerAction_HPRecover(int myNumber, int targetNumber, int skillNumber)
    {
        // �񕜗ʂ��v�Z����
        int recverValue = m_battleSystem.SkillHeal(
                PlayerData.playerDataList[targetNumber].HP,
                PlayerData.playerDataList[myNumber].skillDataList[skillNumber].POW
                );

        // HP���񕜂�����
        m_playerMoveList[targetNumber].RecoverHP(recverValue);

        PlayerAction_Decrement(myNumber, skillNumber);
    }

    /// <summary>
    /// SP�EHP������鏈��
    /// </summary>
    private void PlayerAction_Decrement(int myNumer, int skillNumber)
    {
        int necessaryValue = PlayerData.playerDataList[myNumer].skillDataList[skillNumber].SkillNecessary;

        // SP�EHP�������
        switch (PlayerData.playerDataList[myNumer].skillDataList[skillNumber].Type)
        {
            case NecessaryType.enSP:
                m_playerMoveList[myNumer].DecrementSP(necessaryValue);
                break;
            case NecessaryType.enHP:
                m_playerMoveList[myNumer].DecrementHP(necessaryValue);
                break;
        }
    }

    /// <summary>
    /// 1�^�[���h�䂷�鏈��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    private void PlayerAction_Guard(int myNumber)
    {
        // �h��͂��v�Z
        float defensePower = m_playerMoveList[myNumber].Guard();
    }

    /// <summary>
    /// ���̑���L�����N�^�[�����肷��
    /// </summary>
    private OperatingPlayer NextOperatingPlayer()
    {
        OperatingPlayer operatingPlayer = OperatingPlayer.enAttacker;
        int SPD = int.MinValue;

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

            // SPD�̃p�����[�^����Ȃ�X�V����
            if (PlayerData.playerDataList[i].SPD >= SPD)
            {
                SPD = PlayerData.playerDataList[i].SPD;
                operatingPlayer = (OperatingPlayer)i;
            }
        }

        if (m_playerMoveList[0].ActionEndFlag == true
        && m_playerMoveList[1].ActionEndFlag == true
        && m_playerMoveList[2].ActionEndFlag == true)
        {
            // �s�����I�����Ă���Ȃ�^�[����n��
            m_turnStatus = TurnStatus.enEnemy;
            m_turnSum++;    // �^�[���������Z
        }

        return operatingPlayer;
    }

    /// <summary>
    /// �v���C���[�̃X�e�[�^�X�����Z�b�g����
    /// </summary>
    private void PlayerAction_ResetPlayerAction()
    {

        for (int i = 0; i < m_playerMoveList.Count; i++)
        {
            m_playerMoveList[i].ResetPlayerStatus();
        }
    }

    /// <summary>
    /// �R�}���h�̏�Ԃ����Z�b�g����
    /// </summary>
    public void PlayerAction_ResetBattleButton()
    {
        for (int i = 0; i < BattleButton.Length; i++)
        {
            BattleButton[i].ResetStatus();
        }
    }

    /// <summary>
    /// �X�e�[�^�X��\������
    /// </summary>
    private void PlayerAction_DrawStatus()
    {
        m_drawStatusValue.SetStatus();
        m_drawStatusValue.SetStatusText();
    }

    /// <summary>
    /// �G�l�~�[�̍s��
    /// </summary>
    private void EnemyAction(int number)
    {
        Debug.Log(EnemyData.enemyDataList[m_enemyMoveList[number].MyNumber].EnemyName + "�̃^�[��");
        ActionType actionType = m_enemyMoveList[number].SelectAttackType();
        int skillNumber = m_enemyMoveList[number].SelectSkill();

        if(m_enemyMoveList[number].ActorHPState == ActorHPState.enDie)
        {
            return;
        }

        EnemyAction_Move(number, actionType, skillNumber);
        m_enemyMoveList[number].gameObject.GetComponent<DrawCommandText>().SetCommandText(
            actionType, EnemyData.enemyDataList[number].skillDataList[skillNumber].SkillNumber);
    }

    /// <summary>
    /// �G�l�~�[�̍s��
    /// </summary>
    private void EnemyAction_Move(int myNumber, ActionType actionType, int skillNumber)
    {
        switch (actionType)
        {
            case ActionType.enAttack:
                EnemyAction_Attack(myNumber);
                break;
            case ActionType.enSkillAttack:

                switch (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].SkillType)
                {
                    case SkillType.enAttack:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
                            {
                                EnemyAction_SkillAttack(myNumber, playerNumber, skillNumber);
                            }
                            break;
                        }
                        // �^�[�Q�b�g��I��
                        int skillTarget = m_enemyMoveList[myNumber].SelectTargetEnemy(m_enemyMoveList.Count);
                        EnemyAction_SkillAttack(myNumber, skillTarget, skillNumber);
                        break;
                    case SkillType.enBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                            {
                                EnemyAction_Buff(myNumber, skillNumber, enemyNumber, true);
                            }
                            break;
                        }
                        // �^�[�Q�b�g��I��
                        int buffTarget = m_enemyMoveList[myNumber].SelectTargetEnemy(m_enemyMoveList.Count);
                        EnemyAction_Buff(myNumber, skillNumber, buffTarget, true);
                        break;
                    case SkillType.enDeBuff:
                        // ���ʔ͈͂��S�̂̂Ƃ�
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_enemyMoveList.Count; playerNumber++)
                            {
                                EnemyAction_Buff(myNumber, skillNumber, playerNumber, false);
                            }
                            break;
                        }
                        // �^�[�Q�b�g��I��
                        int debuffTarget = m_enemyMoveList[myNumber].SelectTargetPlayer();
                        EnemyAction_Buff(myNumber, skillNumber, debuffTarget, false);
                        break;
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                        EnemyAction_HPRecover(myNumber, skillNumber);
                        break;
                }
                break;
            case ActionType.enGuard:
                EnemyAction_Guard(myNumber);
                break;
            case ActionType.enEscape:
                EnemyAction_Escape(myNumber);
                break;
            case ActionType.enNull:
                break;
        }
    }

    /// <summary>
    /// �ʏ�U���̏���
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    private void EnemyAction_Attack(int myNumber)
    {
        // �^�[�Q�b�g��I��
        int targetNumber = m_enemyMoveList[myNumber].SelectTargetPlayer();
        // �_���[�W�ʂ��v�Z
        int damage = m_battleSystem.NormalAttack(
            m_enemyMoveList[targetNumber].EnemyStatus.ATK,       // �U����
            m_playerMoveList[targetNumber].PlayerStatus.DEF      // �h���
            );
        // �v�Z�����_���[�W�ʂ�ݒ肷��
        m_playerMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// �X�L���ł̍U������
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    private void EnemyAction_SkillAttack(int myNumber, int targetNumber, int skillNumber)
    {
        // �_���[�W�ʂ��v�Z
        int damage = m_battleSystem.SkillAttack(
            m_enemyMoveList[targetNumber].EnemyStatus.ATK,              // �p�����[�^
            EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].POW,   // �X�L���̊�b�l
            m_playerMoveList[targetNumber].PlayerStatus.DEF             // �h���
            );

        // �������łȂ��Ȃ瑮�����l�������v�Z���s��
        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNone)
        {
            damage = m_battleSystem.PlayerElementResistance(
                PlayerData.playerDataList[targetNumber],                                    // �v���C���[�f�[�^
                skillNumber,                                                            // �X�L���̔ԍ�
                (int)EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].SkillElement, // �X�L���̑���
                damage                                                                  // �_���[�W
                );
        }

        // �_���[�W��ݒ肷��
        m_playerMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// �o�t�E�f�o�t�̏���
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="isBuff">true�Ȃ�o�t�Bfalse�Ȃ�f�o�t</param>
    private void EnemyAction_Buff(int myNumber, int skillNumber, int targetNumber, bool isBuff)
    {
        // �p�����[�^���Q��
        int param = 0;
        switch (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].BuffType)
        {
            case BuffType.enATK:
                param = m_playerMoveList[targetNumber].PlayerStatus.ATK;
                break;
            case BuffType.enDEF:
                param = m_playerMoveList[targetNumber].PlayerStatus.DEF;
                break;
            case BuffType.enSPD:
                param = m_playerMoveList[targetNumber].PlayerStatus.SPD;
                break;
        }

        // �l���v�Z
        int value = m_battleSystem.SkillBuff(
            param,
            EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].POW
            );
        // �l��ݒ肷��
        m_enemyMoveList[targetNumber].SetEnmeyBuffStatus(
            EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
            value,
            skillNumber,
            isBuff
            );
    }

    /// <summary>
    /// HP���񕜂���
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    private void EnemyAction_HPRecover(int myNumber, int skillNumber)
    {
        // �^�[�Q�b�g��I��
        int targetNumber = m_enemyMoveList[myNumber].SelectTargetEnemy(m_enemyMoveList.Count);

        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].SkillType == SkillType.enResurrection)
        {
            // �I�u�W�F�N�g���擾����
            GameObject gameObjct = m_enemyMoveList[myNumber].SelectTargetDieEnemy();

            // �I�u�W�F�N�g�����݂��Ȃ��Ȃ牽�����Ȃ�
            if(gameObject == null)
            {
                return;
            }

            m_enemyMoveList.Add(gameObjct.GetComponent<EnemyMove>());

            gameObject.SetActive(true);
            targetNumber = (int)m_enemyMoveList.Count - 1;

            gameObject.tag = "Enemy";
        }

        // �񕜗ʂ��v�Z
        int recoverValue = m_battleSystem.SkillHeal(
            EnemyData.enemyDataList[targetNumber].HP,
            EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].POW
            );

        // ���ʔ͈͂��S�̂̂Ƃ�
        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
        {
            for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
            {
                // HP���񕜂�����
                m_enemyMoveList[enemyNumber].RecoverHP(recoverValue);
            }
        }
        // ���ʔ͈͂��P�̂̂Ƃ�
        else
        {
            // HP���񕜂�����
            m_enemyMoveList[targetNumber].RecoverHP(recoverValue);
        }
    }

    /// <summary>
    /// 1�^�[���h�䂷��
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    private void EnemyAction_Guard(int myNumber)
    {
        int defensePower = m_enemyMoveList[myNumber].Guard();
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="myNumber">���g�̔ԍ�</param>
    private void EnemyAction_Escape(int myNumber)
    {
        // �����������������ǂ����擾����
        bool isEscape = m_battleSystem.Escape(EnemyData.enemyDataList[myNumber].LUCK);

        if (isEscape == false)
        {
            return;
        }

        // ���������Ȃ玩�g���폜
        Destroy(m_enemyMoveList[myNumber].gameObject);
        m_enemyMoveList.Remove(m_enemyMoveList[myNumber]);
    }
}
