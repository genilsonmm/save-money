using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SaveMoney.Util;
using System;
using System.Threading.Tasks;

namespace SaveMoney.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService _navigationService { get; private set; }
        protected IPageDialogService _pageDialogService { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

        public ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService = null)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
            IsVisible = false;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {
            
        }

        public void Toast(string message)
        {
            Xamarin.Forms.DependencyService.Get<IToastMessage>().ShortAlert(message);
        }

        /// <summary>
        /// Navigate tem como função navegar entre as páginas sem que seja necessário
        /// utilizar os nomes das páginas como string
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameters"></param>
        public async void Navigate<TViewModel>(NavigationParameters parameters = null)
            where TViewModel : ViewModelBase
        {
            var viewModelType = typeof(TViewModel);
            string viewModelName = viewModelType.Name;
            string view = viewModelName.Substring(0, viewModelName.Length - "ViewModel".Length);
            await _navigationService.NavigateAsync(view, parameters, false, true);
        }
    
        public async Task GoBack(NavigationParameters parameters = null)
        {
            await _navigationService.GoBackAsync(parameters, false, true);
        } 

        public Task<bool> ShowDialog(string title, string message)
        {
            return _pageDialogService.DisplayAlertAsync(title, message, "Ok", "Cancelar");
        }

        private string GetClassName(Type view)
        {
            string className = view.Name;
            return className.Substring(0, className.Length - "ViewModel".Length);
        }
    }
}
