using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transiton : MonoBehaviour
{
    [SerializeField, Header("画面遷移用データ"),Tooltip("マテリアル")]
    private Material PostEffectMaterial;
    [SerializeField, Tooltip("画面遷移の時間")]
    private float TransitionTime = 1.0f;

    private string m_sceneName = "";        // 遷移先のシーン名
    private bool m_fadeMode = false;        // trueなら明るく、falseなら暗くなる
    private float m_progress = 1.0f;

    // シェーダー内で定義されている特定のプロパティをint型に変換
    private readonly int m_progressID = Shader.PropertyToID("_Progress");

    private void Start()
    {
        // 自身のRenderCameraにメインカメラを設定する
        if (GetComponent<Canvas>().worldCamera == null)
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
        // 値を初期化
        PostEffectMaterial.SetFloat(m_progressID, 1.0f);
    }

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
    }

    private void Update()
    {
        StartTransition();
    }

    /// <summary>
    /// トランジション処理
    /// </summary>
    /// <returns></returns>
    private void StartTransition()
    {
        // 自身のRenderCameraにメインカメラを設定する
        if (GetComponent<Canvas>().worldCamera == null)
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
        if (m_fadeMode == false)
        {
            // 画面を暗くする
            m_progress -= TransitionTime * Time.deltaTime;

            // 完全に暗くなったのでシーンを変更する
            if (m_progress <= 0.0f)
            {
                PostEffectMaterial.SetFloat(m_progressID, 0.0f);    // 補正
                SceneManager.LoadScene(m_sceneName);
                // 明るくするモードに変更
                m_fadeMode = true;
            }
        }
        else
        {
            // 画面を明るくする
            m_progress += TransitionTime * Time.deltaTime;

            // 完全に明るくなったので自身を削除する
            if (m_progress >= 1.0f)
            {
                PostEffectMaterial.SetFloat(m_progressID, 1.0f);    // 補正
                Destroy(gameObject);
            }
        }
        // シェーダーの_Progressに値を設定
        PostEffectMaterial.SetFloat(m_progressID, m_progress);
    }
}
