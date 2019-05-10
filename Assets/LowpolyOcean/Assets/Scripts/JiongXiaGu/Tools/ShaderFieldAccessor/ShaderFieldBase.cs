using System.Collections.Generic;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{
    public abstract class ShaderFieldBase : IShaderFieldGroup
    {
        public int Mask { get; }
        public IReflectiveField ReflectiveField { get; }
        public List<IShaderFieldGroup> Children => null;
        public abstract ShaderFieldFormat Format { get; }

        public ShaderFieldBase()
        {
        }

        public ShaderFieldBase(int mask, IReflectiveField fieldAccessor)
        {
            Mask = mask;
            ReflectiveField = fieldAccessor;
        }

        public virtual void ForEach(ShaderFieldAction action)
        {
            action.Invoke(this);
        }

        public virtual void ForEach(object v0, ShaderFieldAction1 action)
        {
            action.Invoke(this, v0);
        }

        public virtual void ForEach(object v0, object v1, ShaderFieldAction2 action)
        {
            action.Invoke(this, v0, v1);
        }

        public virtual void ForEach(object v0, object v1, object v2, ShaderFieldAction3 action)
        {
            action.Invoke(this, v0, v1, v2);
        }

        public abstract object GetGlobalValues(object dest);

        public virtual object GetGlobalValues(object dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                return GetGlobalValues(dest);
            }
            return dest;
        }

        public abstract void SetGlobalValues(object source);

        public virtual void SetGlobalValues(object source, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                SetGlobalValues(source);
            }
        }

        public virtual object Copy(object source, object dest)
        {
            var value = ReflectiveField.GetValue(source);
            ReflectiveField.SetValue(dest, value);
            return dest;
        }

        public virtual object Copy(object source, object dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                return Copy(source, dest);
            }
            return dest;
        }

        public abstract void Copy(Material source, Material dest, ICollection<string> enabledKeywords);

        public virtual void Copy(Material source, Material dest, ICollection<string> enabledKeywords, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                Copy(source, dest, enabledKeywords);
            }
        }

        public abstract object Copy(Material source, object dest);

        public virtual object Copy(Material source, object dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                return Copy(source, dest);
            }
            return dest;
        }

        public abstract void Copy(object source, Material dest, ICollection<string> enabledKeywords);

        public virtual void Copy(object source, Material dest, ICollection<string> enabledKeywords, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                Copy(source, dest, enabledKeywords);
            }
        }

        public abstract object CopyWithoutKeywords(object source, object dest);

        public object CopyWithoutKeywords(object source, object dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                CopyWithoutKeywords(source, dest);
            }
            return dest;
        }

        public abstract void CopyWithoutKeywords(Material source, Material dest);

        public void CopyWithoutKeywords(Material source, Material dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                CopyWithoutKeywords(source, dest);
            }
        }

        public abstract object CopyWithoutKeywords(Material source, object dest);

        public object CopyWithoutKeywords(Material source, object dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                CopyWithoutKeywords(source, dest);
            }
            return dest;
        }

        public abstract void CopyWithoutKeywords(object source, Material dest);

        public void CopyWithoutKeywords(object source, Material dest, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                CopyWithoutKeywords(source, dest);
            }
        }

        public abstract object Lerp(object v0, object v1, object dest, float t);

        public virtual object Lerp(object v0, object v1, object dest, float t, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                return Lerp(v0, v1, dest, t);
            }
            return dest;
        }

        public abstract int GetEnabledKeywords();

        public int GetEnabledKeywords(ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                return GetEnabledKeywords();
            }
            return 0;
        }

        public abstract int GetEnabledKeywords(ICollection<string> keywords);

        public int GetEnabledKeywords(ICollection<string> keywords, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                return GetEnabledKeywords(keywords);
            }
            return 0;
        }

        public abstract int GetEnabledKeywords(object source);

        public int GetEnabledKeywords(object source, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                return GetEnabledKeywords(source);
            }
            return 0;
        }

        public abstract int GetEnabledKeywords(object source, ICollection<string> keywords);

        public int GetEnabledKeywords(object source, ICollection<string> keywords, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                return GetEnabledKeywords(source, keywords);
            }
            return 0;
        }

        public abstract int GetEnabledKeywords(Material source);

        public int GetEnabledKeywords(Material source, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                return GetEnabledKeywords(source);
            }
            return 0;
        }

        public abstract int GetEnabledKeywords(Material source, ICollection<string> keywords);

        public int GetEnabledKeywords(Material source, ICollection<string> keywords, ShaderFieldAction filter)
        {
            if (filter.Invoke(this))
            {
                return GetEnabledKeywords(source, keywords);
            }
            return 0;
        }
    }
}
