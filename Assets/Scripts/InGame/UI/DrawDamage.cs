using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class DrawDamage : MonoBehaviour
{
    private CinemachineBrain m_cinemachineBrain;
    private string m_damageText = "";
    private bool m_isChangeCamera = false;
    

    public string Damage
    {
        set => m_damageText = value;
    }

    private void Start()
    {
        // �J�����؂�ւ��C�x���g�o�^
        m_cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        m_cinemachineBrain.m_CameraActivatedEvent.AddListener(OnChangeCamera);
    }

    // �o�[�`�����J�������؂�ւ�����Ƃ��ɌĂ΂��
    private void OnChangeCamera(ICinemachineCamera incomingVcam, ICinemachineCamera outgoingVcam)
    {
        m_isChangeCamera = true;
    }

    private IEnumerator StopDrawCoroutine()
    {
        yield return new WaitUntil(() => m_isChangeCamera == true);
    }

    /// <summary>
    /// �_���[�W��`�悷��
    /// </summary>
    public void Draw()
    {
        StartCoroutine(StopDrawCoroutine());
        // �e�L�X�g�̐ݒ�
        var text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = m_damageText;
    }

    /// <summary>
    /// ���g���폜����
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
