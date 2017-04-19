using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMT
{
    class Wyzazanie
    {
        double[,] tab;
        double Tp = 1000.0;
        double Tk = 0.1;
        double delta;
        double lambda = 0.9;
        double T;
        List<int> trasa = new List<int> { };
        List<int> trasaP = new List<int> { };
        int liczbaMiast;
        bool akceptacja;// = false;

        public Wyzazanie(double[,] tab1, int iloscMiast)
        {
            tab = tab1;
            liczbaMiast = iloscMiast;
            for (int i = 0; i < iloscMiast; i++)
            {
                trasa.Add(i);
                trasaP.Add(i);
            }
        }
        List<int> Kopia(List<int> stara)
        {
            List<int> nowa = new List<int>();
            foreach (int element in stara)
            {
                nowa.Add(element);
            }
            return nowa;
        }

        double D(int a, int b)
        {
            return tab[a, b];
        }
        double L(List<int> Permutacje)//dlugosc trasy
        {
            double suma = 0.0;
            for (int s = 2; s < liczbaMiast; s++)
            {
                suma += D(Permutacje[s - 1], Permutacje[s]);
            }
            return D(Permutacje[0], Permutacje[1]) + suma + D(Permutacje[liczbaMiast - 1], Permutacje[0]);
        }
        void Zamien()
        {
            trasaP = Kopia(trasa);
            Random rnd = new Random();
            int indeks1 = rnd.Next(0, liczbaMiast);
            int indeks2 = rnd.Next(0, liczbaMiast);
            while (indeks1 == indeks2)
            {
                indeks2 = rnd.Next(0, liczbaMiast);
            }
            //MessageBox.Show(indeks1.ToString() + " " + indeks2.ToString());
            int temp = trasaP[indeks1];
            trasaP[indeks1] = trasaP[indeks2];
            trasaP[indeks2] = temp;
            // MessageBox.Show(trasaP[0].ToString() + " " +trasaP[1].ToString() + " "+trasaP[2].ToString() + " "+trasaP[3].ToString() + " ");
        }



        public List<int> Sym_Wyz()
        {
            T = Tp;
            trasaP = Kopia(trasa);
            Random rnd = new Random();
            int licznik = 0;

            while (T > Tk)
            {
                akceptacja = false;
                Zamien();
                if (L(trasaP) < L(trasa))
                {
                    akceptacja = true;
                    //MessageBox.Show("deltaa " + (L(trasaP) - L(trasa)).ToString());
                }
                else
                {
                    delta = L(trasaP) - L(trasa);
                    double prawd = (rnd.Next(0, 100) / 100.0);
                    double eks = Math.Exp(-delta / T);
                    if (prawd < eks)
                    {
                        akceptacja = false;
                        licznik++;
                    }
                    else
                        akceptacja = false;
                }
                if (akceptacja == true)
                {
                    trasa = Kopia(trasaP);
                }
                T = lambda * T;

            }
           // MessageBox.Show("koncowa " + L(trasa).ToString() + " " + licznik.ToString());
            return trasa;
        }
        }
}
