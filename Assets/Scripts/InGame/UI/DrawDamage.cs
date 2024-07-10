using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class DrawDamage : MonoBehaviour
{
    private CinemachineBrain m_cinemachineBrain;
    private string m_damageText = "";
    private bool m_isChangeCamera = false;
    

    public string Damage
    {
        set => m_damageText = value;
    }

    private void Start()
    {
        // カメラ切り替わりイベント登録
        m_cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        m_cinemachineBrain.m_CameraActivatedEvent.AddListener(OnChangeCamera);
    }

    // バーチャルカメラが切り替わったときに呼ばれる
    private void OnChangeCamera(ICinemachineCamera incomingVcam, ICinemachineCamera outgoingVcam)
    {
        m_isChangeCamera = true;
    }

    private IEnumerator StopDrawCoroutine()
    {
        yield return new WaitUntil(() => m_isChangeCamera == true);
    }

    /// <summary>
    /// ダメージを描画する
    /// </summary>
    public void Draw()
    {
        StartCoroutine(StopDrawCoroutine());
        // テキストの設定
        var text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = m_damageText;
    }

    /// <summary>
    /// 自身を削除する
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
