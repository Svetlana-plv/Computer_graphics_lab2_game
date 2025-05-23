using Seasons;

class Program
{
    static void Main(string[] args)
    {
        using (Game game = new Game(750, 750))
        {
            game.Run();
        }
    }
}
