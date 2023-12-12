using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"),Tooltip("�t�F�[�h���s���L�����o�X")]
    private GameObject FadeCanvas;
    [SerializeField, Tooltip("�J�ڐ�̃V�[����")]
    private string SceneName;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            string sceneName = SceneName;

            // �󔒂̏ꍇ�͌��݂̃V�[���̖��O���g�p����
            if (SceneName == "")
            {
                sceneName = SceneManager.GetActiveScene().name;
            }

            // �t�F�[�h���J�n����
            var fadeCanvas = Instantiate(FadeCanvas);
            fadeCanvas.GetComponent<FadeScene>().FadeStart(sceneName);
        }
    }
}
