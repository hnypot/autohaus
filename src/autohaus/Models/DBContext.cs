using Microsoft.EntityFrameworkCore;

namespace autohaus.Models;

public class DBContext : DbContext {
    public DBContext(DbContextOptions<DBContext> options)
        : base(options) { }
    public DbSet<Benutzer> Benutzer { get; set; }
    public DbSet<Kunden> Kunden { get; set; }
    public DbSet<Markt> Markt { get; set; }
    public DbSet<Vertrag> Vertrag { get; set; }
}