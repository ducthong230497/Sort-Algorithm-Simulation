using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace DoAnLTTQ
{
    public partial class FormThuatToanB : Form
    {
        int[] DaySoNguyen;  //là dãy số chính để thao tác cho thuật toán B
        int[] DaySoTemp;    //DaySoTemp dùng để lưu dãy số chính(DaySoNguyen) trong các thao tác: nạp, random, làm lại cho thuật toán B
        O_vuong[] HinhVuongSo;
        Label[] posSquare;                  //mảng label chứa label i, j cho HinhVuongSo2   
        bool daKhoiTaoiVaj = false;
        Label labeli = new Label();
        Label labelj = new Label();
        int m = 0;
        int n = 0;
        int ThuTuSapXep = 0; //0 = giảm dần, 1 = tăng dần
        bool buttonNgungClicked = false;//biến cờ báo hiệu nút ngưng đã được ấn, là true nếu đã được ấn, là false nếu chưa được ấn
        public FormThuatToanB()
        {
            InitializeComponent();
        }

        

        public void BatDauChayThuatToan(string ThuatToan, int ThuTuSapXepp)
        {
            // Khởi tạo hai vị trí i và j
            if (!daKhoiTaoiVaj)
            {
                khoiTaoiVaj();
                daKhoiTaoiVaj = true;
            }
            switch (ThuatToan)
            {
                case "Interchange sort":
                    Interchange_sort();
                    break;
                //case "Selection sort":
                //    Selection_sort();
                //    break;
                //case "Bubble sort":
                //    Bubble_sort();
                //    break;
                //case "Insertion sort":
                //    Insertion_sort();
                //    break;
                //case "Shaker sort":
                //    Shaker_sort();
                //    break;
                //case "BinaryInsertion sort":
                //    BinaryInsertion_sort(DaySoNguyen, HinhVuongSo, ref m, ref n);
                //    break;
                //case "Quick sort":
                //    Quick_sort();
                //    break;
                //case "Shell sort":
                //    Shell_sort();
                //    break;
                //case "Heap sort":
                //    HeapSort();
                //    break;
                //case "Merge sort":

                //    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Hàm dùng để dừng việc chạy thuật toán
        /// </summary>
        public void DungThuatToan()
        {
            timer1.Stop();                      //ngưng timer1
            timer2.Stop();                      //ngưng timer2
            timerThreadSleep.Stop();            //ngưng timer chờ            
            buttonNgungClicked = true;          //biến cho biết nút ngưng được click            

            //set lại 2 cờ exitFlag để thoát khỏi vòng lặp doevents
            exitFlag1 = true;
            exitFlag2 = true;
        }


        /// <summary>
        /// Hàm dùng để tiếp tục việc chạy thuật toán
        /// </summary>
        public void TiepTucThuatToan()
        {
            buttonNgungClicked = false;      //đặt lại nút ngưng đã hết được click
            continueAutoFlag.Cancel();       //huỷ đợi việc ấn nút tiếp tục
        }


        bool buttonLamLaiClicked = false;
        /// <summary>
        /// Hàm dùng để làm lại thuật toán
        /// </summary>
        public void LamLaiThuatToan()
        {
            DaySoNguyen = new int[DaySoTemp.Length];
            //thực hiện việc nhập lại dãy số
            for (int i = 0; i < DaySoNguyen.Length; i++)    
            {
                DaySoNguyen[i] = DaySoTemp[i];
            }

            //vẽ lại hình vuông số cho thuật toán B
            makeNumSquare(DaySoNguyen, HinhVuongSo);
            //vẽ lại ô vị trí cho thuật toán B
            makePosSquare(DaySoNguyen, posSquare);

            m = 0;  //set lại biến cục bộ m
            n = 0;  //set lại biến cục bộ n
            labeli.Visible = false;
            labelj.Visible = false;

            if (buttonNgungClicked) //nếu đang ngưng bên auto và ấn làm lại thì vào
            {
                buttonNgungClicked = false;   //đặt lại cờ ngưng
                buttonLamLaiClicked = true;   //báo hiệu button Làm lại đc ấn
                continueAutoFlag.Cancel();    //nếu đang sắp xếp bên auto thì cancel token tiếp tục
            }
        }


        CancellationTokenSource continueAutoFlag;
        /// <summary>
        /// hàm đợi nút tiếp tục được ấn
        /// </summary>
        /// <returns></returns>
        Task waitForTiepTucClick()
        {
            return Task.Delay(-1, continueAutoFlag.Token); //-1 là đợi mãi mãi, và chỉ đáp ứng lời gọi của continueAutoFlag
        }



        public void initFormThuatToanB(int[] DaySoNguyen1)
        {
            DaySoNguyen = new int[DaySoNguyen1.Length];
            DaySoTemp = new int[DaySoNguyen1.Length];




            for (int i = 0; i < DaySoNguyen1.Length; i++)
            {
                DaySoTemp[i] = DaySoNguyen1[i];
                DaySoNguyen[i] = DaySoNguyen1[i];
            }

            makeNumSquare(DaySoNguyen, HinhVuongSo);
            makePosSquare(DaySoNguyen, posSquare);
        }


        /// <summary>
        /// Hàm dùng để hiện các ô vuông số lên form
        /// </summary>
        /// <param name="DaySoNguyen"></param>
        /// <param name="HinhVuongSo"></param>
        private void makeNumSquare(int[] DaySoNguyen, O_vuong[] HinhVuongSo)
        {
            int X = -75;
            int Y = 60;
            for (int i = 0; i < DaySoNguyen.Length; i++)   //Thêm vào form các hình vuông cần dùng
            {
                HinhVuongSo[i].Location = new Point(X += 90, Y);
                HinhVuongSo[i].Text = DaySoNguyen[i].ToString();
                this.Controls.Add(HinhVuongSo[i]);
                HinhVuongSo[i].layGiaTriToaDoBanDau();
                HinhVuongSo[i].BackColor = Color.Gray;
                HinhVuongSo[i].thietLaplaiStatus();
            }

            for (int i = DaySoNguyen.Length; i < 10; i++)   //Loại khỏi form các hình vuông không cần dùng
            {
                this.Controls.Remove(HinhVuongSo[i]);
            }
        }


        /// <summary>
        /// Hàm dùng để hiện các label vị trí ô vuông lên form
        /// </summary>
        /// <param name="DaySoNguyen"></param>
        /// <param name="lbposSquare"></param>
        private void makePosSquare(int[] DaySoNguyen, Label[] lbposSquare)
        {
            int X = -65;
            int Y = 220;

            for (int i = 0; i < DaySoNguyen.Length; i++)  //Thêm vào form các hình vuông cần dùng
            {
                lbposSquare[i].Visible = true;
                lbposSquare[i].Location = new Point(X += 90, Y);
                lbposSquare[i].Text = i.ToString();
                this.Controls.Add(lbposSquare[i]);
                lbposSquare[i].AutoSize = true;
                lbposSquare[i].ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                lbposSquare[i].Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
                lbposSquare[i].Size = new System.Drawing.Size(28, 31);
                lbposSquare[i].TabIndex = 0;
            }

            for (int i = DaySoNguyen.Length; i < 10; i++)  //Loại khỏi form các hình vuông không cần dùng
            {
                this.Controls.Remove(lbposSquare[i]);
            }
        }


        int timerCounter = 0;       //biến đém số lần timerThreadSleep đã tick 500 1000 1500 2000 2500   50 100 150 200 250
        int waitAmount;     //thời gian đợi giữa các câu lệnh trong thuật toán
        /// <summary>
        /// Hàm đợi n giây trong các thuật toán auto
        /// </summary>
        /// <param name="seconds"></param>
        private void waitNSeconds(int seconds)      //tham số seconds tính như v, 5 = 0.5 giây, 10 = 1 giây, 15 = 1.5 giây
        {
            if (seconds < 5)
                return;
            timerThreadSleep.Start();           //bắt đầu chờ
            while (timerCounter * 50 != seconds * 100 && !buttonNgungClicked)   //vào vòng lặp chờ, trong vòng lặp này timerThreadSleep sẽ tick
            {
                Application.DoEvents();
            }
            timerCounter = 0;
        }

        private void timerThreadSleep_Tick(object sender, EventArgs e)
        {
            timerCounter++;                                //timerThreadSleep tick được 1 lần thì tăng counter lên 1
            if (timerCounter * 50 == waitAmount * 100)     //nếu counter*50 = với thời gian chờ thì ngưng timer
                timerThreadSleep.Stop();                    //vd: 1 giây = 1000 mili giây, interval của timerThreadSleep là 50 mili giây
        }                                                   //Vậy nên timer này phải chạy 20 lần (50 mili giây x 20 lần) = 1000 mili giây = 1 giây       


        #region Thuật toán sắp xếp

        //mới
        #region Thuật toán Interchange Sort
        private async void Interchange_sort()
        {
            continueAutoFlag = new CancellationTokenSource();  //nếu không phải thì đang sắp xếp bên auto
            int i, j = 0;
            for (i = m; i < DaySoNguyen.Length - 1; i++)
            {
                m = i;
                showi(i);
                labeli.Refresh();
                HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                HinhVuongSo[i].Refresh();        //update màu lại cho ô vuông 


                    //nếu ko phải đang chạy step by step thì là đang chạy auto
                    waitNSeconds(waitAmount);
                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
                    {
                        try
                        {
                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
                        }
                        catch
                        {

                        }
                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                        {
                            buttonLamLaiClicked = false;
                            return;
                        }
                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
                    }

                for (j = i + 1; j < DaySoNguyen.Length; j++)
                {
                    showj(j);
                    labelj.Refresh();
                    HinhVuongSo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                    HinhVuongSo[j].Refresh();           //update màu lại cho ô vuông     


                        //nếu ko phải đang chạy step by step thì là đang chạy auto
                        waitNSeconds(waitAmount);
                        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
                        {
                            try
                            {
                                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
                            }
                            catch
                            {

                            }
                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
                        }

                        //nếu ko phải đang chạy step by step thì là đang chạy auto
                        waitNSeconds(waitAmount);
                        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
                        {
                            try
                            {
                                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
                            }
                            catch
                            {

                            }
                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                            {
                                buttonLamLaiClicked = false;
                                return;
                            }
                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
                        }

                    if (ThuTuSapXep == 1 && DaySoNguyen[i] > DaySoNguyen[j])
                    {
                        while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
                        {
                            doiCho2OVuong(i, j);

                            if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
                            {
                                try
                                {
                                    await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
                                }
                                catch
                                {

                                }
                                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }
                                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
                            }
                        }
                        finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

                        int temp = DaySoNguyen[i];
                        DaySoNguyen[i] = DaySoNguyen[j];
                        DaySoNguyen[j] = temp;


                            //nếu ko phải đang chạy step by step thì là đang chạy auto
                            waitNSeconds(waitAmount);
                            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
                            {
                                try
                                {
                                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
                                }
                                catch
                                {

                                }
                                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                {
                                    buttonLamLaiClicked = false;
                                    return;
                                }
                                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
                            }
                    }
                    else
                    {
                        if (ThuTuSapXep == 0 && DaySoNguyen[i] < DaySoNguyen[j])
                        {
                            while (!finishedSwapping)   //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
                            {
                                doiCho2OVuong(i, j);

                                if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
                                {
                                    try
                                    {
                                        await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
                                    }
                                    catch
                                    {

                                    }
                                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                    {
                                        buttonLamLaiClicked = false;
                                        return;
                                    }
                                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
                                }
                            }
                            finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

                            int temp = DaySoNguyen[i];
                            DaySoNguyen[i] = DaySoNguyen[j];
                            DaySoNguyen[j] = temp;

                                //nếu ko phải đang chạy step by step thì là đang chạy auto
                                waitNSeconds(waitAmount);
                                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
                                {
                                    try
                                    {
                                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
                                    }
                                    catch
                                    {

                                    }
                                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
                                    {
                                        buttonLamLaiClicked = false;
                                        return;
                                    }
                                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
                                }
                        }
                    }
                    HinhVuongSo[j].BackColor = Color.Gray;
                    HinhVuongSo[j].Refresh();               //update màu lại cho ô vuông
                }
                HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSo[i].Refresh();                   //update màu lại cho ô vuông
            }
            if (i == DaySoNguyen.Length - 1 && j == DaySoNguyen.Length)
            {
                HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
                HinhVuongSo[i].Refresh();
                m = 0;
                n = 0;
            }
        }
        #endregion

        ////mới
        //#region Thuật toán Selection sort
        //bool changeColor = false;
        //private async void Selection_sort()
        //{
        //    if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
        //    {
        //        continueFlag = new CancellationTokenSource();
        //    }
        //    else
        //    {
        //        continueAutoFlag = new CancellationTokenSource();  //nếu không phải thì đang sắp xếp bên auto
        //    }

        //    int i, j = 0;
        //    for (i = m; i < DaySoNguyen.Length - 1; i++)
        //    {
        //        HighlightRichTextBox(lines, 3, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //        HinhVuongSo[i].Refresh();
        //        m = i;
        //        showi(i);
        //        labeli.Refresh();

        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }

        //        int iMinMax = i;
        //        resetRichtextboxColor(lines, 5);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 5, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();

        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }

        //        for (j = i + 1; j < DaySoNguyen.Length; j++)
        //        {
        //            resetRichtextboxColor(lines, 9);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 6, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            n = j;
        //            showj(j);
        //            labelj.Refresh();
        //            HinhVuongSo[j].BackColor = Color.FromArgb(255, 210, 45);
        //            HinhVuongSo[j].Refresh();

        //            if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //            {
        //                try
        //                {
        //                    await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //            }
        //            else
        //            {
        //                //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                waitNSeconds(waitAmount);
        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }

        //            resetRichtextboxColor(lines, 9);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 7, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();

        //            if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //            {
        //                try
        //                {
        //                    await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //            }
        //            else
        //            {
        //                //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                waitNSeconds(waitAmount);
        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }

        //            if (RadioButtonTangDan.Checked && DaySoNguyen[j] < DaySoNguyen[iMinMax])
        //            {
        //                resetRichtextboxColor(lines, 9);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 8, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                waitNSeconds(waitAmount);
        //                changeColor = true;
        //                iMinMax = j;
        //                n = j;
        //                int p = j - 1;
        //                while (p > i)
        //                {
        //                    HinhVuongSo[p].BackColor = Color.Gray;
        //                    HinhVuongSo[p].Refresh();
        //                    p--;
        //                }
        //                HinhVuongSo[j].BackColor = Color.FromArgb(178, 75, 83);
        //                HinhVuongSo[j].Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //            }
        //            else if (RadioButtonGiamDan.Checked && DaySoNguyen[j] > DaySoNguyen[iMinMax])
        //            {
        //                resetRichtextboxColor(lines, 9);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 8, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                changeColor = true;
        //                iMinMax = j;
        //                n = j;
        //                int p = j - 1;
        //                while (p > i)
        //                {
        //                    HinhVuongSo[p].BackColor = Color.Gray;
        //                    HinhVuongSo[p].Refresh();
        //                    p--;
        //                }
        //                HinhVuongSo[j].BackColor = Color.FromArgb(178, 75, 83);
        //                HinhVuongSo[j].Refresh();
        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //            }
        //            if (!changeColor)
        //            {
        //                HinhVuongSo[j].BackColor = Color.Gray;
        //                HinhVuongSo[j].Refresh();
        //                waitNSeconds(waitAmount);
        //                if (buttonNgungClicked)
        //                    break;
        //            }
        //            changeColor = false;
        //        }
        //        if (iMinMax != i)
        //        {
        //            resetRichtextboxColor(lines, 9);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 9, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();

        //            while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //            {
        //                doiCho2OVuong(i, iMinMax);

        //                if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }
        //            finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //            int temp = DaySoNguyen[i];
        //            DaySoNguyen[i] = DaySoNguyen[iMinMax];
        //            DaySoNguyen[iMinMax] = temp;

        //            HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //            HinhVuongSo[i].Refresh();
        //            HinhVuongSo[iMinMax].BackColor = System.Drawing.Color.Gray;
        //            HinhVuongSo[iMinMax].Refresh();
        //        }
        //        HinhVuongSo[m].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //        HinhVuongSo[m].Refresh();
        //        resetRichtextboxColor(lines, 11);
        //        richTextBoxCodeThuatToan.Refresh();
        //    }
        //    if (i == DaySoNguyen.Length - 1 && j == DaySoNguyen.Length)
        //    {
        //        HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //        HinhVuongSo[i].Refresh();
        //        m = 0;
        //        n = 0;
        //        enableControlsAfterFinish();
        //        KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
        //        ktb.ShowDialog();
        //    }
        //}
        //#endregion

        ////mới
        //#region Thuật toán Bubble sort
        //private async void Bubble_sort()
        //{
        //    if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
        //    {
        //        continueFlag = new CancellationTokenSource();
        //    }
        //    else
        //    {
        //        continueAutoFlag = new CancellationTokenSource();  //nếu không phải thì đang sắp xếp bên auto
        //    }

        //    int i, j;
        //    bool swapped;
        //    for (i = m; i < DaySoNguyen.Length - 1; i++)
        //    {
        //        HighlightRichTextBox(lines, 2, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();

        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }

        //        m = i;
        //        swapped = false;
        //        for (j = n; j < DaySoNguyen.Length - i - 1; j++)
        //        {
        //            resetRichtextboxColor(lines, 11);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 4, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            n = j;
        //            HinhVuongSo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
        //            HinhVuongSo[j].Refresh();
        //            showi(j);                   //làm xuất hiện label biến i tại vị trí j
        //            labeli.Refresh();
        //            HinhVuongSo[j + 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //            HinhVuongSo[j + 1].Refresh();
        //            showj(j + 1);               //làm xuất hiện label biến j tại vị trí j+1
        //            labelj.Refresh();

        //            if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //            {
        //                try
        //                {
        //                    await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //            }
        //            else
        //            {
        //                //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                waitNSeconds(waitAmount);
        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }

        //            resetRichtextboxColor(lines, 7);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 6, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();

        //            if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //            {
        //                try
        //                {
        //                    await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //            }
        //            else
        //            {
        //                //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                waitNSeconds(waitAmount);
        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }

        //            if (RadioButtonTangDan.Checked && DaySoNguyen[j] > DaySoNguyen[j + 1])
        //            {
        //                resetRichtextboxColor(lines, 10);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 8, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();

        //                while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                {
        //                    doiCho2OVuong(j, j + 1);

        //                    if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //                finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                int temp = DaySoNguyen[j];
        //                DaySoNguyen[j] = DaySoNguyen[j + 1];
        //                DaySoNguyen[j + 1] = temp;
        //                swapped = true;
        //            }
        //            else
        //            {
        //                if (RadioButtonGiamDan.Checked && DaySoNguyen[j] < DaySoNguyen[j + 1])
        //                {
        //                    resetRichtextboxColor(lines, 10);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 8, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();

        //                    while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                    {
        //                        doiCho2OVuong(j, j + 1);

        //                        if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }
        //                    finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                    int temp = DaySoNguyen[j];
        //                    DaySoNguyen[j] = DaySoNguyen[j + 1];
        //                    DaySoNguyen[j + 1] = temp;
        //                    swapped = true;
        //                }
        //            }
        //            HinhVuongSo[j + 1].BackColor = System.Drawing.Color.Gray;
        //            HinhVuongSo[j].BackColor = System.Drawing.Color.Gray;
        //            HinhVuongSo[j].Refresh();
        //            HinhVuongSo[j + 1].Refresh();
        //        }
        //        n = 0;
        //        HinhVuongSo[j].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);  //phần tử cuối là phần tử đã được sắp xếp đúng nên thành màu xanh
        //        HinhVuongSo[j].Refresh();
        //        if (swapped == false)
        //        {
        //            for (int x = 0; x < j; x++)
        //            {
        //                HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //            }
        //            m = 0;
        //            n = 0;
        //            enableControlsAfterFinish();
        //            KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
        //            ktb.ShowDialog();
        //            break;
        //        }
        //        resetRichtextboxColor(lines, 13);
        //        richTextBoxCodeThuatToan.Refresh();
        //    }
        //}
        //#endregion

        ////mới
        //#region Thuật toán Insertion sort
        //int key;                                       //dùng để lưu phần tử thứ i trong thuật toán để so sánh với các ptử phía trước
        //private async void Insertion_sort()
        //{
        //    if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
        //    {
        //        continueFlag = new CancellationTokenSource();
        //    }
        //    else
        //    {
        //        continueAutoFlag = new CancellationTokenSource();  //nếu không phải thì đang sắp xếp bên auto
        //    }

        //    int i, j;
        //    for (i = m; i < DaySoNguyen.Length; i++)
        //    {
        //        HighlightRichTextBox(lines, 2, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();

        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }

        //        m = i;
        //        showi(i);
        //        labeli.Refresh();

        //        resetRichtextboxColor(lines, 4);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 4, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();
        //        key = DaySoNguyen[i];     //nếu nút ngưng chưa đc ấn thì key vẫn bằng đúng như i

        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }

        //        for (int x = 0; x <= i; x++)
        //        {
        //            HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //            HinhVuongSo[x].Refresh();
        //        }

        //        resetRichtextboxColor(lines, 5);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 5, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();
        //        j = i - 1;

        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }

        //        resetRichtextboxColor(lines, 11);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 6, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();

        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }

        //        if (RadioButtonTangDan.Checked)
        //        {
        //            while (j >= 0 && DaySoNguyen[j] > key)
        //            {
        //                n = j;
        //                showj(j);
        //                labelj.Refresh();
        //                HinhVuongSo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
        //                HinhVuongSo[j].Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                HinhVuongSo[j + 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //                HinhVuongSo[j + 1].Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                resetRichtextboxColor(lines, 11);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 8, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                {
        //                    doiCho2OVuong(j, j + 1);

        //                    if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //                finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                HinhVuongSo[j + 1].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[j].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[j].Refresh();
        //                HinhVuongSo[j + 1].Refresh();
        //                DaySoNguyen[j + 1] = DaySoNguyen[j];
        //                resetRichtextboxColor(lines, 11);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 9, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                j = j - 1;
        //            }
        //        }
        //        else if (RadioButtonGiamDan.Checked)
        //        {
        //            while (j >= 0 && DaySoNguyen[j] < key)
        //            {
        //                resetRichtextboxColor(lines, 11);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 6, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();

        //                n = j;
        //                showj(j);
        //                labelj.Refresh();
        //                HinhVuongSo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
        //                HinhVuongSo[j].Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                HinhVuongSo[j + 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //                HinhVuongSo[j + 1].Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                resetRichtextboxColor(lines, 11);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 8, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                {
        //                    doiCho2OVuong(j, j + 1);

        //                    if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //                finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                HinhVuongSo[j + 1].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[j].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[j].Refresh();
        //                HinhVuongSo[j + 1].Refresh();
        //                DaySoNguyen[j + 1] = DaySoNguyen[j];

        //                resetRichtextboxColor(lines, 11);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 9, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                j = j - 1;
        //            }
        //        }

        //        resetRichtextboxColor(lines, 12);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 11, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();

        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }

        //        DaySoNguyen[j + 1] = key;

        //        if (i == DaySoNguyen.Length - 1)
        //        {
        //            for (int x = 0; x <= i; x++)
        //                HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //            m = 0;
        //            n = 0;
        //            enableControlsAfterFinish();
        //            KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
        //            ktb.ShowDialog();
        //            break;
        //        }
        //        resetRichtextboxColor(lines, 14);
        //        richTextBoxCodeThuatToan.Refresh();
        //    }
        //}
        //#endregion

        ////chưa sửa
        //#region Thuật toán BinaryInsertion Sort
        //int Binary_Search(int[] a, int item, int low, int high)
        //{
        //    if (high <= low)
        //        return (item > a[low]) ? (low + 1) : low;

        //    int mid = (low + high) / 2;

        //    if (item == a[mid])
        //        return mid + 1;

        //    if (item > a[mid])
        //        return Binary_Search(a, item, mid + 1, high);
        //    return Binary_Search(a, item, low, mid - 1);
        //}

        //static int iBinaryInsertionSort = 1;
        //void BinaryInsertion_sort(int[] a, O_vuong[] HinhVuongSo, ref int m, ref int n)
        //{
        //    int i, loc, j, k, selected, i_n;

        //    for (i = iBinaryInsertionSort; i < a.Length; i++)
        //    {
        //        if (buttonNgungClicked)
        //            break;
        //        iBinaryInsertionSort = i;
        //        for (int x = 0; x <= i; x++)
        //        {
        //            HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //            HinhVuongSo[x].Refresh();
        //        }
        //        Thread.Sleep(1000);
        //        i_n = i;
        //        j = i - 1;
        //        selected = a[i];
        //        loc = Binary_Search(a, selected, 0, j);
        //        while (j >= loc)
        //        {
        //            n = i_n;
        //            m = j;
        //            HinhVuongSo[n].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
        //            HinhVuongSo[n].Refresh();
        //            Thread.Sleep(1000);
        //            HinhVuongSo[m].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //            HinhVuongSo[m].Refresh();
        //            Thread.Sleep(1000);
        //            doiCho2OVuong(i, j);
        //            if (buttonNgungClicked)
        //                break;
        //            a[j + 1] = a[j];
        //            j--;
        //            i_n--;
        //            HinhVuongSo[n].BackColor = System.Drawing.Color.Gray;
        //            HinhVuongSo[m].BackColor = System.Drawing.Color.Gray;
        //            HinhVuongSo[m].Refresh();
        //            HinhVuongSo[n].Refresh();
        //        }
        //        a[j + 1] = selected;
        //        if (buttonNgungClicked)
        //            break;
        //        if (i == a.Length - 1)
        //        {
        //            for (int x = 0; x <= i; x++)
        //                HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //            iBinaryInsertionSort = 0;
        //            buttonNgung.Enabled = false;
        //            RadioButtonTangDan.Enabled = true;
        //            RadioButtonGiamDan.Enabled = true;
        //            ButtonLamLai.Enabled = true;
        //            buttonBatDau.Enabled = false;
        //            KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
        //            ktb.ShowDialog();
        //            break;
        //        }
        //    }
        //}
        //#endregion

        ////chưa sửa
        //#region Thuật toán Heap Sort
        //int old_r = 0;
        //bool checkHeap = false;
        //bool shifting = false;
        //CancellationTokenSource waitforshifting;
        //Task waitforshiftingfinish()
        //{
        //    return Task.Delay(-1, waitforshifting.Token);
        //}
        //CancellationTokenSource waitforcreateheap;
        //Task waitforcreateheapfinish()
        //{
        //    return Task.Delay(-1, waitforcreateheap.Token);
        //}
        //async void HeapSort()
        //{
        //    int r;
        //    HighlightRichTextBox(lines, 3, colorhighlight);
        //    richTextBoxCodeThuatToan.Refresh();
        //    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //    {
        //        try
        //        {
        //            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //        }
        //        catch
        //        {

        //        }
        //        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //        {
        //            buttonLamLaiClicked = false;
        //            return;
        //        }
        //        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //    }
        //    else
        //    {
        //        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //        waitNSeconds(waitAmount);
        //        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //        {
        //            try
        //            {
        //                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //        }
        //    }
        //    /*if (old_r == 0) */
        //    waitforcreateheap = new CancellationTokenSource();

        //    CreateHeap(DaySoNguyen, HinhVuongSo, DaySoNguyen.Length);

        //    try
        //    {
        //        await waitforcreateheapfinish();
        //    }
        //    catch
        //    {

        //    }
        //    //if (buttonNgungClicked) return;
        //    r = DaySoNguyen.Length - 1;
        //    HighlightRichTextBox(lines, 4, richTextBoxCodeThuatToan.BackColor);
        //    HighlightRichTextBox(lines, 5, colorhighlight);
        //    //if (checkHeap) r = old_r;
        //    //old_r = r;
        //    //if (shifting)
        //    //  shift(DaySoNguyen, HinhVuongSo, 0, r);
        //    //if (buttonNgungClicked) return;
        //    while (r > 0)
        //    {
        //        HighlightRichTextBox(lines, 5, richTextBoxCodeThuatToan.BackColor);
        //        HighlightRichTextBox(lines, 6, colorhighlight);
        //        //if (buttonNgungClicked) break;
        //        HinhVuongSo[0].BackColor = Color.FromArgb(66, 104, 166);
        //        HinhVuongSo[0].Refresh();
        //        HinhVuongSo[r].BackColor = Color.FromArgb(178, 75, 83);
        //        HinhVuongSo[r].Refresh();
        //        waitNSeconds(waitAmount);
        //        //if (buttonNgungClicked) break;
        //        while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //        {
        //            doiCho2OVuong(0, r);

        //            if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }
        //        finishedSwapping = false;
        //        //if (buttonNgungClicked) break;
        //        int temp = DaySoNguyen[0];
        //        DaySoNguyen[0] = DaySoNguyen[r];
        //        DaySoNguyen[r] = temp;
        //        HinhVuongSo[0].BackColor = Color.Gray;
        //        HinhVuongSo[0].Refresh();
        //        HinhVuongSo[r].BackColor = Color.FromArgb(58, 130, 90);
        //        HinhVuongSo[r].Refresh();
        //        r--;
        //        old_r = r;
        //        checkHeap = false;
        //        if (r > 0)
        //        {
        //            waitforshifting = new CancellationTokenSource();
        //            shift(DaySoNguyen, HinhVuongSo, 0, r);
        //            try
        //            {
        //                await waitforshiftingfinish();
        //            }
        //            catch
        //            {

        //            }
        //        }

        //        //if (buttonNgungClicked) break;
        //    }
        //    if (r == 0)
        //    {
        //        HinhVuongSo[r].BackColor = Color.FromArgb(58, 130, 90);
        //        HinhVuongSo[r].Refresh();
        //        m = 0;
        //        n = 0;
        //        enableControlsAfterFinish();
        //        KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
        //        ktb.ShowDialog();
        //    }
        //}
        //int old_l;
        //async void CreateHeap(int[] DaySoNguyen, O_vuong[] HinhVuongSo, int n)
        //{
        //    int l;
        //    l = n / 2 - 1;
        //    //if (checkHeap) l = old_l;
        //    //old_l = l;
        //    while (l >= 0)
        //    {
        //        waitforshifting = new CancellationTokenSource();
        //        shift(DaySoNguyen, HinhVuongSo, l, n - 1);
        //        try
        //        {
        //            await waitforshiftingfinish();
        //        }
        //        catch
        //        {

        //        }
        //        //if (buttonNgungClicked) break;
        //        if (!shifting) l--;
        //        if (continueAutoFlag != null) continueAutoFlag.Cancel();
        //    }
        //    waitforcreateheap.Cancel();
        //}
        //bool flag1 = false, flag2 = false;
        //async void shift(int[] DaySoNguyen, O_vuong[] HinhVuongSo, int l, int r)
        //{
        //    shifting = true;
        //    if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
        //    {
        //        continueFlag = new CancellationTokenSource();
        //    }
        //    else
        //    {
        //        continueAutoFlag = new CancellationTokenSource();  //nếu không phải thì đang sắp xếp bên auto
        //    }
        //    int x, i, j;
        //    i = l;
        //    if (checkHeap) i = m;
        //    m = i;
        //    j = 2 * i + 1;
        //    if (checkHeap) j = n;
        //    n = j;
        //    showi(i);
        //    labeli.Refresh();
        //    x = DaySoNguyen[i];
        //    if (!checkHeap)
        //    {
        //        if (j <= r)
        //        {
        //            showj(j);
        //            labelj.Refresh();
        //        }
        //        HinhVuongSo[i].BackColor = Color.FromArgb(255, 210, 45);
        //        HinhVuongSo[i].Refresh();
        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }
        //        HinhVuongSo[j].BackColor = Color.FromArgb(255, 210, 45);
        //        HinhVuongSo[j].Refresh();
        //    }
        //    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //    {
        //        try
        //        {
        //            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //        }
        //        catch
        //        {

        //        }
        //        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //        {
        //            buttonLamLaiClicked = false;
        //            return;
        //        }
        //        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //    }
        //    else
        //    {
        //        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //        waitNSeconds(waitAmount);
        //        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //        {
        //            try
        //            {
        //                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //        }
        //    }
        //    if (j + 1 <= r && r >= 2)
        //    {
        //        if (!checkHeap)
        //        {
        //            HinhVuongSo[j + 1].BackColor = Color.FromArgb(255, 210, 45);
        //            HinhVuongSo[j + 1].Refresh();
        //        }
        //    }
        //    waitNSeconds(waitAmount);
        //    while (j <= r)
        //    {
        //        //if (buttonNgungClicked) break;
        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }
        //        if (j < r)
        //        {
        //            if (RadioButtonTangDan.Checked)
        //            {
        //                if (DaySoNguyen[j] < DaySoNguyen[j + 1])
        //                {
        //                    HinhVuongSo[j].BackColor = Color.Gray;
        //                    HinhVuongSo[j].Refresh();
        //                    showj(++j);
        //                    labelj.Refresh();
        //                    waitNSeconds(waitAmount);
        //                }
        //                else //if (DaySoNguyen[j] > DaySoNguyen[j + 1])
        //                {
        //                    HinhVuongSo[j + 1].BackColor = Color.Gray;
        //                    HinhVuongSo[j + 1].Refresh();
        //                    waitNSeconds(waitAmount);
        //                }
        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (DaySoNguyen[j] > DaySoNguyen[j + 1])
        //                {
        //                    HinhVuongSo[j].BackColor = Color.Gray;
        //                    HinhVuongSo[j].Refresh();
        //                    showj(++j);
        //                    labelj.Refresh();
        //                    waitNSeconds(waitAmount);
        //                }
        //                else if (DaySoNguyen[j] < DaySoNguyen[j + 1])
        //                {
        //                    HinhVuongSo[j + 1].BackColor = Color.Gray;
        //                    HinhVuongSo[j + 1].Refresh();
        //                    waitNSeconds(waitAmount);
        //                }
        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //            }

        //        }
        //        //if (buttonNgungClicked) break;
        //        if (RadioButtonTangDan.Checked)
        //        {
        //            if (DaySoNguyen[j] <= x)
        //            {
        //                HinhVuongSo[i].BackColor = Color.Gray;
        //                HinhVuongSo[i].Refresh();
        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //                HinhVuongSo[2 * i + 1].BackColor = Color.Gray;
        //                HinhVuongSo[2 * i + 1].Refresh();
        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //                if (2 * i + 2 <= r)
        //                {
        //                    HinhVuongSo[2 * i + 2].BackColor = Color.Gray;
        //                    HinhVuongSo[2 * i + 2].Refresh();
        //                }
        //                waitNSeconds(waitAmount);
        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //                checkHeap = false;
        //                shifting = false;
        //                waitforshifting.Cancel();
        //                return;
        //            }
        //        }
        //        else if (RadioButtonGiamDan.Checked)
        //        {
        //            if (DaySoNguyen[j] >= x)
        //            {
        //                HinhVuongSo[i].BackColor = Color.Gray;
        //                HinhVuongSo[i].Refresh();
        //                HinhVuongSo[2 * i + 1].BackColor = Color.Gray;
        //                HinhVuongSo[2 * i + 1].Refresh();
        //                if (2 * i + 2 <= r)
        //                {
        //                    HinhVuongSo[2 * i + 2].BackColor = Color.Gray;
        //                    HinhVuongSo[2 * i + 2].Refresh();
        //                }
        //                waitNSeconds(waitAmount);
        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //                checkHeap = false;
        //                shifting = false;
        //                waitforshifting.Cancel();
        //                return;
        //            }
        //        }
        //        if (!flag1 && !flag2)
        //        {

        //            while (!finishedSwapping)
        //            {
        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //                doiCho2OVuong(i, j);

        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }
        //            finishedSwapping = false;
        //            //if (buttonNgungClicked) break;
        //            if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //            {
        //                try
        //                {
        //                    await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //            }
        //            else
        //            {
        //                //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                //waitNSeconds(waitAmount);
        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }
        //            HinhVuongSo[i].BackColor = Color.FromArgb(255, 210, 45);
        //            HinhVuongSo[i].Refresh();
        //            HinhVuongSo[j].BackColor = Color.FromArgb(255, 210, 45);
        //            HinhVuongSo[j].Refresh();
        //            waitNSeconds(waitAmount);
        //            HinhVuongSo[i].BackColor = Color.Gray;
        //            HinhVuongSo[i].Refresh();
        //            HinhVuongSo[j].BackColor = Color.Gray;
        //            HinhVuongSo[j].Refresh();
        //            waitNSeconds(waitAmount);
        //            DaySoNguyen[i] = DaySoNguyen[j];
        //            DaySoNguyen[j] = x;
        //            i = j;
        //            m = i;
        //            j = 2 * i + 1;
        //            n = j;
        //            showi(i);
        //            labeli.Refresh();
        //            if (j <= r)
        //            {
        //                showj(j);
        //                labelj.Refresh();
        //                HinhVuongSo[i].BackColor = Color.FromArgb(255, 210, 45);
        //                HinhVuongSo[i].Refresh();
        //                HinhVuongSo[j].BackColor = Color.FromArgb(255, 210, 45);
        //                HinhVuongSo[j].Refresh();
        //                if (j + 1 <= r && r >= 2)
        //                {
        //                    HinhVuongSo[j + 1].BackColor = Color.FromArgb(255, 210, 45);
        //                    HinhVuongSo[j + 1].Refresh();
        //                }
        //            }
        //            waitNSeconds(waitAmount);
        //            x = DaySoNguyen[i];
        //        }
        //    }
        //    //if (buttonNgungClicked) return;
        //    checkHeap = false;
        //    shifting = false;
        //    waitforshifting.Cancel();
        //}
        //#endregion

        //#region Thuật toán shaker sort
        //int k = 0; // sau mỗi lần shake, set lại biên trái/phải thông qua k
        //private async void Shaker_sort()
        //{
        //    int LeftShaker;   //biên trái
        //    int RightShaker;  //biên phải

        //    if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
        //    {
        //        continueFlag = new CancellationTokenSource();
        //    }
        //    else
        //    {
        //        continueAutoFlag = new CancellationTokenSource();  //nếu không phải thì đang sắp xếp bên auto
        //    }

        //    HighlightRichTextBox(lines, 2, colorhighlight);
        //    richTextBoxCodeThuatToan.Refresh();
        //    waitNSeconds(waitAmount);
        //    resetRichtextboxColor(lines, 4);
        //    richTextBoxCodeThuatToan.Refresh();
        //    HighlightRichTextBox(lines, 3, colorhighlight);
        //    richTextBoxCodeThuatToan.Refresh();
        //    waitNSeconds(waitAmount);
        //    resetRichtextboxColor(lines, 4);
        //    richTextBoxCodeThuatToan.Refresh();

        //    int i; // biến chạy
        //    LeftShaker = 0; // biên trái
        //    RightShaker = DaySoNguyen.Length - 1; // biên phải            
        //    while (LeftShaker < RightShaker) // khi left còn nhỏ hơn right thì luôn shake
        //    {
        //        resetRichtextboxColor(lines, 26);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 4, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();
        //        waitNSeconds(waitAmount);

        //        if (RadioButtonTangDan.Checked)
        //        {
        //            for (i = LeftShaker; i < RightShaker; i++) // shake lớn về cuối, m dùng như biến Left
        //            {
        //                resetRichtextboxColor(lines, 14);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 6, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                waitNSeconds(waitAmount);

        //                HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
        //                HinhVuongSo[i].Refresh();
        //                showi(i);
        //                labeli.Refresh();
        //                HinhVuongSo[i + 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //                HinhVuongSo[i + 1].Refresh();
        //                showj(i + 1);
        //                labelj.Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                if (DaySoNguyen[i] > DaySoNguyen[i + 1])
        //                {
        //                    resetRichtextboxColor(lines, 14);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 8, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    resetRichtextboxColor(lines, 14);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 10, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                    {
        //                        doiCho2OVuong(i, i + 1);

        //                        if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }
        //                    finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                    int temp = DaySoNguyen[i];
        //                    DaySoNguyen[i] = DaySoNguyen[i + 1];
        //                    DaySoNguyen[i + 1] = temp;

        //                    resetRichtextboxColor(lines, 14);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 11, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    k = i; // khi có đổi chỗ xảy ra, k = i, i luôn tăng cho đến right, lần đổi chỗ cuối cùng trong 1 lần shake là i và i + 1, nên sau khi shake xong i + 1 chứa phần tử lớn nhất, nên biên right sẽ set về k
        //                }
        //                HinhVuongSo[i + 1].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[i].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[i].Refresh();
        //                HinhVuongSo[i + 1].Refresh();
        //            }

        //            resetRichtextboxColor(lines, 15);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 14, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            waitNSeconds(waitAmount);

        //            RightShaker = k;

        //            for (int x = DaySoNguyen.Length - 1; x > RightShaker; x--) // từ giới hạn ngoài cùng trở vào biên phải đã sắp xong
        //            {
        //                HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //                HinhVuongSo[x].Refresh();
        //            }


        //            for (i = RightShaker; i > LeftShaker; i--)
        //            {
        //                resetRichtextboxColor(lines, 23);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 15, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                waitNSeconds(waitAmount);

        //                HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
        //                HinhVuongSo[i].Refresh();
        //                showi(i);
        //                labeli.Refresh();
        //                showj(i - 1);
        //                labelj.Refresh();
        //                HinhVuongSo[i - 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //                HinhVuongSo[i - 1].Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                if (DaySoNguyen[i] < DaySoNguyen[i - 1])
        //                {
        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 17, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 19, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                    {
        //                        doiCho2OVuong(i - 1, i);

        //                        if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }
        //                    finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                    int temp = DaySoNguyen[i];
        //                    DaySoNguyen[i] = DaySoNguyen[i - 1];
        //                    DaySoNguyen[i - 1] = temp;

        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 20, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    k = i;
        //                }
        //                HinhVuongSo[i - 1].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[i].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[i].Refresh();
        //                HinhVuongSo[i - 1].Refresh();
        //            }

        //            resetRichtextboxColor(lines, 24);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 23, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            waitNSeconds(waitAmount);

        //            LeftShaker = k;

        //            for (int x = 0; x < LeftShaker; x++)
        //            {
        //                HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //                HinhVuongSo[x].Refresh();
        //            }
        //        }
        //        else
        //        {
        //            for (i = LeftShaker; i < RightShaker; i++) // shake lớn về cuối, m dùng như biến Left
        //            {
        //                resetRichtextboxColor(lines, 14);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 6, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                waitNSeconds(waitAmount);


        //                HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
        //                HinhVuongSo[i].Refresh();
        //                showi(i);
        //                labeli.Refresh();
        //                HinhVuongSo[i + 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //                HinhVuongSo[i + 1].Refresh();
        //                showj(i + 1);
        //                labelj.Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                if (DaySoNguyen[i] < DaySoNguyen[i + 1])
        //                {
        //                    resetRichtextboxColor(lines, 14);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 8, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    resetRichtextboxColor(lines, 14);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 10, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                    {
        //                        doiCho2OVuong(i, i + 1);

        //                        if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }
        //                    finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                    int temp = DaySoNguyen[i];
        //                    DaySoNguyen[i] = DaySoNguyen[i + 1];
        //                    DaySoNguyen[i + 1] = temp;

        //                    resetRichtextboxColor(lines, 14);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 11, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    k = i; // khi có đổi chỗ xảy ra, k = i, i luôn tăng cho đến right, lần đổi chỗ cuối cùng trong 1 lần shake là i và i + 1, nên sau khi shake xong i + 1 chứa phần tử lớn nhất, nên biên right sẽ set về k
        //                }
        //                HinhVuongSo[i + 1].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[i].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[i].Refresh();
        //                HinhVuongSo[i + 1].Refresh();
        //            }

        //            resetRichtextboxColor(lines, 15);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 14, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            waitNSeconds(waitAmount);

        //            RightShaker = k;

        //            for (int x = DaySoNguyen.Length - 1; x > RightShaker; x--) // từ giới hạn ngoài cùng trở vào biên phải đã sắp xong
        //            {
        //                HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //                HinhVuongSo[x].Refresh();
        //            }

        //            for (i = RightShaker; i > LeftShaker; i--)
        //            {
        //                resetRichtextboxColor(lines, 23);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 15, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                waitNSeconds(waitAmount);

        //                n = i;
        //                HinhVuongSo[i].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
        //                HinhVuongSo[i].Refresh();
        //                showi(i);
        //                labeli.Refresh();
        //                showj(i - 1);
        //                labelj.Refresh();
        //                HinhVuongSo[i - 1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //                HinhVuongSo[i - 1].Refresh();

        //                if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                {
        //                    try
        //                    {
        //                        await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                }
        //                else
        //                {
        //                    //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                    waitNSeconds(waitAmount);
        //                    if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }

        //                if (DaySoNguyen[i] > DaySoNguyen[i - 1])
        //                {
        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 17, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 19, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                    {
        //                        doiCho2OVuong(i - 1, i);

        //                        if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }
        //                    finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                    int temp = DaySoNguyen[i];
        //                    DaySoNguyen[i] = DaySoNguyen[i - 1];
        //                    DaySoNguyen[i - 1] = temp;

        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 20, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    k = i;
        //                }
        //                HinhVuongSo[i - 1].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[i].BackColor = System.Drawing.Color.Gray;
        //                HinhVuongSo[i].Refresh();
        //                HinhVuongSo[i - 1].Refresh();
        //            }

        //            resetRichtextboxColor(lines, 24);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 23, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            waitNSeconds(waitAmount);

        //            LeftShaker = k;

        //            for (int x = 0; x < LeftShaker; x++)
        //            {
        //                HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //                HinhVuongSo[x].Refresh();
        //            }
        //        }
        //        if (RightShaker == LeftShaker)
        //        {
        //            for (int x = 0; x < DaySoNguyen.Length; x++)
        //            {
        //                HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //            }
        //            resetRichtextboxColor(lines, 24);
        //            richTextBoxCodeThuatToan.Refresh();
        //            m = 0;
        //            n = 0;
        //            enableControlsAfterFinish();
        //            KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
        //            ktb.ShowDialog();
        //            break;
        //        }
        //    }
        //}
        //#endregion

        //#region Thuật toán Quick Sort
        //Label labelx = new Label();
        //Label labelibangj = new Label();
        //Boolean iCoVuaBangjKhong = false;
        //private void showx(int pos)
        //{
        //    labelx.Visible = true;
        //    labelx.Top = posSquare[pos].Top - 50;
        //    labelx.Left = posSquare[pos].Left + 5;
        //}
        //#region Hiển thị i và j dành riêng cho quick_sort
        //private void showibangj(int pos)
        //{
        //    iCoVuaBangjKhong = true;
        //    labeli.Visible = false;
        //    labelj.Visible = false;
        //    labelibangj.Visible = true;
        //    labelibangj.Top = posSquare[pos].Top - 25;
        //    labelibangj.Left = posSquare[pos].Left - 5;
        //}
        //private void showi_q(int pos)
        //{
        //    if (iCoVuaBangjKhong)
        //    {
        //        iCoVuaBangjKhong = false;
        //        showj_q(pos - 1);
        //    }
        //    labelibangj.Visible = false;
        //    if (pos >= 0 && pos < DaySoNguyen.Length)
        //    {
        //        labeli.Visible = true;
        //        labeli.Top = posSquare[pos].Top - 25;
        //        labeli.Left = posSquare[pos].Left + 5;
        //    }
        //}
        //private void showj_q(int pos)
        //{
        //    if (iCoVuaBangjKhong)
        //    {
        //        iCoVuaBangjKhong = false;
        //        showi_q(pos + 1);
        //    }
        //    labelibangj.Visible = false;
        //    if (pos >= 0 && pos < DaySoNguyen.Length)
        //    {
        //        labelj.Visible = true;
        //        labelj.Top = posSquare[pos].Top - 25;
        //        labelj.Left = posSquare[pos].Left + 5;
        //    }
        //}

        //#endregion

        //# region khởi tạo 10 lần đợi
        //CancellationTokenSource waitForTheNextActionSort0 = new CancellationTokenSource();
        //Task waitForTheNextActionSortFinish0()
        //{
        //    return Task.Delay(-1, waitForTheNextActionSort0.Token);
        //}
        //CancellationTokenSource waitForTheNextActionSort1 = new CancellationTokenSource();
        //Task waitForTheNextActionSortFinish1()
        //{
        //    return Task.Delay(-1, waitForTheNextActionSort1.Token);
        //}
        //CancellationTokenSource waitForTheNextActionSort2 = new CancellationTokenSource();
        //Task waitForTheNextActionSortFinish2()
        //{
        //    return Task.Delay(-1, waitForTheNextActionSort2.Token);
        //}
        //CancellationTokenSource waitForTheNextActionSort3 = new CancellationTokenSource();
        //Task waitForTheNextActionSortFinish3()
        //{
        //    return Task.Delay(-1, waitForTheNextActionSort3.Token);
        //}
        //CancellationTokenSource waitForTheNextActionSort4 = new CancellationTokenSource();
        //Task waitForTheNextActionSortFinish4()
        //{
        //    return Task.Delay(-1, waitForTheNextActionSort4.Token);
        //}
        //CancellationTokenSource waitForTheNextActionSort5 = new CancellationTokenSource();
        //Task waitForTheNextActionSortFinish5()
        //{
        //    return Task.Delay(-1, waitForTheNextActionSort5.Token);
        //}
        //CancellationTokenSource waitForTheNextActionSort6 = new CancellationTokenSource();
        //Task waitForTheNextActionSortFinish6()
        //{
        //    return Task.Delay(-1, waitForTheNextActionSort6.Token);
        //}
        //CancellationTokenSource waitForTheNextActionSort7 = new CancellationTokenSource();
        //Task waitForTheNextActionSortFinish7()
        //{
        //    return Task.Delay(-1, waitForTheNextActionSort7.Token);
        //}
        //CancellationTokenSource waitForTheNextActionSort8 = new CancellationTokenSource();
        //Task waitForTheNextActionSortFinish8()
        //{
        //    return Task.Delay(-1, waitForTheNextActionSort8.Token);
        //}
        //CancellationTokenSource waitForTheNextActionSort9 = new CancellationTokenSource();
        //Task waitForTheNextActionSortFinish9()
        //{
        //    return Task.Delay(-1, waitForTheNextActionSort9.Token);
        //}
        //#endregion
        //public delegate Task awaitDelegate();
        //#region Trả về đối tượng lần đợi tương ứng và hàm đợi tương ứng
        //// trả về đối tượng lần đợi tương ứng
        //private CancellationTokenSource returnAwaitCorrespondingObject(int solandequi)
        //{
        //    if (solandequi == 0)
        //    {
        //        return waitForTheNextActionSort0;
        //    }
        //    if (solandequi == 1)
        //    {
        //        return waitForTheNextActionSort1;
        //    }
        //    if (solandequi == 2)
        //    {
        //        return waitForTheNextActionSort2;
        //    }
        //    if (solandequi == 3)
        //    {
        //        return waitForTheNextActionSort3;
        //    }
        //    if (solandequi == 4)
        //    {
        //        return waitForTheNextActionSort4;
        //    }
        //    if (solandequi == 5)
        //    {
        //        return waitForTheNextActionSort5;
        //    }
        //    if (solandequi == 0)
        //    {
        //        return waitForTheNextActionSort6;
        //    }
        //    if (solandequi == 7)
        //    {
        //        return waitForTheNextActionSort7;
        //    }
        //    if (solandequi == 8)
        //    {
        //        return waitForTheNextActionSort8;
        //    }
        //    return waitForTheNextActionSort9;
        //}
        //// trả về hàm đợi tương ứng
        //private Task returnAwaitCorresponding(int solandequi)
        //{

        //    if (solandequi == 0)
        //    {
        //        awaitDelegate de0 = new awaitDelegate(waitForTheNextActionSortFinish0);
        //        return de0();
        //    }
        //    if (solandequi == 1)
        //    {
        //        awaitDelegate de1 = new awaitDelegate(waitForTheNextActionSortFinish1);
        //        return de1();
        //    }
        //    if (solandequi == 2)
        //    {
        //        awaitDelegate de2 = new awaitDelegate(waitForTheNextActionSortFinish2);
        //        return de2();
        //    }
        //    if (solandequi == 3)
        //    {
        //        awaitDelegate de3 = new awaitDelegate(waitForTheNextActionSortFinish3);
        //        return de3();
        //    }
        //    if (solandequi == 4)
        //    {
        //        awaitDelegate de4 = new awaitDelegate(waitForTheNextActionSortFinish4);
        //        return de4();
        //    }
        //    if (solandequi == 5)
        //    {
        //        awaitDelegate de5 = new awaitDelegate(waitForTheNextActionSortFinish5);
        //        return de5();
        //    }
        //    if (solandequi == 0)
        //    {
        //        awaitDelegate de6 = new awaitDelegate(waitForTheNextActionSortFinish6);
        //        return de6();
        //    }
        //    if (solandequi == 7)
        //    {
        //        awaitDelegate de7 = new awaitDelegate(waitForTheNextActionSortFinish7);
        //        return de7();
        //    }
        //    if (solandequi == 8)
        //    {
        //        awaitDelegate de8 = new awaitDelegate(waitForTheNextActionSortFinish8);
        //        return de8();
        //    }
        //    awaitDelegate de9 = new awaitDelegate(waitForTheNextActionSortFinish9);
        //    return de9();

        //}
        //#endregion

        //#region Đổi màu vị trí các ô vị trí từ left -> right
        //private void changeColorPos(int left, int right)
        //{
        //    labelx.Visible = false;
        //    labeli.Visible = false;
        //    labelj.Visible = false;
        //    labelibangj.Visible = false;
        //    for (int i = 0; i < DaySoNguyen.Length; i++)            //Thêm vào form các hình vuông cần dùng
        //    {
        //        posSquare[i].ForeColor = System.Drawing.SystemColors.ButtonHighlight;
        //        posSquare[i].Refresh();
        //    }
        //    for (int i = left; i <= right; i++)            //Thêm vào form các hình vuông cần dùng
        //    {
        //        posSquare[i].AutoSize = true;
        //        posSquare[i].TabIndex = 0;
        //        posSquare[i].ForeColor = Color.Aqua;
        //        posSquare[i].Refresh();
        //    }
        //}
        //#endregion
        //private void Quick_sort()
        //{
        //    // khởi tạo x

        //    labelx.AutoSize = true;
        //    labelx.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
        //    labelx.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
        //    labelx.Top = posSquare[0].Top - 25;
        //    labelx.Left = posSquare[0].Left + 5;
        //    labelx.Name = "labelx";
        //    labelx.Size = new System.Drawing.Size(16, 22);
        //    labelx.TabIndex = 0;
        //    labelx.Text = "x";
        //    labelx.Visible = false;
        //    panelHienThiOVuong.Controls.Add(labelx);

        //    //khởi tạo label i=j
        //    labelibangj.AutoSize = true;
        //    labelibangj.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
        //    labelibangj.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
        //    labelibangj.Name = "labelibangj";
        //    labelibangj.Size = new System.Drawing.Size(16, 22);
        //    labelibangj.TabIndex = 0;
        //    labelibangj.Text = "i = j";
        //    labelibangj.Visible = false;
        //    panelHienThiOVuong.Controls.Add(labelibangj);

        //    if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
        //    {
        //        continueFlag = new CancellationTokenSource();
        //    }
        //    else
        //    {
        //        continueAutoFlag = new CancellationTokenSource();  //nếu không phải thì đang sắp xếp bên auto
        //    }

        //    // chạy quicksort
        //    actionSort(DaySoNguyen, 0, DaySoNguyen.Length - 1, 0);

        //}
        //private async void actionSort(int[] a, int left, int right, int solandequi)
        //{
        //    changeColorPos(left, right);
        //    int i, j;
        //    int x;
        //    int posx = (left + right) / 2; // xác định vị trí phần tử giữa
        //    x = a[posx];        // chọn phần tử giữa làm gốc                   


        //    resetRichtextboxColor(lines, 23);
        //    richTextBoxCodeThuatToan.Refresh();
        //    HighlightRichTextBox(lines, 3, colorhighlight);
        //    richTextBoxCodeThuatToan.Refresh();
        //    showx(posx);
        //    labelx.Refresh();

        //    waitNSeconds(waitAmount);

        //    i = left;
        //    j = right;


        //    resetRichtextboxColor(lines, 23);
        //    richTextBoxCodeThuatToan.Refresh();
        //    HighlightRichTextBox(lines, 4, colorhighlight);
        //    richTextBoxCodeThuatToan.Refresh();
        //    showi_q(i);
        //    labeli.Refresh();
        //    showj_q(j);
        //    labelj.Refresh();
        //    waitNSeconds(waitAmount);
        //    do
        //    {
        //        resetRichtextboxColor(lines, 23);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 7, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();
        //        waitNSeconds(waitAmount);

        //        while (a[i] < x)
        //        {
        //            i++;    // lặp đến khi a[i] >= x
        //            if (i == j)
        //            {
        //                resetRichtextboxColor(lines, 23);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 8, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                showibangj(i);
        //                labelibangj.Refresh();
        //                waitNSeconds(waitAmount);
        //            }
        //            else
        //            {
        //                resetRichtextboxColor(lines, 23);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 8, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                showi_q(i);
        //                labeli.Refresh();
        //                waitNSeconds(waitAmount);
        //            }

        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {
        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }
        //        resetRichtextboxColor(lines, 23);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 9, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();
        //        waitNSeconds(waitAmount);

        //        while (a[j] > x)
        //        {
        //            j--;    // lặp đến khi a[i] <= x
        //            if (j == i)
        //            {
        //                resetRichtextboxColor(lines, 23);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 8, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                showibangj(j);
        //                labelibangj.Refresh();
        //                waitNSeconds(waitAmount);
        //            }
        //            else
        //            {
        //                resetRichtextboxColor(lines, 23);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 10, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                showj_q(j);
        //                labelj.Refresh();
        //                waitNSeconds(waitAmount);
        //            }

        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {
        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }
        //        resetRichtextboxColor(lines, 23);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 11, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();
        //        waitNSeconds(waitAmount);

        //        if (i <= j)        // nếu có 2 phần tử a[i] và a[j] ko theo thứ tự
        //        {
        //            resetRichtextboxColor(lines, 23);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 13, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            waitNSeconds(waitAmount);

        //            if (i == j)
        //            {

        //                i++;        // qua phần tử kế tiếp
        //                j--;        // qua phần tử đứng trước

        //                resetRichtextboxColor(lines, 23);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 14, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                showi_q(i);
        //                labeli.Refresh();
        //                waitNSeconds(waitAmount);

        //                if (i == j)
        //                {
        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 15, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    showibangj(i);
        //                    labelibangj.Refresh();
        //                    waitNSeconds(waitAmount);
        //                }
        //                else
        //                {
        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 15, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    showj_q(j);
        //                    labelj.Refresh();
        //                    waitNSeconds(waitAmount);
        //                }

        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {
        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }
        //            else
        //            {
        //                while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                {
        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 13, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    doiCho2OVuong(i, j);

        //                    if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                    {
        //                        try
        //                        {
        //                            await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                    }
        //                }
        //                finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                int temp = a[i];
        //                a[i] = a[j];
        //                a[j] = temp;
        //                i++;        // qua phần tử kế tiếp
        //                j--;        // qua phần tử đứng trước

        //                resetRichtextboxColor(lines, 23);
        //                richTextBoxCodeThuatToan.Refresh();
        //                HighlightRichTextBox(lines, 14, colorhighlight);
        //                richTextBoxCodeThuatToan.Refresh();
        //                showi_q(i);
        //                labeli.Refresh();
        //                waitNSeconds(waitAmount);

        //                if (i == j)
        //                {
        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 15, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    showibangj(i);
        //                    labelibangj.Refresh();
        //                    waitNSeconds(waitAmount);
        //                }
        //                else
        //                {
        //                    resetRichtextboxColor(lines, 23);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 15, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    showj_q(j);
        //                    labelj.Refresh();
        //                    waitNSeconds(waitAmount);
        //                }

        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {
        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }
        //            waitNSeconds(waitAmount);
        //        }
        //    } while (i <= j);
        //    resetRichtextboxColor(lines, 23);
        //    richTextBoxCodeThuatToan.Refresh();
        //    HighlightRichTextBox(lines, 17, colorhighlight);
        //    richTextBoxCodeThuatToan.Refresh();
        //    waitNSeconds(waitAmount);

        //    resetRichtextboxColor(lines, 23);
        //    richTextBoxCodeThuatToan.Refresh();
        //    HighlightRichTextBox(lines, 19, colorhighlight);
        //    richTextBoxCodeThuatToan.Refresh();
        //    waitNSeconds(waitAmount);
        //    if (left < j)    // phân hoạch đoạn bên trái
        //    {
        //        resetRichtextboxColor(lines, 23);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 20, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();
        //        waitNSeconds(waitAmount);
        //        actionSort(a, left, j, solandequi + 1);
        //        try
        //        {
        //            await returnAwaitCorresponding(solandequi + 1);
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    resetRichtextboxColor(lines, 23);
        //    richTextBoxCodeThuatToan.Refresh();
        //    HighlightRichTextBox(lines, 21, colorhighlight);
        //    richTextBoxCodeThuatToan.Refresh();
        //    waitNSeconds(waitAmount);
        //    if (right > i)    // phân hoạch đoạn bên phải
        //    {
        //        resetRichtextboxColor(lines, 23);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 22, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();
        //        waitNSeconds(waitAmount);
        //        actionSort(a, i, right, solandequi + 1);
        //        try
        //        {
        //            await returnAwaitCorresponding(solandequi + 1);
        //        }
        //        catch
        //        {

        //        }
        //    }
        //    if (solandequi == 0)
        //    {
        //        m = 0;
        //        n = 0;
        //        enableControlsAfterFinish();
        //        ButtonLamLai.Enabled = true;
        //        buttonBatDau.Enabled = false;
        //        KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
        //        ktb.ShowDialog();
        //    }
        //    returnAwaitCorrespondingObject(solandequi).Cancel();
        //}
        //#endregion

        //#region Thuật toán Shell Sort
        //private async void Shell_sort()
        //{
        //    if (isStepbyStepStarted) //nếu đã bắt đầu sắp xếp bên step by step thì new token lượt ngưng
        //    {
        //        continueFlag = new CancellationTokenSource();
        //    }
        //    else
        //    {
        //        continueAutoFlag = new CancellationTokenSource();  //nếu không phải thì đang sắp xếp bên auto
        //    }

        //    resetRichtextboxColor(lines, 3);
        //    richTextBoxCodeThuatToan.Refresh();
        //    HighlightRichTextBox(lines, 2, colorhighlight);
        //    richTextBoxCodeThuatToan.Refresh();
        //    waitNSeconds(waitAmount);

        //    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //    {
        //        try
        //        {
        //            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //        }
        //        catch
        //        {

        //        }
        //        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //        {
        //            buttonLamLaiClicked = false;
        //            return;
        //        }
        //        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //    }
        //    else
        //    {
        //        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //        waitNSeconds(waitAmount);
        //        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //        {
        //            try
        //            {
        //                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //        }
        //    }

        //    int[] gap = { 5, 3, 2, 1 }; // mảng bước nhảy
        //    for (int igap = 0; igap < 4; igap++) // bước nhảy bắt đầu từ 5, giảm dần theo dãy fibonacci
        //    {
        //        if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //        {
        //            try
        //            {
        //                await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //            }
        //            catch
        //            {

        //            }
        //            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //            {
        //                buttonLamLaiClicked = false;
        //                return;
        //            }
        //            continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //        }
        //        else
        //        {
        //            //nếu ko phải đang chạy step by step thì là đang chạy auto
        //            waitNSeconds(waitAmount);
        //            if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //            {
        //                try
        //                {
        //                    await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //            }
        //        }

        //        resetRichtextboxColor(lines, 15);
        //        richTextBoxCodeThuatToan.Refresh();
        //        HighlightRichTextBox(lines, 3, colorhighlight);
        //        richTextBoxCodeThuatToan.Refresh();
        //        waitNSeconds(waitAmount);

        //        for (int i = igap; i < DaySoNguyen.Length; i++) // bắt đầu phần tử đầu tiên của bước nhảy gap[igap]
        //        {
        //            if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //            {
        //                try
        //                {
        //                    await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //            }
        //            else
        //            {
        //                //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                waitNSeconds(waitAmount);
        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }

        //            showi(i);
        //            labeli.Refresh();

        //            resetRichtextboxColor(lines, 13);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 5, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            waitNSeconds(waitAmount);

        //            resetRichtextboxColor(lines, 8);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 7, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            waitNSeconds(waitAmount);

        //            int temp = DaySoNguyen[i]; //phần tử đang xét

        //            resetRichtextboxColor(lines, 9);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 8, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            waitNSeconds(waitAmount);

        //            int j;
        //            if (RadioButtonTangDan.Checked)
        //            {
        //                for (j = i; j >= gap[igap] && DaySoNguyen[j - gap[igap]] > temp; j -= gap[igap]) // insert phần tử đang xét vào trước các phần tử trước nó theo bước nhảy gap[igap]
        //                {
        //                    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                    {
        //                        try
        //                        {
        //                            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                    }
        //                    else
        //                    {
        //                        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                        waitNSeconds(waitAmount);
        //                        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }

        //                    resetRichtextboxColor(lines, 11);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 9, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    showi(j);
        //                    labeli.Refresh();

        //                    HinhVuongSo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
        //                    HinhVuongSo[j].Refresh();

        //                    showj(j - gap[igap]);
        //                    labelj.Refresh();

        //                    HinhVuongSo[j - gap[igap]].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //                    HinhVuongSo[j - gap[igap]].Refresh();

        //                    resetRichtextboxColor(lines, 11);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 10, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                    {
        //                        try
        //                        {
        //                            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                    }
        //                    else
        //                    {
        //                        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                        waitNSeconds(waitAmount);
        //                        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }

        //                    while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                    {
        //                        doiCho2OVuong(j - gap[igap], j);

        //                        if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }
        //                    finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                    HinhVuongSo[j - gap[igap]].BackColor = System.Drawing.Color.Gray;
        //                    HinhVuongSo[j].BackColor = System.Drawing.Color.Gray;
        //                    HinhVuongSo[j].Refresh();
        //                    HinhVuongSo[j - gap[igap]].Refresh();

        //                    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                    {
        //                        try
        //                        {
        //                            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                    }
        //                    else
        //                    {
        //                        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                        waitNSeconds(waitAmount);
        //                        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }

        //                    DaySoNguyen[j] = DaySoNguyen[j - gap[igap]];

        //                    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                    {
        //                        try
        //                        {
        //                            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                    }
        //                    else
        //                    {
        //                        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                        waitNSeconds(waitAmount);
        //                        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                for (j = i; j >= gap[igap] && DaySoNguyen[j - gap[igap]] < temp; j -= gap[igap])
        //                {
        //                    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                    {
        //                        try
        //                        {
        //                            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                    }
        //                    else
        //                    {
        //                        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                        waitNSeconds(waitAmount);
        //                        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }

        //                    resetRichtextboxColor(lines, 11);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 9, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    showi(j);
        //                    labeli.Refresh();

        //                    HinhVuongSo[j].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
        //                    HinhVuongSo[j].Refresh();

        //                    showj(j - gap[igap]);
        //                    labelj.Refresh();

        //                    HinhVuongSo[j - gap[igap]].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
        //                    HinhVuongSo[j - gap[igap]].Refresh();

        //                    resetRichtextboxColor(lines, 11);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    HighlightRichTextBox(lines, 10, colorhighlight);
        //                    richTextBoxCodeThuatToan.Refresh();
        //                    waitNSeconds(waitAmount);

        //                    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                    {
        //                        try
        //                        {
        //                            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                    }
        //                    else
        //                    {
        //                        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                        waitNSeconds(waitAmount);
        //                        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }

        //                    while (!finishedSwapping) //nếu chưa hoàn thành đổi chỗ thì vẫn lặp
        //                    {
        //                        doiCho2OVuong(j - gap[igap], j);

        //                        if (buttonNgungClicked)  //nếu đang đổi chổ 2 ô trong auto mà ngưng thì vào if
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }
        //                    finishedSwapping = false;   //thoát ra đc vòng lặp while r thì nghĩa là đã hoàn thành đổi chỗ nên set lại về false

        //                    HinhVuongSo[j - gap[igap]].BackColor = System.Drawing.Color.Gray;
        //                    HinhVuongSo[j].BackColor = System.Drawing.Color.Gray;
        //                    HinhVuongSo[j].Refresh();
        //                    HinhVuongSo[j - gap[igap]].Refresh();

        //                    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                    {
        //                        try
        //                        {
        //                            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                    }
        //                    else
        //                    {
        //                        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                        waitNSeconds(waitAmount);
        //                        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }

        //                    DaySoNguyen[j] = DaySoNguyen[j - gap[igap]];

        //                    if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //                    {
        //                        try
        //                        {
        //                            await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                        }
        //                        catch
        //                        {

        //                        }
        //                        if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                        {
        //                            buttonLamLaiClicked = false;
        //                            return;
        //                        }
        //                        continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //                    }
        //                    else
        //                    {
        //                        //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                        waitNSeconds(waitAmount);
        //                        if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                        {
        //                            try
        //                            {
        //                                await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                            }
        //                            catch
        //                            {

        //                            }
        //                            if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                            {
        //                                buttonLamLaiClicked = false;
        //                                return;
        //                            }
        //                            continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                        }
        //                    }
        //                }
        //            }

        //            resetRichtextboxColor(lines, 12);
        //            richTextBoxCodeThuatToan.Refresh();
        //            HighlightRichTextBox(lines, 11, colorhighlight);
        //            richTextBoxCodeThuatToan.Refresh();
        //            waitNSeconds(waitAmount);

        //            if (isStepbyStepStarted)    //nếu đang làm step by step thì đợi nút tiến
        //            {
        //                try
        //                {
        //                    await waitForbuttonTienClick();     //dừng để đợi người dùng ấn nút Tiến để tiếp tục
        //                }
        //                catch
        //                {

        //                }
        //                if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                {
        //                    buttonLamLaiClicked = false;
        //                    return;
        //                }
        //                continueFlag = new CancellationTokenSource();   //tạo lại token (lượt dừng) mới cho lần dừng tiếp theo
        //            }
        //            else
        //            {
        //                //nếu ko phải đang chạy step by step thì là đang chạy auto
        //                waitNSeconds(waitAmount);
        //                if (buttonNgungClicked)    //nếu đang đợi n giây mà nút ngưng đc ấn thì gọi hàm đợi nút tiếp tục
        //                {
        //                    try
        //                    {
        //                        await waitForTiepTucClick();                        //chờ nút tiếp tục được bấm
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    if (buttonLamLaiClicked)                //nếu button làm lại đc ấn thì thoát hẳn thuật toán
        //                    {
        //                        buttonLamLaiClicked = false;
        //                        return;
        //                    }
        //                    continueAutoFlag = new CancellationTokenSource();   //nếu nút ngưng đc bấm và sau đó nút tiếp tục đc ấn thì phải tạo lại token
        //                }
        //            }

        //            DaySoNguyen[j] = temp;
        //        }

        //        if (igap == 3)
        //        {
        //            for (int x = 0; x <
        //                DaySoNguyen.Length; x++)
        //                HinhVuongSo[x].BackColor = System.Drawing.Color.FromArgb(58, 130, 90);
        //            resetRichtextboxColor(lines, 15);
        //            richTextBoxCodeThuatToan.Refresh();
        //            enableControlsAfterFinish();
        //            KhungThongBao ktb = new KhungThongBao("Thông báo", "Đã sắp xếp xong", true, false);
        //            ktb.ShowDialog();
        //            break;
        //        }
        //    }
        //}
        //#endregion

        //#region Thuật toán merge sort
        //// Merges two subarrays of arr[].
        //// First subarray is arr[l..m]
        //// Second subarray is arr[m+1..r]
        //private void merge(int[] arr, int l, int m, int r)
        //{
        //    int i, j, k;
        //    int n1 = m - l + 1;
        //    int n2 = r - m;

        //    /* create temp arrays */
        //    int[] L = new int[n1];
        //    int[] R = new int[n2];

        //    /* Copy data to temp arrays L[] and R[] */
        //    for (i = 0; i < n1; i++)
        //        L[i] = arr[l + i];
        //    for (j = 0; j < n2; j++)
        //        R[j] = arr[m + 1 + j];

        //    /* Merge the temp arrays back into arr[l..r]*/
        //    i = 0; // Initial index of first subarray
        //    j = 0; // Initial index of second subarray
        //    k = l; // Initial index of merged subarray
        //    while (i < n1 && j < n2)
        //    {
        //        if (L[i] <= R[j])
        //        {
        //            arr[k] = L[i];
        //            i++;
        //        }
        //        else
        //        {
        //            arr[k] = R[j];
        //            j++;
        //        }
        //        k++;
        //    }

        //    /* Copy the remaining elements of L[], if there
        //       are any */
        //    while (i < n1)
        //    {
        //        arr[k] = L[i];
        //        i++;
        //        k++;
        //    }

        //    /* Copy the remaining elements of R[], if there
        //       are any */
        //    while (j < n2)
        //    {
        //        arr[k] = R[j];
        //        j++;
        //        k++;
        //    }
        //}

        ///* l is for left index and r is right index of the
        //   sub-array of arr to be sorted */
        //private void mergeSort(int[] arr, int l, int r)
        //{
        //    if (l < r)
        //    {
        //        // Same as (l+r)/2, but avoids overflow for
        //        // large l and h
        //        int m = l + (r - l) / 2;

        //        // Sort first and second halves
        //        mergeSort(arr, l, m);
        //        mergeSort(arr, m + 1, r);

        //        merge(arr, l, m, r);
        //    }
        //}
        //#endregion
        #endregion

        #region Các hàm liên quan đến sắp xếp dãy số, ô vuông

        //mới
        #region Đổi chổ 2 ô vuông
        static bool exitFlag1 = false;            //exitFlag1 dùng cho hàm timer1_Tick, là true nếu ô vuông m đã vào đúng vị trí
        static bool exitFlag2 = false;            //exitFlag2 dùng cho hàm timer1_Tick, là true nếu ô vuông n đã vào đúng vị trí
        O_vuong HinhVuongtemp;                    //biến temp để hoán vị thông tin của 2 label ô vuông
        int OVuong1 = 0;                          //biến dùng để lưu vị trí của ô vuông thứ nhất cần di chuyển trong hàm doiCho2OVuong
        int OVuong2 = 0;                          //biến dùng để lưu vị trí của ô vuông thứ hai cần di chuyển trong hàm doiCho2OVuong
        bool finishedSwapping;                    //biến dùng để báo hiệu đã hoàn thành đổi chổ 2 ô vuông, true = hoàn thành, false = chưa
        private void doiCho2OVuong(int i, int j)
        {
            //mới        //báo hiệu chưa hoàn thành đổi chỗ 2 ô vuông
            finishedSwapping = false;

            //set lại 2 cờ exitFlag để vào vòng lặp doevents
            exitFlag1 = false;
            exitFlag2 = false;

            //gán 2 biến OVuong1 và OVuong2 bằng với tham số i, j để đổi chổ
            OVuong1 = i;
            OVuong2 = j;

            timer1.Start();                                  //Bắt đầu timer cho ô vuông thứ m, hàm timer1_Tick xử lý việc di chuyển ô vuông m
            timer2.Start();                                  //Bắt đầu timer cho ô vuông thứ m, hàm timer2_Tick xử lý việc di chuyển ô vuông n
            while (exitFlag1 == false && exitFlag2 == false)  //Vòng lập này chạy cho tới khi 2 ô vuông đã vào đúng vị trí
            {
                Application.DoEvents();                      //Hai hàm timer1_Tick và timer2_Tick sẽ được chạy
            }
            if (HinhVuongSo[OVuong1].status1 && HinhVuongSo[OVuong1].status2 && HinhVuongSo[OVuong1].status3 && HinhVuongSo[OVuong2].status1 && HinhVuongSo[OVuong2].status2 && HinhVuongSo[OVuong2].status3)
            {
                HinhVuongtemp = HinhVuongSo[OVuong1];                  //Hoán vị 2 hình vuông
                HinhVuongSo[OVuong1] = HinhVuongSo[OVuong2];           //Hoán vị 2 hình vuông 
                HinhVuongSo[OVuong2] = HinhVuongtemp;                  //Hoán vị 2 hình vuông
                HinhVuongSo[OVuong1].BackColor = System.Drawing.Color.FromArgb(66, 104, 166);
                HinhVuongSo[OVuong2].BackColor = System.Drawing.Color.FromArgb(178, 75, 83);
                HinhVuongSo[OVuong1].Refresh();
                HinhVuongSo[OVuong2].Refresh();
            }
            if (HinhVuongSo[OVuong1].status1 && HinhVuongSo[OVuong1].status2 && HinhVuongSo[OVuong1].status3)
            {
                HinhVuongSo[OVuong1].thietLaplaiStatus();          //Đặt lại các status cho ô vuông m
            }
            if (HinhVuongSo[OVuong2].status1 && HinhVuongSo[OVuong2].status2 && HinhVuongSo[OVuong2].status3)
            {
                HinhVuongSo[OVuong2].thietLaplaiStatus();          //Đặt lại các status cho ô vuông n
            }
            exitFlag1 = false;                               //Đặt lại exitFlag1 sau khi thực hiện xong 1 lần đổ chỗ 2 ô vuông
            exitFlag2 = false;                               //Đặt lại exitFlag2 sau khi thực hiện xong 1 lần đổ chỗ 2 ô vuông          
        }
        #endregion

        #region Sự kiện timer chạy cho ô vuông thứ 1
        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    if (!HinhVuongSo[OVuong1].status1)                                    //Kiểm tra xem ô vuông m đã đi lên hay chưa
        //    {
        //        if (HinhVuongSo[OVuong1].Top > (HinhVuongSo[OVuong1].topBanDau - 60))
        //        {
        //            HinhVuongSo[OVuong1].diLen();
        //        }
        //        else
        //        {
        //            HinhVuongSo[OVuong1].status1 = true;                          //Ô vuông m đi lên rồi thì xử lý tiếp status2
        //        }
        //    }
        //    if (HinhVuongSo[OVuong1].status1 && (HinhVuongSo[OVuong1].Left < HinhVuongSo[OVuong2].leftBanDau))    //Hình vuông m đã lên rồi thì kiểm tra xem ô vuông m đã đi qua phải chưa
        //    {
        //        HinhVuongSo[OVuong1].quaPhai();
        //    }
        //    else if (HinhVuongSo[OVuong1].status1)
        //    {
        //        HinhVuongSo[OVuong1].status2 = true;                                                  //Hình vuông m đã đi lên và qua phải rồi thì xử lý status 3
        //    }
        //    if (HinhVuongSo[OVuong1].status1 && HinhVuongSo[OVuong1].status2 && (HinhVuongSo[OVuong1].Top < HinhVuongSo[OVuong2].topBanDau))  //Hình vuông m đã đi lên, qua phải rồi thì kiểm tra xem ô vuông m đã đi qua xuống chưa
        //    {
        //        HinhVuongSo[OVuong1].diXuong();
        //    }
        //    else if (HinhVuongSo[OVuong1].status1 && HinhVuongSo[OVuong1].status2)                            //Hình vuông m đã đi lên, qua phải, đi xuống rồi thì xử lý tiếp
        //    {
        //        HinhVuongSo[OVuong1].status3 = true;
        //    }
        //    if (HinhVuongSo[OVuong1].status1 && HinhVuongSo[OVuong1].status2 && HinhVuongSo[OVuong1].status3)       //Hình vuông m đã thực hiện đủ 3 bước đi thì ngưng timer và đặt lại exitFlag1 để ngừng vòng lập ở doiCho2OVuong
        //    {
        //        timer1.Stop();
        //        exitFlag1 = true;
        //        finishedSwapping = true;        //báo hiệu đã hoàn thành đổ chổ 2 ô vuông
        //    }
        //}
        #endregion

        #region Sự kiện timer chạy cho ô vuông thứ 2
        //private void timer2_Tick(object sender, EventArgs e)
        //{
        //    if (!HinhVuongSo[OVuong2].status1)     //Kiểm tra xem ô vuông n đã đi xuống hay chưa
        //    {
        //        if (HinhVuongSo[OVuong2].Top < (HinhVuongSo[OVuong2].topBanDau + 60))
        //        {
        //            HinhVuongSo[OVuong2].diXuong();
        //        }
        //        else
        //        {
        //            HinhVuongSo[OVuong2].status1 = true;       //Ô vuông n đi xuống rồi thì xử lý tiếp status2
        //        }
        //    }
        //    if (HinhVuongSo[OVuong2].status1 && (HinhVuongSo[OVuong2].Left > HinhVuongSo[OVuong1].leftBanDau))      //Hình vuông n đã xuống rồi thì kiểm tra xem ô vuông m đã đi qua trái chưa
        //    {
        //        HinhVuongSo[OVuong2].quaTrai();
        //    }
        //    else if (HinhVuongSo[OVuong2].status1)
        //    {
        //        HinhVuongSo[OVuong2].status2 = true;          //Hình vuông n đã đi xuống và qua trái rồi thì xử lý status 3
        //    }
        //    if (HinhVuongSo[OVuong2].status2 && (HinhVuongSo[OVuong2].Top > HinhVuongSo[OVuong1].topBanDau))
        //    {
        //        HinhVuongSo[OVuong2].diLen();
        //    }
        //    else if (HinhVuongSo[OVuong2].status1 && HinhVuongSo[OVuong2].status2)   //Hình vuông n đã đi xuống, qua trái, đi lên rồi thì xử lý tiếp
        //    {
        //        HinhVuongSo[OVuong2].status3 = true;
        //    }
        //    if (HinhVuongSo[OVuong2].status1 && HinhVuongSo[OVuong2].status2 && HinhVuongSo[OVuong2].status3)     //Hình vuông n đã thực hiện đủ 3 bước đi thì ngưng timer và đặt lại exitFlag2 để ngừng vòng lập ở doiCho2OVuong
        //    {
        //        timer2.Stop();
        //        exitFlag2 = true;
        //        finishedSwapping = true;    //báo hiệu đã hoàn thành đổ chổ 2 ô vuông
        //    }
        //}
        #endregion
        #endregion

        #region Khởi tạo i và j
        private void khoiTaoiVaj()
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
            this.Controls.Add(labeli);
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
            this.Controls.Add(labelj);
        }
        #endregion

        #region Hiển thị i, j
        private void showi(int pos)
        {
            labeli.Visible = true;
            labeli.Top = posSquare[pos].Top - 25;
            labeli.Left = posSquare[pos].Left + 5;
        }



        private void showj(int pos)
        {
            labelj.Visible = true;
            labelj.Top = posSquare[pos].Top - 25;
            labelj.Left = posSquare[pos].Left + 5;
        }

        #endregion

        private void FormThuatToanB_Load(object sender, EventArgs e)
        {
            HinhVuongSo = new O_vuong[10];
            posSquare = new Label[10];

            for (int i = 0; i < 10; i++)
            {
                HinhVuongSo[i] = new O_vuong();
                posSquare[i] = new Label();
            }
        }

    }
}
