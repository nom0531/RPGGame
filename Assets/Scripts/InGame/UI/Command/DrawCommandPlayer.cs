using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCommandPlayer : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト"), Tooltip("プレイヤーアイコン")]
    private GameObject[] PlayerIcon;
    [SerializeField, Tooltip("COMMANDのUI")]
    private GameObject CommandUI;
    [SerializeField, Header("表示データ"), Tooltip("回転量")]
    private float AddRotation = 0.5f;
    [SerializeField, Tooltip("移動速度")]
    private float MoveSpeed = 10.0f;

    private BattleManager m_battleManager;
    private int m_operatingPlayer = 0;     // 現在操作しているプレイヤー
    private float m_timer = 0.0f;          // タイマー

    // Start is called before the first frame update
    private void Start()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_operatingPlayer = m_battleManager.OperatingPlayerNumber;
        CommandUI.transform.position = PlayerIcon[m_operatingPlayer].transform.position;
    }

    private void FixedUpdate()
    {
        SetUIRotation();
        SetUIPosition();

        // プレイ中でないなら中断
        if (m_battleManager.GameState != GameState.enPlay)
        {
            return;
        }
        // ポーズ中なら中断
        if (m_battleManager.PauseFlag == true)
        {
            return;
        }

        // エネミーのターン　または　番号が同じ時は処理を実行しない
        if (m_battleManager.TurnStatus == TurnStatus.enEnemy
            || m_operatingPlayer == m_battleManager.OperatingPlayerNumber)
        {
            return;
        }

        m_timer = 0.0f;
    }

    /// <summary>
    /// UIを回転させる処理
    /// </summary>
    private void SetUIRotation()
    {
        CommandUI.transform.Rotate(0, 0, -AddRotation);
    }

    /// <summary>
    /// UIの座標を決定する
    /// </summary>
    private void SetUIPosition()
    {
        if(m_timer >= 1.0f)
        {
            return;
        }

        m_operatingPlayer = m_battleManager.OperatingPlayerNumber;
        // 始点と終点を取得
        var start = CommandUI.transform.position;
        var end = PlayerIcon[m_operatingPlayer].transform.position;
        // 補間位置を計算
        m_timer =+ Time.deltaTime * MoveSpeed;
        // -t^2 + 2t
        float rate = ((Mathf.Pow(m_timer, 2.0f) * -1.0f) + (2.0f * m_timer));
        rate = Mathf.Min(rate, 1.0f);
        // 座標を反映
        CommandUI.transform.position = Vector3.Lerp(start, end, rate);
    }
}
