#BetterServerConfig
This is a simple mod to make your life easier configuring your valheim server.

## Features
Lets you remove password from a dedicated server
Easy config file with all of the options at your fingertips
Extra configurable settings added to what is already accessible to you in valheim
Bash console commands that you can run without going into the game, 
full list of commands can be found using command - help
Automated backup system with configurable amount of saves

### Support
If you would like to support me you can do it here: https://streamlabs.com/ltmadness/tip

### FAQ
<b>Where to find server config?:</b>

<Valheim dedicated server>\BepInEx\config\org.ltmadness.valheim.betterserverconfig.cfg

<b>Do I need anything else to use it?</b>

Yes, you need <b>BepInEx</b>, to install simple drop the ValheimBetterServerConfig.dll into BepInEx/plugins,
Config file will be generated after first server start

<b>How to fix duplicate console lines?</b>

Go to BepInEx config file (<Valheim_dedicated_server>/BepInEx/config/BepInEx.cfg)
Find secrion named [Logging.Console] find setting called Enabled, and set it to false eg:

[Logging.Console]
Enables showing a console for log output.
Setting type: Boolean
Default value: false
Enabled = false

### Changes
##### v0.0.90
- Improvements on Console command framework
- Fix for an issue that was braking multiple commands
- Small fixes and optimisations

##### v0.0.80
- New console commands:
	* online - show list of online players and their steamID (Thanks to Energritz)
- New config option:
	* Show shout chat - when enables console displays shouts (/s messages) in console
- Small fixes and optimisations

##### v0.0.70
- New command system
- Fix for Shutdown command
- Fix for visible setting

##### v0.0.60
- New console commands:
	* say - to say something to players in server as server
	* yell - to shout something at players in server as server
	* config - displays all of the information you have set in config as a list
- New config option:
	* Server Username - the name thats displayed for a server when say/yell command used
- Fix for shutdown command
- Small fixes and optimisations

##### v0.0.55
- Hot fix for change done in Valheim 0.147.3 update

##### v0.0.50
- Added configurable backup system that puts set amount of backups(1 per save) into folder with your world name
- Added new command for server bash console:
	* sleep - fast forward to next morning
- Small fixes and optimisations

##### v0.0.40
- New console commands:
	* unpermit [ip/userID] - remove user from permitted user list
	* addAdmin [userID] - add user to admin list
	* removeAdmin [userID] - remove user from admin list
	* admins - list of admin user ids
	* save - save server
	* shutdown - shutdown the server
	* difficulty [nr] - force difficulty
- Small fixes
