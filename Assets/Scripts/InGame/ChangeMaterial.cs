using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField, Header("マテリアル")]
    private Material[] Materials;

    /// <summary>
    /// マテリアルを変更する
    /// </summary>
    /// <param name="number">配列の添え字</param>
    public Material Change(int number)
    {
        return Materials[number];
    }
}
