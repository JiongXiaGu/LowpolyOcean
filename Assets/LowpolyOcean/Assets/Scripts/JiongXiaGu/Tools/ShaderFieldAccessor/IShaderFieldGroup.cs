using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{


    public interface IShaderFieldGroup
    {
        int Mask { get; }
        IReflectiveField ReflectiveField { get; }
        List<IShaderFieldGroup> Children { get; }
        ShaderFieldFormat Format { get; }

        void ForEach(ShaderFieldAction action);
        void ForEach(object v0, ShaderFieldAction1 action);
        void ForEach(object v0, object v1, ShaderFieldAction2 action);
        void ForEach(object v0, object v1, object v2, ShaderFieldAction3 action);

        object GetGlobalValues(object dest);
        object GetGlobalValues(object dest, ShaderFieldAction filter);

        void SetGlobalValues(object source);
        void SetGlobalValues(object source, ShaderFieldAction filter);

        object Copy(object source, object dest);
        object Copy(object source, object dest, ShaderFieldAction filter);
        void Copy(Material source, Material dest, ICollection<string> enabledKeywords);
        void Copy(Material source, Material dest, ICollection<string> enabledKeywords, ShaderFieldAction filter);
        object Copy(Material source, object dest);
        object Copy(Material source, object dest, ShaderFieldAction filter);
        void Copy(object source, Material dest, ICollection<string> enabledKeywords);
        void Copy(object source, Material dest, ICollection<string> enabledKeywords, ShaderFieldAction filter);

        object CopyWithoutKeywords(object source, object dest);
        object CopyWithoutKeywords(object source, object dest, ShaderFieldAction filter);
        void CopyWithoutKeywords(Material source, Material dest);
        void CopyWithoutKeywords(Material source, Material dest, ShaderFieldAction filter);
        object CopyWithoutKeywords(Material source, object dest);
        object CopyWithoutKeywords(Material source, object dest, ShaderFieldAction filter);
        void CopyWithoutKeywords(object source, Material dest);
        void CopyWithoutKeywords(object source, Material dest, ShaderFieldAction filter);

        object Lerp(object v0, object v1, object dest, float t);
        object Lerp(object v0, object v1, object dest, float t, ShaderFieldAction filter);

        int GetEnabledKeywords();
        int GetEnabledKeywords(ShaderFieldAction filter);
        int GetEnabledKeywords(ICollection<string> keywords);
        int GetEnabledKeywords(ICollection<string> keywords, ShaderFieldAction filter);
        int GetEnabledKeywords(object source);
        int GetEnabledKeywords(object source, ShaderFieldAction filter);
        int GetEnabledKeywords(object source, ICollection<string> keywords);
        int GetEnabledKeywords(object source, ICollection<string> keywords, ShaderFieldAction filter);
        int GetEnabledKeywords(Material source);
        int GetEnabledKeywords(Material source, ShaderFieldAction filter);
        int GetEnabledKeywords(Material source, ICollection<string> keywords);
        int GetEnabledKeywords(Material source, ICollection<string> keywords, ShaderFieldAction filter);
    }
}
