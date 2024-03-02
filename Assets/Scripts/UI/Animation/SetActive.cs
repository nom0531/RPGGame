using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    [SerializeField, Header("Activeを設定するオブジェクト")]
    private GameObject[] GameObjects;

    /// <summary>
    /// オブジェクトを非表示にする
    /// </summary>
    public void SetActive_False()
    {
        for(int i= 0; i < GameObjects.Length; i++)
        {
            GameObjects[i].SetActive(false);
        }
    }

    /// <summary>
    /// オブジェクトを表示する
    /// </summary>
    public void SetActive_True()
    {
        for (int i = 0; i < GameObjects.Length; i++)
        {
            GameObjects[i].SetActive(true);
        }
    }
}
