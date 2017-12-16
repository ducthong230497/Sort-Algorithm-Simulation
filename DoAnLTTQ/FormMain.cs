using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using DoAnLTTQ;
using System.Threading;
using System.Diagnostics;
using Transitions;


namespace DoAnLTTQ
{
    public partial class FormMain : Form
    {
        FormThuatToanB FormThuatToanB;
        string DaySo;       //DaySo chỉ dùng để lưu dãy số đang nằm trong textBoxDaySo để thao tác tiếp
        int[] DaySoNguyen;  //là dãy số chính để thao tác cho thuật toán A
        int[] DaySoNguyen2;  //là dãy số chính để thao tác cho thuật toán B
        int[] DaySoTemp;    //DaySoTemp dùng để lưu dãy số chính(DaySoNguyen) trong các thao tác: nạp, random, làm lại cho thuật toán A
        int[] DaySoTemp2;    //DaySoTemp dùng để lưu dãy số chính(DaySoNguyen) trong các thao tác: nạp, random, làm lại cho thuật toán B
        O_vuong[] HinhVuongSo;
        O_vuong[] HinhVuongSo2;
        Label[] posSquare;                  //mảng label chứa label i, j cho HinhVuongSo
        Label[] posSquare2;                  //mảng label chứa label i, j cho HinhVuongSo2        
        bool daKhoiTaoiVaj = false;
        Label labeli=new Label();
        Label labelj = new Label();
        Label labeli2 = new Label();
        Label labelj2 = new Label();
        System.Threading.Thread ThreadThuatToanA; //thread dùng để chạy thuật toán A trong chế độ so sánh
        System.Threading.Thread ThreadThuatToanB; //thread dùng để chạy thuật toán B trong chế độ so sánh
        BackgroundWorker backgroundWorkerA;
        string[] lines;                     //mảng  dùng để highlight
        Color colorhighlight = Color.Aqua;  //màu highlight        

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {            

            
        }

        public FormMain()
        {
            InitializeComponent();
            for (int i = 0; i < DoAnLTTQ.Load.ThuatToan.Length; i++)
            {
                ComboBoxThuatToan.Items.Add(DoAnLTTQ.Load.ThuatToan[i]);
                ComboBoxThuatToan2.Items.Add(DoAnLTTQ.Load.ThuatToan[i]);
            }
            ComboBoxThuatToan.SelectedIndex = 0;
            ComboBoxThuatToan2.SelectedIndex = 0;

            backgroundWorkerA = new BackgroundWorker();

            backgroundWorkerA.DoWork += backgroundWorker1_DoWork;
        }

        //mới trong load tab auto với debug
#region Thao tác cơ bản trên form

        /// <summary>
        /// Hiển thị code thuật toán A tương ứng với thuật toán được chọn trong combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxThuatToan_SelectedValueChanged(object sender, EventArgs e)
        {
            DoAnLTTQ.Load.LoadThuatToan(ComboBoxThuatToan, richTextBoxCodeThuatToan);
        }


        /// <summary>
        /// Hàm sự kiện ấn nút X để tắt chương trình
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            //Cho xuất hiện khung thông báo hỏi người dùng có muốn thoát
            KhungThongBao ktb = new KhungThongBao("Thoát", "Bạn có muốn thoát không ?", true, true);

            //Nếu đang chạy thuật toán auto thì dừng chạy
            if (buttonNgung.Enabled == true)
            {
                DungThuatToan();
            }
            
            if (continueAutoFlag1 != null)
            {
                continueAutoFlag1.Cancel();
            }            
            if (ktb.ShowDialog() == DialogResult.OK)
            {
                Application.Exit();
            }
        }


        /// <summary>
        /// Hàm sự kiện ấn nút thu nhỏ form xuống taskbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        //Khai báo biến cờ cho biết form có được phóng to chưa
        bool isMaximized = false;

        /// <summary>
        /// Hàm sự kiện ấn nút phóng to/vừa form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxZoom_Click(object sender, EventArgs e) //Phóng to, thu nhỏ màn hình
        {
            if (!isMaximized)
            {

                this.WindowState = FormWindowState.Maximized;
                isMaximized = true;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;                
                isMaximized = false;                
            }
        }


        #region Sự kiện liên quan đến di chuyển vị trí form
        /// <summary>
        /// Kiểm tra chuột có được ấn giữ tại panel3 (thanh chứa các dấu thu nhỏ,...)
        /// </summary>
        bool mouseDown;
        Point MouseLo;
        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            MouseLo = new Point(e.X, e.Y);
        }

        /// <summary>
        /// Di chuyển form theo con chuột nếu con chuột đang được ấn giữ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown == true)
            {
                this.Location = new Point(this.Location.X + e.X - MouseLo.X, this.Location.Y + e.Y - MouseLo.Y);
                this.Update();
            }
        }

        /// <summary>
        /// Kiểm tra nếu con chuột không còn được ấn giữ tại panel3 nữa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel3_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        #endregion


        /// <summary>
        /// Hàm sự kiện load form
        /// Mặc định chế độ chạy từng bước thuật toán sẽ được hiện
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Control x in this.Controls)
            {
                if (x is TextBox || x is ComboBox || x is Label || x is Button || x is RadioButton || x is RichTextBox)
                {
                    x.Visible = false;
                }
            }
            //Ẩn thanh tốc độ
            pictureBoxSpeed.Visible = false;
            pictureBoxSpeedPoint.Visible = false;

            //Ẩn label, combobox thuật toán 2
            Transition t = new Transition(new TransitionType_Deceleration(300));
            t.add(labelChonTT2, "Top", -30);
            t.add(ComboBoxThuatToan2, "Top", -35);
            t.run();

            //Mặc định interval tại mức cao nhất
            timer1.Interval = 50;
            timer2.Interval = 50;

            pictureBox1.Visible = false;
            HinhVuongSo = new O_vuong[10];
            HinhVuongSo2 = new O_vuong[10];            

            posSquare = new Label[10];
            posSquare2 = new Label[10];

            for (int i = 0; i < 10; i++)
            {
                HinhVuongSo[i] = new O_vuong();
                HinhVuongSo2[i] = new O_vuong();
                posSquare[i] = new Label();
                posSquare2[i] = new Label();
            }


            buttonTiepTuc.Click += new EventHandler(buttonTieptuc_Click);

            //Khi mới bật chương trình mặc định hiện
            //tab debug
            PictureBoxDebug_Click(null, null);
        }


        /// <summary>
        /// Hàm sự kiện ấn picturebox so sánh để chuyển sang chế độ chạy thuật toán từng bước
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBoxDebug_Click(object sender, EventArgs e)
        {
            // Đặt là cờ chế độ so sánh
            isSoSanhMode = false;

            PictureBoxAuto.BackColor = System.Drawing.Color.Gray;
            PictureBoxDebug.BackColor = System.Drawing.Color.FromArgb(66, 131, 221);
            pictureBoxSoSanh.BackColor = System.Drawing.Color.Gray;
            foreach (Control x in this.Controls)
            {
                if (x is TextBox || x is ComboBox || x is Label || x is Button || x is RadioButton || x is RichTextBox)
                {
                    x.Visible = true;
                }
            }

            //Ẩn thanh tốc độ
            labelTocDo.Visible = false;
            pictureBoxSpeed.Visible = false;
            labelSpeed0.Visible = false;
            labelSpeed1.Visible = false;
            labelSpeed2.Visible = false;
            labelSpeed3.Visible = false;
            labelSpeed4.Visible = false;
            pictureBoxSpeedPoint.Visible = false;
            waitAmount = 0; //nếu đang thực hiện step by step thì ko cần chờ giữa các dòng trong thuật toán

            //Di chuyển các nút bắt đầu, làm lại xuống phía dưới
            buttonBatDau.Location = new Point(944, 460);
            ButtonLamLai.Location = new Point(944, 633);

            //Ẩn label, combobox thuật toán 2
            Transition t = new Transition(new TransitionType_Deceleration(500));
            t.add(labelChonTT2, "Top", -30);
            t.add(ComboBoxThuatToan2, "Top", -35);

            //Hiện richtextboxHighlight, ẩn panelHienThiOvuong2
            t.add(panelHienThiOVuong2, "Top", 740);
            t.add(richTextBoxCodeThuatToan, "Top", 460);
            t.run();

            labelThoiGianSoSanh1.Visible = false;
            labelThoiGianSoSanh2.Visible = false;
            pictureBox1.Visible = true;
            buttonBatDau.Visible = false;
            buttonTiepTuc.Visible = false;
            buttonNgung.Visible = false;
            RadioButtonTangDan.Checked = true;
            textBoxDaySo.Focus();
            panelHienThiOVuong2.Location = new Point(154, 750);
        }


        /// <summary>
        /// Hàm sự kiện ấn picturebox so sánh để chuyển sang chế độ chạy thuật toán tự động
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBoxAuto_Click(object sender, EventArgs e)
        {
            // Đặt là cờ chế độ so sánh
            isSoSanhMode = false;

            PictureBoxDebug.BackColor = System.Drawing.Color.Gray;
            PictureBoxAuto.BackColor = System.Drawing.Color.FromArgb(66, 131, 221);
            pictureBoxSoSanh.BackColor = System.Drawing.Color.Gray;
            foreach (Control x in this.Controls)
            {
                if (x is TextBox || x is ComboBox || x is Label || x is Button || x is RadioButton || x is RichTextBox)
                {
                    x.Visible = true;
                }
            }

            //Hiện thanh tốc độ
            labelTocDo.Visible = true;
            pictureBoxSpeed.Visible = true;
            labelSpeed0.Visible = true;
            labelSpeed1.Visible = true;
            labelSpeed2.Visible = true;
            labelSpeed3.Visible = true;
            labelSpeed4.Visible = true;
            pictureBoxSpeedPoint.Visible = true;

            //Mặc định thời gian chờ là 0.5 giây = 500 miligiay
            waitAmount = 5;

            //Di chuyển các nút bắt đầu, làm lại
            buttonBatDau.Location = new Point(944, 460);
            ButtonLamLai.Location = new Point(944, 633);

            //Ẩn label, combobox thuật toán 2
            Transition t = new Transition(new TransitionType_Deceleration(500));
            t.add(labelChonTT2, "Top", -30);
            t.add(ComboBoxThuatToan2, "Top", -35);

            //Hiện richtextboxHighlight, ẩn panelHienThiOvuong2
            t.add(panelHienThiOVuong2, "Top", 740);
            t.add(richTextBoxCodeThuatToan, "Top", 460);
            t.run();

            labelThoiGianSoSanh1.Visible = false;
            labelThoiGianSoSanh2.Visible = false;
            ButtonTien.Visible = false;            
            pictureBox1.Visible = true;
            RadioButtonTangDan.Checked = true;
            textBoxDaySo.Focus();
            panelHienThiOVuong2.Location = new Point(154, 750);
        }

        // Biến cờ báo hiệu đang trong chế độ so sánh
        bool isSoSanhMode = false;
        /// <summary>
        /// Hàm sự kiện ấn picturebox so sánh để chuyển sang chế độ so sánh 2 thuật toán
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxSoSanh_Click(object sender, EventArgs e)
        {
            // Đặt là cờ chế độ so sánh
            isSoSanhMode = true;

            PictureBoxDebug.BackColor = System.Drawing.Color.Gray;
            PictureBoxAuto.BackColor = System.Drawing.Color.Gray;
            pictureBoxSoSanh.BackColor = System.Drawing.Color.FromArgb(66, 131, 221);

            // Cho label, combobox thuật toán 2 xuất hiện
            labelChonTT2.Location = new Point(509, -30);
            labelChonTT2.Visible = true;
            ComboBoxThuatToan2.Location = new Point(662, -35);
            ComboBoxThuatToan2.Visible = true;

            // Hiện label, combobox thuật toán 2
            Transition t = new Transition(new TransitionType_Deceleration(500));
            t.add(labelChonTT2, "Top", 38);
            t.add(ComboBoxThuatToan2, "Top", 38);

            //Thanh tốc độ
            labelTocDo.Visible = false;
            pictureBoxSpeed.Visible = false;
            labelSpeed0.Visible = false;
            labelSpeed1.Visible = false;
            labelSpeed2.Visible = false;
            labelSpeed3.Visible = false;
            labelSpeed4.Visible = false;
            pictureBoxSpeedPoint.Visible = false;

            //Mặc định thời gian chờ trong khi so sánh là 0.5 giây
            waitAmount = 5;

            //Di chuyển nút bắt đầu lên phía trên
            buttonBatDau.Visible = true;
            buttonBatDau.Location = new Point(278, 128);
            ButtonLamLai.Visible = true;
            ButtonLamLai.Location = new Point(549, 133);

            //Ẩn richtextboxHighlight, hiện panelHienThiOvuong2
            t.add(panelHienThiOVuong2, "Top", 460);
            t.add(richTextBoxCodeThuatToan, "Top", 740);
            t.run();

            //Ẩn các nút bắt đầu, ngưng, tiến
            ButtonTien.Visible = false;
            buttonNgung.Visible = false;
            buttonTiepTuc.Visible = false;            

            labelThoiGianSoSanh1.Visible = false;
            labelThoiGianSoSanh2.Visible = false;
            pictureBox1.Visible = true;
            RadioButtonTangDan.Checked = true;
            textBoxDaySo.Focus();            
        }


        /// <summary>
        /// Ngăn không cho người dùng nhập ký tự khác số vào textboxNhapDaySo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxDaySo_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Ngăn không cho nhập các phím control,..., các phím chữ
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Space)
            {
                e.Handled = true;
            }
        }


        #region Đổi thứ tự sắp xếp, enable nút bắt đầu
        private void RadioButtonGiamDan_CheckedChanged(object sender, EventArgs e)
        {
            if (input.initArray)
            {
                buttonBatDau.Enabled = true;
            }
        }
        private void RadioButtonTangDan_CheckedChanged(object sender, EventArgs e)
        {
            if (input.initArray)
            {
                buttonBatDau.Enabled = true;
            }
        }
        #endregion


        /// <summary>
        /// Hàm dùng để gán button Random dãy số vào phím F5
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F5))
            {
                ButtonRandom.PerformClick();
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }


        #region Hiệu ứng ControlBox
        #region Đổi màu nút tắt form khi di chuột vào
        private void pictureBoxClose_MouseEnter(object sender, EventArgs e)
        {
            pictureBoxClose.BackColor = Color.FromArgb(242, 60, 45);
        }
        #endregion

        #region Đổi màu nút tắt khi di chuột khỏi
        private void pictureBoxClose_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxClose.BackColor = Color.FromArgb(68, 114, 196);
        }
        #endregion

        #region Đổi màu nút tắt khi ấn chuột
        private void pictureBoxClose_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBoxClose.BackColor = Color.Red;
        }
        #endregion

        #region Đổi màu nút thu nhỏ khi di chuột vào
        private void pictureBoxMinimize_MouseEnter(object sender, EventArgs e)
        {
            pictureBoxMinimize.BackColor = Color.FromArgb(68, 114, 150);
        }
        #endregion

        #region Đổi màu nút thu nhỏ khi di chuột khỏi
        private void pictureBoxMinimize_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxMinimize.BackColor = Color.FromArgb(68, 114, 196);
        }
        #endregion

        #region Đổi màu nút phóng to khi di chuột vào
        private void pictureBoxZoom_MouseEnter(object sender, EventArgs e)
        {
            pictureBoxZoom.BackColor = Color.FromArgb(68, 114, 150);
        }
        #endregion

        #region Đổi màu nút phóng to khi di chuột khỏi
        private void pictureBoxZoom_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxZoom.BackColor = Color.FromArgb(68, 114, 196);
        }
        #endregion

        #region hiện pop up
        ToolTip tip = new ToolTip();
        private void pictureBoxMinimize_MouseHover(object sender, EventArgs e)
        {
            tip.Show("Minimize", pictureBoxMinimize);
        }

        private void pictureBoxZoom_MouseHover(object sender, EventArgs e)
        {
            tip.Show("Maximize", pictureBoxZoom);
        }

        private void pictureBoxClose_MouseHover(object sender, EventArgs e)
        {
            tip.Show("Close", pictureBoxClose);
        }
        #endregion
        #endregion
        #endregion


        /// <summary>
        /// Hàm vẽ hình vuông
        /// </summary>
        /// <param name="DaySoNguyen">biến mảng chứa dãy số đã nhập</param>
        /// <param name="HinhVuongSo">biến mảng chứa các control là các label ô vuông</param>
        /// <param name="panelHienThiHinhVuong">biến panel để chứa các hình vuông</param>
        private void makeNumSquare(int[] DaySoNguyen, O_vuong[] HinhVuongSo, Panel panelHienThiHinhVuong)
        {
            int X = -75;
            int Y;
            if (ComboBoxThuatToan.SelectedItem.ToString() == "Merge sort")
            {
                Y = 80;
            }
            else
            {
                Y = 60;
            }
            for (int i = 0; i < DaySoNguyen.Length; i++)   //Thêm vào form các hình vuông cần dùng
            {
                HinhVuongSo[i].Location = new Point(X+=90, Y);
                HinhVuongSo[i].Text = DaySoNguyen[i].ToString();
                panelHienThiHinhVuong.Controls.Add(HinhVuongSo[i]);
                HinhVuongSo[i].layGiaTriToaDoBanDau();
                HinhVuongSo[i].BackColor = Color.Gray;
                HinhVuongSo[i].thietLaplaiStatus();
            }

            for (int i = DaySoNguyen.Length; i < 10; i++)   //Loại khỏi form các hình vuông không cần dùng
            {
                panelHienThiHinhVuong.Controls.Remove(HinhVuongSo[i]);
            }
        }


        /// <summary>
        /// Hàm dùng để vẽ ô vị trí i, j
        /// </summary>
        /// <param name="DaySoNguyen"></param>
        /// <param name="lbposSquare"></param>
        /// <param name="panelHienThiHinhVuong"></param>
        private void makePosSquare(int[] DaySoNguyen, Label[] lbposSquare, Panel panelHienThiHinhVuong)
        {
            int X = -65;
            int Y = 220;
     
            for (int i = 0; i < DaySoNguyen.Length; i++)  //Thêm vào form các hình vuông cần dùng
            {            
                lbposSquare[i].Visible = true;
                lbposSquare[i].Location = new Point(X += 90, Y);
                lbposSquare[i].Text = i.ToString();
                panelHienThiHinhVuong.Controls.Add(lbposSquare[i]);
                lbposSquare[i].AutoSize = true;
                lbposSquare[i].ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                lbposSquare[i].Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
                lbposSquare[i].Size = new System.Drawing.Size(28, 31);
                lbposSquare[i].TabIndex = 0;
            }
            
            for (int i = DaySoNguyen.Length; i < 10; i++)  //Loại khỏi form các hình vuông không cần dùng
            {
                panelHienThiHinhVuong.Controls.Remove(lbposSquare[i]);
            }
        }


        /// <summary>
        /// Hàm sự kiện ấn nút nạp dãy số
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonNap_Click(object sender, EventArgs e)
        {
            DaySo = textBoxDaySo.Text;   //Lưu chuỗi ký tự từ textbox vào biến DaySo
            DaySo = DaySo.Trim();        //Bỏ dấu cách ở đầu chuỗi và cuối chuỗi (nếu có)

            //Xử lý trường hợp người dùng ấn nhiều dấu cách (spacebar)
            while (DaySo.Contains("  "))
            {
                DaySo = DaySo.Replace("  ", " ");
            }

            //Kiểm tra xem người dùng có nhập gì chưa
            if (DaySo == "")
            {
                KhungThongBao ktb = new KhungThongBao("Lỗi", "Chưa có dữ liệu gì được nhập.", true, false);
            }
            else
            {
                string[] Temp = DaySo.Split(' ');  //Lấy từng số ra từ chuỗi DaySo phân biệt bởi dấu cách và lưu vào mảng chuỗi ký tự DaySoTemp

                //Kiểm tra xem người dùng có nhập quá số lượng phần tử hay không?                
                if (Temp.Length > 10)
                {
                    textBoxDaySo.Clear();
                    KhungThongBao ktb = new KhungThongBao("Lỗi", "Số lượng phần tử nhập quá số lượng quy định (tối đa 10). Vui lòng nhập lại.", true, false);
                    ktb.ShowDialog();
                    return;
                }

                //Nếu nhập đúng số lượng phần tử quy định thì đi tiếp                
                DaySoNguyen = new int[Temp.Length];
                for (int i = 0; i < Temp.Length; i++)
                {
                    DaySoNguyen[i] = int.Parse(Temp[i]);
                }

                //Kiểm tra xem người có nhập số từ 1-99 không                
                for (int i = 0; i < DaySoNguyen.Length; i++)
                {
                    if (DaySoNguyen[i] < 0 || DaySoNguyen[i] > 99)
                    {
                        KhungThongBao ktb = new KhungThongBao("Lỗi", "Chỉ dược nhập các số từ 1-99. Vui lòng nhập lại", true, false);
                        ktb.ShowDialog();
                        return;
                    }
                }

                //Thực hiện sao chép dãy số đã qua kiểm tra
                //vào các dãy số liên quan
                DaySoTemp = new int[DaySoNguyen.Length];
                DaySoNguyen2 = new int[DaySoNguyen.Length];
                for (int i = 0; i < DaySoNguyen.Length; i++)
                {
                    DaySoTemp[i] = DaySoNguyen[i];
                    DaySoNguyen2[i] = DaySoNguyen[i];
                }
                textBoxDaySo.Text = DaySo;

                //Làm xuất hiện các hình vuông, label i, j cho thuật toán A
                makeNumSquare(DaySoNguyen, HinhVuongSo, panelHienThiOVuong);
                makePosSquare(DaySoNguyen, posSquare, panelHienThiOVuong);

                //Làm xuất hiện các hình vuông, label i, j cho thuật toán B
                makeNumSquare(DaySoNguyen2, HinhVuongSo2, panelHienThiOVuong2);
                makePosSquare(DaySoNguyen2, posSquare2, panelHienThiOVuong2);

                // cho phép bắt đầu chạy auto sau khi nạp dãy số
                buttonBatDau.Enabled = true;
                buttonBatDau.BackgroundImage = Properties.Resources.BatDauEnable;

                // cho phép chạy từng bước sau khi nạp dãy số
                ButtonTien.Enabled = true;
                ButtonTien.BackgroundImage = Properties.Resources.TienEnable;
            }
        }


        inputSoLuongPhanTuRandom input = new inputSoLuongPhanTuRandom();
        /// <summary>
        /// Hàm dùng để mở form random dãy số
        /// và random dãy số
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRandom_Click(object sender, EventArgs e)
        {
            if (input.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            DaySoNguyen = new int[input.SoLuongPhanTu]; //khởi tạo mảng dãy số thuật toán A dựa vào biến SoLuongPhanTu từ form Random
            DaySoNguyen2 = new int[input.SoLuongPhanTu]; //khởi tạo mảng dãy số thuật toán A dựa vào biến SoLuongPhanTu từ form Random
            DaySoTemp = new int[input.SoLuongPhanTu];   //khởi tạo dựa vào biến SoLuongPhanTu từ form Random
            DaySoTemp2 = new int[input.SoLuongPhanTu];   //khởi tạo dựa vào biến SoLuongPhanTu từ form Random
            Random rnd = new Random();
            for (int i = 0; i < DaySoNguyen.Length; i++)//tiến hành random
            {
                DaySoNguyen[i] = rnd.Next(0, 99);
                DaySoNguyen2[i] = DaySoNguyen[i];
                DaySoTemp[i] = DaySoNguyen[i];
                DaySoTemp2[i] = DaySoNguyen[i];
            }
            //Làm xuất hiện các hình vuông, label i, j cho thuật toán A
            makeNumSquare(DaySoNguyen, HinhVuongSo, panelHienThiOVuong);
            makePosSquare(DaySoNguyen, posSquare, panelHienThiOVuong);

            //Làm xuất hiện các hình vuông, label i, j cho thuật toán B
            makeNumSquare(DaySoNguyen2, HinhVuongSo2, panelHienThiOVuong2);
            makePosSquare(DaySoNguyen2, posSquare2, panelHienThiOVuong2);

            if (input.btnBatdau == true)
            {
                buttonBatDau.Enabled = true;            //nếu đã nhập đúng số lượng phần tử thì cho phép ấn nút Bắt đầu
                buttonBatDau.Focus();                   //focus vào nút bắt đầu
                ButtonTien.Enabled = true;
                ButtonTien.Focus();
            }
        }


        OpenFileDialog ofd = new OpenFileDialog();
        /// <summary>
        /// Hàm dùng để nhập dãy số từ file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileButton_Click(object sender, EventArgs e)
        {
            string text = "";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string file = ofd.FileName;
                try
                {
                    text = File.ReadAllText(file);
                }
                catch (IOException)
                {
                }
            }
            // Copy đoạn code ở phần NapButton_Click()
            DaySo = text;                                      //Lưu chuỗi ký tự từ textbox vào biến DaySo

            // Xử lý trường hợp người dùng nhập ký tự khác số
            DaySo = Regex.Replace(DaySo, "[^.0-9]", " ");

            DaySo = DaySo.Trim();                                           //Bỏ dấu cách ở đầu chuỗi và cuối chuỗi (nếu có)

            //Xử lý trường hợp người dùng ấn nhiều dấu cách (spacebar)
            while (DaySo.Contains("  "))
            {
                DaySo = DaySo.Replace("  ", " ");
            }

            //Kiểm tra xem người dùng có nhập gì chưa
            if (DaySo == "")
            {
                KhungThongBao ktb = new KhungThongBao("Lỗi", "Chưa có dữ liệu gì được nhập.", true, false);
                ktb.ShowDialog();
            }
            else
            {
                string[] Temp = DaySo.Split(' ');                      //Lấy từng số ra từ chuỗi DaySo phân biệt bởi dấu cách và lưu vào mảng chuỗi ký tự DaySoTemp
                //Kiểm tra xem người dùng có nhập quá số lượng phần tử hay không?
                bool TriggerGioiHanSoPhanTu = true;
                if (Temp.Length > 10)
                {
                    TriggerGioiHanSoPhanTu = false;
                    textBoxDaySo.Clear();
                    KhungThongBao ktb = new KhungThongBao("Lỗi", "Số lượng phần tử nhập quá số lượng quy định (tối đa 10). Vui lòng nhập lại.", true, false);
                    ktb.ShowDialog();
                }

                //Nếu nhập đúng số lượng phần tử quy định thì đi tiếp
                if (TriggerGioiHanSoPhanTu == true)
                {
                    buttonBatDau.Enabled = true;
                    DaySoNguyen = new int[Temp.Length];
                    for (int i = 0; i < Temp.Length; i++)
                    {
                        DaySoNguyen[i] = int.Parse(Temp[i]);
                    }

                    //Kiểm tra xem người có nhập số từ 1-99 không
                    bool TriggerGioiHanSo = true;
                    for (int i = 0; i < DaySoNguyen.Length; i++)
                    {
                        if (DaySoNguyen[i] < 0 || DaySoNguyen[i] > 99)
                        {
                            TriggerGioiHanSo = false;
                            KhungThongBao ktb = new KhungThongBao("Lỗi", "Chỉ dược nhập các số từ 1-99. Vui lòng nhập lại", true, false);
                            ktb.ShowDialog();
                            break;
                        }
                    }
                    if (TriggerGioiHanSo == true)
                    {
                        DaySoTemp = new int[DaySoNguyen.Length];
                        DaySoNguyen2 = new int[DaySoNguyen.Length];
                        for (int i = 0; i < DaySoNguyen.Length; i++)
                        {
                            DaySoTemp[i] = DaySoNguyen[i];
                            DaySoNguyen2[i] = DaySoNguyen[i];
                        }
                        textBoxDaySo.Text = DaySo;

                        //Làm xuất hiện các hình vuông, label i, j cho thuật toán A
                        makeNumSquare(DaySoNguyen, HinhVuongSo, panelHienThiOVuong);
                        makePosSquare(DaySoNguyen, posSquare, panelHienThiOVuong);

                        //Làm xuất hiện các hình vuông, label i, j cho thuật toán B
                        makeNumSquare(DaySoNguyen, HinhVuongSo2, panelHienThiOVuong2);
                        makePosSquare(DaySoNguyen, posSquare2, panelHienThiOVuong2);

                        buttonBatDau.Enabled = true;
                        ButtonTien.Enabled = true;
                    }
                }
            }
        }

        
        /// <summary>
        /// Hàm dùng để ngưng việc thực hiện code trong 
        /// seconds giây cho chế độ auto
        /// </summary>
        /// <param name="seconds">5 = 0.5 giây, 10 = 1 giây, 15 = 1.5 giây</param>
        /// <returns></returns>
        public async Task waitNSeconds(int seconds) 
        {
            await Task.Delay(seconds*100,continueAutoFlag1.Token);
        }


        /// <summary>
        /// Hàm enable các control sau khi hoàn thành sắp xếp
        /// </summary>
        private void enableControlsAfterFinish()
        {

            isStepbyStepStarted = false;    //set lại bằng false cho lần thực hiện mới tiếp

            //đã hoàn thành bước cuối cùng thì ko thể tiến thêm đc nữa
            ButtonTien.Enabled = false;
            ButtonTien.BackgroundImage = Properties.Resources.TienDisable;

            //ko cho phép bắt đầu với dãy số đã hoàn thành
            buttonBatDau.Enabled = false;
            buttonBatDau.BackgroundImage = Properties.Resources.BatDauDisable;

            //ko cho phép ngưng khi chưa bắt đầu
            buttonNgung.Enabled = false;
            buttonNgung.BackgroundImage = Properties.Resources.NgungDisable;

            PictureBoxDebug.Enabled = true;
            PictureBoxAuto.Enabled = true;
            pictureBoxSoSanh.Enabled = true;

            //cho phép nạp dãy số, chọn thuật toán, chọn thứ tự khi hoàn thành
            ButtonNap.Enabled = true;
            ButtonRandom.Enabled = true;
            fileButton.Enabled = true;
            ComboBoxThuatToan.Enabled = true;
            RadioButtonGiamDan.Enabled = true;
            RadioButtonTangDan.Enabled = true;
            ButtonLamLai.Enabled = true;
            ButtonLamLai.BackgroundImage = Properties.Resources.LamLaiEnable;
        }


        /// <summary>
        /// Hàm dùng để đợi 
        /// </summary>
        /// <returns></returns>
        async Task waitForInput()
        {
            if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
            {
                try
                {
                    await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
                }
                catch
                {

                }
                continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
            }
            else
            {
                //nếu ko phải đang chạy step by step thì là đang chạy auto
                try
                {
                    await waitNSeconds(waitAmount);
                }
                catch
                {

                }                
                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
                {
                    try
                    {
                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
                    }
                    catch
                    {

                    }
                    // nếu nút ngưng đc bấm và sau đó nút tiếp 
                    // tục đc ấn thì phải tạo lại token
                    continueAutoFlag = new CancellationTokenSource();
                    continueAutoFlag1 = new CancellationTokenSource();
                }
            } 
        }


