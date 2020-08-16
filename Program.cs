using System;

namespace Minesweeper
{
    /*
        CLASS CELL: The single grid cell
        PARAMETERS:
        - NUM: Defines the type of the cell, -1 means uninitialized, 0 means nothing, 9 means bomb, any other number is the surrounding bomb count
        - FLAG: 0 means nothing, 1 means flag, 2 means question mark
        - MANTLED: TRUE means mantled, FALSE means dismantled
    */
    class Cell
    {
        public int num = -1, flag = 0;
        public bool mantled = true;
    }

    /*
        CLASS COORDINATE: A data structure for coordinate pairs
        PARAMETERS:
        - X, Y: Together they make the coordinates of the cursor
    */
    class Coordinate
    {
        public int x = 0, y = 0;
    }
    class Program
    {
        static void ConsoleSetColor(ConsoleColor fg, ConsoleColor bg = ConsoleColor.Black)
        {
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
        }

        static void Main()
        {
            /* ======== RUNTIME VARIABLES ======== */
            string[] main_menu = { "Play", "Options", "Quit" };
            int grid_height = 10, grid_width = 15, bombs = 20;

            bool cont = true;
            ConsoleKey key; // Used to handle user key input
            int choice; // Used to store user selection

            /* ======== PROGRAM LOOP ======== */
            while (cont)
            {
                // Menu loop
                choice = 0;
                do
                {
                    // Print menu
                    Console.Clear();
                    Console.WriteLine(
                        "Main Menu\n" +
                        "========="
                    );
                    for (int i = 0; i < main_menu.Length; i++)
                    {
                        if (i == choice) ConsoleSetColor(Console.BackgroundColor, Console.ForegroundColor);
                        Console.WriteLine(main_menu[i]);
                        if (i == choice) Console.ResetColor();
                    }

                    // Read user input
                    key = Console.ReadKey(false).Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            if (choice > 0) choice--;
                            break;

                        case ConsoleKey.DownArrow:
                            if (choice < main_menu.Length) choice++;
                            break;
                    }
                } while (key != ConsoleKey.Enter);

                switch (main_menu[choice])
                {
                    case "Quit":
                        cont = false;
                        break;

                    case "Options":
                        // Print settings
                        Console.Clear();
                        Console.WriteLine(
                            "Options\n" +
                            "========"
                        );

                        Console.Write("Grid width: ");
                        grid_width = Convert.ToInt32(Console.ReadLine());

                        Console.Write("Grid height: ");
                        grid_height = Convert.ToInt32(Console.ReadLine());

                        do
                        {
                            Console.Write("Bomb count: ");
                            bombs = Convert.ToInt32(Console.ReadLine());
                            if (bombs >= grid_width * grid_height) Console.WriteLine("Too Many bombs!");
                        } while (bombs >= grid_width * grid_height);

                        break;

                    case "Play":
                        /* ======== GAME RUNTIME VARIABLES ======== */
                        ConsoleColor[] numcolors = new ConsoleColor[] {
                            ConsoleColor.Blue,
                            ConsoleColor.Green,
                            ConsoleColor.Red,
                            ConsoleColor.DarkBlue,
                            ConsoleColor.DarkRed,
                            ConsoleColor.Cyan,
                            ConsoleColor.White,
                            ConsoleColor.DarkGray
                        };
                        Cell[,] grid = new Cell[grid_width, grid_height];
                        Coordinate cursor = new Coordinate();
                        bool playing = true;
                        Random r = new Random();

                        /* ======== GAME FUNCTIONS ======== */
                        /*
                            FUNTION PRINTGRID: Prints the grid and puts values parameter MAP in the corresponding cells
                            PARAMETERS:
                            - MAP: A two-dimensional char array containing the contents of the grid
                        */
                        void PrintGrid(Cell[,] grid, Coordinate cursor)
                        {
                            int w = grid.GetLength(0);
                            int h = grid.GetLength(1);

                            Console.SetCursorPosition(0, 0);
                            Console.CursorVisible = false;
                            // For each row
                            for (int y = 0; y < h; y++)
                            {
                                // If top
                                if (y == 0)
                                    for (int x = 0; x < w; x++)
                                    {
                                        if (x == 0) Console.Write("╔═");
                                        else Console.Write("╤═");
                                        if (cursor.y == 0 && x == cursor.x) Console.Write("╤═");
                                        else Console.Write("══");
                                        if (x == w - 1) Console.Write("╗\n");
                                    }
                                // Else if in middle
                                else
                                    for (int x = 0; x < w; x++)
                                    {
                                        if (x == 0) Console.Write("╟─");
                                        else Console.Write("┼─");
                                        if (cursor.y == y && cursor.x == x) Console.Write("┬─");
                                        else if (cursor.y + 1 == y && cursor.x == x) Console.Write("┴─");
                                        else Console.Write("──");
                                        if (x == w - 1) Console.Write("╢\n");
                                    }

                                for (int x = 0; x < w; x++)
                                {
                                    // If left
                                    if (x == 0 && cursor.x == 0 && cursor.y == y) Console.Write("╟");
                                    else if (x == 0) Console.Write("║");
                                    // Else if in middle
                                    else if (x == cursor.x && y == cursor.y) Console.Write("├");
                                    else if (x == cursor.x + 1 && y == cursor.y) Console.Write("┤");
                                    else Console.Write("│");

                                    if (grid[x, y].mantled)
                                    {
                                        Console.Write("▐");
                                        ConsoleSetColor(Console.BackgroundColor, Console.ForegroundColor);
                                        switch (grid[x, y].flag)
                                        {
                                            case 0:
                                                Console.Write(" ");
                                                break;

                                            case 1:
                                                Console.Write("!");
                                                break;

                                            case 2:
                                                Console.Write("?");
                                                break;
                                        }
                                        Console.ResetColor();
                                        Console.Write("▌");
                                    }
                                    else
                                        switch (grid[x, y].num)
                                        {
                                            case 0:
                                                Console.Write("   ");
                                                break;

                                            case 9:
                                                Console.Write(" X ");
                                                break;

                                            default:
                                                ConsoleSetColor(numcolors[grid[x, y].num - 1]);
                                                Console.Write(String.Format(" {0} ", grid[x, y].num));
                                                Console.ResetColor();
                                                break;
                                        }

                                    // Else if right
                                    if (x == w - 1 && cursor.x == w - 1 && cursor.y == y) Console.Write("╢\n");
                                    else if (x == w - 1) Console.Write("║\n");
                                }
                                // If bottom
                                if (y == h - 1)
                                    for (int x = 0; x < w; x++)
                                    {
                                        if (x == 0) Console.Write("╚═");
                                        else Console.Write("╧═");
                                        if (cursor.y == h - 1 && x == cursor.x) Console.Write("╧═");
                                        else Console.Write("══");
                                        if (x == w - 1) Console.Write("╝\n");
                                    }
                            }
                        }

                        /*
                        FUNCTION DISMANTLE:  Dismantles the selected cell (and calls itself for blank cells)
                        PARAMETERS:
                        - X, Y: The coordinates for the cell to dismantle
                        */
                        void Dismantle(int x, int y)
                        {
                            grid[x, y].mantled = false;
                            if (grid[x, y].num == 0)
                                for (int ry = -1; ry <= 1; ry++)
                                    for (int rx = -1; rx <= 1; rx++)
                                        if (
                                            x + rx >= 0 &&
                                            x + rx < grid_width &&
                                            y + ry >= 0 &&
                                            y + ry < grid_height &&

                                            grid[x + rx, y + ry].mantled
                                        ) Dismantle(x + rx, y + ry);
                        }

                        /* ======== SETUP ======== */
                        // Fill grid with cells
                        for (int y = 0; y < grid.GetLength(1); y++)
                            for (int x = 0; x < grid.GetLength(0); x++)
                                grid[x, y] = new Cell();

                        /* ======== GAME LOOP ======== */
                        Console.Clear();
                        while (playing)
                        {
                            Cell cell = grid[cursor.x, cursor.y];

                            PrintGrid(grid, cursor);

                            // Await user input
                            key = Console.ReadKey(true).Key;
                            switch (key)
                            {
                                case ConsoleKey.UpArrow:
                                    if (cursor.y > 0) cursor.y--;
                                    break;

                                case ConsoleKey.RightArrow:
                                    if (cursor.x < grid_width - 1) cursor.x++;
                                    break;

                                case ConsoleKey.DownArrow:
                                    if (cursor.y < grid_height - 1) cursor.y++;
                                    break;

                                case ConsoleKey.LeftArrow:
                                    if (cursor.x > 0) cursor.x--;
                                    break;

                                case ConsoleKey.Spacebar:
                                    // If the cell is uninitialized
                                    if (cell.num == -1)
                                    {
                                        /* ======== SETUP ======== */
                                        // Initialize cells
                                        for (int y = 0; y < grid_height; y++)
                                            for (int x = 0; x < grid_width; x++)
                                                grid[x, y].num++;

                                        // Place bombs
                                        for (int i = 0; i < bombs; i++)
                                        {
                                            int x, y;
                                            // If possible, place bombs not adjacent to first clicked cell
                                            if (i < grid_width * grid_height - 9)
                                            {
                                                bool adjacent;
                                                do
                                                {
                                                    x = r.Next(grid_width);
                                                    y = r.Next(grid_height);
                                                    adjacent = false;
                                                    for (int ry = -1; ry <= 1; ry++)
                                                        for (int rx = -1; rx <= 1; rx++)
                                                            if (x == cursor.x + rx && y == cursor.y + ry)
                                                                adjacent = true;
                                                }
                                                while (grid[x, y].num == 9 || adjacent);
                                            }
                                            else
                                            {
                                                do
                                                {
                                                    // x = cursor.x + 1 OR -1
                                                    x = cursor.x + 1 - 2 * r.Next(1);
                                                    y = cursor.y + 1 - 2 * r.Next(1);
                                                }
                                                while (grid[x, y].num == 9);
                                            }
                                            grid[x, y].num = 9;
                                        }

                                        // For each cell, count and store the number of adjacent bombs
                                        for (int y = 0; y < grid.GetLength(1); y++)
                                            for (int x = 0; x < grid.GetLength(0); x++)
                                                if (grid[x, y].num != 9)
                                                    for (int ry = -1; ry <= 1; ry++)
                                                        for (int rx = -1; rx <= 1; rx++)
                                                            if (
                                                                x + rx >= 0 &&
                                                                x + rx < grid_width &&
                                                                y + ry >= 0 &&
                                                                y + ry < grid_height &&
                                                                grid[x + rx, y + ry].num == 9
                                                            ) grid[x, y].num++;
                                    }

                                    // If the cell is unflagged
                                    if (cell.flag == 0)
                                    {
                                        Dismantle(cursor.x, cursor.y);
                                        // If the cell is a bomb
                                        if (cell.num == 9)
                                        {
                                            for (int y = 0; y < grid_height; y++)
                                                for (int x = 0; x < grid_width; x++)
                                                    grid[x, y].mantled = false;
                                            PrintGrid(grid, cursor);
                                            Console.WriteLine("Game Over!");
                                            playing = false;
                                        }
                                    }

                                    // Check if won (if mantled cells == bomb count)
                                    int mantled = 0;
                                    for (int y = 0; y < grid_height; y++)
                                        for (int x = 0; x < grid_width; x++)
                                            if (grid[x, y].mantled)
                                                mantled++;
                                    if (mantled == bombs)
                                    {
                                        for (int y = 0; y < grid_height; y++)
                                            for (int x = 0; x < grid_width; x++)
                                                grid[x, y].mantled = false;
                                        PrintGrid(grid, cursor);
                                        Console.WriteLine("You Won!");
                                        playing = false;
                                    }
                                    break;

                                case ConsoleKey.Enter:
                                    cell.flag++;
                                    if (cell.flag == 3) cell.flag = 0;
                                    break;
                            }
                        }
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
