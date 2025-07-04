namespace SoapWebApp.Services
{
    public class FibonacciService : Interfaces.IFibonacciService
    {
        public async Task<int> CalculateFibonacciAsync(int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), "Input must be a non-negative integer.");
            }

            //await Task.Delay(TimeSpan.FromMinutes(1)); // Simulate a delay for the asynchronous operation

            return await Task.Run(() => Fibonacci(n));
        }
        private int Fibonacci(int n)
        {
            if (n <= 1)
            {
                return n;
            }
            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }
    }
}
