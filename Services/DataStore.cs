using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hangman.Models;
using System.IO;

namespace Hangman.Services
{
    public static class DataStore
    {
        private const string UsersFile = "users.json";
        private const string SavesFile = "saves.json";
        private const string StatsFile = "stats.json";

        public static List<User> LoadUsers() => Load<List<User>>(UsersFile) ?? new List<User>();
        public static void SaveUsers(List<User> users) => Save(UsersFile, users);

        public static List<GameState> LoadSaves() => Load<List<GameState>>(SavesFile) ?? new List<GameState>();
        public static void SaveGames(List<GameState> saves) => Save(SavesFile, saves);

        public static List<UserStatistic> LoadStats() => Load<List<UserStatistic>>(StatsFile) ?? new List<UserStatistic>();
        public static void SaveStats(List<UserStatistic> stats) => Save(StatsFile, stats);



        private static T Load<T>(string filePath)
        {
            if (!File.Exists(filePath)) return default;
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }

        private static void Save<T>(string filePath, T data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);

        }

        public static string GetRandomWord(string category)
        {
            var words = new Dictionary<string, List<string>>
            {
                { "Animals", new List<string> { "ELEFANT", "GIRAFA", "LEOPARD", "CANGUR", "PINGUIN", "GORILA", "BALENA", "HAMSTER", "BUFNITA", "PANTERA" } },
                { "Fruits", new List<string> { "PORTOCALA", "BANANA", "ANANAS", "CAPSUNA", "PEPENE", "LAMAIE", "PIERSICA", "CAISA", "GUTUIE", "AFINE" } },
                { "Programming", new List<string> { "CSHARP", "PYTHON", "JAVASCRIPT", "RUBY", "KOTLIN", "SWIFT", "TYPESCRIPT", "PHP", "FORTRAN", "HASKELL" } },
                { "Astronomy", new List<string> { "PLANETA", "GALAXIE", "COMETA", "ASTEROID", "NEBULOASA", "TELESCOP", "SATURN", "UNIVERS", "CONSTELATIE", "ORBITA" } },
                { "Instruments", new List<string> { "CHITARA", "PIAN", "VIOARA", "TROMPETA", "SAXOFON", "TOBE", "HARPA", "FLAUT", "ACORDEON", "VIOLONCEL" } },
                { "Inventions", new List<string> { "TELEFON", "BEC", "RADIO", "MOTOR", "AVION", "MICROSCOP", "TIPAR", "TELEVIZOR", "COMPAS", "TERMOMETRU" } },
                { "Mythology", new List<string> { "ZEUS", "HERCULE", "CENTAUR", "FENIX", "DRAGON", "MINOTAUR", "NEPTUN", "PEGAS", "AFRODITA", "OLIMP" } },
                { "Vegetables", new List<string> { "DOVLEAC", "CONOPIDA", "VINETE", "CARTOF", "MORCOV", "ARDEI", "SPANAC", "BROCCOLI", "CEAPA", "USTUROI" } }

            };

            

            if (category == "All categories")
            {
                var allWords = words.Values.SelectMany(w => w).ToList();
                return allWords[new System.Random().Next(allWords.Count)];

            }

            if (words.ContainsKey(category))
            {
                var list = words[category];
                return list [new System.Random().Next(list.Count)];
            }

            return "UNKNOWN";


        }

    }
}
