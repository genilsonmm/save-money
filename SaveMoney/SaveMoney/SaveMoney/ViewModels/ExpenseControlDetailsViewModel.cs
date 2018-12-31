using System;
using Prism.Commands;
using Prism.Navigation;
using SaveMoney.Data;
using SaveMoney.Model;

namespace SaveMoney.ViewModels
{
    public class ExpenseControlDetailsViewModel : ViewModelBase
    {
        private ExpenseControl _expenseControl;
        public ExpenseControl ExpenseControl
        {
            get { return _expenseControl; }
            set { SetProperty(ref _expenseControl, value); }
        }

        public DelegateCommand AddNewExpenseCommand { get; private set; }
        public DelegateCommand EditExpenseControlCommand { get; private set; }

        public DelegateCommand<Transaction> ItemTappedCommand { get; set; }

        public ExpenseControlDetailsViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            AddNewExpenseCommand = new DelegateCommand(AddNewExpenseTransaction, CanAddOrEditExpense)
                .ObservesProperty(()=> ExpenseControl);

            EditExpenseControlCommand = new DelegateCommand(EditExpenseControl, CanAddOrEditExpense)
                .ObservesProperty(() => ExpenseControl);

            ItemTappedCommand = new DelegateCommand<Transaction>(NavigateToCurrentTransaction);
        }

        private void NavigateToCurrentTransaction(Transaction transaction)
        {
            var parameter = new NavigationParameters();
            parameter.Add("transactionId", transaction.TransactionId);
            parameter.Add("expenseControlId", ExpenseControl.ExpenseControlId);
            Navigate<EditExpenseViewModel>(parameter);
        }

        //Não permite adicionar novos debitos para um mês passado
        private bool CanAddOrEditExpense()
        {
            if (ExpenseControl == null || ExpenseControl.BeginYear < DateTime.Now.Year || ExpenseControl.BeginMonth < DateTime.Now.Month)
                return false;
            return true;
        }

        private void AddNewExpenseTransaction()
        {
            var parameter = new NavigationParameters();
            parameter.Add("expenseControl", ExpenseControl);
            Navigate<NewExpenseViewModel>(parameter);
        }

        private void EditExpenseControl()
        {
            var parameter = new NavigationParameters();
            parameter.Add("expenseControl", ExpenseControl);
            parameter.Add("isEdit", true);
            Navigate<NewAndEditExpenseControlViewModel>(parameter);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("expenseControlId"))
            {
                int expenseControlId = (int)parameters["expenseControlId"];
                GetExpenseControl(expenseControlId);
            }

            if (parameters.ContainsKey("message"))
            {
                Toast(parameters["message"].ToString());
            }
        }

        private void GetExpenseControl(int expenseControlId)
        {
            using (var expenseControlRepository = new ExpenseControlRepository())
            {
                ExpenseControl = expenseControlRepository.GetById(expenseControlId, true);
            }
        }
    }
}