#region Thao tác trên tab step by step
        #region Nút tiến tab step by step

        //cờ báo hiệu cho biết đã bắt đầu vào sắp xếp step by step     
        bool isStepbyStepStarted = false;   

        /// <summary>
        /// Hàm sự kiện ấn nút tiến 1 bước
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonTien_Click(object sender, EventArgs e)
        {
            if (!isStepbyStepStarted)       //nếu chưa ấn tiến lần nào 
            {
                isStepbyStepStarted = true; //đã ấn tiến 1 lần thì set lại true
                lines = richTextBoxCodeThuatToan.Lines;
                // Khởi tạo hai vị trí i và j
                if (!daKhoiTaoiVaj)
                {
                    khoiTaoiVaj(1);
                    khoiTaoiVaj(2);
                    daKhoiTaoiVaj = true;
                }                                
                ButtonNap.Enabled = false;          //đã bắt đầu r thì ko cho nạp cho tới khi làm lại hoặc hoàn thành
                ButtonRandom.Enabled = false;       //đã bắt đầu r thì ko cho nạp random cho tới khi làm lại hoặc hoàn thành
                fileButton.Enabled = false;         //đã bắt đầu r thì ko cho nạp từ file cho tới khi làm lại hoặc hoàn thành                
                RadioButtonTangDan.Enabled = false; //ko cho đổi thứ tự sắp xếp trong khi đang thực hiện sắp xếp
                RadioButtonGiamDan.Enabled = false; //ko cho đổi thứ tự sắp xếp trong khi đang thực hiện sắp xếp
                ComboBoxThuatToan.Enabled = false;  //ko cho đổi thuật toán trong khi đang thực hiện sắp xếp

                //cho phép làm lại bắt cứ lúc nào trong step by step
                ButtonLamLai.Enabled = true;        
                ButtonLamLai.BackgroundImage = Properties.Resources.LamLaiEnable;

                //không cho phép đổi chế độ sắp xếp khi đang sắp xếp
                PictureBoxAuto.Enabled = false;
                PictureBoxDebug.Enabled = false;
                pictureBoxSoSanh.Enabled = false;
                switch (ComboBoxThuatToan.SelectedItem.ToString())
                {
                    case "Selection sort":
                        Selection_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Interchange sort":
                        Interchange_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Bubble sort":
                        Bubble_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Insertion sort":
                        Insertion_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Shaker sort":
                        Shaker_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "BinaryInsertion sort":
                        BinaryInsertion_sort(DaySoNguyen, HinhVuongSo);
                        break;
                    case "Quick sort":
                        Quick_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Shell sort":
                        Shell_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Heap sort":
                        HeapSort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Merge sort":
                        Merge_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    default:
                        break;
                }
            }
            else if(isStepbyStepStarted)   //nếu đã tiến 1 lần trong step by step
            {
                if (continueFlag != null)
                    continueFlag.Cancel();  //huỷ đợi người dùng nhấn nút tiến vì đã đc nhấn r
            }
        }

        CancellationTokenSource continueFlag;   //token lượt ngưng 
        /// <summary>
        /// hàm đợi người dùng ấn nút tiến
        /// </summary>
        /// <returns></returns>
        Task waitForbuttonTienClick()
        {
            //-1 là đợi mãi mãi, 
            //và chỉ đáp ứng lời gọi của continueFlag
            return Task.Delay(-1, continueFlag.Token);
        }

        #endregion

        #endregion


