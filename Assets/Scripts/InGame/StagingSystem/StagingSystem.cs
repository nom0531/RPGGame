using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using TMPro;

public class StagingSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g")]
    private GameObject CommandCanvas;
    [SerializeField]
    private GameObject CommandImage,CommandText,EnemyHPBarCanvas, EnemyHPBarObject;
    [SerializeField,Header("���z�J����")]
    private CinemachineVirtualCameraBase[] Vcam_Stanging;
    [SerializeField, Tooltip("�J�����ɉf���^�[�Q�b�g")]
    private GameObject TargetGroupObject;
    [SerializeField, Header("��{�G�t�F�N�g")]
    private GameObject[] InstantiateEffect;
    [SerializeField, Header("HP�o�[�̃I�u�W�F�N�g")]
    private GameObject EnemyHPBar;
    [SerializeField, Tooltip("�G�t�F�N�g�̐����I���̑҂�����(�b)")]
    private float EndWaitTime = 1.5f;
    [SerializeField, Header("�_���[�W�ʂ�\������e�L�X�g")]
    private GameObject Damagetext;

    private const float TARGET_WEIGHT = 1.0f;
    private const float TARGET_RADIUS = 1.0f;
    private const float EFFECT_SCALE = 20.0f;                   // �G�t�F�N�g�̃X�P�[��
    private const int VCAM_PRIORITY = 20;                       // �J�����̗D��x

    private BattleSystem m_battleSystem;
    private CinemachineTargetGroup m_cinemachineTargetGroup;    // �^�[�Q�b�g�O���[�v
    private SkillDataBase m_skillData;
    private TurnManager m_turnManager;
    private UIAnimation m_commandAnimaton;
    private UIAnimation m_enemyHPBarAnimation;
    private SE m_se;
    private bool m_isPlayEffect = false;                        // true�Ȃ�Đ����Bfalse�Ȃ�Đ����Ă��Ȃ�
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
    /// �^�[�Q�b�g��ݒ肷��
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
    /// �^�[�Q�b�g��ǉ�����
    /// </summary>
    public void AddTarget(GameObject target)
    {
        m_cinemachineTargetGroup.AddMember(
            target.transform, TARGET_WEIGHT, TARGET_RADIUS);
    }

    /// <summary>
    /// �D��x��������
    /// </summary>
    public void ResetPriority()
    {
        for(int i = 0; i < Vcam_Stanging.Length; i++)
        {
            Vcam_Stanging[i].Priority = 0;
        }
    }

    /// <summary>
    /// ���o�p�̃J�����ɐ؂�ւ���
    /// </summary>
    public void ChangeVcam(int number)
    {
        // �D��x��ݒ肷��
        Vcam_Stanging[number].Priority = VCAM_PRIORITY;
    }

    /// <summary>
    /// �R�}���h�̖��O��ݒ肷��
    /// </summary>
    /// <param name="actionType">�s���p�^�[��</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
    private void SetCommandName(ActionType actionType, int skillNumber)
    {
        switch (actionType)
        {
            case ActionType.enAttack:
                CommandImage.SetActive(true);
                CommandText.SetActive(false);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"�U��";
                break;
            case ActionType.enSkillAttack:
                CommandImage.SetActive(true);
                CommandText.SetActive(false);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{m_skillData.skillDataList[skillNumber].SkillName}";
                break;
            case ActionType.enGuard:
                CommandImage.SetActive(true);
                CommandText.SetActive(false);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"�h��";
                break;
            case ActionType.enEscape:
                CommandImage.SetActive(true);
                CommandText.SetActive(false);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"����";
                break;
            case ActionType.enWeak:
                CommandImage.gameObject.SetActive(false);
                SetAddInfoCommandText($"�_�E�����Ă���c");
                break;
            case ActionType.enNull:
                CommandImage.gameObject.SetActive(false);
                SetAddInfoCommandText($"�l�q�����Ă���c");
                break;
        }
        m_commandAnimaton.ButtonDown_Active();
    }

    /// <summary>
    /// �U�����o���Đ�����
    /// </summary>
    /// <param name="actionType">�s���p�^�[��</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
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
        // �ݒ�����Z�b�g
        m_commandAnimaton.ButtonDown_NotActive();
        m_enemyHPBarAnimation.ButtonDown_NotActive();
        m_isPlayEffect = false;
    }

    /// <summary>
    /// �U�����o���쐬
    /// </summary>
    /// <param name="actionType">�s���p�^�[��</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
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
    /// �G�t�F�N�g���쐬����֐�
    /// </summary>
    private void CreateEffect(ActionType actionType, int skillNumber)
    {
        // ��������G�t�F�N�g��ݒ�
        var effect = InstantiateEffect[(int)actionType];
        var scale = EFFECT_SCALE;
        m_se.Number = SENumber.enAttack;
        if (actionType == ActionType.enSkillAttack)
        {
            // �X�L���ł̍U���Ȃ�A�f�[�^����Q�Ƃ���
            effect = m_skillData.skillDataList[skillNumber].SkillEffect;
            scale = m_skillData.skillDataList[skillNumber].EffectScale;
            SetSE(skillNumber);
        }
        // �U�����������Ă��Ȃ��Ȃ�
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
    /// �_���[�W�e�L�X�g���쐬����֐�
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
    /// �_���[�W�ʂ�`��
    /// </summary>
    private void DrawDamage(Transform transform)
    {
        // �_���[�W�e�L�X�g�𐶐�
        var damageCanvas = Instantiate(Damagetext, transform);
        Debug.Log("�������̍��W�F" + transform.position);

        // Canvas�̍��W���r���[�|�[�g���W�ɕϊ�
        var viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
        damageCanvas.transform.position = viewportPoint;
        Debug.Log("CanvasPosition�F" + damageCanvas.transform.position);

        // �_���[�W�ʂ�ݒ�
        var drawDamage = damageCanvas.GetComponent<DrawDamage>();
        drawDamage.Damage = m_damage.ToString();

        // �U�����������Ă��Ȃ��Ȃ�
        if (m_battleSystem.HitFlag == false)
        {
            drawDamage.Damage = "miss";
        }
        drawDamage.Draw();
    }

    /// <summary>
    /// SE��ݒ肷��
    /// </summary>
    private void SetSE(int skillNumber)
    {
        // �X�L���̌��ʕ�
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
        // �����ϐ���
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
    /// �ǉ�����ݒ肷��
    /// </summary>
    /// <param name="text">�\������e�L�X�g</param>
    public void SetAddInfoCommandText(string text)
    {
        CommandText.GetComponent<TextMeshProUGUI>().text = text;
        CommandText.SetActive(true);
    }
}
