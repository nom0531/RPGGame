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
    [SerializeField, Header("プレイヤーリスト"), Tooltip("生成するボタン")]
    private GameObject PlayerIconButton;
    [SerializeField, Tooltip("ボタンを追加するオブジェクト")]
    private GameObject PlayerContent;
    [SerializeField, Header("スキルリスト"), Tooltip("生成するボタン")]
    private GameObject IconButton;
    [SerializeField, Tooltip("ボタンを追加するオブジェクト")]
    private GameObject Content;
    [SerializeField, Header("表示用データ"), Tooltip("画像")]
    private GameObject Data_Sprite;
    [SerializeField, Tooltip("名前")]
    private GameObject Data_Name;
    [SerializeField, Tooltip("属性")]
    private GameObject Data_Element;
    [SerializeField, Tooltip("スキルの効果")]
    private GameObject Data_SkilDetail;
    [SerializeField, Tooltip("必要なEPの量")]
    private GameObject Data_SkillNecessaryEP;
    [SerializeField, Tooltip("所持EP")]
    private GameObject Data_HaveEP;
    [SerializeField, Tooltip("プレイヤー名")]
    private GameObject Data_PlayerName;
    [SerializeField, Header("参照オブジェクト")]
    private GameObject SkillStatus;
    [SerializeField]
    private GameObject SkillReleaseButton;
    [SerializeField]
    private GameObject SkillButton, StatusButton;
    [SerializeField, Tooltip("ボタンの画像")]
    private Sprite ButtonImage;

    private const string BEFORE_ACQUISITION = "解放";
    private const string AFTER_ACQUISITION = "解放済み";

    private SaveDataManager m_saveDataManager;
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
        // スキルデータ
        Data_Sprite.SetActive(false);
        Data_Name.SetActive(false);
        Data_SkilDetail.SetActive(false);
        SkillStatus.SetActive(false);
        // プレイヤーデータ
        Data_PlayerName.SetActive(false);
        Data_HaveEP.SetActive(true);
        m_saveDataManager = GameManager.Instance.SaveData;
        // 値を更新
        Data_HaveEP.GetComponent<TextMeshProUGUI>().text = m_saveDataManager.SaveData.saveData.EnhancementPoint.ToString("N0");

        SkillReleaseButton.GetComponent<Button>().interactable = false;

        for (int playerNumber = 0; playerNumber < PlayerData.playerDataList.Count; playerNumber++)
        {
            // ボタンを生成して子オブジェクトにする
            var playerObject = Instantiate(PlayerIconButton);
            playerObject.transform.SetParent(PlayerContent.transform);
            // サイズを調整
            playerObject.transform.localScale = Vector3.one;
            playerObject.transform.localPosition = Vector3.zero;

            var playerButton = playerObject.GetComponent<PlayerButton>();
            playerButton.SetPlayerEnhancement(
                playerNumber,                                               // 番号
                PlayerData.playerDataList[playerNumber].PlayerSprite,       // 画像
                this
                );
        }
        m_skillButton = SkillReleaseButton.GetComponent<SkillButton>();

        // 初期化
        var skillButton = SkillButton.GetComponent<PlayerButton>();
        skillButton.SetPlayerEnhancement(
            PlayerNumberManager.PlayerNumber,
            ButtonImage,       // 画像
            this
            );
        var statusButton = StatusButton.GetComponent<PlayerButton>();
        statusButton.SetPlayerEnhancement(
            PlayerNumberManager.PlayerNumber,
            ButtonImage,       // 画像
            this
            );

        if(ReferrenceSkillFlag == true)
        {
            DisplaySetSkillData(PlayerNumberManager.PlayerNumber);
            return;
        }
        DisplaySetStatusData(PlayerNumberManager.PlayerNumber);
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
        SkillReleaseButton.GetComponent<Button>().interactable = false;
        // 値を更新する
        Data_PlayerName.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[number].PlayerName;
        // 現在のプレイヤーの番号を記録
        PlayerNumberManager.PlayerNumber = number;
        InstantiateSkillIcon();
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
        SkillReleaseButton.GetComponent<Button>().interactable = false;
        // 値を更新する
        Data_PlayerName.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[number].PlayerName;
        // 現在のプレイヤーの番号を記録
        PlayerNumberManager.PlayerNumber = number;
        InstantiateStatusIcon();
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

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Data_SkilDetail.SetActive(true);
        SkillStatus.SetActive(true);

        // 値を更新する
        Data_Sprite.GetComponent<Image>().sprite =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillName;
        Data_SkilDetail.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillDetail;
        Data_SkillNecessaryEP.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].EnhancementPoint.ToString();
        GetElement(Data_Element, number, PlayerNumberManager.PlayerNumber);

        // 既に使用できる状態ならボタンが押せないようにする
        if (m_saveDataManager.SaveData.saveData.SkillRegisters[PlayerNumberManager.PlayerNumber].PlayerSkills[number] == true)
        {
            SkillReleaseButton.GetComponent<Button>().interactable = false;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = AFTER_ACQUISITION;
        }
        else
        {
            SkillReleaseButton.GetComponent<Button>().interactable = true;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = BEFORE_ACQUISITION;
        }
    }

    /// <summary>
    /// 入力されたステータスデータを表示する
    /// </summary>
    /// <param name="number">ステータスの番号</param>
    public void DisplaySetStatus(int number)
    {
        m_skillButton.SelectNumber = number;

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Data_SkilDetail.SetActive(true);
        SkillStatus.SetActive(true);

        // 値を更新する
        Data_Sprite.GetComponent<Image>().sprite =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillName;
        Data_SkilDetail.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].SkillDetail;
        Data_SkillNecessaryEP.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[number].EnhancementPoint.ToString();
        GetElement(Data_Element, number, PlayerNumberManager.PlayerNumber);

        // 既に使用できる状態ならボタンが押せないようにする
        if (m_saveDataManager.SaveData.saveData.SkillRegisters[PlayerNumberManager.PlayerNumber].PlayerSkills[number] == true)
        {
            SkillReleaseButton.GetComponent<Button>().interactable = false;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = AFTER_ACQUISITION;
        }
        else
        {
            SkillReleaseButton.GetComponent<Button>().interactable = true;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = BEFORE_ACQUISITION;
        }
    }

    /// <summary>
    /// 属性耐性を表示する処理
    /// </summary>
    /// <param name="gameObjct">ゲームオブジェクト</param>
    /// <param name="number">スキルの番号</param>
    /// <param name="playerNumber">プレイヤーの番号</param>
    private void GetElement(GameObject gameObjct, int number,int playerNumber)
    {
        ElementType element = PlayerData.playerDataList[playerNumber].skillDataList[number].SkillElement;

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
    private void InstantiateSkillIcon()
    {
        for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // 識別番号が同じならデータを初期化する
                if (PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillNumber == SkillData.skillDataList[dataNumber].SkillNumber)
                {
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillDetail = SkillData.skillDataList[dataNumber].SkillDetail;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
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
                PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillSprite,
                true,
                this
                );
            button.SetActive(true);
        }
    }

    /// <summary>
    /// アイコンを生成する処理
    /// </summary>
    private void InstantiateStatusIcon()
    {
        for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // 識別番号が同じならデータを初期化する
                if (PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillNumber == SkillData.skillDataList[dataNumber].SkillNumber)
                {
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillDetail = SkillData.skillDataList[dataNumber].SkillDetail;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                    PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
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
            button.GetComponent<Image>().color = Color.cyan;
            // コンポーネントを取得
            var skillButton = button.GetComponent<SkillButton>();
            skillButton.SetPlayerEnhancement(
                skillNumber,
                PlayerData.playerDataList[PlayerNumberManager.PlayerNumber].skillDataList[skillNumber].SkillSprite,
                true,
                this
                );
            button.SetActive(true);
        }
    }
}
