using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitboard
{
    public class ChessMoves
    {
        public enum Pieces
        {
            Pawn, // пешка
            Elephant, // слон
            Rook, // ладья
            Knight, // конь
            Queen, // ферзь
            King // король
        }

        const ulong r2 = 0xff00ul;
        const ulong r1 = 0xfful;
        const ulong cA = 0x101010101010101ul;

        
        // маски учета граничных позиций (слева и справа,
        // а вверх - вниз - можно не обрабатывать, эти варианты исключаются автоматически (вниз - меньше 0 , вверх - обрезка при переполнении сдвигом  )) :
        
        const ulong noL1 = 0xfefefefefefefefe; // вся доска без 1го левого (a) столбца
        const ulong noL2 = 0xfcfcfcfcfcfcfcfc; // вся доска без 2х левых (ab) столбцов
        //const ulong noL3 = 0xf8f8f8f8f8f8f8f8; // вся доска без 3х левых (a-c) столбцов
        //const ulong noL4 = 0xf0f0f0f0f0f0f0f0; // вся доска без 4х левых (a-d) столбцов
        //const ulong noL5 = 0xe0e0e0e0e0e0e0e0; // вся доска без 5х левых (a-e) столбцов
        //const ulong noL6 = 0xc0c0c0c0c0c0c0c0; // вся доска без 6х левых (a-f) столбцов
        //const ulong noL7 = 0x8080808080808080; // вся доска без 7х левых (a-g) столбцов


        const ulong noR1 = 0x7f7f7f7f7f7f7f7f; // вся доска без 1го правого (h) столбца
        const ulong noR2 = 0x3f3f3f3f3f3f3f3f; // вся доска без 2х правых (gh) столбцов
        //const ulong noR3 = 0x1f1f1f1f1f1f1f1f; // вся доска без 3х правых (f-h) столбцов
        //const ulong noR4 = 0x0f0f0f0f0f0f0f0f; // вся доска без 4х правых (e-h) столбцов
        //const ulong noR5 = 0x0707070707070707; // вся доска без 5х правых (d-h) столбцов
        //const ulong noR6 = 0x0303030303030303; // вся доска без 6х правых (c-h) столбцов
        //const ulong noR7 = 0x0101010101010101; // вся доска без 7х правых (b-h) столбцов

        public ulong GetMoves(Pieces piece, int pos)
        {
            ulong targets = 0;
            ulong start = 1ul << pos;

            // переменные позиции с наложением пограничного условия (чтобы не применялись соответствующие слагаемые операции переходов при нахождении фигуры рядом с границей)
            ulong startNoL1 = start & noL1;
            ulong startNoR1 = start & noR1;
            ulong startNoL2 = start & noL2;
            ulong startNoR2 = start & noR2;
            ulong startRow2 = start & r2;

            switch (piece)
            {
                case Pieces.Pawn:
                    if ((start & r1) != 0) // строка 1 для пешек запрещена
                    {
                        throw new Exception("Строка 1 для пешек запрещена");
                    }

                    targets = (startNoL1 << 7) | (start << 8) | (startNoR1 << 9)
                        | (startRow2 << 16); // со второй строки можно пройти на 2 клетки

                    break;

                case Pieces.King:
                    targets =
                        (startNoL1 << 7) | (start << 8) | (startNoR1 << 9) |
                        (startNoL1 >> 1) | (startNoR1 << 1) |
                        (startNoL1 >> 9) | (start >> 8) | (startNoR1 >> 7);
                    break;

                case Pieces.Knight:
                    targets =
                        (startNoL1 << 15) | (startNoR1 << 17) |
                        (startNoL2 << 6) | (startNoR2 << 10) |
                        (startNoL2 >> 10) | (startNoR2 >> 6) |
                        (startNoL1 >> 17) | (startNoR1 >> 15);
                    break;

                case Pieces.Rook: // этот вариант работает только по единичной позиции (нужно получать конкретные координаты)
                    ulong row = GetRow(pos);
                    ulong col = GetCol(pos);
                    targets = row ^ col;
                    break;
                case Pieces.Elephant: // этот вариант работает только по единичной позиции (нужно получать конкретные координаты)
                    ulong diagL = GetDiagL(pos);
                    ulong diagR = GetDiagR(pos);
                    targets = diagL ^ diagR;
                    break;
                case Pieces.Queen: // этот вариант работает только по единичной позиции (нужно получать конкретные координаты)
                    ulong rowQ = GetRow(pos);
                    ulong colQ = GetCol(pos);
                    ulong diagLQ = GetDiagL(pos);
                    ulong diagRQ = GetDiagR(pos);
                    targets = (rowQ ^ colQ) ^ (diagLQ ^ diagRQ);
                    break;
            }

            return targets;
        }



        // получение координат
        ulong GetRow(int pos)
        {
            int row = pos / 8;
            return r1 << (row * 8);
        }

        ulong GetCol(int pos)
        {
            int col = pos % 8;
            return cA << col;
        }

        // диагональ через точку,  параллельно a1-h8
        ulong GetDiagL(int pos)
        {
            // большая диагональ a1-h8
            ulong diag = 0x8040201008040201ul;

            // координаты позиции
            int col = pos % 8;
            int row = pos / 8;

            int offset = col - row; // смещение точки относительно гланой диагонали - по вертикали

            // сдвигаем диагональ по вертикали до указанной точки
            return OffsetVertical(diag, offset);
        }

        // диагональ через точку, параллельно a8-h1
        ulong GetDiagR(int pos)
        {
            // большая диагональ a8-h1
            ulong diag = 0x102040810204080ul;

            // координаты позиции
            int col = pos % 8;
            int row = pos / 8;

            int offset = 7 - col - row;// смещение точки относительно гланой диагонали - по вертикали (учитываем - зеркальный столбец)

            // сдвигаем диагональ по вертикали до указанной точки
            return OffsetVertical(diag, offset);
        }

        // сдвиг по вертикали, если offset > 0 - то вниз, иначе - вверх
        ulong OffsetVertical(ulong sourse, int offset)
        {
            if (offset > 0) //  вниз
            {
                sourse = sourse >> offset * 8;
            }
            else if (offset < 0) //  вверх
            {
                sourse = sourse << offset * -8; // учитываем что в этом случае  offset - отрицательный
            }

            return sourse;
        }
    }
}
