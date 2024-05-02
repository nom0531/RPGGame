using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// �퓬���̃v���C���[�̃X�e�[�^�X
/// </summary>
public struct PlayerBattleStatus
{
    public int HP;                                  // ���݂�HP
    public int SP;                                  // ���݂�SP
    public int ATK;                                 // ���݂�ATK
    public int DEF;                                 // ���݂�DEF
    public int SPD;                                 // ���݂�SPD
    public int LUCK;                                // ���݂�LUCK
}

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private EnemyDataBase EnemyData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField,Header("���g�̔ԍ�")]
    private int m_myNumber = 0;

    private const int HPMIN_VALUE = 0;              // HP�̍ŏ��l
    private const int SPMIN_VALUE = 0;              // SP�̍ŏ��l

    private PlayerAnimation m_playerAnimation;
    private BattleManager m_battleManager;
    private BattleSystem m_battleSystem;
    private PauseManager m_pauseManager;
    private StagingManager m_stagingManager;
    private StateAbnormalCalculation m_abnormalCalculation;
    private BuffCalculation m_buffCalculation;
    private DrawCommandText m_drawCommandText;
    private SaveDataManager m_saveDataManager;                                      // �Z�[�u�f�[�^
    private PlayerBattleStatus m_playerBattleStatus;                                // �v���C���[�̃X�e�[�^�X
    private ActorHPState m_actorHPState = ActorHPState.enMaxHP;                     // HP�̏��
    private ActorAbnormalState m_actorAbnormalState = ActorAbnormalState.enNormal;  // ��Ԉُ�
    private ActionType m_actionType = ActionType.enNull;                            // ���̍s��
    private int m_selectSkillNumber = 0;                                            // �I�����Ă���X�L���̔ԍ�
    private int m_basicValue = 0;                                                   // �_���[�W�ʁE�񕜗�
    private int m_defencePower = 0;                                                 // �h���
    private int m_poisonDamage = 0;                                                 // �ŏ�Ԏ��̃_���[�W
    private bool m_isActionEnd = false;                                             // �s�����I�����Ă��邩�ǂ���
    private bool m_isConfusion = false;                                             // �������Ă��邩�ǂ���

    public int MyNumber
    {
        get => m_myNumber;
    }

    public int SelectSkillNumber
    {
        get => m_selectSkillNumber;
        set => m_selectSkillNumber = value;
    }

    public bool ActionEndFlag
    {
        get => m_isActionEnd;
        set => m_isActionEnd = value;
    }

    public ActionType NextActionType
    {
        get => m_actionType;
        set => m_actionType = value;
    }

    public PlayerBattleStatus PlayerStatus
    {
        get => m_playerBattleStatus;
    }

    public ActorHPState ActorHPState
    {
        get => m_actorHPState;
        set => m_actorHPState = value;
    }

    public ActorAbnormalState ActorAbnormalState
    {
        get => m_actorAbnormalState;
        set => m_actorAbnormalState = value;
    }

    public PlayerAnimation PlayerAnimation
    {
        get => m_playerAnimation;
    }

    public bool ConfusionFlag
    {
        get => m_isConfusion;
        set => m_isConfusion = value;
    }

    public int PoisonDamage
    {
        get => m_poisonDamage;
        set => m_poisonDamage = value;
    }

    public int BasicValue
    {
        get => m_basicValue;
        set => m_basicValue = value;
    }

    private void Awake()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        m_stagingManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<StagingManager>();
        m_pauseManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<PauseManager>();
        m_abnormalCalculation = GetComponent<StateAbnormalCalculation>();
        m_buffCalculation = GetComponent<BuffCalculation>();
        m_drawCommandText = GetComponent<DrawCommandText>();
        m_playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Start()
    {
        m_saveDataManager = GameManager.Instance.SaveDataManager;
        SetStatus();
    }

    /// <summary>
    /// �X�e�[�^�X������������
    /// </summary>
    private void SetStatus()
    {
        m_playerBattleStatus.HP = m_saveDataManager.SaveData.saveData.PlayerList[MyNumber].HP;
        m_playerBattleStatus.SP = m_saveDataManager.SaveData.saveData.PlayerList[MyNumber].SP;
        m_playerBattleStatus.ATK = m_saveDataManager.SaveData.saveData.PlayerList[MyNumber].ATK;
        m_playerBattleStatus.DEF = m_saveDataManager.SaveData.saveData.PlayerList[MyNumber].DEF;
        m_playerBattleStatus.SPD = m_saveDataManager.SaveData.saveData.PlayerList[MyNumber].SPD;
        m_playerBattleStatus.LUCK = m_saveDataManager.SaveData.saveData.PlayerList[MyNumber].LUCK;
    }

    private void FixedUpdate()
    {
        RotationSprite();
        if (m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }
        if (m_pauseManager.PauseFlag == true)
        {
            return;
        }
        for (int i = 0; i < (int)BuffStatus.enNum; i++)
        {
            if (m_buffCalculation.GetEffectEndFlag((BuffStatus)i) == false)
            {
                continue;
            }
            // ���ʎ��Ԃ��I�����Ă���Ȃ�X�e�[�^�X��߂�
            ResetBuffStatus((BuffStatus)i);
            m_buffCalculation.SetEffectEndFlag((BuffStatus)i, false);
        }
    }

    /// <summary>
    /// �ʏ�U���̏���
    /// </summary>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="attackedDEF">�h�䑤�̖h���</param>
    public void PlayerAction_Attack(int targetNumber, int attackedDEF)
    {
        // �_���[�W�ʂ��v�Z
        BasicValue = m_battleSystem.NormalAttack(
            PlayerStatus.ATK,   // �U����
            attackedDEF         // �h���
            );
        // ������ԂȂ�
        if (ConfusionFlag == true)
        {
            m_battleManager.DamagePlayer(targetNumber, BasicValue);
            return;
        }
        m_battleManager.DamageEnemy(targetNumber, BasicValue);
        m_playerAnimation.PlayAnimation(AnimationState.enAttack);
    }

    /// <summary>
    /// �U���^�C�v�̃X�L������
    /// </summary>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="attackedDEF">�h�䑤�̖h���</param>
    public void PlayerAction_SkillAttack(int skillNumber, int targetNumber, int attackedDEF, int enemyDataNumber)
    {
        // �_���[�W�ʂ��v�Z
        BasicValue = m_battleSystem.SkillAttack(
            PlayerStatus.ATK,                                                           // �U����
            PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].POW,       // �X�L���̊�b�l
            attackedDEF                                                                 // �h���
            );
        // �������łȂ��Ȃ瑮�����l�������v�Z���s��
        if (PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNone
            && PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNum)
        {
            BasicValue = m_battleSystem.EnemyElementResistance(
                enemyDataNumber,                                                                    // �G�l�~�[�̃f�[�^���ł̔ԍ�
                (int)PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillElement, // �X�L���̑���
                BasicValue                                                                          // �_���[�W
                );
            // �����ϐ��̓o�^���s��
            m_battleManager.PlayerAction_Register(m_myNumber, skillNumber, targetNumber);
        }
        AddingEffectCalculation(targetNumber, skillNumber);
        // �_���[�W��ݒ肷��
        m_battleManager.DamageEnemy(targetNumber, BasicValue);
        PlayerAction_Decrement(skillNumber);
        m_playerAnimation.PlayAnimation(AnimationState.enSkillAttack);
    }

    /// <summary>
    /// �ǉ����ʂ̌v�Z
    /// </summary>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    private void AddingEffectCalculation(int targetNumber,int skillNumber)
    {
        var abnormalState = PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].StateAbnormalData.ActorAbnormalState;
        // �ǉ����ʂ��Ȃ��Ȃ���s���Ȃ�
        if (abnormalState == ActorAbnormalState.enNormal)
        {
            return;
        }
        // ��Ԉُ�ɂ�����Ȃ������Ȃ���s���Ȃ�
        if(m_abnormalCalculation.SpentToStateAbnormal() != true)
        {
            return;
        }
        // �X�e�[�g��ύX����
        m_battleManager.PlayerAction_ChangeStateEnemy(targetNumber,abnormalState);
    }

    /// <summary>
    /// �o�t�E�f�o�t�̏���
    /// </summary>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="attackedATK">�h�䑤�̍U����</param>
    /// <param name="attackedDEF">�h�䑤�̖h���</param>
    /// <param name="attackedSPD">�h�䑤�̑f����</param>
    public int PlayerAction_Buff(int skillNumber, int attackedATK, int attackedDEF, int attackedSPD)
    {
        // �p�����[�^���Q��
        var param = 0;
        switch (PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].BuffType)
        {
            case BuffType.enATK:
                param = attackedATK;
                break;
            case BuffType.enDEF:
                param = attackedDEF;
                break;
            case BuffType.enSPD:
                param = attackedSPD;
                break;
        }
        // �l�̌v�Z
        var value = m_battleSystem.SkillBuff(
            param,
            PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].POW
            );
        PlayerAction_Decrement(skillNumber);
        return value;
    }

    /// <summary>
    /// HP���񕜂��鏈��
    /// </summary>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    public void PlayerAction_HPRecover(int targetNumber, int skillNumber)
    {
        // �񕜗ʂ��v�Z����
        BasicValue = m_battleSystem.SkillHeal(
                PlayerData.playerDataList[targetNumber].HP,
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].POW
                );

        PlayerAction_Decrement(skillNumber);
    }

    /// <summary>
    /// SP�EHP������鏈��
    /// </summary>
    private void PlayerAction_Decrement(int skillNumber)
    {
        // �l���v�Z����
        var necessaryValue = PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillNecessary;
        // SP�EHP�������
        switch (PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].Type)
        {
            case NecessaryType.enSP:
                DecrementSP(necessaryValue);
                break;
            case NecessaryType.enHP:
                DecrementHP(necessaryValue);
                break;
        }
    }

    /// <summary>
    /// �h�䏈��
    /// </summary>
    public void PlayerAction_Guard()
    {
        // �h��͂��v�Z
        m_defencePower = m_battleSystem.Guard(m_playerBattleStatus.DEF);
        m_playerBattleStatus.DEF += m_defencePower;
    }

    /// <summary>
    /// HP�̉񕜏���
    /// </summary>
    /// <param name="recoverValue">�񕜗�</param>
    public void RecoverHP(int recoverValue)
    {
        m_playerBattleStatus.HP += recoverValue;
        // ���ȏ�Ȃ�␳
        if (m_playerBattleStatus.HP >= m_saveDataManager.SaveData.saveData.PlayerList[MyNumber].HP)
        {
            m_playerBattleStatus.HP = m_saveDataManager.SaveData.saveData.PlayerList[MyNumber].HP;
        }
        ActorHPState = SetHPStatus();
    }

    /// <summary>
    /// HP�̌�������
    /// </summary>
    /// <param name="decrementValue">�_���[�W��</param>
    public void DecrementHP(int decrementValue)
    {
        m_playerBattleStatus.HP -= decrementValue;
        // ���ȉ��Ȃ�␳
        if (m_playerBattleStatus.HP <= HPMIN_VALUE)
        {
            m_playerBattleStatus.HP = HPMIN_VALUE;
        }
        if(m_playerBattleStatus.HP <= 0)
        {
            m_playerAnimation.PlayAnimation(AnimationState.enDamage_Down);
        }
        else
        {
            m_playerAnimation.PlayAnimation(AnimationState.enDamage);
        }
        ActorHPState = SetHPStatus();
    }

    /// <summary>
    /// SP�̌�������
    /// </summary>
    /// <param name="decrementValue">�����</param>
    public void DecrementSP(int decrementValue)
    {
        m_playerBattleStatus.SP -= decrementValue;
        // ���ȉ��Ȃ�␳
        if (m_playerBattleStatus.SP <= SPMIN_VALUE)
        {
            m_playerBattleStatus.SP = SPMIN_VALUE;
        }
    }

    /// <summary>
    /// �o�t�E�f�o�t�����������Ƃ��̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="buffType">�ύX����X�e�[�^�X</param>
    /// <param name="statusFloatingValue">�ύX����l</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    ///  <param name="isBuff">true�Ȃ�o�t�Bfalse�Ȃ�f�o�t</param>
    public void SetBuffStatus(BuffType buffType, int statusFloatingValue,int skillNumber, bool isBuff)
    {
        var effectTime = 1;
        // �X�L���̔ԍ����w�肳��Ă���Ȃ�
        if(skillNumber >= 0)
        {
            effectTime = SkillData.skillDataList[skillNumber].StateAbnormalData.EffectTime;
        }

        switch (buffType)
        {
            case BuffType.enATK:
                if (isBuff == true)
                {
                    m_buffCalculation.CalcBuff(BuffStatus.enBuff_ATK, statusFloatingValue, m_playerBattleStatus.ATK, effectTime);
                    m_drawCommandText.SetStatusText(BuffStatus.enBuff_ATK);
                    break;
                }
                m_buffCalculation.CalcDebuff(BuffStatus.enDeBuff_ATK, statusFloatingValue, m_playerBattleStatus.ATK, effectTime);
                m_drawCommandText.SetStatusText(BuffStatus.enDeBuff_ATK);
                break;
            case BuffType.enDEF:
                if (isBuff == true)
                {
                    m_buffCalculation.CalcBuff(BuffStatus.enBuff_DEF, statusFloatingValue, m_playerBattleStatus.DEF, effectTime);
                    m_drawCommandText.SetStatusText(BuffStatus.enBuff_DEF);
                    break;
                }
                m_buffCalculation.CalcDebuff(BuffStatus.enDeBuff_DEF, statusFloatingValue, m_playerBattleStatus.DEF, effectTime);
                m_drawCommandText.SetStatusText(BuffStatus.enDeBuff_DEF);
                break;
            case BuffType.enSPD:
                if (isBuff == true)
                {
                    m_buffCalculation.CalcBuff(BuffStatus.enBuff_SPD, statusFloatingValue, m_playerBattleStatus.SPD, effectTime);
                    m_drawCommandText.SetStatusText(BuffStatus.enBuff_SPD);
                    break;
                }
                m_buffCalculation.CalcDebuff(BuffStatus.enDeBuff_SPD, statusFloatingValue, m_playerBattleStatus.SPD, effectTime);
                m_drawCommandText.SetStatusText(BuffStatus.enDeBuff_SPD);
                break;
        }
    }

    /// <summary>
    /// �摜����]������
    /// </summary>
    private void RotationSprite()
    {
        var lookAtCamera = Camera.main.transform.position;
        lookAtCamera.y = transform.position.y;  // �␳
        transform.LookAt(lookAtCamera);
    }
    
    /// <summary>
    /// ���g�̍s�����I������
    /// </summary>
    /// <param name="actionType">�s���p�^�[��</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    public void ActionEnd(ActionType actionType, int skillNumber)
    {
#if UNITY_EDITOR
        m_drawCommandText.SetCommandText(actionType, PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].ID);
