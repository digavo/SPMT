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
        private double geoX;      // wspolrzedne geograficzne Y
        private double geoY;      // wspolrzedne geograficzne Y
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
            geoX = get_Location_XorY_Town(GET_GEOXY.GEO_X);
            geoY = get_Location_XorY_Town(GET_GEOXY.GEO_Y);
            if (geoX != 0 && geoY != 0)
                status = true; // miasto znalezione wiec jest ok :) 
            else status = false;
        }
        public string getTown        // zwraca nazwe miasta
        {
            get { return adres; }
        }
        public double getGeoX       // zwraca geo x
        {
            get { return geoX; }
        }
        public double getGeoY     // zwraca geo y
        {
            get { return geoY; }
        }
        public bool resultStatus
        {
            get { return status; }
        }
        // ciekawe czy ktos wogole przegladnie chociaz ten kod 
        // tak i usunie ci twoje funkcje i pozmienia nazwy hahaha
        
    }

}
