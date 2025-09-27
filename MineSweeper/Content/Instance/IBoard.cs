// Copyright (c) 2025 Navylera

using System.Collections.Generic;
using System;

using Microsoft.Xna.Framework.Graphics;

using VoldeNuit.Framework;
using VoldeNuit.Framework.Instances;
using VoldeNuit.Framework.Input;
using VoldeNuit.Framework.Drawing;
using VoldeNuit.Framework.Display;
using VoldeNuit.Framework.Audio;

namespace MineSweeper;

using static Heart;
using static Mouse;
using static Draw;
using static Sound;

using static Settings;

public class IBoard: Instance {

    public Random random = new Random();

    public int[, ] board;

    public bool [, ] board_open;

    public bool [, ] board_flag;

    public int width = 0;
    public int height = 0;

    public int minecount = 0;

    public readonly List<(int, int)> skip = [];

    public bool started = false;

    public bool complete = false;
    public bool fail = false;

    public float time = 0f;
    public int flagcount = 0;

    public int leftrelease = 0;
    public int rightrelease = 0;

    public (int x, int y) point = (-1, -1);

    public IBoard() {

        switch (DIFFICULTY) {

            case Difficulty.BEGINNER: {
                
                board = new int[9, 9];
                board_open = new bool[9, 9];
                board_flag = new bool[9, 9];

                width = 9;
                height = 9;

                minecount = 10;

                room_width = 101;
                room_height = 123;

                break;
            }

            case Difficulty.INTERMEDIATE: {
                
                board = new int[16, 16];
                board_open = new bool[16, 16];
                board_flag = new bool[16, 16];

                width = 16;
                height = 16;

                minecount = 40;

                room_width = 171;
                room_height = 193;

                break;
            }

            case Difficulty.ADVANCED: {
                
                board = new int[30, 16];
                board_open = new bool[30, 16];
                board_flag = new bool[30, 16];

                width = 30;
                height = 16;

                minecount = 99;

                room_width = 311;
                room_height = 193;

                break;
            }
        }

        var t = room_current.camera.Count;

        room_current.camera[0].view.width  = room_width;
        room_current.camera[0].view.height = room_height;

        room_current.camera[0].viewport.width  = 3*room_width;
        room_current.camera[0].viewport.height = 3*room_height;

        int wx = (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/2)-((3*room_width)/2);
        int wy = (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height/2)-((3*room_height)/2);

        Window.GetGameWindow().Position = new Microsoft.Xna.Framework.Point(wx, wy);
    }

    public void CheckAround() {

        int flagaround = 0;

        for (int k=int.Clamp(point.y-1, 0, height);
             k<int.Clamp(point.y+2, 0, height);
             k=k+1) {

            for (int i=int.Clamp(point.x-1, 0, width);
                 i<int.Clamp(point.x+2, 0, width);
                 i=i+1) {

                if (board_flag[i, k]) { flagaround = flagaround+1; }
            }
        }

        if (flagaround == board[point.x, point.y]) {

            for (int k=int.Clamp(point.y-1, 0, height);
                 k<int.Clamp(point.y+2, 0, height);
                 k=k+1) {

                for (int i=int.Clamp(point.x-1, 0, width);
                     i<int.Clamp(point.x+2, 0, width);
                     i=i+1) {

                    if ((i, k) == (point.x, point.y) || board_flag[i, k]) { continue; }

                    if (!audio_is_playing(Instantiate(typeof(SndClick)))) {

                        audio_play_sound(Instantiate(typeof(SndClick)));
                    }
                    
                    Open(i, k);
                }
            }
        }
    }

    public void OpenRecursive(int vx, int vy) {

        if (fail) { return; }

        if (vx < 0 || vx >= width || vy < 0 || vy >= height) { return; }

        if (board_open[vx, vy] || board[vx, vy] > 8) { return; }

        board_open[vx, vy] = true;

        if (board[vx, vy] != 0) { return; }


            for (int k=int.Clamp(vy-1, 0, height);
                 k<int.Clamp(vy+2, 0, height);
                 k=k+1) {

                for (int i=int.Clamp(vx-1, 0, width);
                     i<int.Clamp(vx+2, 0, width);
                     i=i+1) {

                    OpenRecursive(i, k);
                }
            }

        return;
    }

