namespace JeuDeMemo
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model : DbContext
    {
        public Model()
            : base("name=JeuDB")
        {
        }

        public virtual DbSet<Etat> Etat { get; set; }
        public virtual DbSet<Jouer> Jouer { get; set; }
        public virtual DbSet<Partie> Partie { get; set; }
        public virtual DbSet<Utilisateur> Utilisateur { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Etat>()
                .Property(e => e.nomEtat)
                .IsUnicode(false);

            modelBuilder.Entity<Etat>()
                .HasMany(e => e.Jouer)
                .WithRequired(e => e.Etat)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Jouer>()
                .Property(e => e.listeCombine)
                .IsUnicode(false);

            modelBuilder.Entity<Partie>()
                .Property(e => e.dateHeurePartie)
                .IsUnicode(false);

            modelBuilder.Entity<Partie>()
                .HasMany(e => e.Jouer)
                .WithRequired(e => e.Partie)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Utilisateur>()
                .Property(e => e.nomUser)
                .IsUnicode(false);

            modelBuilder.Entity<Utilisateur>()
                .Property(e => e.prenomUser)
                .IsUnicode(false);

            modelBuilder.Entity<Utilisateur>()
                .HasMany(e => e.Jouer)
                .WithRequired(e => e.Utilisateur)
                .WillCascadeOnDelete(false);
        }
    }
}
