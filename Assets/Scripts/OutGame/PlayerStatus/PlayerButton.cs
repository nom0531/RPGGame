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
    /// <param name="enemyImage">�v���C���[�̉摜</param>
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
    public void PlayerStatusButtoonDown()
    {
        m_playerStatus.DisplaySetValue(m_playerNumber);
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void PlayerEnhancementButtoonDown()
    {
        m_playerEnhancement.DisplaySetValue(m_playerNumber);
    }
}
