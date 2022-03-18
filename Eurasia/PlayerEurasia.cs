using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torpedo;
using System.Diagnostics;
using System.Windows;
using System.Collections;

namespace Eurasia
{
    public class PlayerEurasia
    {
        private int[,] board = new int[10, 10];
        private int[,] hits_per_shot = new int[10, 10];

        private int[,] previous_hits_per_shot = new int[10, 10];

        private Dictionary<String, int> ship_size = new Dictionary<string, int>() { { "Aircraft Carrier", 5 }, { "Battleship", 4 }, { "Submarine", 3 }, { "Cruiser", 3 }, { "Destroyer", 2 } };

        private int[] alive_ships = new int[] { 2, 3, 3, 4, 5 };

        private List<Tuple<int, int>> shotHistory = new List<Tuple<int, int>>();

        private bool huntMode = false;

        private bool advancedHuntMode = false;
        private bool isVerticle;
        Queue<Tuple<int, int>> shotsToTake = new Queue<Tuple<int, int>>();
        ArrayList hits = new ArrayList();
        private Tuple<int, int> originShot;

        private List<Tuple<int, int>> corners = new List<Tuple<int, int>>() { Tuple.Create(0, 0), Tuple.Create(9, 0), Tuple.Create(0, 9), Tuple.Create(9, 9), Tuple.Create(0, 4), Tuple.Create(4, 0), Tuple.Create(4, 9), Tuple.Create(9, 4) };

        private int turn = 1;


        /* The NextMove() method is called every time the main program needs a torpedo shot from this player.
         * Locations in this game always start with a letter A - J, and are followed by a number 1 - 10.
         * This is where most of your "artificial intelligence" will come into play.  As an example, I have
         * coded the Eurasia player to have one of the worst possible targetting algorithms.  All it does is
         * pick a random valid square and shoot at it.  It doesn'te ven check to see if they have already
         * shot at this square. */
        public TorpedoShot NextMove()
        {
            TorpedoShot shot = new TorpedoShot();

            previous_hits_per_shot = hits_per_shot;
            hits_per_shot = new int[10, 10];

            int row;
            int column;


            Tuple<int, int> location;

            foreach (int i in alive_ships)
            {
                if (i == 0)
                {
                    continue;
                }

                find_hits_per_spot(i);
            }


            if (shotsToTake.Count != 0)
            {
                Tuple<int, int> foo = shotsToTake.Dequeue();
                row = foo.Item1;
                column = foo.Item2;
            }

            else if (turn % 5 == 0 && corners.Count > 0)
            {

                Tuple<int, int> temp = shotHistory[0];
                Random r = new Random();
                bool skip = false;
                while (shotHistory.Contains(temp))
                {
                    if (corners.Count == 0)
                    {
                        skip = true;
                        break;
                    }
                    int check = r.Next(0, corners.Count);
                    temp = corners[check];
                    corners.RemoveAt(check);

                }

                if (skip)
                {
                    location = find_max();
                    row = location.Item1;
                    column = location.Item2;
                }

                else
                {
                    row = temp.Item1;
                    column = temp.Item2;
                }

            }

            else
            {
                location = find_max();
                row = location.Item1;
                column = location.Item2;
            }




            shotHistory.Add(new Tuple<int, int>(row, column));
            shot = new TorpedoShot(((char)('A' + row)).ToString(), (column + 1).ToString());

            //board[row, column] = 1;
            turn++;
            return shot;
        }

