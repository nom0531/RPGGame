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
    private float m_addRotation = -0.5f;

    private BattleManager m_battleManager;
    private int m_operatingPlayer = 0;     // 現在操作しているプレイヤー

    // Start is called before the first frame update
    private void Start()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        SetUIPosition();
    }

    private void FixedUpdate()
    {
        SetUIRotation();

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

        SetUIPosition();
    }

    /// <summary>
    /// UIを回転させる処理
    /// </summary>
    private void SetUIRotation()
    {
        CommandUI.transform.Rotate(0, 0, m_addRotation);
    }

    /// <summary>
    /// UIの座標を決定する
    /// </summary>
    private void SetUIPosition()
    {
        m_operatingPlayer = m_battleManager.OperatingPlayerNumber;
        CommandUI.transform.position = PlayerIcon[m_operatingPlayer].transform.position;
    }
}