    public (int, int) MouseToBoard() {

        (int x, int y) vt = ((int)float.Floor((mouse_x_on(room_current.camera[0])-XSTART)/(float)CELLSIZE),
                             (int)float.Floor((mouse_y_on(room_current.camera[0])-YSTART)/(float)CELLSIZE)
        );

        if (vt.x < 0 || vt.x >= width ||
            vt.y < 0 || vt.y >= height) {

            vt = (-1, -1);
        }

        return vt;
    }

    public void Open(int vx, int vy) {

        if (!started) {

            started = true;

            int plantedcount = 0;

            while (plantedcount < minecount) {

                int rx = random.Next(width);
                int ry = random.Next(height);

                if (rx >= point.x-1 && rx < point.x+2 &&
                    ry >= point.y-1 && ry < point.y+2) {

                    continue;
                }

                if (board[rx, ry] == 9) { continue; }

                board[rx, ry] = 9;

                for (int k=int.Clamp(ry-1, 0, height);
                     k<int.Clamp(ry+2, 0, height);
                     k=k+1) {

                    for (int i=int.Clamp(rx-1, 0, width);
                         i<int.Clamp(rx+2, 0, width);
                         i=i+1) {

                        if (board[i, k] == 9) { continue; }

                        board[i, k] = board[i, k]+1;
                    }
                }

                plantedcount = plantedcount+1;
            }
        }

        if (board[vx, vy] > 8) {

        board_open[vx, vy] = true;

            audio_play_sound(Instantiate(typeof(SndExplosion)));

            fail = true;

            board[vx, vy] = 10;
        }

        OpenRecursive(vx, vy);
    }

    public void Finish() {

        for (int k=0; k<height; k=k+1) {

            for (int i=0; i<width; i=i+1) {

                if (board_flag[i, k] && (board[i, k] != 9)) {

                    board_flag[i, k] = false;
                    board[i, k] = 11;
                    board_open[i, k] = true;
                }

                if (board_flag[i, k] || board[i, k] != 9) { continue; }

                board_open[i, k] = true;
            }
        }
    }

    public override void Step() {

        if (fail || complete) { return; }

        if (started) { time = time+(1/(float)room_speed); }

        leftrelease = int.Clamp(leftrelease-1, 0, leftrelease);
        rightrelease = int.Clamp(rightrelease-1, 0, rightrelease);

        point = MouseToBoard();

        if (mouse_check_button_released(mb_right)) {

            rightrelease = 10;

            if (point.x < 0 || point.y < 0) { return; }

            if ((mouse_check_button(mb_left) || leftrelease > 0)
                && board_open[point.x, point.y]) {

                CheckAround();
            }

            if (board_open[point.x, point.y]) { goto EXIT_RIGHT; }

            flagcount = board_flag[point.x, point.y]? flagcount-1 : flagcount+1;

            board_flag[point.x, point.y] = !board_flag[point.x, point.y];
        }

        EXIT_RIGHT: 

        if (mouse_check_button_released(mb_left)) {

            leftrelease = 10;

            if (point.x < 0 || point.y < 0) { return; }

            if (rightrelease > 0) { CheckAround(); goto EXIT_LEFT; }

            if (board_open[point.x, point.y] || board_flag[point.x, point.y]) { return; }

            if (board[point.x, point.y] > 8) { 

                audio_play_sound(Instantiate(typeof(SndExplosion)));
                
                fail = true;
                
                goto EXIT_LEFT;
            }

            audio_play_sound(Instantiate(typeof(SndClick)));

            Open(point.x, point.y);
        }

        EXIT_LEFT:

        if (fail) { Finish(); return; }

        int count = 0;

        foreach(bool b in board_open) { count = count+(b? 1:0); }

        if (count+minecount == width*height) { 
            
            complete = true;
            flagcount = minecount;

            for (int k=0; k<height; k=k+1) {

                for (int i=0; i<width; i=i+1) {

                    if (board[i, k] == 9) { board_flag[i, k] = true; }
                }
            }
        }
    }