#region Thuật toán sắp xếp

        #region Thuật toán Interchange Sort 
        private async void Interchange_sort(O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            Stopwatch ThoiGianChay = new Stopwatch();
            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            // Nếu đang chạy trong chế độ so sánh thì
            // tạo stopWatch để đếm thời gian chạy
            if (SoSanh == true)
            {
                ThoiGianChay.Start();
            }

            int i, j = 0;
            for (i = m; i < DaySoNguyenn.Length - 1; i++)
            {
                m = i;
                HighlightRichTextBox(lines, 2, colorhighlight);   //highlight code vòng for i
                richTextBoxCodeThuatToan.Refresh();                
                showi(i, ThuatToan);
                labeli.Refresh();
                HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                HinhVuongSoo[i].Refresh();        //update màu lại cho ô vuông 

                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)                
                {
                    buttonLamLaiClicked = false;
                    return;
                }

                for (j = i + 1; j < DaySoNguyenn.Length; j++)
                {
                    HighlightRichTextBox(lines, 2, richTextBoxCodeThuatToan.BackColor);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 4, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();
                    showj(j, ThuatToan);
                    labelj.Refresh();
                    HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                    HinhVuongSoo[j].Refresh();           //update màu lại cho ô vuông     

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }

                    if (SoSanh != true)
                    {
                        HighlightRichTextBox(lines, 4, richTextBoxCodeThuatToan.BackColor);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 6, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }                    

                    if (RadioButtonTangDan.Checked && DaySoNguyenn[i] > DaySoNguyenn[j])
                    {
                        HighlightRichTextBox(lines, 6, richTextBoxCodeThuatToan.BackColor);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 8, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        doiCho2OVuong(i, j, HinhVuongSoo);

                        HinhVuongtemp = HinhVuongSoo[i];                  //Hoán vị 2 hình vuông
                        HinhVuongSoo[i] = HinhVuongSoo[j];           //Hoán vị 2 hình vuông 
                        HinhVuongSoo[j] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                        HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                        HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                        HinhVuongSoo[i].Refresh();
                        HinhVuongSoo[j].Refresh();

                        HinhVuongSoo[i].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                        HinhVuongSoo[j].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        int temp = DaySoNguyenn[i];
                        DaySoNguyenn[i] = DaySoNguyenn[j];
                        DaySoNguyenn[j] = temp;

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }
                    else
                    {
                        if (RadioButtonGiamDan.Checked && DaySoNguyenn[i] < DaySoNguyenn[j])
                        {
                            HighlightRichTextBox(lines, 6, richTextBoxCodeThuatToan.BackColor);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 8, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();                            

                            doiCho2OVuong(i, j, HinhVuongSoo);

                            HinhVuongtemp = HinhVuongSoo[i];                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[i] = HinhVuongSoo[j];           //Hoán vị 2 hình vuông 
                            HinhVuongSoo[j] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[i].Refresh();
                            HinhVuongSoo[j].Refresh();

                            HinhVuongSoo[i].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                            HinhVuongSoo[j].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            int temp = DaySoNguyenn[i];
                            DaySoNguyenn[i] = DaySoNguyenn[j];
                            DaySoNguyenn[j] = temp;

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }
                    }
                    HinhVuongSoo[j].BackColor = Color.Gray;
                    HinhVuongSoo[j].Refresh();               //update màu lại cho ô vuông
                    HighlightRichTextBox(lines, 6, richTextBoxCodeThuatToan.BackColor);
                    HighlightRichTextBox(lines, 8, richTextBoxCodeThuatToan.BackColor);
                    richTextBoxCodeThuatToan.Refresh();
                }
                HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSoo[i].Refresh();                   //update màu lại cho ô vuông
            }
            if (i == DaySoNguyen.Length - 1 && j == DaySoNguyen.Length)
            {
                HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSoo[i].Refresh();
                m = 0;
                n = 0;
                enableControlsAfterFinish();

                if (SoSanh != true)
                {
                    // Nếu không phải đang chạy so sánh thì
                    // xuất ra thông báo hoàn thành khi xong
                    KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                    gif_timer.Stop();
                    ktb.ShowDialog();                
                }
                else
                {
                    // Nếu đang chạy so sánh thì khi hoàn thành
                    // ngưng đếm thời gian và xuất ra labelThoiGianSoSanh1
                    ThoiGianChay.Stop();
                    TimeSpan ts = ThoiGianChay.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                    if (ThuatToan == 1)
                    {
                        labelThoiGianSoSanh1.Text = "Thời gian: " + elapsedTime;
                        labelThoiGianSoSanh1.Visible = true;
                        labelThoiGianSoSanh1.Refresh();
                    }
                    else if (ThuatToan == 2)
                    {
                        labelThoiGianSoSanh2.Text = "Thời gian: " + elapsedTime;
                        labelThoiGianSoSanh2.Visible = true;
                        labelThoiGianSoSanh2.Refresh();
                    }                
                }
            }
        }
        #endregion
        
        #region Thuật toán Selection sort
        bool changeColor = false;
        private async void Selection_sort(O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {            
            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            Stopwatch ThoiGianChay = new Stopwatch();
            // Nếu đang chạy trong chế độ so sánh thì
            // tạo stopWatch để đếm thời gian chạy
            if (SoSanh == true)
            {
                ThoiGianChay.Start();
            }

            int i, j = 0;
            for (i = m; i < DaySoNguyenn.Length - 1; i++)
            {
                HighlightRichTextBox(lines, 3, colorhighlight);
                richTextBoxCodeThuatToan.Refresh();
                HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                HinhVuongSoo[i].Refresh();
                m = i;
                showi(i, ThuatToan);
                labeli.Refresh();

                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    buttonLamLaiClicked = false;
                    return;
                }

                int iMinMax = i;
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 5);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 5, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }
                

                for (j = i + 1; j < DaySoNguyenn.Length; j++)
                {
                    resetRichtextboxColor(lines, 9);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 6, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();
                    n = j;
                    showj(j, ThuatToan);
                    labelj.Refresh();
                    HinhVuongSoo[j].BackColor = Color.FromArgb(255, 210, 45);
                    HinhVuongSoo[j].Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 9);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 7, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }
                    
                    if (RadioButtonTangDan.Checked && DaySoNguyenn[j] < DaySoNguyenn[iMinMax])
                    {
                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 9);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 8, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();
                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }
                        
                        changeColor = true;
                        iMinMax = j;
                        n = j;
                        int p = j - 1;
                        while (p > i)
                        {
                            HinhVuongSoo[p].BackColor = Color.Gray;
                            HinhVuongSoo[p].Refresh();
                            p--;
                        }
                        HinhVuongSoo[j].BackColor = Color.FromArgb(178, 75, 83);
                        HinhVuongSoo[j].Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }
                    else if (RadioButtonGiamDan.Checked && DaySoNguyenn[j] > DaySoNguyenn[iMinMax])
                    {
                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 9);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 8, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }
                        
                        changeColor = true;
                        iMinMax = j;
                        n = j;
                        int p = j - 1;
                        while (p > i)
                        {
                            HinhVuongSoo[p].BackColor = Color.Gray;
                            HinhVuongSoo[p].Refresh();
                            p--;
                        }
                        HinhVuongSoo[j].BackColor = Color.FromArgb(178, 75, 83);
                        HinhVuongSoo[j].Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }
                    if (!changeColor)
                    {
                        HinhVuongSoo[j].BackColor = Color.Gray;
                        HinhVuongSoo[j].Refresh();
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }
                    changeColor = false;
                }
                if (iMinMax != i)
                {
                    resetRichtextboxColor(lines, 9);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 9, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    doiCho2OVuong(i, iMinMax, HinhVuongSoo);

                    HinhVuongtemp = HinhVuongSoo[i];                  //Hoán vị 2 hình vuông
                    HinhVuongSoo[i] = HinhVuongSoo[iMinMax];           //Hoán vị 2 hình vuông 
                    HinhVuongSoo[iMinMax] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                    HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                    HinhVuongSoo[iMinMax].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                    HinhVuongSoo[i].Refresh();
                    HinhVuongSoo[iMinMax].Refresh();

                    HinhVuongSoo[i].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                    HinhVuongSoo[iMinMax].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }

                    int temp = DaySoNguyenn[i];
                    DaySoNguyenn[i] = DaySoNguyenn[iMinMax];
                    DaySoNguyenn[iMinMax] = temp;

                    HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                    HinhVuongSoo[i].Refresh();
                    HinhVuongSoo[iMinMax].BackColor = System.Drawing.Color.Gray;
                    HinhVuongSoo[iMinMax].Refresh();
                }
                HinhVuongSoo[m].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSoo[m].Refresh();
                resetRichtextboxColor(lines, 11);
                richTextBoxCodeThuatToan.Refresh();
            }
            if (i == DaySoNguyenn.Length - 1 && j == DaySoNguyenn.Length)
            {
                HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSoo[i].Refresh();
                m = 0;
                n = 0;
                enableControlsAfterFinish();

                if (SoSanh != true)
                {
                    // Nếu không phải đang chạy so sánh thì
                    // xuất ra thông báo hoàn thành khi xong
                    KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                    gif_timer.Stop();
                    ktb.ShowDialog();
                }
                else
                {
                    // Nếu đang chạy so sánh thì khi hoàn thành
                    // ngưng đếm thời gian và xuất ra labelThoiGianSoSanh1
                    ThoiGianChay.Stop();
                    TimeSpan ts = ThoiGianChay.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                    if (ThuatToan == 1)
                    {
                        labelThoiGianSoSanh1.Text = "Thời gian: " + elapsedTime;                        
                        labelThoiGianSoSanh1.Visible = true;
                        labelThoiGianSoSanh1.Refresh();
                    }
                    else if (ThuatToan == 2)
                    {
                        labelThoiGianSoSanh2.Text = "Thời gian: " + elapsedTime;
                        labelThoiGianSoSanh2.Visible = true;
                        labelThoiGianSoSanh2.Refresh();
                    }
                }
            }
        }
        #endregion

        #region Thuật toán Bubble sort
        private async void Bubble_sort(O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            Stopwatch ThoiGianChay = new Stopwatch();
            // Nếu đang chạy trong chế độ so sánh thì
            // tạo stopWatch để đếm thời gian chạy
            if (SoSanh == true)
            {
                ThoiGianChay.Start();
            }

            int i, j;
            bool swapped = false;
            for (i = 0; i < DaySoNguyenn.Length - 1; i++)
            {
                if (SoSanh != true)
                {
                    HighlightRichTextBox(lines, 2, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }
                swapped = false;

                for (j = 0; j < DaySoNguyenn.Length - i - 1; j++)
                {
                    resetRichtextboxColor(lines, 11);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 4, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();
                    HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                    HinhVuongSoo[j].Refresh();
                    showi(j, ThuatToan);                   //làm xuất hiện label biến i tại vị trí j
                    labeli.Refresh();
                    HinhVuongSoo[j + 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                    HinhVuongSoo[j + 1].Refresh();
                    showj(j + 1, ThuatToan);               //làm xuất hiện label biến j tại vị trí j+1
                    labelj.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 7);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 6, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }

                    if (RadioButtonTangDan.Checked && DaySoNguyenn[j] > DaySoNguyenn[j + 1])
                    {
                        resetRichtextboxColor(lines, 10);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 8, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        doiCho2OVuong(j, j + 1, HinhVuongSoo);

                        HinhVuongtemp = HinhVuongSoo[j];                  //Hoán vị 2 hình vuông
                        HinhVuongSoo[j] = HinhVuongSoo[j + 1];           //Hoán vị 2 hình vuông 
                        HinhVuongSoo[j+1] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                        HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                        HinhVuongSoo[j+1].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                        HinhVuongSoo[j].Refresh();
                        HinhVuongSoo[j+1].Refresh();

                        HinhVuongSoo[j].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                        HinhVuongSoo[j+1].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        int temp = DaySoNguyenn[j];
                        DaySoNguyenn[j] = DaySoNguyenn[j+1];
                        DaySoNguyenn[j+1] = temp;
                        swapped = true;
                    }
                    else
                    {
                        if (RadioButtonGiamDan.Checked && DaySoNguyenn[j] < DaySoNguyenn[j + 1])
                        {
                            resetRichtextboxColor(lines, 10);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 8, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            doiCho2OVuong(j, j + 1, HinhVuongSoo);

                            HinhVuongtemp = HinhVuongSoo[j];                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[j] = HinhVuongSoo[j+1];           //Hoán vị 2 hình vuông 
                            HinhVuongSoo[j + 1] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[j + 1].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[j].Refresh();
                            HinhVuongSoo[j + 1].Refresh();

                            HinhVuongSoo[j].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                            HinhVuongSoo[j + 1].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            int temp = DaySoNguyenn[j];
                            DaySoNguyenn[j] = DaySoNguyenn[j + 1];
                            DaySoNguyenn[j + 1] = temp;
                            swapped = true;
                        }
                    }
                    HinhVuongSoo[j + 1].BackColor = System.Drawing.Color.Gray;
                    HinhVuongSoo[j].BackColor = System.Drawing.Color.Gray;
                    HinhVuongSoo[j].Refresh();
                    HinhVuongSoo[j + 1].Refresh();
                }

                HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);  //phần tử cuối là phần tử đã được sắp xếp đúng nên thành màu xanh
                HinhVuongSoo[j].Refresh();

                if (swapped == false)
                {
                    for (int x = 0; x < j; x++)
                    {
                        HinhVuongSoo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                    }
                    m = 0;
                    n = 0;
                    enableControlsAfterFinish();

                    if (SoSanh != true)
                    {
                        // Nếu không phải đang chạy so sánh thì
                        // xuất ra thông báo hoàn thành khi xong
                        KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                        gif_timer.Stop();
                        ktb.ShowDialog();                        
                    }
                    else
                    {
                        // Nếu đang chạy so sánh thì khi hoàn thành
                        // ngưng đếm thời gian và xuất ra labelThoiGianSoSanh1
                        ThoiGianChay.Stop();
                        TimeSpan ts = ThoiGianChay.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                        if (ThuatToan == 1)
                        {
                            labelThoiGianSoSanh1.Text = "Thời gian: " + elapsedTime;
                            labelThoiGianSoSanh1.Visible = true;
                            labelThoiGianSoSanh1.Refresh();
                        }
                        else if (ThuatToan == 2)
                        {
                            labelThoiGianSoSanh2.Text = "Thời gian: " + elapsedTime;
                            labelThoiGianSoSanh2.Visible = true;
                            labelThoiGianSoSanh2.Refresh();
                        }                        
                    }
                    break;
                }
                resetRichtextboxColor(lines, 13);
                richTextBoxCodeThuatToan.Refresh();
            }
        }
        #endregion

        #region Thuật toán Insertion sort
        //dùng để lưu phần tử thứ i trong thuật
        //toán để so sánh với các ptử phía trước
        int key;

        private async void Insertion_sort(O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            Stopwatch ThoiGianChay = new Stopwatch();
            // Nếu đang chạy trong chế độ so sánh thì
            // tạo stopWatch để đếm thời gian chạy
            if (SoSanh == true)
            {
                ThoiGianChay.Start();
            }

            int i, j;
            for (i = m; i < DaySoNguyenn.Length; i++)
            {
                if (SoSanh != true)
                {
                    HighlightRichTextBox(lines, 2, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }

                m = i;
                showi(i, ThuatToan);
                labeli.Refresh();

                resetRichtextboxColor(lines, 4);
                richTextBoxCodeThuatToan.Refresh();
                HighlightRichTextBox(lines, 4, colorhighlight);
                richTextBoxCodeThuatToan.Refresh();
                key = DaySoNguyenn[i];     //nếu nút ngưng chưa đc ấn thì key vẫn bằng đúng như i

                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    buttonLamLaiClicked = false;
                    return;
                }

                for (int x = 0; x <= i; x++)
                {
                    HinhVuongSoo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                    HinhVuongSoo[x].Refresh();
                }

                j = i - 1;

                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 5);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 5, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();


                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }

                    resetRichtextboxColor(lines, 11);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 6, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }


                if (RadioButtonTangDan.Checked)
                {
                    while (j >= 0 && DaySoNguyenn[j] > key)
                    {
                        n = j;
                        showj(j, ThuatToan);
                        labelj.Refresh();
                        HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                        HinhVuongSoo[j].Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        HinhVuongSoo[j + 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                        HinhVuongSoo[j + 1].Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 11);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 8, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }
                        
                        doiCho2OVuong(j, j + 1, HinhVuongSoo);

                        HinhVuongtemp = HinhVuongSoo[j];                  //Hoán vị 2 hình vuông
                        HinhVuongSoo[j] = HinhVuongSoo[j+1];           //Hoán vị 2 hình vuông 
                        HinhVuongSoo[j + 1] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                        HinhVuongSoo[j + 1].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[j].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[j].Refresh();
                        HinhVuongSoo[j + 1].Refresh();

                        HinhVuongSoo[j].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                        HinhVuongSoo[j + 1].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        DaySoNguyenn[j + 1] = DaySoNguyenn[j];

                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 11);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 9, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }

                        j = j - 1;
                    }
                }
                else if (RadioButtonGiamDan.Checked)
                {
                    while (j >= 0 && DaySoNguyenn[j] < key)
                    {
                        resetRichtextboxColor(lines, 11);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 6, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        n = j;
                        showj(j, ThuatToan);
                        labelj.Refresh();
                        HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                        HinhVuongSoo[j].Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        HinhVuongSoo[j + 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                        HinhVuongSoo[j + 1].Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 11);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 8, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }

                        doiCho2OVuong(j, j + 1, HinhVuongSoo);

                        HinhVuongtemp = HinhVuongSoo[j];                  //Hoán vị 2 hình vuông
                        HinhVuongSoo[j] = HinhVuongSoo[j + 1];           //Hoán vị 2 hình vuông 
                        HinhVuongSoo[j + 1] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                        HinhVuongSoo[j + 1].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[j].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[j].Refresh();
                        HinhVuongSoo[j + 1].Refresh();

                        HinhVuongSoo[j].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                        HinhVuongSoo[j + 1].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        DaySoNguyenn[j + 1] = DaySoNguyenn[j];

                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 11);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 9, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }

                        j = j - 1;
                    }
                }
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 12);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 11, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }

                DaySoNguyenn[j + 1] = key;

                if (i == DaySoNguyenn.Length - 1)
                {
                    for (int x = 0; x <= i; x++)
                        HinhVuongSoo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                    m = 0;
                    n = 0;
                    enableControlsAfterFinish();

                    if (SoSanh != true)
                    {
                        // Nếu không phải đang chạy so sánh thì
                        // xuất ra thông báo hoàn thành khi xong
                        KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                        gif_timer.Stop();
                        ktb.ShowDialog();
                        break;
                    }
                    else
                    {
                        // Nếu đang chạy so sánh thì khi hoàn thành
                        // ngưng đếm thời gian và xuất ra labelThoiGianSoSanh1
                        ThoiGianChay.Stop();
                        TimeSpan ts = ThoiGianChay.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                        if (ThuatToan == 1)
                        {
                            labelThoiGianSoSanh1.Text = "Thời gian: " + elapsedTime;
                            labelThoiGianSoSanh1.Visible = true;
                            labelThoiGianSoSanh1.Refresh();
                        }
                        else if (ThuatToan == 2)
                        {
                            labelThoiGianSoSanh2.Text = "Thời gian: " + elapsedTime;
                            labelThoiGianSoSanh2.Visible = true;
                            labelThoiGianSoSanh2.Refresh();
                        }
                    }

                }
                resetRichtextboxColor(lines, 14);
                richTextBoxCodeThuatToan.Refresh();
            }
        }
        #endregion

        #region Thuật toán BinaryInsertion Sort
        int Binary_Search(int[] a, int item, int low, int high, bool flag)
        {
            if (high <= low)
            {
                if (flag)
                {
                    return (item > a[low]) ? (low + 1) : low;
                }
                else
                {
                    return (item < a[low]) ? (low + 1) : low;
                }
            }

            int mid = (low + high) / 2;

            if (item == a[mid])
                return mid + 1;

            if (flag)
            {
                if (item > a[mid])
                    return Binary_Search(a, item, mid + 1, high, flag);
                return Binary_Search(a, item, low, mid - 1, flag);
            }
            else
            {
                if (item > a[mid])
                    return Binary_Search(a, item, low, mid - 1, flag);
                return Binary_Search(a, item, mid + 1, high, flag);
            }
        }

        static int iBinaryInsertionSort = 1;
        async void BinaryInsertion_sort(int[] a, O_vuong[] HinhVuongSo)
        {
            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }
            int i, loc, j, k, selected, i_n;
            HighlightRichTextBox(lines, 4, colorhighlight);
            richTextBoxCodeThuatToan.Refresh();
            await waitForInput();
            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
            if (buttonLamLaiClicked)
            {
                buttonLamLaiClicked = false;
                return;
            }
            iBinaryInsertionSort = 1;
            for (i = iBinaryInsertionSort; i < a.Length; i++)
            {
                iBinaryInsertionSort = i;
                for (int x = 0; x < i; x++)
                {
                    HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                    //HinhVuongSo[x].Refresh();
                }
                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    buttonLamLaiClicked = false;
                    return;
                }

                i_n = i;
                j = i - 1;
                selected = a[i];
                O_vuong HinhVuongSelected = HinhVuongSo[i];
                HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                HinhVuongSo[i].Refresh();
                loc = Binary_Search(a, selected, 0, j, RadioButtonTangDan.Checked);
                bool swap = true;

                resetRichtextboxColor(lines, lines.Length);
                HighlightRichTextBox(lines, 15, colorhighlight);
                richTextBoxCodeThuatToan.Refresh();
                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    buttonLamLaiClicked = false;
                    return;
                }
                while (j >= loc)
                {
                    n = i_n;
                    m = j;
                    if (swap)
                    {
                        swap = false;
                        Transition t = new Transition(new TransitionType_Linear(300));
                        t.add(HinhVuongSo[n], "Top", 0);
                        t.run();
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }
                    //HinhVuongSo[m].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                    //HinhVuongSo[m].Refresh();
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                    //doiCho2OVuong(i, j);

                    Transition t1 = new Transition(new TransitionType_Linear(300));
                    t1.add(HinhVuongSo[j], "Left", (j + 1) * 90 + 15);
                    t1.run();
                    HinhVuongSo[j + 1] = HinhVuongSo[j];
                    resetRichtextboxColor(lines, lines.Length);
                    HighlightRichTextBox(lines, 16, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }

                    a[j + 1] = a[j];
                    j--;
                    i_n--;
                    //HinhVuongSo[n].BackColor = System.Drawing.Color.Gray;
                    //HinhVuongSo[m].BackColor = System.Drawing.Color.Gray;
                    //HinhVuongSo[m].Refresh();
                    //HinhVuongSo[n].Refresh();
                }
                swap = true;
                HinhVuongSo[j + 1] = HinhVuongSelected;
                Transition t2 = new Transition(new TransitionType_Linear(200));
                t2.add(HinhVuongSo[j + 1], "Left", (j + 1) * 90 + 15);
                Transition t3 = new Transition(new TransitionType_Linear(300));
                t3.add(HinhVuongSo[j + 1], "Top", 60);
                Transition.runChain(t2, t3);
                HinhVuongSo[j + 1].BackColor = Color.FromArgb(58, 130, 90);
                HinhVuongSo[j + 1].Refresh();
                resetRichtextboxColor(lines, lines.Length);
                HighlightRichTextBox(lines, 17, colorhighlight);
                richTextBoxCodeThuatToan.Refresh();
                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    buttonLamLaiClicked = false;
                    return;
                }

                a[j + 1] = selected;
                if (i == a.Length - 1)
                {
                    for (int x = 0; x <= i; x++)
                        HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                    iBinaryInsertionSort = 1;
                    enableControlsAfterFinish();
                    resetRichtextboxColor(lines, lines.Length);
                    KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                    ktb.ShowDialog();
                    break;
                }
            }
        }
        #endregion

        #region Heap
        #region Thuật toán Heap Sort
        int old_r = 0;
        bool checkHeap = false;
        bool shifting = false;

        //biến token dùng để đợi cho hàm shifting hoàn thành
        CancellationTokenSource waitforshifting;

        /// <summary>
        /// Hàm dùng để đợ cho hàm shifting hoàn thành
        /// </summary>
        /// <returns></returns>
        Task waitforshiftingfinish()
        {
            return Task.Delay(-1, waitforshifting.Token);
        }

        //biến token dùng để đợi cho hàm createheap hoàn thành
        CancellationTokenSource waitforcreateheap;

        /// <summary>
        /// Hàm dùng để đợi cho hàm createheap hoàn thành
        /// </summary>
        /// <returns></returns>
        Task waitforcreateheapfinish()
        {
            return Task.Delay(-1, waitforcreateheap.Token);
        }

        async void HeapSort(O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            Stopwatch ThoiGianChay = new Stopwatch();
            // Nếu đang chạy trong chế độ so sánh thì
            // tạo stopWatch để đếm thời gian chạy
            if (SoSanh == true)
            {
                ThoiGianChay.Start();
            }

            int r;

            if (SoSanh != true)
            {
                HighlightRichTextBox(lines, 3, colorhighlight);
                richTextBoxCodeThuatToan.Refresh();

                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    buttonLamLaiClicked = false;
                    return;
                }
            }

            waitforcreateheap = new CancellationTokenSource();

            CreateHeap(DaySoNguyenn, HinhVuongSoo, DaySoNguyenn.Length, ThuatToan, SoSanh);

            try
            {
                await waitforcreateheapfinish();
            }
            catch
            {

            }
            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
            if (buttonLamLaiClicked)
            {
                buttonLamLaiClicked = false;
                return;
            }

            r = DaySoNguyenn.Length - 1;

            while (r > 0)
            {
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, lines.Length);
                    HighlightRichTextBox(lines, 5, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }

                HinhVuongSoo[0].BackColor = Color.FromArgb(66, 104, 166);
                HinhVuongSoo[0].Refresh();
                HinhVuongSoo[r].BackColor = Color.FromArgb(178, 75, 83);
                HinhVuongSoo[r].Refresh();

                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, lines.Length);
                    HighlightRichTextBox(lines, 7, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }

                doiCho2OVuong(0, r, HinhVuongSoo);

                HinhVuongtemp = HinhVuongSoo[0];                  //Hoán vị 2 hình vuông
                HinhVuongSoo[0] = HinhVuongSoo[r];           //Hoán vị 2 hình vuông 
                HinhVuongSoo[r] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                int temp = DaySoNguyenn[0];
                DaySoNguyenn[0] = DaySoNguyenn[r];
                DaySoNguyenn[r] = temp;
                HinhVuongSoo[0].BackColor = Color.Gray;
                HinhVuongSoo[0].Refresh();
                HinhVuongSoo[r].BackColor = Color.FromArgb(58, 130, 90);
                HinhVuongSoo[r].Refresh();

                HinhVuongSoo[0].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                HinhVuongSoo[r].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    buttonLamLaiClicked = false;
                    return;
                }

                r--;
                old_r = r;
                checkHeap = false;
                if (r > 0)
                {
                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, lines.Length);
                        HighlightRichTextBox(lines, 9, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }
 
                    waitforshifting = new CancellationTokenSource();

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, lines.Length);
                        HighlightRichTextBox(lines, 10, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }

                    shift(DaySoNguyenn, HinhVuongSoo, 0, r, ThuatToan, SoSanh);
                    try
                    {
                        await waitforshiftingfinish();
                    }
                    catch
                    {

                    }
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }
            }
            if (r == 0)
            {
                HinhVuongSoo[r].BackColor = Color.FromArgb(58, 130, 90);
                HinhVuongSoo[r].Refresh();
                m = 0;
                n = 0;
                enableControlsAfterFinish();
                if (SoSanh != true)
                {
                    // Nếu không phải đang chạy so sánh thì
                    // xuất ra thông báo hoàn thành khi xong
                    KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                    gif_timer.Stop();
                    ktb.ShowDialog();
                }
                else
                {
                    // Nếu đang chạy so sánh thì khi hoàn thành
                    // ngưng đếm thời gian và xuất ra labelThoiGianSoSanh1
                    ThoiGianChay.Stop();
                    TimeSpan ts = ThoiGianChay.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                    if (ThuatToan == 1)
                    {
                        labelThoiGianSoSanh1.Text = "Thời gian: " + elapsedTime;
                        labelThoiGianSoSanh1.Visible = true;
                        labelThoiGianSoSanh1.Refresh();
                    }
                    else if (ThuatToan == 2)
                    {
                        labelThoiGianSoSanh2.Text = "Thời gian: " + elapsedTime;
                        labelThoiGianSoSanh2.Visible = true;
                        labelThoiGianSoSanh2.Refresh();
                    }
                }
            }
        }


        async void CreateHeap(int[] DaySoNguyenn, O_vuong[] HinhVuongSoo, int n, int ThuatToan, bool SoSanh)
        {
            int l;
            l = n / 2 - 1;
            while (l >= 0)
            {
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, lines.Length);
                    HighlightRichTextBox(lines, 38, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        waitforcreateheap.Cancel();
                        return;
                    }
                }

                waitforshifting = new CancellationTokenSource();

                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, lines.Length);
                    HighlightRichTextBox(lines, 40, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        waitforcreateheap.Cancel();
                        return;
                    }
                }

                shift(DaySoNguyenn, HinhVuongSoo, l, n - 1, ThuatToan, SoSanh);
                try
                {
                    await waitforshiftingfinish();
                }
                catch
                {

                }
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    waitforcreateheap.Cancel();
                    return;
                }

                if (!shifting) l--;
                if (continueAutoFlag != null) continueAutoFlag.Cancel();
            }
            waitforcreateheap.Cancel();
        }


        bool flag1 = false, flag2 = false;
        async void shift(int[] DaySoNguyenn, O_vuong[] HinhVuongSoo, int l, int r, int ThuatToan, bool SoSanh)
        {
            shifting = true;

            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            int x, i, j;
            i = l;
            if (checkHeap) i = m;
            m = i;
            j = 2 * i + 1;
            if (checkHeap) j = n;
            n = j;
            showi(i, ThuatToan);
            labeli.Refresh();
            x = DaySoNguyenn[i];
            if (!checkHeap)
            {
                if (j <= r)
                {
                    showj(j, ThuatToan);
                    labelj.Refresh();
                }
                HinhVuongSoo[i].BackColor = Color.FromArgb(255, 210, 45);
                HinhVuongSoo[i].Refresh();

                HinhVuongSoo[j].BackColor = Color.FromArgb(255, 210, 45);
                HinhVuongSoo[j].Refresh();
            }

            if (j + 1 <= r && r >= 2)
            {
                if (!checkHeap)
                {
                    HinhVuongSoo[j + 1].BackColor = Color.FromArgb(255, 210, 45);
                    HinhVuongSoo[j + 1].Refresh();
                }
            }

            while (j <= r)
            {
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, lines.Length);
                    HighlightRichTextBox(lines, 19, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        waitforshifting.Cancel();
                        return;
                    }
                }

                if (j < r)
                {
                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, lines.Length);
                        HighlightRichTextBox(lines, 21, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            waitforshifting.Cancel();
                            return;
                        }
                    }
                    if (RadioButtonTangDan.Checked)
                    {
                        if (DaySoNguyenn[j] < DaySoNguyenn[j + 1])
                        {
                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, lines.Length);
                                HighlightRichTextBox(lines, 22, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();
                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    waitforshifting.Cancel();
                                    return;
                                }
                            }

                            HinhVuongSoo[j].BackColor = Color.Gray;
                            HinhVuongSoo[j].Refresh();

                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, lines.Length);
                                HighlightRichTextBox(lines, 22, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();
                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    waitforshifting.Cancel();
                                    return;
                                }
                            }

                            showj(++j, ThuatToan);
                            labelj.Refresh();
                        }
                        else //if (DaySoNguyen[j] > DaySoNguyen[j + 1])
                        {
                            HinhVuongSoo[j + 1].BackColor = Color.Gray;
                            HinhVuongSoo[j + 1].Refresh();
                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                waitforshifting.Cancel();
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (DaySoNguyenn[j] > DaySoNguyenn[j + 1])
                        {
                            HinhVuongSoo[j].BackColor = Color.Gray;
                            HinhVuongSoo[j].Refresh();
                            showj(++j, ThuatToan);
                            labelj.Refresh();
                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                waitforshifting.Cancel();
                                return;
                            }
                        }
                        else if (DaySoNguyenn[j] < DaySoNguyenn[j + 1])
                        {
                            HinhVuongSoo[j + 1].BackColor = Color.Gray;
                            HinhVuongSoo[j + 1].Refresh();
                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                waitforshifting.Cancel();
                                return;
                            }
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            waitforshifting.Cancel();
                            return;
                        }
                    }

                }
                if (RadioButtonTangDan.Checked)
                {
                    if (DaySoNguyenn[j] <= x)
                    {
                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, lines.Length);
                            HighlightRichTextBox(lines, 24, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();
                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                waitforshifting.Cancel();
                                return;
                            }
                        }


                        HinhVuongSoo[i].BackColor = Color.Gray;
                        HinhVuongSoo[i].Refresh();

                        HinhVuongSoo[2 * i + 1].BackColor = Color.Gray;
                        HinhVuongSoo[2 * i + 1].Refresh();

                        if (2 * i + 2 <= r)
                        {
                            HinhVuongSoo[2 * i + 2].BackColor = Color.Gray;
                            HinhVuongSoo[2 * i + 2].Refresh();
                        }

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            waitforshifting.Cancel();
                            return;
                        }

                        checkHeap = false;
                        shifting = false;
                        waitforshifting.Cancel();
                        return;
                    }
                }
                else if (RadioButtonGiamDan.Checked)
                {
                    if (DaySoNguyenn[j] >= x)
                    {
                        HinhVuongSoo[i].BackColor = Color.Gray;
                        HinhVuongSoo[i].Refresh();
                        HinhVuongSoo[2 * i + 1].BackColor = Color.Gray;
                        HinhVuongSoo[2 * i + 1].Refresh();
                        if (2 * i + 2 <= r)
                        {
                            HinhVuongSoo[2 * i + 2].BackColor = Color.Gray;
                            HinhVuongSoo[2 * i + 2].Refresh();
                        }

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            waitforshifting.Cancel();
                            return;
                        }

                        checkHeap = false;
                        shifting = false;
                        waitforshifting.Cancel();
                        return;
                    }
                }
                if (!flag1 && !flag2)
                {
                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, lines.Length);
                        HighlightRichTextBox(lines, 25, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            waitforshifting.Cancel();
                            return;
                        }
                        resetRichtextboxColor(lines, lines.Length);
                        HighlightRichTextBox(lines, 27, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            waitforshifting.Cancel();
                            return;
                        }
                    }

                    doiCho2OVuong(i, j, HinhVuongSoo);

                    HinhVuongtemp = HinhVuongSoo[i];                  //Hoán vị 2 hình vuông
                    HinhVuongSoo[i] = HinhVuongSoo[j];           //Hoán vị 2 hình vuông 
                    HinhVuongSoo[j] = HinhVuongtemp;                  //Hoán vị 2 hình vuông

                    HinhVuongSoo[i].BackColor = Color.FromArgb(255, 210, 45);
                    HinhVuongSoo[i].Refresh();
                    HinhVuongSoo[j].BackColor = Color.FromArgb(255, 210, 45);
                    HinhVuongSoo[j].Refresh();

                    HinhVuongSoo[i].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                    HinhVuongSoo[j].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        waitforshifting.Cancel();
                        return;
                    }

                    HinhVuongSoo[i].BackColor = Color.Gray;
                    HinhVuongSoo[i].Refresh();
                    HinhVuongSoo[j].BackColor = Color.Gray;
                    HinhVuongSoo[j].Refresh();
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        waitforshifting.Cancel();
                        return;
                    }
                    DaySoNguyenn[i] = DaySoNguyenn[j];
                    DaySoNguyenn[j] = x;
                    i = j;
                    m = i;
                    j = 2 * i + 1;
                    n = j;
                    showi(i, ThuatToan);
                    labeli.Refresh();
                    if (j <= r)
                    {
                        showj(j, ThuatToan);
                        labelj.Refresh();
                        HinhVuongSoo[i].BackColor = Color.FromArgb(255, 210, 45);
                        HinhVuongSoo[i].Refresh();
                        HinhVuongSoo[j].BackColor = Color.FromArgb(255, 210, 45);
                        HinhVuongSoo[j].Refresh();
                        if (j + 1 <= r && r >= 2)
                        {
                            HinhVuongSoo[j + 1].BackColor = Color.FromArgb(255, 210, 45);
                            HinhVuongSoo[j + 1].Refresh();
                        }
                    }
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        waitforshifting.Cancel();
                        return;
                    }
                    x = DaySoNguyenn[i];
                }
            }
            checkHeap = false;
            shifting = false;
            waitforshifting.Cancel();
        }

        #endregion
        #endregion


        #region Thuật toán shaker sort
        int k = 0; // sau mỗi lần shake, set lại biên trái/phải thông qua k
        private async void Shaker_sort(O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            int LeftShaker;   //biên trái
            int RightShaker;  //biên phải

            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            Stopwatch ThoiGianChay = new Stopwatch();
            // Nếu đang chạy trong chế độ so sánh thì
            // tạo stopWatch để đếm thời gian chạy
            if (SoSanh == true)
            {
                ThoiGianChay.Start();
            }

            if (SoSanh != true)
            {
                HighlightRichTextBox(lines, 2, colorhighlight);
                richTextBoxCodeThuatToan.Refresh();

                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    buttonLamLaiClicked = false;
                    return;
                }
            }

            if (SoSanh != true)
            {
                resetRichtextboxColor(lines, 4);
                richTextBoxCodeThuatToan.Refresh();
                HighlightRichTextBox(lines, 3, colorhighlight);
                richTextBoxCodeThuatToan.Refresh();

                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    buttonLamLaiClicked = false;
                    return;
                }
            }

            resetRichtextboxColor(lines, 4);
            richTextBoxCodeThuatToan.Refresh();

            int i; // biến chạy
            LeftShaker = 0; // biên trái
            RightShaker = DaySoNguyenn.Length - 1; // biên phải            
            while (LeftShaker < RightShaker) // khi left còn nhỏ hơn right thì luôn shake
            {
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 26);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 4, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }

                if (RadioButtonTangDan.Checked)
                {
                    for (i = LeftShaker; i < RightShaker; i++) // shake lớn về cuối, m dùng như biến Left
                    {
                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 14);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 6, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }

                        HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                        HinhVuongSoo[i].Refresh();
                        showi(i, ThuatToan);
                        labeli.Refresh();
                        HinhVuongSoo[i + 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                        HinhVuongSoo[i + 1].Refresh();
                        showj(i + 1, ThuatToan);
                        labelj.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        if (DaySoNguyenn[i] > DaySoNguyenn[i + 1])
                        {
                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 14);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 8, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }

                                resetRichtextboxColor(lines, 14);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 10, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }
                            }

                            doiCho2OVuong(i, i + 1, HinhVuongSoo);

                            HinhVuongtemp = HinhVuongSoo[i];                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[i] = HinhVuongSoo[i + 1];           //Hoán vị 2 hình vuông 
                            HinhVuongSoo[i + 1] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[i + 1].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[i].Refresh();
                            HinhVuongSoo[i + 1].Refresh();

                            HinhVuongSoo[i].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                            HinhVuongSoo[i + 1].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            int temp = DaySoNguyenn[i];
                            DaySoNguyenn[i] = DaySoNguyenn[i + 1];
                            DaySoNguyenn[i + 1] = temp;

                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 14);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 11, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }
                            }

                            k = i; // khi có đổi chỗ xảy ra, k = i, i luôn tăng cho đến right, lần đổi chỗ cuối cùng trong 1 lần shake là i và i + 1, nên sau khi shake xong i + 1 chứa phần tử lớn nhất, nên biên right sẽ set về k
                        }
                        HinhVuongSoo[i + 1].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[i].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[i].Refresh();
                        HinhVuongSoo[i + 1].Refresh();
                    }

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 15);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 14, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }

                    RightShaker = k;

                    for (int x = DaySoNguyenn.Length - 1; x > RightShaker; x--) // từ giới hạn ngoài cùng trở vào biên phải đã sắp xong
                    {
                        HinhVuongSoo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                        HinhVuongSoo[x].Refresh();
                    }


                    for (i = RightShaker; i > LeftShaker; i--)
                    {
                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 23);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 15, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }

                        HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                        HinhVuongSoo[i].Refresh();
                        showi(i, ThuatToan);
                        labeli.Refresh();
                        showj(i - 1, ThuatToan);
                        labelj.Refresh();
                        HinhVuongSoo[i - 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                        HinhVuongSoo[i - 1].Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        if (DaySoNguyenn[i] < DaySoNguyenn[i - 1])
                        {
                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 23);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 17, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }

                                resetRichtextboxColor(lines, 23);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 19, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }
                            }

                            doiCho2OVuong(i - 1, i, HinhVuongSoo);

                            HinhVuongtemp = HinhVuongSoo[i - 1];                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[i - 1] = HinhVuongSoo[i];           //Hoán vị 2 hình vuông 
                            HinhVuongSoo[i] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[i - 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[i - 1].Refresh();
                            HinhVuongSoo[i].Refresh();

                            HinhVuongSoo[i - 1].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                            HinhVuongSoo[i].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            int temp = DaySoNguyenn[i];
                            DaySoNguyenn[i] = DaySoNguyenn[i - 1];
                            DaySoNguyenn[i - 1] = temp;

                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 23);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 20, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }
                            }

                            k = i;
                        }
                        HinhVuongSoo[i - 1].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[i].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[i].Refresh();
                        HinhVuongSoo[i - 1].Refresh();
                    }

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 24);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 23, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }

                    LeftShaker = k;

                    for (int x = 0; x < LeftShaker; x++)
                    {
                        HinhVuongSoo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                        HinhVuongSoo[x].Refresh();
                    }
                }
                else
                {
                    for (i = LeftShaker; i < RightShaker; i++) // shake lớn về cuối, m dùng như biến Left
                    {
                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 14);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 6, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }

                        HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                        HinhVuongSoo[i].Refresh();
                        showi(i, ThuatToan);
                        labeli.Refresh();
                        HinhVuongSoo[i + 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                        HinhVuongSoo[i + 1].Refresh();
                        showj(i + 1, ThuatToan);
                        labelj.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        if (DaySoNguyenn[i] < DaySoNguyenn[i + 1])
                        {
                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 14);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 8, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }

                                resetRichtextboxColor(lines, 14);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 10, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }
                            }

                            doiCho2OVuong(i, i + 1, HinhVuongSoo);

                            HinhVuongtemp = HinhVuongSoo[i];                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[i] = HinhVuongSoo[i + 1];           //Hoán vị 2 hình vuông 
                            HinhVuongSoo[i + 1] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[i + 1].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[i].Refresh();
                            HinhVuongSoo[i + 1].Refresh();

                            HinhVuongSoo[i].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                            HinhVuongSoo[i + 1].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            int temp = DaySoNguyenn[i];
                            DaySoNguyenn[i] = DaySoNguyenn[i + 1];
                            DaySoNguyenn[i + 1] = temp;

                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 14);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 11, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }
                            }

                            k = i; // khi có đổi chỗ xảy ra, k = i, i luôn tăng cho đến right, lần đổi chỗ cuối cùng trong 1 lần shake là i và i + 1, nên sau khi shake xong i + 1 chứa phần tử lớn nhất, nên biên right sẽ set về k
                        }
                        HinhVuongSoo[i + 1].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[i].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[i].Refresh();
                        HinhVuongSoo[i + 1].Refresh();
                    }

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 15);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 14, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }


                    RightShaker = k;

                    for (int x = DaySoNguyenn.Length - 1; x > RightShaker; x--) // từ giới hạn ngoài cùng trở vào biên phải đã sắp xong
                    {
                        HinhVuongSoo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                        HinhVuongSoo[x].Refresh();
                    }

                    for (i = RightShaker; i > LeftShaker; i--)
                    {
                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 23);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 15, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                        }

                        n = i;
                        HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                        HinhVuongSoo[i].Refresh();
                        showi(i, ThuatToan);
                        labeli.Refresh();
                        showj(i - 1, ThuatToan);
                        labelj.Refresh();
                        HinhVuongSoo[i - 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                        HinhVuongSoo[i - 1].Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        if (DaySoNguyenn[i] > DaySoNguyenn[i - 1])
                        {
                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 23);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 17, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }

                                resetRichtextboxColor(lines, 23);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 19, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }
                            }

                            doiCho2OVuong(i - 1, i, HinhVuongSoo);

                            HinhVuongtemp = HinhVuongSoo[i - 1];                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[i - 1] = HinhVuongSoo[i];           //Hoán vị 2 hình vuông 
                            HinhVuongSoo[i] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[i - 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[i - 1].Refresh();
                            HinhVuongSoo[i].Refresh();

                            HinhVuongSoo[i - 1].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                            HinhVuongSoo[i].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            int temp = DaySoNguyenn[i];
                            DaySoNguyenn[i] = DaySoNguyenn[i - 1];
                            DaySoNguyenn[i - 1] = temp;

                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 23);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 20, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }
                            }

                            k = i;
                        }
                        HinhVuongSoo[i - 1].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[i].BackColor = System.Drawing.Color.Gray;
                        HinhVuongSoo[i].Refresh();
                        HinhVuongSoo[i - 1].Refresh();
                    }

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 24);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 23, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }

                    LeftShaker = k;

                    for (int x = 0; x < LeftShaker; x++)
                    {
                        HinhVuongSoo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                        HinhVuongSoo[x].Refresh();
                    }
                }
                if (RightShaker == LeftShaker)
                {
                    for (int x = 0; x < DaySoNguyenn.Length; x++)
                    {
                        HinhVuongSoo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                    }
                    resetRichtextboxColor(lines, 24);
                    richTextBoxCodeThuatToan.Refresh();
                    m = 0;
                    n = 0;
                    enableControlsAfterFinish();

                    if (SoSanh != true)
                    {
                        // Nếu không phải đang chạy so sánh thì
                        // xuất ra thông báo hoàn thành khi xong
                        KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                        gif_timer.Stop();
                        ktb.ShowDialog();
                        break;
                    }
                    else
                    {
                        // Nếu đang chạy so sánh thì khi hoàn thành
                        // ngưng đếm thời gian và xuất ra labelThoiGianSoSanh1
                        ThoiGianChay.Stop();
                        TimeSpan ts = ThoiGianChay.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                        if (ThuatToan == 1)
                        {
                            labelThoiGianSoSanh1.Text = "Thời gian: " + elapsedTime;
                            labelThoiGianSoSanh1.Visible = true;
                            labelThoiGianSoSanh1.Refresh();
                        }
                        else if (ThuatToan == 2)
                        {
                            labelThoiGianSoSanh2.Text = "Thời gian: " + elapsedTime;
                            labelThoiGianSoSanh2.Visible = true;
                            labelThoiGianSoSanh2.Refresh();
                        }
                    }
                }
            }
        }
        #endregion

        #region Thuật toán Quick Sort
        Label labelx = new Label();
        Label labelibangj = new Label();
        Boolean iCoVuaBangjKhong = false;
        // giá trị middle của thuật toán Quick_sort
        private void showx(int pos)
        {
            labelx.Visible = true;
            labelx.Top = posSquare[pos].Top - 50;
            labelx.Left = posSquare[pos].Left + 5;
        }
        #region Hiển thị i và j dành riêng cho quick_sort
        private void showibangj(int pos)
        {
            iCoVuaBangjKhong = true;
            labeli.Visible = false;
            labelj.Visible = false;
            labelibangj.Visible = true;
            labelibangj.Top = posSquare[pos].Top - 25;
            labelibangj.Left = posSquare[pos].Left - 5;
        }
        private void showi_q(int pos)
        {
            if (iCoVuaBangjKhong)
            {
                iCoVuaBangjKhong = false;
                showj_q(pos - 1);
            }
            labelibangj.Visible = false;
            if (pos >= 0 && pos < DaySoNguyen.Length)
            {
                labeli.Visible = true;
                labeli.Top = posSquare[pos].Top - 25;
                labeli.Left = posSquare[pos].Left + 5;
            }
        }
        private void showj_q(int pos)
        {
            if (iCoVuaBangjKhong)
            {
                iCoVuaBangjKhong = false;
                showi_q(pos + 1);
            }
            labelibangj.Visible = false;
            if (pos >= 0 && pos < DaySoNguyen.Length)
            {
                labelj.Visible = true;
                labelj.Top = posSquare[pos].Top - 25;
                labelj.Left = posSquare[pos].Left + 5;
            }
        }

        #endregion

        # region khởi tạo 10 lần đợi
        CancellationTokenSource waitForTheNextActionSort1;
        Task waitForTheNextActionSortFinish1()
        {
            return Task.Delay(-1, waitForTheNextActionSort1.Token);
        }
        CancellationTokenSource waitForTheNextActionSort2;
        Task waitForTheNextActionSortFinish2()
        {
            return Task.Delay(-1, waitForTheNextActionSort2.Token);
        }
        CancellationTokenSource waitForTheNextActionSort3;
        Task waitForTheNextActionSortFinish3()
        {
            return Task.Delay(-1, waitForTheNextActionSort3.Token);
        }
        CancellationTokenSource waitForTheNextActionSort4;
        Task waitForTheNextActionSortFinish4()
        {
            return Task.Delay(-1, waitForTheNextActionSort4.Token);
        }
        CancellationTokenSource waitForTheNextActionSort5;
        Task waitForTheNextActionSortFinish5()
        {
            return Task.Delay(-1, waitForTheNextActionSort5.Token);
        }
        CancellationTokenSource waitForTheNextActionSort6;
        Task waitForTheNextActionSortFinish6()
        {
            return Task.Delay(-1, waitForTheNextActionSort6.Token);
        }
        CancellationTokenSource waitForTheNextActionSort7;
        Task waitForTheNextActionSortFinish7()
        {
            return Task.Delay(-1, waitForTheNextActionSort7.Token);
        }
        CancellationTokenSource waitForTheNextActionSort8;
        Task waitForTheNextActionSortFinish8()
        {
            return Task.Delay(-1, waitForTheNextActionSort8.Token);
        }
        CancellationTokenSource waitForTheNextActionSort9;
        Task waitForTheNextActionSortFinish9()
        {
            return Task.Delay(-1, waitForTheNextActionSort9.Token);
        }
        CancellationTokenSource waitForTheNextActionSort10;
        Task waitForTheNextActionSortFinish10()
        {
            return Task.Delay(-1, waitForTheNextActionSort10.Token);
        }
        #endregion
        public delegate Task awaitDelegate();
        #region Trả về đối tượng lần đợi tương ứng và hàm đợi tương ứng
        // trả về đối tượng lần đợi tương ứng
        private CancellationTokenSource returnAwaitCorrespondingObject(int solandequi)
        {
            if (solandequi == 10)
            {
                return waitForTheNextActionSort10;
            }
            if (solandequi == 1)
            {
                return waitForTheNextActionSort1;
            }
            if (solandequi == 2)
            {
                return waitForTheNextActionSort2;
            }
            if (solandequi == 3)
            {
                return waitForTheNextActionSort3;
            }
            if (solandequi == 4)
            {
                return waitForTheNextActionSort4;
            }
            if (solandequi == 5)
            {
                return waitForTheNextActionSort5;
            }
            if (solandequi == 6)
            {
                return waitForTheNextActionSort6;
            }
            if (solandequi == 7)
            {
                return waitForTheNextActionSort7;
            }
            if (solandequi == 8)
            {
                return waitForTheNextActionSort8;
            }
            return waitForTheNextActionSort9;
        }
        // trả về hàm đợi tương ứng
        private Task returnAwaitCorresponding(int solandequi)
        {

            if (solandequi == 10)
            {
                waitForTheNextActionSort10 = new CancellationTokenSource();
                awaitDelegate de10 = new awaitDelegate(waitForTheNextActionSortFinish10);
                return de10();
            }
            if (solandequi == 1)
            {
                waitForTheNextActionSort1 = new CancellationTokenSource();
                awaitDelegate de1 = new awaitDelegate(waitForTheNextActionSortFinish1);
                return de1();
            }
            if (solandequi == 2)
            {
                waitForTheNextActionSort2 = new CancellationTokenSource();
                awaitDelegate de2 = new awaitDelegate(waitForTheNextActionSortFinish2);
                return de2();
            }
            if (solandequi == 3)
            {
                waitForTheNextActionSort3 = new CancellationTokenSource();
                awaitDelegate de3 = new awaitDelegate(waitForTheNextActionSortFinish3);
                return de3();
            }
            if (solandequi == 4)
            {
                waitForTheNextActionSort4 = new CancellationTokenSource();
                awaitDelegate de4 = new awaitDelegate(waitForTheNextActionSortFinish4);
                return de4();
            }
            if (solandequi == 5)
            {
                waitForTheNextActionSort5 = new CancellationTokenSource();
                awaitDelegate de5 = new awaitDelegate(waitForTheNextActionSortFinish5);
                return de5();
            }
            if (solandequi == 6)
            {
                waitForTheNextActionSort6 = new CancellationTokenSource();
                awaitDelegate de6 = new awaitDelegate(waitForTheNextActionSortFinish6);
                return de6();
            }
            if (solandequi == 7)
            {
                waitForTheNextActionSort7 = new CancellationTokenSource();
                awaitDelegate de7 = new awaitDelegate(waitForTheNextActionSortFinish7);
                return de7();
            }
            if (solandequi == 8)
            {
                waitForTheNextActionSort8 = new CancellationTokenSource();
                awaitDelegate de8 = new awaitDelegate(waitForTheNextActionSortFinish8);
                return de8();
            }
            waitForTheNextActionSort9 = new CancellationTokenSource();
            awaitDelegate de9 = new awaitDelegate(waitForTheNextActionSortFinish9);
            return de9();

        }
        #endregion

        #region Đổi màu vị trí các ô vị trí từ left -> right
        private void changeColorPos(int left, int right)
        {
            labelx.Visible = false;
            labeli.Visible = false;
            labelj.Visible = false;
            labelibangj.Visible = false;
            for (int i = 0; i < DaySoNguyen.Length; i++)
            {
                posSquare[i].ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                posSquare[i].Refresh();
            }
            for (int i = left; i <= right; i++)
            {
                posSquare[i].AutoSize = true;
                posSquare[i].TabIndex = 0;
                posSquare[i].ForeColor = Color.Aqua;
                posSquare[i].Refresh();
            }
        }
        #endregion
        private void Quick_sort(O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            if (ThuatToan == 1)
            {
                // khởi tạo x

                labelx.AutoSize = true;
                labelx.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
                labelx.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                labelx.Top = posSquare[0].Top - 25;
                labelx.Left = posSquare[0].Left + 5;
                labelx.Name = "labelx";
                labelx.Size = new System.Drawing.Size(16, 22);
                labelx.TabIndex = 0;
                labelx.Text = "x";
                labelx.Visible = false;
                if (panelHienThiOVuong2.Controls.Contains(labelx))
                {
                    panelHienThiOVuong2.Controls.Remove(labelx);
                }
                panelHienThiOVuong.Controls.Add(labelx);

                //khởi tạo label i=j
                labelibangj.AutoSize = true;
                labelibangj.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
                labelibangj.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                labelibangj.Name = "labelibangj";
                labelibangj.Size = new System.Drawing.Size(16, 22);
                labelibangj.TabIndex = 0;
                labelibangj.Text = "i = j";
                labelibangj.Visible = false;
                if (panelHienThiOVuong2.Controls.Contains(labelibangj))
                {
                    panelHienThiOVuong2.Controls.Remove(labelibangj);
                }
                panelHienThiOVuong.Controls.Add(labelibangj);
            }
            else if(ThuatToan == 2)
            {
                // khởi tạo x

                labelx.AutoSize = true;
                labelx.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
                labelx.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                labelx.Top = posSquare[0].Top - 25;
                labelx.Left = posSquare[0].Left + 5;
                labelx.Name = "labelx";
                labelx.Size = new System.Drawing.Size(16, 22);
                labelx.TabIndex = 0;
                labelx.Text = "x";
                labelx.Visible = false;
                if (panelHienThiOVuong.Controls.Contains(labelx))
                {
                    panelHienThiOVuong.Controls.Remove(labelx);
                }
                panelHienThiOVuong2.Controls.Add(labelx);

                //khởi tạo label i=j
                labelibangj.AutoSize = true;
                labelibangj.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
                labelibangj.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                labelibangj.Name = "labelibangj";
                labelibangj.Size = new System.Drawing.Size(16, 22);
                labelibangj.TabIndex = 0;
                labelibangj.Text = "i = j";
                labelibangj.Visible = false;
                if (panelHienThiOVuong.Controls.Contains(labelibangj))
                {
                    panelHienThiOVuong.Controls.Remove(labelibangj);
                }
                panelHienThiOVuong2.Controls.Add(labelibangj);
            }



            //int[] a=new int[DaySoNguyen.Length];
            //Array.Copy(DaySoNguyen,a,DaySoNguyen.Length);
            // chạy quicksort
            if (RadioButtonTangDan.Checked)
                actionSorttangdan(0, DaySoNguyen.Length - 1, 0, HinhVuongSoo, DaySoNguyenn, ThuatToan, SoSanh);
            else
            {
                if (RadioButtonGiamDan.Checked)
                    actionSortgiamdan(0, DaySoNguyen.Length - 1, 0, HinhVuongSoo, DaySoNguyenn,ThuatToan, SoSanh);
            }
            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
            if (buttonLamLaiClicked)
            {
                buttonLamLaiClicked = false;
                return;
            }
        }
        private async void actionSorttangdan(int left, int right, int solandequi, O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            Stopwatch ThoiGianChay = new Stopwatch();
            // Nếu đang chạy trong chế độ so sánh thì
            // tạo stopWatch để đếm thời gian chạy
            if (SoSanh == true)
            {
                ThoiGianChay.Start();
            }

            changeColorPos(left, right);
            int i, j;
            int x;
            int posx = (left + right) / 2; // xác định vị trí phần tử giữa
            x = DaySoNguyenn[posx];        // chọn phần tử giữa làm gốc                   

            resetRichtextboxColor(lines, 23);
            richTextBoxCodeThuatToan.Refresh();
            HighlightRichTextBox(lines, 3, colorhighlight);
            richTextBoxCodeThuatToan.Refresh();
            showx(posx);
            labelx.Refresh();

            await waitForInput();
            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
            if (buttonLamLaiClicked)
            {
                returnAwaitCorrespondingObject(solandequi).Cancel();
                return;
            }

            i = left;
            j = right;

            resetRichtextboxColor(lines, 23);
            richTextBoxCodeThuatToan.Refresh();
            HighlightRichTextBox(lines, 4, colorhighlight);
            richTextBoxCodeThuatToan.Refresh();
            if (left <= i && i <= right)
            {
                showi_q(i);
                labeli.Refresh();
                HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                HinhVuongSoo[i].Refresh();
            }
            if (left <= j && j <= right)
            {
                showj_q(j);
                labelj.Refresh();
                HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                HinhVuongSoo[j].Refresh();
            }
            await waitForInput();
            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
            if (buttonLamLaiClicked)
            {
                returnAwaitCorrespondingObject(solandequi).Cancel();
                return;
            }
            do
            {
                while (DaySoNguyenn[i] < x)
                {
                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 7, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }

                    HinhVuongSoo[i].BackColor = Color.Gray;
                    HinhVuongSoo[i].Refresh();

                    i++;    // lặp đến khi a[i] < x
                    if (i == j)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 8, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (0 <= j && j < DaySoNguyen.Length)
                        {
                            showibangj(j);
                            labelibangj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(151, 99, 230);
                            HinhVuongSoo[j].Refresh();
                        }

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                    else
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 8, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (left <= i && i <= right)
                        {
                            showi_q(i);
                            labeli.Refresh();
                            HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[i].Refresh();
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                }
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 9, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();
                    await waitForInput();

                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                }

                while (DaySoNguyenn[j] > x)
                {
                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 9, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                    HinhVuongSoo[j].BackColor = Color.Gray;
                    HinhVuongSoo[j].Refresh();
                    j--;    // lặp đến khi a[j] > x

                    if (j == i)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 10, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (0 <= j && j < DaySoNguyen.Length)
                        {
                            showibangj(j);
                            labelibangj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(151, 99, 230);
                            HinhVuongSoo[j].Refresh();
                        }

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                    else
                    {

                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 10, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (left <= j && j <= right)
                        {
                            showj_q(j);
                            labelj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[j].Refresh();
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                }
                if (SoSanh != true)
                {
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                }

                if (i <= j)        // nếu có 2 phần tử a[i] và a[j] ko theo thứ tự
                {
                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 11, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }

                    doiCho2OVuong(i, j, HinhVuongSoo);

                    HinhVuongtemp = HinhVuongSoo[i];                  //Hoán vị 2 hình vuông
                    HinhVuongSoo[i] = HinhVuongSoo[j];           //Hoán vị 2 hình vuông 
                    HinhVuongSoo[j] = HinhVuongtemp;                  //Hoán vị 2 hình vuông                  
                    HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                    HinhVuongSoo[i].Refresh();
                    HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                    HinhVuongSoo[j].Refresh();
                    HinhVuongSoo[i].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                    HinhVuongSoo[j].thietLaplaiStatus();

                    int temp = DaySoNguyenn[i];
                    DaySoNguyenn[i] = DaySoNguyenn[j];
                    DaySoNguyenn[j] = temp;

                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 13, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }

                    HinhVuongSoo[i].BackColor = Color.Gray;
                    HinhVuongSoo[i].Refresh();
                    i++;
                    if (i == j)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 14, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (0 <= j && j < DaySoNguyen.Length)
                        {
                            showibangj(j);
                            labelibangj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(151, 99, 230);
                            HinhVuongSoo[j].Refresh();
                        }

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                    else
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 14, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (left <= i && i <= right)
                        {
                            showi_q(i);
                            labelj.Refresh();
                            HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[i].Refresh();
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }

                    HinhVuongSoo[j].BackColor = Color.Gray;
                    HinhVuongSoo[j].Refresh();
                    j--;
                    if (i == j)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 15, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (0 <= j && j < DaySoNguyen.Length)
                        {
                            showibangj(j);
                            labelibangj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(151, 99, 230);
                            HinhVuongSoo[j].Refresh();
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                    else
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 15, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (left <= j && j <= right)
                        {
                            showj_q(j);
                            labelj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[j].Refresh();
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                }
            } while (i <= j);

            if (SoSanh != true)
            {
                resetRichtextboxColor(lines, 23);
                richTextBoxCodeThuatToan.Refresh();
                HighlightRichTextBox(lines, 17, colorhighlight);
                richTextBoxCodeThuatToan.Refresh();

                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    returnAwaitCorrespondingObject(solandequi).Cancel();
                    return;
                }
            }

            if (left < j)    // phân hoạch đoạn bên trái
            {
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 18, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 19, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                }

                actionSorttangdan(left, j, solandequi + 1, HinhVuongSoo, DaySoNguyenn, ThuatToan, SoSanh);

                try
                {
                    await returnAwaitCorresponding(solandequi + 1);
                }
                catch
                {

                }
                //MessageBox.Show(solandequi.ToString());
            }
            else
            {
                HinhVuongSoo[left].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSoo[left].Refresh();                   //update màu lại cho ô vuông
            }
            if ((i - j) > 1)
            {
                HinhVuongSoo[i - 1].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSoo[i - 1].Refresh();                   //update màu lại cho ô vuông
            }
            if (right > i)    // phân hoạch đoạn bên phải
            {
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 20, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }

                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 21, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                }

                actionSorttangdan(i, right, solandequi + 1, HinhVuongSoo, DaySoNguyenn, ThuatToan, SoSanh);
                try
                {
                    await returnAwaitCorresponding(solandequi + 1);
                }
                catch
                {

                }
                //MessageBox.Show(solandequi.ToString());
            }
            else
            {
                HinhVuongSoo[right].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSoo[right].Refresh();                   //update màu lại cho ô vuông
            }
            if (solandequi == 0)
            {
                m = 0;
                n = 0;
                enableControlsAfterFinish();
                ButtonLamLai.Enabled = true;
                buttonBatDau.Enabled = false;
                if (SoSanh != true)
                {
                    // Nếu không phải đang chạy so sánh thì
                    // xuất ra thông báo hoàn thành khi xong
                    KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                    gif_timer.Stop();
                    ktb.ShowDialog();
                }
                else
                {
                    // Nếu đang chạy so sánh thì khi hoàn thành
                    // ngưng đếm thời gian và xuất ra labelThoiGianSoSanh1
                    ThoiGianChay.Stop();
                    TimeSpan ts = ThoiGianChay.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                    if (ThuatToan == 1)
                    {
                        labelThoiGianSoSanh1.Text = "Thời gian: " + elapsedTime;
                        labelThoiGianSoSanh1.Visible = true;
                        labelThoiGianSoSanh1.Refresh();
                    }
                    else if (ThuatToan == 2)
                    {
                        labelThoiGianSoSanh2.Text = "Thời gian: " + elapsedTime;
                        labelThoiGianSoSanh2.Visible = true;
                        labelThoiGianSoSanh2.Refresh();
                    }
                }
            }
            else
            {
                //MessageBox.Show("cancel "+solandequi.ToString());
                returnAwaitCorrespondingObject(solandequi).Cancel();
            }
        }

        private async void actionSortgiamdan(int left, int right, int solandequi, O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            Stopwatch ThoiGianChay = new Stopwatch();
            // Nếu đang chạy trong chế độ so sánh thì
            // tạo stopWatch để đếm thời gian chạy
            if (SoSanh == true)
            {
                ThoiGianChay.Start();
            }

            changeColorPos(left, right);
            int i, j;
            int x;
            int posx = (left + right) / 2; // xác định vị trí phần tử giữa
            x = DaySoNguyenn[posx];        // chọn phần tử giữa làm gốc                   

            resetRichtextboxColor(lines, 23);
            richTextBoxCodeThuatToan.Refresh();
            HighlightRichTextBox(lines, 3, colorhighlight);
            richTextBoxCodeThuatToan.Refresh();
            showx(posx);
            labelx.Refresh();

            await waitForInput();
            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
            if (buttonLamLaiClicked)
            {
                returnAwaitCorrespondingObject(solandequi).Cancel();
                return;
            }

            i = left;
            j = right;

            resetRichtextboxColor(lines, 23);
            richTextBoxCodeThuatToan.Refresh();
            HighlightRichTextBox(lines, 4, colorhighlight);
            richTextBoxCodeThuatToan.Refresh();
            if (left <= i && i <= right)
            {
                showi_q(i);
                labeli.Refresh();
                HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                HinhVuongSoo[i].Refresh();
            }
            if (left <= j && j <= right)
            {
                showj_q(j);
                labelj.Refresh();
                HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                HinhVuongSoo[j].Refresh();
            }
            await waitForInput();
            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
            if (buttonLamLaiClicked)
            {
                returnAwaitCorrespondingObject(solandequi).Cancel();
                return;
            }
            do
            {
                while (DaySoNguyenn[i] > x)
                {
                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 7, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }

                    HinhVuongSoo[i].BackColor = Color.Gray;
                    HinhVuongSoo[i].Refresh();

                    i++;    // lặp đến khi a[i] < x
                    if (i == j)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 8, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (0 <= j && j < DaySoNguyen.Length)
                        {
                            showibangj(j);
                            labelibangj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(151, 99, 230);
                            HinhVuongSoo[j].Refresh();
                        }

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                    else
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 8, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (left <= i && i <= right)
                        {
                            showi_q(i);
                            labeli.Refresh();
                            HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[i].Refresh();
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                }
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 9, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();
                    await waitForInput();

                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                }

                while (DaySoNguyenn[j] < x)
                {
                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 9, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }

                    HinhVuongSoo[j].BackColor = Color.Gray;
                    HinhVuongSoo[j].Refresh();
                    j--;    // lặp đến khi a[j] > x

                    if (j == i)
                    {

                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 10, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (0 <= j && j < DaySoNguyenn.Length)
                        {
                            showibangj(j);
                            labelibangj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(151, 99, 230);
                            HinhVuongSoo[j].Refresh();
                        }

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                    else
                    {

                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 10, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (left <= j && j <= right)
                        {
                            showj_q(j);
                            labelj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[j].Refresh();
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                }
                if (SoSanh != true)
                {
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                }


                if (i <= j)        // nếu có 2 phần tử a[i] và a[j] ko theo thứ tự
                {
                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 11, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }

                    doiCho2OVuong(i, j, HinhVuongSoo);

                    HinhVuongtemp = HinhVuongSoo[i];                  //Hoán vị 2 hình vuông
                    HinhVuongSoo[i] = HinhVuongSoo[j];           //Hoán vị 2 hình vuông 
                    HinhVuongSoo[j] = HinhVuongtemp;                  //Hoán vị 2 hình vuông                  
                    HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                    HinhVuongSoo[i].Refresh();
                    HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                    HinhVuongSoo[j].Refresh();
                    HinhVuongSoo[i].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                    HinhVuongSoo[j].thietLaplaiStatus();

                    int temp = DaySoNguyenn[i];
                    DaySoNguyenn[i] = DaySoNguyenn[j];
                    DaySoNguyenn[j] = temp;

                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 13, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }

                    HinhVuongSoo[i].BackColor = Color.Gray;
                    HinhVuongSoo[i].Refresh();
                    i++;
                    if (i == j)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 14, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (0 <= j && j < DaySoNguyenn.Length)
                        {
                            showibangj(j);
                            labelibangj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(151, 99, 230);
                            HinhVuongSoo[j].Refresh();
                        }

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                    else
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 14, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (left <= i && i <= right)
                        {
                            showi_q(i);
                            labelj.Refresh();
                            HinhVuongSoo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[i].Refresh();
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }

                    HinhVuongSoo[j].BackColor = Color.Gray;
                    HinhVuongSoo[j].Refresh();
                    j--;
                    if (i == j)
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 15, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (0 <= j && j < DaySoNguyenn.Length)
                        {
                            showibangj(j);
                            labelibangj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(151, 99, 230);
                            HinhVuongSoo[j].Refresh();
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                    else
                    {
                        resetRichtextboxColor(lines, 23);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 15, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();
                        if (left <= j && j <= right)
                        {
                            showj_q(j);
                            labelj.Refresh();
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[j].Refresh();
                        }
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            returnAwaitCorrespondingObject(solandequi).Cancel();
                            return;
                        }
                    }
                }
            } while (i <= j);

            if (SoSanh != true)
            {
                resetRichtextboxColor(lines, 23);
                richTextBoxCodeThuatToan.Refresh();
                HighlightRichTextBox(lines, 17, colorhighlight);
                richTextBoxCodeThuatToan.Refresh();

                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    returnAwaitCorrespondingObject(solandequi).Cancel();
                    return;
                }
            }

            if (left < j)    // phân hoạch đoạn bên trái
            {
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 18, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 19, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                }
                
                actionSortgiamdan(left, j, solandequi + 1, HinhVuongSoo, DaySoNguyenn, ThuatToan, SoSanh);

                try
                {
                    await returnAwaitCorresponding(solandequi + 1);
                }
                catch
                {

                }
                //MessageBox.Show(solandequi.ToString());
            }
            else
            {
                HinhVuongSoo[left].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSoo[left].Refresh();                   //update màu lại cho ô vuông
            }
            if ((i - j) > 1)
            {
                HinhVuongSoo[i - 1].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSoo[i - 1].Refresh();                   //update màu lại cho ô vuông
            }
            if (right > i)    // phân hoạch đoạn bên phải
            {
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 20, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }

                    resetRichtextboxColor(lines, 23);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 21, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        returnAwaitCorrespondingObject(solandequi).Cancel();
                        return;
                    }
                }
                
                actionSortgiamdan(i, right, solandequi + 1, HinhVuongSoo, DaySoNguyenn, ThuatToan, SoSanh);
                try
                {
                    await returnAwaitCorresponding(solandequi + 1);
                }
                catch
                {

                }
                //MessageBox.Show(solandequi.ToString());
            }
            else
            {
                HinhVuongSoo[right].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSoo[right].Refresh();                   //update màu lại cho ô vuông
            }
            if (solandequi == 0)
            {
                m = 0;
                n = 0;
                enableControlsAfterFinish();
                ButtonLamLai.Enabled = true;
                buttonBatDau.Enabled = false;
                if (SoSanh != true)
                {
                    // Nếu không phải đang chạy so sánh thì
                    // xuất ra thông báo hoàn thành khi xong
                    KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                    gif_timer.Stop();
                    ktb.ShowDialog();
                }
                else
                {
                    // Nếu đang chạy so sánh thì khi hoàn thành
                    // ngưng đếm thời gian và xuất ra labelThoiGianSoSanh1
                    ThoiGianChay.Stop();
                    TimeSpan ts = ThoiGianChay.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                    if (ThuatToan == 1)
                    {
                        labelThoiGianSoSanh1.Text = "Thời gian: " + elapsedTime;
                        labelThoiGianSoSanh1.Visible = true;
                        labelThoiGianSoSanh1.Refresh();
                    }
                    else if (ThuatToan == 2)
                    {
                        labelThoiGianSoSanh2.Text = "Thời gian: " + elapsedTime;
                        labelThoiGianSoSanh2.Visible = true;
                        labelThoiGianSoSanh2.Refresh();
                    }
                }
            }
            else
            {
                //MessageBox.Show("cancel "+solandequi.ToString());
                returnAwaitCorrespondingObject(solandequi).Cancel();
            }
        }
        #endregion

        #region Thuật toán Shell Sort
        private async void Shell_sort(O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            Stopwatch ThoiGianChay = new Stopwatch();
            // Nếu đang chạy trong chế độ so sánh thì
            // tạo stopWatch để đếm thời gian chạy
            if (SoSanh == true)
            {
                ThoiGianChay.Start();
            }

            if (SoSanh != true)
            {
                resetRichtextboxColor(lines, 3);
                richTextBoxCodeThuatToan.Refresh();
                HighlightRichTextBox(lines, 2, colorhighlight);
                richTextBoxCodeThuatToan.Refresh();

                await waitForInput();
                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                if (buttonLamLaiClicked)
                {
                    buttonLamLaiClicked = false;
                    return;
                }
            }

            int[] gap = { 5, 3, 2, 1 }; // mảng bước nhảy
            for (int igap = 0; igap < 4; igap++) // bước nhảy bắt đầu từ 5, giảm dần theo dãy fibonacci
            {
                if (SoSanh != true)
                {
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }

                    resetRichtextboxColor(lines, 15);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 3, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }
                for (int i = igap; i < DaySoNguyenn.Length; i++) // bắt đầu phần tử đầu tiên của bước nhảy gap[igap]
                {
                    await waitForInput();
                    //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }

                    showi(i, ThuatToan);
                    labeli.Refresh();

                    resetRichtextboxColor(lines, 13);
                    richTextBoxCodeThuatToan.Refresh();
                    HighlightRichTextBox(lines, 5, colorhighlight);
                    richTextBoxCodeThuatToan.Refresh();

                    if (SoSanh != true)
                    {
                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        resetRichtextboxColor(lines, 8);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 7, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }

                        resetRichtextboxColor(lines, 9);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 8, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }
                    int temp = DaySoNguyenn[i]; //phần tử đang xét
                    int j;
                    if (RadioButtonTangDan.Checked)
                    {
                        for (j = i; j >= gap[igap] && DaySoNguyenn[j - gap[igap]] > temp; j -= gap[igap]) // insert phần tử đang xét vào trước các phần tử trước nó theo bước nhảy gap[igap]
                        {
                            if (SoSanh != true)
                            {
                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }

                                resetRichtextboxColor(lines, 11);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 9, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();
                            }

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            showi(j, ThuatToan);
                            labeli.Refresh();

                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[j].Refresh();

                            showj(j - gap[igap], ThuatToan);
                            labelj.Refresh();

                            HinhVuongSoo[j - gap[igap]].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[j - gap[igap]].Refresh();

                            resetRichtextboxColor(lines, 11);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 10, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            doiCho2OVuong(j - gap[igap], j, HinhVuongSoo);

                            HinhVuongtemp = HinhVuongSoo[j - gap[igap]];                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[j - gap[igap]] = HinhVuongSoo[j];           //Hoán vị 2 hình vuông 
                            HinhVuongSoo[j] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[j - gap[igap]].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[j - gap[igap]].Refresh();
                            HinhVuongSoo[j].Refresh();

                            HinhVuongSoo[j - gap[igap]].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                            HinhVuongSoo[j].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            HinhVuongSoo[j - gap[igap]].BackColor = System.Drawing.Color.Gray;
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.Gray;
                            HinhVuongSoo[j].Refresh();
                            HinhVuongSoo[j - gap[igap]].Refresh();

                            DaySoNguyenn[j] = DaySoNguyenn[j - gap[igap]];
                        }
                    }
                    else
                    {
                        for (j = i; j >= gap[igap] && DaySoNguyenn[j - gap[igap]] < temp; j -= gap[igap])
                        {
                            if (SoSanh != true)
                            {
                                await waitForInput();
                                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                if (buttonLamLaiClicked)
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }

                                resetRichtextboxColor(lines, 11);
                                richTextBoxCodeThuatToan.Refresh();
                                HighlightRichTextBox(lines, 9, colorhighlight);
                                richTextBoxCodeThuatToan.Refresh();
                            }

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            showi(j, ThuatToan);
                            labeli.Refresh();

                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[j].Refresh();

                            showj(j - gap[igap], ThuatToan);
                            labelj.Refresh();

                            HinhVuongSoo[j - gap[igap]].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[j - gap[igap]].Refresh();

                            resetRichtextboxColor(lines, 11);
                            richTextBoxCodeThuatToan.Refresh();
                            HighlightRichTextBox(lines, 10, colorhighlight);
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            doiCho2OVuong(j - gap[igap], j, HinhVuongSoo);

                            HinhVuongtemp = HinhVuongSoo[j - gap[igap]];                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[j - gap[igap]] = HinhVuongSoo[j];           //Hoán vị 2 hình vuông 
                            HinhVuongSoo[j] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                            HinhVuongSoo[j - gap[igap]].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                            HinhVuongSoo[j - gap[igap]].Refresh();
                            HinhVuongSoo[j].Refresh();

                            HinhVuongSoo[j - gap[igap]].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
                            HinhVuongSoo[j].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            HinhVuongSoo[j - gap[igap]].BackColor = System.Drawing.Color.Gray;
                            HinhVuongSoo[j].BackColor = System.Drawing.Color.Gray;
                            HinhVuongSoo[j].Refresh();
                            HinhVuongSoo[j - gap[igap]].Refresh();

                            await waitForInput();
                            //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            if (buttonLamLaiClicked)
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }

                            DaySoNguyenn[j] = DaySoNguyenn[j - gap[igap]];
                        }
                    }

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 12);
                        richTextBoxCodeThuatToan.Refresh();
                        HighlightRichTextBox(lines, 11, colorhighlight);
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        if (buttonLamLaiClicked)
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                    }
                    DaySoNguyenn[j] = temp;
                }

                if (igap == 3)
                {
                    for (int x = 0; x <
                        DaySoNguyenn.Length; x++)
                        HinhVuongSoo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                    resetRichtextboxColor(lines, 15);
                    richTextBoxCodeThuatToan.Refresh();
                    enableControlsAfterFinish();

                    if (SoSanh != true)
                    {
                        // Nếu không phải đang chạy so sánh thì
                        // xuất ra thông báo hoàn thành khi xong
                        KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                        gif_timer.Stop();
                        ktb.ShowDialog();
                        break;
                    }
                    else
                    {
                        // Nếu đang chạy so sánh thì khi hoàn thành
                        // ngưng đếm thời gian và xuất ra labelThoiGianSoSanh1
                        ThoiGianChay.Stop();
                        TimeSpan ts = ThoiGianChay.Elapsed;
                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                        if (ThuatToan == 1)
                        {
                            labelThoiGianSoSanh1.Text = "Thời gian: " + elapsedTime;
                            labelThoiGianSoSanh1.Visible = true;
                            labelThoiGianSoSanh1.Refresh();
                        }
                        else if (ThuatToan == 2)
                        {
                            labelThoiGianSoSanh2.Text = "Thời gian: " + elapsedTime;
                            labelThoiGianSoSanh2.Visible = true;
                            labelThoiGianSoSanh2.Refresh();
                        }
                    }
                    break;
                }
            }
        }
        #endregion

        #region Thuật toán Merge sort
        int[] b = new int[10];
        int[] c = new int[10];
        int nb, nc;

        CancellationTokenSource waitForDistribute;
        Task WaitForDistributeFinish()
        {
            return Task.Delay(-1, waitForDistribute.Token);
        }
        CancellationTokenSource waitForMerge;
        Task WaitForMergeFinish()
        {
            return Task.Delay(-1, waitForMerge.Token);
        }

        async void Merge_sort(O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
            {
                continueFlag = new CancellationTokenSource();
            }
            else
            {
                //nếu không phải thì đang sắp xếp bên auto
                //tạo mới token ngưng thuật toán
                continueAutoFlag = new CancellationTokenSource();
                continueAutoFlag1 = new CancellationTokenSource();
            }

            Stopwatch ThoiGianChay = new Stopwatch();
            // Nếu đang chạy trong chế độ so sánh thì
            // tạo stopWatch để đếm thời gian chạy
            if (SoSanh == true)
            {
                ThoiGianChay.Start();
            }

            for (int k = 1; k < DaySoNguyenn.Length; k *= 2)
            {
                if (SoSanh != true)
                {
                    HighlightRichTextBox(lines, 3, colorhighlight);
                    richTextBoxCodeThuatToan.ScrollToCaret();
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }

                waitForDistribute = new CancellationTokenSource();

                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 6);
                    HighlightRichTextBox(lines, 5, colorhighlight);
                    //richTextBoxCodeThuatToan.ScrollToCaret();
                    richTextBoxCodeThuatToan.Refresh();
                    await waitForInput();
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }

                Distribute(k, HinhVuongSoo, DaySoNguyenn, ThuatToan, SoSanh);
                try
                {
                    await WaitForDistributeFinish();
                }
                catch
                {

                }

                waitForMerge = new CancellationTokenSource();

                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 67);
                    HighlightRichTextBox(lines, 6, colorhighlight);
                    //richTextBoxCodeThuatToan.ScrollToCaret();
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    if (buttonLamLaiClicked)
                    {
                        buttonLamLaiClicked = false;
                        return;
                    }
                }

                Merge(nb, nc, k, HinhVuongSoo, DaySoNguyenn, ThuatToan, SoSanh);
                try
                {
                    await WaitForMergeFinish();
                }
                catch
                {

                }
            }
            enableControlsAfterFinish();
            resetRichtextboxColor(lines, 67);
            for (int i = 0; i < HinhVuongSo.Length; i++)
            {
                HinhVuongSo[i].BackColor = Color.FromArgb(58, 130, 90);
            }
            if (SoSanh != true)
            {
                // Nếu không phải đang chạy so sánh thì
                // xuất ra thông báo hoàn thành khi xong
                KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
                gif_timer.Stop();
                ktb.ShowDialog();
            }
            else
            {
                // Nếu đang chạy so sánh thì khi hoàn thành
                // ngưng đếm thời gian và xuất ra labelThoiGianSoSanh1
                ThoiGianChay.Stop();
                TimeSpan ts = ThoiGianChay.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
                if (ThuatToan == 1)
                {
                    labelThoiGianSoSanh1.Text = "Thời gian: " + elapsedTime;
                    labelThoiGianSoSanh1.Visible = true;
                    labelThoiGianSoSanh1.Refresh();
                }
                else if (ThuatToan == 2)
                {
                    labelThoiGianSoSanh2.Text = "Thời gian: " + elapsedTime;
                    labelThoiGianSoSanh2.Visible = true;
                    labelThoiGianSoSanh2.Refresh();
                }
            }
        }

        O_vuong[] HinhVuongb = new O_vuong[10];
        O_vuong[] HinhVuongc = new O_vuong[10];
        async void Distribute(int k, O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            int x = 15; // biến vị trí của mảng bên trên
            int x1 = 15;// biến vị trí của mảng bên dưới
            int i, pa, pb, pc;
            pa = pb = pc = 0;
            while (pa < DaySoNguyenn.Length)
            {
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 67);
                    HighlightRichTextBox(lines, 13, colorhighlight);
                    //richTextBoxCodeThuatToan.ScrollToCaret();
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    if (buttonLamLaiClicked)
                    {
                        waitForDistribute.Cancel();
                        return;
                    }
                }

                for (i = 0; (pa < DaySoNguyenn.Length) && (i < k); i++, pa++, pb++)
                {
                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 67);
                        HighlightRichTextBox(lines, 15, colorhighlight);
                        //richTextBoxCodeThuatToan.ScrollToCaret();
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        if (buttonLamLaiClicked)
                        {
                            waitForDistribute.Cancel();
                            return;
                        }
                    }

                    b[pb] = DaySoNguyenn[pa];
                    HinhVuongb[pb] = HinhVuongSoo[pa];
                    HinhVuongSoo[pa].BackColor = Color.FromArgb(178, 75, 83);
                    Transition t = new Transition(new TransitionType_Linear(400));
                    t.add(HinhVuongSoo[pa], "Top", 0);
                    Transition t1 = new Transition(new TransitionType_Linear(400));
                    t1.add(HinhVuongSoo[pa], "Left", x);
                    Transition.runChain(t, t1);

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 67);
                        HighlightRichTextBox(lines, 16, colorhighlight);
                        //richTextBoxCodeThuatToan.ScrollToCaret();
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        if (buttonLamLaiClicked)
                        {
                            waitForDistribute.Cancel();
                            return;
                        }
                    }

                    x += 90;
                }
                for (i = 0; (pa < DaySoNguyenn.Length) && (i < k); i++, pa++, pc++)
                {
                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 67);
                        HighlightRichTextBox(lines, 17, colorhighlight);
                        //richTextBoxCodeThuatToan.ScrollToCaret();
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        if (buttonLamLaiClicked)
                        {
                            waitForDistribute.Cancel();
                            return;
                        }
                    }

                    c[pc] = DaySoNguyenn[pa];
                    HinhVuongc[pc] = HinhVuongSoo[pa];
                    HinhVuongSoo[pa].BackColor = Color.FromArgb(66, 104, 166);
                    Transition t = new Transition(new TransitionType_Linear(400));
                    t.add(HinhVuongSoo[pa], "Top", 150);
                    Transition t1 = new Transition(new TransitionType_Linear(400));
                    t1.add(HinhVuongSoo[pa], "Left", x1);
                    Transition.runChain(t, t1);

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 67);
                        HighlightRichTextBox(lines, 18, colorhighlight);
                        //richTextBoxCodeThuatToan.ScrollToCaret();
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        if (buttonLamLaiClicked)
                        {
                            waitForDistribute.Cancel();
                            return;
                        }
                    }

                    x1 += 90;
                }
            }
            nb = pb;
            nc = pc;
            waitForDistribute.Cancel();
        }
        async void Merge(int nb, int nc, int k, O_vuong[] HinhVuongSoo, int[] DaySoNguyenn, int ThuatToan, bool SoSanh)
        {
            int PhanTub = nb;
            int PhanTuc = nc;
            int x = 15;
            int y = 80;
            int p, pb, pc, ib, ic, kb, kc;
            p = pb = pc = 0; ib = ic = 0;
            while ((nb > 0) && (nc > 0))
            {
                if (SoSanh != true)
                {
                    resetRichtextboxColor(lines, 67);
                    HighlightRichTextBox(lines, 27, colorhighlight);
                    //richTextBoxCodeThuatToan.ScrollToCaret();
                    richTextBoxCodeThuatToan.Refresh();

                    await waitForInput();
                    if (buttonLamLaiClicked)
                    {
                        waitForMerge.Cancel();
                        return;
                    }
                }

                kb = min(k, nb); kc = min(k, nc);

                if (RadioButtonTangDan.Checked)
                {
                    if (b[pb + ib] <= c[pc + ic])
                    {
                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 67);
                            HighlightRichTextBox(lines, 30, colorhighlight);
                            //richTextBoxCodeThuatToan.ScrollToCaret();
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            if (buttonLamLaiClicked)
                            {
                                waitForMerge.Cancel();
                                return;
                            }
                        }

                        DaySoNguyenn[p] = b[pb + ib];
                        //O_vuong temp = HinhVuongSo[b[pb + ib].ViTri];
                        //HinhVuongSo[b[pb + ib].ViTri] = HinhVuongSo[p];
                        //HinhVuongSo[p] = temp;
                        HinhVuongSoo[p] = HinhVuongb[pb + ib];
                        HinhVuongSoo[p].BackColor = Color.Gray;
                        Transition t = new Transition(new TransitionType_Linear(400));
                        t.add(HinhVuongSoo[p], "Top", y);
                        Transition t1 = new Transition(new TransitionType_Linear(150));
                        t1.add(HinhVuongSoo[p++], "Left", x);
                        Transition.runChain(t, t1);
                        x += 90;

                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 67);
                            HighlightRichTextBox(lines, 32, colorhighlight);
                            //richTextBoxCodeThuatToan.ScrollToCaret();
                            richTextBoxCodeThuatToan.Refresh();
                            await waitForInput();
                            if (buttonLamLaiClicked)
                            {
                                waitForMerge.Cancel();
                                return;
                            }
                        }

                        ib++;

                        if (ib == kb)
                        {
                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 67);
                                HighlightRichTextBox(lines, 33, colorhighlight);
                                //richTextBoxCodeThuatToan.ScrollToCaret();
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                if (buttonLamLaiClicked)
                                {
                                    waitForMerge.Cancel();
                                    return;
                                }
                            }

                            for (; ic < kc; ic++)
                            {
                                if (SoSanh != true)
                                {
                                    resetRichtextboxColor(lines, 67);
                                    HighlightRichTextBox(lines, 35, colorhighlight);
                                    //richTextBoxCodeThuatToan.ScrollToCaret();
                                    richTextBoxCodeThuatToan.Refresh();

                                    await waitForInput();
                                    if (buttonLamLaiClicked)
                                    {
                                        waitForMerge.Cancel();
                                        return;
                                    }
                                }

                                DaySoNguyenn[p] = c[pc + ic];
                                //O_vuong temp1 = HinhVuongSo[c[pc + ic].ViTri];
                                //HinhVuongSo[c[pc + ic].ViTri] = HinhVuongSo[p];
                                //HinhVuongSo[p] = temp1;
                                HinhVuongSoo[p] = HinhVuongc[pc + ic];
                                HinhVuongSoo[p].BackColor = Color.Gray;
                                Transition t2 = new Transition(new TransitionType_Linear(400));
                                t2.add(HinhVuongSoo[p], "Top", y);
                                Transition t3 = new Transition(new TransitionType_Linear(150));
                                t3.add(HinhVuongSoo[p++], "Left", x);
                                Transition.runChain(t2, t3);
                                x += 90;

                                if (SoSanh != true)
                                {
                                    resetRichtextboxColor(lines, 67);
                                    HighlightRichTextBox(lines, 36, colorhighlight);
                                    //richTextBoxCodeThuatToan.ScrollToCaret();
                                    richTextBoxCodeThuatToan.Refresh();

                                    await waitForInput();
                                    if (buttonLamLaiClicked)
                                    {
                                        waitForMerge.Cancel();
                                        return;
                                    }
                                }
                            }
                            pb += kb;
                            pc += kc;
                            ib = ic = 0;
                            nb -= kb;
                            nc -= kc;
                        }
                    }
                    else
                    {
                        DaySoNguyenn[p] = c[pc + ic];
                        //O_vuong temp = HinhVuongSo[c[pc + ic].ViTri];
                        //HinhVuongSo[c[pc + ic].ViTri] = HinhVuongSo[p];
                        //HinhVuongSo[p] = temp;
                        HinhVuongSoo[p] = HinhVuongc[pc + ic];
                        HinhVuongSoo[p].BackColor = Color.Gray;
                        Transition t = new Transition(new TransitionType_Linear(400));
                        t.add(HinhVuongSoo[p], "Top", y);
                        Transition t1 = new Transition(new TransitionType_Linear(150));
                        t1.add(HinhVuongSoo[p++], "Left", x);
                        Transition.runChain(t, t1);
                        x += 90;

                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 67);
                            HighlightRichTextBox(lines, 46, colorhighlight);
                            //richTextBoxCodeThuatToan.ScrollToCaret();
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            if (buttonLamLaiClicked)
                            {
                                waitForMerge.Cancel();
                                return;
                            }
                        }

                        ic++;

                        if (ic == kc)
                        {
                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 67);
                                HighlightRichTextBox(lines, 47, colorhighlight);
                                //richTextBoxCodeThuatToan.ScrollToCaret();
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                if (buttonLamLaiClicked)
                                {
                                    waitForMerge.Cancel();
                                    return;
                                }
                            }

                            for (; ib < kb; ib++)
                            {
                                if (SoSanh != true)
                                {
                                    resetRichtextboxColor(lines, 67);
                                    HighlightRichTextBox(lines, 49, colorhighlight);
                                    //richTextBoxCodeThuatToan.ScrollToCaret();
                                    richTextBoxCodeThuatToan.Refresh();

                                    await waitForInput();
                                    if (buttonLamLaiClicked)
                                    {
                                        waitForMerge.Cancel();
                                        return;
                                    }
                                }

                                DaySoNguyenn[p] = b[pb + ib];
                                //O_vuong temp1 = HinhVuongSo[b[pb + ib].ViTri];
                                //HinhVuongSo[b[pb + ib].ViTri] = HinhVuongSo[p];
                                //HinhVuongSo[p] = temp1;
                                HinhVuongSoo[p] = HinhVuongb[pb + ib];
                                HinhVuongSoo[p].BackColor = Color.Gray;
                                Transition t2 = new Transition(new TransitionType_Linear(400));
                                t2.add(HinhVuongSoo[p], "Top", y);
                                Transition t3 = new Transition(new TransitionType_Linear(150));
                                t3.add(HinhVuongSoo[p++], "Left", x);
                                Transition.runChain(t2, t3);
                                x += 90;

                                if (SoSanh != true)
                                {
                                    resetRichtextboxColor(lines, 67);
                                    HighlightRichTextBox(lines, 50, colorhighlight);
                                    //richTextBoxCodeThuatToan.ScrollToCaret();
                                    richTextBoxCodeThuatToan.Refresh();

                                    await waitForInput();
                                    if (buttonLamLaiClicked)
                                    {
                                        waitForMerge.Cancel();
                                        return;
                                    }
                                }
                            }
                            pb += kb;
                            pc += kc;
                            ib = ic = 0;
                            nb -= kb;
                            nc -= kc;
                        }
                    }
                }
                else
                {
                    if (b[pb + ib] >= c[pc + ic])
                    {
                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 67);
                            HighlightRichTextBox(lines, 30, colorhighlight);
                            //richTextBoxCodeThuatToan.ScrollToCaret();
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            if (buttonLamLaiClicked)
                            {
                                waitForMerge.Cancel();
                                return;
                            }
                        }

                        DaySoNguyenn[p] = b[pb + ib];
                        //O_vuong temp = HinhVuongSo[b[pb + ib].ViTri];
                        //HinhVuongSo[b[pb + ib].ViTri] = HinhVuongSo[p];
                        //HinhVuongSo[p] = temp;
                        HinhVuongSoo[p] = HinhVuongb[pb + ib];
                        HinhVuongSoo[p].BackColor = Color.Gray;
                        Transition t = new Transition(new TransitionType_Linear(400));
                        t.add(HinhVuongSoo[p], "Top", y);
                        Transition t1 = new Transition(new TransitionType_Linear(150));
                        t1.add(HinhVuongSoo[p++], "Left", x);
                        Transition.runChain(t, t1);
                        x += 90;

                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 67);
                            HighlightRichTextBox(lines, 32, colorhighlight);
                            //richTextBoxCodeThuatToan.ScrollToCaret();
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            if (buttonLamLaiClicked)
                            {
                                waitForMerge.Cancel();
                                return;
                            }
                        }

                        ib++;

                        if (ib == kb)
                        {
                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 67);
                                HighlightRichTextBox(lines, 33, colorhighlight);
                                //richTextBoxCodeThuatToan.ScrollToCaret();
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                if (buttonLamLaiClicked)
                                {
                                    waitForMerge.Cancel();
                                    return;
                                }
                            }

                            for (; ic < kc; ic++)
                            {
                                if (SoSanh != true)
                                {
                                    resetRichtextboxColor(lines, 67);
                                    HighlightRichTextBox(lines, 35, colorhighlight);
                                    //richTextBoxCodeThuatToan.ScrollToCaret();
                                    richTextBoxCodeThuatToan.Refresh();

                                    await waitForInput();
                                    if (buttonLamLaiClicked)
                                    {
                                        waitForMerge.Cancel();
                                        return;
                                    }
                                }

                                DaySoNguyenn[p] = c[pc + ic];
                                //O_vuong temp1 = HinhVuongSo[c[pc + ic].ViTri];
                                //HinhVuongSo[c[pc + ic].ViTri] = HinhVuongSo[p];
                                //HinhVuongSo[p] = temp1;
                                HinhVuongSoo[p] = HinhVuongc[pc + ic];
                                HinhVuongSoo[p].BackColor = Color.Gray;
                                Transition t2 = new Transition(new TransitionType_Linear(400));
                                t2.add(HinhVuongSoo[p], "Top", y);
                                Transition t3 = new Transition(new TransitionType_Linear(150));
                                t3.add(HinhVuongSoo[p++], "Left", x);
                                Transition.runChain(t2, t3);
                                x += 90;

                                if (SoSanh != true)
                                {
                                    resetRichtextboxColor(lines, 67);
                                    HighlightRichTextBox(lines, 36, colorhighlight);
                                    //richTextBoxCodeThuatToan.ScrollToCaret();
                                    richTextBoxCodeThuatToan.Refresh();

                                    await waitForInput();
                                    if (buttonLamLaiClicked)
                                    {
                                        waitForMerge.Cancel();
                                        return;
                                    }
                                }
                            }
                            pb += kb;
                            pc += kc;
                            ib = ic = 0;
                            nb -= kb;
                            nc -= kc;
                        }
                    }
                    else
                    {
                        DaySoNguyenn[p] = c[pc + ic];
                        //O_vuong temp = HinhVuongSo[c[pc + ic].ViTri];
                        //HinhVuongSo[c[pc + ic].ViTri] = HinhVuongSo[p];
                        //HinhVuongSo[p] = temp;
                        HinhVuongSoo[p] = HinhVuongc[pc + ic];
                        HinhVuongSoo[p].BackColor = Color.Gray;
                        Transition t = new Transition(new TransitionType_Linear(400));
                        t.add(HinhVuongSoo[p], "Top", y);
                        Transition t1 = new Transition(new TransitionType_Linear(150));
                        t1.add(HinhVuongSoo[p++], "Left", x);
                        Transition.runChain(t, t1);
                        x += 90;

                        if (SoSanh != true)
                        {
                            resetRichtextboxColor(lines, 67);
                            HighlightRichTextBox(lines, 46, colorhighlight);
                            //richTextBoxCodeThuatToan.ScrollToCaret();
                            richTextBoxCodeThuatToan.Refresh();

                            await waitForInput();
                            if (buttonLamLaiClicked)
                            {
                                waitForMerge.Cancel();
                                return;
                            }
                        }

                        ic++;

                        if (ic == kc)
                        {
                            if (SoSanh != true)
                            {
                                resetRichtextboxColor(lines, 67);
                                HighlightRichTextBox(lines, 47, colorhighlight);
                                //richTextBoxCodeThuatToan.ScrollToCaret();
                                richTextBoxCodeThuatToan.Refresh();

                                await waitForInput();
                                if (buttonLamLaiClicked)
                                {
                                    waitForMerge.Cancel();
                                    return;
                                }
                            }

                            for (; ib < kb; ib++)
                            {
                                if (SoSanh != true)
                                {
                                    resetRichtextboxColor(lines, 67);
                                    HighlightRichTextBox(lines, 49, colorhighlight);
                                    //richTextBoxCodeThuatToan.ScrollToCaret();
                                    richTextBoxCodeThuatToan.Refresh();

                                    await waitForInput();
                                    if (buttonLamLaiClicked)
                                    {
                                        waitForMerge.Cancel();
                                        return;
                                    }
                                }

                                DaySoNguyenn[p] = b[pb + ib];
                                //O_vuong temp1 = HinhVuongSo[b[pb + ib].ViTri];
                                //HinhVuongSo[b[pb + ib].ViTri] = HinhVuongSo[p];
                                //HinhVuongSo[p] = temp1;
                                HinhVuongSoo[p] = HinhVuongb[pb + ib];
                                HinhVuongSoo[p].BackColor = Color.Gray;
                                Transition t2 = new Transition(new TransitionType_Linear(400));
                                t2.add(HinhVuongSoo[p], "Top", y);
                                Transition t3 = new Transition(new TransitionType_Linear(150));
                                t3.add(HinhVuongSoo[p++], "Left", x);
                                Transition.runChain(t2, t3);
                                x += 90;

                                if (SoSanh != true)
                                {
                                    resetRichtextboxColor(lines, 67);
                                    HighlightRichTextBox(lines, 50, colorhighlight);
                                    //richTextBoxCodeThuatToan.ScrollToCaret();
                                    richTextBoxCodeThuatToan.Refresh();

                                    await waitForInput();
                                    if (buttonLamLaiClicked)
                                    {
                                        waitForMerge.Cancel();
                                        return;
                                    }
                                }
                            }
                            pb += kb;
                            pc += kc;
                            ib = ic = 0;
                            nb -= kb;
                            nc -= kc;
                        }
                    }
                }
            }
            if (nb > 0)
            {
                for (int i = pb; i < PhanTub; i++)
                {
                    DaySoNguyenn[p] = b[i];
                    //O_vuong temp1 = HinhVuongSo[b[pb + ib].ViTri];
                    //HinhVuongSo[b[pb + ib].ViTri] = HinhVuongSo[p];
                    //HinhVuongSo[p] = temp1;
                    HinhVuongSoo[p] = HinhVuongb[i];
                    HinhVuongSoo[p].BackColor = Color.Gray;
                    Transition t2 = new Transition(new TransitionType_Linear(400));
                    t2.add(HinhVuongSoo[p], "Top", y);
                    Transition t3 = new Transition(new TransitionType_Linear(150));
                    t3.add(HinhVuongSoo[p++], "Left", x);
                    Transition.runChain(t2, t3);
                    x += 90;

                    if (SoSanh != true)
                    {
                        resetRichtextboxColor(lines, 67);
                        HighlightRichTextBox(lines, 50, colorhighlight);
                        //richTextBoxCodeThuatToan.ScrollToCaret();
                        richTextBoxCodeThuatToan.Refresh();

                        await waitForInput();
                        if (buttonLamLaiClicked)
                        {
                            waitForMerge.Cancel();
                            return;
                        }
                    }
                }
            }
            waitForMerge.Cancel();
        }
        int min(int a, int b)
        {
            return a > b ? b : a;
        }
        #endregion
        #endregion


