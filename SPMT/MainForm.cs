using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//using DotNetBrowser;
//using DotNetBrowser.WinForms;

namespace SPMT
{
    public partial class MainForm : Form 
    {
        private BindingList<Klient> ListaKlientów = new BindingList<Klient>();
        private BindingList<Zamówienie> ListaZamówień = new BindingList<Zamówienie>();
        private BindingList<Zamówienie> ListaTrasy = new BindingList<Zamówienie>();
        private TransportDbContext ctx = new TransportDbContext();
        public MainForm()
        {
            InitializeComponent();
            panelEmpty.Dock = DockStyle.Fill;
            tabControlZam.Dock = DockStyle.Fill;
            groupTrasa.Dock = DockStyle.Fill;
            groupMapa.Dock = DockStyle.Fill;
            panelEmpty.BringToFront();

            //nie wiem jak u was ale mi na tych foreach'ach zawsze wyskakuje 'System.Data.SqlClient.SqlException' occurred in EntityFramework.dll 
            //Daga to Kris: http://jaryl-lan.blogspot.com/2014/08/localdb-connection-to-localdb-failed.html - może nie masz lokalnej bazy danych, 
            //              jak to nie zadziała to dodamy connection string ręcznie 

            foreach (var k in ctx.Klienci)
                ListaKlientów.Add(k);
            foreach (var z in ctx.Zamówienia)
                ListaZamówień.Add(z);

            listBox1.DataSource = ListaTrasy;
            listBox2.DataSource = ListaZamówień;
            dataGridView2.DataSource = ListaKlientów;
            dataGridView2.Columns["Id"].Visible = false;
            dataGridView2.Columns["AdresId"].Visible = false;
            dataGridView1.DataSource = ListaZamówień;
            

        }

        // MENU
        private void buttonZam_Click(object sender, EventArgs e)
        {
            label2.Text = "Zamówienia";
            tabControlZam.BringToFront();
        }

        private void buttonTrasa_Click(object sender, EventArgs e)
        {
            label2.Text = "Trasa";
            groupTrasa.BringToFront();
        }

        private void buttonMapa_Click(object sender, EventArgs e)
        {
            label2.Text = "Mapa";
            groupMapa.BringToFront();

            foreach (var z in ListaTrasy)
            {
                
                richTextBox2.Text+=z.Odbiorca.Adres.ToString()+"\n";
            }
            //StringBuilder SB = new StringBuilder("https://maps.googleapis.com/maps/api/staticmap?center=Brooklyn+Bridge,New+York,NY&zoom=13&size=600x300&maptype=roadmap&markers=color:blue|label:S|40.702147,-74.015794&markers=color:green|label:G|40.711614,-74.012318");

            String miejsce = "Brooklyn+Bridge,New+York,NY";
            String znacznik1 = "&markers = color:green | label:G | 40.711614,-74.012318";
            String znacznik2 = "&markers = color:blue  | label:S | 40.702147,-74.015794"; 


            StringBuilder SB = new StringBuilder("https://maps.googleapis.com/maps/api/staticmap?center="+miejsce+"&zoom=13&size=600x300"+znacznik1+znacznik2);


            //String punkt1 ="Wroclaw";// miasto  poczatkowe
            //String punkt2 = "Opole"; // miasto docelowe na razie tylko na pokaz by zobaczyc czy w aplikacji wyswoetla sie trasa
            //String typpojazdu = "/data=!4m2!4m1!3e0"; //wyznacza trase dlasamochodow 
                                                      //StringBuilder SB = new StringBuilder("https://www.google.pl/maps?q=");add.Append(punkt1);add.Append(punkt2);
            //StringBuilder SB = new StringBuilder("https://www.google.pl/maps/dir/" + punkt1 + "/" + punkt2 + "@51.1270779,16.9918639,9z" + typpojazdu);

            SetWebBrowserVersion(11001); // musi byc
            webBrowserMAP.Navigate(SB.ToString()); // wyswietla trase pomiedzy pierwszym i ostatnim miaste ma liscie reszte miast pomija

        }

        // Set a registry DWORD value.
        private void SetRegistryDword(string key_name,
            string value_name, int value)
        {
            // Open the key.
            RegistryKey key =
                Registry.CurrentUser.OpenSubKey(key_name, true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey(key_name,
                    RegistryKeyPermissionCheck.ReadWriteSubTree);

            // Set the desired value.
            key.SetValue(value_name, value, RegistryValueKind.DWord);

            key.Close();
        }

        // Make the WebBrowser control emulate the indicated IE version.
        // See:
        //http://msdn.microsoft.com/library/ee330730.aspx#browser_emulation
        private void SetWebBrowserVersion(int ie_version)
        {
            const string key64bit =
                @"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\" +
                @"MAIN\FeatureControl\FEATURE_BROWSER_EMULATION";
            const string key32bit =
                @"SOFTWARE\Microsoft\Internet Explorer\MAIN\" +
                @"FeatureControl\FEATURE_BROWSER_EMULATION";
            string app_name = System.AppDomain.CurrentDomain.FriendlyName;

            // You can do both if you like.
            SetRegistryDword(key64bit, app_name, ie_version);
            SetRegistryDword(key32bit, app_name, ie_version);
        }

        // ZAMÓWIENIA I KLIENCI
        private void buttonZamDodaj_Click(object sender, EventArgs e)
        {

        }
        private void buttonZamUsun_Click(object sender, EventArgs e)
        {
            if (ListaZamówień.Count() == 0) return;
            int curItem = dataGridView1.SelectedRows[0].Index;
            Zamówienie z = ListaZamówień[curItem];
            ListaZamówień.RemoveAt(curItem);
            ctx.Zamówienia.Remove(z);
            ctx.SaveChanges();
        }

        private void buttonZamDodajDoTrasy_Click(object sender, EventArgs e)
        {
            if (ListaZamówień.Count() == 0) return;
            int curItem = dataGridView1.SelectedRows[0].Index;
            Zamówienie z = ListaZamówień[curItem];
            if (!ListaTrasy.Contains(z))
                ListaTrasy.Add(z);
        }
        private void buttonZamDodajKlient_Click(object sender, EventArgs e)
        {
            //FormKlient formularz = new FormKlient();
            //formularz.Show();
        }
        private void buttonZamUsunKlient_Click(object sender, EventArgs e)
        {
            if (ListaKlientów.Count() == 0) return;
            int curItem = dataGridView2.SelectedRows[0].Index;
            Klient k = ListaKlientów[curItem];
            ListaKlientów.RemoveAt(curItem);
            //ctx.Adresy.Remove(k.Adres);
            ctx.Klienci.Remove(k);
            ctx.SaveChanges();
        }

        // TRASA 
        private void buttonTrasaUsunZam_Click(object sender, EventArgs e)
        {
            if (ListaTrasy.Count() == 0) return;
            ListaTrasy.Remove( (Zamówienie)listBox1.SelectedItem);
        }

        private void buttonTrasaDodajZam_Click(object sender, EventArgs e)
        {
            if (ListaZamówień.Count() == 0) return;
            Zamówienie z = (Zamówienie) listBox2.SelectedItem;
            if (!ListaTrasy.Contains(z))
                ListaTrasy.Add(z);
        }

        private void buttonTrasaWyznacz_Click(object sender, EventArgs e)
        {

        }

        private void buttonTrasaUsun_Click(object sender, EventArgs e)
        {

        }

        private void groupTrasa_Enter(object sender, EventArgs e)
        {

        }

        private void groupMapa_Enter(object sender, EventArgs e)
        {

        }
    }
}
