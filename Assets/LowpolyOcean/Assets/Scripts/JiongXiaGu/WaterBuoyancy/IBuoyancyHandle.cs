using UnityEngine;

namespace JiongXiaGu.WaterBuoyancy
{

    public interface IBuoyancyHandle : IStorageInfoDrawer
    {
        bool IsInfluenceDrag { get; }
        string ComponentName { get; }
        bool TryGetForce(IOceanData data, BuoyancyObject sender, out PosAndForce value);
    }

    public interface IPropellerHandle : IStorageInfoDrawer
    {
        string ComponentName { get; }
        bool TryGetForce(IOceanData data, BuoyancyObject sender, out PosAndForce value);
    }
}
