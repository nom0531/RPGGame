using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^"),Tooltip("�v���C���[�̃f�[�^")]
    private PlayerDataBase PlayerDataBase;
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
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"),Tooltip("�����̃e�L�X�g")]
    private GameObject Element_Text;
    [SerializeField, Tooltip("�X�e�[�^�X�̃e�L�X�g")]
    private GameObject Status_Text;

    /// <summary>
    /// ���͂��ꂽ�f�[�^��\�����鏈��
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    public void DisplaySetValue(int number)
    {
        // �ԍ����X�V
        PlayerNumberManager.PlayerNumber = number;

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Element_Text.SetActive(true);
        Status_Text.SetActive(true);

        // �l���X�V����
        Data_Sprite.GetComponent<Image>().sprite = PlayerDataBase.playerDataList[number].PlayerSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].PlayerName;
        // �����ϐ�
        GetResistance(Data_Fire, number, (int)ElementType.enFire);
        GetResistance(Data_Ice, number, (int)ElementType.enIce);
        GetResistance(Data_Wind, number, (int)ElementType.enWind);
        GetResistance(Data_Thunder, number, (int)ElementType.enThunder);
        GetResistance(Data_Light, number, (int)ElementType.enLight);
        GetResistance(Data_Dark, number, (int)ElementType.enDark);
        // �X�e�[�^�X
        Data_HP.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].HP.ToString("000");
        Data_SP.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].SP.ToString("000");
        Data_ATK.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].ATK.ToString("000");
        Data_DEF.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].DEF.ToString("000");
        Data_SPD.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].SPD.ToString("000");
        Data_LUCK.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].LUCK.ToString("000");
    }

    // Start is called before the first frame update
    void Start()
    {
        Data_Sprite.SetActive(false);
        Data_Name.SetActive(false);
        Element_Text.SetActive(false);
        Status_Text.SetActive(false);

        for (int i = 0; i < PlayerDataBase.playerDataList.Count; i++)
        {
            // �{�^���𐶐����Ďq�I�u�W�F�N�g�ɂ���
            var button = Instantiate(Button);
            button.transform.SetParent(Content.transform);
            // �T�C�Y�𒲐�
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;

            var playerButton = button.GetComponent<PlayerButton>();
            playerButton.SetPlayerStatus(
                i,                                               // �ԍ�
                PlayerDataBase.playerDataList[i].PlayerSprite,   // �摜
                this
                );
        }
        // �\������
        DisplaySetValue(PlayerNumberManager.PlayerNumber);
    }

    /// <summary>
    /// �����ϐ���\�����鏈��
    /// </summary>
    /// <param name="playerNumber">�v���C���[�̔ԍ�</param>
    /// <param name="elementNumber">�����̎��ʔԍ�</param>
    void GetResistance(GameObject gameObjct, int playerNumber, int elementNumber)
    {
        ElementResistance element = PlayerDataBase.playerDataList[playerNumber].PlayerElement[elementNumber];

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
}
