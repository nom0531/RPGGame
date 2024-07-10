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
    private GameObject CommandCanvas;
    [SerializeField]
    private GameObject CommandImage,CommandText,EnemyHPBarCanvas, EnemyHPBarObject;
    [SerializeField,Header("仮想カメラ")]
    private CinemachineVirtualCameraBase[] Vcam_Stanging;
    [SerializeField, Tooltip("カメラに映すターゲット")]
    private GameObject TargetGroupObject;
    [SerializeField, Header("基本エフェクト")]
    private GameObject[] InstantiateEffect;
    [SerializeField, Header("HPバーのオブジェクト")]
    private GameObject EnemyHPBar;
    [SerializeField, Tooltip("エフェクトの生成終了の待ち時間(秒)")]
    private float EndWaitTime = 1.5f;
    [SerializeField, Header("ダメージ量を表示するテキスト")]
    private GameObject Damagetext;

    private const float TARGET_WEIGHT = 1.0f;
    private const float TARGET_RADIUS = 1.0f;
    private const float EFFECT_SCALE = 20.0f;                   // エフェクトのスケール
    private const int VCAM_PRIORITY = 20;                       // カメラの優先度

    private BattleSystem m_battleSystem;
    private CinemachineTargetGroup m_cinemachineTargetGroup;    // ターゲットグループ
    private SkillDataBase m_skillData;
    private TurnManager m_turnManager;
    private UIAnimation m_commandAnimaton;
    private UIAnimation m_enemyHPBarAnimation;
    private SE m_se;
    private bool m_isPlayEffect = false;                        // trueなら再生中。falseなら再生していない
    private int m_damage = 0;

    public SkillDataBase SkillDataBase
    {
        get => m_skillData;
        set => m_skillData = value;
    }

    public int Damage
    {
        set => m_damage = value;
    }

    private void Awake()
    {
        m_cinemachineTargetGroup = TargetGroupObject.GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        m_commandAnimaton = CommandCanvas.GetComponent<UIAnimation>();
        m_commandAnimaton.Animator = CommandCanvas.GetComponent<Animator>();
        m_enemyHPBarAnimation = EnemyHPBarCanvas.GetComponent<UIAnimation>();
        m_enemyHPBarAnimation.Animator = EnemyHPBarCanvas.GetComponent<Animator>();
        m_se = GetComponent<SE>();
        m_battleSystem = GetComponent<BattleSystem>();
        m_turnManager = GetComponent<TurnManager>();
        CommandCanvas.SetActive(false);
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
                CommandImage.SetActive(true);
                CommandText.SetActive(false);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"攻撃";
                break;
            case ActionType.enSkillAttack:
                CommandImage.SetActive(true);
                CommandText.SetActive(false);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{m_skillData.skillDataList[skillNumber].SkillName}";
                break;
            case ActionType.enGuard:
                CommandImage.SetActive(true);
                CommandText.SetActive(false);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"防御";
                break;
            case ActionType.enEscape:
                CommandImage.SetActive(true);
                CommandText.SetActive(false);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"逃走";
                break;
            case ActionType.enWeak:
                CommandImage.gameObject.SetActive(false);
                SetAddInfoCommandText($"ダウンしている…");
                break;
            case ActionType.enNull:
                CommandImage.gameObject.SetActive(false);
                SetAddInfoCommandText($"様子を見ている…");
                break;
        }
        m_commandAnimaton.ButtonDown_Active();
    }

    /// <summary>
    /// 攻撃演出を再生する
    /// </summary>
    /// <param name="actionType">行動パターン</param>
    /// <param name="skillNumber">スキルの番号</param>
    async public void PlayStaging(ActionType actionType, int skillNumber)
    {
        if (actionType == ActionType.enGuard)
        {
            return;
        }
        SetCommandName(actionType, skillNumber);
        if(actionType != ActionType.enNull && m_turnManager.TurnStatus == TurnStatus.enPlayer)
        {
            m_enemyHPBarAnimation.ButtonDown_Active();
        }
        CreateStaging(actionType, skillNumber);
        await UniTask.Delay(TimeSpan.FromSeconds(EndWaitTime));
        // 設定をリセット
        m_commandAnimaton.ButtonDown_NotActive();
        m_enemyHPBarAnimation.ButtonDown_NotActive();
        m_isPlayEffect = false;
    }

    /// <summary>
    /// 攻撃演出を作成
    /// </summary>
    /// <param name="actionType">行動パターン</param>
    /// <param name="skillNumber">スキルの番号</param>
    private void CreateStaging(ActionType actionType, int skillNumber)
    {
        if (m_isPlayEffect == true)
        {
            return;
        }
        m_isPlayEffect = true;
        CreateEffect(actionType,skillNumber);
        CreateDamageText(actionType);
        if (actionType != ActionType.enNull)
        {
            m_se.PlaySE();
        }
    }

    /// <summary>
    /// エフェクトを作成する関数
    /// </summary>
    private void CreateEffect(ActionType actionType, int skillNumber)
    {
        // 生成するエフェクトを設定
        var effect = InstantiateEffect[(int)actionType];
        var scale = EFFECT_SCALE;
        m_se.Number = SENumber.enAttack;
        if (actionType == ActionType.enSkillAttack)
        {
            // スキルでの攻撃なら、データから参照する
            effect = m_skillData.skillDataList[skillNumber].SkillEffect;
            scale = m_skillData.skillDataList[skillNumber].EffectScale;
            SetSE(skillNumber);
        }
        // 攻撃が当たっていないなら
        if (m_battleSystem.HitFlag == false)
        {
            m_se.Number = SENumber.enMiss;
            effect = null;
        }
        if (effect != null)
        {
            effect.transform.localScale = new Vector3(scale, scale, scale);
            Instantiate(effect, TargetGroupObject.transform);
        }
    }

    /// <summary>
    /// ダメージテキストを作成する関数
    /// </summary>
    private void CreateDamageText(ActionType actionType)
    {
        if (actionType != ActionType.enAttack && actionType != ActionType.enSkillAttack)
        {
            return;
        }
        var childCount = m_cinemachineTargetGroup.m_Targets.Length;
        for (int i = 0; i < childCount; i++)
        {
            DrawDamage(m_cinemachineTargetGroup.m_Targets[i].target);
        }
    }

    /// <summary>
    /// ダメージ量を描画
    /// </summary>
    private void DrawDamage(Transform transform)
    {
        // ダメージテキストを生成
        var damageCanvas = Instantiate(Damagetext, transform);
        Debug.Log("生成時の座標：" + transform.position);

        // Canvasの座標をビューポート座標に変換
        var viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
        damageCanvas.transform.position = viewportPoint;
        Debug.Log("CanvasPosition：" + damageCanvas.transform.position);

        // ダメージ量を設定
        var drawDamage = damageCanvas.GetComponent<DrawDamage>();
        drawDamage.Damage = m_damage.ToString();

        // 攻撃が当たっていないなら
        if (m_battleSystem.HitFlag == false)
        {
            drawDamage.Damage = "miss";
        }
        drawDamage.Draw();
    }

    /// <summary>
    /// SEを設定する
    /// </summary>
    private void SetSE(int skillNumber)
    {
        // スキルの効果別
        switch (m_skillData.skillDataList[skillNumber].SkillType)
        {
            case SkillType.enBuff:
                m_se.Number = SENumber.enBuff;
                return;
            case SkillType.enDeBuff:
                m_se.Number = SENumber.enDebuff;
                return;
            case SkillType.enHeal:
            case SkillType.enResurrection:
                m_se.Number = SENumber.enHeal;
                return;
        }
        // 属性耐性別
        switch (m_battleSystem.ResistanceState)
        {
            case ResistanceState.enWeak:
                m_se.Number = SENumber.enWeak;
                return;
            case ResistanceState.enNormal:
                m_se.Number = SENumber.enMagicAttack;
                return;
            case ResistanceState.enResist:
                m_se.Number = SENumber.enRegister;
                return;
        }
    }

    /// <summary>
    /// 追加情報を設定する
    /// </summary>
    /// <param name="text">表示するテキスト</param>
    public void SetAddInfoCommandText(string text)
    {
        CommandText.GetComponent<TextMeshProUGUI>().text = text;
        CommandText.SetActive(true);
    }
}
