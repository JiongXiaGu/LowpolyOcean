using JiongXiaGu.ShaderTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu
{

    [ShaderCustomFieldClass]
    public abstract class ScriptableFrameAnimation : ScriptableObject
    {
        protected ScriptableFrameAnimation()
        {
        }

        public abstract Texture GetCurrentTexture();


        [ShaderCustomFieldMethod]
        private static IShaderField MethodName()
        {
            return new ShaderFieldAccessor();
        }

        private class ShaderFieldAccessor : IShaderField
        {
            public ShaderFieldFormat Format => ShaderFieldFormat.Texture;

            public object GetGlobalValues(ShaderField info)
            {
                return null;
            }

            public void SetGlobalValues(ShaderField info, object value)
            {
                if (value != null)
                {
                    var target = (ScriptableFrameAnimation)value;
                    var texture = target.GetCurrentTexture();
                    Shader.SetGlobalTexture(info.ShderFieldID, texture);
                }
                else
                {
                    Shader.SetGlobalTexture(info.ShderFieldID, null);
                }
            }

            public object GetValue(ShaderField info, Material material)
            {
                return null;
            }

            public void SetMaterial(ShaderField info, Material material, object value)
            {
                if (value != null)
                {
                    var target = (ScriptableFrameAnimation)value;
                    var texture = target.GetCurrentTexture();
                    material.SetTexture(info.ShderFieldID, texture);
                }
                else
                {
                    material.SetTexture(info.ShderFieldID, null);
                }
            }

            public object Lerp(object v0, object v1, float t)
            {
                return ShaderField.LerpClass(v0, v1, t);
            }
        }
    }
}
