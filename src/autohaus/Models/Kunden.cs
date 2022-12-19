using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace autohaus.Models;

[Table("Kunden")]
public partial class Kunden {
    [Key]
    public int Id { get; set; }

    public int BenutzerId { get; set; }

    [StringLength(50)]
    public string Vorname { get; set; } = null!;

    [StringLength(50)]
    public string Nachname { get; set; } = null!;

    [StringLength(75)]
    public string? Email { get; set; }

    [StringLength(16)]
    public string? Telefon { get; set; }

    [StringLength(50)]
    public string? Strasse { get; set; }

    [Column("PLZ")]
    public int? Plz { get; set; }

    [StringLength(30)]
    public string? Ort { get; set; }

    [ForeignKey("BenutzerId")]
    [InverseProperty("Kundens")]
    public virtual Benutzer Benutzer { get; set; } = null!;
}
