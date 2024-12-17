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
        private (int, int) _temperatureIdeale = (13, 29); // 55 and 85 degrees Fahrenheit
        private (int, int) _humiditeIdeale = (65, 85); // 65 and 85% humidity
        private (int, int) _luminositeIdeale = (17391, 26087); // 17391, 26087 lux

        [SetUp]
        public void SetUp()
        {
            _windowServiceMock = new Mock<IWindowService>();
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


        [Test]
        public void GiveAdviceTomatoParameters_TemperatureInferieureMinValue()
        {
            string accesseurConseil = string.Empty;
            int accesseurReelle = 10;
            string propertyName = "Temperature";
            string result = _windowServiceWrapper.GiveAdviceTomatoParameters(_temperatureIdeale.Item1, 
                _temperatureIdeale.Item2, ref accesseurConseil, accesseurReelle, propertyName);
            Assert.That(result, Is.EqualTo("fermer fenêtres, ouvrir chauffage, fermer ventilateurs"));
        }

        [Test]
        public void GiveAdviceTomatoParameters_TemperatureSuperieureMaxValue()
        {
            string accesseurConseil = string.Empty;
            int accesseurReelle = 30;
            string propertyName = "Temperature";
            string result = _windowServiceWrapper.GiveAdviceTomatoParameters(_temperatureIdeale.Item1, 
                _temperatureIdeale.Item2, ref accesseurConseil, accesseurReelle, propertyName);
            Assert.That(result, Is.EqualTo("ouvrir fenêtres, fermer chauffage, ouvrir ventilateurs, ouvrir arrosage"));
        }

        [Test]
        public void GiveAdviceTomatoParameters_HumiditeInferieureMinValue()
        {
            string accesseurConseil = string.Empty;
            int accesseurReelle = 60;
            string propertyName = "Humidite";
            string result = _windowServiceWrapper.GiveAdviceTomatoParameters(_humiditeIdeale.Item1, 
                _humiditeIdeale.Item2, ref accesseurConseil, accesseurReelle, propertyName);
            Assert.That(result, Is.EqualTo("fermer fenêtres, fermer chauffage, fermer ventilateurs, ouvrir arrosage"));
        }

        [Test]
        public void GiveAdviceTomatoParameters_HumiditeSuperieurMaxValue()
        {
            string accesseurConseil = string.Empty;
            int accesseurReelle = 90;
            string propertyName = "Humidite";
            string result = _windowServiceWrapper.GiveAdviceTomatoParameters(_humiditeIdeale.Item1, 
                _humiditeIdeale.Item2, ref accesseurConseil, accesseurReelle, propertyName);
            Assert.That(result, Is.EqualTo("ouvrir fenêtres, ouvrir chauffage, ouvrir ventilateurs, fermer arrosage"));
        }

        [Test]
        public void GiveAdviceTomatoParameters_LuminositeInferieurMinValue()
        {
            string accesseurConseil = string.Empty;
            int accesseurReelle = 15000;
            string propertyName = "Luminosite";
            string result = _windowServiceWrapper.GiveAdviceTomatoParameters(_luminositeIdeale.Item1, 
                _luminositeIdeale.Item2, ref accesseurConseil, accesseurReelle, propertyName);
            Assert.That(result, Is.EqualTo("fermer fenêtres, ouvrir lumières"));
        }

        [Test]
        public void GiveAdviceTomatoParameters_LuminositeSuperieurMaxValue()
        {
            string accesseurConseil = string.Empty;
            int accesseurReelle = 27000;
            string propertyName = "Luminosite";
            string result = _windowServiceWrapper.GiveAdviceTomatoParameters(_luminositeIdeale.Item1, 
                _luminositeIdeale.Item2, ref accesseurConseil, accesseurReelle, propertyName);
            Assert.That(result, Is.EqualTo("ouvrir fenêtres, fermer lumières"));
        }
    }
}