﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EsportManager
{
    /// <summary>
    /// Interakční logika pro LoadGame.xaml
    /// </summary>
    public partial class LoadGame : Window
    {
        public LoadGame()
        {
            InitializeComponent();
            LoadGames();
            LoadGameButton.IsEnabled = false;
        }

        private void LoadGames()
        {
            string[] files = System.IO.Directory.GetFiles("./", "*.db");
            for (int i = 0; i < files.Length; i++)
            {
                GamesLB.Items.Add((files[i].Remove(files[i].Length - 3)).Substring(2)); //za to dopsat aktuální datum + tým, za který se hraje
            }
        }

        private void LoadGameClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void GameChange(object sender, SelectionChangedEventArgs e)
        {
            if (GamesLB.SelectedIndex == -1)
            {
                LoadGameButton.IsEnabled = false;
            }
            else
            {
                LoadGameButton.IsEnabled = true;
            }
        }
    }
}
