using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyButton : MonoBehaviour
{
    // �G�l�~�[�̔ԍ�
    private int m_enemyNumber = -1;
    // �}�ӃV�X�e��
    private PictureBookSystem m_pictureBookSystem;

    /// <summary>
    /// �������p�̊֐��B�G�l�~�[��o�^����
    /// </summary>
    /// <param name="number">�G�l�~�[�̔ԍ�</param>
    /// <param name="enemyImage">�G�l�~�[�̉摜</param>
    /// <param name="interactable">�o�^�ς݂��ǂ���</param>
    public void SetPictureBook(int number,Sprite enemyImage,bool interactable,PictureBookSystem pictureBookSystem)
    { 
        // ���ꂼ��̒l��o�^����
        m_enemyNumber = number;
        GetComponent<Image>().sprite = enemyImage;
        // �����邩�ǂ����ݒ肷��
        GetComponent<Button>().interactable = interactable;
        // �}�ӃV�X�e����o�^����
        m_pictureBookSystem = pictureBookSystem;
    }

    /// <summary>
    /// �{�^���������ꂽ���̏���
    /// </summary>
    public void ButtonDown()
    {
        m_pictureBookSystem.DisplaySetValue(m_enemyNumber);
    }
}
