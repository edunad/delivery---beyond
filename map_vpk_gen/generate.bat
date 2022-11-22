@echo off

REM VARIABLES
set "STEAM_ROOT=D:/Program Files (x86)/Steam"
set "GAME_ROOT=%STEAM_ROOT%/steamapps/common/Source SDK Base 2013 Singleplayer"
set "root=%~dp0"

set "VMF_FOLDER=../map_mdl_gen/vpk"
set "ASSET_FOLDER=delivery_beyond"
set "RAW_ASSET_FOLDER=raw"

echo "Generating textures"
"%root%/VTFCmd/vtfcmd.exe" -folder "%root%%RAW_ASSET_FOLDER%\*.png" -output "%root%%ASSET_FOLDER%\materials\delivery_beyond" -recurse -resize -shader "LightmappedGeneric"

echo "Packing files"
CALL "%GAME_ROOT%/bin/vpk.exe" "./%ASSET_FOLDER%"

echo "Copying files"
echo F|xcopy /F /s /Y "./delivery_beyond.vpk" "%VMF_FOLDER%/delivery_beyond.vpk"
echo D|xcopy /D /s /Y "./%ASSET_FOLDER%" "%GAME_ROOT%/hl2/"