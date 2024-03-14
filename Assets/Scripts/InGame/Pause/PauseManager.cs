using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool m_isPause = false;                             // ポーズ画面かどうか
    private bool m_isPushDown = false;                          // ボタンが押されたかどうか

    public bool PauseFlag
    {
        get => m_isPause;
        set => m_isPushDown = value;
    }

    // Update is called once per frame
    void Update()
    {
        // ポーズ処理
        if (m_isPushDown == true)
        {
            m_isPause = !m_isPause;     // フラグを反転させる
            m_isPushDown = false;       // フラグを戻す
        }
    }
}
