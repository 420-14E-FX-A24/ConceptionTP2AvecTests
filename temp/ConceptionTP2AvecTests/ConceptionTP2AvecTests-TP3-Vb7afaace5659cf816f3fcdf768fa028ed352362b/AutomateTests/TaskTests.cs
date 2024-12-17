using Automate.ViewModels;
using Automate.Interfaces;
using Moq;
using System.Windows;
using Automate.Utils;

namespace Automate.Tests
{
	[TestFixture]
	public class TaskTests
	{
		private Mock<MongoDBService> _mongoServiceMock;
		private Mock<NavigationService> _navigationServiceMock;
		private Mock<IWindowService> _windowServiceMock;
		private EditDayTasksViewModel _viewModel;
		private Window _window;

		[SetUp]
		public void Setup()
		{
			_mongoServiceMock = new Mock<MongoDBService>("AutomateDB");
			_navigationServiceMock = new Mock<NavigationService>();
			_windowServiceMock = new Mock<IWindowService>();
			_window = new Window();
			_viewModel = new EditDayTasksViewModel(_window);
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_AddTask_Correct_Task_Count()
		{
			var count = _viewModel.Tasks.Count;

			_viewModel.AddTaskCommand.Execute(null);

			Assert.That(_viewModel.Tasks.Count, Is.EqualTo(count + 1));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_AddTask_Correct_Date()
		{
			_viewModel.AddTaskCommand.Execute(null);

			Assert.That(_viewModel.Tasks.Last().Date, Is.EqualTo(_viewModel.DateSelection.Date));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_EditTask_Update_Comment()
		{
			Models.Task task = new Models.Task(Models.Task.TypeEnum.Arrosage) { Comment = "comment" };

			_viewModel.Tasks.Add(task);
			_viewModel.SelectedTask = task;
			_viewModel.Comment = "new comment";
			_viewModel.EditTaskCommand.Execute(null);

			Assert.That(task.Comment, Is.EqualTo("new comment"));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_EditTask_Update_Type()
		{
			Models.Task task = new Models.Task(Models.Task.TypeEnum.Arrosage) { Comment = "comment" };

			_viewModel.Tasks.Add(task);
			_viewModel.SelectedTask = task;
			_viewModel.SelectedType = Models.Task.TypeEnum.Rempotage;
			_viewModel.EditTaskCommand.Execute(null);

			Assert.That(task.Type, Is.EqualTo(Models.Task.TypeEnum.Rempotage));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_Remove_Selected_Task_Update_Task_Count()
		{
			Models.Task task = new Models.Task(Models.Task.TypeEnum.Semis);

			_viewModel.Tasks.Add(task);
			_viewModel.SelectedTask = task;

			int count = _viewModel.Tasks.Count;

			_viewModel.RemoveTaskCommand.Execute(null);

			Assert.That(_viewModel.Tasks.Count, Is.EqualTo(count - 1));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void Test_Remove_Selected_Task()
		{
			Models.Task task = new Models.Task(Models.Task.TypeEnum.Semis);

			_viewModel.Tasks.Add(task);
			_viewModel.SelectedTask = task;
			_viewModel.RemoveTaskCommand.Execute(null);

			Assert.That(_viewModel.Tasks.Contains(task), Is.False);
		}
	}
}
