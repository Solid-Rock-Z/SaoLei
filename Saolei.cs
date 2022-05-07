using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameBox
{
    public partial class Saolei : Form
    {
        public Saolei()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {//必要时启用：
            //flowLayoutPanel2.Parent = splitContainer2.Panel1;
            //flowLayoutPanel1.Parent = splitContainer2.Panel2;
            //默认开启游戏参数：
            Hang = Lie = 15; nandu = 0.125;
            DialogResult a =
                MessageBox.Show("开始游戏?", "请确认", MessageBoxButtons.YesNo);
            if (a == DialogResult.Yes) StartGame(); else Close();
        }

        //所需变量定义
        Fangkuai[,] K; int leinum;
        int Hang, Lie, flaged;
        Size Weight;double nandu;
        bool first;//是否为第一次点击（防止第一次点就是雷）

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {   // 属性定义
            int w = 580 / Lie - 1;//自适应宽度；
            Weight = new Size(w, w);
            K = new Fangkuai[Hang, Lie];
            //变量定义：
            flaged = 0; label1.Text = flaged.ToString();
            label2.Text = "已标记数"; first = true;
            //开始绘制
            flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < Hang; i++)
            {
                for (int j = 0; j < Lie; j++)
                {
                    K[i, j] = new Fangkuai
                    {//属性初始化
                        BackgroundImage = Properties.Resources.none,
                        AutoSize = false,//取消button的自动大小方便后续方块的大小控制
                        Lei = false,
                        flag = false,
                        tfgetimage = false,
                        TabStop = false,//禁止键盘 tab索引；
                        X = i,
                        Margin = new Padding(1,1,1,1),
                        Y = j,//X 是 行，Y 是 列；
                        Size = Weight,//以后改为宽度表达式，使其不改变/改变主窗体 时 自适应 行数
                        Parent = flowLayoutPanel1,//设定其父容器【添加（显示）父容器中】
                        BackgroundImageLayout = ImageLayout.Zoom//更改背景图标自适应方式 以适应控件大小
                    }; K[i, j].MouseDown += Kuai_MouseDown;//添加订阅单击事件
                }
            }
        }

        private void Kuai_MouseDown(object sender, MouseEventArgs e)
        {//点击动作
            Fangkuai Kuai = (Fangkuai)sender;
            if (first)//如果是第一次点击
            { //随机标记块为雷
                Kuai.Lei = true;//先标记当前块为雷，防止后续将此块标记为雷；
                int x, y; leinum = Convert.ToInt32(Hang * Lie * nandu);
                Random r = new Random();
                for (int i = 0; i < leinum; i++)
                {
                    do
                    {//随机&&不重复标记为地雷
                        x = r.Next(Hang);
                        y = r.Next(Lie);
                    } while (K[x, y].Lei);
                    K[x, y].Lei = true;
                }
                label1.Text = leinum.ToString();
                Kuai.Lei = false;//作为第一个被点击的块，必然 不会是雷 对吧？；
                first = false;//更改标记，使 后续点击不再生成雷
                timer1.Start();
            }
            if (e.Button == MouseButtons.Left)
            {//左键
                if (!Kuai.flag)
                {
                    if (Kuai.Lei)
                    {
                        foreach (Fangkuai I in K) { ShowBackImage(I.X, I.Y); }
                        timer1.Stop(); MessageBox.Show("你输了", "GameOver");
                        SECOND = 0; StartGame(); label3.Text = "计时：0s";
                    }
                    else
                    {
                        ShowBackImage(Kuai.X, Kuai.Y);//计算周围类数&&改变背景图片
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {//右键
                if (!Kuai.tfgetimage)//感谢 胡思雨 同学的 发现：
                { //如果该块已经翻开，则不可进行标记（flag）操作。
                    if (Kuai.flag)
                    {//已标记 -- 取消标记
                        Kuai.flag = false;//较少已标记数
                        label2.Text = (--flaged).ToString();
                        Kuai.Image = null;
                    }
                    else
                    {//未标记 -- 标记
                        Kuai.flag = true;
                        label2.Text = (++flaged).ToString();//增加已标记数
                        Kuai.Image = Properties.Resources.flag;
                        if (flaged == leinum)
                        {
                            int flagtrue = 0;
                            for (int i = 0; i < Hang; i++)
                            {
                                for (int j = 0; j < Lie; j++)
                                {
                                    if (K[i, j].Lei && K[i, j].flag) { flagtrue++; }
                                }
                            }
                            if (flagtrue == flaged)
                            {
                                timer1.Stop();
                                MessageBox.Show($"你胜利了\r\n用时{SECOND}s", "Win ！");
                                SECOND = 0; StartGame();
                            }
                        }
                    }
                }
            }
        }

        

        private void Button1_Click(object sender, EventArgs e)
        {
            Saolei_Mode mode = new Saolei_Mode();
            DialogResult a = mode.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult q = MessageBox.Show("确认重新开始游戏么？\r\n雷的位置将随之刷新。","确认",MessageBoxButtons.YesNo);
            if (DialogResult.Yes == q) { StartGame(); }
        }

        /// <summary>
        /// 展示各块周围的雷数量数
        /// </summary>
        /// <param name="x">行</param>
        /// <param name="y">列</param>
        private void ShowBackImage(int x, int y)
        {
            if (!K[x, y].tfgetimage)//如果还没展现（翻开）过；
            {
                int num = 0;//x为行，Y为列
                if (K[x, y].Lei)
                {
                    K[x, y].BackgroundImage = Properties.Resources.lei;
                    K[x, y].tfgetimage = true;
                }
                else
                {//查找周围雷数：大前提都是该方向存在方块才进行计算，及每一个if（bool）都是为了判断是否存在
                    num += x > 0 && y > 0 ? K[x - 1, y - 1].Lei ? 1 : 0 : 0;//lu
                    num += x > 0 ? K[x - 1, y].Lei ? 1 : 0 : 0;//u
                    num += x > 0 && y + 1 < Lie ? K[x - 1, y + 1].Lei ? 1 : 0 : 0;//ru
                    num += y > 0 ? K[x, y - 1].Lei ? 1 : 0 : 0;//l
                    num += y + 1 < Lie ? K[x, y + 1].Lei ? 1 : 0 : 0;//r
                    num += x + 1 < Hang && y > 0 ? K[x + 1, y - 1].Lei ? 1 : 0 : 0;//dl
                    num += x + 1 < Hang ? K[x + 1, y].Lei ? 1 : 0 : 0;//d
                    num += x + 1 < Hang && y + 1 < Lie ? K[x + 1, y + 1].Lei ? 1 : 0 : 0;//rd
                    ///如最后一行使用if else等同于：
                    ///if(x+1<Hang&&y+1<Lie)
                    ///{
                    ///   if（K[x+1，y+1].Lei）
                    ///   {
                    ///         num += 1；
                    ///   } 
                    ///   num += 0；//写不写都一样
                    ///} 
                    ///num += 0；//同上

                    ///根据上方得到的值直接更改 方块 的 图；
                    ///如果数量为 0 ，意味着周围一圈都是安全的，
                    ///那么展开该块周围（3 X 3）的所有块 展开时为防止出现递归（a->b->a->b......）应跳过已经展示过的
                    ///此处甚至可以写成一个if else或 ？的表达式;大致为：        //如果资源使用的是本地资源，那么就可以简单使用下列语句进行图片获取；
                    K[x, y].BackgroundImage = num != 0 ? //从这里
                        Image.FromFile($@"..\..\Resources\{num}.png")
                        : Image.FromFile($@"..\..\Resources\none1.png");
                    K[x, y].tfgetimage = true;
                    if (num == 0)      ///该写法无法正常发布后运行，只能在特定路径运行；
                    {
                        if (K[x, y].flag)
                        {
                            K[x, y].Image = Properties.Resources.errorflag;
                            flaged--;
                            label2.Text = flaged.ToString();
                        }
                        AroundImage(x, y);
                    }                                   //到这里；等同于以下/*........*/
                    /*switch (num)                     //以下写法可以正常发布运行
                    {//匹配对应背景图：
                        case 1:
                            K[x, y].BackgroundImage = Properties.Resources._1;
                            K[x, y].tfgetimage = true; break;
                        case 2:
                            K[x, y].BackgroundImage = Properties.Resources._2;
                            K[x, y].tfgetimage = true; break;
                        case 3:
                            K[x, y].BackgroundImage = Properties.Resources._3;
                            K[x, y].tfgetimage = true; break;
                        case 4:
                            K[x, y].BackgroundImage = Properties.Resources._4;
                            K[x, y].tfgetimage = true; break;
                        case 5:
                            K[x, y].BackgroundImage = Properties.Resources._5;
                            K[x, y].tfgetimage = true; break;
                        case 6:
                            K[x, y].BackgroundImage = Properties.Resources._6;
                            K[x, y].tfgetimage = true; break;
                        case 7:
                            K[x, y].BackgroundImage = Properties.Resources._7;
                            K[x, y].tfgetimage = true; break;
                        case 8:
                            K[x, y].BackgroundImage = Properties.Resources._8;
                            K[x, y].tfgetimage = true; break;
                        case 0:
                        default:
                            K[x, y].BackgroundImage = Properties.Resources.none1;
                            if (!K[x, y].tfgetimage)//避免递归地发生（当两个空块挨在一起）
                            {
                                K[x, y].tfgetimage = true;
                                AroundImage(x, y);
                            } break;
                    }*/
                }

                if (!K[x, y].Lei && K[x, y].flag)
                { //对错误标记的处理
                    K[x, y].Image = Properties.Resources.errorflag;
                    label2.Text = (--flaged).ToString();
                }
            }
        }

        private void AroundImage(int x, int y)
        {//大前提都是该方向存在方块才进行显现，及每一个if（bool）都是为了判断是否存在
            if (x > 0 && y > 0) ShowBackImage(x - 1, y - 1);//lu
            if (x > 0) ShowBackImage(x - 1, y);//u
            if (x > 0 && y + 1 < Lie) ShowBackImage(x - 1, y + 1);//ru
            if (y > 0) ShowBackImage(x, y - 1);//l
            if (y + 1 < Lie) ShowBackImage(x, y + 1);//r
            if (x + 1 < Hang && y > 0) ShowBackImage(x + 1, y - 1);//dl
            if (x + 1 < Hang) ShowBackImage(x + 1, y);//d
            if (x + 1 < Hang && y + 1 < Lie) ShowBackImage(x + 1, y + 1);//rd
        }

        int SECOND = 0;//初始化计时器；

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;//计时显现；每隔一秒 + 1；
            label3.Text = (++SECOND).ToString() + "s";
        }
    }
}