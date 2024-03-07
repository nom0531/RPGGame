using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField, Header("�\���p�f�[�^"), Tooltip("�摜")]
    private GameObject Data_Sprite;
    [SerializeField, Tooltip("���O")]
    private GameObject Data_Name;
    [SerializeField, Header("����"), Tooltip("���ϐ�")]
    private GameObject Data_Fire;
    [SerializeField, Tooltip("�X�ϐ�")]
    private GameObject Data_Ice;
    [SerializeField, Tooltip("���ϐ�")]
    private GameObject Data_Wind;
    [SerializeField, Tooltip("���ϐ�")]
    private GameObject Data_Thunder;
    [SerializeField, Tooltip("���ϐ�")]
    private GameObject Data_Light;
    [SerializeField, Tooltip("�őϐ�")]
    private GameObject Data_Dark;
    [SerializeField, Header("��{�X�e�[�^�X")]
    private GameObject Data_HP;
    [SerializeField]
    private GameObject Data_SP, Data_ATK, Data_DEF, Data_SPD, Data_LUCK;
    [SerializeField, Header("�X�L���f�[�^"), Tooltip("�R���e���c")]
    private GameObject SkillDataContent;
    [SerializeField, Tooltip("�X�L���A�C�R��")]
    private GameObject SkillDataIcon;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"),Tooltip("�����̃e�L�X�g")]
    private GameObject Element_Text;
    [SerializeField, Tooltip("�X�e�[�^�X�̃e�L�X�g")]
    private GameObject Status_Text;
    [SerializeField, Header("�\������Canvas")]
    private GameObject Canvas;
    [SerializeField, Header("�������Ă���EP")]
    private GameObject HaveEP;
    [SerializeField, Header("�X�L���f�[�^")]
    private GameObject SkillName;
    [SerializeField]
    private GameObject SkillDetail, EnhancementPoint, SkillElement;
    [SerializeField, Header("OK�{�^��")]
    private GameObject OKButton;
    [SerializeField, Header("���ύX�{�^��")]
    private GameObject[] ChangeButton;

    private GameManager m_gameManager;      // �Q�[���}�l�[�W���[
    private List<PlayerButton> m_playerButtonList;

    // Start is called before the first frame update
    private void Start()
    {
        // �f�[�^���擾
        m_gameManager = GameManager.Instance;
        HaveEP.GetComponent<TextMeshProUGUI>().text = $"<sprite=1>{m_gameManager.SaveDataManager.SaveData.saveData.EnhancementPoint.ToString("N0")}";
        var playerButtonList = FindObjectsOfType<PlayerButton>();
        m_playerButtonList = new List<PlayerButton>(playerButtonList);
        // �f�[�^��\��
        DisplaySetValue(m_gameManager.PlayerNumber);
    }

    /// <summary>
    /// ���͂��ꂽ�f�[�^��\�����鏈��
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    public void DisplaySetValue(int number)
    {
        m_gameManager.PlayerNumber = number;    // �ԍ����X�V
        // �f�[�^��\��
        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Element_Text.SetActive(true);
        Status_Text.SetActive(true);
        SetData(number);
        InstantiateSkillDataButton();
    }

    /// <summary>
    /// �f�[�^�̐ݒ�
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    private void SetData(int number)
    {
        // �l���X�V����
        SpriteAnimation(number);
        Data_Name.GetComponent<TextMeshProUGUI>().text = PlayerData.playerDataList[number].PlayerName;
        // �����ϐ�
        GetResistance(Data_Fire, number, (int)ElementType.enFire);
        GetResistance(Data_Ice, number, (int)ElementType.enIce);
        GetResistance(Data_Wind, number, (int)ElementType.enWind);
        GetResistance(Data_Thunder, number, (int)ElementType.enThunder);
        GetResistance(Data_Light, number, (int)ElementType.enLight);
        GetResistance(Data_Dark, number, (int)ElementType.enDark);
        // �X�e�[�^�X
        Data_HP.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].HP.ToString("000")}";
        Data_SP.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].SP.ToString("000")}";
        Data_ATK.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].ATK.ToString("000")}";
        Data_DEF.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].DEF.ToString("000")}";
        Data_SPD.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].SPD.ToString("000")}";
        Data_LUCK.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].LUCK.ToString("000")}";
    }

    /// <summary>
    /// �摜�̃A�j���[�V����
    /// </summary>
    private void SpriteAnimation(int number)
    {
        var spriteAnimation = Data_Sprite.GetComponent<StatusAnimation>();

        if (m_playerButtonList[0].ButtonDownFlag == true)
        {
            Data_Sprite.GetComponent<Image>().sprite = PlayerData.playerDataList[number].PlayerSprite;
            ResetButton();
        }
        if (m_playerButtonList[1].ButtonDownFlag == true)
        {
            Data_Sprite.GetComponent<Image>().sprite = PlayerData.playerDataList[number].PlayerSprite;
            ResetButton();
            return;
        }
    }

    /// <summary>
    /// �����ϐ����擾����
    /// </summary>
    /// <param name="playerNumber">�v���C���[�̔ԍ�</param>
    /// <param name="elementNumber">�����̎��ʔԍ�</param>
    private void GetResistance(GameObject gameObjct, int playerNumber, int elementNumber)
    {
        var element = PlayerData.playerDataList[playerNumber].PlayerElement[elementNumber];
        switch (element)
        {
            case ElementResistance.enResist:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "��";
                break;
            case ElementResistance.enWeak:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "��";
                break;
            case ElementResistance.enNormal:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "-";
                break;
        }
    }

    /// <summary>
    /// �f�[�^��\��
    /// </summary>
    public void DrawData(int skillNumber)
    {
        // �|�C���g������Ȃ��Ȃ�{�^���͉����Ȃ�
        if (SkillData.skillDataList[skillNumber].EnhancementPoint > m_gameManager.SaveDataManager.SaveData.saveData.EnhancementPoint)
        {
            OKButton.GetComponent<Button>().interactable = false;
        }
        // �f�[�^��ݒ肷��
        SkillName.GetComponent<TextMeshProUGUI>().text = SkillData.skillDataList[skillNumber].SkillName;
        SkillDetail.GetComponent<TextMeshProUGUI>().text = SkillData.skillDataList[skillNumber].SkillDetail;
        SkillElement.GetComponent<TextMeshProUGUI>().text = $"���� {SetElementData(skillNumber)}";
        // �{�^�����ێ����Ă���ԍ����X�V
        OKButton.GetComponent<SkillButton>().MyNumber = skillNumber;
    }

    /// <summary>
    /// �f�[�^���擾���ă{�^���̃e�L�X�g��ύX����
    /// </summary>
    public void GetData(int skillNumber)
    {
        if (m_gameManager.SaveDataManager.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[skillNumber] == true)
        {
            // �{�^���̃e�L�X�g��ύX����
            OKButton.GetComponent<Button>().interactable = false;
            OKButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "����ς�";
            return;
        }
        // �|�C���g������Ȃ��Ȃ�{�^���͉����Ȃ�
        if (SkillData.skillDataList[skillNumber].EnhancementPoint > m_gameManager.SaveDataManager.SaveData.saveData.EnhancementPoint)
        {
            OKButton.GetComponent<Button>().interactable = false;
            EnhancementPoint.GetComponent<TextMeshProUGUI>().text = $"�K�v<sprite=1>{SkillData.skillDataList[skillNumber].EnhancementPoint}";
            return;
        }
        // �{�^���̃e�L�X�g��ύX����
        OKButton.GetComponent<Button>().interactable = true;
        EnhancementPoint.GetComponent<TextMeshProUGUI>().text = $"�K�v<sprite=1>{SkillData.skillDataList[skillNumber].EnhancementPoint}";
    }

    /// <summary>
    /// ��������X�L���f�[�^���Z�[�u����
    /// </summary>
    public void SaveReleaseSkillData(int skillNumber)
    {
        // �l��ݒ肷��
        m_gameManager.SaveDataManager.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[skillNumber] = true;
        m_gameManager.SaveDataManager.SaveData.saveData.EnhancementPoint -= SkillData.skillDataList[skillNumber].EnhancementPoint;
        // �l���X�V
        HaveEP.GetComponent<TextMeshProUGUI>().text = $"<sprite=1>{m_gameManager.SaveDataManager.SaveData.saveData.EnhancementPoint.ToString("N0")}";
        m_gameManager.SaveDataManager.Save();
    }

    /// <summary>
    /// ������ݒ肷��
    /// </summary>
    /// <returns>������</returns>
    private string SetElementData(int skillNumber)
    {
        var element = "";
        switch (SkillData.skillDataList[skillNumber].SkillElement)
        {
            case ElementType.enFire:
                element = "��";
                break;
            case ElementType.enIce:
                element = "�X";
                break;
            case ElementType.enWind:
                element = "��";
                break;
            case ElementType.enThunder:
                element = "��";
                break;
            case ElementType.enLight:
                element = "��";
                break;
            case ElementType.enDark:
                element = "��";
                break;
            case ElementType.enNone:
                element = "�[";
                break;
        }
        return element;
    }

    /// <summary>
    /// Active��؂�ւ���
    /// </summary>
    public void SetActiveTrue()
    {
        Canvas.SetActive(true);
    }

    /// <summary>
    /// �X�L���{�^���𐶐�
    /// </summary>
    private void InstantiateSkillDataButton()
    {
        DestroyIcon();

        for (int i = 0; i < PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList.Count; i++)
        {
            // �{�^���𐶐����Ďq�I�u�W�F�N�g�ɂ���
            var button = Instantiate(SkillDataIcon);
            button.transform.SetParent(SkillDataContent.transform);
            ChangeName(button, i);
            // �T�C�Y�𒲐�
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;
            // ���g�̔ԍ���������
            button.GetComponent<SkillButton>().MyNumber = PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList[i].ID;
            // Animator���������Ă���I�u�W�F�N�g��������
            button.GetComponent<UIAnimation>().Animator = Canvas;
        }
    }

    /// <summary>
    /// ���O��ύX����
    /// </summary>
    public void ChangeName(GameObject gameObject, int skillNumber)
    {
        if (m_gameManager.SaveDataManager.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[skillNumber] == true)
        {
            gameObject.GetComponent<Button>().interactable = true;
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"�@�@{PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList[skillNumber].SkillName}";
            return;
        }
        // ����ł���ꍇ��<sprite=1>���g�p����B

        // ����ł��Ȃ��ꍇ�͉����Ȃ��悤�ɂ���
        //gameObject.GetComponent<Button>().interactable = false;
        gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                        $"<sprite=3>{PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList[skillNumber].SkillName}";
    }

    /// <summary>
    /// SkillNameIcon�^�O�̕t�����I�u�W�F�N�g��S�č폜���鏈��
    /// </summary>
    private void DestroyIcon()
    {
        var skillIcons = GameObject.FindGameObjectsWithTag("SkillNameIcon");
        foreach (var button in skillIcons)
        {
            Destroy(button);
        }
    }

    /// <summary>
    /// ���Z�b�g����
    /// </summary>
    private void ResetButton()
    {
        for(int i= 0; i < ChangeButton.Length; i++)
        {
            m_playerButtonList[i].ResetButtonDownFlag();
        }
    }
}
