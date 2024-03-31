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

    private Animator m_animator;                // アニメーター

    private void Start()
    {
        m_animator = AnimationText.GetComponent<Animator>();
    }

    /// <summary>
    /// シーンを切り替える
    /// </summary>
    public void SceneChange()
    {
        var sceneName = SceneName;
        PlayAnimation();
        // フェードを開始する
        var fadeCanvas = Instantiate(FadeCanvas);
        fadeCanvas.GetComponent<FadeScene>().FadeStart(sceneName);
    }

    private void PlayAnimation()
    {
        m_animator.SetTrigger("ClickTitle");
    }
}
