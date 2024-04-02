namespace OneCore.CategorEyes.Infrastructure.Helpers
{
    internal static class RetryHelper
    {
        private static readonly int _totalAttempts;

        /// <summary>
        /// Initializes the `RetryHelper` class by setting the total number of retry attempts from a configuration, with a default value of 3 attempts if the configuration is unavailable or invalid.
        /// </summary>
        static RetryHelper()
        {
            if (!int.TryParse(ConfigurationHelper.Value("maxAttempts"), out _totalAttempts))
            {
                _totalAttempts = 3;
            }
        }

        /// <summary>
        /// Executes an asynchronous task with a return value up to a maximum number of attempts. In case of an exception, it retries executing the task until the attempt limit is reached, after which, the exception is thrown.
        /// </summary>
        /// <typeparam name="T">The type of object the task returns.</typeparam>
        /// <param name="method">The asynchronous task to be executed, represented as a <see cref="Func{Task{T}}"/>.</param>
        /// <returns>The value from the task of type <see cref="T"/> after successful execution.</returns>
        /// <exception cref="Exception">The caught exception if the maximum number of attempts is exceeded.</exception>
        public async static Task<T> Execute<T>(Func<Task<T>> method)
        {
            int currentRetry = 1;

            T value;

            for (; ; )
            {
                try
                {
                    value = await method();
                    break;
                }
                catch (Exception)
                {
                    currentRetry++;
                    if (currentRetry > _totalAttempts)
                    {
                        throw;
                    }
                }
                Thread.Sleep(50);
            }

            return value;
        }

        /// <summary>
        /// Executes an asynchronous task without a return value up to a maximum number of attempts. In case of an exception, it retries executing the task until the attempt limit is reached, after which, the exception is thrown.
        /// </summary>
        /// <param name="method">The asynchronous task to be executed, represented as a <see cref="Func{Task}"/>.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">The caught exception if the maximum number of attempts is exceeded.</exception>
        public async static Task Execute(Func<Task> method)
        {
            int currentRetry = 1;

            for (; ; )
            {
                try
                {
                    await method();
                    break;
                }
                catch (Exception)
                {
                    currentRetry++;
                    if (currentRetry > _totalAttempts)
                    {
                        throw;
                    }
                }
                Thread.Sleep(50);
            }
        }
    }
}
