using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetEP : MonoBehaviour
{
    [SerializeField, Header("表示するテキスト")]
    private GameObject Text;

    private int m_EP = 0;

    /// <summary>
    /// ドロップしたEPを記録する
    /// </summary>
    /// <param name="EP">EnhancementPoint</param>
    public void SaveDropEP(int EP)
    {
        m_EP += EP;
        GameManager.Instance.SaveDataManager.SaveData.saveData.EnhancementPoint += m_EP;
        GameManager.Instance.SaveDataManager.Save();
    }

    /// <summary>
    /// 獲得したEPの量を描画する
    /// </summary>
    public void DrawText()
    {
        Text.GetComponent<TextMeshProUGUI>().text = $"獲得EP：<sprite=1>{m_EP}";
    }
}
