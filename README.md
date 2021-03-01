# ValheimBetterServerConfig
This is a simple mod to make your life easier configuring your valheim server.

## Features
Lets you remove password from a dedicated server
Easy config file with all of the options at your fingertips
Extra configurable settings added to what is already accessible to you in valheim
Bash console commands that you can run without going into the game, 
full list of commands can be found using command - help
Automated backup system with configurable amount of saves

## Skill/Experience
I usually code in Java so please don't judge me too much for my c# skills :D also, this is my first mod ever :D

### Support
If you would like to support me you can do it here: https://streamlabs.com/ltmadness/tip

### FAQ
<b>Where to find server config?:</b>

<Valheim dedicated server>\BepInEx\config\org.ltmadness.valheim.betterserverconfig.cfg

<b>Do I need anything else to use it?</b>

Yes, you need <b>BepInEx</b>, to install simple drop the ValheimBetterServerConfig.dll into BepInEx/plugins,
Config file will be generated after first server start

### Changes
#### v0.0.50
- Added configurable backup system that puts set amount of backups(1 per save) into folder with your world name
- Added new command for server bash console:
	* sleep - fast foward to next morning
- Code cleanup/fixes

##### v0.0.40
- Added even more commands to bash console:
	* unpermit [ip/userID] - remove user from permitted user list
	* addAdmin [userID] - add user to admin list
	* removeAdmin [userID] - remove user from admin list
	* admins - list of admin user ids
	* save - save server
	* shutdown - shutdown the server
	* difficulty [nr] - force difficulty
- Small fixes

##### v0.0.30
- Added console commands that can be run from server side using bash console:
	* kick [name/ip/userID] - kick user
	* ban [name/ip/userID] - ban user
	* unban [ip/userID] - unban user
	* banned - list banned users
	* permit [ip/userID] - add user to permitted user list
	* permitted - list permitted users
- Small fixes

##### v0.0.2
- Steam lobby server size option
- Easy server name color change by choosing color in config (any color form: https://www.w3schools.com/colors/colors_names.asp)
- Easy way to set serve name to be displayed in italic
- Easy way to set server name to be displayed in bold
- Option to change steam map name to whatever you like separate from your world name
- Small fixes

##### v0.0.1 
- Can change server name from mod config
- Wold name and server port option included in mod config
- Option to choose where server is saved
