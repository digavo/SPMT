using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SPMT
{
    class GA_Adres
    {
        
        //private:   dane
        private string adres;       
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
                string url = @"https://maps.googleapis.com/maps/api/geocode/xml?address=" + adres;
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
                        if (GEO_XY == GET_GEOXY.GEO_X) { XorY = double.Parse(ds.Tables["location"].Rows[0]["lat"].ToString(), System.Globalization.CultureInfo.InvariantCulture); return XorY; } // zwraca wspolrzedne X
                        else if (GEO_XY == GET_GEOXY.GEO_Y) { XorY = double.Parse(ds.Tables["location"].Rows[0]["lng"].ToString(), System.Globalization.CultureInfo.InvariantCulture); return XorY; } // zwraca wspolrzedne Y
                    }
                    //MessageBox.Show("sukces Lokalizacja: \n" + ds.Tables[0].Rows[0]["status"].ToString() + "\n");
                    //MessageBox.Show("sukces Lokalizacja: \n"+ds.Tables["location"].Rows[0]["lat"].ToString() +"\n"+ ds.Tables["location"].Rows[0]["lng"].ToString());                  
                }
            }
            catch { MessageBox.Show("bled podczas pobierania lokalizacji" + adres); status = false; }
            return XorY;
        }

        //public:
        public GA_Adres(string n)       // jaki konstruktor jest kazdy widzi :P  pozatym sam uzupelnia geo_x geo_y i status 
        {
            adres = n;
            status = false;
            geo_x = get_Location_XorY_Town(GET_GEOXY.GEO_X);
            geo_y = get_Location_XorY_Town(GET_GEOXY.GEO_Y);
            if (geo_x != 0 && geo_y != 0) { status = true; } // miasto znalezione wiec jest ok :) 
        }
        public string get_town()        // zwraca nazwe miasta
        {
            return adres;
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
        // bardzo wazne funckje co google API
        protected bool czy_jakosc_powietrza_w_miescie_jest_dobra()
        {
            return false;
        }
        protected bool czy_miasto_nalezy_do_serii_ksiazek_metro_uniwersum() // ciekawe czy ktos wogole przegladnie chociaz ten kod
        {
            if (adres == "Wrocław" || adres == "Moskwa")
                return true;
            else
                return false;
        }
    }

    public class GA_Pomiedzy_Adresami // zawiera dwa miasta oraz info o dystansie i czas miedzy nimi
    {
        private GA_Adres miasto1;
        private GA_Adres miasto2;
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
            this.czas = new TimeSpan(0, ((int)czasowo) / 3600, (((int)czasowo) / 60) % 60, ((int)czasowo) % 60);
            return czasowo;
        }

        public GA_Pomiedzy_Adresami(string m1, string m2)
        {
            this.status = false;
            this.miasto1 = new GA_Adres(m1);
            this.miasto2 = new GA_Adres(m2);
            if (this.miasto1.result_status() == true && this.miasto2.result_status() == true)//miasta sa ok 
            {
                this.dystans = GetTimeORDistance(m1, m2, GET_KM_or_TIME.GET_DISTANCE) / 1000;
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

}
