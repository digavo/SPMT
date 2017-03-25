using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMT
{
    public class Klient
    {
        [Key]
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Rodzaj { get; set; } 
        public string NumerTelefonu { get; set; }
        public int? AdresId { get; set; }
        [ForeignKey("AdresId")]
        public virtual Adres Adres { get; set; }
        public override string ToString()
        {
            return string.Format("{0}, {1}", Nazwa, Adres.ToString());
        }
    }
}
