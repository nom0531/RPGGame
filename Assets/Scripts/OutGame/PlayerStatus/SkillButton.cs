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
    private EnhancementDataBase EnhancementData;
    [SerializeField]
    private PlayerDataBase PlayerData;
    [SerializeField, Header("表示用データ"),Tooltip("所持EP")]
    private GameObject Data_HaveEP;

    private PlayerEnhancementSystem m_playerEnhancement;
    private int m_skillNumber = -1;     // 番号
    private int m_selectNumber = 0;     // 現在選択している番号

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
    /// 初期化用の関数。PlayerEnhancementだけを教える
    /// </summary>
    public void SetPlayerEnhancement(PlayerEnhancementSystem playerEnhancement)
    {
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
        var gameManager = GameManager.Instance;
        SaveReleaseSkillData(gameManager);
        SaveReleaseStatusData(gameManager);
        // 変更後のテキストを表示
        Data_HaveEP.GetComponent<TextMeshProUGUI>().text =
        gameManager.SaveData.SaveData.saveData.EnhancementPoint.ToString();
        // ボタンのテキストを変更する
        GetComponent<Button>().interactable = false;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "解放済み";

        gameManager.SaveData.Save();
    }

    /// <summary>
    /// 解放したスキルデータをセーブする
    /// </summary>
    private void SaveReleaseSkillData(GameManager gameManager)
    {
        if (m_playerEnhancement.ReferrenceSkillFlag == false)
        {
            return;
        }
        // 値を設定する
        gameManager.SaveData.SaveData.saveData.SkillRegisters[gameManager.PlayerNumber].PlayerSkills[m_selectNumber] = true;
        gameManager.SaveData.SaveData.saveData.EnhancementPoint -= SkillData.skillDataList[m_selectNumber].EnhancementPoint;
    }

    /// <summary>
    /// 解放したステータスのデータをセーブする
    /// </summary>
    /// <param name="saveDataManager"></param>
    private void SaveReleaseStatusData(GameManager gameManager)
    {
        if (m_playerEnhancement.ReferrenceSkillFlag == true)
        {
            return;
        }
        // 値を設定する
        gameManager.SaveData.SaveData.saveData.EnhancementRegisters[gameManager.PlayerNumber].PlayerEnhancements[m_selectNumber] = true;
        gameManager.SaveData.SaveData.saveData.EnhancementPoint -= EnhancementData.enhancementDataList[m_selectNumber].EnhancementPoint;
        SavePlayerStatus(gameManager, EnhancementData.enhancementDataList[gameManager.PlayerNumber].AddValue);
    }

    /// <summary>
    /// ステータスをセーブする
    /// </summary>
    private void SavePlayerStatus(GameManager gameManager, int addValue)
    {
        switch (EnhancementData.enhancementDataList[gameManager.PlayerNumber].EnhancementStatus)
        {
            case EnhancementStatus.enHP:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].HP += addValue;
                break;
            case EnhancementStatus.enSP:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].SP += addValue;
                break;
            case EnhancementStatus.enATK:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].ATK += addValue;
                break;
            case EnhancementStatus.enDEF:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].DEF += addValue;
                break;
            case EnhancementStatus.enSPD:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].SPD += addValue;
                break;
            case EnhancementStatus.enLUCK:
                gameManager.SaveData.SaveData.saveData.PlayerList[gameManager.PlayerNumber].LUCK += addValue;
                break;
        }
    }
}
