using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PictureBookSystem : MonoBehaviour
{
    [SerializeField, Header("�}�Ӑݒ�"), Tooltip("�G�l�~�[�f�[�^")]
    private EnemyDataBase EnemyDataBase;
    [SerializeField, Header("�G�l�~�[���X�g"), Tooltip("��������{�^��")]
    private GameObject Button;
    [SerializeField,Tooltip("�{�^����ǉ�����I�u�W�F�N�g")]
    private GameObject Content;
    [SerializeField, Header("�\���p�f�[�^"), Tooltip("���O")]
    private GameObject Data_Name;
    [SerializeField, Tooltip("�Q�[�������f��")]
    private GameObject Data_Sprite;
    [SerializeField,Tooltip("����")]
    private GameObject Data_Detail;
    [SerializeField, Header("����"),Tooltip("���ϐ�")]
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
    [SerializeField, Tooltip("�}�Ӕԍ�")]
    private GameObject Data_EnemyNumber;
    [SerializeField,Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject Element_Text;
    [SerializeField]
    private GameObject RegistrationRateText;

    private SaveDataManager m_saveDataManager;      // �Z�[�u�f�[�^
    private int m_enemyCount = 0;                   // �����Ă���G�l�~�[�̐�
    private int m_elementCount = 0;                 // �����Ă��鑮����

    /// <summary>
    /// ���͂��ꂽ�f�[�^��\�����鏈��
    /// </summary>
    /// <param name="number">�G�l�~�[�̔ԍ�</param>
    public void DisplaySetValue(int number)
    {
        // �l��\������
        Data_Name.SetActive(true);
        Data_Detail.SetActive(true);
        Data_Name.SetActive(true);
        Data_Sprite.SetActive(true);
        Element_Text.SetActive(true);

        // �l���X�V����
        Data_Name.GetComponent<TextMeshProUGUI>().text = EnemyDataBase.enemyDataList[number].EnemyName;
        Data_Detail.GetComponent<TextMeshProUGUI>().text = EnemyDataBase.enemyDataList[number].EnemyDetail;
        Data_Sprite.GetComponent<Image>().sprite = EnemyDataBase.enemyDataList[number].EnemySprite;
        Data_EnemyNumber.GetComponent<TextMeshProUGUI>().text = (number + 1).ToString("00");
        // �����ϐ�
        GetResistance(Data_Fire, number, (int)ElementType.enFire);
        GetResistance(Data_Ice, number, (int)ElementType.enIce);
        GetResistance(Data_Wind, number, (int)ElementType.enWind);
        GetResistance(Data_Thunder, number, (int)ElementType.enThunder);
        GetResistance(Data_Light, number, (int)ElementType.enLight);
        GetResistance(Data_Dark, number, (int)ElementType.enDark);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_enemyCount = 0;
        m_elementCount = 0;

        // �l���\���ɂ���
        Data_Name.SetActive(false);
        Data_Detail.SetActive(false);
        Element_Text.SetActive(false);
        Data_Sprite.SetActive(false);

        m_saveDataManager = GameManager.Instance.SaveData;

        for (int i = 0; i < EnemyDataBase.enemyDataList.Count; i++)
        {
            // �{�^���𐶐����Ďq�I�u�W�F�N�g�ɂ���
            var button = Instantiate(Button);
            button.transform.SetParent(Content.transform);
            // �T�C�Y�A���W�𒲐�
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;

            // �������Ă��Ȃ��Ȃ�摜���Â�����
            if (m_saveDataManager.SaveData.saveData.EnemyRegister[i] == false)
            {
                button.GetComponent<Image>().color = Color.black;
            }
            else
            {
                // �����Ă���Ȃ�J�E���g
                m_enemyCount++;

                for(int elementNumber = 0; elementNumber < (int)ElementType.enNum; elementNumber++)
                {
                    if(m_saveDataManager.SaveData.saveData.ElementRegister[i].Elements[elementNumber] != true)
                    {
                        break;
                    }

                    m_elementCount++;
                }
            }

            // �I�u�W�F�N�g�𐶐�����
            var enemyButton = button.GetComponent<EnemyButton>();
            enemyButton.SetPictureBook(
                i,                                                          // �ԍ�
                EnemyDataBase.enemyDataList[i].EnemySprite,                     // �摜
                m_saveDataManager.SaveData.saveData.EnemyRegister[i],  // �������Ă��邩�ǂ���
                this
                );
        }

        RegistrationRate();
    }

    /// <summary>
    /// �o�^�����擾����
    /// </summary>
    /// <returns>�o�^��</returns>
    private void RegistrationRate()
    {
        float rate = 0.0f;
        // �G�l�~�[�̑����ƃG�������g�̑���
        int allValue = EnemyDataBase.enemyDataList.Count + (EnemyDataBase.enemyDataList.Count * (int)ElementType.enNum);
        // ���ݔ������Ă���G�l�~�[�̐��ƃG�������g�̐�
        int Value = m_enemyCount + m_elementCount;

        rate = (float)Value / (float)allValue;
        rate *= 100;

        string text = $"{rate.ToString("F1")}%";

        RegistrationRateText.GetComponent<TextMeshProUGUI>().text = text;
    }

    /// <summary>
    /// �����ϐ���\�����鏈��
    /// </summary>
    /// <param name="gameObjct">�Q�[���I�u�W�F�N�g</param>
    /// <param name="enemyNumber">�G�l�~�[�̔ԍ�</param>
    /// <param name="elementNumber">�����̎��ʔԍ�</param>
    void GetResistance(GameObject gameObjct,int enemyNumber,int elementNumber)
    {
        // �������Ă��Ȃ��Ȃ�
        if (m_saveDataManager.SaveData.saveData.ElementRegister[enemyNumber].Elements[elementNumber] == false)
        {
            gameObjct.GetComponent<TextMeshProUGUI>().text = "�H";
            return;
        }

        // �������Ă���Ȃ�
        ElementResistance element = EnemyDataBase.enemyDataList[enemyNumber].EnemyElement[elementNumber];

        switch (element)
        {
            case ElementResistance.enResist:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "��";
                break;
            case ElementResistance.enWeak:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "��";
                break;
            case ElementResistance.enNormal:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "�[";
                break;
        }
    }
}