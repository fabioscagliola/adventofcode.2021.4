using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace com.fabioscagliola.AdventOfCode202104
{
    class Program
    {
        /// <summary>
        /// Represents a board used to play bingo against the giant squid 
        /// </summary>
        class BingoBoard
        {
            public const int SIZE = 5;

            protected int[,] matrix;
            protected bool[,] boolMatrix = new bool[SIZE, SIZE];
            protected List<int> drawnNumbers;
            protected List<int> MarkedNumberList { get; set; }

            public List<int> UnmarkedNumberList { get; protected set; }
            public int LastNumber { get; protected set; }
            public int SumOfUnmarkedNumbers => UnmarkedNumberList.Sum();

            /// <param name="matrix">The matrix of numbers on the board</param>
            /// <param name="drawnNumbers">The sequence of the drawn numbers</param>
            public BingoBoard(int[,] matrix, List<int> drawnNumbers)
            {
                MarkedNumberList = new List<int>();
                UnmarkedNumberList = new List<int>();

                this.matrix = matrix;
                this.drawnNumbers = drawnNumbers;

                for (int col = 0; col < SIZE; col++)
                    for (int row = 0; row < SIZE; row++)
                        UnmarkedNumberList.Add(matrix[row, col]);
            }

            protected int __numberOfDrawsToWin;

            /// <summary>
            /// An integer value indicating after how many draws the board wins 
            /// </summary>
            public int NumberOfDrawsToWin
            {
                get
                {
                    if (__numberOfDrawsToWin == 0)
                    {
                        foreach (int number in drawnNumbers)
                        {
                            __numberOfDrawsToWin++;
                            for (int col = 0; col < SIZE; col++)
                            {
                                for (int row = 0; row < SIZE; row++)
                                {
                                    if (matrix[row, col] == number)
                                    {
                                        boolMatrix[row, col] = true;
                                        MarkedNumberList.AddRange(UnmarkedNumberList.FindAll(x => x == number));
                                        UnmarkedNumberList.RemoveAll(x => x == number);
                                        if (IsWinner())
                                        {
                                            LastNumber = number;
                                            return __numberOfDrawsToWin;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return __numberOfDrawsToWin;
                }
            }

            protected bool IsWinner()
            {
                for (int row = 0; row < SIZE; row++)
                    if (boolMatrix[row, 0] && boolMatrix[row, 1] && boolMatrix[row, 2] && boolMatrix[row, 3] && boolMatrix[row, 4])
                        return true;

                for (int col = 0; col < SIZE; col++)
                    if (boolMatrix[0, col] && boolMatrix[1, col] && boolMatrix[2, col] && boolMatrix[3, col] && boolMatrix[4, col])
                        return true;

                return false;
            }

        }

        static void Main()
        {
            // I split the drawn numbers and the boards into two files in order to simplify their parsing 

            // Parse the drawn numbers 
            List<int> drawnNumbers = new List<int>();
            foreach (string s in File.ReadAllText("Input1.txt").Split(','))
                drawnNumbers.Add(int.Parse(s));

            // Parse the boards 
            List<int[,]> matrixList = new List<int[,]>();
            Regex regex = new Regex(@"(?:\b(?:\d+)\b)+");
            MatchCollection matchCollection = regex.Matches(File.ReadAllText("Input2.txt"));
            int col = 0;
            int row = 0;
            int[,] board = new int[BingoBoard.SIZE, BingoBoard.SIZE];
            foreach (Match match in matchCollection)
            {
                board[row, col] = int.Parse(match.Value);
                col++;
                if (col == BingoBoard.SIZE)
                {
                    col = 0;
                    row++;
                    if (row == BingoBoard.SIZE)
                    {
                        row = 0;
                        matrixList.Add(board);
                        board = new int[BingoBoard.SIZE, BingoBoard.SIZE];
                    }
                }
            }

            // Choose the first and last winning boards 
            List<BingoBoard> bingoBoardList = new List<BingoBoard>();
            foreach (int[,] matrix in matrixList)
                bingoBoardList.Add(new BingoBoard(matrix, drawnNumbers));
            bingoBoardList.Sort((a, b) => a.NumberOfDrawsToWin.CompareTo(b.NumberOfDrawsToWin));
            BingoBoard winningBoard = bingoBoardList.First();
            BingoBoard lastWinningBoard = bingoBoardList.Last();
            Console.WriteLine($"My final score if I choose the winning board would be {winningBoard.SumOfUnmarkedNumbers * winningBoard.LastNumber}");
            Console.WriteLine($"THe score of the board that will win last would be {lastWinningBoard.SumOfUnmarkedNumbers * lastWinningBoard.LastNumber}");
        }

    }
}

