using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PictureBookSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private EnemyDataBase EnemyData;
    [SerializeField, Header("エネミーリスト"), Tooltip("生成するボタン")]
    private GameObject Button;
    [SerializeField,Tooltip("ボタンを追加するオブジェクト")]
    private GameObject Content;
    [SerializeField, Header("表示用データ"), Tooltip("名前")]
    private GameObject Data_Name;
    [SerializeField, Tooltip("ゲーム内モデル")]
    private GameObject Data_Sprite;
    [SerializeField,Tooltip("説明")]
    private GameObject Data_Detail;
    [SerializeField, Header("属性"),Tooltip("炎耐性")]
    private GameObject Data_Fire;
    [SerializeField, Tooltip("氷耐性")]
    private GameObject Data_Ice;
    [SerializeField, Tooltip("風耐性")]
    private GameObject Data_Wind;
    [SerializeField, Tooltip("雷耐性")]
    private GameObject Data_Thunder;
    [SerializeField, Tooltip("光耐性")]
    private GameObject Data_Light;
    [SerializeField, Tooltip("闇耐性")]
    private GameObject Data_Dark;
    [SerializeField, Tooltip("図鑑番号")]
    private GameObject Data_EnemyNumber;
    [SerializeField,Header("参照オブジェクト")]
    private GameObject RegistrationRateText;

    private SaveDataManager m_saveDataManager;      // セーブデータ
    private int m_enemyCount = 0;                   // 見つけているエネミーの数
    private int m_elementCount = 0;                 // 見つけている属性数

    /// <summary>
    /// 入力されたデータを表示する処理
    /// </summary>
    /// <param name="number">エネミーの番号</param>
    public void DisplaySetValue(int number)
    {
        // 値を表示する
        Data_Name.SetActive(true);
        Data_Detail.SetActive(true);
        Data_Sprite.SetActive(true);

        // 値を更新する
        Data_Name.GetComponent<TextMeshProUGUI>().text = EnemyData.enemyDataList[number].EnemyName;
        Data_Detail.GetComponent<TextMeshProUGUI>().text = EnemyData.enemyDataList[number].EnemyDetail;
        Data_Sprite.GetComponent<Image>().sprite = EnemyData.enemyDataList[number].EnemySprite;
        Data_EnemyNumber.GetComponent<TextMeshProUGUI>().text = (number + 1).ToString("00");
        // 属性耐性
        GetResistance(Data_Fire, number, (int)ElementType.enFire);
        GetResistance(Data_Ice, number, (int)ElementType.enIce);
        GetResistance(Data_Wind, number, (int)ElementType.enWind);
        GetResistance(Data_Thunder, number, (int)ElementType.enThunder);
        GetResistance(Data_Light, number, (int)ElementType.enLight);
        GetResistance(Data_Dark, number, (int)ElementType.enDark);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 値を非表示にする
        Data_Sprite.SetActive(false);
        m_saveDataManager = GameManager.Instance.SaveDataManager;

        for (int i = 0; i < EnemyData.enemyDataList.Count; i++)
        {
            // ボタンを生成して子オブジェクトにする
            var button = Instantiate(Button);

            button.transform.SetParent(Content.transform);
            // サイズ、座標を調整
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;

            // 見つかっていないなら画像を暗くする
            if (m_saveDataManager.SaveData.saveData.EnemyRegisters[i] == false)
            {
                button.GetComponent<Image>().color = Color.black;
            }
            else
            {
                // 見つけているならカウント
                m_enemyCount++;
                for(int elementNumber = 0; elementNumber < (int)ElementType.enNum; elementNumber++)
                {
                    if(m_saveDataManager.SaveData.saveData.ElementRegisters[i].Elements[elementNumber] != true)
                    {
                        break;
                    }
                    m_elementCount++;
                }
            }
            // オブジェクトを生成する
            var enemyButton = button.GetComponent<EnemyButton>();
            enemyButton.SetPictureBook(
                i,                                                          // 番号
                EnemyData.enemyDataList[i].EnemySprite,                     // 画像
                m_saveDataManager.SaveData.saveData.EnemyRegisters[i],      // 発見しているかどうか
                this
                );
        }

        RegistrationRate();
    }

    /// <summary>
    /// 登録率を取得する
    /// </summary>
    /// <returns>登録率</returns>
    private void RegistrationRate()
    {
        var rate = 0.0f;
        // エネミーの総数とエレメントの総数
        var allValue = EnemyData.enemyDataList.Count + (EnemyData.enemyDataList.Count * (int)ElementType.enNum);
        // 現在発見しているエネミーの数とエレメントの数
        var Value = m_enemyCount + m_elementCount;
        // 割合を計算
        rate = (float)Value / (float)allValue;
        rate *= 100;
        var text = $"{rate.ToString("F1")}%";
        // 値を代入
        RegistrationRateText.GetComponent<TextMeshProUGUI>().text = text;
    }

    /// <summary>
    /// 属性耐性を表示する処理
    /// </summary>
    /// <param name="gameObjct">ゲームオブジェクト</param>
    /// <param name="enemyNumber">エネミーの番号</param>
    /// <param name="elementNumber">属性の識別番号</param>
    void GetResistance(GameObject gameObjct,int enemyNumber,int elementNumber)
    {
        // 発見していないなら
        if (m_saveDataManager.SaveData.saveData.ElementRegisters[enemyNumber].Elements[elementNumber] == false)
        {
            gameObjct.GetComponent<TextMeshProUGUI>().text = "？";
            return;
        }

        // 発見しているなら
        ElementResistance element = EnemyData.enemyDataList[enemyNumber].EnemyElement[elementNumber];

        switch (element)
        {
            case ElementResistance.enResist:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "耐";
                break;
            case ElementResistance.enWeak:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "弱";
                break;
            case ElementResistance.enNormal:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "-";
                break;
        }
    }
}
