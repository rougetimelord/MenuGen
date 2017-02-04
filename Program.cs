using System;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace MenuGen
{
    class Program
    {
        public static string PATH = null;
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            mainMenu();
        }
        static void makeMenu()
        {
            Console.Clear();
            Console.WriteLine("Path?");
            PATH = Console.ReadLine();
            List<string> options = new List<string>();
            Dictionary<int,string> labels = new Dictionary<int, string>();
            bool go = true;
            int blanks = 0;
            string result = "", nl = Environment.NewLine;
            Console.WriteLine("Title?");
            string title = Console.ReadLine();
            Console.WriteLine("Add options");
            while (go)
            {
                string option = Console.ReadLine();
                if (option == "")
                {
                    blanks++;
                    if (blanks == 1)
                        go = false;
                }
                else
                {
                    options.Add(option);
                    blanks = 0;
                }
            }
            Console.WriteLine("Generating");
            result += String.Format("using System;{0}namespace Menu{0}{{{0}class Program{0}{{{0}static void Main(string[] args){0}{{", nl, title);
            result += String.Format("{0}Console.ForegroundColor = ConsoleColor.White;{0}Console.WriteLine(\"{1}\");", nl,title);
            for(var i = 0; i < options.Count; i++)
            {
                string first = options[i].Substring(0, 1);
                string current;
                if (!labels.ContainsValue(first) && !first.Any(char.IsDigit))
                {
                    labels.Add(i, first);
                    current = options[i].Substring(1, options[i].Length - 1);
                }
                else {
                    current = options[i];
                    first = "";
                }
                result += String.Format(@"{0}Console.ForegroundColor = ConsoleColor.Green;{0}Console.Write(""{1}. {2}"");{0}Console.ForegroundColor = ConsoleColor.White;{0}Console.WriteLine(""{3}"");", nl, i + 1, first, current);
            }
            result += String.Format("{0}string input = Console.ReadLine();{0}switch (input){0}{{ {0}", nl);
            for(var i = 0; i < options.Count; i++)
            {
                var str = String.Format("case \"{0}\":{1}", i, nl);
                if (labels.ContainsKey(i))
                    str += String.Format("case \"{0}\":{1}", labels[i], nl);
                result += str + String.Format("{{ {0}// Call your function here {0}break; {0}}} {0}", nl);

            }
            result += String.Format("default:{0}{{{0}Main() {0}break; {0}}}", nl);
            result += String.Format("{0}}} {0}}} {0}}} {0}}}", nl);
            if (!Directory.Exists(PATH))
                Directory.CreateDirectory(PATH);
            PATH = PATH + "menu.txt";
            File.WriteAllText(PATH, result);
            Console.WriteLine("Done");
            Console.ReadLine();
            mainMenu();
        }
        static void previewMenu()
        {
            if(PATH == null || !File.Exists(PATH))
            {
                mainMenu();
            }
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            string exeName = "menuPreview.exe";
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = true;
            cp.OutputAssembly = exeName;
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, PATH);
            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("Errors building {0} into {1}", PATH, cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                }
            }
            else
            {
                Console.WriteLine("Source {0} built into {1} successfully.", PATH, cr.PathToAssembly);
                Process.Start(exeName);
            }
            Console.ReadLine();
            mainMenu();
        }
        static void mainMenu()
        {
            Console.Clear();
            Console.WriteLine("Menu Maker");
            string[] options = { "Make menu", "Preview menu" };
            for(int i = 0; i < options.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("{0}. {1}", i + 1, options[i][0]);
                string current = options[i].Substring(1, options[i].Length - 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(current);
            }
            string input = Console.ReadLine().ToLower();
            switch (input)
            {
                case "m":
                case "1":
                    {
                        makeMenu();
                        break;
                    }
                case "p":
                case "2":
                    {
                        previewMenu();
                        break;
                    }
                default:
                    {
                        mainMenu();
                        break;
                    }
            }
        }
    }
}
