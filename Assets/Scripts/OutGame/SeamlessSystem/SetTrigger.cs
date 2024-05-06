using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTrigger : MonoBehaviour
{
    [SerializeField]
    Animator Animator;
    [SerializeField, Tooltip("Start()で再生するアニメーション")]
    SeamlessNumber SeamlessNumber;

    // Start is called before the first frame update
    private void Start()
    {
        PlayAnimation();
    }

    /// <summary>
    /// アニメーション処理
    /// </summary>
    private void PlayAnimation()
    {
        if(Animator == null)
        {
            return;
        }
        switch (SeamlessNumber)
        {
            case SeamlessNumber.enHomeFromTitle:
                Animator.SetTrigger("Home From Title");
                break;
            case SeamlessNumber.enPictureBookFromHome:
                Animator.SetTrigger("PictureBook From Home");
                break;
            case SeamlessNumber.enPlayerStatusFromHome:
                Animator.SetTrigger("PlayerStatus From Home");
                break;
            case SeamlessNumber.enQuestFromHome:
                Animator.SetTrigger("Quest From Home");
                break;
        }
    }
}
