using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using BookCatalog.Models;

namespace BookCatalog.Services
{
    public class BookService
    {
        private readonly string _csvFilePath;
        private List<Book> _books;

        public BookService(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
            _books = LoadBooksFromCsv();
        }

        public List<Book> GetAllBooks()
        {
            return _books;
        }

        public Book GetBookById(int id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }

        public IEnumerable<Book> FilterBooks(string author = null, string genre = null, int? year = null)
        {
            var query = _books.AsQueryable();
            if (!string.IsNullOrEmpty(author))
                query = query.Where(b => b.Author == author);
            if (!string.IsNullOrEmpty(genre))
                query = query.Where(b => b.Genre == genre);
            if (year.HasValue)
                query = query.Where(b => b.PublishedYear == year.Value);
            return query.ToList();
        }

        public void AddBook(Book book)
        {
            book.Id = _books.Any() ? _books.Max(b => b.Id) + 1 : 1;
            _books.Add(book);
            SaveBooksToCsv();
        }

        public bool UpdateBook(int id, Book updatedBook)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null) return false;
            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.Genre = updatedBook.Genre;
            book.PublishedYear = updatedBook.PublishedYear;
            book.Price = updatedBook.Price;
            SaveBooksToCsv();
            return true;
        }

        public bool DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null) return false;
            _books.Remove(book);
            SaveBooksToCsv();
            return true;
        }

        private List<Book> LoadBooksFromCsv()
        {
            var books = new List<Book>();
            if (!File.Exists(_csvFilePath)) return books;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };
            using (var reader = new StreamReader(_csvFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                books = csv.GetRecords<Book>().ToList();
            }
            return books;
        }

        private void SaveBooksToCsv()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };
            using (var writer = new StreamWriter(_csvFilePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteHeader<Book>();
                csv.NextRecord();
                foreach (var book in _books)
                {
                    csv.WriteRecord(book);
                    csv.NextRecord();
                }
            }
        }
    }
}
