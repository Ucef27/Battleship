using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Collections;
using Torpedo;

namespace Oceania
{
    public class PlayerOceania
    {
        private bool isVertical;
        private bool huntMode = false;
        private bool advHuntMode = false;
        private int parity = 0;
        private Queue<TorpedoShot> shotsToTake = new Queue<TorpedoShot>();
        private List<string> shipLocations = new List<string>();
        private ArrayList hits = new ArrayList();
        private TorpedoShot origin = new TorpedoShot();
        private int[] parityThree = { 4, 7, 10, 13, 16, 19 };
        Dictionary<string, int> shipsNotSunk = new Dictionary<string, int>()
        {
            {"Aircraft Carrier", 5 },
            {"Battleship", 4 },
            {"Cruiser", 3 },
            {"Submarine", 3 },
            {"Destroyer", 2 }
        };

        TorpedoShot lastShot;

        string[] aircraftCarrier;
        string[] battleship;
        string[] cruiser;
        string[] submarine;
        string[] destroyer;

        private List<string> shotsAlreadyTaken;

        /* The NextMove() method is called every time the main program needs a torpedo shot from this player.
         * Locations in this game always start with a letter A - J, and are followed by a number 1 - 10.
         * This is where most of your "artificial intelligence" will come into play.  As an example, I have
         * coded the Oceania player to have a valid, but very unintelligent targetting algorithms.  All it
         * does is pick a random valid square and shoot at it if it hasn't already targetted that square.  If
         * it has targetted that square already, it just tries again with a different randomly picked square.
         * It doesn't even use the ResultOfShot() method to help plan its next shot. */
        public TorpedoShot NextMove()
        {
            TorpedoShot shot = new TorpedoShot();
            string stringShot = "";

            while (stringShot == "")
            {

                if (shotsToTake.Count != 0)
                {
                    shot = shotsToTake.Dequeue();

                    stringShot = "" + shot.Row + shot.Column;
                }
                else if (shipsNotSunk.Count == 1 && shipsNotSunk.ContainsKey("Destroyer"))
                {
                    Random random = new Random();
                    //Debug.WriteLine(parity);
                    int row = random.Next(10);
                    int column = random.Next(10) + 1;
                    if (Math.Pow(-1, row + column) > 0 && parity == 0)
                    {
                        shot = new TorpedoShot(((char)('A' + row)).ToString(), column.ToString());
                        stringShot = "" + shot.Row + shot.Column;
                        if (shotsAlreadyTaken.Contains(stringShot) || hits.Contains(stringShot))
                        {
                            stringShot = "";
                        }
                        else
                        {
                            shotsAlreadyTaken.Add(stringShot);
                        }

                    }
                    else if (Math.Pow(-1, row + column) < 0 && parity == 1)
                    {
                        shot = new TorpedoShot(((char)('A' + row)).ToString(), column.ToString());
                        stringShot = "" + shot.Row + shot.Column;
                        if (shotsAlreadyTaken.Contains(stringShot) || hits.Contains(stringShot))
                        {
                            stringShot = "";
                        }
                        else
                        {
                            shotsAlreadyTaken.Add(stringShot);
                        }

                    }
                    else
                    {
                        stringShot = "";
                    }
                }
                else
                {
                    Random random = new Random();
                    int row = random.Next(10);
                    int column = random.Next(10) + 1;
                    if (parityThree.Contains((row + 1) + column))
                    {
                        shot = new TorpedoShot(((char)('A' + row)).ToString(), column.ToString());
                        stringShot = "" + shot.Row + shot.Column;
                        if (shotsAlreadyTaken.Contains(stringShot) || hits.Contains(stringShot))
                        {
                            stringShot = "";
                        }
                        else
                        {
                            shotsAlreadyTaken.Add(stringShot);
                        }
                    }

                }



            }

            return shot;
        }

        /* The ResultOfShot() method must contain all the code you need to adjust your internal data
         * in response to the result of your latest shot.
         *
         ResultOfShot does will go left until it misses then will not go back to the right for horizontal ships, origin is not updating
        */
        public void ResultOfShot(TorpedoResult result)
        {
            if (result.Sunk.Length > 0)
            {
                huntMode = false;
                advHuntMode = false;
                shipLocations.Add("" + result.Shot.Row + result.Shot.Column);
                shotsToTake.Clear();
                checkEdges(result.Sunk);
                shipsNotSunk.Remove(result.Sunk);
                shipLocations.Clear();
            }
            if (huntMode && result.WasHit)
            {
                if (!advHuntMode)
                {
                    if (result.Shot.Row == origin.Row)
                    {
                        isVertical = false;
                    }
                    else if (result.Shot.Column == origin.Column)
                    {
                        isVertical = true;
                    }
                }
                hits.Add("" + result.Shot.Row + result.Shot.Column);
                shipLocations.Add("" + result.Shot.Row + result.Shot.Column);
                advHuntMode = true;
            }
            if (advHuntMode && result.Sunk.Length == 0)
            {
                shotsToTake.Clear();
                advHunt(result.Shot, result.WasHit);
            }
            if (!advHuntMode && result.WasHit && result.Sunk.Length == 0 && shotsToTake.Count == 0)
            {
                lastShot = result.Shot;
                origin = result.Shot;
                hits.Add("" + result.Shot.Row + result.Shot.Column);
                shipLocations.Add("" + result.Shot.Row + result.Shot.Column);
                huntMode = true;
                hunt(result.Shot);
            }


        }

