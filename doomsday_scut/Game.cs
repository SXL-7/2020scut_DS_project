using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Data.SQLite;

namespace doomsday_scut
{
    public partial class Game : Form
    {
        
        Player[] player = new Player[1];        //PCC数量暂定一个
        Map[] map = new Map[10];
        int animation_ctrl = 0;
        bool start = true;
        static int resolution_value = 64;
        static double resolution_rate = 96.0 / resolution_value;   //只改分母
        Bitmap fighter2 = new Bitmap(Properties.Resources._fighter2);
        struct pic
        {
            public string s;
            public int width;
            public int height;
            public pic(string s,int w,int h)
            {
                this.s = s;
                width = w;
                height = h;
            }
        }
        
        public Game()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
        }

        private void Game_KeyDown(object sender, KeyEventArgs e)
        {
            start = false;
            Point p = Player.key_ctrl(player, map, e);
            Draw();
            //MessageBox.Show(Convert.ToString(p.X) + "," + Convert.ToString(p.Y));
            Text = "Game  坐标表示：" + Convert.ToString(p.X) + "," + Convert.ToString(p.Y) ;
        }

        private void Game_Load(object sender, EventArgs e)
        {

            ProcessBar myprocessbar = new ProcessBar("Loading...", "Loading maps...");
            myprocessbar.Show();
            for (int i = 0; i < 10; i++)
            {
                map[i] = new Map(i);
                myprocessbar.setLabelText("Loading maps(" + Convert.ToString(i + 1) + "/10)...");
                myprocessbar.Refresh();
            }
            myprocessbar.Hide();

            player[0] = new Player();
            player[0].figure = new Bitmap(Properties.Resources._fighter2);
            player[0].figure.SetResolution(resolution_value, resolution_value);
            player[0].is_active = 1;
            
            Draw();
        }
        
