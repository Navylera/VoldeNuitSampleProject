// Copyright (c) 2025 Navylera

using VoldeNuit.Framework;
using VoldeNuit.Framework.Instances;
using VoldeNuit.Framework.Input;
using VoldeNuit.Framework.Audio;
using VoldeNuit.Framework.Display;
using VoldeNuit.Framework.Drawing;

namespace MineSweeper;

using static Heart;
using static Mouse;
using static Sound;
using static VoldeNuit.Framework.Drawing.Draw;

using static Settings;

public class IMainButton: Instance {

    IBoard board = new IBoard();

    public IMainButton() {

        sprite_index = Instantiate(typeof(SButtonMain));

        depth = -1;
    }

    public override void Step() {

        x = room_width/2;
        y = 12;

        image_index = 0;

        if (board.fail) { image_index = 1; }

        if (board.complete) { image_index = 2; }

        if (mouse_check_button(mb_left)) {

            int mx = mouse_x_on(room_current.camera[0]);
            int my = mouse_y_on(room_current.camera[0]);

            if (mx >= x-7 && mx < x+7 && my >= y-7 && my < y+7) {
                
                image_index = image_index+3;
            }
        }

        if (mouse_check_button_released(mb_left)) {

            int mx = mouse_x_on(room_current.camera[0]);
            int my = mouse_y_on(room_current.camera[0]);

            if (mx >= x-7 && mx < x+7 && my >= y-7 && my < y+7) {

                audio_play_sound(Instantiate(typeof(SndClick)));
                
                instance_destroy(typeof(IBoard));

                // board = new IBoard();

                Room.room_restart();
            }
        }

        if (mouse_wheel_up()) {

            DIFFICULTY = (Difficulty)((int)(DIFFICULTY+1)%3);

            instance_destroy(typeof(IBoard));

            board = new IBoard();
        }

        if (mouse_wheel_down()) {

            DIFFICULTY = (Difficulty)((int)(DIFFICULTY+2)%3);

            instance_destroy(typeof(IBoard));

            board = new IBoard();
        }
    }
}