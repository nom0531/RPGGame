using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class FadeScene : MonoBehaviour
{
    [SerializeField, Header("詳細パラメータ"), Tooltip("フェードの速度")]
    private float FadeSpeed = 1.0f;

    // 画像の不透明度
    private float m_alpha = 0.0f;
    // trueなら明るく、falseなら暗くなる
    private bool m_fadeMode = false;
    // 遷移先のシーン名
    private string m_sceneName = "";
    // 自身が使用するImage
    private Image m_image;

    /// <summary>
    /// フェードを開始する処理
    /// </summary>
    public void FadeStart(string sceneName)
    {
        if (sceneName == "")
        {
            // 何も入力されていないなら現在のシーン名を取得する
            m_sceneName = SceneManager.GetActiveScene().name;
        }
        else
        {
            // 遷移先のシーン名を保存
            m_sceneName = sceneName;
        }

        // 自身の子オブジェクトのImageを保存
        m_image = transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        // フェード処理
        if (m_fadeMode == false)
        {
            // 画面を暗くする
            m_alpha += FadeSpeed * Time.deltaTime;

            // 完全に暗くなったのでシーンを変更
            if (m_alpha >= 1.0f)
            {
                // シーンをロードして、明るくする
                SceneManager.LoadScene(m_sceneName);

                m_fadeMode = true;
                // 自身はシーンを跨いでも削除されないようにする
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            // 画面を明るくする
            m_alpha -= FadeSpeed * Time.deltaTime;

            // 完全に明るくなったので自身を削除する
            if (m_alpha <= 0.0f)
            {
                Destroy(gameObject);
            }
        }

        // 不透明度を設定する
        Color nowColor = m_image.color;
        nowColor.a = m_alpha;
        m_image.color = nowColor;
    }
}
