using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE : MonoBehaviour
{
    [SerializeField, Header("Ä¶‚·‚éSE‚Ì”Ô†")]
    private SENumber SENumber;

    public SENumber Number
    {
        set => SENumber = value;
    }

    private SoundManager m_soundManager;

    // Start is called before the first frame update
    private void Start()
    {
        m_soundManager = GameManager.Instance.SoundManager;
    }

    /// <summary>
    /// SE‚ğÄ¶‚·‚é
    /// </summary>
    public void PlaySE()
    {
        m_soundManager.PlaySE(SENumber);
    }
}
