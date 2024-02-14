using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawDamage : MonoBehaviour
{
    [SerializeField, Header("詳細設定"), Tooltip("消えるまでの時間")]
    private float m_deleteTime = 1.0f;
    [SerializeField, Tooltip("消えるまでの時間にどれだけ上に上がるか")]
    private float m_moveRange = 0.05f;
    [SerializeField, Tooltip("消える時の透明度"),Range(0.0f,1.0f)]
    private float m_endAlpha = 0.2f;

    private float m_timeCount;
    private Text m_nowText;

    // Start is called before the first frame update
    private void Start()
    {
        m_timeCount = 0.0f;
        Destroy(
            this.gameObject,    // 破壊するオブジェクト
            m_deleteTime        // オブジェクトを破壊するまでの時間
            );
        m_nowText = GetComponent<Text>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // 座標を移動させる
        m_timeCount += Time.deltaTime;
        this.gameObject.transform.localPosition += 
            new Vector3(0, m_moveRange / m_deleteTime * Time.deltaTime, 0);

        // 透明度の計算処理
        float alpha = 1.0f - (1.0f - m_endAlpha) * (m_timeCount / m_deleteTime);

        if (alpha <= 0.0f)
        {
            alpha = 0.0f;
            m_nowText.color = new Color(m_nowText.color.r, m_nowText.color.g, m_nowText.color.b, alpha);
        }
    }
}
