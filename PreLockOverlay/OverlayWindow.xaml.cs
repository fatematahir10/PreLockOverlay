using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PreLockOverlay
{
    public partial class OverlayWindow : Window
    {
        private int countdown = 10;
        private DispatcherTimer timer;

        public event Action CountdownFinished;

        public OverlayWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            countdown--;
            CountdownText.Text = countdown.ToString();

            if (countdown <= 0)
            {
                timer.Stop();
                CountdownFinished?.Invoke();
            }
        }

        public void CancelCountdown()
        {
            timer.Stop();
             this.Close();
        }
        private void OverlayWindow_KeyDown(object sender, KeyEventArgs e)
        {
            CancelCountdown();
        }

        private void OverlayWindow_MouseMove(object sender, MouseEventArgs e)
        {
            CancelCountdown();
        }
    }

}
