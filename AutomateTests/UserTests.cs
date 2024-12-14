using Automate.ViewModels;
using Automate.Interfaces;
using Moq;
using Automate.Models;
using Automate.Utils;
using Automate.Views;
using System.Windows;

namespace Automate.Tests
{
	[TestFixture]
	public class UserTests
	{
		private Mock<MongoDBService> _mongoServiceMock;
        private Mock<INavigationService> _navigationServiceMock;
        private Mock<IWindowService> _windowServiceMock;
		private LoginViewModel _viewModel;


        [SetUp]
		public void Setup()
		{
       
            _mongoServiceMock = new Mock<MongoDBService>("AutomateDB");
            _navigationServiceMock = new Mock<INavigationService>();
            _windowServiceMock = new Mock<IWindowService>();

			_viewModel = new LoginViewModel(null, _navigationServiceMock.Object);

		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_Username_Validation_When_Empty()
		{
			_viewModel.Username = "";

			Assert.That(_viewModel.HasErrors, Is.True);
			Assert.That(_viewModel.ErrorMessages, Is.EqualTo("Le nom d'utilisateur ne peut pas être vide."));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_Password_Validation_When_Empty()
		{
			_viewModel.Password = "";

			Assert.That(_viewModel.HasErrors, Is.True);
			Assert.That(_viewModel.ErrorMessages, Is.EqualTo("Le mot de passe ne peut pas être vide."));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_Authenticate_With_Invalid_Credentials()
		{
			_viewModel.Username = "invalidUsername";
			_viewModel.Password = "invalidPassword";

			_mongoServiceMock.Setup(x => x.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns((User?)null);

			_viewModel.AuthenticateCommand.Execute(null);

			Assert.That(_viewModel.HasErrors, Is.True);
			Assert.That(_viewModel.ErrorMessages, Is.EqualTo("Nom d'utilisateur ou mot de passe invalide"));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_Authenticate_With_Valid_Credentials()
		{
			_viewModel.Username = "Frederic";
			_viewModel.Password = ".";

			User validUser = new User { Username = "Frederic", PasswordHash = "$2a$11$Rc0K8jktZrVizcxsNmEQU.c94VWEHjKxrmk0I09p5dkBteMSoJ2Bq", IsAdmin = true };
			_mongoServiceMock.Setup(x => x.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(validUser);

			_viewModel.AuthenticateCommand.Execute(null);

			Assert.That(_viewModel.HasErrors, Is.False);
			_navigationServiceMock.Verify(x => x.NavigateTo<HomeWindow>(null, true), Times.Once);
		}



    }
}
