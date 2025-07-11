using SuperStarTrek.Game;

namespace SuperStarTrek.Game
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Super Star Trek";

            var game = new StarTrekGame();
            game.StartNewGame();
        }
    }
}
