namespace JeuDeMemo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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
        [StringLength(200)]
        public string listeCombine { get; set; }

        public virtual Etat Etat { get; set; }

        public virtual Partie Partie { get; set; }

        public virtual Utilisateur Utilisateur { get; set; }
    }
}
