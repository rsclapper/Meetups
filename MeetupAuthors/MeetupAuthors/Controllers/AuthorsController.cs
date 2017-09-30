using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MeetupAuthors.DAL;
using MeetupAuthors.Models;

namespace MeetupAuthors.Controllers
{
    public class AuthorsController : ApiController
    {
        private AuthorRepository repository;

        public AuthorsController()
        {
            repository = new AuthorRepository();
        }
        public HttpResponseMessage Get()
        {
            AuthorRepository repository = new AuthorRepository();
            var authors = repository.GetAll()
                .Select(a => new AuthorDto()
                {
                    Id = a.Id,
                    Name = a.FirstName + " " + a.LastName,
                    Age = CalculateAge(a.BirthDate)
                });
            
            var response = Request.CreateResponse(HttpStatusCode.OK, authors);

            return response;
        }

        public HttpResponseMessage Get(int id)
        {
            AuthorRepository repository = new AuthorRepository();
            Author author = repository.GetById(id);
            if (author == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            List<Book> books = repository.GetAllBooksByAuthor(id);
            AuthorDto authorDto = MapAuthorToDto(author,books);

            return Request.CreateResponse(HttpStatusCode.OK, authorDto);


        }

        public HttpResponseMessage Post(AuthorCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            Author author = new Author();
            author.FirstName = dto.FirstName;
            author.LastName = dto.LastName;
            author.BirthDate = DateTime.SpecifyKind(dto.BirthDate, DateTimeKind.Local);
            
            AuthorRepository repository = new AuthorRepository();

            repository.CreateAuthor(author);

            return Request.CreateResponse(HttpStatusCode.Created, MapAuthorToDto(author, new List<Book>()));


        }

        public HttpResponseMessage Put(int id, AuthorUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            Author author = repository.GetById(id);
            if (author == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            author.FirstName = dto.FirstName;
            author.LastName = dto.LastName;
            author.BirthDate = DateTime.SpecifyKind(dto.BirthDate, DateTimeKind.Local);
            repository.UpdateAuthor(author);

            return Request.CreateResponse(HttpStatusCode.NoContent);

        }

        public HttpResponseMessage Delete(int id)
        {
            Author author = repository.GetById(id);
            if (author == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            repository.DeleteAuthor(id);
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        [Route("api/authors/{id}/Books")]
        public HttpResponseMessage GetBooks(int id)
        {
            Author author = repository.GetById(id);
            if (author == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound,$"This author {id} does not exisit");
            }

            List<Book> books = repository.GetAllBooksByAuthor(id);

            List<BookDto> bookDtos = books.Select(b => new BookDto()
            {
                Description = b.Description,
                Title =  b.Title,
                Id = b.Id
            }).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, bookDtos);
        }

        [Route("api/authors/{id}/Books")]
        [HttpPost]
        public HttpResponseMessage CreateBook(int id, BookCreateDto book)
        {
            Author author = repository.GetById(id);
            if (author == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"This author {id} does not exisit");
            }
            if (book.Title == book.Description)
            {
                ModelState.AddModelError(nameof(BookCreateDto), "The tile can not be the same as the description");
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            
            Book b = new Book();
            b.Title = book.Title;
            b.Description = book.Description;
            b.AuthorId = book.AuthorId;

            repository.AddBook(id, b);

            return Request.CreateResponse(HttpStatusCode.Created, MapBookToDto(b));
        }
        private AuthorDto MapAuthorToDto(Author author, List<Book> books)
        {
            return new AuthorDto()
            {
                Id = author.Id,
                Name = author.FirstName + " " + author.LastName,
                Age = CalculateAge(author.BirthDate),
                TotalBooks = books.Count
            };
        }

        private BookDto MapBookToDto(Book book)
        {
            return new BookDto()
            {
                Description = book.Description,
                Title = book.Title,
                Id = book.Id
            };
        }
        private int CalculateAge(DateTimeOffset argBirthDate)
        {
            int age = 0;
            var today = DateTime.Today;

            age = today.Year - argBirthDate.Year;

            if (argBirthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }


    }
}
