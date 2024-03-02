using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class DrawStatusValue : MonoBehaviour
{
    /// <summary>
    /// HP・SPバーの減少処理
    /// </summary>
    private enum DecreaseProcess
    {
        enStart,    // 開始
        enEnd,      // 終了
    }

    /// <summary>
    /// 減少処理が終了したアクタ
    /// </summary>
    private enum DecreaseState
    {
        enNone,
        enAttcker,
        enBuffer,
        enHealer,
    }

    [SerializeField, Header("参照データ")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private EnemyDataBase EnemyData;
    [SerializeField]
    private StateAbnormalDataBase StateAbnormalData;
    [SerializeField, Header("HP")]
    private GameObject[] Data_HPText;
    [SerializeField, Header("HPゲージ")]
    private GameObject[] Data_HPBarGreen;
    [SerializeField]
    private GameObject[] Data_HPBarRed;
    [SerializeField, Header("SP")]
    private GameObject[] Data_SPText;
    [SerializeField, Header("SPゲージ")]
    private GameObject[] Data_SPBarYellow;
    [SerializeField]
    private GameObject[] Data_SPBarRed;
    [SerializeField, Header("減少処理"),Tooltip("減少速度")]
    private float DecreaseSpped = 1.0f;
    [SerializeField, Header("バフ")]
    private GameObject[] Content;
    [SerializeField, Header("状態異常")]
    private GameObject[] Data_StatusAbnormalImage;
    [SerializeField, Header("名前を表示するテキスト")]
    private GameObject Data_Name;

    private List<PlayerMove> m_playerMove;
    private DecreaseProcess m_decreaseProcess = DecreaseProcess.enEnd;    // バーの減少処理のステート
    private DecreaseState m_decreaseState = DecreaseState.enNone;         // 減少処理が何人終了したか

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

    private void Update()
    {
        // 開始しないならスキップ
        if(m_decreaseProcess == DecreaseProcess.enEnd)
        {
            return;
        }
        Decrease();
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
    /// ステータスを設定する
    /// </summary>
    public void SetStatus()
    {
        for (int i = 0; i < PlayerData.playerDataList.Count; i++)
        {
            SetFillAmount(i);
            DrawStatusAbnormalImage(i);
            DrawBuffStatusImage(i);
        }
    }

    /// <summary>
    /// バーの割合を設定する
    /// </summary>
    async private void SetFillAmount(int number)
    {
        Data_HPBarGreen[number].GetComponent<Image>().fillAmount =
        CalculateRate(m_playerMove[number].PlayerStatus.HP, PlayerData.playerDataList[number].HP);
        Data_SPBarYellow[number].GetComponent<Image>().fillAmount =
        CalculateRate(m_playerMove[number].PlayerStatus.SP, PlayerData.playerDataList[number].SP);
        m_decreaseProcess = DecreaseProcess.enStart;
    }

    /// <summary>
    /// HP・SPの割合を計算する
    /// </summary>
    /// <param name="nowValue">現在値</param>
    /// <param name="maxValue">最大値</param>
    private float CalculateRate(int nowValue, int maxValue)
    {
        var rate = 0.0f;
        return rate = (float)nowValue / (float)maxValue;
    }

    /// <summary>
    /// HP・SPの減少処理
    /// </summary>
    private void Decrease()
    {
        // 減少処理を開始しないなら実行しない
        if(m_decreaseProcess == DecreaseProcess.enEnd)
        {
            return;
        }
        // 計算
        for (int i = 0; i < PlayerData.playerDataList.Count; i++)
        {
            Data_HPBarRed[i].GetComponent<Image>().fillAmount = 
                SetRate(Data_HPBarRed[i].GetComponent<Image>().fillAmount,
                Data_HPBarGreen[i].GetComponent<Image>().fillAmount);
            Data_SPBarRed[i].GetComponent<Image>().fillAmount = 
                SetRate(Data_SPBarRed[i].GetComponent<Image>().fillAmount,
                Data_SPBarYellow[i].GetComponent<Image>().fillAmount);
            m_decreaseState = (DecreaseState)i+1;
            if (m_decreaseState == DecreaseState.enHealer)
            {
                m_decreaseProcess = DecreaseProcess.enEnd;  // 処理を終了
            }
        }
    }

    /// <summary>
    /// 値を設定
    /// </summary>
    /// <param name="rate">自身の割合</param>
    /// <param name="nowRate">現在の割合</param>
    /// <returns>計算後の自身の割合</returns>
    private float SetRate(float rate, float nowRate)
    {
        // 既に割合が同じかそれ以下なら実行しない
        if (rate <= nowRate)
        {
            return rate;
        }
        // 値を減らす
        rate -= Time.deltaTime * DecreaseSpped;
        if (rate <= nowRate)
        {
            rate = nowRate;                             // 補正
        }
        return rate;
    }

    /// <summary>
    /// 状態異常のUIを描画する
    /// </summary>
    private void DrawStatusAbnormalImage(int number)
    {
        // 状態異常がない(enNormalの)場合は表示しない
        if (m_playerMove[number].ActorAbnormalState == ActorAbnormalState.enNormal)
        {
            Data_StatusAbnormalImage[number].SetActive(false);
            return;
        }
        // 現在の状態異常とデータの状態異常が同じなら
        for(int i = 0; i < StateAbnormalData.stateAbnormalList.Count; i++)
        {
            if(m_playerMove[number].ActorAbnormalState != StateAbnormalData.stateAbnormalList[i].ActorAbnormalState)
            {
                continue;
            }
            // 番号を設定
            var stateNumber = (int)StateAbnormalData.stateAbnormalList[i].ID;
            // スプライトを設定する
            Data_StatusAbnormalImage[number].GetComponent<Image>().sprite =
                StateAbnormalData.stateAbnormalList[stateNumber].StateImage;
            // 表示する
            Data_StatusAbnormalImage[number].SetActive(true);
            break;
        }
    }

    /// <summary>
    /// バフ・デバフのUIを描画する
    /// </summary>
    private void DrawBuffStatusImage(int number)
    {
        if(m_playerMove[number].NextActionType == ActionType.enGuard)
        {
            return;
        }
        if(m_playerMove[number].ActorAbnormalState != ActorAbnormalState.enNormal)
        {
            return;
        }

        var buffCalculation = m_playerMove[number].GetComponent<BuffCalculation>();
        if(buffCalculation.GetBuffFlag(BuffStatus.enBuff_ATK) == true)
        {
            if(buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_ATK) == true)
            {
                return;
            }
            SetImage(number, 0, 1, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_ATK) == true)
            {
                return;
            }
            SetImage(number, 0, 1, false);
        }
        if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_DEF) == true)
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_DEF) == true)
            {
                return;
            }
            SetImage(number, 1, 2, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_DEF) == true)
            {
                return;
            }
            SetImage(number, 1, 2, false);
        }
        if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_SPD) == true)
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_SPD) == true)
            {
                return;
            }
            SetImage(number, 2, 3, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_SPD) == true)
            {
                return;
            }
            SetImage(number, 2, 3, false);
        }
        if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_ATK) == true)
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_ATK) == true)
            {
                return;
            }
            SetImage(number, 0, 4, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_ATK) == true)
            {
                return;
            }
            SetImage(number, 0, 4, false);
        }
        if (buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_DEF) == true)
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_DEF) == true)
            {
                return;
            }
            SetImage(number, 1, 5, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_DEF) == true)
            {
                return;
            }
            SetImage(number, 1, 5, false);
        }
        if(buffCalculation.GetBuffFlag(BuffStatus.enDeBuff_SPD) == true)
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_SPD) == true)
            {
                return;
            }
            SetImage(number, 2, 6, true);
        }
        else
        {
            if (buffCalculation.GetBuffFlag(BuffStatus.enBuff_DEF) == true)
            {
                return;
            }
            SetImage(number, 2, 6, false);
        }
    }

    /// <summary>
    /// 画像を設定する
    /// </summary>
    /// <param name="contentNumber">コンテンツの番号</param>
    /// <param name="childNumber">子オブジェクトの番号</param>
    /// <param name="listNumber">リストの番号</param>
    /// <param name="flag">trueなら描画する。falseなら描画しない</param>
    private void SetImage(int contentNumber, int childNumber, int listNumber, bool flag)
    {
        Content[contentNumber].transform.GetChild(childNumber).GetComponent<Image>().sprite = 
            StateAbnormalData.stateAbnormalList[listNumber].StateImage;
        Content[contentNumber].transform.GetChild(childNumber).gameObject.SetActive(flag);
    }
}
