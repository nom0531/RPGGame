using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ"),Tooltip("プレイヤーのデータ")]
    private PlayerDataBase PlayerDataBase;
    [SerializeField, Header("プレイヤーリスト"), Tooltip("生成するボタン")]
    private GameObject Button;
    [SerializeField, Tooltip("ボタンを追加するオブジェクト")]
    private GameObject Content;
    [SerializeField, Header("表示用データ"), Tooltip("画像")]
    private GameObject Data_Sprite;
    [SerializeField, Tooltip("名前")]
    private GameObject Data_Name;
    [SerializeField, Header("属性"), Tooltip("炎耐性")]
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
    [SerializeField, Header("基本ステータス")]
    private GameObject Data_HP;
    [SerializeField]
    private GameObject Data_SP, Data_ATK, Data_DEF, Data_SPD, Data_LUCK;
    [SerializeField, Header("参照オブジェクト"),Tooltip("属性のテキスト")]
    private GameObject Element_Text;
    [SerializeField, Tooltip("ステータスのテキスト")]
    private GameObject Status_Text;

    /// <summary>
    /// 入力されたデータを表示する処理
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    public void DisplaySetValue(int number)
    {
        // 番号を更新
        PlayerNumberManager.PlayerNumber = number;

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Element_Text.SetActive(true);
        Status_Text.SetActive(true);

        // 値を更新する
        Data_Sprite.GetComponent<Image>().sprite = PlayerDataBase.playerDataList[number].PlayerSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].PlayerName;
        // 属性耐性
        GetResistance(Data_Fire, number, (int)ElementType.enFire);
        GetResistance(Data_Ice, number, (int)ElementType.enIce);
        GetResistance(Data_Wind, number, (int)ElementType.enWind);
        GetResistance(Data_Thunder, number, (int)ElementType.enThunder);
        GetResistance(Data_Light, number, (int)ElementType.enLight);
        GetResistance(Data_Dark, number, (int)ElementType.enDark);
        // ステータス
        Data_HP.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].HP.ToString("000");
        Data_SP.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].SP.ToString("000");
        Data_ATK.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].ATK.ToString("000");
        Data_DEF.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].DEF.ToString("000");
        Data_SPD.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].SPD.ToString("000");
        Data_LUCK.GetComponent<TextMeshProUGUI>().text = PlayerDataBase.playerDataList[number].LUCK.ToString("000");
    }

    // Start is called before the first frame update
    void Start()
    {
        Data_Sprite.SetActive(false);
        Data_Name.SetActive(false);
        Element_Text.SetActive(false);
        Status_Text.SetActive(false);

        for (int i = 0; i < PlayerDataBase.playerDataList.Count; i++)
        {
            // ボタンを生成して子オブジェクトにする
            var button = Instantiate(Button);
            button.transform.SetParent(Content.transform);
            // サイズを調整
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;

            var playerButton = button.GetComponent<PlayerButton>();
            playerButton.SetPlayerStatus(
                i,                                               // 番号
                PlayerDataBase.playerDataList[i].PlayerSprite,   // 画像
                this
                );
        }
        // 表示する
        DisplaySetValue(PlayerNumberManager.PlayerNumber);
    }

    /// <summary>
    /// 属性耐性を表示する処理
    /// </summary>
    /// <param name="playerNumber">プレイヤーの番号</param>
    /// <param name="elementNumber">属性の識別番号</param>
    void GetResistance(GameObject gameObjct, int playerNumber, int elementNumber)
    {
        ElementResistance element = PlayerDataBase.playerDataList[playerNumber].PlayerElement[elementNumber];

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
