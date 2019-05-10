using System;
using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{
    public class ShaderEnumFlagsKeyword : ShaderMultipleKeyword
    {
        public override ShaderFieldFormat Format => ShaderFieldFormat.EnumFlagKeyword;

        public ShaderEnumFlagsKeyword(int mask, IReflectiveField fieldAccessor, object[] keywords) : base(mask, fieldAccessor, keywords)
        {
        }

        public override object GetGlobalValues(object dest)
        {
            int mask = 0;
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                string keyword = GetKeyword(i);
                if (keyword != null && Shader.IsKeywordEnabled(keyword))
                {
                    mask |= GetMask(i);
                }
            }
            ReflectiveField.SetValue(dest, mask);
            return dest;
        }

        public override void SetGlobalValues(object source)
        {
            int mask = (int)ReflectiveField.GetValue(source);
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                var keyword = GetKeyword(i);
                if (keyword != null)
                {
                    var enumValue = GetMask(i);
                    if ((enumValue & mask) != 0)
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
                var keyword = GetKeyword(i);
                if (keyword != null && source.IsKeywordEnabled(keyword))
                {
                    enabledKeywords.Add(keyword);
                }
            }
        }

        public override object Copy(Material source, object dest)
        {
            int mask = 0;
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                var keyword = GetKeyword(i);
                if (keyword != null && source.IsKeywordEnabled(keyword))
                {
                    var enumValue = GetMask(i);
                    mask |= enumValue;
                }
            }
            ReflectiveField.SetValue(dest, mask);
            return dest;
        }

        public override void Copy(object source, Material dest, ICollection<string> enabledKeywords)
        {
            int mask = (int)ReflectiveField.GetValue(source);
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                var keyword = GetKeyword(i);
                if (keyword != null)
                {
                    var enumValue = GetMask(i);
                    if ((enumValue & mask) != 0)
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
            int mask = 0;
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                var keyword = GetKeyword(i);
                if (keyword != null)
                {
                    if (Shader.IsKeywordEnabled(keyword))
                    {
                        mask |= GetMask(i);
                    }
                }
            }
            return mask;
        }

        public override int GetEnabledKeywords(ICollection<string> keywords)
        {
            int mask = 0;
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                var keyword = GetKeyword(i);
                if (keyword != null)
                {
                    if (Shader.IsKeywordEnabled(keyword))
                    {
                        mask |= GetMask(i);
                        keywords.Add(keyword);
                    }
                }
            }
            return mask;
        }

        public override int GetEnabledKeywords(object source)
        {
            int mask = 0;
            int value = (int)ReflectiveField.GetValue(source);
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                var keyword = GetKeyword(i);
                if (keyword != null)
                {
                    int cmask = GetMask(i);
                    if ((cmask & value) != 0)
                    {
                        mask |= cmask;
                    }
                }
            }
            return mask;
        }

        public override int GetEnabledKeywords(object source, ICollection<string> keywords)
        {
            int mask = 0;
            int value = (int)ReflectiveField.GetValue(source);
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                var keyword = GetKeyword(i);
                if (keyword != null)
                {
                    int cmask = GetMask(i);
                    if ((cmask & value) != 0)
                    {
                        mask |= cmask;
                        keywords.Add(keyword);
                    }
                }
            }
            return mask;
        }

        public override int GetEnabledKeywords(Material source)
        {
            int mask = 0;
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                var keyword = GetKeyword(i);
                if (keyword != null)
                {
                    if (source.IsKeywordEnabled(keyword))
                    {
                        mask |= GetMask(i);
                    }
                }
            }
            return mask;
        }

        public override int GetEnabledKeywords(Material source, ICollection<string> keywords)
        {
            int mask = 0;
            for (int i = 0; i < KeywordsAndMask.Length; i += 2)
            {
                var keyword = GetKeyword(i);
                if (keyword != null)
                {
                    if (source.IsKeywordEnabled(keyword))
                    {
                        mask |= GetMask(i);
                        keywords.Add(keyword);
                    }
                }
            }
            return mask;
        }
    }

}
