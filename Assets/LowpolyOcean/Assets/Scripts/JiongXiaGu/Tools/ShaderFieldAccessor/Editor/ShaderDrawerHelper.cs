using UnityEditor;

namespace JiongXiaGu.ShaderTools
{
    internal class ShaderDrawerHelper : ShaderGUI
    {
        public static MaterialProperty PublicFindProperty(string name, MaterialProperty[] properties, bool propertyIsMandatory = false)
        {
            return FindProperty(name, properties, propertyIsMandatory);
        }
    }
}
