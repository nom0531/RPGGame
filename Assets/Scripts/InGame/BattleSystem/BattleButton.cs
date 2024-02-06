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
        set => m_isButtonDown = value;
    }

    /// <summary>
    /// ボタンが押せるかどうか設定する
    /// </summary>
    /// <param name="flag">trueなら押せる。falseなら押せない</param>
    private void SetInteractable(bool flag)
    {
        AttackButton.GetComponent<Button>().interactable = flag;
        SkillButton.GetComponent<Button>().interactable = flag;
        GuardButton.GetComponent<Button>().interactable = flag;
    }

    private void Start()
    {
        ResetStatus();

        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_playerSkill = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<PlayerSkill>();

        // playerMoveを人数分用意
        m_playerMoveList = m_battleManager.PlayerMoveList;
    }

    private void FixedUpdate()
    {
        if (m_battleManager.GameState != GameState.enPlay)
        {
            SetInteractable(false);
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
        // スキルが使用不可ならボタンを押せない
        if (m_playerSkill.UseSkillFlag == false)
        {
            OKButton.GetComponent<Button>().interactable = false;
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
        ButtonDown = true;
        m_playerMoveList[m_currentTurnPlayerNumber].NextActionType = ActionType.enAttack;
        SetInteractable(false);
        CommandWindow.SetActive(false);
    }

    /// <summary>
    /// Skillボタンが押された時の処理
    /// </summary>
    public void SKillButtonDown()
    {
        SetInteractable(false);

        // スキルを選択していないときはボタンを押せないようにする
        if (m_playerSkill.SelectSkillNumber < 0)
        {
            OKButton.GetComponent<Button>().interactable = false;
        }

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
        ButtonDown = true;
        m_playerMoveList[m_currentTurnPlayerNumber].NextActionType = ActionType.enGuard;
        CommandWindow.SetActive(false);
        SetInteractable(false);
    }

    /// <summary>
    /// 攻撃に使用するスキルを決定する
    /// </summary>
    public void DeterminationSkillAttack()
    {
        ButtonDown = true;
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
        SetInteractable(true);
    }

    /// <summary>
    /// ステータスをリセットする
    /// </summary>
    public void ResetStatus()
    {
        ButtonDown = false;
        SetInteractable(true);
        CommandWindow.SetActive(true);
    }
}
