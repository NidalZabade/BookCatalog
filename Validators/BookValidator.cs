using BookCatalog.DTOs;

namespace BookCatalog.Validators
{
    public static class BookValidator
    {
        public static List<string> ValidateCreate(CreateBookDto dto)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("Title is required.");
            if (string.IsNullOrWhiteSpace(dto.Author))
                errors.Add("Author is required.");
            if (string.IsNullOrWhiteSpace(dto.Genre))
                errors.Add("Genre is required.");
            if (dto.PublishedYear > DateTime.Now.Year)
                errors.Add("Published year cannot be in the future.");
            if (dto.Price < 0)
                errors.Add("Price must be positive.");
            return errors;
        }

        public static List<string> ValidateUpdate(UpdateBookDto dto)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("Title is required.");
            if (string.IsNullOrWhiteSpace(dto.Author))
                errors.Add("Author is required.");
            if (string.IsNullOrWhiteSpace(dto.Genre))
                errors.Add("Genre is required.");
            if (dto.PublishedYear > DateTime.Now.Year)
                errors.Add("Published year cannot be in the future.");
            if (dto.Price < 0)
                errors.Add("Price must be positive.");
            return errors;
        }
    }
}
