using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawDamageSE : MonoBehaviour
{
    [SerializeField, Header("�摜")]
    private Sprite[] Sprites;
    [SerializeField]
    private GameObject Image;

    private void Start()
    {
        SetSprite();
    }

    /// <summary>
    /// �摜�������_���ɐݒ肷��
    /// </summary>
    private void SetSprite()
    {
        var rand = Random.Range(0, Sprites.Length);
        Image.GetComponent<Image>().sprite = Sprites[rand];
    }

    /// <summary>
    /// ���g���폜����
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
