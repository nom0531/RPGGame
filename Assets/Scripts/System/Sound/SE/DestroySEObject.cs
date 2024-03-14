using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySEObject : MonoBehaviour
{
    private AudioSource m_audioSource;
    private bool m_isPlay = false;          // �Đ����J�n�����Ȃ�true

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
        // �Đ����I�����Ă���Ȃ玩�g���폜����
        if (m_audioSource.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}
