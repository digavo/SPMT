using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Net;
using System.IO;
using System.Xml;

namespace SPMT
{

    public partial class Form1 : Form
    {
        public class GA_MIASTO  // wszystkie info o miescie
        {
            //private:   dane
            private string name;       // nazwa miasta
            private double geo_x;      // wspolrzedne geograficzne Y
            private double geo_y;      // wspolrzedne geograficzne Y
            private bool status;       // zmienna informujaca czy miasto zostalo poprawnie znalezione przy pomocy googleAPI 
            //private:  metody
            private enum GET_GEOXY { GEO_X, GEO_Y }  
            private double get_Location_XorY_Town(GET_GEOXY GEO_XY) // zwracamy wspolrzedna x albo y 
            {
                double XorY = 0; // to zwracamy jesli sie nie uda
                try
                {
                    string url = @"https://maps.googleapis.com/maps/api/geocode/xml?address=" + name;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader sreader = new StreamReader(dataStream);
                    string responsereader = sreader.ReadToEnd();
                    response.Close();

                    DataSet ds = new DataSet();
                    ds.ReadXml(new XmlTextReader(new StringReader(responsereader)));
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0]["status"].ToString() == "OK")
                        {
                            if (GEO_XY == GET_GEOXY.GEO_X){XorY = double.Parse(ds.Tables["location"].Rows[0]["lat"].ToString(), System.Globalization.CultureInfo.InvariantCulture);return XorY; } // zwraca wspolrzedne X
                            else if (GEO_XY == GET_GEOXY.GEO_Y){XorY = double.Parse(ds.Tables["location"].Rows[0]["lng"].ToString(),System.Globalization.CultureInfo.InvariantCulture); return XorY;} // zwraca wspolrzedne Y
                        }
                        //MessageBox.Show("sukces Lokalizacja: \n" + ds.Tables[0].Rows[0]["status"].ToString() + "\n");
                        //MessageBox.Show("sukces Lokalizacja: \n"+ds.Tables["location"].Rows[0]["lat"].ToString() +"\n"+ ds.Tables["location"].Rows[0]["lng"].ToString());                  
                    }
                }
                catch { MessageBox.Show("bled podczas pobierania lokalizacji" + name); status = false;  }
                return XorY;
            }
            
            //public:
            public GA_MIASTO(string n)       // jaki konstruktor jest kazdy widzi :P  pozatym sam uzupelnia geo_x geo_y i status 
            {
                name = n;
                status = false;
                geo_x = get_Location_XorY_Town(GET_GEOXY.GEO_X);
                geo_y = get_Location_XorY_Town(GET_GEOXY.GEO_Y);
                if(geo_x != 0 && geo_y != 0) { status = true; } // miasto znalezione wiec jest ok :) 
            }
            public string get_town()        // zwraca nazwe miasta
            {
                return name;
            } 
            public double get_geoX()       // zwraca geo x
            {
                return geo_x;
            }  
            public double get_geoY()     // zwraca geo y
            {
                return geo_y;
            } 
            public bool result_status()
            {
                return status;
            }
            protected bool czy_jakosc_powietrza_w_miescie_jest_dobra()
            {
                return false;
            }  
            protected bool czy_miasto_nalezy_do_serii_ksiazek_metro_uniwersum() // ciekawe czy ktos wogole przegladnie chociaz ten kod
            {
                if (name == "Wrocław" || name == "Moskwa")
                    return true;
                else
                    return false;
            }
        }

        public class GA_POMIEDZYMIASTAMI // zawiera dwa miasta oraz info o dystansie i czas miedzy nimi
        {
            private GA_MIASTO miasto1;
            private GA_MIASTO miasto2;
            private double dystans;           // w km
            private TimeSpan czas;            // [dni, h, min, sek] dni=0 bez przesady kurierzy nie beda pracowac ponad 24h :P
            private bool status;               // status czy dystans i czas sa niezerowe oraz czy miasto1 i miasto2 zostaly utworzone poprawnie
            private enum GET_KM_or_TIME { GET_TIME, GET_DISTANCE }
            private double GetTimeORDistance(string origin, string destination, GET_KM_or_TIME SorT) // pobiera czas lub droge z google map api 
            {
                double ST = 0; // to zwracamy jesli sie nie uda
                try
                {
                    string url = @"http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + origin + "&destinations=" + destination + "&sensor=false";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader sreader = new StreamReader(dataStream);
                    string responsereader = sreader.ReadToEnd();
                    response.Close();

                    DataSet ds = new DataSet();
                    ds.ReadXml(new XmlTextReader(new StringReader(responsereader)));
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables["element"].Rows[0]["status"].ToString() == "OK")
                        {
                            if (GET_KM_or_TIME.GET_TIME == SorT) { return double.Parse(ds.Tables["duration"].Rows[0]["value"].ToString()); } // zwraca czas
                            else if (GET_KM_or_TIME.GET_DISTANCE == SorT) { return double.Parse(ds.Tables["distance"].Rows[0]["value"].ToString()); }  // zwraca droge
                        }
                    }
                }
                catch { MessageBox.Show("bled podczas pobierania czasu przejazdu lub dystansu przejazdu dla trasy od " + origin + " do " + destination); }
                return ST;
            }
            private double Set_TimeSpan(double czasowo)  //ustawia wartosc TimeSpan czas
            {
                this.czas = new TimeSpan(0, ((int)czasowo) / 3600, (((int)czasowo)/60) % 60, ((int)czasowo)  % 60);
                return czasowo;
            } 

            public GA_POMIEDZYMIASTAMI(string m1, string m2)
            {
                this.status = false;
                this.miasto1 = new GA_MIASTO(m1);
                this.miasto2 = new GA_MIASTO(m2);
                if (this.miasto1.result_status() == true && this.miasto2.result_status() == true)//miasta sa ok 
                {
                    this.dystans = GetTimeORDistance(m1, m2, GET_KM_or_TIME.GET_DISTANCE)/1000;
                    double czasowka = Set_TimeSpan(GetTimeORDistance(m1, m2, GET_KM_or_TIME.GET_TIME));
                    if (this.dystans != 0 && czasowka != 0) { status = true; }
                }
            }                   // konstruktor polaczenie miedzy miastami 
            public int get_time() { return this.czas.Hours * 60 + czas.Minutes; } //zwraca całkowity czas w minutach 
            public int get_hour() { return this.czas.Hours; }                     //zwraca tylko godziny bez minut
            public int get_min() { return this.czas.Minutes; }                     //zwraca tylko minuty bez godzin
            public string get_miasto1() { return this.miasto1.get_town(); }                  // zwraca pierwsze z miast
            public string get_miasto2() { return this.miasto2.get_town(); }                  // zwraca drugie z miast
            public double get_dystans() { return this.dystans; }                      //zwraca droge pomiedzy nimi
            public bool result_status() { return this.status; }
        }

        public class DaneTrasowe    // zawiera liste wszystkie miasta razem z tablica polaczeniami pomiedzy nimi
        {
            enum GET_DATA { TOWN, CZAS, DISTANCE, GEO_X, GEO_Y }
            private List<GA_MIASTO> lista_miast;                     // lista miast 
            private GA_POMIEDZYMIASTAMI[] Tab_Pom_Miast;            // tablica  zawierajaca odleglosci, czas i miasta potrzebne do komiwojarzera 

            public DaneTrasowe() 
            {
                this.lista_miast = new List<GA_MIASTO>();
            }
            public DaneTrasowe(List<String> listaM)
            {
                this.lista_miast = new List<GA_MIASTO>(listaM.Count);
                for (int i = 0; i < listaM.Count; i++)
                {
                    GA_MIASTO GAM = new GA_MIASTO(listaM[i]);
                    this.lista_miast.Add(GAM);
                }
            }
            public void ADD_LIST(string s)
            {
                GA_MIASTO GAM = new GA_MIASTO(s);
                this.lista_miast.Add(GAM);
            }
            public void DEL_LIST(string s)
            {
                GA_MIASTO GAM = new GA_MIASTO(s);
                this.lista_miast.Remove(GAM);
            }
            public void Dane_googleAPI_read()   // MAGIC !!! 
            {
                if (lista_miast.Count >= 2)
                {
                    int factorial = 1; // silnia bo polaczen miedzymiastowych jest (n-1)! gdzie n to liczba miast
                    for (int i = 1; i <= lista_miast.Count - 1; i++) { factorial *= i; }

                    Tab_Pom_Miast = new GA_POMIEDZYMIASTAMI[factorial];
                    for (int i = 0; i < lista_miast.Count; i++)
                    {
                        for (int j = i + 1; j < lista_miast.Count; j++)
                        {
                            Tab_Pom_Miast[i] = new GA_POMIEDZYMIASTAMI(lista_miast[i].get_town(), lista_miast[j].get_town()); // wywolujemy konstruktor a on robi wszystko za nas :P
                        }
                    }
                }
            }
            public string DANE_IN()// komunikat ktory pozwala w szybki sposob zweryfikowac czy wspolrzedne miasta zostaly wyszukane poprawnie 
            {
                string daneIN = "";
                for (int i = 0; i < this.lista_miast.Count; i++)
                {
                    daneIN += this.lista_miast[i].get_town() + " [" + this.lista_miast[i].get_geoX() + " , " + this.lista_miast[i].get_geoY() + "] status=" + this.lista_miast[i].result_status().ToString() +"\n";
                }
                return daneIN;
            }
            public string DANE_OUT()// komunikat ktory pozwala w szybki sposob zobaczyc do zrobila metoda  Dane_googleAPI_read()
            {
                string daneOUT = "";
                for (int i = 0; i < Tab_Pom_Miast.Length; i++)
                {
                    daneOUT += Tab_Pom_Miast[i].get_miasto1() + "\t" + Tab_Pom_Miast[i].get_miasto2() + "\t" + Tab_Pom_Miast[i].get_dystans() + "km \t" + Tab_Pom_Miast[i].get_hour()+ "h "+ Tab_Pom_Miast[i].get_min()+"min \n";
                }
                return daneOUT;
            }
            public void showTrasa(WebBrowser WB)  // wyswietla transe w WebBrowserze obecnie pomiedzy pierwszym a ostatnim miastem
            {
                if (lista_miast.Count >= 2)
                {
                    String punkt1 = lista_miast[0].get_town();// miasto  poczatkowe
                    String punkt2 = lista_miast[lista_miast.Count - 1].get_town(); // miasto docelowe na razie tylko na pokaz by zobaczyc czy w aplikacji wyswoetla sie trasa
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
                    return lista_miast[index_w_liscie].get_geoX();
                }
                return 0;
            }
            public double get_GEOY(int index_w_liscie)
            {
                if (lista_miast.Count > index_w_liscie)
                {
                    return lista_miast[index_w_liscie].get_geoY();
                }
                return 0;
            }
            public int SIZE_LIST()
            {
                return lista_miast.Count;
            }
            public void get_list_form_salesman(List<int> PoprawnaKolejnoscMiast)
            {
                List<GA_MIASTO> copy_lista_miast =  new List<SPMT.Form1.GA_MIASTO> (lista_miast);
                lista_miast.Clear();
                for (int i=0;i< PoprawnaKolejnoscMiast.Count;i++)
                {
                    this.lista_miast.Add(copy_lista_miast[PoprawnaKolejnoscMiast[i]]);
                }
            }
        }

        public Form1() { InitializeComponent(); Form1_Init();}

        public void Form1_Init()
        {
            //albo w taki sposob
            /* 
            DaneTrasowe Miasteczka = new DaneTrasowe();
            List<String> listaM = new List<String>();
            listaM.Add("Wrocław");
            listaM.Add("Opole");
            DaneTrasowe Miasteczka = new DaneTrasowe(listaM); // konstruktor z parametrem
            */
            // albo w taki sposob
            DaneTrasowe Miasteczka = new DaneTrasowe();
            Miasteczka.ADD_LIST("Wrocław");  // dodajac miast klasa sama tworzac obiekt GA_MIASTO dodaje wspolrzedne geograficzne przy pomocy google api 
            Miasteczka.ADD_LIST("Opole");    // ten sam efekt mozna uzyskac tworzac liste string a potem wywolujac konstruktor z parametrem
            Miasteczka.Dane_googleAPI_read();
            /* Dla Eweliny :)
            for (; Miasteczka.SIZE_LIST();)
            {
                Miasteczka.get_GEOX(i);  //dla miasta i wspolrzedne geograficzne x
                Miasteczka.get_GEOY(i); //dla miasta i wspolrzedne geograficzne y
            }
            */



            string msg1 = Miasteczka.DANE_IN(); // komunikat ktory pozwala w szybki sposob zweryfikowac czy wspolrzedne miasta zostaly wyszukane poprawnie 
            Miasteczka.Dane_googleAPI_read();   //jak juz mamy wszystkie miasta dodane to metoda Dane_googleAPI_read tworzy liste wszystkich polaczen miedzy miastami i dodaje odleglosci miedzy nimi oraz czas przejazdu 
            string msg2 = Miasteczka.DANE_OUT(); // komunikat ktory pozwala w szybki sposob zobaczyc do zrobila metoda  Dane_googleAPI_read()
            MessageBox.Show(msg1.ToString()+ "\n \n "+msg2.ToString()); 
        }
    }
}
