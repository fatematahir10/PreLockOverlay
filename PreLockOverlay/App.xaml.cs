using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using Gma.System.MouseKeyHook;

namespace PreLockOverlay
{
    public partial class App : Application
    {
        private System.Timers.Timer idleCheckTimer;
        private int idleThresholdSeconds = 10;
        private List<OverlayWindow> overlayWindows = new List<OverlayWindow>();

        private IKeyboardMouseEvents globalHook;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            globalHook = Hook.GlobalEvents();
            globalHook.KeyDown += OnKeyDown;
            globalHook.MouseMove += OnMouseMove;

            idleCheckTimer = new System.Timers.Timer(1000);
            idleCheckTimer.Elapsed += IdleCheckTimer_Elapsed;
            idleCheckTimer.Start();
        }

        private void IdleCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var idleTime = IdleTimeHelper.GetIdleTime();
            if (idleTime.TotalSeconds >= idleThresholdSeconds && overlayWindows.Count == 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                    {
                        var overlay = new OverlayWindow();
                        overlay.Left = screen.Bounds.Left;
                        overlay.Top = screen.Bounds.Top;
                        overlay.Width = screen.Bounds.Width;
                        overlay.Height = screen.Bounds.Height;
                        overlay.CountdownFinished += () =>
                        {
                            overlay.Close();
                            LockWorkStation();
                        };
                        overlay.Show();
                        overlayWindows.Add(overlay);
                    }
                });
            }
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            CancelOverlays();
        }

        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            CancelOverlays();
        }

        private void CancelOverlays()
        {
            if (overlayWindows.Count > 0)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var w in overlayWindows)
                        w.CancelCountdown();
                    overlayWindows.Clear();
                });
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (globalHook != null)
            {
                globalHook.KeyDown -= OnKeyDown;
                globalHook.MouseMove -= OnMouseMove;
                globalHook.Dispose();
            }

            base.OnExit(e);
        }

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();
    }
}
