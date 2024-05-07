using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    [SerializeField, Header("�V�[���p�f�[�^"),Tooltip("�V�[����؂�ւ���܂ł̕b��")]
    private float Timer = 5.0f;
    [SerializeField, Tooltip("�ύX��̃V�[����")]
    private string ChangeSceneName;
    [SerializeField, Header("�A�j���[�V�����f�[�^"), Tooltip("�����܂ł̎���")]
    private float InstantiateTimer = 2.0f;
    [SerializeField]
    private GameObject AnimationCanvas;

    private SceneButton m_sceneButton;
    private bool m_instantiate = false;     // ���������Ȃ�ture
    private float m_timer = 0.0f;

    // Start is called before the first frame update
    private void Start()
    {
        m_sceneButton = GetComponent<SceneButton>();
        AnimationCanvas.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        SceneChange();
    }

    /// <summary>
    /// �V�[����؂�ւ���
    /// </summary>
    private void SceneChange()
    {
        m_timer += Time.deltaTime;

        if(m_timer >= InstantiateTimer && m_instantiate == false)
        {
            AnimationCanvas.SetActive(true);
            m_instantiate = true;
        }
        if(m_timer >= Timer)
        {
            m_sceneButton.SceneChange(ChangeSceneName);
            GameManager.Instance.SceneNumber = SceneNumber.enTitle;
            m_timer = 0.0f;
        }
    }
}
