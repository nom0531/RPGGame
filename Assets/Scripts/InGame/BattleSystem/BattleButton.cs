using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleButton : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject SkillWindow;
    [SerializeField]
    private GameObject SkillStatus;
    [SerializeField]
    private GameObject CommandWindow;
    [SerializeField, Header("ボタン")]
    private GameObject AttackButton;
    [SerializeField]
    private GameObject SkillButton;
    [SerializeField]
    private GameObject GuardButton;
    [SerializeField]
    private GameObject OKButton;
    [SerializeField, Header("何れかのボタンが押されたかどうか")]
    private bool m_isButtonDown = false;

    private List<PlayerMove> m_playerMoveList;
    private BattleManager m_battleManager;
    private PlayerSkill m_playerSkill;
    // 現在操作しているプレイヤー
    private int m_currentTurnPlayerNumber = 0;

    public bool ButtonDown
    {
        get => m_isButtonDown;
        private set
        {
            AttackButton.GetComponent<Button>().interactable = value;
            SkillButton.GetComponent<Button>().interactable = value;
            GuardButton.GetComponent<Button>().interactable = value;
        }
    }

    private void Start()
    {
        ResetStatus();

        var playerMoveList = FindObjectsOfType<PlayerMove>();
        m_playerMoveList = new List<PlayerMove>(playerMoveList);
        m_playerMoveList.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));    // 番号順にソート

        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_playerSkill = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<PlayerSkill>(); 
    }

    private void Update()
    {
        if (m_battleManager.GameState != GameState.enPlay)
        {
            ButtonDown = false;
            return;
        }

        // 番号が異なる際に値を変更する
        if (m_currentTurnPlayerNumber != m_battleManager.OperatingPlayerNumber)
        {
            m_currentTurnPlayerNumber = m_battleManager.OperatingPlayerNumber;
        }
        // 沈黙状態ならボタンを押せない
        if (m_playerMoveList[m_currentTurnPlayerNumber].ActorAbnormalState == ActorAbnormalState.enSilence)
        {
            SkillButton.GetComponent<Button>().interactable = false;
            return;
        }
        // スキルを選択したならボタンを押せるようにする
        if (m_playerSkill.SelectSkillNumber >= 0 ){
            OKButton.GetComponent<Button>().interactable = true;
        }
    }

    /// <summary>
    /// Attackボタンが押された時の処理
    /// </summary>
    public void AttackButtonDown()
    {
        m_isButtonDown = true;
        m_playerMoveList[m_currentTurnPlayerNumber].NextActionType = ActionType.enAttack;
        ButtonDown = false;
        CommandWindow.SetActive(false);
    }

    /// <summary>
    /// Skillボタンが押された時の処理
    /// </summary>
    public void SKillButtonDown()
    {
        // スキルを選択していないときはボタンを押せないようにする
        if (m_playerSkill.SelectSkillNumber < 0)
        {
            OKButton.GetComponent<Button>().interactable = false;
        }

        ButtonDown = false;
        SkillWindow.SetActive(true);
        SkillStatus.SetActive(true);

        m_playerSkill.DestroySkillButton();
        m_playerSkill.InstantiateSkillButton();
    }

    /// <summary>
    /// Gurdボタンが押された時の処理
    /// </summary>
    public void GurdButtonDown()
    {
        m_isButtonDown = true;
        m_playerMoveList[m_currentTurnPlayerNumber].NextActionType = ActionType.enGuard;
        CommandWindow.SetActive(false);
        ButtonDown = false;
    }

    /// <summary>
    /// 攻撃に使用するスキルを決定する
    /// </summary>
    public void DeterminationSkillAttack()
    {
        m_isButtonDown = true;
        m_playerMoveList[m_currentTurnPlayerNumber].SelectSkillNumber = m_playerSkill.SelectSkillNumber;
        m_playerMoveList[m_currentTurnPlayerNumber].NextActionType = ActionType.enSkillAttack;
        SkillWindow.SetActive(false);
        SkillStatus.SetActive(false);
        CommandWindow.SetActive(false);
    }

    /// <summary>
    /// コマンド選択に戻る
    /// </summary>
    public void CancelSkillAttack()
    {
        m_playerSkill.ResetSelectSkillNumber();

        SkillWindow.SetActive(false);
        SkillStatus.SetActive(false);
        ButtonDown = true;
    }

    /// <summary>
    /// ステータスをリセットする
    /// </summary>
    public void ResetStatus()
    {
        m_isButtonDown = false;
        ButtonDown = true;
        CommandWindow.SetActive(true);
    }
}
