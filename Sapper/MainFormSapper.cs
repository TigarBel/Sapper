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
        public static PictureBox[,] _pictureBoxsField;
        private static int _widthArray;
        private static int _heightArray;
        private int[,] _field;
        private const string _enabled = "Enabled";
        private const string _flag = "Flag";
        private const string _deactive = "Deactive";
        private bool _gameOver = false;
        private int _gameWin;
        private bool right = false;
        private bool left = false;
        private const int easeGameLine = 9;
        private const int easeGameMine = 10;        

        public MainFormSapper()
        {
            InitializeComponent();
        }
        //Вкладка-кнопка(новая игра)
        private void NewGame_Click(object sender, EventArgs e)
        {
            _field = GenerateField(easeGameLine, easeGameLine, easeGameMine);
            _field = MarkingField(_field, easeGameLine, easeGameLine);
            ClearPictureBoxsField(_pictureBoxsField, easeGameLine, easeGameLine);
            _pictureBoxsField = CreatePictureBoxsField(easeGameLine, easeGameLine);       
        }
        //Очистка поля от кнопок
        private void ClearPictureBoxsField(PictureBox[,] pBoxsField, int width, int height)
        {
            if (pBoxsField != null)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        this.Controls.Remove(pBoxsField[j, i]);
                    }
                }                
            }
            _gameOver = false;
        }
        //Создание и размещения кнопок контроля поля
        private PictureBox[,] CreatePictureBoxsField(int width, int height)
        {
            var pBoxsField = new PictureBox[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    pBoxsField[j, i] = new PictureBox()
                    {
                        Name = Convert.ToString(j) + '_' + Convert.ToString(i),
                        Size = new Size(20, 20),
                        Location = new Point(20 * i + 2, 20 * j + 30),
                        Image = Sapper.Properties.Resources.Enable_Field
                    };
                    pBoxsField[j, i].MouseUp += new MouseEventHandler(pictureBoxsField_MouseUp);
                    pBoxsField[j, i].MouseUp += new MouseEventHandler(pictureBoxsField_MouseDown);
                    pBoxsField[j, i].Image.Tag = _enabled;
                    this.Controls.Add(pBoxsField[j, i]);                    
                }
            }           
            return pBoxsField;
        }
        //Скрипт при отпускании кнопки мыши
        private void pictureBoxsField_MouseUp(object sender, MouseEventArgs e)
        {
            var pictureBoxs = (PictureBox)sender;
            int i = Convert.ToInt32(pictureBoxs.Name[0].ToString());
            int j = Convert.ToInt32(pictureBoxs.Name[2].ToString());
            if (e.Button == MouseButtons.Left)
            {
                //var pictureBoxs = (PictureBox)sender;
                //int i = Convert.ToInt32(pictureBoxs.Name[0].ToString());
                //int j = Convert.ToInt32(pictureBoxs.Name[2].ToString());
                Picture(i, j);
                if (_gameOver)
                {
                    for (int n = 0; n < _heightArray; n++)
                    {
                        for(int m =0;m<_widthArray;m++)
                        {
                            Picture(m, n);
                        }
                    }
                }  
                else if (_gameWin == 0)
                {
                    MessageBox.Show(" Вы победили!");
                }    
            }
            else if (e.Button == MouseButtons.Right)
            {
                //var pictureBoxs = (PictureBox)sender;
                //int i = Convert.ToInt32(pictureBoxs.Name[0].ToString());
                //int j = Convert.ToInt32(pictureBoxs.Name[2].ToString());
                PictureFlag(i, j);
                if (_gameWin == 0)
                {
                    MessageBox.Show(" Вы победили!");
                }
            }
        }
        //Если условие выполняется, открыть клетки вокруг заданной клетки, кроме флажков
        private void pictureBoxsField_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Если нажата левая кнопка мыши
                right = true;
            if (e.Button == MouseButtons.Right) // Если нажата правая кнопка мыши
                left = true;
            if (right && left) // Если нажата обе кнопки мыши
            {
                var pictureBoxs = (PictureBox)sender;
                //pictureBoxs.Enabled = false;
                int i = Convert.ToInt32(pictureBoxs.Name[0].ToString());
                int j = Convert.ToInt32(pictureBoxs.Name[2].ToString());
                int numeric = _field[i, j];

                int count = 0;
                for (int m = i - 1; m <= i + 1; m++)
                {
                    for (int k = j - 1; k <= j + 1; k++)
                    {
                        if (m >= 0 && k >= 0
                            && m <= i + 1 && k <= j + 1
                            && m < _heightArray && k < _widthArray)
                         {
                            if (_pictureBoxsField[k, m].Tag == _flag)
                            {
                                count++;
                            }
                        }
                    }
                }
                if (count == numeric)
                {
                    for (int m = i - 1; m <= i + 1; m++)
                    {
                        for (int k = j - 1; k <= j + 1; k++)
                        {
                            if (m >= 0 && k >= 0
                                && m <= i + 1 && k <= j + 1
                                && m < _heightArray && k < _widthArray)
                            {
                                if (_pictureBoxsField[k, m].Tag != _flag)
                                {
                                    Picture(k, m);
                                }
                            }
                        }
                    }
                }
                right = left = false;
            }
        }
        //Изменение картинки поля
        private void /*PictureBox*/ Picture(int width, int height) 
        {
            //PictureBox pictureCellField = new PictureBox();
            int numeric = _field[width, height];
            if (_pictureBoxsField[width, height].Tag == _flag && _gameOver && numeric == -1)
            {
                _pictureBoxsField[width, height].Image = Sapper.Properties.Resources.Defused_Mine_Field;
                _pictureBoxsField[width, height].Tag = _deactive;
            }
            else if (_pictureBoxsField[width, height].Tag == _flag && _gameOver && numeric != -1)
            {
                _pictureBoxsField[width, height].Image = Sapper.Properties.Resources.NonDefused_Mine_Field;
                _pictureBoxsField[width, height].Tag = _deactive;
            }
            else if (_pictureBoxsField[width, height].Tag == _flag)
            {
                _pictureBoxsField[width, height].Image = Sapper.Properties.Resources.Enable_Field;
                _pictureBoxsField[width, height].Tag = _enabled;
                _gameWin++; // Сбрасываем защитанную клетку поля
            }
            else switch (numeric)
            {
                case -1:
                        {
                            _gameOver = true;
                            _pictureBoxsField[width, height].Image = Sapper.Properties.Resources.Mine_Field;
                            _pictureBoxsField[width, height].Tag = _deactive;
                            break; // Проигрыш 
                        }
                case 0:
                        {
                            _pictureBoxsField[width, height].Image = Sapper.Properties.Resources._0_Field;
                            _pictureBoxsField[width, height].Tag = _deactive;
                            //RunOnZero(width, height);
                            _gameWin--; // Защитываем клетку поля
                            break;
                        }
                case 1:
                        {
                            _pictureBoxsField[width, height].Image = Sapper.Properties.Resources._1_Field;
                            _pictureBoxsField[width, height].Tag = _deactive;
                            _gameWin--; // Защитываем клетку поля
                            break;
                        }
                case 2:
                        {
                            _pictureBoxsField[width, height].Image = Sapper.Properties.Resources._2_Field;
                            _pictureBoxsField[width, height].Tag = _deactive;
                            _gameWin--; // Защитываем клетку поля
                            break;
                        }
                case 3:
                        {
                            _pictureBoxsField[width, height].Image = Sapper.Properties.Resources._3_Field;
                            _pictureBoxsField[width, height].Tag = _deactive;
                            _gameWin--; // Защитываем клетку поля
                            break;
                        }
                case 4:
                        {
                            _pictureBoxsField[width, height].Image = Sapper.Properties.Resources._4_Field;
                            _pictureBoxsField[width, height].Tag = _deactive;
                            _gameWin--; // Защитываем клетку поля
                            break;
                        }
                case 5:
                        {
                            _pictureBoxsField[width, height].Image = Sapper.Properties.Resources._5_Field;
                            _pictureBoxsField[width, height].Tag = _deactive;
                            _gameWin--; // Защитываем клетку поля
                            break;
                        }
                case 6:
                        {
                            _pictureBoxsField[width, height].Image = Sapper.Properties.Resources._6_Field;
                            _pictureBoxsField[width, height].Tag = _deactive;
                            _gameWin--; // Защитываем клетку поля
                            break;
                        }
                case 7:
                        {
                            _pictureBoxsField[width, height].Image = Sapper.Properties.Resources._7_Field;
                            _pictureBoxsField[width, height].Tag = _deactive;
                            _gameWin--; // Защитываем клетку поля
                            break;
                        }
                case 8:
                        {
                            _pictureBoxsField[width, height].Image = Sapper.Properties.Resources._8_Field;
                            _pictureBoxsField[width, height].Tag = _deactive;
                            _gameWin--; // Защитываем клетку поля
                            break;
                        }
            }
            //return pictureCellField;
        }
        //Изменение картинки поля на флаг
        private void PictureFlag(int width, int height)
        {
            if (_pictureBoxsField[width, height].Tag != _deactive)
            {
                if (_pictureBoxsField[width, height].Tag != _flag)
                {
                    _pictureBoxsField[width, height].Image = Sapper.Properties.Resources.Flag;
                    _pictureBoxsField[width, height].Tag = _flag;
                    _gameWin--; // Защитываем клетку поля
                }
                else
                {
                    _pictureBoxsField[width, height].Image = Sapper.Properties.Resources.Enable_Field;
                    _pictureBoxsField[width, height].Tag = _enabled;
                    _gameWin++; // Сбрасываем защитанную клетку поля
                }
            }
        }
        //Рекурсивный алгоритм по открыванию пустых клеток
        private void RunOnZero(int width, int height)
        {
            if (width + 1 < _widthArray)
            {
                if (_pictureBoxsField[width + 1, height].Image.Tag != _deactive)
                {
                    Picture(width + 1, height);
                }
            }

            if (width + 1 < _widthArray && height + 1 < _heightArray)
            {
                if (_pictureBoxsField[height + 1, width + 1].Image.Tag != _deactive)
                {
                    Picture(width + 1, height + 1);
                }
            }

            if (height + 1 < _heightArray)
            {
                if (_pictureBoxsField[width, height + 1].Image.Tag != _deactive)
                {
                    Picture(width, height + 1);
                }
            }

            if (width - 1 >= 0 && height + 1 < _heightArray)
            {
                if (_pictureBoxsField[height + 1, width - 1].Image.Tag != _deactive)
                {
                    Picture(width - 1, height + 1);
                }
            }

            if (width - 1 >= 0)
            {
                if (_pictureBoxsField[width - 1, height].Image.Tag != _deactive)
                {
                    Picture(width - 1, height);
                }
            }

            if (width - 1 >= 0 && height - 1 >= 0)
            {
                if (_pictureBoxsField[height - 1, width - 1].Image.Tag != _deactive)
                {
                    Picture(width - 1, height - 1);
                }
            }

            if (height - 1 >= 0)
            {
                if (_pictureBoxsField[width, height - 1].Image.Tag != _deactive)
                {
                    Picture(width, height - 1);
                }
            }

            if (width + 1 < _widthArray && height - 1 >= 0)
            {
                if (_pictureBoxsField[width + 1, height - 1].Image.Tag != _deactive)
                {
                    Picture(width + 1, height - 1);
                }
            }
        }
        //Создание поля с расставленными минами
        private int[,] GenerateField(int width, int height, int mine)
        {
            _widthArray = width;
            _heightArray = height;
            _gameWin = _widthArray * _heightArray; // Сколько должно быть клеток в поле для выйгрыша
            int count = mine;
            int[,] generateField = new int[easeGameLine, easeGameLine];
            while (count > 0)
            {
                int i = new Random().Next(width);
                int j = new Random().Next(height);
                if (generateField[i, j] != -1)
                {
                    generateField[i, j] = -1;
                    count--;
                }
            }
            return generateField;
        }
        //Разметить поле(0-пусто;-1-мина;1,2,3...-близость мин)
        private int[,] /*void*/ MarkingField(int[,] markingField, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (markingField[i, j] != -1)
                    {
                        int count = 0;
                        for (int m = i - 1; m <= i + 1; m++)
                        {
                            for (int k = j - 1; k <= j + 1; k++)
                            {
                                if (m >= 0 && k >= 0 
                                    && m <= i + 1 && k <= j + 1 
                                    && m < width && k < height)
                                {
                                    if (markingField[m, k] == -1)
                                    {
                                        count++;
                                    }
                                }
                            }
                        }
                        markingField[i, j] = count;
                    }
                }
            }
            return markingField;
        }

        
    }
}
