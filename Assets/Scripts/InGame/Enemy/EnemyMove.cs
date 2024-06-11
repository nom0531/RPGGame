using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// 戦闘時のエネミーのステータス
/// </summary>
public struct EnemyBattleStatus
{
    public int HP;                              // 現在のHP
    public int ATK;                             // 現在のATK
    public int DEF;                             // 現在のDEF
    public int SPD;                             // 現在のSPD
}

public class EnemyMove : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private EnemyDataBase EnemyData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField]
    private EnemyMoveDataBase EnemyMoveData;

    private const int HPMIN_VALUE = 0;              // HPの最小値

    private List<PlayerMove> m_playerMoveList;
    private SaveDataManager m_saveDataManager;
    private BattleSystem m_battleSystem;
    private BattleManager m_battleManager;
    private PauseManager m_pauseManager;
    private StagingManager m_stagingManager;
    private StateAbnormalCalculation m_abnormalCalculation;
    private BuffCalculation m_buffCalculation;
    private DrawCommandText m_drawCommandText;
    private EnemyBattleStatus m_enemyBattleStatus;                                      // 戦闘時のステータス
    private ActorHPState m_actorHPState = ActorHPState.enMaxHP;                         // HPの状態
    private ActorAbnormalState m_actorAbnormalState = ActorAbnormalState.enNormal;      // 状態異常
    private ActionType m_actionType = ActionType.enNull;                                // 次の行動
    private int m_myNumber = 0;                                                         // 自身の番号
    private int m_basicValue = 0;                                                       // ダメージ量・回復量
    private int m_defencePower = 0;                                                     // 防御力
    private int m_poisonDamage = 0;                                                     // 毒状態時のダメージ
    private bool m_isConfusion = false;                                                 // 混乱しているかどうか
    private bool m_isActionEnd = false;                                                 // 行動が終了しているかどうか
    private bool m_isWeak = false;                                                      // 弱点を突かれたかどうか

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
    /// エネミーを発見しているか取得する
    /// </summary>
    /// <param name="enemyNumber">エネミーの番号</param>
    /// <returns>trueなら発見している</returns>
    public bool GetTrueEnemyRegister(int enemyNumber)
    {
        return m_saveDataManager.SaveData.saveData.EnemyRegisters[enemyNumber];
    }

    /// <summary>
    /// エネミーを発見したフラグをtrueにする
    /// </summary>
    /// <param name="enemyNumber">エネミーの番号</param>
    public void SetTrueEnemyRegister(int enemyNumber)
    {
        m_saveDataManager.SaveData.saveData.EnemyRegisters[enemyNumber] = true;
        m_saveDataManager.Save(false);
    }

    /// <summary>
    /// 属性の耐性度を発見しているかどうか取得する
    /// </summary>
    /// <param name="elementNumber">属性番号</param>
    public bool GetTrueElementRegister(int elementNumber)
    {
        return m_saveDataManager.SaveData.saveData.ElementRegisters[m_myNumber].Elements[elementNumber];
    }

    /// <summary>
    /// 属性の耐性度を発見したフラグをtrueにする
    /// </summary>
    /// <param name="elementNumber">属性番号</param>
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
        // プレイヤーのリストを参照
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
            // 効果時間が終了しているならステータスを戻す
            ResetBuffStatus((BuffStatus)i);
            m_buffCalculation.SetEffectEndFlag((BuffStatus)i, false);
        }
    }

    /// <summary>
    /// ステータスを初期化する
    /// </summary>
    private void SetStatus()
    {
#if UNITY_EDITOR
        m_enemyBattleStatus.HP = 9999;  // デバッグ用
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
    /// 自身の子オブジェクトの座標を設定する
    /// </summary>
    private void SetLookAtPosition()
    {
        var lookAt = transform.GetChild(0).gameObject;
        lookAt.transform.position = new Vector3(transform.position.x, transform.position.y + SetPositionY(), transform.position.z);
    }

    /// <summary>
    /// Y座標を設定する
    /// </summary>
    /// <returns>加算するY座標</returns>
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
    /// ステータスをリセットする
    /// </summary>
    public void ResetStatus()
    {
        // ガードしていたなら
        if (NextActionType == ActionType.enGuard)
        {
            // 防御力を元に戻す
            m_enemyBattleStatus.DEF -= m_defencePower;
        }
        NextActionType = ActionType.enNull;
        m_isActionEnd = false;
        m_isWeak = false;
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <param name="attackedDEF">防御側の防御力</param>
    /// <returns>ダメージ量</returns>
    public void EnemyAction_Attack(int targetNumber, int attackedDEF)
    {
        // ダメージ量を計算
        BasicValue = m_battleSystem.NormalAttack(
            EnemyStatus.ATK, // 攻撃力
            attackedDEF      // 防御力
            );
        // 混乱状態なら
        if (ConfusionFlag == true)
        {
            m_battleManager.DamageEnemy(targetNumber, BasicValue);
            return;
        }
        m_battleManager.DamagePlayer(targetNumber,BasicValue);
    }

    /// <summary>
    /// スキル処理
    /// </summary>
    ///<param name="attackedDEF">防御側の防御力</param>
    ///<param name="skillNumber">スキルの番号</param>
    ///<returns>ダメージ量</returns>
    public void EnemyAction_SkillAttack(int skillNumber, int targetNumber, int attackedDEF, int playerDataNumber)
    {
        // ダメージ量を計算
        BasicValue = m_battleSystem.SkillAttack(
            EnemyStatus.ATK,                                                            // パラメータ
            EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].POW,         // スキルの基礎値
            attackedDEF                                                                 // 防御力
            );
        // 無属性でないなら属性を考慮した計算を行う
        if (EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNone
            && EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNum)
        {
            BasicValue = m_battleSystem.PlayerElementResistance(
                playerDataNumber,                                                                   // プレイヤーのデータ内での番号
                (int)EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillElement,   // スキルの属性
                BasicValue                                                                          // ダメージ
                );
        }
        AddingEffectCalculation(targetNumber, skillNumber);
        // ダメージを設定する
        m_battleManager.DamagePlayer(targetNumber, BasicValue);
    }

    /// <summary>
    /// 追加効果の計算
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    private void AddingEffectCalculation(int targetNumber, int skillNumber)
    {
        var abnormalState = EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].StateAbnormalData.ActorAbnormalState;
        // 追加効果がないなら実行しない
        if (abnormalState == ActorAbnormalState.enNormal)
        {
            return;
        }
        // 状態異常にかからなかったなら実行しない
        if (m_abnormalCalculation.SpentToStateAbnormal() != true)
        {
            return;
        }
        // ステートを変更する
        m_battleManager.EnemyAction_ChangeStatePlayer(targetNumber, abnormalState);
    }

    /// <summary>
    /// バフ・デバフ処理
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="attackedATK">防御側の攻撃力</param>
    /// <param name="attackedDEF">防御側の防御力</param>
    /// <param name="attackedSPD">防御側の素早さ</param>
    /// <param name="isBuff">trueならバフ。falseならデバフ</param>
    public int EnemyAction_Buff(int skillNumber, int attackedATK, int attackedDEF, int attackedSPD)
    {
        // パラメータを参照
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
        // 値を計算
        var value = m_battleSystem.SkillBuff(
            param,
            EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].POW
            );
        return value;
    }

    /// <summary>
    /// 回復処理
    /// </summary>
    /// <param name="attackedHP">使用される側の体力</param>
    /// <param name="skillNumber">スキルの番号</param>
    public void EnemyAction_HPRecover(int attackedHP,int skillNumber)
    {
        // 回復量を計算
        BasicValue = m_battleSystem.SkillHeal(
            attackedHP,
            EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].POW
            );
    }
    
    /// <summary>
    /// 復活処理
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <returns></returns>
    public int EnemyAction_HPResurrection(int skillNumber, int targetNumber)
    {
        // 選択したのが復活ではない場合、そのまま番号を返す
        if (EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillType != SkillType.enResurrection)
        {
            return targetNumber;
        }
        // オブジェクトを取得する
        var gameObject = SelectTargetDieEnemy();
        // オブジェクトが存在しないなら何もしない
        if (gameObject == null)
        {
            return targetNumber;
        }

        // リストに追加
        var newTargetNumber = m_battleManager.EnemyListAdd(this);
        // オブジェクトの設定を変更する
        gameObject.SetActive(true);
        gameObject.tag = "Enemy";
        return newTargetNumber;
    }

    /// <summary>
    /// 防御処理
    /// </summary>
    public void EnemyAction_Guard()
    {
        m_defencePower = m_battleSystem.Guard(m_enemyBattleStatus.DEF);
        m_enemyBattleStatus.DEF += m_defencePower;
    }

    /// <summary>
    /// 逃走処理
    /// </summary>
    public void EnemyAction_Escape()
    {
        // 逃走が成功したかどうか取得する
        if (m_battleSystem.Escape(EnemyData.enemyDataList[m_myNumber].LUCK) == false)
        {
            return;
        }
        Dissolve();
        m_battleManager.EnemyListRemove(m_myNumber);        // 成功したらリストから自身を削除
    }

    /// <summary>
    /// HPを回復する処理
    /// </summary>
    /// <param name="recoverValue">回復量</param>
    public void RecoverHP(int recoverValue)
    {
        m_enemyBattleStatus.HP += recoverValue;
        // 一定量より多く回復するなら補正
        if (m_enemyBattleStatus.HP >= EnemyData.enemyDataList[m_myNumber].HP)
        {
            // 全回復
            m_enemyBattleStatus.HP = EnemyData.enemyDataList[m_myNumber].HP;
        }
        ActorHPState = SetHPStatus();
    }

    /// <summary>
    /// HPの減少処理
    /// </summary>
    /// <param name="decrementValue">ダメージ量</param>
    public void DecrementHP(int decrementValue)
    {
        m_enemyBattleStatus.HP -= decrementValue;
        // 一定量を下回るなら補正
        if (m_enemyBattleStatus.HP <= HPMIN_VALUE)
        {
            // 死亡した
            m_enemyBattleStatus.HP = HPMIN_VALUE;
        }
        ActorHPState = SetHPStatus();
    }

    /// <summary>
    /// バフ・デバフがかかったときのステータスを変更する
    /// </summary>
    /// <param name="buffType">変更するステータス</param>
    /// <param name="statusFloatingValue">変更する値</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="isBuff">trueならバフ。falseならデバフ</param>
    public void SetBuffStatus(BuffType buffType, int statusFloatingValue, int skillNumber, bool isBuff)
    {
        var effectTime = 1;
        // スキルの番号が指定されているなら
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
    /// ターゲットプレイヤーを選択する処理
    /// </summary>
    /// <returns>プレイヤーの番号</returns>
    public int SelectTargetPlayer()
    {
        var attackNumber = m_battleSystem.GetRandomValue(0, 2);
        // ターゲットがひん死でないならここで終了
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
            attackNumber = i;   // ひん死でないプレイヤーをターゲットにする
            break;
        }
        return attackNumber;
    }

    /// <summary>
    /// ターゲットエネミーを選択する処理
    /// </summary>
    /// <param name="maxNumber">エネミーの最大数</param>
    /// <returns></returns>
    public int SelectTargetEnemy(int maxNumber)
    {
        // 0～エネミーの最大数で乱数を生成
        return m_battleSystem.GetRandomValue(0, maxNumber, false);
    }

    /// <summary>
    /// ひん死状態のエネミーの中からランダムで1体ターゲットにする処理
    /// </summary>
    /// <returns>ランダムに選択したオブジェクト</returns>
    public GameObject SelectTargetDieEnemy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("DieEnemy");
        // 0～エネミーの最大数で乱数を生成
        var rand = m_battleSystem.GetRandomValue(0, enemys.Length, false);
        return enemys[rand].gameObject;
    }

    /// <summary>
    /// 行動を選択する処理
    /// </summary>
    /// <returns>行動番号</returns>
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
            // 行動パターンを設定
            NextActionType = EnemyData.enemyDataList[m_myNumber].enemyMoveList[i].ActionType;
            break;
        }
        return NextActionType;
    }

    /// <summary>
    /// 行動を終了する
    /// </summary>
    public void ActionEnd()
    {
        m_isActionEnd = true;
        WeakFlag = false;
    }

    /// <summary>
    /// HPのステートの判定
    /// </summary>
    /// <param name="number">添え字</param>
    /// <returns>trueなら当てはまっている。falseなら当てはまっていない</returns>
    private bool DecisionHPState(int number)
    {
        // 指定がない場合は無視する
        if(EnemyData.enemyDataList[m_myNumber].enemyMoveList[number].ActorHPState == ActorHPState.enNull)
        {
            return true;
        }
        // 異なるならfalse
        if(EnemyData.enemyDataList[m_myNumber].enemyMoveList[number].ActorHPState != ActorHPState)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 状態異常の判定
    /// </summary>
    /// <param name="number">添え字</param>
    /// <returns>trueなら当てはまっている。falseなら当てはまっていない</returns>
    private bool DecisionAbnormalState(int number)
    {
        // 指定がない場合は無視する
        if(EnemyData.enemyDataList[m_myNumber].enemyMoveList[number].ActorAbnormalState == global::ActorAbnormalState.enNull)
        {
            return true;
        }
        // 異なるならfalse
        if (EnemyData.enemyDataList[m_myNumber].enemyMoveList[number].ActorAbnormalState != ActorAbnormalState)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 状態異常の計算
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
                Debug.Log($"{EnemyData.enemyDataList[m_myNumber].EnemyName}は毒を浴びている");
#endif
                PoisonDamage = m_abnormalCalculation.Poison(EnemyStatus.HP);
                break;
            case ActorAbnormalState.enParalysis:
                if (m_abnormalCalculation.Paralysis() == true)
                {
#if UNITY_EDITOR
                    Debug.Log($"{EnemyData.enemyDataList[m_myNumber].EnemyName}は麻痺している");
#endif
                    NextActionType = ActionType.enNull;
                }
                break;
            case ActorAbnormalState.enConfusion:
                if (m_abnormalCalculation.Confusion() == true)
                {
#if UNITY_EDITOR
                    Debug.Log($"{EnemyData.enemyDataList[m_myNumber].EnemyName}は混乱している");
#endif
                    NextActionType = ActionType.enAttack;
                    ConfusionFlag = true;
                }
                break;
        }
    }

    /// <summary>
    /// 使用するスキルを決定する処理
    /// </summary>
    /// <returns>スキルの番号</returns>
    public int SelectSkill()
    {
        // 0～データが持つスキルの最大数までで乱数を生成する
        return m_battleSystem.GetRandomValue(0, EnemyData.enemyDataList[m_myNumber].skillDataList.Count, false);
    }

    /// <summary>
    /// ステータスを元に戻す
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
    /// HPの状態を設定する
    /// </summary>
    /// <returns>HPの状態</returns>
    private ActorHPState SetHPStatus()
    {
        if (EnemyStatus.HP <= HPMIN_VALUE)
        {
            Die();  // 死亡処理
            return ActorHPState.enDie;
        }
        if (EnemyStatus.HP <= EnemyData.enemyDataList[m_myNumber].HP / 4)
        {
            return ActorHPState.enFewHP;
        }
        return ActorHPState.enMaxHP;
    }

    /// <summary>
    /// 死亡演出
    /// </summary>
    async private void Die()
    {
        // 演出が終了したなら以下の処理を実行する
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        Dissolve();
        tag = "DieEnemy";              // タグを変更する
    }

    /// <summary>
    /// ディソルブ処理
    /// </summary>
    private void Dissolve()
    {
        var changeMaterial = GetComponent<ChangeMaterial>();
        var material = GetComponent<SpriteRenderer>().material = changeMaterial.Change(1);
        material.SetTexture("_MainTex", EnemyData.enemyDataList[MyNumber].EnemySprite.texture);

        GetComponent<Animator>().SetTrigger("Dissolve");
    }
}
