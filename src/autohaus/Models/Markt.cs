using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace autohaus.Models;

[Table("Markt")]
public partial class Markt {
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Titel { get; set; } = null!;

    public string? Beschreibung { get; set; }

    [Column(TypeName = "money")]
    public decimal Preis { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Erstellungsdatum { get; set; } = DateTime.Now;

    public bool Verkauft { get; set; }

    [InverseProperty("Markt")]
    public virtual ICollection<Vertrag> Vertrags { get; } = new List<Vertrag>();
}
