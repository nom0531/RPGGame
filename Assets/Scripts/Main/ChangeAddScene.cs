using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAddScene : MonoBehaviour
{
    [SerializeField]
    private GameObject BGCanvas;

    private Animator m_animator;
    private SeamlessManager m_seamlessManager;

    // Start is called before the first frame update
    private void Start()
    {
        m_animator = BGCanvas.GetComponent<Animator>();
    }

    public void PlayAnimaton()
    {

    }
}
