using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AddState
{
    enAttack,
    enWeak,
    enResist,
    enDamage,
    enGurad,
}

public class AllOutAttackGauge : MonoBehaviour
{
    [SerializeField, Header("データ")]
    private Image Gauge;
    [SerializeField]
    private Image Top;
    [SerializeField]
    private Color BaseColor,MulColor;
    [SerializeField, Header("基準値")]
    private float DefaultPoint = 25.0f;

    private const float MAX_POINT = 100.0f;
    private const float MIN_POINT = 0.0f;

    private AllOutAttackSystem m_allOutAttackSystem;
    private Image m_image;

    private float m_nowPoint = 0.0f;

    private void Start()
    {
        m_allOutAttackSystem = GameObject.FindGameObjectWithTag("BattleSystem").GetComponent<AllOutAttackSystem>();
        m_image = Gauge.GetComponent<Image>();
        SetAmount();
    }

    private void SetColor()
    {
        // 総攻撃ができるなら
        if (m_allOutAttackSystem.CanStartFlag == true)
        {
            Top.GetComponent<Image>().color = BaseColor;
            return;
        }
        Top.GetComponent<Image>().color = MulColor;
    }

    /// <summary>
    /// ボタンを押せるようにする
    /// </summary>
    private void ButtonActive()
    {
        GetComponent<Animator>().SetTrigger("Active");
    }

    /// <summary>
    /// ポイントを加算する
    /// </summary>
    /// <param name="point">加算量</param>
    /// <param name="addState">計算方法</param>
    public void AddPoint(AddState addState)
    {
        var addValue = 0.0f;
        switch (addState)
        {
            case AddState.enAttack:
                addValue = DefaultPoint;
                break;
            case AddState.enWeak:
                addValue = DefaultPoint * 4.0f;
                break;
            case AddState.enResist:
                addValue = DefaultPoint / 4.0f;
                break;
            case AddState.enGurad:
                addValue = DefaultPoint / 2.0f;
                break;
        }
        m_nowPoint += addValue;
        if (m_nowPoint >= MAX_POINT)
        {
            m_nowPoint = MAX_POINT;
        }
        SetAmount();
    }

    /// <summary>
    /// ポイントをリセットする
    /// </summary>
    public void ResetPoint()
    {
        m_nowPoint = 0.0f;
        SetAmount();
    }

    /// <summary>
    /// 割合を設定する
    /// </summary>
    private void SetAmount()
    {
        m_image.fillAmount = m_nowPoint / MAX_POINT;
        if(m_image.fillAmount == 1.0f)
        {
            m_allOutAttackSystem.CanStartFlag = true;
            ButtonActive();
        }
        SetColor();
    }
}
