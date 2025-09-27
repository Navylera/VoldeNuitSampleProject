// Copyright (c) 2025 Navylera

using VoldeNuit.Framework.Display;

using static Settings;

namespace MineSweeper;

public class RMain: Room {

    public RMain() {

        room_speed = 60;

        color_background = 0xbbc3d0u;

        switch (DIFFICULTY) {

            case Difficulty.BEGINNER: {

                room_width = 101;
                room_height = 123;

                break;
            }

            case Difficulty.INTERMEDIATE: {

                room_width = 171;
                room_height = 193;

                break;
            }

            case Difficulty.ADVANCED: {

                room_width = 311;
                room_height = 193;

                break;
            }
        }

        camera.Add(new Camera(0, 0, room_width, room_height, 0, 0, 3*room_width, 3*room_height));

        new IMainButton() {

            x = room_width/2, y = 12
        };
    }
}