using BooksXMLClassLibrary;
using BooksXMLClassLibrary.Model;
using System.IO;
using System.Reflection;
namespace BooksXMLClassLibraryTests
{
    /// <summary>
    /// unit tests. created with help of ChatGPT because it explained what is unit testing. I like ChatGPT so much
    /// </summary>
    [TestClass]
    public class BooksXMLClassLibraryTests
    {
        private string resourceName;
        private string _tempFilePath;

        private string FirstBookAuthorName;
        private string FirstBookTitleName;
        private string LastBookAuthorName;
        private string LastBookTitleName;
        private int indexOfNullPageValue;
        private string NewBookAuthor;
        private string NewBookTitle;
        private uint NewBookPageCount;

        private String knownSearchTerm; // book with title like that should be in dataset
        private String unknownSearchTerm; // book with title like that should not be in dataset

        private String firstAuthorName; // author name that should appear first in sorted dataset
        private String nextAuthorName;  // author name that should appear next in sorted dataset
        private string multipleBookAuthorName; // author name for which there are several books
        private string firstBookforAuthor; 
        private string nextBookforAuthor;

        /// <summary>
        /// init here test values. I moved it into subroutine in case I decide to perform tests on SampleDataEng.xml 
        /// (then I create initValuesEng to match that dataset)
        /// </summary>
        private void initValues()
        {
            resourceName = "BooksXMLClassLibraryTests.sample.SampleData.xml";

            FirstBookAuthorName = "Іван Франко";
            FirstBookTitleName = "Перехресні стежки";
            LastBookAuthorName = "Анатолій Орлов";
            LastBookTitleName = "Будова двигунів внутрішнього згорання";
            indexOfNullPageValue = 4;
            NewBookAuthor = "Колесникова Олександра";
            NewBookTitle = "Готуємо та запікаємо";
            NewBookPageCount = 317;
            knownSearchTerm = "Ремонт";
            unknownSearchTerm = "Астрономія";

            firstAuthorName = "Іван Франко";
            nextAuthorName = "Тарас Григорович Шевченко";

            multipleBookAuthorName = "Тарас Григорович Шевченко";
            firstBookforAuthor = "Балади";
            nextBookforAuthor = "Кобзар";
        }
        [TestInitialize]
        public void Setup()
        {

            initValues();

            var assembly = Assembly.GetExecutingAssembly();
            
            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            _tempFilePath = Path.GetTempFileName();
            string xmlContent = result;
            File.WriteAllText(_tempFilePath, xmlContent);

        }
        [TestMethod]
        public void ShouldReturnCorrectList_WhenXmlIsValid()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            instanceHandling.loadAllBooks(_tempFilePath);
            Assert.IsNotNull(instanceHandling.allBooks);
            Assert.IsTrue(instanceHandling.allBooks.Count > 0);
            Assert.IsTrue(instanceHandling.allBooks[0].title == FirstBookTitleName);
            Assert.IsTrue(instanceHandling.allBooks[0].author == FirstBookAuthorName);

