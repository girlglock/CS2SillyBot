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
    public static async Task RandomCoinflip(string username, string choice, string amount, bool team)
    {
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - cmdCooldownStamp < cmdCooldown) return;

        cmdCooldownStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Print($"Player {username} is flipping a coin...");

        if (!players.TryGetValue(username, out PlayerInfo? player))
        {
            player = new PlayerInfo
            {
                Username = username
            };
            players[username] = player;
        }

        choice = choice.ToLower();

        if (choice != "heads" && choice != "tails" || !float.TryParse(amount, out float money))
        {
            _ = Task.Run(async () => await SendCommand($"{(team ? "say_team" : "say")} [Coinflip] ❯❯ {username}: Invalid usage. ex. !coinflip heads 100"));
            return;
        }

        if (player!.Money <= 0 || player.Money < money)
        {
            await Task.Delay(1000);
            await SendCommand($"{(team ? "say_team" : "say")} [Coinflip] ❯❯ {username}: You have not enough money left.");
            return;
        }

        await Task.Delay(1000);

        string landedOn = random.NextDouble() < 0.5 ? "heads" : "tails";
        if(landedOn == choice) player.Money += money * 2;
        _ = Task.Run(async () => await SendCommand($"{(team ? "say_team" : "say")} [Coinflip] ❯❯ {username}: You tossed a coin and it landed on {landedOn}. Your current balance is ${Math.Round(player.Money)}"));
    }

    public static async Task Money(string username, bool team)
    {
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - cmdCooldownStamp < cmdCooldown) return;

        cmdCooldownStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Print($"Player {username} is checking their money...");

        if (!players.TryGetValue(username, out PlayerInfo? player))
        {
            player = new PlayerInfo
            {
                Username = username
            };
            players[username] = player;
        }

        await Task.Delay(500);

        _ = Task.Run(async () => await SendCommand($"{(team ? "say_team" : "say")} [Wallet] ❯❯ {username}: You have ${Math.Round(player.Money)} in your wallet."));
    }

    public static async Task SendMoney(string username, string target, string amount, bool team)
    {
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - cmdCooldownStamp < cmdCooldown) return;

        cmdCooldownStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Print($"Player {username} is sending ${amount} to {target}...");

        if (!players.TryGetValue(username, out PlayerInfo? player))
        {
            player = new PlayerInfo
            {
                Username = username
            };
            players[username] = player;
        }

        if (!float.TryParse(amount, out float money))
        {
            _ = Task.Run(async () => await SendCommand($"{(team ? "say_team" : "say")} [Wallet] ❯❯ {username}: Invalid usage. ex. !sendmoney playername 100"));
            return;
        }

        if (player!.Money <= 0 || player.Money < money)
        {
            await Task.Delay(500);
            _ = Task.Run(async () => await SendCommand($"{(team ? "say_team" : "say")} [Wallet] ❯❯ {username}: You have not enough money left."));
            return;
        }

        if (!players.TryGetValue(target, out PlayerInfo? targetPlayer))
        {
            _ = Task.Run(async () => await SendCommand($"{(team ? "say_team" : "say")} [Wallet] ❯❯ {username}: Player {target} does not exist."));
            return;
        }

        await Task.Delay(500);

        targetPlayer.Money += money;
        player.Money -= money;

        _ = Task.Run(async () => await SendCommand($"{(team ? "say_team" : "say")} [Wallet] ❯❯ {username}: You have sent ${amount} to {target}."));
    }
}