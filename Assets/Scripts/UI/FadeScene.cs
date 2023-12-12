using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class FadeScene : MonoBehaviour
{
    [SerializeField, Header("�ڍ׃p�����[�^"), Tooltip("�t�F�[�h�̑��x")]
    private float FadeSpeed = 1.0f;

    // �摜�̕s�����x
    private float m_alpha = 0.0f;
    // true�Ȃ疾�邭�Afalse�Ȃ�Â��Ȃ�
    private bool m_fadeMode = false;
    // �J�ڐ�̃V�[����
    private string m_sceneName = "";
    // ���g���g�p����Image
    private Image m_image;

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

        // ���g�̎q�I�u�W�F�N�g��Image��ۑ�
        m_image = transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        // �t�F�[�h����
        if (m_fadeMode == false)
        {
            // ��ʂ��Â�����
            m_alpha += FadeSpeed * Time.deltaTime;

            // ���S�ɈÂ��Ȃ����̂ŃV�[����ύX
            if (m_alpha >= 1.0f)
            {
                // �V�[�������[�h���āA���邭����
                SceneManager.LoadScene(m_sceneName);

                m_fadeMode = true;
                // ���g�̓V�[�����ׂ��ł��폜����Ȃ��悤�ɂ���
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            // ��ʂ𖾂邭����
            m_alpha -= FadeSpeed * Time.deltaTime;

            // ���S�ɖ��邭�Ȃ����̂Ŏ��g���폜����
            if (m_alpha <= 0.0f)
            {
                Destroy(gameObject);
            }
        }

        // �s�����x��ݒ肷��
        Color nowColor = m_image.color;
        nowColor.a = m_alpha;
        m_image.color = nowColor;
    }
}
