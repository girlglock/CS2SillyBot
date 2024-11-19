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
    static async Task SimulatePlinko(int startPosition, string username)
    {
        const int rows = 5;
        const int columns = 5;
        bool[][] board = new bool[rows][];

        for (int i = 0; i < rows; i++)
        {
            board[i] = new bool[columns];
        }

        int currentRow = 0;
        int currentColumn = startPosition;
        bool ballAtBottom = false;

        while (!ballAtBottom)
        {
            board[currentRow][currentColumn] = false;
            currentRow++;

            if (currentRow < rows)
            {
                if (currentColumn > 0 && currentColumn < columns - 1)
                {
                    Random rand = new Random();
                    int direction = rand.Next(0, 2);
                    if (direction == 0)
                    {
                        currentColumn--;
                    }
                    else
                    {
                        currentColumn++;
                    }
                }
                else if (currentColumn == 0)
                {
                    currentColumn++;
                }
                else if (currentColumn == columns - 1)
                {
                    currentColumn--;
                }
            }

            if (currentRow == rows)
            {
                ballAtBottom = true;
            }

            if (!ballAtBottom)
            {
                board[currentRow][currentColumn] = true;
            }

            await PrintBoard(board, username);
        }

        //await SendCommand($"say [CS2Silly] ❯❯ {username}: The ball has landed at position {currentColumn + 1}.");
    }

    static async Task PrintBoard(bool[][] board, string username)
    {
        const string peg = " __ ";
        const string emptySlot = "|   ";
        const string ballSlot = "| o ";

        string[] lines = new string[11];

        for (int i = 0; i < board.Length; i++)
        {
            lines[i * 2] = "";
            lines[i * 2 + 1] = "";

            for (int j = 0; j < board[i].Length; j++)
            {
                lines[i * 2] += peg;

                if (board[i][j])
                {
                    lines[i * 2 + 1] += ballSlot;
                }
                else
                {
                    lines[i * 2 + 1] += emptySlot;
                }
            }

            lines[i * 2] += "|";
            lines[i * 2 + 1] += "|";
        }

        lines[10] = peg;
        for (int j = 0; j < board[0].Length; j++)
        {
            lines[10] += peg;
        }
        lines[10] += "|";

        Console.Clear();
        foreach (string line in lines)
        {
            Console.WriteLine($"[CS2Silly] ❯❯ {username}: " + line);
            await SendCommand($"say [CS2Silly] ❯❯ {username}: {line}");
        }
    }
}
