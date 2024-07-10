using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImage : MonoBehaviour
{
    [SerializeField, Header("�\����")]
    private Image BackGround;
    [SerializeField]
    private Image Character;
    [SerializeField]
    private Image TextImage;
    [SerializeField, Tooltip("�ꖇ�G")]
    private Sprite[] Sprites;
    [SerializeField]
    private Sprite[] Characters, TextImages;

    /// <summary>
    /// �w�i�̉摜��ݒ肷��
    /// </summary>
    /// <param name="number">�ԍ�</param>
    public void SetBackGround(int number)
    {
        BackGround.sprite = Sprites[number];
    }

    /// <summary>
    /// �L�����N�^�[�̉摜��ݒ肷��
    /// </summary>
    /// <param name="number">�ԍ�</param>
    public void SetCharacter(int number)
    {
        Character.sprite = Characters[number];
    }

    /// <summary>
    /// �e�L�X�g�̉摜��ݒ肷��
    /// </summary>
    /// <param name="number">�ԍ�</param>
    public void SetTextImage(int number)
    {
        TextImage.sprite = TextImages[number];
    }
}
