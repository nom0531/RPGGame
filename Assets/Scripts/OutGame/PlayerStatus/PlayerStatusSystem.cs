using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ"),Tooltip("プレイヤーのデータ")]
    private PlayerDataBase PlayerData;
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
    [SerializeField]
    private GameObject Data_AddHP, Data_AddSP, Data_AddATK, Data_AddDEF, Data_AddSPD, Data_AddLUCK;
    [SerializeField, Header("スキルデータ"), Tooltip("コンテンツ")]
    private GameObject SkillDataContent;
    [SerializeField, Tooltip("スキルアイコン")]
    private GameObject SkillDataIcon;
    [SerializeField, Header("参照オブジェクト"),Tooltip("属性のテキスト")]
    private GameObject Element_Text;
    [SerializeField, Tooltip("ステータスのテキスト")]
    private GameObject Status_Text;

    private GameManager m_gameManager;      // ゲームマネージャー

    // Start is called before the first frame update
    private void Start()
    {
        m_gameManager = GameManager.Instance;
        var playerNumber = m_gameManager.PlayerNumber;
        // データを非表示
        Data_Sprite.SetActive(false);
        Data_Name.SetActive(false);
        Element_Text.SetActive(false);
        Status_Text.SetActive(false);
        // ボタンを生成
        for (int i = 0; i < PlayerData.playerDataList.Count; i++)
        {
            // ボタンを生成して子オブジェクトにする
            var button = Instantiate(Button);
            button.transform.SetParent(Content.transform);
            // サイズを調整
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;

            var playerButton = button.GetComponent<PlayerButton>();
            playerButton.SetPlayerStatus(
                i,                                              // 番号
                PlayerData.playerDataList[i].PlayerSprite,      // 画像
                this
                );
        }
        DisplaySetValue(playerNumber);
    }

    /// <summary>
    /// 入力されたデータを表示する処理
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    public void DisplaySetValue(int number)
    {
        m_gameManager.PlayerNumber = number;    // 番号を更新
        // データを表示
        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Element_Text.SetActive(true);
        Status_Text.SetActive(true);
        // 値を更新する
        Data_Sprite.GetComponent<Image>().sprite = PlayerData.playerDataList[number].PlayerSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text = PlayerData.playerDataList[number].PlayerName;
        SetData(number);
        InstantiateSkillDataButton();
    }

    /// <summary>
    /// データの設定
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    private void SetData(int number)
    {
        // 属性耐性
        GetResistance(Data_Fire, number, (int)ElementType.enFire);
        GetResistance(Data_Ice, number, (int)ElementType.enIce);
        GetResistance(Data_Wind, number, (int)ElementType.enWind);
        GetResistance(Data_Thunder, number, (int)ElementType.enThunder);
        GetResistance(Data_Light, number, (int)ElementType.enLight);
        GetResistance(Data_Dark, number, (int)ElementType.enDark);
        // ステータス
        Data_HP.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].HP.ToString("000")}";
        Data_SP.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].SP.ToString("000")}";
        Data_ATK.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].ATK.ToString("000")}";
        Data_DEF.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].DEF.ToString("000")}";
        Data_SPD.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].SPD.ToString("000")}";
        Data_LUCK.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveData.SaveData.saveData.PlayerList[number].LUCK.ToString("000")}";
        // 加算値
        Data_AddHP.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enHP).ToString("000")}";
        Data_AddSP.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enSP).ToString("000")}";
        Data_AddATK.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enATK).ToString("000")}";
        Data_AddDEF.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enDEF).ToString("000")}";
        Data_AddSPD.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enSPD).ToString("000")}";
        Data_AddLUCK.GetComponent<TextMeshProUGUI>().text = $"+{GetAddStatus(number, EnhancementStatus.enLUCK).ToString("000")}";
    }

    /// <summary>
    /// 属性耐性を取得する
    /// </summary>
    /// <param name="playerNumber">プレイヤーの番号</param>
    /// <param name="elementNumber">属性の識別番号</param>
    private void GetResistance(GameObject gameObjct, int playerNumber, int elementNumber)
    {
        var element = PlayerData.playerDataList[playerNumber].PlayerElement[elementNumber];
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

    /// <summary>
    /// 加算値を取得する
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    /// <returns>加算値</returns>
    private int GetAddStatus(int number, EnhancementStatus enhancementStatus)
    {
        var addValue = 0;
        for(int i = 0; i< PlayerData.playerDataList[number].enhancementDataList.Count; i++)
        {
            if (m_gameManager.SaveData.SaveData.saveData.EnhancementRegisters[number].PlayerEnhancements[i] == false)
            {
                continue;   // 取得していないならスキップ
            }
            if(enhancementStatus != PlayerData.playerDataList[number].enhancementDataList[i].EnhancementStatus)
            {
                continue;   // 属性が異なるならスキップ
            }
            addValue += PlayerData.playerDataList[number].enhancementDataList[i].AddValue;
        }
        return addValue;
    }

    /// <summary>
    /// スキルボタンを生成
    /// </summary>
    private void InstantiateSkillDataButton()
    {
        DestroyIcon();

        for (int i = 0; i < PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList.Count; i++)
        {
            if (m_gameManager.SaveData.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[i] == false)
            {
                continue;   // スキルを覚えていないならスキップ
            }
            // ボタンを生成して子オブジェクトにする
            var button = Instantiate(SkillDataIcon);
            button.transform.SetParent(SkillDataContent.transform);
            // サイズを調整
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = 
                PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList[i].SkillName;
        }
    }

    /// <summary>
    /// SkillNameIconタグの付いたオブジェクトを全て削除する処理
    /// </summary>
    private void DestroyIcon()
    {
        var skillIcons = GameObject.FindGameObjectsWithTag("SkillNameIcon");
        foreach (var button in skillIcons)
        {
            Destroy(button);
        }
    }
}
