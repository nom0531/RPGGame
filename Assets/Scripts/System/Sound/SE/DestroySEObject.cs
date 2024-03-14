using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySEObject : MonoBehaviour
{
    private AudioSource m_audioSource;
    private bool m_isPlay = false;          // 再生を開始したならtrue

    public bool PlayFlag
    {
        get => m_isPlay;
        set => m_isPlay = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (PlayFlag == false)
        {
            return;
        }
        // 再生が終了しているなら自身を削除する
        if (m_audioSource.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}
