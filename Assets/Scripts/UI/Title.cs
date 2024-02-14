using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using TMPro;

public class Title : MonoBehaviour
{
    // フェードモード
    private enum FadeMode
    {
        enWhite,
        enBlack,
    }

    [SerializeField, Header("参照オブジェクト"),Tooltip("フェードを行うキャンバス")]
    private GameObject FadeCanvas;
    [SerializeField, Tooltip("遷移先のシーン名")]
    private string SceneName;
    [SerializeField, Header("テキストの明滅"), Tooltip("明滅させるテキスト")]
    private GameObject Text;
    [SerializeField, Tooltip("明滅させる速度")]
    private float Speed = 1.0f;

    private PlayableDirector playableDirector;  // タイムラインの制御

    // テキストデータ
    private TextMeshProUGUI m_text;
    private Color m_color = Color.black;        // カラー 
    private float m_alpha = 1.0f;               // 透明度
    private FadeMode m_fadeMode = FadeMode.enWhite;

    private void Start()
    {
        playableDirector = Camera.main.GetComponent<PlayableDirector>();
        m_text = Text.GetComponent<TextMeshProUGUI>();          // テキストメッシュを取得
        m_color = m_text.color;
    }

    // Update is called once per frame
    private void Update()
    {
        TextBlink();
        SceneChange();
    }

    /// <summary>
    /// シーンを切り替える
    /// </summary>
    private void SceneChange()
    {
        if (Input.GetMouseButtonDown(0) == false)
        {
            return;
        }
        var sceneName = SceneName;
        if (SceneName == "")
        {
            // 空白の場合は現在のシーンの名前を使用する
            sceneName = SceneManager.GetActiveScene().name;
        }
        // フェードを開始する
        var fadeCanvas = Instantiate(FadeCanvas);
        fadeCanvas.GetComponent<FadeScene>().FadeStart(sceneName);
        // タイムラインを再生
        playableDirector.Play();
    }

    /// <summary>
    /// テキストを明滅させる
    /// </summary>
    private void TextBlink()
    {
        // フェード処理
        if (m_fadeMode == FadeMode.enBlack)
        {
            // 画面を暗くする
            m_alpha += Speed * Time.deltaTime;

            // 完全に暗くなったのでシーンを変更
            if (m_alpha >= 1.0f)
            {
                m_fadeMode = FadeMode.enWhite;
            }
        }
        else
        {
            // 画面を明るくする
            m_alpha -= Speed * Time.deltaTime;
            // 完全に明るくなったので自身を削除する
            if (m_alpha <= 0.0f)
            {
                m_fadeMode = FadeMode.enBlack;
            }
        }
        // 不透明度を設定する
        m_text.color = new Vector4(m_color.r, m_color.g, m_color.b, m_alpha);
    }
}
