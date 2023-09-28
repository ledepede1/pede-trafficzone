  if Config.Framework == "Standalone" then
    print("Initiated to Standalone")

  else if Config.Framework == "QBCORE" then
    QBCore = exports['qb-core']:GetCoreObject()

  elseif Config.Framework == "ESX" then
    ESX = exports["es_extended"]:getSharedObject()
    while ESX.GetPlayerData().job == nil do
      Citizen.Wait(10)
    end

    ESX.PlayerData = ESX.GetPlayerData()
  else
    print("ERROR: FRAMEWORK CONFIG!")
  end
end



local PlayerJob = {}

local LoggedIn = false

RegisterNetEvent('QBCore:Client:OnPlayerLoaded')
AddEventHandler('QBCore:Client:OnPlayerLoaded', function()
    Citizen.SetTimeout(1000, function()
     PlayerJob = QBCore.Functions.GetPlayerData().job
     LoggedIn = true
    end)
end)

RegisterNetEvent('QBCore:Client:OnPlayerUnload')
AddEventHandler('QBCore:Client:OnPlayerUnload', function()
    LoggedIn = false
end)

RegisterNetEvent('QBCore:Client:OnJobUpdate')
AddEventHandler('QBCore:Client:OnJobUpdate', function(JobInfo)
    PlayerJob = JobInfo
end)

RegisterNetEvent('esx:playerLoaded')
AddEventHandler('esx:playerLoaded', function(xPlayer)
	ESX.PlayerData = xPlayer
end)

RegisterNetEvent('esx:setJob')
AddEventHandler('esx:setJob', function(job)
	ESX.PlayerData.job = job
end)


RegisterNetEvent("trafficzone:openMenu")
AddEventHandler("trafficzone:openMenu", function()
Citizen.Wait(1)
    lib.showContext('traffic_zone_menu')
end)

RegisterNetEvent("checkUserJob")
AddEventHandler('checkUserJob', function()
if Config.Framework == "QBCORE" then
QBCore.Functions.GetPlayerData(function(PlayerData)
    Citizen.Wait(0)
    if PlayerData.job.name == Config.RequiredJob then
            TriggerEvent("hasJOB")
        else
            Citizen.Wait(0)
    end
  end)
else if Config.Framework == "ESX" then
  Citizen.Wait(0)
    if ESX.PlayerData.job.name == Config.RequiredJob then
      Citizen.Wait(0)
            TriggerEvent("hasJOB")
        else
            Citizen.Wait(0)
          end
        else if Config.Framework == "Standalone" then 
          Citizen.Wait(0)
          TriggerEvent("hasJOB")
        else
          Citizen.Wait(0)
        end
        end
      end
    end)

RegisterNetEvent('pede:removeBlip')
AddEventHandler('pede:removeBlip', function()
        RemoveBlip(blip)
end)

lib.registerContext({
    id = 'traffic_zone_menu',
    title = Config.MenuTitle,
    options = {
      {
        title = Config.StopTrafficTitle,
        description = 'Stop all traffic at an inputted radius.',
        icon = Config.StopTrafficIcon,
        onSelect = function()
            local input = lib.inputDialog(Config.StopTrafficRadiusDialogTitle, {
                {type = 'number', label = Config.StopTrafficRadiusText, description = Config.StopTrafficRadiusTextDescription, required = true, icon = Config.StopTrafficRadiusTextIcon},
              })     
              TriggerEvent("createFullSTOP", input[1]);
              blip = AddBlipForRadius(GetEntityCoords(GetPlayerPed(-1)),40.0)
              SetBlipAlpha(blip, 128)
              SetBlipColour(blip, 1)
        end,
      },
      {
        title = Config.CustomTrafficZoneTitle,
        description = Config.CustomTrafficZoneDescription,
        icon = Config.CustomTrafficZoneIcon,
        onSelect = function()
            local input = lib.inputDialog(Config.CustomTrafficZoneDialogTitle, {
                {type = 'number', label = Config.CustomTrafficZoneRadiusText, description = Config.CustomTrafficZoneRadiusTextDescription, required = true, icon = Config.CustomTrafficZoneRadiusTextIcon},
                {type = 'number', label = Config.CustomTrafficZoneMaxSpeedText, description = Config.CustomTrafficZoneMaxSpeedTextDescription, required = true, icon = Config.CustomTrafficZoneMaxSpeedTextIcon},
              })
            if input[2] == 0 then
              TriggerEvent("trafficZoneCUSTOM", input[1], input[2]);
              blip = AddBlipForRadius(GetEntityCoords(GetPlayerPed(-1)),40.0)
              SetBlipAlpha(blip, 128)
              SetBlipColour(blip, 1)
            else if input[2] >= 4 then
              TriggerEvent("trafficZoneCUSTOM", input[1], input[2]);
              blip = AddBlipForRadius(GetEntityCoords(GetPlayerPed(-1)),40.0)
              SetBlipAlpha(blip, 128)
              SetBlipColour(blip, 1)
              else if input[2] < 4 then
                -- Dont change when under 4 the npcs will start to drive backwards it pretty anoying!
                TriggerEvent("trafficZoneLess4", input[1])
                blip = AddBlipForRadius(GetEntityCoords(GetPlayerPed(-1)),40.0)
                SetBlipAlpha(blip, 128)
                SetBlipColour(blip, 1)
                end
              end
            end
        end,
      },
      {
        title = Config.RemoveZoneTitle,
        description = Config.RemoveZoneDescription,
        icon = Config.RemoveZoneIcon,
        onSelect = function()
            TriggerEvent("clearTrafficZONE");
            TriggerServerEvent("removeBlipSERVER");
        end,
      },
    }
  })


RegisterNetEvent("notify")
AddEventHandler('notify', function(title, description, position, backgroundColor, fontColor, descriptionColor, icon, iconColor)
  lib.notify({
    title = title,
    description = description,
    position = position,
    style = {
        backgroundColor = backgroundColor,
        color = fontColor,
        ['.description'] = {
          color = descriptionColor
        }
    },
    icon = icon,
    iconColor = iconColor
})
end)

