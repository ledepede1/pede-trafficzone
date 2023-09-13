ESX = nil

TriggerEvent('esx:getSharedObject', function(obj)
    ESX = obj
end)

RegisterServerEvent('startDrug')
AddEventHandler('startDrug', function(drugType)

    local xPlayer = ESX.GetPlayerFromId(source)
    local random = math.random(2,10)

    if drugType == "Coke" then
        xPlayer.addInventoryItem("cannabis", random)
    end
end)