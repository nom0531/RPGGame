using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNumberManager : MonoBehaviour
{
    static private int m_playerNumber = 0;

    static public int PlayerNumber
    {
        get => m_playerNumber;
        set => m_playerNumber = value;
    }
}
