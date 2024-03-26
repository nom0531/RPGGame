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
    [SerializeField, Tooltip("アニメーションするテキスト")]
    private GameObject AnimationText;

    private PlayableDirector playableDirector;  // タイムラインの制御
    private Animator m_animator;                // アニメーター

    private void Start()
    {
        playableDirector = Camera.main.GetComponent<PlayableDirector>();
        m_animator = AnimationText.GetComponent<Animator>();
    }

    /// <summary>
    /// シーンを切り替える
    /// </summary>
    public void SceneChange()
    {
        var sceneName = SceneName;
        if (SceneName == "")
        {
            // 空白の場合は現在のシーンの名前を使用する
            sceneName = SceneManager.GetActiveScene().name;
        }
        PlayAnimation();
        // フェードを開始する
        var fadeCanvas = Instantiate(FadeCanvas);
        fadeCanvas.GetComponent<FadeScene>().FadeStart(sceneName);
        // タイムラインを再生
        playableDirector.Play();
    }

    private void PlayAnimation()
    {
        m_animator.SetTrigger("ClickTitle");
    }
}
