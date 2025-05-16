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
                BledyNeuronow[ostatniaWarstwa - 1][i] =  (OczekiwaneWyjscia[i] - WynikiSieci[ostatniaWarstwa][i]) * (B * WynikiSieci[ostatniaWarstwa][i] * (1 - WynikiSieci[ostatniaWarstwa][i]));

            }

            for (int nrWarstwy = struktura.Length - 2; nrWarstwy > 0; nrWarstwy--)//dla neuronow warstw ukrytych -> iteracja po warstwach
            {
                BledyNeuronow[nrWarstwy - 1] = new double[struktura[nrWarstwy]];
                for (int i = 0; i < struktura[nrWarstwy]; i++)//iteracaja po liczbie neuronow w warstwie
                {
                    double SumaBledow = 0;
                    for (int j = 0; j < struktura[nrWarstwy + 1]; j++)//iteracja po liczbie neuronów w warstwie kolejnej -> każdy neuron z warstwy obeznej ma połączenie ze wszystkimi neuronami warstwy nastepnej
                    {
                        SumaBledow += BledyNeuronow[nrWarstwy][j] * wagi[nrWarstwy][j][i + 1];//tylko dla połączeń miedzy neuronami, błąd neuronu kolejnego* waga łącząca te nurony
                    }
                    BledyNeuronow[nrWarstwy - 1][i] = SumaBledow * (B * WynikiSieci[nrWarstwy][i] * (1 - WynikiSieci[nrWarstwy][i]));
                }

            }

            double[][][] WagiAfterUpdate = new double[wagi.Length][][];
            for (int i = 0; i < wagi.Length; i++)
            {
                WagiAfterUpdate[i] = new double[wagi[i].Length][];
                for (int j = 0; j < wagi[i].Length; j++)
                {
                    WagiAfterUpdate[i][j] = new double[wagi[i][j].Length];
                    WagiAfterUpdate[i][j][0] = wagi[i][j][0] + parametrUczenia * BledyNeuronow[i][j];//waga ukryta jako pierwsza
                    for (int k = 1; k < wagi[i][j].Length; k++)
                    {
                        WagiAfterUpdate[i][j][k] = wagi[i][j][k] + parametrUczenia * BledyNeuronow[i][j] * WynikiSieci[i][k - 1];
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
                    wagi[i][j] = new double[strukturaSieci[i]+1];
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
            int[] Struktura2 = new int[] { 2, 2, 2, 2 };
            int[] Struktura3 = new int[] { 3, 3, 2, 2 };
            double[] wejscie = new double[] { 1, 0 };
            double[][] WynikSieci = SiecNeuronowa(wejscie, wagi, Struktura);

            Console.WriteLine("Neuron 1 warstwy ukrytj: " + WynikSieci[1][0]);
            Console.WriteLine("Neuron 2 warstwy ukrytj: " + WynikSieci[1][1]);
            Console.WriteLine("Neuron wyjsciowy: " + WynikSieci[2][0]);

            double[][] WejsciaSieci1 = new double[][]
           {
                    new double[] { 0, 0 },
                    new double[] { 0, 1 },
                    new double[] { 1, 0 },
                    new double[] { 1, 1 }
           };
            double[] OczekiwaneWYniki = { 0, 1, 1, 0 };
            double[][] OczekiwaneWYniki2 = new double[][]
            {
                new double[]{0, 1},
                new double[]{1, 0},
                new double[]{1, 0},
                new double[]{0, 0},
            };
            double[][] WejsciaSieci3 = new double[][]
            {
                new double[] { 0, 0, 0 },
                new double[] { 0, 1, 0 },
                new double[] { 1, 0, 0 },
                new double[] { 1, 1, 0 },
                new double[] { 0, 0, 1 },
                new double[] { 0, 1, 1 },
                new double[] { 1, 0, 1 },
                new double[] { 1, 1, 1 }
            };

            double[][] OczekiwaneWyniki3 = new double[][]
            {
                new double[] { 0, 0 },
                new double[] { 1, 0 },
                new double[] { 1, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 0, 1 },
                new double[] { 0, 1 },
                new double[] { 1, 1 }
            };

            for (int epoki = 0; epoki < 20000; epoki++)
            {
                if (epoki > 0)
                {
                    for (int i = WejsciaSieci1.Length - 1; i > 0; i--)
                    {
                        int j = rand.Next(i + 1);
                        (WejsciaSieci1[i], WejsciaSieci1[j]) = (WejsciaSieci1[j], WejsciaSieci1[i]);
                        (OczekiwaneWYniki[i], OczekiwaneWYniki[j]) = (OczekiwaneWYniki[j], OczekiwaneWYniki[i]);
                    }
                }
                for (int i = 0; i < WejsciaSieci1.Length; i++)
                {
                    wagi = PropagacjaWsteczna(WejsciaSieci1[i], new[] { OczekiwaneWYniki[i] }, wagi, Struktura, parametrUczenia);
                }
            }

            double[] wejscieT = new double[2];
            Console.WriteLine("Testowanie sieci neuronowej : ");
            Console.WriteLine("Podaj wartość 0/1 dla piwerszego wejścia XOR (ZATWIERDZ ENTEREM):");
            wejscieT[0] = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Podaj wartość 0/1 dla drugiego wejścia XOR (ZATWIERDZ ENTEREM):");
            wejscieT[1] = Convert.ToDouble(Console.ReadLine());
            double[][] WynikSieci2 = SiecNeuronowa(wejscieT, wagi, Struktura);
            Console.WriteLine("Wyjscie sieci: " + WynikSieci2[2][0]);


            while (true)
            {
                Console.Clear();
                Console.WriteLine("Wybierz zadanie i zatwierdz enterem:");
                Console.WriteLine("1  XOR ");
                Console.WriteLine("2  XOR i NOR");
                Console.WriteLine("3  Sumator 3-3-2-2");
                Console.WriteLine("4  Daj mi spokoj");
                Console.Write("Twój wybór: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        XOR();
                        break;
                    case "2":
                        XOR_NOR();
                        break;
                    case "3":
                        Sumatorek();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Nieprawidłowy wybór!");
                        Console.ReadKey();
                        break;
                }
            }

        }
        static void MieszajDane(double[][] wejscia, double[] wyjscia)
        {
            for (int i = wejscia.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (wejscia[i], wejscia[j]) = (wejscia[j], wejscia[i]);
                (wyjscia[i], wyjscia[j]) = (wyjscia[j], wyjscia[i]);
            }
        }

        static void MieszajDane(double[][] wejscia, double[][] wyjscia)
        {
            for (int i = wejscia.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (wejscia[i], wejscia[j]) = (wejscia[j], wejscia[i]);
                (wyjscia[i], wyjscia[j]) = (wyjscia[j], wyjscia[i]);
            }
        }
        static void XOR()
        {
            int[] struktura = {2, 2, 1 }; 
            double[][][] wagi = WylosujWagi(struktura);
            double parametrUczenia = 0.2;
            double[][] WejsciaSieci = new double[][]
           {
                    new double[] { 0, 0 },
                    new double[] { 0, 1 },
                    new double[] { 1, 0 },
                    new double[] { 1, 1 }
           };

            double[] WyjsciaOczekiwane = { 0, 1, 1, 0 };


            for (int epoka = 0; epoka < 20000; epoka++)
            {
                if (epoka > 0)
                {
                    MieszajDane(WejsciaSieci, WyjsciaOczekiwane);
                }
                for (int i = 0; i < WejsciaSieci.Length; i++)
                {
                    wagi = PropagacjaWsteczna(WejsciaSieci[i], new[] { WyjsciaOczekiwane[i] }, wagi, struktura, parametrUczenia);
                }
            }
            for(int i = 0; i < WejsciaSieci.Length; i++)
            {
                double[] wejscieT = new double[2];
                Console.WriteLine("Testowanie sieci neuronowej : ");
                Console.WriteLine("Podaj wartość 0/1 dla piwerszego wejścia XOR (ZATWIERDZ ENTEREM):");
                wejscieT[0] = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Podaj wartość 0/1 dla drugiego wejścia XOR (ZATWIERDZ ENTEREM):");
                wejscieT[1] = Convert.ToDouble(Console.ReadLine());
                double[][] WynikSieci2 = SiecNeuronowa(wejscieT, wagi, struktura);
                Console.WriteLine("Wyjscie sieci: " + WynikSieci2[2][0]);
            }
            Console.Write("Kliknij ENTER aby przejsc dalej");
            Console.ReadKey();

        }

        static void XOR_NOR()
        {
            int[] struktura = { 2, 2, 2, 2 }; 
            double[][][] wagi = WylosujWagi(struktura);
            double parametrUczenia = 0.1;

            double[][] WejsciaSieci = {
                new double[] { 0, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 }
            };

            double[][] WyjsciaOczekiwane = {
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 0 },
                new double[] { 0, 0 }
            };

            for (int epoka = 0; epoka < 40000; epoka++)
            {
                if (epoka > 0)
                {
                    MieszajDane(WejsciaSieci, WyjsciaOczekiwane);
                }
                for (int i = 0; i < WejsciaSieci.Length; i++)
                {
                    wagi = PropagacjaWsteczna(WejsciaSieci[i], WyjsciaOczekiwane[i],wagi,struktura, parametrUczenia);
                }
            }

            for (int i = 0; i < WejsciaSieci.Length; i++)
            {
                double[] wejscieT = new double[2];
                Console.WriteLine("Testowanie sieci (XOR i NOR):");
                Console.Write("Podaj wartość pierwszego wejścia): ");
                wejscieT[0] = Convert.ToDouble(Console.ReadLine());
                Console.Write("Podaj wartość drugiego wejścia : ");
                wejscieT[1] = Convert.ToDouble(Console.ReadLine());

                double[][] wynik = SiecNeuronowa(wejscieT, wagi, struktura);
                Console.WriteLine($"XOR: {wynik.Last()[0]}");
                Console.WriteLine($"NOR: {wynik.Last()[1]}");
            }
            Console.Write("Kliknij ENTER aby przejsc dalej");
            Console.ReadKey();

        }

        static void Sumatorek()
        {
            int[] struktura = { 3, 3, 2, 2 };
            double[][][] wagi = WylosujWagi(struktura);
            double parametrUczenia = 0.05;

            double[][] WejsciaSieci = {
                new double[] { 0, 0, 0 },
                new double[] { 0, 0, 1 },
                new double[] { 0, 1, 0 },
                new double[] { 0, 1, 1 },
                new double[] { 1, 0, 0 },
                new double[] { 1, 0, 1 }, 
                new double[] { 1, 1, 0 }, 
                new double[] { 1, 1, 1 }  
             };

            double[][] WyjsciaOczekiwane = {
                new double[] { 0, 0 },
                new double[] { 1, 0 },
                new double[] { 1, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 0, 1 },
                new double[] { 0, 1 },
                new double[] { 1, 1 }
            };


            for (int epoka = 0; epoka < 80000; epoka++)
            {
                if (epoka > 0)
                {
                    MieszajDane(WejsciaSieci, WyjsciaOczekiwane);
                }
                for (int i = 0; i < WejsciaSieci.Length; i++)
                {
                    wagi = PropagacjaWsteczna(WejsciaSieci[i],WyjsciaOczekiwane[i],wagi,struktura,parametrUczenia );
                }
            }

            for (int i = 0; i < 4; i++)
            {
                double[] wejscieT = new double[3];
                Console.WriteLine("Testowanie sumatora :");
                Console.Write("Podaj wejscie1 (0/1): ");
                wejscieT[0] = Convert.ToDouble(Console.ReadLine());
                Console.Write("Podaj wejscie2 (0/1): ");
                wejscieT[1] = Convert.ToDouble(Console.ReadLine());
                Console.Write("Podaj wejscie3 (0/1): ");
                wejscieT[2] = Convert.ToDouble(Console.ReadLine());

                double[][] wynik = SiecNeuronowa(wejscieT, wagi, struktura);
                Console.WriteLine($"WYjscie0: {wynik.Last()[0]}");
                Console.WriteLine($"Wyjscie1: {wynik.Last()[1]}");
            }
            Console.Write("Kliknij ENTER aby przejsc dalej");
            Console.ReadKey();
        }
    }
}