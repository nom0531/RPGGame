using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField]
    private GameObject SaveDataObject, SoundObject, AnimationObject;

    private SaveDataManager m_saveDataManager;
    private SoundManager m_soundManamager;
    private int m_selectLevelNumber = 0;
    private int m_selectPlayerNumber = 0;

    public SaveDataManager SaveDataManager
    {
        get
        {
            if (m_saveDataManager == null)
            {
                m_saveDataManager = FindObjectOfType<SaveDataManager>();
            }
            return m_saveDataManager;
        }
    }

    public SoundManager SoundManager
    {
        get
        {
            if (m_soundManamager == null)
            {
                m_soundManamager = FindObjectOfType<SoundManager>();
            }
            return m_soundManamager;
        }
    }

    public int PlayerNumber
    {
        get => m_selectPlayerNumber;
        set => m_selectPlayerNumber = value;
    }

    public int LevelNumber
    {
        get => m_selectLevelNumber;
        set => m_selectLevelNumber = value;
    }

    private void Start()
    {
        // 自身はシーンを跨いでも削除されないようにする
        DontDestroyOnLoad(gameObject);
        // オブジェクトを作成する
        var saveDataObject = Instantiate(SaveDataObject);
        m_saveDataManager = saveDataObject.GetComponent<SaveDataManager>();
        m_saveDataManager.AnimationObject = AnimationObject;
        Instantiate(SoundObject);
        m_soundManamager = SoundObject.GetComponent<SoundManager>();
        Physics.autoSimulation = false;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveDataManager.Save();
        }
    }
#endif
}