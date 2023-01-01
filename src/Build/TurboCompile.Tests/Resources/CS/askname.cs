Console.WriteLine(Environment.NewLine + "What is your name? ");
var name = Console.ReadLine();
var currentDate = DateTime.Now;
Console.WriteLine($"{Environment.NewLine}Hello, {name}, at {currentDate}!");
Console.Write(Environment.NewLine + "Press any key to exit... ");
Console.Read();
