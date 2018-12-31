using Prism.Commands;
using Prism.Navigation;
using SaveMoney.Data;
using SaveMoney.Model;
using SaveMoney.Util;
using System;
using System.Collections.ObjectModel;

namespace SaveMoney.ViewModels
{
    public class NewExpenseViewModel : ViewModelBase
    {
        private ExpenseControl _expenseControl;
        public string CurrentDateTime => DateTime.Now.ToLongDateString();

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

        public DelegateCommand RegisterCommand { get; private set; }

        public NewExpenseViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Value = "0,00";
            RegisterCommand = new DelegateCommand(RegisterNewDebit, CanRegisterNewDebit)
                                    .ObservesProperty(() => Value)
                                    .ObservesProperty(() => SelectedCategory);
            AddCategory();
            GetAllCategories();
        }

        private bool CanRegisterNewDebit()
        {
            decimal decimalValue = DecimalConverter.Instance().Converter(Value);

            if (_expenseControl != null)
            {
                if (decimalValue > DecimalConverter.Instance().Converter(_expenseControl.AvailableValue))
                {
                    Toast($"Saldo insuficiente");
                    return false;
                }
            }

            if (decimalValue > 0 && SelectedCategory != null)
                return true;

            return false;
        }

        private async void RegisterNewDebit()
        {
            using (var transactionRepository = new TransactionRepository())
            {
                Transaction transaction = new Transaction();
                transaction.ExpenseControlId = _expenseControl.ExpenseControlId;
                transaction.Day = DateTime.Now.Day;
                transaction.Month = DateTime.Now.Month;
                transaction.Year = DateTime.Now.Year;
                transaction.Hour = DateTime.Now.Hour;
                transaction.Minutes = DateTime.Now.Minute;
                transaction.Seconds = DateTime.Now.Second;
                transaction.TransactionType = Enums.TransactionType.Debit;
                transaction.CategoryId = SelectedCategory.CategoryId;
                transaction.Value = Value;
                transaction.Description = Description;

                try
                {
                    transactionRepository.Add(transaction);

                    var parameter = new NavigationParameters();
                    parameter.Add("expenseControlId", _expenseControl.ExpenseControlId);
                    parameter.Add("message", "Registro de debito realizado com sucesso!");
                    await GoBack(parameter);
                }
                catch
                {
                    Toast("Ocorreu uma falha ao cadastrar novo débito.");
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
            if (parameters.ContainsKey("expenseControl"))
            {
                _expenseControl = parameters["expenseControl"] as ExpenseControl;
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            parameters.Add("expenseControlId", _expenseControl.ExpenseControlId);
            base.OnNavigatedFrom(parameters);
        }

        //TODO
        //Remover
        private void AddCategory()
        {

            try
            {
                using (var categoryRepository = new CategoryRepository())
                {
                    categoryRepository.Add(new Category() { Name = "Lanche", Image="spend.png" });
                    categoryRepository.Add(new Category() { Name = "Jantar", Image = "spend.png" });
                    categoryRepository.Add(new Category() { Name = "Almoço", Image = "spend.png" });
                    categoryRepository.Add(new Category() { Name = "Estacionamento", Image = "spend.png" });
                    categoryRepository.Add(new Category() { Name = "Gasolina", Image = "spend.png" });
                    categoryRepository.Add(new Category() { Name = "Cinema", Image = "spend.png" });
                    categoryRepository.Add(new Category() { Name = "Presente", Image = "spend.png" });
                    categoryRepository.Add(new Category() { Name = "Insumos de casa", Image = "spend.png" });
                    categoryRepository.Add(new Category() { Name = "Farmácia", Image = "spend.png" });
                    categoryRepository.Add(new Category() { Name = "Café", Image = "spend.png" });
                }
            }
            catch
            {

            }
        }

    }
}
