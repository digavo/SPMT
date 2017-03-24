using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMT
{
    public class Klient
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public int AdresyId { get; set; }
        public virtual Adres Adres_ { get; set; }
        public override string ToString()
        {
            return string.Format("Id={0}, Imię={1}, AdresId={2}", Id, Nazwa, Adres_.Id);
        }
    }
}
