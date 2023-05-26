using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace szachy
{
    public partial class Form1 : Form
    {
        public Image chessSprites;
        public int[,] map = new int[8, 8]
        {
            {15,14,13,12,11,13,14,15},
            {16,16,16,16,16,16,16,16},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {26,26,26,26,26,26,26,26},
            {25,24,23,22,21,23,24,25},
        };
        public Button[,] butts = new Button[8, 8];

        public int player;

        public Button prevButton;

        public bool moving = false;

        public Form1()
        {
            InitializeComponent();

            Icon = Properties.Resources.szach;
            chessSprites = new Bitmap(Properties.Resources.chess);
            
            Init();
        }

        public void Init()
        {
            map = new int[8, 8]
            {
                {15,14,13,12,11,13,14,15},
                {16,16,16,16,16,16,16,16},
                {0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0},
                {26,26,26,26,26,26,26,26},
                {25,24,23,22,21,23,24,25},
            };
            player = 1;
            CreateMap();
            label2.Text = "BIAŁE";
        }

        public void CreateMap()
        {
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j] = new Button();

                    Button button = new Button();
                    button.Size = new Size(50,50);
                    button.Location = new Point(j*50,i*50);

                    switch (map[i, j]/10)
                    {
                        case 1:
                            Image part = new Bitmap(50, 50);
                            Graphics g = Graphics.FromImage(part);
                            g.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0+150*(map[i,j]%10-1),
                                0, 150, 150, GraphicsUnit.Pixel);
                            button.BackgroundImage = part;
                            break;
                        case 2:
                            Image part2 = new Bitmap(50, 50);
                            Graphics g2 = Graphics.FromImage(part2);
                            g2.DrawImage(chessSprites, new Rectangle(0, 0, 50, 50), 0 + 150 * (map[i, j] % 10 - 1),
                                150, 150, 150, GraphicsUnit.Pixel);
                            button.BackgroundImage = part2;
                            break;
                    }
                    button.BackColor = GetPrevButtonColor(button);
                    button.Click += new EventHandler(OnFigurePress);
                    this.Controls.Add(button);

                    butts[i,j] = button;
                }
            }
        }

        public Color GetPrevButtonColor(Button prevButton)
        {
            if ((prevButton.Location.Y/50 % 2) != 0)
            {
                if ((prevButton.Location.X / 50 % 2) == 0)
                {
                    return Color.Gray;
                }
            }
            if ((prevButton.Location.Y / 50) % 2 == 0)
            {
                if ((prevButton.Location.X / 50) % 2 != 0)
                {
                    return Color.Gray;
                }
            }
            return Color.White;
        }

        public void OnFigurePress(object sender, EventArgs e)
        {
            if(prevButton != null)
                prevButton.BackColor = GetPrevButtonColor(prevButton);

            Button pressed = sender as Button;

            if (map[pressed.Location.Y / 50, pressed.Location.X / 50] != 0 &&
                map[pressed.Location.Y / 50, pressed.Location.X / 50] / 10 == player)
            {
                CloseSteps();
                pressed.BackColor = Color.Red;
                DeactivateAllButtons();
                pressed.Enabled = true;
                ShowSteps(pressed.Location.Y / 50, pressed.Location.X / 50,
                    map[pressed.Location.Y / 50, pressed.Location.X / 50]);

                if (moving)
                {
                    CloseSteps();
                    pressed.BackColor = GetPrevButtonColor(pressed);
                    ActivateAllButtons();
                    moving = false;
                }
                else
                    moving = true;
            }
            else
            {
                if (moving)
                {
                    int temp = map[pressed.Location.Y/50,pressed.Location.X/50];
                    map[pressed.Location.Y / 50, pressed.Location.X / 50] = map[prevButton.Location.Y / 50,
                        prevButton.Location.X / 50];

                    map[prevButton.Location.Y /50, prevButton.Location.X / 50] = temp;
                    pressed.BackgroundImage = prevButton.BackgroundImage;
                    prevButton.BackgroundImage = null;
                    moving = false;
                    CloseSteps();
                    ActivateAllButtons();
                    switchPlayer();
                }
            }
            
            prevButton = pressed;
        }

        public void ShowSteps(int IcurrFigure, int JcurrFigure, int currFigure)
        {
            int dir = player == 1 ? 1 : -1;
            switch (currFigure % 10)
            {
                case 6:
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure))
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure] == 0)
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure].Enabled = true;
                        }
                    }

                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure + 1))
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure + 1] != 0 &&
                            map[IcurrFigure + 1 * dir, JcurrFigure + 1] / 10 != player)
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure + 1].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure + 1].Enabled = true;
                        }
                    }
                    if (InsideBorder(IcurrFigure + 1 * dir, JcurrFigure - 1))
                    {
                        if (map[IcurrFigure + 1 * dir, JcurrFigure - 1] != 0 &&
                            map[IcurrFigure + 1 * dir, JcurrFigure - 1] / 10 != player)
                        {
                            butts[IcurrFigure + 1 * dir, JcurrFigure - 1].BackColor = Color.Yellow;
                            butts[IcurrFigure + 1 * dir, JcurrFigure - 1].Enabled = true;
                        }
                    }
                    break;
                case 5:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    break;

                case 3:
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;

                case 2:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure);
                    ShowDiagonal(IcurrFigure, JcurrFigure);
                    break;

                case 1:
                    ShowVerticalHorizontal(IcurrFigure, JcurrFigure, true);
                    ShowDiagonal(IcurrFigure, JcurrFigure, true);
                    break;

                case 4:
                    ShowHorseSteps(IcurrFigure, JcurrFigure);
                    break;
            }
        }

        public void ShowHorseSteps(int IcurrFigure, int JcurrFigure)
        {
            if (InsideBorder(IcurrFigure - 2, JcurrFigure + 1))
            {
                DeterminePath(IcurrFigure - 2, JcurrFigure + 1);
            }
            if (InsideBorder(IcurrFigure - 2, JcurrFigure - 1))
            {
                DeterminePath(IcurrFigure - 2, JcurrFigure - 1);
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure + 1))
            {
                DeterminePath(IcurrFigure + 2, JcurrFigure + 1);
            }
            if (InsideBorder(IcurrFigure + 2, JcurrFigure - 1))
            {
                DeterminePath(IcurrFigure + 2, JcurrFigure - 1);
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure + 2))
            {
                DeterminePath(IcurrFigure - 1, JcurrFigure + 2);
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure + 2))
            {
                DeterminePath(IcurrFigure + 1, JcurrFigure + 2);
            }
            if (InsideBorder(IcurrFigure - 1, JcurrFigure - 2))
            {
                DeterminePath(IcurrFigure - 1, JcurrFigure - 2);
            }
            if (InsideBorder(IcurrFigure + 1, JcurrFigure - 2))
            {
                DeterminePath(IcurrFigure + 1, JcurrFigure - 2);
            }
        }

        public void DeactivateAllButtons()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = false;
                }
            }
        }

        public void ActivateAllButtons()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].Enabled = true;
                }
            }
        }

        public void ShowDiagonal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure + 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }
        }

        public void ShowVerticalHorizontal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (InsideBorder(i, JcurrFigure))
                {
                    if (!DeterminePath(i, JcurrFigure))
                        break;
                }
                if (isOneStep)
                    break;
            }
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (InsideBorder(i, JcurrFigure))
                {
                    if (!DeterminePath(i, JcurrFigure))
                        break;
                }
                if (isOneStep)
                    break;
            }
            for (int j = JcurrFigure + 1; j < 8; j++)
            {
                if (InsideBorder(IcurrFigure, j))
                {
                    if (!DeterminePath(IcurrFigure, j))
                        break;
                }
                if (isOneStep)
                    break;
            }
            for (int j = JcurrFigure - 1; j >= 0; j--)
            {
                if (InsideBorder(IcurrFigure, j))
                {
                    if (!DeterminePath(IcurrFigure, j))
                        break;
                }
                if (isOneStep)
                    break;
            }
        }

        public bool DeterminePath(int IcurrFigure, int j)
        {
            if (map[IcurrFigure, j] == 0)
            {
                butts[IcurrFigure, j].BackColor = Color.Yellow;
                butts[IcurrFigure, j].Enabled = true;
            }
            else
            {
                if (map[IcurrFigure, j] / 10 != player)
                {
                    butts[IcurrFigure, j].BackColor = Color.Yellow;
                    butts[IcurrFigure, j].Enabled = true;
                }
                return false;
            }
            return true;
        }

        public bool InsideBorder(int ti, int tj)
        {
            if (ti >= 8 || tj >= 8 || ti < 0 || tj < 0)
                return false;
            return true;
        }

        public void CloseSteps()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    butts[i, j].BackColor = GetPrevButtonColor(butts[i,j]);
                }
            }
        }

        public void switchPlayer()
        {
            if (player == 1)
            {
                player = 2;
                label2.Text = "CZARNE";
            }
            else
            {
                player = 1;
                label2.Text = "BIAŁE";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.Controls.Add(button1);
            this.Controls.Add(label1);
            this.Controls.Add(label2);
            Init();
        }
    }
}