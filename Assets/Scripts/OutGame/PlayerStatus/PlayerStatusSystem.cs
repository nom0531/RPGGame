using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField, Header("表示用データ"), Tooltip("画像")]
    private GameObject Data_Sprite, Data_Shadow;
    [SerializeField, Tooltip("名前")]
    private GameObject Data_Name;
    [SerializeField, Tooltip("ロール")]
    private GameObject Data_Roll;
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
    [SerializeField, Header("参照オブジェクト"), Tooltip("属性のテキスト")]
    private GameObject Element_Text;
    [SerializeField, Tooltip("ステータスのテキスト")]
    private GameObject Status_Text;
    [SerializeField, Header("表示するCanvas")]
    private GameObject SkillDataCanvas;
    [SerializeField]
    private GameObject SkillStatusCanvas;
    [SerializeField, Header("所持しているEP")]
    private GameObject HaveEP;
    [SerializeField, Header("スキルデータ")]
    private GameObject SkillName;
    [SerializeField]
    private GameObject SkillDetail, EnhancementPoint, SkillElement;
    [SerializeField, Header("ポップアップ用データ")]
    private GameObject SkillDataName;
    [SerializeField]
    private GameObject SkillDataDetil, SkillDataElement;
    [SerializeField, Header("OKボタン")]
    private GameObject OKButton;
    [SerializeField, Header("情報変更ボタン")]
    private GameObject[] ChangeButton;

    private GameManager m_gameManager;      // ゲームマネージャー
    private List<PlayerButton> m_playerButtonList;

    // Start is called before the first frame update
    private void Start()
    {
        // データを取得
        m_gameManager = GameManager.Instance;
        HaveEP.GetComponent<TextMeshProUGUI>().text = $"<sprite=1>{m_gameManager.SaveDataManager.SaveData.saveData.EnhancementPoint.ToString("N0")}";
        var playerButtonList = FindObjectsOfType<PlayerButton>();
        m_playerButtonList = new List<PlayerButton>(playerButtonList);
        // データを表示
        DisplaySetValue(m_gameManager.PlayerNumber);
        Data_Shadow.GetComponent<Animator>().SetTrigger("Active");
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
        SetData(number);
        InstantiateSkillDataButton();
    }

    /// <summary>
    /// データの設定
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    private void SetData(int number)
    {
        // 値を更新する
        SpriteAnimation(number);
        SetSprite(number);
        Data_Name.GetComponent<TextMeshProUGUI>().text = PlayerData.playerDataList[number].PlayerName;
        SetRoll(PlayerData.playerDataList[number].PlayerRoll);
        // 属性耐性
        GetResistance(Data_Fire, number, (int)ElementType.enFire);
        GetResistance(Data_Ice, number, (int)ElementType.enIce);
        GetResistance(Data_Wind, number, (int)ElementType.enWind);
        GetResistance(Data_Thunder, number, (int)ElementType.enThunder);
        GetResistance(Data_Light, number, (int)ElementType.enLight);
        GetResistance(Data_Dark, number, (int)ElementType.enDark);
        // ステータス
        Data_HP.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].HP.ToString("000")}";
        Data_SP.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].SP.ToString("000")}";
        Data_ATK.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].ATK.ToString("000")}";
        Data_DEF.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].DEF.ToString("000")}";
        Data_SPD.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].SPD.ToString("000")}";
        Data_LUCK.GetComponent<TextMeshProUGUI>().text = $"{m_gameManager.SaveDataManager.SaveData.saveData.PlayerList[number].LUCK.ToString("000")}";
    }

    /// <summary>
    /// 画像を設定する
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    private void SetSprite(int number)
    {
        Data_Sprite.GetComponent<Image>().sprite = PlayerData.playerDataList[number].PlayerSprite;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Data_Sprite.transform);
        Data_Shadow.GetComponent<Image>().sprite = PlayerData.playerDataList[number].PlayerSprite;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Data_Shadow.transform);
    }

    /// <summary>
    /// ロールのテキストを更新する
    /// </summary>
    /// <param name="playerRoll"></param>
    private void SetRoll(PlayerRoll playerRoll)
    {
        switch (playerRoll)
        {
            case PlayerRoll.enAttacker:
                Data_Roll.GetComponent<TextMeshProUGUI>().text = "アタッカー";
                return;
            case PlayerRoll.enBuffer:
                Data_Roll.GetComponent<TextMeshProUGUI>().text = "バッファー";
                return;
            case PlayerRoll.enHealer:
                Data_Roll.GetComponent<TextMeshProUGUI>().text = "ヒーラー";
                return;
        }
    }

    /// <summary>
    /// 画像のアニメーション
    /// </summary>
    private void SpriteAnimation(int number)
    {
        var spriteAnimation = Data_Sprite.GetComponent<StatusAnimation>();

        if (m_playerButtonList[0].ButtonDownFlag == true)
        {
            Data_Sprite.GetComponent<Image>().sprite = PlayerData.playerDataList[number].PlayerSprite;
            ResetButton();
            return;
        }
        if (m_playerButtonList[1].ButtonDownFlag == true)
        {
            Data_Sprite.GetComponent<Image>().sprite = PlayerData.playerDataList[number].PlayerSprite;
            ResetButton();
            return;
        }
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
    public void DrawData(int skillNumber, int skillNumberInPlayerDat, GameObject gameObject)
    {
        // ポイントが足りないならボタンは押せない
        if (SkillData.skillDataList[skillNumber].EnhancementPoint > m_gameManager.SaveDataManager.SaveData.saveData.EnhancementPoint)
        {
            OKButton.GetComponent<Button>().interactable = false;
        }
        // データを設定する
        SkillName.GetComponent<TextMeshProUGUI>().text = SkillData.skillDataList[skillNumber].SkillName;
        SkillDetail.GetComponent<TextMeshProUGUI>().text = SkillData.skillDataList[skillNumber].SkillDetail;
        SkillElement.GetComponent<TextMeshProUGUI>().text = $"属性 {SetElementData(skillNumber)}";
        // ボタンが保持している番号を更新
        var okButton = OKButton.GetComponent<SkillButton>();
        okButton.MyNumber = skillNumber;
        okButton.MyNumberInPlayerData = skillNumberInPlayerDat;
        okButton.SkillNameObject = gameObject;
    }

    /// <summary>
    /// データを取得してボタンのテキストを変更する
    /// </summary>
    public void GetData(int skillNumber, int skillNumberInPlayerData)
    {
        if (m_gameManager.SaveDataManager.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[skillNumberInPlayerData] == true)
        {
            // ボタンのテキストを変更する
            OKButton.GetComponent<Button>().interactable = false;
            OKButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "解放済み";
            return;
        }
        // ポイントが足りないならボタンは押せない
        if (SkillData.skillDataList[skillNumber].EnhancementPoint > m_gameManager.SaveDataManager.SaveData.saveData.EnhancementPoint)
        {
            OKButton.GetComponent<Button>().interactable = false;
            EnhancementPoint.GetComponent<TextMeshProUGUI>().text = $"必要<sprite=1>{SkillData.skillDataList[skillNumber].EnhancementPoint}";
            return;
        }
        // ボタンのテキストを変更する
        OKButton.GetComponent<Button>().interactable = true;
        EnhancementPoint.GetComponent<TextMeshProUGUI>().text = $"必要<sprite=1>{SkillData.skillDataList[skillNumber].EnhancementPoint}";
    }

    /// <summary>
    /// 解放したスキルデータをセーブする
    /// </summary>
    public void SaveReleaseSkillData(int skillNumberInPlayerData)
    {
        // 値を設定する
        m_gameManager.SaveDataManager.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[skillNumberInPlayerData] = true;
        m_gameManager.SaveDataManager.SaveData.saveData.EnhancementPoint -= SkillData.skillDataList[skillNumberInPlayerData].EnhancementPoint;
        // 値を更新
        HaveEP.GetComponent<TextMeshProUGUI>().text = $"<sprite=1>{m_gameManager.SaveDataManager.SaveData.saveData.EnhancementPoint.ToString("N0")}";
        m_gameManager.SaveDataManager.Save();
    }

    /// <summary>
    /// 属性を設定する
    /// </summary>
    /// <returns>属性名</returns>
    public string SetElementData(int skillNumber)
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
            default:
                element = "ー";
                break;
        }
        return element;
    }

    /// <summary>
    /// スキルボタンを生成
    /// </summary>
    private void InstantiateSkillDataButton()
    {
        DestroyIcon();

        for (int i = 0; i < PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList.Count; i++)
        {
            // ボタンを生成して子オブジェクトにする
            var button = Instantiate(SkillDataIcon);
            button.transform.SetParent(SkillDataContent.transform);
            ChangeName(button, i);
            // サイズを調整
            button.transform.localScale = Vector3.one;
            button.transform.localPosition = Vector3.zero;
            // 自身の番号を教える
            var skillButton = button.GetComponent<SkillButton>();
            skillButton.MyNumber = PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList[i].ID;
            skillButton.MyNumberInPlayerData = i;
            skillButton.SkillNameObject = button;
            // オブジェクトを設定
            var skillStatus = button.GetComponent<DrawSkillStatus>();
            skillStatus.SetObject(SkillDataName, SkillDataElement, SkillDataDetil);
            skillStatus.MyNumber = PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList[i].ID;
            // Animatorを所持しているオブジェクトを教える
            var conponents = button.GetComponents<UIAnimation>();
            conponents[0].AnimatorObject = SkillDataCanvas;
            conponents[1].AnimatorObject = SkillStatusCanvas;
        }
    }

    /// <summary>
    /// 名前を変更する
    /// </summary>
    public void ChangeName(GameObject gameObject, int skillNumberInPlayerData)
    {
        if (m_gameManager.SaveDataManager.SaveData.saveData.SkillRegisters[m_gameManager.PlayerNumber].PlayerSkills[skillNumberInPlayerData] == true)
        {
            gameObject.GetComponent<Button>().interactable = true;
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"　　{PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList[skillNumberInPlayerData].SkillName}";
            return;
        }
        // 解放できる場合は<sprite=1>を使用する。

        // 解放できない場合は押せないようにする
        //gameObject.GetComponent<Button>().interactable = false;
        gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                        $"<sprite=3>{PlayerData.playerDataList[m_gameManager.PlayerNumber].skillDataList[skillNumberInPlayerData].SkillName}";
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

    /// <summary>
    /// リセットする
    /// </summary>
    private void ResetButton()
    {
        for(int i= 0; i < ChangeButton.Length; i++)
        {
            m_playerButtonList[i].ResetButtonDownFlag();
        }
    }
}
