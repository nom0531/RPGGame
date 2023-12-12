using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerEnhancementSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ"), Tooltip("プレイヤーデータ")]
    private PlayerDataBase PlayerData;
    [SerializeField, Tooltip("スキルデータ")]
    private SkillDataBase SkillData;
    [SerializeField, Header("プレイヤーリスト"), Tooltip("生成するボタン")]
    private GameObject PlayerIconButton;
    [SerializeField, Tooltip("ボタンを追加するオブジェクト")]
    private GameObject PlayerContent;
    [SerializeField, Header("スキルリスト"), Tooltip("生成するボタン")]
    private GameObject SkillIconButton;
    [SerializeField, Tooltip("ボタンを追加するオブジェクト")]
    private GameObject SkillContent;
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

    private int m_playerNumber = 0;
    private SaveDataManager m_saveDataManager;
    private SkillButton m_skillButton;

    public int PlayerNumber
    {
        get => m_playerNumber;
        set => m_playerNumber = value;
    }

    /// <summary>
    /// 現在選択しているプレイヤーの番号を設定する
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    public void SetSelectPlayerNumber(int number)
    {
        m_playerNumber = number;
    }

    /// <summary>
    /// 現在選択しているプレイヤーの番号を取得する
    /// </summary>
    /// <returns>プレイヤーの番号</returns>
    public int GetSelectPlayerNumber()
    {
        return m_playerNumber;
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
        Data_HaveEP.GetComponent<TextMeshProUGUI>().text =
            m_saveDataManager.SaveData.saveData.EnhancementPoint.ToString();

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
                playerNumber,                                           // 番号
                PlayerData.playerDataList[playerNumber].PlayerSprite,       // 画像
                this
                );
        }

        m_skillButton = SkillReleaseButton.GetComponent<SkillButton>();
        // 表示する
        DisplaySetValue(m_playerNumber);
    }

    /// <summary>
    /// 入力されたデータを表示する処理
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    public void DisplaySetValue(int number)
    {
        m_skillButton.SetSelectPlayerNumber(number);
        DestroySkillIcon();

        Data_HaveEP.SetActive(true);
        Data_PlayerName.SetActive(true);
        SkillReleaseButton.GetComponent<Button>().interactable = false;

        // 値を更新する
        Data_PlayerName.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[number].PlayerName;

        // 現在のプレイヤーの番号を記録
        m_playerNumber = number;

        InstantiateSkillIcon();
    }

    /// <summary>
    /// 入力されたスキルデータを表示する処理
    /// </summary>
    /// <param name="number">スキルの番号</param>
    public void DisplaySetSkill(int number)
    {
        m_skillButton.SetSelectSkillNUmber(number);

        Data_Sprite.SetActive(true);
        Data_Name.SetActive(true);
        Data_SkilDetail.SetActive(true);
        SkillStatus.SetActive(true);

        // 値を更新する
        Data_Sprite.GetComponent<Image>().sprite =
            PlayerData.playerDataList[m_playerNumber].skillDataList[number].SkillSprite;
        Data_Name.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[m_playerNumber].skillDataList[number].SkillName;
        Data_SkilDetail.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[m_playerNumber].skillDataList[number].SkillDetail;
        Data_SkillNecessaryEP.GetComponent<TextMeshProUGUI>().text =
            PlayerData.playerDataList[m_playerNumber].skillDataList[number].EnhancementPoint.ToString();
        GetElement(Data_Element, number, m_playerNumber);

        // 既に使用できる状態ならボタンが押せないようにする
        if (m_saveDataManager.SaveData.saveData.Players[m_playerNumber].PlayerEnhancement[number] == true)
        {
            SkillReleaseButton.GetComponent<Button>().interactable = false;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "解放済み";
        }
        else
        {
            SkillReleaseButton.GetComponent<Button>().interactable = true;
            SkillReleaseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "スキル解放";
        }
    }

    /// <summary>
    /// 属性耐性を表示する処理
    /// </summary>
    /// <param name="gameObjct">ゲームオブジェクト</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="playerNumber">プレイヤーの番号</param>
    void GetElement(GameObject gameObjct, int skillNumber,int playerNumber)
    {
        ElementType element = PlayerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillElement;

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
                gameObjct.GetComponent<TextMeshProUGUI>().text = "ー";
                break;
        }
    }

    /// <summary>
    /// SkillIconタグの付いたオブジェクトを全て削除する処理
    /// </summary>
    private void DestroySkillIcon()
    {
        var skillIcons = GameObject.FindGameObjectsWithTag("SkillIcon");

        foreach (var button in skillIcons)
        {
            Destroy(button);
        }
    }

    /// <summary>
    /// スキルアイコンを生成する処理
    /// </summary>
    private void InstantiateSkillIcon()
    {
        for (int skillNumber = 0; skillNumber < PlayerData.playerDataList[m_playerNumber].skillDataList.Count; skillNumber++)
        {
            for (int dataNumber = 0; dataNumber < SkillData.skillDataList.Count; dataNumber++)
            {
                // 識別番号が同じならデータを初期化する
                if (PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillNumber == SkillData.skillDataList[dataNumber].SkillNumber)
                {
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillName = SkillData.skillDataList[dataNumber].SkillName;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillSprite = SkillData.skillDataList[dataNumber].SkillSprite;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].POW = SkillData.skillDataList[dataNumber].POW;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillElement = SkillData.skillDataList[dataNumber].SkillElement;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillNecessary = SkillData.skillDataList[dataNumber].SkillNecessary;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillDetail = SkillData.skillDataList[dataNumber].SkillDetail;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].EnhancementPoint = SkillData.skillDataList[dataNumber].EnhancementPoint;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].Type = SkillData.skillDataList[dataNumber].Type;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].BuffType = SkillData.skillDataList[dataNumber].BuffType;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillType = SkillData.skillDataList[dataNumber].SkillType;
                    PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].EffectRange = SkillData.skillDataList[dataNumber].EffectRange;
                    break;
                }
            }

            // ボタンを生成
            var button = Instantiate(SkillIconButton);
            button.transform.SetParent(SkillContent.transform);
            // サイズを調整する
            button.transform.localPosition = Vector3.zero;
            button.transform.localScale = Vector3.one;
            button.transform.localRotation = Quaternion.identity;
            // コンポーネントを取得
            var skillButton = button.GetComponent<SkillButton>();
            skillButton.SetPlayerEnhancement(
                skillNumber,
                PlayerData.playerDataList[m_playerNumber].skillDataList[skillNumber].SkillSprite,
                true,
                this
                );

            button.SetActive(true);
        }
    }
}
