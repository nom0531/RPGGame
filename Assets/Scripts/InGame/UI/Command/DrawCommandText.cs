using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCommandText : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private SkillDataBase SkillData;

    private StagingSystem m_stagingSystem;
    private BuffCalculation m_buffCalculation;

    private void Start()
    {
        m_stagingSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<StagingSystem>();
        m_buffCalculation = GetComponent<BuffCalculation>();
    }

    /// <summary>
    /// バフがかかったときのテキスト
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    public void SetStatusText(BuffStatus buffStatus)
    {
        string actionText = "";

        // 既にバフがかかっているならば
        if (m_buffCalculation.GetBuffFlag(buffStatus) == true)
        {
            actionText = "効果時間が伸びた";
            return;
        }

        switch (buffStatus)
        {
            case BuffStatus.enBuff_ATK:
                actionText = "攻撃力が上がった";
                break;
            case BuffStatus.enBuff_DEF:
                actionText = "防御力が上がった";
                break;
            case BuffStatus.enBuff_SPD:
                actionText = "素早さが上がった";
                break;
            case BuffStatus.enDeBuff_ATK:
                actionText = "攻撃力が下がった";
                break;
            case BuffStatus.enDeBuff_DEF:
                actionText = "防御力が下がった";
                break;
            case BuffStatus.enDeBuff_SPD:
                actionText = "素早さが下がった";
                break;
        }
        m_stagingSystem.SetAddInfoCommandText(actionText);
    }

    /// <summary>
    /// バフの効果時間が終了したときのテキスト
    /// </summary>
    public void ReSetStatusText()
    {
        string actionText = "効果時間が終了した";
        m_stagingSystem.SetAddInfoCommandText(actionText);
#if UNITY_EDITOR
        Debug.Log(actionText);
#endif
    }
}
