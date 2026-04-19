using Hangman.Models;
using Hangman.Services;
using Hangman.Utilities;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Hangman.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private User _selectedUser;
        private string _newUserName;

        public ObservableCollection<User> Users { get; set; }

        public User SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        public string NewUserName
        {
            get => _newUserName;
            set { _newUserName = value; OnPropertyChanged(); }
        }

        public RelayCommand PlayCommand { get; }
        public RelayCommand DeleteUserCommand { get; }
        public RelayCommand NewUserCommand { get; }
        public RelayCommand NextUserCommand { get; }
        public RelayCommand PreviousUserCommand { get; }
        public RelayCommand ExitCommand { get; }

        public LoginViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Users = new ObservableCollection<User>(DataStore.LoadUsers());

            PlayCommand = new RelayCommand(o => Play(), o => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(o => DeleteUser(), o => SelectedUser != null);
            NewUserCommand = new RelayCommand(o => AddUser(), o => !string.IsNullOrWhiteSpace(NewUserName));
            ExitCommand = new RelayCommand(o => Application.Current.Shutdown());

            NextUserCommand = new RelayCommand(o => NavigateUser(1), o => Users.Count > 0);
            PreviousUserCommand = new RelayCommand(o => NavigateUser(-1), o => Users.Count > 0);

            if (Users.Count > 0 && SelectedUser == null) SelectedUser = Users[0];
        }

        private void NavigateUser(int direction)
        {
            if (Users.Count == 0) return;
            int currentIndex = Users.IndexOf(SelectedUser);
            int nextIndex = (currentIndex + direction + Users.Count) % Users.Count;
            SelectedUser = Users[nextIndex];
        }

        private void Play()
        {
            _mainViewModel.CurrentViewModel = new GameViewModel(_mainViewModel, SelectedUser);
        }

        private void DeleteUser()
        {
            var user = SelectedUser;
            Users.Remove(user);
            DataStore.SaveUsers(Users.ToList());
            if (Users.Count > 0) SelectedUser = Users[0];

            var saves = DataStore.LoadSaves();
            saves.RemoveAll(s => s.UserName == user.Name);
            DataStore.SaveGames(saves);

            var stats = DataStore.LoadStats();
            stats.RemoveAll(s => s.UserName == user.Name);
            DataStore.SaveStats(stats);
        }

        private void AddUser()
        {
            var dialog = new OpenFileDialog { Filter = "Image Files|*.jpg;*.gif;*.png" };
            string imagePath = "default.png";
            if (dialog.ShowDialog() == true) imagePath = dialog.FileName;

            var user = new User { Name = NewUserName, ImagePath = imagePath };
            Users.Add(user);
            DataStore.SaveUsers(Users.ToList());
            SelectedUser = user;
            NewUserName = string.Empty;
        }
    }
}