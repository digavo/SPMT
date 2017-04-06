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
    class GA_PomiedzyAdresami
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

        public GA_PomiedzyAdresami(string m1, string m2)
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
