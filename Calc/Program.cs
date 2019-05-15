using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * 電卓
             */

            var TupleInNumOrSymbol = GetNumAndSymbolFromConsoleReadLineSplit();
            String[] InNumOrSymbols = TupleInNumOrSymbol.Item1;
            int InNumOrSymbolCount = TupleInNumOrSymbol.Item2;

            Stack<string> ResultMultiplicationAndDivisionStack = CalculateMultiplicationAndDivision(InNumOrSymbols, InNumOrSymbolCount);
            Stack<string> ResultOrganizedTermStack = OrganizeTerm(ResultMultiplicationAndDivisionStack);
            float CalculationResultStack = CalculateAdditionalAndSubtract(ResultOrganizedTermStack);

            Console.WriteLine(CalculationResultStack);
            Console.WriteLine("Please enter to exit");
            Console.ReadKey();
        }

        static Tuple<String[], int> GetNumAndSymbolFromConsoleReadLineSplit()
        {
            /*
             * 標準入力で、空白スペースで区切られた文字を分割し配列に格納する。
             *
             * <returns> InNumOrSymbol
             *      String[]
             *      文字または記号
             * <returns> InNumOrSymbolCount
             *      int
             *      文字と記号の個数
             */

            String in_str = Console.ReadLine();
            // String in_str = "6 * -5 - -4";

            String[] InNumOrSynbol = in_str.Split(' ');
            int InNumOrSymbolCount = InNumOrSynbol.Length;

            return new Tuple<String[], int>(InNumOrSynbol, InNumOrSymbolCount);
        }

        static Stack<string> CalculateMultiplicationAndDivision(String[] InNumOrSymbols, int InNumOrSymbolCount)
        {
            /*
             * 配列で受け取った記号・数字を基に、乗算/除算を行う。
             * 計算結果、数字・記号はスタックにプッシュされる。
             *
             * <param> InNumOrSymbols
             *      String[]
             *      文字または記号
             * <param> InNumOrSymbolCount
             *      int
             *      文字と記号の個数
             * <return> NumAndOperatorStack
             *      Stack<string>
             *      乗算/除算を行った結果、加算/減算が残ったスタック
             * <example>
             *      <param>
             *      InNumOrSymbols = {"1", "+", "2", "*", "3", "-", "4"}
             *      InNumOrSymbolCount = 7
             *      <return>
             *      NumAndOperatorStack = <"1", "+", "6", "-", "4">
             */

            var NumAndOperatorStack = new Stack<string>();
            try
            {
                for (int i = 0; i < InNumOrSymbolCount; i++)
                {
                    if ((InNumOrSymbols[i] == "*" || InNumOrSymbols[i] == "/") && i != 0)
                    {
                        String StrA = NumAndOperatorStack.Pop();
                        String StrB = InNumOrSymbols[i + 1];
                        String MyOperator = InNumOrSymbols[i];

                        String Temp = CalculateByOperator(StrA, StrB, MyOperator).ToString();
                        NumAndOperatorStack.Push(Temp);
                        i++;
                    }
                    else
                    {
                        NumAndOperatorStack.Push(InNumOrSymbols[i]);
                    }
                }

                return NumAndOperatorStack;
            }
            catch (IndexOutOfRangeException)
            {
                ErrorExit("ERROR (010) : Format is incorrect. ");
                return NumAndOperatorStack;
            }
        }

        static Stack<string> OrganizeTerm(Stack<string> InNumAndOperatorStack)
        {
            var OrganizedTermStack = new Stack<string>();
            int StackLength = InNumAndOperatorStack.Count();
            String TempNum;
            String TempOperator;
            int TempIntNum;

            for (int i=0; i<StackLength / 2 + 1; i++)
            {
                if (i < StackLength / 2)
                {
                    TempNum = InNumAndOperatorStack.Pop();
                    TempOperator = InNumAndOperatorStack.Pop();
                    if (TempOperator == "-")
                    {
                        TempIntNum = int.Parse(TempNum) * (-1);
                        TempNum = TempIntNum.ToString();
                    }
                    OrganizedTermStack.Push(TempNum);
                    OrganizedTermStack.Push("+");
                }
                else
                {
                    TempNum = InNumAndOperatorStack.Pop();
                    TempOperator = "";
                    OrganizedTermStack.Push(TempOperator + TempNum);
                }
            }
            return OrganizedTermStack;
        }

        static float CalculateAdditionalAndSubtract(Stack<string> InNumAndOperatorStack)
        {
            /*
             * 数字・記号が入ったスタックを受け取り、
             * 式の末尾から計算を行う。
             * WARNING:
             *      事前に乗算/除算を行っていないと、
             *      誤った計算結果となるので注意する。
             *
             * <param> InNumAndOperatorStack
             *      Stack<string>
             *      加算/減算の数字・記号が入ったスタック
             * <return>
             *      float
             *      計算結果
             * <example>
             *      <param>
             *      InNumAndOperatorStack = <"6", "+", "2", "-", "4">
             *      <return>
             *      4
             */

            int StackLength = InNumAndOperatorStack.Count();
            try
            {
                for (int i = 0; i < StackLength / 2; i++)
                {
                    String StrA = InNumAndOperatorStack.Pop();
                    String MyOperator = InNumAndOperatorStack.Pop();
                    String StrB = InNumAndOperatorStack.Pop();
                    String Temp = CalculateByOperator(StrA, StrB, MyOperator).ToString();
                    InNumAndOperatorStack.Push(Temp);
                }
                return float.Parse(InNumAndOperatorStack.Pop());
            }
            catch (InvalidOperationException)
            {
                ErrorExit("ERROR (020) : Input format is not nomal.");
                return 0;
            }
            catch (FormatException)
            {
                ErrorExit("ERROR (021) : Input format is not nomal.");
                return 0;
            }
        }

        static float CalculateByOperator(String InStrA, String InStrB, String InOperator)
        {
            /*
             * 文字列で数字を二つと記号を入力し、
             * floatに変換のうえ計算を行う。
             * NOTE:
             *      記号が文字列であり、数字の変換と個別に行うのが面倒なので
             *      この関数内で計算前に変換を行っている。
             *
             * <param> InStrA
             *      String
             *      生成する式の第一項
             * <param> InStrB
             *      String
             *      生成する式の第二項
             * <param>
             *      String
             *      式の記号
             * <return>
             *      float
             *      式の計算結果
             */
            float ReturnNum;

            float InNumA = DetermineWhetherNumber(InStrA);
            float InNumB = DetermineWhetherNumber(InStrB);

            switch (InOperator)
            {
                case "+":
                    ReturnNum = InNumA + InNumB;
                    break;

                case "-":
                    ReturnNum = InNumA - InNumB;
                    break;

                case "*":
                    ReturnNum = InNumA * InNumB;
                    break;

                case "/":
                    ReturnNum = InNumA / InNumB;
                    break;

                default:
                    ErrorExit("ERROR (030) : Operator is not nomal.");
                    return 0;
            }

            return ReturnNum;
        }

        static float DetermineWhetherNumber(String InString)
        {
            /*
             * 文字列からfloat型に変換する。
             *
             * <param> InString
             *      String
             *      数字のString型
             * <return> StringToNum
             *      float
             *      数字のfloat型
             */

            try
            {
                return float.Parse(InString);
            }
            catch (FormatException)
            {
                ErrorExit("ERROR (040) : Please input number.");
                return 0;
            }
        }

        static void ErrorExit(String message)
        {
            /*
             * 引数の文字列を出力し、プログラムを終了する。
             * NOTE:
             *      Environment.Exit(0);の前にConsole.ReadKey();を呼び出すことで、
             *      エラーメッセージをユーザーが読めるようにしている。
             */

            Console.WriteLine(message);
            Console.WriteLine("Please enter to exit");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
