using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerEnhancementSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField]
    private EnhancementDataBase EnhancementData;
    [SerializeField, Header("�v���C���[���X�g"), Tooltip("��������{�^��")]
    private GameObject PlayerIconButton;
    [SerializeField, Tooltip("�{�^����ǉ�����I�u�W�F�N�g")]
    private GameObject PlayerContent;
    [SerializeField, Header("�X�L�����X�g"), Tooltip("��������{�^��")]
    private GameObject IconButton;
    [SerializeField, Tooltip("�{�^����ǉ�����I�u�W�F�N�g")]
    private GameObject Content;
    [SerializeField, Header("�\���p�f�[�^")]
    private GameObject Data_Sprite;
    [SerializeField]
    private GameObject Data_Name;
    [SerializeField]
    private GameObject Data_Element;
    [SerializeField, Tooltip("�X�L���̌���")]
    private GameObject Data_Detail;
    [SerializeField, Tooltip("�K�v��EP�̗�")]
    private GameObject Data_NecessaryEP;
    [SerializeField, Tooltip("����EP")]
    private GameObject Data_HaveEP;
    [SerializeField]
    private GameObject Data_PlayerName;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject Status;
    [SerializeField]
    private GameObject ReleaseButton;
    [SerializeField]
    private GameObject SkillButton, StatusButton;

    private GameManager m_gameManager;
    private SkillButton m_skillButton;
    private bool m_isReferenceSkill = true;     // true�Ȃ�X�L���f�[�^�Bfalse�Ȃ�X�e�[�^�X���Q�Ƃ���

    public bool ReferrenceSkillFlag
    {
        get => m_isReferenceSkill;
        set => m_isReferenceSkill = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameManager.Instance;
        // �X�L���f�[�^
        Data_Sprite.SetActive(false);
        Data_Name.SetActive(false);
        Data_Detail.SetActive(false);
        Status.SetActive(false);
        // �v���C���[�f�[�^
        Data_PlayerName.SetActive(false);
        Data_HaveEP.SetActive(true);
        // �l���X�V
        Data_HaveEP.GetComponent<TextMeshProUGUI>().text = m_gameManager.SaveData.SaveData.saveData.EnhancementPoint.ToString("N0");
        ReleaseButton.GetComponent<Button>().interactable = false;

        for (int playerNumber = 0; playerNumber < PlayerData.playerDataList.Count; playerNumber++)
        {
            // �{�^���𐶐����Ďq�I�u�W�F�N�g�ɂ���
            var playerObject = Instantiate(PlayerIconButton);
            playerObject.transform.SetParent(PlayerContent.transform);
            // �T�C�Y�𒲐�
            playerObject.transform.localScale = Vector3.one;
            playerObject.transform.localPosition = Vector3.zero;
            // ������
            var playerButton = playerObject.GetComponent<PlayerButton>();
            playerButton.SetPlayerEnhancement(
                playerNumber,                                               // �ԍ�
                PlayerData.playerDataList[playerNumber].PlayerSprite,       // �摜
                this
                );
        }
        m_skillButton = ReleaseButton.GetComponent<SkillButton>();
        m_skillButton.SetPlayerEnhancement(this);

        // ������
        var skillButton = SkillButton.GetComponent<PlayerButton>();
        skillButton.SetPlayerEnhancement(this);
        var statusButton = StatusButton.GetComponent<PlayerButton>();
        statusButton.SetPlayerEnhancement(this);

        if(ReferrenceSkillFlag == true)
        {
            DisplaySetSkillData(m_gameManager.PlayerNumber);
            return;
        }
        DisplaySetStatusData(m_gameManager.PlayerNumber);
    }

    /// <summary>
    /// �X�L���̃f�[�^��\������
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    public void DisplaySetSkillData(int number)
    {
        StatusButton.GetComponent<Image>().color = Color.white;
        SkillButton.GetComponent<Image>().color = Color.gray;
        StatusButton.GetComponent<Button>().interactable = true;
        SkillButton.GetComponent<Button>().interactable = false;

        DestroyIcon();
        Data_HaveEP.SetActive(true);
        Data_PlayerName.SetActive(true);
        ReleaseButton.GetComponent<Button>().interactable = false;
        // �l���X�V����
        Data_PlayerName.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[number].PlayerName;
        // ���݂̃v���C���[�̔ԍ����L�^
        GameManager.Instance.PlayerNumber = number;
        InstantiateSkillIcon(number);
        // �t���O��ݒ肷��
        ReferrenceSkillFlag = true;
    }

    /// <summary>
    /// �X�e�[�^�X�����̃f�[�^��\������
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    public void DisplaySetStatusData(int number)
    {
        StatusButton.GetComponent<Image>().color = Color.gray;
        SkillButton.GetComponent<Image>().color = Color.white;
        StatusButton.GetComponent<Button>().interactable = false;
        SkillButton.GetComponent<Button>().interactable = true;

        DestroyIcon();
        Data_HaveEP.SetActive(true);
        Data_PlayerName.SetActive(true);
        ReleaseButton.GetComponent<Button>().interactable = false;
        // �l���X�V����
        Data_PlayerName.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[number].PlayerName;
        // ���݂̃v���C���[�̔ԍ����L�^
        GameManager.Instance.PlayerNumber = number;
        InstantiateStatusIcon(number);
        // �t���O��ݒ肷��
        ReferrenceSkillFlag = false;
    }

    /// <summary>
    /// ���͂��ꂽ�X�L���f�[�^��\�����鏈��
    /// </summary>
    /// <param name="number">�X�L���̔ԍ�</param>
    public void DisplaySetSkill(int number)
    {
        m_skillButton.SelectNumber = number;
        var playerNumber = GameManager.Instance.PlayerNumber;
        var enhancementPoint = PlayerData.playerDataList[playerNumber].skillDataList[number].EnhancementPoint;

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Data_Detail.SetActive(true);
        Status.SetActive(true);

        // �l���X�V����
        Data_Sprite.GetComponent<Image>().sprite =
            PlayerData.playerDataList[playerNumber].skillDataList[number].SkillSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[playerNumber].skillDataList[number].SkillName;
        Data_Detail.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[playerNumber].skillDataList[number].SkillDetail;
        Data_NecessaryEP.GetComponent<TextMeshProUGUI>().text = enhancementPoint.ToString();
        GetElement(Data_Element, number, playerNumber);
        // �{�^���������邩�ǂ������肷��
        SetInterctable(enhancementPoint, m_gameManager.SaveData.SaveData.saveData.SkillRegisters[playerNumber].PlayerSkills[number]);
    }

    /// <summary>
    /// ���͂��ꂽ�X�e�[�^�X�f�[�^��\������
    /// </summary>
    /// <param name="number">�X�e�[�^�X�̔ԍ�</param>
    public void DisplaySetStatus(int number)
    {
        m_skillButton.SelectNumber = number;
        var enhancementPoint = PlayerData.playerDataList[m_gameManager.PlayerNumber].enhancementDataList[number].EnhancementPoint;

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Data_Detail.SetActive(true);
        Status.SetActive(true);

        // �l���X�V����
        Data_Sprite.GetComponent<Image>().sprite =
            PlayerData.playerDataList[m_gameManager.PlayerNumber].enhancementDataList[number].EnhancementSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[m_gameManager.PlayerNumber].enhancementDataList[number].EnhancementName;
        Data_NecessaryEP.GetComponent<TextMeshProUGUI>().text = enhancementPoint.ToString();
        GetStatusName(Data_Detail,number);
        Data_Element.GetComponent<TextMeshProUGUI>().text = "-";
        // �{�^���������邩�ǂ������肷��
        SetInterctable(enhancementPoint, m_gameManager.SaveData.SaveData.saveData.
            EnhancementRegisters[m_gameManager.PlayerNumber].PlayerEnhancements[number]);
    }

    /// <summary>
    /// �X�e�[�^�X�̖��O��ݒ肷��
    /// </summary>
    /// <param name="number">�X�e�[�^�X�̔ԍ�</param>
    private void GetStatusName(GameObject gameObject, int number)
    {
        var statusName = "";
        var baseStatus = 0;
        var addValue = PlayerData.playerDataList[m_gameManager.PlayerNumber].enhancementDataList[number].AddValue;
        switch (PlayerData.playerDataList[m_gameManager.PlayerNumber].enhancementDataList[number].EnhancementStatus)
        {
            case EnhancementStatus.enHP:
                statusName = "HP";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].HP;
                break;
            case EnhancementStatus.enSP:
                statusName = "SP";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].SP;
                break;
            case EnhancementStatus.enATK:
                statusName = "ATK";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].ATK;
                break;
            case EnhancementStatus.enDEF:
                statusName = "DEF";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].DEF;
                break;
            case EnhancementStatus.enSPD:
                statusName = "SPD";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].SPD;
                break;
            case EnhancementStatus.enLUCK:
                statusName = "LUCK";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].LUCK;
                break;
        }
        if(m_gameManager.SaveData.SaveData.saveData.EnhancementRegisters[m_gameManager.PlayerNumber].PlayerEnhancements[number] == true)
        {
            gameObject.GetComponent<TextMeshProUGUI>().text =
            $"{statusName}��{addValue}��������B( - )";
            return;
        }
        gameObject.GetComponent<TextMeshProUGUI>().text =�@
            $"{statusName}��{addValue}��������B({baseStatus}��{baseStatus + addValue})";
    }

    /// <summary>
    /// �����ϐ���ݒ肷��
    /// </summary>
    /// <param name="number">�X�L���̔ԍ�</param>
    /// <param name="playerNumber">�v���C���[�̔ԍ�</param>
    private void GetElement(GameObject gameObjct, int number,int playerNumber)
    {
        var element = PlayerData.playerDataList[playerNumber].skillDataList[number].SkillElement;
        switch (element)
        {
            case ElementType.enFire:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "��";
                break;
            case ElementType.enIce:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "�X";
                break;
            case ElementType.enWind:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "��";
                break;
            case ElementType.enThunder:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "��";
                break;
            case ElementType.enLight:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "��";
                break;
            case ElementType.enDark:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "��";
                break;
            default:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "-";
                break;
        }
    }

    /// <summary>
    /// EnhancementIcon�^�O�̕t�����I�u�W�F�N�g��S�č폜���鏈��
    /// </summary>
    private void DestroyIcon()
    {
        var skillIcons = GameObject.FindGameObjectsWithTag("EnhancementIcon");
        foreach (var button in skillIcons)
        {
            Destroy(button);
        }
    }

    /// <summary>
    /// �A�C�R���𐶐����鏈��
    /// </summary>
    /// <param name="playerNumber">�v���C���[�̔ԍ�</param>
    private void InstantiateSkillIcon(int playerNumber)
    {
        for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[playerNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // ���ʔԍ��������Ȃ�f�[�^������������
                if (PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].ID == SkillData.skillDataList[dataNumber].ID)
                {
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillDetail = SkillData.skillDataList[dataNumber].SkillDetail;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillEffect = SkillData.skillDataList[dataNumber].SkillEffect;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
                    break;
                }
            }
            // �{�^���𐶐�
            var button = Instantiate(IconButton);
            button.transform.SetParent(Content.transform);
            // �T�C�Y�𒲐�����
            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = Vector3.one;
            button.transform.localRotation = Quaternion.identity;
            // �R���|�[�l���g���擾
            var skillButton = button.GetComponent<SkillButton>();
            skillButton.SetPlayerEnhancement(
                skillNumber,
                PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillSprite,
                true,
                this
                );
            button.SetActive(true);
        }
    }

    /// <summary>
    /// �A�C�R���𐶐����鏈��
    /// </summary>
    /// <param name="playerNumber">�v���C���[�̔ԍ�</param>
    private void InstantiateStatusIcon(int playerNumber)
    {
        var number = 0; // �����f�[�^��ID
        for (int statusNumber = 0; statusNumber < PlayerData.playerDataList[playerNumber].enhancementDataList.Count; statusNumber++)
        {
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].ID = number;
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].EnhancementName = EnhancementData.enhancementDataList[number].EnhancementName;
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].EnhancementSprite = EnhancementData.enhancementDataList[number].EnhancementSprite;
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].EnhancementStatus = EnhancementData.enhancementDataList[number].EnhancementStatus;
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].EnhancementPoint = EnhancementData.enhancementDataList[number].EnhancementPoint;
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].AddValue = EnhancementData.enhancementDataList[number].AddValue;
            // �{�^���𐶐�
            var button = Instantiate(IconButton);
            button.transform.SetParent(Content.transform);
            // �T�C�Y�𒲐�����
            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = Vector3.one;
            button.transform.localRotation = Quaternion.identity;
            // �R���|�[�l���g���擾
            var skillButton = button.GetComponent<SkillButton>();
            skillButton.SetPlayerEnhancement(
                statusNumber,
                PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].EnhancementSprite,
                true,
                this
                );
            button.SetActive(true);
            number++;
        }
    }

    /// <summary>
    /// �{�^���������邩�ǂ������肷��
    /// </summary>
    /// <param name="enhancementPoint">�K�v�ȃ|�C���g��</param>
    /// <param name="isAcquired">�擾�ς݂��ǂ���</param>
    /// <returns>true�Ȃ牟����Bfalse�Ȃ牟���Ȃ�</returns>
    private void SetInterctable(int enhancementPoint, bool isAcquired)
    {
        // �K�v�ʂɑ���Ă��Ȃ��@�܂��́@���Ɏg�p�ł����ԂȂ�{�^���������Ȃ��悤�ɂ���
        if (isAcquired == true)
        {
            ReleaseButton.GetComponent<Button>().interactable = false;
            ReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "�擾�ς�";
            return;
        }
        if(m_gameManager.SaveData.SaveData.saveData.EnhancementPoint < enhancementPoint)
        {
            ReleaseButton.GetComponent<Button>().interactable = false;
            ReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "EP�s��";
            return;
        }
        ReleaseButton.GetComponent<Button>().interactable = true;
        ReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "�擾";
    }
}
