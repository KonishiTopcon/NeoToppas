using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Template.WPF.ViewModels;

namespace Template.WPF.Services
{
    public class TransitionService
    {
        private readonly IServiceProvider _serviceProvider;

        private Action<object> _setContentAction;

        public event EventHandler<string> Navigated;

        public TransitionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Initialize(Action<object> setContentAction)
        {
            _setContentAction = setContentAction ?? throw new ArgumentNullException(nameof(setContentAction));
        }

        public bool NavigateTo<TView>(object parameter = null) where TView : FrameworkElement
        {
            // TViewのインスタンスをDIコンテナから取得
            var view = _serviceProvider.GetRequiredService<TView>();

            if (view == null)
            {
                throw new InvalidOperationException($"The requested view of type {typeof(TView).Name} could not be found. Please ensure it is properly registered in the service container.");
            }

            if (_setContentAction == null)
            {
                throw new InvalidOperationException("NavigationService is not initialized. Please call Initialize() with a valid action.");
            }

            // パラメータが存在する場合、ViewまたはそのDataContextが INavigationAware を実装しているか確認
            if (parameter != null)
            {
                if (view is ITransitionAware navigationAwareView)
                {
                    navigationAwareView.OnNavigatedTo(parameter);
                }
                else if (view.DataContext is ITransitionAware navigationAwareViewModel)
                {
                    navigationAwareViewModel.OnNavigatedTo(parameter);
                }
            }

            // ReactiveProperty経由でContentを更新
            _setContentAction.Invoke(view);

            // 遷移完了の通知を行う
            OnNavigated(view);

            return true;
        }

        public bool NavigateTo(object page, object parameter = null)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (_setContentAction == null)
            {
                throw new InvalidOperationException("NavigationService is not initialized. Please call Initialize() with a valid action.");
            }

            // パラメータが存在する場合の処理
            if (parameter != null)
            {
                if (page is ITransitionAware navigationAwarePage)
                {
                    navigationAwarePage.OnNavigatedTo(parameter);
                }
                else if (page is FrameworkElement frameworkElement && frameworkElement.DataContext is ITransitionAware navigationAwareViewModel)
                {
                    navigationAwareViewModel.OnNavigatedTo(parameter);
                }
            }

            // ReactiveProperty経由でContentを更新
            _setContentAction.Invoke(page);

            // 遷移完了の通知を行う
            OnNavigated(page);

            return true;
        }

        private void OnNavigated(object content)
        {
            if (content != null)
            {
                Navigated?.Invoke(this, content.GetType().FullName);
            }
        }

        public object GetView<TView>()
        {
            return _serviceProvider.GetRequiredService(typeof(TView));
        }
    }

}