using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBGM : MonoBehaviour
{
    [SerializeField, Header("再生するBGM")]
    private BGMNumber BGMNumber;

    private SoundManager m_soundManager;

    private void Start()
    {
        m_soundManager = GameManager.Instance.SoundManager;
    }

    /// <summary>
    /// BGMを変更する
    /// </summary>
    public void Change()
    {
        //m_soundManager.PlayBGM(m_soundManager.BGMNumber, FadeMode.enSmall);
        m_soundManager.PlayBGM(BGMNumber, FadeMode.enBig);
    }
}
