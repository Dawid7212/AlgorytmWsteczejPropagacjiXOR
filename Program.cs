using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmWsteczejPropagacjiXOR
{
    internal class Program
    {
        private static Random rand = new Random();
        public static (double, double, double) SiecNeuronowa(double[] wejscie, double[] Wagi, int LiczbaWagNeurona)
        {
            double[] neurony = new double[Wagi.Length / LiczbaWagNeurona];
            int y = 0;
            for (int i = 0; i < Wagi.Length / LiczbaWagNeurona; i++)
            {
                double[] PmTymczasowe = new double[LiczbaWagNeurona];
                for (int j = 0; j < LiczbaWagNeurona; j++)
                {
                    PmTymczasowe[j] = Wagi[y];
                    y++;
                }
                if (i <= 1)
                {
                    neurony[i] = FAktywacji(Neuron(PmTymczasowe, wejscie));

                }
                else
                {
                    double[] neuronki = {
                        neurony[0],
                        neurony[1]
                    };
                    neurony[i] = FAktywacji(Neuron(PmTymczasowe, neuronki));
                }
            }
            return (neurony[0], neurony[1], neurony[2]);
        }
        public static double FAktywacji(double WartNeurona, double B = 1.0)
        {
            return 1.0 / (1.0 + Math.Exp(-B * WartNeurona));
        }

        public static double Neuron(double[] Wagi, double[] wejscie)
        {
            double neuron = 0;
            neuron += Wagi[0];
            for (int j = 1; j <= wejscie.Length; j++)
            {
                neuron += Wagi[j] * wejscie[j-1];
            }  
            return neuron;
        }
        static void Main(string[] args)
        {
            double ParametrUczenia = 0.3;
            int LicznaNeuronów = 3;
            int LiczbaWagNeurona = 3;
            double[] WylosowaneWagi = new double[LicznaNeuronów * LiczbaWagNeurona];
            for (int i = 0; i < WylosowaneWagi.Length; i++)
            {
                WylosowaneWagi[i] = (rand.NextDouble() * 2) - 1;
            }
            double[][] WejsciaSieci = new double[][]
            {
                new double[] { 0, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 }
            };
            double[] OczekiwaneWYniki = { 0, 1, 1, 0 };
            for (int epoki = 0; epoki < 2; epoki++)
            {
                for (int i = 0; i < WejsciaSieci.Length; i++)
                {
                    (double NUkryrty1, double Nukryty2, double Nwyjsciowy) = SiecNeuronowa(WejsciaSieci[i], WylosowaneWagi, LiczbaWagNeurona);
                    double d = OczekiwaneWYniki[i];
                    double bladwyjscia1 = d - Nwyjsciowy;
                    Console.WriteLine("Blad wyjsccia1 = "+bladwyjscia1);
                    double PoprawkaWyjscia = (d - Nwyjsciowy) * Nwyjsciowy * (1 - Nwyjsciowy);
                    double PoprawkaNUkryty1 = PoprawkaWyjscia * WylosowaneWagi[7] * NUkryrty1 * (1 - NUkryrty1);
                    double PoprawkaNUkryty2 = PoprawkaWyjscia * WylosowaneWagi[8] * Nukryty2 * (1 - Nukryty2);

                    WylosowaneWagi[6] += ParametrUczenia * PoprawkaWyjscia; 
                    WylosowaneWagi[7] += ParametrUczenia * PoprawkaWyjscia * NUkryrty1; 
                    WylosowaneWagi[8] += ParametrUczenia * PoprawkaWyjscia * Nukryty2; 

                    WylosowaneWagi[0] += ParametrUczenia * PoprawkaNUkryty1;
                    WylosowaneWagi[1] += ParametrUczenia * PoprawkaNUkryty1 * WejsciaSieci[i][0];
                    WylosowaneWagi[2] += ParametrUczenia * PoprawkaNUkryty1 * WejsciaSieci[i][1];
                   
                    WylosowaneWagi[3] += ParametrUczenia * PoprawkaNUkryty2;
                    WylosowaneWagi[4] += ParametrUczenia * PoprawkaNUkryty2 * WejsciaSieci[i][0];
                    WylosowaneWagi[5] += ParametrUczenia * PoprawkaNUkryty2 * WejsciaSieci[i][1];

                    (double NUkryrty1Test, double Nukryty2Test, double NwyjsciowyTest) = SiecNeuronowa(WejsciaSieci[i], WylosowaneWagi, LiczbaWagNeurona);
                    double bladwyjscia2 = d - NwyjsciowyTest;
                    Console.WriteLine("Blad wyjsccia2 = " + bladwyjscia2);
                }

            }
            Console.ReadKey();
        }
    }
}
