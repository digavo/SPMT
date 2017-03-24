using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMT
{
    public class Adres
    {
        public int Id { get; set; }
        public string Miasto { get; set; }
        public string KodPocztowy { get; set; }
        public string Ulica { get; set; }
        
        public override string ToString()
        {
            return string.Format("Id={0}, Miasto={1}, Kod={2}, Ul.={3}", Id, Miasto, KodPocztowy, Ulica);
        }
    }
}
