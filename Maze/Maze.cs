using System;
using System.Windows.Forms;

namespace Maze
{

    internal partial class Maze : Form
    {
        internal const int MazeLength = 4;
        private static int startX, startY, z;
        private static State currnetState;
        private static Agenda agenda;
        private static System.Threading.Thread thread;
        private static System.Drawing.Color red = System.Drawing.Color.Red,
            green = System.Drawing.Color.Green;
        private static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        internal Maze() { InitializeComponent(); }
        private void App_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < MazeLength; i++)
                for (int j = 0; j < MazeLength; j++)
                    State.isOk[i, j] = true;
            button1.BackColor = button2.BackColor = button3.BackColor = button4.BackColor =
                button5.BackColor = button6.BackColor = button7.BackColor = button8.BackColor =
                button9.BackColor = button10.BackColor = button11.BackColor = button12.BackColor =
                button13.BackColor = button14.BackColor = button15.BackColor = button16.BackColor = green;
        }

        private void Start_Click(object sender, EventArgs e)
        {
            char[] d1, d2;
            int q1 = 0, q2 = 0;
            z = 0;
            label2.Text = "            Loading...";
            System.Threading.Tasks.Task.Delay(1).Wait();
            watch = System.Diagnostics.Stopwatch.StartNew();

            d1 = Xtextbox.Text.ToCharArray();
            d2 = Ytextbox.Text.ToCharArray();
            for (int i = 48; i <= 51; i++)
            {
                if (d1[0] == i)
                    q1 = 1;
                if (d2[0] == i)
                    q2 = 1;
            }

            if (!State.isOk[startY, startX])
                label2.Text = "            Start pos has to be accessible";
            else if (q1 == 0 && q2 == 0)
                label2.Text = "            X && Y are not Ok!";
            else if (q1 == 0)
                label2.Text = "            X is not a Ok!";
            else if (q2 == 0)
                label2.Text = "            Y is not a Ok!";
            else
            {
                startX = int.Parse(Xtextbox.Text);
                startY = int.Parse(Ytextbox.Text);
                int[,] Cross = new int[MazeLength + 5, MazeLength + 5];
                for (int i = 0; i < MazeLength; i++)
                    for (int j = 0; j < MazeLength; j++)
                        Cross[i, j] = 0;
                currnetState = new State('.', "S", Cross, startX, startY, 0);
                agenda = new Agenda(currnetState);
                while (!GOAL(currnetState))
                {
                    int x = currnetState.getX(), y = currnetState.getY();
                    int[,] g = currnetState.getCross();
                    if (x < MazeLength - 1 && (currnetState.getLastMove() != 1 || checkBox1.Checked))
                        if (State.isOk[y, x + 1])
                            agenda.Add('R', currnetState.getWay(), g, x + 1, y, 0);

                    if (y < MazeLength - 1 && (currnetState.getLastMove() != 2 || checkBox1.Checked))
                        if (State.isOk[y + 1, x])
                            agenda.Add('D', currnetState.getWay(), g, x, y + 1, 3);

                    if (y >= 1 && (currnetState.getLastMove() != 3 || checkBox1.Checked))
                        if (State.isOk[y - 1, x])
                            agenda.Add('U', currnetState.getWay(), g, x, y - 1, 2);

                    if (x >= 1 && (currnetState.getLastMove() != 0 || checkBox1.Checked))
                        if (State.isOk[y, x - 1])
                            agenda.Add('L', currnetState.getWay(), g, x - 1, y, 1);

                    z++;
                    if (!agenda.Remove(ref currnetState)) { label2.Text = "            No way to Finish!!!\n" + z + " ways has been checked. in " + watch.ElapsedMilliseconds + "ms"; break; }
                }
            }

        }

        private void _Show()
        {
            int[,] cross = currnetState.getCross();

            label1.Text = currnetState.getWay();

            button1.Text = cross[0, 0].ToString();
            button2.Text = cross[1, 0].ToString();
            button3.Text = cross[2, 0].ToString();
            button4.Text = cross[3, 0].ToString();
            button5.Text = cross[0, 1].ToString();
            button6.Text = cross[1, 1].ToString();
            button7.Text = cross[2, 1].ToString();
            button8.Text = cross[3, 1].ToString();
            button9.Text = cross[0, 2].ToString();
            button10.Text = cross[1, 2].ToString();
            button11.Text = cross[2, 2].ToString();
            button12.Text = cross[3, 2].ToString();
            button13.Text = cross[0, 3].ToString();
            button14.Text = cross[1, 3].ToString();
            button15.Text = cross[2, 3].ToString();
            button16.Text = cross[3, 3].ToString();
        }

        private bool GOAL(State currnetState)
        {
            int[,] cross = currnetState.getCross();
            if (z >= 2000000){
                label2.Text = "            Out of RAM!!!\n" + z + " ways has been checked. in " + watch.ElapsedMilliseconds + "ms";
                return true;
            }
            if (currnetState.getWay().Length > 30){
                label2.Text = "            No way to Finish!!!\n" + z + " ways has been checked. in " + watch.ElapsedMilliseconds + "ms";
                return true;
            }
            for (int i = 0; i < MazeLength; i++)
                for (int j = 0; j < MazeLength; j++)
                    if (cross[i, j] == 0 && State.isOk[j, i])
                        return false;

            watch.Stop();
            label2.Text = "            Ok\n" + z + " ways has been checked. in " + watch.ElapsedMilliseconds + "ms";
            _Show();
            thread = new System.Threading.Thread(new System.Threading.ThreadStart(__Show));
            thread.Start();
            return true;
        }

        private void __Show()
        {
            char[] a = currnetState.getWay().ToCharArray();
            int x = startX, y = startY;
            for (int i = 0; i < a.Length; i++)
            {
                _switch(x, y, i, a.Length);
                switch (a[i])
                {
                    case 'R': x++; break;
                    case 'D': y++; break;
                    case 'L': x--; break;
                    case 'U': y--; break;
                    default: break;
                }
                System.Threading.Tasks.Task.Delay(300).Wait();
            }
            _switch(x, y, a.Length, a.Length);
        }

        private void _switch(int x, int y, int i, int l)
        {
            System.Drawing.Color C = System.Drawing.Color.FromArgb((255 * i) / l, (255 * i) / l, (255 * i) / l);
            switch (y)
            {
                case 0:
                    switch (x)
                    {
                        case 0: button1.BackColor = C; break;
                        case 1: button2.BackColor = C; break;
                        case 2: button3.BackColor = C; break;
                        case 3: button4.BackColor = C; break;
                        default: break;
                    }
                    break;
                case 1:
                    switch (x)
                    {
                        case 0: button5.BackColor = C; break;
                        case 1: button6.BackColor = C; break;
                        case 2: button7.BackColor = C; break;
                        case 3: button8.BackColor = C; break;
                        default: break;
                    }
                    break;
                case 2:
                    switch (x)
                    {
                        case 0: button9.BackColor = C; break;
                        case 1: button10.BackColor = C; break;
                        case 2: button11.BackColor = C; break;
                        case 3: button12.BackColor = C; break;
                        default: break;
                    }
                    break;
                case 3:
                    switch (x)
                    {
                        case 0: button13.BackColor = C; break;
                        case 1: button14.BackColor = C; break;
                        case 2: button15.BackColor = C; break;
                        case 3: button16.BackColor = C; break;
                        default: break;
                    }
                    break;
                default: break;
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            if (thread != null)
                if (thread.ThreadState != System.Threading.ThreadState.Stopped)
                    thread.Suspend();

            for (int i = 0; i < MazeLength; i++)
                for (int j = 0; j < MazeLength; j++)
                    State.isOk[i, j] = true;
            button1.BackColor = button2.BackColor = button3.BackColor = button4.BackColor =
                button5.BackColor = button6.BackColor = button7.BackColor = button8.BackColor =
                button9.BackColor = button10.BackColor = button11.BackColor = button12.BackColor =
                button13.BackColor = button14.BackColor = button15.BackColor = button16.BackColor = green;
            label2.Text = "            Ok";
            label1.Text = "...";

            button1.Text = "0 , 0";
            button2.Text = "0 , 1";
            button3.Text = "0 , 2";
            button4.Text = "0 , 3";
            button5.Text = "1 , 0";
            button6.Text = "1 , 1";
            button7.Text = "1 , 2";
            button8.Text = "1 , 3";
            button9.Text = "2 , 0";
            button10.Text = "2 , 1";
            button11.Text = "2 , 2";
            button12.Text = "2 , 3";
            button13.Text = "3 , 0";
            button14.Text = "3 , 1";
            button15.Text = "3 , 2";
            button16.Text = "3 , 3";
            Xtextbox.Text = "0";
            Ytextbox.Text = "0";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            State.isOk[0, 0] = (State.isOk[0, 0]) ? false : true;
            button1.BackColor = (button1.BackColor == red) ? green : red;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            State.isOk[0, 1] = (State.isOk[0, 1]) ? false : true;
            button2.BackColor = (button2.BackColor == red) ? green : red;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            State.isOk[0, 2] = (State.isOk[0, 2]) ? false : true;
            button3.BackColor = (button3.BackColor == red) ? green : red;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            State.isOk[0, 3] = (State.isOk[0, 3]) ? false : true;
            button4.BackColor = (button4.BackColor == red) ? green : red;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            State.isOk[1, 0] = (State.isOk[1, 0]) ? false : true;
            button5.BackColor = (button5.BackColor == red) ? green : red;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            State.isOk[1, 1] = (State.isOk[1, 1]) ? false : true;
            button6.BackColor = (button6.BackColor == red) ? green : red;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            State.isOk[1, 2] = (State.isOk[1, 2]) ? false : true;
            button7.BackColor = (button7.BackColor == red) ? green : red;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            State.isOk[1, 3] = (State.isOk[1, 3]) ? false : true;
            button8.BackColor = (button8.BackColor == red) ? green : red;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            State.isOk[2, 0] = (State.isOk[2, 0]) ? false : true;
            button9.BackColor = (button9.BackColor == red) ? green : red;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            State.isOk[2, 1] = (State.isOk[2, 1]) ? false : true;
            button10.BackColor = (button10.BackColor == red) ? green : red;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            State.isOk[2, 2] = (State.isOk[2, 2]) ? false : true;
            button11.BackColor = (button11.BackColor == red) ? green : red;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            State.isOk[2, 3] = (State.isOk[2, 3]) ? false : true;
            button12.BackColor = (button12.BackColor == red) ? green : red;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            State.isOk[3, 0] = (State.isOk[3, 0]) ? false : true;
            button13.BackColor = (button13.BackColor == red) ? green : red;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            State.isOk[3, 1] = (State.isOk[3, 1]) ? false : true;
            button14.BackColor = (button14.BackColor == red) ? green : red;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            State.isOk[3, 2] = (State.isOk[3, 2]) ? false : true;
            button15.BackColor = (button15.BackColor == red) ? green : red;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            State.isOk[3, 3] = (State.isOk[3, 3]) ? false : true;
            button16.BackColor = (button16.BackColor == red) ? green : red;
        }


    }

    internal class State
    {
        internal State next;
        private char name;
        private const int length = Maze.MazeLength + 1;
        private string way;
        private int x, y, lastMove;
        private int[,] Cross;
        internal static bool[,] isOk = new bool[length, length];
        internal State(char name, string way, int[,] C, int x, int y, int lastMove)
        {
            this.lastMove = lastMove;
            Cross = new int[length, length];
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                    Cross[i, j] = C[i, j];
            Cross[x, y]++;
            this.name = name;
            this.way = way + name;
            this.x = x;
            this.y = y;
        }
        internal string getWay()
        {
            return way;
        }
        internal int getLastMove()
        {
            return lastMove;
        }
        internal int getX()
        {
            return x;
        }
        internal int getY()
        {
            return y;
        }
        internal int[,] getCross()
        {
            return this.Cross;
        }
    }

    internal class Agenda
    {
        private State front, rear;
        internal Agenda(State currnetState)
        {
            front = currnetState;
            rear = currnetState;
            front.next = rear;
        }
        internal bool Add(char name, string way, int[,] Cross, int x, int y, int lastMove)
        {
            State s = new State(name, way, Cross, x, y, lastMove);
            rear.next = s;
            rear = s;
            return true;
        }
        internal bool Remove(ref State item)
        {
            if (front == null) return false;
            item = front;
            if (front == rear) rear = front = null;
            else front = front.next;
            return true;
        }
    }

}
