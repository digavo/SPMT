using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using System.Net;
using System.IO;
using System.Xml;

namespace SPMT
{

    public partial class Form1 : Form
    {
       

        public Form1() { InitializeComponent(); Form1_Init();}

        public void Form1_Init()
        {
            //albo w taki sposob
            /* 
            GA_DaneTrasy Miasteczka = new GA_DaneTrasy();
            List<String> listaM = new List<String>();
            listaM.Add("Wrocław");
            listaM.Add("Opole");
            GA_DaneTrasy Miasteczka = new GA_DaneTrasy(listaM); // konstruktor z parametrem
            */
            // albo w taki sposob

            GA_DaneTrasy Miasteczka = new GA_DaneTrasy();
            Miasteczka.ADD_LIST("Wrocław");  // dodajac miast klasa sama tworzac obiekt GA_MIASTO dodaje wspolrzedne geograficzne przy pomocy google api 
            Miasteczka.ADD_LIST("Opole");    // ten sam efekt mozna uzyskac tworzac liste string a potem wywolujac konstruktor z parametrem
            Miasteczka.Dane_googleAPI_read();
            /* Dla Eweliny :)
            for (; Miasteczka.SIZE_LIST();)
            {
                Miasteczka.get_GEOX(i);  //dla miasta i wspolrzedne geograficzne x
                Miasteczka.get_GEOY(i); //dla miasta i wspolrzedne geograficzne y
            }
            */



            string msg1 = Miasteczka.DANE_IN(); // komunikat ktory pozwala w szybki sposob zweryfikowac czy wspolrzedne miasta zostaly wyszukane poprawnie 
            Miasteczka.Dane_googleAPI_read();   //jak juz mamy wszystkie miasta dodane to metoda Dane_googleAPI_read tworzy liste wszystkich polaczen miedzy miastami i dodaje odleglosci miedzy nimi oraz czas przejazdu 
            string msg2 = Miasteczka.DANE_OUT(); // komunikat ktory pozwala w szybki sposob zobaczyc do zrobila metoda  Dane_googleAPI_read()
            MessageBox.Show(msg1.ToString()+ "\n \n "+msg2.ToString()); 
        }
    }
}
