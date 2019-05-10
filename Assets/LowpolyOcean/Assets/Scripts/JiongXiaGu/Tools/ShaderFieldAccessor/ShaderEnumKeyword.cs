using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace JiongXiaGu.ShaderTools
{

    public class ShaderEnumKeyword : ShaderMultipleKeyword
    {
        public override ShaderFieldFormat Format => ShaderFieldFormat.EnumKeyword;

        public ShaderEnumKeyword(int mask, IReflectiveField fieldAccessor, object[] keywords) : base(mask, fieldAccessor, keywords)
        {
        }

        public override object GetGlobalValues(object dest)
        {
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                string keyword = GetKeyword(i);
                if (keyword != null)
                {
                    if (Shader.IsKeywordEnabled(keyword))
                    {
                        ReflectiveField.SetValue(dest, GetMask(i));
                        return dest;
                    }
                }
            }

            ReflectiveField.SetValue(dest, 0);
            return dest;
        }

        public override void SetGlobalValues(object source)
        {
            var value = (int)ReflectiveField.GetValue(source);
            if (value == 0)
            {
                for (int i = 0; i < KeywordsAndMask.Length; i += 2)
                {
                    string keyword = GetKeyword(i);
                    if (keyword != null)
                    {
                        Shader.DisableKeyword(keyword);
                    }
                }
            }
            else
            {
                for (int i = 0; i < KeywordsAndMask.Length; i += 2)
                {
                    string keyword = GetKeyword(i);
                    var enumValue = GetMask(i);
                    if (keyword != null && enumValue == value)
                    {
                        Shader.EnableKeyword(keyword);
                    }
                    else
                    {
                        Shader.DisableKeyword(keyword);
                    }
                }
            }
        }

        public override void Copy(Material source, Material dest, ICollection<string> enabledKeywords)
        {
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                string keyword = GetKeyword(i);
                if (keyword != null && source.IsKeywordEnabled(keyword))
                {
                    enabledKeywords.Add(keyword);
                }
            }
        }

        public override object Copy(Material source, object dest)
        {
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                string keyword = GetKeyword(i);
                if (keyword != null && source.IsKeywordEnabled(keyword))
                {
                    ReflectiveField.SetValue(dest, GetMask(i));
                    return dest;
                }
            }

            ReflectiveField.SetValue(dest, 0);
            return dest;
        }

        public override void Copy(object source, Material dest, ICollection<string> enabledKeywords)
        {
            var value = (int)ReflectiveField.GetValue(source);
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                if (value == GetMask(i))
                {
                    string keyword = GetKeyword(i);
                    if (keyword != null)
                    {
                        enabledKeywords.Add(keyword);
                    }
                }
            }
        }

        public override object Lerp(object v0, object v1, object dest, float t)
        {
            var value = ReflectiveField.GetValue(v0);
            ReflectiveField.SetValue(dest, value);
            return dest;
        }


        public override int GetEnabledKeywords()
        {
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                string keyword = GetKeyword(i);
                if (keyword != null && Shader.IsKeywordEnabled(keyword))
                {
                    int mask = GetMask(i);
                    return mask;
                }
            }
            return 0;
        }

        public override int GetEnabledKeywords(ICollection<string> keywords)
        {
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                string keyword = GetKeyword(i);
                if (keywords != null && Shader.IsKeywordEnabled(keyword))
                {
                    keywords.Add(keyword);
                    int mask = GetMask(i);
                    return mask;
                }
            }
            return 0;
        }

        public override int GetEnabledKeywords(object source)
        {
            var value = (int)ReflectiveField.GetValue(source);
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                string keyword = GetKeyword(i);
                if (keyword != null)
                {
                    int mask = GetMask(i);
                    if ((mask & value) != 0)
                    {
                        return mask;
                    }
                }
            }
            return 0;
        }

        public override int GetEnabledKeywords(object source, ICollection<string> keywords)
        {
            var value = (int)ReflectiveField.GetValue(source);
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                string keyword = GetKeyword(i);
                if (keyword != null)
                {
                    int mask = GetMask(i);
                    if ((mask & value) != 0)
                    {
                        keywords.Add(keyword);
                        return mask;
                    }
                }
            }
            return 0;
        }

        public override int GetEnabledKeywords(Material source)
        {
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                string keyword = GetKeyword(i);
                if (keyword != null && source.IsKeywordEnabled(keyword))
                {
                    int mask = GetMask(i);
                    return mask;
                }
            }
            return 0;
        }

        public override int GetEnabledKeywords(Material source, ICollection<string> keywords)
        {
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                string keyword = GetKeyword(i);
                if (keyword != null && source.IsKeywordEnabled(keyword))
                {
                    keywords.Add(keyword);
                    int mask = GetMask(i);
                    return mask;
                }
            }
            return 0;
        }
    }
}
