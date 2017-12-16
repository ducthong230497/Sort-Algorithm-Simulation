using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAnLTTQ
{
    public partial class inputSoLuongPhanTuRandom : Form
    {
        public inputSoLuongPhanTuRandom()
        {
            InitializeComponent();
        }

        private void inputSoLuongPhanTuRandom_Load(object sender, EventArgs e)
        {
            textBoxRandomInput.Clear();
        }

#region Thao tác trên ControlBox

        #region Kiểm tra chuột có được ấn giữ tại panel3 (thanh chứa các dấu thu nhỏ,...)
        bool mouseDown;
        Point MouseLo;
        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            MouseLo = new Point(e.X, e.Y);
        }
        #endregion

        #region Di chuyển form theo con chuột nếu con chuột đang được ấn giữ
        private void panel3_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown == true)
            {
                this.Location = new Point(this.Location.X + e.X - MouseLo.X, this.Location.Y + e.Y - MouseLo.Y);
                this.Update();
            }
        }
        #endregion

        #region Kiểm tra nếu con chuột không còn được ấn giữ tại panel3 nữa
        private void panel3_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        #endregion

        #region Ấn dấu X để tắt chương trình
        private void pictureBoxClose_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Hiệu ứng nút tắt
        private void pictureBoxClose_MouseEnter(object sender, EventArgs e)
        {
            pictureBoxClose.BackColor = Color.FromArgb(242, 60, 45);
        }

        private void pictureBoxClose_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxClose.BackColor = Color.FromArgb(68, 114, 196);
        }

        private void pictureBoxClose_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBoxClose.BackColor = Color.Red;
        }


        #endregion

        #endregion


        /// <summary>
        /// Ngăn không cho người dùng nhập ký tự khác
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxRandomInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Ngăn không cho nhập các phím control,..., các phím chữ
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        /// <summary>
        /// Đóng form hỏi số lượng phần tử
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
              

        public int SoLuongPhanTu = 0;
        public bool btnBatdau = false;
        public bool initArray = false;
        /// <summary>
        /// Ấn nút xác nhận số lượng phần tử cần random
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            string input;
            input = textBoxRandomInput.Text;
            input = input.Trim();

            //Xử lý trường hợp người dùng ấn nhiều dấu cách (spacebar)
            while(input.Contains("  "))
            {
                input = input.Replace("  ", " ");
            }
            
            //Kiểm tra xem người dùng có nhập gì chưa
            if (input == "" || input == "0")
            {
                KhungThongBao ktb = new KhungThongBao("Lỗi", "Chưa có dữ liệu gì được nhập.", true, false);
                ktb.ShowDialog();
            }
            else
            {
                initArray = true;
                SoLuongPhanTu = int.Parse(input);
                //Kiểm tra xem người dùng có nhập quá số lượng phần tử hay không?
                bool TriggerGioihanphantu = true;
                if (SoLuongPhanTu > 10)
                {
                    TriggerGioihanphantu = false;
                    textBoxRandomInput.Clear();
                    KhungThongBao ktb = new KhungThongBao("Lỗi", "Só lượng phần tử nhập quá số lượng quy định (tối đa 10). Vui lòng nhập lại.", true, false);
                    ktb.ShowDialog();
                    SoLuongPhanTu = 0;
                }
                if (TriggerGioihanphantu)
                    btnBatdau = true;
            }
        }



    }
}
