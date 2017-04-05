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
            Adres adres = new Adres() { Miasto = "Wrocław", KodPocztowy = "50-370", Ulica = "Wybrzeże Wyspiańskiego 27" };
            Klient klient = new Klient() { Nazwa = "Politechnika Wrocławska", NumerTelefonu="111222333", Rodzaj = "Firma", Adres = adres};
            Adres adres2 = new Adres() { Miasto = "Wrocław", KodPocztowy = "50-354", Ulica = "Sienkiewicza 10/3" };
            Klient klient2 = new Klient() { Nazwa = "Agnieszka Kot", NumerTelefonu = "222333444", Rodzaj = "Osoba", Adres = adres2 };
            context.Klienci.Add(klient);
            context.Klienci.Add(klient2);
            context.Zamówienia.Add(new Zamówienie() { DataNadania = DateTime.Today, Nadawca = klient, Odbiorca = klient2, RodzajPaczki = "List polecony" });
            context.Zamówienia.Add(new Zamówienie() { DataNadania = DateTime.Today, Nadawca = klient2, Odbiorca = klient, RodzajPaczki = "Paczka", WagaPaczki=500 });
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
