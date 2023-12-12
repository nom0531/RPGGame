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
        get => m_saveDataManager.GetComponent<SaveDataManager>();
    }

    new private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instantiate(SaveDataObject);

        m_saveDataManager = FindObjectOfType<SaveDataManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_saveDataManager.GetComponent<SaveDataManager>().Save();
            Debug.Log("�Z�[�u������I");
        }
    }
}