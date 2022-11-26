@echo off

REM VARIABLES
set "STEAM_ROOT=D:/Program Files (x86)/Steam"
set "GAME_ROOT=%STEAM_ROOT%/steamapps/common/Source SDK Base 2013 Singleplayer"
set "root=%~dp0"

set "ASSET_FOLDER=../Assets/Models"

REM MAPS------
set models=office.vmf
set models=%models%;office_props.vmf
set models=%models%;office_collision.vmf

set models=%models%;basement.vmf
set models=%models%;basement_props.vmf
set models=%models%;basement_props_boxes.vmf
set models=%models%;basement_collision.vmf

set models=%models%;mainmenu.vmf

REM PROPS------

set models=%models%;props/box.vmf
set models=%models%;props/box_flat.vmf
set models=%models%;props/box_holder.vmf
set models=%models%;props/box_holder_2.vmf
set models=%models%;props/box_holder_3.vmf
set models=%models%;props/box_holder_big.vmf
set models=%models%;props/box_holder_big_2.vmf

set models=%models%;props/magazine_holder_1.vmf
set models=%models%;props/magazine_holder_2.vmf
set models=%models%;props/magazine_holder_3.vmf
set models=%models%;props/magazine_holder_4.vmf

set models=%models%;props/floppy_holder.vmf

set models=%models%;props/scale_machine.vmf

set models=%models%;props/dot_printer.vmf

set models=%models%;props/poster_colors.vmf

set models=%models%;props/screen.vmf
set models=%models%;props/screen_large.vmf

set models=%models%;props/box_gate.vmf
set models=%models%;props/trash.vmf

set models=%models%;props/elevator.vmf
set models=%models%;props/elevator_collisions.vmf

set models=%models%;props/gate_airlock.vmf

set models=%models%;props/camera.vmf
set models=%models%;props/camera_holder.vmf

set models=%models%;props/light_BROKEN.vmf
set models=%models%;props/light_GOOD.vmf

set models=%models%;props/creepy_client.vmf

for %%a in (%models%) do (
	echo "Parsing map %%a"
	java -jar %root%/VMF2OBJ.jar %root%/vmfs/%%a -o %ASSET_FOLDER%/Generated/%%a -r "%GAME_ROOT%/hl2/hl2_misc_dir.vpk;%GAME_ROOT%/hl2/hl2_textures_dir.vpk;%GAME_ROOT%/platform/platform_misc_dir.vpk;%root%/vpk/delivery_beyond.vpk"
)