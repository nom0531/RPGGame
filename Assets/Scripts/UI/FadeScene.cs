using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;

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
    private Image m_cardImage;
    private TextMeshProUGUI m_textMeshProUGUI;

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
        GetChilds();
    }

    /// <summary>
    /// 子オブジェクトを取得
    /// </summary>
    private void GetChilds()
    {
        var image = transform.GetChild(0);
        m_image = image.GetComponent<Image>();
        m_cardImage = image.GetChild(0).GetComponent<Image>();
        m_textMeshProUGUI = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
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
        m_image.color = SetColor(m_image.color);
        m_cardImage.color = SetColor(m_cardImage.color);
        m_textMeshProUGUI.color = SetColor(m_textMeshProUGUI.color);
    }

    /// <summary>
    /// 不透明度を設定
    /// </summary>
    /// <param name="color">オブジェクトのカラー</param>
    private Color SetColor(Color color)
    {
        Color nowColor = color;
        nowColor.a = m_alpha;
        return nowColor;
    }
}
