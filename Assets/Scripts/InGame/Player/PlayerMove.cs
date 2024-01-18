using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

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
}

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private EnemyDataBase EnemyData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField,Header("自身の番号")]
    private int m_myNumber = 0;

    private const int HPMIN_VALUE = 0;              // HPの最小値
    private const int SPMIN_VALUE = 0;              // SPの最小値

    private BattleManager m_battleManager;
    private BattleSystem m_battleSystem;
    private StagingManager m_stagingManager;
    private StateAbnormalCalculation m_abnormalCalculation;
    private BuffCalculation m_buffCalculation;
    private DrawCommandText m_drawCommandText;
    private PlayerBattleStatus m_playerBattleStatus;                                // プレイヤーのステータス
    private ActorHPState m_actorHPState = ActorHPState.enMaxHP;                     // HPの状態
    private ActorAbnormalState m_actorAbnormalState = ActorAbnormalState.enNormal;  // 状態異常
    private ActionType m_actionType = ActionType.enNull;                            // 次の行動
    private int m_selectSkillNumber = 0;                                            // 選択しているスキルの番号
    private int m_basicValue = 0;                                                   // ダメージ量・回復量
    private int m_defencePower = 0;                                                 // 防御力
    private int m_poisonDamage = 0;                                                 // 毒状態時のダメージ
    private bool m_isActionEnd = false;                                             // 行動が終了しているかどうか
    private bool m_isConfusion = false;                                             // 混乱しているかどうか

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
        m_abnormalCalculation = gameObject.GetComponent<StateAbnormalCalculation>();
        m_buffCalculation = gameObject.GetComponent<BuffCalculation>();
        m_drawCommandText = gameObject.GetComponent<DrawCommandText>();
        SetStatus();
    }
    private void Start()
    {
        SetSkills();
    }

    /// <summary>
    /// ステータスを初期化する
    /// </summary>
    private void SetStatus()
    {
        var saveDataManager = GameManager.Instance.SaveData;

        m_playerBattleStatus.HP = saveDataManager.SaveData.saveData.PlayerList[m_myNumber].HP;
        m_playerBattleStatus.SP = saveDataManager.SaveData.saveData.PlayerList[m_myNumber].SP;
        m_playerBattleStatus.ATK = saveDataManager.SaveData.saveData.PlayerList[m_myNumber].ATK;
        m_playerBattleStatus.DEF = saveDataManager.SaveData.saveData.PlayerList[m_myNumber].DEF;
        m_playerBattleStatus.SPD = saveDataManager.SaveData.saveData.PlayerList[m_myNumber].SPD;
        m_playerBattleStatus.LUCK = saveDataManager.SaveData.saveData.PlayerList[m_myNumber].LUCK;
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
        ActorHPState = SetHPStatus();

        for (int i = 0; i < (int)BuffStatus.enNum; i++)
        {
            if (m_buffCalculation.GetEffectEndFlag((BuffStatus)i) == false)
            {
                continue;
            }
            // 効果時間が終了しているならステータスを戻す
            ResetBuffStatus((BuffStatus)i);
            m_buffCalculation.SetEffectEndFlag((BuffStatus)i, false);
        }
    }

    /// <summary>
    /// 通常攻撃の処理
    /// </summary>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="attackedDEF">防御側の防御力</param>
    public void PlayerAction_Attack(int targetNumber, int attackedDEF)
    {
        // ダメージ量を計算
        BasicValue = m_battleSystem.NormalAttack(
            PlayerStatus.ATK,// 攻撃力
            attackedDEF                                         // 防御力
            );

        // 混乱状態なら
        if (ConfusionFlag == true)
        {
            DecrementHP(BasicValue);
            return;
        }
        m_battleManager.PlayerAction_DamageEnemy(targetNumber, BasicValue);
    }

    /// <summary>
    /// 攻撃タイプのスキル処理
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="attackedDEF">防御側の防御力</param>
    public void PlayerAction_SkillAttack(int skillNumber, int targetNumber, int attackedDEF, int enemyDataNumber)
    {
        // ダメージ量を計算
        BasicValue = m_battleSystem.SkillAttack(
            PlayerStatus.ATK,                                                           // 攻撃力
            PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].POW,       // スキルの基礎値
            attackedDEF                                                                 // 防御力
            );
        // 無属性でないなら属性を考慮した計算を行う
        if (PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNone
            && PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNum)
        {
            BasicValue = m_battleSystem.EnemyElementResistance(
                enemyDataNumber,                                                                    // エネミーのデータ内での番号
                (int)PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillElement, // スキルの属性
                BasicValue                                                                          // ダメージ
                );
            // 属性耐性の登録を行う
            m_battleManager.PlayerAction_Register(m_myNumber, skillNumber, targetNumber);
        }
        AddingEffectCalculation(targetNumber, skillNumber);
        // ダメージを設定する
        m_battleManager.PlayerAction_DamageEnemy(targetNumber, BasicValue);
        PlayerAction_Decrement(skillNumber);
    }

    /// <summary>
    /// 追加効果の計算
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    private void AddingEffectCalculation(int targetNumber,int skillNumber)
    {
        var abnormalState = PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].StateAbnormalData.ActorAbnormalState;
        // 追加効果がないなら実行しない
        if (abnormalState == ActorAbnormalState.enNormal)
        {
            return;
        }
        // 状態異常にかからなかったなら実行しない
        if(m_abnormalCalculation.SpentToStateAbnormal() != true)
        {
            return;
        }

        // ステートを変更する
        m_battleManager.PlayerAction_ChangeStateEnemy(targetNumber,abnormalState);
    }

    /// <summary>
    /// バフ・デバフの処理
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="attackedATK">防御側の攻撃力</param>
    /// <param name="attackDEF">防御側の防御力</param>
    /// <param name="attackSPD">防御側の素早さ</param>
    /// <param name="isBuff">trueならバフ。falseならデバフ</param>
    public void PlayerAction_Buff(int skillNumber, int attackedATK, int attackDEF, int attackSPD, bool isBuff)
    {
        // パラメータを参照
        var param = 0;
        switch (PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].BuffType)
        {
            case BuffType.enATK:
                param = attackedATK;
                break;
            case BuffType.enDEF:
                param = attackDEF;
                break;
            case BuffType.enSPD:
                param = attackSPD;
                break;
        }
        // 値の計算
        var value = m_battleSystem.SkillBuff(
            param,
            PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].POW
            );
        // 値を設定する
        SetBuffStatus(
            PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].BuffType,
            value,
            skillNumber,
            isBuff
            );
        PlayerAction_Decrement(skillNumber);
    }

    /// <summary>
    /// HPを回復する処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    public void PlayerAction_HPRecover(int targetNumber, int skillNumber)
    {
        // 回復量を計算する
        BasicValue = m_battleSystem.SkillHeal(
                PlayerData.playerDataList[targetNumber].HP,
                PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].POW
                );

        // HPを回復させる
        RecoverHP(BasicValue);
        PlayerAction_Decrement(skillNumber);
    }

    /// <summary>
    /// SP・HPを消費する処理
    /// </summary>
    private void PlayerAction_Decrement(int skillNumber)
    {
        // 値を計算する
        var necessaryValue = PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillNecessary;
        // SP・HPを消費する
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
    /// 防御処理
    /// </summary>
    public void PlayerAction_Guard()
    {
        // 防御力を計算
        m_defencePower = m_battleSystem.Guard(m_playerBattleStatus.DEF);
        m_playerBattleStatus.DEF += m_defencePower;
    }

    /// <summary>
    /// HPの回復処理
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
        ActorHPState = SetHPStatus();
    }

    /// <summary>
    /// HPの減少処理
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
        ActorHPState = SetHPStatus();
    }

    /// <summary>
    /// SPの減少処理
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
    /// バフ・デバフがかかったときのステータスを変更する
    /// </summary>
    /// <param name="buffType">変更するステータス</param>
    /// <param name="statusFloatingValue">変更する値</param>
    /// <param name="skillNumber">スキルの番号</param>
    ///  <param name="isBuff">trueならバフ。falseならデバフ</param>
    private void SetBuffStatus(BuffType buffType, int statusFloatingValue,int skillNumber, bool isBuff)
    {
        var effectTime = 1;
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
        var lookAtCamera = Camera.main.transform.position;
        lookAtCamera.y = transform.position.y;  // 補正
        transform.LookAt(lookAtCamera);
    }
    
    /// <summary>
    /// 自身の行動を終了する
    /// </summary>
    /// <param name="actionType">行動パターン</param>
    /// <param name="skillNumber">スキルの番号</param>
    public void ActionEnd(ActionType actionType, int skillNumber)
    {
        m_drawCommandText.SetCommandText(actionType, PlayerData.playerDataList[m_myNumber].skillDataList[skillNumber].SkillNumber);
        ActionEndFlag = true;
    }

    /// <summary>
    /// プレイヤーの行動をリセットする
    /// </summary>
    public void ResetStatus()
    {
        // ガードしていたなら
        if (NextActionType == ActionType.enGuard)
        {
            // 防御力を元に戻す
            m_playerBattleStatus.DEF -= m_defencePower;
        }

        NextActionType = ActionType.enNull;
        m_isActionEnd = false;
    }

    /// <summary>
    /// ステータスを元に戻す
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
    /// HPの状態を設定する
    /// </summary>
    /// <returns>HPの状態</returns>
    private ActorHPState SetHPStatus()
    {
        if(PlayerStatus.HP <= HPMIN_VALUE)
        {
            Die();
            return ActorHPState.enDie;
        }

        if(PlayerStatus.HP <= PlayerData.playerDataList[m_myNumber].HP / 4)
        {
            return ActorHPState.enFewHP;
        }

        return ActorHPState.enMaxHP;
    }

    /// <summary>
    /// 状態異常の計算
    /// </summary>
    public void CalculationAbnormalState()
    {
        if(ActorAbnormalState == ActorAbnormalState.enNormal || ActorAbnormalState == ActorAbnormalState.enSilence)
        {
            return;
        }
        if (m_abnormalCalculation.RecoverToAbnormal(ActorAbnormalState) == true)
        {
            return;
        }
        switch (ActorAbnormalState)
        {
            case ActorAbnormalState.enPoison:
                Debug.Log($"{PlayerData.playerDataList[m_myNumber].PlayerName}は毒を浴びている");
                PoisonDamage = m_abnormalCalculation.Poison(PlayerStatus.HP);
                break;
            case ActorAbnormalState.enParalysis:
                if (m_abnormalCalculation.Paralysis() == true)
                {
                    Debug.Log($"{PlayerData.playerDataList[m_myNumber].PlayerName}は麻痺している");
                    NextActionType = ActionType.enNull;
                }
                break;
            case ActorAbnormalState.enConfusion:
                if (m_abnormalCalculation.Confusion() == true)
                {
                    Debug.Log($"{PlayerData.playerDataList[m_myNumber].PlayerName}は混乱している");
                    NextActionType = ActionType.enAttack;
                    ConfusionFlag = true;
                }
                break;
        }
    }

    /// <summary>
    /// 死亡演出
    /// </summary>
    async private void Die()
    {
        // 演出が終了したら実行する
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StangingState.enStangingEnd);
        m_isActionEnd = true;       // 行動ができないので行動終了のフラグを立てる
        tag = "DiePlayer";
    }
}
