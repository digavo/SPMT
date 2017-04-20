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
        private Adres AdresBazy;
        private GA_DaneTrasy mydt;
        public MainForm()
        {
            InitializeComponent();
            webBrowserMAP.Dock = DockStyle.Fill;
            panelEmpty.Dock = DockStyle.Fill;
            tabControlZam.Dock = DockStyle.Fill;
            groupTrasa.Dock = DockStyle.Fill;
            groupMapa.Dock = DockStyle.Fill;
            panelEmpty.BringToFront();
            labelCzas.Text = "";
            labelDługość.Text = "";

            foreach (var k in ctx.Klienci)
                ListaKlientów.Add(k);
            foreach (var z in ctx.Zamówienia)
                ListaZamówień.Add(z);
            AdresBazy = ctx.Adresy.First();
            buttonBaza.Text += AdresBazy.ToString();
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
            // to poniżej wykonuje się w zakładce Traza -> wyznacz trasę, następnie wyznaczona trasa jest widoczna na mapie

            /*groupMapa.BringToFront();
            if (!ListaTrasy.Any())
            {
                MessageBox.Show("Lista zamówień na trasie jest pusta");
                return;
            }
            GA_DaneTrasy mydt = new GA_DaneTrasy();
            mydt.ADD_LIST(AdresBazy.ToString());
            richTextBox2.Text += "Baza: "+AdresBazy.ToString() + "\n";
            foreach (var z in ListaTrasy)
            {
                mydt.ADD_LIST(z.Odbiorca.Adres.ToString());
                richTextBox2.Text += z.Odbiorca.Adres.ToString() + "\n";
            }
            mydt.Dane_googleAPI_read(); // to musi byc wywolane dokladnie po ostatnim adresie, lecz przed dodaniem adresu bazy na koncu trasy
            richTextBox2.Text += "Baza: " + AdresBazy.ToString() + "\n";
            mydt.ADD_LIST(AdresBazy.ToString());
            mydt.calculate_ST(); // to musi byc wywolane  po dodaniu adresu bazy na koncu trasy*/

            if (mydt !=null)
                mydt.showTrasa(webBrowserMAP);
        }

        // ZAMÓWIENIA I KLIENCI
        private void buttonZamDodaj_Click(object sender, EventArgs e)
        {
            FormZamówienie formularz = new FormZamówienie(ref ListaKlientów, false);
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
            if (dataGridView1.SelectedRows.Count > 1)
            {
                MessageBox.Show("Zaznacz jedno zamówienie do edycji");
                return;
            }
            int curItem = dataGridView1.SelectedRows[0].Index;
            FormZamówienie formularz = new FormZamówienie(ref ListaKlientów, true, ListaZamówień[curItem].Id);
            
            var dialogResult = formularz.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                ctx = new TransportDbContext();
                ListaZamówień[curItem] = ctx.Zamówienia.Where(x => x.Id == formularz.zamId).First();
                dataGridView1.Refresh();
            }
            else if (dialogResult == DialogResult.Cancel)
                return;
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
            { ListaKlientów.Add(ctx.Klienci.Where(x => x.Id == formularz.klientId).First());
                MessageBox.Show(formularz.klientId + " ");
            }
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
                var z = ctx.Zamówienia.Where(x => x.NadawcaId == k.Id || x.OdbiorcaId == k.Id).Count();
                if (z>0) // klient jest w zamówieniu
                {
                    MessageBox.Show(k.Nazwa + ": Nie można usunąć klienta, który jest odbiorcą lub nadawcą zamówienia");
                }
                else
                {
                    ListaKlientów.RemoveAt(curItem);
                    ctx.Klienci.Remove(k);
                }
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
                dataGridView2.Refresh();
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
            if (!ListaTrasy.Any())
            {
                MessageBox.Show("Lista zamówień na trasie jest pusta");
                return;
            }

            mydt = new GA_DaneTrasy();
            mydt.ADD_LIST(AdresBazy.ToString());
            foreach (var z in ListaTrasy)
            {
                mydt.ADD_LIST(z.Odbiorca.Adres.ToString());
            }
            
            mydt.Dane_googleAPI_read(); // to musi byc wywolane dokladnie po ostatnim adresie, lecz przed dodaniem adresu bazy na koncu trasy
            mydt.ADD_LIST(AdresBazy.ToString());


            double[,] tab = new double[mydt.SIZE_LIST(), mydt.SIZE_LIST()];//
            for (int i = 0; i < mydt.SIZE_LIST(); i++)
            {
                for (int j = 0; j < mydt.SIZE_LIST(); j++)
                {
                    if (i == j) { tab[i, j] = 0; }
                    else { tab[i, j] = mydt.getS(mydt.get_TowN(i), mydt.get_TowN(j)); }

                }
            }
            Wyzazanie W = new Wyzazanie(tab, mydt.SIZE_LIST());
            List<int> Kolejnosc = W.Sym_Wyz();
            mydt.get_list_form_salesman(Kolejnosc);


            mydt.calculate_ST(); // to musi byc wywolane  po dodaniu adresu bazy na koncu trasy
            labelCzas.Text = mydt.cala_TimeSpan();
            labelDługość.Text = mydt.cala_droga().ToString() + " km";
        }
        private void buttonTrasaUsun_Click(object sender, EventArgs e)
        {

        }

        private void buttonBaza_Click(object sender, EventArgs e)
        {
        }
    }
}
