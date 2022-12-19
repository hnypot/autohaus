using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace autohaus.Models;

[Table("Vertrag")]
public partial class Vertrag {
    [Key]
    public int Vertragnummer { get; set; }

    public int MarktId { get; set; }

    public int BenutzerId { get; set; }

    public string? Beschreibung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Erstellungsdatum { get; set; } = DateTime.Now;

    public bool Gueltig { get; set; }

    [ForeignKey("BenutzerId")]
    [InverseProperty("Vertrags")]
    public virtual Benutzer Benutzer { get; set; } = null!;

    [ForeignKey("MarktId")]
    [InverseProperty("Vertrags")]
    public virtual Markt Markt { get; set; } = null!;
}