using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JiongXiaGu
{

    /// <summary>
    /// use shader to generate and save images;
    /// </summary>
    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(TextureGenerator))]
    public class TextureGenerator : ScriptableObject
    {
        private TextureGenerator()
        {
        }

        public GenerateForm UseShderType = GenerateForm.Shader;
        public Material Material;
        public Shader Shader;
        public int UsePass = 0;
        public ComputeShader ComputeShader;
        public ParameterList Parameters;
        public RenderTexture Target;
        public Vector2Int TextureSize = new Vector2Int(512, 512);
        public RenderTextureFormat TextureFormat = RenderTextureFormat.Default;

        private void Wirte(RenderTexture renderTexture)
        {
            switch (UseShderType)
            {
                case GenerateForm.Material:
                    DoMaterial(renderTexture);
                    break;

                case GenerateForm.Shader:
                    DoShader(renderTexture);
                    break;

                case GenerateForm.ComputeShader:
                    DoComputeShader(renderTexture);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [ContextMenu(nameof(Write))]
        public void Write()
        {
            RenderTexture renderTexture;
            if (Target == null)
            {
                renderTexture = new RenderTexture(TextureSize.x, TextureSize.y, 0, TextureFormat);
                renderTexture.enableRandomWrite = true;
                try
                {
                    Wirte(renderTexture);
                    TextureGeneratorHelper.WriteTexture(this, renderTexture);
                }
                finally
                {
                    if (renderTexture == RenderTexture.active)
                        RenderTexture.active = null;
                    renderTexture.Release();
                }
            }
            else
            {
                renderTexture = Target;
                Wirte(renderTexture);
            }

            Debug.Log("Write completed!");
        }

        public void DoMaterial(RenderTexture renderTexture)
        {
            if (Material == null)
                throw new ArgumentNullException(nameof(Material));

            Graphics.Blit(null, renderTexture, Material, UsePass);
        }

        public void DoShader(RenderTexture renderTexture)
        {
            if (Shader == null)
                throw new ArgumentNullException(nameof(Shader));

            Material material = new Material(Shader);
            material.hideFlags = HideFlags.HideAndDontSave;
            Parameter.SetParameters(material, Parameters);
            Graphics.Blit(null, renderTexture, material, UsePass);
            DestroyImmediate(material);
        }

        public void DoComputeShader(RenderTexture renderTexture)
        {
            if (ComputeShader == null)
                throw new ArgumentNullException(nameof(ComputeShader));

            int kernelHandle = ComputeShader.FindKernel("CSMain");
            ComputeShader.SetTexture(kernelHandle, "Result", renderTexture);
            Parameter.SetParameters(ComputeShader, kernelHandle, Parameters);
            ComputeShader.Dispatch(kernelHandle, 256 / 8, 256 / 8, 1);
        }


        [Serializable]
        public class ParameterList : CustomReorderableList<Parameter>
        {
        }


        public enum GenerateForm
        {
            Material,
            Shader,
            ComputeShader,
        }

        public enum ParameterFormat
        {
            Int,
            Float,
            Color,
            ColorValue,
            Texture,
        }

        [Serializable]
        public struct Parameter
        {
            public string name;
            public ParameterFormat format;
            public int intValue;
            public float floatValue;
            public Color color;
            public Vector4 colorValue;
            public Texture texture;

            public static void SetParameters(ComputeShader computeShader, int kernelHandle, IEnumerable<Parameter> parameters)
            {
                foreach (var parameter in parameters)
                {
                    switch (parameter.format)
                    {
                        case ParameterFormat.Int:
                            computeShader.SetInt(parameter.name, parameter.intValue);
                            break;

                        case ParameterFormat.Float:
                            computeShader.SetFloat(parameter.name, parameter.floatValue);
                            break;

                        case ParameterFormat.Color:
                            computeShader.SetVector(parameter.name, parameter.color);
                            break;

                        case ParameterFormat.ColorValue:
                            computeShader.SetVector(parameter.name, parameter.colorValue);
                            break;

                        case ParameterFormat.Texture:
                            if (parameter.texture != null)
                                computeShader.SetTexture(kernelHandle, parameter.name, parameter.texture);
                            break;

                        default:
                            Debug.LogWarning("Unkown " + parameter.format);
                            break;
                    }
                }
            }

            public static void SetParameters(Material material, IEnumerable<Parameter> parameters)
            {
                foreach (var parameter in parameters)
                {
                    switch (parameter.format)
                    {
                        case ParameterFormat.Int:
                            material.SetInt(parameter.name, parameter.intValue);
                            break;

                        case ParameterFormat.Float:
                            material.SetFloat(parameter.name, parameter.floatValue);
                            break;

                        case ParameterFormat.Color:
                            material.SetVector(parameter.name, parameter.color);
                            break;

                        case ParameterFormat.ColorValue:
                            material.SetVector(parameter.name, parameter.colorValue);
                            break;

                        case ParameterFormat.Texture:
                            if (parameter.texture != null)
                                material.SetTexture(parameter.name, parameter.texture);
                            break;

                        default:
                            Debug.LogWarning("Unkown " + parameter.format);
                            break;
                    }
                }
            }
        }
    }
}
