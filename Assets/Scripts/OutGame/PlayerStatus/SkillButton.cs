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
    private EnhancementDataBase EnhancementData;
    [SerializeField]
    private PlayerDataBase PlayerData;
    [SerializeField, Header("�\���p�f�[�^"),Tooltip("����EP")]
    private GameObject Data_HaveEP;

    private PlayerEnhancementSystem m_playerEnhancement;
    private int m_skillNumber = -1;     // �ԍ�
    private int m_selectNumber = 0;     // ���ݑI�����Ă���ԍ�

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
    /// �������p�̊֐��BPlayerEnhancement������������
    /// </summary>
    public void SetPlayerEnhancement(PlayerEnhancementSystem playerEnhancement)
    {
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
        var gameManager = GameManager.Instance;
        SaveReleaseSkillData(gameManager);
        SaveReleaseStatusData(gameManager);
        // �ύX��̃e�L�X�g��\��
        Data_HaveEP.GetComponent<TextMeshProUGUI>().text =
        gameManager.SaveData.SaveData.saveData.EnhancementPoint.ToString();
        // �{�^���̃e�L�X�g��ύX����
        GetComponent<Button>().interactable = false;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "����ς�";

        gameManager.SaveData.Save();
    }

    /// <summary>
    /// ��������X�L���f�[�^���Z�[�u����
    /// </summary>
    private void SaveReleaseSkillData(GameManager gameManager)
    {
        if (m_playerEnhancement.ReferrenceSkillFlag == false)
        {
            return;
        }
        // �l��ݒ肷��
        gameManager.SaveData.SaveData.saveData.SkillRegisters[gameManager.PlayerNumber].PlayerSkills[m_selectNumber] = true;
        gameManager.SaveData.SaveData.saveData.EnhancementPoint -= SkillData.skillDataList[m_selectNumber].EnhancementPoint;
    }

    /// <summary>
    /// ��������X�e�[�^�X�̃f�[�^���Z�[�u����
    /// </summary>
    /// <param name="saveDataManager"></param>
    private void SaveReleaseStatusData(GameManager gameManager)
    {
        if (m_playerEnhancement.ReferrenceSkillFlag == true)
        {
            return;
        }
        // �l��ݒ肷��
        gameManager.SaveData.SaveData.saveData.EnhancementRegisters[gameManager.PlayerNumber].PlayerEnhancements[m_selectNumber] = true;
        gameManager.SaveData.SaveData.saveData.EnhancementPoint -= EnhancementData.enhancementDataList[m_selectNumber].EnhancementPoint;
        SavePlayerStatus(gameManager, EnhancementData.enhancementDataList[gameManager.PlayerNumber].AddValue);
    }

    /// <summary>
    /// �X�e�[�^�X���Z�[�u����
    /// </summary>
    private void SavePlayerStatus(GameManager gameManager, int addValue)
    {
        switch (EnhancementData.enhancementDataList[gameManager.PlayerNumber].EnhancementStatus)
        {
            case EnhancementStatus.enHP:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].HP += addValue;
                break;
            case EnhancementStatus.enSP:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].SP += addValue;
                break;
            case EnhancementStatus.enATK:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].ATK += addValue;
                break;
            case EnhancementStatus.enDEF:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].DEF += addValue;
                break;
            case EnhancementStatus.enSPD:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].SPD += addValue;
                break;
            case EnhancementStatus.enLUCK:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].LUCK += addValue;
                break;
        }
    }
}
