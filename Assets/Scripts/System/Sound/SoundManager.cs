using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGM�̔ԍ�
/// </summary>
public enum BGMNumber
{
    enOutGame,
    enInGame,
    enTitle,
    enNone,         // �����Ȃ�
    enNum
}

/// <summary>
/// SE�̔ԍ�
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
    [SerializeField, Header("�T�E���h")]
    private AudioClip[] BGMSounds = new AudioClip[(int)BGMNumber.enNum];
    [SerializeField]
    private AudioClip[] SESounds = new AudioClip[(int)SENumber.enNum];
    [SerializeField, Header("��������I�u�W�F�N�g")]
    private GameObject SEObject;

    private const float MAX = 1.0f;
    private const float MIN = 0.0f;
    private const float VOLUME = 0.5f;

    private GameManager m_gameManager;
    private BGM m_bgm;
    private BGMNumber m_BGMNumber = BGMNumber.enOutGame;    // ���ݍĐ����Ă���BGM
    private bool m_isInit = false;                          // �����������������Ȃ�ture
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
    /// ���ʂ̏�����
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
    /// BGM�̉��ʂ�ύX����
    /// </summary>
    public void SetBGMVolume()
    {
        m_bgm = GameObject.FindGameObjectWithTag("BGM").GetComponent<BGM>();
        m_bgm.AudioSource.volume = BGMVolume;
    }

    /// <summary>
    /// BGM���Đ�����
    /// </summary>
    /// <param name="number">�ԍ�</param>
    /// <param name="mode">�t�F�[�h�̎��</param>
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
    /// SE���Đ�����
    /// </summary>
    /// <param name="number">�ԍ�</param>
    public void PlaySE(SENumber number)
    {
        var gameObject = Instantiate(SEObject);
        var audioSouse = gameObject.GetComponent<AudioSource>();
        // ���y�̍Đ����J�n����
        gameObject.GetComponent<DestroySEObject>().PlayFlag = true;
        audioSouse.volume = SEVolume;
        audioSouse.PlayOneShot(SESounds[(int)number]);
    }
}
