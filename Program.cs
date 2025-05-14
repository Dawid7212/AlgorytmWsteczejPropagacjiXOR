using System;
using System.Linq;
using System.Windows.Forms;

namespace AlgorytmWsteczejPropagacjiXOR
{
    internal class Program
    {

        private static Random rand = new Random();

        public static double[][][] PropagacjaWsteczna(double[] wejscie, double[] OczekiwaneWyjscia, double[][][] wagi, int[] struktura, double parametrUczenia)
        {

            double[][] WynikiSieci = SiecNeuronowa(wejscie, wagi, struktura);
            double B = 1;

            double[][] BledyNeuronow = new double[struktura.Length - 1][]; //[NumerWarstwy][NumerNeuronu]


            int ostatniaWarstwa = struktura.Length - 1;
            BledyNeuronow[ostatniaWarstwa - 1] = new double[struktura[ostatniaWarstwa]];
            for (int i = 0; i < struktura[ostatniaWarstwa]; i++)//dla neuronow warstwy otatniej
            {
                BledyNeuronow[ostatniaWarstwa - 1][i] = parametrUczenia * (OczekiwaneWyjscia[i] - WynikiSieci[ostatniaWarstwa][i]) * (B * WynikiSieci[ostatniaWarstwa][i] * (1 - WynikiSieci[ostatniaWarstwa][i]));

            }

            for (int nrWarstwy = struktura.Length - 2; nrWarstwy > 0; nrWarstwy--)//dla neuronow warstw ukrytych -> iteracja po warstwach
            {
                BledyNeuronow[nrWarstwy - 1] = new double[struktura[nrWarstwy]];
                for (int i = 0; i < struktura[nrWarstwy]; i++)//iteracaja po liczbie neuronow w warstwie
                {
                    double SumaBledow = 0;
                    for (int j = 0; j < struktura[nrWarstwy + 1]; j++)//iteracja po liczbie neuronów w warstwie kolejnej -> każdy neuron z warstwy obeznej ma połączenie ze wszystkimi neuronami warstwy nastepnej
                    {
                        SumaBledow += BledyNeuronow[nrWarstwy + 1][j] * wagi[nrWarstwy][j][i + 1];//tylko dla połączeń miedzy neuronami, błąd neuronu kolejnego* waga łącząca te nurony
                    }
                    BledyNeuronow[nrWarstwy - 1][i] = SumaBledow * (B * WynikiSieci[nrWarstwy][i] * (1 - WynikiSieci[nrWarstwy][i]));
                }

            }

            double[][][] WagiAfterUpdate = new double[wagi.Length][][];
            for (int i = 0; i < wagi.Length; i++)
            {
                WagiAfterUpdate[i] = new double[wagi[i].Length][];
                for (int j =0; j < wagi[i].Length;j++)
                {
                    WagiAfterUpdate[i][j] = new double[wagi[i][j].Length];
                    WagiAfterUpdate[i][j][0] = wagi[i][j][0] + BledyNeuronow[i][j];//waga ukryta jako pierwsza
                    for(int k = 1; k < wagi[i][j].Length; k++)
                    {
                        WagiAfterUpdate[i][j][k] = wagi[i][j][k] + BledyNeuronow[i][j] * WynikiSieci[i][k - 1]; 
                    }
                }
            }

            return WagiAfterUpdate;
        }




