using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq.Expressions;

class Program
{
    static void Main(string[] args)
    {
        if(args.Length==0)
        {
            Console.WriteLine("目的のファイルを指定してください");
            return;
        }
        string inputPath = args[0];
        if(!File.Exists(inputPath))
        {
            Console.WriteLine("指定されたファイルが見つかりません");
            return;
        }
        string outputPath = Path.ChangeExtension(inputPath,".asm");
        List<string> AsmCode = new();
        Console.WriteLine($"{inputPath}を翻訳しています...");
        using (StreamReader sr = new StreamReader(inputPath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                int commentIndex = line.IndexOf("//");
                if (commentIndex != -1) line = line.Substring(0, commentIndex);
                if (line == "") continue;
                string[] tempt = line.Trim().Split(" ");
                if(tempt[0]=="add")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=D+M");
                    AsmCode.Add("D=A+1");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=D");
                }
                else if(tempt[0]=="sub")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=M-D");
                    AsmCode.Add("D=A+1");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=D");
                }
                else if(tempt[0]=="neg")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("M=!M");
                }
                else if(tempt[0]=="eq")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=D-M");
                    AsmCode.Add("D=A+1");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=D");
                }
                else if(tempt[0]=="gt")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=D-M");
                    AsmCode.Add("D=A+1");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=D");
                }
                else if(tempt[0]=="lt")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=D-M");
                    AsmCode.Add("D=A+1");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=D");
                }
                else if(tempt[0]=="and")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=M&D");
                    AsmCode.Add("D=A+1");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=D");
                }
                else if(tempt[0]=="or")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=M|D");
                    AsmCode.Add("D=A+1");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=D");
                }
                else if(tempt[0]=="not")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=!M");
                }
                else if(tempt[0]=="push")
                {
                    if(tempt[1]=="constant")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=D");
                        AsmCode.Add("D=A+1");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="local")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@LCL");
                        AsmCode.Add($"A=M+D");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("D=A+1");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="argument")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@ARG");
                        AsmCode.Add($"A=M+D");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("D=A+1");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="this")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@THIS");
                        AsmCode.Add($"A=M+D");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("D=A+1");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="that")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@THAT");
                        AsmCode.Add($"A=M+D");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("D=A+1");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="static")
                    {
                        AsmCode.Add($"@MAIN.{tempt[2]}");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("D=A+1");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="temp")
                    {
                        AsmCode.Add($"@R{tempt[2]}");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("D=A+1");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="pointer")
                    {
                        AsmCode.Add("");
                    }
                }
                else if(tempt[0]=="pop")
                {
                    
                }
                else if(tempt[0]=="label")
                {
                    tempt[1]=tempt[1].Split("]")[0].Split("[")[1];
                    AsmCode.Add($"({tempt[1]})");
                }
                else if(tempt[0]=="goto")
                {
                    tempt[1]=tempt[1].Split("]")[0].Split("[")[1];
                    AsmCode.Add($"@{tempt[1]}");
                    AsmCode.Add("0;JMP");
                }
                else if(tempt[0]=="if-goto")
                {
                    tempt[1]=tempt[1].Split("]")[0].Split("[")[1];
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add($"@{tempt[1]}");
                    AsmCode.Add("D;JNE");
                }
            }
        }
        
    }
}