    public override void Draw() {

        // Background

        int timecount = int.Clamp((int)float.Floor(time), 0, 999);
        int rmine     = minecount-flagcount<-99? -99 : minecount-flagcount;

        draw_sprite(Instantiate(typeof(SLRT)), 0, 0, 0);

        if (rmine < 0) { draw_sprite(Instantiate(typeof(SNumber)), 10, 13, 10); }
        if (rmine >= 0) { draw_sprite(Instantiate(typeof(SNumber)), rmine/100, 13, 10); }

        rmine = int.Abs(rmine);
        draw_sprite(Instantiate(typeof(SNumber)), rmine%100/10, 17, 10);
        draw_sprite(Instantiate(typeof(SNumber)), rmine%10, 21, 10);

        int addx = 10*(width-9);
        int addy = 10*(height-9);

        draw_sprite(Instantiate(typeof(SLRT)), 1, 71+addx, 0);

        draw_sprite(Instantiate(typeof(SNumber)), timecount/100, 76+addx, 10);
        draw_sprite(Instantiate(typeof(SNumber)), timecount%100/10, 80+addx, 10);
        draw_sprite(Instantiate(typeof(SNumber)), timecount%10, 84+addx, 10);

        draw_set_color(0x96a9c1u);
        draw_rectangle(30, 3, 41+addx, 2);

        draw_set_color(0xd5d6dbu);
        draw_rectangle(30, 20, 41+addx, 2);

        draw_set_color(0x96a9c1u);
        draw_rectangle(30, 25, 41+addx, 3);

        draw_rectangle(3, 28, 2, 92+addy);

        draw_set_color(0xffffffu);

        skip.Clear();

        int cx;
        int cy;

        if (fail) { goto SKIP_EXIT; }

        if (mouse_check_button(mb_left) && point.x >= 0 && point.y >= 0) {

            if (!board_open[point.x, point.y] && !board_flag[point.x, point.y]) { 
                
                skip.Add((point.x, point.y));
            }

            if (mouse_check_button(mb_right)) {

                for (int k=int.Clamp(point.y-1, 0, height);
                     k<int.Clamp(point.y+2, 0, height);
                     k=k+1) {

                    for (int i=int.Clamp(point.x-1, 0, width);
                         i<int.Clamp(point.x+2, 0, width);
                         i=i+1) {

                        if (!board_open[i, k] && !board_flag[i, k]) { skip.Add((i, k)); }
                    }
                }
            }
        }

        SKIP_EXIT: 

        for (int k=0; k<height; k=k+1) {

            for (int i=0; i<width; i=i+1) {
                
                cx = XSTART+(CELLSIZE*i);
                cy = YSTART+(CELLSIZE*k);

                if (skip.Count > 0 && skip.Contains((i, k))) {

                    skip.RemoveAt(0);

                    draw_sprite(Instantiate(typeof(SCellOpen)), 0, cx, cy);

                    continue;
                }

                if (!board_open[i, k]) {

                    draw_sprite(Instantiate(typeof(SCellClosed)), 0, cx, cy);

                    if (!board_flag[i, k]) { continue; }
                        
                    draw_sprite(Instantiate(typeof(SCellClosed)), 1, cx, cy);

                    continue;
                }

                draw_sprite(Instantiate(typeof(SCellOpen)), board[i, k], cx, cy);
            }
        }

        draw_set_color(0xd5d6dbu);
        draw_rectangle(95+addx, 28, 3, 90+addy);
        draw_rectangle(5, 117+addy, 93+addx, 3);

        draw_sprite(Instantiate(typeof(SEdge)), 0, 95+addx, 25);
        draw_sprite(Instantiate(typeof(SEdge)), 0, 3, 117+addy);
        
        draw_set_color(0xffffffu);
    }
}