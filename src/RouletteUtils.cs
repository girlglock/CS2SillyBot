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
partial class CS2SillyBot
{
    public static async Task Roulette(string username, string bet, string amount, bool team)
    {
        if(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - cmdCooldownStamp < cmdCooldown) return;
        
        cmdCooldownStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Print($"Player {username} is placing a bet...");

        if (!players.ContainsKey(username))
        {
            players[username] = new PlayerInfo
            {
                Username = username
            };
        }

        if (players.TryGetValue(username, out PlayerInfo? player))
        {
            if (player!.Money <= 0 || player.Money < float.Parse(amount))
            {
                await Task.Delay(1000);
                await SendCommand($"{(team ? "say_team" : "say")} [Roulette] ❯❯ {username}: You have not enough money left.");
                return;
            }

            if (!float.TryParse(amount, out float money)) await SendCommand($"{(team ? "say_team" : "say")} [CS2Silly] ❯❯ {username}: Invalid amount of money. You have ${Math.Round(player.Money)}");

            switch (bet)
            {
                case "black":
                    player.RouletteBetBlack = float.Parse(amount);
                    player.Money -= player.RouletteBetBlack;
                    await SendCommand($"{(team ? "say_team" : "say")} [Roulette] ❯❯ {username}: You bet ${player.RouletteBetBlack} on black.");
                    break;
                case "red":
                    player.RouletteBetRed = float.Parse(amount);
                    player.Money -= player.RouletteBetRed;
                    await SendCommand($"{(team ? "say_team" : "say")} [Roulette] ❯❯ {username}: You bet ${player.RouletteBetRed} on red.");
                    break;
                case "green":
                    player.RouletteBetGreen = float.Parse(amount);
                    player.Money -= player.RouletteBetGreen;
                    await SendCommand($"{(team ? "say_team" : "say")} [Roulette] ❯❯ {username}: You bet ${player.RouletteBetGreen} on green.");
                    break;
                default:
                    await SendCommand($"{(team ? "say_team" : "say")} [Roulette] ❯❯ {username}: Invalid bet. Please use 'red', 'black', or 'green'.");
                    return;
            }

            if (!isRouletteRunning) _ = Task.Run(async () => await StartRoulette(CancellationToken.None));
        }
    }


    private static Dictionary<string, RouletteWinners> rouletteWinners = new();

    public class RouletteWinners
    {
        public float AmountWon { get; set; }
    }

    private static async Task StartRoulette(CancellationToken cancellationToken)
    {
        if (!isRouletteRunning)
        {
            isRouletteRunning = true;
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(30000, cancellationToken);

                if (!AnyPlayerHasBets())
                    continue;

                string roll = Roll();
                float moneyWon = 0f;

                foreach (var player in players.Values)
                {
                    float mult = 2.0f;
                    switch (roll)
                    {
                        case "black":
                            mult = 2;
                            if (player.RouletteBetBlack > 0) moneyWon = player.RouletteBetBlack * mult;
                            break;
                        case "red":
                            mult = 2;
                            if (player.RouletteBetRed > 0) moneyWon = player.RouletteBetRed * mult;
                            break;
                        case "green":
                            mult = 14;
                            if (player.RouletteBetGreen > 0) moneyWon = player.RouletteBetGreen * mult;
                            break;
                    }

                    player.RouletteBetBlack = 0;
                    player.RouletteBetRed = 0;
                    player.RouletteBetGreen = 0;

                    player.Money += moneyWon;

                    if (!rouletteWinners.TryGetValue(player.Username!, out RouletteWinners? value))
                    {
                        rouletteWinners[player.Username!] = new RouletteWinners
                        {
                            AmountWon = moneyWon
                        };
                    }
                    else
                    {
                        value.AmountWon = moneyWon;
                    }
                }

                var winners = players.Values.Where(p => rouletteWinners[p.Username!].AmountWon > 0)
                                             .Select(p => $"{p.Username} - ${rouletteWinners[p.Username!].AmountWon}");

                await SendCommand($"{(teamOnly ? "say_team" : "say")} [Roulette] Roulette landed on {roll}.{(winners.Any() ? $" Winning players: {string.Join(", ", winners)}" : "")}");

                rouletteWinners.Clear();
            }
        }
    }

    public static string Roll()
    {
        double roll = random.NextDouble() * 100;
        if (roll < 2.7)
        {
            return "green";
        }
        else if (roll < 51.3)
        {
            return "red";
        }
        else
        {
            return "black";
        }
    }
}