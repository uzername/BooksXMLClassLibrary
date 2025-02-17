using BooksXMLClassLibrary;
using BooksXMLClassLibrary.Model;
using System.IO;
using System.Reflection;
namespace BooksXMLClassLibraryTests
{
    /// <summary>
    /// unit tests. created with help of ChatGPT
    /// </summary>
    [TestClass]
    public class BooksXMLClassLibraryTests
    {
        private string _tempFilePath;

        private string FirstBookAuthorName;
        private string FirstBookTitleName;
        private string LastBookAuthorName;
        private string LastBookTitleName;
        private int indexOfNullPageValue;
        private string NewBookAuthor;
        private string NewBookTitle;
        private uint NewBookPageCount;
        /// <summary>
        /// init here test values. I moved it into subroutine in case I decide to perform tests on SampleDataEng.xml
        /// </summary>
        private void initValues()
        {
            FirstBookAuthorName = "Іван Франко";
            FirstBookTitleName = "Перехресні стежки";
            LastBookAuthorName = "Анатолій Орлов";
            LastBookTitleName = "Будова двигунів внутрішнього згорання";
            indexOfNullPageValue = 4;
            NewBookAuthor = "Колесникова Олександра";
            NewBookTitle = "Готуємо та запікаємо";
            NewBookPageCount = 317;
        }
        [TestInitialize]
        public void Setup()
        {

            initValues();

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "BooksXMLClassLibraryTests.sample.SampleData.xml";
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