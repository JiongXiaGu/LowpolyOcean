using UnityEngine;

namespace JiongXiaGu.LowpolyOcean
{
    public abstract class OceanDataBase : ScriptableObject
    {
        public abstract Material GetMaterial(int lodLevel = 0);
        public abstract Material GetMarkMaterial(int lodLevel = 0);
        public abstract PreparedContent GetRenderContents(int lodLevel = 0);
        public abstract float GetHeightAt(Vector3 worldPos);
    }
}
