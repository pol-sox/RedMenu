name 'RedMenu'
description 'Trainer / Menu created for the RedM community, useful for lots of things.'
author 'polsox'
url 'https://github.com/tomgrobbe/redmenu/'
version 'v0.1.0'

fx_version 'cerulean'
game 'rdr3'


files {
    'MenuAPI.dll',
    'Newtonsoft.Json.dll',
}

-- Scripts
client_scripts {
    'RedMenuClient.net.dll'
}
server_scripts {
    'RedMenuServer.net.dll'
}

-- Yes this is an early build, I know.
rdr3_warning 'I acknowledge that this is a prerelease build of RedM, and I am aware my resources *will* become incompatible once RedM ships.'
