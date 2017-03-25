using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SPMT
{
    public class Adres
    {
        [Key]
        public int Id { get; set; }
        public string Miasto { get; set; }
        public string KodPocztowy { get; set; }
        public string Ulica { get; set; }
        
        public override string ToString()
        {
            return string.Format("{0}, {1}", Miasto, Ulica);
        }
    }
}
