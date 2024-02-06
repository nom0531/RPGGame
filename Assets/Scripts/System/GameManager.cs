using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
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
                // ���݂��Ă��Ȃ��Ȃ�T��
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

    new private void Awake()
    {
        if (m_saveDataManager == null)
        {
            Instantiate(SaveDataObject);
            // ���݂��Ă��Ȃ��Ȃ�T��
            m_saveDataManager = FindObjectOfType<SaveDataManager>();
        }
        else
        {
            return;
        }

        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
        Physics.autoSimulation = false;
    }
}