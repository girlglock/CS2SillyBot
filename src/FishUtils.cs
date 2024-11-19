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
#pragma warning disable CA1050
using System.Text.Json;

public enum TimeOfDay
{
    Morning,
    Afternoon,
    Evening,
    Night
}

public enum SeaWeatherCondition
{
    ClearSkies,
    PartlyCloudy,
    Overcast,
    Fog,
    Rain,
    Thunderstorms,
    Windy,
    Calm
}

public class FishData
{
    public required List<FishCategory> Categories { get; set; }
}

public class FishCategory
{
    public required string Rarity { get; set; }
    public required List<Fish> FishList { get; set; }
}

public class Fish
{
    public required string Name { get; set; }
    public float Price { get; set; }
    public FishWeight? Weight { get; set; }
}

public class FishWeight
{
    public float Min { get; set; }
    public float Max { get; set; }
}

partial class CS2SillyBot
{
    public static async Task CastLine(string username, bool team)
    {
        if(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - cmdCooldownStamp < cmdCooldown) return;
        
        cmdCooldownStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Print("Player " + username + " is casting their line...");
        
        if (!players.TryGetValue(username, out PlayerInfo? player))
        {
            player = new PlayerInfo
            {
                Username = username
            };
            players[username] = player;
        }
        
        await Task.Delay(1000);
        await SendCommand($"{(team ? "say_team" : "say")} [GOFISH] 🎣 Player {username} is casting their line...");
        await Task.Delay(1000);
        var weather = GetWeather();
        await SendCommand($"{(team ? "say_team" : "say")} [GOFISH] ❯❯ {username}: 🌊 You casted your line on {weather.Item2} weather");
        await Task.Delay(3000);
        if (random.NextDouble() <= 0.25)
            await SendCommand($"{(team ? "say_team" : "say")} [GOFISH] ❯❯ {username}: 🙁 You didnt catch anything, try again later...");
        else
        {
            var (fishName, price, weight) = GetFishResult(weather.Item1);
            player.Money += price;
            await SendCommand($"{(team ? "say_team" : "say")} [GOFISH] ❯❯ {username}: <)͜͡˒ ⋊ You caught a {fishName}! ⚖️ It weighs {Math.Round(weight, 2)}kg and is worth around ${Math.Round(price, 2)}");
            await Task.Delay(200);
            await SendCommand($"{(team ? "say_team" : "say")} [GOFISH] ❯❯ {username}: Your total money is now ${player.Money}");
        }
    }

    public static void LoadFishDB()
    {
        string json = File.ReadAllText("FishDB.json");
        fishData = JsonSerializer.Deserialize<FishData>(json)!;
    }

    public static (string fishName, float price, float weight) GetFishResult(SeaWeatherCondition seaWeather)
    {
        if (fishData != null)
        {
            double rarityModifier = GetRarityModifier(seaWeather);
            double rarityRoll = random.NextDouble();
            string chosenRarity = ChooseRarity(rarityRoll, fishData!.Categories, rarityModifier);

            var chosenCategory = fishData.Categories.Find(category => category.Rarity == chosenRarity);
            var fishList = chosenCategory!.FishList;

            Fish chosenFish = fishList[random.Next(fishList.Count)];
            double randomDouble = random.NextDouble();
            float weight = (float)(randomDouble * (chosenFish.Weight!.Max - chosenFish.Weight.Min) + chosenFish.Weight.Min);
            float usdPrice = chosenFish.Price * weight;

            return (chosenFish.Name, usdPrice, weight);
        }
        else
        {
            throw new NullReferenceException("fishData was null");
        }
    }

    public static (SeaWeatherCondition, string) GetWeather()
    {
        SeaWeatherCondition forecastedWeather = ForecastSeaWeather();
        string weatherDescription = GetWeatherDescription(forecastedWeather);
        return (forecastedWeather, weatherDescription);
    }

    public static SeaWeatherCondition ForecastSeaWeather()
    {
        TimeOfDay currentTimeOfDay = GetCurrentTimeOfDay();
        var baseCondition = currentTimeOfDay switch
        {
            TimeOfDay.Morning => SeaWeatherCondition.ClearSkies,
            TimeOfDay.Afternoon => SeaWeatherCondition.PartlyCloudy,
            TimeOfDay.Evening => SeaWeatherCondition.Overcast,
            TimeOfDay.Night => SeaWeatherCondition.ClearSkies,
            _ => SeaWeatherCondition.ClearSkies,
        };
        if (random.NextDouble() <= 0.25)
        {
            baseCondition = (SeaWeatherCondition)random.Next(0, Enum.GetNames(typeof(SeaWeatherCondition)).Length);
        }
        return baseCondition;
    }

    private static TimeOfDay GetCurrentTimeOfDay()
    {
        DateTime currentTime = DateTime.Now;
        int currentHour = currentTime.Hour;

        if (currentHour >= 6 && currentHour < 12)
        {
            return TimeOfDay.Morning;
        }
        else if (currentHour >= 12 && currentHour < 18)
        {
            return TimeOfDay.Afternoon;
        }
        else if (currentHour >= 18 && currentHour < 24)
        {
            return TimeOfDay.Evening;
        }
        else
        {
            return TimeOfDay.Night;
        }
    }

    private static string GetWeatherDescription(SeaWeatherCondition condition)
    {
        return condition switch
        {
            SeaWeatherCondition.ClearSkies => "Clear skies with calm seas",
            SeaWeatherCondition.PartlyCloudy => "Partly cloudy skies with gentle breeze",
            SeaWeatherCondition.Overcast => "Overcast skies with potential for rain",
            SeaWeatherCondition.Fog => "Foggy conditions with reduced visibility",
            SeaWeatherCondition.Rain => "Rainfall with choppy seas",
            SeaWeatherCondition.Thunderstorms => "Thunderstorms with rough seas and lightning",
            SeaWeatherCondition.Windy => "Windy conditions with high waves",
            SeaWeatherCondition.Calm => "Calm seas with little to no wind",
            _ => "Unknown weather condition",
        };
    }

    private static double GetRarityModifier(SeaWeatherCondition seaWeather)
    {
        return seaWeather switch
        {
            SeaWeatherCondition.ClearSkies => 1.0,
            SeaWeatherCondition.PartlyCloudy => 1.1,
            SeaWeatherCondition.Overcast => 1.2,
            SeaWeatherCondition.Fog => 0.8,
            SeaWeatherCondition.Rain => 0.7,
            SeaWeatherCondition.Thunderstorms => 0.5,
            SeaWeatherCondition.Windy => 0.9,
            SeaWeatherCondition.Calm => 1.1,
            _ => 1.0,
        };
    }

    private static string ChooseRarity(double roll, List<FishCategory> categories, double modifier)
    {
        double cumulativeChance = 0.0;
        foreach (var category in categories)
        {
            cumulativeChance += RarityChance(category.Rarity) * modifier;
            if (roll <= cumulativeChance)
            {
                return category.Rarity;
            }
        }
        return categories[^1].Rarity;
    }

    private static double RarityChance(string rarity)
    {
        return rarity switch
        {
            "Common" => 0.4,
            "Uncommon" => 0.3,
            "Rare" => 0.2,
            "Very Rare" => 0.1,
            "Epic" => 0.05,
            "Legendary" => 0.025,
            _ => 0.0,
        };
    }
}
