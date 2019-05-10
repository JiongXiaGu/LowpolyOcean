using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{


    [CanEditMultipleObjects]
    [CustomEditor(typeof(OceanFoamArea))]
    public sealed class OceanFoamAreaEditor : Editor
    {
        private const string foamAreaShaderName = "LowpolyOcean/FoamArea";
        private static readonly int textureShaderID = Shader.PropertyToID("_MainTex");
        private static readonly int colorShaderID = Shader.PropertyToID("_Color");
        public const string landColourShaderName = "Hidden/LowpolyOcean/LandColour";
        private const string foamAreaGeneratorShaderName = "Hidden/LowpolyOcean/FoamAreaGenerator";
        private static readonly int foamAreaGeneratorLandTextureShaderName = Shader.PropertyToID("_LandTexture");
        private static readonly int foamAreaGeneratorFoamTextureShaderName = Shader.PropertyToID("_FoamTexture");
        private static readonly int foamAreaGeneratorOffsetArrayShaderName = Shader.PropertyToID("_OffsetArray");
        private static Quaternion up = Quaternion.Euler(90, 0, 0);
        private static Material areaMaterial;
        private static Mesh quadMesh;

        [SerializeField] private ProjectSettings projectSettings = new ProjectSettings();
        private Camera cameraObject;
        private OceanFoamArea Target => target as OceanFoamArea;

        #region Circle

        private const int foamAreaGeneratorValuesCount = 32;
        private static readonly List<Vector4> circleValues = new List<Vector4>();
        private static readonly Vector4[] offsetValues = new Vector4[foamAreaGeneratorValuesCount];

        private void Add(ICollection<Vector4> result, int x, int y, Vector2 offset, float factor)
        {
            result.Add(new Vector4(x * offset.x, y * offset.y, factor));
        }

        private void AddCircle8(ICollection<Vector4> result, bool[,] signals, int x, int y, Vector2 offset, int radius)
        {
            var arrayX = x - 1;
            var arrayY = y - 1;

            if (!signals[arrayX, arrayY])
            {
                float factor = Mathf.Sqrt(x * x + y * y);
                factor = Mathf.InverseLerp(radius, 0, factor);

                Add(result, x, y, offset, factor);
                Add(result, x, y, offset, factor);
                Add(result, y, x, offset, factor);
                Add(result, x, -y, offset, factor);
                Add(result, y, -x, offset, factor);
                Add(result, -y, -x, offset, factor);
                Add(result, -x, -y, offset, factor);
                Add(result, -x, y, offset, factor);
                Add(result, -y, x, offset, factor);

                signals[arrayX, arrayY] = true;
            }

            for (int i = 1; i < y; i++)
            {
                if (!signals[arrayX, i - 1])
                {
                    float factor = Mathf.Sqrt(x * x + i * i);
                    factor = Mathf.InverseLerp(radius, 0, factor);

                    Add(result, x, i, offset, factor);
                    Add(result, x, -i, offset, factor);
                    Add(result, -x, -i, offset, factor);
                    Add(result, -x, i, offset, factor);

                    signals[arrayX, i - 1] = true;
                }
            }

            for (int i = 1; i < x; i++)
            {
                if (!signals[arrayY, i - 1])
                {
                    float factor = Mathf.Sqrt(y * y + i * i);
                    factor = Mathf.InverseLerp(radius, 0, factor);

                    Add(result, y, i, offset, factor);
                    Add(result, y, -i, offset, factor);
                    Add(result, -y, -i, offset, factor);
                    Add(result, -y, i, offset, factor);

                    signals[arrayY, i - 1] = true;
                }
            }
        }

        private void AddCircle5(ICollection<Vector4> result, int y, Vector2 offset, int radius)
        {
            Add(result, 0, 0, offset, 1);

            for (int i = 1; i <= y; i++)
            {
                float factor = Mathf.InverseLerp(radius, 0, i);

                Add(result, 0, i, offset, factor);
                Add(result, 0, -i, offset, factor);
                Add(result, i, 0, offset, factor);
                Add(result, -i, 0, offset, factor);
            }
        }

        private void GetCircleRange(ICollection<Vector4> result, Vector2 offset, int radius)
        {
            var signalsSize = radius;
            bool[,] signals = new bool[signalsSize, signalsSize];

            int x = 0;
            int y = radius;
            int d = 3 - 2 * radius;

            AddCircle5(result, y, offset, radius);

            while (x < y)
            {
                if (d < 0)
                {
                    d = d + 4 * x + 6;
                }
                else
                {
                    d = d + 4 * (x - y) + 10;
                    y--;
                }
                x++;

                AddCircle8(result, signals, x, y, offset, radius);
            }
        }

        public void CircleTest()
        {
            circleValues.Clear();
            GetCircleRange(circleValues, Vector2.one, (int)Target.areaGeneratorOptions.softenRadius);

            var root = new GameObject("CircleTest").transform;

            for (int i = 0; i < circleValues.Count; i++)
            {
                var pos = circleValues[i];
                GameObject item = GameObject.CreatePrimitive(PrimitiveType.Quad);
                pos.w = i;
                item.name = pos.ToString();
                item.transform.SetParent(root, false);
                item.transform.localScale = Vector3.one;
                item.transform.rotation = up;
                item.transform.localPosition = new Vector3(pos.x, pos.z * 5f, pos.y);
            }
        }

        private List<Vector4> GetCircleRange(Vector2 texelSize)
        {
            circleValues.Clear();
            GetCircleRange(circleValues, texelSize, (int)Target.areaGeneratorOptions.softenRadius);
            return circleValues;
        }

        #endregion

        private void GenerateFoamAreaTexture(Texture landTexture, RenderTexture result)
        {
            Material material = new Material(Shader.Find(foamAreaGeneratorShaderName));
            RenderTexture tempResultTexture = null;
            try
            {
                landTexture.wrapMode = TextureWrapMode.Clamp;
                material.SetTexture(foamAreaGeneratorLandTextureShaderName, landTexture);

                var circleValues = GetCircleRange(result.texelSize);
                int index = 0;
                bool resultIsResult = true;

                while (index < circleValues.Count)
                {
                    for (int tempIndex = 0; tempIndex < foamAreaGeneratorValuesCount; tempIndex++, index++)
                    {
                        if (index >= circleValues.Count)
                        {
                            for (; tempIndex < foamAreaGeneratorValuesCount; tempIndex++)
                            {
                                offsetValues[tempIndex] = Vector4.zero;
                            }
                            break;
                        }

                        offsetValues[tempIndex] = circleValues[index];
                    }

                    if (resultIsResult)
                    {
                        material.SetTexture(foamAreaGeneratorFoamTextureShaderName, tempResultTexture);
                        material.SetVectorArray(foamAreaGeneratorOffsetArrayShaderName, offsetValues);
                        Graphics.Blit(null, result, material);
                        resultIsResult = false;
                    }
                    else
                    {
                        if (tempResultTexture == null)
                            tempResultTexture = RenderTexture.GetTemporary(result.descriptor);

                        material.SetTexture(foamAreaGeneratorFoamTextureShaderName, result);
                        material.SetVectorArray(foamAreaGeneratorOffsetArrayShaderName, offsetValues);
                        Graphics.Blit(null, tempResultTexture, material);
                        resultIsResult = true;
                    }
                }

                if (!resultIsResult)
                {
                    Graphics.Blit(tempResultTexture, result);
                }
            }
            finally
            {
                DestroyImmediate(material);
                if (tempResultTexture != null)
                    RenderTexture.ReleaseTemporary(tempResultTexture);
            }
        }

        private void GenerateFoamAreaTexture(RenderTexture result)
        {
            if (cameraObject == null)
            {
                var gameObject = new GameObject("GenerateFoamAreaTextureCamera_EditorOnly");
                gameObject.hideFlags = HideFlags.DontSave;
                cameraObject = gameObject.AddComponent<Camera>();
                cameraObject.enabled = false;
                cameraObject.transform.SetParent(Target.transform, false);
                cameraObject.transform.eulerAngles = new Vector3(90, 0, 0);
                cameraObject.renderingPath = RenderingPath.Forward;
                cameraObject.clearFlags = CameraClearFlags.Depth;
                cameraObject.orthographic = true;
                cameraObject.allowDynamicResolution = false;
                cameraObject.useOcclusionCulling = false;
                cameraObject.allowMSAA = false;
                cameraObject.allowHDR = false;
                cameraObject.orthographicSize = Target.transform.lossyScale.x / 2;
                cameraObject.aspect = Target.transform.lossyScale.x / Target.transform.lossyScale.z;
            }

            var options = Target.areaGeneratorOptions;
            var tempTexture = RenderTexture.GetTemporary(result.width, result.height, 24, RenderTextureFormat.Depth);
            var landTexture = RenderTexture.GetTemporary(result.width, result.height);
            var landColourMat = new Material(Shader.Find(landColourShaderName));
            try
            {
                cameraObject.transform.localPosition = new Vector3(0, options.cameraHeight, 0);
                cameraObject.nearClipPlane = options.nearClipPlane;
                cameraObject.farClipPlane = options.farClipPlane;
                cameraObject.cullingMask = options.CullingMask;

                cameraObject.targetTexture = tempTexture;
                cameraObject.Render();
                cameraObject.targetTexture = null;

                float seaLevelDepth =(options.cameraHeight + options.seaLevel) / (cameraObject.farClipPlane - cameraObject.nearClipPlane);
                landColourMat.SetFloat("_SeaLevelDepth", seaLevelDepth);

                Graphics.Blit(tempTexture, landTexture, landColourMat);

                GenerateFoamAreaTexture(landTexture, result);
            }
            finally
            {
                RenderTexture.ReleaseTemporary(tempTexture);
                RenderTexture.ReleaseTemporary(landTexture);
                DestroyImmediate(landColourMat);
            }
        }

        private void GenerateFoamAreaTexture(string path)
        {
            var options = Target.areaGeneratorOptions;
            RenderTexture result = RenderTexture.GetTemporary(options.textureSize.x, options.textureSize.y);
            try
            {
                GenerateFoamAreaTexture(result);

                using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    result.Write(stream, TextureFileType.PNG);
                }

                AssetDatabase.ImportAsset(path);
                Target.Texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
            }
            finally
            {
                RenderTexture.ReleaseTemporary(result);
            }
        }

        private void GenerateFoamAreaTexture()
        {
            Texture texture = Target.Texture;
            string path;

            if (texture != null)
            {
                path = AssetDatabase.GetAssetPath(texture);
                string message = string.Format("Overwrite texture({0}), does not support undo", path);
                if (EditorUtility.DisplayDialog("Do you want to overwrite texture?", message, "yes", "no, save to..."))
                {
                    path = AssetDatabase.GetAssetPath(texture);
                    if (texture is RenderTexture)
                    {
                        GenerateFoamAreaTexture((RenderTexture)texture);
                    }
                    else
                    {
                        GenerateFoamAreaTexture(path);
                    }
                    return;
                }
            }

            if (texture != null)
            {
                string dir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(texture));
                path = EditorUtility.SaveFilePanelInProject("Save foam area", "FoamAreaTexture", "png", "save to file", dir);
            }
            else
            {
                path = EditorUtility.SaveFilePanelInProject("Save foam area", "FoamAreaTexture", "png", "save to file");
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            GenerateFoamAreaTexture(path);
        }


        [SerializeField] private bool isExpandedSettings;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Undo.RecordObject(this, "RecordObject " + nameof(OceanFoamAreaEditor));

            if (isExpandedSettings = EditorGUILayout.Foldout(isExpandedSettings, nameof(ProjectSettings)))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUI.BeginChangeCheck();

                    projectSettings.AreaColor = EditorGUILayout.ColorField(nameof(ProjectSettings.AreaColor), projectSettings.AreaColor);

                    if (EditorGUI.EndChangeCheck())
                    {
                        SceneView.RepaintAll();
                    }
                }
            }

            if (GUILayout.Button(nameof(GenerateFoamAreaTexture)))
            {
                GenerateFoamAreaTexture();
            }
        }

        private void DrawPreview(Camera camera)
        {
            if (camera.cameraType == CameraType.SceneView)
            {
                areaMaterial.SetColor(colorShaderID, projectSettings.AreaColor);
                areaMaterial.SetTexture(textureShaderID, Target.Texture);
                Matrix4x4 matrix = Matrix4x4.TRS(Target.transform.position, up, new Vector3(Target.transform.lossyScale.x, Target.transform.lossyScale.z, 1));
                Graphics.DrawMesh(quadMesh, matrix, areaMaterial, 0, camera, 0, null, false, false, false);
            }
        }

        /// <summary>
        /// rectangle 1x1, center(0.5, 0.5), normal(0, 0, -1)
        /// </summary>
        public static Mesh CreateQuadMesh()
        {
            var mesh = new Mesh();
            mesh.name = "Painter general mesh";

            mesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0f),
                new Vector3(-0.5f, 0.5f, 0f),
                new Vector3(0.5f, 0.5f, 0f),
                new Vector3(0.5f, -0.5f, 0f),
            };

            mesh.uv = new Vector2[]
            {
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
            };

            mesh.triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3,
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.UploadMeshData(true);
            return mesh;
        }

        private void OnEnable()
        {
            Camera.onPreCull += DrawPreview;

            if (areaMaterial == null)
            {
                var shader = Shader.Find(foamAreaShaderName);
                areaMaterial = new Material(shader);
            }
            if (quadMesh == null)
            {
                quadMesh = CreateQuadMesh();
            }

            projectSettings.Read();
            areaMaterial.SetColor(colorShaderID, projectSettings.AreaColor);
        }

        private void OnDisable()
        {
            Camera.onPreCull -= DrawPreview;
            projectSettings.Wirte();

            if (cameraObject != null)
            {
                DestroyImmediate(cameraObject.gameObject);
                cameraObject = null;
            }
        }

        [Serializable]
        public class ProjectSettings
        {
            private const string preName = nameof(OceanFoamAreaEditor) + "." + nameof(ProjectSettings);

            public Color AreaColor = new Color(1, 1, 1, 0.15f);

            public void Read()
            {
                string jsonValue = EditorPrefs.GetString(preName, null);
                if (jsonValue != null)
                {
                    JsonUtility.FromJsonOverwrite(jsonValue, this);
                }
            }

            public void Wirte()
            {
                string jsonValue = JsonUtility.ToJson(this);
                EditorPrefs.SetString(preName, jsonValue);
            }
        }
    }
}
