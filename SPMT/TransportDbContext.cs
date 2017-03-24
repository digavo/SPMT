using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMT
{
    public class TransportDbContext : DbContext
    {
        /// <summary>
        /// Aby podac wlasna nazwe bazy danych, nalezy wywolac konstruktor bazowy z nazwą jako parametrem.
        /// </summary>
        public TransportDbContext() : base("BazaKurier1")
        {
            // Użyj klasy StudiaDbInitializer do zainicjalizowania bazy danych.
            Database.SetInitializer<TransportDbContext>(new TransportDbInitializer());
        }

        public DbSet<Klient> Klienci { get; set; }
        public DbSet<Adres> Adresy { get; set; }

        
    }

}
