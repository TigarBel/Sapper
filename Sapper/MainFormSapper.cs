using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Sapper
{
    public partial class MainFormSapper : Form
    {
        /// <summary>
        /// Объект игры, где проходит вся логига, собственно, игры
        /// </summary>
        Game game = new Game();
        /// <summary>
        /// Визуальное поле
        /// </summary>
        PictureBox[,] _pictureBoxsField;
        /// <summary>
        /// Для вызова ф-ции при клике правой и левой кнопки мыши
        /// </summary>
        private bool right = false;
        /// <summary>
        /// Для вызова ф-ции при клике правой и левой кнопки мыши
        /// </summary>
        private bool left = false;

        /// <summary>
        /// Начало работы программы
        /// </summary>
        public MainFormSapper()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Вкладка-кнопка(новая игра)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewGame_Click(object sender, EventArgs e)
        {
            game.EaseGame(); // заданы параметры игры
            game.Begin();
            UpdatePictureField();
        }
        /// <summary>
        /// Обновление визуального поля
        /// </summary>
        private void UpdatePictureField()
        {
            if (this._pictureBoxsField != null) // Чистим поле формы
            {
                for (int y = 0; y < game.HeightArray; y++)
                {
                    for (int x = 0; x < game.WidthArray; x++)
                    {
                        this.Controls.Remove(this._pictureBoxsField[y, x]);
                    }
                }
            }
            _pictureBoxsField = new PictureBox[game.HeightArray, game.WidthArray]; // Создаём ячейки поля формы
            for (int y = 0; y < game.HeightArray; y++)
            {
                for (int x = 0; x < game.WidthArray; x++)
                {
                    _pictureBoxsField[y, x] = new PictureBox()
                    {
                        Name = Convert.ToString(x) + '_' + Convert.ToString(y), // Ширина_Высота
                        Size = new Size(20, 20),
                        Location = new Point(20 * y + 2, 20 * x + 30),
                    };
                    _pictureBoxsField[y, x].MouseUp += new MouseEventHandler(pictureBoxsField_MouseUp);
                    _pictureBoxsField[y, x].MouseDown += new MouseEventHandler(pictureBoxsField_MouseDown);
                    this.Controls.Add(_pictureBoxsField[y, x]);
                }
            }
            for (int y = 0; y < game.HeightArray; y++)
            {
                for (int x = 0; x < game.WidthArray; x++)
                {
                    switch (game.ShowClosedCell(x, y))
                    {
                        case 1:
                            {
                                Picture(x, y);
                                break;
                            }
                        case 2:
                            {
                                _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Enable_Field;
                                break;
                            }
                        case 3:
                            {
                                PictureFlag(x, y);
                                break;
                            }
                    }
                }
            }
        }
        /// <summary>
        /// Скрипт при отпускании кнопки мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxsField_MouseUp(object sender, MouseEventArgs e)
        {
            var pictureBoxs = (PictureBox)sender;
            int x = Convert.ToInt32(pictureBoxs.Name[0].ToString()); // Ширина
            int y = Convert.ToInt32(pictureBoxs.Name[2].ToString()); // Высота
            if (e.Button == MouseButtons.Left)
            {
                if (game.ShowClosedCell(x, y) == 2)
                {
                    game.OpenCell(x, y);
                }
                else if (game.ShowClosedCell(x, y) == 3)
                {
                    game.FieldClosedCell(x, y);
                    _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Enable_Field;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (game.ShowClosedCell(x, y) == 2)
                {
                    game.MarkCell(x, y);
                }
                else if (game.ShowClosedCell(x, y) == 3)
                {
                    game.FieldClosedCell(x, y);
                    _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Enable_Field;
                }
            }
            left = right = false; // Чтобы не вызвать следом ф-цию по открыванию вокруг клеток
            UpdatePictureField(); // Обновляем визуальное поле
        }
        /// <summary>
        /// Если условие выполняется, открыть клетки вокруг заданной клетки, кроме флажков
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxsField_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Если нажата левая кнопка мыши
                right = true;
            if (e.Button == MouseButtons.Right) // Если нажата правая кнопка мыши
                left = true;
            if (right && left) // Если нажата обе кнопки мыши
            {
                var pictureBoxs = (PictureBox)sender;
                int x = Convert.ToInt32(pictureBoxs.Name[0].ToString()); // Ширина
                int y = Convert.ToInt32(pictureBoxs.Name[2].ToString()); // Высота
                int numeric = game.Field(x, y);

                int count = 0;
                for (int y2 = y - 1; y2 <= y + 1; y2++)
                {
                    for (int x2 = x - 1; x2 <= x + 1; x2++)
                    {
                        if (x2 >= 0 && y2 >= 0
                            //&& x2 != x && y2 != y // Обманка, без неё можно открывать поле.
                            && x2 <= x + 1 && y2 <= y + 1
                            && x2 < game.WidthArray && y2 < game.HeightArray)
                         {
                            if (game.ShowClosedCell(x2, y2) == 3)
                            {
                                count++; // Считаем, сколько флагов вокрук ячейки
                            }
                        }
                    }
                }
                if (count == numeric)
                {
                    for (int y2 = y - 1; y2 <= y + 1; y2++)
                    {
                        for (int x2 = x - 1; x2 <= x + 1; x2++)
                        {
                            if (x2 >= 0 && y2 >= 0
                                && x2 <= x + 1 && y2 <= y + 1
                                && x2 < game.WidthArray && y2 < game.HeightArray)
                            {
                                if (game.ShowClosedCell(x2, y2) != 3)
                                {
                                    game.OpenCell(x2, y2); // Открываем во внутреннем массиве
                                }
                            }
                        }
                    }
                }
                right = left = false;
                UpdatePictureField(); // Обновляем визуальное поле
            }
        }
        /// <summary>
        /// Изменение картинки поля
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void Picture(int x, int y) 
        {
            int numeric = game.Field(x, y);
            
            switch (numeric)
            {
                case -1:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Mine_Field;
                        break; // Проигрыш 
                    }
                case 0:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._0_Field;
                        break;
                    }
                case 1:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._1_Field;
                        break;
                    }
                case 2:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._2_Field;
                        break;
                    }
                case 3:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._3_Field;
                        break;
                    }
                case 4:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._4_Field;
                        break;
                    }
                case 5:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._5_Field;
                        break;
                    }
                case 6:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._6_Field;
                        break;
                    }
                case 7:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._7_Field;
                        break;
                    }
                case 8:
                    {
                        _pictureBoxsField[y, x].Image = Sapper.Properties.Resources._8_Field;
                        break;
                    }
            }
        }
        /// <summary>
        /// Изменение картинки поля на флаг
        /// </summary>
        /// <param name="x">Ширина</param>
        /// <param name="y">Высота</param>
        private void PictureFlag(int x, int y)
        {
            _pictureBoxsField[y, x].Image = Sapper.Properties.Resources.Flag;
        }
    }
}