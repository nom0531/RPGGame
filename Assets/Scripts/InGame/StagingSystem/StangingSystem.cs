using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using System;

public class StangingSystem : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private SkillDataBase SkillData;
    [SerializeField,Header("���z�J����")]
    private CinemachineVirtualCameraBase[] Vcam_Stanging;
    [SerializeField, Tooltip("�J�����ɉf���^�[�Q�b�g")]
    private GameObject TargetGroupObject;
    [SerializeField, Header("��{�G�t�F�N�g")]
    private GameObject[] InstantiateEffect;
    [SerializeField, Tooltip("�G�t�F�N�g�̐����I���̑҂�����(�b)")]
    private float EndWaitTime = 0.5f;

    private DrawDamageText m_drawDamageText;                    // �_���[�W��
    private CinemachineTargetGroup m_cinemachineTargetGroup;    // �^�[�Q�b�g�O���[�v
    private bool m_isPlayEffect = false;                        // true�Ȃ�Đ����Bfalse�Ȃ�Đ����Ă��Ȃ�

    private const float TARGET_WEIGHT = 1.0f;
    private const float TARGET_RADIUS = 1.0f;
    private const float EFFECT_SCALE = 200.0f;                  // �G�t�F�N�g�̃X�P�[��
    private const int VCAM_PRIORITY = 50;                       // �J�����̗D��x

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
                },
        };
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

        // ��������G�t�F�N�g��ݒ�
        var effect = InstantiateEffect[(int)actionType];
        var scale = EFFECT_SCALE;
        // �X�L���ł̍U���Ȃ�A�f�[�^����Q�Ƃ���
        if (actionType == ActionType.enSkillAttack)
        {
            effect = SkillData.skillDataList[skillNumber].SkillEffect;
            scale = SkillData.skillDataList[skillNumber].EffectScale;
        }
        // �����G�t�F�N�g��Null�łȂ��ꍇ
        if(effect != null)
        {
            // �T�C�Y�𒲐�
            effect.transform.localScale = new Vector3(scale, scale, scale);
            Instantiate(effect, TargetGroupObject.transform);
        }
        // �Đ����J�n����
        m_isPlayEffect = true;
        await UniTask.Delay(TimeSpan.FromSeconds(EndWaitTime));
        // �Đ����I������
        m_isPlayEffect = false;
    }

    /// <summary>
    /// �l��\������
    /// </summary>
    /// <param name="isHit">�q�b�g���Ă��邩�ǂ���</param>
    /// <param name="value">�l</param>
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
    /// �čs�����̉��o
    /// </summary>
    /// <param name="isOneMore">�čs���ł��邩�ǂ���</param>
    public void OneMore(bool isOneMore)
    {
        if(isOneMore == false)
        {
            return;
        }
        // ������\��
        Debug.Log("�čs��");
        // �ēx�s��������
    }
}
