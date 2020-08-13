using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Drawing;
using Console = Colorful.Console;

namespace Threading.Channels.Demo
{
  public class Consumer
  {
    private readonly ChannelReader<string> _reader;
    private readonly int _identifier;
    private readonly int _delay;

    public Consumer(int identifier, ChannelReader<string> reader, int delay)
    {
      _reader = reader;
      _identifier = identifier;
      _delay = delay;
    }

    public async Task ConsumeMessages()
    {
      await foreach (var item in _reader.ReadAllAsync())
      {
        await Task.Delay(_delay);
        Console.WriteLine($"Consumer ({_identifier}): Consuming => {item}", Color.MediumSpringGreen);
      }

      Console.WriteLine($"Consumer ({_identifier}): Completed", Color.Gray);
    }
  }
}
