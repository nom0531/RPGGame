using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    private MeshRenderer m_meshRenderer;
    private Material m_marerial;

    private void Start()
    {
        if(m_meshRenderer.material != null)
        {
            m_marerial = m_meshRenderer.material;
        }
    }
}
