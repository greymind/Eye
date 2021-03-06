using Caliburn.Micro;
using System;
using System.Threading;
using System.Windows;

namespace Greymind.Eye
{
    public class ShellViewModel : PropertyChangedBase, IShell, IViewAware
    {
        private enum State
        {
            Regular,
            Snooze
        }

        private IShellView shellView;

        private Timer timer;

        private bool isEnabled;
        private string interval;
        private string snoozeInterval;
        private bool showInTaskbar;

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                NotifyOfPropertyChange(nameof(IsEnabled));

                if (IsEnabled) StartTimer(Interval, State.Regular);
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

        public string SnoozeInterval
        {
            get { return this.snoozeInterval; }
            set
            {
                this.snoozeInterval = value;
                NotifyOfPropertyChange(nameof(SnoozeInterval));
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
            SnoozeInterval = "3";
            ShowInTaskbar = false;
        }

        private void StartTimer(string intervalInMinutesAsString, State state)
        {
            int intervalInMinutes;
            if (int.TryParse(intervalInMinutesAsString, out intervalInMinutes))
            {
                StartTimer(intervalInMinutes, state);
            }
        }

        private void StartTimer(int intervalInMinutes, State state)
        {
            this.timer = new Timer(TimerElapsed, state, TimeSpan.FromMinutes(intervalInMinutes), TimeSpan.Zero);
        }

        private void StopTimer()
        {
            this.timer.Dispose();
        }

        private void TimerElapsed(object state)
        {
            var isSnooze = (State)state == State.Snooze;

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
                    StartTimer(Interval, State.Regular);
                    break;

                case MessageBoxResult.Cancel:
                    StartTimer(SnoozeInterval, State.Snooze);
                    break;
            }
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