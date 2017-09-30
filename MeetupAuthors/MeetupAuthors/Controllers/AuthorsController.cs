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
            AuthorDto authorDto = MapAuthorToDto(author);

            return Request.CreateResponse(HttpStatusCode.OK, authorDto);


        }

        private AuthorDto MapAuthorToDto(Author author)
        {
            return new AuthorDto()
            {
                Id = author.Id,
                Name = author.FirstName + " " + author.LastName,
                Age = CalculateAge(author.BirthDate)
            };
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

            return Request.CreateResponse(HttpStatusCode.Created, MapAuthorToDto(author));


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
