namespace OneCore.CategorEyes.Client.Services
{
    public class LoadingService
    {
        /// <summary>
        /// Occurs when the loading state changes. Listeners can use this event to react to changes in the loading state, such as showing or hiding a loading indicator.
        /// </summary>
        public event Action<bool> OnLoadingChanged;

        private bool _isLoading;


        /// <summary>
        /// Gets or sets a value indicating whether the loading indicator should be displayed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the loading indicator should be displayed; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Sets the loading state to <c>true</c>, indicating that a loading process has started and a loading indicator may be displayed.
        /// </summary>
        public void StartLoading() => IsLoading = true;

        /// <summary>
        /// Sets the loading state to <c>false</c>, indicating that loading has completed and any loading indicators can be hidden.
        /// </summary>
        public void StopLoading() => IsLoading = false;
    }
}
