using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenSledgehammer
{
    public class ToggleWallInteractionSystem : RestaurantSystem
    {
        EntityQuery sledgehammerQuery;
        EntityQuery replacedWallQuery;

        protected override void Initialise()
        {
            base.Initialise();
            sledgehammerQuery = GetEntityQuery(new QueryHelper().All(typeof(CSledgehammer)));
            replacedWallQuery = GetEntityQuery(typeof(CWallReplaced));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> hammerEntities = sledgehammerQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<CSledgehammer> sledgehammers = sledgehammerQuery.ToComponentDataArray<CSledgehammer>(Allocator.Temp);
            using NativeArray<Entity> wallEntities = replacedWallQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<CWallReplaced> walls = replacedWallQuery.ToComponentDataArray<CWallReplaced>(Allocator.Temp);

            for (int i = 0; i < wallEntities.Length; i++)
            {
                CWallReplaced wall = walls[i];
                if (wall.HasBeenHammered)
                    continue;

                Entity wallEntity = wallEntities[i];

                bool hammerInRange = false;
                for (int j = 0; j < hammerEntities.Length; j++)
                {
                    CSledgehammer sledgehammer = sledgehammers[j];
                    Entity hammer = hammerEntities[j];
                    if (!EntityManager.RequireComponent(hammer, out CToolInUse tool))
                        continue;

                    if (!EntityManager.RequireComponent(tool.User, out CPosition hammerPos))
                        continue;

                    if (Vector3.Distance(wall.WallPosition, hammerPos.Position) <= 2f)//TODO: tweak this more
                        hammerInRange = true;
                }

                //if (hammerInRange && EntityManager.HasComponent<CIsInactive>(wallEntity))
                //    EntityManager.RemoveComponent<CIsInactive>(wallEntity);
                //else if (!hammerInRange && !EntityManager.HasComponent<CIsInactive>(wallEntity))
                //    EntityManager.AddComponentData(wallEntity, new CIsInactive {});
                if (hammerInRange && !EntityManager.HasComponent<CIsInteractive>(wallEntity))
                    EntityManager.AddComponentData(wallEntity, new CIsInteractive { IsLowPriority = true });
                else if (!hammerInRange && EntityManager.HasComponent<CIsInteractive>(wallEntity))
                    EntityManager.RemoveComponent<CIsInteractive>(wallEntity);
                //-> need to use CIsInteractive instead of CIsInactive so that reach over works diagonally

                //TODO: find better way to do this (its important so CanReach works properly without walls interfering)
            }
        }
    }
    public class DisableWallInteractionAtNightSystem : StartOfNightSystem, IModSystem
    {
        EntityQuery replacedWallQuery;

        protected override void Initialise()
        {
            base.Initialise();
            replacedWallQuery = GetEntityQuery(typeof(CWallReplaced));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> wallEntities = replacedWallQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < wallEntities.Length; i++)
            {
                Entity wallEntity = wallEntities[i];
                //if (!EntityManager.HasComponent<CIsInactive>(wallEntity))
                //    EntityManager.AddComponentData(wallEntity, new CIsInactive { });
                if (EntityManager.HasComponent<CIsInteractive>(wallEntity))
                    EntityManager.RemoveComponent<CIsInteractive>(wallEntity);
                //-> need to use CIsInteractive instead of CIsInactive so that actually cant select during night
            }
        }
    }
}