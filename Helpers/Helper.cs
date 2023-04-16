using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenSledgehammer
{
    public static class Helper
    {
        internal static T Find<T>(int id) where T : GameDataObject
        {
            return (T)GDOUtils.GetExistingGDO(id) ?? (T)GDOUtils.GetCustomGameDataObject(id)?.GameDataObject;
        }

        internal static T Find<T, C>() where T : GameDataObject where C : CustomGameDataObject
        {
            return GDOUtils.GetCastedGDO<T, C>();
        }

        internal static T Find<T>(string modName, string name) where T : GameDataObject
        {
            return GDOUtils.GetCastedGDO<T>(modName, name);
        }

        // Prefab / GameObject
        public static GameObject GetPrefab(string name)
        {
            return Mod.Bundle.LoadAsset<GameObject>(name);
        }
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }
        public static T TryAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T comp = gameObject.GetComponent<T>();
            if (comp == null)
                comp = gameObject.AddComponent<T>();
            return comp;
        }
        public static GameObject GetChild(this GameObject gameObject, string childName)
        {
            return gameObject.transform.Find(childName).gameObject;
        }
        public static GameObject GetChild(this GameObject gameObject, int childIndex)
        {
            return gameObject.transform.GetChild(childIndex).gameObject;
        }
        public static GameObject GetChildFromPath(this GameObject gameObject, string childPath)
        {
            return GameObjectUtils.GetChildObject(gameObject, childPath);
        }
        public static int GetChildCount(this GameObject gameObject)
        {
            return gameObject.transform.childCount;
        }
        public static List<GameObject> GetChildren(this GameObject gameObject)
        {
            var children = new List<GameObject>();
            for (var i = 0; i < gameObject.transform.childCount; i++)
                children.Add(gameObject.GetChild(i));

            return children;
        }

        // GDO
        public static T GetGDO<T>(int id) where T : GameDataObject
        {
            return GDOUtils.GetExistingGDO(id) as T;
        }

        // Provider Util
        internal static List<Appliance.ApplianceProcesses> CreateCounterProcesses()
        {
            return new List<Appliance.ApplianceProcesses>()
            {
                new Appliance.ApplianceProcesses()
                {
                    Process = GetGDO<Process>(ProcessReferences.Chop),
                    Speed = 0.75f,
                    IsAutomatic = false,
                    Validity = ProcessValidity.Generic
                },
                new Appliance.ApplianceProcesses()
                {
                    Process = GetGDO<Process>(ProcessReferences.Knead),
                    Speed = 0.75f,
                    IsAutomatic = false,
                    Validity = ProcessValidity.Generic
                },
            };
        }
        internal static void SetupCounter(GameObject prefab)
        {
            GameObject parent = prefab.GetChildFromPath("Block/Counter2");
            var paintedWood = MaterialHelper.GetMaterialArray("Wood 4 - Painted");
            var defaultWood = MaterialHelper.GetMaterialArray("Wood - Default");
            parent.ApplyMaterialToChild("Counter", paintedWood);
            parent.ApplyMaterialToChild("Counter Doors", paintedWood);
            parent.ApplyMaterialToChild("Counter Surface", defaultWood);
            parent.ApplyMaterialToChild("Counter Top", defaultWood);
            parent.ApplyMaterialToChild("Handles", "Knob");
        }
        internal static void SetupThinCounter(GameObject prefab)
        {
            GameObject parent = prefab.GetChildFromPath("Thin Block/ThinCounter");
            var paintedWood = "Wood 4 - Painted";
            var defaultWood = "Wood - Default";
            parent.ApplyMaterialToChild("Base", paintedWood, paintedWood, paintedWood);
            parent.ApplyMaterialToChild("Top", defaultWood);
            parent.GetChild("Base").ApplyMaterialToChild("Handle", "Knob");
        }
        internal static void SetupCounterLimitedItem(GameObject counterPrefab, GameObject itemPrefab)
        {
            Transform holdTransform = GameObjectUtils.GetChildObject(counterPrefab, "Block/HoldPoint").transform;

            counterPrefab.TryAddComponent<HoldPointContainer>().HoldPoint = holdTransform;

            var sourceView = counterPrefab.TryAddComponent<LimitedItemSourceView>();
            sourceView.HeldItemPosition = holdTransform;
            ReflectionUtils.GetField<LimitedItemSourceView>("Items").SetValue(sourceView, new List<GameObject>()
            {
                GameObjectUtils.GetChildObject(counterPrefab, $"Block/HoldPoint/{itemPrefab.name}")
            });
        }
        internal static void SetupThinCounterLimitedItem(GameObject counterPrefab, GameObject itemPrefab, bool hasHeldItemPosition)
        {
            Transform holdTransform = GameObjectUtils.GetChildObject(counterPrefab, "GameObject").transform;

            counterPrefab.TryAddComponent<HoldPointContainer>().HoldPoint = holdTransform;

            var sourceView = counterPrefab.TryAddComponent<LimitedItemSourceView>();

            if (hasHeldItemPosition)
            {
                sourceView.HeldItemPosition = holdTransform;
            }

            ReflectionUtils.GetField<LimitedItemSourceView>("Items").SetValue(sourceView, new List<GameObject>()
            {
                GameObjectUtils.GetChildObject(counterPrefab, $"GameObject/{itemPrefab.name}")
            });
        }
        internal static void SetupTable(GameObject prefab)
        {
            Transform holdTransform = GameObjectUtils.GetChildObject(prefab, "HoldPoint").transform;

            //Transform tableDecoTransform = GameObjectUtils.GetChildObject(prefab, "Table Decoration Attachment").transform;

            prefab.TryAddComponent<HoldPointContainer>().HoldPoint = holdTransform;

            var attachmentView = prefab.GetChild("Table Decoration Attachment").TryAddComponent<AttachmentView>();

            var varStorageView = prefab.GetChild("Table Dirt Attachment").TryAddComponent<ItemVariableStorageView>();

            varStorageView.HeldItemPosition = holdTransform;


            // Table Consumables
            var decoAttachment = "Table Decoration Attachment";
            var sharpCutlery = $"{decoAttachment}/Sharp Cutlery";

            ReflectionUtils.GetField<AttachmentView>("EffectLookups").SetValue(attachmentView, new List<AttachmentView.EffectLookup>()
            {
                new AttachmentView.EffectLookup()
                {
                    Active = GameObjectUtils.GetChildObject(prefab, $"{decoAttachment}/Breadsticks Active"),
                    Effect = (Effect)GDOUtils.GetExistingGDO(EffectReferences.Breadsticks),
                    Inactive = GameObjectUtils.GetChildObject(prefab, $"{decoAttachment}/Breadsticks")
                },
                new AttachmentView.EffectLookup()
                {
                    Active = GameObjectUtils.GetChildObject(prefab, $"{decoAttachment}/Lit Candle"),
                    Effect = (Effect)GDOUtils.GetExistingGDO(EffectReferences.Candles),
                    Inactive = GameObjectUtils.GetChildObject(prefab, $"{decoAttachment}/Unlit Candle")
                },
                new AttachmentView.EffectLookup()
                {
                    Active = GameObjectUtils.GetChildObject(prefab, $"{decoAttachment}/Napkin Active"),
                    Effect = (Effect)GDOUtils.GetExistingGDO(EffectReferences.Napkins),
                    Inactive = GameObjectUtils.GetChildObject(prefab, $"{decoAttachment}/Napkin")
                },
                new AttachmentView.EffectLookup()
                {
                    Active = GameObjectUtils.GetChildObject(prefab, $"{decoAttachment}/Sharp Cutlery"),
                    Effect = (Effect)GDOUtils.GetExistingGDO(EffectReferences.SharpCutlery)
                }
            });

            ReflectionUtils.GetField<AttachmentView>("OrientedObjects").SetValue(attachmentView, new List<AttachmentView.OrientedObject>()
            {
                new AttachmentView.OrientedObject()
                {
                    Active = GameObjectUtils.GetChildObject(prefab, $"{sharpCutlery}/Sharp Cutlery - Layout"),
                    Orientation = Orientation.Up
                },
                new AttachmentView.OrientedObject()
                {
                    Active = GameObjectUtils.GetChildObject(prefab, $"{sharpCutlery}/Sharp Cutlery - Layout (1)"),
                    Orientation = Orientation.Right
                },
                new AttachmentView.OrientedObject()
                {
                    Active = GameObjectUtils.GetChildObject(prefab, $"{sharpCutlery}/Sharp Cutlery - Layout (2)"),
                    Orientation = Orientation.Down
                },
                new AttachmentView.OrientedObject()
                {
                    Active = GameObjectUtils.GetChildObject(prefab, $"{sharpCutlery}/Sharp Cutlery - Layout (3)"),
                    Orientation = Orientation.Left
                }
            });

            // Dirty Plates
            var plates = "Table Dirt Attachment/Dirty Plates/Storage 1";

            ReflectionUtils.GetField<ItemVariableStorageView>("MoveHeldItemPosition").SetValue(varStorageView, true);
            ReflectionUtils.GetField<ItemVariableStorageView>("Storage").SetValue(varStorageView, new List<GameObject>()
            {
                GameObjectUtils.GetChildObject(prefab, $"{plates}"),
                GameObjectUtils.GetChildObject(prefab, $"{plates} (1)"),
                GameObjectUtils.GetChildObject(prefab, $"{plates} (2)"),
                GameObjectUtils.GetChildObject(prefab, $"{plates} (3)"),
                GameObjectUtils.GetChildObject(prefab, $"{plates} (4)"),
                GameObjectUtils.GetChildObject(prefab, $"{plates} (5)")
            });


            // Materials
            GameObject parent = prefab.GetChildFromPath($"{decoAttachment}");

            // Candle
            var candle = MaterialHelper.GetMaterialArray("Wax", "Burned");
            parent.ApplyMaterialToChild("Lit Candle/Candle", candle);
            parent.ApplyMaterialToChild("Unlit Candle/Candle", candle);

            // Breadsticks
            parent.ApplyMaterialToChild("Breadsticks/Breadsticks/Cylinder", "Paint - Red");
            parent.ApplyMaterialToChild("Breadsticks/Breadsticks/Cylinder.002", "Bread");

            parent.ApplyMaterialToChild("Breadsticks Active/Breadsticks/Cylinder", "Paint - Red");
            parent.ApplyMaterialToChild("Breadsticks Active/Breadsticks/Cylinder.002", "Bread");

            // Candle
            var napkin = MaterialHelper.GetMaterialArray("Cloth - Fancy", "Cloth - Fancy");
            parent.ApplyMaterialToChild("Napkin/Napkin", napkin);
            parent.ApplyMaterialToChild("Napkin Active/Napkin", napkin);

            // Sharp Cutlery
            var cutlery = "Sharp Cutlery/Sharp Cutlery - Layout";
            var metalShiny = MaterialHelper.GetMaterialArray("Metal- Shiny");

            parent.GetChild($"{cutlery}/SharpCutlery").ApplyMaterialToChildren("Fork", metalShiny);
            parent.GetChild($"{cutlery}/SharpCutlery").ApplyMaterialToChildren("Spoon", metalShiny);
            parent.GetChild($"{cutlery}/SharpCutlery").ApplyMaterialToChildren("Knife", metalShiny);

            parent.GetChild($"{cutlery} (1)/SharpCutlery").ApplyMaterialToChildren("Fork", metalShiny);
            parent.GetChild($"{cutlery} (1)/SharpCutlery").ApplyMaterialToChildren("Spoon", metalShiny);
            parent.GetChild($"{cutlery} (1)/SharpCutlery").ApplyMaterialToChildren("Knife", metalShiny);

            parent.GetChild($"{cutlery} (2)/SharpCutlery").ApplyMaterialToChildren("Fork", metalShiny);
            parent.GetChild($"{cutlery} (2)/SharpCutlery").ApplyMaterialToChildren("Spoon", metalShiny);
            parent.GetChild($"{cutlery} (2)/SharpCutlery").ApplyMaterialToChildren("Knife", metalShiny);

            parent.GetChild($"{cutlery} (3)/SharpCutlery").ApplyMaterialToChildren("Fork", metalShiny);
            parent.GetChild($"{cutlery} (3)/SharpCutlery").ApplyMaterialToChildren("Spoon", metalShiny);
            parent.GetChild($"{cutlery} (3)/SharpCutlery").ApplyMaterialToChildren("Knife", metalShiny);
        }
        internal static void SetupGenericCrates(GameObject prefab)
        {
            prefab.GetChild("GenericStorage").ApplyMaterialToChildren("Cube", "Wood - Default");
        }
        /*internal static void SetupFridge(GameObject prefab)
        {
            GameObject fridge = prefab.GetChild("Fridge");
            GameObject fridge2 = fridge.GetChild("Fridge2");

            prefab.TryAddComponent<ItemHolderView>();
            fridge.TryAddComponent<ItemHolderView>();

            var sourceView = fridge.TryAddComponent<ItemSourceView>();
            var quad = fridge.GetChild("Quad").GetComponent<MeshRenderer>();
            quad.materials = MaterialHelper.GetMaterialArray("Flat Image");
            ReflectionUtils.GetField<ItemSourceView>("Renderer").SetValue(sourceView, quad);
            ReflectionUtils.GetField<ItemSourceView>("Animator").SetValue(sourceView, fridge2.GetComponent<Animator>());

            var soundSource = fridge2.TryAddComponent<AnimationSoundSource>();
            soundSource.SoundList = new List<AudioClip>() { Mod.Bundle.LoadAsset<AudioClip>("Fridge_mixdown") };
            soundSource.Category = SoundCategory.Effects;
            soundSource.ShouldLoop = false;

            // Fridge Materials
            fridge2.ApplyMaterialToChild("Body", "Metal- Shiny", "Metal- Shiny", "Metal- Shiny");
            fridge2.ApplyMaterialToChild("Door", "Metal- Shiny", "Metal Dark", "Door Glass");
            fridge2.ApplyMaterialToChild("Divider", "Plastic - Dark Grey");
            fridge2.ApplyMaterialToChild("Wire", "Plastic - Blue");
        }
        internal static void SetupLocker(GameObject prefab)
        {
            // Components
            var lockerPrefab = prefab.GetChild("Locker");
            var lockerModel = lockerPrefab.GetChild("Locker");

            prefab.TryAddComponent<ItemHolderView>();
            lockerPrefab.TryAddComponent<ItemHolderView>();

            var sourceView = lockerPrefab.TryAddComponent<ItemSourceView>();
            var quad = lockerPrefab.GetChild("Quad").GetComponent<MeshRenderer>();
            quad.materials = MaterialHelper.GetMaterialArray("Flat Image");
            ReflectionUtils.GetField<ItemSourceView>("Renderer").SetValue(sourceView, quad);
            ReflectionUtils.GetField<ItemSourceView>("Animator").SetValue(sourceView, lockerModel.GetComponent<Animator>());

            var soundSource = lockerModel.TryAddComponent<AnimationSoundSource>();
            soundSource.SoundList = new List<AudioClip>() { Mod.Bundle.LoadAsset<AudioClip>("Fridge_mixdown") };
            soundSource.Category = SoundCategory.Effects;
            soundSource.ShouldLoop = false;

            // Models
            lockerModel.ApplyMaterialToChild("Body", "Metal- Shiny", "Metal- Shiny", "Metal- Shiny", "Plastic - Red", "Plastic - Blue");
            lockerModel.ApplyMaterialToChild("Door", "Metal- Shiny", "Door Glass", "Metal Dark");
        }*/
        internal static void SetupStand(GameObject prefab)
        {
            // Component
            var holdPoint = prefab.TryAddComponent<HoldPointContainer>();
            holdPoint.HoldPoint = prefab.transform.Find("HoldPoint");

            // Model
            var stand = prefab.GetChildFromPath("Stand/Stand");
            stand.ApplyMaterialToChild("Body", "Wood 4 - Painted");
            stand.ApplyMaterialToChild("Doors", "Wood 4 - Painted");
            stand.ApplyMaterialToChild("Handles", "Metal - Brass");
            stand.ApplyMaterialToChild("Sides", "Wood - Default");
            stand.ApplyMaterialToChild("Top", "Wood - Default");
        }
    }
}