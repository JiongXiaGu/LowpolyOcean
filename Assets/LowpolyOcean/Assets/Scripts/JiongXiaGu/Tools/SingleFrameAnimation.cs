using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu
{

    [CreateAssetMenu(menuName = EditorModeHelper.AssetMenuNameRoot + nameof(SingleFrameAnimation))]
    public sealed class SingleFrameAnimation : ScriptableFrameAnimation
    {
        private SingleFrameAnimation()
        {
        }

        [SerializeField]
        private Texture texture;

        public Texture Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public override Texture GetCurrentTexture()
        {
            return texture;
        }
    }
}
