using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public enum SeamlessNumber
{
    enHomeFromTitle,
    enPictureBookFromHome,
    enPlayerStatusFromHome,
    enQuestFromHome,
    enHomeFromPictureBook,
    enHomeFromPlayerStatus,
    enHomeFromQuest,
}

public class PlayTimelineManager : MonoBehaviour
{
    [SerializeField] 
    private PlayableDirector[] m_directors;

    /// <summary>
    /// �^�C�����C���̍Đ�����
    /// </summary>
    /// <param name="number">�Đ�����^�C�����C���̔ԍ�</param>
    public void Play(SeamlessNumber number)
    {
        m_directors[(int)number].Play();
    }
}