        /* The ResultOfShot() method must contain all the code you need to adjust your internal data
         * in response to the result of your latest shot. */
        public void ResultOfShot(TorpedoResult result)
        {
            //Debug.WriteLine(char.Parse(result.Shot.Row) - 64);

            Tuple<int, int> shot_location = new Tuple<int, int>(char.Parse(result.Shot.Row) - 64 - 1, Int32.Parse(result.Shot.Column) - 1);

            Debug.WriteLine(shot_location);

            if (result.WasHit)
            {
                board[shot_location.Item1, shot_location.Item2] = 2;
            }

            else
            {
                board[shot_location.Item1, shot_location.Item2] = 1;
            }

            if (!result.Sunk.Equals(""))
            {
                huntMode = false;
                advancedHuntMode = false;
                var foos = new List<int>(alive_ships);
                foos.Remove(ship_size[result.Sunk]);
                alive_ships = foos.ToArray();
                shotsToTake.Clear();
            }

            if (huntMode && result.WasHit)
            {
                if (!advancedHuntMode)
                {
                    if (shot_location.Item1 == originShot.Item1)
                    {
                        isVerticle = false;
                    }

                    else if (shot_location.Item2 == originShot.Item2)
                    {
                        isVerticle = true;
                    }
                }

                advancedHuntMode = true;
            }

            if (advancedHuntMode && result.Sunk.Equals(""))
            {
                shotsToTake.Clear();
                advancedHunt(shot_location, result.WasHit);
            }

            if (!advancedHuntMode && result.WasHit && result.Sunk.Length == 0 && shotsToTake.Count == 0)
            {
                originShot = shot_location;
                huntMode = true;
                hunt(shot_location);
            }

        }

