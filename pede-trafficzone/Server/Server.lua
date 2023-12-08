RegisterNetEvent('removeBlipSERVER')
AddEventHandler('removeBlipSERVER', function()
    TriggerClientEvent('pede:removeBlip', -1)
end)
