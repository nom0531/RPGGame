using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGround : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private LevelDataBase LevelData;

    private MeshRenderer m_meshRenderer;    // ���b�V�������_���[
    private Material m_material;            // �}�e���A��

    // Start is called before the first frame update
    private void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        //�}�e���A�����K�p����Ă�����ϐ��Ɋi�[
        if (m_meshRenderer.material != null)
        {
            m_material = m_meshRenderer.material;
        }
        SetTexture();
    }

    /// <summary>
    /// �e�N�X�`����ݒ肷��
    /// </summary>
    private void SetTexture()
    {
        var texture = LevelData.levelDataList[GameManager.Instance.LevelNumber].LocationTexture;
        // null�Ȃ���s���Ȃ�
        if(texture == null)
        {
            return;
        }
        // �e�N�X�`����ݒ�
        m_material.SetTexture("MainTexture",texture);
    }
}
