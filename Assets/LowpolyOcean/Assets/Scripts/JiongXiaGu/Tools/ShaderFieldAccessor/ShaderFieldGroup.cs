using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{
    public class ShaderFieldGroup : IShaderFieldGroup
    {
        public int Mask { get; }
        public IReflectiveField ReflectiveField { get; }
        public List<IShaderFieldGroup> Children { get; }
        public ShaderFieldFormat Format => ShaderFieldFormat.None;

        public ShaderFieldGroup(IReflectiveField fieldAccessor, int mask)
        {
            ReflectiveField = fieldAccessor;
            Mask = mask;
            Children = new List<IShaderFieldGroup>();
        }

        public override string ToString()
        {
            return "Name:" + ReflectiveField.Name + ", Mask:" + Mask + ", ChildCount:" + Children.Count;
        }

        public void ForEach(ShaderFieldAction action)
        {
            if (action.Invoke(this))
            {
                foreach (var child in Children)
                {
                    child.ForEach(action);
                }
            }
        }

        public void ForEach(object v0, ShaderFieldAction1 action)
        {
            if (action.Invoke(this, v0))
            {
                var t0 = ReflectiveField.GetValue(v0);
                foreach (var child in Children)
                {
                    child.ForEach(t0, action);
                }
            }
        }

        public void ForEach(object v0, object v1, ShaderFieldAction2 action)
        {
            if (action.Invoke(this, v0, v1))
            {
                var t0 = ReflectiveField.GetValue(v0);
                var t1 = ReflectiveField.GetValue(v1);
                foreach (var child in Children)
                {
                    child.ForEach(t0, t1, action);
                }
            }
        }

        public void ForEach(object v0, object v1, object v2, ShaderFieldAction3 action)
        {
            if (action.Invoke(this, v0, v1, v2))
            {
                var t0 = ReflectiveField.GetValue(v0);
                var t1 = ReflectiveField.GetValue(v1);
                var t2 = ReflectiveField.GetValue(v2);
                foreach (var child in Children)
                {
                    child.ForEach(t0, t1, t2, action);
                }
            }
        }

        public object GetGlobalValues(object dest)
        {
            var value = ReflectiveField.GetValue(dest);
            foreach (var child in Children)
            {
                child.GetGlobalValues(value);
            }
            return dest;
        }

        public object GetGlobalValues(object dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                var value = ReflectiveField.GetValue(dest);
                foreach (var child in Children)
                {
                    child.GetGlobalValues(value, filter);
                }
            }
            return dest;
        }

        public void SetGlobalValues(object source)
        {
            var value = ReflectiveField.GetValue(source);
            foreach (var child in Children)
            {
                child.SetGlobalValues(value);
            }
        }

        public void SetGlobalValues(object source, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                var value = ReflectiveField.GetValue(source);
                foreach (var child in Children)
                {
                    child.SetGlobalValues(value, filter);
                }
            }
        }

        public object Copy(object source, object dest)
        {
            var sourceValue = ReflectiveField.GetValue(source);
            var destValue = ReflectiveField.GetValue(dest);
            foreach (var child in Children)
            {
                child.Copy(sourceValue, destValue);
            }
            return dest;
        }

        public object Copy(object source, object dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                var sourceValue = ReflectiveField.GetValue(source);
                var destValue = ReflectiveField.GetValue(dest);
                foreach (var child in Children)
                {
                    child.Copy(sourceValue, destValue, filter);
                }
            }
            return dest;
        }

        public void Copy(Material source, Material dest, ICollection<string> enabledKeywords)
        {
            foreach (var child in Children)
            {
                child.Copy(source, dest, enabledKeywords);
            }
        }

        public void Copy(Material source, Material dest, ICollection<string> enabledKeywords, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                foreach (var child in Children)
                {
                    child.Copy(source, dest, enabledKeywords, filter);
                }
            }
        }

        public object Copy(Material source, object dest)
        {
            var destValue = ReflectiveField.GetValue(dest);
            foreach (var child in Children)
            {
                child.Copy(source, destValue);
            }
            return dest;
        }

        public object Copy(Material source, object dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                var destValue = ReflectiveField.GetValue(dest);
                foreach (var child in Children)
                {
                    child.Copy(source, destValue, filter);
                }
            }
            return dest;
        }

        public void Copy(object source, Material dest, ICollection<string> enabledKeywords)
        {
            var sourceValue = ReflectiveField.GetValue(source);
            foreach (var child in Children)
            {
                child.Copy(sourceValue, dest, enabledKeywords);
            }
        }

        public void Copy(object source, Material dest, ICollection<string> enabledKeywords, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                var sourceValue = ReflectiveField.GetValue(source);
                foreach (var child in Children)
                {
                    child.Copy(sourceValue, dest, enabledKeywords, filter);
                }
            }
        }

        public object CopyWithoutKeywords(object source, object dest)
        {
            var sourceValue = ReflectiveField.GetValue(source);
            var destValue = ReflectiveField.GetValue(dest);
            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(sourceValue, destValue);
            }
            return dest;
        }

        public object CopyWithoutKeywords(object source, object dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                var sourceValue = ReflectiveField.GetValue(source);
                var destValue = ReflectiveField.GetValue(dest);
                foreach (var child in Children)
                {
                    child.CopyWithoutKeywords(sourceValue, destValue, filter);
                }
            }
            return dest;
        }

        public void CopyWithoutKeywords(Material source, Material dest)
        {
            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(source, dest);
            }
        }

        public void CopyWithoutKeywords(Material source, Material dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                foreach (var child in Children)
                {
                    child.CopyWithoutKeywords(source, dest, filter);
                }
            }
        }

        public object CopyWithoutKeywords(Material source, object dest)
        {
            var destValue = ReflectiveField.GetValue(dest);
            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(source, destValue);
            }
            return dest;
        }

        public object CopyWithoutKeywords(Material source, object dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                var destValue = ReflectiveField.GetValue(dest);
                foreach (var child in Children)
                {
                    child.CopyWithoutKeywords(source, destValue, filter);
                }
            }
            return dest;
        }

        public void CopyWithoutKeywords(object source, Material dest)
        {
            var sourceValue = ReflectiveField.GetValue(source);
            foreach (var child in Children)
            {
                child.CopyWithoutKeywords(sourceValue, dest);
            }
        }

        public void CopyWithoutKeywords(object source, Material dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                var sourceValue = ReflectiveField.GetValue(source);
                foreach (var child in Children)
                {
                    child.CopyWithoutKeywords(sourceValue, dest, filter);
                }
            }
        }

        public object Lerp(object v0, object v1, object dest, float t)
        {
            var value0 = ReflectiveField.GetValue(v0);
            var value1 = ReflectiveField.GetValue(v1);
            var destValue = ReflectiveField.GetValue(dest);
            foreach (var child in Children)
            {
                child.Lerp(value0, value1, destValue, t);
            }
            return dest;
        }

        public object Lerp(object v0, object v1, object dest, float t, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                var value0 = ReflectiveField.GetValue(v0);
                var value1 = ReflectiveField.GetValue(v1);
                var destValue = ReflectiveField.GetValue(dest);
                foreach (var child in Children)
                {
                    child.Lerp(value0, value1, destValue, t, filter);
                }
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
            if (filter.Invoke(this))
            {
                foreach (var child in Children)
                {
                    mask |= child.GetEnabledKeywords(filter);
                }
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
            if (filter.Invoke(this))
            {
                foreach (var child in Children)
                {
                    mask |= child.GetEnabledKeywords(keywords, filter);
                }
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
            if (filter.Invoke(this))
            {
                foreach (var child in Children)
                {
                    mask |= child.GetEnabledKeywords(source, filter);
                }
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
            if (filter.Invoke(this))
            {
                foreach (var child in Children)
                {
                    mask |= child.GetEnabledKeywords(source, keywords, filter);
                }
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
            if (filter.Invoke(this))
            {
                foreach (var child in Children)
                {
                    mask |= child.GetEnabledKeywords(source, filter);
                }
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
            if (filter.Invoke(this))
            {
                foreach (var child in Children)
                {
                    mask |= child.GetEnabledKeywords(source, filter);
                }
            }
            return mask;
        }
    }

}
