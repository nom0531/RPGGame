using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private SkillDataBase SkillData;
    [SerializeField]
    private PlayerDataBase PlayerData;
    [SerializeField, Header("�\���p�f�[�^"),Tooltip("����EP")]
    private GameObject Data_HaveEP;

    // �X�L���̔ԍ�
    private int m_skillNumber = -1;
    // ���ݑI�����Ă���ԍ�
    private int m_selectNumber = 0;
    // �}�ӃV�X�e��
    private PlayerEnhancementSystem m_playerEnhancement;

    public int SelectNumber
    {
        get => m_selectNumber;
        set => m_selectNumber = value;
    }

    /// <summary>
    /// �������p�̊֐��B�X�L����o�^����
    /// </summary>
    /// <param name="number">�X�L���̔ԍ�</param>
    /// <param name="skillImage">�X�L���̉摜</param>
    public void SetPlayerEnhancement(int number, Sprite skillImage,bool interactable, PlayerEnhancementSystem playerEnhancement)
    {
        // ���ꂼ��̒l��o�^����
        m_skillNumber = number;
        GetComponent<Image>().sprite = skillImage;
        GetComponent<Button>().interactable = interactable;
        // �}�ӃV�X�e����o�^����
        m_playerEnhancement = playerEnhancement;
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void ButtonDown()
    {
        if(m_playerEnhancement.ReferrenceSkillFlag == true)
        {
            m_playerEnhancement.DisplaySetSkill(m_skillNumber);
        }
        else
        {
            m_playerEnhancement.DisplaySetStatus(m_skillNumber);
        }
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void IsUseButtonDown()
    {
        var saveDataManager = GameManager.Instance.SaveData;

        SaveSkillData(saveDataManager);
        SaveStatusData(saveDataManager);

        Data_HaveEP.GetComponent<TextMeshProUGUI>().text =
        saveDataManager.SaveData.saveData.EnhancementPoint.ToString();
        // �{�^���̃e�L�X�g��ύX����
        GetComponent<Button>().interactable = false;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "����ς�";

        saveDataManager.Save();
    }

    /// <summary>
    /// ��������X�L���f�[�^���Z�[�u����
    /// </summary>
    private void SaveSkillData(SaveDataManager saveDataManager)
    {
        if(m_playerEnhancement.ReferrenceSkillFlag == false)
        {
            return;
        }

        // �l��ݒ肷��
        saveDataManager.SaveData.saveData.SkillRegisters[PlayerNumberManager.PlayerNumber].PlayerSkills[m_selectNumber] = true;
        saveDataManager.SaveData.saveData.EnhancementPoint -= SkillData.skillDataList[m_selectNumber].EnhancementPoint;
    }

    /// <summary>
    /// ��������X�e�[�^�X�̃f�[�^���Z�[�u����
    /// </summary>
    private void SaveStatusData(SaveDataManager saveDataManager)
    {
        if(m_playerEnhancement.ReferrenceSkillFlag == true)
        {
            return;
        }
        // �l��ݒ肷��
    }
}
