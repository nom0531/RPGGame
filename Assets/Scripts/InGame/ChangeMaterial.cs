using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField, Header("�}�e���A��")]
    private Material[] Materials;

    /// <summary>
    /// �}�e���A����ύX����
    /// </summary>
    /// <param name="number">�z��̓Y����</param>
    public Material Change(int number)
    {
        return Materials[number];
    }
}
