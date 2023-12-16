using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleButton : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject SkillWindow;
    [SerializeField]
    private GameObject SkillStatus;
    [SerializeField]
    private GameObject CommandWindow;
    [SerializeField, Header("�{�^��")]
    private GameObject AttackButton;
    [SerializeField]
    private GameObject SkillButton;
    [SerializeField]
    private GameObject GuardButton;
    [SerializeField]
    private GameObject OKButton;
    [SerializeField, Header("���ꂩ�̃{�^���������ꂽ���ǂ���")]
    private bool m_isButtonDown = false;

    private List<PlayerMove> m_playerMoveList;
    private BattleManager m_battleManager;
    private PlayerSkill m_playerSkill;
    // ���ݑ��삵�Ă���v���C���[
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
        m_playerMoveList.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));    // �ԍ����Ƀ\�[�g

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

        // �ԍ����قȂ�ۂɒl��ύX����
        if (m_currentTurnPlayerNumber != m_battleManager.OperatingPlayerNumber)
        {
            m_currentTurnPlayerNumber = m_battleManager.OperatingPlayerNumber;
        }
        // ���ُ�ԂȂ�{�^���������Ȃ�
        if (m_playerMoveList[m_currentTurnPlayerNumber].ActorAbnormalState == ActorAbnormalState.enSilence)
        {
            SkillButton.GetComponent<Button>().interactable = false;
            return;
        }
        // �X�L����I�������Ȃ�{�^����������悤�ɂ���
        if (m_playerSkill.SelectSkillNumber >= 0 ){
            OKButton.GetComponent<Button>().interactable = true;
        }
    }

    /// <summary>
    /// Attack�{�^���������ꂽ���̏���
    /// </summary>
    public void AttackButtonDown()
    {
        m_isButtonDown = true;
        m_playerMoveList[m_currentTurnPlayerNumber].NextActionType = ActionType.enAttack;
        ButtonDown = false;
        CommandWindow.SetActive(false);
    }

    /// <summary>
    /// Skill�{�^���������ꂽ���̏���
    /// </summary>
    public void SKillButtonDown()
    {
        // �X�L����I�����Ă��Ȃ��Ƃ��̓{�^���������Ȃ��悤�ɂ���
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
    /// Gurd�{�^���������ꂽ���̏���
    /// </summary>
    public void GurdButtonDown()
    {
        m_isButtonDown = true;
        m_playerMoveList[m_currentTurnPlayerNumber].NextActionType = ActionType.enGuard;
        CommandWindow.SetActive(false);
        ButtonDown = false;
    }

    /// <summary>
    /// �U���Ɏg�p����X�L�������肷��
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
    /// �R�}���h�I���ɖ߂�
    /// </summary>
    public void CancelSkillAttack()
    {
        m_playerSkill.ResetSelectSkillNumber();

        SkillWindow.SetActive(false);
        SkillStatus.SetActive(false);
        ButtonDown = true;
    }

    /// <summary>
    /// �X�e�[�^�X�����Z�b�g����
    /// </summary>
    public void ResetStatus()
    {
        m_isButtonDown = false;
        ButtonDown = true;
        CommandWindow.SetActive(true);
    }
}
