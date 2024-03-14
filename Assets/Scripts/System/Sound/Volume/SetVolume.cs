using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// サウンドのステート
/// </summary>
public enum SoundState
{
    enBGM,
    enSE
}

public class SetVolume : MonoBehaviour
{
    [SerializeField]
    private SoundState SoundState;

    private SoundManager m_soundManager;
    private VolumeData m_volumeData;
    private Slider m_slider;

    private void Start()
    {
        m_soundManager = GameManager.Instance.SoundManager;
        m_slider = GetComponent<Slider>();
        InitRate();
    }

    private void Update()
    {
        SetRate();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void InitRate()
    {
        var rate = 0.0f;
        switch (SoundState)
        {
            case SoundState.enBGM:
                rate = m_soundManager.BGMVolume;
                m_soundManager.SetBGMVolume();
                break;
            case SoundState.enSE:
                rate = m_soundManager.SEVolume;
                break;
        }
        m_slider.value = rate;
    }

    /// <summary>
    /// 音量を設定する
    /// </summary>
    private void SetRate()
    {
        if (gameObject.activeSelf == false)
        {
            return;         // オブジェクトが非表示なら実行しない
        }
        var volume = m_slider.value;
        switch (SoundState)
        {
            case SoundState.enBGM:
                m_soundManager.BGMVolume = volume;
                m_soundManager.SetBGMVolume();
                break;
            case SoundState.enSE:
                m_soundManager.SEVolume = volume;
                break;
        }
    }
}