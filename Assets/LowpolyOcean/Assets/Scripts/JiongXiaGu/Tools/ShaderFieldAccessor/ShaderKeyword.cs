using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{

    public class ShaderKeyword : ShaderFieldBase
    {
        public string Keyword { get; }
        public string ShortName { get; private set; }
        public override ShaderFieldFormat Format => ShaderFieldFormat.Keyword;

        public ShaderKeyword(string name, IReflectiveField fieldAccessor, string shortName, int mask) : base(mask, fieldAccessor)
        {
            Keyword = name;
            ShortName = shortName;
        }

        public override string ToString()
        {
            return "Name:" + ReflectiveField.Name + " ,Mask:" + Mask + " ,Keyword:" + Keyword;
        }

        public override object GetGlobalValues(object dest)
        {
            var value = Shader.IsKeywordEnabled(Keyword);
            ReflectiveField.SetValue(dest, value);
            return dest;
        }

        public override void SetGlobalValues(object source)
        {
            bool value = (bool)ReflectiveField.GetValue(source);
            if (value)
                Shader.EnableKeyword(Keyword);
            else
                Shader.DisableKeyword(Keyword);
        }

        public override object Copy(object source, object dest)
        {
            var value = ReflectiveField.GetValue(source);
            ReflectiveField.SetValue(dest, value);
            return dest;
        }

        public override void Copy(Material source, Material dest, ICollection<string> enabledKeywords)
        {
            var value = source.IsKeywordEnabled(Keyword);
            if (value)
            {
                enabledKeywords.Add(Keyword);
            }
        }

        public override object Copy(Material source, object dest)
        {
            var value = source.IsKeywordEnabled(Keyword);
            ReflectiveField.SetValue(dest, value);
            return dest;
        }

        public override void Copy(object source, Material dest, ICollection<string> enabledKeywords)
        {
            bool value = (bool)ReflectiveField.GetValue(source);
            if (value)
            {
                enabledKeywords.Add(Keyword);
            }
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
            if (Shader.IsKeywordEnabled(Keyword))
            {
                return Mask;
            }
            else
            {
                return 0;
            }
        }

        public override int GetEnabledKeywords(ICollection<string> keywords)
        {
            if (Shader.IsKeywordEnabled(Keyword))
            {
                keywords.Add(Keyword);
                return Mask;
            }
            else
            {
                return 0;
            }
        }

        public override int GetEnabledKeywords(object source)
        {
            bool value = (bool)ReflectiveField.GetValue(source);
            if (value)
            {
                return Mask;
            }
            else
            {
                return 0;
            }
        }

        public override int GetEnabledKeywords(object source, ICollection<string> keywords)
        {
            bool value = (bool)ReflectiveField.GetValue(source);
            if (value)
            {
                keywords.Add(Keyword);
                return Mask;
            }
            else
            {
                return 0;
            }
        }

        public override int GetEnabledKeywords(Material source)
        {
            if (source.IsKeywordEnabled(Keyword))
            {
                return Mask;
            }
            else
            {
                return 0;
            }
        }

        public override int GetEnabledKeywords(Material source, ICollection<string> keywords)
        {
            if (source.IsKeywordEnabled(Keyword))
            {
                keywords.Add(Keyword);
                return Mask;
            }
            else
            {
                return 0;
            }
        }
    }

}
