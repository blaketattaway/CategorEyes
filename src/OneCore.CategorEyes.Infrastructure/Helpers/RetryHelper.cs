using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Infrastructure.Helpers
{
    internal static class RetryHelper
    {
        private static readonly int _totalAttempts;

        static RetryHelper()
        {
            if (!int.TryParse(ConfigurationHelper.Value("maxAttempts"), out _totalAttempts))
            {
                _totalAttempts = 3;
            }
        }

        /// <summary>
        /// Ejecuta una tarea asíncrona que devuelve un tipo de objeto hasta N veces y si falla devuelve una excepción
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
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
        /// Ejecuta una tarea asíncrona hasta N veces y si falla devuelve una excepción
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
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
