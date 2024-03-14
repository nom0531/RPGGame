using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeData : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
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
    /// セーブデータに記録する
    /// </summary>
    public void Save()
    {
        m_saveDataManager.SaveData.saveData.BGMVolume = m_soundManager.BGMVolume;
        m_saveDataManager.SaveData.saveData.SEVolume = m_soundManager.SEVolume;
        m_saveDataManager.Save();
    }

    /// <summary>
    /// データをリセットする
    /// </summary>
    public void Reset()
    {
        VolumeVar[0].GetComponent<Slider>().value = m_soundManager.DefaultVolume;
        VolumeVar[1].GetComponent<Slider>().value = m_soundManager.DefaultVolume;
    }
}
