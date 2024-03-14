using System.Collections.Generic;
using UnityEngine;

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
    private SkillDataBase SkillData;
    [SerializeField]
    private LevelDataBase LevelData;
    [SerializeField, Header("参照オブジェクト")]
    private GameObject PauseCanvas;
    [SerializeField, Tooltip("プレイヤーのアイコン")]
    private GameObject[] PlayerIcon;
    [SerializeField, Tooltip("コマンド選択中のアイコン")]
    private GameObject CommandIcon;
    [SerializeField, Header("バトルデータ"), Tooltip("生成する画像")]
    private GameObject Sprite;
    [SerializeField, Tooltip("画像を追加するオブジェクト")]
    private GameObject Content;
    [SerializeField, Tooltip("エネミーのサイズ")]
    private float EnemySpriteSize = 450.0f;
    [SerializeField]
    private BattleButton[] BattleButton;

    private const int MAX_ENEMY_NUM = 4;                        // バトルに出現するエネミーの最大数
    private const float ADD_SIZE = 1.6f;                        // エネミーの画像の乗算サイズ

    private GameState m_gameState;                              // ゲームのステート
    private OperatingPlayer m_operatingPlayer;                  // 操作しているプレイヤー
    private BattleSystem m_battleSystem;                        // バトルシステム
    private PauseManager m_pauseManager;
    private TurnManager m_turnManager;                          // ターン管理システム
    private PlayerTurn m_playerTurn;                            // プレイヤー側の動作
    private EnemyTurn m_enemyTurn;                              // エネミー側の動作
    private StagingManager m_stagingManager;                    // 演出用システム
    private DrawStatusValue m_drawStatusValue;                  // ステータスを表示するUI
    private List<PlayerMove> m_playerMoveList;                  // プレイヤーの行動
    private List<EnemyMove> m_enemyMoveList;                    // エネミーの行動
    private int m_enemySum = 0;                                 // エネミーの総数
    private int m_enemyNumber = 0;                              // エネミーの番号
    private bool m_isStagingStart = false;                      // 演出が開始したかどうか。falseなら開始していない

    public OperatingPlayer OperatingPlayer
    {
        get => m_operatingPlayer;
        set => m_operatingPlayer = value;
    }

    public GameState GameState
    {
        get => m_gameState;
        set => m_gameState = value;
    }

    public PlayerDataBase PlayerDataBase
    {
        get => PlayerData;
    }

    public EnemyDataBase EnemyDataBase
    {
        get => EnemyData;
    }

    public SkillDataBase SkillDataBase
    {
        get => SkillData;
    }

    public bool StagingStartFlag
    {
        get => m_isStagingStart;
        set => m_isStagingStart = value;
    }

    public int EnemySum
    {
        get => m_enemySum;
    }

    public List<PlayerMove> PlayerMoveList
    {
        get => m_playerMoveList;
    }

    public List<EnemyMove> EnemyMoveList
    {
        get => m_enemyMoveList;
    }

    private void Awake()
    {
        m_pauseManager = GetComponent<PauseManager>();
        m_battleSystem = GetComponent<BattleSystem>();
        m_drawStatusValue = GetComponent<DrawStatusValue>();
        m_stagingManager = GetComponent<StagingManager>();
        m_playerTurn = GetComponent<PlayerTurn>();
        m_enemyTurn = GetComponent<EnemyTurn>();
        m_turnManager = GetComponent<TurnManager>();
    }

    private void Start()
    {
        var SPD = int.MinValue;
        for (int i = 0; i < (int)OperatingPlayer.enNum; i++)
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
        InitValue();
    }

    // Update is called once per frame
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_gameState = GameState.enBattleWin;
        }
#endif
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        Debug.Log($"現在のターン数：{m_turnManager.TurnSum}ターン目");
#endif
        // ゲームが終了しているなら、これより以下の処理は実行されない
        if (GameState != GameState.enPlay)
        {
            return;
        }
        // ポーズしているなら実行しない
        if (m_pauseManager.PauseFlag == true)
        {
            return;
        }
        // 演出が開始されたなら実行しない
        if(m_stagingManager.StangingState == StagingState.enStangingStart)
        {
            return;
        }
        switch (m_turnManager.TurnStatus)
        {
            // プレイヤーのターン
            case TurnStatus.enPlayer:
                switch (m_operatingPlayer)
                {
                    case OperatingPlayer.enAttacker:
                    case OperatingPlayer.enBuffer:
                    case OperatingPlayer.enHealer:
                        m_playerTurn.PlayerAction((int)m_operatingPlayer);
                        break;
                }
                break;
            // エネミーのターン
            case TurnStatus.enEnemy:
                for (int enemyNumber = m_enemyNumber; enemyNumber < m_enemyMoveList.Count; enemyNumber++)
                {
                    // 死亡している際は実行しない
                    if (m_enemyMoveList[enemyNumber].ActorHPState == ActorHPState.enDie)
                    {
                        continue;
                    }
                    m_enemyTurn.EnemyAction(enemyNumber);
                }
                m_enemyNumber++;
                break;
        }
        UpdateUIStatus();
        m_turnManager.IsTurnEnd();
    }

    /// <summary>
    /// 値を初期化する
    /// </summary>
    public void InitValue()
    {
        m_operatingPlayer = NextOperatingPlayer();
        m_enemyNumber = 0;
    }

    /// <summary>
    /// 次の操作キャラクターを決定する
    /// </summary>
    public OperatingPlayer NextOperatingPlayer()
    {
        var operatingPlayer = OperatingPlayer;
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
            if (PlayerData.playerDataList[i].SPD > SPD)
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
            m_turnManager.TurnStatus = TurnStatus.enEnemy;
        }
        return operatingPlayer;
    }

    /// <summary>
    /// UIを更新する
    /// </summary>
    private void UpdateUIStatus()
    {
        m_drawStatusValue.SetStatus();
        m_drawStatusValue.SetStatusText();
    }

    /// <summary>
    /// ステータスを初期化する
    /// </summary>
    public void ResetGameStatus()
    {
        // ステータスを初期化
        ResetPlayerAction();
        ResetEnemyAction();
        ResetBattleButton();
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
    /// エネミーにダメージを与える
    /// </summary>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="damage">ダメージ量</param>
    public void DamageEnemy(int targetNumber, int damage)
    {
        EnemyMoveList[targetNumber].DecrementHP(damage);
    }

    /// <summary>
    /// エネミーの状態を変更する
    /// </summary>
    /// <param name="targetNumber">ターゲットの番号</param>
    /// <param name="actorAbnormalState">変更先の状態</param>
    public void PlayerAction_ChangeStateEnemy(int targetNumber, ActorAbnormalState actorAbnormalState)
    {
        EnemyMoveList[targetNumber].ActorAbnormalState = actorAbnormalState;
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
}
