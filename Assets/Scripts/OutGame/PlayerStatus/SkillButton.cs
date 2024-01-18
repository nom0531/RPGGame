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
    // 現在選択している番号
    private int m_selectNumber = 0;
    // 図鑑システム
    private PlayerEnhancementSystem m_playerEnhancement;

    public int SelectNumber
    {
        get => m_selectNumber;
        set => m_selectNumber = value;
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
        if(m_playerEnhancement.ReferrenceSkillFlag == true)
        {
            m_playerEnhancement.DisplaySetSkill(m_skillNumber);
        }
        else
        {
            m_playerEnhancement.DisplaySetStatus(m_skillNumber);
        }
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void IsUseButtonDown()
    {
        var saveDataManager = GameManager.Instance.SaveData;

        SaveSkillData(saveDataManager);
        SaveStatusData(saveDataManager);

        Data_HaveEP.GetComponent<TextMeshProUGUI>().text =
        saveDataManager.SaveData.saveData.EnhancementPoint.ToString();
        // ボタンのテキストを変更する
        GetComponent<Button>().interactable = false;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "解放済み";

        saveDataManager.Save();
    }

    /// <summary>
    /// 解放したスキルデータをセーブする
    /// </summary>
    private void SaveSkillData(SaveDataManager saveDataManager)
    {
        if(m_playerEnhancement.ReferrenceSkillFlag == false)
        {
            return;
        }

        // 値を設定する
        saveDataManager.SaveData.saveData.SkillRegisters[PlayerNumberManager.PlayerNumber].PlayerSkills[m_selectNumber] = true;
        saveDataManager.SaveData.saveData.EnhancementPoint -= SkillData.skillDataList[m_selectNumber].EnhancementPoint;
    }

    /// <summary>
    /// 解放したステータスのデータをセーブする
    /// </summary>
    private void SaveStatusData(SaveDataManager saveDataManager)
    {
        if(m_playerEnhancement.ReferrenceSkillFlag == true)
        {
            return;
        }
        // 値を設定する
    }
}