        /* The Reset() method must contain all the code you need to prepare for a new game, including
         * resetting your internal data for a "rematch". */
        public void Reset()
        {
            shotsAlreadyTaken = new List<string>();
            Random random = new Random();
            parity = random.Next(2);
            shotsToTake.Clear();
            shipLocations.Clear();
            hits.Clear();
            shipsNotSunk = new Dictionary<string, int>()
            {
                {"Aircraft Carrier", 5 },
                {"Battleship", 4 },
                {"Cruiser", 3 },
                {"Submarine", 3 },
                {"Destroyer", 2 }
            };
            aircraftCarrier = new string[5] { "F9", "G9", "H9", "I9", "J9" };
            battleship = new string[4] { "A6", "A7", "A8", "A9" };
            cruiser = new string[3] { "G1", "H1", "I1" };
            submarine = new string[3] { "B1", "C1", "D1" };
            destroyer = new string[2] { "F7", "G7" };
        }

        /* Ship locations are only valid if the ship is positioned horizontally or vertically.
         * This means that either all the letters in the position are constant (horizontal) or
         * all the numbers in the position are constant (vertical).  Furthermore, the sequential
         * information must be in order, from lowest to highest.  Finally, the string array
         * representing the position must be exactly the predefined size of the ship whose position
         * it is representing.  For instance:
         *
         *      Submarine:  {"E5", "F5", "G5"}          // valid
         *      Submarine:  {"E5", "F5", "G6"}          // invalid
         *      Cruiser:    {"E5", "F5", "G5"}          // invalid
         *      Battleship: {"E5", "F5", "G5"}          // invalid
         *      Battleship: {"J6", "J7", "J8", "J9"}    // valid
         *      Battleship: {"J9", "J8", "J7", "J6"}    // invalid
         *      Destroyer:  {"K1", "L1"}                // valid
         *      Destroyer:  {"L1", "K1"}                // invalid
         */

        /* Return the location of your Aircraft Carrier */

