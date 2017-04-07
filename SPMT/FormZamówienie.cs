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
        public FormZamówienie(ref BindingList<Klient> klienci)
        {
            zamId = 0;
            InitializeComponent();
            comboBox1.DataSource = klienci;
            comboBox2.BindingContext = new BindingContext();
            comboBox2.DataSource = klienci;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            dateTimePicker2.Value = dateTimePicker1.Value.AddHours((int)numericUpDown1.Value);
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
            Zamówienie zamowienie = new Zamówienie() {
                NadawcaId = ((Klient)comboBox1.SelectedItem).Id, OdbiorcaId = ((Klient)comboBox2.SelectedItem).Id,
                DataNadania =dateTimePicker1.Value, CzasDostarczenia=(int)numericUpDown1.Value,
                RodzajPaczki =comboBox3.Text, WagaPaczki=(int)numericUpDown2.Value };
            using (var ctx = new TransportDbContext())
            {
                ctx.Zamówienia.Add(zamowienie);
                ctx.SaveChanges();
                zamId = zamowienie.Id;
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
