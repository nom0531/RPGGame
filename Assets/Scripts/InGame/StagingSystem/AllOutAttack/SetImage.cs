using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImage : MonoBehaviour
{
    [SerializeField, Header("表示先")]
    private Image BackGround;
    [SerializeField]
    private Image Character;
    [SerializeField]
    private Image TextImage;
    [SerializeField, Tooltip("一枚絵")]
    private Sprite[] Sprites;
    [SerializeField]
    private Sprite[] Characters, TextImages;

    /// <summary>
    /// 背景の画像を設定する
    /// </summary>
    /// <param name="number">番号</param>
    public void SetBackGround(int number)
    {
        BackGround.sprite = Sprites[number];
    }

    /// <summary>
    /// キャラクターの画像を設定する
    /// </summary>
    /// <param name="number">番号</param>
    public void SetCharacter(int number)
    {
        Character.sprite = Characters[number];
    }

    /// <summary>
    /// テキストの画像を設定する
    /// </summary>
    /// <param name="number">番号</param>
    public void SetTextImage(int number)
    {
        TextImage.sprite = TextImages[number];
    }
}
