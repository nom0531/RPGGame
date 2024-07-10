using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawDamageSE : MonoBehaviour
{
    [SerializeField, Header("‰æ‘œ")]
    private Sprite[] Sprites;
    [SerializeField]
    private GameObject Image;

    private void Start()
    {
        SetSprite();
    }

    /// <summary>
    /// ‰æ‘œ‚ğƒ‰ƒ“ƒ_ƒ€‚Éİ’è‚·‚é
    /// </summary>
    private void SetSprite()
    {
        var rand = Random.Range(0, Sprites.Length);
        Image.GetComponent<Image>().sprite = Sprites[rand];
    }

    /// <summary>
    /// ©g‚ğíœ‚·‚é
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
