using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.Implort
{
    [XmlType("Car")]
    public class ImportCarsDTO
    {
        [XmlElement("make")]
        public string Make { get; set; }
        
        [XmlElement("model")]
        public string Model { get; set; }
        
        [XmlElement("TraveledDistance")]
        public long TraveledDistance { get; set; }

        [XmlArray("parts")]
        public ImportCarPartDTO[] Parts { get; set; }

    }
    [XmlType("partId")]
    public class ImportCarPartDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
//  <Car>
//    <make>Opel</make>
//    <model>Omega</model>
//    <TraveledDistance>176664996</TraveledDistance>
//    <parts>
//      <partId id = "38" />
//      < partId id="102"/>
//      < partId id="32"/>
//      <partId id = "114" />
//    </ parts >
//  </ Car >