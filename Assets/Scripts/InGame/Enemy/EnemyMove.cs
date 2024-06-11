using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// �퓬���̃G�l�~�[�̃X�e�[�^�X
/// </summary>
public struct EnemyBattleStatus
{
    public int HP;                              // ���݂�HP
    public int ATK;                             // ���݂�ATK
    public int DEF;                             // ���݂�DEF
    public int SPD;                             // ���݂�SPD
}

public class EnemyMove : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private EnemyDataBase EnemyData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField]
    private EnemyMoveDataBase EnemyMoveData;

    private const int HPMIN_VALUE = 0;              // HP�̍ŏ��l

    private List<PlayerMove> m_playerMoveList;
    private SaveDataManager m_saveDataManager;
    private BattleSystem m_battleSystem;
    private BattleManager m_battleManager;
    private PauseManager m_pauseManager;
    private StagingManager m_stagingManager;
    private StateAbnormalCalculation m_abnormalCalculation;
    private BuffCalculation m_buffCalculation;
    private DrawCommandText m_drawCommandText;
    private EnemyBattleStatus m_enemyBattleStatus;                                      // �퓬���̃X�e�[�^�X
    private ActorHPState m_actorHPState = ActorHPState.enMaxHP;                         // HP�̏��
    private ActorAbnormalState m_actorAbnormalState = ActorAbnormalState.enNormal;      // ��Ԉُ�
    private ActionType m_actionType = ActionType.enNull;                                // ���̍s��
    private int m_myNumber = 0;                                                         // ���g�̔ԍ�
    private int m_basicValue = 0;                                                       // �_���[�W�ʁE�񕜗�
    private int m_defencePower = 0;                                                     // �h���
    private int m_poisonDamage = 0;                                                     // �ŏ�Ԏ��̃_���[�W
    private bool m_isConfusion = false;                                                 // �������Ă��邩�ǂ���
    private bool m_isActionEnd = false;                                                 // �s�����I�����Ă��邩�ǂ���
    private bool m_isWeak = false;                                                      // ��_��˂��ꂽ���ǂ���

    public int MyNumber
    {
        get => m_myNumber;
        set => m_myNumber = value;
    }

    public EnemyBattleStatus EnemyStatus
    {
        get => m_enemyBattleStatus;
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

    public ActionType NextActionType
    {
        get => m_actionType;
        set => m_actionType = value;
    }

    public bool ActionEndFlag
    {
        get => m_isActionEnd;
    }

    public bool WeakFlag
    {
        get => m_isWeak;
        set => m_isWeak = value;
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

    /// <summary>
    /// �G�l�~�[�𔭌����Ă��邩�擾����
    /// </summary>
    /// <param name="enemyNumber">�G�l�~�[�̔ԍ�</param>
    /// <returns>true�Ȃ甭�����Ă���</returns>
    public bool GetTrueEnemyRegister(int enemyNumber)
    {
        return m_saveDataManager.SaveData.saveData.EnemyRegisters[enemyNumber];
    }

    /// <summary>
    /// �G�l�~�[�𔭌������t���O��true�ɂ���
    /// </summary>
    /// <param name="enemyNumber">�G�l�~�[�̔ԍ�</param>
    public void SetTrueEnemyRegister(int enemyNumber)
    {
        m_saveDataManager.SaveData.saveData.EnemyRegisters[enemyNumber] = true;
        m_saveDataManager.Save(false);
    }

    /// <summary>
    /// �����̑ϐ��x�𔭌����Ă��邩�ǂ����擾����
    /// </summary>
    /// <param name="elementNumber">�����ԍ�</param>
    public bool GetTrueElementRegister(int elementNumber)
    {
        return m_saveDataManager.SaveData.saveData.ElementRegisters[m_myNumber].Elements[elementNumber];
    }

    /// <summary>
    /// �����̑ϐ��x�𔭌������t���O��true�ɂ���
    /// </summary>
    /// <param name="elementNumber">�����ԍ�</param>
    public void SetTrueElementRegister(int elementNumber)
    {
        m_saveDataManager.SaveData.saveData.ElementRegisters[m_myNumber].Elements[elementNumber] = true;
        m_saveDataManager.Save(false);
    }

    private void Awake()
    {
        m_saveDataManager = GameManager.Instance.SaveDataManager;
        m_stagingManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<StagingManager>();
        m_battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_pauseManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<PauseManager>();
        m_abnormalCalculation = GetComponent<StateAbnormalCalculation>();
        m_buffCalculation = GetComponent<BuffCalculation>();
        m_drawCommandText = GetComponent<DrawCommandText>();
    }

    private void Start()
    {
        SetStatus();
        // �v���C���[�̃��X�g���Q��
        m_playerMoveList = m_battleManager.PlayerMoveList;
        SetLookAtPosition();
    }

    private void FixedUpdate()
    {
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
    /// �X�e�[�^�X������������
    /// </summary>
    private void SetStatus()
    {
#if UNITY_EDITOR
        m_enemyBattleStatus.HP = 9999;  // �f�o�b�O�p
        m_enemyBattleStatus.ATK = EnemyData.enemyDataList[m_myNumber].ATK;
        m_enemyBattleStatus.DEF = EnemyData.enemyDataList[m_myNumber].DEF;
        m_enemyBattleStatus.SPD = EnemyData.enemyDataList[m_myNumber].SPD;
#else
        m_enemyBattleStatus.HP = EnemyData.enemyDataList[m_myNumber].HP;
        m_enemyBattleStatus.ATK = EnemyData.enemyDataList[m_myNumber].ATK;
        m_enemyBattleStatus.DEF = EnemyData.enemyDataList[m_myNumber].DEF;
        m_enemyBattleStatus.SPD = EnemyData.enemyDataList[m_myNumber].SPD;
#endif
    }

    /// <summary>
    /// ���g�̎q�I�u�W�F�N�g�̍��W��ݒ肷��
    /// </summary>
    private void SetLookAtPosition()
    {
        var lookAt = transform.GetChild(0).gameObject;
        lookAt.transform.position = new Vector3(transform.position.x, transform.position.y + SetPositionY(), transform.position.z);
    }

    /// <summary>
    /// Y���W��ݒ肷��
    /// </summary>
    /// <returns>���Z����Y���W</returns>
    private float SetPositionY()
    {
        var y = 0.0f;
        switch (EnemyData.enemyDataList[MyNumber].EnemySize)
        {
            case EnemySize.enExtraSmal:
                y =  0.5f;
                break;
            case EnemySize.enSmall:
                y =  1.5f;
                break;
            case EnemySize.enMedium:
                y =  3.0f;
                break;
            case EnemySize.enLarge:
                y =  5.0f;
                break;
        }
        return y;
    }

    /// <summary>
    /// �X�e�[�^�X�����Z�b�g����
    /// </summary>
    public void ResetStatus()
    {
        // �K�[�h���Ă����Ȃ�
        if (NextActionType == ActionType.enGuard)
        {
            // �h��͂����ɖ߂�
            m_enemyBattleStatus.DEF -= m_defencePower;
        }
        NextActionType = ActionType.enNull;
        m_isActionEnd = false;
        m_isWeak = false;
    }

    /// <summary>
    /// �U������
    /// </summary>
    /// <param name="attackedDEF">�h�䑤�̖h���</param>
    /// <returns>�_���[�W��</returns>
    public void EnemyAction_Attack(int targetNumber, int attackedDEF)
    {
        // �_���[�W�ʂ��v�Z
        BasicValue = m_battleSystem.NormalAttack(
            EnemyStatus.ATK, // �U����
            attackedDEF      // �h���
            );
        // ������ԂȂ�
        if (ConfusionFlag == true)
        {
            m_battleManager.DamageEnemy(targetNumber, BasicValue);
            return;
        }
        m_battleManager.DamagePlayer(targetNumber,BasicValue);
    }

    /// <summary>
    /// �X�L������
    /// </summary>
    ///<param name="attackedDEF">�h�䑤�̖h���</param>
    ///<param name="skillNumber">�X�L���̔ԍ�</param>
    ///<returns>�_���[�W��</returns>
    public void EnemyAction_SkillAttack(int skillNumber, int targetNumber, int attackedDEF, int playerDataNumber)
    {
        // �_���[�W�ʂ��v�Z
        BasicValue = m_battleSystem.SkillAttack(
            EnemyStatus.ATK,                                                            // �p�����[�^
            EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].POW,         // �X�L���̊�b�l
            attackedDEF                                                                 // �h���
            );
        // �������łȂ��Ȃ瑮�����l�������v�Z���s��
        if (EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNone
            && EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNum)
        {
            BasicValue = m_battleSystem.PlayerElementResistance(
                playerDataNumber,                                                                   // �v���C���[�̃f�[�^���ł̔ԍ�
                (int)EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillElement,   // �X�L���̑���
                BasicValue                                                                          // �_���[�W
                );
        }
        AddingEffectCalculation(targetNumber, skillNumber);
        // �_���[�W��ݒ肷��
        m_battleManager.DamagePlayer(targetNumber, BasicValue);
    }

    /// <summary>
    /// �ǉ����ʂ̌v�Z
    /// </summary>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    private void AddingEffectCalculation(int targetNumber, int skillNumber)
    {
        var abnormalState = EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].StateAbnormalData.ActorAbnormalState;
        // �ǉ����ʂ��Ȃ��Ȃ���s���Ȃ�
        if (abnormalState == ActorAbnormalState.enNormal)
        {
            return;
        }
        // ��Ԉُ�ɂ�����Ȃ������Ȃ���s���Ȃ�
        if (m_abnormalCalculation.SpentToStateAbnormal() != true)
        {
            return;
        }
        // �X�e�[�g��ύX����
        m_battleManager.EnemyAction_ChangeStatePlayer(targetNumber, abnormalState);
    }

    /// <summary>
    /// �o�t�E�f�o�t����
    /// </summary>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="attackedATK">�h�䑤�̍U����</param>
    /// <param name="attackedDEF">�h�䑤�̖h���</param>
    /// <param name="attackedSPD">�h�䑤�̑f����</param>
    /// <param name="isBuff">true�Ȃ�o�t�Bfalse�Ȃ�f�o�t</param>
    public int EnemyAction_Buff(int skillNumber, int attackedATK, int attackedDEF, int attackedSPD)
    {
        // �p�����[�^���Q��
        var param = 0;
        switch (EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].BuffType)
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
        // �l���v�Z
        var value = m_battleSystem.SkillBuff(
            param,
            EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].POW
            );
        return value;
    }

    /// <summary>
    /// �񕜏���
    /// </summary>
    /// <param name="attackedHP">�g�p����鑤�̗̑�</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    public void EnemyAction_HPRecover(int attackedHP,int skillNumber)
    {
        // �񕜗ʂ��v�Z
        BasicValue = m_battleSystem.SkillHeal(
            attackedHP,
            EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].POW
            );
    }
    
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="targetNumber">�^�[�Q�b�g�̔ԍ�</param>
    /// <returns></returns>
    public int EnemyAction_HPResurrection(int skillNumber, int targetNumber)
    {
        // �I�������̂������ł͂Ȃ��ꍇ�A���̂܂ܔԍ���Ԃ�
        if (EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillType != SkillType.enResurrection)
        {
            return targetNumber;
        }
        // �I�u�W�F�N�g���擾����
        var gameObject = SelectTargetDieEnemy();
        // �I�u�W�F�N�g�����݂��Ȃ��Ȃ牽�����Ȃ�
        if (gameObject == null)
        {
            return targetNumber;
        }

        // ���X�g�ɒǉ�
        var newTargetNumber = m_battleManager.EnemyListAdd(this);
        // �I�u�W�F�N�g�̐ݒ��ύX����
        gameObject.SetActive(true);
        gameObject.tag = "Enemy";
        return newTargetNumber;
    }

    /// <summary>
    /// �h�䏈��
    /// </summary>
    public void EnemyAction_Guard()
    {
        m_defencePower = m_battleSystem.Guard(m_enemyBattleStatus.DEF);
        m_enemyBattleStatus.DEF += m_defencePower;
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void EnemyAction_Escape()
    {
        // �����������������ǂ����擾����
        if (m_battleSystem.Escape(EnemyData.enemyDataList[m_myNumber].LUCK) == false)
        {
            return;
        }
        Dissolve();
        m_battleManager.EnemyListRemove(m_myNumber);        // ���������烊�X�g���玩�g���폜
    }

    /// <summary>
    /// HP���񕜂��鏈��
    /// </summary>
    /// <param name="recoverValue">�񕜗�</param>
    public void RecoverHP(int recoverValue)
    {
        m_enemyBattleStatus.HP += recoverValue;
        // ���ʂ�葽���񕜂���Ȃ�␳
        if (m_enemyBattleStatus.HP >= EnemyData.enemyDataList[m_myNumber].HP)
        {
            // �S��
            m_enemyBattleStatus.HP = EnemyData.enemyDataList[m_myNumber].HP;
        }
        ActorHPState = SetHPStatus();
    }

    /// <summary>
    /// HP�̌�������
    /// </summary>
    /// <param name="decrementValue">�_���[�W��</param>
    public void DecrementHP(int decrementValue)
    {
        m_enemyBattleStatus.HP -= decrementValue;
        // ���ʂ������Ȃ�␳
        if (m_enemyBattleStatus.HP <= HPMIN_VALUE)
        {
            // ���S����
            m_enemyBattleStatus.HP = HPMIN_VALUE;
        }
        ActorHPState = SetHPStatus();
    }

    /// <summary>
    /// �o�t�E�f�o�t�����������Ƃ��̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="buffType">�ύX����X�e�[�^�X</param>
    /// <param name="statusFloatingValue">�ύX����l</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="isBuff">true�Ȃ�o�t�Bfalse�Ȃ�f�o�t</param>
    public void SetBuffStatus(BuffType buffType, int statusFloatingValue, int skillNumber, bool isBuff)
    {
        var effectTime = 1;
        // �X�L���̔ԍ����w�肳��Ă���Ȃ�
        if (skillNumber >= 0)
        {
            effectTime = SkillData.skillDataList[skillNumber].StateAbnormalData.EffectTime;
        }

        switch (buffType)
        {
            case BuffType.enATK:
                if (isBuff == true)
                {
                    m_buffCalculation.CalcBuff(BuffStatus.enBuff_ATK, statusFloatingValue, m_enemyBattleStatus.ATK, effectTime);
                    m_drawCommandText.SetStatusText(BuffStatus.enBuff_ATK);
                    break;
                }
                m_buffCalculation.CalcDebuff(BuffStatus.enDeBuff_ATK, statusFloatingValue, m_enemyBattleStatus.ATK, effectTime);
                m_drawCommandText.SetStatusText(BuffStatus.enDeBuff_ATK);
                break;
            case BuffType.enDEF:
                if (isBuff == true)
                {
                    m_buffCalculation.CalcBuff(BuffStatus.enBuff_DEF, statusFloatingValue, m_enemyBattleStatus.DEF, effectTime);
                    m_drawCommandText.SetStatusText(BuffStatus.enBuff_DEF);
                    break;
                }
                m_buffCalculation.CalcDebuff(BuffStatus.enDeBuff_DEF, statusFloatingValue, m_enemyBattleStatus.DEF, effectTime);
                m_drawCommandText.SetStatusText(BuffStatus.enDeBuff_DEF);
                break;
            case BuffType.enSPD:
                if (isBuff == true)
                {
                    m_buffCalculation.CalcBuff(BuffStatus.enBuff_SPD, statusFloatingValue, m_enemyBattleStatus.SPD, effectTime);
                    m_drawCommandText.SetStatusText(BuffStatus.enBuff_SPD);
                    break;
                }
                m_buffCalculation.CalcDebuff(BuffStatus.enDeBuff_SPD, statusFloatingValue, m_enemyBattleStatus.SPD, effectTime);
                m_drawCommandText.SetStatusText(BuffStatus.enDeBuff_SPD);
                break;
        }
    }

    /// <summary>
    /// �^�[�Q�b�g�v���C���[��I�����鏈��
    /// </summary>
    /// <returns>�v���C���[�̔ԍ�</returns>
    public int SelectTargetPlayer()
    {
        var attackNumber = m_battleSystem.GetRandomValue(0, 2);
        // �^�[�Q�b�g���Ђ񎀂łȂ��Ȃ炱���ŏI��
        if(m_playerMoveList[attackNumber].ActorHPState != ActorHPState.enDie)
        {
            return attackNumber;
        }
        for(int i = 0; i < m_playerMoveList.Count; i++)
        {
            if(m_playerMoveList[attackNumber].ActorHPState == ActorHPState.enDie)
            {
                continue;
            }
            attackNumber = i;   // �Ђ񎀂łȂ��v���C���[���^�[�Q�b�g�ɂ���
            break;
        }
        return attackNumber;
    }

    /// <summary>
    /// �^�[�Q�b�g�G�l�~�[��I�����鏈��
    /// </summary>
    /// <param name="maxNumber">�G�l�~�[�̍ő吔</param>
    /// <returns></returns>
    public int SelectTargetEnemy(int maxNumber)
    {
        // 0�`�G�l�~�[�̍ő吔�ŗ����𐶐�
        return m_battleSystem.GetRandomValue(0, maxNumber, false);
    }

    /// <summary>
    /// �Ђ񎀏�Ԃ̃G�l�~�[�̒����烉���_����1�̃^�[�Q�b�g�ɂ��鏈��
    /// </summary>
    /// <returns>�����_���ɑI�������I�u�W�F�N�g</returns>
    public GameObject SelectTargetDieEnemy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("DieEnemy");
        // 0�`�G�l�~�[�̍ő吔�ŗ����𐶐�
        var rand = m_battleSystem.GetRandomValue(0, enemys.Length, false);
        return enemys[rand].gameObject;
    }

    /// <summary>
    /// �s����I�����鏈��
    /// </summary>
    /// <returns>�s���ԍ�</returns>
    public ActionType SelectAttackType()
    {
        for(int i = 0; i < EnemyData.enemyDataList[m_myNumber].enemyMoveList.Count; i++)
        {
            if(DecisionHPState(i) == false)
            {
                continue;
            }
            if(DecisionAbnormalState(i) == false)
            {
                continue;
            }
            // �s���p�^�[����ݒ�
            NextActionType = EnemyData.enemyDataList[m_myNumber].enemyMoveList[i].ActionType;
            break;
        }
        return NextActionType;
    }

    /// <summary>
    /// �s�����I������
    /// </summary>
    public void ActionEnd()
    {
        m_isActionEnd = true;
        WeakFlag = false;
    }

    /// <summary>
    /// HP�̃X�e�[�g�̔���
    /// </summary>
    /// <param name="number">�Y����</param>
    /// <returns>true�Ȃ瓖�Ă͂܂��Ă���Bfalse�Ȃ瓖�Ă͂܂��Ă��Ȃ�</returns>
    private bool DecisionHPState(int number)
    {
        // �w�肪�Ȃ��ꍇ�͖�������
        if(EnemyData.enemyDataList[m_myNumber].enemyMoveList[number].ActorHPState == ActorHPState.enNull)
        {
            return true;
        }
        // �قȂ�Ȃ�false
        if(EnemyData.enemyDataList[m_myNumber].enemyMoveList[number].ActorHPState != ActorHPState)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// ��Ԉُ�̔���
    /// </summary>
    /// <param name="number">�Y����</param>
    /// <returns>true�Ȃ瓖�Ă͂܂��Ă���Bfalse�Ȃ瓖�Ă͂܂��Ă��Ȃ�</returns>
    private bool DecisionAbnormalState(int number)
    {
        // �w�肪�Ȃ��ꍇ�͖�������
        if(EnemyData.enemyDataList[m_myNumber].enemyMoveList[number].ActorAbnormalState == global::ActorAbnormalState.enNull)
        {
            return true;
        }
        // �قȂ�Ȃ�false
        if (EnemyData.enemyDataList[m_myNumber].enemyMoveList[number].ActorAbnormalState != ActorAbnormalState)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// ��Ԉُ�̌v�Z
    /// </summary>
    public void CalculationAbnormalState()
    {
        if (ActorAbnormalState == ActorAbnormalState.enNormal || ActorAbnormalState == ActorAbnormalState.enSilence)
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
                Debug.Log($"{EnemyData.enemyDataList[m_myNumber].EnemyName}�͓ł𗁂тĂ���");
#endif
                PoisonDamage = m_abnormalCalculation.Poison(EnemyStatus.HP);
                break;
            case ActorAbnormalState.enParalysis:
                if (m_abnormalCalculation.Paralysis() == true)
                {
#if UNITY_EDITOR
                    Debug.Log($"{EnemyData.enemyDataList[m_myNumber].EnemyName}�͖�Ⴢ��Ă���");
#endif
                    NextActionType = ActionType.enNull;
                }
                break;
            case ActorAbnormalState.enConfusion:
                if (m_abnormalCalculation.Confusion() == true)
                {
#if UNITY_EDITOR
                    Debug.Log($"{EnemyData.enemyDataList[m_myNumber].EnemyName}�͍������Ă���");
#endif
                    NextActionType = ActionType.enAttack;
                    ConfusionFlag = true;
                }
                break;
        }
    }

    /// <summary>
    /// �g�p����X�L�������肷�鏈��
    /// </summary>
    /// <returns>�X�L���̔ԍ�</returns>
    public int SelectSkill()
    {
        // 0�`�f�[�^�����X�L���̍ő吔�܂łŗ����𐶐�����
        return m_battleSystem.GetRandomValue(0, EnemyData.enemyDataList[m_myNumber].skillDataList.Count, false);
    }

    /// <summary>
    /// �X�e�[�^�X�����ɖ߂�
    /// </summary>
    private void ResetBuffStatus(BuffStatus buffStatus)
    {
        switch (buffStatus)
        {
            case BuffStatus.enBuff_ATK:
                m_buffCalculation.ResetStatus(BuffStatus.enBuff_ATK, m_enemyBattleStatus.ATK, true);
                break;
            case BuffStatus.enBuff_DEF:
                m_buffCalculation.ResetStatus(BuffStatus.enBuff_DEF, m_enemyBattleStatus.DEF, true);
                break;
            case BuffStatus.enBuff_SPD:
                m_buffCalculation.ResetStatus(BuffStatus.enBuff_SPD, m_enemyBattleStatus.SPD, true);
                break;
            case BuffStatus.enDeBuff_ATK:
                m_buffCalculation.ResetStatus(BuffStatus.enDeBuff_ATK, m_enemyBattleStatus.ATK, false);
                break;
            case BuffStatus.enDeBuff_DEF:
                m_buffCalculation.ResetStatus(BuffStatus.enDeBuff_DEF, m_enemyBattleStatus.DEF, false);
                break;
            case BuffStatus.enDeBuff_SPD:
                m_buffCalculation.ResetStatus(BuffStatus.enDeBuff_SPD, m_enemyBattleStatus.SPD, false);
                break;
        }
    }

    /// <summary>
    /// HP�̏�Ԃ�ݒ肷��
    /// </summary>
    /// <returns>HP�̏��</returns>
    private ActorHPState SetHPStatus()
    {
        if (EnemyStatus.HP <= HPMIN_VALUE)
        {
            Die();  // ���S����
            return ActorHPState.enDie;
        }
        if (EnemyStatus.HP <= EnemyData.enemyDataList[m_myNumber].HP / 4)
        {
            return ActorHPState.enFewHP;
        }
        return ActorHPState.enMaxHP;
    }

    /// <summary>
    /// ���S���o
    /// </summary>
    async private void Die()
    {
        // ���o���I�������Ȃ�ȉ��̏��������s����
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        Dissolve();
        tag = "DieEnemy";              // �^�O��ύX����
    }

    /// <summary>
    /// �f�B�\���u����
    /// </summary>
    private void Dissolve()
    {
        var changeMaterial = GetComponent<ChangeMaterial>();
        var material = GetComponent<SpriteRenderer>().material = changeMaterial.Change(1);
        material.SetTexture("_MainTex", EnemyData.enemyDataList[MyNumber].EnemySprite.texture);

        GetComponent<Animator>().SetTrigger("Dissolve");
    }
}
