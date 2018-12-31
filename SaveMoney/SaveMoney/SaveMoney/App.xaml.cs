using Prism;
using Prism.Ioc;
using SaveMoney.ViewModels;
using SaveMoney.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SaveMoney
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null)
        {

        }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            // Initialize Live Reload.
            #if DEBUG
            LiveReload.Init();
            #endif
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/SelectExpenseControl");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<SelectExpenseControl>();
            containerRegistry.RegisterForNavigation<NewAndEditExpenseControl>();
            containerRegistry.RegisterForNavigation<NewExpense, NewExpenseViewModel>();
            containerRegistry.RegisterForNavigation<ExpenseControlDetails, ExpenseControlDetailsViewModel>();
            containerRegistry.RegisterForNavigation<EditExpense, EditExpenseViewModel>();
        }
    }
}
