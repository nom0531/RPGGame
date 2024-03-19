using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

/// <summary>
/// ターゲットにする相手
/// </summary>
public enum TargetState
{
    enPlayer,
    enEnemy
}

public class LockOnManager : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト"), Tooltip("標準カメラ")]
    private CinemachineVirtualCameraBase[] Vcam_Default;
    [SerializeField,Tooltip("ロックオンカメラ")]
    private CinemachineVirtualCameraBase[] Vcam_LockOn;
    [SerializeField]
    private LockOnButton RightButton, LeftButton, OKButton, CancelButton;
    [SerializeField]
    private GameObject LockOnCanvas;
    [SerializeField, Header("HP")]
    private GameObject EnemyHPObject;

    private const int VCAM_PRIORITY = 10;           // カメラ使用時の優先度
    private const int NUM_MIN = 0;                  // 最小番号

    private BattleManager m_battleManager;
    private TurnManager m_turnManager;
    private PauseManager m_pauseManager;
    private List<EnemyMove> m_enemyMoveList;
    private List<PlayerMove> m_playerMoveList;
    private DrawStatusValue m_drawStatusValue;
    private SkillDataBase m_skillData;
    private EnemyHitPoint m_enemyHitPoint;
    private TargetState m_target;                   // ターゲットにする相手
    private int m_operatingPlayerNumber = 0;              // 現在操作しているプレイヤー
    private int m_selectTargetNumber = 0;           // 現在選択しているターゲットの番号
    private bool m_isLockOnStart = false;           // ロックオンを開始するかどうか
    private bool m_isButtonDown = false;            // ボタンが押されたかどうか
    private bool m_isActive = false;                // 一度Activeにしたかどうか

    public bool ButtonDown
    {
        get => m_isButtonDown;
        set => m_isButtonDown = value;
    }

    public bool LockOn
    {
        set => m_isLockOnStart = value;
    }

    public int TargetNumber
    {
        get => m_selectTargetNumber;
    }

    public TargetState TargetState
    {
        get => m_target;
        set => m_target = value;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        ReSetVcamStatus();
        m_drawStatusValue = GetComponent<DrawStatusValue>();
        m_battleManager = GetComponent<BattleManager>();
        m_turnManager = GetComponent<TurnManager>();
        m_pauseManager = GetComponent<PauseManager>();
        m_skillData = m_battleManager.SkillDataBase;
        SetInteractable(false);
    }

    private void Start()
    {
        m_enemyHitPoint = EnemyHPObject.GetComponent<EnemyHitPoint>();
        m_operatingPlayerNumber = (int)m_battleManager.OperatingPlayer;
        SetCinemachineVirtualCameraPriority(m_operatingPlayerNumber, false, null);
        LockOnCanvas.SetActive(false);
        // エネミーのリスト
        m_enemyMoveList = m_battleManager.EnemyMoveList;
        m_drawStatusValue.EnemyName = m_enemyMoveList[m_selectTargetNumber].MyNumber;
        // プレイヤーのリスト
        m_playerMoveList = m_battleManager.PlayerMoveList;
        m_playerMoveList.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));
        m_drawStatusValue.PlayerName = m_playerMoveList[m_selectTargetNumber].MyNumber;

        DrawPlayers(false);
        m_playerMoveList[m_operatingPlayerNumber].gameObject.SetActive(true);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // ポーズ中なら処理を中断
        if (m_pauseManager.PauseFlag == true)
        {
            return;
        }
        // ロックオンを開始していないなら実行しない
        if(m_isLockOnStart == false)
        {
            return;
        }
        LockOnStart();
    }
    
    /// <summary>
    /// プレイヤーのActiveを設定する
    /// </summary>
    /// <param name="flag">trueなら描画。falseなら描画しない</param>
    private void DrawPlayers(bool flag)
    {
        // 演出が実行中なら無視
        if(m_battleManager.StagingStartFlag == true)
        {
            return;
        }
        for(int i = 0; i < m_playerMoveList.Count; i++)
        {
            m_playerMoveList[i].gameObject.SetActive(flag);
        }
    }

    /// <summary>
    /// カメラを切り替える
    /// </summary>
    public void ResetCinemachine()
    {
        m_operatingPlayerNumber = (int)m_battleManager.OperatingPlayer;
        LockOnEnd();
    }

    /// <summary>
    /// ロックオンを開始する処理
    /// </summary>
    private void LockOnStart()
    {
        // 既に実行しているなら実行はしない
        if (m_isActive == true)
        {
            return;
        }
        // 番号を補正
        m_selectTargetNumber = 0;
        // オブジェクトの表示、非表示を切り替える
        LockOnCanvas.SetActive(true);
        m_playerMoveList[m_operatingPlayerNumber].gameObject.SetActive(true);
        // HPバーを再設定する
        m_enemyHitPoint.SetFillAmount();
        // ボタンが押せるかどうか設定する
        SetInteractable(true);
        // カメラを設定する
        SetCinemachineVirtualCameraPriority(m_operatingPlayerNumber, true, SetLookAtTarget());
        m_isActive = true;
    }

    /// <summary>
    /// ロックオンを終了する処理
    /// </summary>
    public void LockOnEnd()
    {
        // カメラを元に戻す
        SetCinemachineVirtualCameraPriority(m_operatingPlayerNumber, false, null);
        // フラグをリセット
        SetInteractable(false);
        LockOn = false;
        m_battleManager.ResetBattleButton();
        m_isActive = false;
        // プレイヤーの表示を切り替える
        DrawPlayers(false);
        m_playerMoveList[m_operatingPlayerNumber].gameObject.SetActive(true);
    }

    /// <summary>
    /// 1つ右をロックオンする
    /// </summary>
    public void LookAtRightTarget()
    {
        int max = m_enemyMoveList.Count - 1;
        // ターゲットがプレイヤーなら再設定
        if (TargetState == TargetState.enPlayer)
        {
            max = m_playerMoveList.Count - 1;
            m_selectTargetNumber--;
            // 0以下なら補正
            if (m_selectTargetNumber < NUM_MIN)
            {
                m_selectTargetNumber = max;
            }
        }
        else
        {
            m_selectTargetNumber++;
            // 最大値以上なら補正
            if (m_selectTargetNumber > max)
            {
                m_selectTargetNumber = NUM_MIN;
            }
        }
        m_enemyHitPoint.SetFillAmount();
        SetCinemachineVirtualCameraPriority(m_operatingPlayerNumber, true, SetLookAtTarget());
    }

    /// <summary>
    /// 1つ左をロックオンする
    /// </summary>
    public void LookAtLeftTarget()
    {
        int max = m_enemyMoveList.Count - 1;
        // ターゲットがプレイヤーなら再設定
        if (TargetState == TargetState.enPlayer)
        {
            max = m_playerMoveList.Count - 1;
            m_selectTargetNumber++;
            // 最大値以上なら補正
            if (m_selectTargetNumber > max)
            {
                m_selectTargetNumber = NUM_MIN;
            }
        }
        else
        {
            m_selectTargetNumber--;
            // 0以上なら補正
            if (m_selectTargetNumber < NUM_MIN)
            {
                m_selectTargetNumber = max;
            }
        }
        m_enemyHitPoint.SetFillAmount();
        SetCinemachineVirtualCameraPriority(m_operatingPlayerNumber, true, SetLookAtTarget());
    }

    /// <summary>
    /// 各UIボタンが押せるかどうかのフラグを設定する
    /// </summary>
    /// <param name="flag">trueなら押せる。falseなら押せない</param>
    private void SetInteractable(bool flag)
    {
        RightButton.GetComponent<Button>().interactable = flag;
        LeftButton.GetComponent<Button>().interactable = flag;
        OKButton.GetComponent<Button>().interactable = flag;
        CancelButton.GetComponent<Button>().interactable = flag;
    }

    /// <summary>
    /// 仮想カメラの優先度を設定する
    /// </summary>
    /// <param name="number">現在操作しているプレイヤーの番号</param>
    /// <param name="isLockOn">trueならロックオンを開始する。falseならロックオンしない</param>
    /// <param name="gameObject">ロックオンする相手のオブジェクト</param>
    private void SetCinemachineVirtualCameraPriority(int number, bool isLockOn, GameObject gameObject)
    {
        if (m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }
        ReSetVcamStatus();
        if (isLockOn == true)
        {
            // ターゲットを設定する
            Vcam_LockOn[0].Priority = VCAM_PRIORITY;
            Vcam_LockOn[0].LookAt = gameObject.transform;
            return;
        }
        // 優先度を設定
        Vcam_Default[number].Priority = VCAM_PRIORITY;
    }

    /// <summary>
    /// 仮装カメラの優先度をリセットする
    /// </summary>
    private void ReSetVcamStatus()
    {
        Vcam_LockOn[0].Priority = 0;

        for (int i = 0; i < Vcam_Default.Length; i++)
        {
            Vcam_Default[i].Priority = 0;
        }
    }

    /// <summary>
    /// 現在選択しているエネミーの番号を初期化する
    /// </summary>
    private void ResetSelectEnemyNumber()
    {
        // 補正
        if (m_selectTargetNumber >= m_enemyMoveList.Count)
        {
            m_selectTargetNumber = m_enemyMoveList.Count - 1;
        }
        // ターゲットがひん死でないなら実行しない
        if (m_enemyMoveList[m_selectTargetNumber].ActorHPState != ActorHPState.enDie)
        {
            return;
        }
        m_enemyMoveList.Remove(m_enemyMoveList[m_selectTargetNumber]);
        for (int i = 0; i < m_enemyMoveList.Count; i++)
        {
            // エネミーがひん死でないなら
            if (m_enemyMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                // 値を設定する
                m_selectTargetNumber = i;
                break;
            }
        }
    }

    /// <summary>
    /// 現在選択しているプレイヤーの番号をリセットする
    /// </summary>
    private void ResetSelectPlayerNumber()
    {
        // 補正
        if (m_selectTargetNumber >= m_playerMoveList.Count)
        {
            m_selectTargetNumber = m_playerMoveList.Count - 1;
        }
        // ターゲットがひん死でないなら実行しない
        if (m_playerMoveList[m_selectTargetNumber].ActorHPState != ActorHPState.enDie)
        {
            return;
        }
        m_playerMoveList.Remove(m_playerMoveList[m_selectTargetNumber]);
        for (int i = 0; i < m_playerMoveList.Count; i++)
        {
            // プレイヤーがひん死でないなら
            if (m_playerMoveList[i].ActorHPState != ActorHPState.enDie)
            {
                // 値を設定する
                m_selectTargetNumber = i;
                break;
            }
        }
    }

    /// <summary>
    /// ロックオンの対象を設定する
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    public void SetTargetState(int skillNumber, ActionType actionType)
    {
        switch (m_turnManager.TurnStatus)
        {
            case TurnStatus.enPlayer:
                LockOn = true;
                TargetState = TargetState.enEnemy;
                // 次の行動が攻撃ならこれ以下の処理は実行しない
                if (actionType == ActionType.enAttack)
                {
                    return;
                }
                switch (m_skillData.skillDataList[skillNumber].SkillType)
                {
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                    case SkillType.enBuff:
                        TargetState = TargetState.enPlayer;
                        break;
                    default:
                        break;
                }
                break;
            case TurnStatus.enEnemy:
                LockOn = false;
                TargetState = TargetState.enPlayer;
                // 次の行動が攻撃ならこれ以下の処理は実行しない
                if (actionType == ActionType.enAttack)
                {
                    return;
                }
                switch (m_skillData.skillDataList[skillNumber].SkillType)
                {
                    case SkillType.enHeal:
                    case SkillType.enResurrection:
                    case SkillType.enBuff:
                        TargetState = TargetState.enEnemy;
                        break;
                    default:
                        break;
                }
                break;
        }
    }

    /// <summary>
    /// 決定したターゲットを設定する
    /// </summary>
    /// <returns>ターゲットの子オブジェクト</returns>
    private GameObject SetLookAtTarget()
    {
        if(TargetState == TargetState.enPlayer)
        {
            ResetSelectPlayerNumber();
            m_drawStatusValue.PlayerName = m_playerMoveList[m_selectTargetNumber].MyNumber;
            DrawPlayers(true);
            return m_playerMoveList[m_selectTargetNumber].transform.GetChild(0).gameObject;
        }
        ResetSelectEnemyNumber();
        m_playerMoveList[m_operatingPlayerNumber].gameObject.SetActive(false);
        m_drawStatusValue.EnemyName = m_enemyMoveList[m_selectTargetNumber].MyNumber;
        return m_enemyMoveList[m_selectTargetNumber].transform.GetChild(0).gameObject;
    }
}
