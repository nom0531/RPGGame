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
    [SerializeField, Header("�v���C���[���X�g"), Tooltip("��������{�^��")]
    private GameObject PlayerIconButton;
    [SerializeField, Tooltip("�{�^����ǉ�����I�u�W�F�N�g")]
    private GameObject PlayerContent;
    [SerializeField, Header("�X�L�����X�g"), Tooltip("��������{�^��")]
    private GameObject IconButton;
    [SerializeField, Tooltip("�{�^����ǉ�����I�u�W�F�N�g")]
    private GameObject Content;
    [SerializeField, Header("�\���p�f�[�^"), Tooltip("�摜")]
    private GameObject Data_Sprite;
    [SerializeField, Tooltip("���O")]
    private GameObject Data_Name;
    [SerializeField, Tooltip("����")]
    private GameObject Data_Element;
    [SerializeField, Tooltip("�X�L���̌���")]
    private GameObject Data_SkilDetail;
    [SerializeField, Tooltip("�K�v��EP�̗�")]
    private GameObject Data_SkillNecessaryEP;
    [SerializeField, Tooltip("����EP")]
    private GameObject Data_HaveEP;
    [SerializeField, Tooltip("�v���C���[��")]
    private GameObject Data_PlayerName;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject SkillStatus;
    [SerializeField]
    private GameObject SkillReleaseButton;
    [SerializeField]
    private GameObject SkillButton, StatusButton;
    [SerializeField, Tooltip("�{�^���̉摜")]
    private Sprite ButtonImage;

    private const string BEFORE_ACQUISITION = "���";
    private const string AFTER_ACQUISITION = "����ς�";

    private SaveDataManager m_saveDataManager;
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
        // �X�L���f�[�^
        Data_Sprite.SetActive(false);
        Data_Name.SetActive(false);
        Data_SkilDetail.SetActive(false);
        SkillStatus.SetActive(false);
        // �v���C���[�f�[�^
        Data_PlayerName.SetActive(false);
        Data_HaveEP.SetActive(true);
        m_saveDataManager = GameManager.Instance.SaveData;
        // �l���X�V
        Data_HaveEP.GetComponent<TextMeshProUGUI>().text = m_saveDataManager.SaveData.saveData.EnhancementPoint.ToString("N0");

        SkillReleaseButton.GetComponent<Button>().interactable = false;

        for (int playerNumber = 0; playerNumber < PlayerData.playerDataList.Count; playerNumber++)
        {
            // �{�^���𐶐����Ďq�I�u�W�F�N�g�ɂ���
            var playerObject = Instantiate(PlayerIconButton);
            playerObject.transform.SetParent(PlayerContent.transform);
            // �T�C�Y�𒲐�
            playerObject.transform.localScale = Vector3.one;
            playerObject.transform.localPosition = Vector3.zero;

            var playerButton = playerObject.GetComponent<PlayerButton>();
            playerButton.SetPlayerEnhancement(
                playerNumber,                                               // �ԍ�
                PlayerData.playerDataList[playerNumber].PlayerSprite,       // �摜
                this
                );
        }
        m_skillButton = SkillReleaseButton.GetComponent<SkillButton>();

        // ������
        var skillButton = SkillButton.GetComponent<PlayerButton>();
        skillButton.SetPlayerEnhancement(
            PlayerNumberManager.PlayerNumber,
            ButtonImage,       // �摜
            this
            );
        var statusButton = StatusButton.GetComponent<PlayerButton>();
        statusButton.SetPlayerEnhancement(
            PlayerNumberManager.PlayerNumber,
            ButtonImage,       // �摜
            this
            );

        if(ReferrenceSkillFlag == true)
        {
            DisplaySetSkillData(PlayerNumberManager.PlayerNumber);
            return;
        }
        DisplaySetStatusData(PlayerNumberManager.PlayerNumber);
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
        SkillReleaseButton.GetComponent<Button>().interactable = false;
        // �l���X�V����
        Data_PlayerName.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[number].PlayerName;
        // ���݂̃v���C���[�̔ԍ����L�^
        PlayerNumberManager.PlayerNumber = number;
        InstantiateSkillIcon();
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
        SkillReleaseButton.GetComponent<Button>().interactable = false;
        // �l���X�V����
        Data_PlayerName.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[number].PlayerName;
        // ���݂̃v���C���[�̔ԍ����L�^
        PlayerNumberManager.PlayerNumber = number;
        InstantiateStatusIcon();
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

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Data_SkilDetail.SetActive(true);
        SkillStatus.SetActive(true);

        // �l���X�V����
        Data_Sprite.GetComponent<Image>().sprite =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillName;
        Data_SkilDetail.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillDetail;
        Data_SkillNecessaryEP.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].EnhancementPoint.ToString();
        GetElement(Data_Element, number, PlayerNumberManager.PlayerNumber);

        // ���Ɏg�p�ł����ԂȂ�{�^���������Ȃ��悤�ɂ���
        if (m_saveDataManager.SaveData.saveData.SkillRegisters[PlayerNumberManager.PlayerNumber].PlayerSkills[number] == true)
        {
            SkillReleaseButton.GetComponent<Button>().interactable = false;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = AFTER_ACQUISITION;
        }
        else
        {
            SkillReleaseButton.GetComponent<Button>().interactable = true;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = BEFORE_ACQUISITION;
        }
    }

    /// <summary>
    /// ���͂��ꂽ�X�e�[�^�X�f�[�^��\������
    /// </summary>
    /// <param name="number">�X�e�[�^�X�̔ԍ�</param>
    public void DisplaySetStatus(int number)
    {
        m_skillButton.SelectNumber = number;

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Data_SkilDetail.SetActive(true);
        SkillStatus.SetActive(true);

        // �l���X�V����
        Data_Sprite.GetComponent<Image>().sprite =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillName;
        Data_SkilDetail.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillDetail;
        Data_SkillNecessaryEP.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].EnhancementPoint.ToString();
        GetElement(Data_Element, number, PlayerNumberManager.PlayerNumber);

        // ���Ɏg�p�ł����ԂȂ�{�^���������Ȃ��悤�ɂ���
        if (m_saveDataManager.SaveData.saveData.SkillRegisters[PlayerNumberManager.PlayerNumber].PlayerSkills[number] == true)
        {
            SkillReleaseButton.GetComponent<Button>().interactable = false;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = AFTER_ACQUISITION;
        }
        else
        {
            SkillReleaseButton.GetComponent<Button>().interactable = true;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = BEFORE_ACQUISITION;
        }
    }

    /// <summary>
    /// �����ϐ���\�����鏈��
    /// </summary>
    /// <param name="gameObjct">�Q�[���I�u�W�F�N�g</param>
    /// <param name="number">�X�L���̔ԍ�</param>
    /// <param name="playerNumber">�v���C���[�̔ԍ�</param>
    private void GetElement(GameObject gameObjct, int number,int playerNumber)
    {
        ElementType element = PlayerData.playerDataList[playerNumber].skillDataList[number].SkillElement;

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
    private void InstantiateSkillIcon()
    {
        for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // ���ʔԍ��������Ȃ�f�[�^������������
                if (PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillNumber == SkillData.skillDataList[dataNumber].SkillNumber)
                {
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillDetail = SkillData.skillDataList[dataNumber].SkillDetail;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
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
                PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillSprite,
                true,
                this
                );
            button.SetActive(true);
        }
    }

    /// <summary>
    /// �A�C�R���𐶐����鏈��
    /// </summary>
    private void InstantiateStatusIcon()
    {
        for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // ���ʔԍ��������Ȃ�f�[�^������������
                if (PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillNumber == SkillData.skillDataList[dataNumber].SkillNumber)
                {
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillDetail = SkillData.skillDataList[dataNumber].SkillDetail;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
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
            button.GetComponent<Image>().color = Color.cyan;
            // �R���|�[�l���g���擾
            var skillButton = button.GetComponent<SkillButton>();
            skillButton.SetPlayerEnhancement(
                skillNumber,
                PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillSprite,
                true,
                this
                );
            button.SetActive(true);
        }
    }
}
