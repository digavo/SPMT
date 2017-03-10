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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String punkt1 = textBox1.Text+" ";
            String punkt2 = textBox2.Text;
            String typpojazdu = "/data=!4m2!4m1!3e0"; //auto
            //StringBuilder add = new StringBuilder("https://www.google.pl/maps?q=");
            StringBuilder add = new StringBuilder("https://www.google.pl/maps/dir/"+punkt1+"/"+punkt2+"/@51.1270779,16.9918639,11z"+typpojazdu);
            //add.Append(p1);
            //add.Append(p2);
            webBrowser1.Navigate(add.ToString());
            GetDistance(punkt1, punkt2);
        }

        public void GetDistance(string origin, string destination)
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
                    label1.Text = ds.Tables["duration"].Rows[0]["text"].ToString();// czas
                    label2.Text = ds.Tables["distance"].Rows[0]["text"].ToString();// droga
                }
            }

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
