using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerSkill : MonoBehaviour
{
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

    private BattleManager m_battleManager;
    private PlayerDataBase m_playerData;
    private bool m_canUseSkill = true;          // スキルが使えるかどうか
    private int m_playerNumber = 0;             // プレイヤーの番号
    private int m_selectSkillNumber = -1;       // 現在選択しているスキルの番号

    public int SelectSkillNumber
    {
        get => m_selectSkillNumber;
    }

    public bool UseSkillFlag
    {
        get => m_canUseSkill;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_battleManager = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<BattleManager>();
        m_playerData = m_battleManager.PlayerDataBase;
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

        foreach (var button in skillButtons)
        {
            Destroy(button);
        }
    }

    /// <summary>
    /// ボタンを生成する処理
    /// </summary>
    public void InstantiateSkillButton()
    {
        var saveDataManager = GameManager.Instance.SaveDataManager;
        m_playerNumber = (int)m_battleManager.OperatingPlayer;

        for (int i = 0; i < m_playerData.playerDataList[m_playerNumber].skillDataList.Count; i++)
        {
            // 既にスキルが解放されているなら
            if (saveDataManager.SaveData.saveData.SkillRegisters[m_playerNumber].PlayerSkills[i] == false)
            {
                continue;
            }

            // ボタンを生成して子オブジェクトにする
            var gameObject = Instantiate(Button);
            gameObject.transform.SetParent(Content.transform);
            // サイズを調整
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;

            var skillButton = gameObject.GetComponent<PlayerSkillButton>();
            skillButton.SetPlayerSkill(
                i,                                                                          // 番号
                m_playerData.playerDataList[m_playerNumber].skillDataList[i].SkillName,     // 名前
                this
                );

            skillButton.GetComponent<PlayerSkillButton>().SkillNumber = i;                  // 番号を教える
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
            m_playerData.playerDataList[m_playerNumber].skillDataList[number].SkillDetail;
        Data_SkillNecessary.GetComponent<TextMeshProUGUI>().text =
            m_playerData.playerDataList[m_playerNumber].skillDataList[number].SkillNecessary.ToString();
        GetElement(Data_Element, number, m_playerNumber);
        GetNecessaryText(Data_SkillNecessaryText, number, m_playerNumber);
        // 値をセットする
        m_selectSkillNumber = number;
        m_canUseSkill = true;
        SetUseFlag(number, m_playerData.playerDataList[m_playerNumber].skillDataList[number].SkillNecessary,
            m_playerData.playerDataList[m_playerNumber].skillDataList[number].Type);
    }

    /// <summary>
    /// スキルが使用できるかどうかの判定を行う
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    /// <param name="nesessaryPoint">必要ポイント</param>
    /// <param name="necessaryType">消費するポイントの種類</param>
    private void SetUseFlag(int number,int nesessaryPoint, NecessaryType necessaryType)
    {
        switch (necessaryType)
        {
            case NecessaryType.enSP:
                // SPが足りていないならボタンは押せない
                if (m_battleManager.PlayerMoveList[m_playerNumber].PlayerStatus.SP < nesessaryPoint)
                {
                    m_canUseSkill = false;
                }
                return;
            case NecessaryType.enHP:
                var hp = m_battleManager.PlayerMoveList[m_playerNumber].PlayerStatus.HP;
                // HPが足りていない、消費した際にHPが0になるならボタンは押せない
                if (hp < nesessaryPoint || hp - nesessaryPoint <= 0)
                {
                    m_canUseSkill = false;
                }
                return;
        }
    }

    /// <summary>
    /// 消費するデータを取得する
    /// </summary>
    /// <param name="gameObject">ゲームオブジェクト</param>
    /// <param name="skillNumber">スキルの番号</param>
    /// <param name="playerNumber">プレイヤーの番号</param>
    private void GetNecessaryText(GameObject gameObject,int skillNumber,int playerNumber)
    {
        NecessaryType necessary = m_playerData.playerDataList[playerNumber].skillDataList[skillNumber].Type;

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
        ElementType element = m_playerData.playerDataList[playerNumber].skillDataList[skillNumber].SkillElement;

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
                gameObjct.GetComponent<TextMeshProUGUI>().text = "-";
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
