using backend_negosud.Entities;
using Bogus;
using Bogus.Extensions.Portugal;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Supabase.Postgrest.Attributes;

namespace backend_negosud.Seeds;

public class DatabaseSeeder
{
    private readonly PostgresContext _context;

    public DatabaseSeeder(PostgresContext context)
    {
        _context = context;
    }

    public void SeedDatabase()
    {
        if (!_context.Clients.Any())
        {
            // table : Client
            var clientFaker = new Faker<Client>("fr")
                .RuleFor(c => c.Nom, f => f.Name.LastName())
                .RuleFor(c => c.Prenom, f => f.Name.FirstName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Tel, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.MotDePasse, f => f.Internet.Password(20))
                .RuleFor(c => c.EstValide, f => f.Random.Bool())
                .RuleFor(c => c.AcessToken, f => $"{f.Random.AlphaNumeric(10)}.{f.Random.AlphaNumeric(10)}.{f.Random.AlphaNumeric(10)}");

            var clients = clientFaker.Generate(50);
            _context.Clients.AddRange(clients);
            _context.SaveChanges();

            // table :Pays
            var paysFaker = new Faker<Pays>("fr")
                .RuleFor(p => p.Nom, f => f.Address.Country());

            var pays = paysFaker.Generate(50);
            _context.Pays.AddRange(pays);
            _context.SaveChanges();

                //table : Adresse
            var adresseFaker = new Faker<Adresse>("fr")
                .RuleFor(a => a.Numero, f => int.Parse(f.Address.BuildingNumber()))
                .RuleFor(a => a.Ville, f => f.Address.City())
                .RuleFor(a => a.CodePostal, f => int.Parse(f.Address.ZipCode()))
                .RuleFor(a => a.Departement, f => f.Address.County())
                .RuleFor(a => a.PaysId, f => f.PickRandom(pays).PaysId)
                .RuleFor(a => a.ClientId, f => f.PickRandom(clients).ClientId);
            
            var adresses = adresseFaker.Generate(100);
            _context.Adresses.AddRange(adresses);
            _context.SaveChanges();
            
            // table : Famille
            var familles = new List<Famille>
            {
                new Famille { Nom = "Rosé" },
                new Famille { Nom = "Blanc" },
                new Famille { Nom = "Rouge" },
            };
            
            _context.Familles.AddRange(familles);
            _context.SaveChanges();

                // table : Fournisseur
                var fournisseurFaker = new Faker<Fournisseur>("fr")
                    .RuleFor(f => f.Nom, f => f.Company.CompanyName())
                    .RuleFor(f => f.RaisonSociale, f => f.Company.Nipc())
                    .RuleFor(f => f.Email, f => f.Person.Email)
                    .RuleFor(f => f.Tel, f => f.Person.Phone);

                var fournisseurs = fournisseurFaker.Generate(25);
                _context.Fournisseurs.AddRange(fournisseurs);
                _context.SaveChanges();
                
                    // table : TVA
                var tva = new List<Tva>
                {
                    new Tva { Valeur = 20.0 },
                    new Tva { Valeur = 10.0 },
                    new Tva { Valeur = 5.5 },
                    new Tva { Valeur = 2.1 }
                };
                
                _context.Tvas.AddRange(tva);
                _context.SaveChanges();
            
              // table : Article
              var articleFaker = new Faker<Article>("fr")
                  .RuleFor(a => a.Libelle, f => f.Commerce.ProductName())
                  .RuleFor(a => a.Reference, f => f.Commerce.Ean13())
                  .RuleFor(a => a.Prix, f => double.Parse(f.Commerce.Price()))
                  .RuleFor(a => a.FamilleId, f => f.PickRandom(familles).FamilleId)
                  .RuleFor(a => a.FournisseurId, f => f.PickRandom(fournisseurs).FournisseurId)
                  .RuleFor(a => a.TvaId, f => f.PickRandom(tva).TvaId);
            
            var articles = articleFaker.Generate(30);
            _context.Articles.AddRange(articles);
            _context.SaveChanges();
            
            
                // table : Image
            var imageFaker = new Faker<Image>("fr")
                .RuleFor(i => i.ArticleId, f => f.PickRandom(articles).ArticleId)
                .RuleFor(i => i.Format, f => f.Random.AlphaNumeric(10))
                .RuleFor(i => i.Libelle, f => f.Lorem.Sentence())
                .RuleFor(i => i.Slug, f => f.Lorem.Slug());
            
            var images = imageFaker.Generate(25);
            _context.Images.AddRange(images);
            _context.SaveChanges();
            
            
                // table : Stock
            var stockFaker = new Faker<Stock>("fr")
                .RuleFor(s => s.ArticleId, f => f.PickRandom(articles).ArticleId)
                .RuleFor(s => s.Quantite, f => f.Random.Int(0, 1000))
                .RuleFor(s => s.RefLot, f => f.Random.AlphaNumeric(10))
                .RuleFor(s => s.SeuilMinimum, f => f.Random.Int(0, 50))
                .RuleFor(s => s.ReapprovisionnementAuto, f => f.Random.Bool());
            
            var stocks = stockFaker.Generate(100);
            _context.Stocks.AddRange(stocks);
            _context.SaveChanges();
            
                // table : Role
                var roles = new List<Role>
                {
                    new Role { Nom = "admin"},
                    new Role { Nom = "employe"}
                };
                
                _context.Roles.AddRange(roles);
                _context.SaveChanges();
            
                // table : Utilisateur
            var utilisateurFaker = new Faker<Utilisateur>("fr")
                .RuleFor(u => u.Nom, f => f.Person.LastName)
                .RuleFor(u => u.Prenom, f => f.Person.FirstName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.RoleId, f => f.PickRandom(roles).RoleId)
                .RuleFor(u => u.MotDePasse, f => f.Internet.Password(15))
                .RuleFor(u => u.AccessToken, f => $"{f.Random.AlphaNumeric(10)}.{f.Random.AlphaNumeric(10)}.{f.Random.AlphaNumeric(10)}");

            var utilisateurs = utilisateurFaker.Generate(20);
            _context.Utilisateurs.AddRange(utilisateurs);
            _context.SaveChanges();
            

            // table : Inventorier
            var inventorierFaker = new Faker<Inventorier>("fr")
                .RuleFor(i => i.UtilisateurId, f => f.PickRandom(utilisateurs).UtilisateurId)
                .RuleFor(i => i.StockId, f => f.PickRandom(stocks).StockId)
                .RuleFor(i => i.DateModification, f => f.Date.Past())
                .RuleFor(i => i.QuantitePostModification, f => f.Random.Int(0, 1000))
                .RuleFor(i => i.QuantitePrecedente, f => f.Random.Int(0, 1000))
                .RuleFor(i => i.TypeModification, f => f.Random.Word());
            
            var inventoriers = inventorierFaker.Generate(50);

            foreach (var inventorier in inventoriers)
            {
                // vérifie si l'entité existe déjà dans la base
                var existingInventorier = _context.Inventoriers
                    .AsNoTracking() // on désactive le suivi pour éviter les conflits
                    .SingleOrDefault(i => i.UtilisateurId == inventorier.UtilisateurId && i.StockId == inventorier.StockId);

                if (existingInventorier == null)
                {
                    _context.Inventoriers.Add(inventorier);
                }
                else
                {
                    inventorier.UtilisateurId = existingInventorier.UtilisateurId;
                    _context.Inventoriers.Update(inventorier);
                }
            }

// Sauvegarder les modifications dans la base
            _context.SaveChanges();

            
                // table : Livraison
            var livraisonFaker = new Faker<Livraison>("fr")
                .RuleFor(l => l.Livree, f => f.Random.Bool())
                .RuleFor(l => l.DateLivraison, f => f.Date.Past())
                .RuleFor(l => l.DateEstimee, f => f.Date.Past());

            var livraisons = livraisonFaker.Generate(20);
            _context.Livraisons.AddRange(livraisons);
            _context.SaveChanges();
            
            
                // table : Commande
                var commandeFaker = new Faker<Commande>("fr")
                    .RuleFor(c => c.ClientId, f => f.PickRandom(clients).ClientId)
                    .RuleFor(c => c.DateCreation, f => f.Date.Past())
                    .RuleFor(c => c.Valide, f => f.Random.Bool())
                    .RuleFor(c => c.LivraisonId, f => f.PickRandom(livraisons).LivraisonId);

            var commandes = commandeFaker.Generate(50);
            _context.Commandes.AddRange(commandes);
            _context.SaveChanges();
            
                // table : BonCommande
            string[] status = { "en cours", "valide", "livree", "termine"};
            var bonCommandeFaker = new Faker<BonCommande>()
                .RuleFor(b => b.Prix, f => f.Random.Double())
                .RuleFor(b => b.UtilisateurId, f => f.PickRandom(utilisateurs).UtilisateurId)
                .RuleFor(b => b.Status, f => f.Random.ArrayElement(status))
                .RuleFor(b => b.Reference, f => f.Commerce.Ean13());

            var bonCommandes = bonCommandeFaker.Generate(50);
            _context.BonCommandes.AddRange(bonCommandes);
            _context.SaveChanges();
            
            
                
                // table : LigneBonCommande
            // var ligneBonCommandeFaker = new Faker<LigneBonCommande>()
            //     .RuleFor(l => l.ArticleId, f => f.PickRandom(articles).ArticleId)
            //     .RuleFor(l => l.BonCommandeId, f => f.PickRandom(bonCommandes).BonCommandeId) 
            //     .RuleFor(l => l.LigneLivraisonId, f => f.Random.Int(1, 20))
            //     .RuleFor(l => l.PrixUnitaire, f => double.Parse(f.Commerce.Price()))
            //     .RuleFor(l => l.Quantite, f => f.Random.Int(1, 50));
            //
            // var ligneBonCommandes = ligneBonCommandeFaker.Generate(200);
            // _context.LigneBonCommandes.AddRange(ligneBonCommandes);
            // _context.SaveChanges();

        }
    }
}