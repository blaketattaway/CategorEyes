namespace OneCore.CategorEyes.Client.Services
{
    public class LoadingService
    {
        public event Action<bool> OnLoadingChanged;

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value) return;

                _isLoading = value;
                OnLoadingChanged?.Invoke(_isLoading);
            }
        }

        public void StartLoading() => IsLoading = true;

        public void StopLoading() => IsLoading = false;
    }
}
