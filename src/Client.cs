
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using Config.Reader;
using CitizenFX.Core.NaturalMotion;
using System.Drawing;
using System.Xml.Linq;

namespace Client {
    public class Main : BaseScript {
        iniconfig config = new iniconfig("pede-trafficzone", "Config.ini");
        public int speedzoneD { get; set; }
        public bool zoneCreated = false;


        public Main() {
            // Config for commands and keybinds
            dynamic commandname = config.GetStringValue("CommandsKeybinds", "commandname", "fallback");
            dynamic keybindDescription = config.GetStringValue("CommandsKeybinds", "keybindDescription", "fallback");

            API.RegisterKeyMapping(commandname, keybindDescription, "keyboard", "k");
            API.RegisterCommand(commandname, new Action<int, List<object>, string>(async (source, args, rawCommand) =>
            {
                await Delay(1);
                TriggerEvent("checkUserJob");
            }), false);
        }
        [EventHandler("hasJOB")]
        public void hasJob() {
            TriggerEvent("trafficzone:openMenu");
        }
        public void createTrafficZ(int radius, float maxSpeed) {
            // Config for notifications
            dynamic creatingTrafficZoneTitle = config.GetStringValue("Notifications", "creatingTrafficZoneTitle", "fallback");
            dynamic creatingTrafficZoneDescription = config.GetStringValue("Notifications", "creatingTrafficZoneDescription", "fallback");
            dynamic creatingTrafficZoneDescription2 = config.GetStringValue("Notifications", "creatingTrafficZoneDescription2", "fallback");
            // 
            dynamic maxRadius = config.GetStringValue("Traffic", "maxRadius", "fallback");
            dynamic maxSPEED = config.GetStringValue("Traffic", "maxSPEED", "fallback");

            if (radius > Convert.ToInt32(maxRadius)) {
                TriggerEvent("notify", "Traffic Manager", "Your radius can only be " + maxRadius, "top-left", "#141517", "#C1C2C5", "#909296", "circle-xmark", "#880808");
            }
            if (maxSpeed > Convert.ToInt32(maxSPEED)) {
                TriggerEvent("notify", "Traffic Manager", "Your Speed can only be " + maxSPEED, "top-left", "#141517", "#C1C2C5", "#909296", "circle-xmark", "#880808");
            }
            else {
                if (radius <= Convert.ToInt32(maxRadius)) {
                    if (maxSpeed <= Convert.ToInt32(maxSPEED)) {
                        if (zoneCreated == false) {
                            int speedzone = API.AddSpeedZoneForCoord(API.GetEntityCoords(API.PlayerPedId(), true).X, API.GetEntityCoords(API.PlayerPedId(), true).Y, API.GetEntityCoords(API.PlayerPedId(), true).Z, radius, maxSpeed / 3.6F, true);
                            speedzoneD = speedzone;
                            if (maxSpeed != 0) {
                                TriggerEvent("notify", creatingTrafficZoneTitle, creatingTrafficZoneDescription + " " + radius + " " + creatingTrafficZoneDescription2 + " " + maxSpeed, "top-left", "#141517", "#C1C2C5", "#909296", "circle-check", "#009E60");
                            }
                            else {
                                TriggerEvent("notify", "Traffic Manager", "Stopper alt trafik i en radius af: " + radius, "top-left", "#141517", "#C1C2C5", "#909296", "circle-check", "#009E60");
                            }
                            zoneCreated = true;

                        }
                        else if (zoneCreated == true) {
                            TriggerEvent("notify", "Traffic Manager", "Can only have 1 traffic zone at a time!", "top-left", "#141517", "#C1C2C5", "#909296", "circle-xmark", "#880808");
                        }
                    }
                }
            }
        }

        // Making custom Traffic Zone
        [EventHandler("trafficZoneCUSTOM")]
        private void trafficZone(int radius, int maxspeed) {
            createTrafficZ(radius, maxspeed);
        }


        // Dont change its for npcs not driving backwards!
        [EventHandler("trafficZoneLess4")]
        private void trafficZoneL4(int radius) {
            createTrafficZ(radius, 4);
        }

        // Clear current Traffic Zone & if user does not have any zone it will alert them
        [EventHandler("clearTrafficZONE")]
        private void clearTrafficzone() {
            if (zoneCreated == true) {
                API.RemoveSpeedZone(speedzoneD);
                TriggerEvent("notify", "Traffic Manager", "Removed current traffic zone", "top-left", "#141517", "#C1C2C5", "#909296", "circle-xmark", "#880808");
                zoneCreated = false;
            }
            else if (zoneCreated == false) {
                TriggerEvent("notify", "Traffic Manager", "You have no trafic zone right now!", "top-left", "#141517", "#C1C2C5", "#909296", "circle-xmark", "#880808");
            }
        }

        // Full stop traffic zone
        [EventHandler("createFullSTOP")]
        private void createFullStop(int radius) {
           
                createTrafficZ(radius, 0);
        }

    }
}
