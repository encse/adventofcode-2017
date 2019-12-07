using System;
using System.Linq;
using System.Text;
using AdventOfCode.Model;

namespace AdventOfCode.Generator {

    public class SplashScreenGenerator {
        public string Generate(Calendar calendar) {
            string calendarPrinter = CalendarPrinter(calendar);
            return $@"
                |using System;
                |
                |namespace AdventOfCode.Y{calendar.Year} {{
                |
                |    class SplashScreenImpl : AdventOfCode.SplashScreen {{
                |
                |        public void Show() {{
                |
                |            var color = Console.ForegroundColor;
                |            {calendarPrinter.Indent(12)}
                |            Console.ForegroundColor = color;
                |            Console.WriteLine();
                |        }}
                |
                |       private static void Write(int rgb, string text){{
                |           Console.Write($""\u001b[38;2;{{(rgb>>16)&255}};{{(rgb>>8)&255}};{{rgb&255}}m{{text}}"");
                |       }}
                |    }}
                |}}".StripMargin();
        }

        private string CalendarPrinter(Calendar calendar) {

            var lines = calendar.Lines.Select(line =>
                new[] { new CalendarToken { Text = "           " } }.Concat(line)).ToList();
            lines.Insert(0, new[]{new CalendarToken {
                ConsoleColor = 0xffff66,
                Text = $@"
                    |  __   ____  _  _  ____  __ _  ____     __  ____     ___  __  ____  ____         
                    | / _\ (    \/ )( \(  __)(  ( \(_  _)   /  \(  __)   / __)/  \(    \(  __)        
                    |/    \ ) D (\ \/ / ) _) /    /  )(    (  O )) _)   ( (__(  O )) D ( ) _)         
                    |\_/\_/(____/ \__/ (____)\_)__) (__)    \__/(__)     \___)\__/(____/(____)  {calendar.Year}
                    |"
                .StripMargin()
            }});

            var bw = new BufferWriter();
            foreach (var line in lines) {
                foreach (var token in line) {
                    bw.Write(token.ConsoleColor, token.Text);
                }

                bw.Write(-1, "\n");
            }
            return bw.GetContent();
        }

        bool Matches(string[] selector, object x){
            return true;
        }

        class BufferWriter {
            StringBuilder sb = new StringBuilder();
            int bufferColor = -1;
            string buffer = "";

            public void Write(int color, string text) {
                if (!string.IsNullOrWhiteSpace(text)) {
                    if (color != bufferColor && !string.IsNullOrWhiteSpace(buffer)) {
                        Flush();
                    }
                    bufferColor = color;
                }
                buffer += text;
            }

            private void Flush() {
                while (buffer.Length > 0) {
                    var block = buffer.Substring(0, Math.Min(100, buffer.Length));
                    buffer = buffer.Substring(block.Length);
                    block = block.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
                    sb.AppendLine($@"Write(0x{bufferColor.ToString("x")}, ""{block}"");");
                }
                buffer = "";
            }

            public string GetContent() {
                Flush();
                return sb.ToString();
            }
        }
    }
}