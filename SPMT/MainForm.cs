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
            
            //Daga to Kris - jak zamierzasz otwierać okna nie powiązane z tym oknem to rób to w Program.cs
            //Form1 instance = new Form1();  // otoz w tej jedenj linijce jest cale oprogramowanie do googleAPI // instance.Form1_Init();

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
    }
}
