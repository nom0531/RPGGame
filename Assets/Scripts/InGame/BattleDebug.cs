using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleDebug : MonoBehaviour
{
    [SerializeField]
    private GameObject Text;
    [SerializeField]
    private int FrameRate = 60;

    private void Awake()
    {
        Application.targetFrameRate = FrameRate;
    }

    // Update is called once per frame
    private void Update()
    {
        float fps = 1f / Time.deltaTime;
        Text.GetComponent<TextMeshProUGUI>().text = $"{fps.ToString("00")}:fps";
    }
}
