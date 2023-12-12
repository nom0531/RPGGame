using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DrawStatusValue : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private EnemyDataBase EnemyData;
    [SerializeField, Header("HP")]
    private GameObject[] Data_HPText;
    [SerializeField]
    private GameObject[] Data_HPBar;
    [SerializeField, Header("SP")]
    private GameObject[] Data_SPText;
    [SerializeField]
    private GameObject[] Data_SPBar;
    [SerializeField]
    private GameObject Data_Name;

    private List<PlayerMove> m_playerMove;
    private List<EnemyMove> m_enemyMove;

    public int EnemyName
    {
        set
        {
            Data_Name.GetComponent<TextMeshProUGUI>().text = EnemyData.enemyDataList[value].EnemyName;
        }
    }

    public int PlayerName
    {
        set
        {
            Data_Name.GetComponent<TextMeshProUGUI>().text = PlayerData.playerDataList[value].PlayerName;
        }
    }

    private void Awake()
    {
        // playerMoveを人数分用意
        PlayerMove[] playerMove = FindObjectsOfType<PlayerMove>();
        m_playerMove = new List<PlayerMove>(playerMove);
        m_playerMove.Sort((a, b) => a.MyNumber.CompareTo(b.MyNumber));    // 番号順にソート
    }

    private void Start()
    {
        // enemyMoveを人数分用意
        EnemyMove[] enemyMove = FindObjectsOfType<EnemyMove>();
        m_enemyMove = new List<EnemyMove>(enemyMove);
    }

    /// <summary>
    /// テキストを設定する
    /// </summary>
    public void SetStatusText()
    {
        for(int i = 0; i < PlayerData.playerDataList.Count; i++)
        {
            Data_HPText[i].GetComponent<TextMeshProUGUI>().text =
                m_playerMove[i].PlayerStatus.HP.ToString();

            Data_SPText[i].GetComponent<TextMeshProUGUI>().text =
                m_playerMove[i].PlayerStatus.SP.ToString();
        }
    }

    /// <summary>
    /// HP・SPのバーを設定する
    /// </summary>
    public void SetStatusBar()
    {
        for (int i = 0; i < PlayerData.playerDataList.Count; i++)
        {
            Data_HPBar[i].GetComponent<Image>().fillAmount =
                CalculateRate(
                    m_playerMove[i].PlayerStatus.HP,
                    PlayerData.playerDataList[i].HP
                    );

            Data_SPBar[i].GetComponent<Image>().fillAmount =
                CalculateRate(
                    m_playerMove[i].PlayerStatus.SP,
                    PlayerData.playerDataList[i].SP
                    );
        }
    }

    /// <summary>
    /// HP・SPの割合を計算する
    /// </summary>
    /// <param name="nowValue">現在の値</param>
    /// <param name="maxValue">最大値</param>
    private float CalculateRate(int nowValue, int maxValue)
    {
        float rate = 0.0f;
        return rate = (float)nowValue / (float)maxValue;
    }
}
