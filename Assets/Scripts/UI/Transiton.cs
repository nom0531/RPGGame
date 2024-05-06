using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionState
{
    enTransition_ReduceValue,       // トランジション画面を多くする
    enTransition_IncreaseVaue,      // Scene画面を多くする
}

public class Transiton : MonoBehaviour
{
    [SerializeField, Header("画面遷移用データ"),Tooltip("ポストエフェクト")]
    private Material PostEffectMaterial;
    [SerializeField, Tooltip("画面遷移の時間")]
    private float TransitionTime = 2.0f;

    // シェーダー内で定義されている特定のプロパティをint型に変換
    private readonly int m_progressID = Shader.PropertyToID("_Progress");

    // Start is called before the first frame update
    private void Start()
    {
        if(PostEffectMaterial != null)
        {
            StartCoroutine(StartTransition(TransitionState.enTransition_ReduceValue));
        }
    }

    public IEnumerator StartTransition(TransitionState transitionState)
    {
        float t = 0.0f;
        switch (transitionState)
        {
            case TransitionState.enTransition_ReduceValue:
                // tが設定した時間内の間実行する
                while (t < TransitionTime)
                {
                    var progress = t / TransitionTime;

                    // シェーダーの_Progressに値を設定
                    PostEffectMaterial.SetFloat(m_progressID, progress);
                    yield return null;

                    t += Time.deltaTime;
                }
                PostEffectMaterial.SetFloat(m_progressID, 0.0f);    // 補正
                break;
            case TransitionState.enTransition_IncreaseVaue:
                // tが設定した時間内の間実行する
                while (t < TransitionTime)
                {
                    var progress = t / TransitionTime;

                    // シェーダーの_Progressに値を設定
                    PostEffectMaterial.SetFloat(m_progressID, progress);
                    yield return null;

                    t += Time.deltaTime;
                }
                PostEffectMaterial.SetFloat(m_progressID, 1.0f);    // 補正
                break;
        }
    }
}
