using GeolocationAPI.Services;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }



        [Test]
        [TestCase("192.168.0.105")]
        [TestCase("5.134.213.82")]
        public void IsIPAddressValid_CorrectAddress_ShouldReturnTrue(string ip)
        {
            //Arrange
            //Act
            var result = Helper.IsIPAddressValid(ip);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("http/adres.com")]
        [TestCase("12")]
        [TestCase("false")]
        public void IsIPAddressValid_IncorrectCorrectAddress_ShouldReturnFalse(string ip)
        {
            //Arrange
            //Act
            var result = Helper.IsIPAddressValid(ip);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void IsIPAddressValid_AdressIsNullOrEmpty_ShouldReturnFalse(string ip)
        {
            //Arrange
            //Act
            var result = Helper.IsIPAddressValid(ip);

            //Assert
            Assert.IsFalse(result);
        }


        
        [Test]
        [TestCase("http://wykop.pl")]
        [TestCase("wykop.pl")]
        [TestCase("http://onet.pl")]
        public void IsValidURI_CorrectAddress_ShouldReturnTrue(string URI)
        {
            //Arrange
            //Act
            var result = Helper.IsValidURI(URI);

            //Assert
            Assert.IsTrue(result);
        }
    }
}