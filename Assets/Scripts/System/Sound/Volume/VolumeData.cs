using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeData : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject[] VolumeVar;

    private SaveDataManager m_saveDataManager;
    private SoundManager m_soundManager;

    // Start is called before the first frame update
    void Start()
    {
        m_saveDataManager = GameManager.Instance.SaveDataManager;
        m_soundManager = GameManager.Instance.SoundManager;
    }

    /// <summary>
    /// �Z�[�u�f�[�^�ɋL�^����
    /// </summary>
    public void Save()
    {
        m_saveDataManager.SaveData.saveData.BGMVolume = m_soundManager.BGMVolume;
        m_saveDataManager.SaveData.saveData.SEVolume = m_soundManager.SEVolume;
        m_saveDataManager.Save();
    }

    /// <summary>
    /// �f�[�^�����Z�b�g����
    /// </summary>
    public void Reset()
    {
        VolumeVar[0].GetComponent<Slider>().value = m_soundManager.DefaultVolume;
        VolumeVar[1].GetComponent<Slider>().value = m_soundManager.DefaultVolume;
    }
}
