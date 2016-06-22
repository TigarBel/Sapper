using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sapper
{
    /// <summary>
    /// Делегат для вызова события "Завершить игру"
    /// </summary>
    public delegate void DelegateGameEnd(int points);
    /// <summary>
    /// Класс логики игры сапёра
    /// </summary>
    class Game
    {
        /// <summary>
        /// Количество мин на поле
        /// </summary>
        private int _mine = 0;
        /// <summary>
        /// Массив внутренний, с числами
        /// </summary>
        private int[,] _field;
        /// <summary>
        /// Массив равный размером с внутренним, для определения открытых, закрытых и помеченных ячеек поля (1-открыто; 2-закрыто; 3-флаг)
        /// </summary>
        private int[,] _fieldClosedCell;
        /// <summary>
        /// Подтверждение проигрыша
        /// </summary>
        private bool _gameOver;
        /// <summary>
        /// Количество вскрытых клеток и закрытых мин
        /// </summary>
        private bool _gameWin;
        /// <summary>
        /// Количество очков для выигрыша
        /// </summary>
        private int _pointsForWin;
        /// <summary>
        /// Собитие по окончанию игры
        /// </summary>
        public event DelegateGameEnd GameEnded;
        /// <summary>
        /// Свойство по проверке проигрыша
        /// </summary>
        public bool GameOver
        {
            get { return _gameOver; }
            private set
            {
                if (value == true)
                {
                    _gameOver = value;
                    PointsForWin = -1;
                }
            }
        }
        /// <summary>
        /// Свойство по проверки выигрыша. Если полученное число равно 0, то открыты (проверенны) все ячейки массива
        /// </summary>
        public bool GameWin
        {
            get { return _gameWin; }
        }
        /// <summary>
        /// Количество очков для выигрыша
        /// </summary>
        private int PointsForWin
        {
            get { return _pointsForWin; }
            set
            {
                _pointsForWin = value;
                if (value == 0 || value == -1) // 0 - выиграл, -1 - проиграл
                {
                    _gameWin = true;
                    GameEnded(value); // Зажег событие
                }
            }
        }
        //(V)TODO: Зачем это свойство на set? Этот параметр задается только при создании игры, а значит он должен быть параметром конструктора
        /// <summary>
        /// Ширина массива (игрового поля)
        /// </summary>
        public int WidthArray
        {
            get
            {
                return _field.GetLength(1);
            }
        }
        //(V)TODO: Зачем это свойство на set? Этот параметр задается только при создании игры, а значит он должен быть параметром конструктора
        /// <summary>
        /// Высота массиыва (игрового поля)
        /// </summary>
        public int HeightArray
        {
            get
            {
                return _field.GetLength(0);
            }
        }
        /// <summary>
        /// Количество мин. Для внесения количества мин, нужно ввести ширину и высоту поля
        /// </summary>
        public int Mine
        {
            get
            {
                return _mine;
            }
            set
            {
                if (HeightArray >= 9 && HeightArray <= 16 && // Нужная высота у поля
                    WidthArray >= 9 && WidthArray <= 30 &&  // Нужная ширина у поля
                    value >= 10 && value <= 100 &&  // Нужное количество мин
                    //(V)TODO: максимум две свободных клетки? Жестко
                    value <= (HeightArray * WidthArray) / 2) // Количество мин не превышает площадь поля, максимум 2 свободных поля
                { _mine = value; }
            }
        }

        public Game()
        {
            // easeGameLine = 9;
            _field = new int[9, 9]; // Создаём пустое поле близости мин
            _fieldClosedCell = new int[9, 9]; // Создаём пустое поле закрытых ячеек
            Mine = 10; // easeGameMine = 10;
            GenerateField(); // Генерируем поле
        }
        /// <summary>
        /// Вывод числа обозначающего состояние ячейки массива (октрыка, закрыта или помечена)
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        /// <returns>(1-открыто; 2-закрыто; 3-флаг)</returns>
        public int ShowClosedCell(int x, int y)
        {
            //(V)TODO: имхо, бесполезные проверки, ибо в случае неправильных индексов и так всё упадет
            return _fieldClosedCell[y, x];
        }
        /// <summary>
        /// Обозначить ячейку поля открытой
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void FieldOpenCell(int x, int y)
        {
            _fieldClosedCell[y, x] = 1;
        }
        /// <summary>
        /// Обозначить ячейку поля закрытой
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        public void FieldClosedCell(int x, int y)
        {
            _fieldClosedCell[y, x] = 2;
            PointsForWin++; // Закрываем ячейку
        }
        /// <summary>
        /// Обозначить ячейку поля помеченной (поставить флажок)
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void FieldMarkCell(int x, int y)
        {
            _fieldClosedCell[y, x] = 3;
        }
        /// <summary>
        /// Обозначить ячейку поля обезвреженной (поставить мину с другим фоном)
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        public void DefusedCell(int x, int y)
        {
            _fieldClosedCell[y, x] = 4;
        }
        /// <summary>
        /// Обозначить ячейку поля ошибочным обезвреживанием (поставить перечёркнутую мину)
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        public void NonDefusedCell(int x, int y)
        {
            _fieldClosedCell[y, x] = 5;
        }
        /// <summary>
        /// Вывод числа обозначающего близость мин, из конкретной ячейки массива
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        /// <returns>(0-пусто;-1-мина;1,2,3...-близость мин)</returns>
        public int Field(int x, int y)
        {
            return _field[y, x];
        }
        //(V)TODO: а не логичнее ли передавать длину/ширину поля и количество мин в этот метод?  Генерация, создание поля с расставленными минами
        private void GenerateField()
        {
            _gameOver = false;
            _gameWin = false;
            int count = Mine;
            PointsForWin = HeightArray * WidthArray; // Количество открытых (проверенных) клеток для победы
            var random = new Random();
            int y, x;
            while (count > 0)
            {
                //(V)TODO: зачем ты создаешь ДВА генератора случайных чисел на КАЖДОЙ итерации? Создай один ДО цикла и вызывай у него метод Next
                y = random.Next(HeightArray);
                x = random.Next(WidthArray);
                if (_field[y, x] != -1)
                {
                    //(V)TODO: а не проще ли при генерации мин сразу же увеличивать на единицу все соседние клетки (если они не мины)? Тогда метод MarkingField будет не нужен
                    _field[y, x] = -1;
                    count--;
                    for (int m = y - 1; m <= y + 1; m++)
                    {
                        for (int k = x - 1; k <= x + 1; k++)
                        {
                            if (m >= 0 && k >= 0
                                && m <= y + 1 && k <= x + 1
                                && m < HeightArray && k < WidthArray)
                            {
                                if (_field[m, k] != -1)
                                {
                                    _field[m, k]++; // Увеличиваем близость мин в ячейках вокруг мины на 1
                                }
                            }
                        }
                    }
                }
            }
            for (y = 0; y < HeightArray; y++)
            {
                for (x = 0; x < WidthArray; x++)
                {
                    _fieldClosedCell[y, x] = 2; // Закрываем все поля, для последующего открытия
                }
            }
        }
        /// <summary>
        /// Открыть ячейку поля
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        /// <returns>(0-пусто;-1-мина;1,2,3...-близость мин)</returns>
        public void OpenCell(int x, int y)
        {
            FieldOpenCell(x, y);
            if (Field(x, y) == -1) // Встал на мину. Вызов события "Конец игры!"  
            {
                GameOver = true;
            }
            else PointsForWin--; // Иначе ещё одна ячейка открыта
            if (Field(x, y) == 0) { RunOnZero(x, y); } // Вызов рекурсивной ф-ции по открытию массива
        }
        /// <summary>
        /// Пометить (поставить флажок) ячейку массива
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        public void MarkCell(int x, int y)
        {
            FieldMarkCell(x, y);
            PointsForWin--; // Ещё одна ячейка открыта
        }
        /// <summary>
        /// Рекурсивный алгоритм по открыванию нулевых ячеек массива
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void RunOnZero(int x, int y)
        {
            if (x + 1 < WidthArray) // вправо
            {
                if (ShowClosedCell(x + 1, y) != 1)
                {
                    OpenCell(x + 1, y);
                }
            }
            if (x + 1 < WidthArray && y + 1 < HeightArray) // вправо и вниз
            {
                if (ShowClosedCell(x + 1, y + 1) != 1)
                {
                    OpenCell(x + 1, y + 1);
                }
            }
            if (y + 1 < HeightArray) // вниз
            {
                if (ShowClosedCell(x, y + 1) != 1)
                {
                    OpenCell(x, y + 1);
                }
            }
            if (x - 1 >= 0 && y + 1 < HeightArray) // влево и вниз
            {
                if (ShowClosedCell(x - 1, y + 1) != 1)
                {
                    OpenCell(x - 1, y + 1);
                }
            }
            if (x - 1 >= 0) // влево
            {
                if (ShowClosedCell(x - 1, y) != 1)
                {
                    OpenCell(x - 1, y);
                }
            }
            if (x - 1 >= 0 && y - 1 >= 0) // влево и вверх
            {
                if (ShowClosedCell(x - 1, y - 1) != 1)
                {
                    OpenCell(x - 1, y - 1);
                }
            }
            if (y - 1 >= 0) // вверх
            {
                if (ShowClosedCell(x, y - 1) != 1)
                {
                    OpenCell(x, y - 1);
                }
            }
            if (x + 1 < WidthArray && y - 1 >= 0) // вправо и вверх
            {
                if (ShowClosedCell(x + 1, y - 1) != 1)
                {
                    OpenCell(x + 1, y - 1);
                }
            }
        }
    }
}

