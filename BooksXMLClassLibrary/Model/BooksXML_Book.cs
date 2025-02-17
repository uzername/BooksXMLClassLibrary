using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BooksXMLClassLibrary.Model
{
    /// <summary>
    ///  Each book has a title, an author, and number of pages
    /// </summary>
    [XmlType(TypeName = "Book")]
    public class BooksXML_Book  {
        [XmlElement(ElementName = "BookTitle")]
        public string title {  get; set; }
        [XmlElement(ElementName = "BookAuthor")]
        public string author { get; set; }
        // sometimes pages may be not specified. here I do incantatums
        [XmlIgnore]
        public uint? pages { get; set; }

        [XmlElement(ElementName = "NumPages")]
        public string ValuePages
        {
            get => pages?.ToString();
            set => pages = string.IsNullOrEmpty(value) ? (uint?)null : uint.Parse(value);
        }


        /// <summary>
        /// this one is useful for various lists and bindings and additions but it should not be serialized
        /// you should not manually change this - just think on it
        /// </summary>
        [XmlIgnore]
        public uint bookNumber { get; set; }
    }
}