#region Thao tác trên tab Auto

        // Biến cờ cho biết đang chạy thuật toán
        // A hay B trong so sánh, 1=A, 2=B
        int ThuatToan = 1;
        int m = 0;
        int n = 0;
        /// <summary>
        /// Hàm sự kiện ấn nút bắt đầu chạy thuật toán
        /// chế độ tự động
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBatDau_Click(object sender, EventArgs e)
        {
            if (isSoSanhMode == true)
            {
                if (ComboBoxThuatToan.Text == ComboBoxThuatToan2.Text)
                {
                    KhungThongBao ktb = new KhungThongBao("Lỗi", "2 thuật toán được chọn phải khác nhau để so sánh", true, false);
                    ktb.ShowDialog();
                    return;
                }
            }

            lines = richTextBoxCodeThuatToan.Lines;
            // Khởi tạo hai vị trí i và j
            if (!daKhoiTaoiVaj)
            {
                khoiTaoiVaj(1);
                khoiTaoiVaj(2);
                daKhoiTaoiVaj = true;
            }
            PictureBoxDebug.Enabled = false;
            PictureBoxAuto.Enabled = false;
            pictureBoxSoSanh.Enabled = false;

            buttonLamLaiClicked = false; // bắt đầu r thì đc phép làm lại

            // cho phép Ngưng sau khi bắt đầu
            buttonNgung.Enabled = true; 
            buttonNgung.BackgroundImage = Properties.Resources.NgungEnable;

            // đã bắt đầu r thì ko cho bắt đầu nữa, chỉ có tiếp tục
            buttonBatDau.Enabled = false; 
            buttonBatDau.BackgroundImage = Properties.Resources.BatDauDisable;

            ButtonNap.Enabled = false; // đã bắt đầu r thì ko cho nạp cho tới khi làm lại hoặc hoàn thành
            ButtonRandom.Enabled = false; // đã bắt đầu r thì ko cho nạp random cho tới khi làm lại hoặc hoàn thành
            fileButton.Enabled = false; // đã bắt đầu r thì ko cho nạp từ file cho tới khi làm lại hoặc hoàn thành
            buttonNgung.Focus(); // focus vào nút ngưng, cải thiện luồng thao tác

            //ko cho đổi thứ tự sắp xếp trong khi đang thực hiện sắp xếp
            RadioButtonTangDan.Enabled = false; 
            RadioButtonGiamDan.Enabled = false;
            ComboBoxThuatToan.Enabled = false;  //ko cho đổi thuật toán trong khi đang thực hiện sắp xếp

            if (isSoSanhMode == true)
            {
                switch (ComboBoxThuatToan.SelectedItem.ToString())
                {
                    case "Selection sort":
                        Selection_sort(HinhVuongSo, DaySoNguyen, 1, true);
                        break;
                    case "Interchange sort":
                        Interchange_sort(HinhVuongSo, DaySoNguyen, 1, true);
                        break;
                    case "Bubble sort":
                        Bubble_sort(HinhVuongSo, DaySoNguyen, 1, true);
                        break;
                    case "Insertion sort":
                        Insertion_sort(HinhVuongSo, DaySoNguyen, 1, true);
                        break;
                    case "Shaker sort":
                        Shaker_sort(HinhVuongSo, DaySoNguyen, 1, true);
                        break;
                    case "BinaryInsertion sort":
                        BinaryInsertion_sort(DaySoNguyen, HinhVuongSo);
                        break;
                    case "Quick sort":
                        Quick_sort(HinhVuongSo, DaySoNguyen, 1, true);
                        break;
                    case "Shell sort":
                        Shell_sort(HinhVuongSo, DaySoNguyen, 1, true);
                        break;
                    case "Heap sort":
                        HeapSort(HinhVuongSo, DaySoNguyen, 1, true);
                        break;
                    case "Merge sort":
                        Merge_sort(HinhVuongSo, DaySoNguyen, 1, true);
                        break;
                    default:
                        break;
                }

                switch (ComboBoxThuatToan2.SelectedItem.ToString())
                {
                    case "Selection sort":
                        Selection_sort(HinhVuongSo2, DaySoNguyen2, 2, true);
                        break;
                    case "Interchange sort":
                        Interchange_sort(HinhVuongSo2, DaySoNguyen2, 2, true);
                        break;
                    case "Bubble sort":
                        Bubble_sort(HinhVuongSo2, DaySoNguyen2, 2, true);
                        break;
                    case "Insertion sort":
                        Insertion_sort(HinhVuongSo2, DaySoNguyen2, 2, true);
                        break;
                    case "Shaker sort":
                        Shaker_sort(HinhVuongSo2, DaySoNguyen2, 2, true);
                        break;
                    case "BinaryInsertion sort":
                        BinaryInsertion_sort(DaySoNguyen, HinhVuongSo);
                        break;
                    case "Quick sort":
                        Quick_sort(HinhVuongSo2, DaySoNguyen2, 2, true);
                        break;
                    case "Shell sort":
                        Shell_sort(HinhVuongSo2, DaySoNguyen2, 2, true);
                        break;
                    case "Heap sort":
                        HeapSort(HinhVuongSo2, DaySoNguyen2, 2, true);
                        break;
                    case "Merge sort":
                        Merge_sort(HinhVuongSo2, DaySoNguyen2, 2, true);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (ComboBoxThuatToan.SelectedItem.ToString())
                {
                    case "Selection sort":
                        Selection_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Interchange sort":
                        Interchange_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Bubble sort":
                        Bubble_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Insertion sort":
                        Insertion_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Shaker sort":
                        Shaker_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "BinaryInsertion sort":
                        BinaryInsertion_sort(DaySoNguyen, HinhVuongSo);
                        break;
                    case "Quick sort":
                        Quick_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Shell sort":
                        Shell_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Heap sort":
                        HeapSort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    case "Merge sort":
                        Merge_sort(HinhVuongSo, DaySoNguyen, 1, false);
                        break;
                    default:
                        break;
                }
            }
            // Khởi tạo timer để chụp panel
            bitmapList = new List<Bitmap>();

            gif_timer.Tick += gif_timer_Tick;
            gif_timer.Start();

        }
        void gif_timer_Tick(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(panelHienThiOVuong.Width, panelHienThiOVuong.Height);
            panelHienThiOVuong.DrawToBitmap(bmp, new Rectangle(0, 0, panelHienThiOVuong.Width, panelHienThiOVuong.Height));
            imageHandler.RestorePrevious();
            imageHandler.CurrentBitmap = (Bitmap)Bitmap.FromFile(@"images\" + ComboBoxThuatToan.SelectedItem.ToString() + ".png");
            imageHandler.InsertImage(bmp, 2, 200);
            bitmapList.Add(imageHandler.CurrentBitmap);
            imageHandler.CurrentBitmap = imageHandler.BitmapBeforeProcessing;
        }
        System.Windows.Forms.Timer gif_timer = new System.Windows.Forms.Timer();
        List<Bitmap> bitmapList;
        ImageHandler imageHandler = new ImageHandler();
     
     #region Nút ngưng chạy auto và các hàm liên quan

        //biến cờ báo hiệu nút ngưng đã được ấn
        //là true nếu đã được ấn
        //là false nếu chưa được ấn
        bool buttonNgungClicked = false;
        /// <summary>
        /// Hàm sự kiện ấn nút ngưng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNgung_Click(object sender, EventArgs e)
        {
            DungThuatToan();
            if (continueAutoFlag1 != null)
            {
                continueAutoFlag1.Cancel();
            }            
        }

        
        private void DungThuatToan()            //hàm này dùng để dừng thuật toán
        {
            //cho phép ấn nút tiếp tục sau khi ngưng
            buttonTiepTuc.Enabled = true;
            buttonTiepTuc.BackgroundImage = Properties.Resources.TiepTucEnable;

            //đã ấn ngưng rối thì không được ấn nữa
            buttonNgung.Enabled = false;
            buttonNgung.BackgroundImage = Properties.Resources.NgungDisable;

            //cho phép làm lại nếu đã ngưng
            ButtonLamLai.Enabled = true;
            ButtonLamLai.BackgroundImage = Properties.Resources.LamLaiEnable;

            buttonTiepTuc.Focus();                                  
            buttonNgungClicked = true;          //biến cho biết nút ngưng được click            
            
            //set lại 2 cờ exitFlag để thoát khỏi vòng lặp doevents
            exitFlag1 = true;
            exitFlag2 = true;
        }        
        #endregion

     #region Nút tiếp tục sắp xếp Auto
        /// <summary>
        /// Hàm sự kiện ấn nút tiếp tục chạy auto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTieptuc_Click(object sender, EventArgs e)
        {
            buttonNgungClicked = false;      //đặt lại nút ngưng đã hết được click

            //cho phép ấn nút ngưng sau khi tiếp tục
            buttonNgung.Enabled = true;
            buttonNgung.BackgroundImage = Properties.Resources.NgungEnable;

            //không cho phép ấn nút tiếp tục vì đã ấn rồi
            buttonTiepTuc.Enabled = false;
            buttonTiepTuc.BackgroundImage = Properties.Resources.TiepTucDisable;

            //đã tiếp tục thì ko cho phép làm lại
            ButtonLamLai.Enabled = false;
            ButtonLamLai.BackgroundImage = Properties.Resources.LamLaiDisable;

            buttonNgung.Focus();             //tập trung vào nút Ngưng, ấn spacebar để dùng
            continueAutoFlag.Cancel();       //huỷ đợi việc ấn nút tiếp tục
        }


        CancellationTokenSource continueAutoFlag;
        CancellationTokenSource continueAutoFlag1;
        /// <summary>
        /// Hàm dùng để ngưng thực hiện thuật toán
        /// cho tới khi nút tiếp tục được ấn
        /// </summary>
        /// <returns></returns>
        Task waitForTiepTucClick()
        {
            //-1 là đợi mãi mãi
            //và chỉ đáp ứng lời gọi của continueAutoFlag
            return Task.Delay(-1, continueAutoFlag.Token);  
        }
        #endregion

     #region Nút làm lại
        bool buttonLamLaiClicked = false;
        private void ButtonLamLai_Click(object sender, EventArgs e)
        {
            // cho phép nạp dãy số, chọn thuật toán, chọn thứ tự khi làm lại
            ButtonNap.Enabled = true;
            ButtonRandom.Enabled = true;
            fileButton.Enabled = true;
            ComboBoxThuatToan.Enabled = true;
            RadioButtonGiamDan.Enabled = true;
            RadioButtonTangDan.Enabled = true;

            // ko cho phép làm lại sau khi ấn làm lại
            ButtonLamLai.Enabled = false;
            ButtonLamLai.BackgroundImage = Properties.Resources.LamLaiDisable;

            // ko cho phép tiếp tục khi chưa bắt đầu sau khi làm lại
            buttonTiepTuc.Enabled = false;
            buttonTiepTuc.BackgroundImage = Properties.Resources.TiepTucDisable;

            // cho phép tiến sau khi làm lại
            ButtonTien.Enabled = true;
            ButtonTien.BackgroundImage = Properties.Resources.TienEnable;

            // cho phép bắt đầu khi làm lại
            buttonBatDau.Enabled = true;
            buttonBatDau.BackgroundImage = Properties.Resources.BatDauEnable;

            // sau khi làm lại mà chưa bắt đầu thì chưa đc ngưng
            buttonNgung.Enabled = false;
            buttonNgung.BackgroundImage = Properties.Resources.NgungDisable;

            PictureBoxDebug.Enabled = true;
            PictureBoxAuto.Enabled = true;
            pictureBoxSoSanh.Enabled = true;

            // Ẩn label thời gian so sánh của 2 thuật toán
            labelThoiGianSoSanh1.Visible = false;
            labelThoiGianSoSanh1.Text = string.Empty;
            labelThoiGianSoSanh2.Visible = false;
            labelThoiGianSoSanh2.Text = string.Empty;

            DaySoNguyen = new int[DaySoTemp.Length];
            for (int i = 0; i < DaySoNguyen.Length; i++) // thực hiện việc nhập lại dãy số
            {
                DaySoNguyen[i] = DaySoTemp[i];
                DaySoNguyen2[i] = DaySoTemp[i];
            }
            //vẽ lại hình vuông cho thuật toán A
            makeNumSquare(DaySoNguyen, HinhVuongSo, panelHienThiOVuong);

            //vẽ lại ô vị trí cho thuật toán A
            makePosSquare(DaySoNguyen, posSquare, panelHienThiOVuong);

            //vẽ lại hình vuông cho thuật toán B
            makeNumSquare(DaySoNguyen2, HinhVuongSo2, panelHienThiOVuong2);

            //vẽ lại ô vị trí cho thuật toán B
            makePosSquare(DaySoNguyen2, posSquare2, panelHienThiOVuong2);

            m = 0;  // set lại biến cục bộ m
            n = 0;  // set lại biến cục bộ n
            labeli.Visible = false;
            labelj.Visible = false;

            // set màu lại cho richtextbox
            resetRichtextboxColor(lines, lines.Length);

            // câu if này chỉ chạy nếu hoàn thành sắp xếp hoặc đang sắp xếp mà ấn làm lại
            if (isStepbyStepStarted)     // nếu đang chạy bên step by step mà làm lại thì
            {
                isStepbyStepStarted = false; // set lại bằng false cho lần thực hiện mới tiếp  
                buttonLamLaiClicked = true; // báo hiệu button Làm lại đc ấn
                continueFlag.Cancel();
            }
            else if (buttonNgungClicked) //nếu đang ngưng bên auto và ấn làm lại thì vào
            {
                buttonNgungClicked = false; // đặt lại cờ ngưng
                buttonLamLaiClicked = true; // báo hiệu button Làm lại đc ấn
                continueAutoFlag.Cancel();  // nếu đang sắp xếp bên auto thì cancel token tiếp tục
            }

            // new lại mảng phụ cho merge sort
            b = new int[10];
            c = new int[10];

        }
        #endregion

     #region Hàm chỉnh tốc độ của timer
        private void modifySpeed(int speed)
        {
            timer1.Interval = speed;
            timer2.Interval = speed;
        }
        #endregion

     #region Sự kiện của thanh tốc độ auto
        int waitAmount;     //thời gian đợi giữa các câu lệnh trong thuật toán
        private void labelSpeed4_Click(object sender, EventArgs e)
        {
            waitAmount = 5;
            modifySpeed(50);
            pictureBoxSpeedPoint.Location = new Point(629, 116);
        }

        private void labelSpeed3_Click(object sender, EventArgs e)
        {
            waitAmount = 10;
            modifySpeed(100);
            pictureBoxSpeedPoint.Location = new Point(540, 116);
        }

        private void labelSpeed2_Click(object sender, EventArgs e)
        {
            waitAmount = 15;
            modifySpeed(150);
            pictureBoxSpeedPoint.Location = new Point(455, 116);
        }

        private void labelSpeed1_Click(object sender, EventArgs e)
        {
            waitAmount = 20;
            modifySpeed(200);
            pictureBoxSpeedPoint.Location = new Point(370, 116);
        }

        private void labelSpeed0_Click(object sender, EventArgs e)
        {
            waitAmount = 25;
            modifySpeed(250);
            pictureBoxSpeedPoint.Location = new Point(285, 116);
        }
        #endregion

        #endregion


#region Các hàm liên quan đến sắp xếp dãy số, ô vuông


    #region Đổi chổ 2 ô vuông
        static bool exitFlag1 = false;            //exitFlag1 dùng cho hàm timer1_Tick, là true nếu ô vuông m đã vào đúng vị trí
        static bool exitFlag2 = false;            //exitFlag2 dùng cho hàm timer1_Tick, là true nếu ô vuông n đã vào đúng vị trí
        O_vuong HinhVuongtemp;                    //biến temp để hoán vị thông tin của 2 label ô vuông
        int OVuong1 = 0;                          //biến dùng để lưu vị trí của ô vuông thứ nhất cần di chuyển trong hàm doiCho2OVuong
        int OVuong2 = 0;                          //biến dùng để lưu vị trí của ô vuông thứ hai cần di chuyển trong hàm doiCho2OVuong

        /// <summary>
        /// Hàm dùng để đổi chổ 2 ô vuông
        /// </summary>
        /// <param name="i">vị trí cả ô vuông 1 trong mảng ô vuông</param>
        /// <param name="j">vị trí cả ô vuông 2 trong mảng ô vuông</param>
        private void doiCho2OVuong(int i, int j, O_vuong[] HinhVuongSoo)
        {

            //set lại 2 cờ exitFlag để vào vòng lặp doevents
            exitFlag1 = false;
            exitFlag2 = false;

            //gán 2 biến OVuong1 và OVuong2 bằng với tham số i, j để đổi chổ
            OVuong1 = i;
            OVuong2 = j;

            // Di chuyển 2 hình vuông đi lên và xuống
            Transition t1 = new Transition(new TransitionType_Linear(100));
            t1.add(HinhVuongSoo[i], "Top", 0);
            t1.add(HinhVuongSoo[j], "Top", 120);

            // Di chuyển 2 hình vuông qua phải và trái
            Transition t2 = new Transition(new TransitionType_Linear(300));
            t2.add(HinhVuongSoo[i], "Left", HinhVuongSoo[j].Left);
            t2.add(HinhVuongSoo[j], "Left", HinhVuongSoo[i].Left);

            // Di chuyển 2 hình vuông đi xuống và lên
            Transition t3 = new Transition(new TransitionType_Linear(100));
            t3.add(HinhVuongSoo[i], "Top", 60);
            t3.add(HinhVuongSoo[j], "Top", 60);

            Transition.runChain(t1, t2, t3);
     
            exitFlag1 = false;                               //Đặt lại exitFlag1 sau khi thực hiện xong 1 lần đổ chỗ 2 ô vuông
            exitFlag2 = false;                               //Đặt lại exitFlag2 sau khi thực hiện xong 1 lần đổ chỗ 2 ô vuông          
        }
    #endregion
 #endregion

#region Khởi tạo i và j
        private void khoiTaoiVaj(int PanelHienThiOVuongg){
            if (PanelHienThiOVuongg == 1)
            {
                // khởi tạo i
                labeli.AutoSize = true;
                labeli.Font = new System.Drawing.Font("Times New Roman", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
                labeli.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                labeli.Top = posSquare[0].Top - 25;
                labeli.Left = posSquare[0].Left + 5;
                labeli.Name = "labeli";
                labeli.Size = new System.Drawing.Size(16, 22);
                labeli.TabIndex = 0;
                labeli.Text = "i";
                labeli.Visible = false;
                panelHienThiOVuong.Controls.Add(labeli);
                // khởi tạo j
                labelj.AutoSize = true;
                labelj.Font = new System.Drawing.Font("Times New Roman", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
                labelj.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                labelj.Top = posSquare[0].Top - 25;
                labelj.Left = posSquare[0].Left + 5;
                labelj.Name = "labelj";
                labelj.Size = new System.Drawing.Size(16, 22);
                labelj.TabIndex = 0;
                labelj.Text = "j";
                labelj.Visible = false;
                panelHienThiOVuong.Controls.Add(labelj);
            }
            else if (PanelHienThiOVuongg == 2)
            {
                // khởi tạo i
                labeli2.AutoSize = true;
                labeli2.Font = new System.Drawing.Font("Times New Roman", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
                labeli2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                labeli2.Top = posSquare[0].Top - 25;
                labeli2.Left = posSquare[0].Left + 5;
                labeli2.Name = "labeli";
                labeli2.Size = new System.Drawing.Size(16, 22);
                labeli2.TabIndex = 0;
                labeli2.Text = "i";
                labeli2.Visible = false;
                panelHienThiOVuong2.Controls.Add(labeli2);
                // khởi tạo j
                labelj2.AutoSize = true;
                labelj2.Font = new System.Drawing.Font("Times New Roman", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
                labelj2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                labelj2.Top = posSquare[0].Top - 25;
                labelj2.Left = posSquare[0].Left + 5;
                labelj2.Name = "labelj";
                labelj2.Size = new System.Drawing.Size(16, 22);
                labelj2.TabIndex = 0;
                labelj2.Text = "j";
                labelj2.Visible = false;
                panelHienThiOVuong2.Controls.Add(labelj2);
            }
            
        }
#endregion  

#region Hiển thị i, j
        private void showi(int pos, int ThuatToan)
        {
            if(ThuatToan == 1)
            {
                labeli.Visible = true;
                labeli.Top = posSquare[pos].Top - 25;
                labeli.Left = posSquare[pos].Left + 5;
                labeli.Refresh();
            }
            else if (ThuatToan == 2)
            {
                labeli2.Visible = true;
                labeli2.Top = posSquare[pos].Top - 25;
                labeli2.Left = posSquare[pos].Left + 5;
                labeli2.Refresh();
            }
        }


        
        private void showj(int pos, int ThuatToan)
        {
            if (ThuatToan == 1)
            {
                labelj.Visible = true;
                labelj.Top = posSquare[pos].Top - 25;
                labelj.Left = posSquare[pos].Left + 5;
                labelj.Refresh();
            }
            else if (ThuatToan == 2)
            {
                labelj2.Visible = true;
                labelj2.Top = posSquare[pos].Top - 25;
                labelj2.Left = posSquare[pos].Left + 5;
                labelj2.Refresh();
            }
        }

        #endregion

        /// <summary>
        /// Hàm dùng để highlight code thuật toán
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="dong"></param>
        /// <param name="color"></param>
        private void HighlightRichTextBox(string[] lines, int dong, Color color)
        {
            if (dong < 0 || dong > lines.Length)
                return;
            int batdau = richTextBoxCodeThuatToan.GetFirstCharIndexFromLine(dong);
            int dodai = lines[dong].Length;
            richTextBoxCodeThuatToan.Select(batdau, dodai);
            richTextBoxCodeThuatToan.SelectionBackColor = color;
            richTextBoxCodeThuatToan.ScrollToCaret();
        }
        

        /// <summary>
        /// Hàm dùng để reset màu của 1 dòng code thuật toán
        /// về màu ban đầu (đen)
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="dong"></param>
        private void resetRichtextboxColor(string [] lines, int dong)
        {
            if (dong < 0 || dong > lines.Length)
                return;
            int temp = 0;
            while (temp < dong)
            {
                int batdau = richTextBoxCodeThuatToan.GetFirstCharIndexFromLine(temp);
                int dodai = lines[temp++].Length;
                richTextBoxCodeThuatToan.Select(batdau, dodai);
                richTextBoxCodeThuatToan.SelectionBackColor = richTextBoxCodeThuatToan.BackColor;
            }
        }

        private void xuatfilegif_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            if (f.ShowDialog() == DialogResult.OK)
            {
                System.Windows.Media.Imaging.GifBitmapEncoder gEnc = new System.Windows.Media.Imaging.GifBitmapEncoder();
                foreach (var bmp in bitmapList)
                {
                    var hbmp = bmp.GetHbitmap();
                    var src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    gEnc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(src));
                }
                string fileName = ComboBoxThuatToan.SelectedItem.ToString() + ".gif";
                using (FileStream fs = new FileStream(@f.SelectedPath + fileName, FileMode.Create))
                {
                    gEnc.Save(fs);
                }
                bitmapList.Clear();
            }
        }    
    }
}


