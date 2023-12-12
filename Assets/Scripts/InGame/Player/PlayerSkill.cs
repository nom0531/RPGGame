using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^"), Tooltip("�v���C���[�̃f�[�^")]
    private PlayerDataBase PlayerDataBase;
    [SerializeField, Tooltip("��������{�^��")]
    private GameObject Button;
    [SerializeField, Tooltip("�{�^����ǉ�����I�u�W�F�N�g")]
    private GameObject Content;
    [SerializeField, Header("�\���p�f�[�^"), Tooltip("���g�̑���")]
    private GameObject Data_Element;
    [SerializeField, Tooltip("�X�L���̌���")]
    private GameObject Data_SkilDetail;
    [SerializeField, Tooltip("�����l")]
    private GameObject Data_SkillNecessary;
    [SerializeField, Tooltip("��������邩�̃e�L�X�g")]
    private GameObject Data_SkillNecessaryText;
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"), Tooltip("�X�L���̃e�L�X�g")]
    private GameObject Skill_Status;
    [SerializeField]
    private GameObject Skill_ElementText;
    [SerializeField]
    private GameObject Skill_NecessaryText;

    private int m_playerNumber;                 // �v���C���[�̔ԍ�
    private int m_selectSkillNumber = -1;       // ���ݑI�����Ă���X�L���̔ԍ�
    private BattleManager m_battleManager;

    public int SelectSkillNumber
    {
        get => m_selectSkillNumber;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();

        // �X�L���f�[�^
        Data_Element.SetActive(false);
        Data_SkillNecessary.SetActive(false);
        Data_SkillNecessaryText.SetActive(false);
        Data_SkilDetail.SetActive(false);
        Skill_Status.SetActive(false);
        Skill_ElementText.SetActive(false);
        Skill_NecessaryText.SetActive(false);
    }

    /// <summary>
    /// SkillButton�^�O���t�����S�ẴI�u�W�F�N�g���폜����
    /// </summary>
    public void DestroySkillButton()
    {
        GameObject[] skillButtons = GameObject.FindGameObjectsWithTag("SkillButton");

        foreach (GameObject button in skillButtons)
        {
            Debug.Log("�{�^�����폜");
            Destroy(button);
        }
    }

    /// <summary>
    /// �{�^���𐶐����鏈��
    /// </summary>
    public void InstantiateSkillButton()
    {
        SaveDataManager saveData = GameManager.Instance.SaveData;
        m_playerNumber = m_battleManager.OperatingPlayerNumber;

        for (int i = 0; i < PlayerDataBase.playerDataList[m_playerNumber].skillDataList.Count; i++)
        {
            // ���ɃX�L�����������Ă���Ȃ�
            if (saveData.SaveData.saveData.Players[m_playerNumber].PlayerEnhancement[i] == false)
            {
                continue;
            }

            // �{�^���𐶐����Ďq�I�u�W�F�N�g�ɂ���
            GameObject gameObject = Instantiate(Button);
            gameObject.transform.SetParent(Content.transform);
            // �T�C�Y�𒲐�
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

            PlayerSkillButton skillButton = gameObject.GetComponent<PlayerSkillButton>();
            skillButton.SetPlayerSkill(
                i,                                                                  // �ԍ�
                PlayerDataBase.playerDataList[m_playerNumber].skillDataList[i].SkillName,   // ���O
                Color.black,
                this
                );

            skillButton.GetComponent<PlayerSkillButton>().SkillNumber = i;          // �ԍ���������
        }
    }

    /// <summary>
    /// ���͂��ꂽ�X�L���f�[�^��\�����鏈��
    /// </summary>
    /// <param name="number">�X�L���̔ԍ�</param>
    public void DisplaySetPlayerSkill(int number)
    {
        Data_Element.SetActive(true);
        Data_SkillNecessary.SetActive(true);
        Data_SkillNecessaryText.SetActive(true);
        Data_SkilDetail.SetActive(true);
        Skill_Status.SetActive(true);
        Skill_ElementText.SetActive(true);
        Skill_NecessaryText.SetActive(true);

        // �l���X�V����
        Data_SkilDetail.GetComponent<TextMeshProUGUI>().text =
            PlayerDataBase.playerDataList[m_playerNumber].skillDataList[number].SkillDetail;
        Data_SkillNecessary.GetComponent<TextMeshProUGUI>().text =
            PlayerDataBase.playerDataList[m_playerNumber].skillDataList[number].SkillNecessary.ToString();
        GetElement(Data_Element, number, m_playerNumber);
        GetNecessaryText(Data_SkillNecessaryText, number, m_playerNumber);

        // �l���Z�b�g����
        m_selectSkillNumber = number;
    }

    /// <summary>
    /// �����f�[�^���擾����
    /// </summary>
    /// <param name="gameObject">�Q�[���I�u�W�F�N�g</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="playerNumber">�v���C���[�̔ԍ�</param>
    private void GetNecessaryText(GameObject gameObject,int skillNumber,int playerNumber)
    {
        NecessaryType necessary = PlayerDataBase.playerDataList[playerNumber].skillDataList[skillNumber].Type;

        switch(necessary)
        {
            case NecessaryType.enSP:
                gameObject.GetComponent<TextMeshProUGUI>().text = "SP";
                break;
            case NecessaryType.enHP:
                gameObject.GetComponent<TextMeshProUGUI>().text = "HP";
                break;
        }
    }

    /// <summary>
    /// �����ϐ���\�����鏈��
    /// </summary>
    /// <param name="gameObjct">�Q�[���I�u�W�F�N�g</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    /// <param name="playerNumber">�v���C���[�̔ԍ�</param>
    private void GetElement(GameObject gameObjct, int skillNumber, int playerNumber)
    {
        ElementType element = PlayerDataBase.playerDataList[playerNumber].skillDataList[skillNumber].SkillElement;

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
            case ElementType.enNone:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "��";
                break;
            default:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "�[";
                break;
        }
    }

    /// <summary>
    /// �I�����Ă���X�L���̔ԍ������Z�b�g����
    /// </summary>
    public void ResetSelectSkillNumber()
    {
        m_selectSkillNumber = -1;

        Data_Element.SetActive(false);
        Data_SkillNecessary.SetActive(false);
        Data_SkillNecessaryText.SetActive(false);
        Data_SkilDetail.SetActive(false);
        Skill_Status.SetActive(false);
        Skill_ElementText.SetActive(false);
        Skill_NecessaryText.SetActive(false);
    }
}
