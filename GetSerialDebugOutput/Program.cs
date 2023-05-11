using System.IO.Ports;
using System.Reflection;

var version = Assembly.GetEntryAssembly()?.GetName().Version;
Console.WriteLine($"GetSerialDebugOutput {version?.ToString(3)}");

var portName = "COM4";
if (args.Length >= 1)
{
  portName = args[0];
}

Console.WriteLine($"Starting with {portName}");

var _serialPort = new SerialPort(portName)
{
  BaudRate = 115200,
  Parity = Parity.None,
  DataBits = 8,
  StopBits = StopBits.One,
  Handshake = Handshake.XOnXOff
};

_serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

_serialPort.Open();

Console.WriteLine("Press key to exit");

Console.ReadKey();
_serialPort.Close();

Console.WriteLine("\nFinished");

static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
{
  if (sender is not SerialPort sp)
  {
    Console.Error.WriteLine("sender in DataReceivedHandler is null");
    return;
  }

  var buffer = new byte[2048];

  using var fileStream = new FileStream("stream.bin", FileMode.Append, FileAccess.Write, FileShare.None);
  using var bw = new BinaryWriter(fileStream);
  while (sp.BytesToRead > 0)
  {
    int read = sp.Read(buffer, 0, buffer.Length);
    bw.Write(buffer, 0, read);

    var text = System.Text.Encoding.ASCII.GetString(buffer, 0, read);
    Console.Write(text);
  }

}
