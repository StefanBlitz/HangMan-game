using Hangman.Models;
using Hangman.Services;
using Hangman.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Hangman.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private readonly User _currentUser;
        private DispatcherTimer _timer;

        private string _targetWord;
        private string _displayWord;
        private int _mistakes;
        private int _timeLeft;
        private int _level;
        private string _selectedCategory = "All categories";
        private ObservableCollection<string> _lifeIndicators;

        private bool _isLoadPanelVisible;
        private ObservableCollection<GameState> _userSaves;
        private GameState _selectedSave;

        public ObservableCollection<LetterItem> Letters { get; set; }
        public List<string> Categories { get; set; } = new List<string> { "All categories", "Animals", "Fruits", "Programming", "Astronomy", "Instruments", "Inventions", "Mythology", "Vegetables" };

        public ObservableCollection<string> LifeIndicators { get => _lifeIndicators; set { _lifeIndicators = value; OnPropertyChanged(); } }
        public string SelectedCategory { get => _selectedCategory; set { if (_selectedCategory != value) { _selectedCategory = value; OnPropertyChanged(); ResetLevel(); } } }
        public string DisplayWord { get => _displayWord; set { _displayWord = value; OnPropertyChanged(); } }
        public int Mistakes { get => _mistakes; set { _mistakes = value; OnPropertyChanged(); OnPropertyChanged(nameof(HangmanImage)); } }
        public int TimeLeft { get => _timeLeft; set { _timeLeft = value; OnPropertyChanged(); } }
        public int Level { get => _level; set { _level = value; OnPropertyChanged(); } }
        public User CurrentUser => _currentUser;
        public string HangmanImage => $"/Images/hangman_{Mistakes}.png";

        public bool IsLoadPanelVisible { get => _isLoadPanelVisible; set { _isLoadPanelVisible = value; OnPropertyChanged(); } }
        public ObservableCollection<GameState> UserSaves { get => _userSaves; set { _userSaves = value; OnPropertyChanged(); } }
        public GameState SelectedSave { get => _selectedSave; set { _selectedSave = value; OnPropertyChanged(); } }

        public RelayCommand GuessCommand { get; }
        public RelayCommand NewGameCommand { get; }
        public RelayCommand SaveGameCommand { get; }
        public RelayCommand OpenLoadPanelCommand { get; }
        public RelayCommand LoadSelectedSaveCommand { get; }
        public RelayCommand CloseLoadPanelCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand AboutCommand { get; }
        public RelayCommand ShowStatsCommand { get; }

        public GameViewModel(MainViewModel mainViewModel, User user)
        {
            _mainViewModel = mainViewModel;
            _currentUser = user;
            Letters = new ObservableCollection<LetterItem>();
            for (char c = 'A'; c <= 'Z'; c++) Letters.Add(new LetterItem { Character = c, IsEnabled = true });

            GuessCommand = new RelayCommand(GuessLetter);
            NewGameCommand = new RelayCommand(o => { if (o is string cat) SelectedCategory = cat; else ResetLevel(); });
            SaveGameCommand = new RelayCommand(o => SaveGame());

            OpenLoadPanelCommand = new RelayCommand(o => OpenLoadPanel());
            LoadSelectedSaveCommand = new RelayCommand(o => LoadGame(), o => SelectedSave != null);
            CloseLoadPanelCommand = new RelayCommand(o => IsLoadPanelVisible = false);

            CancelCommand = new RelayCommand(o => _mainViewModel.CurrentViewModel = new LoginViewModel(_mainViewModel));
            AboutCommand = new RelayCommand(o => MessageBox.Show("Nume: Oiste Stefan-Vlad\nGrupa: 10LF243\nSpecializare: Info\nAnul: 2", "About"));
            ShowStatsCommand = new RelayCommand(o => ShowStats());

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;
            StartNewGame();
        }

        private void ResetLevel() { Level = 0; StartNewGame(); }

        private void StartNewGame()
        {
            _timer.Stop();
            _targetWord = DataStore.GetRandomWord(SelectedCategory);
            DisplayWord = new string('_', _targetWord.Length);
            Mistakes = 0;
            TimeLeft = 30;
            LifeIndicators = new ObservableCollection<string> { " ", " ", " ", " ", " ", " " };
            foreach (var l in Letters) l.IsEnabled = true;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e) { TimeLeft--; if (TimeLeft <= 0) { _timer.Stop(); EndGame(false); } }

        private void GuessLetter(object parameter)
        {
            if (parameter is char c)
            {
                var letterItem = Letters.FirstOrDefault(l => l.Character == c);
                if (letterItem == null || !letterItem.IsEnabled) return;
                letterItem.IsEnabled = false;
                if (_targetWord.Contains(c))
                {
                    char[] chars = DisplayWord.ToCharArray();
                    for (int i = 0; i < _targetWord.Length; i++) if (_targetWord[i] == c) chars[i] = c;
                    DisplayWord = new string(chars);
                    if (!DisplayWord.Contains('_')) { _timer.Stop(); Level++; if (Level >= 3) EndGame(true); else StartNewGame(); }
                }
                else { if (Mistakes < 6) LifeIndicators[Mistakes] = "X"; Mistakes++; if (Mistakes >= 6) { _timer.Stop(); EndGame(false); } }
            }
        }

        private void EndGame(bool isWin)
        {
            UpdateStats(isWin);
            if (isWin) MessageBox.Show("Victorie!");
            else MessageBox.Show("Game Over!");
            Level = 0; StartNewGame();
        }

        private void UpdateStats(bool isWin)
        {
            var stats = DataStore.LoadStats();
            var s = stats.FirstOrDefault(st => st.UserName == _currentUser.Name && st.Category == SelectedCategory);
            if (s == null) { s = new UserStatistic { UserName = _currentUser.Name, Category = SelectedCategory }; stats.Add(s); }
            s.GamesPlayed++; if (isWin) s.GamesWon++;
            DataStore.SaveStats(stats);
        }

        private void SaveGame()
        {
            var saves = DataStore.LoadSaves();
            string saveName = $"Save_{DateTime.Now:dd.MM_HH:mm:ss}";
            var disabled = string.Join("", Letters.Where(l => !l.IsEnabled).Select(l => l.Character));

            saves.Add(new GameState
            {
                SaveName = saveName,
                UserName = _currentUser.Name,
                Level = Level,
                Mistakes = Mistakes,
                TimeLeft = TimeLeft,
                Category = SelectedCategory,
                TargetWord = _targetWord,
                CurrentWordState = DisplayWord,
                DisabledLetters = disabled
            });
            DataStore.SaveGames(saves);
            MessageBox.Show("Joc salvat!");
        }

        private void OpenLoadPanel()
        {
            var allSaves = DataStore.LoadSaves();
            
            UserSaves = new ObservableCollection<GameState>(allSaves.Where(s => s.UserName == _currentUser.Name));
            IsLoadPanelVisible = true;
        }

        private void LoadGame()
        {
            if (SelectedSave == null) return;
            _timer.Stop();
            SelectedCategory = SelectedSave.Category;
            Level = SelectedSave.Level;
            Mistakes = SelectedSave.Mistakes;
            TimeLeft = SelectedSave.TimeLeft;
            _targetWord = SelectedSave.TargetWord;
            DisplayWord = SelectedSave.CurrentWordState;
            LifeIndicators = new ObservableCollection<string> { " ", " ", " ", " ", " ", " " };
            for (int i = 0; i < Mistakes; i++) LifeIndicators[i] = "X";
            foreach (var l in Letters) l.IsEnabled = !SelectedSave.DisabledLetters.Contains(l.Character);
            IsLoadPanelVisible = false;
            _timer.Start();
        }

        private void ShowStats() { _mainViewModel.CurrentViewModel = new StatisticsViewModel(_mainViewModel); }
    }
}