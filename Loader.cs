using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Winch.Core;

namespace TrawlTIR
{
    public class Loader
    {
        public static void Initialize()
        {
            WinchCore.Log.Debug("TrawlTIR Initializing");

            ApplicationEvents.Instance.OnGameLoaded += () =>
            {
                WinchCore.Log.Debug("TrawlTIR OnGameLoaded fired");
                try
                {
                    CreateTIRHarvestZones();
                }
                catch (Exception e)
                {
                    WinchCore.Log.Error("TrawlTIR encountered an error:");
                    WinchCore.Log.Error(e);
                }
            }; ;
            WinchCore.Log.Debug("TrawlTIR fn attached");
        }

        public static void CreateTIRHarvestZones()
        {
            if (!GameManager.Instance.EntitlementManager.GetHasEntitlement(Entitlement.DLC_2))
            {
                WinchCore.Log.Debug("TIR not owned, aborting CreateTIRHarvestZones");
                return;
            }

            WinchCore.Log.Debug("Creating TIR HarvestZones");
            try
            {
                Transform root = GameObject.Find("FullZones").transform;

                var ironRigIds = new[] {
                    "abyssal-gar", "arapaima", "boreaspis", "dunkleosteus", "eagle-shark",
                    "giant-dragonfish", "kerygmachela", "lancetfish", "mahi-mahi", "nautilus",
                    "opabinia", "osteostracan", "paddlefish", "sawfish", "sollasina", "swordfish",
                    "tripod-spiderfish", "tullimonstrum", "vetulicolia", "xiphactinus"
                };

                var baseHZs = new List<HarvestZone>();

                void addHzComponent(Transform obj) => baseHZs.Add(obj.GetComponent<HarvestZone>());

                foreach (Transform child in root) addHzComponent(child);
                foreach (Transform child in GameObject.Find("DemoZones").transform) addHzComponent(child);

                foreach (string id in ironRigIds)
                {
                    FishItemData fishData = GameManager.Instance.ItemManager.GetItemDataById<FishItemData>(id);
                    fishData.canBeCaughtByNet = true;

                    var fishZone = fishData.zonesFoundIn;

                    var matchingSourceHZ = baseHZs.First(z => Equals(z.harvestableItems[0].zonesFoundIn, fishZone));
                    var cloneGameObject = UnityEngine.Object.Instantiate(matchingSourceHZ.gameObject, root, false);
                    cloneGameObject.name = id;
                    var cloneHZ = cloneGameObject.GetComponent<HarvestZone>();

                    var arr = (HarvestableItemData[])cloneHZ.harvestableItems.Clone();
                    arr.SetValue(fishData, 0);
                    cloneHZ.harvestableItems = arr;

                    WinchCore.Log.Debug("Created " + id + " from " + matchingSourceHZ.gameObject.name + " (" + fishZone + ")");
                }
            }
            catch (Exception e)
            {
                WinchCore.Log.Error("TrawlTIR encountered an error:");
                WinchCore.Log.Error(e);
            }
        }
    }
}
