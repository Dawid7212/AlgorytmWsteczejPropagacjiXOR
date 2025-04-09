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
        public static double SiecNeuronowa(double[] wejscie, double[] Osobnik, int LiczbaPmNeurona)
        {
            double[] neurony = new double[Osobnik.Length / LiczbaPmNeurona];
            double wyjscie = 0.0;
            int y = 0;
            for (int i = 0; i < Osobnik.Length / LiczbaPmNeurona; i++)
            {
                double[] PmTymczasowe = new double[LiczbaPmNeurona];
                for (int j = 0; j < LiczbaPmNeurona; j++)
                {
                    PmTymczasowe[j] = Osobnik[y];
                    y++;
                }
                if (i <= 1)
                {
                    neurony[i] = FAktywacji(Neuron(PmTymczasowe, wejscie));

                }
                else
                {
                    double[] neuronki = {
                        neurony[i - 1],
                        neurony[i - 2]
                    };
                    neurony[i] = FAktywacji(Neuron(PmTymczasowe, neuronki));
                }
            }
            wyjscie = neurony[(Osobnik.Length / LiczbaPmNeurona) - 1];
            return wyjscie;
        }
        public static double FAktywacji(double WartNeurona, double B = 1.0)
        {
            return 1.0 / (1.0 + Math.Exp(-B * WartNeurona));
        }

        public static double Neuron(double[] Wagi, double[] wejscie)
        {
            double neuron = 0;

            for (int j = 0; j < wejscie.Length; j++)
            {
                neuron += Wagi[j] * wejscie[j];
            }
            neuron += Wagi[Wagi.Length - 1];
            return neuron;
        }
        static void Main(string[] args)
        {
            
        }
    }
}
