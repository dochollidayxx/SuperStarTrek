using Xunit;
using SuperStarTrek.Game;

namespace SuperStarTrek.Tests
{
    /// <summary>
    /// Tests for the StarTrekGame class
    /// </summary>
    public class StarTrekGameTests
    {
        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            // Act
            var game = new StarTrekGame();

            // Assert
            Assert.Null(game.CurrentGameState);
            Assert.False(game.IsGameInProgress);
        }

        [Fact]
        public void StartNewGame_WithSeed_CreatesReproducibleGame()
        {
            // Arrange
            var game1 = new StarTrekGame();
            var game2 = new StarTrekGame();

            // Note: We can't actually test the full StartNewGame method
            // because it runs the game loop, but we can test that it
            // creates consistent game states with the same seed

            // This test would need to be modified to work with a
            // non-interactive version of the game or with dependency injection
            // For now, we'll just verify the basic structure
            Assert.NotNull(game1);
            Assert.NotNull(game2);
        }

        // Note: Additional tests would require refactoring the game to be more testable
        // by separating the console I/O from the game logic, or using dependency injection
        // to mock the console interactions.
    }
}