        private void Game_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        public void Draw()
        {
            if(start)
                pictureBox1.Refresh();
            fighter2.SetResolution(resolution_value, resolution_value);
            //create image g1 on pictureBox1
            Graphics g1 = pictureBox1.CreateGraphics();
            //create image on RAM and let g be the image of picturebox1
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            BufferedGraphics myBuffer = currentContext.Allocate(g1, this.DisplayRectangle);
            //draw image
            animation_ctrl += 1;
            Map.Draw(map, player, myBuffer.Graphics, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            //Player.Draw(player,myBuffer.Graphics);
            //release resource
            myBuffer.Render();
            myBuffer.Dispose();
        }

        private void Game_KeyUp(object sender, KeyEventArgs e)
        {
            Player.key_ctrl_up(player, e);
            Draw();
        }

        public class Player
        {
            //Class of PCC Role
            public int x = 48;                      //x coordinate
            public int y = 48;                      //y coordinate
            public int coordinate_x = 1;
            public int coordinate_y = 2;
            public int face = 1;
            public int anm_frame = 0;               //current animation frame
            public long last_walk_time = 0;         //record last moving time point
            public long walk_interval = 100;        //set time interval between two steps
            public int speed = 48;

            public Bitmap figure;                   //figure picture
            public static int current_player = 0;   //current player
            public int is_active = 0;               //current player is active or not

            public Player()
            {
                figure = new Bitmap(Properties.Resources._fighter2);
                figure.SetResolution(resolution_value, resolution_value);
            }
            public static Point key_ctrl(Player[] player, Map[] map, KeyEventArgs e)
            {
                Point nullpoint = new Point(-1, -1);
                Player p = player[current_player];
                //change role （由于暂时不用该功能，所以先注释掉）
                //if (e.KeyCode == Keys.Tab)  {  key_change_player(player);  }
                
                //change face or not
                if (e.KeyCode == Keys.Up && p.face != 4) { p.face = 4; }
                if (e.KeyCode == Keys.Down && p.face != 1) { p.face = 1; }
                if (e.KeyCode == Keys.Left && p.face != 2) { p.face = 2; }
                if (e.KeyCode == Keys.Right && p.face != 3) { p.face = 3; }

                //time interval determination
                if (Comm.Time() - p.last_walk_time <= p.walk_interval)
                    return nullpoint;

                //move
                if (e.KeyCode == Keys.Up && Map.can_through(map,p.coordinate_x,p.coordinate_y - 1)) { p.y -= p.speed; }
                else if (e.KeyCode == Keys.Down && Map.can_through(map, p.coordinate_x, p.coordinate_y + 1)) { p.y += p.speed; }
                else if (e.KeyCode == Keys.Left && Map.can_through(map, p.coordinate_x - 1, p.coordinate_y)) { p.x -= p.speed; }
                else if (e.KeyCode == Keys.Right && Map.can_through(map, p.coordinate_x + 1, p.coordinate_y)) { p.x += p.speed; }
                else return nullpoint;

                //animation frame
                p.anm_frame += 1;
                if (p.anm_frame >= int.MaxValue) p.anm_frame = 0;
                //time
                p.last_walk_time = Comm.Time();
                p.coordinate_x = p.x / 48;
                p.coordinate_y = p.y / 48 + 1;
                return new Point(p.coordinate_x, p.coordinate_y);
            }
            public static void Draw(Player[] player, Graphics g, int map_sx,int map_sy)
            {
                Player p = player[current_player];
                Rectangle crazycoderRgl = new Rectangle(p.figure.Width / 4 * (p.anm_frame % 4), p.figure.Height / 4 * (p.face - 1), p.figure.Width / 4, p.figure.Height / 4);
                Bitmap b0 = p.figure.Clone(crazycoderRgl, p.figure.PixelFormat);
                g.DrawImage(b0, map_sx + p.x, map_sy + p.y);
            }

            public static void key_change_player(Player[] player)
            {
                //this function change the role
                for (int i = current_player + 1; i < player.Length; i++)
                {
                    if (player[i].is_active == 1)
                    {
                        set_player(player, current_player, i);
                        return;
                    }
                }
                for (int i = 0; i < current_player; i++)
                {
                    if (player[i].is_active == 1)
                    {
                        set_player(player, current_player, i);
                        return;
                    }
                }
            }

            public static void set_player(Player[] player, int oldindex, int newindex)
            {
                current_player = newindex;
                player[newindex].x = player[oldindex].x;
                player[newindex].y = player[oldindex].y;
                player[newindex].face = player[oldindex].face;
            }

            public static void key_ctrl_up(Player[] player, KeyEventArgs e)
            {
                //this function correct the face of the figure when the key is up
                Player p = player[current_player];
                //animation frame
                p.anm_frame = 0;
                p.last_walk_time = 0;
            }

            public static void set_position(Player[] player,int x,int y,int face)
            {
                player[current_player].x = x;
                player[current_player].y = y;
                player[current_player].face = face;
            }

            public static Point get_position(Player[] player)
            {
                return new Point(player[current_player].x, player[current_player].y);
            }

            public static int get_face(Player[] player)
            {
                return player[current_player].face;
            }

        }

        public class Map
        {
            //这个类负责地图管理与绘制
            public static int current_map = 4;          //current map number
            public string bitmap_path;
            public Bitmap bitmap;
            public static Bitmap map_resource = new Bitmap(Properties.Resources.map_test);
            public static Bitmap wall;
            public static Bitmap road;
            public static byte[] wallValue;
            public static byte[] roadValue;
            public static Rectangle wall_r = new Rectangle(64, 0, 32, 32);
            public static Rectangle road_r = new Rectangle(0, 0, 32, 32);
            public string mapdata_string;
            public int map_width;
            public int map_height;
            public int this_map_id;

            public Map(Bitmap bmp)
            {
                bitmap = bmp;
            }

            public Map(int id)
            {
                this_map_id = id;
                string path = @System.AppDomain.CurrentDomain.BaseDirectory+@"data.db";
                SQLiteConnection cn = new SQLiteConnection("data source=" + path);
                cn.Open();
                SQLiteCommand cmd = new SQLiteCommand("select * from map_data where map_id = " + Convert.ToString(this_map_id) + ";", cn);
                cmd.CommandType = CommandType.Text;
                SQLiteDataReader dReader = cmd.ExecuteReader();
                DataTable dTable = new DataTable();
                dTable.Load(dReader);
                mapdata_string = dTable.Rows[0]["data"].ToString();
                map_width = Convert.ToInt32(dTable.Rows[0]["width"].ToString());
                map_height = Convert.ToInt32(dTable.Rows[0]["height"].ToString());
                cn.Close();

                //以下三行为测试通过内存修改来初始化地图，但发现效率提升并不大，故暂时废弃
                //wallValue = bitmap2byte(wall, true);
                //roadValue = bitmap2byte(road, true);
                //bitmap = mapBuild(bmp, mapdata_matrix);
                bitmap = LoadImage(id);
                bitmap.SetResolution(resolution_value, resolution_value);
            }

            public static byte[] bitmap2byte(Bitmap bmp, bool is_unlock)
            {
                //lock BMP in RAM
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData bmpData =
                    bmp.LockBits(rect, ImageLockMode.ReadWrite,
                    bmp.PixelFormat);
                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;
                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                byte[] rgbValues = new byte[bytes];
                // Copy the RGB values into the array.  
                Marshal.Copy(ptr, rgbValues, 0, bytes);
                // Unlock the bits.解锁
                if (is_unlock)
                    bmp.UnlockBits(bmpData);
                return rgbValues;
            }

            public static Bitmap mapBuild(Bitmap bmp, int [,] mapdata_matrix)
            {
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);  
                IntPtr ptr = bmpData.Scan0;  
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                byte[] rgbValues = new byte[bytes];  
                Marshal.Copy(ptr, rgbValues, 0, bytes);

                // copy wall and road to original bmp
                for (int i = 0; i < rgbValues.Length; i += 4)
                {
                    if (mapdata_matrix[((i / 4) / bmp.Width) / 32, ((i / 4) % bmp.Width) / 32] == 0)
                    {
                        for (int j = i; j <= i + 3; j++)
                        {
                            rgbValues[j] = roadValue[(((i / 4) % bmp.Width) % 32) * road.Width + (((i / 4) / bmp.Width) % 32) + j - i];
                        }
                    }
                    else
                    {
                        for (int j = i; j <= i + 3; j++)
                        {
                            rgbValues[j] = wallValue[(((i / 4) / bmp.Width) % 32) * wall.Width + (((i / 4) % bmp.Width) % 32) + j - i];
                        }
                    }
                }
                // Copy the RGB values back to the bitmap 
                Marshal.Copy(rgbValues, 0, ptr, bytes);
                // Unlock the bits.  
                bmp.UnlockBits(bmpData);

                return bmp;
            }


            public static void Draw(Map[] map, Player[] player, Graphics g, Rectangle stage)
            {
                Map m = map[current_map];
                //drawing position
                int map_sx = 0;
                int map_sy = 0;
                int p_x = Player.get_position(player).X;
                int p_y = Player.get_position(player).Y;
                int map_w = Convert.ToInt32(Convert.ToDouble(m.bitmap.Width) * resolution_rate);
                int map_h = Convert.ToInt32(Convert.ToDouble(m.bitmap.Height) * resolution_rate);

                if(p_x <= stage.Width / 2)
                {
                    map_sx = 0;
                }
                else if (p_x >= map_w  - stage.Width  / 2)
                {
                    map_sx = stage.Width  - map_w ;
                }
                else
                {
                    map_sx = stage.Width / 2 - p_x ;
                }

                if(p_y <= stage.Height / 2)
                {
                    map_sy = 0;
                }
                else if (p_y >= map_h  - stage.Height / 2)
                {
                    map_sy = stage.Height - map_h ;
                }
                else
                {
                    map_sy = stage.Height / 2 - p_y ;
                }
                //draw
                g.DrawImage(m.bitmap, map_sx, map_sy);
                Player.Draw(player, g, map_sx, map_sy);
            }

            public static void change_map(Map[] map, Player[] player, int newindex, int x, int y, int face)
            {
                //unload old map resource
                if (map[current_map].bitmap!=null)
                {
                    map[current_map].bitmap = null;
                }
                //load new map resource
                map[newindex].bitmap = new Bitmap(map[newindex].bitmap_path);
                map[newindex].bitmap.SetResolution(resolution_value, resolution_value);
                //current_map
                current_map = newindex;
                Player.set_position(player, x, y, face);
            }
            public static bool can_through(Map[] map,int x,int y)
            {
                Map m = map[current_map];
                if (x < 0) return false;
                else if (x >= m.map_width) return false;
                else if (y < 0) return false;
                else if (y >= m.map_height) return false;

                if (m.mapdata_string[y * m.map_width + x] == '1')
                    return false;
                else
                    return true;
                
            }
            public void WriteMessage(string msg)
            {
                using (FileStream fs = new FileStream(@"e:\test.txt", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.BaseStream.Seek(0, SeekOrigin.End);
                        sw.WriteLine("{0}\n", msg, DateTime.Now);
                        sw.Flush();
                    }
                }
            }
            //SQLITE FUNCTION
            public static byte[] ImageToByte(Image image, System.Drawing.Imaging.ImageFormat format)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Convert Image to byte[]
                    image.Save(ms, format);
                    byte[] imageBytes = ms.ToArray();
                    return imageBytes;
                }
            }
            //SQLITE FUNCTION
            public static Image ByteToImage(byte[] imageBytes)
            {
                // Convert byte[] to Image
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = new Bitmap(ms);
                return image;
            }
            //SQLITE FUNCTION
            public static void SaveImage(byte[] imagen,int map_id)
            {
                string conStringDatosUsuarios = @" Data Source = "+ @System.AppDomain.CurrentDomain.BaseDirectory + @"data.db";
                SQLiteConnection con = new SQLiteConnection(conStringDatosUsuarios);
                SQLiteCommand cmd = con.CreateCommand();
                cmd.CommandText = String.Format("UPDATE map_data SET map_image_block_data = (@0) WHERE map_id = " + Convert.ToString(map_id) + ";");
                SQLiteParameter param = new SQLiteParameter("@0", System.Data.DbType.Binary);
                param.Value = imagen;
                cmd.Parameters.Add(param);
                con.Open();

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exc1)
                {
                    MessageBox.Show(exc1.Message);
                }
                con.Close();
            }
            //SQLITE FUNCTION
            Bitmap LoadImage(int map_id)
            {
                Bitmap[] ans = new Bitmap[1];
                string query = "SELECT map_image_data FROM map_data WHERE map_id=" + Convert.ToString(map_id) + ";";
                string conString = @" Data Source = "+ @System.AppDomain.CurrentDomain.BaseDirectory + @"data.db";
                SQLiteConnection con = new SQLiteConnection(conString);
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                con.Open();
                try
                {
                    IDataReader rdr = cmd.ExecuteReader();
                    try
                    {
                        while (rdr.Read())
                        {
                            byte[] a = (System.Byte[])rdr[0];
                            ans[0] = new Bitmap(ByteToImage(a));
                        }
                    }
                    catch (Exception exc) { MessageBox.Show(exc.Message); }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                con.Close();
                return ans[0];
            }

            public static Bitmap string2bitmap(string mapdata_string,int map_width,int map_height)
            {
                //这个函数用于将01string转化为bitmap便于存储
                int [,]mapdata_matrix = new int[map_height, map_width];
                for (int i = 0; i < map_height; i++)
                {
                    for (int j = 0; j < map_width; j++)
                    {
                        mapdata_matrix[i, j] = Convert.ToInt32(mapdata_string[i * map_width + j]) - '0';
                    }
                }
                
                //bmp是要被贴图的对象
                Bitmap bmp = new Bitmap(mapdata_matrix.GetLength(1) * 32, mapdata_matrix.GetLength(0) * 32);
                wall = map_resource.Clone(wall_r, map_resource.PixelFormat);
                road = map_resource.Clone(road_r, map_resource.PixelFormat);
                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {

                        if (i / 32 >= mapdata_matrix.GetLength(0) || j / 32 >= mapdata_matrix.GetLength(1))
                        {
                            continue;
                        }
                        if (mapdata_matrix[i / 32, j / 32] == 0)
                        {
                            bmp.SetPixel(j, i, road.GetPixel(j % 32, i % 32));
                            //bmp.SetPixel(j, i, Color.White);

                        }
                        else
                        {
                            bmp.SetPixel(j, i, wall.GetPixel(j % 32, i % 32));
                            //bmp.SetPixel(j, i, Color.Black);

                        }
                    }
                }
                return bmp;
            }
        }
    }
}
