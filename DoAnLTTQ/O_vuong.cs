using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transitions;

namespace DoAnLTTQ
{
    class O_vuong: System.Windows.Forms.Label
    {
        public int topBanDau;
        public int leftBanDau;
        public bool status1; //status đi lên/xuống  
        public bool status2; //status qua phải/trái
        public bool status3; //status đi xuống/lên
        public O_vuong()
        {
            //Trang trí ô vuông
            this.Width = 50;                                                
            this.Height = 50;
            this.Visible = true;
            this.BackColor = System.Drawing.Color.Gray;
            this.Font = new System.Drawing.Font("Calibri", 20);
            this.AutoSize = false;
            this.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ForeColor = System.Drawing.Color.White;
            this.status1 = false;
            this.status2 = false;
            this.status3 = false;
        }

        /// <summary>
        /// Hàm dùng để di chuyển ô vuông đi lên
        /// </summary>
        public void diLen(int topValue)
        {
            Transition t = new Transition(new TransitionType_Linear(100));
            t.add(this, "Top", topValue);
            t.run();
            //this.Top = this.Top - 10;
        }


        /// <summary>
        /// Hàm dùng để di chuyển ô vuông đi xuống
        /// </summary>
        public void diXuong(int topValue)
        {
            Transition t = new Transition(new TransitionType_Linear(100));
            t.add(this, "Top", topValue);
            t.run();
            //this.Top = this.Top + 10;
        }


        /// <summary>
        /// Hàm dùng để di chuyển ô vuông qua phải
        /// </summary>
        public void quaPhai(int leftValue)
        {
            Transition t = new Transition(new TransitionType_Linear(100));
            t.add(this, "Left", leftValue);
            t.run();
            //this.Left = this.Left + 10;
        }


        /// <summary>
        /// Hàm dùng để di chuyển ô vuông qua trái
        /// </summary>
        public void quaTrai(int leftValue)
        {
            Transition t = new Transition(new TransitionType_Linear(100));
            t.add(this, "Left", leftValue);
            t.run();
            //this.Left = this.Left - 10;
        }


        /// <summary>
        /// Hàm dùng để lấy giá trị tọa độ ban đầu của ô vuông
        /// </summary>
        public void layGiaTriToaDoBanDau()
        {
            this.topBanDau = this.Top;
            this.leftBanDau = this.Left;
        }
        public void thietLaplaiStatus()     //Đặt lại status sau khi ô vuông vào đúng vị trí
        {
            this.status1 = false;
            this.status2 = false;
            this.status3 = false;
            this.topBanDau = this.Top;
            this.leftBanDau = this.Left;
        }
    }
    
}
