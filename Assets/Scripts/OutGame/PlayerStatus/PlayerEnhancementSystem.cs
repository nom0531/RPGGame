using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerEnhancementSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^"), Tooltip("�v���C���[�f�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField, Tooltip("�X�L���f�[�^")]
    private SkillDataBase SkillData;
    [SerializeField, Header("�v���C���[���X�g"), Tooltip("��������{�^��")]
    private GameObject PlayerIconButton;
    [SerializeField, Tooltip("�{�^����ǉ�����I�u�W�F�N�g")]
    private GameObject PlayerContent;
    [SerializeField, Header("�X�L�����X�g"), Tooltip("��������{�^��")]
    private GameObject SkillIconButton;
    [SerializeField, Tooltip("�{�^����ǉ�����I�u�W�F�N�g")]
    private GameObject SkillContent;
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

    private int m_playerNumber = 0;
    private SaveDataManager m_saveDataManager;
    private SkillButton m_skillButton;

    public int PlayerNumber
    {
        get => m_playerNumber;
        set => m_playerNumber = value;
    }

    /// <summary>
    /// ���ݑI�����Ă���v���C���[�̔ԍ���ݒ肷��
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    public void SetSelectPlayerNumber(int number)
    {
        m_playerNumber = number;
    }

    /// <summary>
    /// ���ݑI�����Ă���v���C���[�̔ԍ����擾����
    /// </summary>
    /// <returns>�v���C���[�̔ԍ�</returns>
    public int GetSelectPlayerNumber()
    {
        return m_playerNumber;
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
        Data_HaveEP.GetComponent<TextMeshProUGUI>().text =
            m_saveDataManager.SaveData.saveData.EnhancementPoint.ToString();

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
                playerNumber,                                           // �ԍ�
                PlayerData.playerDataList[playerNumber].PlayerSprite,       // �摜
                this
                );
        }

        m_skillButton = SkillReleaseButton.GetComponent<SkillButton>();
        // �\������
        DisplaySetValue(m_playerNumber);
    }

    /// <summary>
    /// ���͂��ꂽ�f�[�^��\�����鏈��
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    public void DisplaySetValue(int number)
    {
        m_skillButton.SetSelectPlayerNumber(number);
        DestroySkillIcon();

        Data_HaveEP.SetActive(true);
        Data_PlayerName.SetActive(true);
        SkillReleaseButton.GetComponent<Button>().interactable = false;

        // �l���X�V����
        Data_PlayerName.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[number].PlayerName;

        // ���݂̃v���C���[�̔ԍ����L�^
        m_playerNumber = number;

        InstantiateSkillIcon();
    }

    /// <summary>
    /// ���͂��ꂽ�X�L���f�[�^��\�����鏈��
    /// </summary>
    /// <param name="number">�X�L���̔ԍ�</param>
    public void DisplaySetSkill(int number)
    {
        m_skillButton.SetSelectSkillNUmber(number);

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Data_SkilDetail.SetActive(true);
        SkillStatus.SetActive(true);

        // �l���X�V����
        Data_Sprite.GetComponent<Image>().sprite =
            PlayerData.playerDataList[m_playerNumber].skillDataList[number].SkillSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[m_playerNumber].skillDataList[number].SkillName;
        Data_SkilDetail.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[m_playerNumber].skillDataList[number].SkillDetail;
        Data_SkillNecessaryEP.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[m_playerNumber].skillDataList[number].EnhancementPoint.ToString();
        GetElement(Data_Element, number, m_playerNumber);

        // ���Ɏg�p�ł����ԂȂ�{�^���������Ȃ��悤�ɂ���
        if (m_saveDataManager.SaveData.saveData.Players[m_playerNumber].PlayerEnhancement[number] == true)
        {
            SkillReleaseButton.GetComponent<Button>().interactable = false;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "����ς�";
        }
        else
        {
            SkillReleaseButton.GetComponent<Button>().interactable = true;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "�X�L�����";
        }
    }

    /// <summary>
    /// �����ϐ���\�����鏈��
    /// </summary>
    /// <param name="gameObjct">�Q�[���I�u�W�F�N�g</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="playerNumber">�v���C���[�̔ԍ�</param>
    void GetElement(GameObject gameObjct, int skillNumber,int playerNumber)
    {
        ElementType element = PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillElement;

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
                gameObjct.GetComponent<TextMeshProUGUI>().text = "�[";
                break;
        }
    }

    /// <summary>
    /// SkillIcon�^�O�̕t�����I�u�W�F�N�g��S�č폜���鏈��
    /// </summary>
    private void DestroySkillIcon()
    {
        var skillIcons = GameObject.FindGameObjectsWithTag("SkillIcon");

        foreach (var button in skillIcons)
        {
            Destroy(button);
        }
    }

    /// <summary>
    /// �X�L���A�C�R���𐶐����鏈��
    /// </summary>
    private void InstantiateSkillIcon()
    {
        for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[m_playerNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // ���ʔԍ��������Ȃ�f�[�^������������
                if (PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillNumber == SkillData.skillDataList[dataNumber].SkillNumber)
                {
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillDetail = SkillData.skillDataList[dataNumber].SkillDetail;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
                    break;
                }
            }

            // �{�^���𐶐�
            var button = Instantiate(SkillIconButton);
            button.transform.SetParent(SkillContent.transform);
            // �T�C�Y�𒲐�����
            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = Vector3.one;
            button.transform.localRotation = Quaternion.identity;
            // �R���|�[�l���g���擾
            var skillButton = button.GetComponent<SkillButton>();
            skillButton.SetPlayerEnhancement(
                skillNumber,
                PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillSprite,
                true,
                this
                );

            button.SetActive(true);
        }
    }
}
