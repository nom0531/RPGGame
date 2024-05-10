using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"), Tooltip("�t�F�[�h���J�n����L�����o�X")]
    private GameObject FadeCanvas;
    [SerializeField, Header("�ǉ��Ń��[�h����V�[��")]
    SceneNumber SceneNumber;

    private bool m_push = false;

    /// <summary>
    /// �V�[����J�ڂ���
    /// </summary>
    /// <param name="sceneName">�J�ڐ�̃V�[����</param>
    public void SceneChange(string sceneName)
    {
        if(m_push == true)
        {
            return;
        }
        m_push = true;
        // �t�F�[�h���J�n����
        var fadeCanvas = Instantiate(FadeCanvas);
        fadeCanvas.GetComponent<FadeScene>().FadeStart(sceneName);
        // �Z�[�u
        var gameManager = GameManager.Instance;
        if(sceneName == "OutGame_Main")
        {
            gameManager.SceneNumber = SceneNumber;      // �ǉ��Ń��[�h����V�[����ݒ�
        }
        gameManager.SaveDataManager.Save(false);
    }
}
