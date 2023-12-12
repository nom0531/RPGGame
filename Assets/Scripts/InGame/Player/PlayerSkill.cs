using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSkill : MonoBehaviour
{
    [SerializeField, Header("参照データ"), Tooltip("プレイヤーのデータ")]
    private PlayerDataBase PlayerDataBase;
    [SerializeField, Tooltip("生成するボタン")]
    private GameObject Button;
    [SerializeField, Tooltip("ボタンを追加するオブジェクト")]
    private GameObject Content;
    [SerializeField, Header("表示用データ"), Tooltip("自身の属性")]
    private GameObject Data_Element;
    [SerializeField, Tooltip("スキルの効果")]
    private GameObject Data_SkilDetail;
    [SerializeField, Tooltip("消費する値")]
    private GameObject Data_SkillNecessary;
    [SerializeField, Tooltip("何を消費するかのテキスト")]
    private GameObject Data_SkillNecessaryText;
    [SerializeField, Header("参照オブジェクト"), Tooltip("スキルのテキスト")]
    private GameObject Skill_Status;
    [SerializeField]
    private GameObject Skill_ElementText;
    [SerializeField]
    private GameObject Skill_NecessaryText;

    private int m_playerNumber;                 // プレイヤーの番号
    private int m_selectSkillNumber = -1;       // 現在選択しているスキルの番号
    private BattleManager m_battleManager;

    public int SelectSkillNumber
    {
        get => m_selectSkillNumber;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();

        // スキルデータ
        Data_Element.SetActive(false);
        Data_SkillNecessary.SetActive(false);
        Data_SkillNecessaryText.SetActive(false);
        Data_SkilDetail.SetActive(false);
        Skill_Status.SetActive(false);
        Skill_ElementText.SetActive(false);
        Skill_NecessaryText.SetActive(false);
    }

    /// <summary>
    /// SkillButtonタグが付いた全てのオブジェクトを削除する
    /// </summary>
    public void DestroySkillButton()
    {
        GameObject[] skillButtons = GameObject.FindGameObjectsWithTag("SkillButton");

        foreach (GameObject button in skillButtons)
        {
            Debug.Log("ボタンを削除");
            Destroy(button);
        }
    }

    /// <summary>
    /// ボタンを生成する処理
    /// </summary>
    public void InstantiateSkillButton()
    {
        SaveDataManager saveData = GameManager.Instance.SaveData;
        m_playerNumber = m_battleManager.OperatingPlayerNumber;

        for (int i = 0; i < PlayerDataBase.playerDataList[m_playerNumber].skillDataList.Count; i++)
        {
            // 既にスキルが解放されているなら
            if (saveData.SaveData.saveData.Players[m_playerNumber].PlayerEnhancement[i] == false)
            {
                continue;
            }

            // ボタンを生成して子オブジェクトにする
            GameObject gameObject = Instantiate(Button);
            gameObject.transform.SetParent(Content.transform);
            // サイズを調整
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

            PlayerSkillButton skillButton = gameObject.GetComponent<PlayerSkillButton>();
            skillButton.SetPlayerSkill(
                i,                                                                  // 番号
                PlayerDataBase.playerDataList[m_playerNumber].skillDataList[i].SkillName,   // 名前
                Color.black,
                this
                );

            skillButton.GetComponent<PlayerSkillButton>().SkillNumber = i;          // 番号を教える
        }
    }

    /// <summary>
    /// 入力されたスキルデータを表示する処理
    /// </summary>
    /// <param name="number">スキルの番号</param>
    public void DisplaySetPlayerSkill(int number)
    {
        Data_Element.SetActive(true);
        Data_SkillNecessary.SetActive(true);
        Data_SkillNecessaryText.SetActive(true);
        Data_SkilDetail.SetActive(true);
        Skill_Status.SetActive(true);
        Skill_ElementText.SetActive(true);
        Skill_NecessaryText.SetActive(true);

        // 値を更新する
        Data_SkilDetail.GetComponent<TextMeshProUGUI>().text =
            PlayerDataBase.playerDataList[m_playerNumber].skillDataList[number].SkillDetail;
        Data_SkillNecessary.GetComponent<TextMeshProUGUI>().text =
            PlayerDataBase.playerDataList[m_playerNumber].skillDataList[number].SkillNecessary.ToString();
        GetElement(Data_Element, number, m_playerNumber);
        GetNecessaryText(Data_SkillNecessaryText, number, m_playerNumber);

        // 値をセットする
        m_selectSkillNumber = number;
    }

    /// <summary>
    /// 消費するデータを取得する
    /// </summary>
    /// <param name="gameObject">ゲームオブジェクト</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="playerNumber">プレイヤーの番号</param>
    private void GetNecessaryText(GameObject gameObject,int skillNumber,int playerNumber)
    {
        NecessaryType necessary = PlayerDataBase.playerDataList[playerNumber].skillDataList[skillNumber].Type;

        switch(necessary)
        {
            case NecessaryType.enSP:
                gameObject.GetComponent<TextMeshProUGUI>().text = "SP";
                break;
            case NecessaryType.enHP:
                gameObject.GetComponent<TextMeshProUGUI>().text = "HP";
                break;
        }
    }

    /// <summary>
    /// 属性耐性を表示する処理
    /// </summary>
    /// <param name="gameObjct">ゲームオブジェクト</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="playerNumber">プレイヤーの番号</param>
    private void GetElement(GameObject gameObjct, int skillNumber, int playerNumber)
    {
        ElementType element = PlayerDataBase.playerDataList[playerNumber].skillDataList[skillNumber].SkillElement;

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
            case ElementType.enNone:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "無";
                break;
            default:
                gameObjct.GetComponent<TextMeshProUGUI>().text = "ー";
                break;
        }
    }

    /// <summary>
    /// 選択しているスキルの番号をリセットする
    /// </summary>
    public void ResetSelectSkillNumber()
    {
        m_selectSkillNumber = -1;

        Data_Element.SetActive(false);
        Data_SkillNecessary.SetActive(false);
        Data_SkillNecessaryText.SetActive(false);
        Data_SkilDetail.SetActive(false);
        Skill_Status.SetActive(false);
        Skill_ElementText.SetActive(false);
        Skill_NecessaryText.SetActive(false);
    }
}
