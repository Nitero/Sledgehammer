using Kitchen;
using Unity.Collections;
using Unity.Entities;

namespace KitchenSledgehammer
{
    [UpdateInGroup(typeof(DurationLocks))]
    public class LockWallDurations : RestaurantSystem
    {
        EntityQuery replacedWallQuery;

        protected override void Initialise()
        {
            base.Initialise();
            replacedWallQuery = GetEntityQuery(typeof(CTakesDuration), typeof(CWallReplaced));
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = replacedWallQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<CTakesDuration> durations = replacedWallQuery.ToComponentDataArray<CTakesDuration>(Allocator.Temp);
            using NativeArray<CWallReplaced> walls = replacedWallQuery.ToComponentDataArray<CWallReplaced>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CTakesDuration duration = durations[i];
                CWallReplaced wall = walls[i];

                if (Has<SIsNightTime>() || !wall.HammeringWasAttemptedToday)
                {
                    duration.IsLocked = true;
                    Set(entity, duration);
                }
            }
        }
    }
}