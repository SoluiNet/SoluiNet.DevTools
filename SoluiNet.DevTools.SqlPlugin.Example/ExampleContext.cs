using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoluiNet.DevTools.SqlPlugin.Example
{
    public partial class ExampleContext : DbContext
    {
        public ExampleContext(string alternativeEnvironment = "")
            : base("name=ExampleConnection" + (string.IsNullOrEmpty(alternativeEnvironment)
                       ? string.Empty
                       : "." + alternativeEnvironment))
        {
        }

        public virtual DbSet<Entities.Customer> Customer { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Customer>()
                .Property(e => e.Email)
                .IsUnicode(false);
        }
    }
}
