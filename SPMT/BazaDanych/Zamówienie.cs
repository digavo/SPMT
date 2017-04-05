using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMT
{
    public class Zamówienie
    {
        [Key]
        public int Id { get; set; }
        public DateTime DataNadania { get; set; }
        public DateTime? DataDostarczenia { get; set; }
        public int? NadawcaId { get; set; }
        [ForeignKey("NadawcaId")]
        public virtual Klient Nadawca { get; set; }
        public int? OdbiorcaId { get; set; }
        [ForeignKey("OdbiorcaId")]
        public virtual Klient Odbiorca { get; set; }
        public string RodzajPaczki { get; set; }
        public int? WagaPaczki { get; set; } //gramy

        public override string ToString()
        {
            return String.Format("{0}; {1}; {2}; {3}gram", Id, Nadawca.Adres, RodzajPaczki, WagaPaczki);
        }
    }
}
