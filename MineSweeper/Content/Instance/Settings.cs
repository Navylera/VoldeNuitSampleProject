// Copyright (c) 2025 Navylera

public static class Settings {

    public enum Difficulty {

        BEGINNER, INTERMEDIATE, ADVANCED
    }

    public static Difficulty DIFFICULTY { get; set; } = Difficulty.ADVANCED;

    public const int XSTART = 5;
    public const int YSTART = 27;
    public const int CELLSIZE = 10;
}