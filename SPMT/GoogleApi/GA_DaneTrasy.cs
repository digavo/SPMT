﻿using System;
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
            if (lista_miast.Count >= 2)
            {
                int l_pol = 0; // silnia bo polaczen miedzymiastowych jest (n-1)! gdzie n to liczba miast
                for (int i = 0; i <= lista_miast.Count-1; i++) { l_pol += i; }
                MessageBox.Show("lpol="+l_pol.ToString());
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
            //MessageBox.Show(Tab_Pom_Miast.Length.ToString());
            for (int i = 0; i < Tab_Pom_Miast.Length; i++)
            {
                daneOUT += Tab_Pom_Miast[i].getMiasto1+ "\t";
                daneOUT += Tab_Pom_Miast[i].getMiasto2 + "\t";
                daneOUT += Tab_Pom_Miast[i].getDystans + "km \t";
                daneOUT += Tab_Pom_Miast[i].getHour + "h ";
                daneOUT += Tab_Pom_Miast[i].getMin + "min \n";
            }
            return daneOUT;
        }
        public void showTrasa(WebBrowser WB)  // wyswietla transe w WebBrowserze obecnie pomiedzy pierwszym a ostatnim miastem
        {
            if (lista_miast.Count >= 2)
            {
                String punkt1 = lista_miast[0].getTown;// miasto  poczatkowe
                String punkt2 = lista_miast[lista_miast.Count - 1].getTown; // miasto docelowe na razie tylko na pokaz by zobaczyc czy w aplikacji wyswoetla sie trasa
                String typpojazdu = "/data=!4m2!4m1!3e0"; //wyznacza trase dlasamochodow 
                                                          //StringBuilder SB = new StringBuilder("https://www.google.pl/maps?q=");add.Append(punkt1);add.Append(punkt2);
                StringBuilder SB = new StringBuilder("https://www.google.pl/maps/dir/" + punkt1 + "/" + punkt2 + "@51.1270779,16.9918639,11z" + typpojazdu);
                WB.Navigate(SB.ToString()); // wyswietla trase pomiedzy pierwszym i ostatnim miaste ma liscie reszte miast pomija
            }
            else { MessageBox.Show("bledna liczba miast"); }
        }

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
