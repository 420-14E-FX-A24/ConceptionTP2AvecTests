using NUnit.Framework;
using Moq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Automate.Interfaces;
using Automate.Utils;
using System.Windows;

namespace Automate.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ReadTests
    {
        private Mock<IWindowService> _windowServiceMock;
        private WindowServiceWrapper _windowServiceWrapper;


        [SetUp]
        public void SetUp()
        {
            _windowServiceMock = new Mock<IWindowService>();

            // Mock the OpenRead method
            string fileContent = "DateTime,Température (°C),Humidité (%),Luminosité (lux)\n" +
                                 "2023-10-01 00:00,18,70,5\n" +
                                 "2023-10-01 01:00,17,72,5\n" +
                                 "2023-10-01 02:00,17,73,5\n" +
                                 "2023-10-01 03:00,16,75,5\n" +
                                 "2023-10-01 04:00,16,76,5\n" +
                                 "2023-10-01 05:00,16,74,10";
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var fileStream = new MemoryStream(encoding.GetBytes(fileContent));

            _windowServiceMock.Setup(ws => ws.OpenRead(It.IsAny<string>())).Returns(fileStream);

            // Create the WindowServiceWrapper instance with the mock
            _windowServiceWrapper = new WindowServiceWrapper(null, null, null, _windowServiceMock.Object);
        }

        [Test]
        public void LoadFile_CallLoadFileMethod()
        {
            string fileName = "testfile.txt";
            _windowServiceMock.Object.LoadFile(fileName);
            _windowServiceMock.Verify(ws => ws.LoadFile(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void LoadFile_PopulateFileData()
        {
            _windowServiceWrapper.LoadFile("testfile.txt");
            _windowServiceMock.Verify(ws => ws.OpenRead(It.IsAny<string>()), Times.Once);
            Assert.That(_windowServiceWrapper.FileData.Count, Is.EqualTo(6));
            Assert.That(_windowServiceWrapper.FileData[0], Is.EqualTo(new[] 
            { "2023-10-01 00:00", "18", "70", "5" }));
            Assert.That(_windowServiceWrapper.FileData[1], Is.EqualTo(new[] 
            { "2023-10-01 01:00", "17", "72", "5" }));
            Assert.That(_windowServiceWrapper.FileData[2], Is.EqualTo(new[] 
            { "2023-10-01 02:00", "17", "73", "5" }));
            Assert.That(_windowServiceWrapper.FileData[3], Is.EqualTo(new[] 
            { "2023-10-01 03:00", "16", "75", "5" }));
            Assert.That(_windowServiceWrapper.FileData[4], Is.EqualTo(new[] 
            { "2023-10-01 04:00", "16", "76", "5" }));
            Assert.That(_windowServiceWrapper.FileData[5], Is.EqualTo(new[] 
            { "2023-10-01 05:00", "16", "74", "10" }));
        }
    }
}