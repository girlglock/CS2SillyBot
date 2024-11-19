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
using System.Text;
using Newtonsoft.Json;

partial class CS2SillyBot
{
    public static void LoadConfig()
    {
        string json = File.ReadAllText("config.json");
        Config config = JsonConvert.DeserializeObject<Config>(json);

        gamePath = config.GamePath!;
        teamOnly = config.TeamOnly;
        cmdCooldown = config.CmdCooldown;

        Print("Game Path: " + gamePath);
        Print("Team Only: " + teamOnly);
        Print("Command Cooldown: " + cmdCooldown);
    }

    public static void Print(string msg)
    {
        Console.WriteLine($"\x1b[95m[CS2Silly] \x1b[39m{msg}");
    }

    public static void PrintCon(string msg)
    {
        if (!msg.Contains("[Command Queue]")) Console.WriteLine($"\x1b[96m[Chat] \x1b[39m{msg}");
    }

    /* private static void SetupConsole()
    {
        SetConsoleOutputCP(65001); // Set console output to UTF-8 encoding
        IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
        GetConsoleMode(hConsole, out uint mode);
        SetConsoleMode(hConsole, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
    } */

    static string CombineArrays(string[] array1, string[] array2)
    {
        if (array1.Length != array2.Length)
        {
            throw new ArgumentException("Arrays must be of the same length");
        }

        StringBuilder sb = new();

        for (int i = 0; i < array1.Length; i++)
        {
            sb.Append(array1[i]).Append(": ").Append(array2[i]);

            if (i < array1.Length - 1)
            {
                sb.Append(", ");
            }
        }

        return sb.ToString();
    }

    public static bool AnyPlayerHasBets()
    {
        foreach (var player in players.Values)
        {
            if (player.RouletteBetRed > 0 || player.RouletteBetBlack > 0 || player.RouletteBetGreen > 0)
            {
                return true;
            }
        }
        return false;
    }

    static void PrintAsciiArt()
    {
        string[] asciiArt = {
            "\x1b[97m                                           %&&&@@&&&&%                          ",
            "\x1b[97m                              ...       %&@@@@@@@@@@@&&/                        ",
            "\x1b[97m                         ,&&@@@@@&&&&&*#@@@@@@@@@@@&&&@&&/                      ",
            "\x1b[97m                       (&@@@@@@@&@@@@@&&/&@%,@@# %@@@@@@@&%                     ",
            "\x1b[97m                      %&@@@@@@@@##&@@@@@@%   #&    &@@@@@&(,                    ",
            "\x1b[97m                     %&&@@@@@#&@%    *@@@@#        %@@@@@@%.                    ",
            "\x1b[97m                     #&@@@@@@         *@@@@       /@@@@@@(.                     ",
            "\x1b[33m                   ,./\x1b[97m&&@@@@@*        (@@@&#,  ,%&@@&&@@\x1b[33m&%(,,                   ",
            "\x1b[33m                 ,,*%%%\x1b[97m&@@@@@@&.     &@@@&(&@@@@@@@@@@\x1b[33m&#%%%%#,,                 ",
            "\x1b[33m                .,%%%%%#%\x1b[97m&@&&&@@@@@@@@@@#&@@&&&&&&&\x1b[33m%##%%%%%%%(,                ",
            "\x1b[33m              .,*%%%%%%%%%%%%#*\x1b[97m.#@@@@@\x1b[33m&%############%(#%%%%%%%%%,,              ",
            "\x1b[33m             .,*%%%%%%%%%%%%%%%%# ,*,,,****,....*((.%%%%%%%%%%%%%/,.            ",
            "\x1b[33m             ,,%%%%%%%%%%%%%%%%%%,\x1b[97m  (@@/&@@@@@#    \x1b[33m#%%%%%%%%%%%%%%(,            ",
            "\x1b[33m      **,##(,,%%%%%%%%%%%%%%%%%(                    /%%%%%%%%%%%%%%/,,,,,,*     ",
            "\x1b[33m      /**%%%%%%%%%%%%%%%%%%%%%*                        %%%%%%%%%%%%%,,,,,,*     ",
            "\x1b[33m       ../%%%%%%%%%%%%%%%%%%%%,                       /%%%%%%%%%%%%%#,,,,,      ",
            "\x1b[33m        ,,,%%%%%%%%%%%%%%%%%%%,                       %%%%%%%%%%%%%%%,,.        ",
            "\x1b[33m          .,*%%%%%%%%%%%%%%%%%,                      /%%%%%%%%%%%%%%%*,         ",
            "\x1b[33m           *,%%%%%%%%%%%%%%%%%/      \x1b[91m,,\x1b[33m              %%%%%%%%%%%%%%%%/,         ",
            "\x1b[33m           *,#%%%%%%%%%%%%%%%%%   \x1b[91m./((((((((/\x1b[33m       #%%%%%%%%%%%%%%%%*,.        ",
            "\x1b[33m           ,,(%%%%%%%%%%%%%%%%%. \x1b[91m,((((((((((((,\x1b[33m    *%%%%%%%%%%%%%%%%%,.         ",
            "\x1b[33m            ,,%%%%%%%%%%%%%%%%%( \x1b[91m/(((((((((((((,\x1b[33m  .%%%%%%%%%%%%%%%%%#,.         ",
            "\x1b[33m            ,,/%%%%%%%%%%%%%%%%%,\x1b[91m(((((((((((((((\x1b[33m ,%%%%%%%%%%%%%%%%%%,,          ",
            "\x1b[33m             .,/%%%%%#####%%%%%%#/@\x1b[91m(((((((((((((\x1b[33m/%%%%#########%%%%%/,,          ",
            "\x1b[33m              *,*#################,#\x1b[91m(((((((&&\x1b[33m(,#################%%*,,           ",
            "\x1b[33m               ..,#################(.\x1b[97m/@@@@@&*\x1b[33m(###################,,.            ",
            "\x1b[33m                 .,,###################(//#####################*,*              ",
            "\x1b[33m                    ,,/#####################################(,,.                ",
            "\x1b[33m                      ..,,(##############################/,,.                   ",
            "\x1b[33m                           .,,,*/###(((##########((/,,,,.                       ",
            "\x1b[33m                               ,,,((((((,,,,,,,,,,,                             ",
            "\x1b[33m                                  ,*(((,, ,,,,,,,.                              "
        };

        foreach (string line in asciiArt)
        {
            Print(line);
            Thread.Sleep(100);
        }
        Print("CS2Silly loaded");
    }
}
