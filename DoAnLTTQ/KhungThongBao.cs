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
    public partial class KhungThongBao : Form
    {
        bool ok, cancel;
        public KhungThongBao(string ThongBao, string NoiDung, bool buttonOK, bool buttonCancel)
        {
            InitializeComponent();
            labelThongBao.Text = ThongBao;
            labelNoiDung.Text = NoiDung;
            btnOK.Visible = buttonOK;
            ok = buttonOK;
            btnCancel.Visible = buttonCancel;
            cancel = buttonCancel;
        }

        bool mouseDown;
        Point MouseLo;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            MouseLo = new Point(e.X, e.Y);
        }


        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown == true)
            {
                this.Location = new Point(this.Location.X + e.X - MouseLo.X, this.Location.Y + e.Y - MouseLo.Y);
                this.Update();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxClose_MouseEnter(object sender, EventArgs e)
        {
            pictureBoxClose.BackColor = Color.FromArgb(242, 60, 45);
        }

        private void pictureBoxClose_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxClose.BackColor = Color.FromArgb(68, 114, 196);
        }

        private void KhungThongBao_Load(object sender, EventArgs e)
        {
            this.Width = labelNoiDung.Size.Width + 46;
            if (ok && cancel)
            {
                btnCancel.Location = new Point(this.Width - 100, 146);
                btnOK.Location = new Point(btnCancel.Width - 50, 146);
            }
            else if (ok)
            {
                btnOK.Location = new Point(this.Width / 2 - btnOK.Width / 2, 146);
            }
            Location = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - Size.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2 - Size.Height / 2);//asdasdasd
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxClose_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBoxClose.BackColor = Color.Red;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
