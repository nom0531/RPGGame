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
        set => m_isButtonDown = value;
    }

    /// <summary>
    /// �{�^���������邩�ǂ����ݒ肷��
    /// </summary>
    /// <param name="flag">true�Ȃ牟����Bfalse�Ȃ牟���Ȃ�</param>
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

        // playerMove��l�����p��
        m_playerMoveList = m_battleManager.PlayerMoveList;
    }

    private void FixedUpdate()
    {
        if (m_battleManager.GameState != GameState.enPlay)
        {
            SetInteractable(false);
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
        // �X�L�����g�p�s�Ȃ�{�^���������Ȃ�
        if (m_playerSkill.UseSkillFlag == false)
        {
            OKButton.GetComponent<Button>().interactable = false;
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
        ButtonDown = true;
        m_playerMoveList[m_currentTurnPlayerNumber].NextActionType = ActionType.enAttack;
        SetInteractable(false);
        CommandWindow.SetActive(false);
    }

    /// <summary>
    /// Skill�{�^���������ꂽ���̏���
    /// </summary>
    public void SKillButtonDown()
    {
        SetInteractable(false);

        // �X�L����I�����Ă��Ȃ��Ƃ��̓{�^���������Ȃ��悤�ɂ���
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
    /// Gurd�{�^���������ꂽ���̏���
    /// </summary>
    public void GurdButtonDown()
    {
        ButtonDown = true;
        m_playerMoveList[m_currentTurnPlayerNumber].NextActionType = ActionType.enGuard;
        CommandWindow.SetActive(false);
        SetInteractable(false);
    }

    /// <summary>
    /// �U���Ɏg�p����X�L�������肷��
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
    /// �R�}���h�I���ɖ߂�
    /// </summary>
    public void CancelSkillAttack()
    {
        m_playerSkill.ResetSelectSkillNumber();

        SkillWindow.SetActive(false);
        SkillStatus.SetActive(false);
        SetInteractable(true);
    }

    /// <summary>
    /// �X�e�[�^�X�����Z�b�g����
    /// </summary>
    public void ResetStatus()
    {
        ButtonDown = false;
        SetInteractable(true);
        CommandWindow.SetActive(true);
    }
}
