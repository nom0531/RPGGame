using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGMNumber
{
    enOutGame,
    enInGame,
    enTitle,
    enNum
}

public enum SENumber
{
    enButtonDown,
    enSceneChange,
    enChancel,
    enDeleteSave,
    enUseEP,
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

    private BGM m_bgm;
    private BGMNumber m_BGMNumber = BGMNumber.enOutGame;    // 現在再生しているBGM
    private float m_BGMVolume = VOLUME;
    private float m_SEVolume = VOLUME;

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

    new private void Awake()
    {
        CheckInstance();
        DontDestroyOnLoad(gameObject);
        var saveDataManager = GameManager.Instance.SaveDataManager;
        m_BGMVolume = saveDataManager.SaveData.saveData.BGMVolume;
        m_SEVolume = saveDataManager.SaveData.saveData.SEVolume;
    }

    private void Start()
    {
        m_bgm = GameObject.FindGameObjectWithTag("BGM").GetComponent<BGM>();
        PlayBGM(BGMNumber.enInGame, FadeMode.enNone);
    }

    /// <summary>
    /// BGMの音量を変更する
    /// </summary>
    public void SetBGMVolume()
    {
        m_bgm.AudioSource.volume = BGMVolume;
    }

    /// <summary>
    /// BGMを再生する
    /// </summary>
    /// <param name="number">番号</param>
    /// <param name="mode">フェードの種類</param>
    public void PlayBGM(BGMNumber number, FadeMode mode)
    {
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
