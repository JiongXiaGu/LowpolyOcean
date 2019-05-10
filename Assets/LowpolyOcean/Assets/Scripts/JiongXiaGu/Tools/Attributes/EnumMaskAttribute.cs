using System;
using UnityEngine;

namespace JiongXiaGu
{

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumMaskAttribute : PropertyAttribute
    {
    }
}
