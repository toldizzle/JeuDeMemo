namespace JeuDeMemo
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Jouer")]
    public partial class Jouer
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idUser { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int idEtat { get; set; }

        public int idPartie { get; set; }

        [Required]
        [StringLength(1000)]
        public string listeCombine { get; set; }

        public virtual Etat Etat { get; set; }

        public virtual Partie Partie { get; set; }

        public virtual Utilisateur Utilisateur { get; set; }
    }
}