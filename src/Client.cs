
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using Config.Reader;
using CitizenFX.Core.NaturalMotion;
using System.Drawing;
using System.Xml.Linq;
using System.Net.NetworkInformation;
using System.Diagnostics.Eventing.Reader;

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
            // KPH or MPH
            int kphmph = config.GetIntValue("Notifications", "kphmph", 1);

            // Config for notifications Colors, Icons and so on
            dynamic iconSuccsess = config.GetStringValue("Notifications", "iconSuccsess", "fallback");

            dynamic iconError = config.GetStringValue("Notifications", "iconError", "fallback");

            dynamic NotificationPosistion = config.GetStringValue("Notifications", "NotificationPosistion", "fallback");


            // Config for notifications 1
            dynamic trafficTitle = config.GetStringValue("Notifications", "trafficTitle", "fallback");
            dynamic creatingTrafficZoneDescription = config.GetStringValue("Notifications", "creatingTrafficZoneDescription", "fallback");
            dynamic creatingTrafficZoneDescription2 = config.GetStringValue("Notifications", "creatingTrafficZoneDescription2", "fallback");
            // Config for notifications 2
            dynamic stoppingTrafficDescription = config.GetStringValue("Notifications", "stoppingTrafficDescription", "fallback");
            // Config for notifications 3
            dynamic only1TrafficZoneATA = config.GetStringValue("Notifications", "only1TrafficZoneATA", "fallback");
            // Config for error handling
            dynamic exceededMaxRadius = config.GetStringValue("Notifications", "exceededMaxRadius", "fallback");
            dynamic exceededMaxSpeed = config.GetStringValue("Notifications", "exceededMaxSpeed", "fallback");
            // 
            dynamic maxRadius = config.GetStringValue("Traffic", "maxRadius", "fallback");
            dynamic maxSPEED = config.GetStringValue("Traffic", "maxSPEED", "fallback");

            if (radius > Convert.ToInt32(maxRadius)) {
                TriggerEvent("notify", trafficTitle, exceededMaxRadius + " " +  maxRadius, NotificationPosistion, "#141517", "#C1C2C5", "#909296", iconError, "#880808");
            }
            if (maxSpeed > Convert.ToInt32(maxSPEED)) {
                TriggerEvent("notify", trafficTitle, exceededMaxSpeed + " " + maxSPEED, NotificationPosistion, "#141517", "#C1C2C5", "#909296", iconError, "#880808");
            }
            else {
                if (radius <= Convert.ToInt32(maxRadius)) {
                    if (maxSpeed <= Convert.ToInt32(maxSPEED)) {
                        if (zoneCreated == false) {
                            int speedzone = API.AddSpeedZoneForCoord(API.GetEntityCoords(API.PlayerPedId(), true).X, API.GetEntityCoords(API.PlayerPedId(), true).Y, API.GetEntityCoords(API.PlayerPedId(), true).Z, radius, maxSpeed / kphmph, true);
                            speedzoneD = speedzone;
                            chatmessage(maxSpeed, false);
                            if (maxSpeed != 0) {
                                TriggerEvent("notify", trafficTitle, creatingTrafficZoneDescription + " " + radius + " " + creatingTrafficZoneDescription2 + " " + maxSpeed, NotificationPosistion, "#141517", "#C1C2C5", "#909296", iconSuccsess, "#009E60");
                            }
                            else {
                                TriggerEvent("notify", trafficTitle, stoppingTrafficDescription + " " + radius, NotificationPosistion, "#141517", "#C1C2C5", "#909296", iconSuccsess, "#009E60");
                            }
                            zoneCreated = true;

                        }
                        else if (zoneCreated == true) {
                            TriggerEvent("notify", trafficTitle, only1TrafficZoneATA, NotificationPosistion, "#141517", "#C1C2C5", "#909296", iconError, "#880808");
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

            // Config for notifications Colors, Icons and so on
            dynamic trafficTitle = config.GetStringValue("Notifications", "trafficTitle", "fallback");
            dynamic iconSuccsess = config.GetStringValue("Notifications", "iconSuccsess", "fallback");

            dynamic iconError = config.GetStringValue("Notifications", "iconError", "fallback");

            dynamic NotificationPosistion = config.GetStringValue("Notifications", "NotificationPosistion", "fallback");
            // Config for notifications
            dynamic removedCurrentZone = config.GetStringValue("Notifications", "removedCurrentZone", "fallback");
            dynamic playerDontHaveCurrentZone = config.GetStringValue("Notifications", "playerDontHaveCurrentZone", "fallback");

            if (zoneCreated == true) {
                API.RemoveSpeedZone(speedzoneD);
                TriggerEvent("notify", trafficTitle, removedCurrentZone, NotificationPosistion, "#141517", "#C1C2C5", "#909296", iconSuccsess, "#009E60");
                zoneCreated = false;
                chatmessage(-1, true);
            }
            else if (zoneCreated == false) {
                TriggerEvent("notify", trafficTitle, playerDontHaveCurrentZone, NotificationPosistion, "#141517", "#C1C2C5", "#909296", iconError, "#880808");
            }
        }

        // Full stop traffic zone
        [EventHandler("createFullSTOP")]
        private void createFullStop(int radius) {
           
                createTrafficZ(radius, 0);
        }

        private void chatmessage(float speed, bool removing) {

            // Config for chat messages
            dynamic stoppedTraficChatMessage = config.GetStringValue("Notifications", "stoppedTraficChatMessage", "fallback");
            dynamic customTraficZoneChatMessage1 = config.GetStringValue("Notifications", "customTraficZoneChatMessage1", "fallback");
            dynamic customTraficZoneChatMessage2 = config.GetStringValue("Notifications", "customTraficZoneChatMessage2", "fallback");
            dynamic customTraficZoneChatMessage3 = config.GetStringValue("Notifications", "customTraficZoneChatMessage3", "fallback");
            dynamic traficZoneRemovedChatMessage1 = config.GetStringValue("Notifications", "traficZoneRemovedChatMessage1", "fallback");
            dynamic traficZoneRemovedChatMessage2 = config.GetStringValue("Notifications", "traficZoneRemovedChatMessage2", "fallback");

            dynamic policeLabelChatMessage = config.GetStringValue("Notifications", "policeLabelChatMessage", "fallback");

            var vector_p6 = Vector3.Zero;
            var streetName = new uint();
            var crossingRoad = new uint();
            var stringStreetName = API.GetStreetNameFromHashKey(streetName);

            API.GetStreetNameAtCoord(vector_p6.X, vector_p6.Y, vector_p6.Z, ref streetName, ref crossingRoad);
            stringStreetName = API.GetStreetNameFromHashKey(streetName);

            if (speed == 0 && removing == false) {
                BaseScript.Delay(1);
                TriggerEvent("chat:addMessage", new
                {
                    color = new[] { 255, 0, 0 },
                    multiline = true,
                    args = new[] { $"{policeLabelChatMessage}", stoppedTraficChatMessage + " " + stringStreetName }
                });
            }
            else if (speed > 0 && removing == false) {
                BaseScript.Delay(1);
                TriggerEvent("chat:addMessage", new
                {
                    color = new[] { 255, 0, 0 },
                    multiline = true,
                    args = new[] { $"{policeLabelChatMessage}", customTraficZoneChatMessage1 + " " + stringStreetName + " " + customTraficZoneChatMessage2 + " " + speed + " " + customTraficZoneChatMessage3 }
                });
            }
            else if (removing == true) {
                TriggerEvent("chat:addMessage", new
                {
                    color = new[] { 255, 0, 0 },
                    multiline = true,
                    args = new[] { $"{policeLabelChatMessage}", traficZoneRemovedChatMessage1 + " "  + stringStreetName + " " + traficZoneRemovedChatMessage2}
                });
            } 
        }
    }
}
