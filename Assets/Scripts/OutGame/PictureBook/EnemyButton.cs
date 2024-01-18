using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyButton : MonoBehaviour
{
    // エネミーの番号
    private int m_enemyNumber = -1;
    // 図鑑システム
    private PictureBookSystem m_pictureBookSystem;

    /// <summary>
    /// 初期化用の関数。エネミーを登録する
    /// </summary>
    /// <param name="number">エネミーの番号</param>
    /// <param name="enemyImage">エネミーの画像</param>
    /// <param name="interactable">登録済みかどうか</param>
    public void SetPictureBook(int number,Sprite enemyImage,bool interactable,PictureBookSystem pictureBookSystem)
    { 
        // それぞれの値を登録する
        m_enemyNumber = number;
        GetComponent<Image>().sprite = enemyImage;
        // 押せるかどうか設定する
        GetComponent<Button>().interactable = interactable;
        // 図鑑システムを登録する
        m_pictureBookSystem = pictureBookSystem;
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void ButtonDown()
    {
        m_pictureBookSystem.DisplaySetValue(m_enemyNumber);
    }
}
