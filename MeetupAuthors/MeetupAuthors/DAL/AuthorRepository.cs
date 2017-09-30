using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeetupAuthors.Models;

namespace MeetupAuthors.DAL
{
    public class AuthorRepository
    {
        private static List<Author> _authors;

        public AuthorRepository()
        {
            if (_authors == null)
            {
                _authors = new List<Author>()
                {
                    new Author(){FirstName = "John",
                        LastName = "Smith",
                        Id = 1,
                        BirthDate = new DateTimeOffset(1980,1,1,0,0,0,new TimeSpan())
                    }
                };
            }
        }

        public List<Author> GetAll()
        {
            return _authors;
        }

        public Author GetById(int id)
        {
            return _authors.FirstOrDefault(a => a.Id == id);

        }

        public void CreateAuthor(Author author)
        {
            int nextId = 0;
            if (_authors.Any())
            {
                nextId = _authors.Max(a => a.Id);
            }
            nextId++;
            author.Id = nextId;

            _authors.Add(author);
        }
    }
}