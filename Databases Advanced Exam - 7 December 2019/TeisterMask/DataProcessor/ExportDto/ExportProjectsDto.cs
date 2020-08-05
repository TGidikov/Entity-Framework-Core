using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Project")]
    public class ExportProjectsDto
    {
        [XmlAttribute("TasksCount")]
        public int TasksCount { get; set; }

        [XmlElement("ProjectName")]
        public string ProjectName { get; set; }

        [XmlElement("HasEndDate")]
        public string  HasEndDate { get; set; }

        [XmlArray("Tasks")]
        public UserTasksDto[] Tasks { get; set; }
    }
    [XmlType("Task")]
    public class UserTasksDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Label")]
        public string Label { get; set; }

    }
}
//<? xml version="1.0" encoding="utf-16"?>
//<Projects>
//  <Project TasksCount = "10" >
//    < ProjectName > Hyster - Yale </ ProjectName >
//    < HasEndDate > No </ HasEndDate >
//    < Tasks >
//      < Task >
//        < Name > Broadleaf </ Name >
//        < Label > JavaAdvanced </ Label >
//      </ Task >
