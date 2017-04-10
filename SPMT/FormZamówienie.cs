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
    public partial class FormZamówienie : Form
    {
        public int zamId;
        private bool edycja = false;
        public FormZamówienie(ref BindingList<Klient> klienci, bool ed, int id = 0)
        {
            zamId = 0;
            edycja = ed;
            InitializeComponent();
            comboBox1.BindingContext = new BindingContext();
            comboBox1.DataSource = klienci;
            comboBox2.BindingContext = new BindingContext();
            comboBox2.DataSource = klienci;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0; 
            dateTimePicker2.Value = dateTimePicker1.Value.AddHours((int)numericUpDown1.Value);
            if (edycja)
            {
                this.Text = "Edycja Zamówienia";
                button1.Text = "Zapisz";
                using (var ctx = new TransportDbContext())
                {
                    Zamówienie zam = ctx.Zamówienia.Where(x => x.Id == id).First();
                    comboBox1.SelectedItem = klienci.Single(x => x.Id == zam.NadawcaId);
                    comboBox2.SelectedItem = klienci.Single(x => x.Id == zam.OdbiorcaId);
                    dateTimePicker1.Value = zam.DataNadania;
                    numericUpDown1.Value = zam.CzasDostarczenia;
                    comboBox3.SelectedItem = zam.RodzajPaczki;
                    numericUpDown2.Value = zam.WagaPaczki!=null? (decimal)zam.WagaPaczki: 0;
                    dateTimePicker2.Value = dateTimePicker1.Value.AddHours((int)numericUpDown1.Value);
                    zamId = zam.Id;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!comboBox1.Text.Any() || !comboBox2.Text.Any()) 
            {
                MessageBox.Show("Wypełnij puste pola");
                return;
            }
            if (dateTimePicker1.Value > DateTime.Now)
            {
                MessageBox.Show("Termin nadania nie może być późniejszy niż aktualna data");
                return;
            }
            if (comboBox1.SelectedIndex==comboBox2.SelectedIndex)
            {
                MessageBox.Show("Odbioraca i nadawca powinni się różnić");
                return;
            }
            
            using (var ctx = new TransportDbContext())
            {
                if (edycja)
                {
                    Zamówienie z = ctx.Zamówienia.Single(x => x.Id == zamId);
                    if (z != null)
                    {
                        z.NadawcaId = ((Klient)comboBox1.SelectedItem).Id;
                        z.OdbiorcaId = ((Klient)comboBox2.SelectedItem).Id;
                        z.DataNadania = dateTimePicker1.Value;
                        z.CzasDostarczenia = (int)numericUpDown1.Value;
                        z.RodzajPaczki = comboBox3.Text;
                        z.WagaPaczki = (int)numericUpDown2.Value;
                        ctx.SaveChanges();
                    }
                }
                else
                {
                    Zamówienie zamowienie = new Zamówienie()
                    {
                        NadawcaId = ((Klient)comboBox1.SelectedItem).Id,
                        OdbiorcaId = ((Klient)comboBox2.SelectedItem).Id,
                        DataNadania = dateTimePicker1.Value,
                        CzasDostarczenia = (int)numericUpDown1.Value,
                        RodzajPaczki = comboBox3.Text,
                        WagaPaczki = (int)numericUpDown2.Value
                    };
                    ctx.Zamówienia.Add(zamowienie);
                    zamId = zamowienie.Id;
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

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Value = dateTimePicker1.Value.AddHours((int)numericUpDown1.Value);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Value = dateTimePicker1.Value.AddHours((int)numericUpDown1.Value);
        }
    }
}
