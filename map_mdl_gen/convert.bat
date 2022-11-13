@echo off

Rem VARIABLES
set "STEAM_ROOT=D:/Program Files (x86)/Steam"
set "GAME_ROOT=%STEAM_ROOT%/steamapps/common/Source SDK Base 2013 Singleplayer"
set "root=%~dp0"

set "ASSET_FOLDER=../Assets/Models"

set maps=office.vmf
rem set maps=%list%;office.vmf

Rem SETUP
rem RMDIR "%ASSET_FOLDER%/Generated" /S /Q
rem mkdir "%ASSET_FOLDER%/Generated"

for %%a in (%maps%) do (
	java -jar %root%/VMF2OBJ.jar %root%/vmfs/%%a -o %ASSET_FOLDER%/Generated/%%a -r "%GAME_ROOT%/hl2/hl2_misc_dir.vpk;%GAME_ROOT%/hl2/hl2_textures_dir.vpk;%GAME_ROOT%/platform/platform_misc_dir.vpk"
)