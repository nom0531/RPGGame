using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawDamageSE : MonoBehaviour
{
    [SerializeField, Header("画像")]
    private Sprite[] Sprites;
    [SerializeField]
    private GameObject Image;

    private void Start()
    {
        SetSprite();
    }

    /// <summary>
    /// 画像をランダムに設定する
    /// </summary>
    private void SetSprite()
    {
        var rand = Random.Range(0, Sprites.Length);
        Image.GetComponent<Image>().sprite = Sprites[rand];
    }

    /// <summary>
    /// 自身を削除する
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
