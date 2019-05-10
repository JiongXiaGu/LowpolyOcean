using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace JiongXiaGu.ShaderTools
{

    public abstract class ShaderMultipleKeyword : ShaderFieldBase, IEnumerable<KeyValuePair<int, string>>
    {
        public object[] KeywordsAndMask { get; }
        public string[] SortNames { get; }

        public string this[int index]
        {
            get { return (string)KeywordsAndMask[index * 2]; }
        }

        public ShaderMultipleKeyword(int mask, IReflectiveField fieldAccessor, object[] keywords) : base(mask, fieldAccessor)
        {
            if (keywords == null)
                throw new ArgumentNullException(nameof(keywords));
            if (keywords.Length == 0)
                throw new ArgumentException("keywords count is zero");
            if ((keywords.Length & 1) != 0)
                throw new ArgumentException("keywords count must even");

            KeywordsAndMask = keywords;
            SortNames = new string[keywords.Length / 2];
            for (int i = 0; i < keywords.Length; i += 2)
            {
                SortNames[i / 2] = ReflectiveField.FieldType.GetEnumName(GetMask(i));
            }
        }

        public override string ToString()
        {
            return "Name:" + ReflectiveField.Name + " ,Mask:" + Mask + " ,KeywordCount:" + KeywordsAndMask.Length / 2;
        }

        public string GetKeyword(int index)
        {
            return KeywordsAndMask[index] as string;
        }

        public int GetMask(int index)
        {
            return (int)KeywordsAndMask[index + 1];
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

        public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private struct Enumerator : IEnumerator<KeyValuePair<int, string>>
        {
            public ShaderMultipleKeyword Field { get; }
            private int index;
            public KeyValuePair<int, string> Current { get; private set; }
            object IEnumerator.Current => Current;

            public Enumerator(ShaderMultipleKeyword field)
            {
                Field = field;
                index = 0;
            }

            public bool MoveNext()
            {
                if (index < Field.KeywordsAndMask.Length)
                {
                    int mask = Field.GetMask(index);
                    string keyword = Field.GetKeyword(index);
                    Current = new KeyValuePair<int, string>(mask, keyword);
                    index += 2;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                index = 0;
            }

            public void Dispose()
            {
            }
        }
    }

}
