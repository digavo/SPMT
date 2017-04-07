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
    public partial class FormKlient : Form
    {
        public int klientId;
        private bool edycja = false;
        public FormKlient(bool ed, int id=0)
        {
            klientId = 0;
            edycja = ed;
            InitializeComponent();
            if (edycja)
            {
                this.Text = "Edycja Klienta";
                button1.Text = "Zapisz";
                //TODO sprawdzić czy poprzedni adres się zmieni/usunie, czy trzeba usunąć ręcznie
                using (var ctx = new TransportDbContext())
                {
                    Klient klient = ctx.Klienci.Where(x => x.Id == id).First();
                    textBox1.Text = klient.Nazwa;
                    textBox2.Text = klient.Adres.Ulica;
                    textBox3.Text = klient.Adres.Miasto;
                    maskedTextBox1.Text = klient.Adres.KodPocztowy;
                    maskedTextBox2.Text = klient.NumerTelefonu;
                    if (klient.Rodzaj == "Firma") checkBox1.Checked = true;
                    klientId = klient.Id;

                    //MessageBox.Show(" " + klient.Id + " " +klient.Nazwa);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Any() || !textBox2.Text.Any() || !textBox3.Text.Any() || !maskedTextBox1.MaskFull || !maskedTextBox2.MaskFull)
            {
                MessageBox.Show("Wypełnij puste pola");
                return;
            }
            Adres adres = new Adres() { Miasto = textBox3.Text, Ulica = textBox2.Text, KodPocztowy = maskedTextBox1.Text };
            Klient klient = new Klient() { Nazwa = textBox1.Text, Adres = adres, NumerTelefonu = maskedTextBox2.Text, Rodzaj = checkBox1.Checked ? "Firma":"Osoba" };
            using (var ctx = new TransportDbContext())
            {
                if (edycja)
                {
                    Klient k = ctx.Klienci.SingleOrDefault(x => x.Id == klientId);
                    if (k != null)
                    {
                        k.Nazwa = textBox1.Text; k.Adres = adres; k.NumerTelefonu = maskedTextBox2.Text; k.Rodzaj = checkBox1.Checked ? "Firma" : "Osoba";
                        ctx.SaveChanges();
                    }
                }
                else
                {
                    ctx.Klienci.Add(klient);
                    klientId = klient.Id;
                    ctx.SaveChanges();
                }
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
