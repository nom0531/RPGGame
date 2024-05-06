using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using TMPro;

public class Title : MonoBehaviour
{
    [SerializeField, Tooltip("0��Text 1��UICanvas")]
    private Animator[] m_animators;
    private bool m_isPush = false;

    public void PlayAnimation()
    {
        if(m_isPush == true)
        {
            return;
        }
        m_isPush = true;
        m_animators[0].SetTrigger("ClickTitle");
        m_animators[1].SetTrigger("Home From Title");
        GameManager.Instance.PlayTimelineManager.Play(SeamlessNumber.enHomeFromTitle);
    }
}
