using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace JiongXiaGu.ShaderTools
{

    public class ShaderAccessor : IShaderFieldGroup
    {
        public Type TargetType { get; }
        public List<IShaderFieldGroup> Children { get; }
        public int Mask { get; private set; }
        IReflectiveField IShaderFieldGroup.ReflectiveField => null;
        ShaderFieldFormat IShaderFieldGroup.Format => ShaderFieldFormat.None;

        internal ShaderAccessor(Type targetType, List<IShaderFieldGroup> children, int mask)
        {
            TargetType = targetType;
            Children = children;
            Mask = mask;
        }

        public ShaderAccessor(Type targetType)
        {
            TargetType = targetType;
            Children = ShaderFieldHelper.Find(targetType);
            Mask = ~0;
        }


        public override string ToString()
        {
            return string.Format("TargetType:" + TargetType.Name + ", ChildCount:" + Children.Count);
        }

        public void ForEach(ShaderFieldAction action)
        {
            foreach (var child in Children)
            {
                child.ForEach(action);
            }
        }

        public void ForEach(object v0, ShaderFieldAction1 action)
        {
            foreach (var child in Children)
            {
                child.ForEach(v0, action);
            }
        }

        public void ForEach(object v0, object v1, ShaderFieldAction2 action)
        {
            foreach (var child in Children)
            {
                child.ForEach(v0, v1, action);
            }
        }

        public void ForEach(object v0, object v1, object v2, ShaderFieldAction3 action)
        {
            foreach (var child in Children)
            {
                child.ForEach(v0, v1, v2, action);
            }
        }

        public object GetGlobalValues(object dest)
        {
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.GetGlobalValues(dest);
            }
            return dest;
        }

        public object GetGlobalValues(object dest, ShaderFieldAction filter)
        {
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.GetGlobalValues(dest, filter);
            }
            return dest;
        }

        public void SetGlobalValues(object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var child in Children)
            {
                child.SetGlobalValues(source);
            }
        }

        public void SetGlobalValues(object source, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var child in Children)
            {
                child.SetGlobalValues(source, filter);
            }
        }

        public object Copy(object source, object dest)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.Copy(source, dest);
            }
            return dest;
        }

        public object Copy(object source, object dest, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.Copy(source, dest, filter);
            }
            return dest;
        }


        private void InternalCopy(object source, Material dest, ICollection<string> enabledKeywords)
        {
            foreach (var child in Children)
            {
                child.Copy(source, dest, enabledKeywords);
            }
        }

        public void Copy(object source, Material dest)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            var enabledKeywords = new List<string>();
            InternalCopy(source, dest, enabledKeywords);
            dest.shaderKeywords = enabledKeywords.ToArray();
        }

        public void Copy(object source, Material dest, ICollection<string> enabledKeywords)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            InternalCopy(source, dest, enabledKeywords);
            dest.shaderKeywords = enabledKeywords.ToArray();
        }

        private void InternalCopy(object source, Material dest, ICollection<string> enabledKeywords, ShaderFieldAction filter)
        {
            foreach (var child in Children)
            {
                child.Copy(source, dest, enabledKeywords, filter);
            }
        }

        public void Copy(object source, Material dest, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            var enabledKeywords = new List<string>();
            InternalCopy(source, dest, enabledKeywords, filter);
            dest.shaderKeywords = enabledKeywords.ToArray();
        }

        public void Copy(object source, Material dest, ICollection<string> enabledKeywords, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            InternalCopy(source, dest, enabledKeywords, filter);
            dest.shaderKeywords = enabledKeywords.ToArray();
        }


        public void Copy(Material source, Material dest)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.Copy(source, dest);
            }
            dest.shaderKeywords = source.shaderKeywords.ToArray();
        }

        public void Copy(Material source, Material dest, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            var enabledKeywords = new List<string>();
            foreach (var child in Children)
            {
                child.Copy(source, dest, enabledKeywords, filter);
            }
            dest.shaderKeywords = enabledKeywords.ToArray();
        }

        public void Copy(Material source, Material dest, ICollection<string> enabledKeywords)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (enabledKeywords == null)
                throw new ArgumentNullException(nameof(enabledKeywords));

            foreach (var child in Children)
            {
                child.Copy(source, dest, enabledKeywords);
            }
            dest.shaderKeywords = enabledKeywords.ToArray();
        }

        public void Copy(Material source, Material dest, ICollection<string> enabledKeywords, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (enabledKeywords == null)
                throw new ArgumentNullException(nameof(enabledKeywords));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            foreach (var child in Children)
            {
                child.Copy(source, dest, enabledKeywords, filter);
            }
            dest.shaderKeywords = enabledKeywords.ToArray();
        }

        public object Copy(Material source, object dest)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.Copy(source, dest);
            }
            return dest;
        }

        public object Copy(Material source, object dest, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            foreach (var child in Children)
            {
                child.Copy(source, dest, filter);
            }
            return dest;
        }

        public object CopyWithoutKeywords(object source, object dest)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(source, dest);
            }
            return dest;
        }

        public object CopyWithoutKeywords(object source, object dest, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(source, dest, filter);
            }
            return dest;
        }

        public void CopyWithoutKeywords(object source, Material dest)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(source, dest);
            }
        }

        public void CopyWithoutKeywords(object source, Material dest, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(source, dest, filter);
            }
        }

        public object CopyWithoutKeywords(Material source, object dest)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(source, dest);
            }
            return dest;
        }

        public object CopyWithoutKeywords(Material source, object dest, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(source, dest, filter);
            }
            return dest;
        }

        public void CopyWithoutKeywords(Material source, Material dest)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(source, dest);
            }
        }

        public void CopyWithoutKeywords(Material source, Material dest, ShaderFieldAction filter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(source, dest, filter);
            }
        }

        public object Lerp(object v0, object v1, object dest, float t)
        {
            if (v0 == null)
                throw new ArgumentNullException(nameof(v0));
            if (v1 == null)
                throw new ArgumentNullException(nameof(v1));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            foreach (var child in Children)
            {
                child.Lerp(v0, v1, dest, t);
            }
            return dest;
        }

        public object Lerp(object v0, object v1, object dest, float t, ShaderFieldAction filter)
        {
            if (v0 == null)
                throw new ArgumentNullException(nameof(v0));
            if (v1 == null)
                throw new ArgumentNullException(nameof(v1));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            foreach (var child in Children)
            {
                child.Lerp(v0, v1, dest, t, filter);
            }
            return dest;
        }


        public int GetEnabledKeywords()
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords();
            }
            return mask;
        }

        public int GetEnabledKeywords(ShaderFieldAction filter)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(filter);
            }
            return mask;
        }

        public int GetEnabledKeywords(ICollection<string> keywords)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(keywords);
            }
            return mask;
        }

        public int GetEnabledKeywords(ICollection<string> keywords, ShaderFieldAction filter)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(keywords, filter);
            }
            return mask;
        }

        public int GetEnabledKeywords(object source)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(source);
            }
            return mask;
        }

        public int GetEnabledKeywords(object source, ShaderFieldAction filter)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(source, filter);
            }
            return mask;
        }

        public int GetEnabledKeywords(object source, ICollection<string> keywords)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(source, keywords);
            }
            return mask;
        }

        public int GetEnabledKeywords(object source, ICollection<string> keywords, ShaderFieldAction filter)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(source, keywords, filter);
            }
            return mask;
        }

        public int GetEnabledKeywords(Material source)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(source);
            }
            return mask;
        }

        public int GetEnabledKeywords(Material source, ShaderFieldAction filter)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(source, filter);
            }
            return mask;
        }

        public int GetEnabledKeywords(Material source, ICollection<string> keywords)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(source, keywords);
            }
            return mask;
        }

        public int GetEnabledKeywords(Material source, ICollection<string> keywords, ShaderFieldAction filter)
        {
            int mask = 0;
            foreach (var child in Children)
            {
                mask |= child.GetEnabledKeywords(source, keywords, filter);
            }
            return mask;
        }


        public static bool FilterByMask(IShaderFieldGroup group, int mask)
        {
            return (mask & group.Mask) != 0;
        }


    }

}
