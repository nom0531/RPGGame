using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 戦闘時のプレイヤーのステータス
/// </summary>
public struct PlayerBattleStatus
{
    public int HP;                                  // 現在のHP
    public int SP;                                  // 現在のSP
    public int ATK;                                 // 現在のATK
    public int DEF;                                 // 現在のDEF
    public int SPD;                                 // 現在のSPD
    public int LUCK;                                // 現在のLUCK
    public ActorHPState HPState;                    // HPの状態
    public ActorAbnormalState AbnormalState;        // 状態異常
    public ActionType ActionType;                   // 次の行動
}

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField,Header("自身の番号")]
    private int m_myNumber = 0;

    private const int HPMIN_VALUE = 0;              // HPの最小値
    private const int SPMIN_VALUE = 0;              // SPの最小値

    private BattleSystem m_battleSystem;
    private PlayerBattleStatus m_playerBattleStatus;// 戦闘のステータス
    private BuffCalculation m_buffCalculation;      // バフの計算
    private DrawCommandText m_drawCommandText;      // コマンドの表示
    private int m_selectSkillNumber = 0;            // 選択しているスキルの番号
    private bool m_isActionEnd = false;             // 行動が終了しているかどうか
    private bool m_isConfusion = false;             // 混乱しているかどうか

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
    /// ステータスを初期化する
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
    /// スキルデータを初期化する
    /// </summary>
    private void SetSkills()
    {
        for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[m_myNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // 識別番号が同じならデータを初期化する
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
            // 効果時間が終了しているならステータスを戻す
            ResetPlayerBuffStatus((BuffStatus)i);
            m_buffCalculation.SetEffectEndFlag((BuffStatus)i, false);
        }
    }

    /// <summary>
    /// HPを回復させる処理
    /// </summary>
    /// <param name="recoverValue">回復量</param>
    public void RecoverHP(int recoverValue)
    {
        m_playerBattleStatus.HP += recoverValue;

        // 一定以上なら補正
        if (m_playerBattleStatus.HP >= PlayerData.playerDataList[m_myNumber].HP)
        {
            m_playerBattleStatus.HP = PlayerData.playerDataList[m_myNumber].HP;
        }

        m_playerBattleStatus.HPState = SetHPStatus();
    }

    /// <summary>
    /// HPを減少させる処理
    /// </summary>
    /// <param name="decrementValue">ダメージ量</param>
    public void DecrementHP(int decrementValue)
    {
        m_playerBattleStatus.HP -= decrementValue;

        // 一定以下なら補正
        if (m_playerBattleStatus.HP <= HPMIN_VALUE)
        {
            m_playerBattleStatus.HP = HPMIN_VALUE;
        }

        m_playerBattleStatus.HPState = SetHPStatus();
    }

    /// <summary>
    /// SPを減少させる処理
    /// </summary>
    /// <param name="decrementValue">消費量</param>
    public void DecrementSP(int decrementValue)
    {
        m_playerBattleStatus.SP -= decrementValue;

        // 一定以下なら補正
        if (m_playerBattleStatus.SP <= SPMIN_VALUE)
        {
            m_playerBattleStatus.SP = SPMIN_VALUE;
        }
    }

    /// <summary>
    /// 防御処理
    /// </summary>
    /// <returns>防御力。小数点以下は切り捨て</returns>
    public int Guard()
    {
        int defensePower = m_battleSystem.Guard(m_playerBattleStatus.DEF);
        return defensePower;
    }

    /// <summary>
    /// バフ・デバフがかかったときのステータスを変更する
    /// </summary>
    /// <param name="buffType">変更するステータス</param>
    /// <param name="statusFloatingValue">変更する値</param>
    /// <param name="skillNumber">スキルの番号</param>
    ///  <param name="isBuff">trueならバフ。falseならデバフ</param>
    public void SetPlayerBuffStatus(BuffType buffType, int statusFloatingValue,int skillNumber, bool isBuff)
    {
        int effectTime = 1;
        // スキルの番号が指定されているなら
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
    /// 画像を回転させる
    /// </summary>
    private void RotationSprite()
    {
        Vector3 lookAtCamera = Camera.main.transform.position;
        lookAtCamera.y = transform.position.y;  // 補正
        transform.LookAt(lookAtCamera);
    }

    /// <summary>
    /// プレイヤーの行動をリセットする
    /// </summary>
    public void ResetPlayerStatus()
    {
        // ガードしていたなら
        if (m_playerBattleStatus.ActionType == ActionType.enGuard)
        {
            // 防御力を元に戻す
            m_playerBattleStatus.DEF -= m_buffCalculation.GetBuffParam(BuffStatus.enBuff_DEF);
        }

        m_playerBattleStatus.ActionType = ActionType.enNull;
        m_isActionEnd = false;
    }

    /// <summary>
    /// プレイヤーのステータスを元に戻す
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
    /// HPの状態を設定する
    /// </summary>
    /// <param name="NowHP">現在のHP</param>
    /// <returns>HPの状態</returns>
    private ActorHPState SetHPStatus()
    {
        if(PlayerStatus.HP <= HPMIN_VALUE)
        {
            m_isActionEnd = true;       // 行動ができないので行動終了のフラグを立てる
            return ActorHPState.enDie;
        }

        if(PlayerStatus.HP <= PlayerData.playerDataList[m_myNumber].HP / 4)
        {
            return ActorHPState.enFewHP;
        }

        return ActorHPState.enMaxHP;
    }
}
