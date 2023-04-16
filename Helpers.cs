using KitchenData;
using KitchenLib.References;
using KitchenLib.Utils;
using System;
using UnityEngine;

namespace KitchenSledgehammer
{
    public static class Helpers
    {
        public static Transform TryToRepalceWallWithHatch(Vector3 position, Transform floorplan = null)
        {
            if(floorplan == null)
                floorplan = GameObject.Find("Kitchen Floorplan(Clone)").transform;

            Transform closestWall = null;
            float closestDistance = float.MaxValue;
            foreach (Transform child in floorplan.transform)
            {
                if (child.name != "Short Wall Section(Clone)" || !child.gameObject.activeSelf)
                    continue;

                float distance = Vector3.Distance(child.position, position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWall = child;
                }
            }
            if (closestWall != null && closestDistance < 1 && closestWall.gameObject.activeSelf)
            {
                closestWall.gameObject.SetActive(false);
                GameObject.Instantiate(Refs.HatchHammered.Prefab, closestWall.position, closestWall.rotation, floorplan);
                return closestWall;
            }
            return null;
        }
    }
}