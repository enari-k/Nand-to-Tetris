using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq.Expressions;
using System.Text;

class Program
{
    static int flag = 1;
    static void Main(string[] args)
    {
        if(args.Length==0)
        {
            Console.WriteLine("目的のファイルを指定してください");
            return;
        }
        string inputPath = Path.GetFullPath(args[0]);
        if(!File.Exists(inputPath)&&!Directory.Exists(inputPath))
        {
            Console.WriteLine("指定されたファイルまたはディレクトリが見つかりません");
            return;
        }
        string[] vmFiles;
        string outputPath;
        if (Directory.Exists(inputPath)) {
        // ディレクトリの場合：中の .vm ファイルをすべて取得
        vmFiles = Directory.GetFiles(inputPath, "*.vm");
        // 出力ファイル名はディレクトリ名にする
        outputPath = Path.Combine(inputPath, Path.GetFileName(inputPath.TrimEnd(Path.DirectorySeparatorChar)) + ".asm");
        } else {
        // 単一ファイルの場合
            vmFiles = new[] { inputPath };
            outputPath = Path.ChangeExtension(inputPath, ".asm");
        }
        List<string> AsmCode = new();
        AsmCode.Add("@256");
        AsmCode.Add("D=A");
        AsmCode.Add("@SP");
        AsmCode.Add("M=D");
        if (Directory.Exists(inputPath)) 
        {
            // リターンアドレスをプッシュ（ここでは仮に bootstrap_return とする）
            AsmCode.Add("@BOOTSTRAP_RETURN");
            AsmCode.Add("D=A");
            AsmCode.Add("@SP");
            AsmCode.Add("A=M");
            AsmCode.Add("M=D");
            AsmCode.Add("@SP");
            AsmCode.Add("M=M+1");

            // LCL, ARG, THIS, THAT を 0 (または現在の値) でプッシュ (5回分)
            foreach (var seg in new[] { "LCL", "ARG", "THIS", "THAT" }) {
                AsmCode.Add($"@{seg}");
                AsmCode.Add("D=M");
                AsmCode.Add("@SP");
                AsmCode.Add("A=M");
                AsmCode.Add("M=D");
                AsmCode.Add("@SP");
                AsmCode.Add("M=M+1");
            }

            // ARG = SP - 5 - 0
            AsmCode.Add("@5");
            AsmCode.Add("D=A");
            AsmCode.Add("@SP");
            AsmCode.Add("D=M-D");
            AsmCode.Add("@ARG");
            AsmCode.Add("M=D");

            // LCL = SP
            AsmCode.Add("@SP");
            AsmCode.Add("D=M");
            AsmCode.Add("@LCL");
            AsmCode.Add("M=D");

            // Sys.init へジャンプ
            AsmCode.Add("@Sys.init");
            AsmCode.Add("0;JMP");

            // リターンラベル
            AsmCode.Add("(BOOTSTRAP_RETURN)");
        }
        Console.WriteLine($"{inputPath}を翻訳しています...");
        foreach (var filePath in vmFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Translate(filePath,fileName,AsmCode);
        }
        AsmCode.Add("(Infinity_Loop)");
        AsmCode.Add("@Infinity_Loop");
        AsmCode.Add("0;JMP");
        File.WriteAllLines(outputPath, AsmCode);

        Console.WriteLine($"{outputPath} ファイルを作成しました！");

        static void Translate(string path,string fileName,List<string> AsmCode)
        {
            using StreamReader sr = new StreamReader(path);            
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                int commentIndex = line.IndexOf("//");
                if (commentIndex != -1) line = line.Substring(0, commentIndex);
                if (line == "") continue;
                string[] tempt = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
                if(tempt.Length==0) continue;
                if(tempt[0]=="add")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=D+M");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=M-1");
                }
                else if(tempt[0]=="sub")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=M-D");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=M-1");
                }
                else if(tempt[0]=="neg")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("M=-M");
                }
                else if(tempt[0]=="eq")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("D=D-M");
                    AsmCode.Add($"@TRUE{flag}");
                    AsmCode.Add("D;JEQ");
                    AsmCode.Add("@SP");
                    AsmCode.Add("AM=M-1");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=0");
                    AsmCode.Add($"@END{flag}");
                    AsmCode.Add("0;JMP");
                    AsmCode.Add($"(TRUE{flag})");
                    AsmCode.Add("@SP");
                    AsmCode.Add("AM=M-1");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=-1");
                    AsmCode.Add($"(END{flag})");
                    flag++;
                }
                else if(tempt[0]=="gt")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("D=D-M");
                    AsmCode.Add($"@TRUE{flag}");
                    AsmCode.Add("D;JLT");
                    AsmCode.Add("@SP");
                    AsmCode.Add("AM=M-1");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=0");
                    AsmCode.Add($"@END{flag}");
                    AsmCode.Add("0;JMP");
                    AsmCode.Add($"(TRUE{flag})");
                    AsmCode.Add("@SP");
                    AsmCode.Add("AM=M-1");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=-1");
                    AsmCode.Add($"(END{flag})");
                    flag++;
                }
                else if(tempt[0]=="lt")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("D=D-M");
                    AsmCode.Add($"@TRUE{flag}");
                    AsmCode.Add("D;JGT");
                    AsmCode.Add("@SP");
                    AsmCode.Add("AM=M-1");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=0");
                    AsmCode.Add($"@END{flag}");
                    AsmCode.Add("0;JMP");
                    AsmCode.Add($"(TRUE{flag})");
                    AsmCode.Add("@SP");
                    AsmCode.Add("AM=M-1");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=-1");
                    AsmCode.Add($"(END{flag})");
                    flag++;
                }
                else if(tempt[0]=="and")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=M&D");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=M-1");
                }
                else if(tempt[0]=="or")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("A=A-1");
                    AsmCode.Add("M=M|D");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=M-1");
                }
                else if(tempt[0]=="not")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M");
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
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=M+1");
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
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=M+1");
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
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=M+1");
                    }
                    else if(tempt[1]=="this")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@THIS");
                        AsmCode.Add("A=M+D");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=M+1");
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
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=M+1");
                    }
                    else if(tempt[1]=="static")
                    {
                        AsmCode.Add($"@{fileName}.{tempt[2]}");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=M+1");
                    }
                    else if(tempt[1]=="temp")
                    {
                        AsmCode.Add("@5");
                        AsmCode.Add("D=A");
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("A=D+A");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=M+1");
                    }
                    else if(tempt[1]=="pointer")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add($"@TRUE{flag}");
                        AsmCode.Add("D;JEQ");
                        AsmCode.Add("@4");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=M+1");
                        AsmCode.Add($"@END{flag}");
                        AsmCode.Add("0;JMP");
                        AsmCode.Add($"(TRUE{flag})");
                        AsmCode.Add("@3");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=M+1");
                        AsmCode.Add($"(END{flag})");
                        flag++;
                    }
                }
                else if(tempt[0]=="pop")
                {
                    if(tempt[1]=="local")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@LCL");
                        AsmCode.Add("D=D+M");
                        AsmCode.Add("@R13");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("AM=M-1");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@R13");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="argument")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@ARG");
                        AsmCode.Add("D=D+M");
                        AsmCode.Add("@R13");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("AM=M-1");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@R13");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="this")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@THIS");
                        AsmCode.Add("D=D+M");
                        AsmCode.Add("@R13");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("AM=M-1");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@R13");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="that")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@THAT");
                        AsmCode.Add("D=D+M");
                        AsmCode.Add("@R13");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("AM=M-1");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@R13");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="static")
                    {
                        AsmCode.Add($"@{fileName}.{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@R13");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("AM=M-1");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@R13");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="temp")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add("@5");
                        AsmCode.Add("D=D+A");
                        AsmCode.Add("@R13");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("AM=M-1");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@R13");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                    }
                    else if(tempt[1]=="pointer")
                    {
                        AsmCode.Add($"@{tempt[2]}");
                        AsmCode.Add("D=A");
                        AsmCode.Add($"@TRUE{flag}");
                        AsmCode.Add("D;JEQ");
                        AsmCode.Add("@SP");
                        AsmCode.Add("AM=M-1");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@4");
                        AsmCode.Add("M=D");
                        AsmCode.Add($"@END{flag}");
                        AsmCode.Add("0;JMP");
                        AsmCode.Add($"(TRUE{flag})");
                        AsmCode.Add("@SP");
                        AsmCode.Add("AM=M-1");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@3");
                        AsmCode.Add("M=D");
                        AsmCode.Add($"(END{flag})");
                        flag++;
                    }
                }
                else if(tempt[0]=="label")
                {
                    AsmCode.Add($"({tempt[1]})");
                }
                else if(tempt[0]=="goto")
                {
                    AsmCode.Add($"@{tempt[1]}");
                    AsmCode.Add("0;JMP");
                }
                else if(tempt[0]=="if-goto")
                {
                    AsmCode.Add("@SP");
                    AsmCode.Add("AM=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add($"@{tempt[1]}");
                    AsmCode.Add("D;JNE");
                }
                else if(tempt[0]=="return")
                {
                    // 1. frame = LCL (一時変数R13に避難。LCLを直接壊さない)
                    AsmCode.Add("@LCL");
                    AsmCode.Add("D=M");
                    AsmCode.Add("@R13");
                    AsmCode.Add("M=D");

                    // 2. retAddr = *(frame - 5) (R14に避難)
                    AsmCode.Add("@5");
                    AsmCode.Add("A=D-A"); // DはまだLCLの値。A = LCL - 5
                    AsmCode.Add("D=M");   // D = RAM[LCL-5]（戻り先アドレス）
                    AsmCode.Add("@R14");
                    AsmCode.Add("M=D");

                    // 3. *ARG = pop() (関数の戻り値をARG[0]に置く)
                    AsmCode.Add("@SP");
                    AsmCode.Add("AM=M-1");
                    AsmCode.Add("D=M");
                    AsmCode.Add("@ARG");
                    AsmCode.Add("A=M");
                    AsmCode.Add("M=D");

                    // 4. SP = ARG + 1
                    AsmCode.Add("@ARG");
                    AsmCode.Add("D=M+1");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=D");

                    // 5. THAT, THIS, ARG, LCL を復元 (R13を基準に遡る)
                    // THAT = *(frame-1), THIS = *(frame-2)...
                    int offset = 1;
                    foreach (var seg in new[] { "THAT", "THIS", "ARG", "LCL" }) {
                        AsmCode.Add("@R13");
                        AsmCode.Add("D=M");
                        AsmCode.Add($"@{offset}");
                        AsmCode.Add("A=D-A"); // A = frame - offset
                        AsmCode.Add("D=M");   // D = その場所の中身
                        AsmCode.Add($"@{seg}");
                        AsmCode.Add("M=D");   // 各レジスタを復元
                        offset++;
                    }

                    // 6. goto retAddr
                    AsmCode.Add("@R14");
                    AsmCode.Add("A=M");
                    AsmCode.Add("0;JMP");
                }
                else if(tempt[0]=="function")
                {
                    AsmCode.Add($"({tempt[1]})");
                    AsmCode.Add($"@{tempt[2]}");
                    AsmCode.Add("D=A");
                    AsmCode.Add($"(LOOP_INIT{flag})"); // ラベル名も少し分かりやすく
                    AsmCode.Add($"@LOOP_END{flag}");
                    AsmCode.Add("D;JEQ");             // ★ Dが0になったらループ終了へ飛ぶ
                    AsmCode.Add("D=D-1");
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M");
                    AsmCode.Add("M=0");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=M+1");
                    AsmCode.Add($"@LOOP_INIT{flag}");
                    AsmCode.Add("0;JMP");             // 無条件で先頭に戻ってDをチェック
                    AsmCode.Add($"(LOOP_END{flag})");
                    flag++;
                }
                else if(tempt[0]=="call")
                {
                // 1. returnAddress を push
                    AsmCode.Add($"@return{flag}");
                    AsmCode.Add("D=A");
                    AsmCode.Add("@SP");
                    AsmCode.Add("A=M");
                    AsmCode.Add("M=D");
                    AsmCode.Add("@SP");
                    AsmCode.Add("M=M+1"); // 次の空き地へ

                    // 2. LCL, ARG, THIS, THAT を順番に push (同じパターンで繰り返す)
                    string[] segments = { "LCL", "ARG", "THIS", "THAT" };
                    foreach (var seg in segments) {
                        AsmCode.Add($"@{seg}");
                        AsmCode.Add("D=M");
                        AsmCode.Add("@SP");
                        AsmCode.Add("A=M");
                        AsmCode.Add("M=D");
                        AsmCode.Add("@SP");
                        AsmCode.Add("M=M+1");
                    }

                    // 3. ARG = SP - 5 - nArgs
                    AsmCode.Add($"@{5 + int.Parse(tempt[2])}");
                    AsmCode.Add("D=A");
                    AsmCode.Add("@SP");
                    AsmCode.Add("D=M-D"); // D = SPの「値」 - (5+nArgs)
                    AsmCode.Add("@ARG");
                    AsmCode.Add("M=D");

                    // 4. LCL = SP
                    AsmCode.Add("@SP");
                    AsmCode.Add("D=M");
                    AsmCode.Add("@LCL");
                    AsmCode.Add("M=D");

                    // 5. goto f
                    AsmCode.Add($"@{tempt[1]}");
                    AsmCode.Add("0;JMP");

                    // 6. 戻り先ラベル
                    AsmCode.Add($"(return{flag})");
                    flag++;
                }
            }
        }
    }
}