//(V)TODO: В общем, в настоящий момент у тебя основной косяк с делегатом.
//(V)TODO: После каждого открытия поля ты изменяешь значение PointsToWin. При изменении этого числа у тебя идет ЕЩЕ ОДНА подписка делегата на метод GoodEnd.
//(V)TODO: Затем ты запускаешь этот делегат. Таким образом, после первого открытого поля метод GoodEnd вызывается один раз. После второй клетки два раза, после третьей - три раза.
//(V)TODO: Отсюда и переполнение стека. Отсюда и еще один нюанс: в методе GoodEnd, на который ты постоянно подписываешься, есть вызов метода MarkCell.
//(V)TODO: В этом методе опять же происходит изменение PointsToWin (плюс еще одна ненужная подписка) и последующий вызов делегата! Фактически, это рекурсия.

//(V)TODO: 1) Тебе нужно создать публичное событие GameEnded (игра закончена)
//(V)TODO: 2) В качестве аргументов события ты должен передавать количество игровых очков, чтобы подписчик на событие смог определить, удачно ли окончилась игра или нет.
//(V)TODO: 3) Подписка на событие происходит в MainForm в конструкторе и только один. В обработчике события описываются действия при завершении игры.
//(V)TODO: 4) Внутри класса Game НИ ОДИН МЕТОД ИЛИ СВОЙСТВО не подписываются на это событие.
//(V)TODO: 5) Внутри PointToWin сделать условие: "Если количество очков равно 0 или -1, то зажечь событие". Не подписаться на событие, а зажечь его

//(V)TODO: если сложно в голове ужержать всю игровую логику, то попробуй нарисовать на бумаге блок-схемы, что и как должно происходить при тех или иных действиях пользователя.

//TODO: когда избавишься от рекурсии и исправишь игровую логику, будем исправлять косячную перерисовку игрового поля