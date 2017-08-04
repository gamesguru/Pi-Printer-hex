using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiP_Debug
{
    class Program
    {
        static void Main(string[] args)
        {

            int[] hs = new int[250];
            string[] ds = new string[250];


            for (int k = 0; k < 25; k++)
            {
                double temp = sFinal(10 * k);  //see this function for the pure maths references
                for (int i = 0; i < 10; i++)
                {
                    if (temp < 0)
                    { temp -= Math.Floor(temp); }  //abrogates negative values

                    if (i == 10 * k && temp > 1)
                    { temp -= Math.Floor(temp); }

                    if (temp > 1)
                    { temp -= Math.Floor(temp); hs[10 * k + i] = 0; }  //returns zero if greater than 1, and NOT on the first iteration of the sequence

                    temp *= 16;
                    hs[10 * k + i] = Convert.ToInt32(Math.Floor(temp));
                    temp -= Math.Floor(temp);
                }
            }


            //base 16 "raw output"
            Console.WriteLine("Base 16:");
            Console.Write("3.");
            for (int i = 0; i < 250; i++)
            {
                Console.Write(hexParse(hs[i]));
                if ((i + 1) % 5 == 0)
                {
                    Console.Write(" ");
                }
            }

            //the rest of the code is dedicated to the arduous task of converting a base 16 decimal to a base 10 one
            //
            //remove the exponent "E" notation, and preformat with a corresponding number of zeros (10 * k + i - 1).  this makes tabular addition possible
            for (int k = 0; k < 25; k++)
            {               
                for (int i = 0; i < 10; i++)
                {
                    double d = hs[10 * k + i] / (Math.Pow(16, 10 * k + i + 1));
                    string num = null;

                    if (d.ToString().Contains("E"))
                    {
                        int exp = Convert.ToInt32(d.ToString().Substring(d.ToString().IndexOf("E") + 1));  //grabs the exponent number from the end of the double
                        num = d.ToString().Substring(0, d.ToString().IndexOf("E")); //clips the E and number off the end, leaving a decimal
                        num = num.Replace(".", ""); //removes the decimal point
                        for (int j = 1; j < -exp; j++)  //places the appropriate number of zeros before, making tabular addition possible
                        {
                            num = num.Insert(0, "0");
                        }
                        d *= Math.Pow(10, -exp);
                    }

                    else
                    {
                        num = d.ToString();
                        num = num.TrimStart('0');
                        num = num.Replace(".", "");
                    }


                    ds[10 * k + i] = num;
                }
            }


            //post-format with zeros to make strings equal length
            for (int i = 0; i < 250; i++)
            {
                int deltaA = ds[249].ToString().Length - ds[i].ToString().Length;
                for (int k = 0; k < deltaA; k++)
                {
                    ds[i] = ds[i].Insert(ds[i].Length, "0");
                }
            }


            //peform termwise addition
            int dA = ds[249].ToString().Length;
            int[] termSums = new int[dA]; //since dA = 316, this one has much more than 250 elements.  due to base 16 extending faster than base 10

            for (int k = dA - 1; k >= 0; k--)
            {
                for (int i = 0; i < 249; i++)
                {
                    termSums[dA - 1 - k] += (int)Char.GetNumericValue(ds[i][k]);               
                }
            }

            //divide by 10, add result to next term, and save remainder for current term
            for (int k = 0; k < dA - 1; k++)
            {
                termSums[k + 1] += termSums[k] / 10;
                termSums[k] %= 10;
            }

            Array.Reverse(termSums);  //reverse the order
            Console.WriteLine();
            Console.WriteLine();

            //outputs a processed base 10 format
            Console.WriteLine("Base 10:");
            Console.Write("3.");
            for (int k = 0; k < 21; k++)
            {
                Console.Write(termSums[k]);
                if ((k + 1) % 5 == 0)
                { Console.Write(" "); }
            }

            Console.Read();  //waits for the user to exit the program
        }


        public static string hexParse(int h)
        {
            switch (h)
            {
                case 10:
                    return "A";

                case 11:
                    return "B";

                case 12:
                    return "C";

                case 13:
                    return "D";

                case 14:
                    return "E";

                case 15:
                    return "F";

                default:
                    return h.ToString(); //if 9 or less
            }
        }

        //the so-called Bailey–Borwein–Plouffe formula for calculating arbitrary digits around 'd' of pi in base 16
        public static double sFinal(int d)
        {
            return (4 * S(d, 1) - 2 * S(d, 4) - S(d, 5) - S(d, 6));
        }


        public static double S(int d, int j)
        {
            double sum = 0;

            for (int k = 0; k <= d; k++)
            {
                double T = Math.Pow(16, d - k);
                double q = 8 * k + j;
                sum += (T % q) / q;
            }
            for (int k = d + 1; k < d + 40; k++)
            {
                double T = Math.Pow(16, d - k);
                double q = 8 * k + j;
                sum += T / q;
            }

            return sum;
        }

    }
}
