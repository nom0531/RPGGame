using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawDamageText : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject DamageObject;

    private const float TEXT_SCALE = 1.0f;

    /// <summary>
    /// �_���[�W��\������
    /// </summary>
    /// <param name="damage">�_���[�W��</param>
    /// <param name="parentObject">�e�I�u�W�F�N�g</param>
    /// <param name="isReverse">�e�L�X�g�𔽓]�����邩�ǂ����Btrue�Ȃ甽�]������</param>
    public void ViewDamage(int damage , GameObject parentObject,bool isReverse = false)
    {
        GameObject gameObject = Instantiate(
            DamageObject,               // ��������I�u�W�F�N�g
            parentObject.transform      // �����ʒu
            );

        gameObject.GetComponent<TextMeshProUGUI>().text = damage.ToString();

        if (isReverse == true)
        {
            gameObject.transform.localScale = new Vector3(-TEXT_SCALE, TEXT_SCALE, TEXT_SCALE);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(TEXT_SCALE, TEXT_SCALE, TEXT_SCALE);
        }
    }
}