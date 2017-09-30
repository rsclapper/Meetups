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
        private static List<Book> _books;

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
                _books = new List<Book>()
                {
                    new Book()
                    {
                        Id = 1,
                        AuthorId = 1,
                        Title = "Ice and Fire",
                        Description = "Lots of people dying"
                    
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

        public void UpdateAuthor(Author author)
        {
            int index = _authors.FindIndex(a => a.Id == author.Id);
            if (index >= 0)
                _authors[index] = author;
        }

        public void DeleteAuthor(int id)
        {
            int index = _authors.FindIndex(a => a.Id == id);
            if (index >= 0)
                _authors.RemoveAt(index);
        }

        public List<Book> GetAllBooksByAuthor(int authorId)
        {
            return _books.Where(b => b.AuthorId == authorId).ToList();
        }

        public void AddBook(int authorId, Book book)
        {
            int nextId = 0;
            if (_books.Any())
            {
                nextId = _books.Max(a => a.Id);
            }
            nextId++;
            book.Id = nextId;
            _books.Add(book);
        }
    }
}