using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool m_isPause = false;                             // �|�[�Y��ʂ��ǂ���
    private bool m_isPushDown = false;                          // �{�^���������ꂽ���ǂ���

    public bool PauseFlag
    {
        get => m_isPause;
        set => m_isPushDown = value;
    }

    // Update is called once per frame
    void Update()
    {
        // �|�[�Y����
        if (m_isPushDown == true)
        {
            m_isPause = !m_isPause;     // �t���O�𔽓]������
            m_isPushDown = false;       // �t���O��߂�
        }
    }
}
