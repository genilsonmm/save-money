using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SaveMoney.Data;
using SaveMoney.Model;
using SaveMoney.Util;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SaveMoney.ViewModels
{
    public class EditExpenseViewModel : ViewModelBase
    {
        private Transaction _transaction;

        private string _registeredDateTime;
        public string RegisteredDateTime { get { return _registeredDateTime; } set { SetProperty(ref _registeredDateTime, value); } }

        private string _value;
        public string Value
        {
            get { return CurrencyConverter.Instance().Converter(_value); }
            set { SetProperty(ref _value, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set { SetProperty(ref _selectedCategory, value); }
        }

        private ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand RemoveCommand { get; private set; }

        public EditExpenseViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            Value = "0,00";
            RegisteredDateTime = string.Empty;
            Description = string.Empty;

            SaveCommand = new DelegateCommand(UpdateDebit, CanUpdateDebit)
                                    .ObservesProperty(() => Value)
                                    .ObservesProperty(() => SelectedCategory);
            RemoveCommand = new DelegateCommand(RemoveTransaction);
        }

        private async void RemoveTransaction()
        {
            bool confirm = await ShowDialog("Exclusão", "Confirmar exclusão do ítem?");
            if (confirm)
            {
                using (var transactionRepository = new TransactionRepository())
                {
                    try
                    {
                        transactionRepository.Delete(_transaction);
                        await GoLastPage("Ítem removido com sucesso!");
                    }
                    catch (Exception error)
                    {
                        Toast($"Falha ao excluir ítem!");
                    }
                }
            }
        }

        private bool CanUpdateDebit()
        {
            decimal decimalValue = DecimalConverter.Instance().Converter(Value);

            if (_transaction != null)
            {
                if (decimalValue > DecimalConverter.Instance().Converter(_transaction.ExpenseControl.AvailableValue))
                {               
                    Toast($"Saldo insuficiente");
                    return false;
                }
            }

            if (decimalValue > 0 && SelectedCategory != null)
                return true;

            return false;
        }

        private async void UpdateDebit()
        {
            using (var transactionRepository = new TransactionRepository())
            {
                _transaction.CategoryId = SelectedCategory.CategoryId;
                _transaction.Value = Value;
                _transaction.Description = Description;

                try
                {
                    transactionRepository.Update(_transaction);
                    await GoLastPage("Registro de debito atualizado com sucesso!");
                }
                catch
                {
                    Toast("Ocorreu uma falha ao atualizar o débito.");
                }
            }
        }

        private void GetAllCategories()
        {
            using (var categoryRepository = new CategoryRepository())
            {
                Categories = new ObservableCollection<Category>(categoryRepository.GetAll());
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            GetAllCategories();

            if (parameters.ContainsKey("transactionId") && parameters.ContainsKey("expenseControlId"))
            {
                using (var transactionRepository = new TransactionRepository())
                {
                    int transactionId = (int)parameters["transactionId"];
                    _transaction = transactionRepository.GetById(transactionId);
                    
                    Value = _transaction.Value;
                    SelectedCategory = Categories.Single(c => c.CategoryId == _transaction.CategoryId);
                    Description = _transaction.Description;
                }
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            parameters.Add("expenseControlId", _transaction.ExpenseControlId);
            base.OnNavigatedFrom(parameters);
        }

        private async Task GoLastPage(string message)
        {
            var parameter = new NavigationParameters();
            parameter.Add("expenseControlId", _transaction.ExpenseControl.ExpenseControlId);
            parameter.Add("message", message);
            await GoBack(parameter);
        }
    }
}
