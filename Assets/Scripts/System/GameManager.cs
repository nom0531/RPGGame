using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject SaveDataObject;

    private SaveDataManager m_saveDataManager;

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

    new private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (m_saveDataManager == null)
        {
            Instantiate(SaveDataObject);
            // ���݂��Ă��Ȃ��Ȃ�T��
            m_saveDataManager = FindObjectOfType<SaveDataManager>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_saveDataManager.Save();
            Debug.Log("�Z�[�u������I");
        }
    }
}