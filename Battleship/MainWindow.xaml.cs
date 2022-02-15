using Eurasia;
using Oceania;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Torpedo;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlayerOceania oceania = new PlayerOceania();
        private PlayerEurasia eurasia = new PlayerEurasia();
        private TorpedoShot torpedoShot;
        private DispatcherTimer dispatcherTimer;

        private Dictionary<Ships, HashSet<String>> oceania_ships = new Dictionary<Ships, HashSet<string>>();
        private Dictionary<Ships, HashSet<String>> eurasia_ships = new Dictionary<Ships, HashSet<string>>();
        private Dictionary<Ships, HashSet<String>> oceania_ships_hit = new Dictionary<Ships, HashSet<string>>();
        private Dictionary<Ships, HashSet<String>> eurasia_ships_hit = new Dictionary<Ships, HashSet<string>>();

        private Dictionary<Ships, String> convert = new Dictionary<Ships, string>();


        private bool oceania_turn = true;



        private void place(int row, int column, bool hit, bool Oceana)
        {
            Grid g;
            if (Oceana)
            {
                g = this.Oceania;
            }

            else
            {
                g = this.Eurasia;
            }

            if (hit)
            {
                Rectangle rect = new Rectangle();
                rect.Width = 45;
                rect.Height = 45;
                SolidColorBrush blueBrush = new SolidColorBrush();
                blueBrush.Color = Colors.Red;
                rect.Fill = blueBrush;


                Grid.SetRow(rect, row);
                Grid.SetColumn(rect, column);

                g.Children.Add(rect);

            }

            else
            {
                Rectangle rect = new Rectangle();
                rect.Width = 100;
                rect.Height = 100;
                SolidColorBrush blueBrush = new SolidColorBrush();
                blueBrush.Color = Colors.Blue;
                rect.Fill = blueBrush;


                Grid.SetRow(rect, row);
                Grid.SetColumn(rect, column);

                g.Children.Add(rect);
            }
        }

        private void place_ships(int row, int column, bool Oceana)
        {
            Grid g;
            if (Oceana)
            {
                g = this.Oceania;
            }

            else
            {
                g = this.Eurasia;
            }
            Rectangle rect = new Rectangle();
            rect.Width = 100;
            rect.Height = 100;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            rect.Fill = blackBrush;


            Grid.SetRow(rect, row);
            Grid.SetColumn(rect, column);

            g.Children.Add(rect);
        }

        public void place_text(int row, int column, bool Oceana, String stuff)
        {
            Grid g;
            if (Oceana)
            {
                g = this.Oceania;
            }

            else
            {
                g = this.Eurasia;
            }
            Label rect = new Label();
            rect.Width = 100;
            rect.Height = 100;
            rect.Content = stuff;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;


            Grid.SetRow(rect, row);
            Grid.SetColumn(rect, column);

            g.Children.Add(rect);
        }

        public void makeBorders()
        {
            Grid g = this.Oceania;

            Grid g2 = this.Eurasia;

            for (int r = 0; r < 11; r++)
            {
                for (int c = 0; c < 11; c++)
                {
                    Border b = new Border();
                    Border b2 = new Border();
                    b.BorderThickness = new Thickness(1);
                    b.BorderBrush = Brushes.Black;
                    b2.BorderThickness = new Thickness(1);
                    b2.BorderBrush = Brushes.Black;

                    b.Name = "O" + (c + 1) + (r + 1);
                    b2.Name = "E" + (c + 1) + (r + 1);

                    Grid.SetRow(b, r);
                    Grid.SetColumn(b, c);
                    g.Children.Add(b);

                    Grid.SetRow(b2, r);
                    Grid.SetColumn(b2, c);
                    g2.Children.Add(b2);
                }
            }
        }

        public MainWindow()
        {
            
            InitializeComponent();

            makeBorders();

            Grid g1 = this.Oceania;
            Grid g2 = this.Eurasia;

            for (int i = 0; i < 10; i++)
            {
                Label mylab = new Label();
                Label mylab2 = new Label();

                mylab.Content = Convert.ToChar(65 + i);
                mylab2.Content = Convert.ToChar(65 + i);

                Grid.SetRow(mylab, i + 1);
                Grid.SetColumn(mylab, 0);
                Grid.SetRow(mylab2, i + 1);
                Grid.SetColumn(mylab2, 0);

                g1.Children.Add(mylab);
                g2.Children.Add(mylab2);

            }

            for (int i = 0; i < 10; i++)
            {
                Label mylab = new Label();
                Label mylab2 = new Label();

                mylab.Content = i + 1;
                mylab2.Content = i + 1;

                Grid.SetRow(mylab, 0);
                Grid.SetColumn(mylab, i + 1);
                Grid.SetRow(mylab2, 0);
                Grid.SetColumn(mylab2, i + 1);

                g1.Children.Add(mylab);
                g2.Children.Add(mylab2);

            }

            convert[Ships.AircraftCarrier] = "Aircraft Carrier";
            convert[Ships.Battleship] = "Battleship";
            convert[Ships.Cruiser] = "Cruiser";
            convert[Ships.Submarine] = "Submarine";
            convert[Ships.Destroyer] = "Destroyer";



            setup();

            drawShips();


            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(startGame);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();



            //startGame();



        }

        private void drawShips()
        {
            foreach (Ships shi in oceania_ships.Keys)
            {
                var arr =  oceania_ships[shi];
                foreach (String s in arr)
                {
                    int l = char.Parse(s.Substring(0, 1));
                    l -= 64;
                    int n = Int32.Parse(s.Substring(1));
                    place_ships(l, n, true);
                }
            }
            foreach (Ships shi in eurasia_ships.Keys)
            {
                var arr = eurasia_ships[shi];
                foreach (String s in arr)
                {
                    int l = char.Parse(s.Substring(0, 1));
                    l -= 64;
                    int n = Int32.Parse(s.Substring(1));
                    place_ships(l, n, false);
                }
            }
        }
    }
}
