using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace MouseMover
{
    internal class MouseMoverVM : INotifyPropertyChanged
    {
        #region Singleton
        private MouseMoverVM()
        {
            ClickDelay = 10000;
            MouseMoverDelay = 10000;
            MouseMoverDisabledButton = false;
            MouseMoverEnabledButton = true;
            startMouseMover();
        }

        private static MouseMoverVM _instance;

        public static MouseMoverVM Instance
        {
            set { }
            get
            {
                if (_instance == null)
                {
                    _instance = new MouseMoverVM();
                }
                return _instance;
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyname = "")
        {
            if (!string.IsNullOrEmpty(propertyname))
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        #endregion INotifyPropertyChanged


        private const int OFFSET = 10;

        private int clickDelay;
        public int ClickDelay
        {
            get => clickDelay;
            set
            {
                if (clickDelay != value)
                {
                    clickDelay = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool mouseMoverEnabledButton;
        public bool MouseMoverEnabledButton
        {
            get => mouseMoverEnabledButton;
            set
            {
                if (mouseMoverEnabledButton != value)
                {
                    mouseMoverEnabledButton = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool mouseMoverDisabledButton;
        public bool MouseMoverDisabledButton
        {
            get => mouseMoverDisabledButton;
            set
            {
                if (mouseMoverDisabledButton != value)
                {
                    mouseMoverDisabledButton = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private CancellationTokenSource CTS_MouseMover { get; set; }
        private CancellationTokenSource CTS_AutoClicker { get; set; }
        public bool autoClickerEnabled { get; set; }
        public int MouseMoverDelay { get; set; }


        public void startMouseMover()
        {
            CTS_MouseMover = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(new WaitCallback(moveMouse), CTS_MouseMover.Token);
            MouseMoverEnabledButton = false;
            MouseMoverDisabledButton = true;
        }

        private void moveMouse(object obj)
        {
            Point oldPos = Mouse.GetCursorPosition();
            Point newPos;
            bool change = true;
            while (!((CancellationToken)obj).IsCancellationRequested)
            {
                newPos = Mouse.GetCursorPosition();
                if (newPos.X == oldPos.X && newPos.Y == oldPos.Y)
                {
                    if (change)
                    {
                        Mouse.SetCursorPosition((int)newPos.X + OFFSET, (int)newPos.Y);
                    }
                    else
                    {
                        Mouse.SetCursorPosition((int)newPos.X - OFFSET, (int)newPos.Y);
                    }
                    change = !change;
                }
                oldPos = newPos;
                System.Threading.Thread.Sleep(MouseMoverDelay);
            }
        }

        public void endMouseMover()
        {
            MouseMoverEnabledButton = true;
            MouseMoverDisabledButton = false;
            CTS_MouseMover.Cancel();
            Thread.Sleep(200);
            CTS_MouseMover.Dispose();
        }

        public void startAutoClicker()
        {
            CTS_AutoClicker = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(new WaitCallback(autoClicker), CTS_AutoClicker.Token);
        }

        public void endAutoClicker()
        {
            CTS_AutoClicker.Cancel();
            Thread.Sleep(500);
            CTS_AutoClicker.Dispose();
        }

        private void autoClicker(object obj)
        {
            Point pos = Mouse.GetCursorPosition();
            while (!((CancellationToken)obj).IsCancellationRequested)
            {
                Mouse.SetCursorPosition((int)pos.X, (int)pos.Y);
                Mouse.DoMouseClick();
                Mouse.SetCursorPosition((int)pos.X, (int)pos.Y);
                Thread.Sleep(ClickDelay);
            }
        }
    }
}
