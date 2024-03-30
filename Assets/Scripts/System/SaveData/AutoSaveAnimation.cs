using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSaveAnimation : MonoBehaviour
{
    [SerializeField, Header("‰æ‘œ")]
    private Sprite Sprite;
    [SerializeField]
    private Sprite[] ChangeSprites;

    /// <summary>
    /// ‰æ‘œ‚ğ·‚µ‘Ö‚¦‚é
    /// </summary>
    public void ChangeSprite()
    {
        // ‰æ‘œ‚ğƒ‰ƒ“ƒ_ƒ€‚É·‚µ‘Ö‚¦‚é
        var imageNumber = Random.Range(0, ChangeSprites.Length);
        transform.GetChild(0).GetComponent<Image>().sprite = ChangeSprites[imageNumber];
    }

    /// <summary>
    /// ‰æ‘œ‚ğŒ³‚É–ß‚·
    /// </summary>
    public void ReturnSprite()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = Sprite;
    }
}
