using BooksXMLClassLibrary.Model;
using System.Xml.Serialization;
using System;
using System.IO;

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
            if (allBooks == null)
            {
                allBooks = new List<BooksXML_Book>();
            }
            uint cMaxNumber = getMaxBookNumber();
            // get real!
            if (cMaxNumber+1 == uint.MaxValue) { return null; }
            newBook.bookNumber = cMaxNumber+1;
            allBooks.Add(newBook);
            return newBook.bookNumber;
        }

        public List<BooksXML_Book> FindBooksByPartOfTitle(String partTitle, bool ignoreCase)
        {
            List<BooksXML_Book> rslt = new List<BooksXML_Book>();
            if ((allBooks == null)||(allBooks.Count == 0)) {
                return rslt;
            }
            // if search term is empty then return full list
            if (String.IsNullOrEmpty(partTitle)) {
                return allBooks;
            }
            if (ignoreCase == false)   {
                rslt = allBooks.FindAll((BooksXML_Book parm) => { return parm.title.Contains(partTitle); });
            } else {
                rslt = allBooks.FindAll((BooksXML_Book parm) => { return parm.title.Contains(partTitle, StringComparison.InvariantCultureIgnoreCase); });
            }
            return rslt;
        }
        /// <summary>
        /// used to compare two book entries for sorting.
        /// https://learn.microsoft.com/ru-ru/dotnet/api/system.collections.generic.list-1.sort?view=net-8.0
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>number: 0 if they are "equal"</returns>
        private static int booksComparator(BooksXML_Book x, BooksXML_Book y)
        {
            // x and y are referenced from outside
            int compareTitles()
            {
                if (x.title == null && y.title == null) return 0;
                else if (x.title == null) return -1;
                else if (y.title == null) return 1;
                else return x.title.CompareTo(y.title);
            }

            if ((x == null) || (y == null)) return 0;
            if (x.author == null)
            {
                if (y.author == null)
                {
                    // apparently authors are same and null
                    return compareTitles();
                }
                else
                {
                    // If first author is null and second author is not null, second author is greater.
                    return -1;
                }
            }
            else
            {
                // If first author is not null...
                if (y == null)
                {
                    // ...and second is null, first is greater.
                    return 1;
                }
                else
                {
                    int retval = x.author.CompareTo(y.author);
                    if (retval != 0)
                    {
                        // authors are different
                        return retval;
                    }
                    else
                    {
                        // If the authors are same then compare book titles
                        return compareTitles();
                    }

                }
            }
        }
        /// <summary>
        /// Sort the list in alphabetical order by author first. Then for each author sort it in alphabetical order by title.
        /// allBooks list gets sorted
        /// </summary>
        public void SortBooksInPlaceByAuthorAndTitle()
        {
            if ((allBooks == null)||(allBooks.Count ==0)) { return; }
            allBooks.Sort( booksComparator );
        }

        /// <summary>
        /// Sort the list in alphabetical order by author first. Then for each author sort it in alphabetical order by title
        /// allBooks list remains same
        /// </summary>
        /// <returns>get sorted list of all books</returns>
        public List<BooksXML_Book> SortBooksByAuthorAndTitle()
        {
            if ((allBooks == null) || (allBooks.Count == 0)) { return new List<BooksXML_Book>(); }
            return allBooks.OrderBy(p=>p.author).ThenBy(p=>p.title).ToList();
        }

        /// <summary>
        /// save edits to a book entry, by its number
        /// </summary>
        /// <param name="editedBook">contains data to save</param>
        public void applyEditsToBook(BooksXML_Book editedBook)
        {
            if ((allBooks == null) || (allBooks.Count == 0)) return;
            for (int i = 0; i < allBooks.Count; i++)
            {
                if (editedBook.bookNumber == allBooks[i].bookNumber)
                {
                    allBooks[i].title = editedBook.title;
                    allBooks[i].author = editedBook.author;
                    allBooks[i].pages = editedBook.pages;
                    break;
                }
            }
        }

        public bool removeBook(BooksXML_Book removedBook)
        {
            if ((allBooks == null) || (allBooks.Count == 0)) return false;
            int position = -1;
            position = allBooks.FindIndex((BooksXML_Book pp) => { return pp.bookNumber == removedBook.bookNumber; });
            if (position == -1) { return false; } else { allBooks.RemoveAt(position); return true; }
        }

        /// <summary>
        /// save current content of list with entities to file
        /// </summary>
        /// <param name="filePath">path where to save</param>
        public void saveAllBooks(string filePath)
        {
            using (var writer = new FileStream(filePath, FileMode.Create))
            {
                XmlSerializer ser = new XmlSerializer(typeof(BooksXML_ListOfBooks), new XmlRootAttribute(allBooksRoot));
                ser.Serialize(writer, allBooks);
            }
        }
    }
}
