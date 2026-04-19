namespace Hangman.Models
{
    public class GameState
    {
        public string SaveName { get; set; }
        public string UserName { get; set; }
        public int Level { get; set; }
        public int Mistakes { get; set; }
        public int TimeLeft { get; set; }
        public string Category { get; set; }
        public string TargetWord { get; set; }
        public string CurrentWordState { get; set; }
        public string DisabledLetters { get; set; }
    }
}