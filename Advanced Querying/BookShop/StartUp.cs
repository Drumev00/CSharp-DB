namespace BookShop
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using Data;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
				//DbInitializer.ResetDatabase(db);
				Console.WriteLine(RemoveBooks(db));
            }
        }
		// Problem 01
		public static string GetBooksByAgeRestriction(BookShopContext context, string command)
		{
			StringBuilder sb = new StringBuilder();

			var books = context
				.Books
				.OrderBy(b => b.Title)
				.Where(b => b.AgeRestriction.ToString().ToLower() ==
						command.ToLower())
				.Select(b => b.Title)
				.ToList();

			foreach (var book in books)
			{
				sb.AppendLine(book);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 02
		public static string GetGoldenBooks(BookShopContext context)
		{
			StringBuilder sb = new StringBuilder();

			var books = context
				.Books
				.OrderBy(b => b.BookId)
				.Where(b => b.EditionType.ToString() == "Gold" &&
						b.Copies < 5000)
				.Select(b => b.Title)
				.ToList();

			foreach (var book in books)
			{
				sb.AppendLine(book);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 03
		public static string GetBooksByPrice(BookShopContext context)
		{
			StringBuilder sb = new StringBuilder();

			var books = context
				.Books
				.OrderByDescending(b => b.Price)
				.Where(b => b.Price > 40)
				.Select(b => new
				{
					b.Title,
					b.Price
				})
				.ToList();

			foreach (var book in books)
			{
				sb.AppendLine($"{book.Title} - ${book.Price:f2}");
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 04
		public static string GetBooksNotReleasedIn(BookShopContext context, int year)
		{
			StringBuilder sb = new StringBuilder();

			var books = context
				.Books
				.OrderBy(b => b.BookId)
				.Where(b => b.ReleaseDate.Value.Year != year)
				.Select(b => b.Title)
				.ToList();

			foreach (var book in books)
			{
				sb.AppendLine(book);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 05
		public static string GetBooksByCategory(BookShopContext context, string input)
		{
			StringBuilder sb = new StringBuilder();

			string[] categories = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

			List<string> titles = new List<string>();

			foreach (var category in categories)
			{
				var books = context.Books
					.Where(b => b.BookCategories
					.Any(bc => bc.Category.Name.ToLower() == category.ToLower()))
				.ToList();

				foreach (var book in books)
				{
					titles.Add(book.Title);
				}
			}

			foreach (var bookTitle in titles.OrderBy(x => x))
			{
				sb.AppendLine(bookTitle);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 06
		public static string GetBooksReleasedBefore(BookShopContext context, string date)
		{
			StringBuilder sb = new StringBuilder();

			string[] dateComponents = date.Split("-", StringSplitOptions.RemoveEmptyEntries);

			DateTime dateToCompare = new DateTime(int.Parse(dateComponents[2]), int.Parse(dateComponents[1]), int.Parse(dateComponents[0]));

			var books = context
				.Books
				.OrderByDescending(b => b.ReleaseDate)
				.Where(b => b.ReleaseDate.Value != null && b.ReleaseDate.Value < dateToCompare)
				.Select(b => new
				{
					b.Title,
					EditionType = b.EditionType.ToString(),
					b.Price
				})
				.ToList();

			foreach (var book in books)
			{
				sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 07
		public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
		{
			var authors = context.Authors
				.OrderBy(a => a.FirstName)
				.ThenBy(a => a.LastName)
				.Where(a => a.FirstName.EndsWith(input))
				.Select(a => new
				{
					FullName = a.FirstName + " " + a.LastName
				})
				.ToList();

			StringBuilder sb = new StringBuilder();

			foreach (var author in authors)
			{
				sb.AppendLine(author.FullName);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 08
		public static string GetBookTitlesContaining(BookShopContext context, string input)
		{
			var books = context
				.Books
				.OrderBy(b => b.Title)
				.Where(b => b.Title.ToLower().Contains(input.ToLower()))
				.Select(b => b.Title)
				.ToList();

			StringBuilder sb = new StringBuilder();

			foreach (var book in books)
			{
				sb.AppendLine(book);
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 09
		public static string GetBooksByAuthor(BookShopContext context, string input)
		{
			var booksAndAuthors = context
				.Books
				.OrderBy(b => b.BookId)
				.Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
				.Select(b => new
				{
					b.Title,
					AuthorName = b.Author.FirstName + " " + b.Author.LastName,

				})
				.ToList();

			StringBuilder sb = new StringBuilder();

			foreach (var baa in booksAndAuthors)
			{
				sb.AppendLine($"{baa.Title} ({baa.AuthorName})");
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 10
		public static int CountBooks(BookShopContext context, int lengthCheck)
		{
			var bookCount = context
				.Books
				.Where(b => b.Title.Length > lengthCheck)
				.ToList();

			return bookCount.Count;
		}
		//Problem 11
		public static string CountCopiesByAuthor(BookShopContext context)
		{
			var authors = context
				.Authors
				.Select(a => new
				{
					FullName = a.FirstName + " " + a.LastName,
					CopiesCount = a.Books
							.Sum(b => b.Copies)
				})
				.ToList();

			StringBuilder sb = new StringBuilder();

			foreach (var author in authors.OrderByDescending(a => a.CopiesCount))
			{
				sb.AppendLine($"{author.FullName} - {author.CopiesCount}");
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 12
		public static string GetTotalProfitByCategory(BookShopContext context)
		{
			var profitForCategories = context
				.Categories
				.Select(c => new
				{
					c.Name,
					Category = c.CategoryBooks
							.Select(cb => new
							{
								cb.Book.Copies,
								cb.Book.Price
							})
				})
				.ToList();

			StringBuilder sb = new StringBuilder();

			foreach (var pfc in profitForCategories.OrderByDescending(x => x.Category.Sum(x => x.Price * x.Copies))
																			.ThenBy(c => c.Name))
			{
				sb.AppendLine($"{pfc.Name} ${pfc.Category.Sum(x => x.Price * x.Copies)}");
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 13
		public static string GetMostRecentBooks(BookShopContext context)
		{
			var recentBooks = context
				.Categories
				.OrderBy(c => c.Name)
				.Select(c => new
				{
					c.Name,
					Books = c.CategoryBooks.Select(cb => new
					{
						cb.Book.Title,
						cb.Book.ReleaseDate
					})
					.OrderByDescending(cb => cb.ReleaseDate)
					.Take(3)
				})
				.ToList();

			StringBuilder sb = new StringBuilder();

			foreach (var category in recentBooks)
			{
				sb.AppendLine($"--{category.Name}");
				foreach (var book in category.Books)
				{
					sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
				}
			}

			return sb.ToString().TrimEnd();
		}
		//Problem 14
		public static void IncreasePrices(BookShopContext context)
		{
			var books = context
				.Books
				.Where(b => b.ReleaseDate.Value.Year < 2010)
				.ToList();

			foreach (var book in books)
			{
				book.Price += 5;
			}
		}
		//Problem 15
		public static int RemoveBooks(BookShopContext context)
		{
			var books = context
				.Books
				.Where(b => b.Copies < 4200)
				.ToList();

			context.RemoveRange(books);

			context.SaveChanges();

			return books.Count;
		}
	}
}
