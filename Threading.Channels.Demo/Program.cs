using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Threading.Channels.Demo
{
  class Program
  {
    static async Task Main(string[] args)
    {
      ConsoleKeyInfo key;

      do
      {
        Console.ResetColor();
        Console.WriteLine("Which one?");
        Console.WriteLine("1. Single producer, single consumer");
        Console.WriteLine("2. Multiple producers, single consumer");
        Console.WriteLine("3. Single producer, multiple consumers");
        Console.WriteLine("4. Multiple producers, multiple consumers");
        Console.WriteLine("9. to exit");

        key = Console.ReadKey();
        Console.WriteLine(string.Empty);

        switch (key.KeyChar)
        {
          case '1':
            await SingleProducerSingleConsumer();
            break;

          case '2':
            await MultipleProducersSingleConsumer();
            break;

          case '3':
            await SingleProducerMultipleConsumers();
            break;

          case '4':
            await MultipleProducersMultipleConsumers();
            break;

          default:            
            break;
        }
      } while (key.KeyChar != '9');

    }

    private static async Task SingleProducerSingleConsumer()
    {
      var channel = Channel.CreateUnbounded<string>(
        new UnboundedChannelOptions
        {
          SingleWriter = true,
          SingleReader = true
        });

      var producer1 = new Producer(1, channel.Writer, 1000);
      var consumer1 = new Consumer(1, channel.Reader, 500);

      var consumerTask1 = consumer1.ConsumeMessages();
      var producerTask1 = producer1.StartProducing();

      await producerTask1.ContinueWith(_ => channel.Writer.Complete());
      await consumerTask1;
    }

    private static async Task MultipleProducersSingleConsumer()
    {
      var channel = Channel.CreateBounded<string>(
        new BoundedChannelOptions(2)
        {
          SingleWriter = false,
          SingleReader = true
        });

      var producer1 = new Producer(1, channel.Writer, 1500);
      var producer2 = new Producer(2, channel.Writer, 1500);
      var consumer1 = new Consumer(1, channel.Reader, 1000);

      var consumerTask1 = consumer1.ConsumeMessages();

      var producerTask1 = producer1.StartProducing();

      //Just to have some kind of mix beetwen producers
      await Task.Delay(500);

      var producerTask2 = producer2.StartProducing();

      await Task.WhenAll(producerTask1, producerTask2)
                .ContinueWith(_ => channel.Writer.Complete());

      await consumerTask1;
    }

    public static async Task SingleProducerMultipleConsumers()
    {
      var channel = Channel.CreateUnbounded<string>(
        new UnboundedChannelOptions
        {
          SingleWriter = true,
          SingleReader = false
        });

      var producer1 = new Producer(1, channel.Writer, 100);
      var consumer1 = new Consumer(1, channel.Reader, 1500);
      var consumer2 = new Consumer(2, channel.Reader, 1500);
      var consumer3 = new Consumer(3, channel.Reader, 1500);

      Task consumerTask1 = consumer1.ConsumeMessages();
      Task consumerTask2 = consumer2.ConsumeMessages();
      Task consumerTask3 = consumer3.ConsumeMessages();

      Task producerTask1 = producer1.StartProducing();

      await producerTask1.ContinueWith(_ => channel.Writer.Complete());

      await Task.WhenAll(consumerTask1, consumerTask2, consumerTask3);
    }

    public static async Task MultipleProducersMultipleConsumers()
    {
      var channel = Channel.CreateUnbounded<string>(
        new UnboundedChannelOptions
        {
          SingleWriter = false,
          SingleReader = false
        });

      var producer1 = new Producer(1, channel.Writer, 100);
      var producer2 = new Producer(2, channel.Writer, 100);
      var producer3 = new Producer(3, channel.Writer, 100);

      var consumer1 = new Consumer(1, channel.Reader, 200);
      var consumer2 = new Consumer(2, channel.Reader, 200);
      var consumer3 = new Consumer(3, channel.Reader, 200);

      Task consumerTask1 = consumer1.ConsumeMessages();
      Task consumerTask2 = consumer2.ConsumeMessages();
      Task consumerTask3 = consumer3.ConsumeMessages();

      Task producerTask1 = producer1.StartProducing();

      //Just to have some kind of mix beetwen producers
      await Task.Delay(300);
      Task producerTask2 = producer2.StartProducing();

      //Just to have some kind of mix beetwen producers
      await Task.Delay(100);
      Task producerTask3 = producer3.StartProducing();

      await Task.WhenAll(producerTask1, producerTask2, producerTask3)
                .ContinueWith(_ => channel.Writer.Complete());

      await Task.WhenAll(consumerTask1, consumerTask2, consumerTask3);
    }
  }
}
