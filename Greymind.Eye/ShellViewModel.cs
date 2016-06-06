using Caliburn.Micro;
using System;
using System.Threading;
using System.Windows;

namespace Greymind.Eye
{
    public class ShellViewModel : PropertyChangedBase, IShell, IViewAware
    {
        private const int SnoozeIntervalInMinutes = 5;
        private const string SnoozeState = "Snooze";

        private IShellView shellView;

        private Timer timer;

        private bool isEnabled;
        private string interval;
        private bool showInTaskbar;

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                NotifyOfPropertyChange(nameof(IsEnabled));

                if (IsEnabled) StartTimerWithUserInterval();
                else StopTimer();
            }
        }

        public string Interval
        {
            get { return this.interval; }
            set
            {
                this.interval = value;
                NotifyOfPropertyChange(nameof(Interval));
            }
        }

        public bool ShowInTaskbar
        {
            get
            {
                return this.showInTaskbar;
            }
            set
            {
                this.showInTaskbar = value;
                NotifyOfPropertyChange(nameof(ShowInTaskbar));
            }
        }

        public ShellViewModel()
        {
            Interval = "20";
            ShowInTaskbar = false;
        }

        private void StartTimerWithUserInterval()
        {
            int intervalInMinutes;
            if (int.TryParse(Interval, out intervalInMinutes))
            {
                StartTimer(intervalInMinutes, string.Empty);
            }
        }

        private void StartTimer(int intervalInMinutes, string state)
        {
            this.timer = new Timer(TimerElapsed, state, TimeSpan.FromMinutes(intervalInMinutes), TimeSpan.Zero);
        }

        private void StopTimer()
        {
            this.timer.Dispose();
        }

        private void TimerElapsed(object state)
        {
            var isSnooze = (string)state == SnoozeState;

            StopTimer();

            this.shellView.BringFocusToWindow();

            var result = MessageBox.Show
                (
                    messageBoxText: isSnooze ? "Snooze time is up!" : "Time to take a break!",
                    caption: "Greymind Eye",
                    button: MessageBoxButton.OKCancel,
                    icon: MessageBoxImage.Information,
                    defaultResult: MessageBoxResult.Cancel
                );

            switch (result)
            {
                case MessageBoxResult.OK:
                    StartTimerWithUserInterval();
                    break;

                case MessageBoxResult.Cancel:
                    StartSnoozeTimer();
                    break;
            }
        }

        private void StartSnoozeTimer()
        {
            StartTimer(SnoozeIntervalInMinutes, SnoozeState);
        }

        public void AttachView(object view, object context = null)
        {
            this.shellView = (IShellView)view;
        }

        public object GetView(object context = null)
        {
            return this.shellView;
        }

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;
    }
}