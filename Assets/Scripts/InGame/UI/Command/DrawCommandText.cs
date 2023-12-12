using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCommandText : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private SkillDataBase SkillData;

    /// <summary>
    /// 行動テキストの表示処理
    /// </summary>
    /// <param name="actionType">行動パターン</param>
    /// <param name="skillNumber">使用スキルの番号</param>
    public void SetCommandText(ActionType actionType, int skillNumber = 0)
    {
        string actionText = "";

        // テキストの分岐
        switch (actionType)
        {
            case ActionType.enAttack:
                actionText = "攻撃";
                break;
            case ActionType.enSkillAttack:
                actionText = SkillData.skillDataList[skillNumber].SkillName;
                break;
            case ActionType.enGuard:
                actionText = "防御";
                break;
            case ActionType.enEscape:
                actionText = "逃走";
                break;
            case ActionType.enNull:
                actionText = "様子を見ている";
                break;
        }

        Debug.Log(actionText);
    }

    /// <summary>
    /// バフがかかったときのテキスト
    /// </summary>
    /// <param name="buffStatus">バフのタイプ</param>
    public void SetStatusText(BuffStatus buffStatus)
    {
        string actionText = "";

        // 既にバフがかかっているならば
        if (this.gameObject.GetComponent<BuffCalculation>().GetBuffFlag(buffStatus) == true)
        {
            actionText = "効果時間が伸びた";
            Debug.Log(actionText);
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

        Debug.Log(actionText);
    }

    /// <summary>
    /// バフの効果時間が終了したときのテキスト
    /// </summary>
    public void ReSetStatusText()
    {
        string actionText = "効果時間が終了した";
        Debug.Log(actionText);
    }
}
