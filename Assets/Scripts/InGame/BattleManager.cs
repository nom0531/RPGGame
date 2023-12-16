using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// ターンを回す側
/// </summary>
public enum TurnStatus
{
    enPlayer,   // プレイヤー
    enEnemy,    // エネミー
}

/// <summary>
/// 操作しているキャラクター
/// </summary>
public enum OperatingPlayer
{
    enAttacker, // アタッカー
    enBuffer,   // バッファー
    enHealer,   // ヒーラー
    enNum
}

/// <summary>
/// ゲームのステータス
/// </summary>
public enum GameState
{
    enPlay,         // ゲーム中
    enBattleWin,    // バトル終了。勝利
    enBattleLose,   // バトル終了。敗北
}

public class BattleManager : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private EnemyDataBase EnemyData;
    [SerializeField]
    private LevelDataBase LevelData;
    [SerializeField, Header("参照オブジェクト")]
    private GameObject PauseCanvas;
    [SerializeField]
    private BattleButton[] BattleButton;
    [SerializeField, Tooltip("プレイヤーのアイコン")]
    private GameObject[] PlayerIcon;
    [SerializeField, Tooltip("コマンド選択中のアイコン")]
    private GameObject CommandIcon;
    [SerializeField, Header("バトル用データ"), Tooltip("生成する画像")]
    private GameObject Sprite;
    [SerializeField, Tooltip("画像を追加するオブジェクト")]
    private GameObject Content;
    [SerializeField, Header("バトルデータ"), Tooltip("ターン開始時の先行側")]
    private TurnStatus m_turnStatus = TurnStatus.enPlayer;

    private const int MAX_ENEMY_NUM = 4;                        // バトルに出現するエネミーの最大数
    private const float ADD_SIZE = 1.4f;                        // エネミーの画像の乗算サイズ

    private BattleSystem m_battleSystem;                        // バトルシステム
    private GameState m_gameState = GameState.enPlay;           // ゲームの状態
    private List<PlayerMove> m_playerMoveList;                  // プレイヤーの行動
    private List<EnemyMove> m_enemyMoveList;                    // エネミーの行動
    private OperatingPlayer m_operatingPlayer;                  // 操作しているプレイヤー
    private LockOnSystem m_lockOnSystem;                        // ロックオンシステム
    private StateAbnormalCalculation m_abnormalCalculation;     // 状態異常の計算
    private DrawStatusValue m_drawStatusValue;                  // ステータスを表示するUI
    private DrawBattleResult m_drawBattleResult;                // 演出
    private int m_turnSum = 0;                                  // 総合ターン数
    private int m_selectQuestNumber = 0;                        // 選択したクエストの番号
    private int m_enemySum = 0;                                 // エネミーの総数
    private bool m_isPause = false;                             // ポーズ画面かどうか
    private bool m_isPushDown = false;                          // ボタンが押されたかどうか

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
            // SPDのパラメータが上なら更新する
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

        // エネミーを用意する
        m_enemySum = m_battleSystem.GetRandomValue(1, MAX_ENEMY_NUM);

        // エネミーの画像を用意する
        for (int i = 0; i < m_enemySum; i++)
        {
            var sprite = Instantiate(Sprite);
            sprite.transform.SetParent(Content.transform);
            // スプライトを設定する
            int rand = m_battleSystem.GetRandomValue(0, 1, false);
            sprite.GetComponent<SpriteRenderer>().sprite = EnemyData.enemyDataList[rand].EnemySprite;
            // サイズを取得する
            var spriteRenderer = sprite.GetComponent<SpriteRenderer>();
            float width = spriteRenderer.bounds.size.x * ADD_SIZE;
            float height = spriteRenderer.bounds.size.y * ADD_SIZE;
            // サイズ、座標を調整
            sprite.transform.localScale = new Vector3(width, height, 1.0f);
            sprite.transform.localPosition = Vector3.zero;
            sprite.transform.localRotation = Quaternion.identity;
            // データを取り出すために番号を取得しておく
            // エネミーに自身の番号を教える
            sprite.GetComponent<EnemyMove>().MyNumber = EnemyData.enemyDataList[rand].EnemyNumber;

            // 図鑑に未登録なら
            if (sprite.GetComponent<EnemyMove>().GetTrueEnemyRegister(
                EnemyData.enemyDataList[rand].EnemyNumber) == false)
            {
                // 登録する
                sprite.GetComponent<EnemyMove>().SetTrueEnemyRegister(
                    EnemyData.enemyDataList[rand].EnemyNumber);
            }
        }

        // 配列を用意
        // playerMoveを人数分用意
        var playerMoveList = FindObjectsOfType<PlayerMove>();
        m_playerMoveList = new List<PlayerMove>(playerMoveList);
        m_playerMoveList.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));    // 番号順にソート

        // enemyMoveを人数分用意
        var enemyMoveList = FindObjectsOfType<EnemyMove>();
        m_enemyMoveList = new List<EnemyMove>(enemyMoveList);

        // 最初の操作キャラクターを決定する
        m_operatingPlayer = NextOperatingPlayer();

        // タスクを設定する
        GameClearTask().Forget();
        GameOverTask().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        // ポーズ処理
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

            m_isPause = !m_isPause;     // フラグを反転させる
            m_isPushDown = false;       // フラグを戻す
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

        // ゲームが終了しているなら、これより以下の処理は実行されない
        if (m_gameState != GameState.enPlay)
        {
            return;
        }

        if (PauseFlag == true)
        {
            return;
        }

        // 行動処理
        switch (m_turnStatus)
        {
            // プレイヤーのターン
            case TurnStatus.enPlayer:
                switch (m_operatingPlayer)
                {
                    case OperatingPlayer.enAttacker:
                    case OperatingPlayer.enBuffer:
                    case OperatingPlayer.enHealer:
                        PlayerAction((int)m_operatingPlayer);

                        // 再度行動可能なら
                        if (m_battleSystem.OneMore == true)
                        {
                            // ターンを渡す
                            m_playerMoveList[(int)m_operatingPlayer].ResetPlayerStatus();
                            BattleButton[(int)m_operatingPlayer].ResetStatus();
                            break;
                        }
                        break;
                }
                break;
            // エネミーのターン
            case TurnStatus.enEnemy:
                for(int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                {
                    // 死亡している際は実行しない
                    if (m_enemyMoveList[enemyNumber].ActorHPState == ActorHPState.enDie)
                    {
                        m_enemySum--;
                        m_enemyMoveList.Remove(m_enemyMoveList[enemyNumber]);
                        continue;
                    }

                    EnemyAction(enemyNumber);

                    // 再度行動可能なら
                    if (m_battleSystem.OneMore == true)
                    {
                        enemyNumber--;
                        continue;
                    }
                }
                // ターンを渡す
                m_turnStatus = TurnStatus.enPlayer;
                m_turnSum++;    // ターン数を加算
                // ステータスを初期化
                PlayerAction_ResetPlayerAction();
                PlayerAction_ResetBattleButton();
                // 次の操作キャラクターを決定、カメラを再設定する
                m_operatingPlayer = NextOperatingPlayer();
                m_lockOnSystem.ResetCinemachine();
                break;
        }
    }

    /// <summary>
    /// ゲームクリアか取得する
    /// </summary>
    private void IsGameClear()
    {
        for(int i = 0; i < m_enemyMoveList.Count; i++)
        {
            // 相手が1体でも生存しているならゲームクリアではない
            if(m_enemyMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                return;
            }
        }
        // 相手が全滅した
        m_gameState = GameState.enBattleWin;
    }

    /// <summary>
    /// ゲームオーバーか取得する
    /// </summary>
    private void IsGameOver()
    {

        for(int i = 0; i < m_playerMoveList.Count; i++)
        {
            // 1体でも生存しているならゲームオーバーではない
            if (m_playerMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                return;
            }
        }
        // 全滅した
        m_gameState = GameState.enBattleLose;
    }

    /// <summary>
    /// ゲームクリア演出のタスク
    /// </summary>
    async UniTask GameClearTask()
    {
        await UniTask.WaitUntil(() => m_gameState == GameState.enBattleWin);
        m_drawBattleResult.GameClearStaging();
    }

    /// <summary>
    /// ゲームオーバー演出のタスク
    /// </summary>
    async UniTask GameOverTask()
    {
        await UniTask.WaitUntil(() => m_gameState == GameState.enBattleLose);
        m_drawBattleResult.GameOverStaging();
    }

    /// <summary>
    /// プレイヤーの行動
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    async private void PlayerAction(int number)
    {
        Debug.Log(PlayerData.playerDataList[(int)m_operatingPlayer].PlayerName + "のターン");

        // 既に行動しているなら行動はしない
        if(m_playerMoveList[(int)m_operatingPlayer].ActionEndFlag == true)
        {
            m_operatingPlayer = NextOperatingPlayer();
            return;
        }

        for (int i = 0; i < BattleButton.Length; i++)
        {
            // もしいずれかが押されたら
            if (BattleButton[i].ButtonDown == true)
            {
                ActionType actionType = m_playerMoveList[number].NextActionType;
                int skillNumber = m_playerMoveList[number].SelectSkillNumber;
                int targetNumber = 0;

                // ガード以外のコマンド  かつ　単体攻撃なら
                if (actionType != ActionType.enGuard
                    && PlayerData.playerDataList[number].skillDataList[skillNumber].EffectRange != EffectRange.enAll)
                {
                    // ロックオンの対象を決定する
                    m_lockOnSystem.SetTargetState(PlayerData.playerDataList[number].skillDataList[skillNumber].SkillNumber, actionType);
                    m_lockOnSystem.LockOn = true;
                    // 攻撃対象が選択されたら以下の処理を実行する
                    await UniTask.WaitUntil(() => m_lockOnSystem.ButtonDown == true);

                    targetNumber = m_lockOnSystem.TargetNumber;
                    PlayerAction_Move(number, targetNumber, actionType, skillNumber);
                    Debug.Log("自分のHP : " + m_playerMoveList[number].PlayerStatus.HP);
                    Debug.Log("相手のHP : " + m_enemyMoveList[targetNumber].EnemyStatus.HP);
                    break;
                }
                // ロックオンせずにダメージ計算を行う
                m_lockOnSystem.LockOn = false;

                PlayerAction_Move(number, targetNumber, actionType, skillNumber);
                Debug.Log("自分のHP : " + m_playerMoveList[number].PlayerStatus.HP);
                Debug.Log("相手のHP : " + m_enemyMoveList[targetNumber].EnemyStatus.HP);
                break;
            }
        }
    }

    /// <summary>
    /// プレイヤーの行動
    /// </summary>
    private void PlayerAction_Move(int myNumber, int targetNumber, ActionType actionType, int skillNumber)
    {
        // 麻痺状態なら
        if (m_abnormalCalculation.Paralysis(m_playerMoveList[myNumber].ActorAbnormalState) == true)
        {
            Debug.Log($"{PlayerData.playerDataList[myNumber].PlayerName}は麻痺している");
            actionType = ActionType.enNull;
        }

        // 混乱状態なら
        if (m_abnormalCalculation.Confusion(m_playerMoveList[myNumber].ActorAbnormalState) == true)
        {
            Debug.Log($"{PlayerData.playerDataList[myNumber].PlayerName}は混乱している");
            actionType = ActionType.enAttack;
            m_playerMoveList[myNumber].Confusion = true;
        }

            switch (actionType)
        {
            case ActionType.enAttack:
                int DEF = m_enemyMoveList[targetNumber].EnemyStatus.DEF;
                // 混乱状態ならターゲットを再設定する
                if (m_playerMoveList[myNumber].Confusion == true)
                {
                    targetNumber = m_battleSystem.GetRandomValue(0, m_playerMoveList.Count);
                    DEF = m_playerMoveList[targetNumber].PlayerStatus.DEF;
                    Debug.Log($"{PlayerData.playerDataList[targetNumber].PlayerName}に攻撃");
                }
                PlayerAction_Attack(myNumber, targetNumber, DEF);
                break;
            case ActionType.enSkillAttack:
                switch (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillType)
                {
                    case SkillType.enAttack:
                        // 効果範囲が全体のとき
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
                        // 効果範囲が全体のとき
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
                        // 効果範囲が全体のとき
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
                        // 効果範囲が全体のとき
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

        // 毒状態ならHPを減らす
        int damage = m_abnormalCalculation.Poison(
            m_playerMoveList[(int)m_operatingPlayer].ActorAbnormalState, m_playerMoveList[myNumber].PlayerStatus.HP);
        m_playerMoveList[myNumber].DecrementHP(damage);

        m_playerMoveList[myNumber].gameObject.GetComponent<DrawCommandText>().SetCommandText(
            actionType, PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillNumber);
        // 行動を終了し、次のプレイヤーを選択する
        m_playerMoveList[(int)m_operatingPlayer].ActionEndFlag = true;
        m_operatingPlayer = NextOperatingPlayer();
        // ロックオンの設定を初期化・再設定する
        m_lockOnSystem.ButtonDown = false;
        m_lockOnSystem.ResetCinemachine();
    }

    /// <summary>
    /// 通常攻撃の処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    private void PlayerAction_Attack(int myNumber, int targetNumber, int DEF)
    {
        // ダメージ量を計算
        int damage = m_battleSystem.NormalAttack(
            m_playerMoveList[myNumber].PlayerStatus.ATK,// 攻撃力
            DEF                                         // 防御力
            );
        // 混乱状態なら
        if (m_playerMoveList[myNumber].Confusion)
        {
            m_playerMoveList[targetNumber].DecrementHP(damage);
            return;
        }
        // ダメージを設定する
        m_enemyMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// 攻撃タイプのスキル処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    private void PlayerAction_SkillAttack(int myNumber,int targetNumber,int skillNumber)
    {
        // ダメージ量を計算
        int damage = m_battleSystem.SkillAttack(
            m_playerMoveList[myNumber].PlayerStatus.ATK,             // 攻撃力
            PlayerData.playerDataList[myNumber].skillDataList[skillNumber].POW,     // スキルの基礎値
            m_enemyMoveList[targetNumber].EnemyStatus.DEF            // 防御力
            );

        // 無属性でないなら属性を考慮した計算を行う
        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNone
            && PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNum)
        {
            damage = m_battleSystem.EnemyElementResistance(
                EnemyData.enemyDataList[targetNumber],                                          // エネミーデータ
                skillNumber,                                                                // スキルの番号
                (int)PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement,   // スキルの属性
                damage                                                                      // ダメージ
                );

            // 既に属性耐性を発見していないなら
            if (m_enemyMoveList[targetNumber].GetTrueElementRegister
                ((int)PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement) == false)
            {
                // 発見したフラグをtureにする
                m_enemyMoveList[targetNumber].SetTrueElementRegister
                    ((int)PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement);
            }
        }

        // ダメージを設定する
        m_enemyMoveList[targetNumber].DecrementHP(damage);
        PlayerAction_Decrement(myNumber, skillNumber);
    }

    /// <summary>
    /// バフ・デバフの処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="isBuff">trueならバフ。falseならデバフ</param>
    private void PlayerAction_Buff(int myNumber, int targetNumber, int skillNumber, bool isBuff)
    {
        // パラメータを参照
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
        // 値の計算
        int value = m_battleSystem.SkillBuff(
            param,
            PlayerData.playerDataList[myNumber].skillDataList[skillNumber].POW
            );

        // 値を設定する
        m_playerMoveList[targetNumber].SetPlayerBuffStatus(
            PlayerData.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
            value,
            skillNumber,
            isBuff
            );

        PlayerAction_Decrement(myNumber, skillNumber);
    }

    /// <summary>
    /// HPを回復する処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    private void PlayerAction_HPRecover(int myNumber, int targetNumber, int skillNumber)
    {
        // 回復量を計算する
        int recverValue = m_battleSystem.SkillHeal(
                PlayerData.playerDataList[targetNumber].HP,
                PlayerData.playerDataList[myNumber].skillDataList[skillNumber].POW
                );

        // HPを回復させる
        m_playerMoveList[targetNumber].RecoverHP(recverValue);

        PlayerAction_Decrement(myNumber, skillNumber);
    }

    /// <summary>
    /// SP・HPを消費する処理
    /// </summary>
    private void PlayerAction_Decrement(int myNumer, int skillNumber)
    {
        int necessaryValue = PlayerData.playerDataList[myNumer].skillDataList[skillNumber].SkillNecessary;

        // SP・HPを消費する
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
    /// 1ターン防御する処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    private void PlayerAction_Guard(int myNumber)
    {
        // 防御力を計算
        float defensePower = m_playerMoveList[myNumber].Guard();
    }

    /// <summary>
    /// 次の操作キャラクターを決定する
    /// </summary>
    private OperatingPlayer NextOperatingPlayer()
    {
        OperatingPlayer operatingPlayer = OperatingPlayer.enAttacker;
        int SPD = int.MinValue;

        for (int i = 0; i < (int)OperatingPlayer.enNum; i++)
        {
            // 既に行動しているなら処理を実行しない
            if (m_playerMoveList[i].ActionEndFlag == true)
            {
                continue;
            }
            // ひん死なら処理を実行しない
            if (m_playerMoveList[i].ActorHPState == ActorHPState.enDie)
            {
                continue;
            }

            // SPDのパラメータが上なら更新する
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
            // 行動が終了しているならターンを渡す
            m_turnStatus = TurnStatus.enEnemy;
            m_turnSum++;    // ターン数を加算
        }

        return operatingPlayer;
    }

    /// <summary>
    /// プレイヤーのステータスをリセットする
    /// </summary>
    private void PlayerAction_ResetPlayerAction()
    {

        for (int i = 0; i < m_playerMoveList.Count; i++)
        {
            m_playerMoveList[i].ResetPlayerStatus();
        }
    }

    /// <summary>
    /// コマンドの状態をリセットする
    /// </summary>
    public void PlayerAction_ResetBattleButton()
    {
        for (int i = 0; i < BattleButton.Length; i++)
        {
            BattleButton[i].ResetStatus();
        }
    }

    /// <summary>
    /// ステータスを表示する
    /// </summary>
    private void PlayerAction_DrawStatus()
    {
        m_drawStatusValue.SetStatus();
        m_drawStatusValue.SetStatusText();
    }

    /// <summary>
    /// エネミーの行動
    /// </summary>
    private void EnemyAction(int number)
    {
        Debug.Log(EnemyData.enemyDataList[m_enemyMoveList[number].MyNumber].EnemyName + "のターン");
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
    /// エネミーの行動
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
                        // 効果範囲が全体のとき
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
                            {
                                EnemyAction_SkillAttack(myNumber, playerNumber, skillNumber);
                            }
                            break;
                        }
                        // ターゲットを選択
                        int skillTarget = m_enemyMoveList[myNumber].SelectTargetEnemy(m_enemyMoveList.Count);
                        EnemyAction_SkillAttack(myNumber, skillTarget, skillNumber);
                        break;
                    case SkillType.enBuff:
                        // 効果範囲が全体のとき
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                            {
                                EnemyAction_Buff(myNumber, skillNumber, enemyNumber, true);
                            }
                            break;
                        }
                        // ターゲットを選択
                        int buffTarget = m_enemyMoveList[myNumber].SelectTargetEnemy(m_enemyMoveList.Count);
                        EnemyAction_Buff(myNumber, skillNumber, buffTarget, true);
                        break;
                    case SkillType.enDeBuff:
                        // 効果範囲が全体のとき
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_enemyMoveList.Count; playerNumber++)
                            {
                                EnemyAction_Buff(myNumber, skillNumber, playerNumber, false);
                            }
                            break;
                        }
                        // ターゲットを選択
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
    /// 通常攻撃の処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    private void EnemyAction_Attack(int myNumber)
    {
        // ターゲットを選択
        int targetNumber = m_enemyMoveList[myNumber].SelectTargetPlayer();
        // ダメージ量を計算
        int damage = m_battleSystem.NormalAttack(
            m_enemyMoveList[targetNumber].EnemyStatus.ATK,       // 攻撃力
            m_playerMoveList[targetNumber].PlayerStatus.DEF      // 防御力
            );
        // 計算したダメージ量を設定する
        m_playerMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// スキルでの攻撃処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    private void EnemyAction_SkillAttack(int myNumber, int targetNumber, int skillNumber)
    {
        // ダメージ量を計算
        int damage = m_battleSystem.SkillAttack(
            m_enemyMoveList[targetNumber].EnemyStatus.ATK,              // パラメータ
            EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].POW,   // スキルの基礎値
            m_playerMoveList[targetNumber].PlayerStatus.DEF             // 防御力
            );

        // 無属性でないなら属性を考慮した計算を行う
        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].SkillElement != ElementType.enNone)
        {
            damage = m_battleSystem.PlayerElementResistance(
                PlayerData.playerDataList[targetNumber],                                    // プレイヤーデータ
                skillNumber,                                                            // スキルの番号
                (int)EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].SkillElement, // スキルの属性
                damage                                                                  // ダメージ
                );
        }

        // ダメージを設定する
        m_playerMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// バフ・デバフの処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="isBuff">trueならバフ。falseならデバフ</param>
    private void EnemyAction_Buff(int myNumber, int skillNumber, int targetNumber, bool isBuff)
    {
        // パラメータを参照
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

        // 値を計算
        int value = m_battleSystem.SkillBuff(
            param,
            EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].POW
            );
        // 値を設定する
        m_enemyMoveList[targetNumber].SetEnmeyBuffStatus(
            EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
            value,
            skillNumber,
            isBuff
            );
    }

    /// <summary>
    /// HPを回復する
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    private void EnemyAction_HPRecover(int myNumber, int skillNumber)
    {
        // ターゲットを選択
        int targetNumber = m_enemyMoveList[myNumber].SelectTargetEnemy(m_enemyMoveList.Count);

        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].SkillType == SkillType.enResurrection)
        {
            // オブジェクトを取得する
            GameObject gameObjct = m_enemyMoveList[myNumber].SelectTargetDieEnemy();

            // オブジェクトが存在しないなら何もしない
            if(gameObject == null)
            {
                return;
            }

            m_enemyMoveList.Add(gameObjct.GetComponent<EnemyMove>());

            gameObject.SetActive(true);
            targetNumber = (int)m_enemyMoveList.Count - 1;

            gameObject.tag = "Enemy";
        }

        // 回復量を計算
        int recoverValue = m_battleSystem.SkillHeal(
            EnemyData.enemyDataList[targetNumber].HP,
            EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].POW
            );

        // 効果範囲が全体のとき
        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
        {
            for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
            {
                // HPを回復させる
                m_enemyMoveList[enemyNumber].RecoverHP(recoverValue);
            }
        }
        // 効果範囲が単体のとき
        else
        {
            // HPを回復させる
            m_enemyMoveList[targetNumber].RecoverHP(recoverValue);
        }
    }

    /// <summary>
    /// 1ターン防御する
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    private void EnemyAction_Guard(int myNumber)
    {
        int defensePower = m_enemyMoveList[myNumber].Guard();
    }

    /// <summary>
    /// 逃走処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    private void EnemyAction_Escape(int myNumber)
    {
        // 逃走が成功したかどうか取得する
        bool isEscape = m_battleSystem.Escape(EnemyData.enemyDataList[myNumber].LUCK);

        if (isEscape == false)
        {
            return;
        }

        // 成功したなら自身を削除
        Destroy(m_enemyMoveList[myNumber].gameObject);
        m_enemyMoveList.Remove(m_enemyMoveList[myNumber]);
    }
}
