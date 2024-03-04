using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject SaveDataObject;

    private SaveDataManager m_saveDataManager;
    private int m_selectLevelNumber = 0;
    private int m_selectPlayerNumber = 0;

    public SaveDataManager SaveData
    {
        get
        {
            if (m_saveDataManager == null)
            {
                // 存在していないなら探す
                m_saveDataManager = FindObjectOfType<SaveDataManager>();
            }
            return m_saveDataManager;
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
        // セーブデータを作成する
        var saveDataObject = Instantiate(SaveDataObject);
        m_saveDataManager = saveDataObject.GetComponent<SaveDataManager>();
        Physics.autoSimulation = false;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveData.Save();
        }
    }
#endif
}