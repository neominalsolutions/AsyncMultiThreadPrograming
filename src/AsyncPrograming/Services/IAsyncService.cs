namespace AsyncPrograming.Services
{
  public class Product
  {
    public string Name { get; set; }

  }

  public interface IAsyncService
  {
    Task<Product> HandleAsync(CancellationToken cancellationToken);
  }

  public class AsyncServiceSample : IAsyncService
  {
    public async Task<Product> HandleAsync(CancellationToken cancellationToken)
    {

      int a = 3;
      int b = 0;

      var task = Task.Run(() => a / b);

      await Task.WhenAll(task);

      // task.Wait();
      // Task.WaitAll(task); // waitAll ile tüm taskların bitmesini bekletip ona göre failed bir state düşersek bu durumda exception döndürebildik.

      if (task.Status == TaskStatus.Faulted)
      {
        return await Task.FromException<Product>(new Exception("Hata"));
      }
      else
      {
        return await Task.FromResult(new Product { Name = "Ürün-1" });
      }



    }
  }
}
