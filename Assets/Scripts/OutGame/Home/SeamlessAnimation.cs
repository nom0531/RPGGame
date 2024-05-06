using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeamlessAnimation : MonoBehaviour
{
    [SerializeField, Header("�Đ�����A�j���[�V����")]
    private SeamlessNumber TimelineNumber;
    [SerializeField]
    private string TriggerName;

    private Animator m_animator;
    private PlayTimelineManager m_playTimelineManager;
    private bool m_isPush = false;

    private void Start()
    {
        m_animator = transform.parent.GetComponent<Animator>();
        m_playTimelineManager = GameManager.Instance.PlayTimelineManager;
        m_isPush = false;
    }

    /// <summary>
    /// �A�j���[�V�����̍Đ�����
    /// </summary>
    public void PlayAnimation()
    {
        if(m_isPush == true)
        {
            return;
        }
        m_isPush = true;
        m_animator.SetTrigger(TriggerName);
        m_playTimelineManager.Play(TimelineNumber);
    }
}
