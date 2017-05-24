using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
            string url = @"http://maps.googleapis.com/maps/api/distancematrix/xml?origins=" + origin + "&destinations=" + destination + "&sensor=false";
            string toShow = "";
            try
            {
                bool overQuery = false;
                do
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(url);

                    string status1 = xDoc.SelectSingleNode("/DistanceMatrixResponse/status").InnerText;
                    toShow += status1 + " ";
                    if (status1=="OK")
                    {
                        string status2 = xDoc.SelectSingleNode("/DistanceMatrixResponse/row/element/status").InnerText;
                        toShow += status2 + " ";
                        if (status2 == "OK")
                        {
                            string duration = xDoc.SelectSingleNode("/DistanceMatrixResponse/row/element/duration/value").InnerText;
                            string distance = xDoc.SelectSingleNode("/DistanceMatrixResponse/row/element/distance/value").InnerText;
                            toShow += duration + " " +distance;
                            if (GET_KM_or_TIME.GET_TIME == SorT)
                                return double.Parse(duration);
                            else if (GET_KM_or_TIME.GET_DISTANCE == SorT)
                                return double.Parse(distance);
                        }
                    }
                    else //OVER QUERY LIMIT
                    {
                        MessageBox.Show(status1);
                        Thread.Sleep(2000); //2s
                        overQuery = true;
                    }





                    /*HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader sreader = new StreamReader(dataStream);
                    string responsereader = sreader.ReadToEnd();
                    response.Close();
                    toShow += responsereader + "\n";
                    DataSet ds = new DataSet();
                    ds.ReadXml(new XmlTextReader(new StringReader(responsereader)));
                    toShow += " ile kolumn: " + ds.Tables.Count + ", 0: " + ds.Tables[0] + " ";
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables["status"].ToString() == "OK")
                        {
                            if (ds.Tables["element"].Rows[0]["status"].ToString() == "OK")
                            {
                                if (GET_KM_or_TIME.GET_TIME == SorT) { return double.Parse(ds.Tables["duration"].Rows[0]["value"].ToString()); } // zwraca czas
                                else if (GET_KM_or_TIME.GET_DISTANCE == SorT) { return double.Parse(ds.Tables["distance"].Rows[0]["value"].ToString()); }  // zwraca droge
                            }
                            overQuery = false;
                        }
                        else //OVER QUERY LIMIT
                        {
                            MessageBox.Show(ds.Tables["status"].ToString());
                            Thread.Sleep(2000); //2s
                            overQuery = true;
                        }
                    }*/
                } while (!overQuery);
                
            }
            catch (Exception ex) { MessageBox.Show("błąd podczas pobierania czasu przejazdu lub dystansu przejazdu dla trasy od " + origin + " do " + destination+"\n"+ex.ToString()+"\n"+url+"\n"+toShow); }
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
            if (this.miasto1.resultStatus == true && this.miasto2.resultStatus == true)//miasta sa ok 
            {
                this.dystans = GetTimeORDistance(m1, m2, GET_KM_or_TIME.GET_DISTANCE) / 1000;
                double czasowka = Set_TimeSpan(GetTimeORDistance(m1, m2, GET_KM_or_TIME.GET_TIME));
                if (this.dystans != 0 && czasowka != 0) { status = true; }
                else if (m1==m2) { status = true; }
            }
        }                   // konstruktor polaczenie miedzy miastami 

        public double getDetailTime //zwraca szczaegolowy czas w minutach po przecinku w sekundach  [60*h+min.sek]
        {
            get {
                double t1 = ( (double) (this.czas.Hours * 60 + this.czas.Minutes));
                double t2 = ( (double) (this.czas.Seconds)) / 100;
                return t1+t2;
            }
        } //zwraca całkowity czas w minutach 
          //public int getTime { get { return this.czas.Hours * 60 + czas.Minutes; } } //zwraca całkowity czas w minutach 
        public TimeSpan getTimeSpan  { get{return czas;}} 

        public int getHour { get { return this.czas.Hours; } }                     //zwraca tylko godziny bez minut
        public int getMin { get { return this.czas.Minutes; } }                  //zwraca tylko minuty bez godzin
        public int getSec { get { return this.czas.Seconds; } }                  //zwraca tylko sekundy 
        public string getMiasto1 { get { return this.miasto1.getTown; } }              // zwraca pierwsze z miast
        public string getMiasto2 { get { return this.miasto2.getTown; } }                 // zwraca drugie z miast
        public double getDystans { get { return this.dystans; } }                    //zwraca droge pomiedzy nimi
        public bool resultStatus { get { return this.status; } }
    }
}
