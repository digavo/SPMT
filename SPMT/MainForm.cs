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
            
            foreach (var k in ctx.Klienci)
                ListaKlientów.Add(k);
            foreach (var z in ctx.Zamówienia)
                ListaZamówień.Add(z);
             
            dataGridView1.DataSource = ListaKlientów;
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["AdresId"].Visible = false;
            dataGridView2.DataSource = ListaZamówień;


            Form1 instance = new Form1();  // otoz w tej jedenj linijce jest cale oprogramowanie do googleAPI // instance.Form1_Init();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = "Zamówienia";
            tabControlZam.BringToFront();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label2.Text = "Trasa";
            groupTrasa.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label2.Text = "Mapa";
            groupMapa.BringToFront();
        }
        
    }
}
