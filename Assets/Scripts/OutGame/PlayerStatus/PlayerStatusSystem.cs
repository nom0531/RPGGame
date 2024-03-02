using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private SkillDataBase SkillData;
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
    [SerializeField, Header("スキルデータ"), Tooltip("コンテンツ")]
    private GameObject SkillDataContent;
    [SerializeField, Tooltip("スキルアイコン")]
    private GameObject SkillDataIcon;
    [SerializeField, Header("参照オブジェクト"),Tooltip("属性のテキスト")]
    private GameObject Element_Text;
    [SerializeField, Tooltip("ステータスのテキスト")]
    private GameObject Status_Text;
    [SerializeField, Header("表示するCanvas")]
    private GameObject Canvas;
    [SerializeField, Header("所持しているEP")]
    private GameObject HaveEP;
    [SerializeField, Header("スキルデータ")]
    private GameObject SkillName;
    [SerializeField]
    private GameObject SkillDetail, EnhancementPoint, SkillElement;
    [SerializeField, Header("OKボタン")]
    private GameObject OKButton;

    private GameManager m_gameManager;      // ゲームマネージャー

    // Start is called before the first frame update
    private void Start()
    {
        m_gameManager = GameManager.Instance;
        HaveEP.GetComponent<TextMeshProUGUI>().text = m_gameManager.SaveData.SaveData.saveData.EnhancementPoint.ToString();
        DisplaySetValue(m_gameManager.PlayerNumber);
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
    /// データを表示
    /// </summary>
    public void DrawData(int skillNumber)
    {
        // データを設定する
        SkillName.GetComponent<TextMeshProUGUI>().text = SkillData.skillDataList[skillNumber].SkillName;
        SkillDetail.GetComponent<TextMeshProUGUI>().text = SkillData.skillDataList[skillNumber].SkillDetail;
        SkillElement.GetComponent<TextMeshProUGUI>().text = $"属性 {SetElementData(skillNumber)}";
        EnhancementPoint.GetComponent<TextMeshProUGUI>().text = $"必要EP {SkillData.skillDataList[skillNumber].EnhancementPoint.ToString()}";
        // ボタンが保持している番号を更新
        OKButton.GetComponent<SkillButton>().MyNumber = skillNumber;
    }

    /// <summary>
    /// データを取得してボタンのテキストを変更する
    /// </summary>
    /// <returns>trueなら既に開放している。falseなら開放していない</returns>
    public void GetData(int skillNumber)
    {
        if(m_gameManager.SaveData.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[skillNumber] == true)
        {
            // ボタンのテキストを変更する
            OKButton.GetComponent<Button>().interactable = false;
            OKButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "解放済み";
            return;
        }
        // ボタンのテキストを変更する
        OKButton.GetComponent<Button>().interactable = true;
        OKButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "解放";
    }

    /// <summary>
    /// 解放したスキルデータをセーブする
    /// </summary>
    public void SaveReleaseSkillData(int skillNumber)
    {
        // 値を設定する
        m_gameManager.SaveData.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[skillNumber] = true;
        m_gameManager.SaveData.SaveData.saveData.EnhancementPoint -= SkillData.skillDataList[skillNumber].EnhancementPoint;
        // 値を更新
        HaveEP.GetComponent<TextMeshProUGUI>().text = m_gameManager.SaveData.SaveData.saveData.EnhancementPoint.ToString();
        m_gameManager.SaveData.Save();
    }

    /// <summary>
    /// 属性を設定する
    /// </summary>
    /// <returns>属性名</returns>
    private string SetElementData(int skillNumber)
    {
        var element = "";
        switch (SkillData.skillDataList[skillNumber].SkillElement)
        {
            case ElementType.enFire:
                element = "炎";
                break;
            case ElementType.enIce:
                element = "氷";
                break;
            case ElementType.enWind:
                element = "風";
                break;
            case ElementType.enThunder:
                element = "雷";
                break;
            case ElementType.enLight:
                element = "光";
                break;
            case ElementType.enDark:
                element = "闇";
                break;
            case ElementType.enNone:
                element = "ー";
                break;
        }
        return element;
    }

    /// <summary>
    /// Activeを切り替える
    /// </summary>
    public void SetActive(bool flag)
    {
        Canvas.SetActive(flag);
    }

    /// <summary>
    /// スキルボタンを生成
    /// </summary>
    private void InstantiateSkillDataButton()
    {
        DestroyIcon();

        for (int i = 0; i < PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList.Count; i++)
        {
            ChangeTexture(i);
            // ボタンを生成して子オブジェクトにする
            var button = Instantiate(SkillDataIcon);
            button.transform.SetParent(SkillDataContent.transform);
            // サイズを調整
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = 
                PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList[i].SkillName;
            // 自身の番号を教える
            button.GetComponent<SkillButton>().MyNumber = i;
        }
    }

    /// <summary>
    /// テクスチャを変更する
    /// </summary>
    /// <param name="skillNumber">スキルの番号</param>
    private void ChangeTexture(int skillNumber)
    {
        if (m_gameManager.SaveData.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[skillNumber] == false)
        {
            // スキルを覚えていない
            return;
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
