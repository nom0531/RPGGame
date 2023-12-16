using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public ActorHPState HPState;                // HPの状態
    public ActorAbnormalState AbnormalState;    // 状態異常
    public ActionType ActionType;               // 次の行動
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

    private List<PlayerMove> m_playerMove;
    private SaveDataManager m_saveDataManager;
    private BattleSystem m_battleSystem;
    private BuffCalculation m_buffCalculation;      // バフの計算
    private DrawCommandText m_drawCommandText;      // コマンドの表示
    private EnemyBattleStatus m_enemyBattleStatus;  // 戦闘時のステータス
    private int m_myNumber = 0;                     // 自身の番号
    private bool m_isConfusion = false;             // 混乱しているかどうか

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
        get => m_enemyBattleStatus.HPState;
    }

    public ActorAbnormalState ActorAbnormalState
    {
        get => m_enemyBattleStatus.AbnormalState;
        set => m_enemyBattleStatus.AbnormalState = value;
    }

    public bool Confusion
    {
        get => m_isConfusion;
        set => m_isConfusion = value;
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
        m_saveDataManager.Save();
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
        m_saveDataManager.Save();
    }

    private void Awake()
    {
        m_saveDataManager = GameManager.Instance.SaveData;
        m_battleSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleSystem>();
        m_buffCalculation = this.gameObject.GetComponent<BuffCalculation>();
        m_drawCommandText = this.gameObject.GetComponent<DrawCommandText>();

        SetStatus();
        SetSkills();
        SetMoves();
    }

    private void Start()
    {
        // playerMoveを人数分用意
        PlayerMove[] playerMove = FindObjectsOfType<PlayerMove>();
        m_playerMove = new List<PlayerMove>(playerMove);
        // ソート
        m_playerMove.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));
    }

    private void FixedUpdate()
    {
        RotationSprite();
        m_enemyBattleStatus.HPState = SetHPStatus();
        IsStateEnDie();

        for (int i = 0; i < (int)BuffStatus.enNum; i++)
        {
            if (m_buffCalculation.GetEffectEndFlag((BuffStatus)i) == false)
            {
                continue;
            }
            // 効果時間が終了しているならステータスを戻す
            ResetEnemyBuffStatus((BuffStatus)i);
            m_buffCalculation.SetEffectEndFlag((BuffStatus)i, false);
        }
    }

    /// <summary>
    /// ステータスを初期化する
    /// </summary>
    private void SetStatus()
    {
        m_enemyBattleStatus.HP = EnemyData.enemyDataList[m_myNumber].HP;
        m_enemyBattleStatus.ATK = EnemyData.enemyDataList[m_myNumber].ATK;
        m_enemyBattleStatus.DEF = EnemyData.enemyDataList[m_myNumber].DEF;
        m_enemyBattleStatus.SPD = EnemyData.enemyDataList[m_myNumber].SPD;
        m_enemyBattleStatus.HPState = SetHPStatus();
        m_enemyBattleStatus.ActionType = ActionType.enNull;
    }

    /// <summary>
    /// スキルデータの初期化
    /// </summary>
    private void SetSkills()
    {
        for (int skillNumber = 0; skillNumber < EnemyData.enemyDataList[m_myNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // 識別番号が同じならデータを初期化する
                if (EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillNumber != SkillData.skillDataList[dataNumber].SkillNumber)
                {
                    continue;
                }
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
            }
        }
    }

    /// <summary>
    /// 行動パターンの初期化
    /// </summary>
    private void SetMoves()
    {
        for (int moveNumber = 0; moveNumber < EnemyData.enemyDataList[m_myNumber].enemyMoveList.Count; moveNumber++)
        {
            for (int dataNumber = 0; dataNumber < EnemyMoveData.enemyMoveDataList.Count; dataNumber++)
            {
                // 識別番号が同じならデータを初期化する
                if (EnemyData.enemyDataList[m_myNumber].enemyMoveList[moveNumber].MoveNumber != EnemyMoveData.enemyMoveDataList[dataNumber].MoveNumber)
                {
                    continue;
                }
                EnemyData.enemyDataList[m_myNumber].enemyMoveList[moveNumber].MoveNumber = EnemyMoveData.enemyMoveDataList[dataNumber].MoveNumber;
                EnemyData.enemyDataList[m_myNumber].enemyMoveList[moveNumber].MoveName = EnemyMoveData.enemyMoveDataList[dataNumber].MoveName;
                EnemyData.enemyDataList[m_myNumber].enemyMoveList[moveNumber].ActorHPState = EnemyMoveData.enemyMoveDataList[dataNumber].ActorHPState;
                EnemyData.enemyDataList[m_myNumber].enemyMoveList[moveNumber].ActorAbnormalState = EnemyMoveData.enemyMoveDataList[dataNumber].ActorAbnormalState;
                EnemyData.enemyDataList[m_myNumber].enemyMoveList[moveNumber].ActionType = EnemyMoveData.enemyMoveDataList[dataNumber].ActionType;
            }
        }
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

        m_enemyBattleStatus.HPState = SetHPStatus();
    }

    /// <summary>
    /// HPを減らす処理
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

        m_enemyBattleStatus.HPState = SetHPStatus();
    }

    /// <summary>
    /// 防御処理
    /// </summary>
    /// <returns></returns>
    public int Guard()
    {
        int defensePower = m_battleSystem.Guard(m_enemyBattleStatus.DEF);
        return defensePower;
    }

    /// <summary>
    /// バフ・デバフがかかったときのステータスを変更する
    /// </summary>
    /// <param name="buffType">変更するステータス</param>
    /// <param name="statusFloatingValue">変更する値</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="isBuff">trueならバフ。falseならデバフ</param>
    public void SetEnmeyBuffStatus(BuffType buffType, int statusFloatingValue, int skillNumber, bool isBuff)
    {
        int effectTime = 1;
        // スキルの番号が指定されているなら
        if (skillNumber >= 0)
        {
            effectTime = EnemyData.enemyDataList[m_myNumber].skillDataList[skillNumber].StateAbnormalData.EffectTime;
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
    /// 画像を回転させる
    /// </summary>
    private void RotationSprite()
    {
        Vector3 lookAtCamera = Camera.main.transform.position;
        lookAtCamera.y = transform.parent.gameObject.transform.position.y;
        transform.parent.gameObject.transform.LookAt(lookAtCamera);
    }

    /// <summary>
    /// 自身の状態がひん死なら
    /// </summary>
    private void IsStateEnDie()
    {
        if (m_enemyBattleStatus.HPState!= ActorHPState.enDie)
        {
            return;
        }

        gameObject.SetActive(false);   // 自身を非表示にする
        tag = "DieEnemy";              // タグを変更する
    }

    /// <summary>
    /// ターゲットプレイヤーを選択する処理
    /// </summary>
    /// <returns>プレイヤーの番号</returns>
    public int SelectTargetPlayer()
    {
        // 乱数を生成
        int attackNumber = m_battleSystem.GetRandomValue(0, 2);
        // ターゲットがひん死でないならここで終了
        if(m_playerMove[attackNumber].ActorHPState != ActorHPState.enDie)
        {
            return attackNumber;
        }

        for(int i = 0; i < m_playerMove.Count; i++)
        {
            if(m_playerMove[attackNumber].ActorHPState == ActorHPState.enDie)
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
        int rand = m_battleSystem.GetRandomValue(0, maxNumber, false);
        return rand;
    }

    /// <summary>
    /// ひん死状態のエネミーの中からランダムで1体ターゲットにする処理
    /// </summary>
    /// <returns>ランダムに選択したオブジェクト</returns>
    public GameObject SelectTargetDieEnemy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("DieEnemy");

        // 0～エネミーの最大数で乱数を生成
        int rand = m_battleSystem.GetRandomValue(0, enemys.Length, false);

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
            m_enemyBattleStatus.ActionType = EnemyData.enemyDataList[m_myNumber].enemyMoveList[i].ActionType;
            break;
        }

        return m_enemyBattleStatus.ActionType;
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
        if(EnemyData.enemyDataList[m_myNumber].enemyMoveList[number].ActorHPState != m_enemyBattleStatus.HPState)
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
        if (EnemyData.enemyDataList[m_myNumber].enemyMoveList[number].ActorAbnormalState != m_enemyBattleStatus.AbnormalState)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 使用するスキルを決定する処理
    /// </summary>
    /// <returns>スキルの番号</returns>
    public int SelectSkill()
    {
        // 0～データが持つスキルの最大数までで乱数を生成する
        int skillNumber = m_battleSystem.GetRandomValue(0, EnemyData.enemyDataList[m_myNumber].skillDataList.Count, false);

        return skillNumber;
    }

    /// <summary>
    /// プレイヤーのステータスを元に戻す
    /// </summary>
    private void ResetEnemyBuffStatus(BuffStatus buffStatus)
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
    /// <param name="NowHP">現在のHP</param>
    /// <returns>HPの状態</returns>
    private ActorHPState SetHPStatus()
    {
        if (EnemyStatus.HP <= HPMIN_VALUE)
        {
            return ActorHPState.enDie;
        }

        if (EnemyStatus.HP <= EnemyData.enemyDataList[m_myNumber].HP / 4)
        {
            return ActorHPState.enFewHP;
        }

        return ActorHPState.enMaxHP;
    }
}
