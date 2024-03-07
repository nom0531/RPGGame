using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveDelete : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject GameObject;

    private const float TIMER = 3.0f;   // ボタンを押せるようになるまでの待機時間

    private Button m_button;
    private SetTimer m_setTimer;

    /// <summary>
    /// セーブデータを削除する
    /// </summary>
    public void Delete()
    {
        GameManager.Instance.SaveDataManager.Delete();
    }

    private void Start()
    {
        m_setTimer = GameObject.GetComponent<SetTimer>();
        m_button = GetComponent<Button>();
    }

    private void FixedUpdate()
    {
        WaitTime();
    }

    /// <summary>
    /// ボタンが押せるようになるまで待機する
    /// </summary>
    private void WaitTime()
    {
        // 既にボタンが押せるなら実行しない
        if(m_setTimer.InteractableFlag == true)
        {
            return;
        }
        m_button.interactable = false;
        m_setTimer.Timer += Time.deltaTime;

        // 値が一定以下なら実行しない
        if(m_setTimer.Timer <= TIMER)
        {
            return;
        }
        m_button.interactable = true;
        m_setTimer.InteractableFlag = true;
    }
}
