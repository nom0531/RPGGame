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
    // ���ݑI�����Ă���X�L���̔ԍ�
    private int m_selectSkillNumber = 0;
    private int m_selectPlayerNumber = 0;
    // �}�ӃV�X�e��
    private PlayerEnhancementSystem m_playerEnhancement;

    /// <summary>
    /// �I�����Ă���X�L���̔ԍ���������
    /// </summary>
    /// <param name="number">�X�L���̔ԍ�</param>
    public void SetSelectSkillNUmber(int number)
    {
        m_selectSkillNumber = number;
    }

    /// <summary>
    /// �I�����Ă���v���C���[�̔ԍ���������
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    public void SetSelectPlayerNumber(int number)
    {
        m_selectPlayerNumber = number;
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
        m_playerEnhancement.DisplaySetSkill(m_skillNumber);
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void IsUseButtonDown()
    {
        var saveDataManager = GameManager.Instance.SaveData;

        // �l��ݒ肷��
        saveDataManager.SaveData.saveData.SkillRegisters[m_selectPlayerNumber].PlayerSkills[m_selectSkillNumber] = true;
        saveDataManager.SaveData.saveData.EnhancementPoint -= SkillData.skillDataList[m_selectSkillNumber].EnhancementPoint;
        Data_HaveEP.GetComponent<TextMeshProUGUI>().text =
            saveDataManager.SaveData.saveData.EnhancementPoint.ToString();

        // �{�^���̃e�L�X�g��ύX����
        GetComponent<Button>().interactable = false;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "����ς�";

        saveDataManager.Save();
    }
}
