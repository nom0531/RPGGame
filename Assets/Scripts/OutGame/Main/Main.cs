using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private GameManager m_gameManager;

    private void Awake()
    {
        m_gameManager = GameManager.Instance;
        PlayTimeline();
    }

    private void Start()
    {
        GetComponent<SeamlessManager>().AddScene(m_gameManager.SceneNumber);
    }

    /// <summary>
    /// ƒ^ƒCƒ€ƒ‰ƒCƒ“‚ÌÄ¶ˆ—
    /// </summary>
    private void PlayTimeline()
    {
        switch (m_gameManager.SceneNumber)
        {
            case SceneNumber.enTitle:
                return;
            case SceneNumber.enHome:
                m_gameManager.PlayTimelineManager.Play(SeamlessNumber.enHomeFromTitle);
                return;
            case SceneNumber.enPictureBook:
                m_gameManager.PlayTimelineManager.Play(SeamlessNumber.enPictureBookFromHome);
                return;
            case SceneNumber.enPlayerStatus:
                m_gameManager.PlayTimelineManager.Play(SeamlessNumber.enPlayerStatusFromHome);
                return;
            case SceneNumber.enQuest:
                m_gameManager.PlayTimelineManager.Play(SeamlessNumber.enQuestFromHome);
                return;
        }
    }
}
