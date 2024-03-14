using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト"), Tooltip("0はsound、1はsystem")]
    private GameObject[] SystemObject;

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        SystemObject[0].SetActive(true);
        SystemObject[1].SetActive(false);
    }

    /// <summary>
    /// リセットする
    /// </summary>
    public void Reset()
    {
        Init();
    }
}
