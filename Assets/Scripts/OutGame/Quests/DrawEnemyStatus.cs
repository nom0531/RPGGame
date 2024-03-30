using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawEnemyStatus : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private EnemyDataBase EnemyData;

    private GameObject Data_EnemyName, Data_Fire, Data_Ice, Data_Wind, Data_Thunder, Data_Light, Data_Dark;
    private SaveDataManager m_saveDataManager;
    private int m_myNumber = 0;     // ���g�̔ԍ�

    public int MyNumber
    {
        set => m_myNumber = value;
    }
    
    /// <summary>
    /// �f�[�^��ݒ肷��
    /// </summary>
    public void SetObject(GameObject name, GameObject fire, GameObject ice, GameObject wind, GameObject thunder, GameObject light, GameObject dark)
    {
        Data_EnemyName = name;
        Data_Fire = fire;
        Data_Ice = ice;
        Data_Wind = wind;
        Data_Thunder = thunder;
        Data_Light = light;
        Data_Dark = dark;
    }

    /// <summary>
    /// �f�[�^��ݒ肷��
    /// </summary>
    public void SetData()
    {
        m_saveDataManager = GameManager.Instance.SaveDataManager;
        Data_EnemyName.GetComponent<TextMeshProUGUI>().text = SetName();
        SetResistance(Data_Fire, (int)ElementType.enFire);
        SetResistance(Data_Ice, (int)ElementType.enIce);
        SetResistance(Data_Wind, (int)ElementType.enWind);
        SetResistance(Data_Thunder, (int)ElementType.enThunder);
        SetResistance(Data_Light, (int)ElementType.enLight);
        SetResistance(Data_Dark, (int)ElementType.enDark);
    }

    private string SetName()
    {
        // �������Ă��Ȃ��Ȃ�
        if (m_saveDataManager.SaveData.saveData.EnemyRegisters[m_myNumber] == false)
        {
            return "�H�H�H";
        }
        return EnemyData.enemyDataList[m_myNumber].EnemyName;
    }

    /// <summary>
    /// �����ϐ���\�����鏈��
    /// </summary>
    /// <param name="gameObjct">�Q�[���I�u�W�F�N�g</param>

    /// <param name="elementNumber">�����̎��ʔԍ�</param>
    private void SetResistance(GameObject gameObjct, int elementNumber)
    {
        // �������Ă��Ȃ��Ȃ�
        if (m_saveDataManager.SaveData.saveData.ElementRegisters[m_myNumber].Elements[elementNumber] == false)
        {
            gameObjct.GetComponent<TextMeshProUGUI>().text = "�H";
            return;
        }
        // �������Ă���Ȃ�
        ElementResistance element = EnemyData.enemyDataList[m_myNumber].EnemyElement[elementNumber];

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
