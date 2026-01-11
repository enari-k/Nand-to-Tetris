using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq.Expressions;

class Program {
    static void Main(string[] args) {
        // 1. 引数があるかチェック
        if (args.Length == 0) {
            Console.WriteLine("使い方: dotnet run <ファイル名.asm>");
            return;
        }

        string inputPath = args[0]; // 入力ファイル名（例：Max.asm）

        // 2. ファイルの存在確認
        if (!File.Exists(inputPath)) {
            Console.WriteLine($"エラー: {inputPath} が見つかりません。");
            return;
        }

        // 3. 出力ファイル名の作成（例：Max.asm -> Max.hack）
        string outputPath = Path.ChangeExtension(inputPath, ".hack");

        // 変換結果を溜めるリスト
        List<string> hackCode = new List<string>();

        Console.WriteLine($"{inputPath} を翻訳中...");
int romAddress = 0;
var symbolTable =new Dictionary<string,int>{{ "SP", 0 }, { "LCL", 1 }, { "ARG", 2 }, { "THIS", 3 }, { "THAT", 4 },
    { "R0", 0 }, { "R1", 1 }, { "R2", 2 }, { "R3", 3 }, { "R4", 4 },
    { "R5", 5 }, { "R6", 6 }, { "R7", 7 }, { "R8", 8 }, { "R9", 9 },
    { "R10", 10 }, { "R11", 11 }, { "R12", 12 }, { "R13", 13 }, { "R14", 14 }, { "R15", 15 },
    { "SCREEN", 16384 }, { "KBD", 24576 }};
using (StreamReader sr = new StreamReader(inputPath))
{
    string line;
    while ((line = sr.ReadLine()) != null)
    {
        // 前処理（空白・コメント削除）は今までと同じ
        line = line.Replace(" ", "");
        int commentIndex = line.IndexOf("//");
        if (commentIndex != -1) line = line.Substring(0, commentIndex);
        if (line == "") continue;

        if (line.StartsWith("(") && line.EndsWith(")"))
        {
            // ラベル定義 (LOOP) などの場合
            string labelName = line.Substring(1, line.Length - 2);
            symbolTable[labelName] = romAddress; // 現在の命令の行数を保存
        }
        else
        {
            // A命令かC命令なら、ROMの行数を進める
            romAddress++;
        }
    }
}

using (StreamReader sr = new StreamReader(inputPath))
{
    string line;
    int nextaddress = 16;
    while ((line = sr.ReadLine()) != null)
    {
        line = line.Replace(" ", ""); // スペースを全部消す
        int commentIndex = line.IndexOf("//"); // コメントの開始位置を探す
        if (commentIndex != -1) {
            line = line.Substring(0, commentIndex); // コメント以降を切り捨てる
        }
        if (line == "") continue; // 空っぽの行（またはコメントのみの行）なら飛ばす
        string output = "";
        if(line[0]=='@')
        {
            output = "0";
            string number = "";
            for(int i = 1;i < line.Length;i++)
            {
                number += line[i];
            }
            string tempt1 = "";
            if(int.TryParse(number,out int kazu))
            {
                if(kazu == 0) tempt1 = "0";
                while(kazu > 0)
                {
                    tempt1 = (kazu % 2).ToString() + tempt1;
                    kazu /= 2;
                }
            }
            else
            {
                if(symbolTable.ContainsKey(number)) tempt1 = Convert.ToString(symbolTable[number],2);
                else
                {
                    symbolTable[number] = nextaddress;
                    tempt1 = Convert.ToString(symbolTable[number],2);
                    nextaddress++;
                }
            }
            output += tempt1.PadLeft(15,'0');
        }
        else if(line[0]!='(')
        {
            output = "111";
            string a = "0";
            string dest = "000";
            string comp = "";
            string jump = "000";
            string tempt2 = line;
            string[] di = {"0","0","0"};
            if(line.Contains("="))
            {
                string dPart = line.Split('=')[0];
                foreach(char mozi in dPart)
                {
                    if(mozi == 'M') di[2] = "1";
                    else if(mozi == 'A') di[0] = "1";
                    else if(mozi == 'D') di[1] = "1";
                }
                dest = di[0] + di[1] + di[2];
            }
            if(line.Contains(";"))
            {
                tempt2 = tempt2.Split(';')[0];
            }
            if(line.Contains("="))
            {
                tempt2 = tempt2.Split('=')[1];
            }
                if(tempt2 == "0") comp = "101010";
                            else if(tempt2 == "1") comp = "111111";
                            else if(tempt2 == "-1") comp = "111010";
                            else if(tempt2 == "D") comp = "001100";
                            else if(tempt2 == "A") comp = "110000";
                            else if(tempt2 == "M")
                            {
                                comp = "110000";
                                a = "1";
                            }
                            else if(tempt2 == "!D") comp = "001101";
                            else if(tempt2 == "!A") comp = "110001";
                            else if(tempt2 == "!M")
                            {
                                comp = "110001";
                                a = "1";
                            }
                            else if(tempt2 == "-D") comp = "001111";
                            else if(tempt2 == "-A") comp = "110011";
                            else if(tempt2 == "-M")
                            {
                                comp = "110011";
                                a = "1";
                            }
                            else if(tempt2 == "D+1"||tempt2 =="1+D") comp = "011111";
                            else if(tempt2 == "A+1"||tempt2 =="1+A") comp = "110111";
                            else if(tempt2 == "M+1"||tempt2 =="1+M")
                            {
                                comp = "110111";
                                a = "1";
                            }
                            else if(tempt2 == "D-1") comp = "001110";
                            else if(tempt2 == "A-1") comp = "110010";
                            else if(tempt2 == "M-1")
                            {
                                comp = "110010";
                                a = "1";
                            }
                            else if(tempt2 == "D+A"||tempt2 =="A+D") comp = "000010";
                            else if(tempt2 == "D+M"||tempt2 =="M+D")
                            { 
                                comp = "000010";
                                a = "1";
                            }
                            else if(tempt2 == "D-A") comp = "010011";
                            else if(tempt2 == "D-M")
                            {
                                comp = "010011";
                                a = "1";
                            }
                            else if(tempt2 == "A-D") comp = "000111";
                            else if(tempt2 == "M-D")
                            {
                                comp = "000111";
                                a = "1";
                            }
                            else if(tempt2 == "D&A"||tempt2 == "A&D") comp = "000000";
                            else if(tempt2 == "D&M"||tempt2 == "M&D")
                            {
                                comp = "000000";
                                a = "1";
                            }
                            else if(tempt2 == "D|A"||tempt2 =="A|D") comp = "010101";
                            else if(tempt2 == "D|M"||tempt2 =="M|D")
                            {
                                comp = "010101";
                                a = "1";
                            }
                if(line.Contains(";"))
                {
                string tempt3 = line.Split(';')[1];
                if(tempt3 == "JGT") jump = "001";
                else if(tempt3 == "JEQ") jump = "010";
                else if(tempt3 == "JGE") jump = "011";
                else if(tempt3 == "JLT") jump = "100";
                else if(tempt3 == "JNE") jump = "101";
                else if(tempt3 == "JLE") jump = "110";
                else if(tempt3 == "JMP") jump = "111";
                }
            output = output + a + comp + dest + jump;
            }
        else continue;
        hackCode.Add(output);
        }
        // 1行読み込むごとに処理を行う
    }
    File.WriteAllLines(outputPath, hackCode);

    Console.WriteLine($"{outputPath} ファイルを作成しました！");
    }
}