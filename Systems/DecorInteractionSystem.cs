using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenSledgehammer
{
    public class DecorInteractionSystem : RestaurantSystem, IModSystem
    {
        private EntityQuery replacedWallQuery;
        private EntityQuery decorChangeEventQuery;

        protected override void Initialise()
        {
            base.Initialise();
            replacedWallQuery = GetEntityQuery(typeof(CTakesDuration), typeof(CWallReplaced));
            decorChangeEventQuery = GetEntityQuery(typeof(CChangeDecorEvent));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> wallEntities = replacedWallQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<CWallReplaced> walls = replacedWallQuery.ToComponentDataArray<CWallReplaced>(Allocator.Temp);
            using NativeArray<CChangeDecorEvent> ChangeDecorEvents = decorChangeEventQuery.ToComponentDataArray<CChangeDecorEvent>(Allocator.Temp);

            for (int i = 0; i < ChangeDecorEvents.Length; i++)
            {
                CChangeDecorEvent changeDecorEvent = ChangeDecorEvents[i];
                if (changeDecorEvent.Type != KitchenData.LayoutMaterialType.Wallpaper)
                    continue;

                for (int j = 0; j < wallEntities.Length; j++)
                {
                    Entity entity = wallEntities[j];
                    CWallReplaced wall = walls[j];

                    int newMaterial = Mod.WallpaperIdsToMaterialIds[changeDecorEvent.DecorID];
                    if (wall.RoomA == changeDecorEvent.RoomID && newMaterial != wall.MaterialA)
                    {
                        wall.MaterialA = newMaterial;
                        EntityManager.SetComponentData(entity, wall);
                    }
                    if (wall.RoomB == changeDecorEvent.RoomID && newMaterial != wall.MaterialB)
                    {
                        wall.MaterialB = newMaterial;
                        EntityManager.SetComponentData(entity, wall);
                    }
                }
            }
            //TODO: why is it running every frame after a change? should I delete the CChangeDecorEvent entity?
        }
    }
}