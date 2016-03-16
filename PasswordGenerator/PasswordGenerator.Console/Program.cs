using PasswordGenerator.Core;

namespace PasswordGenerator.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                System.Console.WriteLine("请输入关键字：");
                var word = System.Console.ReadLine();
                System.Console.WriteLine("请输入长度：");
                int length = int.Parse(System.Console.ReadLine());

                //大写2位，小写2位，数字2位，符号2位，符号对应12位
                var modeState = "11001100111111111111";//大写必选,小写可选,数字必选,符号可选,子符号都可选
                modeState = Generator.ModeStateOctToHex(modeState);

                System.Console.WriteLine($"Key:{modeState}");

                var result = Generator.Generate(word, length, modeState);

                System.Console.WriteLine($"结果为：{result}");
            }
        }
    }
}