        private void advHunt(TorpedoShot shot, bool wasHit)
        {
            int row = (int)Char.Parse(shot.Row) - 64;
            int col = Int32.Parse(shot.Column);


            int orow = (int)Char.Parse(origin.Row) - 64;
            int ocol = Int32.Parse(origin.Column);



            if (isVertical)
            {
                TorpedoShot nextShot = new TorpedoShot(((char)('A' + (row - 1))).ToString(), (col).ToString());
                if (row > 1)
                {
                    nextShot = new TorpedoShot(((char)('A' + (row - 2))).ToString(), (col).ToString());
                }

                if (nextShot.Row == origin.Row && nextShot.Column == origin.Column && orow > 1)
                {
                    nextShot = new TorpedoShot(((char)('A' + (orow - 2))).ToString(), (ocol).ToString());
                }
                if (!wasHit && row < 10 || row == 1 || hits.Contains("" + nextShot.Row + nextShot.Column) || shotsAlreadyTaken.Contains("" + nextShot.Row + nextShot.Column) && row < 10)
                {
                    nextShot = new TorpedoShot(((char)('A' + (orow))).ToString(), (ocol).ToString());
                    origin = nextShot;

                }
                shotsToTake.Enqueue(nextShot);
                shotsAlreadyTaken.Add(nextShot.Row + nextShot.Column);
            }
            else
            {
                TorpedoShot nextShot = new TorpedoShot(((char)('A' + (row - 1))).ToString(), (col).ToString());
                if (col > 1)
                {
                    nextShot = new TorpedoShot(((char)('A' + (row - 1))).ToString(), (col - 1).ToString());
                }
                if (nextShot.Row == origin.Row && nextShot.Column == origin.Column && ocol > 1)
                {
                    nextShot = new TorpedoShot(((char)('A' + (orow - 1))).ToString(), (ocol - 1).ToString());
                }
                if (!wasHit && col < 11 || col == 1 || hits.Contains("" + nextShot.Row + nextShot.Column) || shotsAlreadyTaken.Contains("" + nextShot.Row + nextShot.Column) && col < 11)
                {
                    nextShot = new TorpedoShot(((char)('A' + (orow - 1))).ToString(), (ocol + 1).ToString());
                    origin = nextShot;
                }

                shotsToTake.Enqueue(nextShot);
                shotsAlreadyTaken.Add(nextShot.Row + nextShot.Column);
            }

        }
        private void hunt(TorpedoShot shot)
        {
            if (shotsToTake.Count == 0)
            {
                int row = (int)Char.Parse(shot.Row) - 64;
                int col = Int32.Parse(shot.Column);

                TorpedoShot shot1;
                TorpedoShot shot2;
                TorpedoShot shot3;
                TorpedoShot shot4;

                if (col > 1)
                {
                    shot1 = new TorpedoShot(((char)('A' + row - 1)).ToString(), (col - 1).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot1.Row + shot1.Column))
                    {
                        shotsToTake.Enqueue(shot1);
                    }
                }
                if (row < 10)
                {
                    shot2 = new TorpedoShot(((char)('A' + (row))).ToString(), (col).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot2.Row + shot2.Column))
                    {
                        shotsToTake.Enqueue(shot2);
                    }
                }
                if (col < 11)
                {
                    shot3 = new TorpedoShot(((char)('A' + row - 1)).ToString(), (col + 1).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot3.Row + shot3.Column))
                    {
                        shotsToTake.Enqueue(shot3);
                    }
                }
                if (row > 1)
                {
                    shot4 = new TorpedoShot(((char)('A' + (row - 2))).ToString(), (col).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot4.Row + shot4.Column))
                    {
                        shotsToTake.Enqueue(shot4);
                    }
                }



            }
        }

        public void checkEdges(string ship)
        {
            shipLocations.Sort();
            //for (int i = 0; i < shipLocations.Count; i++)
            //{
            //    Debug.WriteLine("[" + shipLocations[i] + "]");
            //}

            if (isVertical && shipsNotSunk[ship] < shipLocations.Count)
            {
                //Grabbing furthest left and right locations
                int topRow = (int)Char.Parse("" + shipLocations[0][0]) - 64;
                int topCol = Int32.Parse("" + shipLocations[0][1]);

                int botRow = (int)Char.Parse("" + shipLocations[shipLocations.Count - 1][0]) - 64;
                int botCol = Int32.Parse("" + shipLocations[shipLocations.Count - 1][1]);

                TorpedoShot shot1;
                TorpedoShot shot2;
                TorpedoShot shot3;
                TorpedoShot shot4;

                if (topCol > 1)
                {
                    shot1 = new TorpedoShot(((char)('A' + topRow - 1)).ToString(), (topCol - 1).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot1.Row + shot1.Column))
                    {
                        shotsToTake.Enqueue(shot1);
                    }
                }
                if (topCol < 11)
                {
                    shot2 = new TorpedoShot(((char)('A' + topRow - 1)).ToString(), (topCol + 1).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot2.Row + shot2.Column))
                    {
                        shotsToTake.Enqueue(shot2);
                    }
                }
                if (botCol > 1)
                {
                    shot3 = new TorpedoShot(((char)('A' + botRow - 1)).ToString(), (botCol - 1).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot3.Row + shot3.Column))
                    {
                        shotsToTake.Enqueue(shot3);
                    }
                }
                if (botCol < 11)
                {
                    shot4 = new TorpedoShot(((char)('A' + botRow - 1)).ToString(), (botCol + 1).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot4.Row + shot4.Column))
                    {
                        shotsToTake.Enqueue(shot4);
                    }
                }
            }
            else if (!isVertical && shipsNotSunk[ship] < shipLocations.Count)
            {
                //Grabbing furthest left and right locations
                int leftRow = (int)Char.Parse("" + shipLocations[0][0]) - 64;
                int leftCol = Int32.Parse("" + shipLocations[0][1]);

                int rigRow = (int)Char.Parse("" + shipLocations[shipLocations.Count - 1][0]) - 64;
                int rigCol = Int32.Parse("" + shipLocations[shipLocations.Count - 1][1]);

                TorpedoShot shot1;
                TorpedoShot shot2;
                TorpedoShot shot3;
                TorpedoShot shot4;

                if (leftRow < 10)
                {
                    shot1 = new TorpedoShot(((char)('A' + (leftRow))).ToString(), (leftCol).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot1.Row + shot1.Column))
                    {
                        shotsToTake.Enqueue(shot1);
                    }
                }
                if (leftRow > 1)
                {
                    shot2 = new TorpedoShot(((char)('A' + (leftRow - 2))).ToString(), (leftCol).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot2.Row + shot2.Column))
                    {
                        shotsToTake.Enqueue(shot2);
                    }
                }
                if (rigRow < 10)
                {
                    shot3 = new TorpedoShot(((char)('A' + (rigRow))).ToString(), (rigCol).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot3.Row + shot3.Column))
                    {
                        shotsToTake.Enqueue(shot3);
                    }
                }
                if (rigRow > 1)
                {
                    shot4 = new TorpedoShot(((char)('A' + (rigRow - 2))).ToString(), (rigCol).ToString());
                    if (!shotsAlreadyTaken.Contains("" + shot4.Row + shot4.Column))
                    {
                        shotsToTake.Enqueue(shot4);
                    }
                }
            }
        }
        public string[] GetAircraftCarrier()
        {
            return aircraftCarrier;
        }

        /* Return the location of your Battleship */
        public string[] GetBattleship()
        {
            return battleship;
        }

        /* Return the location of your Cruiser */
        public string[] GetCruiser()
        {
            return cruiser;
        }

        /* Return the location of your Submarine */
        public string[] GetSubmarine()
        {
            return submarine;
        }

        /* Return the location of your Destroyer */
        public string[] GetDestroyer()
        {
            return destroyer;
        }
    }
}