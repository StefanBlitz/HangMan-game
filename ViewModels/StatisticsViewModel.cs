using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangman.Utilities;
using Hangman.Services;
using Hangman.Models;
using System.Collections.ObjectModel;

namespace Hangman.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

        public ObservableCollection<UserStatistic> Statistics { get; set; }
        public RelayCommand BackCommand { get; }

        public StatisticsViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Statistics = new ObservableCollection<UserStatistic>(DataStore.LoadStats());
            BackCommand = new RelayCommand(o => _mainViewModel.CurrentViewModel = new LoginViewModel(_mainViewModel));
        }

    }
}