        /* The Reset() method must contain all the code you need to prepare for a new game, including
         * resetting your internal data for a "rematch". */
        public void Reset()
        {
            board = new int[10, 10];
            hits_per_shot = new int[10, 10];

            previous_hits_per_shot = new int[10, 10];

            ship_size = new Dictionary<string, int>() { { "Aircraft Carrier", 5 }, { "Battleship", 4 }, { "Submarine", 3 }, { "Cruiser", 3 }, { "Destroyer", 2 } };

            alive_ships = new int[] { 2, 3, 3, 4, 5 };

            shotHistory = new List<Tuple<int, int>>();

            huntMode = false;

            advancedHuntMode = false;
            isVerticle = false;
            shotsToTake = new Queue<Tuple<int, int>>();
            hits = new ArrayList();

            corners = new List<Tuple<int, int>>() { Tuple.Create(0, 0), Tuple.Create(9, 0), Tuple.Create(0, 9), Tuple.Create(9, 9), Tuple.Create(0, 4), Tuple.Create(4, 0), Tuple.Create(4, 9), Tuple.Create(9, 4) };

            turn = 1;
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
        public string[] GetAircraftCarrier()
        {
            string[] ship = new string[5] { "B2", "B3", "B4", "B5", "B6" };

            return ship;
        }

        /* Return the location of your Battleship */
        public string[] GetBattleship()
        {
            string[] ship = new string[4] { "D5", "E5", "F5", "G5" };

            return ship;
        }

        /* Return the location of your Cruiser */
        public string[] GetCruiser()
        {
            string[] ship = new string[3] { "J8", "J9", "J10" };

            return ship;
        }

        /* Return the location of your Submarine */
        public string[] GetSubmarine()
        {
            string[] ship = new string[3] { "A1", "B1", "C1" };

            return ship;
        }

        /* Return the location of your Destroyer */
        public string[] GetDestroyer()
        {
            string[] ship = new string[2] { "F6", "F7" };

            return ship;
        }

        private bool can_place(Tuple<int, int> start, Tuple<int, int> end, bool row_check)
        {
            if (row_check)
            {
                for (int i = start.Item2; i < end.Item2; i++)
                {
                    if (board[start.Item1, i] == 1 || board[start.Item1, i] == 2)
                    {
                        return false;
                    }
                }
            }

            else
            {
                for (int i = start.Item1; i < end.Item1; i++)
                {
                    if (board[i, start.Item2] == 1 || board[i, start.Item2] == 2)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        private void find_hits_per_spot(int length)
        {
            for (int i = 0; i < 9 + 1; i++)
            {
                for (int j = 0; j < (9 + 1) - length + 1; j++)
                {
                    if (can_place(Tuple.Create(i, j), Tuple.Create(i, j + length), true))
                    {
                        for (int l = j; l < j + length; l++)
                        {
                            hits_per_shot[i, l] += 1;
                        }
                    }
                }
            }

            for (int i = 0; i < 9 + 1; i++)
            {
                for (int j = 0; j < (9 + 1) - length + 1; j++)
                {
                    if (can_place(Tuple.Create(j, i), Tuple.Create(j + length, i), false))
                    {
                        for (int l = j; l < j + length; l++)
                        {
                            hits_per_shot[l, i] += 1;
                        }
                    }
                }
            }
        }

        private Tuple<int, int> find_max()
        {
            int max_value = 0;
            List<Tuple<int, int>> maxs = new List<Tuple<int, int>>();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (hits_per_shot[i, j] >= max_value)
                    {

                        if (hits_per_shot[i, j] > max_value)
                        {
                            maxs.Clear();
                            max_value = hits_per_shot[i, j];
                        }

                        maxs.Add(Tuple.Create(i, j));
                    }
                }
            }
            Random r = new Random();
            return maxs[r.Next(0, maxs.Count)];
        }


        private void hunt(Tuple<int, int> shot)
        {
            if (shotsToTake.Count == 0)
            {
                int row = shot.Item1;
                int col = shot.Item2;

                Tuple<int, int> shot1;
                Tuple<int, int> shot2;
                Tuple<int, int> shot3;
                Tuple<int, int> shot4;

                if (col > 0)
                {
                    shot1 = new Tuple<int, int>(row, col - 1);
                    if (!shotHistory.Contains(shot1))
                    {
                        shotsToTake.Enqueue(shot1);
                    }
                }

                if (row < 9)
                {
                    shot2 = new Tuple<int, int>(row + 1, col);
                    if (!shotHistory.Contains(shot2))
                    {
                        shotsToTake.Enqueue(shot2);
                    }
                }

                if (col < 9)
                {
                    shot3 = new Tuple<int, int>(row, col + 1);
                    if (!shotHistory.Contains(shot3))
                    {
                        shotsToTake.Enqueue(shot3);
                    }
                }

                if (row > 0)
                {
                    shot4 = new Tuple<int, int>(row - 1, col);
                    if (!shotHistory.Contains(shot4))
                    {
                        shotsToTake.Enqueue(shot4);
                    }
                }
            }
        }

        private void advancedHunt(Tuple<int, int> shot, bool wasHit)
        {
            int row = shot.Item1;
            int col = shot.Item2;

            int orow = originShot.Item1;
            int ocol = originShot.Item2;

            if (isVerticle)
            {
                Tuple<int, int> nextShot = new Tuple<int, int>(row, col);
                if (row > 0)
                {
                    nextShot = new Tuple<int, int>(row - 1, col);
                }

                if (nextShot.Item1 == orow && nextShot.Item2 == ocol && orow > 0)
                {
                    nextShot = new Tuple<int, int>(orow - 1, ocol);
                }

                if (!wasHit && orow < 9 || orow == 0 || shotHistory.Contains(nextShot))
                {
                    nextShot = new Tuple<int, int>(orow + 1, ocol);

                    if (shotHistory.Contains(nextShot))
                    {
                        nextShot = new Tuple<int, int>(orow + 2, ocol);
                    }
                    originShot = nextShot;
                }
                shotsToTake.Enqueue(nextShot);
                shotHistory.Add(nextShot);
            }

            else
            {
                Tuple<int, int> nextShot = new Tuple<int, int>(row, col);
                if (col > 0)
                {
                    nextShot = new Tuple<int, int>(row, col - 1);
                }

                if (nextShot.Item1 == orow && nextShot.Item2 == ocol && ocol > 0)
                {
                    nextShot = new Tuple<int, int>(orow, ocol - 1);
                }

                if (!wasHit && ocol < 9 || ocol == 0 || shotHistory.Contains(nextShot))
                {
                    nextShot = new Tuple<int, int>(orow, ocol + 1);

                    if (shotHistory.Contains(nextShot)) {
                        nextShot = new Tuple<int, int>(orow, ocol + 2);
                    }
                    originShot = nextShot;
                }
                shotsToTake.Enqueue(nextShot);
                shotHistory.Add(nextShot);
            }
        }
    }
}