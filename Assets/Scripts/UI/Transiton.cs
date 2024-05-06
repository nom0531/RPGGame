using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionState
{
    enTransition_ReduceValue,       // �g�����W�V������ʂ𑽂�����
    enTransition_IncreaseVaue,      // Scene��ʂ𑽂�����
}

public class Transiton : MonoBehaviour
{
    [SerializeField, Header("��ʑJ�ڗp�f�[�^"),Tooltip("�|�X�g�G�t�F�N�g")]
    private Material PostEffectMaterial;
    [SerializeField, Tooltip("��ʑJ�ڂ̎���")]
    private float TransitionTime = 2.0f;

    // �V�F�[�_�[���Œ�`����Ă������̃v���p�e�B��int�^�ɕϊ�
    private readonly int m_progressID = Shader.PropertyToID("_Progress");

    // Start is called before the first frame update
    private void Start()
    {
        if(PostEffectMaterial != null)
        {
            StartCoroutine(StartTransition(TransitionState.enTransition_ReduceValue));
        }
    }

    public IEnumerator StartTransition(TransitionState transitionState)
    {
        float t = 0.0f;
        switch (transitionState)
        {
            case TransitionState.enTransition_ReduceValue:
                // t���ݒ肵�����ԓ��̊Ԏ��s����
                while (t < TransitionTime)
                {
                    var progress = t / TransitionTime;

                    // �V�F�[�_�[��_Progress�ɒl��ݒ�
                    PostEffectMaterial.SetFloat(m_progressID, progress);
                    yield return null;

                    t += Time.deltaTime;
                }
                PostEffectMaterial.SetFloat(m_progressID, 0.0f);    // �␳
                break;
            case TransitionState.enTransition_IncreaseVaue:
                // t���ݒ肵�����ԓ��̊Ԏ��s����
                while (t < TransitionTime)
                {
                    var progress = t / TransitionTime;

                    // �V�F�[�_�[��_Progress�ɒl��ݒ�
                    PostEffectMaterial.SetFloat(m_progressID, progress);
                    yield return null;

                    t += Time.deltaTime;
                }
                PostEffectMaterial.SetFloat(m_progressID, 1.0f);    // �␳
                break;
        }
    }
}
