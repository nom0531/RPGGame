using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTimer : MonoBehaviour
{
    private float m_timer = 0.0f;
    private bool m_isInteractableFlag = false;  // �{�^���������邩�ǂ���

    public float Timer
    {
        get => m_timer;
        set => m_timer = value;
    }

    public bool InteractableFlag
    {
        get => m_isInteractableFlag;
        set => m_isInteractableFlag = value;
    }

    public void ReSetTimer()
    {
        Timer = 0.0f;
        InteractableFlag = false;
    }
}
