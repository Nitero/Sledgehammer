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
        public int RoomA;
        public int RoomB;
        public int MaterialA;//TODO: why cant this be a string or mat?
        public int MaterialB;
        public bool HasBeenHammered;
        public bool HammeringWasAttemptedToday;

        public CWallReplaced(Vector3 wallPosition, Vector3 sideA, Vector3 sideB, int roomA, int roomB, int materialA, int materialB, bool hasBeenHammered)
        {
            WallPosition = wallPosition;
            SideA = sideA;
            SideB = sideB;
            RoomA = roomA;
            RoomB = roomB;
            MaterialA = materialA;
            MaterialB = materialB;
            HasBeenHammered = hasBeenHammered;
            HammeringWasAttemptedToday = false;
        }
    }
}