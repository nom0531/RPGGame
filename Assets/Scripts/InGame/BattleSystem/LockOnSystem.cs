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

public class LockOnSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private SkillDataBase SkillData;
    [SerializeField, Header("参照オブジェクト"), Tooltip("標準カメラ")]
    private CinemachineVirtualCameraBase[] Vcam_Default;
    [SerializeField,Tooltip("ロックオンカメラ")]
    private CinemachineVirtualCameraBase[] Vcam_LockOn;
    [SerializeField, Tooltip("プレイヤーのオブジェクト")]
    private GameObject[] PlayerObject;
    [SerializeField]
    private LockOnButton RightButton, LeftButton, OKButton, CancelButton;
    [SerializeField]
    private GameObject LockOnButtons;
    [SerializeField]
    private GameObject CommandWindow;
    [SerializeField]
    private GameObject TargetNameWindow;

    private const int VCAM_PRIORITY = 10;           // カメラ使用時の優先度
    private const int NUM_MIN = 0;                  // 最小番号

    private BattleManager m_battleManager;
    private List<EnemyMove> m_enemyMoveList;
    private List<PlayerMove> m_playerMoveList;
    private DrawStatusValue m_drawStatusValue;
    private TargetState m_target;                   // ターゲットにする相手
    private int m_operatingPlayer = 0;              // 現在操作しているプレイヤー
    private int m_selectTargetNumber = 0;           // 現在選択しているターゲットの番号
    private int m_selectPlayerNumber = 0;           // 現在選択しているプレイヤーの番号
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

    // Start is called before the first frame update
    private void Awake()
    {
        ReSetVcamStatus();

        m_drawStatusValue = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<DrawStatusValue>();
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_operatingPlayer = m_battleManager.OperatingPlayerNumber;

        m_selectTargetNumber = 0;

        SetCinemachineVirtualCameraPriority(m_operatingPlayer, false, null);
        SetInteractable(false);
    }

    private void Start()
    {
        TargetNameWindow.SetActive(false);

        // enemyMoveを人数分用意
        var enemyMoveList = FindObjectsOfType<EnemyMove>();
        m_enemyMoveList = new List<EnemyMove>(enemyMoveList);
        // 名前を設定する
        m_drawStatusValue.EnemyName = m_enemyMoveList[m_selectTargetNumber].MyNumber;
        // playerMoveを人数分用意
        var playerMoveList = FindObjectsOfType<PlayerMove>();
        m_playerMoveList = new List<PlayerMove>(playerMoveList);
        // ソート
        m_playerMoveList.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));
        // 名前を設定する
        m_drawStatusValue.PlayerName = m_playerMoveList[m_selectTargetNumber].MyNumber;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }

        // ポーズ中なら処理を中断
        if (m_battleManager.PauseFlag == true)
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
    /// カメラを切り替える
    /// </summary>
    public void ResetCinemachine()
    {
        m_operatingPlayer = m_battleManager.OperatingPlayerNumber;
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

        // オブジェクトの表示、非表示を切り替える
        CommandWindow.SetActive(false);
        LockOnButtons.SetActive(true);
        TargetNameWindow.SetActive(true);
        // ボタンが押せるかどうか設定する
        SetInteractable(true);
        // カメラを設定する
        SetCinemachineVirtualCameraPriority(m_operatingPlayer, true, SetLookAtTarget());
        m_isActive = true;
    }

    /// <summary>
    /// ロックオンを終了する処理
    /// </summary>
    public void LockOnEnd()
    {
        // カメラを元に戻す
        SetCinemachineVirtualCameraPriority(m_operatingPlayer, false, null);
        // オブジェクトの表示、非表示を切り替える
        CommandWindow.SetActive(true);
        LockOnButtons.SetActive(false);
        TargetNameWindow.SetActive(false);
        PlayerObject[m_operatingPlayer].SetActive(true);
        // フラグをリセット
        SetInteractable(false);
        LockOn = false;
        m_battleManager.PlayerAction_ResetBattleButton();
        m_isActive = false;
    }

    /// <summary>
    /// 1つ右をロックオンする
    /// </summary>
    public void LookAtRightTarget()
    {
        // 最大値を設定する
        int max = m_enemyMoveList.Count - 1;
        // ターゲットがプレイヤーなら再設定
        if (m_target == TargetState.enPlayer)
        {
            max = m_playerMoveList.Count - 1;
        }

        m_selectTargetNumber++;

        // 一定以下なら補正
        if (m_selectTargetNumber > max)
        {
            // 最大値を選択する
            m_selectTargetNumber = NUM_MIN;
        }

        SetCinemachineVirtualCameraPriority(m_operatingPlayer, true, SetLookAtTarget());
    }

    /// <summary>
    /// 1つ左をロックオンする
    /// </summary>
    public void LookAtLeftTarget()
    {
        // 最大値を設定する
        int max = m_enemyMoveList.Count - 1;
        // ターゲットがプレイヤーなら再設定
        if (m_target == TargetState.enPlayer)
        {
            max = m_playerMoveList.Count - 1;
        }

        m_selectTargetNumber--;

        // 一定以上なら補正
        if (m_selectTargetNumber < NUM_MIN)
        {
            // 最小値を選択する
            m_selectTargetNumber = max;
        }

        SetCinemachineVirtualCameraPriority(m_operatingPlayer, true, SetLookAtTarget());
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
            Vcam_LockOn[number].Priority = VCAM_PRIORITY;
            Vcam_LockOn[number].LookAt = gameObject.transform;
            return;
        }

        Vcam_Default[number].Priority = VCAM_PRIORITY;
    }

    /// <summary>
    /// 仮装カメラの優先度をリセットする
    /// </summary>
    private void ReSetVcamStatus()
    {
        for(int i = 0; i < Vcam_Default.Length; i++)
        {
            Vcam_Default[i].Priority = 0;
            Vcam_LockOn[i].Priority = 0;
        }
    }

    /// <summary>
    /// 現在選択しているエネミーの番号を初期化する
    /// </summary>
    private void ResetSelectEnemyNumber()
    {
        // 選択していたエネミーがひん死でないなら実行しない
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
        // もし要素数以上なら補正する
        if(m_selectTargetNumber >= m_playerMoveList.Count)
        {
            m_selectTargetNumber = 0;
        }

        // 選択していたエネミーがひん死でないなら実行しない
        if (m_playerMoveList[m_selectTargetNumber].PlayerStatus.HPState != ActorHPState.enDie)
        {
            return;
        }

        m_playerMoveList.Remove(m_playerMoveList[m_selectTargetNumber]);

        for (int i = 0; i < m_playerMoveList.Count; i++)
        {
            // プレイヤーがひん死でないなら
            if (m_playerMoveList[i].PlayerStatus.HPState != ActorHPState.enDie)
            {
                // 値を設定する
                m_selectTargetNumber = i;
                break;
            }
        }
    }

    /// <summary>
    /// ターゲット先を設定する
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    public void SetTargetState(int skillNumber, ActionType actionType)
    {
        m_target = TargetState.enEnemy;

        // 次の行動が攻撃ならこれ以下の処理は実行しない
        if(actionType == ActionType.enAttack)
        {
            return;
        }

        switch (SkillData.skillDataList[skillNumber].SkillType)
        {
            case SkillType.enHeal:
            case SkillType.enResurrection:
            case SkillType.enBuff:
                m_target = TargetState.enPlayer;
                break;
        }
    }

    /// <summary>
    /// 決定したターゲットを設定する
    /// </summary>
    /// <returns>オブジェクト</returns>
    private GameObject SetLookAtTarget()
    {
        if(m_target == TargetState.enPlayer)
        {
            ResetSelectPlayerNumber();
            m_drawStatusValue.PlayerName = m_playerMoveList[m_selectTargetNumber].MyNumber;
            PlayerObject[m_operatingPlayer].SetActive(true);
            return m_playerMoveList[m_selectTargetNumber].gameObject;
        }

        ResetSelectEnemyNumber();
        m_drawStatusValue.EnemyName = m_enemyMoveList[m_selectTargetNumber].MyNumber;
        PlayerObject[m_operatingPlayer].SetActive(false);
        return m_enemyMoveList[m_selectTargetNumber].gameObject;
    }
}
