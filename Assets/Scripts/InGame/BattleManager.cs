using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// ターンを回す側
/// </summary>
public enum TurnStatus
{
    enPlayer,   // プレイヤー
    enEnemy,    // エネミー
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
    [SerializeField, Header("バトルデータ"), Tooltip("生成する画像")]
    private GameObject Sprite;
    [SerializeField, Tooltip("画像を追加するオブジェクト")]
    private GameObject Content;
    [SerializeField, Tooltip("ターン開始時の先行側")]
    private TurnStatus m_turnStatus = TurnStatus.enPlayer;
    [SerializeField, Tooltip("エネミーのサイズ")]
    private float EnemySpriteSize = 450.0f;
    [SerializeField, Header("リザルトデータ"), Tooltip("UIを表示するまでの待機時間")]
    private float WaitTime = 1.0f;

    private const int MAX_ENEMY_NUM = 4;                        // バトルに出現するエネミーの最大数
    private const float ADD_SIZE = 1.6f;                        // エネミーの画像の乗算サイズ

    private GameState m_gameState = GameState.enPlay;           // ゲームの状態
    private OperatingPlayer m_operatingPlayer;                  // 操作しているプレイヤー
    private BattleSystem m_battleSystem;                        // バトルシステム
    private StagingManager m_stagingManager;                    // 演出用システム
    private LockOnSystem m_lockOnSystem;                        // ロックオンシステム
    private DrawStatusValue m_drawStatusValue;                  // ステータスを表示するUI
    private DrawBattleResult m_drawBattleResult;                // 演出
    private List<PlayerMove> m_playerMoveList;                  // プレイヤーの行動
    private List<EnemyMove> m_enemyMoveList;                    // エネミーの行動
    private int m_turnSum = 1;                                  // 総合ターン数
    private int m_enemySum = 0;                                 // エネミーの総数
    private int m_enemyNumber = 0;                              // エネミーの番号
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
            // SPDのパラメータが上なら更新する
            if (PlayerData.playerDataList[i].SPD >= SPD)
            {
                SPD = PlayerData.playerDataList[i].SPD;
                m_operatingPlayer = (OperatingPlayer)i;
            }
        }
        var playerMoveList = FindObjectsOfType<PlayerMove>();
        m_playerMoveList = new List<PlayerMove>(playerMoveList);
        m_playerMoveList.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));    // 番号順にソート
    }

    private void Start()
    {
        DrawStatus();
        var levelNumber = GameManager.Instance.LevelNumber;
        // エネミーを用意する
        m_enemySum = m_battleSystem.GetRandomValue(1, MAX_ENEMY_NUM);
        // エネミーの画像を用意する
        for (int i = 0; i < m_enemySum; i++)
        {
            var sprite = Instantiate(Sprite);
            sprite.transform.SetParent(Content.transform);
            // スプライトを設定する
            var rand = m_battleSystem.GetRandomValue(0, LevelData.levelDataList[levelNumber].enemyDataList.Count, false);
            var number = LevelData.levelDataList[levelNumber].enemyDataList[rand].ID;
            sprite.GetComponent<SpriteRenderer>().sprite = EnemyData.enemyDataList[number].EnemySprite;
            // エネミーに自身の番号を教える
            var enemyMove = sprite.GetComponent<EnemyMove>();
            enemyMove.MyNumber = EnemyData.enemyDataList[number].ID;
            // サイズ、座標を調整
            var width = EnemySpriteSize * ADD_SIZE;
            var height = EnemySpriteSize * ADD_SIZE;
            sprite.transform.localScale = new Vector3(width, height, 1.0f);
            sprite.transform.localPosition = Vector3.zero;
            sprite.transform.localRotation = Quaternion.identity;
            // 図鑑に未登録なら
            if (enemyMove.GetTrueEnemyRegister(enemyMove.MyNumber) == false)
            {
                // 登録する
                enemyMove.SetTrueEnemyRegister(enemyMove.MyNumber);
            }
        }
        var enemyMoveList = FindObjectsOfType<EnemyMove>();
        m_enemyMoveList = new List<EnemyMove>(enemyMoveList);
        // 最初の操作キャラクターを決定する
        m_operatingPlayer = NextOperatingPlayer();
        m_enemyNumber = 0;
        // タスクを設定する
        GameClearTask().Forget();
        GameOverTask().Forget();
    }

    // Update is called once per frame
    private void Update()
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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_playerMoveList[0].DecrementHP(999);
            m_playerMoveList[1].DecrementHP(999);
            m_playerMoveList[2].DecrementHP(999);
        }
    }

    private void FixedUpdate()
    {
        Debug.Log($"現在のターン数：{m_turnSum}ターン目");
        IsGameClear();
        IsGameOver();

        // ゲームが終了しているなら、これより以下の処理は実行されない
        if (m_gameState != GameState.enPlay)
        {
            return;
        }
        // ポーズしているなら実行しない
        if (PauseFlag == true)
        {
            return;
        }
        // 演出が開始されたなら実行しない
        if(m_stagingManager.StangingState == StagingState.enStangingStart)
        {
            return;
        }
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
                            m_playerMoveList[(int)m_operatingPlayer].ResetStatus();
                            BattleButton[(int)m_operatingPlayer].ResetStatus();
                            break;
                        }
                        break;
                }
                break;
            // エネミーのターン
            case TurnStatus.enEnemy:
                for(int enemyNumber = m_enemyNumber; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                {
                    // 死亡している際は実行しない
                    if (m_enemyMoveList[enemyNumber].ActorHPState == ActorHPState.enDie)
                    {
                        continue;
                    }
                    EnemyAction(enemyNumber);
                    // 再度行動可能なら
                    if (m_battleSystem.OneMore == true)
                    {
                        // ターンを渡す
                        m_enemyMoveList[enemyNumber].ResetStatus();
                        enemyNumber--;
                        continue;
                    }
                }
                m_enemyNumber++;
                break;
        }
        DrawStatus();
        IsTurnEnd();
    }

    /// <summary>
    /// ターンを終了しているか判定する
    /// </summary>
    private void IsTurnEnd()
    {
        // 全員の行動が終了していないなら実行しない
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
    /// 場のステータスをリセットして、次のターンに移行する
    /// </summary>
    private void TurnEnd()
    {
        // 次の操作キャラクターを決定、カメラを再設定する
        m_operatingPlayer = NextOperatingPlayer();
        m_lockOnSystem.ResetCinemachine();
        m_enemyNumber = 0;
        // ターンを渡す
        m_turnStatus = TurnStatus.enPlayer;
        m_turnSum++;
        // ステータスを初期化
        ResetPlayerAction();
        ResetEnemyAction();
        ResetBattleButton();
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
        m_gameState = GameState.enBattleLose;
    }
    
    /// <summary>
    /// ゲームクリア演出のタスク
    /// </summary>
    async UniTask GameClearTask()
    {
        // 演出が終了したなら以下の処理を実行する
        await UniTask.WaitUntil(() => m_gameState == GameState.enBattleWin);
        await UniTask.Delay(TimeSpan.FromSeconds(WaitTime));
        m_drawBattleResult.GameClearStaging();
    }

    /// <summary>
    /// ゲームオーバー演出のタスク
    /// </summary>
    async UniTask GameOverTask()
    {
        // 演出が終了したなら以下の処理を実行する
        await UniTask.WaitUntil(() => m_gameState == GameState.enBattleLose);
        await UniTask.Delay(TimeSpan.FromSeconds(WaitTime));
        m_drawBattleResult.GameOverStaging();
    }

    /// <summary>
    /// プレイヤーの行動
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    async private void PlayerAction(int myNumber)
    {
        Debug.Log(PlayerData.playerDataList[(int)m_operatingPlayer].PlayerName + "のターン");
        // 演出が開始されたなら実行しない
        if (m_stagingManager.StangingState == StagingState.enStangingStart)
        {
            return;
        }
        // 既に行動しているなら行動はしない
        if (m_playerMoveList[(int)m_operatingPlayer].ActionEndFlag == true)
        {
            m_operatingPlayer = NextOperatingPlayer();
            return;
        }

        for (int i = 0; i < BattleButton.Length; i++)
        {
            // もしいずれかのボタンが押されたら以下の処理を実行する
            if (BattleButton[i].ButtonDown == true)
            {
                var skillNumber = m_playerMoveList[myNumber].SelectSkillNumber;
                var targetNumber = 0;

                // ガード以外のコマンド  かつ　単体攻撃なら
                if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange != EffectRange.enAll
                    && m_playerMoveList[myNumber].NextActionType != ActionType.enGuard)
                {
                    m_lockOnSystem.SetTargetState(PlayerData.playerDataList[myNumber].skillDataList[skillNumber].ID,
                        m_playerMoveList[myNumber].NextActionType);
                    // 攻撃対象が選択されたら以下の処理を実行する
                    await UniTask.WaitUntil(() => m_lockOnSystem.ButtonDown == true);
                    // 対象を再設定する
                    skillNumber = m_playerMoveList[myNumber].SelectSkillNumber;
                    targetNumber = m_lockOnSystem.TargetNumber;
                }
                PlayerAction_Move(myNumber, skillNumber, targetNumber);
            }
        }
    }

    /// <summary>
    /// プレイヤーの行動
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    async private void PlayerAction_Move(int myNumber, int skillNumber, int targetNumber)
    {
        m_playerMoveList[myNumber].CalculationAbnormalState();
        PlayerAction_Command(myNumber, targetNumber, m_playerMoveList[myNumber].NextActionType, skillNumber);
        // 演出を開始する
        m_stagingManager.ActionType = m_playerMoveList[myNumber].NextActionType;
        m_stagingManager.RegistrationTargets(m_turnStatus, targetNumber, myNumber, PlayerData.playerDataList[myNumber].skillDataList[skillNumber].ID, 
            PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange);
        // 行動を終了する
        m_playerMoveList[myNumber].ActionEnd(m_playerMoveList[myNumber].NextActionType, skillNumber);
        // 演出が終了したなら以下の処理を実行する
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        m_playerMoveList[myNumber].DecrementHP(m_playerMoveList[myNumber].PoisonDamage);
        // 次のプレイヤーを設定する
        m_operatingPlayer = NextOperatingPlayer();
        // ロックオンの設定を初期化・再設定する
        m_lockOnSystem.ButtonDown = false;
        m_lockOnSystem.ResetCinemachine();
    }

    /// <summary>
    /// 行動処理
    /// </summary>
    private void PlayerAction_Command(int myNumber, int targetNumber, ActionType actionType, int skillNumber)
    {
        // 既に行動しているなら実行しない
        if (m_playerMoveList[myNumber].ActionEndFlag == true)
        {
            return;
        }
        // 行動
        switch (actionType)
        {
            case ActionType.enAttack:
                var DEF = m_enemyMoveList[targetNumber].EnemyStatus.DEF;
                // 混乱状態ならターゲットを再設定する
                if (m_playerMoveList[myNumber].ConfusionFlag == true)
                {
                    targetNumber = m_battleSystem.GetRandomValue(0, m_playerMoveList.Count);
                    DEF = m_playerMoveList[targetNumber].PlayerStatus.DEF;
                    Debug.Log($"{PlayerData.playerDataList[targetNumber].PlayerName}に攻撃");
                }
                m_playerMoveList[myNumber].PlayerAction_Attack(targetNumber, DEF);
                break;
            case ActionType.enSkillAttack:
                var value = 0;
                switch (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillType)
                {
                    case SkillType.enAttack:
                        // 効果範囲が全体のとき
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_enemySum; enemyNumber++)
                            {
                                m_playerMoveList[myNumber].PlayerAction_SkillAttack(
                                    skillNumber,                                        // スキルの番号
                                    enemyNumber,                                        // ターゲットの番号
                                    m_enemyMoveList[enemyNumber].EnemyStatus.DEF,       // 防御力
                                    m_enemyMoveList[enemyNumber].MyNumber               // エネミーの番号
                                    );
                            }
                            break;
                        }
                        m_playerMoveList[myNumber].PlayerAction_SkillAttack(
                            skillNumber,                                                // スキルの番号
                            targetNumber,                                               // ターゲットの番号
                            m_enemyMoveList[targetNumber].EnemyStatus.DEF,              // 防御力
                            m_enemyMoveList[targetNumber].MyNumber                      // エネミーの番号
                            );
                        break;
                    case SkillType.enBuff:
                        // 効果範囲が全体のとき
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
                            {
                                value = m_playerMoveList[myNumber].PlayerAction_Buff(
                                    skillNumber,                                        // スキルの番号
                                    m_playerMoveList[playerNumber].PlayerStatus.ATK,      // 攻撃力
                                    m_playerMoveList[playerNumber].PlayerStatus.DEF,      // 防御力
                                    m_playerMoveList[playerNumber].PlayerStatus.SPD       // 素早さ
                                    );
                                // 値を設定する
                                m_playerMoveList[targetNumber].SetBuffStatus(
                                    PlayerData.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    true);
                            }
                            break;
                        }
                        value =  m_playerMoveList[myNumber].PlayerAction_Buff(
                            skillNumber,                                                // スキルの番号
                            m_playerMoveList[targetNumber].PlayerStatus.ATK,              // 攻撃力
                            m_playerMoveList[targetNumber].PlayerStatus.DEF,              // 防御力
                            m_playerMoveList[targetNumber].PlayerStatus.SPD               // 素早さ
                            );
                        // 値を設定する
                        m_playerMoveList[targetNumber].SetBuffStatus(
                            PlayerData.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            true);
                        break;
                    case SkillType.enDeBuff:
                        // 効果範囲が全体のとき
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                            {
                                value = m_playerMoveList[myNumber].PlayerAction_Buff(
                                    skillNumber,                                        // スキルの番号
                                    m_enemyMoveList[enemyNumber].EnemyStatus.ATK,       // 攻撃力
                                    m_enemyMoveList[enemyNumber].EnemyStatus.DEF,       // 防御力
                                    m_enemyMoveList[enemyNumber].EnemyStatus.SPD        // 素早さ
                                    );
                                // 値を設定する
                                m_enemyMoveList[enemyNumber].SetBuffStatus(
                                    PlayerData.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    false);
                            }
                            break;
                        }
                        value = m_playerMoveList[myNumber].PlayerAction_Buff(
                            skillNumber,                                                // スキルの番号
                            m_enemyMoveList[targetNumber].EnemyStatus.ATK,              // 攻撃力
                            m_enemyMoveList[targetNumber].EnemyStatus.DEF,              // 防御力
                            m_enemyMoveList[targetNumber].EnemyStatus.SPD               // 素早さ
                            );
                        // 値を設定する
                        m_enemyMoveList[targetNumber].SetBuffStatus(
                            PlayerData.playerDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            false);
                        break;
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                        // 効果範囲が全体のとき
                        if (PlayerData.playerDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
                            {
                                m_playerMoveList[myNumber].PlayerAction_HPRecover(playerNumber, skillNumber);
                                m_playerMoveList[playerNumber].RecoverHP(m_playerMoveList[myNumber].BasicValue);
                            }
                            break;
                        }
                        m_playerMoveList[myNumber].PlayerAction_HPRecover(targetNumber, skillNumber);
                        m_playerMoveList[targetNumber].RecoverHP(m_playerMoveList[myNumber].BasicValue);
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
    /// エネミーにダメージを与える
    /// </summary>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="damage">ダメージ量</param>
    public void DamageEnemy(int targetNumber, int damage)
    {
        m_enemyMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// エネミーの状態を変更する
    /// </summary>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="actorAbnormalState">変更先の状態</param>
    public void PlayerAction_ChangeStateEnemy(int targetNumber, ActorAbnormalState actorAbnormalState)
    {
        m_enemyMoveList[targetNumber].ActorAbnormalState = actorAbnormalState;
    }

    /// <summary>
    /// 属性耐性の登録処理
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="targetNumber">ターゲットの番号</param>
    public void PlayerAction_Register(int myNumber, int skillNumber, int targetNumber)
    {
        // 既に属性耐性を発見しているなら実行しない
        if (m_enemyMoveList[targetNumber].GetTrueElementRegister
            ((int)PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement) != true)
        {
            return;
        }
        // フラグをtureにする
        m_enemyMoveList[targetNumber].SetTrueElementRegister
            ((int)PlayerData.playerDataList[myNumber].skillDataList[skillNumber].SkillElement);
    }

    /// <summary>
    /// 次の操作キャラクターを決定する
    /// </summary>
    private OperatingPlayer NextOperatingPlayer()
    {
        var operatingPlayer = OperatingPlayer.enAttacker;
        var SPD = int.MinValue;

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
            // SPDのパラメータが上なら更新
            if (PlayerData.playerDataList[i].SPD >= SPD)
            {
                SPD = PlayerData.playerDataList[i].SPD;
                operatingPlayer = (OperatingPlayer)i;
            }
        }
        // 行動が終了しているならターンを渡す
        if (m_playerMoveList[0].ActionEndFlag == true
        && m_playerMoveList[1].ActionEndFlag == true
        && m_playerMoveList[2].ActionEndFlag == true)
        {
            m_turnStatus = TurnStatus.enEnemy;
        }

        return operatingPlayer;
    }

    /// <summary>
    /// プレイヤーのステータスをリセットする
    /// </summary>
    private void ResetPlayerAction()
    {
        for (int i = 0; i < m_playerMoveList.Count; i++)
        {
            m_playerMoveList[i].ResetStatus();
        }
    }

    /// <summary>
    /// コマンドの状態をリセットする
    /// </summary>
    public void ResetBattleButton()
    {
        for (int i = 0; i < BattleButton.Length; i++)
        {
            BattleButton[i].ResetStatus();
        }
    }

    /// <summary>
    /// ステータスを表示する
    /// </summary>
    private void DrawStatus()
    {
        m_drawStatusValue.SetStatus();
        m_drawStatusValue.SetStatusText();
    }

    /// <summary>
    /// エネミーの行動
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    private void EnemyAction(int myNumber)
    {
        Debug.Log(EnemyData.enemyDataList[m_enemyMoveList[myNumber].MyNumber].EnemyName + "のターン");

        // 演出が開始されたなら実行しない
        if (m_stagingManager.StangingState == StagingState.enStangingStart)
        {
            return;
        }
        // ひん死なら実行しない
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
    /// エネミーの行動
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    /// <param name="actionType">行動パターン</param>
    /// <param name="skillNumber">スキルの番号</param>
    async private void EnemyAction_Move(int myNumber, ActionType actionType, int skillNumber)
    {
        m_enemyMoveList[myNumber].CalculationAbnormalState();
        // ターゲットの番号を取得する
        var targetNumber = m_enemyMoveList[myNumber].SelectTargetPlayer();
        EnemyAction_Command(myNumber, actionType, skillNumber, targetNumber);
        // 演出を開始する
        m_stagingManager.ActionType = actionType;
        m_stagingManager.RegistrationTargets(m_turnStatus, targetNumber, myNumber);
        m_enemyMoveList[myNumber].ActionEnd(actionType, skillNumber);
        // 演出が終了したなら以下の処理を実行する
        await UniTask.WaitUntil(() => m_stagingManager.StangingState == StagingState.enStangingEnd);
        // 毒状態時のダメージを与える
        m_enemyMoveList[myNumber].DecrementHP(m_enemyMoveList[myNumber].PoisonDamage);
        m_drawStatusValue.SetStatus();
    }

    /// <summary>
    /// 行動処理
    /// </summary>
    private void EnemyAction_Command(int myNumber, ActionType actionType, int skillNumber, int targetNumber)
    {
        // 既に行動しているなら実行しない
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
                var value = 0;
                switch (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].SkillType)
                {
                    // タイプ：攻撃
                    case SkillType.enAttack:
                        // 効果範囲が全体のとき
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_playerMoveList.Count; playerNumber++)
                            {
                                m_enemyMoveList[myNumber].EnemyAction_SkillAttack(
                                    skillNumber,                                                            // スキルの番号
                                    playerNumber,                                                           // ターゲットの番号
                                    m_playerMoveList[playerNumber].PlayerStatus.DEF,                        // 防御力
                                    m_playerMoveList[playerNumber].MyNumber                                 // プレイヤーの番号
                                    );
                            }
                            break;
                        }
                        m_enemyMoveList[myNumber].EnemyAction_SkillAttack(
                            skillNumber,                                                                    // スキルの番号
                            targetNumber,                                                           // ターゲットの番号
                            m_playerMoveList[targetNumber].PlayerStatus.DEF,                                // 防御力
                            m_playerMoveList[targetNumber].MyNumber                                         // プレイヤーの番号
                            );
                        break;
                    // タイプ：バフ
                    case SkillType.enBuff:
                        // 効果範囲が全体のとき
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                            {
                                value = m_enemyMoveList[myNumber].EnemyAction_Buff(
                                    skillNumber,                                                            // スキルの番号
                                    m_enemyMoveList[enemyNumber].EnemyStatus.ATK,                           // 攻撃力
                                    m_enemyMoveList[enemyNumber].EnemyStatus.DEF,                           // 防御力
                                    m_enemyMoveList[enemyNumber].EnemyStatus.SPD                            // 素早さ
                                    );
                                m_enemyMoveList[enemyNumber].SetBuffStatus(
                                    EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    true);
                            }
                            break;
                        }
                        // ターゲットを再選択
                        targetNumber = m_enemyMoveList[myNumber].SelectTargetEnemy(m_enemyMoveList.Count);
                        value = m_enemyMoveList[myNumber].EnemyAction_Buff(
                            skillNumber,                                                                    // スキルの番号
                            m_enemyMoveList[targetNumber].EnemyStatus.ATK,                                  // 攻撃力
                            m_enemyMoveList[targetNumber].EnemyStatus.DEF,                                  // 防御力
                            m_enemyMoveList[targetNumber].EnemyStatus.SPD                                   // 素早さ
                            );
                        m_enemyMoveList[targetNumber].SetBuffStatus(
                            EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            true);
                        break;
                    // タイプ：デバフ
                    case SkillType.enDeBuff:
                        // 効果範囲が全体のとき
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            for (int playerNumber = 0; playerNumber < m_enemyMoveList.Count; playerNumber++)
                            {
                                value = m_enemyMoveList[myNumber].EnemyAction_Buff(
                                    skillNumber,                                                            // スキルの番号
                                    m_playerMoveList[playerNumber].PlayerStatus.ATK,                        // 攻撃力
                                    m_playerMoveList[playerNumber].PlayerStatus.DEF,                        // 防御力
                                    m_playerMoveList[playerNumber].PlayerStatus.SPD                         // 素早さ
                                    );
                                m_playerMoveList[playerNumber].SetBuffStatus(
                                    EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                                    value,
                                    skillNumber,
                                    false);
                            }
                            break;
                        }
                        m_enemyMoveList[myNumber].EnemyAction_Buff(
                            skillNumber,                                                                    // スキルの番号
                            m_playerMoveList[targetNumber].PlayerStatus.ATK,                                // 攻撃力
                            m_playerMoveList[targetNumber].PlayerStatus.DEF,                                // 防御力
                            m_playerMoveList[targetNumber].PlayerStatus.SPD                                 // 素早さ
                            );
                        m_playerMoveList[targetNumber].SetBuffStatus(
                            EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].BuffType,
                            value,
                            skillNumber,
                            false);
                        break;
                    // タイプ：回復
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                        // ターゲットを選択
                        targetNumber = m_enemyMoveList[myNumber].SelectTargetEnemy(m_enemyMoveList.Count);
                        m_enemyMoveList[myNumber].EnemyAction_HPResurrection(skillNumber, targetNumber);
                        m_enemyMoveList[myNumber].EnemyAction_HPRecover(m_enemyMoveList[targetNumber].EnemyStatus.HP, skillNumber);
                        // 効果範囲が全体のとき
                        if (EnemyData.enemyDataList[myNumber].skillDataList[skillNumber].EffectRange == EffectRange.enAll)
                        {
                            EnemyAction_AllRecover(m_enemyMoveList[myNumber].BasicValue);
                            return;
                        }
                        // HPを回復させる
                        m_enemyMoveList[targetNumber].RecoverHP(m_enemyMoveList[myNumber].BasicValue);
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
    /// プレイヤーにダメージを与える
    /// </summary>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="damage">ダメージ量</param>
    public void DamagePlayer(int targetNumber, int damage)
    {
        m_playerMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// エネミーの状態を変更する
    /// </summary>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="actorAbnormalState">変更先の状態</param>
    public void EnemyAction_ChangeStatePlayer(int targetNumber, ActorAbnormalState actorAbnormalState)
    {
        m_enemyMoveList[targetNumber].ActorAbnormalState = actorAbnormalState;
    }

    /// <summary>
    /// エネミーのステータスをリセットする
    /// </summary>
    private void ResetEnemyAction()
    {
        for (int i = 0; i < m_enemyMoveList.Count; i++)
        {
            m_enemyMoveList[i].ResetStatus();
        }
    }

    /// <summary>
    /// リストに追加し、番号を再設定する
    /// </summary>
    /// <returns>ターゲットの番号</returns>
    public int EnemyListAdd(EnemyMove enemyMove)
    {
        m_enemyMoveList.Add(enemyMove);
        return (int)m_enemyMoveList.Count - 1;
    }

    /// <summary>
    /// リストから自身を削除する
    /// </summary>
    /// <param name="myNumber">自身の番号</param>
    public void EnemyListRemove(int myNumber)
    {
        for(int i= 0; i < m_enemyMoveList.Count; i++)
        {
            if(m_enemyMoveList[i].MyNumber != myNumber)
            {
                continue;
            }
            Destroy(m_enemyMoveList[i].gameObject);
            m_enemyMoveList.Remove(m_enemyMoveList[i]);
            break;
        }
    }

    /// <summary>
    /// 全体を回復させる
    /// </summary>
    /// <param name="recoverValue">回復量</param>
    public void EnemyAction_AllRecover(int recoverValue)
    {
        for (int enemyNumber = 0; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
        {
            m_enemyMoveList[enemyNumber].RecoverHP(recoverValue);
        }
    }
}
