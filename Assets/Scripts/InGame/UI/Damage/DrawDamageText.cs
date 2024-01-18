using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawDamageText : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject DamageObject;

    private const float TEXT_SCALE = 0.5f;

    /// <summary>
    /// �_���[�W��\������
    /// </summary>
    /// <param name="text">�\������e�L�X�g</param>
    /// <param name="parentObject">�e�I�u�W�F�N�g</param>
    /// <param name="isReverse">�e�L�X�g�𔽓]�����邩�ǂ����Btrue�Ȃ甽�]������</param>
    public void ViewDamage(string text , GameObject parentObject,bool isReverse = false)
    {
        var gameObject = Instantiate(
            DamageObject,               // ��������I�u�W�F�N�g
            parentObject.transform      // �����ʒu
            );
        // �e�L�X�g����
        gameObject.GetComponent<TextMeshProUGUI>().text = text;
        // �e�L�X�g�𔽓]������
        if (isReverse == true)
        {
            gameObject.transform.localScale = new Vector3(-TEXT_SCALE, TEXT_SCALE, TEXT_SCALE);
            gameObject.transform.position = parentObject.transform.position;
            return;
        }
        gameObject.transform.localScale = new Vector3(TEXT_SCALE, TEXT_SCALE, TEXT_SCALE);
        gameObject.transform.position = parentObject.transform.position;
    }
}