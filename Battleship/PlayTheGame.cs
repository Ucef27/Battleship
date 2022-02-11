using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Torpedo;

/* The purpose of this file is to show you all how to have multiple files hold the code
 * for a common class.  What you do is create a new file as a brand new class, with the
 * file name you desire.  You can leave its initial class as Class1.cs, because you will
 * be deleting that.  After you get the new file in your IDE, change its class definition
 * to:
        public partial class MainWindow : Window
 * 
 * "Partial" classes are how WPF lets you spread code for the same class over multipl files.
 * Consider you original MainWindow.xaml.cs file.  It starts with a partial class.  This is
 * because it shares a code file with MainWindow.xaml! */
namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private void setup()
        {
            oceania.Reset();
            oceania_ships[Ships.AircraftCarrier] = new HashSet<String>(oceania.GetAircraftCarrier());
            oceania_ships[Ships.Battleship] = new HashSet<String>(oceania.GetBattleship());
            oceania_ships[Ships.Cruiser] = new HashSet<String>(oceania.GetCruiser());
            oceania_ships[Ships.Submarine] = new HashSet<String>(oceania.GetSubmarine());
            oceania_ships[Ships.Destroyer] = new HashSet<String>(oceania.GetDestroyer());

            eurasia_ships[Ships.AircraftCarrier] = new HashSet<String>(oceania.GetAircraftCarrier());
            eurasia_ships[Ships.Battleship] = new HashSet<String>(oceania.GetBattleship());
            eurasia_ships[Ships.Cruiser] = new HashSet<String>(oceania.GetCruiser());
            eurasia_ships[Ships.Submarine] = new HashSet<String>(oceania.GetSubmarine());
            eurasia_ships[Ships.Destroyer] = new HashSet<String>(oceania.GetDestroyer());

            oceania_ships_hit[Ships.AircraftCarrier] = new HashSet<String>();
            oceania_ships_hit[Ships.Battleship] = new HashSet<String>();
            oceania_ships_hit[Ships.Cruiser] = new HashSet<String>();
            oceania_ships_hit[Ships.Submarine] = new HashSet<String>();
            oceania_ships_hit[Ships.Destroyer] = new HashSet<String>();

            eurasia_ships_hit[Ships.AircraftCarrier] = new HashSet<String>();
            eurasia_ships_hit[Ships.Battleship] = new HashSet<String>();
            eurasia_ships_hit[Ships.Cruiser] = new HashSet<String>();
            eurasia_ships_hit[Ships.Submarine] = new HashSet<String>();
            eurasia_ships_hit[Ships.Destroyer] = new HashSet<String>();
        }
        private void startGame(Object sender, EventArgs e)
        {

            

            if (oceania_turn)
            {
                TorpedoShot currentShot = oceania.NextMove();

                String shot_location = currentShot.Row + currentShot.Column;

                bool isHit = false;

                String sunk = "";

                for (int i = 0; i < 5; i++)
                {
                    if (eurasia_ships[(Ships)i].Contains(shot_location))
                    {
                        eurasia_ships_hit[(Ships)i].Add(shot_location);
                        isHit = true;
                        if (oceania_ships_hit[(Ships)i].Count == oceania_ships[(Ships)i].Count)
                        {
                            oceania_ships_hit[(Ships)i].Add("LOLI");
                            sunk = convert[(Ships)i];

                        }
                        break;
                    }
                }

                place(char.Parse(currentShot.Row) - 64, Int32.Parse(currentShot.Column), isHit, true);
                oceania.ResultOfShot(new TorpedoResult(currentShot, isHit, sunk));
                oceania_turn = false;
                
            }

            else
            {
                TorpedoShot currentShot = eurasia.NextMove();

                String shot_location = currentShot.Row + currentShot.Column;

                bool isHit = false;

                String sunk = "";

                for (int i = 0; i < 5; i++)
                {
                    if (oceania_ships[(Ships)i].Contains(shot_location))
                    {
                        oceania_ships_hit[(Ships)i].Add(shot_location);
                        isHit = true;
                        if (oceania_ships_hit[(Ships)i].Count == oceania_ships[(Ships)i].Count)
                        {
                            oceania_ships_hit[(Ships)i].Add("MILF");
                            sunk = convert[(Ships)i];

                        }
                        break;
                    }
                }

                place(char.Parse(currentShot.Row) - 64, Int32.Parse(currentShot.Column) + 1, isHit, false);
                eurasia.ResultOfShot(new TorpedoResult(currentShot, isHit, sunk));
                oceania_turn = true;
            }

            
        }
    }
}