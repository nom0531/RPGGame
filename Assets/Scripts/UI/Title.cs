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

    private PlayableDirector playableDirector;  // �^�C�����C���̐���
    private Animator m_animator;                // �A�j���[�^�[

    private void Start()
    {
        playableDirector = Camera.main.GetComponent<PlayableDirector>();
        m_animator = AnimationText.GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        SceneChange();
    }

    /// <summary>
    /// �V�[����؂�ւ���
    /// </summary>
    private void SceneChange()
    {
        if (Input.GetMouseButtonDown(0) == false)
        {
            return;
        }
        PlayAnimation();
        var sceneName = SceneName;
        if (SceneName == "")
        {
            // �󔒂̏ꍇ�͌��݂̃V�[���̖��O���g�p����
            sceneName = SceneManager.GetActiveScene().name;
        }
        // �t�F�[�h���J�n����
        var fadeCanvas = Instantiate(FadeCanvas);
        fadeCanvas.GetComponent<FadeScene>().FadeStart(sceneName);
        // �^�C�����C�����Đ�
        playableDirector.Play();
    }

    /// <summary>
    /// �A�j���[�V�������Đ�����
    /// </summary>
    private void PlayAnimation()
    {
        m_animator.SetTrigger("ClickTitle");
    }
}
