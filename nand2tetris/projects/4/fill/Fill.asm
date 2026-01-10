// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/4/Fill.asm

// Runs an infinite loop that listens to the keyboard input. 
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel. When no key is pressed, 
// the screen should be cleared.

//// Replace this comment with your code.
(LOOP)
@KBD
D=M
@ARI
D;JGT
@color
M=0
@Fill
0;JMP
(ARI)
@color
M=-1
@Fill
0;JMP
(Fill)
@SCREEN
D=A
@adder
M=D
(Fill_LOOP)
@adder
D=M
@24576
D=D-A
@LOOP
D;JEQ
@color
D=M
@adder
A=M
M=D
@adder
M=M+1
@Fill_LOOP
0;JMP