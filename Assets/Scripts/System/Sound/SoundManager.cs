using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGMの番号
/// </summary>
public enum BGMNumber
{
    enOutGame,
    enInGame,
    enTitle,
    enNone,         // 流さない
    enNum
}

/// <summary>
/// SEの番号
/// </summary>
public enum SENumber
{
    enButtonDown,
    enSceneChange,
    enOK,
    enChancel,
    enDeleteSave,
    enUseEP,
    enCutin,
    enAttack,
    enMagicAttack,
    enWeak,
    enRegister,
    enAbsorption,
    enReflection,
    enHeal,
    enBuff,
    enDebuff,
    enWin,
    enLose,
    enNum
}

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField, Header("サウンド")]
    private AudioClip[] BGMSounds = new AudioClip[(int)BGMNumber.enNum];
    [SerializeField]
    private AudioClip[] SESounds = new AudioClip[(int)SENumber.enNum];
    [SerializeField, Header("生成するオブジェクト")]
    private GameObject SEObject;

    private const float MAX = 1.0f;
    private const float MIN = 0.0f;
    private const float VOLUME = 0.5f;

    private GameManager m_gameManager;
    private BGM m_bgm;
    private BGMNumber m_BGMNumber = BGMNumber.enOutGame;    // 現在再生しているBGM
    private bool m_isInit = false;                          // 初期化が完了したならture
    private float m_BGMVolume = 0.0f;
    private float m_SEVolume = 0.0f;

    public BGMNumber BGMNumber
    {
        get => m_BGMNumber;
    }

    public float DefaultVolume
    {
        get => VOLUME;
    }

    public float BGMVolume
    {
        get => m_BGMVolume;
        set
        {
            if(value > MAX)
            {
                value = MAX;
            }
            if(value < MIN)
            {
                value = MIN;
            }
            m_BGMVolume = value;
        }
    }

    public float SEVolume
    {
        get => m_SEVolume;
        set
        {
            if (value > MAX)
            {
                value = MAX;
            }
            if (value < MIN)
            {
                value = MIN;
            }
            m_SEVolume = value;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        InitVolume();
        SetBGMVolume();
        PlayBGM(BGMNumber.enTitle, FadeMode.enNone);
    }

    /// <summary>
    /// 音量の初期化
    /// </summary>
    private void InitVolume()
    {
        if(m_isInit == true)
        {
            return;
        }
        m_gameManager = GameManager.Instance;
        m_BGMVolume = m_gameManager.SaveDataManager.SaveData.saveData.BGMVolume;
        m_SEVolume = m_gameManager.SaveDataManager.SaveData.saveData.SEVolume;
        m_isInit = true;
}

    /// <summary>
    /// BGMの音量を変更する
    /// </summary>
    public void SetBGMVolume()
    {
        m_bgm = GameObject.FindGameObjectWithTag("BGM").GetComponent<BGM>();
        m_bgm.AudioSource.volume = BGMVolume;
    }

    /// <summary>
    /// BGMを再生する
    /// </summary>
    /// <param name="number">番号</param>
    /// <param name="mode">フェードの種類</param>
    public void PlayBGM(BGMNumber number, FadeMode mode)
    {
        if(number == BGMNumber.enNone)
        {
            return;
        }
        InitVolume();
        m_BGMNumber = number;
        m_bgm.AudioSource.clip = BGMSounds[(int)number];
        m_bgm.FadeStart(mode);
    }

    /// <summary>
    /// SEを再生する
    /// </summary>
    /// <param name="number">番号</param>
    public void PlaySE(SENumber number)
    {
        var gameObject = Instantiate(SEObject);
        var audioSouse = gameObject.GetComponent<AudioSource>();
        // 音楽の再生を開始する
        gameObject.GetComponent<DestroySEObject>().PlayFlag = true;
        audioSouse.volume = SEVolume;
        audioSouse.PlayOneShot(SESounds[(int)number]);
    }
}
