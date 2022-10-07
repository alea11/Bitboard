using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bitboard.ChessMoves;

namespace Bitboard
{
    class Program
    {
        // пути к папкам с данными тестирования (относительно места сборки)
        private const string path_king = "..\\..\\..\\TestingFiles\\1.Bitboard - Король";
        private const string path_knight = "..\\..\\..\\TestingFiles\\2.Bitboard - Конь";
        private const string path_rook = "..\\..\\..\\TestingFiles\\3.Bitboard - Ладья";
        private const string path_elephant = "..\\..\\..\\TestingFiles\\4.Bitboard - Слон";
        private const string path_queen = "..\\..\\..\\TestingFiles\\5.Bitboard - Ферзь";

        static void Main(string[] args)
        {
            while (Work()) ;


            

        }

        public static bool Work() //int len, int max
        {
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("Select Piece:");
            Console.WriteLine("1 - King");
            Console.WriteLine("2 - Knight");
            Console.WriteLine("3 - Rook");
            Console.WriteLine("4 - Elephant");
            Console.WriteLine("5 - Queen");
            Console.WriteLine("other - quit");
            char c = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.WriteLine();

            string testingfilesPath;
            Pieces piece;

            switch (c)
            {
                case '1':
                    testingfilesPath = path_king;
                    piece = Pieces.King;
                    break;
                case '2':
                    testingfilesPath = path_knight;
                    piece = Pieces.Knight;
                    break;
                case '3':
                    testingfilesPath = path_rook;
                    piece = Pieces.Rook;
                    break;
                case '4':
                    testingfilesPath = path_elephant;
                    piece = Pieces.Elephant;
                    break;
                case '5':
                    testingfilesPath = path_queen;
                    piece = Pieces.Queen;
                    break;
                default:
                    return false;
            }

            Run(testingfilesPath, piece);

            return true;
        }

        static void Run(string testingfilesPath, Pieces piece)
        {
            ChessMoves cm = new ChessMoves();
            BitsCounter bc = new BitsCounter();
            bc.PrepareCounter3();

            int testNumber = 0;
            while (true)
            {
                string inFile = Path.Combine(testingfilesPath, $"test.{testNumber}.in");
                string outFile = Path.Combine(testingfilesPath, $"test.{testNumber}.out");

                if (!File.Exists(inFile) || !File.Exists(outFile))
                    break;

                string spos = File.ReadAllText(inFile).Trim();
                int pos = int.Parse(spos);

                string[] expects = File.ReadAllLines(outFile);
                int expectBits = int.Parse(expects[0]);
                ulong expectMask = ulong.Parse(expects[1]);

                ulong mask = cm.GetMoves(piece, pos);
                int bits1 = bc.GetBitsCount1(mask);
                int bits2 = bc.GetBitsCount2(mask);
                int bits3 = bc.GetBitsCount2(mask);

                Console.WriteLine($"{piece}: mask: {mask :X}, bits: {bits1},   \tCheck: mask: {mask== expectMask}, bits1: {expectBits == bits1}, bits2: {expectBits == bits2}, bits3: {expectBits == bits3}");

                testNumber++;
            }
        }


    }
}
