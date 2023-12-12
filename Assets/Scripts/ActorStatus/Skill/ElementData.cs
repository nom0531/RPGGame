using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 属性の定義
/// </summary>
public enum ElementType
{
    enFire,     // 火属性
    enIce,      // 氷属性
    enWind,     // 風属性
    enThunder,  // 雷属性
    enLight,    // 光属性
    enDark,     // 闇属性
    enNone,     // 無属性
    enNum,      // 全属性数
}

/// <summary>
/// 属性耐性の定義
/// </summary>
public enum ElementResistance
{ 
    enResist,   // 耐性
    enWeak,     // 弱点
    enNormal    // 通常
}
