using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSaveAnimation : MonoBehaviour
{
    [SerializeField, Header("�摜")]
    private Sprite Sprite;
    [SerializeField]
    private Sprite[] ChangeSprites;

    /// <summary>
    /// �摜�������ւ���
    /// </summary>
    public void ChangeSprite()
    {
        // �摜�������_���ɍ����ւ���
        var imageNumber = Random.Range(0, ChangeSprites.Length);
        transform.GetChild(0).GetComponent<Image>().sprite = ChangeSprites[imageNumber];
    }

    /// <summary>
    /// �摜�����ɖ߂�
    /// </summary>
    public void ReturnSprite()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = Sprite;
    }
}
