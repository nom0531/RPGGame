using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^"),Tooltip("�v���C���[�̃f�[�^")]
    private PlayerDataBase PlayerData;
    [SerializeField, Header("�v���C���[���X�g"), Tooltip("��������{�^��")]
    private GameObject Button;
    [SerializeField, Tooltip("�{�^����ǉ�����I�u�W�F�N�g")]
    private GameObject Content;
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
    [SerializeField]
    private GameObject Data_AddHP, Data_AddSP, Data_AddATK, Data_AddDEF, Data_AddSPD, Data_AddLUCK;
    [SerializeField, Header("�X�L���f�[�^"), Tooltip("�R���e���c")]
    private GameObject SkillDataContent;
    [SerializeField, Tooltip("�X�L���A�C�R��")]
    private GameObject SkillDataIcon;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"),Tooltip("�����̃e�L�X�g")]
    private GameObject Element_Text;
    [SerializeField, Tooltip("�X�e�[�^�X�̃e�L�X�g")]
    private GameObject Status_Text;

    private GameManager m_gameManager;      // �Q�[���}�l�[�W���[

    // Start is called before the first frame update
    private void Start()
    {
        m_gameManager = GameManager.Instance;
        var playerNumber = m_gameManager.PlayerNumber;
        // �f�[�^���\��
        Data_Sprite.SetActive(false);
        Data_Name.SetActive(false);
        Element_Text.SetActive(false);
        Status_Text.SetActive(false);
        // �{�^���𐶐�
        for (int i = 0; i < PlayerData.playerDataList.Count; i++)
        {
            // �{�^���𐶐����Ďq�I�u�W�F�N�g�ɂ���
            var button = Instantiate(Button);
            button.transform.SetParent(Content.transform);
            // �T�C�Y�𒲐�
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;

            var playerButton = button.GetComponent<PlayerButton>();
            playerButton.SetPlayerStatus(
                i,                                              // �ԍ�
                PlayerData.playerDataList[i].PlayerSprite,      // �摜
                this
                );
        }
        DisplaySetValue(playerNumber);
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
        // �l���X�V����
        Data_Sprite.GetComponent<Image>().sprite = PlayerData.playerDataList[number].PlayerSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text = PlayerData.playerDataList[number].PlayerName;
        SetData(number);
        InstantiateSkillDataButton();
    }

    /// <summary>
    /// �f�[�^�̐ݒ�
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    private void SetData(int number)
    {
        // �����ϐ�
        GetResistance(Data_Fire, number, (int)ElementType.enFire);
        GetResistance(Data_Ice, number, (int)ElementType.enIce);
        GetResistance(Data_Wind, number, (int)ElementType.enWind);
        GetResistance(Data_Thunder, number, (int)ElementType.enThunder);
        GetResistance(Data_Light, number, (int)ElementType.enLight);
        GetResistance(Data_Dark, number, (int)ElementType.enDark);
        // �X�e�[�^�X
        Data_HP.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].HP.ToString("000")}";
        Data_SP.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].SP.ToString("000")}";
        Data_ATK.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].ATK.ToString("000")}";
        Data_DEF.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].DEF.ToString("000")}";
        Data_SPD.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].SPD.ToString("000")}";
        Data_LUCK.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].LUCK.ToString("000")}";
        // ���Z�l
        Data_AddHP.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enHP).ToString("000")}";
        Data_AddSP.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enSP).ToString("000")}";
        Data_AddATK.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enATK).ToString("000")}";
        Data_AddDEF.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enDEF).ToString("000")}";
        Data_AddSPD.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enSPD).ToString("000")}";
        Data_AddLUCK.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enLUCK).ToString("000")}";
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
    /// ���Z�l���擾����
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    /// <returns>���Z�l</returns>
    private int GetAddStatus(int number, EnhancementStatus enhancementStatus)
    {
        var addValue = 0;
        for(int i = 0; i< PlayerData.playerDataList[number].enhancementDataList.Count; i++)
        {
            if (m_gameManager.SaveData.SaveData.saveData.EnhancementRegisters[number].PlayerEnhancements[i] == false)
            {
                continue;   // �擾���Ă��Ȃ��Ȃ�X�L�b�v
            }
            if(enhancementStatus != PlayerData.playerDataList[number].enhancementDataList[i].EnhancementStatus)
            {
                continue;   // �������قȂ�Ȃ�X�L�b�v
            }
            addValue += PlayerData.playerDataList[number].enhancementDataList[i].AddValue;
        }
        return addValue;
    }

    /// <summary>
    /// �X�L���{�^���𐶐�
    /// </summary>
    private void InstantiateSkillDataButton()
    {
        DestroyIcon();

        for (int i = 0; i < PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList.Count; i++)
        {
            if (m_gameManager.SaveData.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[i] == false)
            {
                continue;   // �X�L�����o���Ă��Ȃ��Ȃ�X�L�b�v
            }
            // �{�^���𐶐����Ďq�I�u�W�F�N�g�ɂ���
            var button = Instantiate(SkillDataIcon);
            button.transform.SetParent(SkillDataContent.transform);
            // �T�C�Y�𒲐�
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = 
                PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList[i].SkillName;
        }
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
}
