using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    [SerializeField, Header("シーン用データ"),Tooltip("シーンを切り替えるまでの秒数")]
    private float Timer = 5.0f;
    [SerializeField, Tooltip("変更先のシーン名")]
    private string ChangeSceneName;
    [SerializeField, Header("アニメーションデータ"), Tooltip("生成までの時間")]
    private float InstantiateTimer = 2.0f;
    [SerializeField]
    private GameObject AnimationCanvas;

    private SceneButton m_sceneButton;
    private bool m_instantiate = false;     // 生成したならture
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
    /// シーンを切り替える
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
