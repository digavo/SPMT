using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            dataGridView1.Columns["DataDostarczenia"].Visible = false;
            dataGridView1.Columns["NadawcaId"].Visible = false;
            dataGridView1.Columns["OdbiorcaId"].Visible = false;
        }

        //GOOGLE MAPS

        private void SetRegistryDword(string key_name, string value_name, int value)
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
            GA_DaneTrasy mydt = new GA_DaneTrasy();

            richTextBox2.Text = "";
            foreach (var z in ListaTrasy)
            {
                mydt.ADD_LIST(z.Odbiorca.Adres.ToString());
                richTextBox2.Text += z.Odbiorca.Adres.ToString() + "\n";
            }

            //mydt.ADD_LIST("Wrocław");
            //mydt.ADD_LIST("Opole");
            //mydt.ADD_LIST("Kraków");
            //mydt.ADD_LIST("Łódź");
            //mydt.ADD_LIST("Kielce");
            //mydt.Dane_googleAPI_read();
            //MessageBox.Show(mydt.DANE_IN());
            //MessageBox.Show(mydt.DANE_OUT());
            mydt.showTrasa(webBrowserMAP);
        }

        // ZAMÓWIENIA I KLIENCI
        private void buttonZamDodaj_Click(object sender, EventArgs e)
        {
            FormZamówienie formularz = new FormZamówienie(ref ListaKlientów);
            var dialogResult = formularz.ShowDialog();
            if (dialogResult == DialogResult.OK)
                ListaZamówień.Add(ctx.Zamówienia.Where(x => x.Id == formularz.zamId).First());
            else if (dialogResult == DialogResult.Cancel)
                return;

        }
        private void buttonZamUsun_Click(object sender, EventArgs e)
        {
            if (ListaZamówień.Count() == 0) return;
            for (int i = dataGridView1.SelectedRows.Count - 1; i >= 0; i--) 
            {
                int curItem = dataGridView1.SelectedRows[i].Index;
                Zamówienie z = ListaZamówień[curItem];
                ListaZamówień.RemoveAt(curItem);
                ctx.Zamówienia.Remove(z);
            }
            ctx.SaveChanges();
        }
        private void buttonZamEdytuj_Click(object sender, EventArgs e)
        {
            /*FormZamówienie formularz = new FormZamówienie(ref ListaKlientów);
            var dialogResult = formularz.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                ListaZamówień.Remove(ctx.Zamówienia.Where(x => x.Id == formularz.zamId).First());
                ListaZamówień.Add(ctx.Zamówienia.Where(x => x.Id == formularz.zamId).First());
            }
            else if (dialogResult == DialogResult.Cancel)
                return;*/
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
            FormKlient formularz = new FormKlient(false);
            var dialogResult = formularz.ShowDialog();
            if (dialogResult == DialogResult.OK)
                ListaKlientów.Add(ctx.Klienci.Where(x => x.Id == formularz.klientId).First());
            else if (dialogResult == DialogResult.Cancel)
                return;
        }
        private void buttonZamUsunKlient_Click(object sender, EventArgs e)
        {
            if (ListaKlientów.Count() == 0) return;
            for (int i = dataGridView2.SelectedRows.Count - 1; i >= 0; i--)
            {
                int curItem = dataGridView2.SelectedRows[i].Index;
                Klient k = ListaKlientów[curItem];
                ListaKlientów.RemoveAt(curItem);
                ctx.Klienci.Remove(k);
            }
            ctx.SaveChanges();
        }
        private void buttonZamEdytujKlient_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 1)
            {
                MessageBox.Show("Zaznacz jednego klienta do edycji");
                return;
            }
            int curItem = dataGridView2.SelectedRows[0].Index;
            FormKlient formularz = new FormKlient(true, ListaKlientów[curItem].Id);
            var dialogResult = formularz.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                ctx = new TransportDbContext();
                ListaKlientów[curItem] = ctx.Klienci.Where(x => x.Id == formularz.klientId).First();
                dataGridView1.Refresh();
            }
            else if (dialogResult == DialogResult.Cancel)
                return;
        }
        // TRASA 
        private void buttonTrasaUsunZam_Click(object sender, EventArgs e)
        {
            if (ListaTrasy.Count() == 0) return;
            ListaTrasy.Remove((Zamówienie)listBox1.SelectedItem);
        }
        private void buttonTrasaDodajZam_Click(object sender, EventArgs e)
        {
            if (ListaZamówień.Count() == 0) return;
            Zamówienie z = (Zamówienie)listBox2.SelectedItem;
            if (!ListaTrasy.Contains(z))
                ListaTrasy.Add(z);
        }
        private void buttonTrasaWyznacz_Click(object sender, EventArgs e)
        {

        }
        private void buttonTrasaUsun_Click(object sender, EventArgs e)
        {

        }


    }
}
