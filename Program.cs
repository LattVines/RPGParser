using System;
using System.Collections;
using System.Text;



namespace RPGParser
{
    class Program
    {
        static void Main(string[] args)
        {
            //"[a|b] [c|d|e|f|[1|2] thing|last] "
            //smaller test
            string test_string = "[a|b] [c|d|e|f|[1|2] thing|last]";
            RPGParser parser_obj = new RPGParser(test_string);

            Console.Write("result: " + RPGParser.ParseString(test_string));
            Console.ReadLine();

        }
    }












}
