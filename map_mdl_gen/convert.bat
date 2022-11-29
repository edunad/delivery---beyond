@echo off

REM VARIABLES
set "STEAM_ROOT=D:/Program Files (x86)/Steam"
set "GAME_ROOT=%STEAM_ROOT%/steamapps/common/Source SDK Base 2013 Singleplayer"
set "root=%~dp0"

set "ASSET_FOLDER=../Assets/Models"

REM MAPS------
set models=victory.vmf

for %%a in (%models%) do (
	echo "Parsing map %%a"
	java -jar %root%/VMF2OBJ.jar %root%/vmfs/%%a -o %ASSET_FOLDER%/Generated/%%a -r "%GAME_ROOT%/hl2/hl2_misc_dir.vpk;%GAME_ROOT%/hl2/hl2_textures_dir.vpk;%GAME_ROOT%/platform/platform_misc_dir.vpk;%root%/vpk/delivery_beyond.vpk"
)