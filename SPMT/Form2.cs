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
    public partial class Form2 : Form
    {
        private BindingList<Klient> ListaKlientów = new BindingList<Klient>();
        private BindingList<Zamówienie> ListaZamówień = new BindingList<Zamówienie>();
        private TransportDbContext ctx = new TransportDbContext();
        public Form2()
        {
            InitializeComponent();
            //listBox1.DataSource = ListaZamówień;
            foreach (var k in ctx.Klienci)
                ListaKlientów.Add(k);
            foreach (var z in ctx.Zamówienia)
                ListaZamówień.Add(z);

            dataGridView1.DataSource = ListaKlientów;
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["AdresId"].Visible = false;
            dataGridView2.DataSource = ListaZamówień;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Any() || !textBox2.Text.Any() || !textBox3.Text.Any() || !maskedTextBox1.MaskFull)
            {
                MessageBox.Show("Wypełnij puste pola");
            }
            Adres adres = new Adres() { Miasto = textBox3.Text, Ulica = textBox2.Text, KodPocztowy = maskedTextBox1.Text };
                
            Klient klient = new Klient() { Nazwa = textBox1.Text, Adres = adres };
            ctx.Klienci.Add(klient);
            ctx.SaveChanges();
            ListaKlientów.Add(klient);
            textBox1.Text = textBox2.Text = textBox3.Text = "";
            maskedTextBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ListaKlientów.Count() == 0) return;
            int curItem = listBox1.SelectedIndex;
            Klient klient = ListaKlientów[curItem];
            ListaKlientów.RemoveAt(curItem);
            ctx.Adresy.Remove(klient.Adres);
            ctx.Klienci.Remove(klient);
            ctx.SaveChanges();
        }
    
        private void button3_Click(object sender, EventArgs e)
        {


            textBox4.Text = textBox5.Text = textBox6.Text = "";
            maskedTextBox2.Text = "";
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            tabPage2_Set();
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabControl1.TabPages[1])
                tabPage2_Set();
        }
        private void tabPage2_Set()
        {
            Klient klient = (Klient)dataGridView1.CurrentRow.DataBoundItem;
            textBox4.Text = klient.Nazwa;
            textBox5.Text = klient.Adres.Ulica;
            textBox6.Text = klient.Adres.Miasto;
            maskedTextBox2.Text = klient.Adres.KodPocztowy;
        }
    }
    
}
