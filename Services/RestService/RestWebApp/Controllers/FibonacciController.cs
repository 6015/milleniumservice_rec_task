using Microsoft.AspNetCore.Mvc;

namespace RestWebApp.Controllers
{
    [Route("api/fibonacci")]
    public class FibonacciController : ControllerBase
    {
        private static int _counter = 0;

        [HttpGet("{n}")]
        public IActionResult Get(int n)
        {
            if (n < 0)
            {
                return BadRequest("Input must be a non-negative integer.");
            }

            int currentCount = Interlocked.Increment(ref _counter);

            if(currentCount % 10 == 0)
            {
                return StatusCode(500, "Simulated error.");
            }

            long result = Fibonacci(n);
            return Ok(result);
        }

        private int Fibonacci(int n)
        {
            if (n == 0) return 0;
            if (n == 1) return 1;
            int a = 0, b = 1, c = 0;
            for (int i = 2; i <= n; i++)
            {
                c = a + b;
                a = b;
                b = c;
            }
            return c;
        }
    }
}
