﻿using System;
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
        public FormKlient()
        {
            klientId = 0;
            InitializeComponent();
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
                ctx.Klienci.Add(klient);
                ctx.SaveChanges();
                klientId = klient.Id;
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
