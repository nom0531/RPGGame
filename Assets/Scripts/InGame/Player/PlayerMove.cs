using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public ActorHPState HPState;                    // HP�̏��
    public ActorAbnormalState AbnormalState;        // ��Ԉُ�
    public ActionType ActionType;                   // ���̍s��
}

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField,Header("���g�̔ԍ�")]
    private int m_myNumber = 0;

    private const int HPMIN_VALUE = 0;              // HP�̍ŏ��l
    private const int SPMIN_VALUE = 0;              // SP�̍ŏ��l

    private BattleSystem m_battleSystem;
    private PlayerBattleStatus m_playerBattleStatus;// �퓬�̃X�e�[�^�X
    private BuffCalculation m_buffCalculation;      // �o�t�̌v�Z
    private DrawCommandText m_drawCommandText;      // �R�}���h�̕\��
    private int m_selectSkillNumber = 0;            // �I�����Ă���X�L���̔ԍ�
    private bool m_isActionEnd = false;             // �s�����I�����Ă��邩�ǂ���
    private bool m_isConfusion = false;             // �������Ă��邩�ǂ���

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
        get => m_playerBattleStatus.ActionType;
        set => m_playerBattleStatus.ActionType = value;
    }

    public PlayerBattleStatus PlayerStatus
    {
        get => m_playerBattleStatus;
    }

    public ActorHPState ActorHPState
    {
        get => m_playerBattleStatus.HPState;
    }

    public ActorAbnormalState ActorAbnormalState
    {
        get => m_playerBattleStatus.AbnormalState;
        set => m_playerBattleStatus.AbnormalState = value;
    }

    public bool Confusion
    {
        get => m_isConfusion;
        set => m_isConfusion = value;
    }

    private void Awake()
    {
        m_battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        m_buffCalculation = this.gameObject.GetComponent<BuffCalculation>();
        m_drawCommandText = this.gameObject.GetComponent<DrawCommandText>();
        //m_drawDamageText =GameObject.FindGameObjectWithTag("UICanvas").GetComponent<DrawDamageText>();

        SetStatus();
        SetSkills();
    }

    /// <summary>
    /// �X�e�[�^�X������������
    /// </summary>
    private void SetStatus()
    {
        SaveDataManager saveData = GameManager.Instance.SaveData;

        m_playerBattleStatus.HP = saveData.SaveData.saveData.PlayerList[m_myNumber].HP;
        m_playerBattleStatus.SP = saveData.SaveData.saveData.PlayerList[m_myNumber].SP;
        m_playerBattleStatus.ATK = saveData.SaveData.saveData.PlayerList[m_myNumber].ATK;
        m_playerBattleStatus.DEF = saveData.SaveData.saveData.PlayerList[m_myNumber].DEF;
        m_playerBattleStatus.SPD = saveData.SaveData.saveData.PlayerList[m_myNumber].SPD;
        m_playerBattleStatus.LUCK = saveData.SaveData.saveData.PlayerList[m_myNumber].LUCK;
        m_playerBattleStatus.HPState = SetHPStatus();
        m_playerBattleStatus.AbnormalState = ActorAbnormalState.enNormal;
        m_playerBattleStatus.ActionType = ActionType.enNull;
    }

    /// <summary>
    /// �X�L���f�[�^������������
    /// </summary>
    private void SetSkills()
    {
        for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[m_myNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // ���ʔԍ��������Ȃ�f�[�^������������
                if (PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillNumber != SkillData.skillDataList[dataNumber].SkillNumber)
                {
                    continue;
                }

                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillDetail = SkillData.skillDataList[dataNumber].SkillDetail;
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
            }
        }
    }

    private void FixedUpdate()
    {
        RotationSprite();
        m_playerBattleStatus.HPState = SetHPStatus();

        for (int i = 0; i < (int)BuffStatus.enNum; i++)
        {
            if (m_buffCalculation.GetEffectEndFlag((BuffStatus)i) == false)
            {
                continue;
            }
            // ���ʎ��Ԃ��I�����Ă���Ȃ�X�e�[�^�X��߂�
            ResetPlayerBuffStatus((BuffStatus)i);
            m_buffCalculation.SetEffectEndFlag((BuffStatus)i, false);
        }
    }

    /// <summary>
    /// HP���񕜂����鏈��
    /// </summary>
    /// <param name="recoverValue">�񕜗�</param>
    public void RecoverHP(int recoverValue)
    {
        m_playerBattleStatus.HP += recoverValue;

        // ���ȏ�Ȃ�␳
        if (m_playerBattleStatus.HP >= PlayerData.playerDataList[m_myNumber].HP)
        {
            m_playerBattleStatus.HP = PlayerData.playerDataList[m_myNumber].HP;
        }

        m_playerBattleStatus.HPState = SetHPStatus();
    }

    /// <summary>
    /// HP�����������鏈��
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

        m_playerBattleStatus.HPState = SetHPStatus();
    }

    /// <summary>
    /// SP�����������鏈��
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
    /// �h�䏈��
    /// </summary>
    /// <returns>�h��́B�����_�ȉ��͐؂�̂�</returns>
    public int Guard()
    {
        int defensePower = m_battleSystem.Guard(m_playerBattleStatus.DEF);
        return defensePower;
    }

    /// <summary>
    /// �o�t�E�f�o�t�����������Ƃ��̃X�e�[�^�X��ύX����
    /// </summary>
    /// <param name="buffType">�ύX����X�e�[�^�X</param>
    /// <param name="statusFloatingValue">�ύX����l</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    ///  <param name="isBuff">true�Ȃ�o�t�Bfalse�Ȃ�f�o�t</param>
    public void SetPlayerBuffStatus(BuffType buffType, int statusFloatingValue,int skillNumber, bool isBuff)
    {
        int effectTime = 1;
        // �X�L���̔ԍ����w�肳��Ă���Ȃ�
        if(skillNumber >= 0)
        {
            effectTime = PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].StateAbnormalData.EffectTime;
        }

        switch (buffType)
        {
            case BuffType.enATK:
                if (isBuff == true)
                {
                    m_buffCalculation.CalcBuff(
                        BuffStatus.enBuff_ATK, statusFloatingValue, m_playerBattleStatus.ATK, effectTime);
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
        Vector3 lookAtCamera = Camera.main.transform.position;
        lookAtCamera.y = transform.position.y;  // �␳
        transform.LookAt(lookAtCamera);
    }

    /// <summary>
    /// �v���C���[�̍s�������Z�b�g����
    /// </summary>
    public void ResetPlayerStatus()
    {
        // �K�[�h���Ă����Ȃ�
        if (m_playerBattleStatus.ActionType == ActionType.enGuard)
        {
            // �h��͂����ɖ߂�
            m_playerBattleStatus.DEF -= m_buffCalculation.GetBuffParam(BuffStatus.enBuff_DEF);
        }

        m_playerBattleStatus.ActionType = ActionType.enNull;
        m_isActionEnd = false;
    }

    /// <summary>
    /// �v���C���[�̃X�e�[�^�X�����ɖ߂�
    /// </summary>
    private void ResetPlayerBuffStatus(BuffStatus buffStatus)
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
    /// <param name="NowHP">���݂�HP</param>
    /// <returns>HP�̏��</returns>
    private ActorHPState SetHPStatus()
    {
        if(PlayerStatus.HP <= HPMIN_VALUE)
        {
            m_isActionEnd = true;       // �s�����ł��Ȃ��̂ōs���I���̃t���O�𗧂Ă�
            return ActorHPState.enDie;
        }

        if(PlayerStatus.HP <= PlayerData.playerDataList[m_myNumber].HP / 4)
        {
            return ActorHPState.enFewHP;
        }

        return ActorHPState.enMaxHP;
    }
}
