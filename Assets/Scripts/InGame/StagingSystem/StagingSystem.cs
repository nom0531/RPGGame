using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using TMPro;

public class StagingSystem : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト")]
    private GameObject CommandImage;
    [SerializeField]
    private GameObject CommandText;
    [SerializeField,Header("仮想カメラ")]
    private CinemachineVirtualCameraBase[] Vcam_Stanging;
    [SerializeField, Tooltip("カメラに映すターゲット")]
    private GameObject TargetGroupObject;
    [SerializeField, Header("基本エフェクト")]
    private GameObject[] InstantiateEffect;
    [SerializeField, Tooltip("エフェクトの生成終了の待ち時間(秒)")]
    private float EndWaitTime = 1.5f;

    private DrawDamageText m_drawDamageText;                    // ダメージ量
    private CinemachineTargetGroup m_cinemachineTargetGroup;    // ターゲットグループ
    private SkillDataBase m_skillData;
    private bool m_isPlayEffect = false;                        // trueなら再生中。falseなら再生していない
    private const float TARGET_WEIGHT = 1.0f;
    private const float TARGET_RADIUS = 1.0f;
    private const float EFFECT_SCALE = 20.0f;                   // エフェクトのスケール
    private const int VCAM_PRIORITY = 20;                       // カメラの優先度

    public SkillDataBase SkillDataBase
    {
        get => m_skillData;
        set => m_skillData = value;
    }

    private void Awake()
    {
        m_cinemachineTargetGroup = TargetGroupObject.GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        m_drawDamageText = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<DrawDamageText>();
        CommandImage.SetActive(false);
        CommandText.SetActive(false);
        ResetPriority();
    }

    /// <summary>
    /// ターゲットを設定する
    /// </summary>
    public void SetCameraTarget(GameObject target)
    {
        m_cinemachineTargetGroup.m_Targets = new CinemachineTargetGroup.Target[]
        {
                new CinemachineTargetGroup.Target
                {
                    target = target.transform,
                    weight = TARGET_WEIGHT,
                    radius = TARGET_RADIUS,
                }
        };
    }

    /// <summary>
    /// ターゲットを追加する
    /// </summary>
    public void AddTarget(GameObject target)
    {
        m_cinemachineTargetGroup.AddMember(
            target.transform, TARGET_WEIGHT, TARGET_RADIUS);
    }

    /// <summary>
    /// 優先度を初期化
    /// </summary>
    public void ResetPriority()
    {
        for(int i = 0; i < Vcam_Stanging.Length; i++)
        {
            Vcam_Stanging[i].Priority = 0;
        }
    }

    /// <summary>
    /// 演出用のカメラに切り替える
    /// </summary>
    public void ChangeVcam(int number)
    {
        // 優先度を設定する
        Vcam_Stanging[number].Priority = VCAM_PRIORITY;
    }

    /// <summary>
    /// コマンドの名前を設定する
    /// </summary>
    /// <param name="actionType">行動パターン</param>
    /// <param name="skillNumber">スキルの番号</param>
    private void SetCommandName(ActionType actionType, int skillNumber)
    {
        switch (actionType)
        {
            case ActionType.enAttack:
                CommandImage.gameObject.SetActive(true);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"攻撃";
                break;
            case ActionType.enSkillAttack:
                CommandImage.gameObject.SetActive(true);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{m_skillData.skillDataList[skillNumber].SkillName}";
                break;
            case ActionType.enGuard:
                CommandImage.gameObject.SetActive(true);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"防御";
                break;
            case ActionType.enEscape:
                CommandImage.gameObject.SetActive(true);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"逃走";
                break;
            case ActionType.enNull:
                CommandImage.gameObject.SetActive(false);
                SetAddInfoCommandText($"様子を見ている…");
                break;
        }
    }

    /// <summary>
    /// エフェクトを再生する
    /// </summary>
    /// <param name="actionType">行動パターン</param>
    /// <param name="skillNumber">スキルの番号</param>
    async public void PlayEffect(ActionType actionType, int skillNumber)
    {
        if(m_isPlayEffect == true)
        {
            return;
        }
        if(actionType == ActionType.enGuard)
        {
            return;
        }

        SetCommandName(actionType, skillNumber);
        // 生成するエフェクトを設定
        var effect = InstantiateEffect[(int)actionType];
        var scale = EFFECT_SCALE;
        // スキルでの攻撃なら、データから参照する
        if (actionType == ActionType.enSkillAttack)
        {
            effect = m_skillData.skillDataList[skillNumber].SkillEffect;
            scale = m_skillData.skillDataList[skillNumber].EffectScale;
        }
        // 生成エフェクトがNullでない場合
        if(effect != null)
        {
            // サイズを調整
            effect.transform.localScale = new Vector3(scale, scale, scale);
            Instantiate(effect, TargetGroupObject.transform);
        }
        m_isPlayEffect = true;
        await UniTask.Delay(TimeSpan.FromSeconds(EndWaitTime));
        CommandImage.SetActive(false);                        // テキストを非表示
        m_isPlayEffect = false;                               // 再生を終了する
    }

    /// <summary>
    /// 追加情報を設定する
    /// </summary>
    /// <param name="text">表示するテキスト</param>
    async public void SetAddInfoCommandText(string text)
    {
        CommandText.GetComponent<TextMeshProUGUI>().text = text;
        CommandText.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(EndWaitTime));
        CommandText.SetActive(false);
    }

    /// <summary>
    /// 値を表示する
    /// </summary>
    public void DrawValue(List<TextData> textDataList)
    {
        for (int i = 0; i< textDataList.Count; i++)
        {
            if (textDataList[i].isHit == false)
            {
                m_drawDamageText.ViewDamage("miss", textDataList[i].gameObject);
                return;
            }
            m_drawDamageText.ViewDamage(textDataList[i].value.ToString(), textDataList[i].gameObject);
        }
    }
}
