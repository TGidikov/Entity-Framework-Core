using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
    public class ExportUserSoldProductDTO
    {
        [XmlElement("firstName")]
        public string  FirstName { get; set; }
       
        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlArray("soldProducts")]
        public UserProductDTO[] SoldProducts { get; set; }

    }
    [XmlType("Product")]
    public class UserProductDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal  Price { get; set; }


    }


}
 // <User>
 //   <firstName>Almire</firstName>
 //   <lastName>Ainslee</lastName>
 //   <soldProducts>
 //     <Product>
 //       <name>olio activ mouthwash</name>
 //       <price>206.06</price>
 //     </Product>
 //