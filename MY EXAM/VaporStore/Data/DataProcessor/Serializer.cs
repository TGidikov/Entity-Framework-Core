namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {


            var games = context
                .Genres
                .ToArray()
                .Where(x => genreNames.Contains(x.Name) == true)
                .Select(x => new
                {
                    Id = x.Id,
                    Genre = x.Name,
                    Games = x.Games
                        .Where(g => g.Purchases.Count > 0)
                        .Select(g => new
                        {
                            Id = g.Id,
                            Title = g.Name,
                            Developer = g.Developer.Name,
                            Tags = string.Join(", ", g.GameTags.Select(gt => gt.Tag.Name)),
                            Players = g.Purchases.Count()
                        })
                        .OrderByDescending(g => g.Players)
                        .ThenBy(g => g.Id)
                        .ToList(),
                    TotalPlayers = x.Games.SelectMany(g => g.Purchases).Count(),
                })
                .OrderByDescending(x => x.TotalPlayers)
                .ThenBy(x => x.Id)
                .ToList();



            var json = JsonConvert.SerializeObject(games, Formatting.Indented);
            return json;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			var sb = new StringBuilder();
			var serializer = new XmlSerializer(typeof(List<ExportUserPurchasesDto>), new XmlRootAttribute("Users"));
			var xmlNamespaces = new XmlSerializerNamespaces();
			xmlNamespaces.Add("", "");
			var result = context.Users
				.Include(u => u.Cards)
				.ThenInclude(c => c.Purchases)
				.ThenInclude(p => p.Game)
				.ThenInclude(g => g.Genre)
				.ToList()
				.Where(u => u.Cards.SelectMany(c => c.Purchases).Count() > 0)
				.Select(u => new ExportUserPurchasesDto
				{
					Username = u.Username,
					Purchases = u.Cards.SelectMany(c => c.Purchases)
						.Where(p => p.Type.ToString() == storeType)
						.Select(p => new PurchasesDto
						{
							Card = p.Card.Number,
							Cvc = p.Card.Cvc,
							Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
							Game = new GameDto
							{
								Title = p.Game.Name,
								Genre = p.Game.Genre.Name,
								Price = p.Game.Price
							}
						})
						.OrderBy(p => DateTime.Parse(p.Date)).ToList(),
					TotalMoneySpent = u.Cards.SelectMany(c => c.Purchases)
						.Where(p => p.Type.ToString() == storeType).Sum(p => p.Game.Price)
				})
				.Where(u => u.Purchases.Count > 0)
				.OrderByDescending(u => u.TotalMoneySpent)
				.ThenBy(u => u.Username)
				.ToList();

			serializer.Serialize(new StringWriter(sb), result, xmlNamespaces);
			return sb.ToString();
		}
	}
}