            Assert.IsTrue(instanceHandling.allBooks.Last().author == LastBookAuthorName);
            Assert.IsTrue(instanceHandling.allBooks.Last().title == LastBookTitleName);
        }
        /// <summary>
        /// I intentionally DO NOT set number of pages on some entry heheh
        /// </summary>
        [TestMethod]
        public void ShouldNullValueBeSetForPageCount_WhenXmlIsLoaded()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            instanceHandling.loadAllBooks(_tempFilePath);
            Assert.IsNotNull(instanceHandling.allBooks);
            Assert.IsTrue(instanceHandling.allBooks.Count > 0);
            Assert.IsTrue(instanceHandling.allBooks[indexOfNullPageValue].pages==null);
        }
        [TestMethod]
        public void ShouldBookNumberBeLarge_WhenXmlIsLoaded()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            instanceHandling.loadAllBooks(_tempFilePath);
            Assert.IsNotNull(instanceHandling.allBooks);
            uint maxIndex = instanceHandling.getMaxBookNumber();
            Assert.IsTrue(instanceHandling.allBooks.Count == 0 || maxIndex > 0);
        }
        /// <summary>
        /// actually adding should work even if xml was not loaded. Worth to think about it
        /// </summary>
        [TestMethod]
        public void ShouldBookAddingWork_WhenXmlIsLoaded()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            instanceHandling.loadAllBooks(_tempFilePath);
            BooksXML_Book nBook = new BooksXML_Book { author = NewBookAuthor, pages = NewBookPageCount, title = NewBookTitle };
            uint? newNumber = instanceHandling.AddAnotherBook(nBook);
            Assert.IsNotNull(newNumber);
            Assert.IsTrue(newNumber > 0);
        }
        /// <summary>
        /// useful if we add to list without reading
        /// </summary>
        [TestMethod]
        public void ShouldBookAddingWork_WhenXmlIsNotLoaded()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            BooksXML_Book nBook = new BooksXML_Book { author = NewBookAuthor, pages = NewBookPageCount, title = NewBookTitle };
            uint? newNumber = instanceHandling.AddAnotherBook(nBook);
            Assert.IsNotNull(newNumber);
            Assert.IsTrue(newNumber > 0);
            Assert.IsTrue(newNumber == 1);
        }
        [TestMethod]
        public void ShouldSearchWork_WhenXmlIsLoaded()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            instanceHandling.loadAllBooks(_tempFilePath);
            // find known title
            List<BooksXML_Book> serchRslt = instanceHandling.FindBooksByPartOfTitle(knownSearchTerm, true);
            Assert.IsTrue( serchRslt.Count > 0 );
            // find unknown title
            List<BooksXML_Book> serchRslt2 = instanceHandling.FindBooksByPartOfTitle(unknownSearchTerm, true);
            Assert.IsTrue(serchRslt2.Count == 0);
        }
        // === verify sorting ===
        [TestMethod]
        public void ShouldSortingInPlaceByAuthorWork_WhenXmlIsLoaded()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            instanceHandling.loadAllBooks(_tempFilePath);
            instanceHandling.SortBooksInPlaceByAuthorAndTitle();
            int indx1 = -1; int indx2 = -1;
            indx1= instanceHandling.allBooks.FindIndex((BooksXML_Book pp) => { return (pp.author == multipleBookAuthorName)&&(pp.title == firstBookforAuthor); });
            indx2= instanceHandling.allBooks.FindIndex((BooksXML_Book pp) => { return (pp.author == nextAuthorName) && (pp.title == nextBookforAuthor); });
            Assert.IsTrue((indx1 != -1) && (indx2 != -1));
            Assert.IsTrue(indx1<=indx2);
        }
        [TestMethod]
        public void ShouldSortingInPlaceByAuthorSortBookTitles_WhenXmlIsLoaded()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            instanceHandling.loadAllBooks(_tempFilePath);
            instanceHandling.SortBooksInPlaceByAuthorAndTitle();
            int indx1 = -1; int indx2 = -1;
            indx1 = instanceHandling.allBooks.FindIndex((BooksXML_Book pp) => { return pp.author == firstAuthorName; });
            indx2 = instanceHandling.allBooks.FindIndex((BooksXML_Book pp) => { return pp.author == nextAuthorName; });
            Assert.IsTrue((indx1 != -1) && (indx2 != -1));
            Assert.IsTrue(indx1 <= indx2);
        }

        [TestMethod]
        public void ShouldSortingByAuthorWork_WhenXmlIsLoaded()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            instanceHandling.loadAllBooks(_tempFilePath);
            List<BooksXML_Book> orderedList = instanceHandling.SortBooksByAuthorAndTitle();
            int indx1 = -1; int indx2 = -1;
            indx1 = orderedList.FindIndex((BooksXML_Book pp) => { return (pp.author == multipleBookAuthorName) && (pp.title == firstBookforAuthor); });
            indx2 = orderedList.FindIndex((BooksXML_Book pp) => { return (pp.author == nextAuthorName) && (pp.title == nextBookforAuthor); });
            Assert.IsTrue((indx1 != -1) && (indx2 != -1));
            Assert.IsTrue(indx1 <= indx2);
        }
        [TestMethod]
        public void ShouldSortingByAuthorSortBookTitles_WhenXmlIsLoaded()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            instanceHandling.loadAllBooks(_tempFilePath);
            List<BooksXML_Book> orderedList = instanceHandling.SortBooksByAuthorAndTitle();
            int indx1 = -1; int indx2 = -1;
            indx1 = instanceHandling.allBooks.FindIndex((BooksXML_Book pp) => { return pp.author == firstAuthorName; });
            indx2 = instanceHandling.allBooks.FindIndex((BooksXML_Book pp) => { return pp.author == nextAuthorName; });
            Assert.IsTrue((indx1 != -1) && (indx2 != -1));
            Assert.IsTrue(indx1 <= indx2);
        }
        [TestMethod]
        public void ShouldSaveToFileWork_WhenXmlIsLoaded()
        {
            BooksXMLHandling instanceHandling = new BooksXMLHandling();
            instanceHandling.loadAllBooks(_tempFilePath);
            BooksXML_Book nBook = new BooksXML_Book { author = NewBookAuthor, pages = NewBookPageCount, title = NewBookTitle };
            uint? newNumber = instanceHandling.AddAnotherBook(nBook);
            Assert.IsNotNull(newNumber);
            instanceHandling.saveAllBooks(_tempFilePath);
            instanceHandling.loadAllBooks(_tempFilePath);
            List<BooksXML_Book> srchRslt = instanceHandling.FindBooksByPartOfTitle(NewBookTitle, true);
            Assert.IsTrue(srchRslt.Count > 0);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }
    }
}