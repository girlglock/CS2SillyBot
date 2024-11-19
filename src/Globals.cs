/*
Copyright (C) 2024 Deana Brcka

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System.Runtime.InteropServices;
using CSGSI;

partial class CS2SillyBot
{
    static GameStateListener? gsl;
    static readonly string hostname = "localhost";
    static readonly int port = 2121;
    private static SemaphoreSlim semaphore = new(1, 1);
    static string gamePath = "C:/Program Files (x86)/Steam/steamapps/common/Counter-Strike Global Offensive/game/csgo/";
    static bool teamOnly = false;
    static int cmdCooldown = 1;
    static long cmdCooldownStamp = 0;


    private static readonly Random random = new();

    static FishData? fishData;
    static bool isLogLocked = true;
    static bool isRouletteRunning = false;


    public class Config
    {
        public string? GamePath { get; set; }
        public bool TeamOnly { get; set; }
        public int CmdCooldown { get; set; }
    }

    private static Dictionary<string, PlayerInfo> players = new();
    public class PlayerInfo
    {
        public string? Username { get; set; }
        public bool IsFemboy { get; set; }
        public float Money { get; set; } = 100;
        public float RouletteBetRed { get; set; }
        public float RouletteBetBlack { get; set; }
        public float RouletteBetGreen { get; set; }
        public float RouletteWinnings { get; set; }
    }
}
