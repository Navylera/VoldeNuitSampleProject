// Copyright (c) 2025 Navylera

using System.Linq;
using VoldeNuit.Framework.Audio;

namespace MineSweeper;

public class SndExplosion: Sound {

    public SndExplosion() {

        embedded = true;

        sound_path = Main.assembly.GetManifestResourceNames().First(n => n.EndsWith("SndExplosion.wav"));

        volume = 0.05f;
    }
}