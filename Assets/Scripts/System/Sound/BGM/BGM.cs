using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �t�F�[�h�̎��
/// </summary>
public enum FadeMode
{
    enSmall,    // ����������
    enBig,      // �傫������
    enNone      // �������Ȃ�
}

public class BGM : SingletonMonoBehaviour<BGM>
{
    [SerializeField, Header("�t�F�[�h�̑��x")]
    private float FadeSpeed = 0.5f;

    /// <summary>
    /// �t�F�[�h�p�̃f�[�^
    /// </summary>
    private struct FadeDatas
    {
        public FadeMode m_fadeMode; // �t�F�[�h�̎��
        public bool m_isFade;       // �t�F�[�h�������Ȃ�ture
        public float m_fadeSpeed;   // �t�F�[�h�̑��x
        public float m_volume;      // ����
    }

    private AudioSource m_audioSource;
    private SoundManager m_soundManager;
    private FadeDatas m_fadeDatas;

    public AudioSource AudioSource
    {
        get => m_audioSource;
    }

    public bool FadeFlag
    {
        get => m_fadeDatas.m_isFade;
    }

    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        m_audioSource = GetComponent<AudioSource>();
        m_soundManager = GameManager.Instance.SoundManager;
        m_fadeDatas.m_fadeSpeed = FadeSpeed;
    }

    /// <summary>
    /// �t�F�[�h���J�n����
    /// </summary>
    public void FadeStart(FadeMode mode)
    {
        m_audioSource.Play();           // BGM���Đ�
        if (m_fadeDatas.m_fadeMode == FadeMode.enNone)
        {
            m_audioSource.volume = m_soundManager.BGMVolume;
            return;
        }
        m_fadeDatas.m_fadeMode = mode;
        m_fadeDatas.m_isFade = true;
        // ���ʂ��Đݒ肷��
        if (mode == FadeMode.enSmall)
        {
            m_fadeDatas.m_volume = m_soundManager.BGMVolume;
        }
        else
        {
            m_fadeDatas.m_volume = 0.0f;
        }
    }

    private void Update()
    {
        Fade();
    }

    /// <summary>
    /// �t�F�[�h����
    /// </summary>
    private void Fade()
    {
        if (m_fadeDatas.m_isFade == false)
        {
            return;
        }
        // ���ʂ�ύX����
        if (m_fadeDatas.m_fadeMode == FadeMode.enSmall)
        {
            // ���ʂ�����������
            m_fadeDatas.m_volume -= m_fadeDatas.m_fadeSpeed * Time.deltaTime;
            m_audioSource.volume = m_fadeDatas.m_volume;
            // ���ʂ����ɒB����ΏI��
            if (m_fadeDatas.m_volume >= m_soundManager.BGMVolume)
            {
                m_fadeDatas.m_isFade = false;
                return;
            }
        }
        if (m_fadeDatas.m_fadeMode == FadeMode.enBig)
        {
            // ���ʂ�傫������
            m_fadeDatas.m_volume -= m_fadeDatas.m_fadeSpeed * Time.deltaTime;
            m_audioSource.volume = m_fadeDatas.m_volume;
            // ���ʂ����ɒB����ΏI��
            if (m_fadeDatas.m_volume <= 0.0f)
            {
                m_fadeDatas.m_isFade = false;
                return;
            }
        }
    }
}
