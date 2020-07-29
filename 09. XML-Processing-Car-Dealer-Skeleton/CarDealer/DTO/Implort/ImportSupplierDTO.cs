using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.Implort
{
    [XmlType("Supplier")]
    public class ImportSupplierDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("isImporter")]
        public bool IsImporter { get; set; }

    }
}
//<Suppliers>
//    <Supplier>
//        <name>3M Company</name>
//        <isImporter>true</isImporter>
//    </Supplier>
//