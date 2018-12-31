using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SaveMoney.Data;
using SaveMoney.Model;
using System;
using System.Collections.ObjectModel;

namespace SaveMoney.ViewModels
{
    public class SelectExpenseControlViewModel : ViewModelBase
    {
        private string _currentDate;
        public string CurrentDate
        {
            get { return _currentDate; }
            set {
                SetProperty(ref _currentDate, value);
                GetCurrentExpenses();
            }
        }

        private ObservableCollection<string> _expenseYearMonths;
        public ObservableCollection<string> ExpenseYearMonths
        {
            get { return _expenseYearMonths; }
            set { SetProperty(ref _expenseYearMonths, value); }
        }

        private ObservableCollection<ExpenseControl> _currentExpenseControl;
        public ObservableCollection<ExpenseControl> CurrentExpenseControl
        {
            get { return _currentExpenseControl; }
            set { SetProperty(ref _currentExpenseControl, value);}
        }

        public DelegateCommand NewExpenseControl { get; private set; }
        public DelegateCommand<ExpenseControl> RemoveExpenseControl { get; private set; }
        public DelegateCommand<ExpenseControl> ItemTappedCommand { get; set; }

        public SelectExpenseControlViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            NewExpenseControl = new DelegateCommand(NavigateToNewExpenseControl);
            InitVariables();
        }

        private void InitVariables()
        {
            Title = "Seleção de controle de gasto";

            NewExpenseControl = new DelegateCommand(NavigateToNewExpenseControl);
            RemoveExpenseControl = new DelegateCommand<ExpenseControl>(RemoveTappedExpenseControl);
            ItemTappedCommand = new DelegateCommand<ExpenseControl>(NavigateToCurrentExpense);

            GetDates();
            CurrentDate = $"{DateTime.Now.Year}/{DateTime.Now.Month}";
        }

        private void GetDates()
        {
            ExpenseYearMonths?.Clear();

            using (var expenseControlRepository = new ExpenseControlRepository())
            {
                ExpenseYearMonths = new ObservableCollection<string>(expenseControlRepository.GetAllYearAndMonths());
            }
        }

        private async void RemoveTappedExpenseControl(ExpenseControl expenseControl)
        {
            bool task = await ShowDialog("Remover", "Confirmar exclusão de item?");
            if (task)
            {
                using (var expenseControlRepository = new ExpenseControlRepository())
                {
                    int id = expenseControlRepository.Remove(expenseControl);
                    if (id > 0)
                    {
                        GetCurrentExpenses();
                        Toast("Item removido com sucesso");
                    }
                    else
                    {
                        Toast("Não foi possível remover o item");
                    }
                }
            }
        }

        private void GetCurrentExpenses()
        {
            CurrentExpenseControl?.Clear();

            using (var expenseControlRepository = new ExpenseControlRepository())
            {
                GetCurrentDate(out int year, out int month);
                CurrentExpenseControl = new ObservableCollection<ExpenseControl>(expenseControlRepository.GetByDate(year, month));
            }

            if (CurrentExpenseControl.Count > 0)
                IsVisible = true;
            else
                IsVisible = false;
        }

        private void GetCurrentDate(out int year, out int month)
        {
            string[] currentDate = CurrentDate.Split('/');
            year = int.Parse(currentDate[0]);
            month = int.Parse(currentDate[1]);
        }

        private void NavigateToCurrentExpense(ExpenseControl expenseControl)
        {
            var parameters = new NavigationParameters();
            parameters.Add("expenseControlId", expenseControl.ExpenseControlId);
            Navigate<ExpenseControlDetailsViewModel>(parameters);
        }

        private void NavigateToNewExpenseControl()
        {
            Navigate<NewAndEditExpenseControlViewModel>();         
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("message"))
            {
                Toast(parameters["message"].ToString());
            }

            GetCurrentExpenses();
        }
    }
}
