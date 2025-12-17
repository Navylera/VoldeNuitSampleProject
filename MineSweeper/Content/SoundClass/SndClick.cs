// Copyright (c) 2025 Navylera

using System.Linq;

using VoldeNuit.Framework.Audio;

namespace MineSweeper;

public class SndClick: Sound {

    public SndClick() {

        embedded = true;

        sound_path = Main.assembly.GetManifestResourceNames().First(n => n.EndsWith("SndClick.wav"));

        volume = 0.3f;
    }
}