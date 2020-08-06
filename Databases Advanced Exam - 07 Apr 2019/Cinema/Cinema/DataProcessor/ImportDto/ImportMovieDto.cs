using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.DataProcessor.ImportDto
{
    public class ImportMovieDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Title { get; set; }
       
        public string Genre { get; set; }

      
        public TimeSpan Duration { get; set; }

        [Range(1, 10)]
        public double Rating { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Director { get; set; }


    }
    //}
    //    "Title": "Little Big Man",
    //    "Genre": "Western",
    //    "Duration": "01:58:00",
    //    "Rating": 28,
    //    "Director": "Duffie Abrahamson"
    //  },
}