using BooksXMLClassLibrary.Model;
using System.Xml.Serialization;
using System;

namespace BooksXMLClassLibrary
{
    /// <summary>
    /// Class that loads books from XML file, then allows to create, view and edit a list of books.
    /// it stores list of items in memory
    /// </summary>
    public class BooksXMLHandling
    {
        /// <summary>
        /// currently parsed items from XML file
        /// </summary>
        public List<BooksXML_Book> allBooks { get; private set; }
        private const string allBooksRoot = "AllBooks";
        /// <summary>
        /// read values from xml file and fill in allBooks array
        /// </summary>
        /// <param name="filePath">file path to read</param>
        public void loadAllBooks(string filePath)
        {
            allBooks = new List<BooksXML_Book>();
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.", filePath);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(BooksXML_ListOfBooks));
            using (FileStream fs = new FileStream(filePath, FileMode.Open))  {
                allBooks = xmlSerializer.Deserialize(fs) as List<BooksXML_Book>;
            }
            uint indx = 1;
            for (int i = 0; i<allBooks.Count; i++) 
            {
                allBooks[i].bookNumber = indx;
                indx++;
            }
        }
        /// <summary>
        /// get the maximal value of ID. no LINQ but good old style
        /// </summary>
        /// <returns> max value of bookNumber or 0 if list empty </returns>
        public uint getMaxBookNumber()
        {
            uint maxValue = 0;
            if (allBooks == null) return maxValue;
            for (int i = 0; i < allBooks.Count; i++)  {
                if (maxValue < allBooks[i].bookNumber) maxValue = allBooks[i].bookNumber;
            }
            return maxValue;
        }
        /// <summary>
        /// adds new book
        /// </summary>
        /// <param name="newBook"></param>
        /// <returns>null if something went wrong, or number of book if it is fine</returns>
        public uint? AddAnotherBook(BooksXML_Book newBook)
        {
            if (newBook == null )  {
                return null;
            }
            uint cMaxNumber = getMaxBookNumber();
            // get real!
            if (cMaxNumber+1 == uint.MaxValue) { return null; }
            newBook.bookNumber = cMaxNumber+1;
            allBooks.Add(newBook);
            return newBook.bookNumber;
        }



        /// <summary>
        /// save current content of list with entities to file
        /// </summary>
        /// <param name="filePath">path where to save</param>
        public void saveAllBooks(string filePath)
        {
            using (var writer = new FileStream(filePath, FileMode.Create))
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<BooksXML_ListOfBooks>), new XmlRootAttribute(allBooksRoot));
                ser.Serialize(writer, allBooks);
            }
        }
    }
}
