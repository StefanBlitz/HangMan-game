using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman.Models
{
    public class UserStatistic
    {
        public string UserName { get; set; }
        public string Category { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
    }
}
