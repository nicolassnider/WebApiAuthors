using AutoMapper;
using WebApiAuthors.Controllers.Entities;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Utilities
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AuthorCreationDTO, Author>();
            CreateMap<Author, AuthorDTO>();
            CreateMap<Author, AuthorDTOWithBooks>()
                .ForMember(authorDTO => authorDTO.Books,options => options.MapFrom(MapAuthorDTOBooks));
            CreateMap<BookCreationDTO, Book>()
                .ForMember(book=>book.AuthorsBooks, options=>options.MapFrom(MapAuthorsBooks));
            CreateMap<Book, BookDTO>();
            CreateMap<Book, BookDTOWithAuthors>()
                .ForMember(bookDTO => bookDTO.Authors, options => options.MapFrom(MapBookDTOAuthors));
            CreateMap<BookPatchDTO, Book>().ReverseMap();
            CreateMap<CommentCreationDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
        }
        private List<AuthorBook> MapAuthorsBooks(BookCreationDTO bookCreationDTO, Book book) {
            var result = new List<AuthorBook>();
            if (bookCreationDTO.AuthorIds == null) { return result; }
            foreach(var authorId in bookCreationDTO.AuthorIds)
            { result.Add(new AuthorBook() { AuthorId = authorId }); }
            return result;
        }

        private List<AuthorDTO> MapBookDTOAuthors(Book book,BookDTO bookDTO)
        {
            var result = new List<AuthorDTO>();
            if (book.AuthorsBooks == null) return result;
            foreach (var authorBook in book.AuthorsBooks) result.Add(new AuthorDTO() { Id = authorBook.AuthorId, Name = authorBook.Author.Name });
            return result;
        }
        private List<BookDTO> MapAuthorDTOBooks(Author author, AuthorDTO authorDTO)
        {
            var result = new List<BookDTO>();
            if (author.AuthorsBooks == null) return result;
            foreach (var authorBook in author.AuthorsBooks) result.Add(new BookDTO() { Id = authorBook.BookId, Title = authorBook.Book.Title });
            return result;
        }

    }
}
