using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{  
    [XmlType("Project")]
   public class ImportProjectDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }
        
        [XmlElement("DueDate")]
        public string DueDate { get; set; }

        [XmlArray("Tasks")]
        public ImportTaskDTO[] Tasks { get; set; }
    }

    [XmlType("Task")]
    public class ImportTaskDTO
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }

        [Required]
        [XmlElement("DueDate")]
        public string DueDate { get; set; }

        //[Required]
        [XmlElement("ExecutionType")]
        [Range(0,3)]
        public int ExecutionType { get; set; }

        //[Required]
        [XmlElement("LabelType")]
        [Range(0,4)]
        public int LabelType { get; set; }

    }
}
