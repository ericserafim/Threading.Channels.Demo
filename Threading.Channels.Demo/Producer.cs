using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Drawing;
using Console = Colorful.Console;

namespace Threading.Channels.Demo
{
  public class Producer
  {
    private readonly ChannelWriter<string> _writer;
    private readonly int _identifier;
    private readonly int _delay;

    public Producer(int identifier, ChannelWriter<string> writer, int delay)
    {
      _writer = writer;
      _identifier = identifier;
      _delay = delay;
    }

    public async Task StartProducing()
    {
      for (var i = 0; i < 10; i++)
      {
        await Task.Delay(_delay);
        var message = $"Producer {_identifier}, {DateTime.UtcNow:G}";
        Console.WriteLine($"Producer ({_identifier}): Creating => {message}", Color.Yellow);

        await _writer.WriteAsync(message);
      }

      Console.WriteLine($"Producer ({_identifier}): Completed", Color.Gray);
    }
  }
}