#endif
        ActionEndFlag = true;
    }

    /// <summary>
    /// �v���C���[�̍s�������Z�b�g����
    /// </summary>
    public void ResetStatus()
    {
        // �K�[�h���Ă����Ȃ�
        if (NextActionType == ActionType.enGuard)
        {
            // �h��͂����ɖ߂�
            m_playerBattleStatus.DEF -= m_defencePower;
        }

        NextActionType = ActionType.enNull;
        m_isActionEnd = false;
    }

    /// <summary>
    /// �X�e�[�^�X�����ɖ߂�
    /// </summary>
    private void ResetBuffStatus(BuffStatus buffStatus)
    {
        switch (buffStatus)
        {
            case BuffStatus.enBuff_ATK:
                m_buffCalculation.ResetStatus(BuffStatus.enBuff_ATK, m_playerBattleStatus.ATK, true);
                break;
            case BuffStatus.enBuff_DEF:
                m_buffCalculation.ResetStatus(BuffStatus.enBuff_DEF, m_playerBattleStatus.DEF, true);
                break;
            case BuffStatus.enBuff_SPD:
                m_buffCalculation.ResetStatus(BuffStatus.enBuff_SPD, m_playerBattleStatus.SPD, true);
                break;
            case BuffStatus.enDeBuff_ATK:
                m_buffCalculation.ResetStatus(BuffStatus.enDeBuff_ATK, m_playerBattleStatus.ATK, false);
                break;
            case BuffStatus.enDeBuff_DEF:
                m_buffCalculation.ResetStatus(BuffStatus.enDeBuff_DEF, m_playerBattleStatus.DEF, false);
                break;
            case BuffStatus.enDeBuff_SPD:
                m_buffCalculation.ResetStatus(BuffStatus.enDeBuff_SPD, m_playerBattleStatus.SPD, false);
                break;
        }
    }

    /// <summary>
    /// HP�̏�Ԃ�ݒ肷��
    /// </summary>
    /// <returns>HP�̏��</returns>
    private ActorHPState SetHPStatus()
    {
        if(PlayerStatus.HP <= HPMIN_VALUE)
        {
            Die();  // ���S����
            return ActorHPState.enDie;
        }
        if(PlayerStatus.HP <= PlayerData.playerDataList[m_myNumber].HP / 4)
        {
            return ActorHPState.enFewHP;
        }
        return ActorHPState.enMaxHP;
    }

    /// <summary>
    /// ��Ԉُ�̌v�Z
    /// </summary>
    public void CalculationAbnormalState()
    {
        if(ActorAbnormalState == ActorAbnormalState.enNormal || ActorAbnormalState == ActorAbnormalState.enSilence)
        {
            return;
        }
        if (m_abnormalCalculation.RecoverToAbnormal(ActorAbnormalState) == true)
        {
            PoisonDamage = 0;
            NextActionType = ActionType.enNull;
            ConfusionFlag = false;
            ActorAbnormalState = ActorAbnormalState.enNormal;
            return;
        }
        switch (ActorAbnormalState)
        {
            case ActorAbnormalState.enPoison:
#if UNITY_EDITOR
                Debug.Log($"{PlayerData.playerDataList[m_myNumber].PlayerName}�͓ł𗁂тĂ���");
#endif
                PoisonDamage = m_abnormalCalculation.Poison(PlayerStatus.HP);
                break;
            case ActorAbnormalState.enParalysis:
                if (m_abnormalCalculation.Paralysis() == true)
                {
#if UNITY_EDITOR
                    Debug.Log($"{PlayerData.playerDataList[m_myNumber].PlayerName}�͖�Ⴢ��Ă���");
#endif
                    NextActionType = ActionType.enNull;
                }
                break;
            case ActorAbnormalState.enConfusion:
                if (m_abnormalCalculation.Confusion() == true)
                {
#if UNITY_EDITOR
                    Debug.Log($"{PlayerData.playerDataList[m_myNumber].PlayerName}�͍������Ă���");
#endif
                    NextActionType = ActionType.enAttack;
                    ConfusionFlag = true;
                }
                break;
        }
    }

    /// <summary>
    /// ���S���o
    /// </summary>
    async private void Die()
    {
        // ���o���I����������s����
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        m_isActionEnd = true;       // �s�����ł��Ȃ��̂ōs���I���̃t���O�𗧂Ă�
        tag = "DiePlayer";
    }
}
