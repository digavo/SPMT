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

namespace SPMT
{
    public partial class KurierForm : Form
    {
        private BindingList<Klient> ListaKlientów = new BindingList<Klient>();
        private BindingList<Zamówienie> ListaZamówień = new BindingList<Zamówienie>();
        private BindingList<Zamówienie> ListaTrasy = new BindingList<Zamówienie>();
        private TransportDbContext ctx = new TransportDbContext();
        private Adres AdresBazy;
        private GA_DaneTrasy mydt;
        public KurierForm()
        {
            InitializeComponent();
            panelTrasa.Dock = DockStyle.Fill;
            panelBaza.Dock = DockStyle.Fill;
            panelZam.BringToFront();
            btnZam.BackColor = Color.Gainsboro;
            panelBaza.BringToFront();
            labelCzas.Text = "";
            labelDługość.Text = "";

            foreach (var k in ctx.Klienci)
                ListaKlientów.Add(k);
            foreach (var z in ctx.Zamówienia)
                ListaZamówień.Add(z);
            AdresBazy = ctx.Adresy.First();
            labelBaza.Text += AdresBazy.ToString();
            listBox1.DataSource = ListaTrasy;
            //listBox2.DataSource = ListaZamówień;
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
        private void btnTrasa_Click(object sender, EventArgs e)
        {
            panelTrasa.BringToFront();
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
            List<Zamówienie> KolejnoscZam = new List<Zamówienie>();
            foreach (int k in Kolejnosc)
            {
                if (k == 0 || k == (Kolejnosc.Count()-1)) continue;
                KolejnoscZam.Add(ListaTrasy[k-1]);
            }
            listBox2.DataSource=KolejnoscZam;

            mydt.calculate_ST(); // to musi byc wywolane po dodaniu adresu bazy na koncu trasy
            labelCzas.Text = mydt.cala_TimeSpan();
            labelDługość.Text = mydt.cala_droga().ToString() + " km";

            mydt.showTrasa(webBrowserMAP);
        }

        private void btnBaza_Click(object sender, EventArgs e)
        {
            panelBaza.BringToFront();
        }

        private void btnZ_Click(object sender, EventArgs e)
        {
            panelZ.BringToFront();
            btnZ.BackColor = Color.Gainsboro;
            btnK.BackColor = Color.White;
            btnZam.BackColor = Color.White;
        }

        private void btnK_Click(object sender, EventArgs e)
        {
            panelK.BringToFront();
            btnK.BackColor = Color.Gainsboro;
            btnZ.BackColor = Color.White;
            btnZam.BackColor = Color.White;
        }

        private void btnZam_Click(object sender, EventArgs e)
        {
            panelZam.BringToFront();
            btnZam.BackColor = Color.Gainsboro;
            btnZ.BackColor = Color.White;
            btnK.BackColor = Color.White;
        }
        
        // ZAMÓWIENIA I KLIENCI
        private void btnK1_Click(object sender, EventArgs e)
        {
            FormKlient formularz = new FormKlient(false);
            var dialogResult = formularz.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                ListaKlientów.Add(ctx.Klienci.Where(x => x.Id == formularz.klientId).First());
                MessageBox.Show(formularz.klientId + " ");
            }
            else if (dialogResult == DialogResult.Cancel)
                return;
        }

        private void btnK2_Click(object sender, EventArgs e)
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

        private void btnK3_Click(object sender, EventArgs e)
        {
            if (ListaKlientów.Count() == 0) return;
            for (int i = dataGridView2.SelectedRows.Count - 1; i >= 0; i--)
            {
                int curItem = dataGridView2.SelectedRows[i].Index;
                Klient k = ListaKlientów[curItem];
                var z = ctx.Zamówienia.Where(x => x.NadawcaId == k.Id || x.OdbiorcaId == k.Id).Count();
                if (z > 0) // klient jest w zamówieniu
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

        private void btnZ1_Click(object sender, EventArgs e)
        {
            FormZamówienie formularz = new FormZamówienie(ref ListaKlientów, false);
            var dialogResult = formularz.ShowDialog();
            if (dialogResult == DialogResult.OK)
                ListaZamówień.Add(ctx.Zamówienia.Where(x => x.Id == formularz.zamId).First());
            else if (dialogResult == DialogResult.Cancel)
                return;
        }

        private void btnZ2_Click(object sender, EventArgs e)
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

        private void btnZ3_Click(object sender, EventArgs e)
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

        private void btnZ4_Click(object sender, EventArgs e)
        {
            if (ListaZamówień.Count() == 0) return;
            int curItem = dataGridView1.SelectedRows[0].Index;
            Zamówienie z = ListaZamówień[curItem];
            if (!ListaTrasy.Contains(z))
                ListaTrasy.Add(z);
        }

        private void btnLZ1_Click(object sender, EventArgs e)
        {
            foreach (var z in ctx.Zamówienia)
                if (!ListaTrasy.Contains(z))
                    ListaTrasy.Add(z);
        }
        private void btnLZ2_Click(object sender, EventArgs e)
        {
            if (ListaTrasy.Count() == 0) return;
            ListaTrasy.Remove((Zamówienie)listBox1.SelectedItem);
        }

    }
}
