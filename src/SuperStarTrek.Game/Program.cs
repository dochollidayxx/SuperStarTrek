using SuperStarTrek.Game;

namespace SuperStarTrek.Game
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== SUPER STAR TREK ===");
            Console.WriteLine("C# Port of the Classic Space Strategy Game");
            Console.WriteLine();

            var game = new StarTrekGame();
            game.Run();
        }
    }
}
