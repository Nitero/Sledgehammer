using Kitchen;
using Kitchen.Layouts;
using KitchenMods;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenSledgehammer
{
    public class ChangeWallsToHatchViaLayoutTestSystem : StartOfNightSystem, IModSystem
    {
        EntityQuery LayoutFeatures;
        EntityQuery LayoutViews;
        LayoutBlueprint Blueprint;

        protected override void Initialise()
        {
            base.Initialise();
            //LayoutFeatures = GetEntityQuery(typeof(SKitchenLayout), typeof(CLayoutFeature), typeof(CLayoutRoomTile));
            LayoutFeatures = GetEntityQuery(typeof(SLayout), typeof(CLayoutOccupant), typeof(CLayoutRoomTile), typeof(CLayoutFeature));
            LayoutViews = GetEntityQuery(typeof(CLayoutView), typeof(CLinkedView));

            Blueprint = new LayoutBlueprint();
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = LayoutFeatures.ToEntityArray(Allocator.Temp);

            using NativeArray<Entity> viewEntities = LayoutViews.ToEntityArray(Allocator.Temp);
            using NativeArray<CLayoutView> layoutViews = LayoutViews.ToComponentDataArray<CLayoutView>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                bool isUpdated = false;
                var entity = entities[i];

                Blueprint.FromEntity(base.EntityManager, entity);
                EntityContext ctx = new EntityContext(EntityManager);
                DynamicBuffer<CLayoutRoomTile> tileBuffer = ctx.GetBuffer<CLayoutRoomTile>(entity);

                foreach (KeyValuePair<LayoutPosition, Room> tile in Blueprint.Tiles)
                {
                    foreach (LayoutPosition internalWall in Blueprint.GetInternalWalls(tile.Key))
                    {
                        Debug.Log("FROM: " + tile.Key.x + ", " + tile.Key.y + " TO: " + internalWall.x + ", " + internalWall.y + " == " +
                            (tile.Key.x < internalWall.x || tile.Key.y < internalWall.y) + " && " + !Blueprint.HasFeature(tile.Key, internalWall) + " ==> " +
                            ((tile.Key.x < internalWall.x || tile.Key.y < internalWall.y) && !Blueprint.HasFeature(tile.Key, internalWall)));

                        if ((tile.Key.x < internalWall.x || tile.Key.y < internalWall.y) && !Blueprint.HasFeature(tile.Key, internalWall))//TODO: this doesnt quite get all the tiles?
                        {
                            ctx.AppendToBuffer(entity, new CLayoutFeature()
                            {
                                Tile1 = tile.Key,
                                Tile2 = internalWall,
                                Type = FeatureType.Hatch
                            });

                            for (int j = 0; j < tileBuffer.Length; j++)
                            {
                                CLayoutRoomTile testTile = tileBuffer[j];
                                Reachability reachability = testTile.Reachability;

                                //reachability[0, 0] = true;
                                //reachability[0, 1] = true;
                                //reachability[0, -1] = true;
                                //reachability[1, 0] = true;
                                //reachability[-1, 0] = true;
                                //reachability[-1, -1] = true;
                                //reachability[-1, 1] = true;
                                //reachability[1, -1] = true;
                                //reachability[-1, -1] = true;


                                //bool isHorizontal = Mathf.Approximately(tile.Key.y, internalWall.y);
                                //Reachability reachabilitySideA = default(Reachability);
                                //Reachability reachabilitySideB = default(Reachability);
                                //if (isHorizontal)
                                //{
                                //    int direction = (int)Mathf.Sign(internalWall.x - tile.Key.x);
                                //    reachabilitySideA[direction, 1] = true;
                                //    reachabilitySideA[direction, 0] = true;
                                //    reachabilitySideA[direction, -1] = true;

                                //    reachabilitySideB[direction * -1, 1] = true;
                                //    reachabilitySideB[direction * -1, 0] = true;
                                //    reachabilitySideB[direction * -1, -1] = true;
                                //}
                                //else
                                //{
                                //    int direction = (int)Mathf.Sign(internalWall.y - tile.Key.y);
                                //    reachabilitySideA[1, direction] = true;
                                //    reachabilitySideA[0, direction] = true;
                                //    reachabilitySideA[-1, direction] = true;

                                //    reachabilitySideB[1, direction * -1] = true;
                                //    reachabilitySideB[0, direction * -1] = true;
                                //    reachabilitySideB[-1, direction * -1] = true;
                                //}
                                //testTile.Reachability = reachabilitySideA | reachabilitySideB;
                                //tileBuffer[j] = testTile;


                                int radius = 2;
                                int radiusSquared = radius * radius;
                                for (int r = -radius; r <= radius; r++)
                                    for (int c = -radius; c <= radius; c++)
                                        if (r * r + c * c <= radiusSquared)//round radius
                                            reachability[r, c] = true;
                                //TODO: this works, but now doors never become walls if place something infront...
                                //      it happens even without any reachability changes, so its the thing below? happens when isUpdated = true

                                testTile.Reachability = reachability;
                                tileBuffer[j] = testTile;
                            }

                            isUpdated = true;
                        }
                    }
                }

                if (!isUpdated)
                    return;

                for (int j = 0; j < viewEntities.Length; j++)
                {
                    var viewEntity = viewEntities[j];
                    var layoutView = layoutViews[j];
                    if (layoutView.Layout != entity)
                        continue;

                    EntityManager.DestroyEntity(viewEntity);
                    Entity newViewEntity = EntityManager.CreateEntity();
                    Set(newViewEntity, new CLayoutView()
                    {
                        Layout = entity
                    });
                    Set(newViewEntity, (CPosition)Vector3.zero);
                    Set(newViewEntity, (CRequiresView)ViewType.FranchiseFloorplan);
                    //TODO: find out why the way the view is refreshed makes doors not turn into hatches
                }
            }
        }
    }
}