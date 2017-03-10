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
    
    // taki sobie komentarz2 by sprawdzicz czy git jest ok :P
    public partial class Form1 : Form
    {
        public List<String> lista_miast = new List<String>();
        public Graf[,] tabelapocalosci;
        public class Graf
        {
            private string miasto1;
            private string miasto2;
            private double droga;
            private TimeSpan czas;

            public void set(string m1 = "", string m2 = "", double s = 0.0, int h = 0, int min = 0)
            {
                this.miasto1 = m1;
                this.miasto2 = m2;
                this.droga = s;
                this.czas = new TimeSpan(0, h, min, 0);
            }
            public int gettime() { return czas.Hours * 60 + czas.Minutes; }
            public int gethour() { return czas.Hours; }
            public int getmin() { return czas.Minutes; }
            public string getmiasto1() { return miasto1; }
            public string getmiasto2() { return miasto2; }
            public double getdroga() { return droga; }
        }

        public Form1()
        {
            InitializeComponent();
            lista_miast.Add("Wrocław");
            lista_miast.Add("Opole");
            aktualizuj_liste_miast_do_wyswietlenia();
        }

        private void wyznacz_trase__Click(object sender, EventArgs e)
        {
            //tabelapocalosci = new Graf[lista_miast.Count, lista_miast.Count];
            //webBrowser2.Update();
            //webBrowser1.UserAgent:= 'Mozilla/5.0 (Windows NT 6.1; WOW64; rv:26.0) Gecko/20100101 Firefox/26.0';
            if (lista_miast.Count >= 2)
            {
                zainicjuj_tabele();
                wyswietl_tabele();
                String punkt1 = lista_miast[0];
                String punkt2 = lista_miast[lista_miast.Count - 1];
                String typpojazdu = "/data=!4m2!4m1!3e0"; //auto
                //StringBuilder add = new StringBuilder("https://www.google.pl/maps?q=");
                //add.Append(punkt1);
                //add.Append(punkt2);
                StringBuilder add = new StringBuilder("https://www.google.pl/maps/dir/" + punkt1 + "/" + punkt2 + "@51.1270779,16.9918639,11z" + typpojazdu);
                //StringBuilder add = new StringBuilder("https://google.pl");
                webBrowser2.Navigate(add.ToString());
            
                GetDistance(punkt1, punkt2);
                GetTime(punkt1, punkt2);
            }
            else { MessageBox.Show("bledna liczba miast"); }
        }

        private void plus_Click(object sender, EventArgs e)
        {
            lista_miast.Add(textBox1.Text);
            aktualizuj_liste_miast_do_wyswietlenia();
        }
        private void minus_Click(object sender, EventArgs e) //usuwa miasto z listy miast
        {
            lista_miast.Remove(textBox1.Text);
            aktualizuj_liste_miast_do_wyswietlenia();
        }
        void aktualizuj_liste_miast_do_wyswietlenia()
        {
            string stringout = "Lista miast: \n\n";
            for (int i = 0; i < lista_miast.Count; i++)
            {
                stringout += lista_miast[i] + "\n";
            }
            richTextBox1.Text = stringout;
        }

        public string GetTimeORDistance(string origin, string destination, bool tryb)
        {
            //true to czas
            //false to dystans
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
                        if (tryb == true) { return ds.Tables["duration"].Rows[0]["text"].ToString(); }
                        else { return ds.Tables["distance"].Rows[0]["text"].ToString(); }
                    }
                }
                return "0";
            }
            catch
            {
                if (tryb == true)
                    MessageBox.Show("bledpodczas pobierania czasu przejazdu dla trasy od " + origin + " do " + destination);
                else
                    MessageBox.Show("bledpodczas pobierania dystansu dla trasy od " + origin + " do " + destination);
                return "0";
            }
        }

        public double GetDistance(string origin, string destination)
        {
            string str1 = GetTimeORDistance(origin, destination, false);
            label2.Text = str1;
            String[] substrings = str1.Split(' ');
            try
            {
                double s;
                s = double.Parse(substrings[0], System.Globalization.CultureInfo.InvariantCulture);
                //Double.TryParse(substrings[0],out s);//Convert.ToDouble(substrings[0]);
                return s;
            }
            catch
            {
                MessageBox.Show("bledna konwersja z " + substrings[0] + " na wartosc double ");
                return 0;
            }
        }

        public int GetTime(string origin, string destination)
        {
            string str2 = GetTimeORDistance(origin, destination, true);
            label1.Text = str2;

            String[] substrings = str2.Split(' ');
            string s_h = "0", s_m = "0";
            int t_h = 0, t_m = 0;
            if (substrings[1] == "min" || substrings[1] == "mins")
            {
                s_m = substrings[0];
            }
            else
            {
                s_h = substrings[0];

                try { s_m = substrings[2]; } catch { MessageBox.Show("brak indeksu dla substrings[2] w wyrazeniu" + str2); }
            }
            try
            {
                Int32.TryParse(s_h, out t_h);
                Int32.TryParse(s_m, out t_m);
                return t_h * 60 + t_m;
            }
            catch
            {
                MessageBox.Show("bledna konwersja z " + s_h + " lub " + s_m + " na wartosc int ");
                return 0;
            }
        }

        private void zainicjuj_tabele()
        {
            tabelapocalosci = new Graf[lista_miast.Count, lista_miast.Count];
            for (int i = 0; i < lista_miast.Count; i++)
            {
                for (int j = 0; j < lista_miast.Count; j++)
                {
                    double s = GetDistance(lista_miast[i], lista_miast[j]);
                    int total_min = GetTime(lista_miast[i], lista_miast[j]);
                    tabelapocalosci[i, j] = new Graf();//.set(lista_miast[i], lista_miast[j],0.0,0,0);
                    tabelapocalosci[i, j].set(lista_miast[i], lista_miast[j], s, total_min / 60, total_min % 60);
                }
            }
        }
        private void wyswietl_tabele()
        {

            string daneout = "\t";
            for (int i = 0; i < lista_miast.Count; i++)
            {
                daneout += lista_miast[i] + "\t\t";
            }
            for (int i = 0; i < lista_miast.Count; i++)
            {
                daneout += "\n" + lista_miast[i] + "\t";
                for (int j = 0; j < lista_miast.Count; j++)
                {
                    daneout += tabelapocalosci[i, j].getdroga().ToString() + "km";
                    daneout += "[" + tabelapocalosci[i, j].gethour().ToString() + "h " + tabelapocalosci[i, j].getmin().ToString() + "min]";
                    daneout += "\t";
                }
            }
            MessageBox.Show(daneout);
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
