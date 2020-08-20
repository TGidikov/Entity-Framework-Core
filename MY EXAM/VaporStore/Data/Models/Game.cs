using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VaporStore.Data.Models
{
   public class Game
    {
        public Game()
        {
            this.Purchases = new HashSet<Purchase>();
            
            this.GameTags = new HashSet<GameTag>();

        }
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }
        
        [Required]
        [ForeignKey("Developer")]
        public int DeveloperId { get; set; }
        
        [Required]
        public Developer Developer { get; set; }
       
        [Required]
        [ForeignKey("Genre")]
        public int GenreId { get; set; }
       
        [Required]
        public Genre Genre { get; set; }

        public virtual ICollection<Purchase>  Purchases{ get; set; }

        [Required]
        public virtual ICollection<GameTag> GameTags { get; set; }

    }
}
