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
using System.Text.RegularExpressions;

partial class CS2SillyBot
{
    public static void SetupLogProcessor()
    {
        Print("Loading...");
        cmdCooldownStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Timer timer = new(static _ =>
        {
            isLogLocked = false;
            PrintAsciiArt();
        }, null, 5000, Timeout.Infinite);

        long lastReadPosition = 0;
        DateTime? creationDate = null;

        string logFilePath = Path.Join(gamePath, "console.log");

        while (true)
        {
            try
            {
                if (creationDate != File.GetCreationTime(logFilePath!)) lastReadPosition = 0;

                using FileStream fs = new(logFilePath!, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader reader = new(fs);
                fs.Seek(lastReadPosition, SeekOrigin.Begin);

                string? lastLine;
                while ((lastLine = reader.ReadLine()) != null)
                {
                    if (!isLogLocked)
                    {
                        ProcessLine(lastLine);
                    }
                    creationDate = File.GetCreationTime(logFilePath!);
                    lastReadPosition = fs.Position;
                }
            }
            catch (Exception ex)
            {
                Print("error reading log file: " + ex.Message);
            }
            Thread.Sleep(200);
        }
    }

    public static void ProcessLine(string line)
    {
        string pattern = @"\[(ALL|CT|T)\]\s+(.*?)‎?\s*(?:\[DEAD\])?:\s+(.*)";
        bool team;
        string username = "";
        string[] args = Array.Empty<string>();
        string command = "";

        Match match = Regex.Match(line, pattern);
        if (match.Success)
        {
            team = match.Groups[1].Value != "ALL";
            username = Regex.Replace(match.Groups[2].Value, "﹫.*", string.Empty);
            args = match.Groups[3].Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if(teamOnly && !team) return;
            
            if (args.Length > 0)
            {
                command = args[0];
                args = args.Skip(1).ToArray();

                switch (command)
                {
                    case "+gofish":
                        _ = Task.Run(async () => await CastLine(username, team));
                        break;
                    case "!coinflip":
                        _ = Task.Run(async () => await RandomCoinflip(username, args[0], args[1], team));
                        break;
                    case "!roulette":
                        _ = Task.Run(async () => await Roulette(username, args[0], args[1], team));
                        break;
                    case "!money":
                        _ = Task.Run(async () => await Money(username, team));
                        break;
                    case "!sendmoney":
                        _ = Task.Run(async () => await SendMoney(username, args[0], args[1], team));
                        break;
                        /* case "!plinko":
                            _ = Task.Run(async () => await SimulatePlinko(2, username));
                            break; */
                    default:
                        PrintCon(team ? $"[TEAM] {username}: {command} {string.Join(' ', args)}" : $"[ALL] {username}: {command} {string.Join(' ', args)}");
                        break;
                }
            }
        }
    }
}