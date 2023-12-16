using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private SkillDataBase SkillData;
    [SerializeField]
    private PlayerDataBase PlayerData;
    [SerializeField, Header("表示用データ"),Tooltip("所持EP")]
    private GameObject Data_HaveEP;

    // スキルの番号
    private int m_skillNumber = -1;
    // 現在選択しているスキルの番号
    private int m_selectSkillNumber = 0;
    private int m_selectPlayerNumber = 0;
    // 図鑑システム
    private PlayerEnhancementSystem m_playerEnhancement;

    /// <summary>
    /// 選択しているスキルの番号を教える
    /// </summary>
    /// <param name="number">スキルの番号</param>
    public void SetSelectSkillNUmber(int number)
    {
        m_selectSkillNumber = number;
    }

    /// <summary>
    /// 選択しているプレイヤーの番号を教える
    /// </summary>
    /// <param name="number">プレイヤーの番号</param>
    public void SetSelectPlayerNumber(int number)
    {
        m_selectPlayerNumber = number;
    }

    /// <summary>
    /// 初期化用の関数。スキルを登録する
    /// </summary>
    /// <param name="number">スキルの番号</param>
    /// <param name="skillImage">スキルの画像</param>
    public void SetPlayerEnhancement(int number, Sprite skillImage,bool interactable, PlayerEnhancementSystem playerEnhancement)
    {
        // それぞれの値を登録する
        m_skillNumber = number;
        GetComponent<Image>().sprite = skillImage;
        GetComponent<Button>().interactable = interactable;
        // 図鑑システムを登録する
        m_playerEnhancement = playerEnhancement;
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void ButtonDown()
    {
        m_playerEnhancement.DisplaySetSkill(m_skillNumber);
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void IsUseButtonDown()
    {
        var saveDataManager = GameManager.Instance.SaveData;

        // 値を設定する
        saveDataManager.SaveData.saveData.SkillRegisters[m_selectPlayerNumber].PlayerSkills[m_selectSkillNumber] = true;
        saveDataManager.SaveData.saveData.EnhancementPoint -= SkillData.skillDataList[m_selectSkillNumber].EnhancementPoint;
        Data_HaveEP.GetComponent<TextMeshProUGUI>().text =
            saveDataManager.SaveData.saveData.EnhancementPoint.ToString();

        // ボタンのテキストを変更する
        GetComponent<Button>().interactable = false;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "解放済み";

        saveDataManager.Save();
    }
}
