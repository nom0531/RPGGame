using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawDamageText : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject DamageObject;

    private const float TEXT_SCALE = 0.5f;

    /// <summary>
    /// ダメージを表示する
    /// </summary>
    /// <param name="text">表示するテキスト</param>
    /// <param name="parentObject">親オブジェクト</param>
    /// <param name="isReverse">テキストを反転させるかどうか。trueなら反転させる</param>
    public void ViewDamage(string text , GameObject parentObject,bool isReverse = false)
    {
        var gameObject = Instantiate(
            DamageObject,               // 生成するオブジェクト
            parentObject.transform      // 生成位置
            );
        // テキストを代入
        gameObject.GetComponent<TextMeshProUGUI>().text = text;
        // テキストを反転させる
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