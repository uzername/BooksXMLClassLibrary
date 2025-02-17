using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BooksXMLClassLibrary.Model
{
    /// <summary>
    /// Good for deserializing list
    /// https://www.csharp.com/blogs/deserializing-xml-into-a-list-or-array-of-objects
    /// </summary>
    [XmlRoot(ElementName = "AllBooks")]
    public class BooksXML_ListOfBooks : List<BooksXML_Book>
    {
    }
}
