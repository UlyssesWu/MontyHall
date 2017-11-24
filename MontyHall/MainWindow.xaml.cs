using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MontyHall
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Match> ObMatches;
        public float WinRate => (float) WinCount / GameCount;
        public int GameCount { get; private set; } = 0;
        public int WinCount { get; private set; } = 0;
        public int ChangeWinCount { get; private set; } = 0;
        public float ChangeWinRate => (float)ChangeWinCount / GameCount;
        public float UnchangeWinRate => (float)UnchangeWinCount / GameCount;
        public int UnchangeWinCount { get; private set; } = 0;
        public bool GameStart { get; set; } = false;
        public bool DoorPicked { get; set; } = false;
        public bool GameEnd => !GameStart;
        public int DoorCount { get; set; } = 10;
        public Button Picked { get; set; }
        public Button Another { get; set; }
        private Random _random = new Random();
        private int _gift = 0;
        public MainWindow()
        {
            InitializeComponent();
            DeskPanel.Children.Clear();
            DataContext = this;
            ObMatches = new ObservableCollection<Match>();
            DataMatch.ItemsSource = ObMatches;
        }


        private async void Start(object sender, RoutedEventArgs e)
        {
            BtnStart.IsEnabled = false;
            GameStart = true;
            DoorPicked = false;
            BtnChangeDoor.IsEnabled = false;
            BtnResult.IsEnabled = false;
            SliderCard.IsEnabled = false;
            DeskPanel.Children.Clear();
            for (int i = 1; i <= DoorCount; i++)
            {
                Button btn = new Button(){Height = 50, Width = 50, BorderThickness = new Thickness(5), Margin = new Thickness(3), Content = i};
                btn.Click += ClickButton;
                DeskPanel.Children.Add(btn);
                await Task.Delay(200);
            }
            _gift = _random.Next(1, DoorCount + 1);
            Console.WriteLine($"Set gift at {_gift}");
            TextCheat.Text = BtnShowGift.IsChecked == true ? $"No.{_gift} is gift!" : "";
            Dealer.Text = "The gift is now set in one of the doors. Now choose one door!";
            BtnStart.IsEnabled = true;
        }

        private void ChangeDoor(object sender, RoutedEventArgs e)
        {
            Check(true);
        }

        private void End(object sender, RoutedEventArgs e)
        {
            Check();
        }

        private void Check(bool change = false)
        {
            GameCount++;
            GameStart = false;
            DoorPicked = false;
            BtnChangeDoor.IsEnabled = false;
            BtnResult.IsEnabled = false;
            Picked.Click -= End;
            Another.Click -= ChangeDoor;
            if (change)
            {
                var temp = Picked;
                Picked = Another;
                Another = temp;
            }
            bool win = false;
            if (Picked.Content.ToString() == _gift.ToString())
            {
                Picked.Background = new SolidColorBrush(Colors.Gold);
                WinCount++;
                if (change)
                {
                    ChangeWinCount++;
                }
                else
                {
                    UnchangeWinCount++;
                }
                Dealer.Text = "You Win!";
                win = true;
            }
            else
            {
                Another.Background = new SolidColorBrush(Colors.Gold);
                Dealer.Text = "You Lose...";
            }
            ObMatches.Add(new Match()
            {
                Changed = change,
                Doors = DoorCount,
                Result = win ? "Win" : "Lose",
                Round = GameCount
            });
            SliderCard.IsEnabled = true;

            UpdateRates();
        }

        private void UpdateRates()
        {
            LblChangeWin.Content = "Change Win:" + ChangeWinRate.ToString("F5");
            LblUnchangeWin.Content = "Unchange Win:" + UnchangeWinRate.ToString("F5");
            LblWin.Content = "Win Rate:" + WinRate.ToString("F5");
        }

        private async void ClickButton(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            Picked = btn;
            btn.BorderBrush = new SolidColorBrush(Colors.Red);

            Button another = new Button();
            List<Button> btnList = new List<Button>(DoorCount);
            foreach (var deskPanelChild in DeskPanel.Children)
            {
                if (deskPanelChild is Button deskButton)
                {
                    deskButton.Click -= ClickButton;
                    if (deskButton.Content.ToString() != _gift.ToString())
                    {
                        btnList.Add(deskButton);
                    }
                    else
                    {
                        another = deskButton;
                    }
                }
            }

            btnList.Remove(btn);
            Dealer.Text = "You have picked a door! Now opening some fake doors...";
            for (int i = 0; i < DoorCount - 2; i++)
            {
                await Task.Delay(200);
                var index = _random.Next(btnList.Count);
                btnList[index].Background = new SolidColorBrush(Colors.SlateGray);
                btnList.RemoveAt(index);
            }
            if (btnList.Count > 0)
            {
                another = btnList[0];
            }
            another.BorderBrush = new SolidColorBrush(Colors.MediumPurple);
            Another = another;
            Dealer.Text = $"You picked door {Picked.Content.ToString()}. Will you change to {Another.Content.ToString()} ? Click `Change Door` or `Show Result` to end the game.";
            DoorPicked = true;
            BtnChangeDoor.IsEnabled = true;
            BtnResult.IsEnabled = true;

            another.Click += ChangeDoor;
            Picked.Click += End;
        }

        private void AutoPlay(object sender, RoutedEventArgs e)
        {
            for (int c = 0; c < 10; c++)
            {
                Console.WriteLine($"[Auto] GAME {GameCount}");
                bool change = !((Button)sender).Name.Contains("Unchange");
                int gift = _random.Next(0, DoorCount);
                Console.WriteLine($"[Auto] Set gift at {gift}");
                int pick = _random.Next(0, DoorCount);
                Console.WriteLine($"[Auto] Pick {pick}");
                List<int> lst = new List<int>(DoorCount);
                for (int i = 0; i < DoorCount; i++)
                {
                    if (i != pick && i!= gift)
                    {
                        lst.Add(i);
                    }
                }
                for (int i = 0; i < DoorCount - 2; i++)
                {
                    lst.RemoveAt(_random.Next(lst.Count));
                }
                if (lst.Count <= 0)
                {
                    lst.Add(gift);
                }
                bool win = false;
                if (change)
                {
                    Console.WriteLine($"[Auto] Changed to {lst[0]}");
                    if (lst[0] == gift)
                    {
                        win = true;
                        WinCount++;
                        ChangeWinCount++;
                    }
                }
                else
                {
                    Console.WriteLine($"[Auto] Not change to {lst[0]}");
                    if (pick == gift)
                    {
                        win = true;
                        WinCount++;
                        UnchangeWinCount++;
                    }
                }
                Console.WriteLine(win ? "[Auto] Win!" : "[Auto] Lose...");
                ObMatches.Add(new Match()
                {
                    Round = GameCount++,
                    Changed = change,
                    Doors = DoorCount,
                    Result = win ? "Win" : "Lose"
                });
                UpdateRates();
            }
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            GameCount = 0;
            WinCount = 0;
            UnchangeWinCount = 0;
            ChangeWinCount = 0;
            ObMatches.Clear();
            UpdateRates();
        }
    }
}
