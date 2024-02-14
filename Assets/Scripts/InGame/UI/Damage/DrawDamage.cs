using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawDamage : MonoBehaviour
{
    [SerializeField, Header("�ڍאݒ�"), Tooltip("������܂ł̎���")]
    private float m_deleteTime = 1.0f;
    [SerializeField, Tooltip("������܂ł̎��Ԃɂǂꂾ����ɏオ�邩")]
    private float m_moveRange = 0.05f;
    [SerializeField, Tooltip("�����鎞�̓����x"),Range(0.0f,1.0f)]
    private float m_endAlpha = 0.2f;

    private float m_timeCount;
    private Text m_nowText;

    // Start is called before the first frame update
    private void Start()
    {
        m_timeCount = 0.0f;
        Destroy(
            this.gameObject,    // �j�󂷂�I�u�W�F�N�g
            m_deleteTime        // �I�u�W�F�N�g��j�󂷂�܂ł̎���
            );
        m_nowText = GetComponent<Text>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // ���W���ړ�������
        m_timeCount += Time.deltaTime;
        this.gameObject.transform.localPosition += 
            new Vector3(0, m_moveRange / m_deleteTime * Time.deltaTime, 0);

        // �����x�̌v�Z����
        float alpha = 1.0f - (1.0f - m_endAlpha) * (m_timeCount / m_deleteTime);

        if (alpha <= 0.0f)
        {
            alpha = 0.0f;
            m_nowText.color = new Color(m_nowText.color.r, m_nowText.color.g, m_nowText.color.b, alpha);
        }
    }
}
