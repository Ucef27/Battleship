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

        private void placeChars()
        {
            Grid g1 = this.Oceania;
            Grid g2 = this.Eurasia;

            for (int i = 0; i < 10; i++)
            {
                Label mylab = new Label();
                Label mylab2 = new Label();

                mylab.Content = Convert.ToChar(65 + i);
                mylab2.Content = Convert.ToChar(65 + i);

                mylab.HorizontalAlignment = HorizontalAlignment.Center;
                mylab.VerticalAlignment = VerticalAlignment.Center;
                mylab.HorizontalContentAlignment = HorizontalAlignment.Center;
                mylab.VerticalContentAlignment = VerticalAlignment.Center;

                mylab2.HorizontalAlignment = HorizontalAlignment.Center;
                mylab2.VerticalAlignment = VerticalAlignment.Center;
                mylab2.HorizontalContentAlignment = HorizontalAlignment.Center;
                mylab2.VerticalContentAlignment = VerticalAlignment.Center;

                Grid.SetRow(mylab, i + 1);
                Grid.SetColumn(mylab, 0);
                Grid.SetRow(mylab2, i + 1);
                Grid.SetColumn(mylab2, 0);

                g1.Children.Add(mylab);
                g2.Children.Add(mylab2);

            }
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

            place(2, 2, true, true);

            place(2, 5, false, false);
        }
    }
}
