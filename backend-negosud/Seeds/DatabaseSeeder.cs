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
        if (!_context.Utilisateurs.Any())
        {
            var adminRole = _context.Roles.FirstOrDefault(r => r.Nom == "admin");
            if (adminRole == null)
            {
                // Créer le rôle admin s'il n'existe pas encore
                adminRole = new Role { Nom = "admin" };
                _context.Roles.Add(adminRole);
                _context.SaveChanges();
            }

            // Créer l'utilisateur admin avec des informations prédéfinies
            var adminUser = new Utilisateur
            {
                Nom = "Admin",
                Prenom = "System",
                Email = "admin@negosud.com",
                MotDePasse = "$2a$11$wkJKJ2Ijw5c7tKKnpUIGdOOAFdeM3Mgmt3SP3TDV8Fe1oWfmMt9d.",
                RoleId = adminRole.RoleId,
                AccessToken = Guid.NewGuid().ToString()
            };

            _context.Utilisateurs.Add(adminUser);
            _context.SaveChanges();
        }
        
        if (!_context.Clients.Any())
        {
            // table : Client
            var clientFaker = new Faker<Client>("fr")
                .RuleFor(c => c.Nom, f => f.Name.LastName().ToLower())
                .RuleFor(c => c.Prenom, f => f.Name.FirstName().ToLower())
                .RuleFor(c => c.Email, f => f.Internet.Email().ToLower())
                .RuleFor(c => c.Tel, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.MotDePasse, f => f.Internet.Password(20))
                .RuleFor(c => c.EstValide, f => f.Random.Bool())
                .RuleFor(c => c.AcessToken, f => $"{f.Random.AlphaNumeric(10)}.{f.Random.AlphaNumeric(10)}.{f.Random.AlphaNumeric(10)}");

            var clients = clientFaker.Generate(50);
            _context.Clients.AddRange(clients);
            _context.SaveChanges();

            // table : Pays
            var paysFaker = new Faker<Pays>("fr")
                .RuleFor(p => p.Nom, f => f.Address.Country());

            var pays = paysFaker.Generate(50);
            _context.Pays.AddRange(pays);
            _context.SaveChanges();

            // table : Adresse
            var adresseFaker = new Faker<Adresse>("fr")
                .RuleFor(a => a.Numero, f => int.Parse(f.Address.BuildingNumber()))
                .RuleFor(a => a.Ville, f => f.Address.City().ToLower())
                .RuleFor(a => a.CodePostal, f => int.Parse(f.Address.ZipCode()))
                .RuleFor(a => a.Departement, f => f.Address.County().ToLower())
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
                .RuleFor(f => f.Nom, f => f.Company.CompanyName().ToLower())
                .RuleFor(f => f.RaisonSociale, f => f.Company.Nipc())
                .RuleFor(f => f.Email, f => f.Person.Email.ToLower())
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
                .RuleFor(a => a.Libelle, f => f.Commerce.ProductName().ToLower())
                .RuleFor(a => a.Reference, f => f.Commerce.Ean13())
                .RuleFor(a => a.Prix, f => Math.Round(f.Random.Double(5, 150), 2))
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
                .RuleFor(i => i.Libelle, f => f.Lorem.Sentence().ToLower())
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
                .RuleFor(u => u.Nom, f => f.Person.LastName.ToLower())
                .RuleFor(u => u.Prenom, f => f.Person.FirstName.ToLower())
                .RuleFor(u => u.Email, f => f.Person.Email.ToLower())
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
                .RuleFor(i => i.DateModification, f => DateTime.SpecifyKind(f.Date.Past(), DateTimeKind.Utc))
                .RuleFor(i => i.QuantitePostModification, f => f.Random.Int(0, 1000))
                .RuleFor(i => i.QuantitePrecedente, f => f.Random.Int(0, 1000))
                .RuleFor(i => i.TypeModification, f => f.Random.Word().ToLower());

            // génération des entités avec des clés uniques
            var uniqueCombinations = new HashSet<(int, int)>();
            var inventoriers = new List<Inventorier>();

            while (inventoriers.Count < 50)
            {
                var inventorier = inventorierFaker.Generate();
                var key = (inventorier.UtilisateurId, inventorier.StockId);
    
                if (!uniqueCombinations.Contains(key))
                {
                    uniqueCombinations.Add(key);
                    inventoriers.Add(inventorier);
                }
            }

            // vérificaiton si ces entités existent déjà en base de données
            var existingKeys = _context.Inventoriers
                .Select(i => new { i.UtilisateurId, i.StockId })
                .AsNoTracking()
                .ToList();

            foreach (var inventorier in inventoriers)
            {
                var exists = existingKeys.Any(k => k.UtilisateurId == inventorier.UtilisateurId && k.StockId == inventorier.StockId);
    
                if (!exists)
                {
                    _context.Inventoriers.Add(inventorier);
                }
            }

            _context.SaveChanges();

            // table : Livraison
            var livraisonFaker = new Faker<Livraison>("fr")
                .RuleFor(l => l.Livree, f => f.Random.Bool())
                .RuleFor(l => l.DateLivraison, f=> DateTime.SpecifyKind(f.Date.Past(), DateTimeKind.Utc))
                .RuleFor(l => l.DateEstimee, f=> DateTime.SpecifyKind(f.Date.Past(), DateTimeKind.Utc));

            var livraisons = livraisonFaker.Generate(20);
            _context.Livraisons.AddRange(livraisons);
            _context.SaveChanges();

            // table : Commande
            var commandeFaker = new Faker<Commande>("fr")
                .RuleFor(c => c.ClientId, f => f.PickRandom(clients).ClientId)
                .RuleFor(c => c.DateCreation, f=> DateTime.SpecifyKind(f.Date.Past(), DateTimeKind.Utc))
                .RuleFor(c => c.Valide, f => f.Random.Bool())
                .RuleFor(c => c.LivraisonId, f => f.PickRandom(livraisons).LivraisonId);

            var commandes = commandeFaker.Generate(50);
            _context.Commandes.AddRange(commandes);
            _context.SaveChanges();

            // table : BonCommande
            string[] status = { "En attente", "Validée", "En cours de livraison", "Livrée", "Annulée"};
            var bonCommandeFaker = new Faker<BonCommande>()
                .RuleFor(b => b.DateCreation, f => DateTime.SpecifyKind(f.Date.Past(), DateTimeKind.Utc)) 
                .RuleFor(b => b.Prix, f => f.Random.Double())
                .RuleFor(b => b.Reference, f => f.Commerce.Ean13())
                .RuleFor(b => b.UtilisateurId, f => f.PickRandom(utilisateurs).UtilisateurId)
                .RuleFor(b => b.FournisseurId, f => f.PickRandom(fournisseurs).FournisseurId)
                .RuleFor(b => b.Status, f => f.Random.ArrayElement(status));

            var bonCommandes = bonCommandeFaker.Generate(50);
            _context.BonCommandes.AddRange(bonCommandes);
            _context.SaveChanges();

            // table : LigneBonCommande
            var ligneBonCommandeFaker = new Faker<LigneBonCommande>()
                .RuleFor(l => l.ArticleId, f => f.PickRandom(articles).ArticleId)
                .RuleFor(l => l.BonCommandeId, f => f.PickRandom(bonCommandes).BonCommandeId)
                .RuleFor(l => l.PrixUnitaire, f => double.Parse(f.Commerce.Price()))
                .RuleFor(l => l.Quantite, f => f.Random.Int(1, 50))
                .RuleFor(l => l.Livree, f => f.Random.Bool());

            var ligneBonCommandes = ligneBonCommandeFaker.Generate(200);
            _context.LigneBonCommandes.AddRange(ligneBonCommandes);
            _context.SaveChanges();
        }
    }
}
