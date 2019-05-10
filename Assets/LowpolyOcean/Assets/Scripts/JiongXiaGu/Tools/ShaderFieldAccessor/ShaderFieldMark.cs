using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{

    /// <summary>
    /// Mark member for autoDrawer
    /// </summary>
    public class ShaderFieldMark : ShaderFieldBase
    {
        public ShaderFieldMark(int mask, IReflectiveField fieldAccessor) : base(mask, fieldAccessor)
        {
        }

        public override ShaderFieldFormat Format => ShaderFieldFormat.Mark;

        public override object GetGlobalValues(object dest)
        {
            return dest;
        }

        public override void SetGlobalValues(object source)
        {
        }

        public override void Copy(Material source, Material dest, ICollection<string> enabledKeywords)
        {
        }

        public override object Copy(Material source, object dest)
        {
            return dest;
        }

        public override void Copy(object source, Material dest, ICollection<string> enabledKeywords)
        {
        }

        public override object CopyWithoutKeywords(object source, object dest)
        {
            return dest;
        }

        public override void CopyWithoutKeywords(Material source, Material dest)
        {
        }

        public override object CopyWithoutKeywords(Material source, object dest)
        {
            return dest;
        }

        public override void CopyWithoutKeywords(object source, Material dest)
        {
        }

        public override object Lerp(object v0, object v1, object dest, float t)
        {
            return dest;
        }

        public override int GetEnabledKeywords()
        {
            return 0;
        }

        public override int GetEnabledKeywords(ICollection<string> keywords)
        {
            return 0;
        }

        public override int GetEnabledKeywords(object source)
        {
            return 0;
        }

        public override int GetEnabledKeywords(object source, ICollection<string> keywords)
        {
            return 0;
        }

        public override int GetEnabledKeywords(Material source)
        {
            return 0;
        }

        public override int GetEnabledKeywords(Material source, ICollection<string> keywords)
        {
            return 0;
        }
    }

}
