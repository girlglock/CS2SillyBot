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
using CSGSI;

partial class CS2SillyBot
{
    public static void GameState(GameState gs)
    {
        _ = Task.Run(async () => await GSIJSON(gs.JSON));
    }

    public static void SetupGSI()
    {
        gsl = new GameStateListener(3000);
        gsl.NewGameState += new NewGameStateHandler(GameState);
        if (!gsl.Start())
        {
            Environment.Exit(0);
        }
    }
}
