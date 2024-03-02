using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGround : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    private LevelDataBase LevelData;

    private MeshRenderer m_meshRenderer;    // メッシュレンダラー
    private Material m_material;            // マテリアル

    // Start is called before the first frame update
    private void Start()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        //マテリアルが適用されていたら変数に格納
        if (m_meshRenderer.material != null)
        {
            m_material = m_meshRenderer.material;
        }
        SetTexture();
    }

    /// <summary>
    /// テクスチャを設定する
    /// </summary>
    private void SetTexture()
    {
        var texture = LevelData.levelDataList[GameManager.Instance.LevelNumber].LocationTexture;
        // nullなら実行しない
        if(texture == null)
        {
            return;
        }
        // テクスチャを設定
        m_material.SetTexture("MainTexture",texture);
    }
}
