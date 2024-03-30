using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フェードの種類
/// </summary>
public enum FadeMode
{
    enSmall,    // 小さくする
    enBig,      // 大きくする
    enNone      // 何もしない
}

public class BGM : SingletonMonoBehaviour<BGM>
{
    [SerializeField, Header("フェードの速度")]
    private float FadeSpeed = 0.5f;

    /// <summary>
    /// フェード用のデータ
    /// </summary>
    private struct FadeDatas
    {
        public FadeMode m_fadeMode; // フェードの種類
        public bool m_isFade;       // フェード処理中ならture
        public float m_fadeSpeed;   // フェードの速度
        public float m_volume;      // 音量
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
    /// フェードを開始する
    /// </summary>
    public void FadeStart(FadeMode mode)
    {
        m_audioSource.Play();           // BGMを再生
        if (m_fadeDatas.m_fadeMode == FadeMode.enNone)
        {
            m_audioSource.volume = m_soundManager.BGMVolume;
            return;
        }
        m_fadeDatas.m_fadeMode = mode;
        m_fadeDatas.m_isFade = true;
        // 音量を再設定する
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
    /// フェード処理
    /// </summary>
    private void Fade()
    {
        if (m_fadeDatas.m_isFade == false)
        {
            return;
        }
        // 音量を変更する
        if (m_fadeDatas.m_fadeMode == FadeMode.enSmall)
        {
            // 音量を小さくする
            m_fadeDatas.m_volume -= m_fadeDatas.m_fadeSpeed * Time.deltaTime;
            m_audioSource.volume = m_fadeDatas.m_volume;
            // 音量が一定に達すれば終了
            if (m_fadeDatas.m_volume >= m_soundManager.BGMVolume)
            {
                m_fadeDatas.m_isFade = false;
                return;
            }
        }
        if (m_fadeDatas.m_fadeMode == FadeMode.enBig)
        {
            // 音量を大きくする
            m_fadeDatas.m_volume -= m_fadeDatas.m_fadeSpeed * Time.deltaTime;
            m_audioSource.volume = m_fadeDatas.m_volume;
            // 音量が一定に達すれば終了
            if (m_fadeDatas.m_volume <= 0.0f)
            {
                m_fadeDatas.m_isFade = false;
                return;
            }
        }
    }
}
