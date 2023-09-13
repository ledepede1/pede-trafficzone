fx_version 'adamant'

game 'gta5'

description 'Description'

version '1.0'

file'Config.ini'

lua54 'yes'
shared_script '@ox_lib/init.lua'

client_scripts {
  'Config.lua',
  'Client/Client.net.dll',
  'Client/Client.lua',
}

server_scripts {
  'Config.lua',
  'Server/Server.lua',
}