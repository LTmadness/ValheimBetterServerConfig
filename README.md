# BetterServerConfig
This is a simple mod to make your life easier configuring your valheim server.

## Features
Lets you remove password from a dedicated server
Easy config file with all of the options at your fingertips
Extra configurable settings added to what is already accessible to you in valheim
Bash console commands that you can run without going into the game, 
full list of commands can be found using command - help
Automated backup system with configurable amount of saves

### Support
If you would like to support me you can do it here: https://www.patreon.com/LTmadness

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
##### v0.1.2
- New ability to run commans by typing it in a chat and adding '/' before it e.g. /sleep
- New permision tear - moderator
- New console commands:
	* addMod - add user to moderator list
	* removeMode - remove user from moderator list
	* mods - print list of moderator steam ids in console
	* modCommands - list of commands available to moderators
- New config options:
	* Show Chat - See in game chat on server console
	* Commands Allowed for Mods - List of commands allowed to use by moderators using in game chat

##### v0.1.1
- New console commands:
	* tps - shows server updates/sec
	* version - show server and better server config version
- Small fixes
	
##### v0.1.0
- New console commands:
	* ip - shows server IP in the console
	* updateLists - force update admin/permitted/banned lists from files
- New Config option:
	* Announce saves - announces when the server being saved
	* Game Description - allows you to choose what you want to be written under Game tab in Steam Server Browser
- Small fixes and optimizations

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
