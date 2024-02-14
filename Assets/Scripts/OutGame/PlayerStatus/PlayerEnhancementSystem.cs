using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerEnhancementSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private PlayerDataBase PlayerData;
    [SerializeField]
    private SkillDataBase SkillData;
    [SerializeField]
    private EnhancementDataBase EnhancementData;
    [SerializeField, Header("プレイヤーリスト"), Tooltip("生成するボタン")]
    private GameObject PlayerIconButton;
    [SerializeField, Tooltip("ボタンを追加するオブジェクト")]
    private GameObject PlayerContent;
    [SerializeField, Header("スキルリスト"), Tooltip("生成するボタン")]
    private GameObject IconButton;
    [SerializeField, Tooltip("ボタンを追加するオブジェクト")]
    private GameObject Content;
    [SerializeField, Header("表示用データ")]
    private GameObject Data_Sprite;
    [SerializeField]
    private GameObject Data_Name;
    [SerializeField]
    private GameObject Data_Element;
    [SerializeField, Tooltip("スキルの効果")]
    private GameObject Data_Detail;
    [SerializeField, Tooltip("必要なEPの量")]
    private GameObject Data_NecessaryEP;
    [SerializeField, Tooltip("所持EP")]
    private GameObject Data_HaveEP;
    [SerializeField]
    private GameObject Data_PlayerName;
    [SerializeField, Header("参照オブジェクト")]
    private GameObject Status;
    [SerializeField]
    private GameObject ReleaseButton;
    [SerializeField]
    private GameObject SkillButton, StatusButton;

    private GameManager m_gameManager;
    private SkillButton m_skillButton;
    private bool m_isReferenceSkill = true;     // trueならスキルデータ。falseならステータスを参照する

    public bool ReferrenceSkillFlag
    {
        get => m_isReferenceSkill;
        set => m_isReferenceSkill = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GameManager.Instance;
        // スキルデータ
        Data_Sprite.SetActive(false);
        Data_Name.SetActive(false);
        Data_Detail.SetActive(false);
        Status.SetActive(false);
        // プレイヤーデータ
        Data_PlayerName.SetActive(false);
        Data_HaveEP.SetActive(true);
        // 値を更新
        Data_HaveEP.GetComponent<TextMeshProUGUI>().text = m_gameManager.SaveData.SaveData.saveData.EnhancementPoint.ToString("N0");
        ReleaseButton.GetComponent<Button>().interactable = false;

        for (int playerNumber = 0; playerNumber < PlayerData.playerDataList.Count; playerNumber++)
        {
            // ボタンを生成して子オブジェクトにする
            var playerObject = Instantiate(PlayerIconButton);
            playerObject.transform.SetParent(PlayerContent.transform);
            // サイズを調整
            playerObject.transform.localScale = Vector3.one;
            playerObject.transform.localPosition = Vector3.zero;
            // 初期化
            var playerButton = playerObject.GetComponent<PlayerButton>();
            playerButton.SetPlayerEnhancement(
                playerNumber,                                               // 番号
                PlayerData.playerDataList[playerNumber].PlayerSprite,       // 画像
                this
                );
        }
        m_skillButton = ReleaseButton.GetComponent<SkillButton>();
        m_skillButton.SetPlayerEnhancement(this);

        // 初期化
        var skillButton = SkillButton.GetComponent<PlayerButton>();
        skillButton.SetPlayerEnhancement(this);
        var statusButton = StatusButton.GetComponent<PlayerButton>();
        statusButton.SetPlayerEnhancement(this);

        if(ReferrenceSkillFlag == true)
        {
            DisplaySetSkillData(m_gameManager.PlayerNumber);
            return;
        }
        DisplaySetStatusData(m_gameManager.PlayerNumber);
    }

    /// <summary>
    /// スキルのデータを表示する
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    public void DisplaySetSkillData(int number)
    {
        StatusButton.GetComponent<Image>().color = Color.white;
        SkillButton.GetComponent<Image>().color = Color.gray;
        StatusButton.GetComponent<Button>().interactable = true;
        SkillButton.GetComponent<Button>().interactable = false;

        DestroyIcon();
        Data_HaveEP.SetActive(true);
        Data_PlayerName.SetActive(true);
        ReleaseButton.GetComponent<Button>().interactable = false;
        // 値を更新する
        Data_PlayerName.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[number].PlayerName;
        // 現在のプレイヤーの番号を記録
        GameManager.Instance.PlayerNumber = number;
        InstantiateSkillIcon(number);
        // フラグを設定する
        ReferrenceSkillFlag = true;
    }

    /// <summary>
    /// ステータス強化のデータを表示する
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    public void DisplaySetStatusData(int number)
    {
        StatusButton.GetComponent<Image>().color = Color.gray;
        SkillButton.GetComponent<Image>().color = Color.white;
        StatusButton.GetComponent<Button>().interactable = false;
        SkillButton.GetComponent<Button>().interactable = true;

        DestroyIcon();
        Data_HaveEP.SetActive(true);
        Data_PlayerName.SetActive(true);
        ReleaseButton.GetComponent<Button>().interactable = false;
        // 値を更新する
        Data_PlayerName.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[number].PlayerName;
        // 現在のプレイヤーの番号を記録
        GameManager.Instance.PlayerNumber = number;
        InstantiateStatusIcon(number);
        // フラグを設定する
        ReferrenceSkillFlag = false;
    }

    /// <summary>
    /// 入力されたスキルデータを表示する処理
    /// </summary>
    /// <param name="number">スキルの番号</param>
    public void DisplaySetSkill(int number)
    {
        m_skillButton.SelectNumber = number;
        var playerNumber = GameManager.Instance.PlayerNumber;
        var enhancementPoint = PlayerData.playerDataList[playerNumber].skillDataList[number].EnhancementPoint;

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Data_Detail.SetActive(true);
        Status.SetActive(true);

        // 値を更新する
        Data_Sprite.GetComponent<Image>().sprite =
            PlayerData.playerDataList[playerNumber].skillDataList[number].SkillSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[playerNumber].skillDataList[number].SkillName;
        Data_Detail.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[playerNumber].skillDataList[number].SkillDetail;
        Data_NecessaryEP.GetComponent<TextMeshProUGUI>().text = enhancementPoint.ToString();
        GetElement(Data_Element, number, playerNumber);
        // ボタンが押せるかどうか判定する
        SetInterctable(enhancementPoint, m_gameManager.SaveData.SaveData.saveData.SkillRegisters[playerNumber].PlayerSkills[number]);
    }

    /// <summary>
    /// 入力されたステータスデータを表示する
    /// </summary>
    /// <param name="number">ステータスの番号</param>
    public void DisplaySetStatus(int number)
    {
        m_skillButton.SelectNumber = number;
        var enhancementPoint = PlayerData.playerDataList[m_gameManager.PlayerNumber].enhancementDataList[number].EnhancementPoint;

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Data_Detail.SetActive(true);
        Status.SetActive(true);

        // 値を更新する
        Data_Sprite.GetComponent<Image>().sprite =
            PlayerData.playerDataList[m_gameManager.PlayerNumber].enhancementDataList[number].EnhancementSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[m_gameManager.PlayerNumber].enhancementDataList[number].EnhancementName;
        Data_NecessaryEP.GetComponent<TextMeshProUGUI>().text = enhancementPoint.ToString();
        GetStatusName(Data_Detail,number);
        Data_Element.GetComponent<TextMeshProUGUI>().text = "-";
        // ボタンを押せるかどうか判定する
        SetInterctable(enhancementPoint, m_gameManager.SaveData.SaveData.saveData.
            EnhancementRegisters[m_gameManager.PlayerNumber].PlayerEnhancements[number]);
    }

    /// <summary>
    /// ステータスの名前を設定する
    /// </summary>
    /// <param name="number">ステータスの番号</param>
    private void GetStatusName(GameObject gameObject, int number)
    {
        var statusName = "";
        var baseStatus = 0;
        var addValue = PlayerData.playerDataList[m_gameManager.PlayerNumber].enhancementDataList[number].AddValue;
        switch (PlayerData.playerDataList[m_gameManager.PlayerNumber].enhancementDataList[number].EnhancementStatus)
        {
            case EnhancementStatus.enHP:
                statusName = "HP";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].HP;
                break;
            case EnhancementStatus.enSP:
                statusName = "SP";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].SP;
                break;
            case EnhancementStatus.enATK:
                statusName = "ATK";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].ATK;
                break;
            case EnhancementStatus.enDEF:
                statusName = "DEF";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].DEF;
                break;
            case EnhancementStatus.enSPD:
                statusName = "SPD";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].SPD;
                break;
            case EnhancementStatus.enLUCK:
                statusName = "LUCK";
                baseStatus = m_gameManager.SaveData.SaveData.saveData.PlayerList[m_gameManager.PlayerNumber].LUCK;
                break;
        }
        if(m_gameManager.SaveData.SaveData.saveData.EnhancementRegisters[m_gameManager.PlayerNumber].PlayerEnhancements[number] == true)
        {
            gameObject.GetComponent<TextMeshProUGUI>().text =
            $"{statusName}を{addValue}強化する。( - )";
            return;
        }
        gameObject.GetComponent<TextMeshProUGUI>().text =　
            $"{statusName}を{addValue}強化する。({baseStatus}→{baseStatus + addValue})";
    }

    /// <summary>
    /// 属性耐性を設定する
    /// </summary>
    /// <param name="number">スキルの番号</param>
    /// <param name="playerNumber">プレイヤーの番号</param>
    private void GetElement(GameObject gameObjct, int number,int playerNumber)
    {
        var element = PlayerData.playerDataList[playerNumber].skillDataList[number].SkillElement;
        switch (element)
        {
            case ElementType.enFire:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "炎";
                break;
            case ElementType.enIce:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "氷";
                break;
            case ElementType.enWind:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "風";
                break;
            case ElementType.enThunder:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "雷";
                break;
            case ElementType.enLight:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "光";
                break;
            case ElementType.enDark:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "闇";
                break;
            default:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "-";
                break;
        }
    }

    /// <summary>
    /// EnhancementIconタグの付いたオブジェクトを全て削除する処理
    /// </summary>
    private void DestroyIcon()
    {
        var skillIcons = GameObject.FindGameObjectsWithTag("EnhancementIcon");
        foreach (var button in skillIcons)
        {
            Destroy(button);
        }
    }

    /// <summary>
    /// アイコンを生成する処理
    /// </summary>
    /// <param name="playerNumber">プレイヤーの番号</param>
    private void InstantiateSkillIcon(int playerNumber)
    {
        for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[playerNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // 識別番号が同じならデータを初期化する
                if (PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].ID == SkillData.skillDataList[dataNumber].ID)
                {
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillDetail = SkillData.skillDataList[dataNumber].SkillDetail;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillEffect = SkillData.skillDataList[dataNumber].SkillEffect;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                    PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
                    break;
                }
            }
            // ボタンを生成
            var button = Instantiate(IconButton);
            button.transform.SetParent(Content.transform);
            // サイズを調整する
            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = Vector3.one;
            button.transform.localRotation = Quaternion.identity;
            // コンポーネントを取得
            var skillButton = button.GetComponent<SkillButton>();
            skillButton.SetPlayerEnhancement(
                skillNumber,
                PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillSprite,
                true,
                this
                );
            button.SetActive(true);
        }
    }

    /// <summary>
    /// アイコンを生成する処理
    /// </summary>
    /// <param name="playerNumber">プレイヤーの番号</param>
    private void InstantiateStatusIcon(int playerNumber)
    {
        var number = 0; // 強化データのID
        for (int statusNumber = 0; statusNumber < PlayerData.playerDataList[playerNumber].enhancementDataList.Count; statusNumber++)
        {
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].ID = number;
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].EnhancementName = EnhancementData.enhancementDataList[number].EnhancementName;
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].EnhancementSprite = EnhancementData.enhancementDataList[number].EnhancementSprite;
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].EnhancementStatus = EnhancementData.enhancementDataList[number].EnhancementStatus;
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].EnhancementPoint = EnhancementData.enhancementDataList[number].EnhancementPoint;
            PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].AddValue = EnhancementData.enhancementDataList[number].AddValue;
            // ボタンを生成
            var button = Instantiate(IconButton);
            button.transform.SetParent(Content.transform);
            // サイズを調整する
            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = Vector3.one;
            button.transform.localRotation = Quaternion.identity;
            // コンポーネントを取得
            var skillButton = button.GetComponent<SkillButton>();
            skillButton.SetPlayerEnhancement(
                statusNumber,
                PlayerData.playerDataList[playerNumber].enhancementDataList[statusNumber].EnhancementSprite,
                true,
                this
                );
            button.SetActive(true);
            number++;
        }
    }

    /// <summary>
    /// ボタンが押せるかどうか判定する
    /// </summary>
    /// <param name="enhancementPoint">必要なポイント量</param>
    /// <param name="isAcquired">取得済みかどうか</param>
    /// <returns>trueなら押せる。falseなら押せない</returns>
    private void SetInterctable(int enhancementPoint, bool isAcquired)
    {
        // 必要量に足りていない　または　既に使用できる状態ならボタンが押せないようにする
        if (isAcquired == true)
        {
            ReleaseButton.GetComponent<Button>().interactable = false;
            ReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "取得済み";
            return;
        }
        if(m_gameManager.SaveData.SaveData.saveData.EnhancementPoint < enhancementPoint)
        {
            ReleaseButton.GetComponent<Button>().interactable = false;
            ReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "EP不足";
            return;
        }
        ReleaseButton.GetComponent<Button>().interactable = true;
        ReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "取得";
    }
}
