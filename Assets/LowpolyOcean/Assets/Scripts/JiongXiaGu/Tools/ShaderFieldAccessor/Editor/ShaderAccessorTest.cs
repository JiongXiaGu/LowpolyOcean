using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{


    public class ShaderAccessorTest
    {
        private static Texture tempTexture;
        private static Texture CreateTempTexture()
        {
            if (tempTexture == null)
            {
                tempTexture = new Texture2D(2, 2);
                tempTexture.name = "TempTexture";
            }
            return tempTexture;
        }

        private static readonly ShaderOptions readOnlyEmtpy = ShaderOptions.CreateEmpty();
        private void ReleaseTemporary()
        {
            if (tempTexture == null)
            {
                UnityEngine.Object.DestroyImmediate(tempTexture);
                tempTexture = null;
            }

            ShaderOptions.Accessor.SetGlobalValues(readOnlyEmtpy);
        }

        [Test]
        public void GlobalAssignmentTest()
        {
            try
            {
                ShaderOptions shaderOptions = ShaderOptions.CreateExample0();
                ShaderOptions.Accessor.SetGlobalValues(shaderOptions);

                var newShaderOptions = ShaderOptions.CreateEmpty();
                ShaderOptions.Accessor.GetGlobalValues(newShaderOptions);

                shaderOptions.Mode.AreEqual(newShaderOptions.Mode);
                shaderOptions.Values0.AreEqualWithoutTextureOffsetScale(newShaderOptions.Values0);
                shaderOptions.Values1.AreEqual(newShaderOptions.Values1);
            }
            finally
            {
                ReleaseTemporary();
            }
        }

        [Test]
        public void GlobalAssignmentMaskTest()
        {
            try
            {
                int mask = (int)(Mask.Group0 | Mask.Group1 | Mask.Group2);

                ShaderOptions shaderOptions = ShaderOptions.CreateExample0();
                ShaderOptions.Accessor.SetGlobalValues(shaderOptions, group => ShaderAccessor.FilterByMask(group, mask));

                var newShaderOptions = ShaderOptions.CreateEmpty();
                ShaderOptions.Accessor.GetGlobalValues(newShaderOptions);

                shaderOptions.Values0.AreEqualWithoutTextureOffsetScale(newShaderOptions.Values0);
                shaderOptions.Values1.AreNotEqual(newShaderOptions.Values1);
            }
            finally
            {
                ReleaseTemporary();
            }
        }

        [Test]
        public void CopyInstanceTest()
        {
            try
            {
                ShaderOptions shaderOptions = ShaderOptions.CreateExample0();
                ShaderOptions newShaderOptions = ShaderOptions.CreateEmpty();
                ShaderOptions.Accessor.Copy(shaderOptions, newShaderOptions);

                shaderOptions.Mode.AreEqual(newShaderOptions.Mode);
                shaderOptions.Values0.AreEqual(newShaderOptions.Values0);
                shaderOptions.Values1.AreEqual(newShaderOptions.Values1);
            }
            finally
            {
                ReleaseTemporary();
            }
        }

        [Test]
        public void CopyInstanceWithoutKeywordTest()
        {
            try
            {
                ShaderOptions shaderOptions = ShaderOptions.CreateExample0();
                ShaderOptions newShaderOptions = ShaderOptions.CreateEmpty();
                ShaderOptions.Accessor.CopyWithoutKeywords(shaderOptions, newShaderOptions);

                shaderOptions.Mode.AreNotEqual(newShaderOptions.Mode);
                shaderOptions.Values0.AreEqual(newShaderOptions.Values0);
                shaderOptions.Values1.AreEqual(newShaderOptions.Values1);
            }
            finally
            {
                ReleaseTemporary();
            }
        }

        [Test]
        public void MemberCountTest()
        {
            Assert.AreEqual(ShaderOptions.Values1Accessor.Children.Count, 3);
            Assert.IsTrue(ShaderOptions.Values1Accessor.Contains(nameof(Values1.FieldType0)));
            Assert.IsTrue(ShaderOptions.Values1Accessor.Contains(nameof(Values1.FieldType1)));
            Assert.IsTrue(ShaderOptions.Values1Accessor.Contains(nameof(Values1.PropertyType)));
            Assert.IsFalse(ShaderOptions.Values1Accessor.Contains(nameof(Values1.IgnoreProperty0Type)));
            Assert.IsFalse(ShaderOptions.Values1Accessor.Contains(nameof(Values1.IgnoreProperty1Type)));
        }

        [Test]
        public void GetKeywordTest()
        {
            try
            {
                Mode mode = new Mode()
                {
                    Group0 = true,
                    ModeSwitch = ModeSwitch.EnableGroup1,
                    ModeFlag = ModeFlag.Property,
                };
                var target = Mask.Group0 | Mask.Group1 | Mask.Property;

                var mask = (Mask)ShaderOptions.ModeAccessor.GetEnabledKeywords(mode);
                Assert.AreEqual(target, mask);
            }
            finally
            {
                ReleaseTemporary();
            }
        }

        [Flags]
        public enum Mask
        {
            Group0 = 1 << 0,
            Group1 = 1 << 1,
            Group2 = 1 << 2,
            Field = 1 << 3,
            Property = 1 << 4,
        }

        public enum ModeSwitch
        {
            None = 0,
            EnableGroup1 = Mask.Group1,
            EnableGroup2 = Mask.Group2,
        }

        [Flags]
        public enum ModeFlag
        {
            Field = Mask.Field,
            Property = Mask.Property,
        }

        [Serializable]
        [ShaderFieldGroup]
        public class Mode
        {
            public const string Group0Keyword = "_ShaderAccessorTestGroup0";
            public const string Group1Keyword = "_ShaderAccessorTestGroup1";
            public const string Group2Keyword = "_ShaderAccessorTestGroup2";
            public const string FieldKeyword = "_ShaderAccessorTestField";
            public const string PropertyKeyword = "_ShaderAccessorTestProperty";

            [ShaderFieldKeyword(Group0Keyword, Mask.Group0)] public bool Group0;

            [ShaderFieldEnumKeyword(null, ModeSwitch.None
                , Group1Keyword, ModeSwitch.EnableGroup1
                , Group2Keyword, ModeSwitch.EnableGroup2)]
            public ModeSwitch ModeSwitch;

            [ShaderFieldEnumKeyword(FieldKeyword, ModeFlag.Field
                , PropertyKeyword, ModeFlag.Property)]
            public ModeFlag ModeFlag;

            public static Mode CloseAll => new Mode()
            {
                Group0 = false,
                ModeSwitch = ModeSwitch.None,
                ModeFlag = 0,
            };

            public void AreEqual(Mode actual)
            {
                Assert.NotNull(actual);
                Assert.AreEqual(Group0, actual.Group0);
                Assert.AreEqual(ModeSwitch, actual.ModeSwitch);
                Assert.AreEqual(ModeFlag, actual.ModeFlag);
            }

            public void AreNotEqual(Mode actual)
            {
                Assert.NotNull(actual);
                Assert.AreNotEqual(Group0, actual.Group0);
                Assert.AreNotEqual(ModeSwitch, actual.ModeSwitch);
                Assert.AreNotEqual(ModeFlag, actual.ModeFlag);
            }
        }

        [Serializable]
        [ShaderFieldGroup(Mask.Group0 | Mask.Group1 | Mask.Group2)]
        public class Values0
        {
            public const string FloatValueName = "_FloatValue";
            [ShaderField(FloatValueName, Mask.Group0)] public float floatValue;

            public const string IntValueName = "_IntValue";
            [ShaderField(IntValueName, Mask.Group0)] public int intValue;

            public const string ColorValueName = "_ColorValue";
            [ShaderField(ColorValueName, Mask.Group1)] public Color ColorValue;

            public const string Vector2ValueName = "_Vertor2Value";
            [ShaderField(Vector2ValueName, Mask.Group1)] public Vector2 Vector2Value;

            public const string Vector3ValueName = "_Vertor3Value";
            [ShaderField(Vector3ValueName, Mask.Group1)] public Vector3 Vector3Value;

            public const string Vector4ValueName = "_Vertor4Value";
            [ShaderField(Vector4ValueName, Mask.Group1)] public Vector4 Vector4Value;

            public const string EnumValueName = "_EnumValue";
            [ShaderField(EnumValueName, Mask.Group1)] public ModeFlag EnumValue;

            public const string TextureValueName = "_TextureValue";
            [ShaderField(TextureValueName, Mask.Group2)] public Texture TextureValue;

            public const string TextureScaleValueName = "_TextureValueScale";
            [ShaderField(TextureScaleValueName, Mask.Group2)] public Vector2 TextureScale;

            public const string TextureOffsetValueName = "_TextureValueOffset";
            [ShaderField(TextureOffsetValueName, Mask.Group2)] public Vector2 TextureOffset;

            public void AreNotEqual(Values0 actual)
            {
                Assert.NotNull(actual);
                Assert.AreNotEqual(floatValue, actual.floatValue);
                Assert.AreNotEqual(intValue, actual.intValue);
                Assert.AreNotEqual(Vector2Value, actual.Vector2Value);
                Assert.AreNotEqual(Vector3Value, actual.Vector3Value);
                Assert.AreNotEqual(Vector4Value, actual.Vector4Value);
            }

            public void AreEqualWithoutTextureOffsetScale(Values0 actual)
            {
                Assert.NotNull(actual);
                Assert.AreEqual(floatValue, actual.floatValue);
                Assert.AreEqual(intValue, actual.intValue);
                Assert.AreEqual(Vector2Value, actual.Vector2Value);
                Assert.AreEqual(Vector3Value, actual.Vector3Value);
                Assert.AreEqual(Vector4Value, actual.Vector4Value);
            }

            public void AreEqual(Values0 actual)
            {
                AreEqualWithoutTextureOffsetScale(actual);
                Assert.AreEqual(TextureValue, actual.TextureValue);
                Assert.AreEqual(TextureScale, actual.TextureScale);
                Assert.AreEqual(TextureOffset, actual.TextureOffset);
            }
        }

        [Serializable]
        [ShaderFieldGroup(Mask.Field | Mask.Property)]
        public class Values1
        {
            public const string Field0TypeName = "_FieldType0";
            [ShaderField(Field0TypeName, Mask.Field)] public int FieldType0;

            public const string Field1TypeName = "_FieldType1";
            [ShaderField(Field1TypeName, Mask.Field)] internal int FieldType1;

            public const string PropertyTypeName = "_PropertyType";
            [ShaderField(PropertyTypeName, Mask.Property)] public int PropertyType { get; set; }

            public const string IgnoreProperty0TypeName = "_IgnorePropertyType0";
            [ShaderField(IgnoreProperty0TypeName, Mask.Property)] internal int IgnoreProperty0Type { get; set; }

            public const string IgnoreProperty1TypeName = "_IgnorePropertyType1";
            [ShaderField(IgnoreProperty1TypeName, Mask.Property)] public int IgnoreProperty1Type { get; internal set; }

            public void AreEqual(Values1 actual)
            {
                Assert.NotNull(actual);
                Assert.AreEqual(FieldType0, actual.FieldType0);
                Assert.AreEqual(FieldType1, actual.FieldType1);
                Assert.AreEqual(PropertyType, actual.PropertyType);

                Assert.AreNotEqual(IgnoreProperty0Type, actual.IgnoreProperty0Type);
                Assert.AreNotEqual(IgnoreProperty1Type, actual.IgnoreProperty1Type);
            }

            public void AreNotEqual(Values1 actual)
            {
                Assert.NotNull(actual);
                Assert.AreNotEqual(FieldType0, actual.FieldType0);
                Assert.AreNotEqual(FieldType1, actual.FieldType1);
                Assert.AreNotEqual(PropertyType, actual.PropertyType);
            }
        }

        public class ShaderOptions
        {
            public static ShaderAccessor Accessor { get; } = new ShaderAccessor(typeof(ShaderOptions));
            public static ShaderAccessor ModeAccessor { get; } = Accessor.CreateAccessor(nameof(Mode));
            public static ShaderAccessor Values0Accessor { get; } = Accessor.CreateAccessor(nameof(Values0));
            public static ShaderAccessor Values1Accessor { get; } = Accessor.CreateAccessor(nameof(Values1));

            public Mode Mode;
            public Values0 Values0;
            public Values1 Values1;

            public static ShaderOptions CreateEmpty()
            {
                ShaderOptions item = new ShaderOptions()
                {
                    Mode = new Mode(),
                    Values0 = new Values0(),
                    Values1 = new Values1(),
                };
                return item;
            }

            public static ShaderOptions CreateExample0()
            {
                ShaderOptions item = new ShaderOptions()
                {
                    Mode = new Mode()
                    {
                        Group0 = true,
                        ModeSwitch = ModeSwitch.EnableGroup1,
                        ModeFlag = ModeFlag.Property | ModeFlag.Field,
                    },
                    Values0 = new Values0()
                    {
                        floatValue = 115884.484f,
                        intValue = 5486313,
                        ColorValue = Color.red,
                        Vector2Value = new Vector2(1.55f, 2.66f),
                        Vector3Value = new Vector3(684.55f, 4206f),
                        Vector4Value = new Vector4(16.25f, 19.65f),
                        EnumValue = ModeFlag.Property,
                        TextureValue = CreateTempTexture(),
                        TextureScale = new Vector2(2f, 5f),
                        TextureOffset = new Vector2(35, 25f),
                    },
                    Values1 = new Values1()
                    {
                        FieldType0 = 123,
                        FieldType1 = 486,
                        PropertyType = 789,
                        IgnoreProperty0Type = 120,
                        IgnoreProperty1Type = 159,
                    },
                };
                return item;
            }
        }
    }
}
