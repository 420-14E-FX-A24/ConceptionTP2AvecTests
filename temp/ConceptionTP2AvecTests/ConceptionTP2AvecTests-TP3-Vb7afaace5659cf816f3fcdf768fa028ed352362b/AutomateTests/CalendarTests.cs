using Automate.Interfaces;
using Automate.Utils;
using Automate.ViewModels;
using Moq;
using System.Collections.ObjectModel;
using System.Windows;

namespace Automate.Tests
{

	[TestFixture]
	public class CalendarTests
	{
		private Mock<MongoDBService> _mongoServiceMock;
		private Mock<IWindowService> _windowServiceMock;
		private HomeViewModel _viewModel;
		private Window _window;

		[SetUp]
		public void Setup()
		{
			_mongoServiceMock = new Mock<MongoDBService>("AutomateDB");
			_windowServiceMock = new Mock<IWindowService>();
			_window = new Window();

			_viewModel = new HomeViewModel(_window);

			ObservableCollection<Models.Task> tasks = new ObservableCollection<Models.Task>
			{
				new Models.Task(Models.Task.TypeEnum.Semis) { Date = new DateTime(2020, 11, 5) },
				new Models.Task(Models.Task.TypeEnum.Arrosage) { Date = new DateTime(2024, 11, 12) },
				new Models.Task(Models.Task.TypeEnum.Rempotage) { Date = new DateTime(2024, 12, 1) },
				new Models.Task(Models.Task.TypeEnum.Rempotage) { Date = new DateTime(2025, 1, 1) },
				new Models.Task(Models.Task.TypeEnum.Rempotage) { Date = new DateTime(2025, 1, 1) }
			};

			_viewModel.Tasks = tasks;
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void GetTasksFromDate_For_Specific_Date()
		{
			DateTime targetDate = new DateTime(2025, 1, 1);

			ObservableCollection<Models.Task> tasks = _viewModel.GetTasksFromDate(targetDate);

			Assert.That(tasks.All(task => task.Date == targetDate));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void GetTasksFromDate_For_Specific_Date_Count()
		{
			DateTime targetDate = new DateTime(2025, 1, 1);

			ObservableCollection<Models.Task> tasks = _viewModel.GetTasksFromDate(targetDate);

			Assert.That(tasks.Count, Is.EqualTo(2));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void GetTasksFromDate_With_No_Tasks()
		{
			DateTime targetDate = new DateTime(2024, 11, 1);

			ObservableCollection<Models.Task> tasks = _viewModel.GetTasksFromDate(targetDate);

			Assert.That(tasks, Is.Empty);
		}
	}
}