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


            foreach (var k in ctx.Klienci)
                ListaKlientów.Add(k);
            foreach (var z in ctx.Zamówienia)
                ListaZamówień.Add(z);

            dataGridView1.DataSource = ListaKlientów;
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["AdresId"].Visible = false;
            dataGridView2.DataSource = ListaZamówień;

            
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
