using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerButton : MonoBehaviour
{
    // �v���C���[�̔ԍ�
    private int m_playerNumber = -1;
    // �}�ӃV�X�e��
    private PlayerStatusSystem m_playerStatus;
    private PlayerEnhancementSystem m_playerEnhancement;

    /// <summary>
    /// �������p�̊֐��B�v���C���[��o�^����
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    /// <param name="playerImage">�v���C���[�̉摜</param>
    public void SetPlayerStatus(int number, Sprite playerImage, PlayerStatusSystem playerStatus)
    {
        // ���ꂼ��̒l��o�^����
        m_playerNumber = number;
        GetComponent<Image>().sprite = playerImage;
        // �}�ӃV�X�e����o�^����
        m_playerStatus = playerStatus;
    }

    /// <summary>
    /// �������p�̊֐��B�v���C���[��o�^����
    /// </summary>
    /// <param name="number">�v���C���[�̔ԍ�</param>
    /// <param name="playerImage">�v���C���[�̉摜</param>
    public void SetPlayerEnhancement(int number, Sprite playerImage, PlayerEnhancementSystem playerEnhancement)
    {
        // ���ꂼ��̒l��o�^����
        m_playerNumber = number;
        GetComponent<Image>().sprite = playerImage;
        // �}�ӃV�X�e����o�^����
        m_playerEnhancement = playerEnhancement;
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void PlayerStatusButtonDown()
    {
        m_playerStatus.DisplaySetValue(m_playerNumber);
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void PlayerEnhancementButtoonDown()
    {
        if(m_playerEnhancement.ReferrenceSkillFlag == true)
        {
            m_playerEnhancement.DisplaySetSkillData(m_playerNumber);
            return;
        }
        else
        {
            m_playerEnhancement.DisplaySetStatusData(m_playerNumber);
        }
    }

    /// <summary>
    /// �X�L���f�[�^�̕\��
    /// </summary>
    public void PlayerEnhancement_SkillButtonDown()
    {
        m_playerEnhancement.DisplaySetSkillData(PlayerNumberManager.PlayerNumber);
    }

    /// <summary>
    /// �X�e�[�^�X�����̕\��
    /// </summary>
    public void PlayerEnhancement_StatusButtonDown()
    {
        m_playerEnhancement.DisplaySetStatusData(PlayerNumberManager.PlayerNumber);
    }
}
