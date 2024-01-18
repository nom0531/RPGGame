using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using System;

public class StangingSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private SkillDataBase SkillData;
    [SerializeField,Header("仮想カメラ")]
    private CinemachineVirtualCameraBase[] Vcam_Stanging;
    [SerializeField, Tooltip("カメラに映すターゲット")]
    private GameObject TargetGroupObject;
    [SerializeField, Header("基本エフェクト")]
    private GameObject[] InstantiateEffect;
    [SerializeField, Tooltip("エフェクトの生成終了の待ち時間(秒)")]
    private float EndWaitTime = 0.5f;

    private DrawDamageText m_drawDamageText;                    // ダメージ量
    private CinemachineTargetGroup m_cinemachineTargetGroup;    // ターゲットグループ
    private bool m_isPlayEffect = false;                        // trueなら再生中。falseなら再生していない

    private const float TARGET_WEIGHT = 1.0f;
    private const float TARGET_RADIUS = 1.0f;
    private const float EFFECT_SCALE = 200.0f;                  // エフェクトのスケール
    private const int VCAM_PRIORITY = 50;                       // カメラの優先度

    public bool PlayEffectFlag
    {
        get => m_isPlayEffect;
    }

    private void Awake()
    {
        m_cinemachineTargetGroup = TargetGroupObject.GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        m_drawDamageText = GameObject.FindGameObjectWithTag
            ("UICanvas").GetComponent<DrawDamageText>();

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
                },
        };
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

        // 生成するエフェクトを設定
        var effect = InstantiateEffect[(int)actionType];
        var scale = EFFECT_SCALE;
        // スキルでの攻撃なら、データから参照する
        if (actionType == ActionType.enSkillAttack)
        {
            effect = SkillData.skillDataList[skillNumber].SkillEffect;
            scale = SkillData.skillDataList[skillNumber].EffectScale;
        }
        // 生成エフェクトがNullでない場合
        if(effect != null)
        {
            // サイズを調整
            effect.transform.localScale = new Vector3(scale, scale, scale);
            Instantiate(effect, TargetGroupObject.transform);
        }
        // 再生を開始する
        m_isPlayEffect = true;
        await UniTask.Delay(TimeSpan.FromSeconds(EndWaitTime));
        // 再生を終了する
        m_isPlayEffect = false;
    }

    /// <summary>
    /// 値を表示する
    /// </summary>
    /// <param name="isHit">ヒットしているかどうか</param>
    /// <param name="value">値</param>
    public void DrawValue(bool isHit, int value, GameObject gameObject)
    {
        if(isHit == false)
        {
            m_drawDamageText.ViewDamage("miss", gameObject);
            return;
        }
        m_drawDamageText.ViewDamage(value.ToString(), gameObject);
    }

    /// <summary>
    /// 再行動時の演出
    /// </summary>
    /// <param name="isOneMore">再行動できるかどうか</param>
    public void OneMore(bool isOneMore)
    {
        if(isOneMore == false)
        {
            return;
        }
        // 文字を表示
        Debug.Log("再行動");
        // 再度行動させる
    }
}
