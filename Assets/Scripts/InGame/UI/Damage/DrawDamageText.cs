using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawDamageText : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject DamageObject;

    private const float TEXT_SCALE = 1.0f;

    /// <summary>
    /// ダメージを表示する
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <param name="parentObject">親オブジェクト</param>
    /// <param name="isReverse">テキストを反転させるかどうか。trueなら反転させる</param>
    public void ViewDamage(int damage , GameObject parentObject,bool isReverse = false)
    {
        GameObject gameObject = Instantiate(
            DamageObject,               // 生成するオブジェクト
            parentObject.transform      // 生成位置
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