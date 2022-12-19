using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace autohaus.Models;

[Table("Benutzer")]
public partial class Benutzer {
    [Key]
    public int Id { get; set; }

    [StringLength(30)]
    public string Benutzername { get; set; } = null!;

    [StringLength(64)]
    [Unicode(false)]
    public string Passwort { get; set; } = null!;

    public bool Admin { get; set; }

    [InverseProperty("Benutzer")]
    public virtual ICollection<Kunden> Kundens { get; } = new List<Kunden>();

    [InverseProperty("Benutzer")]
    public virtual ICollection<Vertrag> Vertrags { get; } = new List<Vertrag>();
}