using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SaveMoney.Data;
using SaveMoney.Model;
using SaveMoney.Util;
using System;
using System.Threading.Tasks;

namespace SaveMoney.ViewModels
{
    public class NewAndEditExpenseControlViewModel : ViewModelBase
    {
        private readonly string NewExpenseControl = "Cadastro de novo controle de gasto";
        private readonly string EditExpenseControl = "Editar controle de gasto";
        private bool isEditing = false;
        private ExpenseControl expenseControl;

        private string _pageTitle;
        public string PageTitle { get { return _pageTitle; }set { SetProperty(ref _pageTitle, value); }}

        private string _saveButtonName;
        public string SaveButtonName { get { return _saveButtonName; } set { SetProperty(ref _saveButtonName, value); } }

        private string _title;
        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }


        private string _value;
        public string Value { get { return CurrencyConverter.Instance().Converter(_value); } set { SetProperty(ref _value, value); } }

        private DateTime _selectedDate;
        public DateTime SelectedDate {get {
                return _selectedDate; }set {
                SetProperty(ref _selectedDate, value); } }

        private string _currentDate;
        public string CurrentDate { get { return _currentDate; } set { SetProperty(ref _currentDate, value); } }

        public DelegateCommand RegisterCommand { get; private set; }

        public NewAndEditExpenseControlViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
            Value = "0,00";
            CurrentDate = string.Empty;
            SelectedDate = DateTime.Now;
            SaveButtonName = string.Empty;

            RegisterCommand = new DelegateCommand(RegisterOrUpdateExpenseControl, CanRegisterOrUpdate)
                .ObservesProperty(() => Title)
                .ObservesProperty(() => Value)
                .ObservesProperty(() => SelectedDate);
        }

        private bool CanRegisterOrUpdate()
        {
            decimal decimalValue = DecimalConverter.Instance().Converter(Value);

            if (isEditing)
            {
                if (decimalValue > 0 && SelectedDate.Year >= expenseControl.BeginYear &&
                    SelectedDate.Month >= expenseControl.BeginMonth && SelectedDate.Day >= expenseControl.BeginDay
                    && !string.IsNullOrEmpty(Title))
                    return true;
            }
            else
            {
                if (decimalValue > 0 && SelectedDate.Year >= DateTime.Now.Year &&
                    SelectedDate.Month >= DateTime.Now.Month && SelectedDate.Day >= DateTime.Now.Day && !string.IsNullOrEmpty(Title))
                    return true;
            }
            return false;
        }

        private async void RegisterOrUpdateExpenseControl()
        {
            if(isEditing)
            {
                await UpdateExpenseControl();
            }
            else
            {
                await RegisterNewExpenseControl();
            }
        }

        private async Task UpdateExpenseControl()
        {
            bool confirm = await ShowDialog("Atualizar", "Confirmar edição?");
            if (confirm)
            {
                var parameter = new NavigationParameters();

                if (UpdateExpenseControl(_selectedDate, _value))
                {
                    parameter.Add("message", "Controle de gasto atualizado com sucesso!");
                    await GoBack(parameter);
                }
                else
                {
                    Toast("Falha durante a atualização do controle de gasto!");
                }

            }
        }

        private async Task RegisterNewExpenseControl()
        {
            bool confirm = await ShowDialog("Salvar", "Confirmar cadastro?");
            if (confirm)
            {
                var parameter = new NavigationParameters();

                if (AddExpenseControl(_selectedDate, _value))
                {
                    
                    parameter.Add("message", "Novo controle de gasto cadastrado com sucesso!");
                    await GoBack(parameter);
                }
                else
                {
                    Toast("Falha durante o cadastro do controle de gasto!");
                }

            }
        }

        private bool AddExpenseControl(DateTime endDate, string value)
        {
            using (var expenseControlRepository = new ExpenseControlRepository())
            {
                try
                {
                    expenseControlRepository.Add(new ExpenseControl()
                    {
                        Title = Title,
                        BeginYear = DateTime.Now.Year,
                        BeginMonth = DateTime.Now.Month,
                        BeginDay = DateTime.Now.Day,
                        EndYear = endDate.Year,
                        EndMonth = endDate.Month,
                        EndDay = endDate.Day,
                        TotalValue = value
                    });

                    return true;
                }
                catch (Exception error)
                {
                    return false;
                }
            }
        }

        private bool UpdateExpenseControl(DateTime endDate, string value)
        {
            using (var expenseControlRepository = new ExpenseControlRepository())
            {
                try
                {
                    expenseControl.TotalValue = value;
                    expenseControl.EndYear = endDate.Year;
                    expenseControl.EndMonth = endDate.Month;
                    expenseControl.EndDay = endDate.Day;
                    expenseControl.Title = Title;

                    expenseControlRepository.Update(expenseControl);
                    expenseControl = expenseControlRepository.GetById(expenseControl.ExpenseControlId, true);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if(parameters.ContainsKey("isEdit") && (bool)parameters["isEdit"])
            {
                isEditing = true;
                expenseControl = (ExpenseControl)parameters["expenseControl"];

                CurrentDate = expenseControl.BeginDateFormatted;
                PageTitle = EditExpenseControl;
                SaveButtonName = "Salvar";
                Value = expenseControl.TotalValue;
                Title = expenseControl.Title;
                SelectedDate = new DateTime(expenseControl.EndYear, expenseControl.EndMonth, expenseControl.EndDay);

                RegisterCommand.RaiseCanExecuteChanged();
            }
            else
            {
                SelectedDate = DateTime.Now.AddDays(2);
                PageTitle = NewExpenseControl;
                SaveButtonName = "Cadastrar";
                CurrentDate = DateTime.Now.ToShortDateString();
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            if (isEditing)
            {
                parameters.Add("expenseControlId", expenseControl.ExpenseControlId);
                base.OnNavigatedFrom(parameters);
            }
        }
    }
}
