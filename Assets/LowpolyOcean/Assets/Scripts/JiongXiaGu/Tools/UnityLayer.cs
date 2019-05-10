using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu
{

    [Serializable]
    public struct UnityLayer : IEquatable<UnityLayer>, IEquatable<int>
    {
        public int Layer;

        public UnityLayer(int layer)
        {
            Layer = layer;
        }

        public override string ToString()
        {
            return Layer.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is int)
            {
                return Equals((int)obj);
            }
            else if (obj is UnityLayer)
            {
                return Equals((UnityLayer)obj);
            }
            return false;
        }

        public bool Equals(UnityLayer other)
        {
            return Layer == other.Layer;
        }

        public bool Equals(int other)
        {
            return Layer.Equals(other);
        }

        public override int GetHashCode()
        {
            return Layer.GetHashCode();
        }

        public static bool operator ==(UnityLayer layer1, UnityLayer layer2)
        {
            return layer1.Equals(layer2);
        }

        public static bool operator !=(UnityLayer layer1, UnityLayer layer2)
        {
            return !(layer1 == layer2);
        }

        public static implicit operator int(UnityLayer unityLayer)
        {
            return unityLayer.Layer;
        }

        public static implicit operator UnityLayer(int layer)
        {
            return new UnityLayer(layer);
        }
    }
}
