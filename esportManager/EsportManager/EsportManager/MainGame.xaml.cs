﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EsportManager
{
    /// <summary>
    /// Interakční logika pro MainGame.xaml
    /// </summary>
    public partial class MainGame : Window
    {

        class MatchDataGrid
        {
            public string Datum { get; set; }
            public string Souper { get; set; }
            public string Turnaj { get; set; }
            public string Misto { get; set; }
        }

        class PlayersDataGrid
        {
            public string Pozice { get; set; }
            public string Nick { get; set; }
            public string Jmeno { get; set; }
            public string Prijmeni { get; set; }
            public int E { get; set; }
        }
        public string DatabaseName { get; set; }
        List<TeamSectionBasic> sectionsList = new List<TeamSectionBasic>();
        string date;
        int idTeam;
        int todaysMatchTeam;
        int matchTeamSection;
        public MainGame(string database)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DatabaseName = database;
            InitializeComponent();
            SetLabels();
            AddSectionsToList();
            SetTabs();
            AddAllMatches();
            AddAllPlayers();
            ChangePropertiesOfNextActionButton();
            NonPlayerTeamsActions();
        }

        private void NonPlayerTeamsActions()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                /*conn.Open();
                SQLiteCommand command = new SQLiteCommand("select date from info;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                string curDate = reader.GetString(0);
                //(u hráčů, kteří nepatří do týmu se na 50 % prodlouží smlouva, zbytku ne)
                command = new SQLiteCommand("select player.id_player, player.contractEnd, player.individualSkill, player.teamplaySkill, team.id_team from player join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where team.id_team=" + idTeam + ";", conn);
            */
            }
        }

        private void SetLabels()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_team, date from info;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                idTeam = reader.GetInt32(0);
                date = reader.GetString(1);
                DateLabel.Content = TransformDate(date);
                reader.Close();
                command = new SQLiteCommand("select name, budget, logo from team where id_team=" + idTeam + ";", conn);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TeamNameLabel.Content = reader.GetString(0);
                    BudgetLabel.Content = reader.GetInt32(1) + "$";
                    byte[] data = (byte[])reader[2];
                    BitmapImage imageSource = new BitmapImage();
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        imageSource.BeginInit();
                        imageSource.StreamSource = ms;
                        imageSource.CacheOption = BitmapCacheOption.OnLoad;
                        imageSource.EndInit();
                    }


                    Logo.Source = imageSource;
                }
                reader.Close();
            }
        }

        private string TransformDate(string v)
        {
            string year = v.Remove(4, 6);
            string month = v.Remove(7, 3).Remove(0, 5);
            string day = v.Remove(0, 8);
            return day + ". " + month + ". " + year;
        }

        private string NextDay(string v)
        {
            int day = int.Parse(v.Remove(0, 8));
            int month = int.Parse(v.Remove(7, 3).Remove(0, 5));
            if ((day == 30 && (month == 4 || month == 6 || month == 9 || month == 11)) || day == 31 || (day == 28 && month == 2))
            {
                day = 1;
                month++;
            }
            else
            {
                day++;
            }

            string dayString = day.ToString();
            if (dayString.Length == 1)
            {
                dayString = "0" + dayString;
            }
            string monthString = month.ToString();
            if (monthString.Length == 1)
            {
                monthString = "0" + monthString;
            }
            return v.Remove(5, 5) + monthString + "-" + dayString;
        }

        private void ChangePropertiesOfNextActionButton()
        {
            NextActionButton.Click -= SponsorContractExpired;
            NextActionButton.Click -= NextMonthClick;
            NextActionButton.Click -= PlayerContractExpired;
            NextActionButton.Click -= NextDayClick;
            NextActionButton.Click -= EndYearClick;
            NextActionButton.Click -= PlayMatch;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select date from info;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                string curDate = reader.GetString(0);
                reader.Close();
                // hledání hráčů, kterým končí smlouvy v týmu!!!!  
                command = new SQLiteCommand("select player.contractEnd from player join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where team.id_team=" + idTeam + ";", conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.GetString(0).CompareTo(curDate) <= 0)
                    {
                        NextActionButton.Content = "hráči končí smlouva";
                        NextActionButton.Click += PlayerContractExpired;
                        return;
                    }
                }
                reader.Close();
                // končí zaměstnanci smlouva

                // končí sponzorská smlouva
                int yearCurrent = int.Parse(date.Remove(4, 6));
                int monthCurrent = int.Parse(date.Remove(7, 3).Remove(0, 5));
                int dayCurrent = int.Parse(date.Remove(0, 8));
                command = new SQLiteCommand("select teamxsponsor.expiration_date from sponsor2 join teamxsponsor on sponsor2.id_sponsor=teamxsponsor.id_sponsor where teamxsponsor.id_team=" + idTeam + ";", conn);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int yearExpire = int.Parse(reader.GetString(0).Remove(4, 6));
                    int monthExpire = int.Parse(reader.GetString(0).Remove(7, 3).Remove(0, 5));
                    int dayExpire = int.Parse(reader.GetString(0).Remove(0, 8));
                    if (yearExpire == yearCurrent && monthCurrent == monthExpire && dayExpire == dayCurrent)
                    {
                        NextActionButton.Content = "končí sponzorská smlouva";
                        NextActionButton.Click += SponsorContractExpired;
                        return;
                    }
                }
                reader.Close();
                // je na řadě zápas
                for (int i = 0; i < sectionsList.Count; i++)
                {
                    command = new SQLiteCommand("select count(*) from '2019match1' where match_date='" + curDate + "' and (id_teamxsection_home=" + sectionsList.ElementAt(i).ID + " or id_teamxsection_away=" + sectionsList.ElementAt(i).ID + ") and home_score is null", conn);
                    reader = command.ExecuteReader();
                    reader.Read();
                    if (reader.GetInt32(0) > 0)
                    {
                        // otevřít zápas
                        NextActionButton.Content = "Zápas";
                        NextActionButton.Click += PlayMatch;
                        todaysMatchTeam = sectionsList.ElementAt(i).ID;
                        matchTeamSection = sectionsList.ElementAt(i).sectionID;
                        return;
                    }
                    reader.Close();
                }


                // nic se neděje - konec roku
                if (date.Remove(0, 5) == "31-12")
                {
                    NextActionButton.Content = "ukončit rok";
                    NextActionButton.Click += EndYearClick;
                    return;
                }
                //nic se neděje - konec měsíce
                int month = int.Parse(date.Remove(7, 3).Remove(0, 5));
                int day = int.Parse(date.Remove(0, 8));
                if ((day == 30 && (month == 4 || month == 6 || month == 9 || month == 11)) || day == 31 || (day == 28 && month == 2))
                {
                    NextActionButton.Content = "další měsíc";
                    NextActionButton.Click += NextMonthClick;
                    return;
                }
                // nic se neděje - další den
                NextActionButton.Content = "další den";
                NextActionButton.Click += NextDayClick;
                return;

            }
            /* hledání, co se vlastně bude dít.. možnosti:
               1 - nic se neděje - a - další den
                                 - b - konec roku
               2 - je na řadě zápas, takže button posune na okno se zápasem
               3 - končí sponzorská smlouva, vyjede okno, jestli chceme přesunout na smlouvy
               4 - končí hráčovi smlouva, vyjede okno, jestli chceme přesunout na přehled hráčů     DONE
               5 - končí zaměstnanci smlouva, vyjede okno, jestli chceme přesunout na 
            */
        }

        private void PlayMatch(object sender, RoutedEventArgs e)
        {
            Match win2 = new Match(DatabaseName, todaysMatchTeam, matchTeamSection);
            win2.Show();
        }

        private void SponsorContractExpired(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Nejméně jedna smlouva se sponzorem dnes končí, kliknutím na Ano skončí se sponzorem/sponzory spolupráce.", "Končí smlouva se sponzorem", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("delete from teamxsponsor where id_team=" + idTeam + " and expiration_date='" + date + "';", conn);
                    command.ExecuteReader();
                    ChangePropertiesOfNextActionButton();
                }
            }
        }

        private void NextMonthClick(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select sum(monthly_payment) from sponsor2 join teamxsponsor on teamxsponsor.id_sponsor=sponsor2.id_sponsor and teamxsponsor.id_team=" + idTeam + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                int income = 0;
                if (reader[0].GetType() != typeof(DBNull))
                {
                     income = reader.GetInt32(0);
                }
                reader.Close();
                command = new SQLiteCommand("select sum(salary) from player join teamxsection on teamxsection.id_team=player.team_fk and teamxsection.id_team=" + idTeam + ";", conn);
                reader = command.ExecuteReader();
                reader.Read();
                income -= reader.GetInt32(0);
                Console.WriteLine(income);
                reader.Close();
                command = new SQLiteCommand("update team set budget=budget+" + income + " where id_team=" + idTeam + ";", conn);
                command.ExecuteReader();
            }
            NextDayClick(sender, e);
            SetLabels();
        }

        private void NextDayClick(object sender, RoutedEventArgs e)
        {
            // odehrávání zápasů
            PlayAllOtherMatches();
            // losování turnajů
            int curYear = int.Parse(date.Remove(4, 6));
            int curMonth = int.Parse(date.Remove(7, 3).Remove(0, 5));
            int curDay = int.Parse(date.Remove(0, 8));
            List<int> tournamentsToDraw = new List<int>();
            int tournamentYear;
            int tournamentMonth;
            int tournamentDay;
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id_tournament, start_date from tournament where start_date>'" + date + "' and drawn=0;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tournamentYear = int.Parse(reader.GetString(1).Remove(4, 6));
                    tournamentMonth = int.Parse(reader.GetString(1).Remove(7, 3).Remove(0, 5));
                    tournamentDay = int.Parse(reader.GetString(1).Remove(0, 8));
                    if (tournamentMonth <= curMonth + 1)
                    {
                        tournamentsToDraw.Add(reader.GetInt32(0));
                    }
                }
                reader.Close();
            }
            //už vím turnaje, které jsou potřeba nalosovat
            //DrawTournaments(tournamentsToDraw);

            // další den
            date = NextDay(date);
            DateLabel.Content = TransformDate(date);
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("update info set date='" + date + "';", conn);
                command.ExecuteReader();
            }
            ChangePropertiesOfNextActionButton();
        }

        private void PlayAllOtherMatches()
        {
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                int year = int.Parse(date.Substring(0, 4));
                conn.Open();
                // PŘEDĚLAT SEKCE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                SQLiteCommand command = new SQLiteCommand("select id_match, m.id_teamxsection_home, m.id_teamxsection_away, tour.games_best_of from '" + year + "match1' m join teamxsection a on a.id_teamxsection = m.id_teamxsection_home join teamxsection b on b.id_teamxsection = m.id_teamxsection_away join tournament tour on tour.id_tournament = m.id_tournament where match_date = '" + date + "'", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                int homeStrength, awayStrength, idMatch, homeTeam, awayTeam, bestOf, homeScore, awayScore;
                while (reader.Read())
                {
                    idMatch = reader.GetInt32(0);
                    homeTeam = reader.GetInt32(1);
                    awayTeam = reader.GetInt32(2);
                    bestOf = reader.GetInt32(3);
                    SQLiteCommand command2 = new SQLiteCommand("select sum(individualSkill + teamplaySkill + playerCoop) / count(*) as i from player where team_fk=" + homeTeam + ";", conn);
                    SQLiteDataReader reader2 = command2.ExecuteReader();
                    reader2.Read();
                    homeStrength = reader2.GetInt32(0);
                    reader2.Close();
                    command2 = new SQLiteCommand("select sum(individualSkill + teamplaySkill + playerCoop) / count(*) as i from player where team_fk=" + awayTeam + ";", conn);
                    reader2 = command2.ExecuteReader();
                    reader2.Read();
                    awayStrength = reader2.GetInt32(0);
                    reader2.Close();
                    homeScore = 0;
                    awayScore = 0;
                    while (homeScore < bestOf && awayScore < bestOf)
                    {
                        // tady nějaké výpočty a propočty
                        if (awayStrength > homeStrength)
                        {
                            awayScore++;

                        }
                        else
                        {
                            homeScore++;
                        }
                    }
                    command2 = new SQLiteCommand("update '" + year + "match1' set home_score=" + homeScore + ", away_score=" + awayScore + " where id_match=" + idMatch + ";", conn);
                    command2.ExecuteReader();
                }
                reader.Close();
            }
        }

        private void DrawTournaments(List<int> tournamentsToDraw)
        {
            for (int tourToDraw = 0; tourToDraw < tournamentsToDraw.Count; tourToDraw++)
            {

            
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select start_date, playing_days, n_of_teams, end_date, game, system from tournament where id_tournament=" + tournamentsToDraw.ElementAt(tourToDraw) + ";", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                int game = reader.GetInt32(4);
                int startYear = int.Parse(reader.GetString(0).Remove(4, 6));
                int startMonth = int.Parse(reader.GetString(0).Remove(7, 3).Remove(0, 5));
                int startDay = int.Parse(reader.GetString(0).Remove(0, 8));
                int endYear = int.Parse(reader.GetString(3).Remove(4, 6));
                int endMonth = int.Parse(reader.GetString(3).Remove(7, 3).Remove(0, 5));
                int endDay = int.Parse(reader.GetString(3).Remove(0, 8));
                DateTime startDate = new DateTime(startYear, startMonth, startDay);
                DateTime endDate = new DateTime(endYear, endMonth, endDay);
                int weeks = (int)Math.Ceiling((endDate - startDate).TotalDays / 7);
                string playingDays = reader.GetString(1);
                int system = reader.GetInt32(5);
                int teams = reader.GetInt32(2);
                bool oddTeams = teams % 2 == 1;
                if (oddTeams)
                {
                    teams++;
                }
                reader.Close();
                int games = (teams - 1) * teams / 2; //single round robin, double až po losu
                int[,] drawTeams = new int[teams / 2, 2];
                // single round robin
                for (int i = 0; i < teams / 2; i++)
                {
                    drawTeams[i, 0] = i;
                    drawTeams[i, 1] = i + teams / 2;
                }
                // [počet kol,počet zápasů v kole]
                int[,] draw = new int[teams - 1, teams];
                for (int i = 0; i < teams - 1; i++)
                {
                    for (int j = 0; j < teams; j++)
                    {
                        if (j % 2 == 0)
                        {
                            draw[i, j] = drawTeams[j / 2, 0];
                        }
                        else
                        {
                            draw[i, j] = drawTeams[j / 2, 1];
                        }
                    }
                    int temp = drawTeams[0, 1];
                    for (int j = 0; j < teams / 2 - 1; j++)
                    {
                        drawTeams[j, 1] = drawTeams[j + 1, 1];
                    }
                    drawTeams[teams / 2 - 1, 1] = drawTeams[teams / 2 - 1, 0];
                    for (int j = teams / 2 - 1; j > 1; j--)
                    {
                        drawTeams[j, 0] = drawTeams[j - 1, 0];
                    }
                    drawTeams[1, 0] = temp;
                }
                for (int i = 0; i < teams - 1; i++)
                {
                    if (i % 2 == 1)
                    {
                        for (int j = 0; j < teams / 2; j++)
                        {
                            int temp = draw[i, j * 2];
                            draw[i, j * 2] = draw[i, j * 2 + 1];
                            draw[i, j * 2 + 1] = temp;
                        }
                    }
                }

                // je double round robin
                int[,] finalDraw = null;
                if (system == 2)
                {
                    finalDraw = new int[(teams - 1) * teams, 2];
                    int counter = 0;
                    for (int i = 0; i < teams - 1; i++)
                    {
                        for (int j = 0; j < teams; j=j+2)
                        {
                            finalDraw[counter, 0] = draw[i, j];
                            finalDraw[counter, 1] = draw[i, j + 1];
                            finalDraw[counter + (teams - 1) * teams/2, 0] = draw[i, j + 1];
                            finalDraw[counter + (teams - 1) * teams / 2, 1] = draw[i, j];
                            counter++;
                        }
                    }
                }
                else
                {
                    finalDraw = new int[(teams - 1) * teams / 2, 2];
                    int counter = 0;
                    for (int i = 0; i < teams - 1; i++)
                    {
                        for (int j = 0; j < teams / 2; j++)
                        {
                            finalDraw[counter, 0] = draw[i, j];
                            finalDraw[counter, 1] = draw[i, j + 1];
                            counter++;
                        }
                    }
                }
                command = new SQLiteCommand("select id_teamxsection from tournament_token where id_tournament_to=" + tournamentsToDraw.ElementAt(tourToDraw) + " order by seed", conn);
                reader = command.ExecuteReader();
                int[] realIds = new int[teams];
                for (int i = 0; i < teams; i++)
                {
                    reader.Read();
                    realIds[i] = reader.GetInt32(0);
                }
                reader.Close();
                DateTime date = startDate;
                int[] daysPlaying = new int[7];
                for (int i = 0; i < daysPlaying.Length; i++)
                {
                    daysPlaying[i] = int.Parse(playingDays.Substring(i, 1));
                }
                int matchCounter = 0;
                while (date.CompareTo(endDate) <= 0)
                {
                    int dayOfWeek;
                    switch (date.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            dayOfWeek = 6;
                            break;
                        case DayOfWeek.Monday:
                            dayOfWeek = 0;
                            break;
                        case DayOfWeek.Tuesday:
                            dayOfWeek = 1;
                            break;
                        case DayOfWeek.Wednesday:
                            dayOfWeek = 2;
                            break;
                        case DayOfWeek.Thursday:
                            dayOfWeek = 3;
                            break;
                        case DayOfWeek.Friday:
                            dayOfWeek = 4;
                            break;
                        case DayOfWeek.Saturday:
                            dayOfWeek = 5;
                            break;
                        default:
                            dayOfWeek = -1;
                            break;
                    }
                    if (daysPlaying[dayOfWeek] != 0)
                    {
                        int matchesInDay = daysPlaying[dayOfWeek];
                        string separator = "-";
                        string separator2 = "-";
                        if (date.Month < 10)
                        {
                            separator += "0";
                        }
                        if (date.Day < 10)
                        {
                            separator2 += "0";
                        }
                        string stringDate = date.Year + separator + date.Month + separator2 + date.Day;
                        for (int i = 0; i < matchesInDay; i++)
                        {
                            command = new SQLiteCommand("insert into '" + date.Year + "match" + game + "' ('id_teamxsection_home', 'id_teamxsection_away', 'match_date', 'id_tournament') values (" + realIds[finalDraw[matchCounter, 0]] + "," + realIds[finalDraw[matchCounter, 1]] + ",'" + stringDate + "'," + tournamentsToDraw.ElementAt(tourToDraw) + ");", conn);
                            command.ExecuteReader();
                            matchCounter++;
                        }
                    }
                    date = date.AddDays(1);
                }
                command = new SQLiteCommand("update tournament set drawn=1 where id_tournament=" + tournamentsToDraw.ElementAt(tourToDraw) + ";", conn);
                command.ExecuteReader();
                    Console.WriteLine("HOTOVO " + tourToDraw);
            }
            }
        }


        private void EndYearClick(object sender, RoutedEventArgs e)
        {
            ChangePropertiesOfNextActionButton();
            SetLabels();
        }

        private void PlayerContractExpired(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Nejméně jednomu vašemu hráči končí smlouva, kliknutím na Ano tým opustí.", "Hráčům končí smlouva", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
                {
                    conn.Open();
                    SQLiteCommand command = new SQLiteCommand("select player.id_player from player join teamxsection on player.team_fk=teamxsection.id_teamxsection join team on team.id_team=teamxsection.id_team where team.id_team=" + idTeam + " and contractEnd<='" + date + "';", conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    string comm = "";
                    while (reader.Read())
                    {
                        comm += "update player set value=0, salary=0, team_fk=NULL, contractEnd='' where id_player=" + reader.GetInt32(0) + ";";
                        /*SQLiteCommand command2 = new SQLiteCommand("update player set value=0, salary=0, team_fk=NULL, contractEnd='' where id_player=" + reader.GetInt32(0) + ";", conn);
                        command2.ExecuteReader();*/
                    }
                    reader.Close();
                    command = new SQLiteCommand(comm, conn);
                    command.ExecuteReader();
                    AddAllPlayers();
                    //hráči jsou volní
                    ChangePropertiesOfNextActionButton();
                }
            }
        }

        private void AddAllPlayers()
        {

            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                for (int i = 0; i < sectionsList.Count; i++)
                {
                    List<PlayersDataGrid> players = new List<PlayersDataGrid>();
                    SQLiteCommand command = new SQLiteCommand("select nick, player.name, surname, position_type.name, energy from player join position_type on position_type.id_position_in_game=player.position and position_type.id_section=player.game where team_fk=" + sectionsList.ElementAt(i).ID, conn);
                    SQLiteDataReader reader = command.ExecuteReader();
                    string nick, jmeno, prijmeni, pozice;
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                        {
                            nick = "";
                        }
                        else
                        {
                            nick = reader.GetString(0);
                        }
                        if (reader.IsDBNull(1))
                        {
                            jmeno = "";
                        }
                        else
                        {
                            jmeno = reader.GetString(1);
                        }
                        if (reader.IsDBNull(2))
                        {
                            prijmeni = "";
                        }
                        else
                        {
                            prijmeni = reader.GetString(2);
                        }
                        if (reader.IsDBNull(3))
                        {
                            pozice = "";
                        }
                        else
                        {
                            pozice = reader.GetString(3);
                        }
                        //players.Add(new PlayersDataGrid() { Nick = reader.GetString(0), Jmeno = reader.GetString(1), Prijmeni = reader.GetString(2), Pozice = reader.GetString(3) });
                        players.Add(new PlayersDataGrid() { Pozice = pozice, Nick = nick, Jmeno = jmeno, Prijmeni = prijmeni, E = reader.GetInt32(4) });
                    }

                    reader.Close();
                    command = new SQLiteCommand("select nick, coach.name, surname, team_fk from coach join teamxsection on teamxsection.id_team=coach.team_fk where teamxsection.id_teamxsection=" + sectionsList.ElementAt(i).ID + " and game=" + sectionsList.ElementAt(i).sectionID + ";", conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        players.Add(new PlayersDataGrid() { Pozice = "Trenér", Nick = reader.GetString(0), Jmeno = reader.GetString(1), Prijmeni = reader.GetString(2), E = 100 });
                    }
                    switch (i)
                    {
                        case 0:
                            Section1PlayersList.ItemsSource = players;
                            break;
                        case 1:
                            Section2PlayersList.ItemsSource = players;
                            break;
                        case 2:
                            Section3PlayersList.ItemsSource = players;
                            break;
                        case 3:
                            Section4PlayersList.ItemsSource = players;
                            break;
                        case 4:
                            Section5PlayersList.ItemsSource = players;
                            break;
                        case 5:
                            Section6PlayersList.ItemsSource = players;
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        private void AddAllMatches()
        {

            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command;
                SQLiteDataReader reader;
                string year = date.Remove(4, 6);

                for (int i = 0; i < sectionsList.Count; i++)
                {
                    List<MatchDataGrid> tournaments = new List<MatchDataGrid>();
                    string matchTableName = year + "match" + sectionsList.ElementAt(i).sectionID;
                    command = new SQLiteCommand("select tournament.shortcut, match_date, id_teamxsection_home, id_teamxsection_away, tournament.city_fk from '" + matchTableName + "' join tournament on tournament.id_tournament='" + matchTableName + "'.id_tournament where id_teamxsection_home=" + sectionsList.ElementAt(i).ID + " or id_teamxsection_away=" + sectionsList.ElementAt(i).ID + ";", conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SQLiteCommand command2;
                        if (reader.GetInt32(2) == sectionsList.ElementAt(i).ID)
                        {
                            command2 = new SQLiteCommand("select shortcut from team join teamxsection on team.id_team=teamxsection.id_team where teamxsection.id_teamxsection=" + reader.GetInt32(3), conn);
                        }
                        else
                        {
                            command2 = new SQLiteCommand("select shortcut from team join teamxsection on team.id_team=teamxsection.id_team where teamxsection.id_teamxsection=" + reader.GetInt32(2), conn);
                        }
                        SQLiteDataReader reader2 = command2.ExecuteReader();
                        reader2.Read();
                        tournaments.Add(new MatchDataGrid() { Turnaj = reader.GetString(0), Datum = reader.GetString(1), Misto = reader.GetInt32(4).ToString(), Souper = reader2.GetString(0) });
                    }
                    switch (i)
                    {
                        case 0:
                            Section1TournamentsList.ItemsSource = tournaments;
                            break;
                        case 1:
                            Section2TournamentsList.ItemsSource = tournaments;
                            break;
                        case 2:
                            Section3TournamentsList.ItemsSource = tournaments;
                            break;
                        case 3:
                            Section4TournamentsList.ItemsSource = tournaments;
                            break;
                        case 4:
                            Section5TournamentsList.ItemsSource = tournaments;
                            break;
                        case 5:
                            Section6TournamentsList.ItemsSource = tournaments;
                            break;
                        default:
                            break;
                    }
                }
            }


        }

        private void AddSectionsToList()
        {
            sectionsList.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select teamxsection.id_teamxsection, section.id_section, name from section join teamxsection on teamxsection.id_section=section.id_section where id_team=" + idTeam + " order by section.id_section;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                int sectionBefore = -1;
                while (reader.Read())
                {
                    if (sectionBefore == reader.GetInt32(1))
                    {
                        //je to B tým
                        sectionsList.Add(new TeamSectionBasic(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2) + " B"));
                    }
                    else
                    {
                        //je to A tým
                        sectionsList.Add(new TeamSectionBasic(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2)));
                    }

                    sectionBefore = reader.GetInt32(1);
                }
                reader.Close();
            }
        }

        private void SetTabs()
        {
            int numberOfSections = GetNumberOfSections() + 1;
            SectionTabs.Items.Clear();
            SectionTabs.Items.Add(Section1);
            SectionTabs.Items.Add(Section2);
            SectionTabs.Items.Add(Section3);
            SectionTabs.Items.Add(Section4);
            SectionTabs.Items.Add(Section5);
            SectionTabs.Items.Add(Section6);
            while (numberOfSections < SectionTabs.Items.Count)
            {
                SectionTabs.Items.RemoveAt(numberOfSections);
            }
            switch (numberOfSections)
            {
                case 2:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabAsNewSection(Section2, Section2Body);
                    break;
                case 3:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1));
                    SetTabAsNewSection(Section3, Section3Body);
                    break;
                case 4:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2));
                    SetTabAsNewSection(Section4, Section4Body);
                    break;
                case 5:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2));
                    SetTabsDesign(Section4, Section4Body, sectionsList.ElementAt(3));
                    SetTabAsNewSection(Section5, Section5Body);
                    break;
                case 6:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2));
                    SetTabsDesign(Section4, Section4Body, sectionsList.ElementAt(3));
                    SetTabsDesign(Section5, Section5Body, sectionsList.ElementAt(4));
                    SetTabAsNewSection(Section6, Section6Body);
                    break;
                case 7:
                    SetTabsDesign(Section1, Section1Body, sectionsList.ElementAt(0));
                    SetTabsDesign(Section2, Section2Body, sectionsList.ElementAt(1));
                    SetTabsDesign(Section3, Section3Body, sectionsList.ElementAt(2));
                    SetTabsDesign(Section4, Section4Body, sectionsList.ElementAt(3));
                    SetTabsDesign(Section5, Section5Body, sectionsList.ElementAt(4));
                    SetTabsDesign(Section6, Section6Body, sectionsList.ElementAt(5));
                    break;
                default:
                    break;
            }

        }

        private void SetTabAsNewSection(TabItem tab, Grid body)
        {

            tab.Header = "+";
            body.Children.Clear();
            Button addSectionButton = new Button();
            addSectionButton.Width = 150;
            addSectionButton.Height = 70;
            addSectionButton.Content = "Přidat novou sekci";
            addSectionButton.Name = "addSection";
            addSectionButton.Click += AddNewSection;
            body.Children.Add(addSectionButton);
        }

        private void AddNewSection(object sender, RoutedEventArgs e)
        {
            // vyjede form s přidáním sekce
            AddNewSection win2 = new AddNewSection(DatabaseName);
            win2.Show();

        }

        private void SetTabsDesign(TabItem tab, Grid body, TeamSectionBasic teamSectionBasic)
        {
            Brush brush = Brushes.Black;
            if (teamSectionBasic.sectionID % 4 == 0) { brush = Brushes.Brown; }
            if (teamSectionBasic.sectionID % 4 == 1) { brush = Brushes.Cyan; }
            if (teamSectionBasic.sectionID % 4 == 2) { brush = Brushes.Salmon; }
            if (teamSectionBasic.sectionID % 4 == 3) { brush = Brushes.Snow; }
            tab.Background = brush;
            body.Background = brush;
            tab.Header = teamSectionBasic.sectionName;
        }

        private void GoToNextDay(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            using (SQLiteConnection conn = new SQLiteConnection(@"Data Source=.\" + DatabaseName + ";"))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select player.id_player, player.individualSkill, player.teamplaySkill, team.reputation from player join teamxsection on player.team_fk = teamxsection.id_teamxsection join team on team.id_team = teamxsection.id_team;", conn);
                SQLiteDataReader reader = command.ExecuteReader();
                double salary, salary4;
                int salary2, salary3;
                while (reader.Read())
                {
                    /* teamplayS = Math.Min((int)(rand.Next(-4, 5) + reader.GetInt32(2) * 0.9), 100);
                     individualS = Math.Min((int)(rand.Next(-4, 5) + reader.GetInt32(2) * 0.9), 100);
                     teamplayP = Math.Min(teamplayS + rand.Next(2, 11), 100);
                     individualP = Math.Min(individualS + rand.Next(2, 11), 100);
                     SQLiteCommand command2 = new SQLiteCommand("update player set individualSkill=" + individualS + ", teamplaySkill=" + teamplayS + ", teamplayPotencial=" + teamplayP + ", individualPotencial=" + individualP + " where id_player=" + reader.GetInt32(0) + ";", conn);
                     SQLiteDataReader reader2 = command2.ExecuteReader();*/
                    /*salary = 1000 + (int)(((reader.GetInt32(3) * (reader.GetInt32(1) + reader.GetInt32(2)) / 2) - 3600) * 1.02);
                    salary4 = (salary * 100 / 3);
                    salary2 = (int)(salary4 / 100);
                    salary3 = salary2 * 100;
                    SQLiteCommand command2 = new SQLiteCommand("update player set salary=" + salary + ", value=" + salary3 +" where id_player=" + reader.GetInt32(0) + ";", conn);
                    //SQLiteCommand command2 = new SQLiteCommand("update player set value=" + salary3 + " where id_player=" + reader.GetInt32(0) +";", conn);
                    SQLiteDataReader reader2 = command2.ExecuteReader();*/
                }
                reader.Close();
            }

        }

        private int GetNumberOfSections()
        {
            return sectionsList.Count;
        }

        private void ShowRegisteredTournaments(object sender, RoutedEventArgs e)
        {
            TournamentsParticipating win2 = new TournamentsParticipating(DatabaseName);
            win2.Show();
        }

        private void ShowSponsors(object sender, RoutedEventArgs e)
        {
            ViewSponsors win2 = new ViewSponsors(1, DatabaseName);
            win2.Show();
        }

        private void QuitGame(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void ShowManual(object sender, RoutedEventArgs e)
        {
            Manual win2 = new Manual();
            //this.all
            win2.Show();
        }

        private void ShowFreeEmployees(object sender, RoutedEventArgs e)
        {
            FreeCoaches win2 = new FreeCoaches(DatabaseName);
            win2.ShowDialog();
        }

        private void ShowPowerRanking(object sender, RoutedEventArgs e)
        {

        }

        private void ShowTeams(object sender, RoutedEventArgs e)
        {
            TeamSearch win2 = new TeamSearch();
            win2.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(1);
        }

        private void ShowFreePlyaers(object sender, RoutedEventArgs e)
        {
            FreePlayers win2 = new FreePlayers(DatabaseName);
            win2.Show();
        }

        private void ShowAllPlayers(object sender, RoutedEventArgs e)
        {
            PlayerSearch win2 = new PlayerSearch(DatabaseName);
            win2.Show();
        }

        private void Window_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            /*if (IsEnabled)
            {
                SetLabels();
                AddSectionsToList();
                SetTabs();
                AddAllTournaments();
                AddAllPlayers();
                ChangePropertiesOfNextActionButton();
                NonPlayerTeamsActions();
            }*/
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            SetLabels();
            AddSectionsToList();
            SetTabs();
            AddAllMatches();
            AddAllPlayers();
            ChangePropertiesOfNextActionButton();
            NonPlayerTeamsActions();
        }

        private void ShowTraining(object sender, RoutedEventArgs e)
        {
            Training win2 = new Training();
            win2.Show();
        }
    }
}
