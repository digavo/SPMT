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
        public Form2()
        {
            InitializeComponent();
            groupBox1.Visible = false;
            WypiszBaze();
        }

        private void WypiszBaze ()
        {
            richTextBox1.Text = "";
            using (var ctx = new TransportDbContext())
            {
                foreach (var k in ctx.Klienci)
                {
                    richTextBox1.Text += k.ToString() + "\n";
                }
                richTextBox1.Text += " -------------- \n";

                foreach (var a in ctx.Adresy)
                {
                    richTextBox1.Text += a.ToString() + "\n";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            maskedTextBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Any() || !textBox2.Text.Any() || !textBox3.Text.Any() || !maskedTextBox1.MaskFull)
            {
                MessageBox.Show("Wypełnij puste pola");
            }
            using (var ctx = new TransportDbContext())
            {
                Adres adres = new Adres() { Miasto = textBox3.Text, Ulica = textBox2.Text, KodPocztowy = maskedTextBox1.Text };
                
                Klient klient = new Klient() { Nazwa = textBox1.Text, Adres_ = adres };
                ctx.Klienci.Add(klient);
                ctx.SaveChanges();
            }
            WypiszBaze();
        }
    }
    
}
