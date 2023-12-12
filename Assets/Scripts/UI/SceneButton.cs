using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"), Tooltip("�t�F�[�h���J�n����L�����o�X")]
    private GameObject FadeCanvas;

    /// <summary>
    /// �V�[����J�ڂ���
    /// </summary>
    /// <param name="sceneName">�J�ڐ�̃V�[����</param>
    public void SceneChange(string sceneName)
    {
        // �t�F�[�h���J�n����
        var fadeCanvas = Instantiate(FadeCanvas);
        fadeCanvas.GetComponent<FadeScene>().FadeStart(sceneName);
    }
}
