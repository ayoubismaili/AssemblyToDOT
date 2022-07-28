// See https://aka.ms/new-console-template for more information

using AssemblyToDOT;

Function func = new Function(@"C:\Users\Sphere\Desktop\sub_2E3C90.asm");
DOTBuilder dotBuilder = new DOTBuilder();
dotBuilder.AddFunction(func);
string result = dotBuilder.Build();
//string result = func.ToString();
Console.WriteLine(result);

Console.ReadLine();


