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
    private GameObject CommandImage;
    [SerializeField]
    private GameObject CommandText;
    [SerializeField,Header("���z�J����")]
    private CinemachineVirtualCameraBase[] Vcam_Stanging;
    [SerializeField, Tooltip("�J�����ɉf���^�[�Q�b�g")]
    private GameObject TargetGroupObject;
    [SerializeField, Header("��{�G�t�F�N�g")]
    private GameObject[] InstantiateEffect;
    [SerializeField, Tooltip("�G�t�F�N�g�̐����I���̑҂�����(�b)")]
    private float EndWaitTime = 1.5f;

    private DrawDamageText m_drawDamageText;                    // �_���[�W��
    private CinemachineTargetGroup m_cinemachineTargetGroup;    // �^�[�Q�b�g�O���[�v
    private SkillDataBase m_skillData;
    private bool m_isPlayEffect = false;                        // true�Ȃ�Đ����Bfalse�Ȃ�Đ����Ă��Ȃ�
    private const float TARGET_WEIGHT = 1.0f;
    private const float TARGET_RADIUS = 1.0f;
    private const float EFFECT_SCALE = 20.0f;                   // �G�t�F�N�g�̃X�P�[��
    private const int VCAM_PRIORITY = 20;                       // �J�����̗D��x

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
                CommandImage.gameObject.SetActive(true);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"�U��";
                break;
            case ActionType.enSkillAttack:
                CommandImage.gameObject.SetActive(true);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{m_skillData.skillDataList[skillNumber].SkillName}";
                break;
            case ActionType.enGuard:
                CommandImage.gameObject.SetActive(true);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"�h��";
                break;
            case ActionType.enEscape:
                CommandImage.gameObject.SetActive(true);
                CommandImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"����";
                break;
            case ActionType.enNull:
                CommandImage.gameObject.SetActive(false);
                SetAddInfoCommandText($"�l�q�����Ă���c");
                break;
        }
    }

    /// <summary>
    /// �G�t�F�N�g���Đ�����
    /// </summary>
    /// <param name="actionType">�s���p�^�[��</param>
    /// <param name="skillNumber">�X�L���̔ԍ�</param>
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
        // ��������G�t�F�N�g��ݒ�
        var effect = InstantiateEffect[(int)actionType];
        var scale = EFFECT_SCALE;
        // �X�L���ł̍U���Ȃ�A�f�[�^����Q�Ƃ���
        if (actionType == ActionType.enSkillAttack)
        {
            effect = m_skillData.skillDataList[skillNumber].SkillEffect;
            scale = m_skillData.skillDataList[skillNumber].EffectScale;
        }
        // �����G�t�F�N�g��Null�łȂ��ꍇ
        if(effect != null)
        {
            // �T�C�Y�𒲐�
            effect.transform.localScale = new Vector3(scale, scale, scale);
            Instantiate(effect, TargetGroupObject.transform);
        }
        m_isPlayEffect = true;
        await UniTask.Delay(TimeSpan.FromSeconds(EndWaitTime));
        CommandImage.SetActive(false);                        // �e�L�X�g���\��
        m_isPlayEffect = false;                               // �Đ����I������
    }

    /// <summary>
    /// �ǉ�����ݒ肷��
    /// </summary>
    /// <param name="text">�\������e�L�X�g</param>
    async public void SetAddInfoCommandText(string text)
    {
        CommandText.GetComponent<TextMeshProUGUI>().text = text;
        CommandText.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(EndWaitTime));
        CommandText.SetActive(false);
    }

    /// <summary>
    /// �l��\������
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
