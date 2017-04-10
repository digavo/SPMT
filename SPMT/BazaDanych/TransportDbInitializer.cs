using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMT
{
    class TransportDbInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<TransportDbContext>
    {
        /// <summary>
        /// Metoda wwoływana po pierwszym użyciu odniesienia do bazy, 
        /// inicjuje bazę początkowymi danymi
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(TransportDbContext context)
        {
            Adres baza = new Adres() { Id = 1, Miasto = "Wrocław", KodPocztowy = "50-370", Ulica = "Legnicka 70" };
            context.Adresy.Add(baza);
            context.SaveChanges();
            Adres adres = new Adres() { Miasto = "Wrocław", KodPocztowy = "50-370", Ulica = "Wybrzeże Wyspiańskiego 27" };
            Klient klient = new Klient() { Nazwa = "Politechnika Wrocławska", NumerTelefonu="111222333", Rodzaj = "Firma", Adres = adres};
            Adres adres2 = new Adres() { Miasto = "Wrocław", KodPocztowy = "50-354", Ulica = "Sienkiewicza 10/3" };
            Klient klient2 = new Klient() { Nazwa = "Agnieszka Kot", NumerTelefonu = "222333444", Rodzaj = "Osoba", Adres = adres2 };
            Adres adres3 = new Adres() { Miasto = "Wrocław", KodPocztowy = "51-311", Ulica = "Mydlana 10" };
            Klient klient3 = new Klient() { Nazwa = "Łucja Zalewska", NumerTelefonu = "211111111", Rodzaj = "Osoba", Adres = adres3 };
            context.Klienci.Add(klient);
            context.Klienci.Add(klient2);
            context.Klienci.Add(klient3);
            context.Zamówienia.Add(new Zamówienie() { DataNadania = DateTime.Now, Nadawca = klient, Odbiorca = klient3, RodzajPaczki = "List polecony", CzasDostarczenia=72 });
            context.Zamówienia.Add(new Zamówienie() { DataNadania = DateTime.Now, Nadawca = klient2, Odbiorca = klient, RodzajPaczki = "Paczka", WagaPaczki=500, CzasDostarczenia=24 });
            context.Zamówienia.Add(new Zamówienie() { DataNadania = DateTime.Now, Nadawca = klient3, Odbiorca = klient2, RodzajPaczki = "Paczka", WagaPaczki = 50, CzasDostarczenia = 30 });
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
