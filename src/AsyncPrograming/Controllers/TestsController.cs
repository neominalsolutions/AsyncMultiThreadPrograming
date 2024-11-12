using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AsyncPrograming.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestsController : ControllerBase
  {
    private HttpClient client = new HttpClient();

    [HttpGet("sync")]
    public IActionResult SyncRequest()
    {
      // Sync Programalama Main Thread ile aynı Thread kullandı

      Thread.Sleep(3000);
      Console.Out.WriteLine($"Sync Thread {Thread.CurrentThread.ManagedThreadId} \n");

      return Ok();
    }

    [HttpGet("async")]
    public async Task<IActionResult> AsyncRequest()
    {
      // asenkron programalama Main Thread veya bunun yerine farklı bir Thread kullandı. Fakat Sync den farkı Main Thread bloke etmeden bu işlemi yaptı.

      await Task.Delay(3000);
      await Console.Out.WriteLineAsync($"Async Thread {Thread.CurrentThread.ManagedThreadId}");

      return Ok();
    }


    [HttpGet("taskRun")]
    public async Task<IActionResult> TaskRun()
    {
      // Task Run ile senkron yazılmış bir kod bloğu non blocking olarak async şekilde çalıştırılabilir.

      // Dosya açma ve kalsöre kaydetme kodu
      // Arkaplanda veri tabanına kodu yazdırma olabilir.
      // Email Gönderme
      Action<string> action = value =>
      {
        Thread.Sleep(5000);
        Console.Out.WriteLineAsync($"Thread Task Run {Thread.CurrentThread.ManagedThreadId}");
      };


      Task.Run(() => action("message"));

      return Ok();

    }

    // await ile result arasındaki fark
    [HttpGet("results")]
    public IActionResult ResultVsAwait()
    {
      // Asenkron kod bloğunun arkasına yazılan Result değerinden dolayı kod bloke olur.

      using (HttpClient client = new HttpClient())
      {
        //var data = client.GetStringAsync("https://www.google.com").Result;
        var data = client.GetStringAsync("https://www.google.com").GetAwaiter().GetResult();
        Console.Out.WriteLine($"Dosya Boyutu {data.Length}");
        Console.Out.WriteLineAsync($"ThreadId {Thread.CurrentThread.ManagedThreadId}");
      }


      return Ok();
    }

    // await ile result arasındaki fark
    [HttpGet("await")]
    public async Task<IActionResult> Await()
    {
      // Not: Asenkron bir kod bloğu ile çalışıyorsak sonucu alırken non-blocking olması için await keyword kullanmamız gerekiyor.

      // Not: HttpClient aşağıdaki kod bloğunda await ile kod bekletilip sıralı bir işlem olmadığından dolayı using kod bloğu hata verirken, bu örnek de kod asenkron olarak sıralı bir şekilde tanımlandığından herhangi bir client hatası meydana gelmedi.
      using (HttpClient _client = new HttpClient())
      {

        var data = await _client.GetStringAsync("https://www.google.com");
        var data2 = await _client.GetStringAsync("https://neominal.com");

        await Console.Out.WriteLineAsync($"Neominal Boyutu {data2.Length}");
        await Console.Out.WriteLineAsync($"Google Boyutu {data.Length}");



        await Console.Out.WriteLineAsync($"Await {Thread.CurrentThread.ManagedThreadId}");
      }


      return Ok();
    }


    // await ile result arasındaki fark
    [HttpGet("continueWith")]
    public IActionResult ContinueWith()
    {
      // Not: Asenkron bir kod bloğu ile çalışıyorsak sonucu alırken non-blocking olması için await keyword kullanmamız gerekiyor.

      // ContinueWith ile birbirinden bağımsız asenkron kod bloklarının verilerinin çözümlendiği anı yakalayabiliriz.

      client.GetStringAsync("https://www.google.com").ContinueWith(async (data2) =>
      {
        await Console.Out.WriteLineAsync($"Google Boyutu {data2.Result.Length}");
      });


      client.GetStringAsync("https://www.neominal.com").ContinueWith(async (data1) =>
       {
         await Console.Out.WriteLineAsync($"Neominal Boyutu {data1.Result.Length}");

       });


      // Senkron Kod blogu

      Console.Out.WriteLine($"ContinueWith {Thread.CurrentThread.ManagedThreadId}");



      return Ok();
    }




  }
}
