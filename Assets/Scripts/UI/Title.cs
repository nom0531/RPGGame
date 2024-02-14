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
    [SerializeField, Header("�e�L�X�g�̖���"), Tooltip("���ł�����e�L�X�g")]
    private GameObject Text;
    [SerializeField, Tooltip("���ł����鑬�x")]
    private float Speed = 1.0f;

    private PlayableDirector playableDirector;  // �^�C�����C���̐���

    // �e�L�X�g�f�[�^
    private TextMeshProUGUI m_text;
    private Color m_color = Color.black;        // �J���[ 
    private float m_alpha = 1.0f;               // �����x
    private FadeMode m_fadeMode = FadeMode.enWhite;

    private void Start()
    {
        playableDirector = Camera.main.GetComponent<PlayableDirector>();
        m_text = Text.GetComponent<TextMeshProUGUI>();          // �e�L�X�g���b�V�����擾
        m_color = m_text.color;
    }

    // Update is called once per frame
    private void Update()
    {
        TextBlink();
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
    /// �e�L�X�g�𖾖ł�����
    /// </summary>
    private void TextBlink()
    {
        // �t�F�[�h����
        if (m_fadeMode == FadeMode.enBlack)
        {
            // ��ʂ��Â�����
            m_alpha += Speed * Time.deltaTime;

            // ���S�ɈÂ��Ȃ����̂ŃV�[����ύX
            if (m_alpha >= 1.0f)
            {
                m_fadeMode = FadeMode.enWhite;
            }
        }
        else
        {
            // ��ʂ𖾂邭����
            m_alpha -= Speed * Time.deltaTime;
            // ���S�ɖ��邭�Ȃ����̂Ŏ��g���폜����
            if (m_alpha <= 0.0f)
            {
                m_fadeMode = FadeMode.enBlack;
            }
        }
        // �s�����x��ݒ肷��
        m_text.color = new Vector4(m_color.r, m_color.g, m_color.b, m_alpha);
    }
}