        public static double[][][] WylosujWagi(int[] strukturaSieci)
        {
            //int liczbaWag = 0;
            double[][][] wagi = new double[strukturaSieci.Length - 1][][];
            for (int i = 0; i < strukturaSieci.Length - 1; i++)
            {
                wagi[i] = new double[strukturaSieci[i + 1]][];//+1 bo liczba wag zalezna od nastepnej warstwy
                for(int j =0; j < strukturaSieci[i + 1]; j++)
                {
                    for(int ktoraWaga = 0; ktoraWaga < strukturaSieci[i] + 1; ktoraWaga++)
                    {
                        wagi[i][j][ktoraWaga] = (rand.NextDouble()*2)-1;
                    }
                }
            }
                

            return wagi;
        }
        public static double[][] SiecNeuronowa(double[] wejscie, double[][][] Wagi, int[] struktura)
        {
            int lWarstw = struktura.Length;
            double[][] wyjscie = new double[lWarstw][]; // to będzie zlepek wszystkich wyników neuronów podzielonych na warstwy

            wyjscie[0] = new double[wejscie.Length];
            for (int i = 0; i < wejscie.Length; i++)
            {
                wyjscie[0][i] = wejscie[i];
            }


            for (int i = 1; i < lWarstw; i++)
            {
                int LiczbaNeuronowWarstwy = struktura[i];
                int LiczbaNuronowPoprzedniejWarstwy = struktura[i - 1];
                wyjscie[i] = new double[LiczbaNeuronowWarstwy];

                for (int n = 0; n < LiczbaNeuronowWarstwy; n++)
                {
                    double[] wagiNeuronu = Wagi[i - 1][n];

                    double z = Neuron(wagiNeuronu, wyjscie[i - 1]);
                    wyjscie[i][n] = FAktywacji(z);
                }
            }
            return wyjscie;
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
                neuron += Wagi[j] * wejscie[j - 1];
            }
            return neuron;
        }
        static void Main(string[] args)
        {
            double parametrUczenia = 0.2;
            int liczbaWejsc = 2;
            int liczbaWarstw = 4;
            int liczbaWarstwUkrytych = 2;
            int liczbaNeuWarstwUkrytej = 2;
            int liczbaWyjsc = 1;
            int[] WarstwyIneurny = new int[liczbaWarstw];
            WarstwyIneurny[0] = liczbaWejsc;
            for (int i = 1; i <= liczbaWarstwUkrytych; i++)
            {
                WarstwyIneurny[i] = liczbaNeuWarstwUkrytej;
            }
            WarstwyIneurny[liczbaWarstw - 1] = liczbaWyjsc;
            //double[] wagi = new double[] { 0.3, 0.1, 0.2, 0.6, 0.4, 0.5, 0.9, 0.7, -0.8 };
            double[][][] wagi = new double[2][][];
            wagi[0] = new double[2][];
            wagi[1] = new double[1][];
            wagi[0][0] = new double[] { 0.3, 0.1, 0.2  };
            wagi[0][1] = new double[] { 0.6, 0.4, 0.5 };
            wagi[1][0] = new double[] { 0.9, 0.7, -0.8 };
           
            int[] Struktura = new int[] { 2, 2, 1 };
            double[] wejscie = new double[] { 1, 0 };
            double[][] WynikSieci = SiecNeuronowa(wejscie, wagi, Struktura);

            Console.WriteLine("Neuron 1 warstwy ukrytj: " + WynikSieci[1][0]);
            Console.WriteLine("Neuron 2 warstwy ukrytj: " + WynikSieci[1][1]);
            Console.WriteLine("Neuron wyjsciowy: " + WynikSieci[2][0]);

            double[][] WejsciaSieci = new double[][]
           {
                    new double[] { 0, 0 },
                    new double[] { 0, 1 },
                    new double[] { 1, 0 },
                    new double[] { 1, 1 }
           };
            double[] OczekiwaneWYniki = { 0, 1, 1, 0 };

            for (int epoki = 0; epoki < 20000; epoki++)
            {
                for (int i = 0; i < WejsciaSieci.Length; i++)
                {
                    // Propagacja wsteczna i aktualizacja wag
                    wagi = PropagacjaWsteczna(WejsciaSieci[i], new[] { OczekiwaneWYniki[i] }, wagi, Struktura, parametrUczenia);
                }
            }
                /*
                double ParametrUczenia = 0.3;
                int LicznaNeuronów = 3;
                int LiczbaWagNeurona = 3;
                double B = 1;
                WylosowaneWagi = new double[LicznaNeuronów * LiczbaWagNeurona];
                for (int i = 0; i < WylosowaneWagi.Length; i++)
                {
                    WylosowaneWagi[i] = (rand.NextDouble() * 2) - 1;
                }
                //WylosowaneWagi = new double[] { 0.3, 0.1, 0.2, 0.6, 0.4, 0.5, 0.9, 0.7, -0.8 };//tego uzywalem tylko dla testow sieci



                double[][] WejsciaSieci = new double[][]
                {
                    new double[] { 0, 0 },
                    new double[] { 0, 1 },
                    new double[] { 1, 0 },
                    new double[] { 1, 1 }
                };
                double[] OczekiwaneWYniki = { 0, 1, 1, 0 };

                for (int epoki = 0; epoki < 20000; epoki++)
                {
                    //dla epok kolejnych niż pierwsza - wejscia uczące sieć są w różniej kolejności
                    if (epoki > 0)
                    {
                        for (int i = WejsciaSieci.Length - 1; i > 0; i--)
                        {
                            int j = rand.Next(i + 1);
                            (WejsciaSieci[i], WejsciaSieci[j]) = (WejsciaSieci[j], WejsciaSieci[i]);
                            (OczekiwaneWYniki[i], OczekiwaneWYniki[j]) = (OczekiwaneWYniki[j], OczekiwaneWYniki[i]);
                        }
                    }

                    for (int i = 0; i < WejsciaSieci.Length; i++)
                    {

                        (double NUkryrty1, double Nukryty2, double Nwyjsciowy) = SiecNeuronowa(WejsciaSieci[i], WylosowaneWagi, LiczbaWagNeurona, WarstwyIneurny);
                        double d = OczekiwaneWYniki[i];
                        //double bladwyjscia1 = d - Nwyjsciowy;
                        //Console.WriteLine("Blad wyjsccia1 = "+bladwyjscia1);
                        double PoprawkaWyjscia = ParametrUczenia*(d - Nwyjsciowy) *(B* Nwyjsciowy * (1 - Nwyjsciowy));//pmUczenia*błąd wyjscia * pochodna
                        double PoprawkaNUkryty1 = PoprawkaWyjscia * WylosowaneWagi[7] *(B* NUkryrty1 * (1 - NUkryrty1));
                        double PoprawkaNUkryty2 = PoprawkaWyjscia * WylosowaneWagi[8] * (B*Nukryty2 * (1 - Nukryty2));

                        WylosowaneWagi[6] +=  PoprawkaWyjscia;
                        WylosowaneWagi[7] += (PoprawkaWyjscia * NUkryrty1);
                        WylosowaneWagi[8] += (PoprawkaWyjscia * Nukryty2);

                        WylosowaneWagi[0] += PoprawkaNUkryty1;
                        WylosowaneWagi[1] += (PoprawkaNUkryty1 * WejsciaSieci[i][0]);
                        WylosowaneWagi[2] += (PoprawkaNUkryty1 * WejsciaSieci[i][1]);

                        WylosowaneWagi[3] += PoprawkaNUkryty2;
                        WylosowaneWagi[4] += PoprawkaNUkryty2 * WejsciaSieci[i][0];
                        WylosowaneWagi[5] += PoprawkaNUkryty2 * WejsciaSieci[i][1];

                        //(double NUkryrty1Test, double Nukryty2Test, double NwyjsciowyTest) = SiecNeuronowa(WejsciaSieci[i], WylosowaneWagi, LiczbaWagNeurona);
                        // double bladwyjscia2 = d - NwyjsciowyTest;
                        //Console.WriteLine("Blad wyjsccia2 = " + bladwyjscia2);
                    }

                }
                Console.WriteLine("Końcowe wagi:");
                Console.WriteLine("pierwszy neuron (warstwa ukryta) = " + WylosowaneWagi[0] + " , " + WylosowaneWagi[1] + " , " + WylosowaneWagi[2]);
                Console.WriteLine("drugi neuron (warstwa ukryta) = " + WylosowaneWagi[3] + " , " + WylosowaneWagi[4] + " , " + WylosowaneWagi[5]);
                Console.WriteLine("ostatni neuron (warstwa wyjciowa) = " + WylosowaneWagi[6] + " , " + WylosowaneWagi[7] + " , " + WylosowaneWagi[8]);

                double[] wejscie = new double[2];
                Console.WriteLine("Testowanie sieci neuronowej : ");
                Console.WriteLine("Podaj wartość 0/1 dla piwerszego wejścia XOR (ZATWIERDZ ENTEREM):");
                wejscie[0] = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Podaj wartość 0/1 dla drugiego wejścia XOR (ZATWIERDZ ENTEREM):");
                wejscie[1] = Convert.ToDouble(Console.ReadLine());
                (double x1, double x2, double wyjscie) = SiecNeuronowa(wejscie, WylosowaneWagi, LiczbaWagNeurona, WarstwyIneurny);
                Console.WriteLine("Wyjscie sieci: " + wyjscie);
                */
                Console.ReadKey();

        }
    }
}