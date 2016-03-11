using PasswordGenerator.Core;

namespace PasswordGenerator.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("请输入关键字：");
            var word = System.Console.ReadLine();
            System.Console.WriteLine("请输入长度：");
            int length = int.Parse(System.Console.ReadLine());

            //大写2位，小写2位，数字2位，符号2位，符号对应12位
            var modeState = "11001100111111111111";

            System.Console.WriteLine($"Key:{Generator.ModeState2To16(modeState)}");

            var result = Generator.Generate(word, length, modeState);

            System.Console.WriteLine($"结果为：{result}");
        }
    }
}
