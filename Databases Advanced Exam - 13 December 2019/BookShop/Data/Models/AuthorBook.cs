using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookShop.Data.Models
{
   public class AuthorBook
    {
        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        public Author Author { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

        public Book Book { get; set; }
    }
}
