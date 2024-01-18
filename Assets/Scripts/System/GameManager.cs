using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject SaveDataObject;

    private SaveDataManager m_saveDataManager;

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

    new private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (m_saveDataManager == null)
        {
            Instantiate(SaveDataObject);
            // 存在していないなら探す
            m_saveDataManager = FindObjectOfType<SaveDataManager>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_saveDataManager.Save();
            Debug.Log("セーブしたよ！");
        }
    }
}