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
        public FormZamówienie()
        {
            zamId = 0;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!comboBox1.Text.Any() || !comboBox2.Text.Any()) 
            {
                MessageBox.Show("Wypełnij puste pola");
                return;
            }
            if (dateTimePicker1.Value > DateTime.Now || dateTimePicker2.Value < DateTime.Now)
            {
                MessageBox.Show("Termin nadania nie może być późniejszy niż aktualna data\nTermin dostawy nie może być wcześniejszy niż aktualna data");
                return;
            }
            /*Zamówienie zamowienie = new Zamówienie() { };
            using (var ctx = new TransportDbContext())
            {
                ctx.Zamówienia.Add(zamowienie);
                ctx.SaveChanges();
                    zamId = zamowienie.Id;
            }
            */
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
