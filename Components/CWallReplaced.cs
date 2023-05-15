using KitchenMods;
using UnityEngine;

namespace KitchenSledgehammer
{
    //public struct CWallReplaced : IComponentData, IApplianceProperty, IAttachableProperty //is there a difference?
    public struct CWallReplaced : IModComponent
    {
        public Vector3 WallPosition;
        public Vector3 SideA;
        public Vector3 SideB;
        public bool HasBeenHammered;
        public bool HammeringWasAttemptedToday;

        public CWallReplaced(Vector3 wallPosition, Vector3 sideA, Vector3 sideB, bool hasBeenHammered)
        {
            WallPosition = wallPosition;
            SideA = sideA;
            SideB = sideB;
            HasBeenHammered = hasBeenHammered;
            HammeringWasAttemptedToday = false;
        }
    }
}