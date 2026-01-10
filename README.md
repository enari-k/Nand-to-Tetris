# Nand to Tetris Extended (C# Implementation)

コンピュータの基礎をNandゲートから構築し、さらに現代的なコンパイラ技術（LLVM IR・SSA形式）へと橋渡しする、低レイヤ深掘りプロジェクトです。

[![Zenn Article](https://img.shields.io/badge/Zenn-Part1-3EA8FF?style=for-the-badge&logo=zenn&logoColor=white)](https://zenn.dev/enari_k/articles/e63b21cd3057df)
[![Zenn Article](https://img.shields.io/badge/Zenn-Part2-3EA8FF?style=for-the-badge&logo=zenn&logoColor=white)](https://zenn.dev/enari_k/articles/93526fb1ba8529)

## 📌 コンセプト：理論の網羅と現代的拡張
本プロジェクトは、名著『コンピュータシステムの理論と実装』をベースにしつつ、以下の3点を独自の挑戦（Extra Steps）として設定しています。

1. **Hardware Optimization:** HDL設計におけるゲート数および遅延（段数）の最小化。
2. **Compiler Architecture:** AST（抽象構文木）を介した、フロントエンドとバックエンドの完全分離。
3. **Bridge to Modern:** スタックマシン（Jack VM）からレジスタマシン（LLVM IR / SSA形式）へのマッピング。

---

## 🗺 ロードマップ

### 🧱 Part 1: Hardware & Architecture (Nand to Tetris)
| 章 | テーマ | 内容・独自課題 | ステータス |
| :--- | :--- | :--- | :---: |
| 1 | **Boolean Logic** | 基礎論理回路の製作。**[Ex]** Nand使用数の最小化。 | ✅ |
| 2 | **Boolean Arithmetic** | ALU・加算器の製作。**[Ex]** 回路段数の削減（遅延最適化）。 | ✅ |
| 3 | **Sequential Logic** | メモリ・DFFの実装。**[Ex]** C#による回路シミュレータの自作。 | 🏃 |
| 4 | **Machine Language** | アセンブリの理解。**[Ex]** C#命令とアセンブリの実行効率比較。 | ⏳ |
| 5 | **Computer Arch** | Hackコンピュータ本体の製作。 | ⏳ |
| 6 | **Assembler** | Hackアセンブラの実装。 | ⏳ |

### 🛠 Part 2: Extended Compiler Steps (Ex)
標準のJackコンパイラを拡張し、現代的なコンパイラ基盤へと発展させます。

#### Step A: 疎結合なJackコンパイラの構築
- [ ] **Frontend:** Tokenizer & Parserの実装およびASTのメモリ上構築。
- [ ] **Abstraction:** `ICodeGenerator` によるマルチバックエンド対応。

#### Step B: LLVM IR へのマッピング (SSA Conversion)
- [ ] **SSA形式への変換:** スタック操作からレジスタ代入形式（Static Single Assignment）への変換。
- [ ] **LLVMSharp:** LLVMコンパイラ基盤を利用したネイティブコード生成。
- [ ] **Optimization:** LLVMのPassを用いたコード最適化。

#### Step C: Jack++ (言語拡張)
- [ ] **Type System:** `f32` (浮動小数点数) や `pointer` 操作の導入。

---

## 🔬 技術的な見どころ

### 1. 論理回路の最適化 (Hardware Optimization)
1〜2章では、単に回路を動かすだけでなく、**「論理圧縮」**を重視しています。Nandの数を減らすことはコスト削減に、段数を減らすことはクロック周波数の向上に直結するという、材料工学とも親和性の高い「物理的な効率」を意識した設計を行っています。

### 2. スタックマシンからレジスタマシンへの変換
Jack VM（スタックマシン）とLLVM IR（レジスタマシン）のセマンティクス・ギャップを埋める実装が本プロジェクトのハイライトです。

- **Jack VM (Stack-based):**
  ```text
  push a
  push b
  add
