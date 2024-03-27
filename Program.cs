using System;
using System.Collections.Generic;
using System.Linq;

class Position
{
    
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
}

class Player
{
    public string Name { get; set; }
    public Position Position { get; set; }
    public int GemCount { get; set; }
    public bool GemAcquired { get; set; } = false;
    public Player(string name, Position position)
    {
        Name = name;
        Position = position;
        GemCount = 0;
    }

    public void Move(char direction)
    {
        switch (direction)
        {
            case 'U':
                Position.Y--;
                break;
            case 'D':
                Position.Y++;
                break;
            case 'L':
                Position.X--;
                break;
            case 'R':
                Position.X++;
                break;
        }
    }
}

class Cell
{
    public string Occupant { get; set; }

    public Cell(string occupant)
    {
        Occupant = occupant;
    }
}

class Board
{
    public Cell[,] Grid { get; set; }
    private List<Position> gems;

    public Board()
    {
        Grid = new Cell[6, 6];
        gems = new List<Position>();

        InitializeGrid();
        PlaceRandomGems();
    }

    private void InitializeGrid()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Grid[i, j] = new Cell("-");
            }
        }

        Grid[0, 0].Occupant = "P1";
        Grid[5, 5].Occupant = "P2";
    }

    private void PlaceRandomGems()
    {
        Random random = new Random();

        for (int i = 0; i < 6; i++)
        {
            int x, y;
            do
            {
                x = random.Next(6);
                y = random.Next(6);
            } while (Grid[y, x].Occupant != "-");

            Grid[y, x].Occupant = "G";
            gems.Add(new Position(x, y));
        }
    }

    public void Display()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Console.Write(Grid[i, j].Occupant + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }


    public int GetPlayerGemCount(string playerName)
    {
        int count = 0;

        foreach (var gemPosition in gems)
        {
            if (Grid[gemPosition.Y, gemPosition.X].Occupant == "G" && (playerName == "P1" || playerName == "P2"))
            {
                count++;
            }
        }

        return count;
    }


    public bool IsValidMove(Player player, char direction)
    {
        int newX = player.Position.X;
        int newY = player.Position.Y;

        switch (direction)
        {
            case 'U':
                newY--;
                break;
            case 'D':
                newY++;
                break;
            case 'L':
                newX--;
                break;
            case 'R':
                newX++;
                break;
        }

        if (newX >= 0 && newX < 6 && newY >= 0 && newY < 6)
        {
            return Grid[newY, newX].Occupant == "-" || Grid[newY, newX].Occupant == "G";
        }

        return false;
    }

    public void CollectGem(Player player)
    {


        Position playerPosition = new Position(player.Position.X, player.Position.Y);

        if (gems.Any(gem => gem.X == player.Position.X && gem.Y == player.Position.Y))
        {
            player.GemCount++;
            gems.RemoveAll(gem => gem.X == player.Position.X && gem.Y == player.Position.Y);
            Console.WriteLine($"Player {player.Name} collected a gem! Gem Count: {player.GemCount}");
        }

    }

    public void MovePlayer(Player player, Position newPosition)
    {

        Grid[player.Position.Y, player.Position.X].Occupant = "-";
        Grid[newPosition.Y, newPosition.X].Occupant = player.Name;
        player.Position = newPosition;

    }
}

class Game
{
    public Board Board { get; set; }
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
    public Player CurrentTurn { get; set; }
    public int TotalTurns { get; set; }

    public Game()
    {
        Board = new Board();
        Player1 = new Player("P1", new Position(0, 0));
        Player2 = new Player("P2", new Position(5, 5));
        CurrentTurn = Player1;
        TotalTurns = 0;
    }

    public void Start()
    {
        do
        {
            Board.Display();

            Console.WriteLine($"Current Turn: {CurrentTurn.Name}");
            Console.WriteLine();

            Console.Write("Enter move (U/D/L/R): ");

            char move = Char.ToUpper(Console.ReadKey().KeyChar);

            Console.WriteLine();

            if (Board.IsValidMove(CurrentTurn, move))
            {
                Position newPosition = new Position(CurrentTurn.Position.X, CurrentTurn.Position.Y);

                switch (move)
                {
                    case 'U':
                        newPosition.Y--;
                        break;
                    case 'D':
                        newPosition.Y++;
                        break;
                    case 'L':
                        newPosition.X--;
                        break;
                    case 'R':
                        newPosition.X++;
                        break;
                }

                Board.MovePlayer(CurrentTurn, newPosition);
                Console.WriteLine($"{CurrentTurn.Name}'s gem count: {Board.GetPlayerGemCount(CurrentTurn.Name)}");
                Console.WriteLine();
                Board.CollectGem(CurrentTurn);
                TotalTurns++;
                SwitchTurn();
            }
            else
            {
                Console.WriteLine("Invalid move! Please Try again.");
            }
        } while (!IsGameOver());

        Board.Display();
        AnnounceWinner();
    }

    public void SwitchTurn()
    {
        CurrentTurn = (CurrentTurn == Player1) ? Player2 : Player1;
    }

    public bool IsGameOver()
    {
        return TotalTurns == 30;
    }

    public void DisplayCollectedGems()
    {
        Console.WriteLine($"Player {Player1.Name} collected {Player1.GemCount} gems.");
        Console.WriteLine($"Player {Player2.Name} collected {Player2.GemCount} gems.");
        Console.WriteLine($"Total Gems Collected: {Player1.GemCount + Player2.GemCount}");
        Console.WriteLine();
    }

    public void AnnounceWinner()
    {
        Console.WriteLine("Game Over Buddy!");
        DisplayCollectedGems();

        if (Player1.GemCount > Player2.GemCount)
        {
            Console.WriteLine($"Player {Player1.Name} wins!");
        }
        else if (Player2.GemCount > Player1.GemCount)
        {
            Console.WriteLine($"Player {Player2.Name} wins!");
        }
        else
        {
            Console.WriteLine("It's a tie!");
        }
    }
}

class Program
{
    static void Main()
    {
        Game gemHuntersGame = new Game();
        gemHuntersGame.Start();
    }
}