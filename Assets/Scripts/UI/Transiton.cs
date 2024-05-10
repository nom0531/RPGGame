using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transiton : MonoBehaviour
{
    [SerializeField, Header("��ʑJ�ڗp�f�[�^"),Tooltip("�}�e���A��")]
    private Material PostEffectMaterial;
    [SerializeField, Tooltip("��ʑJ�ڂ̎���")]
    private float TransitionTime = 1.0f;

    private string m_sceneName = "";        // �J�ڐ�̃V�[����
    private bool m_fadeMode = false;        // true�Ȃ疾�邭�Afalse�Ȃ�Â��Ȃ�
    private float m_progress = 1.0f;

    // �V�F�[�_�[���Œ�`����Ă������̃v���p�e�B��int�^�ɕϊ�
    private readonly int m_progressID = Shader.PropertyToID("_Progress");

    private void Start()
    {
        // ���g��RenderCamera�Ƀ��C���J������ݒ肷��
        if (GetComponent<Canvas>().worldCamera == null)
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
        // �l��������
        PostEffectMaterial.SetFloat(m_progressID, 1.0f);
    }

    /// <summary>
    /// �t�F�[�h���J�n���鏈��
    /// </summary>
    public void FadeStart(string sceneName)
    {
        if (sceneName == "")
        {
            // �������͂���Ă��Ȃ��Ȃ猻�݂̃V�[�������擾����
            m_sceneName = SceneManager.GetActiveScene().name;
        }
        else
        {
            // �J�ڐ�̃V�[������ۑ�
            m_sceneName = sceneName;
        }
    }

    private void Update()
    {
        StartTransition();
    }

    /// <summary>
    /// �g�����W�V��������
    /// </summary>
    /// <returns></returns>
    private void StartTransition()
    {
        // ���g��RenderCamera�Ƀ��C���J������ݒ肷��
        if (GetComponent<Canvas>().worldCamera == null)
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
        if (m_fadeMode == false)
        {
            // ��ʂ��Â�����
            m_progress -= TransitionTime * Time.deltaTime;

            // ���S�ɈÂ��Ȃ����̂ŃV�[����ύX����
            if (m_progress <= 0.0f)
            {
                PostEffectMaterial.SetFloat(m_progressID, 0.0f);    // �␳
                SceneManager.LoadScene(m_sceneName);
                // ���邭���郂�[�h�ɕύX
                m_fadeMode = true;
            }
        }
        else
        {
            // ��ʂ𖾂邭����
            m_progress += TransitionTime * Time.deltaTime;

            // ���S�ɖ��邭�Ȃ����̂Ŏ��g���폜����
            if (m_progress >= 1.0f)
            {
                PostEffectMaterial.SetFloat(m_progressID, 1.0f);    // �␳
                Destroy(gameObject);
            }
        }
        // �V�F�[�_�[��_Progress�ɒl��ݒ�
        PostEffectMaterial.SetFloat(m_progressID, m_progress);
    }
}
