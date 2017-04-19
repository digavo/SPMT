using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPMT
{
    class GA_DaneTrasy
    {
        enum GET_DATA { TOWN, CZAS, DISTANCE, GEO_X, GEO_Y }
        private List<GA_Adres> lista_miast;                     // lista miast 
        private GA_PomiedzyAdresami[] Tab_Pom_Miast;            // tablica  zawierajaca odleglosci, czas i miasta potrzebne do komiwojarzera 
        private GA_View ga_mapka;
        private double cala_droga_przejazdu;
        private double caly_czas_przejazdu;
        private TimeSpan caly_TiemSpan_przejazdu;

        public double getS(string miasto1, string miasto2)
        {
            double droga = 0;
            try
            {
                for (int i = 0; i < Tab_Pom_Miast.Length; i++)
                {
                    if ((Tab_Pom_Miast[i].getMiasto1 == miasto1 && Tab_Pom_Miast[i].getMiasto2 == miasto2) || (Tab_Pom_Miast[i].getMiasto1 == miasto2 && Tab_Pom_Miast[i].getMiasto2 == miasto1))
                    {
                        return Tab_Pom_Miast[i].getDystans;
                    }
                }
            }
            catch { MessageBox.Show("Blad getST,Za malo punktow adresowych albo nie wywolano funkcji googleAPIRead "); }
            //MessageBox.Show(s);
            return droga;//s
        }
        private double getT(string miasto1, string miasto2)
        {

            double t = 0;
            try
            {
                for (int i = 0; i < Tab_Pom_Miast.Length; i++)
                {
                    if ((Tab_Pom_Miast[i].getMiasto1 == miasto1 && Tab_Pom_Miast[i].getMiasto2 == miasto2) || (Tab_Pom_Miast[i].getMiasto1 == miasto2 && Tab_Pom_Miast[i].getMiasto2 == miasto1))
                    {
                        return Tab_Pom_Miast[i].getDetailTime;
                    }
                }
            }
            catch { MessageBox.Show("Blad getST,Za malo punktow adresowych albo nie wywolano funkcji googleAPIRead "); }
            //MessageBox.Show(s);
            return t;
        }
        private TimeSpan getTimeSpan(string miasto1, string miasto2)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0);
            try
            {
                for (int i = 0; i < Tab_Pom_Miast.Length; i++)
                {
                    if ((Tab_Pom_Miast[i].getMiasto1 == miasto1 && Tab_Pom_Miast[i].getMiasto2 == miasto2) || (Tab_Pom_Miast[i].getMiasto1 == miasto2 && Tab_Pom_Miast[i].getMiasto2 == miasto1))
                    {
                        return Tab_Pom_Miast[i].getTimeSpan;
                    }
                }
            }
            catch { MessageBox.Show("Blad getST,Za malo punktow adresowych albo nie wywolano funkcji googleAPIRead "); }
            //MessageBox.Show(s);
            return ts;
        }

        public GA_DaneTrasy()
        {
            this.lista_miast = new List<GA_Adres>();
        }
        public GA_DaneTrasy(List<String> listaM)
        {
            this.lista_miast = new List<GA_Adres>(listaM.Count);
            for (int i = 0; i < listaM.Count; i++)
            {
                GA_Adres GAM = new GA_Adres(listaM[i]);
                this.lista_miast.Add(GAM);
            }
        }
        public void ADD_LIST(string s)
        {
            //GA_Adres GAM = new GA_Adres(s);
            this.lista_miast.Add(new GA_Adres(s));
        }
        public void DEL_LIST(string s)
        {
            GA_Adres GAM = new GA_Adres(s);
            this.lista_miast.Remove(GAM);
        }
        public void Dane_googleAPI_read()   // MAGIC !!! 
        {
            caly_TiemSpan_przejazdu = new TimeSpan(0, 0, 0, 0);
            if (lista_miast.Count >= 2)
            {
                int l_pol = 0; // silnia bo polaczen miedzymiastowych jest (n-1)! gdzie n to liczba miast
                for (int i = 0; i <= lista_miast.Count - 1; i++) { l_pol += i; }
                //MessageBox.Show("lpol="+l_pol.ToString());
                Tab_Pom_Miast = new GA_PomiedzyAdresami[l_pol];
                int iter = 0;
                for (int i = 0; i < lista_miast.Count; i++)
                {
                    for (int j = i + 1; j < lista_miast.Count; j++)
                    {
                        Tab_Pom_Miast[iter] = new GA_PomiedzyAdresami(lista_miast[i].getTown, lista_miast[j].getTown); // wywolujemy konstruktor a on robi wszystko za nas :P
                        iter++;
                    }
                }
                //sprawdzenie czy jest ok
                for (int i = 0; i < l_pol; i++)
                {
                    if (Tab_Pom_Miast[i].resultStatus == false)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            Tab_Pom_Miast[i] = new GA_PomiedzyAdresami(Tab_Pom_Miast[i].getMiasto1, Tab_Pom_Miast[i].getMiasto2);
                            if (Tab_Pom_Miast[i].resultStatus == true) { j = 10; }
                        }
                        if (Tab_Pom_Miast[i].resultStatus == false) { MessageBox.Show("Nie mozna odczytać dla połączenia: " + Tab_Pom_Miast[i].getMiasto1.ToString() + " i " + Tab_Pom_Miast[i].getMiasto2.ToString()); }
                    }
                }

            }
        }
        public string DANE_IN()// komunikat ktory pozwala w szybki sposob zweryfikowac czy wspolrzedne miasta zostaly wyszukane poprawnie 
        {
            string daneIN = "";
            for (int i = 0; i < this.lista_miast.Count; i++)
            {
                daneIN += this.lista_miast[i].getTown + " [" + this.lista_miast[i].getGeoX + " , " + this.lista_miast[i].getGeoY + "] status=" + this.lista_miast[i].resultStatus.ToString() + "\n";
            }
            return daneIN;
        }
        public string DANE_OUT()// komunikat ktory pozwala w szybki sposob zobaczyc do zrobila metoda  Dane_googleAPI_read()
        {
            string daneOUT = "";
            try
            {
                for (int i = 0; i < Tab_Pom_Miast.Length; i++)
                {
                    daneOUT += Tab_Pom_Miast[i].getMiasto1 + "\t";
                    daneOUT += Tab_Pom_Miast[i].getMiasto2 + "\t";
                    daneOUT += Tab_Pom_Miast[i].getDystans + "km \t";
                    daneOUT += Tab_Pom_Miast[i].getHour + "h ";
                    daneOUT += Tab_Pom_Miast[i].getMin + "min ";
                    daneOUT += Tab_Pom_Miast[i].getSec + "s \n";
                }
            }
            catch { MessageBox.Show("Blad DANEOUT, Za malo punktow adresowych albo nie wywolano funkcji googleAPIRead "); }
            return daneOUT;
        }
        public void showTrasa(WebBrowser WB)  // wyswietla transe w WebBrowserze obecnie pomiedzy pierwszym a ostatnim miastem
        {
            if (lista_miast.Count >= 2)
            {
                List<string> lm = new List<string>();
                for (int i = 0; i < lista_miast.Count; i++) { lm.Add(lista_miast[i].getTown); }
                ga_mapka = new GA_View();
                ga_mapka.dynmap_show(lm, WB);
            }
            else { MessageBox.Show("bledna liczba miast"); }
        }

        public void calculate_ST()
        {
            double sumS = 0;
            TimeSpan sumTime = new TimeSpan(0, 0, 0, 0);
            double sumT = 0;
            //string g = "";
            for (int i = 1; i < lista_miast.Count; i++)
            {
                sumS += getS(lista_miast[i - 1].getTown, lista_miast[i].getTown);
                sumTime += getTimeSpan(lista_miast[i - 1].getTown, lista_miast[i].getTown);
                sumT += getT(lista_miast[i - 1].getTown, lista_miast[i].getTown);
                //g+= getST(lista_miast[i - 1].getTown, lista_miast[i].getTown);
            }
            //MessageBox.Show("dane out:\n" + g);
            this.cala_droga_przejazdu = sumS;
            this.caly_czas_przejazdu = sumT;
            this.caly_TiemSpan_przejazdu = sumTime;
        }
        public double caly_czas() { return this.caly_czas_przejazdu; }
        public double cala_droga() { return this.cala_droga_przejazdu; }
        //public string cala_TimeSpan() { return caly_TiemSpan_przejazdu.Days.ToString() + "dni "+caly_TiemSpan_przejazdu.Hours.ToString() + "h " + caly_TiemSpan_przejazdu.Minutes.ToString() + "min " + caly_TiemSpan_przejazdu.Seconds.ToString() + "sec "; }
        public string cala_TimeSpan() { return caly_TiemSpan_przejazdu.Hours.ToString() + "h " + caly_TiemSpan_przejazdu.Minutes.ToString() + "min " + caly_TiemSpan_przejazdu.Seconds.ToString() + "sec "; }


        //Funkcje dla Eweliny
        public double get_GEOX(int index_w_liscie)
        {
            if (lista_miast.Count > index_w_liscie)
            {
                return lista_miast[index_w_liscie].getGeoX;
            }
            return 0;
        }
        public double get_GEOY(int index_w_liscie)
        {
            if (lista_miast.Count > index_w_liscie)
            {
                return lista_miast[index_w_liscie].getGeoY;
            }
            return 0;
        }
        public string get_TowN(int i)
        {
            return lista_miast[i].getTown;
        }
        public int SIZE_LIST()
        {
            return lista_miast.Count;
        }
        public void get_list_form_salesman(List<int> PoprawnaKolejnoscMiast)
        {
            List<GA_Adres> copy_lista_miast = new List<GA_Adres>(lista_miast);
            lista_miast.Clear();
            for (int i = 0; i < PoprawnaKolejnoscMiast.Count; i++)
            {
                this.lista_miast.Add(copy_lista_miast[PoprawnaKolejnoscMiast[i]]);
            }
        }
    }
}
