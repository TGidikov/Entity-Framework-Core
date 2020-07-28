using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Import
{
    [XmlType("User")]
    public class ImportUserDto
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }
       
        [XmlElement("lastName")]
        public string Lastname { get; set; }
        
        [XmlElement("age")]
        public int Age { get; set; }

    }
}
  // <User>
  //      <firstName>Chrissy</firstName>
  //      <lastName>Falconbridge</lastName>
  //      <age>50</age>
  //  </User>