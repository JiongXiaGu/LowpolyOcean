using System;

namespace JiongXiaGu.ShaderTools
{

    /// <summary>
    /// Mark this class is a collection of shader parameters that will create a group
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ShaderFieldGroupAttribute : ShaderFieldBaseAttribute
    {
        public int Mask { get; set; }

        public ShaderFieldGroupAttribute(int mask)
        {
            Mask = mask;
        }

        /// <summary>
        /// Define mask that does not have to go inside when filtering
        /// </summary>
        public ShaderFieldGroupAttribute(object mask) : this((int)mask)
        {
        }

        public ShaderFieldGroupAttribute() : this(~0)
        {
        }
    }
}
