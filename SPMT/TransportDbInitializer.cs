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
            
            Klient klient = new Klient() { Nazwa = "Politechnika Wrocławska", Adres_ = adres};
            
            context.Klienci.Add(klient);
            
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
