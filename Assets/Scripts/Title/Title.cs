using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using TMPro;

public class Title : MonoBehaviour
{
    // �t�F�[�h���[�h
    private enum FadeMode
    {
        enWhite,
        enBlack,
    }

    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"),Tooltip("�t�F�[�h���s���L�����o�X")]
    private GameObject FadeCanvas;
    [SerializeField, Tooltip("�J�ڐ�̃V�[����")]
    private string SceneName;
    [SerializeField, Tooltip("�A�j���[�V��������e�L�X�g")]
    private GameObject AnimationText;

    private Animator m_animator;                // �A�j���[�^�[

    private void Start()
    {
        m_animator = AnimationText.GetComponent<Animator>();
    }

    /// <summary>
    /// �V�[����؂�ւ���
    /// </summary>
    public void SceneChange()
    {
        var sceneName = SceneName;
        PlayAnimation();
        // �t�F�[�h���J�n����
        var fadeCanvas = Instantiate(FadeCanvas);
        fadeCanvas.GetComponent<FadeScene>().FadeStart(sceneName);
    }

    private void PlayAnimation()
    {
        m_animator.SetTrigger("ClickTitle");
    }
}
