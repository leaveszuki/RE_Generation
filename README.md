# RE_Generation
从自动机生成正则表达式(Generating regular expressions from finite automaton)

## 主要功能（Functions）
输入自动机的编号（格式：状态数_编号），根据已有的状态排序算法，将自动机转化为正则表达式，各算法的参考文献如下：
1. 流量法(Traffic Algorithm)：MCNAUGHTON R, YAMADA H. Regular Expressions and State Graphs for Automata[J]. IRE Transactions on Electronic Computers, 1960, EC-9(1): 39–47.
2. 桥状态法（Bridge Algorithm）：AHN J H, HAN Y S. Implementation of state elimination using heuristics[J]. Lecture Notes in Computer Science (including subseries Lecture Notes in Artificial Intelligence and Lecture Notes in Bioinformatics), 2009, 5642 LNCS: 178–187.
3. 权重法（Algorithm DM）：DELGADO M, MORAIS J. Approximation to the Smallest Regular Expression for a Given Regular Language[G]//DOMARATZKI M, AND OKHOTIN A, AND SALOMAA K. Implementation and Application of Automata. Berlin, Heidelberg: Springer Berlin Heidelberg, 2005: 312–314.
4. 回路计数法（Cycle Count Algorithm）：MOREIRA N, NABAIS D, REIS R. State Elimination Ordering Strategies: Some Experimental Results[J]. Electronic Proceedings in Theoretical Computer Science, 2010, 31(Dcfs): 139–148.
5. 动态度乘积法（Algorithm 0B）：LOMBARDY S, RÉGIS-GIANAS Y, SAKAROVITCH J. Introducing VAUCANSON[J]. Theoretical Computer Science, 2004, 328(1–2): 77–96.

通过JFLAP将自动机可视化。本工具通过编码标记确定性有限自动机，如下图所示自动机编号为6_10886。将自动机转为不包含初态和终态的关系矩阵，用字符串表示，即0110 0001 0101 0100。反转字符串并转为十进制，结合自动机的状态数可得标记。工具可将自动机编码为XML文件，然后在JFLAP上可视化。

## 使用方法（Usage）
1. 若仅作为工具使用，请打开StringRuleMining.exe文件，输入自动机编号即可使用。
2. 本工具基于Win7+Visual Studio 2019进行编写，使用相应的IDE即可打开文件StringRuleMining.sln
