using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApiApp4115.Models;

using WebApiApp4115.Util;

namespace WebApiApp4115.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBConnection db = new DBConnection();

        public BooksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/books/get-all
        [HttpGet("get-all")]
        public IActionResult GetAllBooks()
        {
            var list = new List<Books>();
            var conn = db.GetConn();
            db.ConOpen();

            string sql = "SELECT * FROM Books";
            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Books
                {
                    BookID = (int)reader["BookID"],
                    Title = reader["Title"].ToString(),
                    Author = reader["Author"].ToString(),
                    ISBN = reader["ISBN"].ToString(),
                    PublishedYear = (int)reader["PublishedYear"]
                });
            }

            db.ConClose();
            return Ok(list);
        }

        // GET: api/books/id?id=1
        [HttpGet("id")]
        public IActionResult GetBookById(int id)
        {
            var conn = db.GetConn();
            db.ConOpen();

            string sql = "SELECT * FROM Books WHERE BookID = @BookID";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BookID", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var book = new Books
                {
                    BookID = (int)reader["BookID"],
                    Title = reader["Title"].ToString(),
                    Author = reader["Author"].ToString(),
                    ISBN = reader["ISBN"].ToString(),
                    PublishedYear = (int)reader["PublishedYear"]
                };
                db.ConClose();
                return Ok(book);
            }

            db.ConClose();
            return NotFound("Book not found");
        }

        [HttpPost("add")]
        public IActionResult AddBook([FromBody] Books book)
        {
            var conn = db.GetConn();
            db.ConOpen();

            string sql = "INSERT INTO Books (Title, Author, ISBN, PublishedYear) VALUES (@Title, @Author, @ISBN, @PublishedYear)";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@Author", book.Author);
            cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
            cmd.Parameters.AddWithValue("@PublishedYear", book.PublishedYear);

            int rows = cmd.ExecuteNonQuery();
            db.ConClose();

            return rows > 0 ? Ok("Book added successfully") : StatusCode(500, "Failed to add book");
        }

        // DELETE: api/books/delete/1
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteBook(int id)
        {
            var conn = db.GetConn();
            db.ConOpen();

            string sql = "DELETE FROM Books WHERE BookID = @BookID";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BookID", id);

            int rows = cmd.ExecuteNonQuery();
            db.ConClose();

            return rows > 0 ? Ok("Book deleted successfully") : NotFound("Book not found");
        }
    }
}
