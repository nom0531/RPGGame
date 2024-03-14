using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃI�u�W�F�N�g"), Tooltip("0��sound�A1��system")]
    private GameObject[] SystemObject;

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// ������
    /// </summary>
    private void Init()
    {
        SystemObject[0].SetActive(true);
        SystemObject[1].SetActive(false);
    }

    /// <summary>
    /// ���Z�b�g����
    /// </summary>
    public void Reset()
    {
        Init();
    }
}
